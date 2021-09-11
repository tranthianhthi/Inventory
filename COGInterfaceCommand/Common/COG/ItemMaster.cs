using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace COGInterfaceCommand.Common.COG
{
    public class ItemMaster : ICOGItem
    {
        public char Type { get; set; }
        public char Mode { get; set; }        
        public string Brand { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public string SizeDesc { get; set; }
        public string BrandDescription { get; set; }
        public string StyleDescription { get; set; }
        public string ColorDescription { get; set; }
        public string VMTag { get; set; }
        public string HSCode { get; set; }
        public float UnitWeight { get; set; }
        public string CommodityLevelDesc { get; set; }
        public string COGFileName { get; set; }
        public DateTime CreatedDate { get; set; }

        private string InsertCommand = "INSERT INTO COG_ItemMaster ( " +
            "Brand" +
            ", Department" +
            ", Category" +
            ", Style" +
            ", Color" +
            ", SizeDesc" +
            ", BrandDescription" +
            ", StyleDescription" +
            ", ColorDescription" +
            ", VMTag" +
            ", HSCode" +
            ", UnitWeight" +
            ", CommodityLevelDesc" +
            ", Created_Date" +
            ", Document_Name" +
            ", Active_Flag ) " +
            "VALUES (";

        private string UpdateCommand = "";

        private string DeleteCommand = "UPDATE COG_ItemMaster ";
        private static string itemacknowlegment = "";
        private string errormess = "Failed to load item ";
        private string filename = Environment.CurrentDirectory + "\\Uploads\\ACFC_Ack_";//"D://Projects//Documents//Interface CottonOn//ACFC_Ack_";
        private static int totalline = 0;

        public ItemMaster(string rowText, string file, int line)
        {
            
            string[] arr = rowText.Split('|');
            Type = arr[0][0];
            Mode = arr[1][0];
            Brand = arr[3];
            Department = arr[4];
            Category = arr[5];
            Style = arr[6];
            Color = arr[7];
            SizeDesc = arr[8];
            BrandDescription = arr[2];
            StyleDescription = arr[9];
            ColorDescription = arr[10];
            VMTag = arr[11];
            HSCode = arr[12];
            if (arr[13] != null) UnitWeight = string.IsNullOrWhiteSpace(arr[13]) ? (float)0 : float.Parse(arr[13]);
            else UnitWeight=0;

            CommodityLevelDesc = arr[14];
            CheckItemMaster(rowText, file, line);
        }

        private Match RegexMatch( string input, string pattern, RegexOptions regexOptions = RegexOptions.IgnoreCase)
        {
            return Regex.Match(input, pattern, regexOptions);
        }
        public void CheckItemMaster(string rowText, string file, int line)
        {
           
            filename = filename + file;
            string[] arr = rowText.Split('|');
            Mode = arr[1][0];
            if (Mode == 'A' || Mode=='C')
            {   
                if (arr[3] == "")
                {
                    if (itemacknowlegment == "") { itemacknowlegment = itemacknowlegment + file + "|N|" + errormess + Style + " due to missing Brand"; }
                    else{int index = itemacknowlegment.IndexOf(Style + " due to missing Brand", StringComparison.InvariantCulture);
                        if (index < 0){itemacknowlegment = itemacknowlegment + ";" + errormess + Style + " due to missing Brand";}}
                }
                if (arr[4] == "")
                {
                    if (itemacknowlegment == "") { itemacknowlegment = itemacknowlegment + file + "|N|" + errormess + Style + " due to missing Department"; }
                    else{int index = itemacknowlegment.IndexOf(Style + " due to missing Department", StringComparison.InvariantCulture);
                        if (index < 0){itemacknowlegment = itemacknowlegment + ";" + errormess + Style + " due to missing Department";}}
                }
                if (arr[5] == "")
                {
                    if (itemacknowlegment == "") { itemacknowlegment = itemacknowlegment + file + "|N|" + errormess + Style + " due to missing Category"; }
                    else{int index = itemacknowlegment.IndexOf(Style + " due to missing Category", StringComparison.InvariantCulture);
                        if (index < 0){itemacknowlegment = itemacknowlegment + ";" + errormess + Style + " due to missing Category";}}
                }
                if (arr[6] == "")
                {
                    if (itemacknowlegment == "") { itemacknowlegment = itemacknowlegment + file + "|N|" + errormess + Style + " duedue to missing style"; }
                    else{int index = itemacknowlegment.IndexOf(Style + " due to missing style", StringComparison.InvariantCulture);
                        if (index < 0){itemacknowlegment = itemacknowlegment + ";" + errormess + Style + " due to missing style";}
                    }
                    if (Style == "") { itemacknowlegment = itemacknowlegment + ";" + errormess + Style + " due to missing style"; }
                }
                if (arr[7] == "")
                {
                    if (itemacknowlegment == "") { itemacknowlegment = itemacknowlegment + file + "|N|" + errormess + Style + " due to missing color"; }
                    else{int index = itemacknowlegment.IndexOf(Style + " due to missing color", StringComparison.InvariantCulture);
                        if (index < 0){itemacknowlegment = itemacknowlegment + ";" + errormess + Style + " due to missing color";}}
                }
                if (arr[8] == "")
                {
                    if (itemacknowlegment == "") { itemacknowlegment = itemacknowlegment + file + "|N|" + errormess + Style + " due to missing SizeDesc"; }
                    else{int index = itemacknowlegment.IndexOf(Style + " due to missing SizeDesc", StringComparison.InvariantCulture);
                        if (index < 0){itemacknowlegment = itemacknowlegment + ";" + errormess + Style + " due to missing SizeDesc";}}
                }
                if (arr[9] == "")
                {
                   if (itemacknowlegment == "") { itemacknowlegment = itemacknowlegment + file + "|N|" + errormess + Style + " due to missing StyleDescription"; }
                   else { int index = itemacknowlegment.IndexOf(Style + " due to missing StyleDescription", StringComparison.InvariantCulture); 
                        if(index<0){itemacknowlegment = itemacknowlegment + ";" + errormess + Style + " due to missing StyleDescription";}}                    
                }
                if (arr[10] == "")
                {
                    if (itemacknowlegment == "") { itemacknowlegment = itemacknowlegment + file + "|N|" + errormess + Style + " due to missing ColorDescription"; }
                    else{int index = itemacknowlegment.IndexOf(Style + " due to missing ColorDescription", StringComparison.CurrentCultureIgnoreCase);
                        if (index < 0){itemacknowlegment = itemacknowlegment + ";" + errormess + Style + " due to missing ColorDescription";}}                  
                }             
            }
            if (totalline + 1 == line)
            {
                if (itemacknowlegment.Length > 8000) itemacknowlegment = itemacknowlegment.Substring(0, 8000);
                if (itemacknowlegment != ""){
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                        writer.Write(itemacknowlegment.Substring(8000) +"|0"); totalline = -1; itemacknowlegment = "";
                }
                else{
                    itemacknowlegment = itemacknowlegment + file + "|Y||0";
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                        writer.Write(itemacknowlegment); totalline = -1; itemacknowlegment = "";
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

            command += "'" + config.ConvertStringToOracleString(Brand) + "' ";
            command += ", '" + config.ConvertStringToOracleString(Department) + "' ";
            command += ", '" + config.ConvertStringToOracleString(Category) + "' ";
            command += ", '" + config.ConvertStringToOracleString(Style) + "' ";
            command += ", '" + config.ConvertStringToOracleString(Color) + "' ";
            command += ", '" + config.ConvertStringToOracleString(SizeDesc) + "' ";
            command += ", '" + config.ConvertStringToOracleString(BrandDescription) + "' ";
            command += ", '" + config.ConvertStringToOracleString(StyleDescription) + "' ";
            command += ", '" + config.ConvertStringToOracleString(ColorDescription) + "' ";
            command += ", '" + config.ConvertStringToOracleString(VMTag) + "' ";
            command += ", '" + config.ConvertStringToOracleString(HSCode) + "' ";
            command += ", " + UnitWeight + " ";
            command += ", '" + config.ConvertStringToOracleString(CommodityLevelDesc) + "' ";
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
            //throw new NotImplementedException();
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
            command += " AND Department='" + config.ConvertStringToOracleString(Department) + "' ";
            command += " AND Category='" + config.ConvertStringToOracleString(Category) + "' ";
            command += " AND Style='" + config.ConvertStringToOracleString(Style) + "' ";
            command += " AND Color='" + config.ConvertStringToOracleString(Color) + "' ";
            command += " AND SizeDesc='" + config.ConvertStringToOracleString(SizeDesc) + "' ";

            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch { throw; }
        }
    }
}
