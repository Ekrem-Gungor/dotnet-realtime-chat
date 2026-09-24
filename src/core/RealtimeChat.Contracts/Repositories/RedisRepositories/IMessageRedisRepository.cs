using RealtimeChat.Domain.CachingModels;

namespace RealtimeChat.Contracts.Repositories.RedisRepositories;

public interface IMessageRedisRepository
{
    Task AddMessageAsync(RedisChatMessage message);
    Task<List<RedisChatMessage>> GetMessagesFromLastMinutesAsync(int minutes);
}
