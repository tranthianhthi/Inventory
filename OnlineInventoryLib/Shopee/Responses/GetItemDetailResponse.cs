using OnlineInventoryLib.Shopee.Models;

namespace OnlineInventoryLib.Shopee.Responses
{
    public class GetItemDetailResponse
    {
        public ShopeeItem item { get; set; }
        public string request_id { get; set; }
    }
}
