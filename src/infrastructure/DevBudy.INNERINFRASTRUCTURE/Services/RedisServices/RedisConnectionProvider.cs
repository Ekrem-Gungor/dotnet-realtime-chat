using DevBudy.APPLICATION.Services.RedisServices;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.INNERINFRASTRUCTURE.Services.RedisServices
{
    public class RedisConnectionProvider : IRedisConnectionProvider
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisConnectionProvider(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.GetDatabase();
        }
    }
}
