using Learnova.Application.Exceptions;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Favorites.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Entites;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Favorites.Commands.AddCourseToFavorites
{
    public class AddCourseToFavoritesCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<AddCourseToFavoritesCommand>
    {
        public async Task Handle(AddCourseToFavoritesCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var course = await unitOfWork.course.GetById(request.CourseId);

            if (course is null)
                throw new NotFoundException("Course not found.");

            if (course.Status != CourseStatus.Published)
                throw new BadRequestException("Course is not available.");

            var activeEnrollmentSpec = new ActiveEnrollmentByStudentAndCourseSpecification(user.Id, request.CourseId);
            var existingEnrollment = await unitOfWork.enrollment.GetEntityWithSpecAsync(activeEnrollmentSpec);

            if (existingEnrollment is not null)
                throw new BadRequestException("You are already enrolled in this course.");

            var favoriteSpec = new FavoriteByStudentAndCourseSpecification(user.Id, request.CourseId);
            var favoriteAlreadyExists = await unitOfWork.Repository<FavoriteList>().AnyWithSpecAsync(favoriteSpec);

            if (favoriteAlreadyExists)
                throw new BadRequestException("Course already exists in favorites.");

            await unitOfWork.Repository<FavoriteList>().Add(new FavoriteList
            {
                StudentId = user.Id,
                CourseId = request.CourseId,
                AddedAt = DateTime.UtcNow
            });

            await unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}
