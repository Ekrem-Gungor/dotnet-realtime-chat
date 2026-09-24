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

namespace RealtimeChat.Application.Features.Auths.Queries
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
                    UserName = user.UserName ?? string.Empty,
                    IsOnline = user.IsOnline
                }).ToListAsync();
            return connectedUsers;
        }
    }
}
