using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entites
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    }
}
