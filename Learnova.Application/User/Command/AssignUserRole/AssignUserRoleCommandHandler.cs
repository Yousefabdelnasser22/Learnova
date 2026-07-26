using Learnova.Application.Exceptions;
using Learnova.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.User.Command.AssignUserRole
{
    public class AssignUserRoleCommandHandler(UserManager<ApplicationUser> userManager , RoleManager<IdentityRole> roleManager) : IRequestHandler<AssignUserRoleCommand>
    {
        public async Task Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
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

            if (await userManager.IsInRoleAsync(user, role.Name!))
            {
                throw new ConflictException("User already has this role.");
            }

            var result = await userManager.AddToRoleAsync(user, role.Name!);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                throw new BadRequestException(errors);
            }
           
        }
    }
}
