using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;

namespace COGInterfaceCommand.Common.COG
{
    public class WHToStoreTransferMaster
    {
        public string storeCode { get; set; }

        [JsonIgnore]
        [ScriptIgnore]
        public string consigmentNumber { get; set; }

        public List<TransferDetails> skus { get; set; }

        public WHToStoreTransferMaster(DataRow rowMaster, DataTable details)
        {
            storeCode = rowMaster["in_glob_store_no"].ToString();
            consigmentNumber = rowMaster["consigment_number"].ToString();

            skus = new List<TransferDetails>();

            foreach(DataRow row in details.Rows)
            {
                string sku = row["sku"].ToString();
                int qty = int.Parse(row["qty"].ToString());

                skus.Add(new TransferDetails(sku, qty));
            }
        }

        public WHToStoreTransferMaster(string in_glob_store_no, string consignment_number, DataTable details)
        {
            storeCode = in_glob_store_no;
            consigmentNumber = consignment_number;

            skus = new List<TransferDetails>();

            foreach (DataRow row in details.Rows)
            {
                string sku = row["sku"].ToString();
                int qty = int.Parse(row["qty"].ToString());

                skus.Add(new TransferDetails(sku, qty));
            }
        }
    }
}
