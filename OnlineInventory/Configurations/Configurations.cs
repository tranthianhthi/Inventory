using System;

namespace OnlineInventory
{
    public enum OnlineStore
    {
        Lazada,
        Shopee,
        Tiki
    }

    public class Configurations : IConfigurations
    {
        
        public Configurations()
        {
            Properties.Settings.Default.Reload();
        }



        //public bool SendNotificationMail
        //{
        //    get { return Properties.Settings.Default.SendNotificationMail; }
        //}

        //public string EmailAddresses
        //{
        //    get { return Properties.Settings.Default.EmailAddresses; }
        //}


        /// <summary>
        /// Refresh interval
        /// </summary>
        public int Interval
        {
            get
            {
                return Properties.Settings.Default.SyncTime;
            }
            set { }
        }
        /// <summary>
        /// Paging for API request / response
        /// </summary>
        public int PageSize
        {
            get { return Properties.Settings.Default.PageSize; }
            set { }
        }

        /// <summary>
        /// Prism - DB server name
        /// </summary>
        public string dbName
        {
            get { return Properties.Settings.Default.DataSource; }
            set { }
        }

        /// <summary>
        /// Prism - Store code
        /// </summary>
        public string pickupStore
        {
            get { return Properties.Settings.Default.PickupStore; }
            set { }
        }

        public DateTime? LastSyncDateTime 
        {
            get { return null; }//(Properties.Settings.Default.LastSyncDateTime == new DateTime() ? null : (DateTime?)Properties.Settings.Default.LastSyncDateTime); 
            set { } 
        }

        public bool EnableShopee
        {
            get { return Properties.Settings.Default.EnableShopee; }
            set { }
        }

        public bool EnableLazada
        {
            get { return Properties.Settings.Default.EnableLazada; }
            set { }
        }

        public string OnlineFilePath
        {
            get { return Properties.Settings.Default.ExcludedUPCs; }
            set { }
        }

        //public bool UseKeepOfflineFile
        //{
        //    get { return Properties.Settings.Default.UseKeepOfflineFile; }
        //    set { }
        //}

        public bool IsShopeeStore
        {
            get { return Properties.Settings.Default.EnableShopee; }
            set { }
        }
        public bool IsLazadaStore
        {
            get { return Properties.Settings.Default.EnableLazada; }
            set { }
        }


        //public bool IsReservedStock
        //{
        //    get { return Properties.Settings.Default.ReserveStock; }
        //    set { }
        //}


        //#region SHOPEE



        ///// <summary>
        ///// Shopee - Shop ID
        ///// </summary>
        //public int ShopeeShopId
        //{
        //    get
        //    {
        //        return Properties.Settings.Default.ShopeeShopId;
        //    }
        //    set { }
        //}
        ///// <summary>
        ///// Shopee - Partner ID
        ///// </summary>
        //public int ShopeePartnerId
        //{
        //    get
        //    {
        //        return Properties.Settings.Default.ShopeePartnerId;
        //    }
        //    set { }
        //}
        ///// <summary>
        ///// Shopee - API Secret Key
        ///// </summary>
        //public string ShopeeSecretKey
        //{
        //    get
        //    {
        //        return Properties.Settings.Default.ShopeeSecretKey;
        //    }
        //    set { }
        //}
        ///// <summary>
        ///// Shopee - Base domain
        ///// </summary>
        //public string ShopeeDomain
        //{
        //    get
        //    {
        //        return Properties.Settings.Default.ShopeeDomain;
        //    }
        //    set { }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        //public string ShopeeVariationSKUFormat { get => Properties.Settings.Default.ShopeeVariationSkuFormat; set => throw new NotImplementedException(); }

        //public string ShopeeVariationSKUSeparator { get => Properties.Settings.Default.ShopeeVariationSkuSeparator; set => throw new NotImplementedException(); }

        //#endregion



        //#region LAZADA

        ///// <summary>
        ///// Lazada - App key
        ///// </summary>
        //public string LazadaAppKey
        //{
        //    get
        //    {
        //        return Properties.Settings.Default.LazadaAppKey;
        //    }
        //    set { }
        //}
        ///// <summary>
        ///// Lazada - App secret
        ///// </summary>
        //public string LazadaAppSecret
        //{
        //    get
        //    {
        //        return Properties.Settings.Default.LazadaAppSecret;
        //    }
        //    set { }
        //}
        ///// <summary>
        ///// Lazada - Domain
        ///// </summary>
        //public string LazadaDomain
        //{
        //    get
        //    {
        //        return Properties.Settings.Default.LazadaDomain;
        //    }
        //    set { }
        //}
        ///// <summary>
        ///// Lazada - Access token
        ///// </summary>
        //public string LazadaAccessToken
        //{
        //    get;set;
        //}
        ///// <summary>
        ///// Lazada - Refresh token
        ///// </summary>
        //public string LazadaRefreshToken
        //{
        //    get;
        //    set;
        //}

        //#endregion 

        /// <summary>
        /// Cho timer chạy hay không
        /// </summary>
        public Boolean RunAsTimer
        {
            get
            {
                return Properties.Settings.Default.RunAsTimer;
            }
            set { }
        }

        //public bool UseMultiWarehouse { get => Properties.Settings.Default.UseMultiWarehouse; set { } }
        //public string WarehouseCode { get => Properties.Settings.Default.WarehouseCode; set { } }

        //public void UpdateLastSyncDateTime(DateTime syncDateTime)
        //{
        //    Properties.Settings.Default.LastSyncDateTime = syncDateTime;
        //    Properties.Settings.Default.Save();
        //}

        //public void UpdateSyncTime(int syncTime)
        //{
        //    Properties.Settings.Default.SyncTime = syncTime;
        //    Properties.Settings.Default.Save();
        //}
    }
}
