namespace OnlineInventoryLib.Lazada.Models
{
    public class LazadaSKU
    {
        public bool UseMultiWarehouse { get; set; }

        public string WarehouseCode { get; set; }

        /// <summary>
        /// Mã sku trên sàn ~ mã upc trên Prism
        /// </summary>
        public string SellerSku { get; set; }

        /// <summary>
        /// Số lượng available trên Lazada ( Stock - ordered qty ) 
        /// </summary>
        public int Available { get; set; }

        /// <summary>
        /// Số lượng available trên Lazada ( Stock - ordered qty ) 
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Số lượng tồn kho trên Prism
        /// </summary>
        public int PrismOHQty { get; set; }

        /// <summary>
        /// Số lượng hàng giữ bán offline
        /// </summary>
        public int OfflineQty { get; set; }

        /// <summary>
        /// Số lượng sẽ update lên sàn
        /// </summary>
        public int NewQuantity { get; set; }

        public bool IsNeedToUpdate { get { return NewQuantity != Quantity; } }

        private readonly string SKUXml = @"<Sku><SellerSku>{0}</SellerSku><Quantity>{1}</Quantity></Sku>";
        private readonly string SKUXmlMultiWarehouse = @"<Sku><SellerSku>{0}</SellerSku><MultiWarehouseInventories><MultiWarehouseInventory><WarehouseCode>{2}</WarehouseCode><Quantity>{1}</Quantity></MultiWarehouseInventory></MultiWarehouseInventories></Sku>";

        public string GenerateUpdateSKUQuantity()
        {
            if (UseMultiWarehouse)
            {
                return string.Format(SKUXmlMultiWarehouse, SellerSku, NewQuantity, WarehouseCode);
            }
            else
            {
                return string.Format(SKUXml, SellerSku, NewQuantity);
            }
            
        }

        /// <summary>
        /// Create log string for SKU
        /// </summary>
        /// <returns></returns>
        public string GenerateSKULogString()
        {
            if (NewQuantity != Quantity)
            {
                if (UseMultiWarehouse)
                {
                    return string.Format("SKU:{0} - Warehouse:{3} - Current Qty:{1} - New Qty:{2}", SellerSku, Quantity, NewQuantity, WarehouseCode);
                }
                else
                {
                    return string.Format("SKU:{0} - Current Qty:{1} - New Qty:{2}", SellerSku, Quantity, NewQuantity);
                }
            }

            return "";
        }
    }
}
