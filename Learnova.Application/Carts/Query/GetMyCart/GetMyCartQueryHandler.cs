using Learnova.Application.Carts.DTO;
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

namespace Learnova.Application.Carts.Query.GetMyCart
{
    public class GetMyCartQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<GetMyCartQuery, CartDto>
    {
        public async Task<CartDto> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var cartSpec = new CartWithItemsByStudentIdSpecification(user.Id);
            var cart = await unitOfWork.Repository<Cart>().GetEntityWithSpecAsync(cartSpec);

            if (cart is null)
            {
                return new CartDto();
            }

            if (!cart.Items.Any())
            {
                return new CartDto { CartId = cart.Id };
            }

            var items = cart.Items.Select(x => new CartItemDto
            {
                CartItemId = x.Id,
                CourseId = x.CourseId,
                CourseTitle = x.Course.Title,
                UnitPrice = x.UnitPrice,
                Currency = x.Currency
            }).ToList();

            return new CartDto
            {
                CartId = cart.Id,
                Items = items,
                TotalAmount = items.Sum(x => x.UnitPrice),
                Currency = items.FirstOrDefault()?.Currency
            };
        }
    }
}
