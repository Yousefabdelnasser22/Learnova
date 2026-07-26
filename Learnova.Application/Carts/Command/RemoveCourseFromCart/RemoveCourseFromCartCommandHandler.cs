using Learnova.Application.Carts.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Carts.Command.RemoveCourseFromCart
{
    public class RemoveCourseFromCartCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<RemoveCourseFromCartCommand>
    {
        public async Task Handle(RemoveCourseFromCartCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var cartSpec = new CartWithItemsByStudentIdSpecification(user.Id);
            var cart = await unitOfWork.Repository<Cart>().GetEntityWithSpecAsync(cartSpec);

            if (cart is null)
                throw new NotFoundException("Cart not found.");

            var cartItem = cart.Items.FirstOrDefault(x => x.CourseId == request.CourseId);

            if (cartItem is null)
                throw new NotFoundException("Course not found in cart.");

            unitOfWork.Repository<CartItem>().HardDelete(cartItem);

            cart.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}
