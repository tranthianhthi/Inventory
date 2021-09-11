using System;

namespace OnlineInventoryAPI.Models
{
    public class OnlineStoreAuthenticate
    {
        public int ID { get; set; }
        public string StoreCode { get; set; }
        public string Platform { get; set; }
        public string LazadaAccessToken { get; set; }
        public string LazadaRefreshToken { get; set; }
        public DateTime AuthenticateDate { get; set; }
        public bool IsLazada { get { return string.Compare(Platform, "Lazada", true) == 0; } }
        public bool IsShopee { get { return string.Compare(Platform, "Shopee", true) == 0; } }
        public bool IsActiveStore { get; set; }
        
    }
}
