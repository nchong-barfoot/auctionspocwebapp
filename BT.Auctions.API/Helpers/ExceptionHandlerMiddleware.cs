using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BT.Auctions.API.Helpers
{
    /// <summary>
    /// Exception handler used to filter all uncaught application exceptions
    /// All application requests will pass through the invoke method 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute" />
    public class ExceptionHandlerMiddleware : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next">Request Delegate currently being invoked</param>
        /// <param name="logger">The logger.</param>
        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var status = HttpStatusCode.InternalServerError;
                string reason;
                var exceptionType = ex.GetType();
                if (exceptionType == typeof(UnauthorizedAccessException) || ex.InnerException?.GetType() == typeof(UnauthorizedAccessException))
                {
                    status = HttpStatusCode.Unauthorized;
                    _logger.LogWarning("Unauthorised access", ex);
                    reason = "Unauthorised Access to Auctions API";
                }
                else if (exceptionType == typeof(NotImplementedException))
                {
                    status = HttpStatusCode.NotImplemented;
                    reason = "Requested Auctions API method is not yet implemented";
                }
                else
                {
                    _logger.LogError("Uncaught Exception Occurred {ExceptionStackTrace}", ex.ToString());
                    reason = "A Server Error has Occurred";
                }

                var response = context.Response;
                response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = reason;
                response.StatusCode = (int)status;
            }
        }
    }
}
