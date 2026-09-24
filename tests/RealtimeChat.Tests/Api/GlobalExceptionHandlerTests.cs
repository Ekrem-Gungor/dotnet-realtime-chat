using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RealtimeChat.Api.ErrorHandling;
using RealtimeChat.Application.Common.Exceptions;
using System.Text.Json;

namespace RealtimeChat.Tests.Api;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_WithValidationException_Returns400ProblemDetails()
    {
        ValidationException exception = new(
        [
            new ValidationFailure("Message", "Message is required.")
        ]);

        HandledProblem result = await HandleAsync(exception);

        Assert.True(result.Handled);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal(
            "Request validation failed.",
            result.Body.GetProperty("title").GetString());

        JsonElement errors = result.Body.GetProperty("errors");

        Assert.Equal(
            "Message is required.",
            errors.GetProperty("Message")[0].GetString());

        Assert.True(result.Body.TryGetProperty("traceId", out _));
    }

    [Fact]
    public async Task TryHandleAsync_WithUnauthorizedException_Returns401ProblemDetails()
    {
        HandledProblem result = await HandleAsync(
            new UnauthorizedAccessException("Internal authentication detail."));

        Assert.True(result.Handled);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        Assert.Equal(
            "Authentication failed.",
            result.Body.GetProperty("title").GetString());

        Assert.DoesNotContain(
            "Internal authentication detail.",
            result.Body.GetRawText());
    }

    [Fact]
    public async Task TryHandleAsync_WithQuotaException_Returns429ProblemDetails()
    {
        HandledProblem result = await HandleAsync(
            new MessageQuotaExceededException());

        Assert.True(result.Handled);
        Assert.Equal(StatusCodes.Status429TooManyRequests, result.StatusCode);
        Assert.Equal(
            "Message quota exceeded.",
            result.Body.GetProperty("title").GetString());
        Assert.Equal(
            "The hourly message quota has been exceeded.",
            result.Body.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task TryHandleAsync_WithUnexpectedException_ReturnsSafe500ProblemDetails()
    {
        HandledProblem result = await HandleAsync(
            new InvalidOperationException("Sensitive database detail."));

        Assert.True(result.Handled);
        Assert.Equal(
            StatusCodes.Status500InternalServerError,
            result.StatusCode);
        Assert.Equal(
            "An unexpected error occurred.",
            result.Body.GetProperty("title").GetString());

        Assert.DoesNotContain(
            "Sensitive database detail.",
            result.Body.GetRawText());
    }

    private static async Task<HandledProblem> HandleAsync(Exception exception)
    {
        ServiceCollection services = new();

        services.AddLogging();
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] =
                    context.HttpContext.TraceIdentifier;
            };
        });

        await using ServiceProvider provider =
            services.BuildServiceProvider();

        DefaultHttpContext httpContext = new()
        {
            RequestServices = provider
        };

        httpContext.Request.Path = "/api/test";
        httpContext.Request.Headers.Accept = "application/problem+json";
        httpContext.Response.Body = new MemoryStream();

        GlobalExceptionHandler handler = new(
            provider.GetRequiredService<IProblemDetailsService>(),
            provider.GetRequiredService<ILogger<GlobalExceptionHandler>>());

        bool handled = await handler.TryHandleAsync(
            httpContext,
            exception,
            CancellationToken.None);

        httpContext.Response.Body.Position = 0;

        using JsonDocument document = await JsonDocument.ParseAsync(
            httpContext.Response.Body);

        return new HandledProblem(
            handled,
            httpContext.Response.StatusCode,
            document.RootElement.Clone());
    }

    private sealed record HandledProblem(
        bool Handled,
        int StatusCode,
        JsonElement Body);
}