using Learnova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entites
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public int CourseId { get; set; }

        public decimal UnitPrice { get; set; }
        public Currency Currency { get; set; }

        public string CourseTitleSnapshot { get; set; } = null!;

        public Order Order { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
