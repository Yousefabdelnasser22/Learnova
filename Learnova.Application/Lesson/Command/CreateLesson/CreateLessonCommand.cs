using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Lesson.Command.CreateLesson
{
    public class CreateLessonCommand : IRequest
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int CourseId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public int ModuleId { get; set; }

        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public string? VideoUrl { get; set; }
        public string? TextContent { get; set; }
        public string? PdfUrl { get; set; }

        public int Position { get; set; }
    }
}
