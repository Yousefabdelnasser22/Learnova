using Learnova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entites
{
    public class Course:BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? Thumbnail { get; set; }

        public CourseLevel Level { get; set; } = CourseLevel.Beginner;

        public string Language { get; set; } = "Arabic";

        public string? PreviewVideoUrl { get; set; }

        public Currency Currency { get; set; } = Currency.EGP;
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        public DateTime? SubmittedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public int DurationInHours { get; set; } = 0;

        [ForeignKey("Instructor")]
        public string InstructorId { get; set; } = null!;
        public ApplicationUser Instructor { get; set; } = null!;

        public ICollection<Module> Modules { get; set; } = new List<Module>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
          
        public int SubCategoryId { get; set; }

        public SubCategory SubCategory { get; set; } = default!;
    }
}
