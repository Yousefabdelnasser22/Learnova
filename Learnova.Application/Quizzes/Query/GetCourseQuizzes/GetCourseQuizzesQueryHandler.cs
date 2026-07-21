using AutoMapper;
using Learnova.Application.Courses.Services;
using Learnova.Application.Exceptions;
using Learnova.Application.Quizzes.DTO;
using Learnova.Application.Quizzes.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Quizzes.Query.GetCourseQuizzes
{
    public class GetCourseQuizzesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetCourseQuizzesQuery> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService) : IRequestHandler<GetCourseQuizzesQuery, IEnumerable<GetAllQuizzesDTO>>
    {
        public async Task<IEnumerable<GetAllQuizzesDTO>> Handle(GetCourseQuizzesQuery request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();
            if (user is null)
            {
                logger.LogWarning("Course quizzes request failed because current user was not found. CourseId: {CourseId}", request.CourseId);
                throw new UnauthorizedException("User is not authenticated.");
            }

            var course = await unitOfWork.course.GetById(request.CourseId);
            if (course == null)
            {
                throw new NotFoundException("Course not found.");
            }

            await courseAccessService.EnsureCanViewCourseContentAsync(
                course.Id,
                user,
                cancellationToken);

            var spec = new CourseQuizzesSpecification(
                course.Id,
                request.PageNumber,
                request.PageSize,
                request.Search?.Trim());

            var quizzes = await unitOfWork.quiz.GetAllWithSpecAsync(spec);

            return mapper.Map<IEnumerable<GetAllQuizzesDTO>>(quizzes);
        }
    }
}
