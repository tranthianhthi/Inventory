using System;
using System.Text;
using System.Data;
using System.IO;


namespace COGInterfaceCommand.Common.COG
{
   public class Sales
    {
        string storecode { get; set; }
        string registerno { get; set; }
        string transactionno { get; set; }
        string salespersonid { get; set; }
        string transactiondate { get; set; }
        string transactiontime { get; set; }
        string sellcode { get; set; }
        int qty { get; set; }
        double unitprice { get; set; }
        double unitdiscount { get; set; }
        string customerid { get; set; }
        double unitcost { get; set; }
        int totalline { get; set; }

        private static string finnalmess = "";
        private string filename = "X_ACFC_"; //+ DateTime.Now.Date.ToString("yyyyMMdd"); // ACFC_7100_ StoreSOHRecon_COG_20190406.TXT 
        private  string pathname = Environment.CurrentDirectory + "\\FileUpload\\";
        public Sales (DataRow row, int line, string globalstoreno, int filetime)
        {           
            if (row != null)
            {                
                storecode = globalstoreno;
                registerno = "";
                transactionno = row["INVC_SID"].ToString().Substring(row["INVC_SID"].ToString().Length-16,16);
                salespersonid = "";
                transactiondate = row["CREATED_DATE"].ToString();
                transactiontime = row["CREATED_HOUR"].ToString();
                sellcode = row["UPC"].ToString();
                qty = int.Parse(row["Qty"].ToString());
                unitprice=double.Parse(row["Price"].ToString());
                if(double.Parse(row["disc_perc"].ToString())>0 && double.Parse(row["Disc_amt"].ToString())==0)
                        {
                    unitdiscount = double.Parse(row["disc_amt_total"].ToString())/ qty;
                    unitprice = unitprice - unitdiscount;
                }
                else
                { unitdiscount = double.Parse(row["Disc_amt"].ToString()); }
                
                customerid = "";
                unitcost = 0;
                totalline = int.Parse(row["total_invoice"].ToString());
                finnalmess = finnalmess + storecode+",,"+ transactionno + ",,"+transactiondate+","+transactiontime+","+sellcode+","+qty+","+unitprice+","+unitdiscount+",,\r\n";
                if(line==totalline-1) //write file
                {
                    if (filetime != 0)
                    {
                        filename = filename + globalstoreno + "_" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                    }
                    else
                    {
                        filename = filename + globalstoreno + "_" + DateTime.Now.Date.ToString("yyyyMMdd");
                    }                        
                    if (filetime != 0)
                    {
                        filename = filename + "-0" + filetime;
                    }
                    pathname = pathname + filename + ".txt";                   
                    if (finnalmess!="")
                    using (
                        StreamWriter writer = new StreamWriter(pathname, false, Encoding.UTF8))
                    {
                        writer.Write(finnalmess.Trim());                           
                        finnalmess = "";
                    }
                    filename = "";
                    pathname = "";
                }
            }
        }
                
    }
}
