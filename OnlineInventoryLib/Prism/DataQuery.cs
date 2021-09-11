using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace OnlineInventoryLib.Prism
{
    public abstract class DataQuery
    {
        public string connectionString { get; set; }

        public DataQuery(string cnn)
        {
            connectionString = cnn;
        }

        /// <summary>
        /// Query data từ oracle & trả về datatable
        /// </summary>
        /// <param name="cmd">câu sql</param>
        /// <param name="parameters">array tên query param</param>
        /// <param name="values">array value query</param>
        /// <returns></returns>
        public DataTable QueryData(string cmd, string[] parameters, object[] values)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand command = new OracleCommand() { CommandText = cmd, Connection = connection };

            int i = 0;
            while ( i < parameters.Length )
            {
                command.Parameters.Add(new OracleParameter(parameters[i], values[i]));
                i++;
            }
            
            OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(command);

            try
            {
                DataTable tb = new DataTable();
                oracleDataAdapter.Fill(tb);
                return tb;
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                throw ex;
            }
        }

        /// <summary>
        /// Query data từ oracle & trả về datatable
        /// </summary>
        /// <param name="cmd">câu sql</param>
        /// <param name="parameters">array tên query param</param>
        /// <param name="values">array value query</param>
        /// <returns></returns>
        public int ExecuteCommand(string cmd, string[] parameters, object[] values)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand command = new OracleCommand() { CommandText = cmd, Connection = connection };

            int i = 0;
            while (i < parameters.Length)
            {
                command.Parameters.Add(new OracleParameter(parameters[i], values[i]));
                i++;
            }

            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();
                connection.Close();
                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}
