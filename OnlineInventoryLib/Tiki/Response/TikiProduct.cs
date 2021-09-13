using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OnlineInventoryLib.Tiki.Response
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Inventory
    {
        public string inventory_type;
        public string fulfillment_type;
        public int quantity;
        public int quantity_available;
        public int quantity_reserved;
        public int quantity_sellable;
    }

   
    public class TikiProduct 
    {
        public int product_id;
        public string sku;
        public string name;
        public int master_id;
        public string master_sku;
        public int super_id;
        public string super_sku;
        public int active;
        public string original_sku { get; set; }

        public int price;
        public int market_price;
        public string created_at;
        public string updated_at;

        [JsonIgnore]
        public int PrismOHQty { get; set; }

        /// <summary>
        /// Số lượng hàng giữ bán offline
        /// </summary>
        [JsonIgnore]
        public int stock { get ; set; }

        [JsonIgnore]
        public int OfflineQty { get; set; }

        /// <summary>
        /// Số lượng sẽ update lên sàn
        /// </summary>
        [JsonIgnore]
        public int NewQuantity { get; set; }

        

        public Inventory inventory;
    }

    public class Paging
    {
        public int total;
        public int per_page;
        public int current_page;
        public int last_page;
        public int from;
        public int to;
    }

    
    public class Root
    {       
        public List<TikiProduct> data;
        public Paging paging;
    }
}
