using OnlineInventoryLib.Prism.Models;
using System;
using System.Collections.Generic;

namespace OnlineInventoryLib.Interfaces
{
    public interface IPrismLib
    {
        /// <summary>
        ///  Đọc custom list của brand từ database thay vì từ local file
        /// </summary>
        /// <returns></returns>
        Dictionary<string, int> LoadCustomListFromDB();

        /// <summary>
        /// Lấy tồn kho cửa hàng theo thời gian đồng bộ
        /// </summary>
        /// <param name="lastSync"></param>
        /// <returns></returns>
        Dictionary<string, int> GetOnhands(string pickupStore, DateTime? lastSync);

        /// <summary>
        /// Lấy tồn kho cửa hàng theo thời gian đồng bộ
        /// </summary>
        /// <param name="lastSync"></param>
        /// <returns></returns>
        Dictionary<string, int> GetMultiOnhands(string storeList, DateTime? lastSync);

        /// <summary>
        /// Kiểm tra trạng thái active của store trên bảng cáu hình
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="pickupStore">Pickup store đăng ký với platform</param>
        /// <returns></returns>
        OnlineStoreAuthenticate CheckActive(string pickupStore, string platform);

        /// <summary>
        /// Lấy tất cả thông tin của store Lazada
        /// </summary>
        /// <param name="pickupStore">Pickup store đăng ký với platform</param>
        /// <returns></returns>
        Online_store_Lazada GetLazadaAuthenticate(string pickupStore);

        /// <summary>
        /// Lấy tất cả thông tin của store Shopee
        /// </summary>
        /// <param name="pickupStore">Pickup store đăng ký với platform</param>
        /// <returns></returns>
        Online_Store_Shopee GetShopeeAuthenticate(string pickupStore);

        /// <summary>
        /// Cập nhật token cho store Lazada
        /// </summary>
        /// <param name="storeAuthenticate"></param>
        /// <returns></returns>
        bool UpdateAuthenticate(Online_store_Lazada storeAuthenticate);

        bool CreateAuthenticate(Online_store_Lazada storeAuthenticate);
    }
}
///// <summary>
///// Lấy tồn kho 1 upc theo thời gian đồng bộ
///// </summary>
///// <param name="upc">mã upc</param>
///// <param name="lastSync">lần đồng bộ trước đây</param>
///// <returns></returns>
//PrismQtyOnHand GetQtyOnHand(string upc, DateTime? lastSync = null);

///// <summary>
///// Lấy tồn kho cả cửa hàng theo thời gian đồng bộ
///// </summary>
///// <param name="lastSync">lần đồng bộ trước đây</param>
///// <returns></returns>
//List<PrismQtyOnHand> GetQtyOnHands(DateTime? lastSync);