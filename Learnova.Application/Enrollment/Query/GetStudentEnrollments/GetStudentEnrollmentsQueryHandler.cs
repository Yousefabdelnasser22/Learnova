using AutoMapper;
using Learnova.Application.Enrollment.DTO;
using Learnova.Application.Enrollment.Specifications;
using Learnova.Application.Exceptions;
using Learnova.Application.User;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Enrollment.Query.GetStudentEnrollments
{

    public class GetStudentEnrollmentsQueryHandler(IUnitOfWork unitOfWork , ILogger<GetStudentEnrollmentsQueryHandler> logger,IUserContext userContext,IMapper mapper)
        : IRequestHandler<GetStudentEnrollmentsQuery, IEnumerable<StudentEnrollmentDto>>
    {
        public async Task<IEnumerable<StudentEnrollmentDto>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
        {

            var user = userContext.GetCurrentUser();

            if (user is null)
            {
                logger.LogWarning("Fetching student enrollments failed because current user was not found.");
                throw new UnauthorizedException("User is not authenticated.");
            }

            var spec = new StudentEnrollmentsWithCourseSpecification(
                user.Id,
                request.PageNumber,
                request.PageSize,
                request.Search?.Trim());

            var enrollments = await unitOfWork.enrollment.GetAllWithSpecAsync(spec);

          
          return mapper.Map<IEnumerable<StudentEnrollmentDto>>(enrollments);
        }
    }
}
