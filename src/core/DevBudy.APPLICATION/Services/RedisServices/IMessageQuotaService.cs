using DevBudy.DOMAIN.CachingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Services.RedisServices
{
    public interface IMessageQuotaService : IRedisService<MessageQuota>
    {
        Task<bool> TryConsumeAsync(string userId);
        Task<MessageQuota> GetQuotaAsync(string userId);
        Task<IEnumerable<string>> GetAllQuotaKeysAsync();
        Task SetQuotaAsync(string userId, MessageQuota quota);
        Task InitializeUserMessageQuotaAsync(string userId);
        Task ResetQuotaIfExpiredAsync(string userId);
        Task<TimeSpan?> GetKeyTtlAsync(string key);
    }
}
