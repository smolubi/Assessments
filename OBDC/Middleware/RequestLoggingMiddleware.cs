
namespace OBDC.API.Middleware
{
    public class RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(continueOnCapturedContext: false);
            }
            finally
            {
                _logger.LogInformation(
                    "Request {method} {url} => {statusCode}",
                    context.Request?.Method,
                    context.Request?.Path.Value,
                    context.Response?.StatusCode);
            }
        }
    }
}
