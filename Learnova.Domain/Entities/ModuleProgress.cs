using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class ModuleProgress : BaseEntity
    {
        public string StudentId { get; set; } = default!;
        public int ModuleId { get; set; }

        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }

        public ApplicationUser Student { get; set; } = null!;
        public Module Module { get; set; } = null!;
    }
}
