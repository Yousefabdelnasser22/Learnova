using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Command.DeleteReview
{
    public class DeleteReviewCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, ILogger<DeleteReviewCommandHandler> logger) : IRequestHandler<DeleteReviewCommand, bool>
    {
        public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Review deletion failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var review = await unitOfWork.review.GetById(request.ReviewId);
            if (review is null || review.StudentId != user.Id)
            {
                logger.LogWarning("Review deletion failed because review {ReviewId} was not found for student {StudentId}.", request.ReviewId, user.Id);
                throw new NotFoundException("Review not found.");
            }

            await unitOfWork.review.Delete(request.ReviewId);
            await unitOfWork.CompleteAsync(cancellationToken);

            logger.LogInformation("Review {ReviewId} deleted successfully by student {StudentId}.", request.ReviewId, user.Id);

            return true;
        }
    }
}

