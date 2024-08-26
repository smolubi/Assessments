using System.Diagnostics;

namespace OBDC.API.Middleware
{
    public class RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<RequestTimingMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context)
        {
            var watch = Stopwatch.StartNew();
            await _next(context).ConfigureAwait(continueOnCapturedContext: false);
            watch.Stop();

            _logger.LogInformation(
                "Request {method} {url} took {elapsed}ms",
                context.Request?.Method,
                context.Request?.Path.Value,
                watch.ElapsedMilliseconds);
        }
    }
}
