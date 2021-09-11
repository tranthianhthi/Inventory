using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace COGInterfaceCommand.Common.COG
{
    public class StockAdjustMaster
    {
        [JsonIgnore]
        [ScriptIgnore]
        public string storeCodeFrom { get; set; }
        [JsonIgnore]
        [ScriptIgnore]
        public string storeCodeTo { get; set; }
        public string reasonCode { get; set; }

        public List<StoreToStoreDetails> skus { get; set; }

        public StockAdjustMaster(DataRow rowMaster, DataTable details)
        {
            storeCodeFrom = rowMaster["in_glob_store_code"].ToString();
            storeCodeTo = rowMaster["out_glob_store_code"].ToString();
            reasonCode = "ST";//rowMaster["consigment_number"].ToString();

            skus = new List<StoreToStoreDetails>();

            foreach (DataRow row in details.Rows)
            {
                string sku = row["sku"].ToString();
                int qty = int.Parse(row["qty"].ToString()) * int.Parse(row["Modifier"].ToString());

                skus.Add(new StoreToStoreDetails(sku, qty));
            }
        }
    }
}
