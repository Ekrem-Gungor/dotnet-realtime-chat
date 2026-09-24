using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Application.Features.Auths.Commands;
using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Domain.Entities.Concretes;
using RealtimeChat.Persistence.ContextClasses;
using RealtimeChat.Tests.Infrastructure;

namespace RealtimeChat.Tests.Application;

public sealed class LoginUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidPassword_ReturnsUserWithoutCreatingCookie()
    {
        await using ServiceProvider provider = IdentityTestServices.CreateProvider();
        await provider.GetRequiredService<RealtimeChatDbContext>().Database.EnsureCreatedAsync();

        UserManager<AppUser> userManager = provider.GetRequiredService<UserManager<AppUser>>();
        SignInManager<AppUser> signInManager = provider.GetRequiredService<SignInManager<AppUser>>();
        IHttpContextAccessor httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();

        AppUser user = await CreateUserAsync(userManager);
        LoginUserCommandHandler handler = new(userManager, signInManager);

        LoginResponseDto response = await handler.Handle(
            new LoginUserCommand
            {
                UserName = user.UserName!,
                Password = "Valid_password1!"
            },
            CancellationToken.None);

        Assert.Equal(user.Id, response.UserId);
        Assert.False(httpContextAccessor.HttpContext.Response.Headers.ContainsKey("Set-Cookie"));
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        await using ServiceProvider provider = IdentityTestServices.CreateProvider();
        await provider.GetRequiredService<RealtimeChatDbContext>().Database.EnsureCreatedAsync();

        UserManager<AppUser> userManager = provider.GetRequiredService<UserManager<AppUser>>();
        SignInManager<AppUser> signInManager = provider.GetRequiredService<SignInManager<AppUser>>();
        await CreateUserAsync(userManager);
        LoginUserCommandHandler handler = new(userManager, signInManager);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(
            new LoginUserCommand
            {
                UserName = "test.user",
                Password = "Wrong_password1!"
            },
            CancellationToken.None));
    }

    private static async Task<AppUser> CreateUserAsync(UserManager<AppUser> userManager)
    {
        AppUser user = new()
        {
            UserName = "test.user",
            Email = "test.user@example.invalid",
            EmailConfirmed = true
        };

        IdentityResult result = await userManager.CreateAsync(user, "Valid_password1!");
        Assert.True(result.Succeeded, string.Join(", ", result.Errors.Select(error => error.Description)));

        return user;
    }
}
