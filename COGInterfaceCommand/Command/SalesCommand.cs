using COGInterfaceCommand.Common;
using COGInterfaceCommand.Common.COG;
using System.Data;
using System;
using System.IO;


namespace COGInterfaceCommand.Command
{
   public class SalesCommand : SftpCommand
    {
        public SalesCommand(string Server, string Login, string Password, int Port, string backup, string inbound, string outbound, string localDownload, string localUpload)
           : base(Server, Login, Password, Port, backup, inbound, outbound, localDownload, localUpload)
        {
        }

        Sales sales = new Sales(null,0,"",0);
        SalesReconciliation salesRecon = new SalesReconciliation(null, 0, "", 0);
        string fileuploadReconsale = Environment.CurrentDirectory + "\\FileUpload\\UploadReconSale";
        public void GetSales()        
        { 
            try
            {
                Configurations config = new Configurations();               
                DataTable store = new DataTable();                
                DataTable td = new DataTable();
                DateTime qdate = DateTime.Now.Date;//DateTime.Parse("11/05/2019");//DateTime.Now.Date;

                string sqlstore = "select store_no,glob_store_code from store where store_code like 'I%' ";
                store = config.ExecuteQueryData(config.RPConnection, sqlstore, null);
                if(store.Rows.Count!=0)
                {
                    foreach (DataRow rstore in store.Rows)
                    {
                        int line = 0;
                        string inv_no = "";
                        string storeno = rstore["glob_store_code"].ToString();
                        string sql = "Select hd.sbs_no ,hd.store_no,st.Store_code,st.Store_Name,to_char(trunc(hd.Created_Date), 'mm/dd/yyyy') as Invc_Created_Date,to_char(trunc(hd.Created_Date), 'yyyymmdd') as Created_Date,to_char(hd.Modified_date, 'hh24mi') as Created_Hour,hd.Month,hd.Year,hd.invc_sid,hd.invc_no,(trim(cust.first_name) || ' ' || trim(cust.last_name)) AS BillTo,hd.vend_code,hd.dcs_code,TO_CHAR(hd.style_sid) as Style_Sid,it.text5 ,hd.item_sid,hd.UPC,hd.item_note1,hd.description1 ,hd.description2 as Season,hd.description4 as ietNam_Desc,hd.description3 as English_Desc,hd.attr as Color,hd.siz as Siz,hd.D_name,hd.C_name,hd.S_name,hd.D_long_name,hd.C_long_name,hd.S_long_name,hd.Department,hd.Class,hd.SubClass,hd.Department_Name,hd.Class_Name,hd.SubClass_Name,hd.Dept_Long_name,hd.Class_Long_name,hd.SubClass_Long_Name,hd.Orig_Price,hd.Price,hd.Tax_amt,hd.Qty,hd.item_pos,hd.Orig_Amt_Total,(CASE WHEN NVL(hd.Orig_Amt_Total,0) <> 0 THEN CAST((NVL(hd.DISC_AMT_TOTAL,0)/ NVL(hd.Orig_Amt_Total,1)) * 100 AS NUMBER(5,2)) WHEN NVL(hd.Orig_Amt_Total,0) = 0 AND NVL(hd.Orig_Amt_Total,0) = 0 THEN 0 WHEN NVL(hd.Orig_Amt_Total,0) = 0 AND NVL(hd.DISC_AMT_TOTAL,0) <> 0 THEN CAST((NVL(hd.DISC_AMT_TOTAL,0)/ (NVL(hd.DISC_AMT_TOTAL,0) + NVL(hd.Ext_AMT_TOTAL, 0))) * 100 AS NUMBER(5,2)) END) AS Disc_Perc,hd.Orig_Price - hd.Price as Disc_amt,hd.DISC_AMT_TOTAL,hd.Ext_AMT_TOTAL,COUNT(*) OVER () AS total_invoice from (select i.sbs_no,i.store_no,i.created_date AS Created_Date,i.modified_date AS Modified_date,Extract (Month from i.created_date) as Month,Extract (year from i.created_date) as Year,i.invc_sid,i.invc_no,i.cust_sid,inv.vend_code,inv.dcs_code,TO_CHAR(inv.style_sid) as style_sid,inv.item_sid,inv.UPC,it.item_note1,inv.description1,inv.description2,inv.description3,inv.description4,inv.attr,inv.Siz,dcs.D_name,dcs.C_name,dcs.S_name,dcs.D_long_name,dcs.C_long_name,dcs.S_long_name,dcs.Department,dcs.Class,dcs.SubClass,dcs.Department_Name,dcs.Class_Name,dcs.SubClass_Name,dcs.Dept_Long_name,dcs.Class_Long_name,dcs.SubClass_Long_Name,it.Orig_Price as Orig_Price,it.Price as Price,it.item_pos,Sum(it.Tax_Amt) as Tax_Amt,SUM(it.qty * i.Report_Modifier) AS Qty,SUM(it.Orig_Price  * it.Qty * i.Report_Modifier) AS Orig_AMT_TOTAL,ROUND(SUM(((it.Orig_Price - NVL(it.Price,0)) * it.Qty  *  i.Report_Modifier) + ((NVL(it.Price,0) * NVL(it.qty * i.Report_Modifier, 0) * NVL(i.Disc_Perc, 0) /100))),0) AS DISC_AMT_TOTAL,ROUND(SUM((NVL(it.Price,0) * (1 - NVL(i.Disc_Perc, 0) / 100) * it.Qty * i.Report_Modifier)),0) AS Ext_AMT_TOTAL from invoice_v i, invc_item_v it, Inventory_ov Inv , dcs_V Dcs where i.invc_sid = it.invc_sid AND it.item_sid = Inv.item_sid(+) AND INV.sbs_no = DCS.Sbs_no AND INV.DCS_CODE = DCS.DCS_CODE AND INV.DCS_CODE <> 'AAABBBCCC' And inv.Sbs_no = i.sbs_no AND i.held <> 1 AND nvl(i.HiSec_Type,0) NOT IN (2,3,4,5,10,11) AND i.Invc_Type IN (0,2) AND (i.Sbs_No in (1,2)) AND nvl(it.Kit_Flag,0) NOT IN (2, 3) group by i.sbs_no,i.store_no,i.created_date,i.modified_date,Extract (Month from i.created_date),Extract (year from i.created_date),i.invc_sid,i.invc_no,i.cust_sid,inv.vend_code, inv.dcs_code,TO_CHAR (inv.style_sid),inv.item_sid,inv.UPC,inv.description1,inv.description2,inv.description3,inv.description4,inv.Attr,Inv.siz,dcs.D_name,dcs.C_name,dcs.S_name,dcs.D_long_name,dcs.C_long_name,dcs.S_long_name,dcs.Department,dcs.Class,dcs.SubClass,dcs.Department_Name,dcs.Class_Name,dcs.SubClass_Name,dcs.Dept_Long_name,dcs.Class_Long_name,dcs.SubClass_Long_Name,it.Orig_Price,it.item_note1,it.Price,it.item_pos)hd,customer_v cust,Store_v St, invn_sbs it where hd.cust_sid = cust.cust_sid(+) and hd.item_sid = it.item_sid(+) and hd.store_no = st.Store_no(+) and hd.sbs_no = st.sbs_no(+)  and hd.store_no ='" + rstore["store_no"].ToString() + "' and Trunc(hd.Created_Date) BETWEEN to_date('"+qdate.ToShortDateString()+"', 'MM/DD/YYYY') AND to_date('"+qdate.ToShortDateString()+ "', 'MM/DD/YYYY')  ORDER BY hd.invc_sid,hd.invc_no";
                        td = config.ExecuteQueryData(config.RPConnection, sql, null);
                        if (td.Rows.Count != 0)
                        {
                            foreach (DataRow row in td.Rows)
                            {   
                                sales = new Sales(row, line, storeno, 0);
                                line = line + 1;
                                if(inv_no=="" || inv_no != row[9].ToString())
                                {
                                    string inv_sql = "Insert into INVC_INTERFACE_COG values('"+ row[9].ToString() + "',to_date('" + row[4] + "', 'MM/DD/YYYY'),to_date('" + qdate.ToShortDateString() + "', 'MM/DD/YYYY'),'',0,-1)";                                   
                                    config.ExecuteRPCommand(config.RPConnection, inv_sql, null);
                                }
                                inv_no = row[9].ToString();
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
        public void GetSalesRemain()
        {
            try
            {
                Configurations config = new Configurations();
                int filetime = -1;
                DataTable store = new DataTable();
                DataTable td = new DataTable();
                DataTable tdrecon = new DataTable();
                DateTime qdate = DateTime.Now.AddDays(-1);// DateTime.Parse("11/05/2019");// DateTime.Now.AddDays(-1);
                string timeinvsql = "select max(filetime) as filetime from INVC_INTERFACE_COG where Trunc(invc_Created_Date) BETWEEN to_date('" + qdate.ToShortDateString() + "', 'MM/DD/YYYY') AND to_date('" + qdate.ToShortDateString() + "', 'MM/DD/YYYY')";
                var filetimetb = config.ExecuteQueryData(config.RPConnection, timeinvsql, null);
                filetime = string.IsNullOrWhiteSpace(filetimetb.Rows[0]["filetime"].ToString()) ? (int)-1 : int.Parse(filetimetb.Rows[0]["filetime"].ToString());
                int timeline = filetime + 1;

                string sqlstore = "select store_no,glob_store_code from store where store_code like 'I%' ";
                store = config.ExecuteQueryData(config.RPConnection, sqlstore, null);
                if (store.Rows.Count != 0)
                {
                    foreach (DataRow rstore in store.Rows)
                    {
                        int line = 0;
                        string inv_no = "";
                        string storeno = rstore["glob_store_code"].ToString();
                        string sql = "Select hd.sbs_no ,hd.store_no,st.Store_code,st.Store_Name,to_char(trunc(hd.Created_Date), 'mm/dd/yyyy') as Invc_Created_Date,to_char(trunc(hd.Created_Date), 'yyyymmdd') as Created_Date,to_char(hd.Modified_date, 'hh24mi') as Created_Hour,hd.Month,hd.Year,hd.invc_sid,hd.invc_no,(trim(cust.first_name) || ' ' || trim(cust.last_name)) AS BillTo,hd.vend_code,hd.dcs_code,TO_CHAR(hd.style_sid) as Style_Sid,it.text5 ,hd.item_sid,hd.UPC,hd.item_note1,hd.description1 ,hd.description2 as Season,hd.description4 as ietNam_Desc,hd.description3 as English_Desc,hd.attr as Color,hd.siz as Siz,hd.D_name,hd.C_name,hd.S_name,hd.D_long_name,hd.C_long_name,hd.S_long_name,hd.Department,hd.Class,hd.SubClass,hd.Department_Name,hd.Class_Name,hd.SubClass_Name,hd.Dept_Long_name,hd.Class_Long_name,hd.SubClass_Long_Name,hd.Orig_Price,hd.Price,hd.Tax_amt,hd.Qty,hd.item_pos,hd.Orig_Amt_Total,(CASE WHEN NVL(hd.Orig_Amt_Total,0) <> 0 THEN CAST((NVL(hd.DISC_AMT_TOTAL,0)/ NVL(hd.Orig_Amt_Total,1)) * 100 AS NUMBER(5,2)) WHEN NVL(hd.Orig_Amt_Total,0) = 0 AND NVL(hd.Orig_Amt_Total,0) = 0 THEN 0 WHEN NVL(hd.Orig_Amt_Total,0) = 0 AND NVL(hd.DISC_AMT_TOTAL,0) <> 0 THEN CAST((NVL(hd.DISC_AMT_TOTAL,0)/ (NVL(hd.DISC_AMT_TOTAL,0) + NVL(hd.Ext_AMT_TOTAL, 0))) * 100 AS NUMBER(5,2)) END) AS Disc_Perc,hd.Orig_Price - hd.Price as Disc_amt,hd.DISC_AMT_TOTAL,hd.Ext_AMT_TOTAL,COUNT(*) OVER () AS total_invoice from (select i.sbs_no,i.store_no,i.created_date AS Created_Date,i.modified_date AS Modified_date,Extract (Month from i.created_date) as Month,Extract (year from i.created_date) as Year,i.invc_sid,i.invc_no,i.cust_sid,inv.vend_code,inv.dcs_code,TO_CHAR(inv.style_sid) as style_sid,inv.item_sid,inv.UPC,it.item_note1,inv.description1,inv.description2,inv.description3,inv.description4,inv.attr,inv.Siz,dcs.D_name,dcs.C_name,dcs.S_name,dcs.D_long_name,dcs.C_long_name,dcs.S_long_name,dcs.Department,dcs.Class,dcs.SubClass,dcs.Department_Name,dcs.Class_Name,dcs.SubClass_Name,dcs.Dept_Long_name,dcs.Class_Long_name,dcs.SubClass_Long_Name,it.Orig_Price as Orig_Price,it.Price as Price,it.item_pos,Sum(it.Tax_Amt) as Tax_Amt,SUM(it.qty * i.Report_Modifier) AS Qty,SUM(it.Orig_Price  * it.Qty * i.Report_Modifier) AS Orig_AMT_TOTAL,ROUND(SUM(((it.Orig_Price - NVL(it.Price,0)) * it.Qty  *  i.Report_Modifier) + ((NVL(it.Price,0) * NVL(it.qty * i.Report_Modifier, 0) * NVL(i.Disc_Perc, 0) /100))),0) AS DISC_AMT_TOTAL,ROUND(SUM((NVL(it.Price,0) * (1 - NVL(i.Disc_Perc, 0) / 100) * it.Qty * i.Report_Modifier)),0) AS Ext_AMT_TOTAL from invoice_v i, invc_item_v it, Inventory_ov Inv , dcs_V Dcs where i.invc_sid = it.invc_sid AND it.item_sid = Inv.item_sid(+) AND INV.sbs_no = DCS.Sbs_no AND INV.DCS_CODE = DCS.DCS_CODE AND INV.DCS_CODE <> 'AAABBBCCC' And inv.Sbs_no = i.sbs_no AND i.held <> 1 AND nvl(i.HiSec_Type,0) NOT IN (2,3,4,5,10,11) AND i.Invc_Type IN (0,2) AND (i.Sbs_No in (1,2)) AND nvl(it.Kit_Flag,0) NOT IN (2, 3) group by i.sbs_no,i.store_no,i.created_date,i.modified_date,Extract (Month from i.created_date),Extract (year from i.created_date),i.invc_sid,i.invc_no,i.cust_sid,inv.vend_code, inv.dcs_code,TO_CHAR (inv.style_sid),inv.item_sid,inv.UPC,inv.description1,inv.description2,inv.description3,inv.description4,inv.Attr,Inv.siz,dcs.D_name,dcs.C_name,dcs.S_name,dcs.D_long_name,dcs.C_long_name,dcs.S_long_name,dcs.Department,dcs.Class,dcs.SubClass,dcs.Department_Name,dcs.Class_Name,dcs.SubClass_Name,dcs.Dept_Long_name,dcs.Class_Long_name,dcs.SubClass_Long_Name,it.Orig_Price,it.item_note1,it.Price,it.item_pos)hd,customer_v cust,Store_v St, invn_sbs it where hd.cust_sid = cust.cust_sid(+) and hd.item_sid = it.item_sid(+) and hd.store_no = st.Store_no(+) and hd.sbs_no = st.sbs_no(+)  and hd.store_no ='" + rstore["store_no"].ToString() + "' and Trunc(hd.Created_Date) BETWEEN to_date('" + qdate.ToShortDateString() + "', 'MM/DD/YYYY') AND to_date('" + qdate.ToShortDateString() + "', 'MM/DD/YYYY') and hd.invc_sid not in (select invc_sid from invc_interface_cog where Trunc(invc_Created_Date) BETWEEN to_date('" + qdate.ToShortDateString() + "', 'MM/DD/YYYY') AND to_date('" + qdate.ToShortDateString() + "', 'MM/DD/YYYY')) ORDER BY hd.invc_sid,hd.invc_no";
                        td = config.ExecuteQueryData(config.RPConnection, sql, null);
                        if (td.Rows.Count != 0)
                        {
                            foreach (DataRow row in td.Rows)
                            {
                                sales = new Sales(row, line, storeno, filetime + 1);
                                line = line + 1;
                                if (inv_no == "" || inv_no != row[9].ToString())
                                {   
                                    string inv_sql = "Insert into INVC_INTERFACE_COG values('" + row[9].ToString() + "',to_date('" + row[4] + "', 'MM/DD/YYYY'),to_date('" + DateTime.Now.ToShortDateString() + "', 'MM/DD/YYYY'),''," + timeline + ",-1 )";
                                    config.ExecuteRPCommand(config.RPConnection, inv_sql, null);
                                }
                                inv_no = row[9].ToString();
                            }
                            // Neu remain chạy lại ngày thứ 3 có, thì phai chay lai store recon
                            string dayrecon = DateTime.Now.DayOfWeek.ToString();
                            DateTime tdaterecon = DateTime.Now.AddDays(-1);
                            DateTime fdaterecon = tdaterecon.AddDays(-6);
                            string storenorecon = rstore["glob_store_code"].ToString();
                            if (dayrecon == "Tuesday")
                            {
                                int linetime = 0;
                                string sqlrecon = "SELECT  i.store_no,st.Store_code, TO_CHAR(TRUNC(i.created_date), 'DD/MM/YYYY') AS transaction_date ,sum(invitem.qty*i.Report_Modifier) AS total_item,ROUND(SUM((NVL(invitem.Price, 0) * (1 - NVL(i.Disc_Perc, 0) / 100) * invitem.Qty * i.Report_Modifier)), 0)  AS staff_revenue,count(DISTINCT i.cust_sid) as staff_no_of_customers, COUNT(*) OVER() AS total_record FROM invoice_v i INNER JOIN invc_item_v invitem ON i.invc_sid = invitem.invc_sid INNER JOIN invn_sbs inv ON invitem.item_sid = inv.item_sid , Store_v St WHERE i.sbs_no = st.sbs_no and i.store_no = st.Store_no and i.store_no = '" + rstore["store_no"].ToString() + "'  AND i.sbs_no = 1 AND Trunc(i.Created_Date) BETWEEN to_date('" + fdaterecon.ToShortDateString() + "', 'MM/DD/YYYY') AND to_date('" + tdaterecon.ToShortDateString() + "', 'MM/DD/YYYY') GROUP BY i.store_no,st.Store_code, TO_CHAR(TRUNC(i.created_date), 'DD/MM/YYYY') order by transaction_date";
                                tdrecon = config.ExecuteQueryData(config.RPConnection, sqlrecon, null);
                                if (tdrecon.Rows.Count != 0)
                                {
                                    foreach (DataRow row in tdrecon.Rows)
                                    {
                                        salesRecon = new SalesReconciliation(row, linetime, storenorecon, 0);
                                        linetime = linetime + 1;
                                    }
                                }
                                this.PutFileToSFTPServer("Inbound/SalesRecon/",1);
                                string[] filelistrecon = Directory.GetFiles(fileuploadReconsale);
                                if (filelistrecon != null && filelistrecon.Length > 0)
                                {
                                    foreach (string file in filelistrecon)
                                    {
                                        File.Move(file, string.Concat(fileuploadReconsale + "\\FileBackUp", "\\" + Path.GetFileName(file)));
                                    }
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
