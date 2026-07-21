using Learnova.Application.Exceptions;
using Learnova.Application.User;
using System.Security.Claims;

namespace Learnova.Api.Services
{
    public sealed class HttpUserContext(
        IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public CurrentUser GetCurrentUser()
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user is null)
            {
                throw new UnauthorizedException("User context is not present.");
            }

            if (user.Identity is null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedException("User is not authenticated.");
            }

            var userId =
                user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("sub");
            var email =
                user.FindFirstValue(ClaimTypes.Email)
                ?? user.FindFirstValue("email")
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedException("User is not authenticated.");
            }

            var roles = user.Claims
                .Where(claim => claim.Type == ClaimTypes.Role)
                .Select(claim => claim.Value);

            return new CurrentUser(userId, email, roles);
        }
    }
}
