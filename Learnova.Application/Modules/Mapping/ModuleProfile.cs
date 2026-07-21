using AutoMapper;
using Learnova.Application.Modules.Command.CreateModule;
using Learnova.Application.Modules.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Modules.Mapping
{
    public class ModuleProfile:Profile
    {
        public ModuleProfile()
        {
            CreateMap<CreateModuleCommand, Learnova.Domain.Entites.Module>().ReverseMap();
            CreateMap< Learnova.Domain.Entites.Module,ModuleDTO>().ForMember(x => x.CourseName, opt => opt.MapFrom(s => s.Course.Title))
                .ReverseMap();
        }
    }
}
