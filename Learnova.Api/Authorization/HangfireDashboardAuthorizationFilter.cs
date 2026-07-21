using Hangfire.Dashboard;
using Learnova.Domain.Constant;

namespace Learnova.Api.Authorization
{
    public sealed class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return httpContext.User.Identity?.IsAuthenticated == true &&
                httpContext.User.IsInRole(UserRole.Admin);
        }
    }
}
