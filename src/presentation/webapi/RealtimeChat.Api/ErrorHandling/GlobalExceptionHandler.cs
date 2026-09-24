using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Common.Exceptions;

namespace RealtimeChat.Api.ErrorHandling
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            IProblemDetailsService problemDetailsService,
            ILogger<GlobalExceptionHandler> logger)
        {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails = CreateProblemDetails(httpContext, exception);

            if (problemDetails.Status >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception occurred while processing {Method} {Path}",
                    httpContext.Request.Method,
                    httpContext.Request.Path);
            }

            httpContext.Response.StatusCode =
                problemDetails.Status ?? StatusCodes.Status500InternalServerError;

            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    Exception = exception,
                    ProblemDetails = problemDetails
                });
        }

        private static ProblemDetails CreateProblemDetails(
            HttpContext httpContext,
            Exception exception)
        {
            return exception switch
            {
                ValidationException validationException => CreateValidationProblemDetails(httpContext, validationException),

                MessageQuotaExceededException quotaException => CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status429TooManyRequests,
                    "Message quota exceeded.",
                    quotaException.Message),

                UnauthorizedAccessException => CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status401Unauthorized,
                    "Authentication failed.",
                    "Authentication is required or the supplied credentials are invalid."),

                ArgumentException argumentException => CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status400BadRequest,
                    "Invalid request.",
                    argumentException.Message),

                _ => CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.",
                    "The server could not complete the request.")
            };
        }

        private static ValidationProblemDetails CreateValidationProblemDetails(
            HttpContext httpContext,
            ValidationException exception)
        {
            Dictionary<string, string[]> errors = exception.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .Distinct()
                        .ToArray());

            return new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Request validation failed.",
                Detail = "One or more validation errors occurred.",
                Type = GetProblemType(StatusCodes.Status400BadRequest),
                Instance = httpContext.Request.Path
            };
        }

        private static ProblemDetails CreateProblemDetails(
            HttpContext httpContext,
            int statusCode,
            string title,
            string detail)
        {
            return new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Type = GetProblemType(statusCode),
                Instance = httpContext.Request.Path
            };
        }

        private static string GetProblemType(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest =>
                    "https://www.rfc-editor.org/rfc/rfc9110.html#name-400-bad-request",

                StatusCodes.Status401Unauthorized =>
                    "https://www.rfc-editor.org/rfc/rfc9110.html#name-401-unauthorized",

                StatusCodes.Status429TooManyRequests =>
                    "https://www.rfc-editor.org/rfc/rfc6585.html#section-4",

                _ =>
                    "https://www.rfc-editor.org/rfc/rfc9110.html#name-500-internal-server-error"
            };
        }
    }
}
