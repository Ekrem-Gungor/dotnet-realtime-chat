using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Contracts.Repositories.RedisRepositories
{
    public interface IRedisRepository<R> where R : class
    {
        /// <summary>
        /// İsteğe bağlı bir son kullanma süresi ile Redis önbelleğinde bir değer saklar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        Task SetAsync(string key, R value, TimeSpan? expiry = null);

        /// <summary>
        /// Redis önbelleğinden belirtilen anahtarla ilişkili değeri alır.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<R> GetAsync(string key);

        /// <summary>
        /// Redis önbelleğinden tüm değerleri alır.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<R>> GetAllAsync(string prefixKey);

        /// <summary>
        /// Redis önbelleğinden belirtilen anahtarla ilişkili değeri kaldırır.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string key);
    }
}
