namespace OnlineInventoryLib.Prism.Models
{
    public class Online_Store_Shopee : OnlineStoreAuthenticate
    {
        #region Bảng app

        public string PartnerID { get; set; }
        public string SecretKey { get; set; }
        public string AppURL { get; set; }

        #endregion


        #region Bảng setting

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

        #endregion

        #region Bảng Shopee

        public int ShopID { get; set; }

        /// <summary>
        /// Format SKU trên Shopee ( tùy, có thể là loại combine upc-sku-color-size hoặc gì cũng được.
        /// </summary>
        public string VariationSkuFormat { get; set; }

        /// <summary>
        /// Phân cách giữa các cột trên Shopee
        /// </summary>
        public string VariationSkuSeparator { get; set; }

        #endregion 
    }
}
