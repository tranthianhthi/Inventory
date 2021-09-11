namespace OnlineInventoryLib.Shopee.Requests
{
    public class GetOrderDetailsRequest : BaseRequest
    {
        public string[] ordersn_list { get; set; }

        public GetOrderDetailsRequest(string[] ordersn_list, int shopid, int partner_id) : base(shopid, partner_id)
        {
            this.ordersn_list = ordersn_list;
        }
    }
}
