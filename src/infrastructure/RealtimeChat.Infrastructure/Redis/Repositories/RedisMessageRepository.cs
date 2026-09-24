using RealtimeChat.Contracts.Repositories.RedisRepositories;
using RealtimeChat.Domain.CachingModels;
using StackExchange.Redis;
using System.Text.Json;

namespace RealtimeChat.Infrastructure.Redis.Repositories;

public sealed class RedisMessageRepository : IMessageRedisRepository
{
    private const string MessagesKey = "chat:messages";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly IDatabase _database;

    public RedisMessageRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task AddMessageAsync(RedisChatMessage message)
    {
        string json = JsonSerializer.Serialize(message, SerializerOptions);
        long score = new DateTimeOffset(message.CreatedAt).ToUnixTimeMilliseconds();

        await _database.SortedSetAddAsync(MessagesKey, json, score);
    }

    public async Task<List<RedisChatMessage>> GetMessagesFromLastMinutesAsync(int minutes)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(minutes, 0);

        DateTimeOffset now = DateTimeOffset.UtcNow;
        long to = now.ToUnixTimeMilliseconds();
        long from = now.AddMinutes(-minutes).ToUnixTimeMilliseconds();
        RedisValue[] values = await _database.SortedSetRangeByScoreAsync(MessagesKey, from, to);

        return values
            .Select(value => JsonSerializer.Deserialize<RedisChatMessage>(value.ToString(), SerializerOptions))
            .OfType<RedisChatMessage>()
            .ToList();
    }
}
