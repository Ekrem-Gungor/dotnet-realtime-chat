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

namespace DevBudy.APPLICATION.Features.Auths.Queries
{
    public class OnlineUsersQueryHandler : IRequestHandler<OnlineUsersQuery, List<ConnectedUserDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        public OnlineUsersQueryHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<ConnectedUserDto>> Handle(OnlineUsersQuery request, CancellationToken cancellationToken)
        {
            List<ConnectedUserDto> connectedUsers = await _userManager.Users
                .Select(user => new ConnectedUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    IsOnline = user.IsOnline
                }).ToListAsync();
            return connectedUsers;
        }
    }
}
