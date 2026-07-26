using Learnova.Application.Certificates.Specifications;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Common;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Certificates.Command.IssueCertificate
{
    using EnrollmentEntity = Learnova.Domain.Entities.Enrollment;

    public class IssueCertificateCommandHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<IssueCertificateCommandHandler> logger) : IRequestHandler<IssueCertificateCommand>
    {
        public async Task Handle(IssueCertificateCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Certificate issuance failed because current user was not found. CourseId: {CourseId}", request.CourseId);
                throw new UnauthorizedException("User is not authenticated.");
            }

            logger.LogInformation("User {UserId} requested certificate for course {CourseId}", user.Id, request.CourseId);

            var course = await unitOfWork.course.GetById(request.CourseId);
            if (course == null)
            {
                logger.LogWarning("Course {CourseId} not found", request.CourseId);
                throw new NotFoundException($"Course with ID {request.CourseId} not found.");
            }

            logger.LogInformation("Found course with ID {CourseId}", course.Id);

            var enroll = await unitOfWork.enrollment.GetByStudentAndCourseAsync(user.Id, course.Id);
            if (enroll == null)
            {
                logger.LogWarning("User {UserId} is not enrolled in course {CourseId}", user.Id, course.Id);
                throw new NotFoundException($"User with ID {user.Id} is not enrolled in course {course.Id}");
            }

            var completedEnrollmentSpec = new CompletedEnrollmentByStudentAndCourseSpecification(user.Id, course.Id);
            var isComplete = await unitOfWork.Repository<EnrollmentEntity>().AnyWithSpecAsync(completedEnrollmentSpec);
            if (!isComplete)
            {
                logger.LogWarning("User {UserId} has not completed course {CourseId}", user.Id, course.Id);
                throw new NotFoundException($"User with ID {user.Id} has not completed course {course.Id}");
            }

            logger.LogInformation("User {UserId} has completed course {CourseId}", user.Id, course.Id);

            var existingCertificateSpec = new CertificateByStudentAndCourseSpecification(user.Id, course.Id);
            var certificateAlreadyIssued = await unitOfWork.Repository<Certificate>().AnyWithSpecAsync(existingCertificateSpec);
            if (certificateAlreadyIssued)
            {
                logger.LogWarning("Certificate already issued for User {UserId} and Course {CourseId}", user.Id, course.Id);
                throw new ConflictException("Certificate has already been issued for this course.");
            }

            var certificateNo = await GenerateUniqueCertificateNumberAsync();
            logger.LogInformation("Generated certificate number {CertificateNo}", certificateNo);

            var certificate = new Certificate
            {
                CourseId = request.CourseId,
                StudentId = user.Id,
                CertificateNo = certificateNo,
                ImageUrl = $"/certificates/{certificateNo}.png"
            };

            await unitOfWork.certificate.Add(certificate);
            logger.LogInformation("Certificate issued for User {UserId} for Course {CourseId} with CertificateNo {CertificateNo}", user.Id, course.Id, certificateNo);

            await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Changes saved to the database successfully.");
        }

        private async Task<string> GenerateUniqueCertificateNumberAsync()
        {
            const int maxAttempts = 5;

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                var certificateNo = CertificateNumberGenerator.Generate();
                var numberExistsSpec = new CertificateByNumberSpecification(certificateNo);
                var numberExists = await unitOfWork.Repository<Certificate>().AnyWithSpecAsync(numberExistsSpec);

                if (!numberExists)
                {
                    return certificateNo;
                }
            }

            throw new ConflictException("Could not generate a unique certificate number. Please try again.");
        }
    }
}
