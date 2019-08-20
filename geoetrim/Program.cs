using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoEtrim
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            /*
            Application.Run(new open());
            if (open.app_key == true)
                Application.Run(new Form1());
            else
            {
                Application.Run(new Register());
                if (open.app_key == true)
                    Application.Run(new Form1());
            }
            */
        }
    }
}
