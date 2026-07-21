using FluentValidation;

namespace Learnova.Application.Courses.Query.GetCourseForManagement
{
    public class GetCourseForManagementQueryValidator : AbstractValidator<GetCourseForManagementQuery>
    {
        public GetCourseForManagementQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be more than 0");
        }
    }
}
