using COGInterfaceCommand.Common;
using COGInterfaceCommand.Common.COG;
using COGInterfaceCommand.Common.COG.ASN;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Data;

namespace COGInterfaceCommand.Command
{
   public class StockReconciliationCommand : SftpCommand
    {
        public StockReconciliationCommand(string Server, string Login, string Password, int Port, string backup, string inbound, string outbound, string localDownload, string localUpload)
         : base(Server, Login, Password, Port, backup, inbound, outbound, localDownload, localUpload)
        {
        }

        StockReconcilication stcokrecon = new StockReconcilication(null, 0, "");

        public void GetSales()
        {
            try
            {
                Configurations config = new Configurations();
                DataTable store = new DataTable();
                DataTable td = new DataTable();
                string day = DateTime.Now.DayOfWeek.ToString();
                DateTime tdate = DateTime.Now.Date;
                DateTime fdate = tdate.AddDays(-6);
                if (day == "Monday")
                {
                    string sqlstore = "select store_no,glob_store_code from store where store_code like 'I%' ";
                    store = config.ExecuteQueryData(config.RPConnection, sqlstore, null);
                    if (store.Rows.Count != 0)
                    {
                        foreach (DataRow rstore in store.Rows)
                        {
                            int line = 0;
                            string storeno = rstore["glob_store_code"].ToString();
                            string sql = "SELECT invitem.description1 as style, invitem.attr as color, invitem.siz as ItemSize,qty.store_no,st.glob_store_code, sum(nvl(qty.qty,0)) as qtyinv,COUNT(*) OVER() AS total_record FROM invn_sbs_qty qty inner join invn_sbs invitem on qty.item_sid = invitem.item_sid inner join store st on qty.store_no = st.store_no WHERE qty.store_no = " + rstore["store_no"] + " group by invitem.description1 , invitem.attr , invitem.siz ,qty.store_no,st.glob_store_code";
                            //string sql = "SELECT i.store_no,st.Store_code,inv.description1 as style, inv.attr as color, inv.siz as ItemSize, sum(invitem.qty) AS total_item, COUNT(*) OVER() AS total_record FROM invoice_v i INNER JOIN invc_item_v invitem ON i.invc_sid = invitem.invc_sid INNER JOIN invn_sbs inv ON invitem.item_sid = inv.item_sid , Store_v St WHERE i.sbs_no = st.sbs_no and i.store_no = st.Store_no and 0 = (case when i.Status = 1 then 1 WHEN BitAnd(65536, i.Proc_Status) <> 0 THEN 2 WHEN BitAnd(131072, i.Proc_Status) <> 0 THEN 3 ELSE 0 END) and i.store_no ='" + rstore["store_no"].ToString() + "' AND i.sbs_no = 1 AND Trunc(i.Created_Date) BETWEEN to_date('11/04/2019', 'MM/DD/YYYY') AND to_date('11/04/2019', 'MM/DD/YYYY') GROUP BY i.store_no,st.Store_code,inv.description1, inv.attr, inv.siz";
                            td = config.ExecuteQueryData(config.RPConnection, sql, null);
                            if (td.Rows.Count != 0)
                            {
                                foreach (DataRow row in td.Rows)
                                {
                                    stcokrecon = new StockReconcilication(row, line, storeno);
                                    line = line + 1;
                                }
                            }
                        }
                    }
                    this.PutFileToSFTPServer(InFolder,0);
                    string[] filelist = Directory.GetFiles(LocalUploadFolder);
                    if (filelist != null && filelist.Length > 0)
                    {
                        foreach (string file in filelist)
                        {
                            File.Move(file, string.Concat(LocalUploadFolder + "\\FileBackUp", "\\" + Path.GetFileName(file)));
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

        }
    }
}
