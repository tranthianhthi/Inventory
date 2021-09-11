using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace COGInterfaceCommand.Common.COG
{
    public class IFL_PKL_Mater
    {
        public char Mode { get; set; }
        public string Cog_PO_No { get; set; }
        public string Cog_PKL { get; set; }
        public string Carton_Id { get; set; }
        public string Cog_Barcode { get; set; }
        public string Cog_Origin { get; set; }
        //public string Cog_LotNo { get; set; }
        //public DateTime Cog_ExpiryDate { get; set; }
        public int Qty { get; set; }
        public DateTime PKL_Date { get; set; }
        public string Cog_Asn_Number { get; set; }
        public string Cog_Document_Name { get; set; }


        private string InsertCommand = "INSERT INTO COG_IFL_PKL (COG_PO_NO,COG_PKL,CARTON_ID,COG_BARCODE,COG_ORIGIN,QTY,PKL_DATE,Cog_Asn_Number,Cog_Document_Name) values(";

        private string DeleteCommand = "DELETE FROM  COG_IFL_PKL";
        private string UpdateCommand = " UPDATE  COG_IFL_PKL";

        public IFL_PKL_Mater(DataRow row, int poline, char mode, string asn, string docname)
        {            
            Mode = mode;
            Cog_PO_No = "PO-COG" + DateTime.Now.Year.ToString().Substring(2) + "-" + row["style"].ToString() + "-RT";
            Cog_PKL = "PKL-COG" + DateTime.Now.Year.ToString().Substring(2) + "-" + row["style"].ToString() + "-RT";
            Carton_Id =  row["license_plate_ref"].ToString();
            Cog_Barcode = row["barcode"].ToString();
            Cog_Origin = row["origin"].ToString(); 
            //Cog_LotNo = row["Cog_LotNo"].ToString(); 
            //Cog_ExpiryDate = DateTime.Parse(row["Cog_ExpiryDate"].ToString()); 
            Qty = int.Parse(row["Qty"].ToString());
            PKL_Date = DateTime.Now.Date;
            Cog_Asn_Number = asn;
            Cog_Document_Name = docname;
        }
        private void AddToDataBase(OracleTransaction trans = null)
        {
            Configurations config = new Configurations();
            string command = InsertCommand;
            command += "'" + config.ConvertStringToOracleString(Cog_PO_No) + "', ";
            command += "'" + config.ConvertStringToOracleString(Cog_PKL) + "', ";
            command += "'" + config.ConvertStringToOracleString(Carton_Id) + "', ";
            command += "'" + config.ConvertStringToOracleString(Cog_Barcode) + "', ";
            command += "'" + config.ConvertStringToOracleString(Cog_Origin) + "' ";
            //command += "'" + config.ConvertStringToOracleString(Cog_LotNo) + "', ";
            //command += "TO_DATE('" + Cog_ExpiryDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS' )";           
            command += ", " + Qty + " ";          
            command += ",TO_DATE('" + PKL_Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS')";
            command += ",'" + config.ConvertStringToOracleString(Cog_Asn_Number) + "' ";
            command += ",'" + config.ConvertStringToOracleString(Cog_Document_Name) + "') ";
            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void DeleteFromDataBase(string asn, string docname)
        {
            Configurations config = new Configurations();
            string command = DeleteCommand;           
            command += " WHERE  Cog_Asn_Number= '" + config.ConvertStringToOracleString(asn) + "' ";
            command += " AND Cog_Document_Name= '" + config.ConvertStringToOracleString(docname) + "' ";
            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void ProcessObject(string asn, string docname)
        {
           
            switch (Mode)
            {
                case 'A':
                    AddToDataBase();
                    break;
                case 'D':
                    DeleteFromDataBase(asn, docname);
                    AddToDataBase();
                    break;
                default:
                    break;
            }
        }
    }

}
