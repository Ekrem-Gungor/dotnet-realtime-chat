using RealtimeChat.Application.Services.RedisServices;
using RealtimeChat.Contracts.Repositories.RedisRepositories;
using RealtimeChat.Domain.CachingModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace RealtimeChat.Persistence.Repositories.RedisRepositories
{
    public class RedisMessageRepository : RedisRepository<RedisChatMessage>, IMessageRedisRepository
    {
        public RedisMessageRepository(IRedisConnectionProvider redisConnectionProvider) : base(redisConnectionProvider)
        {

        }

        public async Task<List<RedisChatMessage>> GetMessagesFromLastMinutesAsync(int minutes)
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();

            long from = now - (minutes * 60);

            RedisValue[] redisValues = await _db.SortedSetRangeByScoreAsync("messages", from, now);

            List<RedisChatMessage> redisResult = redisValues
                .Select(value => JsonSerializer.Deserialize<RedisChatMessage>(value))
                .Where(m => m != null)
                .ToList();
            return redisResult;
        }

        public async Task AddMessageToSortedSetAsync(string setKey, RedisChatMessage value, double score)
        {
            string jsonData = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            });
            await _db.SortedSetAddAsync(setKey, jsonData, score);
        }
    }
}

