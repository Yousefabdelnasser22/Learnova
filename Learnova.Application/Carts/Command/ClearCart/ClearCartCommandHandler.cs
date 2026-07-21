using Learnova.Application.Carts.Specifications;
using Learnova.Application.User;
using Learnova.Domain.Entites;
using Learnova.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Carts.Command.ClearCart
{
    public class ClearCartCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<ClearCartCommand>
    {
        public async Task Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var cartSpec = new CartWithItemsByStudentIdSpecification(user.Id);
            var cart = await unitOfWork.Repository<Cart>().GetEntityWithSpecAsync(cartSpec);

            if (cart is null || !cart.Items.Any())
                return;

            foreach (var item in cart.Items.ToList())
            {
                unitOfWork.Repository<CartItem>().HardDelete(item);
            }

            cart.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}
