using System;
using System.ComponentModel;

namespace OnlineInventoryLib.Shopee.Models
{
    public class ShopeeOrder
    {
        public enum Status { UNPAID, READY_TO_SHIP, COMPLETED, IN_CANCEL, CANCELLED, TO_RETURN }

        [DefaultValue(null)]
        public string ordersn { get; set; }

        [DefaultValue(null)]
        public string order_status { get; set; }

        [DefaultValue(0)]
        public long update_time { get; set; }

        [DefaultValue(null)]
        public DateTime? update_date_time
        {
            get
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(update_time);
                DateTime dateTime = dateTimeOffset.LocalDateTime;
                return dateTime;
            }
        }

        [DefaultValue(0)]
        public long create_time { get; set; }

        [DefaultValue(null)]
        public DateTime? create_date_time
        {
            get
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(create_time);
                DateTime dateTime = dateTimeOffset.LocalDateTime;
                return dateTime;
            }
        }

        [DefaultValue(null)]
        public string country { get; set; }

        [DefaultValue(null)]
        public string currency { get; set; }

        public bool cod { get; set; }

        [DefaultValue(null)]
        public string tracking_no { get; set; }

        [DefaultValue(0)]
        public int day_to_ship { get; set; }

        [DefaultValue(null)]
        public ShopeeRecipientAddress recipient_address { get; set; }

        [DefaultValue(null)]
        public ShopeeOrderItem[] items { get; set; }
    }
}
