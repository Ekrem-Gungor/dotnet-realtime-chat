using RealtimeChat.Domain.CachingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Contracts.Repositories.RedisRepositories
{
    public interface IMessageRedisRepository : IRedisRepository<RedisChatMessage>
    {
        Task AddMessageToSortedSetAsync(string setKey, RedisChatMessage value, double score);
        Task<List<RedisChatMessage>> GetMessagesFromLastMinutesAsync(int minutes);
    }
}
