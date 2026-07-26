using AutoMapper;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.Reviews.DTO;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Command.CreateReview
{
    public class CreateReviewCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, ILogger<CreateReviewCommandHandler> logger) : IRequestHandler<CreateReviewCommand, ReviewDto>
    {
        public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Review creation failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var course = await unitOfWork.course.GetById(request.CourseId);
            if (course is null)
            {
                logger.LogWarning("Review creation failed because course with id {CourseId} was not found.", request.CourseId);
                throw new NotFoundException("Course not found.");
            }

            var activeEnrollmentSpec = new ActiveEnrollmentByStudentAndCourseSpecification(
                user.Id,
                request.CourseId);

            var hasActiveEnrollment = await unitOfWork
                .Repository<Learnova.Domain.Entities.Enrollment>()
                .AnyWithSpecAsync(activeEnrollmentSpec);

            if (!hasActiveEnrollment)
            {
                logger.LogWarning("Review creation failed because student {StudentId} is not enrolled in course {CourseId}.", user.Id, request.CourseId);
                throw new ForbiddenAccessException("You are not allowed to review this course.");
            }

            var existingReview = (await unitOfWork.review.GetAllWithCondition(r => r.StudentId == user.Id && r.CourseId == request.CourseId)).FirstOrDefault();
            if (existingReview is not null)
            {
                logger.LogWarning("Review creation failed because student {StudentId} already reviewed course {CourseId}.", user.Id, request.CourseId);
                throw new ConflictException("Student has already reviewed this course.");
            }

            var review = new Review
            {
                CourseId = request.CourseId,
                StudentId = user.Id,
                Rating = request.Rating,
                Comment = request.Comment
            };

            await unitOfWork.review.Add(review);
            await unitOfWork.CompleteAsync(cancellationToken);

            var createdReview = await unitOfWork.review.GetById(review.Id, r => r.Student);
            if (createdReview is null)
            {
                logger.LogWarning("Review was created but could not be loaded again. ReviewId: {ReviewId}", review.Id);
                throw new NotFoundException("Review not found.");
            }

            logger.LogInformation("Review {ReviewId} created successfully by student {StudentId} for course {CourseId}.", createdReview.Id, user.Id, request.CourseId);

            return mapper.Map<ReviewDto>(createdReview);
        }
    }
}

