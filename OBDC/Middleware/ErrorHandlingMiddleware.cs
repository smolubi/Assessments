using System.Net;
using System.Text.Json;
using OBDC.API.Exceptions;

namespace OBDC.API.Middleware
{
    public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception ex)
            {
                await GlobalExceptionHandler.HandleExceptionAsync(context, ex, _logger).ConfigureAwait(continueOnCapturedContext: false);
            }
        }         
    }
}
