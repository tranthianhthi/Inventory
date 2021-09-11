using Oracle.DataAccess.Client;
using sLib;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace RPAutoSync
{
    public partial class RPAutoSync : Form
    {
        string host = "";
        string sCon = "";
        string store_code = "";
        private ContextMenuStrip notifyContextMenuStrip;
        int TimeCheck = 5;
        public bool isBackground = false;
        public static DateTime dtRunning = DateTime.Now.AddDays(0);

        /// <summary>
        /// Ngày đồng bộ dữ liệu
        /// </summary>
        public string fromDate = string.Format("{0:MM/dd/yyyy}", dtRunning.AddDays(-1));
        public string toDate = string.Format("{0:MM/dd/yyyy}", dtRunning.AddDays(0));

        public string VATpath = "";

        public string LocalLogFile = "{0}.dat";


        private bool started = false;
        private void timeroff_Tick(object sender, EventArgs e)
        {
            //notifyIcon1.Dispose();
            //Application.Exit();

            //label1.Text = started.ToString();
            DateTime now = DateTime.Now;  // Set time auto close app is 23:55
            DateTime start = new DateTime(now.Year, now.Month, now.Day, 08, 30, 0);
            DateTime stop = new DateTime(now.Year, now.Month, now.Day, 23, 30, 0);
            DateTime dtsync = new DateTime(now.Year, now.Month, now.Day, 21, 50, 0);
            if (now > start) start = start.AddDays(1);
            if (now > stop) stop = stop.AddDays(1);

            if (Math.Round((start - now).TotalMinutes) == 0 && !started)
            {
                //MessageBox.Show("Start" + (start - now).Minutes.ToString());
                label1.Text = "Processing START";
                timerServer.Enabled = true;
                started = true;
            }
            if (Math.Round((stop - now).TotalMinutes) == 0 && started)
            {
                //MessageBox.Show("Stop" + (stop - now).Minutes.ToString());
                label1.Text = "Processing STOP";
                timerServer.Enabled = false;
                started = false;
            }
        }
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            
            notifyIcon1.Dispose();
            timerClient.Enabled = false;
            Application.Exit();
        }
        public string ReadFile(string Filename)
        {
            try
            {
                StreamReader myreader = File.OpenText(Filename);
                string ss = myreader.ReadToEnd().Trim();
                myreader.Close();
                if (ss == "")
                    return null;
                else
                    return ss.Trim();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
                return null;
            }
        }
        public DataTable GetData(string mSQL)
        {
            sCon = "Data Source=(DESCRIPTION=" +
                    "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=1521)))" +
                    "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=rproods)));" +
                    "User Id=reportuser;Password=report";

            DBServicesLib dblib = new DBServicesLib();
            DataTable dtable = new DataTable();
            try
            {
                dtable = dblib.GetData(mSQL, sCon);
            }
            catch (Exception e)
            {
                throw (e);
            }
            return dtable;

        }
        
        /// <summary>
        /// Đồng bộ dữ liệu RP
        /// </summary>
        private void Sync()
        {            
            //Process myProcess = new Process();            
            try
            {
                //----------if UseShell = true --> use Windowstyle, else CreateNowindow
                ProcessStartInfo info = new ProcessStartInfo(Application.StartupPath + @"\AutoSyncStore.bat")
                {
                    UseShellExecute = true,
                    Verb = "runas",
                    //info.CreateNoWindow = false;
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                //info.Arguments = "";
                Process.Start(info);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
            }
        }

        private void RPAutoSync_Resize(object sender, EventArgs e)
        {
            //notifyIcon1.BalloonTipTitle = "VAT";
            notifyIcon1.BalloonTipText = "RPAutoSync " + store_code;
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1);
                this.Hide();
            }
            //MessageBox.Show("aaa");
            //else if (FormWindowState.Normal == this.WindowState)
            //{
            //    notifyIcon1.Visible = false;
            //}
        }

        #region Xử lý khi shutdown máy tính
        /*This is the Message constant Windows sent to let the application to let it know is going to shutdown*/
        private const int WM_QUERYENDSESSION = 0x0011;
        private bool isShuttingDown = false;
        /*This is the method (WndProc) which receive Windows Messages we are overriding it to make it works as we want*/

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_QUERYENDSESSION)
            {
                isShuttingDown = true;
            }
            base.WndProc(ref m);
        }
        #endregion
        
        /// <summary>
        /// Nhấn nút đóng chương trình => minimize về start menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RPAutoSync_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    this.WindowState = FormWindowState.Minimized;
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Hiện cửa sổ chính của chương trình
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon1_DoubleClick_1(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void RPAutoSync_Load(object sender, EventArgs e)
        {
            if (host == "192.168.10.12")
                richTextBox1.Visible = true;
        }

        /// <summary>
        /// Tạo danh sách chạy input cho 1 brand
        /// </summary>
        private void Button1_Click(object sender, EventArgs e)
        {
            /// Server mode
            if (host == "192.168.10.12")
            {
                if (richTextBox1.Text == "")
                    MessageBox.Show("Input Vendor");
                else
                {
                    if (!chkbMovelist.Checked)
                    {
                        WriteMltECMFile(richTextBox1.Text.Trim(), ECMFolder + @"ECM_OUT_NIGHT_S.txt");
                        MessageBox.Show("The file exported to " + ECMFolder);
                    }
                    else
                    {
                        WriteStationList(richTextBox1.Text.Trim(), ECMFolder + @"MOVELIST\Templates\StationList.txt");
                        MessageBox.Show("The file exported to " + ECMFolder + @"MOVELIST\Templates\");
                    }
                    //RunMultiECM();
                }
            }
            /// Client mode
            else
            {
                // Send manual then send perform all jobs
                SyncVAT();
                SyncSALE_SUMMARY();
                Sync();
                MessageBox.Show("Done");

            }
        }
        string sbs_str_list = "";
        public string GetListSubFolder()
        {
            string a = "";
            try
            {
                // Lấy danh sách thư mục con trong pooling của ECM
                var directories = Directory.GetDirectories(ECMFolder + "Polling");
                
                // Nếu tên thư mục kết thúc bằng A => xử lý
                foreach (string sub in directories)
                {
                    if (sub.Substring(sub.Length - 1, 1) == "A")
                    {
                        //a += sub.Substring(sub.Length - 7, 7) + ";";
                        string sbs_str = sub.Substring(sub.Length - 5, 1) + "_" + int.Parse(sub.Substring(sub.Length - 4, 3)).ToString();
                        //MessageBox.Show(sbs_str_list);
                        string[] list = sbs_str_list.Split(';');
                        foreach (string ss in list)
                        {
                            //MessageBox.Show(ss);
                            if (ss == sbs_str)
                                a += sub.Substring(sub.Length - 7, 7) + ";";
                        }
                    }
                }
                //MessageBox.Show(a);
                return a.Trim();
            }
            catch (Exception ex)
            {
                return a.Trim();
                //MessageBox.Show(ex.Message);

            }
            
        }
        public string GetListSubFolder(string s)
        {
            string a = "";
            try
            {
                var directories = Directory.GetDirectories(ECMFolder + "Polling");
                //string a = "";
                //MessageBox.Show(directories[0].ToString());
                foreach (string sub in directories)
                {
                   
                    if (sub.Substring(sub.Length - 1, 1) == "A")
                    {
                        if (sub.Substring(sub.Length - 5, 3) == "115" || sub.Substring(sub.Length - 5, 3) == "116" || sub.Substring(sub.Length - 5, 3) == "117" || sub.Substring(sub.Length - 5, 3) == "118" || sub.Substring(sub.Length - 5, 3) == "119")
                        {
                            //MessageBox.Show(sub);
                            //a += sub.Substring(sub.Length - 7, 7) + ";";
                            string sbs_str = sub.Substring(sub.Length - 5, 1) + "_" + int.Parse(sub.Substring(sub.Length - 4, 3)).ToString();
                            //MessageBox.Show(sbs_str_list);
                            string[] list = sbs_str_list.Split(';');
                            foreach (string ss in list)
                            {
                                //MessageBox.Show(ss);
                                if (ss == sbs_str)
                                    a += sub.Substring(sub.Length - 7, 7) + ";";
                            }
                        }
                    }
                }
                //MessageBox.Show(a);
                return a.Trim();
            }
            catch (Exception ex)
            {
                return a.Trim();
                //MessageBox.Show(ex.Message);

            }

        }
        private void RUNSubFolderList()
        {
            try
            {
                string StationList = GetListSubFolder("2");
                //MessageBox.Show(StationList);
                string last_station = "";
                if (StationList != "")
                {
                    //ProcessInServer(StationList);
                    //MessageBox.Show(StationList);
                    //isProcessing = true;
                    string[] station_list = StationList.Split(';');
                    last_station = station_list[station_list.Length - 2].ToString();
                    //foreach (string station in station_list)
                    //{
                    //    //Process myProcess = new Process();
                    //    //MessageBox.Show(station + isProcessing.ToString());
                    //    if (station != "" && !isProcessing)
                    //    {
                    //        ProcessInServer(station);
                    //        //MessageBox.Show(station);
                    //    }
                    //    if (station == last_station)
                    //        isProcessing = false;
                    //}
                    //MessageBox.Show(last_station);

                    while (pos < station_list.Length && !isProcessing)
                    {
                        if (pos > -1)
                        {

                            //isRunning = true;
                            string station = station_list[pos];
                            //pos++;
                            //Process myProcess = new Process();
                            //MessageBox.Show(station + isProcessing.ToString()+last_station);
                            if (station != "" && !isProcessing)
                            {

                                string station_path = ECMFolder + @"Polling\" + station + @"\PROC\IN\";
                                if (CheckECMFolderNOTEmpty(station_path))
                                {
                                    string station_path_LOCK = ECMFolder + @"Polling\" + station + @"\";
                                    if (!CheckECMFolderLocked(station_path_LOCK) && CheckStationActive(station))
                                    {                                        
                                        label1.Text = "Processing " + station;
                                        ProcessInServer(station);
                                        File.AppendAllText(Application.StartupPath + @"\log\completed_log.txt", DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString() + " - " + station + "\n");
                                    }
                                    else
                                    {
                                        pos++;
                                        isProcessing = false;
                                    }
                                    //MessageBox.Show(station + isProcessing.ToString());
                                    
                                    //pos++;
                                    //isProcessing = false;
                                }
                                else
                                {
                                    pos++;
                                    isProcessing = false;
                                }
                                //cur_pos++;
                            }
                            if (station == last_station.Trim() && pos > -1)
                            {
                                isRunning = false;
                                pos = -1;
                                label1.Text = "Processing NULL";
                                //cur_pos = 0;
                                //MessageBox.Show(station + last_station);
                            }
                        }
                        else
                            pos = station_list.Length;

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "RUNSubFolderList", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        /// <summary>  Đường dẫn tới thư mục ECM trên server.</summary>
        private string ECMFolder = @"\\192.168.10.12\ECM\";
        private int pos = -1;
        private int cur_pos = 0;
        private void RUNSyncList()
        {
            try
            {

                string mSQL = "Select 	st.sbs_no ||'_'|| st.store_no as sbs_str,    st.Store_code,    st.Store_name ,to_char(CURRENT_TIMESTAMP, 'mm/dd/yyyy hh:mi:ss AM') as Running_Date_Time   from Store_v st where sbs_no > 0 and store_name is not null order by store_code";
                DataTable dtb1 = new DataTable("Table2");
                dtb1 = GetData(mSQL);  
                DataTable tbEx = new DataTable("Table1");
                //tbEx.Columns.Add("store_no", typeof(String));
                tbEx.Columns.Add("sbs_str", typeof(String));
                tbEx.Columns.Add("last_sync_date_time", typeof(String));
                tbEx.Columns.Add("difference_days", typeof(String));
                tbEx.Columns.Add("difference_hours", typeof(String));
                tbEx.Columns.Add("difference_minutes", typeof(String));

                string filename = ECMFolder + @"stations_ex.xml";
                XmlReader reader = XmlReader.Create(filename);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "STATIONS")
                    {
                        while (reader.Read())
                        {
                            if (reader.Name == "STATION")
                            {
                                foreach (DataRow dr in dtb1.Rows)
                                {
                                    if (dr["sbs_str"].ToString() == reader.GetAttribute("sbs_no") + "_" + reader.GetAttribute("store_no") && reader.GetAttribute("station_char") == "A")
                                    {
                                        TimeSpan difference =  Convert.ToDateTime(dr["running_date_time"]) - Convert.ToDateTime(reader.GetAttribute("last_comm_date_time"));
                                        //MessageBox.Show(difference.Minutes.ToString());
                                        //if (difference.Minutes > 30)
                                        {
                                            DataRow d = tbEx.NewRow();
                                            d["sbs_str"] = reader.GetAttribute("sbs_no") + "_" + reader.GetAttribute("store_no");
                                            d["last_sync_date_time"] = reader.GetAttribute("last_comm_date_time");
                                            d["difference_days"] = difference.Days;
                                            d["difference_hours"] = difference.Hours;
                                            d["difference_minutes"] = difference.Minutes;
                                            tbEx.Rows.Add(d);
                                        }
                                    }
                                }

                            } //end if                        
                        } //end while
                    } //end if
                } //end while
                
                DataTable dtable = sLib.DataTableLib.JoinTable(tbEx, dtb1,"sbs_str");

                DataTable dt = new DataTable();
                foreach (DataColumn dc in dtable.Columns)
                {
                    dt.Columns.Add(dc.ColumnName.ToString().ToLower(), dc.DataType);
                }
                foreach (DataRow dr in dtable.Rows)
                {
                    DataRow d = dt.NewRow();
                    if (int.Parse(dr["difference_minutes"].ToString().Trim()) <= TimeCheck && int.Parse(dr["difference_hours"].ToString().Trim()) < 3 && int.Parse(dr["difference_days"].ToString().Trim()) < 1)
                    {
                        //MessageBox.Show(dr["difference"].ToString());
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            d[i] = dr[i];
                        }
                        dt.Rows.Add(d);

                    }
                }
                dataGridView1.DataSource = null;                    
                dataGridView1.DataSource = dt;

                string StationList = "";
                //string ECM_IN_List = "";
                string last_station = "";
                if (dt.Rows.Count > 0)
                {
                    string last_sbs_store = dt.Rows[dt.Rows.Count - 1]["sbs_str"].ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string sbs_store = dt.Rows[i]["sbs_str"].ToString();
                        StationList += "00" + sbs_store.Substring(0, 1) + sbs_store.Substring(2, sbs_store.Length - 2).PadLeft(3, '0') + "A;";
                       // ECM_IN_List += "-IN -STID:00" + sbs_store.Substring(0, 1) + sbs_store.Substring(2, sbs_store.Length - 2).PadLeft(3, '0') + "A001004A -SHOW -A\n";
                    }
                    //last_station = dt.Rows[dt.Rows.Count - 1]["sbs_str"].ToString();
                    last_station = "00" + last_sbs_store.Substring(0, 1) + last_sbs_store.Substring(2, last_sbs_store.Length - 2).PadLeft(3, '0') + "A";
                }
                //MessageBox.Show(last_station);
                if (StationList != "")
                {
                    string[] station_list = StationList.Split(';');

                    while (pos < station_list.Length && !isProcessing)
                    {
                        if (pos > -1)
                        {
                            string station = station_list[pos];

                            if (station != "" && !isProcessing)
                            {

                                string station_path = @"\\192.168.10.12\apps\ECM\Polling\" + station + @"\PROC\IN\";
                                if (CheckECMFolderNOTEmpty(station_path))
                                {
                                    ProcessInServer(station);
                                    File.AppendAllText(Application.StartupPath + @"\log\completed_log.txt", DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString() + " - " + station + "\n");
                                }
                                else
                                {
                                    pos++;
                                    isProcessing = false;
                                }
                            }
                            if (station == last_station.Trim() && pos > -1)
                            {
                                isRunning = false;
                                pos = -1;
                            }
                        }
                        else
                            pos = station_list.Length;

                    }
                }               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
        private bool CheckInLoop(string[] list)
        {
            bool check = false;
            if (pos < list.Length && !isProcessing)
                check = true;
            return check;
        }
        private bool CheckECMFolderNOTEmpty(string station_path)
        {

            bool isEmpty = true;//= File.Exists(station_path);
            string[] files = System.IO.Directory.GetFiles(station_path, "*.xml", System.IO.SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                isEmpty = false;
            //MessageBox.Show(station_path+isEmpty.ToString());
            return isEmpty;
        }
        private bool CheckECMFolderLocked(string station_path)
        {

            bool isEmpty = true;//= File.Exists(station_path);
            string[] files = System.IO.Directory.GetFiles(station_path, "*.lck", System.IO.SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                isEmpty = false;
            //MessageBox.Show(station_path+isEmpty.ToString());
            return isEmpty;
        }

        /// <summary>
        ///   <strong>Lấy danh sách store</strong>
        /// </summary>
        /// <returns></returns>
        private DataTable LoadStationList()
        {
            DataTable dtable = new DataTable();
            //dtable.Columns.Add("store_no", typeof(String));
            dtable.Columns.Add("sbs_no", typeof(String));
            dtable.Columns.Add("store_no", typeof(String));
            dtable.Columns.Add("active", typeof(String));

            string filename = ECMFolder + @"stations.xml";
            XmlReader reader = XmlReader.Create(filename);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "STATION")
                {
                    //while (reader.Read())
                    //{
                        if (reader.Name == "STATION")
                        {
                            if (reader.GetAttribute("store_no") != "" && reader.GetAttribute("station_char") == "A")
                            {
                                DataRow dr = dtable.NewRow();
                                dr["sbs_no"] = reader.GetAttribute("sbs_no");
                                dr["store_no"] = reader.GetAttribute("store_no");
                                dr["active"] = reader.GetAttribute("active");
                                dtable.Rows.Add(dr);
                            }
                        } //end if                        
                    //} //end while
                } //end if
            } //end while
            //MessageBox.Show(isActive.ToString());
            return dtable;
        }

        /// <summary>  Kiểm tra store có còn active hay không</summary>
        /// <param name="station"> store code</param>
        /// <returns>Trạng thái store ( active = true; inactive = false )</returns>
        private bool CheckStationActive(string station)
        {

            string sbs_no = station.Substring(2, 1);
            string store_no = int.Parse(station.Substring(3, 3)).ToString();
            string station_char = station.Substring(6, 1);
            bool isActive = true;
            foreach (DataRow dr in DtableStationList.Rows)
            {
                if (dr["sbs_no"].ToString() == sbs_no && dr["store_no"].ToString() == store_no && dr["active"].ToString() == "0")
                    isActive = false;
            }
            return isActive;
        }
        public bool isProcessing = false;
        public bool isRunning = false;

        /// <summary>  Xử lý server</summary>
        /// <param name="station"> store code</param>
        private void ProcessInServer(string station)
        {                   
            try
            {
                isProcessing = true;
                Process myProcess = new Process();
                if (station != "")
                {
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.StartInfo.FileName = @"psexec.exe";
                    myProcess.StartInfo.Verb = "runas";
                    myProcess.EnableRaisingEvents = true;
                    myProcess.StartInfo.Arguments = @" \\192.168.10.12 -u acfc\retailpro -p 12345678 -s cmd /c " + ECMFolder + "ECMProc.exe -IN -STID:" + station + " -SHOW -A ";
                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    myProcess.Start();
                    myProcess.Exited += new EventHandler(MyProcess_Exited);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
            }

        }
        
        

        /// <summary>
        /// Thông báo đồng bộ hoàn tất
        /// </summary>
        private void MyProcess_Exited1(object sender, System.EventArgs e)
        {
            MessageBox.Show("DONE");
        }


        private void MyProcess_Exited(object sender, System.EventArgs e)
        {
            //MessageBox.Show("DONE");
        }

        #region Server functions

        /// <summary>Writes the list of ECM IN command in 1 file.</summary>
        /// <param name="dt">List of substore</param>
        /// <param name="filename">Output file name</param>
        public void WriteECM_IN_List(DataTable dt, string filename)
        {
            string inLine = "-IN -STD:00{0}{1}A -SHOW -A";
            try
            {
                StreamWriter sw = new StreamWriter(filename, false);
                
                int iColCount = dt.Columns.Count;

                foreach (DataRow dr in dt.Rows)
                {                    
                    string line = "";
                    
                    string sbs_store = dr["sbs_str"].ToString();
                    //line += "-IN -STID:00" + sbs_store.Substring(0, 1) + sbs_store.Substring(2, sbs_store.Length - 2).PadLeft(3, '0') + "A -SHOW -A";
                    line += string.Format(inLine, sbs_store.Substring(0, 1), sbs_store.Substring(2, sbs_store.Length - 2).PadLeft(3, '0'));
                    //MessageBox.Show(line);

                    sw.Write(line);
                    sw.Write(sw.NewLine);
                }
                //write footer--------
                //sw.Write(footer);
                sw.Close();
                //MessageBox.Show("DONE","Message",MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Tạo file chứa danh sách out cho 1 brand
        /// </summary>
        /// <param name="vend">The vend.</param>
        /// <param name="filename">The filename.</param>
        public void WriteMltECMFile(string vend, string filename)
        {
            try
            {
                DataTable dt = new DataTable();
                string mSQL = "Select 	st.sbs_no ||'_'|| st.store_no as sbs_str,    st.Store_code,    st.Store_name   from Store_v st where sbs_no > 0 and store_name is not null and active = 1 and store_name like '" + vend.ToUpper() + "%' order by store_code";
                dt = GetData(mSQL);
                StreamWriter sw = new StreamWriter(filename, false);

                int iColCount = dt.Columns.Count;

                foreach (DataRow dr in dt.Rows)
                {
                    string line = "";

                    string sbs_store = dr["sbs_str"].ToString();
                    line += "-OUT -STID:00" + sbs_store.Substring(0, 1) + sbs_store.Substring(2, sbs_store.Length - 2).PadLeft(3, '0') + "A -PRF:INVN_HQ -SHOW -A";
                    //MessageBox.Show(line);

                    sw.Write(line);
                    sw.Write(sw.NewLine);
                }
                //write footer--------
                //sw.Write(footer);
                sw.Close();
                //MessageBox.Show("DONE","Message",MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        #endregion


        /// <summary>Ghi file PROCESS OUT</summary>
        /// <param name="vend">tên brand</param>
        /// <param name="filename">file dữ liệu cuối</param>
        public void WriteStationList(string vend, string filename)
        {
            try
            {
                DataTable dt = new DataTable();
                string mSQL = "Select 	st.sbs_no ||'_'|| st.store_no as sbs_str,    st.Store_code,    st.Store_name   from Store_v st where sbs_no > 0 and store_name is not null and active = 1 and store_name like '" + vend.ToUpper() + "%' order by store_code";
                dt = GetData(mSQL);
                StreamWriter sw = new StreamWriter(filename, false);

                int iColCount = dt.Columns.Count;
                string line = "";
                for (int i = 0; i< dt.Rows.Count; i++)
                {                   
                    DataRow dr = dt.Rows[i];
                    string sbs_store = dr["sbs_str"].ToString();
                    if (i == dt.Rows.Count - 1)
                        line += "00" + sbs_store.Substring(0, 1) + sbs_store.Substring(2, sbs_store.Length - 2).PadLeft(3, '0') + "A";
                    else
                        line += "00" + sbs_store.Substring(0, 1) + sbs_store.Substring(2, sbs_store.Length - 2).PadLeft(3, '0') + "A,";
                                                          
                }
                sw.Write(line);
                sw.Write(sw.NewLine);
                //write footer--------
                //sw.Write(footer);
                sw.Close();
                //MessageBox.Show("DONE","Message",MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        public void WriteLog(String header, string filename)
        {
            StreamWriter sw = new StreamWriter(filename, false);

            // First we will write the headers.
            sw.Write(header);
            sw.Write(sw.NewLine);

            sw.Close();
            //MessageBox.Show("DONE","Message",MessageBoxButtons.OK);
        }


        DataTable DtableStationList = new DataTable();

        #region form events

        #region constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public RPAutoSync()
        {
            InitializeComponent();

            //DateTime now = DateTime.Now;  // Set time auto close app is 23:55
            //DateTime when = new DateTime(now.Year, now.Month, now.Day, 23, 55, 0);
            //if (now > when) when = when.AddDays(1);
            //timeroff.Interval = (int)((when - now).TotalMilliseconds);
            //timeroff.Start();
            VATpath = System.IO.Directory.GetParent(Application.StartupPath).FullName + @"\VAT Setup\";

            store_code = ReadFile(VATpath + @"\store_code.txt");
            host = ReadFile(Application.StartupPath + @"\IPAddress.txt");
            TimeCheck = int.Parse(ReadFile(Application.StartupPath + @"\TimeCheck.txt").Trim());
            sCon = "Data Source=(DESCRIPTION=" +
                 "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=1521)))" +
                 "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=rproods)));" +
                 "User Id=reportuser;Password=report";
            //store_code = ReadFile(Application.StartupPath + @"\store_code.txt");

            //-------Context menu when right click on the Icon
            notifyContextMenuStrip = new ContextMenuStrip();
            var messageMenuItem = notifyContextMenuStrip.Items.Add("IP: " + host);
            //messageMenuItem.Click += new System.EventHandler(this.SyncVAT);
            var showMenuItem = notifyContextMenuStrip.Items.Add("Show");
            showMenuItem.Click += new System.EventHandler(this.notifyIcon1_DoubleClick);
            notifyContextMenuStrip.Items.Add("-");
            var exitMenuItem = notifyContextMenuStrip.Items.Add("Exit");
            exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            //exitMenuItem.Image = ...;

            notifyIcon1.ContextMenuStrip = notifyContextMenuStrip;
        }

        /// <summary>
        /// Constructor with parameter
        /// </summary>
        /// <param name="RunOption"></param>
        public RPAutoSync(string RunOption)
        {
            InitializeComponent();

            //DateTime now = DateTime.Now;  // Set time auto close app is 23:55
            //DateTime when = new DateTime(now.Year, now.Month, now.Day, 23, 55, 0);
            //if (now > when) when = when.AddDays(1);
            //timeroff.Interval = (int)((when - now).TotalMilliseconds);
            //timeroff.Start();

            host = ReadFile(Application.StartupPath + @"\IPAddress.txt");
            TimeCheck = int.Parse(ReadFile(Application.StartupPath + @"\TimeCheck.txt").Trim());
            sCon = "Data Source=(DESCRIPTION=" +
                 "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=1521)))" +
                 "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=rproods)));" +
                 "User Id=reportuser;Password=report";

            store_code = ReadFile(Application.StartupPath + @"\store_code.txt");

            //-------Context menu when right click on the Icon
            notifyContextMenuStrip = new ContextMenuStrip();
            var messageMenuItem = notifyContextMenuStrip.Items.Add("Store: " + store_code + " IP: " + host);
            //messageMenuItem.Click += new System.EventHandler(this.SyncVAT);
            var showMenuItem = notifyContextMenuStrip.Items.Add("Show");
            showMenuItem.Click += new System.EventHandler(this.notifyIcon1_DoubleClick);
            notifyContextMenuStrip.Items.Add("-");
            var exitMenuItem = notifyContextMenuStrip.Items.Add("Exit");
            exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            //exitMenuItem.Image = ...;

            notifyIcon1.ContextMenuStrip = notifyContextMenuStrip;
            isBackground = true;
            StartWork(RunOption.Trim());
        }

        #endregion


        private void RPAutoSync_Shown(object sender, EventArgs e)
        {
            label1.Text = "IP: " + host;
            ///Nếu là server
            if (host == "192.168.10.12")
            {
                string mSQL = "Select 	st.sbs_no ||'_'|| st.store_no as sbs_str,    st.Store_code,    st.Store_name ,to_char(CURRENT_TIMESTAMP, 'mm/dd/yyyy hh:mi:ss AM') as Running_Date_Time   from Store_v st where sbs_no > 0 and store_no < 240 and store_name is not null and active = 1 order by store_code";
                DataTable dtb1 = new DataTable("Table2");
                dtb1 = GetData(mSQL);
                //string sbs_str_list = "";
                if (dtb1.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtb1.Rows)
                        sbs_str_list += dr["sbs_str"].ToString() + ";";
                }
                //MessageBox.Show(dtb1.Rows.Count.ToString());
                DtableStationList = LoadStationList();
                //dataGridView1.DataSource = DtableStationList;
                //button1.Visible = true;
                chkbMovelist.Visible = true;
                label1.Text = host;
                if (!started)
                {
                    timerServer.Enabled = true;
                    started = true;
                }
            }

            // Máy client
            else
            {
                timerSyncVAT.Interval = TimeCheck * 60 * 1000;
                label1.Text = "IP: " + host;
                button1.Text = "Send";
                //MessageBox.Show(timerSyncVAT.Interval.ToString());
            }
        }

        /// <summary>
        /// Hàm xử lý timer cho phía server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                
                if (host == "192.168.10.12" && !isRunning)
                {
                    //MessageBox.Show("hello server");
                    pos = 0;
                    //RUNSyncList();
                    RUNSubFolderList();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        /// <summary>
        /// Sự kiện đồng bộ lên server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerSyncVAT_Tick(object sender, EventArgs e)
        {
            if (host == "127.0.0.1")
            {
                try
                {
                    label1.Text = "Sync Client";

                    //MessageBox.Show("hello client");
                    //string sql = "select * from customer where store_no = (select store_no from store where store_code ='" + store_code + "' ) and created_date >= (CURRENT_TIMESTAMP -  (3 / 1440))";
                    string sql = "select * from invoice where created_date >= (CURRENT_TIMESTAMP -  (" + (TimeCheck + 2).ToString() + " / 1440))"; // Select Sale Transaction in 17 minutes before
                    DataTable dt = GetData(sql);
                    if (dt.Rows.Count > 0)
                    {
                        // Automatic: Perform if has data in the Interval time
                        SyncVAT();
                        SyncSALE_SUMMARY();
                        Sync();
                    }
                    ////////////////////////////////////////////////
                    //MessageBox.Show(sql);
                    //SyncVAT();
                    //SyncSALE_SUMMARY();
                    //Sync();
                    //MessageBox.Show("DONE");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    //return null;
                }
            }
        
        }

        #region Background worker

        /// <summary>
        /// Khởi tạo background worker, gán các event và chạy background worker.
        /// </summary>
        /// <param name="p"></param>
        private void StartWork(string p)
        {
            /* Khởi tạo background worker */
            BackgroundWorker bgw1 = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };

            /* Gán event handlers */
            bgw1.ProgressChanged += new ProgressChangedEventHandler(bgw1_ProgressChanged);
            bgw1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw1_RunWorkerCompleted);
            bgw1.DoWork += new DoWorkEventHandler(bgw1_DoWork);

            bgw1.RunWorkerAsync(p);
        }

        /// <summary>
        /// Hàm xử lý sự kiện thực thi background worker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bgw1_DoWork(object sender, DoWorkEventArgs e)
        {
            string toption = (string)e.Argument;
            WorkProcess(toption, sender as BackgroundWorker);
        }

        /// <summary>
        /// Hàm xử lý cập nhật trạng thái background worker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bgw1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Hàm xử lý sự kiện background worker chạy xong
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bgw1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
            }

            else if (!(e.Error == null))
            {
            }

            else
            {
            }
            if (isBackground)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Hàm chính - xử lý business logic của background worker
        /// </summary>
        /// <param name="strRunOption"></param>
        /// <param name="bwg"></param>
        private void WorkProcess(string strRunOption, BackgroundWorker bwg)
        {
            try
            {
                string runningdateime = (string.Format("{0:yyyy}", DateTime.Now) + string.Format("{0:MM}", DateTime.Now) + string.Format("{0:dd}", DateTime.Now.AddDays(0)) + "_" + string.Format("{0:HH}", DateTime.Now) + string.Format("{0:mm}", DateTime.Now) + string.Format("{0:ss}", DateTime.Now));//6

                if (strRunOption == "1")
                {
                    timerServer.Enabled = true;
                    timerServer.Interval = 3000;
                    timerServer.Start();
                    if (host == "192.168.10.12" && !isRunning)
                    {
                        //MessageBox.Show("hello server");
                        pos = 0;
                        //RUNSyncList();
                        RUNSubFolderList();
                    }
                }

            }
            catch { }
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            //RUNSubFolderList();
        }


        #endregion


        #endregion

        #region Sync functions


        /// <summary>
        /// Đồng bộ hóa đơn VAT
        /// </summary>
        private void SyncVAT()
        {
            try
            {
                if (store_code != "000" && store_code != "400")
                {
                    string serverCon = "Data Source=(DESCRIPTION=" +
                         "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + "rp.acfc.com.vn" + ")(PORT=1521)))" +
                         "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=rproods)));" +
                         "User Id=reportuser;Password=report";
                    string clientsql = "select v.store_code,v.invc_sid,v.vat_invoice_no,v.predigit,v.continuousnumber,v.created_date,v.invc_no,v.invoicesign from vatinvoice v,invoice i where v.invc_sid = i.invc_sid";
                    clientsql += " and Trunc(i.created_date) >= TO_DATE('" + fromDate + "', 'MM/DD/YYYY')";
                    clientsql += " and Trunc(i.created_date) <= TO_DATE('" + toDate + "', 'MM/DD/YYYY')";
                    DataTable clientTable = GetData(clientsql);
                    string invc_list = "";
                    for (int i = 0; i < clientTable.Rows.Count; i++)
                    {
                        if (i == clientTable.Rows.Count - 1)
                            invc_list += clientTable.Rows[i]["invc_sid"].ToString();
                        else
                            invc_list += clientTable.Rows[i]["invc_sid"].ToString() + ",";
                    }
                    if (clientTable.Rows.Count > 0)
                    {
                        using (OracleConnection connection = new OracleConnection(serverCon))
                        {
                            connection.Open();
                            OracleCommand command = connection.CreateCommand();
                            command.CommandType = CommandType.Text;
                            //---------------sql delete Record on server
                            command.CommandText = "delete vatinvoice where store_code = '" + store_code + "' and invc_sid in (" + invc_list + ")";
                            command.Connection = connection;
                            command.ExecuteNonQuery();
                            command.CommandText = "Commit";
                            //-----------------sql insert to server table----------
                            for (int i = 0; i < clientTable.Rows.Count; i++)
                            {
                                command.CommandText = "insert into vatinvoice (store_code,invc_sid,vat_invoice_no,predigit,continuousnumber,created_date,invc_no,invoicesign) ";
                                command.CommandText += "values('"
                                    + clientTable.Rows[i]["store_code"].ToString() + "' ,"
                                    + clientTable.Rows[i]["invc_sid"].ToString() + " ,'"
                                    + clientTable.Rows[i]["vat_invoice_no"].ToString() + "' ,'',"
                                    + clientTable.Rows[i]["continuousnumber"].ToString() + " ,'' ,"
                                    + clientTable.Rows[i]["invc_no"].ToString() + " ,'"
                                    + clientTable.Rows[i]["invoicesign"].ToString() + "')";
                                command.Connection = connection;
                                command.ExecuteNonQuery();
                                command.CommandText = "Commit";
                            }
                            command.Connection.Close();
                        }
                    }
                    //MessageBox.Show("( " + clientTable.Rows.Count + " ) HD đã đồng bộ", "Message", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Đồng bộ doanh thu cửa hàng
        /// </summary>
        private void SyncSALE_SUMMARY()
        {
            try
            {
                if (store_code != "000" && store_code != "400")
                {

                    string serverCon = "Data Source=(DESCRIPTION=" +
                         "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + "rp.acfc.com.vn" + ")(PORT=1521)))" +
                         "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=rproods)));" +
                         "User Id=reportuser;Password=report";

                    string vendor = ReadFile(VATpath + @"\Vend_code.txt");
                    string clientsql = ReadFile(Application.StartupPath + @"\SaleSummary.sql");
                    clientsql = clientsql.Replace("strcode",store_code);
                    clientsql = clientsql.Replace("fDate", fromDate);
                    DataTable clientTable = GetData(clientsql);
                    //MessageBox.Show(clientsql);
                    //string st_code = clientTable.Rows[0]["store_code"].ToString();
                    string store_name = "";
                    string CREATED_DATE = "";
                    //string store_code = vendor + "' ,'',"
                    string QTY = "";
                    string AMT = "";
                    string BILL_COUNT = "";
                    string COLLECTION = "";
                    string NOTE = "";
                    string CASH = "";
                    string CREDIT = "";
                    string GIFT_ACFC = "";
                    string GIFT_OTHER = "";
                    string DIFF = "";
                    string CUST_COUNT = "";
                    
                    using (OracleConnection connection = new OracleConnection(serverCon))
                    {
                        connection.Open();
                        OracleCommand command = connection.CreateCommand();
                        command.CommandType = CommandType.Text;
                        //---------------sql delete Record on server
                        command.CommandText = "delete INVC_SUM_MANUAL where store_code = '" + store_code + "' and Trunc(created_date) = TO_DATE('" + fromDate + "', 'MM/DD/YYYY')";
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                        command.CommandText = "Commit";
                        //-----------------sql insert to server table----------
                        if (clientTable.Rows.Count > 0)
                        {
                            store_name = clientTable.Rows[0]["store_name"].ToString();
                            CREATED_DATE = clientTable.Rows[0]["CREATED_DATE"].ToString();
                            //string store_code = vendor + "' ,'',"
                            QTY = clientTable.Rows[0]["QTY"].ToString();
                            AMT = clientTable.Rows[0]["AMT"].ToString();
                            BILL_COUNT = clientTable.Rows[0]["BILL_COUNT"].ToString();
                            COLLECTION = clientTable.Rows[0]["COLLECTION"].ToString();
                            NOTE = clientTable.Rows[0]["NOTE"].ToString();
                            CASH = clientTable.Rows[0]["CASH"].ToString();
                            CREDIT = clientTable.Rows[0]["CREDIT"].ToString();
                            GIFT_ACFC = clientTable.Rows[0]["GIFT_ACFC"].ToString();
                            GIFT_OTHER = clientTable.Rows[0]["GIFT_OTHER"].ToString();
                            DIFF = clientTable.Rows[0]["DIFF"].ToString();
                            CUST_COUNT = clientTable.Rows[0]["CUST_COUNT"].ToString();

                            command.CommandText = "Insert into INVC_SUM_MANUAL(STORE_CODE,STORE_NAME,CREATED_DATE,VEND_CODE,QTY,AMT,BILL_COUNT,COLLECTION,NOTE, CASH,CREDIT,GIFT_ACFC,GIFT_OTHER,  DIFF, CUS_COUNT) ";
                            command.CommandText += "values('"
                                + store_code + "' ,'"
                                + store_name + "' ,"
                                + "TO_DATE('"+fromDate + "', 'MM/DD/YYYY'),'"
                                + vendor + "','"
                                + QTY + "' ,'"
                                + AMT + "' ,'"
                                + BILL_COUNT + "' ,'"
                                + COLLECTION + "' ,'"
                                + NOTE + "' ,'"
                                + CASH + "' ,'"
                                + CREDIT + "' ,'"
                                + GIFT_ACFC + "' ,'"
                                + GIFT_OTHER + "' ,'"
                                + DIFF + "' ,'"
                                + CUST_COUNT + "')";
                            //MessageBox.Show(command.CommandText);
                            command.Connection = connection;
                            command.ExecuteNonQuery();
                            command.CommandText = "Commit";
                        }
                        else
                        {
                            string clientsql1 = "select store_code, store_name from store where store_code ='" + store_code + "'";
                            DataTable clientTable1 = GetData(clientsql1);

                            command.CommandText = "Insert into INVC_SUM_MANUAL(STORE_CODE,STORE_NAME,CREATED_DATE,VEND_CODE,QTY,AMT,BILL_COUNT,COLLECTION,NOTE, CASH,CREDIT,GIFT_ACFC,GIFT_OTHER,  DIFF, CUS_COUNT) ";
                            command.CommandText += "values('"
                                + clientTable1.Rows[0]["store_code"].ToString() + "' ,'"
                                + clientTable1.Rows[0]["store_name"].ToString() + " ',"
                                + "TO_DATE('" + fromDate + "', 'MM/DD/YYYY'),'"
                                + vendor + "',"
                                + 0 + " ,"
                                + 0 + " ,"
                                + 0 + " ,"
                                + 0 + " ,'"
                                + "No Sales" + " ',"
                                + 0 + " ,"
                                + 0 + " ,"
                                + 0 + " ,"
                                + 0 + " ,"
                                + 0 + " ,"
                                + 0 + ")";
                            //MessageBox.Show(command.CommandText);
                            command.Connection = connection;
                            command.ExecuteNonQuery();
                            command.CommandText = "Commit";
                        }
                        command.Connection.Close();
                    }                    
                    //MessageBox.Show("( " + clientTable.Rows.Count + " ) HD đã đồng bộ", "Message", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        #endregion

    }
}
////SetUpTimer(new TimeSpan(16, 00, 00));
//private System.Threading.Timer timer;
//private void SetUpTimer(TimeSpan alertTime)
//{
//    DateTime current = DateTime.Now;
//    TimeSpan timeToGo = alertTime - current.TimeOfDay;
//    if (timeToGo < TimeSpan.Zero)
//    {
//        return;//time already passed
//    }
//    this.timer = new System.Threading.Timer(x => {this.SomeMethodRunsAt1600();}, null, timeToGo, Timeout.Infinite);
//}

//private void SomeMethodRunsAt1600()
//{
//    //this runs at 16:00:00
//}
//private void tmrMain_Tick(object sender, EventArgs e)
//{
//    if (ShouldRunNow())
//        PerformClick();
//}

//private void PerformClick()
//{
//    MessageBox.Show(DateTime.Now.ToShortTimeString());
//}

//private bool ShouldRunNow()
//{
//    TimeSpan startTime = new TimeSpan(14, 04, 0);
//    TimeSpan endTime = new TimeSpan(18, 0, 0);
//    DateTime now = DateTime.Now;

//    // Only run Saturday and Sunday
//    if (now.DayOfWeek != DayOfWeek.Saturday && now.DayOfWeek != DayOfWeek.Sunday)
//        return false;

//    // Only run between the specified times.
//    if (endTime == startTime)
//        return true;

//    if (endTime < startTime)
//        return now.TimeOfDay <= endTime || now.TimeOfDay >= startTime;

//    return now.TimeOfDay >= startTime && now.TimeOfDay <= endTime;
//}


#region RunMultiECM

//private void RunMultiECM()
//{
//    try
//    {
//        Process myProcess = new Process();
//        {
//            myProcess.StartInfo.UseShellExecute = true;
//            myProcess.StartInfo.FileName = @"psexec.exe";
//            myProcess.StartInfo.Verb = "runas";
//            myProcess.EnableRaisingEvents = true;
//            myProcess.StartInfo.Arguments = @" \\192.168.10.12 -u acfc\retailpro -p 12345678 -s cmd /c \\192.168.10.12\APPS\ecm\MultECM.exe \\192.168.10.12\APPS\ecm\ECM_OUT_NIGHT_S.txt 5 0";
//            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
//            myProcess.Start();
//            myProcess.Exited += new EventHandler(MyProcess_Exited1);
//        }
//    }
//    catch (Exception ex)
//    {
//        MessageBox.Show(ex.Message.ToString());
//    }

//}

#endregion