using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entites
{
    public class Certificate:BaseEntity
    {
      
        public string StudentId { get; set; } = null!;
        public int CourseId { get; set; }
        public string CertificateNo { get; set; } = null!;
        public string? ImageUrl { get; set; }       
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

     
        public ApplicationUser Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
