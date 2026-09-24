using RealtimeChat.Application.Services.RedisServices;
using RealtimeChat.Contracts.Repositories.RedisRepositories;
using RealtimeChat.Domain.CachingModels;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RealtimeChat.Persistence.Repositories.RedisRepositories
{
    /// <summary>
    /// Redis ile veri saklama işlemlerini yöneten repository sınıfıdır.
    /// <para>
    /// Bu sınıf, Redis'e bağlanmak için dışarıdan bir <c>RedisConnectionProvider</c> üzerinden 
    /// <c>ConnectionMultiplexer</c> nesnesi alır. Bağlantı, DI (Dependency Injection) ile 
    /// singleton olarak enjekte edilmektedir.
    /// </para>
    /// <para>
    /// Redis, tam olarak bir veritabanı gibi davranmadığı için EF Core Entity modelleriyle doğrudan eşlenmez.
    /// Bu sınıf içerisinde kullanılan metotlar genellikle caching için oluşturulmuş,
    /// bağımsız <c>CachingModels</c> altında tutulmaktadır.
    /// </para>
    /// <para>
    /// Generic <c>SetAsync</c>, <c>GetAsync</c>, <c>RemoveAsync</c> gibi metotlar 
    /// cacheleme senaryoları için kullanılmak üzere geliştirilmiştir.
    /// </para>
    /// </summary>
    /// <typeparam name="R">Redis'e yazılacak model tipi. Genellikle <c>CachingModels</c> altındaki sınıflar.</typeparam>
    public class RedisRepository<R> : IRedisRepository<R> where R : class
    {
        protected readonly IDatabase _db;

        public RedisRepository(IRedisConnectionProvider redisConnectionProvider)
        {
            _db = redisConnectionProvider.GetDatabase();
        }

        public async Task SetAsync(string key, R value, TimeSpan? expiry = null)
        {
            string jsonData = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            });
            await _db.StringSetAsync(key, jsonData, expiry);
        }

        public async Task<R> GetAsync(string key)
        {
            RedisValue jsonData = await _db.StringGetAsync(key);
            if (jsonData.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<R>(jsonData);
        }

        public async Task<bool> RemoveAsync(string key)
        {
            return await _db.KeyDeleteAsync(key);
        }

        public async Task<IEnumerable<R>> GetAllAsync(string prefixKey)
        {
            string key = $"{prefixKey}:*";
            RedisValue json = await _db.StringGetAsync(key);
            return string.IsNullOrEmpty(json) ? Enumerable.Empty<R>() : JsonSerializer.Deserialize<List<R>>(json);
        }

    }
}
