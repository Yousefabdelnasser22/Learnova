using Learnova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Carts.DTO
{
    public class CartDto
    {
        public int? CartId { get; set; }

        public List<CartItemDto> Items { get; set; } = new();

        public decimal TotalAmount { get; set; }

        public Currency? Currency { get; set; }
    }
}
