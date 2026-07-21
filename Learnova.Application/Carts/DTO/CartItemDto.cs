using Learnova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Carts.DTO
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }

        public int CourseId { get; set; }

        public string CourseTitle { get; set; } = null!;

        public decimal UnitPrice { get; set; }

        public Currency Currency { get; set; }
    }
}
