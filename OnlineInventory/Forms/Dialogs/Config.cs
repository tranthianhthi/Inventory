using System;
using System.Windows.Forms;

namespace OnlineInventory
{
    public partial class Config : Form
    {
        public Configurations config { get; set; }
        public OnlineInventoryServiceV2 service { get; set; }

        public Config()
        {
            InitializeComponent();
        }

        private void Config_Load(object sender, EventArgs e)
        {
            if (config == null)
                return;

            txtPickupStore.Text = config.pickupStore;
            chkRunAsTimer.Checked = config.RunAsTimer;
            txtTimer.Text = (config.Interval / 60000).ToString();
            //chkSendMail.Checked = config.SendNotificationMail;   
            //txtEmail.Text = config.EmailAddresses;
            //chkUseOfflineFile.Checked = config.UseKeepOfflineFile;
            txtOnlineFile.Text = config.OnlineFilePath;

            chkLaz.Checked = config.IsLazadaStore;
            //txtLazDomain.Text = config.LazadaDomain;
            //txtLazAppKey.Text = config.LazadaAppKey;
            //txtLazAppSecret.Text = config.LazadaAppSecret;
            //txtLazAccessKey.Text = config.LazadaAccessToken;
            //txtLazRefreshKey.Text = config.LazadaRefreshToken;
            //chkMultiWarehouse.Checked = config.UseMultiWarehouse;
            //txtWarehouseCode.Text = config.WarehouseCode;

            chkShopee.Checked = config.IsShopeeStore;
            
        }

        private void chkRunAsTimer_CheckedChanged(object sender, EventArgs e)
        {
            txtTimer.Enabled = chkRunAsTimer.Checked;
        }

        private void chkSendMail_CheckedChanged(object sender, EventArgs e)
        {
            txtEmail.Enabled = chkSendMail.Checked;
        }

        private void chkLaz_CheckedChanged(object sender, EventArgs e)
        {
            grbLaz.Enabled = chkLaz.Checked;
        }

        private void chkShopee_CheckedChanged(object sender, EventArgs e)
        {
            grbShopee.Enabled = chkShopee.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtOnlineFile.Enabled = chkUseOfflineFile.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dialogs.AuthorizationURL authorizationURL = new Dialogs.AuthorizationURL(service, OnlineStore.Lazada, this.config);
            authorizationURL.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
