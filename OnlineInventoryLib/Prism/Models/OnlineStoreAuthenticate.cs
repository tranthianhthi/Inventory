namespace OnlineInventoryLib.Prism.Models
{
    public class OnlineStoreAuthenticate
    {
        #region Bảng app

        public string BaseURL { get; set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }

        #endregion


        #region Bảng setting

        // setting id
        public int ID { get; set; }

        public string StoreCode { get; set; }
        public string Platform { get; set; }
        public string ShopId { get; set; }

        /// <summary>
        /// Sử dụng danh sách quản lý tồn riêng ( true - có ; false - không )
        /// </summary>
        public bool UseCustomList { get; set; }

        /// <summary>
        /// Danh sách sử dụng để giữ stock lại cửa hàng ( true - keep offline ; false - sellable )
        /// </summary>
        public bool IsReservedStock { get; set; }

        /// <summary>
        /// Có sử dụng file local hay sử dụng trên db ( true - local ; false - db )
        /// </summary>
        public bool UseLocalFile { get; set; }

        /// <summary>
        /// Sử dụng tổng inventory của nhiều store để up lên online( true - có ; false - không )
        /// </summary>
        public bool UseMultiStoresInventory { get; set; }

        /// <summary>
        /// Danh sách store code cách nhau bởi dấu ",". Không quan tâm nếu UseMultiStoresInventory = false.
        /// </summary>
        public string StoreList { get; set; }

        public bool IsLazada { get { return string.Compare(Platform, "lazada", true) == 0; } }
        public bool IsShopee { get { return string.Compare(Platform, "shopee", true) == 0; } }
        public bool ActiveStore { get; set; }

        #endregion

    }
}
