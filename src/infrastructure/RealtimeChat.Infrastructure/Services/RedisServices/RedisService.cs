using RealtimeChat.Application.Services.RedisServices;
using RealtimeChat.Contracts.Repositories.RedisRepositories;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RealtimeChat.Infrastructure.Services.RedisServices
{
    public class RedisService<T> : IRedisService<T> where T : class
    {
        private readonly IRedisRepository<T> _redisRepository;

        public RedisService(IRedisRepository<T> redisRepository)
        {
            _redisRepository = redisRepository;
        }

        public Task<T> GetAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            return _redisRepository.GetAsync(key);
        }

        public Task<bool> RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            return _redisRepository.RemoveAsync(key);
        }

        public Task SetAsync(string key, T value, TimeSpan? expiry = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty.");
            else if (value == null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            return _redisRepository.SetAsync(key, value, expiry);
        }
    }
}
