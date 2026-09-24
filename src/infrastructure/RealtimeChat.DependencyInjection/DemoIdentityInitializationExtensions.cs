using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.DependencyInjection.DemoIdentity;

namespace RealtimeChat.DependencyInjection;

public static class DemoIdentityInitializationExtensions
{
    public static async Task BootstrapDemoIdentityAsync(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        DevelopmentDemoIdentityInitializer initializer =
            scope.ServiceProvider.GetRequiredService<DevelopmentDemoIdentityInitializer>();

        await initializer.InitializeAsync();
    }
}
