using OnlineInventoryLib.Shopee.Models;

namespace OnlineInventoryLib.Shopee.Requests
{
    public class UpdateVariationStockBatchRequest : BaseRequest
    {
        public ShopeeVariation[] variations { get; set; }
        public UpdateVariationStockBatchRequest(ShopeeVariation[] variations, int shopid, int partner_id) : base(shopid, partner_id)
        {
            this.variations = variations;
        }
    }
}
