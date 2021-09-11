namespace OnlineInventoryLib.Prism.Models
{
    public class PrismQtyOnHand
    {
        public string UPC { get; set; }

        /// <summary>
        /// Số lượng onhand trên Prism
        /// </summary>
        public int? QtyOnHand { get; set; }

        /// <summary>
        /// Số lượng hàng giữ lại store 
        /// </summary>
        public int keep_offline { get; set; }

        /// <summary>
        /// Số lượng sẽ bán trên store
        /// </summary>
        public int QtyOnline { get; set; }
    }
}
