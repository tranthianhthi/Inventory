using System.Collections.Generic;
using System.Threading.Tasks;


namespace OnlineInventoryAPI.Repositories.Interfaces
{
    public interface IOnlineStoreAuthenticateRepo<TEntity, TDto>
    {
        Task<TEntity> Get(string storeCode, string platform);
        void Update(TEntity entityToUpdate, TEntity entity);
    }
}
