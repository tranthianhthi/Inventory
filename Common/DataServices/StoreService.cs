using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACFC.Common.Interfaces;
using ACFC.Common.Models;
using ACFC.Common.DataAdapter;

using Oracle.ManagedDataAccess.Client;

using System.Data;

namespace ACFC.Common.DataServices
{
    public class StoreService
    {
        public Store FindStore(int storeNo)
        {
            Store store = null;

            DataTable tb = new DataTable();
            string command = "SELECT * FROM store WHERE Active = 1 AND store_no = :storeNo";

            OracleParameter[] paras = new OracleParameter[]
            {
                new OracleParameter("storeNo", storeNo)
            };

            try
            {
                using (OracleConnection conn = new OracleConnection(RPSettings.RPConnectionString))
                {
                    using (DataHelper dh = new DataHelper())
                    {
                        tb = dh.QueryData(command, conn, paras);
                        if (tb.Rows.Count == 1)
                            store = new Store(tb.Rows[0]);
                    }
                }
                return store;
            }
            catch { throw; }

            
        }

        public List<Store> FindStores(string storeCode)
        {
            List<Store> stores = new List<Store>();
            DataTable tb = new DataTable();

            storeCode += "%";
            string command = "SELECT * FROM store WHERE Active = 1 AND store_code LIKE :storeCode";
            OracleParameter[] paras = new OracleParameter[]
            {
                new OracleParameter("storeCode", storeCode)
            };

            try
            {
                using (OracleConnection conn = new OracleConnection(RPSettings.RPConnectionString))
                {
                    using (DataHelper dh = new DataHelper())
                    {
                        tb = dh.QueryData(command, conn, paras);
                        foreach(DataRow row in tb.Rows)
                        {
                            stores.Add(new Store(row));
                        }
                    }
                }
            }
            catch { throw; }

            return stores;
        }

        public List<Store> FindStores()
        {
            List<Store> stores = new List<Store>();
            DataTable tb = new DataTable();

            string command = "SELECT * FROM store WHERE Active = 1 ";
            OracleParameter[] paras = new OracleParameter[0];

            try
            {
                using (OracleConnection conn = new OracleConnection(RPSettings.RPConnectionString))
                {
                    using (DataHelper dh = new DataHelper())
                    {
                        tb = dh.QueryData(command, conn, paras);
                        foreach (DataRow row in tb.Rows)
                        {
                            stores.Add(new Store(row));
                        }
                    }
                }
            }
            catch { throw; }

            return stores;
        }
    }
}
