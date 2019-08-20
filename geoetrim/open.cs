using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;


namespace GeoEtrim
{
    public partial class open : Form
    {
        static int opn = 0;
        static public bool System_change = false;
        static public bool app_key = true;
        public open()
        {
            InitializeComponent(); 
        }
        private void open_Load(object sender, EventArgs e)
        {
            pictureBox1.Location = new Point(0, 0);
            this.Width = pictureBox1.Size.Width;

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\GeoEtrim", true);
            if (rk == null)
            {
                Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("GeoEtrim",RegistryKeyPermissionCheck.ReadWriteSubTree);     
                rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\GeoEtrim", true);
            }
            if(rk.GetValue("Key") == null)
                rk.SetValue("Key", "-1", RegistryValueKind.String);
            if(rk.GetValue("DayOfYear")==null)
                rk.SetValue("DayOfYear", DateTime.Now.DayOfYear.ToString(), RegistryValueKind.String);
            if (rk.GetValue("Year") == null)
                rk.SetValue("Year", DateTime.Now.Year.ToString(), RegistryValueKind.String);

            rk.Close();
            timer1.Enabled = true;
        } 
        private void timer1_Tick(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment",true);
            string app_path = Application.StartupPath.ToString();
            string lib = app_path+"\\lib";
            if (rk.GetValue("Path") == null)
                rk.SetValue("Path", "",RegistryValueKind.String);
            string sys_path = rk.GetValue("Path").ToString();
            if (sys_path.ToCharArray()[sys_path.Length - 1] != ';')
                sys_path = sys_path + ';';
            int cheak = 0;
            if (sys_path.IndexOf(lib) == -1)
            {
                sys_path = sys_path + lib;
                cheak = 1;
            }
            if (cheak == 1)
            {
                rk.SetValue("Path", sys_path, RegistryValueKind.String);
                System_change = true;
            }
            rk.Close();
     
            rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\GeoEtrim", true);
            string key = rk.GetValue("Key").ToString();
            if(key!="9868-7367-3355-2718")
            {
                int year = DateTime.Now.Year - Convert.ToInt32(rk.GetValue("Year").ToString());
                int day = DateTime.Now.DayOfYear;
                if (year == 1) day = day + 365;
                else if (year != 0) app_key = false;
                day = day - Convert.ToInt32(rk.GetValue("DayOfYear").ToString());
                if (day>30) app_key = false;
            }
            rk.Close();
            
            opn = opn + 1;
            if (opn == 1) this.Close();
        }
    }
}
