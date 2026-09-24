using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RealtimeChat.DependencyInjection.DemoIdentity;
using RealtimeChat.Domain.Entities.Concretes;
using RealtimeChat.Persistence.ContextClasses;

namespace RealtimeChat.Tests.Infrastructure;

public sealed class DevelopmentDemoIdentityInitializerTests
{
    [Fact]
    public async Task InitializeAsync_CreatesOneMemberUserWhenCalledTwice()
    {
        await using ServiceProvider provider = IdentityTestServices.CreateProvider();
        await provider.GetRequiredService<RealtimeChatDbContext>().Database.EnsureCreatedAsync();

        UserManager<AppUser> userManager = provider.GetRequiredService<UserManager<AppUser>>();
        RoleManager<AppRole> roleManager = provider.GetRequiredService<RoleManager<AppRole>>();
        int initialUserCount = await userManager.Users.CountAsync();

        DevelopmentDemoIdentityInitializer initializer = new(
            userManager,
            roleManager,
            new TestHostEnvironment(Environments.Development),
            Options.Create(CreateOptions()));

        await initializer.InitializeAsync();
        await initializer.InitializeAsync();

        AppUser? demoUser = await userManager.FindByNameAsync("realtime.demo");

        Assert.NotNull(demoUser);
        Assert.True(demoUser.EmailConfirmed);
        Assert.True(await userManager.CheckPasswordAsync(demoUser, "Valid_password1!"));
        Assert.True(await userManager.IsInRoleAsync(demoUser, "Member"));
        Assert.Equal(initialUserCount + 1, await userManager.Users.CountAsync());
    }

    [Fact]
    public async Task InitializeAsync_WhenEnabledOutsideDevelopment_Throws()
    {
        await using ServiceProvider provider = IdentityTestServices.CreateProvider();
        UserManager<AppUser> userManager = provider.GetRequiredService<UserManager<AppUser>>();
        RoleManager<AppRole> roleManager = provider.GetRequiredService<RoleManager<AppRole>>();

        DevelopmentDemoIdentityInitializer initializer = new(
            userManager,
            roleManager,
            new TestHostEnvironment(Environments.Production),
            Options.Create(CreateOptions()));

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => initializer.InitializeAsync());

        Assert.Contains("Development", exception.Message);
    }

    private static DemoIdentityOptions CreateOptions()
    {
        return new DemoIdentityOptions
        {
            Enabled = true,
            UserName = "realtime.demo",
            Email = "realtime.demo@example.invalid",
            Password = "Valid_password1!"
        };
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public TestHostEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "RealtimeChat.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
