using System.Threading.Tasks;

namespace OnlineInventoryLib
{
    public interface IShopeeLib
    {
        /// <summary>
        /// Tạo link authorization 
        /// </summary>
        /// <param name="url">Shopee URL</param>
        /// <param name="id">Shopee App partner ID</param>
        /// <param name="key">Shopee App partner key</param>
        /// <param name="redirect">Call back webhook</param>
        /// <returns></returns>
        string GetAuthorizationURL(string url, string id, string key, string redirect);


        /// <summary>
        /// Gọi async API cập nhật stock nhiều variation ( UPC trong Prism )
        /// </summary>
        /// <typeparam name="T">Generic type - Response object type</typeparam>
        /// <typeparam name="R">Generic type - Request object type</typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<T> UpdateVariationStockBatch<T, R>(R request);

        /// <summary>
        /// Gọi async API lấy danh sách item trên Shopee
        /// </summary>
        /// <typeparam name="T">Generic type - Response object type</typeparam>
        /// <typeparam name="R">Generic type - Request object type</typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<T> GetItemList<T, R>(R request);


        /// <summary>
        /// Gọi async API lấy chi tiết item trên Shopee
        /// </summary>
        /// <typeparam name="T">Generic type - Response object type</typeparam>
        /// <typeparam name="R">Generic type - Request object type</typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<T> GetItemDetail<T, R>(R request);


        Task<T> GetOrdersByStatus<T, R>(R request);
    }
}
