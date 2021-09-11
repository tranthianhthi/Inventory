using System;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PrismDataProvider
{
    public class PrismConnection
    {
        private string cnString = "Data Source=" +
            "(" +
            "DESCRIPTION=" +
            "(" +
            "ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))" +
            ")" +
            "(" +
            "CONNECT_DATA=(SERVICE_NAME={2})" +
            ")" +
            ");" +
            "User Id={3};Password={4};Connection Timeout={5}";

        private string host;
        private int port;
        private string dbName;
        private string userName;
        private string password;
        private int timeOut;

        public PrismConnection(string host, int port, string dbName, string userName, string password, int timeOut)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentException($"'{nameof(host)}' cannot be null or empty", nameof(host));
            }

            if (port == 0)
            {
                throw new ArgumentException($"'{nameof(port)}' cannot be 0", nameof(host));
            }

            if (timeOut == 0)
            {
                throw new ArgumentException($"'{nameof(timeOut)}' cannot be 0", nameof(host));
            }

            if (string.IsNullOrEmpty(dbName))
            {
                throw new ArgumentException($"'{nameof(dbName)}' cannot be null or empty", nameof(dbName));
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException($"'{nameof(userName)}' cannot be null or empty", nameof(userName));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty", nameof(password));
            }

            this.host = host;
            this.port = port;
            this.dbName = dbName;
            this.userName = userName;
            this.password = password;
            this.timeOut = timeOut;
        }

        //public string Host { get => host; set => host = value; }

        //[DefaultValue(1521)]
        //public int Port { get => port; set => port = value; }


        //public string DBName { get => dbName; set => dbName = value; }
        //public string UserName { get => userName; set => userName = value; }
        //public string Password { get => password; set => password = value; }
        //public int TimeOut { get => timeOut; set => timeOut = value; }

        public OracleConnection GetConnection()
        {
            OracleConnection connection = new OracleConnection(string.Format(cnString, host, port, dbName, userName, password, timeOut));
            return connection;
        }
    }
}
