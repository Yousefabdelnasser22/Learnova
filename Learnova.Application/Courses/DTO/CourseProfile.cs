using AutoMapper;
using Learnova.Application.Courses.Command.CreateCourse;
using Learnova.Application.Courses.Command.UpdateCourse;
using Learnova.Domain.Entities;
using Learnova.Domain.Enums;

namespace Learnova.Application.Courses.DTO
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDTO>()
                .ForMember(x => x.InstructorEmail, opt => opt.MapFrom(s => s.Instructor.Email))
                .ForMember(x => x.SubCategoryName, opt => opt.MapFrom(s => s.SubCategory != null ? s.SubCategory.Name : string.Empty))
                .ForMember(x => x.CategoryId, opt => opt.MapFrom(s => s.SubCategory != null ? s.SubCategory.CategoryId : (int?)null))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(s => s.SubCategory != null && s.SubCategory.Category != null ? s.SubCategory.Category.Name : string.Empty))
                .ReverseMap();

            CreateMap<CreateCourseCommand, Course>()
                .ForMember(x => x.Status, opt => opt.MapFrom(_ => CourseStatus.Draft))
                .ForMember(x => x.Language, opt => opt.MapFrom(s => string.IsNullOrWhiteSpace(s.Language) ? "Arabic" : s.Language));

            CreateMap<UpdateCourseCommand, Course>()
                .ForMember(x => x.Status, opt => opt.Ignore())
                .ForMember(x => x.Language, opt => opt.MapFrom(s => string.IsNullOrWhiteSpace(s.Language) ? "Arabic" : s.Language));
        }
    }
}
