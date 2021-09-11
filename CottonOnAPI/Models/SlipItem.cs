namespace CottonOnAPI.Models
{
    public class SlipItem
    {
        public long slip_id { get; set; }
        public int item_pos { get; set; }
        public long item_sid { get; set; }
        public int qty { get; set; }
        public float price { get; set; }
        public float cost { get; set; }
        public int tax_code { get; set; }
        public int tax_perc { get; set; }

    }
}
