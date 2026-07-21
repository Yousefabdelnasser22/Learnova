using AutoMapper;
using Learnova.Application.Courses.DTO;
using Learnova.Application.Courses.Services;
using Learnova.Application.Courses.Specifications;
using Learnova.Domain.Entites;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Courses.Query.SearchCourses
{
    public class SearchCoursesQueryHandler(
        ILogger<SearchCoursesQueryHandler> logger,
        ICourseSearchService courseSearchService,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<SearchCoursesQuery, List<CourseDTO>>
    {
        public async Task<List<CourseDTO>> Handle(SearchCoursesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Searching courses. SearchTerm: {SearchTerm}, Limit: {Limit}",
                request.SearchTerm,
                request.Limit);

            List<int> courseIds;
            try
            {
                courseIds = await courseSearchService.SearchAsync(
                    request.SearchTerm,
                    request.Limit,
                    cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(
                    ex,
                    "Semantic course search failed. Falling back to SQL search. SearchTerm: {SearchTerm}",
                    request.SearchTerm);

                var fallbackSpec = new CoursesWithDetailsSpecification(
                    1,
                    request.Limit,
                    request.SearchTerm,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null);

                var fallbackCourses = await unitOfWork.Repository<Course>().GetAllWithSpecAsync(fallbackSpec);
                return mapper.Map<List<CourseDTO>>(fallbackCourses);
            }

            if (!courseIds.Any())
            {
                return new List<CourseDTO>();
            }

            var spec = new CoursesByIdsSpec(courseIds);
            var courses = await unitOfWork.Repository<Course>().GetAllWithSpecAsync(spec);
            var coursesById = courses.ToDictionary(c => c.Id);

            var orderedCourses = courseIds
                .Where(coursesById.ContainsKey)
                .Select(id => coursesById[id])
                .ToList();

            var result = mapper.Map<List<CourseDTO>>(orderedCourses);

            logger.LogInformation("SearchCoursesQuery completed successfully. Courses count: {Count}", result.Count);

            return result;
        }
    }
}
