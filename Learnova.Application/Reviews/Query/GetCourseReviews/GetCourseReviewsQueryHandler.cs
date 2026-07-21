using AutoMapper;
using Learnova.Application.Reviews.DTO;
using Learnova.Application.Reviews.Specifications;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Query.GetCourseReviews
{
    public class GetCourseReviewsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCourseReviewsQueryHandler> logger) : IRequestHandler<GetCourseReviewsQuery, IEnumerable<CourseReviewDto>>
    {
        public async Task<IEnumerable<CourseReviewDto>> Handle(GetCourseReviewsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching reviews for course {CourseId}.", request.CourseId);

            var spec = new CourseReviewsSpecification(
                request.CourseId,
                request.PageNumber,
                request.PageSize,
                request.Search?.Trim());

            var reviews = await unitOfWork.review.GetAllWithSpecAsync(spec);

            return mapper.Map<IEnumerable<CourseReviewDto>>(reviews);
        }
    }
}
