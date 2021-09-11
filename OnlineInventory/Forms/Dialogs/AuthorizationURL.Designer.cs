namespace OnlineInventory.Dialogs
{
    partial class AuthorizationURL
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLazadaCode = new System.Windows.Forms.TextBox();
            this.btnGetAccessToken = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 36);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(445, 20);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Authorization URL";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(294, 62);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(163, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Open link in browser";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Lazada Code";
            // 
            // txtLazadaCode
            // 
            this.txtLazadaCode.Location = new System.Drawing.Point(12, 119);
            this.txtLazadaCode.Name = "txtLazadaCode";
            this.txtLazadaCode.Size = new System.Drawing.Size(445, 20);
            this.txtLazadaCode.TabIndex = 4;
            // 
            // btnGetAccessToken
            // 
            this.btnGetAccessToken.Location = new System.Drawing.Point(294, 145);
            this.btnGetAccessToken.Name = "btnGetAccessToken";
            this.btnGetAccessToken.Size = new System.Drawing.Size(163, 23);
            this.btnGetAccessToken.TabIndex = 5;
            this.btnGetAccessToken.Text = "GetToken";
            this.btnGetAccessToken.UseVisualStyleBackColor = true;
            this.btnGetAccessToken.Click += new System.EventHandler(this.btnGetAccessToken_Click);
            // 
            // AuthorizationURL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 178);
            this.Controls.Add(this.btnGetAccessToken);
            this.Controls.Add(this.txtLazadaCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "AuthorizationURL";
            this.Text = "AuthorizationURL";
            this.Load += new System.EventHandler(this.AuthorizationURL_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLazadaCode;
        private System.Windows.Forms.Button btnGetAccessToken;
    }
}