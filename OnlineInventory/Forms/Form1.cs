using ClosedXML.Excel;
using OnlineInventory.Dialogs;
using OnlineInventoryLib.Lazada.Models;
using OnlineInventoryLib.Shopee.Models;
using OnlineInventoryLib.Tiki.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnlineInventory
{
    public partial class Form1 : Form
    {
        #region Properties

        /// <summary>
        /// Service để đồng bộ số tồn lên online store
        /// </summary>
        OnlineInventoryServiceV2 service;

        /// <summary>
        /// Trạng thái đang load 
        /// </summary>
        private bool isLoading;

        /// <summary>
        /// Danh sách SellerSku ( UPC ) đang available trên Lazada
        /// </summary>
        BindingList<LazadaSKU> skus;


        BindingList<ShopeeVariation> shopeeVariations;

        Configurations config;

        DateTime? lastSync = null;

        BindingList<TikiProduct> tikiList;

        #endregion



        #region Form events

        public Form1()
        {
            InitializeComponent();

            // Vì các hàm chạy sync đều là async function nên phải đăng ký bằng EventHandler
            try
            {
                this.button1.Click +=
                     new EventHandler(
                         async (sender, events) => await this.Button1_Click(sender, events)
                     );
                this.timer1.Tick +=
                     new EventHandler(
                         async (sender, events) => await this.Timer1_Tick(sender, events)
                     );
                this.excludeUPCsToolStripMenuItem.Click +=
                    new EventHandler(
                         async (sender, events) => await this.ExcludeUPCsToolStripMenuItem_Click(sender, events)
                     );

                this.Load +=
                     new EventHandler(
                         async (sender, events) => await this.Form1_Load(sender, events)
                     );
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// Authorize Lazada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateAuthorizationURLToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AuthorizationURL frm = new AuthorizationURL(service, OnlineStore.Lazada, this.config);
            frm.ShowDialog();
        }

        /// <summary>
        /// Authorize Shopee 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateAuthorizationURLToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AuthorizationURL frm = new AuthorizationURL(service, OnlineStore.Shopee, this.config);
            frm.ShowDialog();
        }

        private void LoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.ShowDialog();

            settingsToolStripMenuItem.Enabled = (login.DialogResult == DialogResult.OK);
            adminToolStripMenuItem.Enabled = (login.DialogResult == DialogResult.OK);
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }
            notifyIcon1.Visible = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
                notifyIcon1.Visible = true;
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            notifyIcon1.Visible = false;
        }

        private void ShowAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (config.EnableLazada)
                {
                    if (textBox1.TextLength > 0)
                    {
                        var Lazresult = skus.Where(i => i.SellerSku.ToString().Contains(textBox1.Text));
                        dgLazada.DataSource = Lazresult.ToList();
                    }
                    else
                    {
                        dgLazada.DataSource = skus.OrderBy(l => l.Quantity);
                    }
                }


                if (config.EnableShopee)
                {
                    if (textBox1.TextLength > 0)
                    {
                        var ShopeeRes = shopeeVariations.Where(i => i.variation_sku.ToString().Contains(textBox1.Text));
                        dgShopee.DataSource = ShopeeRes.ToList();
                    }
                    else
                    {
                        dgShopee.DataSource = shopeeVariations.OrderBy(s => s.stock);
                    }
                }
            }

        }

        /// <summary>
        /// Button gửi mail - chưa chạy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button3_Click(object sender, EventArgs e)
        {
            //ACFCEmailSender emailSender = new ACFCEmailSender("smtp.office365.com", "STARTTLS / smtp.office365.com", 857, "it.support@acfc.com.vn", "itACFC@2020", true);
            //emailSender.SendMail("it.support@acfc.com.vn", "vu.nguyen@acfc.com.vn", "Lazada product", "Lazada product is updated.", fromAlias: "ACFC IT Dept");
        }


        private void ApplicationConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            Config config = new Config() { config = this.config, service = this.service };
            config.ShowDialog();
            timer1.Start();
        }


        /// <summary>
        /// Export Lazada grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button4_Click(object sender, EventArgs e)
        {
            ExportToExcel<LazadaSKU>(skus);
        }

        /// <summary>
        /// Export Shopee grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button5_Click(object sender, EventArgs e)
        {
            ExportToExcel<ShopeeVariation>(shopeeVariations);
        }

        /// <summary>
        /// Sort data trên grid ( building )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgShopee_ColumnSortModeChanged(object sender, DataGridViewColumnEventArgs e)
        {
            //MessageBox.Show( e.Column.Name );
        }

        /// <summary>
        /// Các event chạy async
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        #region Async events

        private async Task Form1_Load(object sender, EventArgs e)
        {
            config = new Configurations();

            lblPickupStore.Text = config.pickupStore;
            chkShopee.Checked = config.IsShopeeStore;
            chkLazada.Checked = config.IsLazadaStore;
            chkTiki.Checked = config.EnableTiki;
            isLoading = true;
            timer1.Interval = config.Interval;

            service = new OnlineInventoryServiceV2(config, Environment.CurrentDirectory);

            if (config.IsLazadaStore || config.IsShopeeStore || config.EnableTiki)
            {
                await LoadConfigAndPrepareApplication();
            }
            else
            {
                timer1.Stop();
            }



            if (config.RunAsTimer)
            {
                // do nothing
            }
            else
            {
                //if (config.SendNotificationMail)
                //{

                //}
                //Environment.Exit(Environment.ExitCode);
            }

        }

        /// <summary>
        /// Đồng bộ dữ liệu manual
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task Button1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            lastSync = DateTime.Now;
            bool result = await service.Init();
            if (result)
            {
                //SyncData();
                await Timer1_Tick(sender, e);
            }
        }

        /// <summary>
        /// Timer điều khiển việc sync stock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task Timer1_Tick(object sender, EventArgs e)
        {
            if (isLoading)
            {
                
            }
            else
            {
                isLoading = true;
                UpdateInterface();

                timer1.Stop();
                
                lblLastSync.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                await SyncData();
            }
        }


        private async Task ExcludeUPCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diagOpenFile.ShowDialog();
            if (diagOpenFile.FileName.Length > 0)
            {
                //string destFile = Environment.CurrentDirectory + "\\" + config.OnlineFilePath; // local
                string destFile = @"X:\" + config.OnlineFilePath;  // server
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                File.Copy(diagOpenFile.FileName, destFile, true);

                bool result = await service.Init();
            }
            else
            {

            }
        }

        #endregion
       
        #endregion



        #region Private helpers

        /// <summary>
        /// Enable / disable button dựa vào trạng thái đang sync hay không.
        /// </summary>
        public void UpdateInterface()
        {
            contextMenuStrip1.Enabled = !isLoading;
            button1.Enabled = !isLoading;
            button2.Enabled = !isLoading;
            button4.Enabled = !isLoading;
            button5.Enabled = !isLoading;

            if (isLoading)
                notifyIcon1.BalloonTipText = "Updating online inventory....";
            else
                notifyIcon1.BalloonTipText = "Online Store"; 
        }

        /// <summary>
        /// Display dữ liệu vừa sync trong service lên grid
        /// </summary>
        public void DisplayDataOnGrid()
        {

            if (config.IsLazadaStore)
            {
                skus = new BindingList<LazadaSKU>(service.LazadaSKU);
                dgLazada.DataSource = skus;
            }
            if (config.IsShopeeStore)
            {
                shopeeVariations = new BindingList<ShopeeVariation>(service.ShopeeVariations);
                dgShopee.DataSource = shopeeVariations;
            }
            if (config.EnableTiki)
            {
                tikiList = new BindingList<TikiProduct>(service.TikiProducts.ToList<TikiProduct>());
                dgTiki.DataSource = tikiList;
            }
        }


        /// <summary>
        /// Khởi tạo các dữ liệu của Sync service
        /// </summary>
        private async Task LoadConfigAndPrepareApplication()
        {
           
            dgLazada.AutoGenerateColumns = false;
            dgPrism.AutoGenerateColumns = false;
            dgShopee.AutoGenerateColumns = false;
            dgTiki.AutoGenerateColumns = false;

            try
            {
                bool result = await this.service.Init(); // Init();
                if (result)
                {
                    await SyncData();
                    DisplayDataOnGrid();
                }
                else
                {
                    MessageBox.Show("Store không active.\nVui lòng kiểm tra lại cấu hình trên CSDL");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                isLoading = false;
                UpdateInterface();
            }
        }

        

        /// <summary>
        /// Cập nhật số tồn từ Prism qua OnlineStore
        /// </summary>
        private async Task SyncData()
        {
            try
            {
                //DateTime? syncTime = lastSync;
                lastSync = DateTime.Now;
                await service.UpdateOnlineStock();
            }
            catch { }
            finally
            {
                isLoading = false;
                UpdateInterface();

                
                DisplayDataOnGrid();
                timer1.Start();
                GC.Collect();
            }
        }


        /// <summary>
        /// Export dữ liệu trên grid ra file Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        private void ExportToExcel<T>(BindingList<T> data)
        {
            diagSaveFile.ShowDialog();

            if ( !string.IsNullOrEmpty(diagSaveFile.FileName) )
            {
                try
                {
                    var workbook = new XLWorkbook();
                    var ws = workbook.Worksheets.Add("Sheet1");


                    PropertyInfo[] properties = data.First().GetType().GetProperties();
                    List<string> headerNames = properties.Select(prop => prop.Name).ToList();
                    for (int i = 0; i < headerNames.Count; i++)
                    {
                        ws.Cell(1, i + 1).Value = headerNames[i];
                    }

                    ws.Cell(2, 1).InsertData(data.AsEnumerable());

                    workbook.SaveAs(diagSaveFile.FileName);
                    MessageBox.Show("DONE");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        #endregion

    }
}