using OnlineInventoryLib.Shopee.Models;

namespace OnlineInventoryLib.Shopee.Responses
{
    public class GetOrdersListResponse
    {
        public ShopeeOrder[] orders { get; set; }
        public bool more { get; set; }
        public string request_id { get; set; }
    }
}
