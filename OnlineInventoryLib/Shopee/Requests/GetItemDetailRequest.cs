using System;

namespace OnlineInventoryLib.Shopee.Requests
{
    public class GetItemDetailRequest : BaseRequest
    {
        /// <summary>
        /// Request lấy chi tiết item
        /// </summary>
        /// <param name="item_id">Mã item trong shopee</param>
        /// <param name="shopid">Shopee shop ID</param>
        /// <param name="partner_id">Shopee App Partner ID</param>
        public GetItemDetailRequest(UInt64 item_id, int shopid, int partner_id) : base(shopid, partner_id)
        {
            this.item_id = item_id;
        }

        public UInt64 item_id { get; set; }
    }
}
