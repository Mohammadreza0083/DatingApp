using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

/// <summary>
/// Middleware for handling exceptions and returning appropriate error responses
/// </summary>
public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    IHostEnvironment env
)
{
    /// <summary>
    /// Processes the HTTP request and handles any exceptions that occur
    /// </summary>
    /// <param name="context">The HTTP context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create appropriate error response based on environment
            ApiException response = env.IsDevelopment()
                ? new ApiException(
                    context.Response.StatusCode,
                    ex.Message,
                    ex.StackTrace
                )
                : new ApiException(context.Response.StatusCode, "Internal Server Error", null);

            // Configure JSON serialization options
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            // Serialize and return the error response
            string json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
