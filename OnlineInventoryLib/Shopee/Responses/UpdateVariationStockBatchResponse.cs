using OnlineInventoryLib.Shopee.Models;
using System;

namespace OnlineInventoryLib.Shopee.Responses
{
    public class UpdateVariationStockBatchResponse
    {
        public string request_id { get; set; }
        public BatchResponse batch_result { get; set; }   
    }

    public class BatchResponse
    {
        public ShopeeVariation[] modifications { get; set; }
        public UpdateVariationStockBatchFailure[] failures { get; set; }
    }
    public class UpdateVariationStockBatchFailure
    {
        public UInt64 item_id { get; set; }
        public UInt64 variation_id { get; set; }
        public string error_description { get; set; }
    }
}
