using System;
using Oracle.ManagedDataAccess.Client;


namespace COGInterfaceCommand.Common.COG.ASN
{
    public class D_SKU : ICOGItem
    {
        public string ASN_Number { get; set; }
        public string Brand { get; set; }
        public string Colour { get; set; }
        public float Invoice_cost { get; set; }
        public string Invoice_currency { get; set; }
        public float RRP { get; set; }
        public string RRP_currency { get; set; }
        public string SizeDesc { get; set; }
        public string Style { get; set; }
        public string license_plate_ref { get; set; }
        public string order_number { get; set; }
        public string product_UOM { get; set; }
        public int seq_number { get; set; }
        public int shipped_qty { get; set; }
        public string SKU { get { return Style + "_" + Colour + "_" + SizeDesc; } }
        public string origin_country_code { get; set; }

        public string COGFileName { get; set; }
        public DateTime CreatedDate { get; set; }

        private string InsertCommand = "INSERT INTO COG_ASN_SKU ( " +
            "ASN_Number " +
            ",Brand " +
            ",Colour " +
            ",Invoice_cost " +
            ",Invoice_currency " +
            ",RRP " +
            ",RRP_currency " +
            ",SizeDesc " +
            ",Style " +
            ",license_plate_ref " +
            ",order_number " +
            ",product_UOM " +
            ",seq_number " +
            ",shipped_qty " +
            ",created_date " +
            ",document_name" +
            ",ORIGIN_COUNTRY_CODE" +
            ") " +
            "VALUES ( ";
        //private string UpdateCommand = "";

        //private string DeleteCommand = "";

        public void AddToDatabase(OracleTransaction trans = null)
        {
            Configurations config = new Configurations();
            string command = InsertCommand;

            command += "'" + ASN_Number + "'";
            command += ", '" + Brand + "'";
            command += ", '" + Colour + "'";
            command += ", " + Invoice_cost + "";
            command += ", '" + Invoice_currency + "'";
            command += ", " + RRP + "";
            command += ", '" + RRP_currency + "'";
            command += ", '" + SizeDesc + "'";
            command += ", '" + Style + "'";
            command += ", '" + license_plate_ref + "'";
            command += ", '" + order_number + "'";
            command += ", '" + product_UOM + "'";
            command += ", " + seq_number + "";
            command += ", " + shipped_qty + "";
            command += ", TO_DATE('" + CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS' )";
            command += ", '" + COGFileName + "'";
            command += ", '" + origin_country_code + "' )"; 

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
           // throw new NotImplementedException();
        }
        public void UpdateToDatabase()
        {
            //throw new NotImplementedException();
        }
        public string checkSkuError(D_SKU sku)
        {
            int qty;
            float rp;
            string skuerrormess = "";
            if (ASN_Number == "") skuerrormess = skuerrormess + "ASN Number cannot be null";
            if (Brand == "") skuerrormess = skuerrormess + "Brand cannot be null";
            if (Colour == "") skuerrormess = skuerrormess + "Colour cannot be null";
            if (SizeDesc == "") skuerrormess = skuerrormess + "SizeDesc cannot be null";
            if (Style == "") skuerrormess = skuerrormess + "Style cannot be null";
            if (license_plate_ref == "") skuerrormess = skuerrormess + "license_plate_ref cannot be null";
            if (origin_country_code == "") skuerrormess = skuerrormess + "origin_country_code cannot be null";
            if (product_UOM == "") skuerrormess = skuerrormess + "product_UOM cannot be null";
            if (!float.TryParse(RRP.ToString(), out rp)) skuerrormess = skuerrormess + "RRP cannot be null";
            if (!int.TryParse(shipped_qty.ToString(), out qty)) skuerrormess = skuerrormess + "shipped_qty cannot be null";
            if (!int.TryParse(seq_number.ToString(), out qty)) skuerrormess = skuerrormess + "seq_number cannot be null";
            if (!int.TryParse(seq_number.ToString(), out qty)) skuerrormess = skuerrormess + "seq_number cannot be null";
            if (order_number == "") skuerrormess = skuerrormess + "order_number cannot be null";
            if (COGFileName == "") skuerrormess = skuerrormess + "COGFileName cannot be null";
            return skuerrormess;
        }
    }
}