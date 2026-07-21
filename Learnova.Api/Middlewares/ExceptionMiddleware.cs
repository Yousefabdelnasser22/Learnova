using FluentValidation;
using Learnova.Application.Exceptions;

namespace Learnova.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (ex is not BaseException &&
                    ex is not ValidationException &&
                    ex is not UnauthorizedAccessException)
                {
                    _logger.LogError(ex, "Unhandled exception while processing request.");
                }

                await HandleException(context, ex);
            }
        }

        private static async Task HandleException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            int statusCode = 500;
            object response;

            if (ex is BaseException baseEx)
            {
                statusCode = baseEx.StatusCode;

                response = new
                {
                    message = baseEx.Message
                };
            }
            else if (ex is ValidationException validationEx)
            {
                statusCode = 400;

                response = new
                {
                    message = string.Join(", ", validationEx.Errors.Select(e => e.ErrorMessage).Distinct())
                };
            }
            else if (ex is UnauthorizedAccessException)
            {
                statusCode = 401;

                response = new
                {
                    message = string.IsNullOrWhiteSpace(ex.Message)
                        ? "User is not authenticated."
                        : ex.Message
                };
            }
            else if (ex is BadHttpRequestException badRequestEx)
            {
                statusCode = badRequestEx.StatusCode;
                response = new
                {
                    message = "The request is missing a required value or contains an invalid value."
                };
            }
            else
            {
                response = new
                {
                    message = "Internal Server Error"
                };
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
