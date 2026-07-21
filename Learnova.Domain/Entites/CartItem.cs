using Learnova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entites
{
    public class CartItem : BaseEntity
    {
        public int CartId { get; set; }
        public int CourseId { get; set; }

        public decimal UnitPrice { get; set; }
        public Currency Currency { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public Cart Cart { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
