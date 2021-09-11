namespace OnlineInventory
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dgLazada = new System.Windows.Forms.DataGridView();
            this.UPC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrismOnHandQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OfflineQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excludeUPCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cấuHìnhPhầnMềmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adminToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createAuthorizationURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createAuthorizationURLToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.lazadaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createAuthorizationURLToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.applicationConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTotalSKUs = new System.Windows.Forms.Label();
            this.diagOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button4 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.dgShopee = new System.Windows.Forms.DataGridView();
            this.ShopeePrismUPC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShopeePrismOnHand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShopeeKeepOffline = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShopeeOnSale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dgPrism = new System.Windows.Forms.DataGridView();
            this.PrismUPC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrismOH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showAppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.lblLastSync = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblPickupStore = new System.Windows.Forms.Label();
            this.chkLazada = new System.Windows.Forms.CheckBox();
            this.chkShopee = new System.Windows.Forms.CheckBox();
            this.diagSaveFile = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dgLazada)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgShopee)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPrism)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgLazada
            // 
            this.dgLazada.AllowUserToAddRows = false;
            this.dgLazada.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Gainsboro;
            this.dgLazada.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgLazada.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgLazada.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgLazada.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UPC,
            this.PrismOnHandQty,
            this.OfflineQty,
            this.Quantity});
            this.dgLazada.Location = new System.Drawing.Point(6, 42);
            this.dgLazada.Name = "dgLazada";
            this.dgLazada.ReadOnly = true;
            this.dgLazada.Size = new System.Drawing.Size(729, 378);
            this.dgLazada.TabIndex = 0;
            // 
            // UPC
            // 
            this.UPC.DataPropertyName = "SellerSKU";
            this.UPC.HeaderText = "UPC";
            this.UPC.Name = "UPC";
            this.UPC.ReadOnly = true;
            // 
            // PrismOnHandQty
            // 
            this.PrismOnHandQty.DataPropertyName = "PrismOHQty";
            this.PrismOnHandQty.HeaderText = "Prism on hand qty (1)";
            this.PrismOnHandQty.Name = "PrismOnHandQty";
            this.PrismOnHandQty.ReadOnly = true;
            // 
            // OfflineQty
            // 
            this.OfflineQty.DataPropertyName = "OfflineQty";
            this.OfflineQty.HeaderText = "Keep offline qty (2)";
            this.OfflineQty.Name = "OfflineQty";
            this.OfflineQty.ReadOnly = true;
            // 
            // Quantity
            // 
            this.Quantity.DataPropertyName = "Quantity";
            this.Quantity.HeaderText = "Lazada Available qty (3) = (1) - (2)";
            this.Quantity.Name = "Quantity";
            this.Quantity.ReadOnly = true;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(347, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(162, 36);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cập nhật số tồn online";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.adminToolStripMenuItem,
            this.loginToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(776, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excludeUPCsToolStripMenuItem,
            this.cấuHìnhPhầnMềmToolStripMenuItem});
            this.settingsToolStripMenuItem.Enabled = false;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // excludeUPCsToolStripMenuItem
            // 
            this.excludeUPCsToolStripMenuItem.Name = "excludeUPCsToolStripMenuItem";
            this.excludeUPCsToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.excludeUPCsToolStripMenuItem.Text = "Danh sách hàng bán online";
            // 
            // cấuHìnhPhầnMềmToolStripMenuItem
            // 
            this.cấuHìnhPhầnMềmToolStripMenuItem.Name = "cấuHìnhPhầnMềmToolStripMenuItem";
            this.cấuHìnhPhầnMềmToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.cấuHìnhPhầnMềmToolStripMenuItem.Text = "Cấu hình phần mềm";
            this.cấuHìnhPhầnMềmToolStripMenuItem.Visible = false;
            // 
            // adminToolStripMenuItem
            // 
            this.adminToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createAuthorizationURLToolStripMenuItem,
            this.lazadaToolStripMenuItem,
            this.applicationConfigToolStripMenuItem});
            this.adminToolStripMenuItem.Enabled = false;
            this.adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            this.adminToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.adminToolStripMenuItem.Text = "Admin";
            // 
            // createAuthorizationURLToolStripMenuItem
            // 
            this.createAuthorizationURLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createAuthorizationURLToolStripMenuItem1});
            this.createAuthorizationURLToolStripMenuItem.Name = "createAuthorizationURLToolStripMenuItem";
            this.createAuthorizationURLToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.createAuthorizationURLToolStripMenuItem.Text = "Shopee";
            // 
            // createAuthorizationURLToolStripMenuItem1
            // 
            this.createAuthorizationURLToolStripMenuItem1.Name = "createAuthorizationURLToolStripMenuItem1";
            this.createAuthorizationURLToolStripMenuItem1.Size = new System.Drawing.Size(207, 22);
            this.createAuthorizationURLToolStripMenuItem1.Text = "Create Authorization URL";
            this.createAuthorizationURLToolStripMenuItem1.Click += new System.EventHandler(this.CreateAuthorizationURLToolStripMenuItem1_Click);
            // 
            // lazadaToolStripMenuItem
            // 
            this.lazadaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createAuthorizationURLToolStripMenuItem2});
            this.lazadaToolStripMenuItem.Name = "lazadaToolStripMenuItem";
            this.lazadaToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.lazadaToolStripMenuItem.Text = "Lazada";
            // 
            // createAuthorizationURLToolStripMenuItem2
            // 
            this.createAuthorizationURLToolStripMenuItem2.Name = "createAuthorizationURLToolStripMenuItem2";
            this.createAuthorizationURLToolStripMenuItem2.Size = new System.Drawing.Size(207, 22);
            this.createAuthorizationURLToolStripMenuItem2.Text = "Create Authorization URL";
            this.createAuthorizationURLToolStripMenuItem2.Click += new System.EventHandler(this.CreateAuthorizationURLToolStripMenuItem2_Click);
            // 
            // applicationConfigToolStripMenuItem
            // 
            this.applicationConfigToolStripMenuItem.Name = "applicationConfigToolStripMenuItem";
            this.applicationConfigToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.applicationConfigToolStripMenuItem.Text = "Application Config";
            this.applicationConfigToolStripMenuItem.Click += new System.EventHandler(this.ApplicationConfigToolStripMenuItem_Click);
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.LoginToolStripMenuItem_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(515, 69);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(162, 36);
            this.button2.TabIndex = 5;
            this.button2.Text = "Read data from online store(s)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "ACTIVE LAZADA SKU(s):";
            // 
            // lblTotalSKUs
            // 
            this.lblTotalSKUs.AutoSize = true;
            this.lblTotalSKUs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalSKUs.Location = new System.Drawing.Point(155, 12);
            this.lblTotalSKUs.Name = "lblTotalSKUs";
            this.lblTotalSKUs.Size = new System.Drawing.Size(14, 13);
            this.lblTotalSKUs.TabIndex = 9;
            this.lblTotalSKUs.Text = "_";
            // 
            // diagOpenFile
            // 
            this.diagOpenFile.Filter = "Excel Work Book | *.xlsx";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(15, 134);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(749, 452);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.dgLazada);
            this.tabPage1.Controls.Add(this.lblTotalSKUs);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(741, 426);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "LAZADA";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(623, 6);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(112, 30);
            this.button4.TabIndex = 10;
            this.button4.Text = "Excel";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.button5);
            this.tabPage2.Controls.Add(this.dgShopee);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(741, 426);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "SHOPEE";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(7, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(207, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Shopee Variation SKU = Prism UPC";
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(623, 6);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(112, 30);
            this.button5.TabIndex = 11;
            this.button5.Text = "Excel";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.Button5_Click);
            // 
            // dgShopee
            // 
            this.dgShopee.AllowUserToAddRows = false;
            this.dgShopee.AllowUserToDeleteRows = false;
            this.dgShopee.AllowUserToOrderColumns = true;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Gainsboro;
            this.dgShopee.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgShopee.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgShopee.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgShopee.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ShopeePrismUPC,
            this.ShopeePrismOnHand,
            this.ShopeeKeepOffline,
            this.ShopeeOnSale});
            this.dgShopee.Location = new System.Drawing.Point(6, 42);
            this.dgShopee.Name = "dgShopee";
            this.dgShopee.ReadOnly = true;
            this.dgShopee.Size = new System.Drawing.Size(729, 378);
            this.dgShopee.TabIndex = 1;
            this.dgShopee.ColumnSortModeChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.DgShopee_ColumnSortModeChanged);
            // 
            // ShopeePrismUPC
            // 
            this.ShopeePrismUPC.DataPropertyName = "PrismUPC";
            this.ShopeePrismUPC.HeaderText = "PrismUPC";
            this.ShopeePrismUPC.Name = "ShopeePrismUPC";
            this.ShopeePrismUPC.ReadOnly = true;
            // 
            // ShopeePrismOnHand
            // 
            this.ShopeePrismOnHand.DataPropertyName = "Prism_on_hand";
            this.ShopeePrismOnHand.HeaderText = "Prism on hand (1)";
            this.ShopeePrismOnHand.Name = "ShopeePrismOnHand";
            this.ShopeePrismOnHand.ReadOnly = true;
            // 
            // ShopeeKeepOffline
            // 
            this.ShopeeKeepOffline.DataPropertyName = "keep_offline";
            this.ShopeeKeepOffline.HeaderText = "Keep Offline (2)";
            this.ShopeeKeepOffline.Name = "ShopeeKeepOffline";
            this.ShopeeKeepOffline.ReadOnly = true;
            // 
            // ShopeeOnSale
            // 
            this.ShopeeOnSale.DataPropertyName = "stock";
            this.ShopeeOnSale.HeaderText = "Shopee Stock (3) = (1) - (2)";
            this.ShopeeOnSale.Name = "ShopeeOnSale";
            this.ShopeeOnSale.ReadOnly = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dgPrism);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(741, 426);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "PRISM";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgPrism
            // 
            this.dgPrism.AllowUserToAddRows = false;
            this.dgPrism.AllowUserToDeleteRows = false;
            this.dgPrism.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgPrism.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPrism.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PrismUPC,
            this.PrismOH});
            this.dgPrism.Location = new System.Drawing.Point(6, 3);
            this.dgPrism.Name = "dgPrism";
            this.dgPrism.ReadOnly = true;
            this.dgPrism.Size = new System.Drawing.Size(732, 420);
            this.dgPrism.TabIndex = 2;
            // 
            // PrismUPC
            // 
            this.PrismUPC.DataPropertyName = "UPC";
            this.PrismUPC.HeaderText = "UPC";
            this.PrismUPC.Name = "PrismUPC";
            this.PrismUPC.ReadOnly = true;
            // 
            // PrismOH
            // 
            this.PrismOH.DataPropertyName = "QtyOnHand";
            this.PrismOH.HeaderText = "Online Qty";
            this.PrismOH.Name = "PrismOH";
            this.PrismOH.ReadOnly = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(17, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "UPC";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(55, 108);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(245, 20);
            this.textBox1.TabIndex = 14;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Online Store";
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAppToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(127, 48);
            // 
            // showAppToolStripMenuItem
            // 
            this.showAppToolStripMenuItem.Name = "showAppToolStripMenuItem";
            this.showAppToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.showAppToolStripMenuItem.Text = "Show app";
            this.showAppToolStripMenuItem.Click += new System.EventHandler(this.ShowAppToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(16, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Lần cập nhật cuối";
            // 
            // lblLastSync
            // 
            this.lblLastSync.AutoSize = true;
            this.lblLastSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastSync.ForeColor = System.Drawing.Color.Red;
            this.lblLastSync.Location = new System.Drawing.Point(132, 92);
            this.lblLastSync.Name = "lblLastSync";
            this.lblLastSync.Size = new System.Drawing.Size(11, 13);
            this.lblLastSync.TabIndex = 13;
            this.lblLastSync.Text = ".";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(515, 27);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(162, 36);
            this.button3.TabIndex = 16;
            this.button3.Text = "Send mail";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(77, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Pickup store";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(77, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Online";
            // 
            // lblPickupStore
            // 
            this.lblPickupStore.AutoSize = true;
            this.lblPickupStore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPickupStore.ForeColor = System.Drawing.Color.Red;
            this.lblPickupStore.Location = new System.Drawing.Point(165, 34);
            this.lblPickupStore.Name = "lblPickupStore";
            this.lblPickupStore.Size = new System.Drawing.Size(11, 13);
            this.lblPickupStore.TabIndex = 19;
            this.lblPickupStore.Text = ".";
            // 
            // chkLazada
            // 
            this.chkLazada.AutoSize = true;
            this.chkLazada.Enabled = false;
            this.chkLazada.Location = new System.Drawing.Point(168, 54);
            this.chkLazada.Name = "chkLazada";
            this.chkLazada.Size = new System.Drawing.Size(61, 17);
            this.chkLazada.TabIndex = 20;
            this.chkLazada.Text = "Lazada";
            this.chkLazada.UseVisualStyleBackColor = true;
            // 
            // chkShopee
            // 
            this.chkShopee.AutoSize = true;
            this.chkShopee.Enabled = false;
            this.chkShopee.Location = new System.Drawing.Point(235, 54);
            this.chkShopee.Name = "chkShopee";
            this.chkShopee.Size = new System.Drawing.Size(63, 17);
            this.chkShopee.TabIndex = 21;
            this.chkShopee.Text = "Shopee";
            this.chkShopee.UseVisualStyleBackColor = true;
            // 
            // diagSaveFile
            // 
            this.diagSaveFile.DefaultExt = "xlsx";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 598);
            this.Controls.Add(this.chkShopee);
            this.Controls.Add(this.chkLazada);
            this.Controls.Add(this.lblPickupStore);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblLastSync);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cập nhật store online";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgLazada)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgShopee)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgPrism)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridView dgLazada;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem adminToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createAuthorizationURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createAuthorizationURLToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem lazadaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createAuthorizationURLToolStripMenuItem2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotalSKUs;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem excludeUPCsToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog diagOpenFile;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dgShopee;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dgPrism;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showAppToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cấuHìnhPhầnMềmToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblLastSync;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrismUPC;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrismOH;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn UPC;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrismOnHandQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn OfflineQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolStripMenuItem applicationConfigToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblPickupStore;
        private System.Windows.Forms.CheckBox chkLazada;
        private System.Windows.Forms.CheckBox chkShopee;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.SaveFileDialog diagSaveFile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShopeePrismUPC;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShopeePrismOnHand;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShopeeKeepOffline;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShopeeOnSale;
    }
}

