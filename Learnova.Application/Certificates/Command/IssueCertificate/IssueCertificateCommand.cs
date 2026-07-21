using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Certificates.Command.IssueCertificate
{
    public class IssueCertificateCommand:IRequest
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int CourseId { get; set; }
    }
}
