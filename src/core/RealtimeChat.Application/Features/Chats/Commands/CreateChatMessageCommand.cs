using MediatR;
using RealtimeChat.Application.Features.Chats.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Features.Chats.Commands
{
    public class CreateChatMessageCommand : IRequest<ChatMessageDto>
    {
        public string SenderUserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
