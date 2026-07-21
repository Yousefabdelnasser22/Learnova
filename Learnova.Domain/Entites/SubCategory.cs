using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entites
{
    public class SubCategory : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = default!;

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
