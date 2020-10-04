using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Threading.Tasks;
using ContactsApi.Exceptions;
using Microsoft.Extensions.Hosting;

namespace IdentityServer.Securrency.Web.Middlewares
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        private readonly Dictionary<ErrorType, int> _errorTypeToHttpStatusCodeMap = new Dictionary<ErrorType, int>
        {
            { ErrorType.ClientError, StatusCodes.Status400BadRequest },
            { ErrorType.ServerError, StatusCodes.Status500InternalServerError },
            { ErrorType.AuthenticationRequired, StatusCodes.Status401Unauthorized },
            { ErrorType.ResourceNotFound, StatusCodes.Status404NotFound },
            { ErrorType.ResourceAlreadyExists, StatusCodes.Status409Conflict },
            { ErrorType.AccessForbidden, StatusCodes.Status403Forbidden },
            { ErrorType.NotAllowed, StatusCodes.Status405MethodNotAllowed },
            { ErrorType.ValidationError, StatusCodes.Status400BadRequest },
            { ErrorType.Unprocessable, StatusCodes.Status422UnprocessableEntity }
        };

        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
           await AddProcessingExceptions(context);
        }

        private async Task AddProcessingExceptions(HttpContext context)
        {
            try
            {
                if (_next != null)
                {
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                await HandleIdentityServerException(context, ex);
            }
        }

        private Task HandleIdentityServerException(HttpContext context, Exception ex)
        {
            //if (ex.ErrorType == ErrorType.ServerError)
            //{
            //    _logger.LogError(ex, "An error occurred while processing an API request");
            //}
            //else
            //{
            //    _logger.LogWarning(ex, "A warning occurred while processing an API request");
            //}
            var aaa = ex.GetType();
            //int httpStatusCode = GetHttpCodeByErrorType(ex.GetType());
            //dynamic errorResponse = CreateErrorResponse(ex.ErrorCode, ex.Message, ex.StackTrace);
            return HandleException(context, 401, "errorResponse");
        }

        private Task HandleApiInternalException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing an API request.");

            dynamic errorResponse = CreateErrorResponse(StatusCodes.Status500InternalServerError, "Internal server error.", ex.StackTrace);
            return HandleException(context, StatusCodes.Status500InternalServerError, errorResponse);
        }

        private dynamic CreateErrorResponse(int errorCode, string message, string stackTrace)
        {
            dynamic errorResponse = new ExpandoObject();

            errorResponse.ErrorCode = errorCode;
            errorResponse.Message = message;

            if (_env.IsEnvironment("Local") || _env.IsDevelopment())
            {
                errorResponse.StackTrace = stackTrace;
            }

            return errorResponse;
        }

        private Task HandleException(HttpContext context, int httpStatusCode, object errorResponse)
        {
            context.Response.ContentType = MediaTypes.APPLICATION_JSON;
            context.Response.StatusCode = httpStatusCode;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }

        private int GetHttpCodeByErrorType(ErrorType errorType)
        {
            if (!_errorTypeToHttpStatusCodeMap.TryGetValue(errorType, out var statusCode))
            {
                statusCode = StatusCodes.Status500InternalServerError;
            }
            return statusCode;
        }
    }
}