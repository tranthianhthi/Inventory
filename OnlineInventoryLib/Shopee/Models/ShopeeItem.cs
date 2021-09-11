using System;
using System.ComponentModel;

namespace OnlineInventoryLib.Shopee.Models
{
    public class ShopeeItem
    {
        [DefaultValue(0)]
        public UInt64 shopid { get; set; }

        public UInt64 item_id { get; set; }

        [DefaultValue(null)]
        public string item_sku { get; set; }

        [DefaultValue(null)]
        public int stock { get; set; }

        [DefaultValue(0.0)]
        public float price { get; set; }

        public int modified_time { get; set; }

        [DefaultValue(0)]
        public UInt64 variation_id { get; set; }

        [DefaultValue(null)]
        public string error_description { get; set; }

        public ShopeeVariation[] variations { get; set; }
    }
}
