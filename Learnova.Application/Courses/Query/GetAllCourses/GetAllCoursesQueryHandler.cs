using AutoMapper;
using Learnova.Application.Courses.DTO;
using Learnova.Application.Courses.Specifications;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Courses.Query.GetAllCourses
{
    public class GetAllCoursesQueryHandler( ILogger<GetAllCoursesQueryHandler> logger, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAllCoursesQuery, IEnumerable<CourseDTO>>
    {
        public async Task<IEnumerable<CourseDTO>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting all courses started.");
            var spec = new CoursesWithDetailsSpecification(
                request.PageNumber,
                request.PageSize,
                request.Search?.Trim(),
                request.CategoryId,
                request.SubCategoryId,
                request.MinPrice,
                request.MaxPrice,
                request.Level,
                request.Sort?.Trim());

            var query = await unitOfWork.Repository<Course>().GetAllWithSpecAsync(spec);
            var courses =  mapper.Map<IEnumerable<CourseDTO>>(query);

            logger.LogInformation("GetAllCoursesQuery completed successfully. Courses count: {Count}", courses.Count());

            return  courses;
        }
    }
}
