namespace OnlineInventory.Models
{
    public class RPShopeeMapping
    {
        public RPShopeeMapping(string shopeeSKU, string shopeeItemSid, string prismStyleCC, string prismUPC)
        {
            ShopeeSKU = shopeeSKU;
            ShopeeItemSid = shopeeItemSid;
            PrismUPC = prismUPC;
            PrismStyleCC = prismStyleCC;
        }

        public string ShopeeSKU { get; set; }
        public string ShopeeItemSid { get; set; }
        public string PrismUPC { get; set; }
        public string PrismStyleCC { get; set; }
    }
}
