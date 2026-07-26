using Learnova.Application.Exceptions;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.Command.EnrollStudent
{
    using EnrollmentEntity = Learnova.Domain.Entities.Enrollment;

    public class EnrollStudentCommandHandler(IUserContext userContext, ILogger<EnrollStudentCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<EnrollStudentCommand>
    {
        public async Task Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
        {

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Enrollment failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var course = await unitOfWork.course.GetById(request.CourseId);
            if (course is null)
            {
                logger.LogWarning("Enrollment failed because course with id {CourseId} was not found.", request.CourseId);
                throw new NotFoundException("Course not found.");
            }

            if (course.Status != CourseStatus.Published)
            {
                throw new BadRequestException("Course is not available for enrollment.");
            }

            if (course.Price > 0)
            {
                throw new BadRequestException("Paid courses must be purchased through checkout.");
            }

            var enrollmentSpec = new EnrollmentByStudentAndCourseSpecification(user.Id, request.CourseId);
            var alreadyEnrolled = await unitOfWork.Repository<EnrollmentEntity>()
                .AnyWithSpecAsync(enrollmentSpec);

            if (alreadyEnrolled)
            {
                logger.LogWarning(
                    "Enrollment failed because student {StudentId} is already enrolled in course {CourseId}.",
                    user.Id,
                    request.CourseId);

                throw new ConflictException("Student is already enrolled in this course.");
            }

            var enrollment = new Learnova.Domain.Entities.Enrollment
            {
                CourseId = request.CourseId,
                StudentId = user.Id,
                Status = EnrollmentStatus.Active

            };

            await unitOfWork.enrollment.Add(enrollment);
            await unitOfWork.CompleteAsync(cancellationToken);

            logger.LogInformation(
                "Student {StudentId} enrolled successfully in course {CourseId}.",
                user.Id,
                request.CourseId);
        }
    }
    }

