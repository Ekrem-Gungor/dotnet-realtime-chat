using DevBudy.API.Hubs;
using DevBudy.APPLICATION.Events.ChatMessages;
using DevBudy.APPLICATION.Features.Chats.Dtos;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace DevBudy.API.EventHandlers.ChatMessages
{
    public class ChatMessageCreatedEventHandler : INotificationHandler<ChatMessageCreatedEvent>
    {
        // Refactor : Core katmanda bir IChatPublisher interface tanımlanacak. API katmanında bu interface'İn SignalR implementasyonu olacak. EventHandler'lar bu interface üzerinden publish edecekler. Böylelikle EventHandler'lar hem Application katmanında kalır hem de SignalR'a bağımlı olmayacak ve test edilebilirlik artacak.
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
