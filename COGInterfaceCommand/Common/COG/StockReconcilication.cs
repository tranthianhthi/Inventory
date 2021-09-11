using System;
using System.Text;
using System.Data;
using System.IO;

namespace COGInterfaceCommand.Common.COG
{
   public class StockReconcilication
    {
        string storecode { get; set; }
        string style { get; set; }
        string color { get; set; }
        string sizedesc { get; set; }
        int sohqty { get; set; }
        int totalline { get; set; }

        private static string finnalmess = "";
        private string filename = "_StoreSOHRecon_COG_" + DateTime.Now.Date.ToString("yyyyMMdd"); // ACFC_7100_ StoreSOHRecon_COG_20190406.TXT 
        private string pathname = Environment.CurrentDirectory + "\\FileUpload\\";

        public StockReconcilication(DataRow row, int line, string globalstoreno)
        {
            if (row != null)
            {
                storecode = globalstoreno;
                style = row["STYLE"].ToString();
                color = row["COLOR"].ToString();
                sizedesc = row["ItemSize"].ToString();
                sohqty =int.Parse(row["QTYINV"].ToString());               
                totalline = int.Parse(row["total_record"].ToString());
                finnalmess = finnalmess + storecode + "|" + style + "|" + color + "|" + sizedesc + "|" + sohqty + "\r\n";
                if (line == totalline - 1) //write file
                {
                    filename = "ACFC_" + globalstoreno + filename;
                    pathname = pathname + filename + ".txt";
                    if (finnalmess != "")
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
