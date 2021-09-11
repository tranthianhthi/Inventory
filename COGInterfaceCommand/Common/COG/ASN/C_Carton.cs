using System;
using Oracle.ManagedDataAccess.Client;


namespace COGInterfaceCommand.Common.COG.ASN
{
    public class C_Carton : ICOGItem 
    {
        public string ASN_Number { get; set; }
        /// <summary>
        /// Mã barcode của mỗi thùng
        /// </summary>
        public string license_plate_ref { get; set; }
        public string order_number { get; set; }
        public int seq_number { get; set; }
        public string COGFileName { get; set; }
        public DateTime CreatedDate { get; set; }

        private string InsertCommand = "INSERT INTO COG_ASN_CARTON ( " +
            "ASN_Number " +
            ",license_plate_ref " +
            ",order_number " +
            ",seq_number " +      
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
            command += ", '" + license_plate_ref + "'";
            command += ", '" + order_number + "'";
            command += ", " + seq_number + "";
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
           // throw new NotImplementedException();
        }
        public string checkCartonError(C_Carton carton)
        {
            int qty;
            string cartonerrormess = "";
            if (ASN_Number == "") cartonerrormess = cartonerrormess + "ASN Number cannot be null";
            if (license_plate_ref == "") cartonerrormess = cartonerrormess + "license_plate_ref cannot be null";
            if (order_number == "") cartonerrormess = cartonerrormess + "order_number cannot be null";
            if (!int.TryParse(seq_number.ToString(), out qty)) cartonerrormess = cartonerrormess + "seq_number cannot be null";
            if (COGFileName == "") cartonerrormess = cartonerrormess + "COGFileName cannot be null";
            return cartonerrormess;
        }

    }
}
