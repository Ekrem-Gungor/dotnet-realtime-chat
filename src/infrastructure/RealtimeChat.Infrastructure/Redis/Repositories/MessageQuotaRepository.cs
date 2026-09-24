using RealtimeChat.Contracts.Repositories.RedisRepositories;
using StackExchange.Redis;

namespace RealtimeChat.Infrastructure.Redis.Repositories;

public sealed class MessageQuotaRepository : IMessageQuotaRepository
{
    private const int DefaultMessageLimit = 30;
    private const int QuotaWindowSeconds = 60 * 60;

    private const string ConsumeQuotaScript = """
        local current = redis.call('GET', KEYS[1])

        if not current then
            redis.call('SET', KEYS[1], tonumber(ARGV[1]) - 1, 'EX', ARGV[2])
            return 1
        end

        if tonumber(current) <= 0 then
            return 0
        end

        redis.call('DECR', KEYS[1])
        return 1
        """;

    private readonly IDatabase _database;

    public MessageQuotaRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<bool> TryConsumeAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        RedisKey key = $"chat:quota:{userId}";

        // Kontrol ve azaltma tek Redis çağrısında yapılır; eş zamanlı istekler aynı hakkı tüketemez.
        RedisResult result = await _database.ScriptEvaluateAsync(
            ConsumeQuotaScript,
            [key],
            [DefaultMessageLimit, QuotaWindowSeconds]);

        return (long)result == 1;
    }
}
