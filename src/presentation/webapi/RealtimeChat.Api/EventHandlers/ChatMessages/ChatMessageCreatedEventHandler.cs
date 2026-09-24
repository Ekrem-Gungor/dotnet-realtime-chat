using RealtimeChat.Api.Hubs;
using RealtimeChat.Application.Events.ChatMessages;
using RealtimeChat.Application.Features.Chats.Dtos;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace RealtimeChat.Api.EventHandlers.ChatMessages
{
    public class ChatMessageCreatedEventHandler : INotificationHandler<ChatMessageCreatedEvent>
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatMessageCreatedEventHandler(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(ChatMessageCreatedEvent notification, CancellationToken cancellationToken)
        {
            ChatMessageDto message = notification.ChatMessage;
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", new
            {
                message.Message,
                message.SenderUserName,
                message.SendAt
            });
        }
    }
}
