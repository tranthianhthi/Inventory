using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACFCPrismAPI.Models
{
    public class ItemInventoryOnHand
    {
        public UInt64 item_sid {get;set;}
        public int qty { get; set; }
        public int sbs_no { get; set; }
        public int store_name { get; set; }
        public string store_code { get; set; }
    }
}
