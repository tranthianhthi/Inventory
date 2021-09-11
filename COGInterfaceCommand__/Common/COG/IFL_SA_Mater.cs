using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace COGInterfaceCommand.Common.COG
{
   public class IFL_SA_Mater
    {
        public char Mode { get; set; }
        public string Warehouse { get; set; }
        public string TransCode { get; set; }
        public string Reamrks1 { get; set; }
        public string Reamrks2 { get; set; }
        public string AllocationType { get; set; }
        public string OrderRef { get; set; }
        public DateTime ReqDeliveryDate { get; set; }
        public string Barcode { get; set; }
        public string LcoationCode { get; set; }
        public string InvStatus { get; set; }
        public string CartonID { get; set; }
        public int PackBeforeShip { get; set; }
        public int StoreCodeQty { get; set; }       
        public DateTime IFL_SA_Date { get; set; }
        public string Cog_Asn_Number { get; set; }
        public string Cog_Document_Name { get; set; }


        private string InsertCommand = "INSERT INTO COG_IFL_SA (WAREHOUSE,TRANS_CODE,REMARKS1,ODERREF,REQ_DELIVERY_DATE,BARCODE," +
            "PACKBEFORESHIP,COT01,COG_Asn_Number,Cog_ASN_Document_Name,REMARKS2) values(";

        private string DeleteCommand = "DELETE FROM  COG_IFL_SA";
        private string UpdateCommand = " UPDATE  COG_IFL_SA";

        public IFL_SA_Mater(DataRow row, int poline, char mode, string asn, string docname)
        {            
            Mode = mode;
            Warehouse = "ACFC";
            TransCode = "SOD";
            Reamrks1 = "COT";
            Reamrks2 = asn;
            OrderRef = asn+"-"+docname;
            ReqDeliveryDate = DateTime.Now.AddDays(2).Date;
            Barcode = row["Barcode"].ToString();
            PackBeforeShip = 1;
            StoreCodeQty = int.Parse(row["Qty"].ToString());
            IFL_SA_Date = DateTime.Now.Date;
            Cog_Asn_Number = asn;
            Cog_Document_Name = docname;
        }
        private void AddToDataBase(OracleTransaction trans = null)
        {
            Configurations config = new Configurations();
            string command = InsertCommand;
            command += "'" + config.ConvertStringToOracleString(Warehouse) + "', ";
            command += "'" + config.ConvertStringToOracleString(TransCode) + "', ";
            command += "'" + config.ConvertStringToOracleString(Reamrks1) + "', ";
            command += "'" + config.ConvertStringToOracleString(OrderRef) + "', ";
            command += "TO_DATE('" + ReqDeliveryDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS'),";
            command += "'" + config.ConvertStringToOracleString(Barcode) + "' ";
            //command += "'" + config.ConvertStringToOracleString(Cog_LotNo) + "', ";
            //command += "TO_DATE('" + Cog_ExpiryDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS' )";           
            command += ", " + PackBeforeShip + " ";
            command += ", " + StoreCodeQty + " ";
            command += ",'" + config.ConvertStringToOracleString(Cog_Asn_Number) + "' ";
            command += ",'" + config.ConvertStringToOracleString(Cog_Document_Name) + "', ";
            command += "'" + config.ConvertStringToOracleString(Reamrks2) + "') ";
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
            command += " AND COG_ASN_DOCUMENT_NAME= '" + config.ConvertStringToOracleString(docname) + "' ";
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
