using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InfologCostHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

       

        void reset()
        {
            lstUPCs.DataSource = null;
        }

        string getReturnQuery()
        {
            return 
                "select x.*, a.asn_no, c.cust_code, c.duns  from interface_acfc_goods_return_download x left join asn a with(nolock) on a.id = x.asn_id left join customer c with(nolock) on c.cust_code = a.aopt_field1 " + 
                "where 1 = 1 " +
                "and x.downloaded = 0 " +
                "and a.asn_no in ( @param ) " +
                "order by id, updated_date asc";
        }

        string getSlipQuery()
        {
            return
                "select x.* , s.order_no, s.cust_code, c.duns from interface_acfc_shipment_confirm_download x with(nolock) left join so s with(nolock) on s.id =x.so_id left join customer c on c.cust_code = s.cust_code " +
                "where 1 = 1 " +
                "and x.downloaded = 0 " +
                "and s.order_no in ( @param ) " +
                "order by updated_date asc";
        }

        string getReturnByDate()
        {
            return
                "select x.*, a.asn_no, c.cust_code, c.duns  from interface_acfc_goods_return_download x left join asn a with(nolock) on a.id = x.asn_id left join customer c with(nolock) on c.cust_code = a.aopt_field1 " +
                "where 1 = 1 " +
                "and x.downloaded = 0 " +
                "and x.created_date between @from and @to " +
                "order by id, updated_date asc"; ;
        }
        string getSlipByDate()
        {
            return 
                "select x.*, a.asn_no, c.cust_code, c.duns  from interface_acfc_goods_return_download x left join asn a with(nolock) on a.id = x.asn_id left join customer c with(nolock) on c.cust_code = a.aopt_field1 " +
                "where 1 = 1 " +
                "and x.downloaded = 0 " +
                "and x.created_date between @from and @to " +
                "order by updated_date asc";
        }

        DataTable executeCommand(string command)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["connection"]);
            SqlCommand cmd = new SqlCommand(command, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable tb = new DataTable();
            try
            {
                da.Fill(tb);
            }
            catch
            {
            }

            return tb;
        }

        DataTable executeCommand(string command, DateTime from, DateTime to)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["connection"]);
            SqlCommand cmd = new SqlCommand(command, conn);
            cmd.Parameters.Add(new SqlParameter("from", from));

            cmd.Parameters.Add(new SqlParameter("to", to));

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable tb = new DataTable();
            try
            {
                da.Fill(tb);
            }
            catch
            {
            }

            return tb;
        }

        List<string> extractUPCs(DataTable tb)
        {
            List<string> Upcs = new List<string>();
            string endRowTag = "</tr>";
            string memoryString = "";
            foreach(DataRow row in tb.Rows)
            {
                string remarks = row["remarks"].ToString().Replace("\r\n", "").Replace("\t", "");
                remarks = remarks.Substring(0, remarks.Length - 35);
                string table = remarks.Substring(remarks.IndexOf("<table>")).Replace("<table>", "").Replace("</table>", "");
                List<string> rows = new List<string>();
                int count = 0;
                while (table.Contains(endRowTag))
                {
                    count++;
                    int i = table.IndexOf(endRowTag);
                    // Nếu vẫn còn tr mới
                    if (count > 7)
                    {
                        // Dòng 8 băt đầu data
                        string upc = "";
                        if (i > 0)
                        {
                            string rString = table.Substring(0, i).Replace("<tr>", "");
                            upc = extractUpcFromRow(rString);
                        }
                    
                        if (upc.Length > 0 && !Upcs.Contains(upc))
                        {
                            memoryString += upc + "," + Environment.NewLine;
                            Upcs.Add(upc);
                        }
                    }

                    table = i > 0? table.Substring(i + 5) : table;

                }

                if(table.Length > 0)
                {
                    string upc = extractUpcFromRow(table);
                    if (upc.Length > 0 && !Upcs.Contains(upc))
                    {
                        memoryString += upc + "," + Environment.NewLine;
                        Upcs.Add(upc);
                    }
                }
            }
            if (memoryString.Length > 0)
                Clipboard.SetText(memoryString);

            return Upcs;
        }

        private void btnAddSlip_Click(object sender, EventArgs e)
        {
            if (txtSlip.TextLength == 0)
            {
                return;
            }
            string slips = txtSlip.Text;

            if (slips.Contains(","))
            {
                slips = slips.Replace(",", "','") ;
            }
            else
            {
            }

            txtArray.Text += (txtArray.Text.Trim().Length == 0 ? "'" : ",'") + txtSlip.Text.Trim() + "'";
            txtSlip.Clear();
        }

        string extractUpcFromRow(string rString)
        {
            string upc = "";
            // Lấy cột thứ 2 ( UPC ) bằng cách xác định index của end of col 1
            int endOfFirstCol = rString.IndexOf("</td><td>");
            if (endOfFirstCol > 0)
            {
                upc = rString.Substring(endOfFirstCol + 9);
                upc = upc.Contains("</td>") ? upc.Substring(0, upc.IndexOf("</td>")) : upc;
            }
            else
            {
                upc = rString.Contains("</td>") ? rString.Substring(0, upc.IndexOf("</td>")) : rString;
            }

            return upc;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string command;
            DataTable tb = new DataTable();
            reset();

            if (tabControl1.SelectedTab == tabPage1)
            {
                command = rdbIsSlip.Checked ? getSlipQuery() : getReturnQuery();
                command = command.Replace("@param", txtArray.Text);
                try
                {
                    tb = executeCommand(command);
                }
                catch { }
            }
            else
            {
                if (dateTimePicker2.Value < dateTimePicker1.Value)
                {
                    MessageBox.Show("Invalid range");
                    return;
                }
                command = rdbIsSlip.Checked ? getSlipByDate() : getReturnByDate();
                try
                {
                    tb = executeCommand(command, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
                }
                catch { }
            }

            List<string> upcs = extractUPCs(tb);
            MessageBox.Show("UPCs list is copied to clipboard");
            lstUPCs.DataSource = upcs;
        }

        private void dateTimePicker2_Validated(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
