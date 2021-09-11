using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        string InsertCommand = "INSERT INTO COG_PRICING ( BrandDescription, Brand, Style, Color, SizeDesc, RRPCurrency, RRPCurrent, RRPOrginal, Created_Date, Document_Name, Active_Flag,STARTDATE,ENDDATE ) VALUES (";
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
            StartDate = string.IsNullOrEmpty(arr[10]) ? DateTime.Now : DateTime.ParseExact(arr[10], "dd/MM/yyyy", CultureInfo.InvariantCulture); 
            EndDate = string.IsNullOrEmpty(arr[11]) ? DateTime.Now : DateTime.ParseExact(arr[11], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            CheckPriceMaster(rowText, file, line);
        }
        public void CheckPriceMaster(string rowText, string file, int line)
        {
            filename = filename + file;
            string[] arr = rowText.Split('|');
            Mode = arr[1][0];
            if (Mode == 'A')
            {
                if (arr[2] == "")
                {
                    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing BrandDescription"; }
                        else{int index = priceacknowlegment.IndexOf(Style + " due to missing BrandDescription", StringComparison.InvariantCulture);
                            if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing BrandDescription"; }}
                    
                }                   
                if (arr[3] == "")
                {                   
                    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing brand"; }
                    else{int index = priceacknowlegment.IndexOf(Style + " due to missing brand", StringComparison.InvariantCulture);
                        if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing brand"; }}                    
                }                    
                if (arr[4] == "")
                {                  
                    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing Style"; }
                    else{int index = priceacknowlegment.IndexOf(Style + " due to missing Style", StringComparison.InvariantCulture);
                        if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing Style"; }}
                    if (Style == "") { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing style"; }
                }                   
                if (arr[5] == "")
                {                    
                    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing Color"; }
                    else{int index = priceacknowlegment.IndexOf(Style + " due to missing Color", StringComparison.InvariantCulture);
                        if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing Color"; }}                    
                }                   
                if (arr[6] == "")
                {                    
                    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing Size"; }
                    else{int index = priceacknowlegment.IndexOf(Style + " due to missing Size", StringComparison.InvariantCulture);
                        if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing Size"; }}                    
                }                
            }
            if (Mode == 'C')
            {
                if (arr[2] == "")
                {
                    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing BrandDescription"; }
                    else
                    {
                        int index = priceacknowlegment.IndexOf(Style + " due to missing BrandDescription", StringComparison.InvariantCulture);
                        if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing BrandDescription"; }
                    }

                }
                if (arr[3] == "")
                {
                    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing brand"; }
                    else
                    {
                        int index = priceacknowlegment.IndexOf(Style + " due to missing brand", StringComparison.InvariantCulture);
                        if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing brand"; }
                    }
                }
                if (arr[4] == "")
                {
                    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing Style"; }
                    else
                    {
                        int index = priceacknowlegment.IndexOf(Style + " due to missing Style", StringComparison.InvariantCulture);
                        if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing Style"; }
                    }
                    if (Style == "") { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing style"; }
                }
                if (arr[5] == "")
                {
                    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing Color"; }
                    else
                    {
                        int index = priceacknowlegment.IndexOf(Style + " due to missing Color", StringComparison.InvariantCulture);
                        if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing Color"; }
                    }
                }
                if (arr[6] == "")
                {
                    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing Size"; }
                    else
                    {
                        int index = priceacknowlegment.IndexOf(Style + " due to missing Size", StringComparison.InvariantCulture);
                        if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing Size"; }
                    }
                }
                //if (RRPCurrent == 0)
                //{
                //    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing RRPCurrent"; }
                //    else{int index = priceacknowlegment.IndexOf(Style + " due to missing RRPCurrent", StringComparison.InvariantCulture);
                //        if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing RRPCurrent"; }}
                //}
                if (RRPOrginal == 0 && RRPCurrent==0)
                {
                    if (priceacknowlegment == "") { priceacknowlegment = priceacknowlegment + file + "|N|" + errormess + Style + " due to missing RRPOrginal"; }
                    else
                    {
                        int index = priceacknowlegment.IndexOf(Style + " due to missing RRPOrginal and RRPCurrent", StringComparison.InvariantCulture);
                        if (index < 0) { priceacknowlegment = priceacknowlegment + ";" + errormess + Style + " due to missing RRPOrginal and RRPCurrent"; }
                    }
                }
            }

            if (totalline + 1 == line)
            {
                if(priceacknowlegment != "")
                {
                    if (priceacknowlegment.Length > 8000) priceacknowlegment = priceacknowlegment.Substring(0, 8000);
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                    writer.Write(priceacknowlegment + "|0"); totalline = -1; priceacknowlegment = "";
                }
                else
                {priceacknowlegment = priceacknowlegment + file + "|Y||0";
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                        writer.Write(priceacknowlegment); totalline = -1; priceacknowlegment = "";
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
                    DeleteFromDatabase();
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
            command += ", 1 ";
            command += ", TO_DATE('" + StartDate.ToString("yyyy-MM-dd") + "', 'YYYY-MM-DD' )";
            command += ", TO_DATE('" + EndDate.ToString("yyyy-MM-dd") + "', 'YYYY-MM-DD' ))";

            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch { throw; }
        }
        private void UpdateToDatabase()
        {
           // throw new NotImplementedException();
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
            command += " AND Style='" + config.ConvertStringToOracleString(Style) + "' ";
            command += " AND Color='" + config.ConvertStringToOracleString(Color) + "' ";
            command += " AND SizeDesc='" + config.ConvertStringToOracleString(SizeDesc) + "' ";
            command += " AND RRPCurrency='" + config.ConvertStringToOracleString(RRPCurrency) + "' ";
            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch { throw; }
        }
    }
}
