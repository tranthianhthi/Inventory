using COGInterfaceCommand.Common;
using COGInterfaceCommand.Common.COG;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace COGInterfaceCommand.Command
{
   public class StockAdjustCommand : APICommand
    {
        string url = "stores/"; //https://cogapimanagementprd01.azure-api.net/Licensee/v1/stores/{storeCode}/stocks 
        string addurl = "/stocks";
        string urlAcknowledgement = "ASN/acknowledge";
        string fileType = "STOCKADJUST";

        private const string StoreCodePrefix = "I";

        private string QueryStore2StoreAdjustVouchers =           
            "SELECT v.vou_sid, " +
                    "stv.store_code as in_store_code " +
                    ", stv.glob_store_code as in_glob_store_code " +
                    ", sts.store_code as out_store_code " +
                    ", sts.glob_store_code as out_glob_store_code " +
                    ", stv.glob_store_code || substr(slip_sid, 12) as consigment_number  " +
                    ", s.slip_no " +
                    ", v.vou_no " +
            "FROM voucher v " +
            "INNER JOIN slip s ON v.vou_sid = s.vou_sid " +
            "INNER JOIN store stv ON v.sbs_no = stv.sbs_no AND v.store_no = stv.store_no " +
            "INNER JOIN store sts ON s.sbs_no = sts.sbs_no AND s.out_store_no = sts.store_no " +
            "WHERE  v.vou_class = 0 " +
            "AND v.vou_type = 0 " +
            "AND v.held = 0 " +
            "AND (( stv.store_code LIKE '999' AND sts.store_code LIKE '" + StoreCodePrefix + "%' ) OR ( sts.store_code LIKE '999' AND stv.store_code LIKE '" + StoreCodePrefix + "%' ))";


        private string QueryVoucherItem =
            "SELECT bm.barcode as sku, SUM(vi.qty) as qty , CASE WHEN stv.store_code = '999' THEN -1 ELSE 1 END as Modifier " +
            "FROM voucher v " +
            "INNER JOIN vou_item vi ON v.vou_sid = vi.vou_sid " +
            "INNER JOIN store stv ON v.sbs_no = stv.sbs_no AND v.store_no = stv.store_no " +
            "LEFT JOIN cog_BarcodeMaster bm ON TO_CHAR(vi.item_sid) = bm.barcode  ";

        public StockAdjustCommand(string URL, string id, string secret, string resource, string apiURL, string primarySub, string secondarySub, string apiParameter, string grantType = "client_credentials") : base(URL, id, secret, resource, apiURL, primarySub, secondarySub, apiParameter, grantType)
        {
            //fileName = Environment.CurrentDirectory + "\\" + fileName;
        }

        public void SubmitStoreStransferAdjust(string date, string skuCombineString)
        {
            if (DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture, 0, out DateTime voucherDate))
            {
                try
                {
                    List<StockAdjustMaster> store2storeTransfersAdjust = GetDailyStore2StoreAdjust(date);

                    foreach (StockAdjustMaster transfer in store2storeTransfersAdjust)
                    {
                        string storecode = "";
                        if (transfer.storeCodeFrom =="") storecode = transfer.storeCodeTo;
                        else storecode = transfer.storeCodeFrom;
                        base.GetCOGToken();
                        base.PostDataToCOG(APIURL + this.url + storecode + addurl, transfer,true);
                    }
                }
                catch { throw; }
            }
        }

        private List<StockAdjustMaster> GetDailyStore2StoreAdjust(string date)
        {
            List<StockAdjustMaster> list = new List<StockAdjustMaster>();

            try
            {
                // Lấy danh sách các voucher của COG đã generate trong ngày
                string masterCommand =
                    QueryStore2StoreAdjustVouchers +
                    "AND trunc(v.created_date) = TO_DATE('" + date + "', 'MM/DD/YYYY') ";


                DataTable tbMaster = new DataTable();
                Configurations config = new Configurations();
                tbMaster = config.ExecuteQueryData(config.rPConnection, masterCommand, null);

                foreach (DataRow row in tbMaster.Rows)
                {
                    // Lấy chi tiết item đã nhận trong voucher - nhóm theo SKU & tính tổng số lượng
                    string vou_sid = row["vou_sid"].ToString();

                    DataTable tbDetails = new DataTable();
                    string detailCommand = QueryVoucherItem + " WHERE v.vou_sid = " + vou_sid + " GROUP BY bm.barcode,stv.store_code"; 

                    tbDetails = config.ExecuteQueryData(config.rPConnection, detailCommand, null);
                  
                    list.Add(new StockAdjustMaster(row, tbDetails));
                        
                }

                return list;
            }
            catch (Exception ex) { throw ex; }
        }

    }
}
