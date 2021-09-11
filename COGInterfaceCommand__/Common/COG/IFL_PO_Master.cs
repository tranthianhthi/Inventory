using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.IO;

namespace COGInterfaceCommand.Common.COG
{
    public class IFL_PO_Master 
    {       
        public char Mode { get; set; }
        public string WareHouse { get; set; }
        public string Owner { get; set; }
        public string PO { get; set; }
        public DateTime PODate { get; set; }
        public int POLine { get; set; }
        public int Qty { get; set; }
        public string Cog_Asn_Number { get; set; }
        public string Cog_Document_Name { get; set; }
        public DateTime Cog_Asn_CreatedDate { get; set; }        

        private string InsertCommand = "INSERT INTO COG_IFL_PO (WareHouse,Owner,PO,PO_Date,PO_Line_No," +
            "Qty,Cog_Asn_Number,Cog_Document_Name,Cog_Asn_CreatedDate) values(";

        private string DeleteCommand = "DELETE  COG_IFL_PO";       

        public IFL_PO_Master(DataRow row, int poline,char mode)
        {
            if(row!=null)
            {
                Mode = mode;
                WareHouse = "ACFC";
                Owner = "ACFC";
                PO = "COG" + DateTime.Now.Year.ToString().Substring(2) + "-" + row["ASN_NUMBER"].ToString() + "-RT";
                PODate = DateTime.Now.Date;
                POLine = poline;
                Qty = int.Parse(row["QTY"].ToString());
                Cog_Asn_Number = row["ASN_NUMBER"].ToString();
                Cog_Document_Name = row["DOCUMENT_NAME"].ToString();
                Cog_Asn_CreatedDate = DateTime.Parse(row["CREATED_DATE"].ToString());
            }
          
        }
        private void AddToDataBase()
        {
            Configurations config = new Configurations();
            string command = InsertCommand;
            command += "'" + config.ConvertStringToOracleString(WareHouse) + "', ";
            command += "'" + config.ConvertStringToOracleString(Owner) + "', ";
            command += "'" + config.ConvertStringToOracleString(PO) + "' ";
            command += ",TO_DATE('" + PODate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS' )";
            command += ", " + POLine + " ";
            command += ", " + Qty + " ";
            command += ",'" + config.ConvertStringToOracleString(Cog_Asn_Number) + "' ";
            command += ",'" + config.ConvertStringToOracleString(Cog_Document_Name) + "' ";
            command += ",TO_DATE('" + Cog_Asn_CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS' ))";
            try
            {
                config.ExecuteRPCommand(config.RPConnection, command,null);
            }
            catch (Exception ex)
            {
                throw ;
            }
        }
        private void DeleteFromDataBase(string asn, string docname)
        {
            Configurations config = new Configurations();
            string command = DeleteCommand;
            command += " WHERE Cog_Asn_Number= '" + config.ConvertStringToOracleString(asn) + "' ";
            command += " AND Cog_Document_Name= '" + config.ConvertStringToOracleString(docname) + "' ";
            try
            {
                config.ExecuteRPCommand(config.RPConnection, command, null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void ProcessObject(string asn, string docname)
        {            
            switch (Mode)
            {
                case 'A':                   
                    AddToDataBase();                   
                    break;
                case 'D':
                    DeleteFromDataBase(asn, docname);
                    AddToDataBase();
                    break;
                default:
                    break;
            }
        }

        public void DeleteProcessObject(string asn, string docname)
        {           
             DeleteExistFromDataBase(asn, docname);
        }

        private void DeleteExistFromDataBase(string asn, string docname)
        {
            Configurations config = new Configurations();
            string sqlpo = "DELETE COG_IFL_PO WHERE COG_ASN_NUMBER= '" + config.ConvertStringToOracleString(asn) + "' AND COG_DOCUMENT_NAME= '" + config.ConvertStringToOracleString(docname) + "' ";
            string sqlpkl = "DELETE COG_IFL_PKL WHERE COG_ASN_NUMBER= '" + config.ConvertStringToOracleString(asn) + "' AND COG_DOCUMENT_NAME= '" + config.ConvertStringToOracleString(docname) + "' ";
            string sqlsa = "DELETE COG_IFL_SA WHERE COG_ASN_NUMBER= '" + config.ConvertStringToOracleString(asn) + "' AND COG_ASN_DOCUMENT_NAME= '" + config.ConvertStringToOracleString(docname) + "' ";

            try
            {
                config.ExecuteRPCommand(config.RPConnection, sqlpo, null);
                config.ExecuteRPCommand(config.RPConnection, sqlpkl, null);
                config.ExecuteRPCommand(config.RPConnection, sqlsa, null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void CreateCSVFile(DataTable dt, string strFilePath)
        {
            try
            {
                StreamWriter sw = new StreamWriter(strFilePath, false);
                int columnCount = dt.Columns.Count;

                for (int i = 0; i < columnCount; i++)
                {
                    sw.Write(dt.Columns[i]);

                    if (i < columnCount - 1)
                    {
                        sw.Write(",");
                    }
                }

                sw.Write(sw.NewLine);

                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            sw.Write(dr[i].ToString());
                        }

                        if (i < columnCount - 1)
                        {
                            sw.Write(",");
                        }
                    }

                    sw.Write(sw.NewLine);
                }

                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
