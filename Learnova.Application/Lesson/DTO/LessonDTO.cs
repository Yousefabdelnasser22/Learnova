using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Lesson.DTO
{
    public class LessonDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int Position { get; set; }

        public string? VideoUrl { get; set; }
        public string? TextContent { get; set; }
        public string? PdfUrl { get; set; }

        public string ModuleName { get; set; } = string.Empty;
    }
}
