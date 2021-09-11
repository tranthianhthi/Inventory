using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;

namespace COGInterfaceCommand.Common.COG
{
   public class BackStoreToStoreTransferMaster
    {
        [JsonIgnore]
        [ScriptIgnore]
        public string storeCodeFrom { get; set; }
        public string storeCode { get; set; }
        [JsonIgnore]
        [ScriptIgnore]
        public string consignmentNumber { get; set; }

        public List<BackTransferDetails> skus { get; set; }

        public BackStoreToStoreTransferMaster(DataRow rowMaster, DataTable details)
        {
            storeCode = rowMaster["in_glob_store_code"].ToString();
            storeCodeFrom = rowMaster["out_glob_store_code"].ToString();
            consignmentNumber = rowMaster["consigment_number"].ToString();

            skus = new List<BackTransferDetails>();

            foreach (DataRow row in details.Rows)
            {
                string sku = row["sku"].ToString();
                int qty = int.Parse(row["qty"].ToString());

                skus.Add(new BackTransferDetails(sku, qty));
            }
        }
        public BackStoreToStoreTransferMaster(StoreToStoreTransferMaster master)
        {
            this.storeCode  = master.storeCodeTo;
            this.storeCodeFrom = master.storeCodeFrom;
            this.consignmentNumber = master.consignmentNumber;

            this.skus = new List<BackTransferDetails>();

            foreach(TransferDetails sku in master.skus)
            {
                this.skus.Add(new BackTransferDetails(sku));
            }
            //this.skus = master.skus;
        }
    }
}
