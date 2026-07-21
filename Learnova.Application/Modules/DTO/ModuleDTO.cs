using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Modules.DTO
{
    public class ModuleDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int Position { get; set; }

        public string CourseName { get; set; } = string.Empty;
    }
}
