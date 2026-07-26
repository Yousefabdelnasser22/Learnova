using AutoMapper;
using Learnova.Application.Enrollment.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.Mapping
{
    public class EnrollmentProfile : Profile
    {
        public EnrollmentProfile()
        {
            CreateMap<Learnova.Domain.Entities.Enrollment, StudentEnrollmentDto>()
                .ForMember(dest => dest.EnrollmentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.CourseDescription, opt => opt.MapFrom(src => src.Course.Description));



            CreateMap<Learnova.Domain.Entities.Enrollment, CourseEnrollmentDto>()
          .ForMember(dest => dest.EnrollmentId, opt => opt.MapFrom(src => src.Id))
          .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.Student.Email));
        }
    }
}
