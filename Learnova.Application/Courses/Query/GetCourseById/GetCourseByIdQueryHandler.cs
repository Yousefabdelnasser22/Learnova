using AutoMapper;
using Learnova.Application.Courses.DTO;
using Learnova.Application.Courses.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learnova.Application.Courses.Query.GetCourseById
{
    public class GetCourseByIdQueryHandler(ILogger<GetCourseByIdQueryHandler> logger ,IUnitOfWork unitOfWork,IMapper mapper) : IRequestHandler<GetCourseByIdQuery, CourseDTO>
    {
        public async Task<CourseDTO> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling GetCourseByIdQuery for Course Id: {Id}", request.Id);

            var spec = new CoursesWithDetailsSpecification(request.Id);
            var query = await unitOfWork.Repository<Course>().GetEntityWithSpecAsync(spec);

            if (query is null)
            {
                throw new NotFoundException("Course not found.");
            }

            var course = mapper.Map<CourseDTO>(query);

            return course;
        }
    }
}
