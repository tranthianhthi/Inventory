namespace OnlineInventoryLib.Shopee.Requests
{
    public class UpdateStockRequest : BaseRequest
    {
        public UpdateStockRequest(int item_id, int stock, int shopid, int partner_id) : base(shopid, partner_id)
        {
            this.item_id = item_id;
            this.stock = stock;
        }

        public int item_id { get; set; }
        public int stock { get; set; }
    }
}
