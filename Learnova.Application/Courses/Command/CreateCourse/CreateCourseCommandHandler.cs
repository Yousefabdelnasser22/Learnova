using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Courses.Command.CreateCourse
{
    public class CreateCourseCommandHandler(ILogger<CreateCourseCommandHandler>logger,IUnitOfWork unitOfWork,IUserContext userContext) : IRequestHandler<CreateCourseCommand>
    {
        public async Task Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Starting course creation. Title: {Title}, Price: {Price}, DurationInHours: {DurationInHours}",
                request.Title,
                request.Price,
                request.DurationInHours);

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Course creation failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

           
                var subCategory = await unitOfWork.subCategory.GetById(request.SubCategoryId);

                if (subCategory is null)
                {
                    logger.LogWarning("SubCategory not found. SubCategoryId: {SubCategoryId}", request.SubCategoryId);
                    throw new NotFoundException("SubCategory not found.");
                }
           

            Course course = new Course()
            {
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                Thumbnail = request.Thumbnail,
                Level = request.Level,
                Language = string.IsNullOrWhiteSpace(request.Language) ? "Arabic" : request.Language,
                PreviewVideoUrl = request.PreviewVideoUrl,
                Status = CourseStatus.Draft,
                DurationInHours = request.DurationInHours,
                InstructorId = user.Id,
                SubCategoryId = request.SubCategoryId,
            };

            await unitOfWork.course.Add(course);
           
            await unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}

