using Learnova.Domain.Enums;

namespace Learnova.Application.Courses.DTO
{
    public class CourseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; } = 0;
        public string? Thumbnail { get; set; }
        public CourseLevel Level { get; set; }
        public string Language { get; set; } = "Arabic";
        public string? PreviewVideoUrl { get; set; }
        public CourseStatus Status { get; set; }
        public int DurationInHours { get; set; } = 0;
        public string InstructorEmail { get; set; } = null!;
        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
