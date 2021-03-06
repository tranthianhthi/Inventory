using System;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace COGInterfaceCommand.Common.COG.ASN
{
    public class A_Header : ICOGItem
    {
        public string ASN_Number { get; set; }
        public DateTime ETA { get; set; }
        public DateTime  ETD { get; set; }
        public string carrier { get; set; }
        public string consignment_note { get; set; }
        public string container_number { get; set; }
        public string destination_location_code { get; set; }
        public string destination_port_code { get; set; }
        public string origin_location_code { get; set; }
        public string origin_port_code { get; set; }
        public string shipment_number { get; set; }
        public string shipment_type { get; set; }
        public int total_cartons { get; set; }
        public int total_units { get; set; }
        public string voyage { get; set; }
        public string COGFileName { get; set; }
        public DateTime CreatedDate { get; set; }

        private string InsertCommand = "INSERT INTO COG_ASN ( " +
            "ASN_Number " +
            ",ETA " +
            ",ETD " +
            ",carrier " +
            ",consignment_note " +
            ",container_number " +
            ",destination_location_code " +
            ",destination_port_code " +
            ",origin_location_code " +
            ",origin_port_code " +
            ",shipment_number " +
            ",shipment_type " +
            ",total_cartons " +
            ",total_units " +
            ",voyage " +
            ",created_date " +
            ",document_name ) " +
            "VALUES ( " ;
        private string UpdateCommand = "";
        private string DeleteCommand = "";
      
        public void AddToDatabase(OracleTransaction trans = null)
        {
            Configurations config = new Configurations();          
            string command = InsertCommand;

            command += "'" + ASN_Number + "'";
            command += ", TO_DATE('" + ETA.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS' )" ;
            command += ", TO_DATE('" + ETD.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS' )";
            command += ", '" + carrier + "'";
            command += ",'" + consignment_note + "'";
            command += ", '" + container_number + "'";
            command += ", '" + destination_location_code + "'";
            command += ",'" + destination_port_code + "'";
            command += ", '" + origin_location_code + "'";
            command += ",'" + origin_port_code + "'";
            command += ",'" + shipment_number + "'";
            command += ",'" + shipment_type + "'";
            command += "," + total_cartons + "";
            command += "," + total_units + "";
            command += ", '" + voyage + "'";
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
           // throw new NotImplementedException();
        }
        public void ProcessObject()
        {
            
        }
        public void UpdateToDatabase()
        {
            //throw new NotImplementedException();
        }
        public string checkHeaderError(A_Header header)        {
            int qty;
            string headerrormess ="";
            if (ASN_Number == "")
            {
                if (headerrormess == "") { headerrormess = headerrormess + "ASN Number cannot be null;"; }
                else
                {
                    int index = headerrormess.IndexOf("ASN Number cannot be null.;", StringComparison.InvariantCulture);
                    if (index < 0) { headerrormess = headerrormess + "ASN Number cannot be null;"; }
                }
            }            
            if (consignment_note == "")
            {
                if (headerrormess == "") { headerrormess = headerrormess + "consignment_note cannot be null;"; }
                else
                {
                    int index = headerrormess.IndexOf("consignment_note cannot be null.;", StringComparison.InvariantCulture);
                    if (index < 0) { headerrormess = headerrormess + "consignment_note cannot be null;"; }
                }
            }   
                   
            if (!int.TryParse(total_cartons.ToString(), out qty))
            {
                if (headerrormess == "") { headerrormess = headerrormess + "total_cartons cannot be null;"; }
                else
                {
                    int index = headerrormess.IndexOf("total_cartons cannot be null;", StringComparison.InvariantCulture);
                    if (index < 0) { headerrormess = headerrormess + "total_cartons cannot be null;"; }
                }
            }   
            if (!int.TryParse(total_units.ToString(), out qty))
            {
                if (headerrormess == "") { headerrormess = headerrormess + "total_units cannot be null;"; }
                else
                {
                    int index = headerrormess.IndexOf("total_units cannot be null;", StringComparison.InvariantCulture);
                    if (index < 0) { headerrormess = headerrormess + "total_units cannot be null;"; }
                }
            }             
            if (COGFileName == "")
            {
                if (headerrormess == "") { headerrormess = headerrormess + "COGFileName cannot be null;"; }
                else
                {
                    int index = headerrormess.IndexOf("COGFileName cannot be null;", StringComparison.InvariantCulture);
                    if (index < 0) { headerrormess = headerrormess + "COGFileName cannot be null;"; }
                }
            }   
            return headerrormess;
        }
        public void DeleteExitAsn(string asnnumber)
        {
            Configurations config = new Configurations();
            string sqlasn = "DELETE COG_ASN WHERE ASN_NUMBER= '" + config.ConvertStringToOracleString(asnnumber) + "' ";
            string sqlasndetail = "DELETE COG_ASN_DETAIL WHERE ASN_NUMBER= '" + config.ConvertStringToOracleString(asnnumber) + "' ";
            string sqlcarton = "DELETE COG_ASN_CARTON WHERE ASN_NUMBER= '" + config.ConvertStringToOracleString(asnnumber) + "' ";
            string sqlasnsku = "DELETE COG_ASN_SKU WHERE ASN_NUMBER= '" + config.ConvertStringToOracleString(asnnumber) + "' ";
            try
            {
                config.ExecuteRPCommand(config.RPConnection, sqlasn, null);
                config.ExecuteRPCommand(config.RPConnection, sqlasndetail, null);
                config.ExecuteRPCommand(config.RPConnection, sqlcarton, null);
                config.ExecuteRPCommand(config.RPConnection, sqlasnsku, null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
