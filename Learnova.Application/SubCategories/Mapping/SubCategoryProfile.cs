using AutoMapper;
using Learnova.Application.SubCategories.DTO;
using Learnova.Domain.Entities;

namespace Learnova.Application.SubCategories.Mapping
{
    public class SubCategoryProfile : Profile
    {
        public SubCategoryProfile()
        {
            CreateMap<SubCategory, SubCategoryDTO>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ReverseMap();
        }
    }
}
