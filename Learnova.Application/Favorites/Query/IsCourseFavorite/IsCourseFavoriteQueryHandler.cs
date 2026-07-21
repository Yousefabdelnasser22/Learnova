using Learnova.Application.Favorites.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Entites;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Favorites.Query.IsCourseFavorite
{
    public class IsCourseFavoriteQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<IsCourseFavoriteQuery, bool>
    {
        public async Task<bool> Handle(IsCourseFavoriteQuery request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var favoriteSpec = new FavoriteByStudentAndCourseSpecification(user.Id, request.CourseId);
            var favorite = await unitOfWork
                .Repository<FavoriteList>()
                .GetEntityWithSpecAsync(favoriteSpec);

            return favorite?.Course is not null &&
                !favorite.Course.IsDeleted &&
                favorite.Course.Status == CourseStatus.Published;
        }
    }
}
