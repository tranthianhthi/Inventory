using OnlineInventoryLib.Shopee.Models;

namespace OnlineInventoryLib.Shopee.Responses
{
    public class UpdateStockResponse
    {
        public string request_id { get; set; }
        public ShopeeItem item { get; set; }
    }
}
