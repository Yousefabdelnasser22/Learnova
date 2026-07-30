using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Constant;
using Learnova.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.User.Command.UnassignUserRole
{
    public class UnassignUserRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserContext userContext) : IRequestHandler<UnassignUserRoleCommand>
    {
        public async Task Handle(UnassignUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.UserEmail);
            if (user is null)
            {
                throw new NotFoundException("User not found.");
            }

            var role =  await roleManager.FindByNameAsync(request.RoleName);
            if (role is null)
            {
                throw new NotFoundException("Role not found.");
            }

            if (!await userManager.IsInRoleAsync(user, role.Name!))
            {
                throw new ConflictException("User does not have this role.");
            }

            if (string.Equals(role.Name, UserRole.Admin, StringComparison.OrdinalIgnoreCase))
            {
                var currentUser = userContext.GetCurrentUser();
                if (currentUser?.Id == user.Id)
                {
                    throw new BadRequestException("You cannot remove your own Admin role.");
                }

                var admins = await userManager.GetUsersInRoleAsync(UserRole.Admin);
                if (admins.Count <= 1)
                {
                    throw new BadRequestException("At least one Admin user must remain.");
                }
            }

            var result = await userManager.RemoveFromRoleAsync(user, role.Name!);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                throw new BadRequestException(errors);
            }
        }
    }
}
