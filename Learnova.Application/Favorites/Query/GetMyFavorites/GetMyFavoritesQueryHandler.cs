using Learnova.Application.Favorites.DTO;
using Learnova.Application.Favorites.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Entites;
using Learnova.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Favorites.Query.GetMyFavorites
{
    public class GetMyFavoritesQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<GetMyFavoritesQuery, IEnumerable<FavoriteCourseDto>>
    {
        public async Task<IEnumerable<FavoriteCourseDto>> Handle(GetMyFavoritesQuery request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var favoriteSpec = new FavoritesByStudentIdSpecification(user.Id);
            var favorites = await unitOfWork.Repository<FavoriteList>().GetAllWithSpecAsync(favoriteSpec);

            return favorites.Select(x => new FavoriteCourseDto
            {
                FavoriteId = x.Id,
                CourseId = x.CourseId,
                CourseTitle = x.Course.Title,
                Thumbnail = x.Course.Thumbnail,
                Price = x.Course.Price,
                Currency = x.Course.Currency,
                Level = x.Course.Level,
                AddedAt = x.AddedAt
            }).ToList();
        }
    }
}
