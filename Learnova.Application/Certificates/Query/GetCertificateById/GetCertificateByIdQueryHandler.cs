using AutoMapper;
using Learnova.Application.Certificates.DTO;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Certificates.Query.GetCertificateById
{
    public class GetCertificateByIdQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, ILogger<GetCertificateByIdQueryHandler> logger) : IRequestHandler<GetCertificateByIdQuery, GetCertificateByIdDto>
    {
        public async Task<GetCertificateByIdDto> Handle(GetCertificateByIdQuery request, CancellationToken cancellationToken)
        {
            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Fetching certificate failed because current user was not found. CertificateId: {CertificateId}", request.CertificateId);
                throw new UnauthorizedException("User is not authenticated.");
            }

            var certificates = await unitOfWork.certificate.GetAllWithCondition(
                c => c.Id == request.CertificateId && c.StudentId == user.Id,
                c => c.Student,
                c => c.Course);

            var certificate = certificates.FirstOrDefault();

            if (certificate is null)
            {
                logger.LogWarning("Certificate not found for current student. CertificateId: {CertificateId}, StudentId: {StudentId}", request.CertificateId, user.Id);
                throw new NotFoundException("Certificate not found.");
            }

            var activeEnrollmentSpec = new ActiveEnrollmentByStudentAndCourseSpecification(
                user.Id,
                certificate.CourseId);

            var hasActiveEnrollment = await unitOfWork
                .Repository<Learnova.Domain.Entities.Enrollment>()
                .AnyWithSpecAsync(activeEnrollmentSpec);

            if (!hasActiveEnrollment)
            {
                logger.LogWarning(
                    "Certificate hidden because student enrollment is not active. CertificateId: {CertificateId}, StudentId: {StudentId}, CourseId: {CourseId}",
                    request.CertificateId,
                    user.Id,
                    certificate.CourseId);

                throw new NotFoundException("Certificate not found.");
            }

            return mapper.Map<GetCertificateByIdDto>(certificate);
        }
    }
}

