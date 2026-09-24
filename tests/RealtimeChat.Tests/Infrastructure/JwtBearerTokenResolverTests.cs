using Microsoft.AspNetCore.Http;
using RealtimeChat.DependencyInjection.Authentication;

namespace RealtimeChat.Tests.Infrastructure;

public sealed class JwtBearerTokenResolverTests
{
    [Fact]
    public void ResolveFallbackToken_ForSignalRHandshake_PrefersQueryToken()
    {
        DefaultHttpContext context = CreateContextWithCookie();
        context.Request.Path = JwtAuthenticationDefaults.HubPath;
        context.Request.QueryString = QueryString.Create(
            "access_token",
            "hub-token");
        context.Request.Headers.Authorization = "Bearer header-token";

        string? token = JwtBearerTokenResolver.ResolveFallbackToken(
            context.Request);

        Assert.Equal("hub-token", token);
    }

    [Fact]
    public void ResolveFallbackToken_WithAuthorizationHeader_DoesNotOverrideHeader()
    {
        DefaultHttpContext context = CreateContextWithCookie();
        context.Request.Path = "/api/Chat/messages";
        context.Request.Headers.Authorization = "Bearer header-token";

        string? token = JwtBearerTokenResolver.ResolveFallbackToken(
            context.Request);

        Assert.Null(token);
    }

    [Fact]
    public void ResolveFallbackToken_WithoutAuthorizationHeader_UsesCookie()
    {
        DefaultHttpContext context = CreateContextWithCookie();
        context.Request.Path = "/api/Chat/messages";

        string? token = JwtBearerTokenResolver.ResolveFallbackToken(
            context.Request);

        Assert.Equal("cookie-token", token);
    }

    private static DefaultHttpContext CreateContextWithCookie()
    {
        DefaultHttpContext context = new();
        context.Request.Headers.Cookie =
            $"{JwtAuthenticationDefaults.AccessTokenCookieName}=cookie-token";

        return context;
    }
}
