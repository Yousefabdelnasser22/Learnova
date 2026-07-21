using AutoMapper;
using Learnova.Application.Certificates.DTO;
using Learnova.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Certificates.Mapping
{
    public class CertificateProfile : Profile
    {
        public CertificateProfile()
        {
            CreateMap<Certificate, GetCertificateByIdDto>()
                .ForMember(x => x.CertificateId, opt => opt.MapFrom(s => s.Id))
                .ForMember(x => x.StudentName, opt => opt.MapFrom(s => s.Student.UserName!))
                .ForMember(x => x.CourseTitle, opt => opt.MapFrom(s => s.Course.Title));

            CreateMap<Certificate, GetMyCertificatesDto>()
                .ForMember(x => x.CertificateId, opt => opt.MapFrom(s => s.Id))
                .ForMember(x => x.CourseTitle, opt => opt.MapFrom(s => s.Course.Title));
        }
    }
}
