using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RPAutoSync
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] agurs)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new RPAutoSync());
            if (agurs.Length != 0)                
                Application.Run(new RPAutoSync(agurs[0].Trim()));
            else
            {
                Application.Run(new RPAutoSync());                
            }
        }
    }
}
