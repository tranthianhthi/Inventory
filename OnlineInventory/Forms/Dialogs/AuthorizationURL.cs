using OnlineInventoryLib.Lazada.Responses;
using System;
using System.Windows.Forms;

namespace OnlineInventory.Dialogs
{
    public partial class AuthorizationURL : Form
    {
        readonly OnlineInventoryServiceV2 service;
        readonly OnlineStore store;
        Configurations config;

        public AuthorizationURL()
        {
            InitializeComponent();
            config = new Configurations();
        }

        public AuthorizationURL(OnlineInventoryServiceV2 service, OnlineStore onlineStore, Configurations config )
        {
            InitializeComponent();
            this.service = service;
            this.store = onlineStore;
            this.config = config;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 0)
            {
                System.Diagnostics.Process.Start(textBox1.Text);
                if (this.store == OnlineStore.Shopee)
                    this.Close();
            }
        }

        private void AuthorizationURL_Load(object sender, EventArgs e)
        {
            string url = "";
            switch (this.store)
            {
                case OnlineStore.Lazada:
                    url = service.GetLazadaAuthenticateURL();
                    break;
                case OnlineStore.Shopee:
                    url = service.GetShopeeAuthorizationURL(@"https://partner.shopeemobile.com/api/v1", "846217", "d8c6555a69388b10ed98715a4c229563eb95a624b2f02969f5b361ea61e383e6", "www.acfc.com.vn");
                    break;
                default:
                    break;
            }

            textBox1.Text = url;
        }

        private void btnGetAccessToken_Click(object sender, EventArgs e)
        {
            if (txtLazadaCode.TextLength > 0)
            {
                AccessTokenResponse response = service.GetAccessToken(txtLazadaCode.Text);
                //config.UpdateLazadaAccessToken(response.access_token);
                //config.UpdateLazadaRefreshToken(response.refresh_token);
            }
        }
    }
}