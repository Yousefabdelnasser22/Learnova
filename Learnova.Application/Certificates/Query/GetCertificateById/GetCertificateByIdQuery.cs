using Learnova.Application.Certificates.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Certificates.Query.GetCertificateById
{
    public class GetCertificateByIdQuery(int certificateId) : IRequest<GetCertificateByIdDto>
    {
        public int CertificateId { get; } = certificateId;
    }
}
