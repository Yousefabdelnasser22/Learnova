using AutoMapper;
using Learnova.Application.Certificates.DTO;
using Learnova.Application.Certificates.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Certificates.Query.GetMyCertificates
{
    public class GetMyCertificatesQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, ILogger<GetMyCertificatesQueryHandler> logger) : IRequestHandler<GetMyCertificatesQuery, IEnumerable<GetMyCertificatesDto>>
    {
        public async Task<IEnumerable<GetMyCertificatesDto>> Handle(GetMyCertificatesQuery request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Fetching certificates failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var spec = new StudentCertificatesSpecification(
                user.Id,
                request.PageNumber,
                request.PageSize,
                request.Search?.Trim());

            var certificates = await unitOfWork.certificate.GetAllWithSpecAsync(spec);

            return mapper.Map<IEnumerable<GetMyCertificatesDto>>(certificates);
        }
    }
}
