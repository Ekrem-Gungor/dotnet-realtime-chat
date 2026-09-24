using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace RealtimeChat.DependencyInjection.Authentication;

public static class JwtBearerTokenResolver
{
    public static string? ResolveFallbackToken(HttpRequest request)
    {
        string? hubToken = request.Query["access_token"];

        if (!string.IsNullOrWhiteSpace(hubToken)
            && request.Path.StartsWithSegments(JwtAuthenticationDefaults.HubPath))
        {
            return hubToken;
        }

        // Açıkça gönderilen Authorization başlığı her zaman cookie'den önceliklidir.
        if (request.Headers.ContainsKey(HeaderNames.Authorization))
            return null;

        return request.Cookies.TryGetValue(
            JwtAuthenticationDefaults.AccessTokenCookieName,
            out string? cookieToken)
                ? cookieToken
                : null;
    }
}
