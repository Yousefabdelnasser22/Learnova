using Learnova.Application.Common.Queries;
using Learnova.Application.Enrollment.DTO;
using MediatR;

namespace Learnova.Application.Enrollment.Query.GetStudentEnrollments
{
    public class GetStudentEnrollmentsQuery : PagedSearchQuery, IRequest<IEnumerable<StudentEnrollmentDto>>
    {
    }
}
