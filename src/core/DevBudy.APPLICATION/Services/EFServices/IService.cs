using DevBudy.DOMAIN.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Services.EFServices
{
    public interface IService<D, T> where D : class, IEntity where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(params object[] id);
        Task<T> FirstOrDefaultAsync(Expression<Func<D, bool>> exp);
        Task<bool> AnyAsync(Expression<Func<D, bool>> exp);
    }
}
