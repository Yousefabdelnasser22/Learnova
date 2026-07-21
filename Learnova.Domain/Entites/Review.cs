using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entites
{
    public class Review:BaseEntity
    {
        
        public string StudentId { get; set; } = null!;  
        public int CourseId { get; set; }
        public int Rating { get; set; }               
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        public ApplicationUser Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
