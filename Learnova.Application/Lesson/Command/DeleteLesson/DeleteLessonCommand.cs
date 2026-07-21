using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Lesson.Command.DeleteLesson
{
    public class DeleteLessonCommand(int id):IRequest
    {
        public int Id { get; set; } = id;
        public int CourseId { get; set; }
        public int ModuleId { get; set; }


    }
}
