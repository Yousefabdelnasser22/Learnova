using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Learnova.Api.Extensions
{
    public static class RateLimitingExtensions
    {
        private const string AuthIpPolicy = "auth-ip";
        private const string UserSensitivePolicy = "user-sensitive";
        private const string WebhookPolicy = "webhook";
        private const string UnknownClient = "unknown-client";

        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        GetClientIpPartitionKey(httpContext),
                        _ => CreateFixedWindowOptions(300, TimeSpan.FromMinutes(1))));

                options.AddPolicy(AuthIpPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        GetClientIpPartitionKey(httpContext),
                        _ => CreateFixedWindowOptions(10, TimeSpan.FromMinutes(1))));

                options.AddPolicy(UserSensitivePolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        GetUserOrIpPartitionKey(httpContext),
                        _ => CreateFixedWindowOptions(20, TimeSpan.FromMinutes(1))));

                options.AddPolicy(WebhookPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        GetClientIpPartitionKey(httpContext),
                        _ => CreateFixedWindowOptions(120, TimeSpan.FromMinutes(1))));
            });

            return services;
        }

        private static FixedWindowRateLimiterOptions CreateFixedWindowOptions(
            int permitLimit,
            TimeSpan window)
        {
            return new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            };
        }

        private static string GetClientIpPartitionKey(HttpContext httpContext)
        {
            return $"ip:{httpContext.Connection.RemoteIpAddress?.ToString() ?? UnknownClient}";
        }

        private static string GetUserOrIpPartitionKey(HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return string.IsNullOrWhiteSpace(userId)
                ? GetClientIpPartitionKey(httpContext)
                : $"user:{userId}";
        }
    }
}
