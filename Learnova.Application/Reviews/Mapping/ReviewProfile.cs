using AutoMapper;
using Learnova.Application.Reviews.DTO;
using Learnova.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Mapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewDto>()
                .ForMember(x => x.ReviewId, opt => opt.MapFrom(s => s.Id))
                .ForMember(x => x.StudentName, opt => opt.MapFrom(s => s.Student.UserName!));

            CreateMap<Review, CourseReviewDto>()
                .ForMember(x => x.ReviewId, opt => opt.MapFrom(s => s.Id))
                .ForMember(x => x.StudentName, opt => opt.MapFrom(s => s.Student.UserName!));
        }
    }
}
