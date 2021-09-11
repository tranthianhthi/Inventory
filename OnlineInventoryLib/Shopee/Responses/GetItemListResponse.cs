using OnlineInventoryLib.Shopee.Models;

namespace OnlineInventoryLib.Shopee.Responses
{
    public class GetItemListResponse
    {
        public ShopeeItem[] items { get; set; }
        public bool more { get; set; }
        public string request_id { get; set; }
        public int total { get; set; }
    }
}
