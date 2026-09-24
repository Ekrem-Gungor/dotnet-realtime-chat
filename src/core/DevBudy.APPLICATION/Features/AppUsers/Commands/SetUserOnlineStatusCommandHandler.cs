using DevBudy.APPLICATION.Features.Auths.Dtos.Response;
using DevBudy.DOMAIN.Entities.Concretes;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.AppUsers.Commands
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
            AppUser appUser = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (appUser == null)
            {
                throw new ArgumentException("User not found.", nameof(request.UserId));
            }
            appUser.IsOnline = request.IsOnline;
            appUser.LastLogin = request.LastLogin;
            appUser.LastLogout = request.LastLogout;
            IdentityResult result = await _userManager.UpdateAsync(appUser);
            if (result.Succeeded) return true;
            else throw new Exception("Durum güncellemesi başarısız oldu!");

        }
    }
}
