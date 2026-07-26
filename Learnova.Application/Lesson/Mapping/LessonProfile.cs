using AutoMapper;
using Learnova.Application.Lesson.Command.CreateLesson;
using Learnova.Application.Lesson.DTO;
using Learnova.Application.Modules.DTO;
using Learnova.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Lesson.Mapping
{
    public class LessonProfile:Profile
    {
        public LessonProfile()
        {
            CreateMap<CreateLessonCommand,Learnova.Domain.Entities.Lesson>();
            CreateMap<Learnova.Domain.Entities.Lesson, LessonDTO>().ForMember(x => x.ModuleName, opt => opt.MapFrom(s => s.Module.Title))
               .ReverseMap();
        }
    }
}
