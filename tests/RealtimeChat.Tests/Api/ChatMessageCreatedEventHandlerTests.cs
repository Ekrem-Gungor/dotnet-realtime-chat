using Microsoft.AspNetCore.SignalR;
using Moq;
using RealtimeChat.Api.EventHandlers.ChatMessages;
using RealtimeChat.Api.Hubs;
using RealtimeChat.Application.Events.ChatMessages;
using RealtimeChat.Application.Features.Chats.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealtimeChat.Tests.Api
{
    public sealed class ChatMessageCreatedEventHandlerTests
    {
        [Fact]
        public async Task Handle_BroadcastsCompleteMessageContract()
        {
            ChatMessageDto message = new()
            {
                Id = Guid.Parse("f9f53075-fbba-4f23-a817-0a4e76ea17e1"),
                SenderUserId = 4,
                SenderUserName = "realtime.demo",
                Message = "SignalR contract test",
                SendAt = new DateTime(2026, 9, 26, 0, 0, 0, DateTimeKind.Utc)
            };

            Mock<IClientProxy> clientProxy = new();

            Mock<IHubClients> hubClients = new();
            hubClients.SetupGet(clients => clients.All).Returns(clientProxy.Object);

            Mock<IHubContext<ChatHub>> hubContext = new();
            hubContext.SetupGet(context => context.Clients).Returns(hubClients.Object);

            ChatMessageCreatedEventHandler handler = new(hubContext.Object);

            await handler.Handle(new ChatMessageCreatedEvent(message), CancellationToken.None);

            clientProxy.Verify(proxy => proxy.SendCoreAsync(ChatHubEvent.ReceiveMessage, It.Is<object?[]>
                (arguments => arguments.Length == 1 && ReferenceEquals(arguments[0], message)), CancellationToken.None), Times.Once);
        }
    }
}