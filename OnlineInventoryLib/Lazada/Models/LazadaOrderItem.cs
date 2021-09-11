using System.ComponentModel;

namespace OnlineInventoryLib.Lazada.Models
{
    public class LazadaOrderItem
    {
        public string sku { get; set; }

        [DefaultValue(1)]
        public int qty { get; set; }
    }
}
