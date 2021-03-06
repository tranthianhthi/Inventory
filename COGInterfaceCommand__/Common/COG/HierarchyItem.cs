using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace COGInterfaceCommand.Common.COG
{
    public class HierarchyItem : ICOGItem
    {
        public char Mode { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryDesc { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentDesc { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionDesc { get; set; }
        public string COGFileName { get; set; }
        public DateTime CreatedDate { get; set; }

        private string InsertCommand = 
            "INSERT INTO COG_HIERARCHY ( " +
            "CategoryCode, CategoryDesc, DepartmentCode, DepartmentDesc, DivisionCode, DivisionDesc, Created_Date, Document_Name, Active_Flag ) " +
            "VALUES ( ";
        private string UpdateCommand = "";
        private string DeleteCommand = "UPDATE COG_HIERARCHY ";
        private static string hierarchyacknowlegment = "";
        private string errormess = "Failed to load Hierarchy ";
        private string filename = Environment.CurrentDirectory + "\\Uploads\\ACFC_Ack_";//"D://Projects//Documents//Interface CottonOn//ACFC_Ack_";
        private static int totalline = 0;
        public HierarchyItem(string rowText, string file, int line)
        {
            string[] arr = rowText.Split('|');
            Mode = arr[1][0];
            CategoryCode = arr[2];
            CategoryDesc = arr[3];
            DepartmentCode = arr[4];
            DepartmentDesc = arr[5];
            DivisionCode = arr[6];
            DivisionDesc = arr[7];
            CheckHierarchyMaster(rowText, file, line);
        }
        public void CheckHierarchyMaster(string rowText, string file, int line)
        {
            filename = filename + file;
            string[] arr = rowText.Split('|');
            Mode = arr[1][0];
            if (Mode == 'A' || Mode=='C')
            {
                if (arr[1] == "") hierarchyacknowlegment = hierarchyacknowlegment + file + "|N|" + errormess + CategoryCode + " due to missing CategoryCode|0 \r\n";
                if (arr[2] == "") hierarchyacknowlegment = hierarchyacknowlegment + file + "|N|" + errormess + CategoryCode + " due to missing CategoryDesc|0 \r\n";
                if (arr[3] == "") hierarchyacknowlegment = hierarchyacknowlegment + file + "|N|" + errormess + CategoryCode + " due to missing DepartmentCode|0 \r\n";
                if (arr[4] == "") hierarchyacknowlegment = hierarchyacknowlegment + file + "|N|" + errormess + CategoryCode + " due to missing DepartmentDesc|0 \r\n";
                if (arr[5] == "") hierarchyacknowlegment = hierarchyacknowlegment + file + "|N|" + errormess + CategoryCode + " due to missing DivisionCode|0 \r\n";
                if (arr[6] == "") hierarchyacknowlegment = hierarchyacknowlegment + file + "|N|" + errormess + CategoryCode + " due to missing DivisionDesc|0 \r\n";
            }
            if (totalline + 1 == line)
            {
                if (hierarchyacknowlegment != "")
                {
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                        writer.Write(hierarchyacknowlegment);
                }
                else
                {
                    hierarchyacknowlegment = hierarchyacknowlegment + file + "|Y||0";
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                        writer.Write(hierarchyacknowlegment);
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

            command += " '" + config.ConvertStringToOracleString(CategoryCode) + "' ";
            command += ", '" + config.ConvertStringToOracleString(CategoryDesc) + "' ";
            command += ", '" + config.ConvertStringToOracleString(DepartmentCode) + "' ";
            command += ", '" + config.ConvertStringToOracleString(DepartmentDesc) + "' ";
            command += ", '" + config.ConvertStringToOracleString(DivisionCode) + "' ";
            command += ", '" + config.ConvertStringToOracleString(DivisionDesc) + "' ";
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
            string command = InsertCommand;

            command += " SET ";
            command += " Active_Flag = 0 ";
            command += " , Modified_date = TO_DATE('" + CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS')";
            command += " , Modified_doc = '" + config.ConvertStringToOracleString(COGFileName) + "' ";
            command += " WHERE ";
            command += " CategoryCode = " + config.ConvertStringToOracleString(CategoryCode) + "' ";
            command += " AND DepartmentCode = '" + config.ConvertStringToOracleString(DepartmentCode) + "' ";
            command += " AND DivisionCode = '" + config.ConvertStringToOracleString(DivisionCode) + "' ";

            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch { throw; }
        }
    }
}
