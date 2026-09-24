using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Domain.Entities.Concretes;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.Auths.Commands
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public LoginUserCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            AppUser? user = await _userManager.FindByNameAsync(request.UserName);
            if (user is null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            SignInResult result = await _signInManager.PasswordSignInAsync(user, request.Password, true, true);
            if (!result.Succeeded) throw new UnauthorizedAccessException("Invalid username or password.");

            LoginResponseDto loginResponseDto = new() { UserId = user.Id };

            return loginResponseDto;
        }
    }
}
