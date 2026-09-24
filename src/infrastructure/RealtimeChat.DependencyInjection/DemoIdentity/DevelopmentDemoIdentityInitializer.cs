using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RealtimeChat.Domain.Entities.Concretes;

namespace RealtimeChat.DependencyInjection.DemoIdentity;

public sealed class DevelopmentDemoIdentityInitializer
{
    private const string MemberRole = "Member";

    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IHostEnvironment _environment;
    private readonly DemoIdentityOptions _options;

    public DevelopmentDemoIdentityInitializer(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IHostEnvironment environment,
        IOptions<DemoIdentityOptions> options)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _environment = environment;
        _options = options.Value;
    }

    public async Task InitializeAsync()
    {
        if (!_options.Enabled)
            return;

        // Demo hesabı yalnızca yerel akış içindir; yanlış ortamda sessizce kullanıcı üretmemeli.
        if (!_environment.IsDevelopment())
            throw new InvalidOperationException(
                "Demo identity can only be enabled in Development.");

        ValidateConfiguration();
        await EnsureMemberRoleAsync();

        AppUser? user = await _userManager.FindByNameAsync(_options.UserName);
        if (user is null)
        {
            user = new AppUser
            {
                UserName = _options.UserName,
                Email = _options.Email,
                EmailConfirmed = true
            };

            EnsureSucceeded(
                await _userManager.CreateAsync(user, _options.Password),
                "Demo user could not be created.");
        }
        else if (!string.Equals(user.Email, _options.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The configured demo username belongs to a different email address.");
        }

        if (!await _userManager.IsInRoleAsync(user, MemberRole))
        {
            EnsureSucceeded(
                await _userManager.AddToRoleAsync(user, MemberRole),
                "Demo user could not be assigned to the Member role.");
        }
    }

    private async Task EnsureMemberRoleAsync()
    {
        if (await _roleManager.RoleExistsAsync(MemberRole))
            return;

        EnsureSucceeded(
            await _roleManager.CreateAsync(new AppRole { Name = MemberRole }),
            "Member role could not be created.");
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_options.UserName)
            || string.IsNullOrWhiteSpace(_options.Email)
            || string.IsNullOrWhiteSpace(_options.Password))
        {
            throw new InvalidOperationException(
                "DemoIdentity requires UserName, Email, and Password when enabled.");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string message)
    {
        if (result.Succeeded)
            return;

        string errors = string.Join(
            ", ",
            result.Errors.Select(error => error.Description));

        throw new InvalidOperationException($"{message} {errors}");
    }
}
