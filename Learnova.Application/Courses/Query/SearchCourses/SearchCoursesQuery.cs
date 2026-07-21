using Learnova.Application.Courses.DTO;
using MediatR;

namespace Learnova.Application.Courses.Query.SearchCourses
{
    public record SearchCoursesQuery(string SearchTerm, int Limit = 10) : IRequest<List<CourseDTO>>;
}
