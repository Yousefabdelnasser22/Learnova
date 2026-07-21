using Learnova.Domain.Enums;
using MediatR;

namespace Learnova.Application.Courses.Command.UpdateCourse
{
    public class UpdateCourseCommand : IRequest
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; } = 0;
        public string? Thumbnail { get; set; }
        public CourseLevel Level { get; set; } = CourseLevel.Beginner;
        public string Language { get; set; } = "Arabic";
        public string? PreviewVideoUrl { get; set; }
        public int DurationInHours { get; set; } = 0;
        public int SubCategoryId { get; set; }
    }
}
