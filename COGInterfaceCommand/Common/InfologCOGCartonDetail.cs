using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COGInterfaceCommand.Common
{
    public class InfologCOGCartonDetail
    {
        public int id { get; set; }
        public int carton_id { get; set; }
        public string barcode { get; set; }
        public float qty { get; set; }

        private static string InsertCOGCartonDetailToRetailPro = "INSERT INTO Infolog_COG_Carton_Details ( carton_id, barcode, qty ) VALUES ( {0}, '{1}', {2} ) ";
        private static string GetCOGConsignmentDetails =
            "SELECT d.barcode as sku, d.qty " + 
            "FROM Infolog_COG_Carton c " +
            "INNER JOIN Infolog_COG_Carton_Details d ON c.id = d.carton_id " + //"INNER JOIN COG_BarcodeMaster b ON d.barcode = b.barcode " +
            "WHERE c.consignment_number = '{0}' and c.sent_dated is null and c.Slip_no ='S{1}'";
        
        public InfologCOGCartonDetail()
        {

        }
        public InfologCOGCartonDetail( int carton_id, string barcode, float qty)
        {
            this.carton_id = carton_id;
            this.barcode = barcode;
            this.qty = qty;
        }

        public static DataTable GetConsignmentDetails(string consignment_number, string slipno)
        {
            Configurations config = new Configurations();
            DataTable tb = new DataTable();
            string command = string.Format(GetCOGConsignmentDetails, consignment_number, slipno);

            try
            {
                tb = config.ExecuteQueryData(config.RPConnection, command, null);
                return tb;
            }
            catch { throw; }
        }

        public bool SaveToRetailPro(OracleTransaction trans)
        {
            Configurations config = new Configurations();
            DataTable tb = new DataTable();
            try
            {
                string command = string.Format(InsertCOGCartonDetailToRetailPro, carton_id, barcode, qty);
                int result = config.ExecuteRPCommand(config.RPConnection, command, null);
                return result == 1;
            }
            catch { throw; }
        }
    }
}
