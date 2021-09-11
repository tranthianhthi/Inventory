using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ACFC.Common.DataAdapter
{
    public class DataHelper : IDisposable 
    {
        public void Dispose()
        {
        }

        public DataTable QueryData(string command, OracleConnection conn, OracleParameter[] paras)
        {
            DataTable tb = new DataTable();
            try
            {
                OracleCommand cmd = new OracleCommand(command, conn);
                foreach(OracleParameter para in paras)
                {
                    cmd.Parameters.Add(para);
                }
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                da.Fill(tb);

                return tb;
            }
            catch { throw; }
        }
    }
}
