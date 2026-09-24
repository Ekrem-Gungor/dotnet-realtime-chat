using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.AppUsers.Commands
{
    public class SetUserOnlineStatusCommand : IRequest<bool>
    {
        public int UserId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime LastLogout { get; set; }
    }
}
