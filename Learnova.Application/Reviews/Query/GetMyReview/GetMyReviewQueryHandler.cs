using AutoMapper;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.Reviews.DTO;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Query.GetMyReview
{
    public class GetMyReviewQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, ILogger<GetMyReviewQueryHandler> logger) : IRequestHandler<GetMyReviewQuery, ReviewDto>
    {
        public async Task<ReviewDto> Handle(GetMyReviewQuery request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Fetching review failed because current user was not found. CourseId: {CourseId}", request.CourseId);
                throw new UnauthorizedException("User is not authenticated.");
            }

            var reviews = await unitOfWork.review.GetAllWithCondition(
                r => r.CourseId == request.CourseId && r.StudentId == user.Id,
                r => r.Student);

            var review = reviews.FirstOrDefault();

            if (review is null)
            {
                logger.LogWarning("Review not found for current student. CourseId: {CourseId}, StudentId: {StudentId}", request.CourseId, user.Id);
                throw new NotFoundException("Review not found.");
            }

            var activeEnrollmentSpec = new ActiveEnrollmentByStudentAndCourseSpecification(
                user.Id,
                review.CourseId);

            var hasActiveEnrollment = await unitOfWork
                .Repository<Learnova.Domain.Entities.Enrollment>()
                .AnyWithSpecAsync(activeEnrollmentSpec);

            if (!hasActiveEnrollment)
            {
                logger.LogWarning(
                    "Review hidden because student {StudentId} no longer has active enrollment in course {CourseId}.",
                    user.Id,
                    review.CourseId);

                throw new NotFoundException("Review not found.");
            }

            return mapper.Map<ReviewDto>(review);
        }
    }
}

