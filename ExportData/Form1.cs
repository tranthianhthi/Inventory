using ClosedXML.Excel;
using Oracle.ManagedDataAccess.Client;
using PrismDataProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExportData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.button1.Click +=
                new System.EventHandler(
                    async (sender, events) => await this.button1_ClickAsync(sender, events)
                );
        }

        readonly string cpoaConnectionString = "Data Source=" +
            "(" +
            "DESCRIPTION=" +
            "(" +
            "ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=rpoffice.acfc.com.vn)(PORT=1521))" +
            ")" +
            "(" +
            "CONNECT_DATA=(SERVICE_NAME=rproods)" +
            ")" +
            ");" +
            "User Id=reportuser;Password=report;Connection Timeout=999";

        private async Task button1_ClickAsync(object sender, EventArgs e)
        {
            string sql = "";
            var workbook = new XLWorkbook();
            DataTable tb = await executeQuery();
            workbook.Worksheets.Add(tb, "Sheet1");
            workbook.SaveAs("D:\\Export_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
            MessageBox.Show("DONE");
        }

        private async Task<DataTable> executeQuery()
        {
            PrismConnection connection = new PrismConnection("rpoffice.acfc.com.vn", 1521, "rproods", "reportuser", "report", 999999);
            PrismDataProvider.PrismDataProvider provider = new PrismDataProvider.PrismDataProvider(connection);
            return await provider.executeThreadingQueryDataAsync(txtSQL.Text, new DateTime(2020, 11, 1), new DateTime(2020, 11, 30));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            diagSaveFile.ShowDialog();
            if (diagSaveFile.FileName.Length > 0 && !diagSaveFile.CheckFileExists)
            {
                txtFileName.Text = diagSaveFile.FileName;
            }
        }
    }
}
