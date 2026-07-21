using Learnova.Application.Carts.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Carts.Query.GetMyCart
{
    public class GetMyCartQuery : IRequest<CartDto>;
}
