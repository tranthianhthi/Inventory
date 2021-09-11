using System;

namespace OnlineInventoryLib.Prism.Models
{
    public class Online_store_Lazada : OnlineStoreAuthenticate
    {

        public string LazadaAccount { get; set; }
        public string LazadaPassword { get; set; }
        public string LazadaAccessToken { get; set; }
        public string LazadaRefreshToken { get; set; }
        public DateTime AuthenticateDate { get; set; }
        public bool LazadaMultiWH { get; set; }
        public string LazadaWHCode { get; set; }

      

        #region Bảng app

        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public string AppURL { get; set; }

        #endregion 
    }
}
