using AutoMapper;
using Learnova.Application.Courses.DTO;
using Learnova.Application.Courses.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Constant;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Courses.Query.GetCourseForManagement
{
    public class GetCourseForManagementQueryHandler(
        ILogger<GetCourseForManagementQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        IMapper mapper) : IRequestHandler<GetCourseForManagementQuery, CourseDTO>
    {
        public async Task<CourseDTO> Handle(GetCourseForManagementQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling GetCourseForManagementQuery for Course Id: {Id}", request.Id);

            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                throw new UnauthorizedException("User is not authenticated.");
            }

            var spec = new CoursesWithDetailsSpecification(request.Id, includeUnpublished: true);
            var course = await unitOfWork.Repository<Course>().GetEntityWithSpecAsync(spec);

            if (course is null)
            {
                throw new NotFoundException("Course not found.");
            }

            if (!user.IsInRole(UserRole.Admin) && course.InstructorId != user.Id)
            {
                throw new ForbiddenAccessException("You are not allowed to view this course.");
            }

            return mapper.Map<CourseDTO>(course);
        }
    }
}
