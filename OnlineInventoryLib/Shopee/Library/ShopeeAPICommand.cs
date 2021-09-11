using Newtonsoft.Json;
using OnlineInventoryLib.Util;
using System.Threading.Tasks;

namespace OnlineInventoryLib.Shopee
{
    public class ShopeeAPICommand : IShopeeLib
    {
        public string secretKey;
        public string ShopeeDomain;
        private readonly ShopeeBaseLib lib;

        public ShopeeAPICommand(string shopeeDomain, string secretKey)
        {
            this.ShopeeDomain = shopeeDomain;
            lib = new ShopeeBaseLib(secretKey);
        }

        /// <summary>
        /// Hàm xử lý post data qua SHOPEE API
        /// </summary>
        /// <typeparam name="TRes">response data type</typeparam>
        /// <typeparam name="TReq">request data type</typeparam>
        /// <param name="url">url string</param>
        /// <param name="rurns>request">request object</param>
        /// <returns></ret
        private async Task<TRes> PostData<TRes, TReq>(string url, TReq request)
        {
            string result = await lib.PostData(url, APIUtil.ConvertToJson(request));
            TRes response = JsonConvert.DeserializeObject<TRes>(result);
            return response;
        }


        public string GetAuthorizationURL(string url, string id, string key, string redirect)
        {
            return lib.CalculateToken(url, id, key, redirect);
        }


        public async Task<T> GetOrderDetails<T, R>(R request)
        {
            return await PostData<T, R>(ShopeeDomain + "/orders/detail", request);
        }

        public async Task<T> GetOrdersByStatus<T, R>(R request)
        {
            return await PostData<T, R>(ShopeeDomain + "/orders/get", request);
        }

        public async Task<T> GetOrdersList<T, R>(R request)
        {
            return await PostData<T, R>(ShopeeDomain + "/orders/basics", request);
        }


        public async Task<T> GetShopInfo<T, R>(R request)
        {
            return await PostData<T, R>(ShopeeDomain + "/shop/get", request);
        }

        public async Task<T> UpdateStock<T, R>(R request)
        {
            return await PostData<T, R>(ShopeeDomain + "/items/update_stock", request);
        }

        public async Task<T> UpdateStockBatch<T, R>(R request)
        {
            return await PostData<T, R>(ShopeeDomain + "/items/update/items_stock", request);
        }

        public async Task<T> GetItemList<T, R>(R request)
        {
            return await PostData<T, R>(ShopeeDomain + "/items/get", request);
        }

        public async Task<T> GetItemDetail<T, R>(R request)
        {
            return await PostData<T, R>(ShopeeDomain + "/item/get", request);
        }

        public async Task<T> UpdateVariationStockBatch<T, R>(R request)
        {
            return await PostData<T, R>(ShopeeDomain + "/items/update/vars_stock", request);
        }
    }
}
