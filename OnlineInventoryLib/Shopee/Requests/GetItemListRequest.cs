using System.ComponentModel;

namespace OnlineInventoryLib.Shopee.Requests
{
    public class GetItemListRequest : BaseRequest
    {
        /// <summary>
        /// Request lấy danh sách item
        /// </summary>
        /// <param name="shopid">Shopee shop ID</param>
        /// <param name="partner_id">Shoppe App Partner ID</param>
        /// <param name="update_time_from">Thời gian update</param>
        /// <param name="update_time_to">Thời gian update</param>
        /// <param name="pagination_offset">Page đang load</param>
        /// <param name="pagination_entries_per_page">Số item 1 lần load</param>
        public GetItemListRequest(int shopid, int partner_id, long update_time_from = 0, long update_time_to = 0, int pagination_offset = 0, int pagination_entries_per_page = 100) : base(shopid, partner_id)
        {
            this.update_time_from = update_time_from;
            this.update_time_to = update_time_to;
            this.pagination_entries_per_page = pagination_entries_per_page;
            this.pagination_offset = pagination_offset;
        }

        [DefaultValue(-1)]
        public int pagination_offset { get; set; }

        //[DefaultValue(100)]
        public int pagination_entries_per_page { get; set; }
        public long update_time_from { get; set; }
        public long update_time_to { get; set; }

    }
}
