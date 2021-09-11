using System.Collections.Generic;
using System.Data;

namespace COGInterfaceCommand.Common.COG
{
    public class StoreToStoreMaster
    {
        public string storeCodeFrom { get; set; }
        public string storeCodeTo { get; set; }
        public string consigmentNumber { get; set; }

        public List<StoreToStoreDetails> skus { get; set; }

        public StoreToStoreMaster(DataRow rowMaster, DataTable details)
        {
            storeCodeFrom = rowMaster["in_glob_store_no"].ToString();
            storeCodeTo = rowMaster["out_glob_store_no"].ToString();
            consigmentNumber = rowMaster["consigment_number"].ToString();

            skus = new List<StoreToStoreDetails>();

            foreach(DataRow row in details.Rows)
            {
                string sku = row["sku"].ToString();
                int qty = int.Parse(row["qty"].ToString());

                skus.Add(new StoreToStoreDetails(sku, qty));
            }
        }
    }
}
