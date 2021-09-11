using OnlineInventoryLib.Shopee.Requests;
using OnlineInventoryLib.Shopee.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineInventoryLib.Shopee.Models
{
    public class ShopeeUpdateVariationsLog
    {
        public ShopeeUpdateVariationsLog()
        {
            Info = new List<ShopeeUpdateVariationsInfo>();
        }

        public int TotalVariations { get; set; }
        public double TotalPage { get; set; }
        public int PageSize { get; set; }
        public List<ShopeeUpdateVariationsInfo> Info { get; set; }

    }

    public class ShopeeUpdateVariationsInfo
    {
        public DateTime RequestTime { get; set; }
        public UpdateVariationStockBatchRequest Request { get; set; }
        public UpdateVariationStockBatchResponse Response { get; set; }
        public DateTime ResponseTime { get; set; }
    }
}
