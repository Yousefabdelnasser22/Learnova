using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Certificates.DTO
{
    public class GetMyCertificatesDto
    {
        public int CertificateId { get; set; }
        public string CertificateNo { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public DateTime IssuedAt { get; set; }
        public string CourseTitle { get; set; } = null!;
    }
}
