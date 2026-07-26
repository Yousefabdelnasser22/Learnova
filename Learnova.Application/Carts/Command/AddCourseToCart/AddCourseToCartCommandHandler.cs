using Learnova.Application.Carts.Specifications;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Learnova.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Carts.Command.AddCourseToCart
{
    public class AddCourseToCartCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<AddCourseToCartCommand>
    {
        public async Task Handle(AddCourseToCartCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var course = await unitOfWork.course.GetById(request.CourseId);

            if (course is null)
                throw new NotFoundException("Course not found.");

          
            if (course.Status != CourseStatus.Published)
                throw new BadRequestException("Course is not available for purchase.");

            var activeEnrollmentSpec = new ActiveEnrollmentByStudentAndCourseSpecification(user.Id, request.CourseId);
            var existingEnrollment = await unitOfWork.enrollment.GetEntityWithSpecAsync(activeEnrollmentSpec);

            if (existingEnrollment is not null)
                throw new BadRequestException("You are already enrolled in this course.");

            var cartSpec = new CartWithItemsByStudentIdSpecification(user.Id);
            var cart = await unitOfWork.Repository<Cart>().GetEntityWithSpecAsync(cartSpec);

            if (cart is null)
            {
                cart = new Cart
                {
                    StudentId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await unitOfWork.Repository<Cart>().Add(cart);
            }

            var itemAlreadyExists = cart.Items.Any(x => x.CourseId == request.CourseId);

           
            if (itemAlreadyExists)
                throw new BadRequestException("Course already exists in cart.");
           
            cart.Items.Add(new CartItem
            {
                CourseId = course.Id,
                UnitPrice = course.Price,
                Currency = course.Currency,
                AddedAt = DateTime.UtcNow
            });

            cart.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}
