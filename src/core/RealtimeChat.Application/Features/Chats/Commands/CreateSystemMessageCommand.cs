using RealtimeChat.Application.Features.Chats.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.Chats.Commands
{
    public class CreateSystemMessageCommand : IRequest<SystemMessageDto>
    {
        public string JoinedUserName { get; set; }
    }
}
