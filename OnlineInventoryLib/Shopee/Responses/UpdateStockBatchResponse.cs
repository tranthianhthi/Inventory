using OnlineInventoryLib.Shopee.Models;
using System;

namespace OnlineInventoryLib.Shopee.Responses
{
    public class UpdateStockBatchResponse
    {
        public BatchResult batch_result { get; set; }
        public string request_id { get; set; }

        public override string ToString()
        {
            string result = string.Format("UpdateStockBatch - Request ID: {0}", request_id) + Environment.NewLine;
            string error = "{0} cannot update stock to item {1} - Reason: {2}";
            string success = "{0} Stock of item {1} is updated to {2}";


            foreach (ShopeeItem item in batch_result.modifications)
            {
                result += string.Format(success, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), item.item_id, item.stock) + Environment.NewLine;
            }

            foreach (ShopeeItem item in batch_result.failures)
            {
                result += string.Format(error, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), item.item_id, item.error_description) + Environment.NewLine;
            }

            return result;
        }
    }

    public class BatchResult
    {
        public ShopeeItem[] modifications { get; set; }
        public ShopeeItem[] failures { get; set; }
    }
}
