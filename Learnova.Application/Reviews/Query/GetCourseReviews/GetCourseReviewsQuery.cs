using Learnova.Application.Common.Queries;
using Learnova.Application.Reviews.DTO;
using MediatR;

namespace Learnova.Application.Reviews.Query.GetCourseReviews
{
    public class GetCourseReviewsQuery : PagedSearchQuery, IRequest<IEnumerable<CourseReviewDto>>
    {
        public int CourseId { get; set; }
    }
}
