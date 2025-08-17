using DevBudy.APPLICATION.Services.RedisServices;
using DevBudy.CONTRACT.Repositories.RedisRepositories;
using DevBudy.DOMAIN.CachingModels;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.INNERINFRASTRUCTURE.Services.RedisServices
{
    public class MessageQuotaService : RedisService<MessageQuota>, IMessageQuotaService
    {
        private readonly IMessageQuotaRepository _msgQuotaRepo;
        private readonly IDatabase _redisDb;
        public MessageQuotaService(IMessageQuotaRepository msgQuotaRepo, IConnectionMultiplexer redis) : base(msgQuotaRepo)
        {
            _msgQuotaRepo = msgQuotaRepo;
            _redisDb = redis.GetDatabase();
        }

        public async Task<IEnumerable<string>> GetAllQuotaKeysAsync()
        {
            var userIds = await _redisDb.SetMembersAsync("chat:quota-user-ids");
            return userIds.Select(id => _msgQuotaRepo.GetQuotaKey(id.ToString()));
        }

        public async Task<TimeSpan?> GetKeyTtlAsync(string key)
        {
            return await _redisDb.KeyTimeToLiveAsync(key);
        }

        public async Task<MessageQuota> GetQuotaAsync(string userId)
        {
            return await _msgQuotaRepo.GetQuotaAsync(userId);
        }

        public Task InitializeUserMessageQuotaAsync(string userId)
        {
            // Kullanıcı register olduğu zaman bu metot çağrılacak.
            return _msgQuotaRepo.InitializeUserMessageQuotaAsync(userId);
        }

        // Bunu test etmedim henüz...
        public async Task ResetQuotaIfExpiredAsync(string userId)
        {
            MessageQuota quota = await _msgQuotaRepo.GetQuotaAsync(userId);
            if (quota == null) return;
            
            if (!TimeSpan.TryParse(quota.ExpireAt.ToString(), out var expireTime)) return;

            string key = _msgQuotaRepo.GetQuotaKey(userId);
            var ttl = await _redisDb.KeyTimeToLiveAsync(key);

            if (ttl == null || ttl <= TimeSpan.Zero)
            {
                MessageQuota newQuota = new()
                {
                    ReaminingMessage = 30,
                    ExpireAt = quota.ExpireAt
                };

                await SetQuotaAsync(userId, newQuota);

                await _redisDb.KeyExpireAsync(key, expireTime);
            }
        }

        public async Task SetQuotaAsync(string userId, MessageQuota quota)
        {
            await _msgQuotaRepo.SetQuotaAsync(userId, quota);
        }

        public Task<bool> TryConsumeAsync(string userId)
        {
            return _msgQuotaRepo.TryConsumeAsync(userId);
        }
    }
}
