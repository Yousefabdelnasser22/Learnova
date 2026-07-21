using Learnova.Application.Lesson.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Lesson.Query.GetLessonById
{
    public class GetLessonByIdQuery:IRequest<LessonDTO>
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public int ModuleId { get; set; }
    }
}
