using DevBudy.API.Hubs;
using DevBudy.APPLICATION.Events.ChatMessages;
using DevBudy.APPLICATION.Features.Chats.Dtos;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace DevBudy.API.EventHandlers.ChatMessages
{
    public class SystemMessageCreateEventHandler : INotificationHandler<SystemMessageCreateEvent>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public SystemMessageCreateEventHandler(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(SystemMessageCreateEvent notification, CancellationToken cancellationToken)
        {
            SystemMessageDto message = notification.SystemMessage;
            await _hubContext.Clients.All.SendAsync("ReceiveSystemMessage", new
            {
                message.JoinedUserName,
                message.SenderUserName,
                message.Message,
                message.SendAt
            });
        }
    }
}
