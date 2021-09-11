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
        private string UpdateCommand = "";
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
                if (arr[3] == "") barcodeacknowlegment = barcodeacknowlegment+ file + "|N|" + errormess + Barcode + " due to missing brand|0 \r\n";
                if (arr[4] == "") barcodeacknowlegment = barcodeacknowlegment+ file + "|N|" + errormess + Barcode + " due to missing style|0 \r\n";
                if (arr[5] == "") barcodeacknowlegment = barcodeacknowlegment + file + "|N|" + errormess + Barcode + " due to missing color|0 \r\n";
                if (arr[6] == "") barcodeacknowlegment = barcodeacknowlegment + file + "|N|" + errormess + Barcode + " due to missing size|0 \r\n";
                if (arr[7] == "") barcodeacknowlegment = barcodeacknowlegment + file + "|N|" + errormess + Barcode + " due to missing barcode|0 \r\n";
                if (arr[8] == "") barcodeacknowlegment = barcodeacknowlegment + file + "|N|" + errormess + Barcode + " due to missing primary|0 \r\n";               
            }
            if (totalline+1 == line)
            {
                if (barcodeacknowlegment != "")
                {
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                        writer.Write(barcodeacknowlegment);
                }
                else
                {
                    barcodeacknowlegment = barcodeacknowlegment + file + "|Y||0";
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                        writer.Write(barcodeacknowlegment);
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
                    AddToDatabase();
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
        private void UpdateToDatabase()
        {
            throw new NotImplementedException();
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

            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch { throw; }
        }
    }
}
