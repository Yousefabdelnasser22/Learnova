using Learnova.Domain.Constant;
using Learnova.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Infrastructure.Seeder
{
    public class Seeder(AppDbContext context) : Iseeder
    {
        public async Task seed()
        {
            if (await context.Database.CanConnectAsync())
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

                var existingRoleNames = await context.Roles
                    .Select(role => role.NormalizedName)
                    .ToListAsync();

                var missingRoles = GetRole()
                    .Where(role => !existingRoleNames.Contains(role.NormalizedName))
                    .ToList();

                if (missingRoles.Count != 0)
                {
                    context.Roles.AddRange(missingRoles);
                    await context.SaveChangesAsync();
                }
            }
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
    }
}
