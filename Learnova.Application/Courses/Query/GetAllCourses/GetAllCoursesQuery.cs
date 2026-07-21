using Learnova.Application.Common.Queries;
using Learnova.Application.Courses.DTO;
using Learnova.Domain.Enums;
using MediatR;

namespace Learnova.Application.Courses.Query.GetAllCourses
{
    public class GetAllCoursesQuery : PagedSearchQuery, IRequest<IEnumerable<CourseDTO>>
    {
        public int? CategoryId { get; init; }

        public int? SubCategoryId { get; init; }

        public decimal? MinPrice { get; init; }

        public decimal? MaxPrice { get; init; }

        public CourseLevel? Level { get; init; }

        public string? Sort { get; init; }
    }
}
