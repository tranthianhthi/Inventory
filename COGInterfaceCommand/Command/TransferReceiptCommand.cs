using COGInterfaceCommand.Common.COG;
using System.IO;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Data;
using COGInterfaceCommand.Common;
using System;
using System.Globalization;
using System.Linq;
using Oracle.ManagedDataAccess.Client;

namespace COGInterfaceCommand.Command
{
    public class TransferReceiptCommand : APICommand
    {
        string url = "transfers";
        string urlAcknowledgement = "transfers";
        string dc_code = "D001";
        string content_type = "WH2STORE";

        private const int ICDStoreNo = 2;
        private const string StoreCodePrefix = "I%";

        //private string QueryStore2StoreVouchers =
        //    "SELECT v.vou_sid, " +
        //            "stv.store_code as in_store_code " +
        //            ", stv.glob_store_code as in_glob_store_code " +
        //            ", sts.store_code as out_store_code " +
        //            ", sts.glob_store_code as out_glob_store_code " +
        //            ", stv.store_code || substr(slip_sid, 12) as consigment_number  " +
        //            ", s.slip_no " +
        //            ", v.vou_no " +
        //    "FROM voucher v " +
        //    "INNER JOIN slip s ON v.vou_sid = s.vou_sid " +
        //    "INNER JOIN store stv ON v.sbs_no = stv.sbs_no AND v.store_no = stv.store_no " +
        //    "INNER JOIN store sts ON s.sbs_no = sts.sbs_no AND s.out_store_no = sts.store_no AND sts.store_no = " + ICDStoreNo.ToString() + " " +
        //    "WHERE v.vou_class = 0 " +
        //    "AND v.vou_type = 0 " +
        //    "AND v.held = 0 " +
        //    "AND stv.store_code LIKE '" + StoreCodePrefix + "%' ";


        private string QueryWH2Stores =
            "SELECT v.vou_sid " +
                    ", stv.store_code as in_store_code " +
                    ", stv.glob_store_code as in_glob_store_code " +
                    ", sts.store_code as out_store_code " +
                    ", sts.glob_store_code as out_glob_store_code " +
                    " " +
                    ", s.slip_no " +
                    ", v.vou_no " +
            "FROM voucher v " +
            "INNER JOIN slip s ON v.vou_sid = s.vou_sid " +
            "INNER JOIN store stv ON v.sbs_no = stv.sbs_no AND v.store_no = stv.store_no " +
            "INNER JOIN store sts ON s.sbs_no = sts.sbs_no AND s.out_store_no = sts.store_no AND sts.store_no = {0} " +
            "WHERE v.vou_class = 0 " +
            "AND v.vou_type = 0 " +
            "AND v.held = 0 " +
            "AND stv.store_code LIKE '{1}' " + 
            "{2} ";
        private string updatestatus = "Update Infolog_COG_Carton set SENT=1,SENT_DATED=TO_DATE('"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS') where consignment_number='{0}' and Slip_no='S{1}'";
               
        //private string QueryVoucherItem =
        //    "SELECT bm.style || bm.color || bm.sizedesc as sku, SUM(vi.qty) as qty " +
        //    "FROM voucher v " +
        //    "INNER JOIN vou_item vi ON v.vou_sid = vi.vou_sid " +
        //    "LEFT JOIN cog_BarcodeMaster bm ON TO_CHAR(vi.item_sid) = bm.barcode ";

        public TransferReceiptCommand(string URL, string id, string secret, string resource, string apiURL, string primarySub, string secondarySub, string apiParameter, string grantType = "client_credentials") : base(URL, id, secret, resource, apiURL, primarySub, secondarySub, apiParameter, grantType)
        {
            
        }
        HttpClient client = new HttpClient();


        /// <summary>
        /// Hàm interface các voucher đã generate từ kho đến cửa hàng.
        /// </summary>
        /// <param name="date">Ngày generate voucher</param>
        /// <param name="skuCombineString"></param>
        /// <param name="COGBrandList"></param>
        public void SubmitWH2StoreStransfer(string date, string skuCombineString, string COGBrandList)
        {
            if (DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture, 0, out DateTime voucherDate))
            {
                try
                {
                    GetDailyWHConfirmShipment(COGBrandList); //Get data from WH first
                    List<WHToStoreTransferMaster> wh2storeTransfers = GetDailyWH2Store(date, COGBrandList);
                    foreach (WHToStoreTransferMaster transfer in wh2storeTransfers)
                    {
                        base.GetCOGToken();
                        base.PatchDataToCOG(this.APIURL + this.url + "/" + transfer.consigmentNumber, transfer, true);
                        UdpateSlipToCogStatus(transfer.consigmentNumber, transfer.slipno);
                    }
                }
                catch { throw; }
            }
        }

        /// <summary>
        /// Hàm lấy danh sách các shipment đã được kho confirm & lưu dữ liệu trước qua RetailPro
        /// </summary>
        /// <param name="brandList"></param>
        public void GetDailyWHConfirmShipment(string brandList)
        {
            Configurations config = new Configurations();
            try
            {
                #region Lấy thông tin từ infolog qua RetailPro

                // Lấy ngày confirm_shipment mới nhất của infolog trong INFOLOG_COG_CARTON table
                DateTime? lastDate = InfologCOGCarton.GetLastConfirmShipmentDate();

                // Lấy list carton  - sku  - qty đã xuất từ ngày lastDate trên đến now từ Infolog 
                List<ConfirmShipmentDetail> shipmentDetails = new List<ConfirmShipmentDetail>();
                shipmentDetails = ConfirmShipmentDetail.GetConfirmShipmentDetails(brandList, lastDate);

                // Lấy distinct cartons trong table từ Infolog & lưu vào RP
                var distinctCartons = shipmentDetails.AsQueryable().Select(i => new { i.carton_no, i.cfm_ship_date, i.order_no }).Distinct();
                List<InfologCOGCarton> infologCartons = new List<InfologCOGCarton>();
                //string slipno = "";
                foreach (var item in distinctCartons)
                {
                    OracleTransaction trans = config.CreateTransaction(config.RPConnection);
                    try
                    {
                        // Lưu carton master
                        InfologCOGCarton carton = new InfologCOGCarton(item.carton_no, item.cfm_ship_date, item.order_no);
                        carton.SaveToRetailPro(trans);

                        //lưu vào table infolog_interface_slip
                        //if(slipno!= item.order_no)
                        //{
                        //    string sqlslip = "Insert into infolog_interface_slip values('" + item.order_no + "',to_date('" + DateTime.Now.ToShortDateString() + "', 'MM/DD/YYYY'))";
                        //    config.ExecuteRPCommand(config.RPConnection, sqlslip, null);
                        //}
                        //slipno = item.order_no;

                        // Lưu carton details
                        List<ConfirmShipmentDetail> cartonDetails = shipmentDetails.Where(d => d.carton_no == carton.carton_no && d.order_no==carton.slip_no).ToList();
                        foreach (ConfirmShipmentDetail detail in cartonDetails)
                        {
                            InfologCOGCartonDetail infologCartonDetail = new InfologCOGCartonDetail(carton.id, detail.prod_code, detail.qty);
                            if (!infologCartonDetail.SaveToRetailPro(trans))
                            {
                                trans.Rollback();
                            }
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                }
            }
            catch { throw; }
            #endregion

        }
               

        /// <summary>
        /// Lấy danh sách voucher từ WH đến store đã generate trong ngày
        /// </summary>
        /// <param name="voucherDate">Ngày generate voucher tại cửa hàng</param>
        /// <param name="brandList">Danh sách các brand của COG - định dạng dữ liệu: string - ví dụ: brandList = "'COC', 'TPO'"</param>
        /// <returns></returns>
        private List<WHToStoreTransferMaster> GetDailyWH2Store(string voucherDate, string brandList )
        {
            List<WHToStoreTransferMaster> list = new List<WHToStoreTransferMaster>();
            Configurations config = new Configurations();
            try
            {
                #region Lấy slip theo ngày generate voucher
                
                // Lấy danh sách slip đã generate voucher trong ngày cua RT
                string masterCommand = string.Format(QueryWH2Stores, ICDStoreNo, StoreCodePrefix, "AND trunc(v.created_date) = TO_DATE(' "+ voucherDate + " ', 'MM/DD/YYYY') ");

                DataTable tbSlipMaster = new DataTable();
                tbSlipMaster = config.ExecuteQueryData(config.rPConnection, masterCommand, null);

                foreach(DataRow row in tbSlipMaster.Rows)
                {
                    // Lấy danh sách Consignment_number của COG trong slip
                    DataTable consignments = new DataTable();
                    consignments = InfologCOGCarton.GetConsignmentNumbersInSlip(row["slip_no"].ToString());
                    
                    foreach(DataRow consignmentRow in consignments.Rows)
                    {
                        // Lấy danh sách item trong consignment
                        DataTable details = new DataTable();
                        details = InfologCOGCartonDetail.GetConsignmentDetails(consignmentRow["consignment_number"].ToString(), row["slip_no"].ToString());

                        list.Add(new WHToStoreTransferMaster(row["in_glob_store_code"].ToString(), consignmentRow["consignment_number"].ToString(), row["slip_no"].ToString(), details));
                    }
                }
                return list;

                #endregion 
            }
            catch (Exception ex) { throw ex; }
        }

        public void UdpateSlipToCogStatus(string consignment_number, string slip_no)
        {           
            Configurations config = new Configurations();
            string sql = string.Format(updatestatus, consignment_number, slip_no, DateTime.Parse(DateTime.Now.ToShortDateString()));
            config.ExecuteRPCommand(config.RPConnection, sql, null);
        }

        //public string TransferReceipt()
        //{
        //    string author= base.GetCOGTokenForSendFile();          

        //    var header = new TransferReceiptHeader()
        //    {
        //        Authorization= "Bearer " + author,                
        //        OcpApimSubscriptionKey = PrimarySubcriptionKey,
        //        DC_Code= dc_code                
        //    };

        //    var skus = new TransferReceiptSku[]
        //        {
        //            new TransferReceiptSku{ sku="1234",QuantityReceived=1 },
        //            new TransferReceiptSku{ sku="12345",QuantityReceived=10 },

        //        };

        //    var obj = new TransferReceiptDetail
        //    {
        //        hearder=header,
        //        storeCode = "C001",
        //        Skus=skus
        //    };
        //    var json = new JavaScriptSerializer().Serialize(obj);
        //    File.WriteAllText(@"D:\json.json", json);

        //    return json;
        //}

        //public void  TransferReceiptMain()
        //{
        //    string mess = TransferReceipt();
        //    base.SubmitAPIToCOG(APIURL+urlAcknowledgement, mess);

        //}

    }
}
