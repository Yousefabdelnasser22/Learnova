using FluentValidation;
using Learnova.Domain.Constant;

namespace Learnova.Application.User.Command.AssignUserRole
{
    public class AssignUserRoleCommandValidator : AbstractValidator<AssignUserRoleCommand>
    {
        private static readonly string[] AllowedRoles =
        [
            UserRole.Admin,
            UserRole.Student,
            UserRole.Instructor
        ];

        public AssignUserRoleCommandValidator()
        {
            RuleFor(x => x.UserEmail)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.RoleName)
                .NotEmpty()
                .Must(role => AllowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
                .WithMessage("RoleName must be Admin, Student, or Instructor.");
        }
    }
}
