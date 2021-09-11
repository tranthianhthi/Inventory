using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace COGInterfaceCommand.Common.COG
{
    public class PricingMaster : ICOGItem
    {
        public char Type { get; set; }
        public char Mode { get; set; }
        public string BrandDescription { get; set; }
        public string Brand { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public string SizeDesc { get; set; }
        public string RRPCurrency { get; set; }
        public float RRPCurrent { get; set; }
        public float RRPOrginal { get; set; }
        public string COGFileName { get; set; }
        public DateTime CreatedDate { get; set; }

        string InsertCommand = "INSERT INTO COG_PRICING ( BrandDescription, Brand, Style, Color, SizeDesc, RRPCurrency, RRPCurrent, RRPOrginal, Created_Date, Document_Name, Active_Flag ) VALUES (";
        string UpdateCommand = "";
        string DeleteCommand = "UPDATE COG_PRICING ";
        private static string priceacknowlegment = "";
        private string errormess = "Failed to load Pricing ";
        private string filename = Environment.CurrentDirectory + "\\Uploads\\ACFC_Ack_";//"D://Projects//Documents//Interface CottonOn//ACFC_Ack_";
        private static int totalline = 0;
        public PricingMaster(string rowText, string file, int line)
        {
            string[] arr = rowText.Split('|');

            Type = arr[0][0];
            Mode = arr[1][0];
            BrandDescription = arr[2];
            Brand = arr[3];
            Style = arr[4];
            Color = arr[5];
            SizeDesc = arr[6];
            RRPCurrency = arr[7];
            RRPCurrent = string.IsNullOrWhiteSpace(arr[8]) ? (float)0 : float.Parse(arr[8]);
            RRPOrginal = string.IsNullOrWhiteSpace(arr[9]) ? (float)0 : float.Parse(arr[9]);
            CheckPriceMaster(rowText, file, line);
        }
        public void CheckPriceMaster(string rowText, string file, int line)
        {
            filename = filename + file;
            string[] arr = rowText.Split('|');
            Mode = arr[1][0];
            if (Mode == 'A')
            {
                if (arr[2] == "") priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing BrandDescription|0 \r\n";
                if (arr[3] == "") priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing brand|0 \r\n";
                if (arr[4] == "") priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing style|0 \r\n";
                if (arr[5] == "") priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing color|0 \r\n";
                if (arr[6] == "") priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing size|0 \r\n";               
            }
            if (totalline + 1 == line)
            {
                if (priceacknowlegment != "")
                {
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                        writer.Write(priceacknowlegment);
                }
                else
                {
                    priceacknowlegment = priceacknowlegment + file + "|Y||0";
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                        writer.Write(priceacknowlegment);
                }
            }
            totalline = totalline + 1;
        }

        public void ProcessObject()
        {
            switch (Mode)
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

            command += "'" + config.ConvertStringToOracleString(BrandDescription) + "' ";
            command += ", '" + config.ConvertStringToOracleString(Brand) + "' ";
            command += ", '" + config.ConvertStringToOracleString(Style) + "' ";
            command += ", '" + config.ConvertStringToOracleString(Color) + "' ";
            command += ", '" + config.ConvertStringToOracleString(SizeDesc) + "' ";
            command += ", '" + config.ConvertStringToOracleString(RRPCurrency) + "' ";
            command += ", " + RRPCurrent + " ";
            command += ", " + RRPOrginal + " ";
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
            command += " AND Style" + config.ConvertStringToOracleString(Style) + "' ";
            command += " AND Color" + config.ConvertStringToOracleString(Color) + "' ";
            command += " AND SizeDesc" + config.ConvertStringToOracleString(SizeDesc) + "' ";
            command += " AND RRPCurrency" + config.ConvertStringToOracleString(RRPCurrency) + "' ";

            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch { throw; }
        }
    }
}
