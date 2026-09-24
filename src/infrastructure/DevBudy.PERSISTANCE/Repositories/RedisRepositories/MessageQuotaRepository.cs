using DevBudy.APPLICATION.Services.RedisServices;
using DevBudy.CONTRACT.Repositories.RedisRepositories;
using DevBudy.DOMAIN.CachingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.PERSISTANCE.Repositories.RedisRepositories
{
    public class MessageQuotaRepository : RedisRepository<MessageQuota>, IMessageQuotaRepository
    {
        private const int DefaultMessageCount = 0;
        public MessageQuotaRepository(IRedisConnectionProvider redisConnectionProvider) : base(redisConnectionProvider)
        {

        }

        public string GetQuotaKey(string userId)
        {
            return $"chat:quota:{userId}";
        }

        public async Task InitializeUserMessageQuotaAsync(string userId)
        {
            string key = GetQuotaKey(userId);
            bool exists = await _db.KeyExistsAsync(key);
            if (!exists)
            {
                int offset = Math.Abs(userId.GetHashCode()) % 30;
                TimeSpan expireTime = TimeSpan.FromHours(1).Add(TimeSpan.FromSeconds(offset));
                MessageQuota quota = new(DefaultMessageCount, expireTime);
                await SetAsync(key, quota, quota.ExpireAt);
                await _db.SetAddAsync("chat:quota-user-ids", userId);
            }
        }
        public async Task<MessageQuota> GetQuotaAsync(string userId)
        {
            MessageQuota quota = await GetAsync(userId);
            return quota;
        }

        public async Task SetQuotaAsync(string userId, MessageQuota quota)
        {
            await SetAsync(userId, quota, quota.ExpireAt);
        }

        public async Task<bool> TryConsumeAsync(string userId)
        {
            MessageQuota quota = await GetQuotaAsync(userId);
            if (quota == null || quota.ReaminingMessage <= 0)
                return false;

            string key = GetQuotaKey(userId);
            quota = new MessageQuota(quota.ReaminingMessage - 1, quota.ExpireAt);
            long score = DateTimeOffset.Now.ToUnixTimeSeconds();
            await SetAsync(key, quota, quota.ExpireAt);
            return true;
        }
    }
}
