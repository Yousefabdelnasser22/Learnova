using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entites
{
    public class Lesson : BaseEntity
    {
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int Position { get; set; }

        public string? VideoUrl { get; set; }
        public string? TextContent { get; set; }
        public string? PdfUrl { get; set; }
    }
}
