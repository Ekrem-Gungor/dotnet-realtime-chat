using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Domain.Entities.Concretes;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.AppUsers.Commands
{
    public class SetUserOnlineStatusCommandHandler : IRequestHandler<SetUserOnlineStatusCommand, bool>
    {
        private readonly UserManager<AppUser> _userManager;
        public SetUserOnlineStatusCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> Handle(SetUserOnlineStatusCommand request, CancellationToken cancellationToken)
        {
            AppUser? appUser = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (appUser == null)
            {
                throw new ArgumentException("User not found.", nameof(request.UserId));
            }
            appUser.IsOnline = request.IsOnline;
            if (request.LastLogin.HasValue)
                appUser.LastLogin = request.LastLogin.Value;

            if (request.LastLogout.HasValue)
                appUser.LastLogout = request.LastLogout.Value;

            IdentityResult result = await _userManager.UpdateAsync(appUser);
            if (result.Succeeded)
                return true;

            throw new InvalidOperationException("Kullanıcı durumu güncellenemedi.");

        }
    }
}
