using System;
using System.Windows.Forms;

namespace OnlineInventory.Dialogs
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter )
            {
                this.DialogResult = (textBox1.Text == "sysadmin!@#")? DialogResult.OK : DialogResult.Cancel;
                this.Close();
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
