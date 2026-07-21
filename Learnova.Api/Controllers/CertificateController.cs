using Learnova.Application.Certificates.Command.IssueCertificate;
using Learnova.Application.Certificates.Query.GetCertificateById;
using Learnova.Application.Certificates.Query.GetMyCertificates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Learnova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController(IMediator mediator) : ControllerBase
    {
        [HttpGet("{certificateId}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get a certificate by ID",
            Description = "Retrieves a certificate available to the authenticated user.")]
        public async Task<IActionResult> GetById(int certificateId)
        {
            var certificate = await mediator.Send(new GetCertificateByIdQuery(certificateId));

            if (certificate == null)
            {
                return NotFound();
            }

            return Ok(certificate);
        }

        [HttpGet("my")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get my certificates",
            Description = "Retrieves a paginated list of certificates issued to the authenticated user.")]
        public async Task<IActionResult> GetMyCertificates(
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var certificates = await mediator.Send(new GetMyCertificatesQuery
            {
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            if (certificates == null)
            {
                return NotFound();
            }

            return Ok(certificates);
        }

        [HttpPost]
        [Route("/api/courses/{courseId}/IssueCertificate")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Issue a course certificate",
            Description = "Issues a certificate to the authenticated student after satisfying the course completion requirements.")]
        public async Task<IActionResult> Issue(int courseId, IssueCertificateCommand command)
        {
            command.CourseId = courseId;

            await mediator.Send(command);

            return Created();
        }
    }
}
