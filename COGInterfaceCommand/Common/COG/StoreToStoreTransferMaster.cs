using System.Collections.Generic;
using System.Data;

namespace COGInterfaceCommand.Common.COG
{
    public class StoreToStoreTransferMaster
    {
        public string storeCodeFrom { get; set; }
        public string storeCodeTo { get; set; }
        public string consignmentNumber { get; set; }

        public List<TransferDetails> skus { get; set; }

        public StoreToStoreTransferMaster(DataRow rowMaster, DataTable details)
        {
            storeCodeTo = rowMaster["in_glob_store_code"].ToString();
            storeCodeFrom = rowMaster["out_glob_store_code"].ToString();
            consignmentNumber = rowMaster["consigment_number"].ToString();

            skus = new List<TransferDetails>();

            foreach(DataRow row in details.Rows)
            {
                string sku = row["sku"].ToString();
                int qty = int.Parse(row["qty"].ToString());

                skus.Add(new TransferDetails(sku, qty));
            }
        }
    }
}
