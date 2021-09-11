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
        [JsonIgnore]
        [ScriptIgnore]
        public string slipno { get; set; }

        public List<BackTransferDetails> skus { get; set; }

        public WHToStoreTransferMaster(StoreToStoreTransferMaster master)
        {
            storeCode = master.storeCodeTo;
            consigmentNumber = master.consignmentNumber;

            this.skus = new List<BackTransferDetails>();

            foreach (TransferDetails sku in master.skus)
            {
                this.skus.Add(new BackTransferDetails(sku));
            }
        }
        public WHToStoreTransferMaster(DataRow rowMaster, DataTable details)
        {
            storeCode = rowMaster["in_glob_store_no"].ToString();
            consigmentNumber = rowMaster["consigment_number"].ToString();

            skus = new List<BackTransferDetails>();

            foreach(DataRow row in details.Rows)
            {
                string sku = row["sku"].ToString();
                int qty = int.Parse(row["qty"].ToString());
                skus.Add(new BackTransferDetails(sku, qty));
            }
        }

        public WHToStoreTransferMaster(string in_glob_store_no, string consignment_number,string slip_no, DataTable details)
        {
            storeCode = in_glob_store_no;
            consigmentNumber = consignment_number;
            slipno = slip_no;

            skus = new List<BackTransferDetails>();

            foreach (DataRow row in details.Rows)
            {
                string sku = row["sku"].ToString();
                int qty = int.Parse(row["qty"].ToString());

                skus.Add(new BackTransferDetails(sku, qty));
            }
        }

      
    }
}
