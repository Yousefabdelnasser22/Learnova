using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Certificates.DTO
{
    public class GetCertificateByIdDto
    {
        public int CertificateId { get; set; }
        public string CertificateNo { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public DateTime IssuedAt { get; set; }
        public string StudentName { get; set; } = null!;
        public string CourseTitle { get; set; } = null!;
    }
}
