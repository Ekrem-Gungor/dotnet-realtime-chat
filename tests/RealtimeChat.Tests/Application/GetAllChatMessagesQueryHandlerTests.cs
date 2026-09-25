using RealtimeChat.Application.Features.Chats.Dtos;
using RealtimeChat.Application.Features.Chats.Queries;
using RealtimeChat.Contracts.Repositories.RedisRepositories;
using RealtimeChat.Domain.CachingModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealtimeChat.Tests.Application
{
    public sealed class GetAllChatMessagesQueryHandlerTests
    {
        [Fact]
        public async Task Handle_MapsCachedMessagesToApplicationDtos()
        {
            Guid messageId = Guid.Parse("f9f53075-fbba-4f23-a817-0a4e76ea17e1");

            DateTime createdAt = new(2026, 9, 25, 12, 0, 0, DateTimeKind.Utc);

            StubMessageRepository repository = new([new RedisChatMessage
            {
                MessageId = messageId,
                SenderUserId = 4,
                SenderUserName = "realtime.demo",
                Message = "Cached message",
                CreatedAt = createdAt
            }]);

            GetAllChatMessagesQueryHandler handler = new(repository);

            List<ChatMessageDto> result = await handler.Handle(new GetAllChatMessagesQuery(), CancellationToken.None);

            Assert.Equal(30, repository.ReceivedMinutes);

            ChatMessageDto message = Assert.Single(result);

            Assert.Equal(messageId, message.Id);
            Assert.Equal(4, message.SenderUserId);
            Assert.Equal("realtime.demo", message.SenderUserName);
            Assert.Equal("Cached message", message.Message);
            Assert.Equal(createdAt, message.SendAt);
        }

        private sealed class StubMessageRepository : IMessageRedisRepository
        {
            private readonly List<RedisChatMessage> _messages;

            public StubMessageRepository(List<RedisChatMessage> messages)
            {
                _messages = messages;
            }

            public int? ReceivedMinutes { get; private set; }

            public Task AddMessageAsync(RedisChatMessage message)
            {
                return Task.CompletedTask;
            }

            public Task<List<RedisChatMessage>> GetMessagesFromLastMinutesAsync(int minutes)
            {
                ReceivedMinutes = minutes;
                return Task.FromResult(_messages);
            }
        }
    }
}
