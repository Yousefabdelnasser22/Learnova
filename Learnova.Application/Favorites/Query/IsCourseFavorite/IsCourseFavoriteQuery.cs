using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Favorites.Query.IsCourseFavorite
{
    public class IsCourseFavoriteQuery(int courseId) : IRequest<bool>
    {
        public int CourseId { get; set; } = courseId;
    }
}
