using OnlineInventoryLib.Shopee.Models;

namespace OnlineInventoryLib.Shopee.Requests
{
    public class UpdateStockBatchRequest : BaseRequest
    {
        public ShopeeItem[] items { get; set; }
        public UpdateStockBatchRequest(ShopeeItem[] items, int shopid, int partner_id) : base(shopid, partner_id)
        {
            this.items = items;
        }
    }
}
