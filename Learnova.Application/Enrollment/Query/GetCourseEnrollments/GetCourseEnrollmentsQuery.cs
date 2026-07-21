using Learnova.Application.Common.Queries;
using Learnova.Application.Enrollment.DTO;
using MediatR;

namespace Learnova.Application.Enrollment.Query.GetCourseEnrollments
{
    public class GetCourseEnrollmentsQuery : PagedSearchQuery, IRequest<IEnumerable<CourseEnrollmentDto>>
    {
        public int CourseId { get; set; }
    }
}
