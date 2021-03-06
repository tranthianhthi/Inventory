namespace OnlineInventoryLib.Prism.Models
{
    public class OnlineUPC
    {
        public OnlineUPC(string uPC, int qty)
        {
            UPC = uPC;
            Qty = qty;
        }

        /// <summary>
        /// UPC sẽ được bán trên online store
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// Số lượng upc sẽ keep lại để bán offline
        /// </summary>
        public int Qty { get; set; }
    }
}
