using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Learnova.Api.OpenApi
{
    public class TooManyRequestsResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.TryAdd(
                StatusCodes.Status429TooManyRequests.ToString(),
                new OpenApiResponse
                {
                    Description = "Too many requests. The rate limit has been exceeded."
                });
        }
    }
}
