using Learnova.Application.Certificates.DTO;
using Learnova.Application.Common.Queries;
using MediatR;

namespace Learnova.Application.Certificates.Query.GetMyCertificates
{
    public class GetMyCertificatesQuery : PagedSearchQuery, IRequest<IEnumerable<GetMyCertificatesDto>>
    {
    }
}
