using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMAPI.Repositories
{
    interface IRepository<TEntity, TDto>
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> Get(long id);
        Task<TDto> GetDto(long id);
        Task<bool> Add(TEntity entity);
        void Update(TEntity entityToUpdate, TEntity entity);
        void Delete(TEntity entity);
    }
}
