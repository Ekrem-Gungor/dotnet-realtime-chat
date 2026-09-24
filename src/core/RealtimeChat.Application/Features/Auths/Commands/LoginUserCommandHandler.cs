using MediatR;
using Microsoft.AspNetCore.Identity;
using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Domain.Entities.Concretes;

namespace RealtimeChat.Application.Features.Auths.Commands;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public LoginUserCommandHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<LoginResponseDto> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByNameAsync(request.UserName);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid username or password.");

        // JWT ayrı üretildiği için burada Identity application cookie oluşturmuyoruz.
        SignInResult result = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Invalid username or password.");

        return new LoginResponseDto { UserId = user.Id };
    }
}
