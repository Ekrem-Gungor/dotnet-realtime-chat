using RealtimeChat.Domain.CachingModels;
using RealtimeChat.Infrastructure.Redis.Repositories;

namespace RealtimeChat.Tests.Infrastructure;

[Collection(RedisCollection.Name)]
public sealed class RedisRepositoryTests
{
    private const string MessagesKey = "chat:messages";
    private readonly RedisFixture _fixture;

    public RedisRepositoryTests(RedisFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task TryConsumeAsync_AllowsExactlyThirtyConcurrentMessages()
    {
        string userId = Guid.NewGuid().ToString("N");
        string quotaKey = $"chat:quota:{userId}";
        MessageQuotaRepository repository = new(_fixture.Connection);

        try
        {
            Task<bool>[] consumeTasks = Enumerable
                .Range(0, 50)
                .Select(_ => repository.TryConsumeAsync(userId))
                .ToArray();

            bool[] results = await Task.WhenAll(consumeTasks);

            Assert.Equal(30, results.Count(consumed => consumed));
            Assert.Equal(20, results.Count(consumed => !consumed));
        }
        finally
        {
            await _fixture.Database.KeyDeleteAsync(quotaKey);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetMessagesFromLastMinutesAsync_ReturnsOnlyMessagesInsideWindow()
    {
        await _fixture.Database.KeyDeleteAsync(MessagesKey);
        RedisMessageRepository repository = new(_fixture.Connection);

        RedisChatMessage oldMessage = CreateMessage(DateTime.UtcNow.AddMinutes(-31));
        RedisChatMessage recentMessage = CreateMessage(DateTime.UtcNow.AddMinutes(-1));

        try
        {
            await repository.AddMessageAsync(oldMessage);
            await repository.AddMessageAsync(recentMessage);

            List<RedisChatMessage> messages =
                await repository.GetMessagesFromLastMinutesAsync(30);

            Assert.Contains(messages, message => message.MessageId == recentMessage.MessageId);
            Assert.DoesNotContain(messages, message => message.MessageId == oldMessage.MessageId);
        }
        finally
        {
            await _fixture.Database.KeyDeleteAsync(MessagesKey);
        }
    }

    [Fact]
    public async Task GetMessagesFromLastMinutesAsync_RejectsInvalidWindow()
    {
        RedisMessageRepository repository = new(_fixture.Connection);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => repository.GetMessagesFromLastMinutesAsync(0));
    }

    private static RedisChatMessage CreateMessage(DateTime createdAt)
    {
        return new RedisChatMessage
        {
            MessageId = Guid.NewGuid(),
            Message = "Test message",
            SenderUserName = "test-user",
            CreatedAt = createdAt
        };
    }
}
