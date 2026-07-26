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

namespace Learnova.Application.Reviews.Command.UpdateReview
{
    public class UpdateReviewCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, ILogger<UpdateReviewCommandHandler> logger) : IRequestHandler<UpdateReviewCommand, ReviewDto>
    {
        public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Review update failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var review = await unitOfWork.review.GetById(request.ReviewId, r => r.Student);
            if (review is null || review.StudentId != user.Id)
            {
                logger.LogWarning("Review update failed because review {ReviewId} was not found for student {StudentId}.", request.ReviewId, user.Id);
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
                    "Review update failed because student {StudentId} no longer has active enrollment in course {CourseId}.",
                    user.Id,
                    review.CourseId);

                throw new ForbiddenAccessException("You are not allowed to update this review.");
            }

            review.Rating = request.Rating;
            review.Comment = request.Comment;

            unitOfWork.review.Update(review);
            await unitOfWork.CompleteAsync(cancellationToken);

            var updatedReview = await unitOfWork.review.GetById(review.Id, r => r.Student);
            if (updatedReview is null)
            {
                logger.LogWarning("Review was updated but could not be loaded again. ReviewId: {ReviewId}", review.Id);
                throw new NotFoundException("Review not found.");
            }

            logger.LogInformation("Review {ReviewId} updated successfully by student {StudentId}.", updatedReview.Id, user.Id);

            return mapper.Map<ReviewDto>(updatedReview);
        }
    }
}

