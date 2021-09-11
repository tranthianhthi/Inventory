using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace COGInterfaceCommand.Common.COG
{
    public class BarcodeMaster : ICOGItem
    {
        public char Type { get; set; }
        public char Mode { get; set; }
        public string BrandDescription { get; set; }
        public string Brand { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public string SizeDesc { get; set; }
        public string Barcode { get; set; }
        public char Primary { get; set; }
        public string COGFileName { get; set; }
        public DateTime CreatedDate { get; set; }


        private string InsertCommand = "INSERT INTO COG_BarcodeMaster ( BrandDescription, Brand, Style, Color, SizeDesc, Barcode, Primary, Created_Date, Document_Name, Active_Flag ) VALUES ( ";
        private string UpdateCommand = "UPDATE COG_BarcodeMaster ";
        private string DeleteCommand = "UPDATE COG_BarcodeMaster ";
        private static string barcodeacknowlegment = "";
        private string errormess = "Failed to load Barcode ";
        private string filename = Environment.CurrentDirectory + "\\Uploads\\ACFC_Ack_"; //D://Projects//Documents//Interface CottonOn//ACFC_Ack_";
        private static int totalline = 0;
        public BarcodeMaster(string rowText, string file, int line)
        {
            string[] arr = rowText.Split('|');

            Type = arr[0][0];
            Mode = arr[1][0];
            BrandDescription = arr[2];
            Brand = arr[3];
            Style = arr[4].Trim();
            Color = arr[5].Trim();
            SizeDesc = arr[6].Trim();
            Barcode = arr[7].Trim();
            Primary = arr[8][0];
            CheckBarcodeMaster(rowText,file,line);
        }
               
        public void CheckBarcodeMaster(string rowText, string file, int line)
        {            
            filename = filename + file;        
            string[] arr = rowText.Split('|');
            Mode = arr[1][0];
            if (Mode =='A' || Mode=='C')
            {
                if (arr[3] == "")
                {
                    if (barcodeacknowlegment == "") { barcodeacknowlegment = barcodeacknowlegment + file + "|N|" + errormess + Barcode + " due to missing Brand"; }
                    else{int index = barcodeacknowlegment.IndexOf(Barcode + " due to missing Brand", StringComparison.InvariantCulture);
                        if (index < 0) { barcodeacknowlegment = barcodeacknowlegment + ";" + errormess + Barcode + " due to missing Brand"; }}
                }                
                if (arr[4] == "")
                {
                    if (barcodeacknowlegment == "") { barcodeacknowlegment = barcodeacknowlegment + file + "|N|" + errormess + Barcode + " due to missing Style"; }
                    else{int index = barcodeacknowlegment.IndexOf(Barcode + " due to missing Style", StringComparison.InvariantCulture);
                        if (index < 0) { barcodeacknowlegment = barcodeacknowlegment + ";" + errormess + Barcode + " due to missing Style"; }}
                }                   
                if (arr[5] == "")
                {
                    if (barcodeacknowlegment == "") { barcodeacknowlegment = barcodeacknowlegment + file + "|N|" + errormess + Barcode + " due to missing Color"; }
                    else{int index = barcodeacknowlegment.IndexOf(Barcode + " due to missing Color", StringComparison.InvariantCulture);
                        if (index < 0) { barcodeacknowlegment = barcodeacknowlegment + ";" + errormess + Barcode + " due to missing Color"; }}
                }                   
                if (arr[6] == "")
                {
                    if (barcodeacknowlegment == "") { barcodeacknowlegment = barcodeacknowlegment + file + "|N|" + errormess + Barcode + " due to missing Size"; }
                    else{int index = barcodeacknowlegment.IndexOf(Barcode + " due to missing Size", StringComparison.InvariantCulture);
                        if (index < 0) { barcodeacknowlegment = barcodeacknowlegment + ";" + errormess + Barcode + " due to missing Size"; }}                   
                }                   
                if (arr[7] == "")
                {
                    if (barcodeacknowlegment == "") { barcodeacknowlegment = barcodeacknowlegment + file + "|N|" + errormess + Barcode + " due to missing Barcode"; }
                    else{int index = barcodeacknowlegment.IndexOf(Barcode + " due to missing Barcode", StringComparison.InvariantCulture);
                        if (index < 0) { barcodeacknowlegment = barcodeacknowlegment + ";" + errormess + Barcode + " due to missing Barcode"; }}
                    if (Barcode == "") { barcodeacknowlegment = barcodeacknowlegment + ";" + errormess + Barcode + " due to missing Barcode"; }
                }                   
                if (arr[8] == "")
                {
                    if (barcodeacknowlegment == "") { barcodeacknowlegment = barcodeacknowlegment + file + "|N|" + errormess + Barcode + " due to missing Primary"; }
                    else{int index = barcodeacknowlegment.IndexOf(Barcode + " due to missing Primary", StringComparison.InvariantCulture);
                        if (index < 0) { barcodeacknowlegment = barcodeacknowlegment + ";" + errormess + Barcode + " due to missing Primary"; }}
                }              
            }
            if (totalline+1 == line)
            {
                if (barcodeacknowlegment.Length > 8000) barcodeacknowlegment = barcodeacknowlegment.Substring(0, 8000);
                if (barcodeacknowlegment != ""){using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                        writer.Write(barcodeacknowlegment.Substring(8000) + "|0"); totalline = -1; barcodeacknowlegment = "";
                }
                else{barcodeacknowlegment = barcodeacknowlegment + file + "|Y||0";
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                        writer.Write(barcodeacknowlegment); totalline = -1; barcodeacknowlegment = "";
                }
            }
            totalline = totalline + 1;            
        }
        public void ProcessObject()
        {
            switch(Mode)
            {
                case 'A':
                    AddToDatabase();
                    break;
                case 'C':
                    //UpdateToDatabase();
                    //AddToDatabase();
                    InsertOrUpdate();
                    break;
                case 'D':
                    DeleteFromDatabase();
                    break;
                default:
                    break;
            }
        }
        private void AddToDatabase(OracleTransaction trans = null)
        {
            Configurations config = new Configurations();


            string command = InsertCommand;
            command += "'" + config.ConvertStringToOracleString(BrandDescription)  + "'" ;
            command += ", '" + config.ConvertStringToOracleString(Brand) + "'";
            command += ", '" + config.ConvertStringToOracleString(Style) + "'";
            command += ", '" + config.ConvertStringToOracleString(Color) + "'";
            command += ", '" + config.ConvertStringToOracleString(SizeDesc) + "'";
            command += ", '" + config.ConvertStringToOracleString(Barcode) + "'";
            command += ", '" + Primary + "'";
            command += ", TO_DATE('" + CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS' )";
            command += ", '" + config.ConvertStringToOracleString(COGFileName) + "' ";
            command += ", 1 )";

            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch { throw; }
        }
        private void InsertOrUpdate()
        {
            Configurations config = new Configurations();
            string sql = "select * from COG_BarcodeMaster where ";
            sql += " Brand = '" + config.ConvertStringToOracleString(Brand) + "' ";
            sql += " AND Style = '" + config.ConvertStringToOracleString(Style) + "' ";
            sql += " AND Color = '" + config.ConvertStringToOracleString(Color) + "' ";
            sql += " AND SizeDesc = '" + config.ConvertStringToOracleString(SizeDesc) + "' ";
            sql += " AND Barcode = '" + config.ConvertStringToOracleString(Barcode) + "' ";
            sql += " AND Primary = '" + Primary + "' ";

            var result = config.ExecuteQueryData(config.RPConnection, sql, null);
            if (result.Rows.Count != 0)
                UpdateToDatabase();
            else
                AddToDatabase();
        }
        private void UpdateToDatabase()
        {
            Configurations config = new Configurations();
            string command = UpdateCommand;

            command += " SET ";
            command += "Brand = '" + config.ConvertStringToOracleString(Brand) + "' ";
            command += ",BRANDDESCRIPTION = '" + config.ConvertStringToOracleString(BrandDescription) + "' ";
            command += ",Style = '" + config.ConvertStringToOracleString(Style) + "' ";
            command += ", Color = '" + config.ConvertStringToOracleString(Color) + "' ";
            command += ", SizeDesc = '" + config.ConvertStringToOracleString(SizeDesc) + "' ";
            command += ", Barcode = '" + config.ConvertStringToOracleString(Barcode) + "' ";
            command += ", PRIMARY = '" + Primary + "' ";
            command += " , Modified_Date = TO_DATE('" + CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS' ) ";
            command += " , Modified_Doc = '" + config.ConvertStringToOracleString(COGFileName) + "' ";
            command += " WHERE ";
            command += " Brand = '" + config.ConvertStringToOracleString(Brand) + "' ";
            command += " AND Style = '" + config.ConvertStringToOracleString(Style) + "' ";
            command += " AND Color = '" + config.ConvertStringToOracleString(Color) + "' ";
            command += " AND SizeDesc = '" + config.ConvertStringToOracleString(SizeDesc) + "' ";
            command += " AND Barcode = '" + config.ConvertStringToOracleString(Barcode) + "' ";

            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch { throw; }
        }
        private void DeleteFromDatabase()
        {
            Configurations config = new Configurations();
            string command = DeleteCommand;

            command += " SET ";
            command += " Active_Flag = 0 ";
            command += " , Modified_Date = TO_DATE('" + CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS' ) ";
            command += " , Modified_Doc = '" + config.ConvertStringToOracleString(COGFileName) + "' ";
            command += " WHERE ";
            command += " Brand = '" + config.ConvertStringToOracleString(Brand) + "' ";
            command += " AND Style = '" + config.ConvertStringToOracleString(Style) + "' ";
            command += " AND Color = '" + config.ConvertStringToOracleString(Color) + "' ";
            command += " AND SizeDesc = '" + config.ConvertStringToOracleString(SizeDesc) + "' ";
            command += " AND Barcode = '" + config.ConvertStringToOracleString(Barcode) + "' ";
            command += " AND Primary = '" + Primary + "' ";

            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch { throw; }
        }
    }
}
