using Learnova.Application.Courses.DTO;
using MediatR;

namespace Learnova.Application.Courses.Query.GetCourseForManagement
{
    public record GetCourseForManagementQuery(int Id) : IRequest<CourseDTO>;
}
