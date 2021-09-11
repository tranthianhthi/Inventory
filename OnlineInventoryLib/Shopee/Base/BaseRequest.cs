using System;

namespace OnlineInventoryLib.Shopee.Requests
{
    public class BaseRequest : IRequestData
    {
        public BaseRequest(int shopid, int partner_id)
        {
            this.shopid = shopid;
            this.partner_id = partner_id;
        }

        public long timestamp { get { return DateTimeOffset.UtcNow.ToUnixTimeSeconds(); } }
        public int shopid { get; set; }
        public int partner_id { get; set; }
    }
}
