using OnlineInventoryLib.Interfaces;
using OnlineInventoryLib.Prism.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineInventoryLib.Prism
{
    public partial class PrismLib : DataQuery, IPrismLib
    {
        public PrismLib(string connectionString) : base(connectionString)
        {
            //this.pickupStore = pickupStore;
        }

        //public string pickupStore { get; set; }

        //public string GetQtyOnHandByDocsCmd = 
            //"SELECT upc, closing as qty FROM table(get_oh_qty(:storeCode)) WHERE upc = :upc ";

        private string GetQtyOnHandByDateTimeCmd = 
            "SELECT q.* " + 
            "FROM RPS.invn_sbs_item_qty q, RPS.invn_sbs_item i, RPS.store s " + 
            "WHERE i.sid = q.invn_sbs_item_sid " + 
            "AND q.store_sid = s.sid " + 
            "AND i.upc = :upc " + 
            "AND s.store_code = :storeCode " + 
            "AND i.modified_datetime >= TO_DATE(:lastSync, 'YYYY/MM/DD HH24:MI:SS')";

        private string GetQtyOnHandMultiStoreCmd =
            "SELECT q.item_sid upc, sum(nvl(qty, 0)) qty " +
            "FROM invn_sbs i, invn_sbs_qty q, store st " +
            "WHERE i.item_sid = q.item_sid " +
            "AND q.store_no = st.store_no AND q.sbs_no = st.sbs_no " +
            "AND st.store_code IN " +
            " ( " +
                " SELECT REGEXP_SUBSTR( :storeCode , '[^,]+', 1, LEVEL) " +
                " FROM dual " +
                " CONNECT BY REGEXP_SUBSTR( :storeCode , '[^,]+', 1, LEVEL) IS NOT NULL " +
            " ) " + 
            " {0} " +
            "GROUP BY q.item_sid " +
            "HAVING sum(nvl(qty, 0)) > 0 ";


        private string GetQtyOnHandCmd = 
            "SELECT q.item_sid upc, nvl(qty, 0) qty " + 
            "FROM invn_sbs i, invn_sbs_qty q, store st " + 
            "WHERE i.item_sid = q.item_sid " + 
            "AND q.store_no = st.store_no AND q.sbs_no = st.sbs_no " + 
            "AND st.store_code = :storeCode " + 
            "AND q.qty > 0 ";

        private string GetKeepOfflineList = "SELECT * FROM onlinestore_keepoffline WHERE store_code = :storeCode AND start_date <= CURRENT_TIMESTAMP ";

        /// <summary>
        /// Lấy tồn của 1 cửa hàng
        /// </summary>
        /// <param name="pickupStore">mã cửa hàng</param>
        /// <param name="lastSync">Lần cập nhật stock cuối trên Prism</param>
        /// <returns></returns>
        public Dictionary<string,int> GetOnhands(string pickupStore, DateTime? lastSync)
        {
            Dictionary<string, int>  onHands = new Dictionary<string, int>();

            OracleConnection connection = new OracleConnection(connectionString);
            //string GetQtyOnHandCmd = "SELECT q.item_sid upc, nvl(qty, 0) qty FROM invn_sbs i, invn_sbs_qty q, store st WHERE i.item_sid = q.item_sid AND q.store_no = st.store_no AND q.sbs_no = st.sbs_no AND st.store_code = :storeCode AND q.qty > 0 "; //"SELECT upc, closing as qty FROM table(get_oh_qty(:storeCode)) ";
            string filterByDate = " AND i.modified_date >= TO_DATE(:lastSync, 'YYYY/MM/DD HH24:MI:SS')";
            OracleCommand command = new OracleCommand() { Connection = connection };
            command.Parameters.Add(new OracleParameter(":storeCode", pickupStore));

            if (lastSync.HasValue)
            {
                command.CommandText = GetQtyOnHandCmd + filterByDate;
                command.Parameters.Add(new OracleParameter(":lastSync", lastSync.Value.ToString("yyyy/MM/dd HH:mm:ss")));
            }
            else
            {
                command.CommandText = GetQtyOnHandCmd;
            }

            try
            {
                connection.Open();
                OracleDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    onHands.Add(reader[0].ToString(),  int.Parse(reader[1].ToString() ));
                }

                reader.Close();
                connection.Close();

                return onHands;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Lấy tổng tồn nhiều cửa hàng
        /// </summary>
        /// <param name="storeList">Danh sách cửa hàng</param>
        /// <param name="lastSync">Lần cập nhật stock cuối trên Prism</param>
        /// <returns></returns>
        public Dictionary<string, int> GetMultiOnhands(string storeList, DateTime? lastSync)
        {
            Dictionary<string, int> onHands = new Dictionary<string, int>();
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand command = new OracleCommand() { Connection = connection };
            command.Parameters.Add(new OracleParameter(":storeCode", storeList));

            if (lastSync.HasValue)
            {
                string filterByDate = " AND i.modified_date >= TO_DATE(:lastSync, 'YYYY/MM/DD HH24:MI:SS')";
                command.CommandText = string.Format(GetQtyOnHandMultiStoreCmd , filterByDate);
                command.Parameters.Add(new OracleParameter(":lastSync", lastSync.Value.ToString("yyyy/MM/dd HH:mm:ss")));
            }
            else
            {
                command.CommandText = string.Format(GetQtyOnHandMultiStoreCmd, "");
            }

            try
            {
                connection.Open();
                OracleDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    onHands.Add(reader[0].ToString(), int.Parse(reader[1].ToString()));
                }

                reader.Close();
                connection.Close();

                return onHands;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        public Dictionary<string, int> LoadCustomListFromDB() 
        {
            Dictionary<string, int> customList = new Dictionary<string, int>();

            return customList;
        }
    }
}

///// <summary>
///// Get on hand qty of 1 upc
///// </summary>
///// <param name="upc">upc</param>
///// <returns>Object QtyOnHand</returns>
//public PrismQtyOnHand GetQtyOnHand(string upc, DateTime? lastSync = null)
//{
//    try
//    {
//        DataTable tb = new DataTable();

//        if (lastSync.HasValue)
//        {
//            tb = QueryData(GetQtyOnHandByDateTimeCmd, new string[] { ":upc", ":storeCode", ":lastSync" }, new object[] { upc, pickupStore, lastSync.Value.ToString("yyyy/MM/dd HH:mm:ss") });
//        }
//        else
//        {
//            tb = QueryData(GetQtyOnHandCmd, new string[] { ":upc", ":storeCode" }, new object[] { upc, pickupStore, });
//        }

//        if (tb.Rows.Count == 1)
//            return new PrismQtyOnHand() { UPC = upc, QtyOnHand = tb.Rows[0]["qty"] == DBNull.Value ? null : (int?)int.Parse(tb.Rows[0]["qty"].ToString()) };

//        return null;
//    }
//    catch
//    {
//        throw;
//    }
//}

///// <summary>
///// Get on hand qty of all items in current store. Filter condition: qty > 0
///// </summary>
///// <returns>List QtyOnHand Objects</returns>
//public List<PrismQtyOnHand> GetQtyOnHands(DateTime? lastSync)
//{
//    List<PrismQtyOnHand> onHands = new List<PrismQtyOnHand>();

//    OracleConnection connection = new OracleConnection(connectionString);
//     //"SELECT upc, closing as qty FROM table(get_oh_qty(:storeCode)) ";

//    OracleCommand command = new OracleCommand() { Connection = connection };
//    command.Parameters.Add(new OracleParameter(":storeCode", pickupStore));

//    if (lastSync.HasValue)
//    {
//        string filterByDate = " AND i.modified_date >= TO_DATE(:lastSync, 'YYYY/MM/DD HH24:MI:SS')";
//        command.CommandText = GetQtyOnHandCmd + filterByDate;
//        command.Parameters.Add(new OracleParameter(":lastSync", lastSync.Value.ToString("yyyy/MM/dd HH:mm:ss")));
//    }
//    else
//    {
//        command.CommandText = GetQtyOnHandCmd;
//    }

//    try
//    {
//        connection.Open();
//        OracleDataReader reader = command.ExecuteReader();

//        while (reader.Read())
//        {
//            onHands.Add(new PrismQtyOnHand() { UPC = reader[0].ToString(), QtyOnHand = int.Parse(reader[1].ToString()) });
//        }

//        reader.Close();
//        connection.Close();

//        return onHands;
//    }
//    catch(Exception ex)
//    {
//        throw ex;
//    }
//    finally
//    {
//        GC.Collect();
//    }
//}
