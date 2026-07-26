using Learnova.Application.Exceptions;
using Learnova.Application.Favorites.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Favorites.Commands.RemoveCourseFromFavorites
{
    public class RemoveCourseFromFavoritesCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<RemoveCourseFromFavoritesCommand>
    {
        public async Task Handle(RemoveCourseFromFavoritesCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var favoriteSpec = new FavoriteByStudentAndCourseSpecification(user.Id, request.CourseId);
            var favorite = await unitOfWork.Repository<FavoriteList>().GetEntityWithSpecAsync(favoriteSpec);

            if (favorite is null)
                throw new NotFoundException("Course not found in favorites.");

            unitOfWork.Repository<FavoriteList>().HardDelete(favorite);

            await unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}
