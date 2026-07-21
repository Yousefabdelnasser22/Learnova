using AutoMapper;
using Learnova.Application.Enrollment.Query.GetStudentEnrollments;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.Query.IsStudentEnrolled
{
    using EnrollmentEntity = Learnova.Domain.Entites.Enrollment;

    public class IsStudentEnrolledQueryHandler(IUnitOfWork unitOfWork, ILogger<GetStudentEnrollmentsQueryHandler> logger, IUserContext userContext) : IRequestHandler<IsStudentEnrolledQuery, bool>
    {
        public async Task<bool> Handle(IsStudentEnrolledQuery request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Checking enrollment failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var course = await unitOfWork.course.GetById(request.CourseId);

            if (course is null)
            {
                logger.LogWarning(" course with id {CourseId} was not found.", request.CourseId);
                throw new NotFoundException($"Course with id {request.CourseId} was not found.");
            }

            var enrollmentSpec = new ActiveEnrollmentByStudentAndCourseSpecification(user.Id, request.CourseId);
            var isEnrolled = await unitOfWork.Repository<EnrollmentEntity>()
                .AnyWithSpecAsync(enrollmentSpec);

            return isEnrolled;
        }
    }
}

