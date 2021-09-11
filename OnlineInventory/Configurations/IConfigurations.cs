using System;

namespace OnlineInventory
{
    public interface IConfigurations
    {
        int Interval { get; set; }
        /// <summary>
        /// Paging for API request / response
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// Prism - DB server name
        /// </summary>
        string dbName { get; set; }

        /// <summary>
        /// Prism - Store code
        /// </summary>
        string pickupStore { get; set; }

        DateTime? LastSyncDateTime { get; set; } //(Properties.Settings.Default.LastSyncDateTime == new DateTime() ? null : (DateTime?)Properties.Settings.Default.LastSyncDateTime);

        bool EnableShopee { get; set; }

        bool EnableLazada { get; set; }

        string OnlineFilePath { get; set; }
        //bool UseKeepOfflineFile { get; set; }
        //bool IsReservedStock { get; set; }


        bool IsShopeeStore { get; set; }
        bool IsLazadaStore { get; set; }

        //string AccessToken { get; }

        //string RefreshToken { get;}

        //#region SHOPEE
        ///// <summary>
        ///// Shopee - Shop ID
        ///// </summary>
        //int ShopeeShopId { get; set; }
        ///// <summary>
        ///// Shopee - Partner ID
        ///// </summary>
        //int ShopeePartnerId { get; set; }
        ///// <summary>
        ///// Shopee - API Secret Key
        ///// </summary>
        //string ShopeeSecretKey { get; set; }
        ///// <summary>
        ///// Shopee - Base domain
        ///// </summary>
        //string ShopeeDomain { get; set; }

        ///// <summary>
        ///// Shopee - Variation SKU Format 
        ///// </summary>
        //string ShopeeVariationSKUFormat { get; set; }

        ///// <summary>
        ///// Shopee - Variation SKU Format 
        ///// </summary>
        //string ShopeeVariationSKUSeparator { get; set; }
        //#endregion

        //#region LAZADA
        ///// <summary>
        ///// Lazada - App key
        ///// </summary>
        //string LazadaAppKey { get; set; }
        ///// <summary>
        ///// Lazada - App secret
        ///// </summary>
        //string LazadaAppSecret { get; set; }
        ///// <summary>
        ///// Lazada - Domain
        ///// </summary>
        //string LazadaDomain { get; set; }
        ///// <summary>
        ///// Lazada - Access token
        ///// </summary>
        //string LazadaAccessToken { get; set; }
        ///// <summary>
        ///// Lazada - Refresh token
        ///// </summary>
        //string LazadaRefreshToken { get; set; }

        ///// <summary>
        ///// Lazada - Có sử dụng nhiều warehouse không
        ///// </summary>
        //bool UseMultiWarehouse { get; set; }

        ///// <summary>
        ///// Lazada - Mã warehouse
        ///// </summary>
        //string WarehouseCode { get; set; }

        ///// <summary>
        ///// Cho timer chạy hay không
        ///// </summary>
        //Boolean RunAsTimer { get; set; }

        //#endregion

        //void UpdateLazadaAccessToken(string path, string newAccessToken);

        //void UpdateLazadaRefreshToken(string path, string newRefreshToken);

        //void UpdateLastSyncDateTime(DateTime syncDateTime);

        //void UpdateSyncTime(int syncTime);
    }
}
