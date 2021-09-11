using OnlineInventoryLib.Shopee.Models;

namespace OnlineInventoryLib.Shopee.Responses
{
    public class GetOrderDetailsResponse
    {
        public ShopeeOrder[] orders { get; set; }
        public string request_id { get; set; }
    }
}
