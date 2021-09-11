using System;
using System.Text;
using System.Data;
using System.IO;

namespace COGInterfaceCommand.Common.COG
{
   public class SalesReconciliation
    {
        string storecode { get; set; }
        string tradingdate { get; set; }
        double totalsale { get; set; }
        int totalunit { get; set; }
        int totalline { get; set; }       

        private static string finnalmess = "";
        private string filename = "_SalesRecon_COG_"; // ACFC_7100_ StoreSOHRecon_COG_20190406.TXT 
        private string pathname = Environment.CurrentDirectory + "\\FileUpload\\UploadReconSale\\";

        public SalesReconciliation(DataRow row, int line, string globalstoreno, int filetime)
        {
            if (row != null)
            {
                storecode = globalstoreno;
                tradingdate = row["transaction_date"].ToString();
                totalsale = double.Parse(row["staff_revenue"].ToString());
                totalunit =int.Parse(row["Total_Item"].ToString());
                totalline = int.Parse(row["total_record"].ToString());
                finnalmess = finnalmess + storecode + "|" + tradingdate + "|" + totalsale + "|" + totalunit + "\r\n";
                if (line == totalline - 1) //write file
                {
                    if (filetime != 0)
                    {
                        filename = "ACFC_" + globalstoreno + filename+ DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                    }
                    else
                    {
                        filename = "ACFC_" + globalstoreno + filename+ DateTime.Now.Date.ToString("yyyyMMdd");
                    }

                    //filename = "ACFC_" + globalstoreno + filename;
                    if (filetime != 0)
                    {
                        filename = filename + "-0" + filetime;
                    }
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
