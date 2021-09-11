using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;

namespace CottonOnAPI.Common
{
    public class Configurations
    {
        public string rPConnection =
            "Data Source=" +
            "(" +
            "DESCRIPTION=" +
            "(" +
            "ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=rp.acfc.com.vn)(PORT=1521))" +
            ")" +
            "(" +
            "CONNECT_DATA=(SERVICE_NAME=rproods)" +
            ")" +
            ");" +
            "User Id=reportuser;Password=report";
        //"Data Source=192.168.80.4;Initial Catalog=ACFCInventory;User ID=acfcmango;Password=acfcmango;Pooling=false;";

        public string SelectSlips = "SELECT * FROM Slip WHERE created_date >= :selectdate";
        public string SelectSlipBySlipNo = "SELECT * FROM Slip ";
        public string SelectSlipDetailBySlipID = "SELECT * FROM slip_item ";

        public string RPConnection { get => rPConnection; set => rPConnection = value; }



        /// <summary>
        /// Converts the table to json string.
        /// </summary>
        /// <param name="tb">The tb.</param>
        /// <returns></returns>
        public string ConvertTableToJsonString(DataTable tb)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;

            JsonSerializerSettings format = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            //for (int i = 0; i < 40; i++)
            //{
            foreach (DataRow row in tb.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in tb.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            //}
            return JsonConvert.SerializeObject(parentRow, format);
            //return jsSerializer.Serialize(parentRow);
        }

        /// <summary>
        /// Converts the excel column name to number.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">columnName</exception>
        public int ConvertExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum;
        }

        /// <summary>
        /// Executes the query and get jsonstring as result.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="paras">The paras.</param>
        /// <returns>jsonString of datatables</returns>
        public string ExecuteRPQuery(string connectionString, string commandText, List<OracleParameter> paras)
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

                return ConvertTableToJsonString(tb);
            }
            catch (Exception ex) { throw; }
        }

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


    }
}
