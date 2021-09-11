namespace OnlineInventoryLib.Shopee.Requests
{
    public class GetOrdersByStatusRequest : BaseRequest
    {
        public string order_status { get; set; }
        public long create_time_from { get; set; }
        public long create_time_to { get; set; }
        public int pagination_entries_per_page { get; set; }
        public int pagination_offset { get; set; }

        public GetOrdersByStatusRequest(string order_status, int shopid, int partner_id, long update_time_from = 0, long update_time_to = 0, int pagination_offset = 0, int pagination_entries_per_page = 100) : base(shopid, partner_id)
        {
            this.order_status = order_status;
            this.create_time_from = create_time_from;
            this.create_time_to = create_time_to;
            this.pagination_entries_per_page = pagination_entries_per_page;
            this.pagination_offset = pagination_offset;
        }
     
    }
}
