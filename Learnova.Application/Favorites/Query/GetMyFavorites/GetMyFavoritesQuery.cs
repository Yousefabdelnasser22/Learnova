using Learnova.Application.Favorites.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Favorites.Query.GetMyFavorites
{
    public class GetMyFavoritesQuery : IRequest<IEnumerable<FavoriteCourseDto>>;
}
