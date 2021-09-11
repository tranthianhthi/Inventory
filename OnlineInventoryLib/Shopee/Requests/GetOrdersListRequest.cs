using System.ComponentModel;

namespace OnlineInventoryLib.Shopee.Requests
{
    public class GetOrdersListRequest : BaseRequest
    {
        public GetOrdersListRequest(int shopid, int partner_id, bool use_create_time, long create_time_from, long create_time_to, long update_time_from, long update_time_to, int pagination_entries_per_page, int pagination_offset) : base(shopid, partner_id)
        {
            if (use_create_time)
            {
                this.create_time_from = create_time_from;
                this.create_time_to = create_time_to;
            }
            else
            {
                this.update_time_from = update_time_from;
                this.update_time_to = update_time_to;
            }

            this.pagination_entries_per_page = pagination_entries_per_page;
            this.pagination_offset = pagination_offset;
        }

        [DefaultValue(0)]
        public long create_time_from { get; set; }

        [DefaultValue(0)]
        public long create_time_to { get; set; }

        [DefaultValue(0)]
        public long update_time_from { get; set; }

        [DefaultValue(0)]
        public long update_time_to { get; set; }

        [DefaultValue(0)]
        public int pagination_entries_per_page { get; set; }

        [DefaultValue(0)]
        public int pagination_offset { get; set; }
    }
}
