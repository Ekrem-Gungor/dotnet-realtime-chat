using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Services.RedisServices
{
    public interface IRedisService<T> where T : class
    {
        /// <summary>
        /// İsteğe bağlı bir son kullanma süresi ile Redis önbelleğinde bir değer saklar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        Task SetAsync(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// Redis önbelleğinden belirtilen anahtarla ilişkili değeri alır.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync(string key);

        /// <summary>
        /// Redis önbelleğinden belirtilen anahtarla ilişkili değeri kaldırır.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string key);
    }
}
