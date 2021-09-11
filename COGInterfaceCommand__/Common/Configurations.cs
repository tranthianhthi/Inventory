using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace COGInterfaceCommand.Common
{
    public class Configurations
    {

        public enum COGData
        {
            ASN,
            Barcodes,
            Items,
            Stores
        }

        public string rPConnection =
            "Data Source=" +
            "(" +
            "DESCRIPTION=" +
            "(" +
            "ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))" +
            ")" +
            "(" +
            "CONNECT_DATA=(SERVICE_NAME=rproods)" +
            ")" +
            ");" +
            "User Id=reportuser;Password=report";

        public string infologConnection =
            "Server=112.109.92.58;Database=Infolog;User Id=acfcit;Password=acfcIT123";

        public Configurations()
        {
            rPConnection = string.Format(rPConnection, ConfigurationManager.AppSettings["ServerIP"]);
        }
        //"Data Source=192.168.80.4;Initial Catalog=ACFCInventory;User ID=acfcmango;Password=acfcmango;Pooling=false;";



        public string RPConnection { get => rPConnection; set => rPConnection = value; }

        /* 
         * •	Prefix :- COG_
         * •	File identification :- Pricing
         * •	Destination :- Name of COG business partner
         * •	Postfix :- Date format YYYYMMDDHH24MI
         * •	File Type :- txt
         *  */

        public string Prefix = "COG_";
        public string PartnerName = "ACFC";
        public List<string> FileIdentifications = new List<string>() { "Item", "Barcode", "Pricing", "Hierarchy" };
        public string DateFormat = "YYYYMMDDHH24MI";

        public string folder = "D:\\Projects\\Documents\\Interface CottonOn";

        public string ConvertStringToOracleString(string str)
        {
            string result = str.Replace("'", "''");
            return result;
        }
        public OracleTransaction CreateTransaction(string connectionString)
        {
            try
            {
                OracleConnection conn = new OracleConnection(connectionString);
                conn.Open();
                OracleTransaction trans = conn.BeginTransaction();
                return trans;
            }
            catch(Exception ex) { throw; }
        }
        public void CommitTransaction(OracleTransaction trans, bool isSuccess)
        {
            try
            {
                if (isSuccess)
                    trans.Commit();
                else
                    trans.Rollback();
            }
            catch { throw; }
        }

        /// <summary>
        /// Chạy câu lệnh Oracle đơn
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public int ExecuteRPCommand(string connectionString, string commandText, List<OracleParameter> paras)
        {
            int result = 0;
            try
            {
                OracleConnection conn = new OracleConnection(connectionString);
                OracleCommand cmd = new OracleCommand(commandText, conn);

                if (paras == null)
                {

                }
                else
                {
                    foreach (OracleParameter param in paras)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                conn.Open();
                result = cmd.ExecuteNonQuery();
                conn.Close();
                return result;
            }
            catch (Exception ex) { throw; }
        }

        /// <summary>
        /// Chạy câu lệnh Oracle theo transaction
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public int ExecuteRPCommand(OracleTransaction trans, string commandText, List<OracleParameter> paras)
        {
            int result = 0;
            try
            {
                //OracleConnection conn = new OracleConnection(connectionString);
                OracleCommand cmd = new OracleCommand(commandText, trans.Connection);
                cmd.Transaction = trans;

                if (paras == null)
                {

                }
                else
                {
                    foreach (OracleParameter param in paras)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                //conn.Open();
                result = cmd.ExecuteNonQuery();
                //conn.Close();

                return result;
            }
            catch (Exception ex) { throw; }
        }

        /// <summary>
        /// Query data từ Oracle
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public DataTable ExecuteQueryData(string connectionString, string commandText, List<OracleParameter> paras)
        {
            try
            {
                OracleConnection conn = new OracleConnection(connectionString);
                OracleCommand cmd = new OracleCommand(commandText, conn);

                if (paras == null)
                {
                }
                else
                {
                    foreach (OracleParameter param in paras)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                OracleDataAdapter da = new OracleDataAdapter(cmd);

                DataTable tb = new DataTable();
                conn.Open();
                da.Fill(tb);
                conn.Close();

                return tb;
            }
            catch (Exception ex) { throw; }
        }

        public DataTable ExecuteSQLServerCommand(string connectionString, string commandText)
        {
            DataTable tb = new DataTable();

            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(commandText, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            try
            {
                da.Fill(tb);
                return tb;
            }
            catch
            {
                throw;
            }
            
        }
      
    }
}
