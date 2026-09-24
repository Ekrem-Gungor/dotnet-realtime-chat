using RealtimeChat.Application.Features.Chats.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Application.Events.ChatMessages
{
    public class SystemMessageCreateEvent : INotification
    {
        public SystemMessageDto SystemMessage { get; }
        public SystemMessageCreateEvent(SystemMessageDto systemMessage)
        {
            SystemMessage = systemMessage;
        }
    }
}
