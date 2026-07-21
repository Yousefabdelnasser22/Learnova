using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Favorites.Commands.RemoveCourseFromFavorites
{
    public class RemoveCourseFromFavoritesCommand(int courseId) : IRequest
    {
        public int CourseId { get; set; } = courseId;
    }
}
