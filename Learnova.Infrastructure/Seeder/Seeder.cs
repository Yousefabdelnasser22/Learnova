using Learnova.Domain.Constant;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Configuration;
using Learnova.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Learnova.Infrastructure.Seeder
{
    public class Seeder(
        AppDbContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IOptions<AdminBootstrapSettings> adminBootstrapOptions,
        ILogger<Seeder> logger) : Iseeder
    {
        public async Task seed()
        {
            await context.Database.ExecuteSqlRawAsync(
                """
                IF OBJECT_ID(N'[dbo].[ModuleProgress]', N'U') IS NOT NULL
                   AND COL_LENGTH('dbo.ModuleProgress', 'IsDeleted') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[ModuleProgress]
                    ADD [IsDeleted] bit NOT NULL CONSTRAINT [DF_ModuleProgress_IsDeleted] DEFAULT(0);
                END
                """
            );

            foreach (var role in GetRole())
            {
                if (await roleManager.RoleExistsAsync(role.Name!))
                {
                    continue;
                }

                var roleResult = await roleManager.CreateAsync(role);
                EnsureSucceeded(roleResult, $"create the '{role.Name}' role");
            }

            await EnsureAdminExistsAsync();
        }

        public IEnumerable<IdentityRole> GetRole()
        {
            List<IdentityRole> roles =
                [ new (UserRole.Admin)
                {
                    NormalizedName = UserRole.Admin.ToUpper()
                },
                  new (UserRole.Student)
                  {
                    NormalizedName = UserRole.Student.ToUpper()
                  },

                  new (UserRole.Instructor)
                  {
                    NormalizedName = UserRole.Instructor.ToUpper()
                },
                ];

            return roles;
        }

        private async Task EnsureAdminExistsAsync()
        {
            var existingAdmins = await userManager.GetUsersInRoleAsync(UserRole.Admin);
            if (existingAdmins.Count != 0)
            {
                return;
            }

            var settings = adminBootstrapOptions.Value;
            if (string.IsNullOrWhiteSpace(settings.Email)
                || string.IsNullOrWhiteSpace(settings.Password))
            {
                throw new InvalidOperationException(
                    "No Admin user exists. Configure AdminBootstrap:Email and " +
                    "AdminBootstrap:Password through user secrets, environment variables, " +
                    "or a managed secret store, then restart the application.");
            }

            var email = settings.Email.Trim();
            if (await userManager.FindByEmailAsync(email) is not null)
            {
                throw new InvalidOperationException(
                    $"Cannot bootstrap the Admin user because '{email}' is already registered " +
                    $"without the '{UserRole.Admin}' role. Configure a different bootstrap email.");
            }

            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, settings.Password);
            EnsureSucceeded(createResult, "create the bootstrap Admin user");

            var roleResult = await userManager.AddToRoleAsync(admin, UserRole.Admin);
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(admin);
                EnsureSucceeded(roleResult, $"assign the '{UserRole.Admin}' role to the bootstrap user");
            }

            var removeStudentResult = await userManager.RemoveFromRoleAsync(admin, UserRole.Student);
            if (!removeStudentResult.Succeeded)
            {
                logger.LogWarning(
                    "The bootstrap Admin was created, but its temporary Student role could not be removed: {Errors}",
                    string.Join("; ", removeStudentResult.Errors.Select(error => error.Description)));
            }

            logger.LogInformation(
                "Created the bootstrap Admin account for {AdminEmail}. Remove the bootstrap password from configuration.",
                email);
        }

        private static void EnsureSucceeded(IdentityResult result, string operation)
        {
            if (result.Succeeded)
            {
                return;
            }

            var errors = string.Join("; ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Failed to {operation}: {errors}");
        }
    }
}
