using System.Net;
using System.Text.Json;

namespace OBDC.API.Exceptions
{
    public class GlobalExceptionHandler
    {
        public static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            logger.LogError(exception, "An unhandled exception has occurred");

            var code = HttpStatusCode.InternalServerError;
            var result = JsonSerializer.Serialize(new { error = "An error occurred while processing your request." });

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}
