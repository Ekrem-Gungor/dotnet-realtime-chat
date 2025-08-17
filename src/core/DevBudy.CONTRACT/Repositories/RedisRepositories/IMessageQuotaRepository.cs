using DevBudy.DOMAIN.CachingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.CONTRACT.Repositories.RedisRepositories
{
    public interface IMessageQuotaRepository : IRedisRepository<MessageQuota>
    {
        Task<bool> TryConsumeAsync(string userId);

        public string GetQuotaKey(string userId);

        Task<MessageQuota> GetQuotaAsync(string userId);

        Task SetQuotaAsync(string userId, MessageQuota quota);

        Task InitializeUserMessageQuotaAsync(string userId);
    }
}
