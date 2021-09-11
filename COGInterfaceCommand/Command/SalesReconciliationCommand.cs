using COGInterfaceCommand.Common;
using COGInterfaceCommand.Common.COG;
using System.Data;
using System;
using System.IO;


namespace COGInterfaceCommand.Command
{
   public class SalesReconciliationCommand: SftpCommand
    {
        public SalesReconciliationCommand(string Server, string Login, string Password, int Port, string backup, string inbound, string outbound, string localDownload, string localUpload)
          : base(Server, Login, Password, Port, backup, inbound, outbound, localDownload, localUpload)
        {
        }

        SalesReconciliation salesRecon = new SalesReconciliation(null, 0,"",0);
        string fileuploadReconsale = Environment.CurrentDirectory + "\\FileUpload\\UploadReconSale";
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
                if(day=="Monday")
                {
                    string sqlstore = "select store_no,glob_store_code from store where store_code like 'I%' ";
                    store = config.ExecuteQueryData(config.RPConnection, sqlstore, null);
                    if (store.Rows.Count != 0)
                    {
                        foreach (DataRow rstore in store.Rows)
                        {
                            int line = 0;
                            string storeno = rstore["glob_store_code"].ToString();
                            string sql = "SELECT  i.store_no,st.Store_code, TO_CHAR(TRUNC(i.created_date), 'DD/MM/YYYY') AS transaction_date ,sum(invitem.qty*i.Report_Modifier) AS total_item,ROUND(SUM((NVL(invitem.Price, 0) * (1 - NVL(i.Disc_Perc, 0) / 100) * invitem.Qty * i.Report_Modifier)), 0)  AS staff_revenue,count(DISTINCT i.cust_sid) as staff_no_of_customers, COUNT(*) OVER() AS total_record FROM invoice_v i INNER JOIN invc_item_v invitem ON i.invc_sid = invitem.invc_sid INNER JOIN invn_sbs inv ON invitem.item_sid = inv.item_sid , Store_v St WHERE i.sbs_no = st.sbs_no and i.store_no = st.Store_no and i.store_no = '"+ rstore["store_no"].ToString() + "'  AND i.sbs_no = 1 AND Trunc(i.Created_Date) BETWEEN to_date('"+fdate.ToShortDateString()+"', 'MM/DD/YYYY') AND to_date('"+tdate.ToShortDateString()+"', 'MM/DD/YYYY') GROUP BY i.store_no,st.Store_code, TO_CHAR(TRUNC(i.created_date), 'DD/MM/YYYY')";
                            td = config.ExecuteQueryData(config.RPConnection, sql, null);
                            if (td.Rows.Count != 0)
                            {
                                foreach (DataRow row in td.Rows)
                                {
                                    salesRecon = new SalesReconciliation(row, line, storeno,0);
                                    line = line + 1;
                                }
                            }
                        }
                    }
                    this.PutFileToSFTPServer(InFolder,1);
                    string[] filelist = Directory.GetFiles(fileuploadReconsale);
                    if (filelist != null && filelist.Length > 0)
                    {
                        foreach (string file in filelist)
                        {
                            File.Move(file, string.Concat(fileuploadReconsale + "\\FileBackUp", "\\" + Path.GetFileName(file)));
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public void GetSalesReconRemian()
        {
            try
            {
                int filetime = 0;
                Configurations config = new Configurations();
                DataTable store = new DataTable();
                DataTable td = new DataTable();
                string day = DateTime.Now.DayOfWeek.ToString();
                DateTime datetoday = DateTime.Now.Date;

                if (day == "Tuesday")
                {
                    DateTime dateyesterday = datetoday.AddDays(-1);

                    string timeinvsql = "select max(filetime_salerecon) as filetime from INVC_INTERFACE_COG where Trunc(invc_Created_Date) BETWEEN to_date('" + dateyesterday.ToShortDateString() + "', 'MM/DD/YYYY') AND to_date('" + dateyesterday.ToShortDateString() + "', 'MM/DD/YYYY')";
                    var filetimetb = config.ExecuteQueryData(config.RPConnection, timeinvsql, null);
                    filetime = string.IsNullOrWhiteSpace(filetimetb.Rows[0]["filetime"].ToString()) ? -1 : int.Parse(filetimetb.Rows[0]["filetime"].ToString());

                    string sqlstore = "select store_no,glob_store_code from store where store_code like 'I%' ";
                    store = config.ExecuteQueryData(config.RPConnection, sqlstore, null);
                    if (store.Rows.Count != 0)
                    {
                        foreach (DataRow rstore in store.Rows)
                        {
                            int line = 0;                           
                            string storeno = rstore["glob_store_code"].ToString();
                            string sql = "SELECT  i.store_no,st.Store_code, TO_CHAR(TRUNC(i.created_date), 'DD/MM/YYYY') AS transaction_date ,sum(invitem.qty*i.Report_Modifier) AS total_item,ROUND(SUM((NVL(invitem.Price, 0) * (1 - NVL(i.Disc_Perc, 0) / 100) * invitem.Qty * i.Report_Modifier)), 0)  AS staff_revenue,count(DISTINCT i.cust_sid) as staff_no_of_customers, COUNT(*) OVER() AS total_record FROM invoice_v i INNER JOIN invc_item_v invitem ON i.invc_sid = invitem.invc_sid INNER JOIN invn_sbs inv ON invitem.item_sid = inv.item_sid , Store_v St WHERE i.sbs_no = st.sbs_no and i.store_no = st.Store_no and i.store_no = '" + rstore["store_no"].ToString() + "'  AND i.sbs_no = 1 AND Trunc(i.Created_Date) BETWEEN to_date('" + dateyesterday.ToShortDateString() + "', 'MM/DD/YYYY') AND to_date('" + dateyesterday.ToShortDateString() + "', 'MM/DD/YYYY') and i.invc_sid not in (select invc_sid from invc_interface_cog where Trunc(invc_Created_Date) BETWEEN to_date('" + dateyesterday.ToShortDateString() + "', 'MM/DD/YYYY') AND to_date('" + dateyesterday.ToShortDateString() + "', 'MM/DD/YYYY')) GROUP BY i.store_no,st.Store_code, TO_CHAR(TRUNC(i.created_date), 'DD/MM/YYYY')";
                            td = config.ExecuteQueryData(config.RPConnection, sql, null);
                            if (td.Rows.Count != 0)
                            {
                                foreach (DataRow row in td.Rows)
                                {
                                    salesRecon = new SalesReconciliation(row, line, storeno, filetime + 1);
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
