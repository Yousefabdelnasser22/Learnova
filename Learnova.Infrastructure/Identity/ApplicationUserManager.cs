using Learnova.Domain.Constant;
using Learnova.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Learnova.Infrastructure.Identity
{
    public sealed class ApplicationUserManager(
        IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<ApplicationUser>> logger,
        RoleManager<IdentityRole> roleManager)
        : UserManager<ApplicationUser>(
            store,
            optionsAccessor,
            passwordHasher,
            userValidators,
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger)
    {
        public override async Task<IdentityResult> CreateAsync(
            ApplicationUser user,
            string password)
        {
            if (!await roleManager.RoleExistsAsync(UserRole.Student))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DefaultRoleNotFound",
                    Description = $"The default role '{UserRole.Student}' has not been initialized."
                });
            }

            var createResult = await base.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                return createResult;
            }

            var roleResult = await AddToRoleAsync(user, UserRole.Student);
            if (roleResult.Succeeded)
            {
                return createResult;
            }

            var deleteResult = await DeleteAsync(user);
            var errorsToReturn = deleteResult.Succeeded
                ? roleResult.Errors
                : roleResult.Errors.Concat(deleteResult.Errors);

            return IdentityResult.Failed(errorsToReturn.ToArray());
        }
    }
}
