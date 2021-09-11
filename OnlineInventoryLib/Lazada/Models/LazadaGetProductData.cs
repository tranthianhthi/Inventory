namespace OnlineInventoryLib.Lazada.Models
{
    public class LazadaGetProductData
    {
        public LazadaProduct[] products { get; set; }
        public int total_products { get; set; }

        public int CountProducts()
        {
            if (products == null)
                return 0;

            return products.Length;
        }

        public int CountTotalSKUs()
        {
            if (products == null)
                return 0;

            int count = 0;
            foreach (LazadaProduct prod in products)
            {
                count += prod.CountTotalSKUs();
            }

            return count;
        }
    }
}
