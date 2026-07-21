using AutoMapper;
using Learnova.Application.Courses.Services;
using Learnova.Application.Enrollment.DTO;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.Query.GetCourseEnrollments
{
    public class GetCourseEnrollmentsQueryHandler(
        IMapper mapper,
        ILogger<GetCourseEnrollmentsQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ICourseAccessService courseAccessService) : IRequestHandler<GetCourseEnrollmentsQuery, IEnumerable<CourseEnrollmentDto>>
    {
        public async Task<IEnumerable<CourseEnrollmentDto>> Handle(GetCourseEnrollmentsQuery request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Fetching course enrollments failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            await courseAccessService.EnsureInstructorOwnsCourseAsync(
                request.CourseId,
                user.Id,
                cancellationToken);

            var spec = new CourseEnrollmentsWithStudentSpecification(
                request.CourseId,
                request.PageNumber,
                request.PageSize,
                request.Search?.Trim());

            var enrollments = await unitOfWork.enrollment.GetAllWithSpecAsync(spec);

            return mapper.Map<IEnumerable<CourseEnrollmentDto>>(enrollments);
        }
    }
    }
