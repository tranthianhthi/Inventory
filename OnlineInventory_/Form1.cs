using System.Windows.Forms;

namespace OnlineInventory
{
    public partial class Form1 : Form
    {
        OnlineInventoryService service;


        public Form1()
        {
            InitializeComponent();

            service = new OnlineInventoryService();
        }

        

        
    }
}
