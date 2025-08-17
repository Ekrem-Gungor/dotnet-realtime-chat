using AutoMapper;
using DevBudy.APPLICATION.Services.EFServices;
using DevBudy.CONTRACT.Repositories.EFRepositories;
using DevBudy.DOMAIN.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.INNERINFRASTRUCTURE.Services.EfServices
{
    public class BaseService<D, T> : IService<D, T> where D : class, IEntity where T : class
    {
        readonly IRepository<D> _iRep;
        protected readonly IMapper _mapper;
        public BaseService(IRepository<D> iRep, IMapper mapper)
        {
            _iRep = iRep;
            _mapper = mapper;
        }

        protected D DtoToDomainMapping(T dtoItem)
        {
            return _mapper.Map<D>(dtoItem);
        }

        protected T DomainToDtoMapping(D domainItem)
        {
            return _mapper.Map<T>(domainItem);
        }

        public async Task<bool> AnyAsync(Expression<Func<D, bool>> exp)
        {
            return await _iRep.AnyAsync(exp);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<D, bool>> exp)
        {
            D value = await _iRep.FirstOrDefaultAsync(exp);
            return DomainToDtoMapping(value);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            IEnumerable<D> entities = await _iRep.GetAllAsync();
            return _mapper.Map<IEnumerable<T>>(entities);
        }

        public async Task<T> GetByIdAsync(params object[] id)
        {
            D entity = await _iRep.GetByIdAsync(id);
            return DomainToDtoMapping(entity);
        }
    }
}
