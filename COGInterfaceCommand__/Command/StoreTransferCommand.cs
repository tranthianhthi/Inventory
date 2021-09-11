using COGInterfaceCommand.Common;
using COGInterfaceCommand.Common.COG;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace COGInterfaceCommand.Command
{
    public class StoreTransferCommand : APICommand 
   {
        string url = "transfers";
        string urlAcknowledgement = "ASN/acknowledge";
        string fileType = "STORE2STORE";

        private const string StoreCodePrefix = "I";

        private string QueryStore2StoreVouchers =
            "SELECT v.vou_sid, " +
                    "stv.store_code as in_store_code " +
                    ", stv.glob_store_code as in_glob_store_code " +
                    ", sts.store_code as out_store_code " +
                    ", sts.glob_store_code as out_glob_store_code " +
                    ", stv.store_code || substr(slip_sid, 12) as consigment_number  " +
                    ", s.slip_no " +
                    ", v.vou_no " +
            "FROM voucher v " +
            "INNER JOIN slip s ON v.vou_sid = s.vou_sid " +
            "INNER JOIN store stv ON v.sbs_no = stv.sbs_no AND v.store_no = stv.store_no " +
            "INNER JOIN store sts ON s.sbs_no = sts.sbs_no AND s.out_store_no = sts.store_no " +
            "WHERE  v.vou_class = 0 " + 
            "AND v.vou_type = 0 " + 
            "AND v.held = 0 " + 
            "AND stv.store_code LIKE '" + StoreCodePrefix + "%' " +
            "AND sts.store_code LIKE '" + StoreCodePrefix + "%' ";


        private string QueryVoucherItem =
            "SELECT bm.style || bm.color || bm.sizedesc as sku, SUM(vi.qty) as qty " + 
            "FROM voucher v " +
            "INNER JOIN vou_item vi ON v.vou_sid = vi.vou_sid " +
            "LEFT JOIN cog_BarcodeMaster bm ON TO_CHAR(vi.item_sid) = bm.barcode ";

        public StoreTransferCommand(string URL, string id, string secret, string resource, string apiURL, string primarySub, string secondarySub, string apiParameter, string grantType = "client_credentials") : base(URL, id, secret, resource, apiURL, primarySub, secondarySub, apiParameter, grantType)
        {
            //fileName = Environment.CurrentDirectory + "\\" + fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">Chuỗi ngày tháng theo định dạng MM/DD/YYYY</param>
        public void SubmitStoreStransfer(string date, string skuCombineString)
        {
            if (DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture, 0, out DateTime voucherDate))
            {
                try
                {
                    List<StoreToStoreTransferMaster> store2storeTransfers = GetDailyStore2Store(date);

                    foreach (StoreToStoreTransferMaster transfer in store2storeTransfers)
                    {
                        base.GetCOGToken();
                        base.PostDataToCOG(this.url, transfer);
                    }
                }
                catch { throw; }
            }
        }

        private List<StoreToStoreTransferMaster> GetDailyStore2Store(string date)
        {
            List<StoreToStoreTransferMaster> list = new List<StoreToStoreTransferMaster>();

            try
            {
                // Lấy danh sách các voucher của COG đã generate trong ngày
                string masterCommand =
                    QueryStore2StoreVouchers +
                    "AND trunc(v.created_date) = TO_DATE('" + date + "', 'MM/DD/YYYY') ";
                    
                    
                DataTable tbMaster = new DataTable();
                Configurations config = new Configurations();
                tbMaster = config.ExecuteQueryData(config.rPConnection, masterCommand, null);
                
                foreach(DataRow row in tbMaster.Rows)
                {
                    // Lấy chi tiết item đã nhận trong voucher - nhóm theo SKU & tính tổng số lượng
                    string vou_sid = row["vou_sid"].ToString();
                    
                    DataTable tbDetails = new DataTable();
                    string detailCommand = QueryVoucherItem + " WHERE v.vou_sid = " + vou_sid + " GROUP BY bm.style || bm.color || bm.sizedesc"; ;

                    tbDetails = config.ExecuteQueryData(config.rPConnection, detailCommand, null);

                    list.Add(new StoreToStoreTransferMaster(row, tbDetails));
                }

                return list;
            }
            catch(Exception ex) { throw ex; }
        }
    }
}
