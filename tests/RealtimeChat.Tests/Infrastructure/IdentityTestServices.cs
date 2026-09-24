using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Domain.Entities.Concretes;
using RealtimeChat.Persistence.ContextClasses;

namespace RealtimeChat.Tests.Infrastructure;

internal static class IdentityTestServices
{
    public static ServiceProvider CreateProvider()
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddAuthentication();
        services.AddHttpContextAccessor();
        services.AddDbContext<RealtimeChatDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString("N")));

        services
            .AddIdentityCore<AppUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<AppRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<RealtimeChatDbContext>();

        return services.BuildServiceProvider();
    }
}
