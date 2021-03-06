using System;
using System.Collections.Generic;
using System.Data;

namespace COGInterfaceCommand.Common
{
    public class ConfirmShipmentDetail
    {
        public DateTime cfm_ship_date { get; set; }
        public string carton_no { get; set; }
        public string prod_code { get; set; }
        public float qty { get; set; }
        public string order_no { get; set; }



        //private static string QueryVoucherItemFromInfolog =
        //    "select " +
        //            "d.cfm_ship_date, " +
        //            "(select top 1 wpt.serial_no " +
        //                "from wm_prod_tagging wpt " +
        //                "where wpt.owner_code=dld.owner_code " +
        //                    "and wpt.whs_code= dld.whs_code " +
        //                    "and wpt.grn_id= dld.grn_id " +
        //                    "" +
        //                    "and wpt.to_serial_no= dld.serial_no " +
        //                    "and wpt.prod_code= dld.prod_code " +
        //            ") carton_no, " +
        //            "dld.prod_code,  " +
        //            "s.order_no, " +
        //            "sum(dld.qty_shipped) as qty " +
        //    "from do_line_d dld " +
        //    "join do d on d.id= dld.do_id " +
        //    "join so s on s.id=d.so_id " +
        //    "where s.remarks1 IN ( {0} ) " +
        //    "{1} " +
        //    "group by d.cfm_ship_date, dld.owner_code, dld.whs_code, dld.grn_id, dld.pack_id, dld.serial_no, dld.prod_code, s.order_no ";

        private static string QueryVoucherItemFromInfolog = "select d.cfm_ship_date,"+
            "(select top 1 wpt.serial_no from wm_prod_tagging wpt where wpt.owner_code= dld.owner_code and wpt.whs_code= dld.whs_code"+
        " and wpt.grn_id= dld.grn_id and wpt.to_serial_no= dld.serial_no and wpt.prod_code= dld.prod_code) as carton_no, "+
        "(select top 1 wpl.tag_field1 from wm_pack_list wpl where wpl.owner_code=dld.owner_code and wpl.whs_code= dld.whs_code"+
    " and wpl.carton_id= dld.serial_no and wpl.sku= dld.prod_code) as carton_no1, dld.prod_code,  s.order_no, "+
    "sum(dld.qty_shipped) as qty from do_line_d dld join do d on d.id= dld.do_id join so s on s.id=d.so_id where s.remarks1 IN ( {0} ) " +
    " {1} "+
    "group by d.cfm_ship_date, dld.owner_code, dld.whs_code, dld.grn_id, dld.pack_id, dld.serial_no, dld.prod_code, s.order_no";

        public ConfirmShipmentDetail()
        {

        }
        public ConfirmShipmentDetail(DataRow row)
        {
            cfm_ship_date = (DateTime)row["cfm_ship_date"];
            if (row["carton_no"].ToString() == "") { carton_no= row["carton_no1"].ToString(); } else { carton_no = row["carton_no"].ToString(); }            
            prod_code = row["prod_code"].ToString();
            qty = float.Parse(row["qty"].ToString());
            order_no = row["order_no"].ToString();
        }

        /// <summary>
        /// Lấy thông tin confirm shipment từ infolog
        /// </summary>
        /// <param name="brandList">Danh sách các brand của COG - định dạng chuỗi. Ví dụ: brandList=""'COC', 'TPO'""</param>
        /// <param name="lastDate">Lần sync dữ liệu cuối cùng - NULL nếu là lần đầu tiên</param>
        /// <returns></returns>
        public static List<ConfirmShipmentDetail> GetConfirmShipmentDetails(string brandList, DateTime? lastDate)
        {
            List<ConfirmShipmentDetail> details = new List<ConfirmShipmentDetail>();

            Configurations config = new Configurations();
            string infologCommand = QueryVoucherItemFromInfolog;
            if (lastDate.HasValue)
            {
                infologCommand = string.Format(infologCommand, brandList, "AND d.cfm_ship_date >= '" + lastDate.Value.ToString("MM/dd/yyyy") + " 23:59:59'");
                //infologCommand = string.Format(infologCommand, brandList, " AND d.cfm_ship_date > '11/12/2019'  and s.order_no in ('S190209309' ) ");
            }
            else
            {
                infologCommand = string.Format(infologCommand, brandList, "");
            }
            //infologCommand = "select * from infolog_table";
            DataTable tbInfolog = new DataTable();
            try
            {
                tbInfolog = config.ExecuteSQLServerCommand(config.infologConnection, infologCommand);
                //tbInfolog = config.ExecuteQueryData(config.rPConnection, infologCommand, null);
                foreach (DataRow row in tbInfolog.Rows)
                {
                    details.Add(new ConfirmShipmentDetail(row));
                }
                return details;
            }
            catch { throw; }
        }
    }
}
