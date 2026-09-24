using DevBudy.CONTRACT.Repositories.EFRepositories;
using DevBudy.DOMAIN.Entities.Abstracts;
using DevBudy.PERSISTANCE.ContextClasses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.PERSISTANCE.Repositories.EFRepositories
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly DevBudyContext _context;

        public Repository(DevBudyContext context)
        {
            _context = context;
        }

        protected async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> exp)
        {
            return await _context.Set<T>().AnyAsync(exp);
        }

        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await SaveAsync();
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> exp)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(exp);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(params object[] id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            T originalEntity = await GetByIdAsync(entity.ID);
            _context.Set<T>().Entry(originalEntity).CurrentValues.SetValues(entity);
            await SaveAsync();
        }
    }
}
