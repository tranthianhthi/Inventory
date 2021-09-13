using System;

namespace OnlineInventoryLib.Prism.Models
{
    public class Online_store_Tiki : OnlineStoreAuthenticate
    {

        public string ApplicationID { get; set; }
        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }

        public DateTime AuthenticateDate { get; set; }
        public bool MultiWH { get; set; }
        public string WHCode { get; set; }

        public int ExpiresIn { get; set; }
      
        public string Method { get; set; }

        public string ContentType { get; set; }
        #region Bảng app

        //public string AppKey { get; set; }
        // public string AppSecret { get; set; }
        public string AppURL { get; set; }

        #endregion 
    }
}
