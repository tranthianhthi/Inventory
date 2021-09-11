using System;
using System.Linq;

namespace OnlineInventoryLib.Lazada.Models
{
    public class LazadaProduct
    {
        /// <summary>
        /// Danh sách upc -> size
        /// </summary>
        public LazadaSKU[] skus { get; set; }

        /// <summary>
        /// Chuỗi XML để update thông tin 1 sản phẩm trên sàn
        /// </summary>
        private readonly string ProductXml = @"<Product><Skus>{0}</Skus></Product>";

        /// <summary>
        /// Số lượng size 1 mã
        /// </summary>
        /// <returns></returns>
        public int CountTotalSKUs()
        {
            return skus == null ? 0 : skus.Length;
        }

        /// <summary>
        /// Generate XML Payload
        /// </summary>
        /// <returns></returns>
        public string GenerateUpdateProductQuantity()
        {
            string SKUsXml = "";
            foreach (LazadaSKU sku in skus)
            {
                SKUsXml += sku.GenerateUpdateSKUQuantity();
            }
            return string.Format(ProductXml, SKUsXml);
        }

        /// <summary>
        /// Create log string
        /// </summary>
        /// <returns></returns>
        public string GenerateProductLogString()
        {
            string result = "Product" + Environment.NewLine;
            var newQtyList = skus.Where(s => s.Quantity != s.NewQuantity);

            if (newQtyList.Count() > 0)
            {
                foreach (LazadaSKU sku in newQtyList)
                {
                    result += sku.GenerateSKULogString() + Environment.NewLine;
                }
                return string.Format(result + Environment.NewLine);
            }
            return "";
        }
    }
}
