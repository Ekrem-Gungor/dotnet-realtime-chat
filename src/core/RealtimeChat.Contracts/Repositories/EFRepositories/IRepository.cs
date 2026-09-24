using RealtimeChat.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Contracts.Repositories.EFRepositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(params object[] id);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> exp);
        Task<bool> AnyAsync(Expression<Func<T, bool>> exp);
    }
}
