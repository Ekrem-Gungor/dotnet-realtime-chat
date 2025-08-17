
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Services.RedisServices
{
    public interface IRedisConnectionProvider
    {
        IDatabase GetDatabase();
    }
}
