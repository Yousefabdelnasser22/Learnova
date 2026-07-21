using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;

namespace Learnova.Application.Enrollment.Command.UnenrollStudent
{
    public class UnenrollStudentCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<UnenrollStudentCommand>
    {
        public async Task Handle(UnenrollStudentCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedException("User is not authenticated.");

            var enrollment = await unitOfWork.enrollment
                .GetByStudentAndCourseAsync(user.Id, request.CourseId);

            if (enrollment is null)
                throw new NotFoundException("Enrollment not found.");

            await unitOfWork.enrollment.Delete(enrollment.Id);
            await unitOfWork.CompleteAsync(cancellationToken);
            
        }
    }
}

