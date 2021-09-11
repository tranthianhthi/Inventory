using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineInventoryAPI.Models.DTO
{
    public class OnlineStoreAuthenticateDTO
    {
        public int ID { get; set; }
        public string LazadaAccessToken { get; set; }
        public string LazadaRefreshToken { get; set; }
    }
}
