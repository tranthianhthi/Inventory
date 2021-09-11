using System;
using Oracle.ManagedDataAccess.Client;


namespace COGInterfaceCommand.Common.COG.ASN
{
    public class B_Detail : ICOGItem 
    {
        public string ASN_Number { get; set; }
        public string HBL_Number { get; set; }
        public string commercial_invoice_number { get; set; }
        public string order_number { get; set; }
        public string order_type { get; set; }
        public string COGFileName { get; set; }
        public DateTime CreatedDate { get; set; }

        private string InsertCommand = "INSERT INTO COG_ASN_DETAIL ( " +
            "ASN_Number " +
            ",HBL_Number " +
            ",commercial_invoice_number " +
            ",order_number " +
            ",order_type " +
            ",created_date " +
            ",document_name" +
            ") " +
            "VALUES ( ";
        private string UpdateCommand = "";
        private string DeleteCommand = "";

        public void AddToDatabase(OracleTransaction trans = null)
        {
            Configurations config = new Configurations();
            string command = InsertCommand;

            command += "'" + ASN_Number + "'";
            command += ", '" + HBL_Number + "'";
            command += ", '" + commercial_invoice_number + "'";
            command += ", '" + order_number + "'";
            command += ", '" + order_type + "'";
            command += ", TO_DATE('" + CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS' )";
            command += ", '" + COGFileName + "' )";

            try
            {
                if (trans == null)
                    config.ExecuteRPCommand(config.rPConnection, command, null);
                else
                    config.ExecuteRPCommand(trans, command, null);
            }
            catch { throw; }

        }
        public void DeleteFromDatabase()
        {
            //throw new NotImplementedException();
        }
        public void ProcessObject()
        {
            //throw new NotImplementedException();
        }
        public void UpdateToDatabase()
        {
            //throw new NotImplementedException();
        }
        public string checkDetailError(B_Detail detail)
        {
            int qty;
            string detailerrormess = "";
            if (ASN_Number == "") detailerrormess = detailerrormess + "ASN Number cannot be null";           
            if (order_number == "") detailerrormess = detailerrormess + "order_number cannot be null";          
            if (COGFileName == "") detailerrormess = detailerrormess + "COGFileName cannot be null";
            return detailerrormess;
        }
    }
}
