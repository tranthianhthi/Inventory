using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;

namespace SlipFinder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnFind_Click(object sender, EventArgs e)
        {
            string fmt = "TEST     0";
            int i = 1234;

            string res = i.ToString(fmt);
            MessageBox.Show(res + "-" + res.Length + '-' + fmt.Length.ToString());


            //string spaces = new string(' ', 128);
            //MessageBox.Show("a" + spaces + "b");

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void BgFinder_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SearchingCriteria criteria = new SearchingCriteria(txtSlipNo.Text, dtFrom.Value.Date, dtTo.Value.Date.AddDays(1));

            bgFinder.RunWorkerAsync(criteria);

            
        }
        
        private void BgFinder_DoWork(object sender, DoWorkEventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(txtFolder.Text);
            List<FileInfo> files = d.GetFiles().Where(f => f.CreationTime >= dtFrom.Value.Date && f.CreationTime < dtTo.Value.Date.AddDays(1)).ToList();

            XmlDocument doc = new XmlDocument();

            foreach (FileInfo file in files)
            {
                doc.Load(file.FullName);
                XmlNode node = doc.DocumentElement.SelectSingleNode("/DOCUMENT/SLIPS/SLIP");
                string attr = node.Attributes["slip_no"]?.InnerText;
                if (attr == txtSlipNo.Text)
                {
                    txtFileName.Text = file.Name;
                    break;
                }
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtFolder.Text = folderBrowserDialog1.SelectedPath;
        }
    }

    class SearchingCriteria
    {
        string slipNo;
        DateTime from;
        DateTime to;

        public SearchingCriteria(string slipNo, DateTime from, DateTime to)
        {
            this.SlipNo = slipNo;
            this.From = from;
            this.To = to;
        }

        public string SlipNo { get => slipNo; set => slipNo = value; }
        public DateTime From { get => from; set => from = value; }
        public DateTime To { get => to; set => to = value; }
    }
}
