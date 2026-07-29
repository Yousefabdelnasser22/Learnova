using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Learnova.Api.OpenApi
{
    public sealed class IdentityRegistrationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var path = context.ApiDescription.RelativePath?.TrimEnd('/');

            if (!string.Equals(
                    context.ApiDescription.HttpMethod,
                    HttpMethods.Post,
                    StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(
                    path,
                    "api/identity/register",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            operation.Summary = "Register a new account";
            operation.Description =
                "Use a real email address. We will send you a confirmation link, " +
                "and you will not be able to sign in until your email is confirmed. " +
                "After registering, check your inbox and spam folder, then follow " +
                "the confirmation link.";

            if (operation.Responses.TryGetValue(StatusCodes.Status200OK.ToString(), out var response))
            {
                response.Description =
                    "The account was created successfully. Check your inbox or spam " +
                    "folder and follow the email confirmation link.";
            }
        }
    }
}
