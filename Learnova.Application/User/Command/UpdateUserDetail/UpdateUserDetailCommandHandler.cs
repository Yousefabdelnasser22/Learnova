using Learnova.Application.Exceptions;
using Learnova.Domain.Entites;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.User.Command.UpdateUserDetail
{
    public class UpdateUserDetailCommandHandler(IUserContext userContext , IUserStore<ApplicationUser> userStore) : IRequestHandler<UpdateUserDetailCommand>
    {
        public async Task Handle(UpdateUserDetailCommand request, CancellationToken cancellationToken)
        {
           var user = userContext.GetCurrentUser();

           var UserDb = await userStore.FindByIdAsync(user!.Id,cancellationToken);

            if (UserDb is null)
            {
                throw new NotFoundException("User not found.");
            }

            UserDb.Age = request.Age;
            UserDb.City = request.City;

            var result = await userStore.UpdateAsync(UserDb, cancellationToken);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                throw new BadRequestException(errors);
            }
        }
    }
}
