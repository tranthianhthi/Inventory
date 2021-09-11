using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COGInterfaceCommand.Common
{
    public class InfologCOGCarton
    {
        public int id { get; set; }
        public string carton_no { get; set; }
        public DateTime confirm_shipment_date { get; set; }
        public string COG_ASN_Number { get; set; }
        public string consignment_number { get; set; }
        public string slip_no { get; set; }
        public int sent { get; set; }

        public InfologCOGCarton() { }
        public InfologCOGCarton(string cartonNo, DateTime confirmDateTime, string slip_no)
        {
            this.carton_no = cartonNo;
            this.confirm_shipment_date = confirmDateTime;
            this.slip_no = slip_no;
        }

        private static string QueryLastConfirmShipmentDate = "SELECT max(confirm_shipment_date) as LastDate FROM INFOLOG_COG_CARTON ";

        private static string QueryASNFromCartonNo = "SELECT asn.ASN_Number, asn.consignment_note FROM COG_ASN_CARTON car INNER JOIN COG_ASN asn ON car.asn_number = asn.asn_number WHERE LICENSE_PLATE_REF = '{0}'";
        private static string InsertCOGCartonToRetailPro = "INSERT INTO Infolog_COG_Carton ( carton_no, confirm_shipment_date, cog_asn_number, slip_no, consignment_number, sent ) VALUES ( '{0}', {1}, '{2}', '{3}', '{4}', 0 ) RETURNING ID INTO :cartonID ";
        private static string QueryConsignmentStatus = "SELECT DISTINCT consignment_number FROM Infolog_COG_Carton WHERE Sent = 0 AND Slip_no = {0} ";

        /// <summary>
        /// Lấy ngày confirm_shipment cuối cùng trong bảng COG_CARTON ( ~ lần cuối sync dữ liệu )
        /// </summary>
        /// <returns>DateTime? -> NULL nếu là lần đầu lấy dữ liệu</returns>
        public static DateTime? GetLastConfirmShipmentDate()
        {
            DateTime? date;

            Configurations config = new Configurations();
            DataTable tb = new DataTable();

            try
            {
                tb = config.ExecuteQueryData(config.RPConnection, QueryLastConfirmShipmentDate, null);
                date = (DateTime?)tb.Rows[0]["LastDate"];
            }
            catch { throw; }
            return date;
        }

        /// <summary>
        /// Lưu object vào database
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public bool SaveToRetailPro(OracleTransaction trans)
        {
            Configurations config = new Configurations();
            DataTable tb = new DataTable();

            try
            {
                // Lấy asn dựa vào carton number
                string command = string.Format(QueryASNFromCartonNo, carton_no);
                tb = config.ExecuteQueryData(config.RPConnection, QueryASNFromCartonNo, null);
                this.COG_ASN_Number = tb.Rows.Count == 1 ? tb.Rows[0]["asn_number"].ToString() : "";
                this.consignment_number = tb.Rows.Count == 1 ? tb.Rows[0]["consignment_note"].ToString() : "";

                if (string.IsNullOrEmpty(this.COG_ASN_Number + this.consignment_number))
                {
                    return false;
                }

                // Lưu carton vào bảng master - dùng transaction
                List<OracleParameter> paras = new List<OracleParameter>() { new OracleParameter("cartonID", OracleDbType.Int32, ParameterDirection.Output) };
                
                command = string.Format(InsertCOGCartonToRetailPro
                    , carton_no
                    , "TO_DATE('" + confirm_shipment_date.ToString("MM/dd/yyyy") + "', 'MM/DD/YYYY')"
                    , this.COG_ASN_Number
                    , slip_no
                    , this.consignment_number );
                int result = config.ExecuteRPCommand(trans, command, paras);
                id = (int)paras[0].Value;

                return result == 1;
            }
            catch { throw; }
        }

        public static DataTable GetConsignmentNumbersInSlip(string slip_no)
        {
            Configurations config = new Configurations();
            DataTable tb = new DataTable();
            string command = string.Format(QueryConsignmentStatus, slip_no);

            try
            {
                tb = config.ExecuteQueryData(config.RPConnection, command, null);
                return tb;
            }
            catch { throw; }
        }

    }
}
