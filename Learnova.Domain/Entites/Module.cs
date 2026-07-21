using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entites
{
    public class Module:BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int Position { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }  
        public Course Course { get; set; } = null!;

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
