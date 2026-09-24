using RealtimeChat.Api.Controllers;
using RealtimeChat.Api.OpenApi;
using System.Reflection;

namespace RealtimeChat.Tests.Api;

public sealed class SwaggerAuthorizationOperationFilterTests
{
    [Fact]
    public void RequiresAuthorization_ForProtectedChatEndpoint_ReturnsTrue()
    {
        MethodInfo endpoint = typeof(ChatController).GetMethod(
            nameof(ChatController.GetAllChatMessages))!;

        bool result = SwaggerAuthorizationOperationFilter
            .RequiresAuthorization(endpoint);

        Assert.True(result);
    }

    [Fact]
    public void RequiresAuthorization_ForAnonymousLoginEndpoint_ReturnsFalse()
    {
        MethodInfo endpoint = typeof(AuthController).GetMethod(
            nameof(AuthController.Login))!;

        bool result = SwaggerAuthorizationOperationFilter
            .RequiresAuthorization(endpoint);

        Assert.False(result);
    }
}
