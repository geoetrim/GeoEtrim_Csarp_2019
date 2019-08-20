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
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }
        void button_control()
        {
            if ( (textBox1.Text != "") && (textBox2.Text != "") && 
                (textBox3.Text != "") && (textBox4.Text != ""))
                button1.Enabled = true;
            else
                button1.Enabled = false;
        }
        private void Register_Load(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\GeoEtrim", true);
            string key = rk.GetValue("Key").ToString();
            if (key == "9868-7367-3355-2718")
            {
                if (rk.GetValue("Name") == null)
                    rk.SetValue("Name", "?", RegistryValueKind.String);
                if (rk.GetValue("Surname") == null)
                    rk.SetValue("Surname", "?", RegistryValueKind.String);
                if (rk.GetValue("Mail") == null)
                    rk.SetValue("Mail", "?", RegistryValueKind.String);
                button1.Text = "Passive";
                textBox1.Text = rk.GetValue("Name").ToString();
                textBox2.Text = rk.GetValue("Surname").ToString();
                textBox3.Text = rk.GetValue("Mail").ToString();
                textBox4.Text = "****-****-****-****";
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                label1.ForeColor = Color.Blue;
                label1.Text = "Successfully activated";
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                button1.Text = "Activate";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                label1.ForeColor = Color.Red;
                string days_not = "??";
                int day = DateTime.Now.DayOfYear;
                int year = DateTime.Now.Year - Convert.ToInt32(rk.GetValue("Year").ToString());
                if (year == 1) day = day + 365;
                else if (year != 0)
                {
                    days_not = "??";
                }
                day = day - Convert.ToInt32(rk.GetValue("DayOfYear").ToString());
                if (day > 30)
                {
                    days_not = "0";
                }
                else
                    days_not = (30 - day).ToString();
                label1.Text = "Last " + days_not + " days";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\GeoEtrim", true);
            string key = textBox4.Text;
            if (key == "9868-7367-3355-2718")
            {
                rk.SetValue("Name", textBox1.Text, RegistryValueKind.String);
                rk.SetValue("Surname", textBox2.Text, RegistryValueKind.String);
                rk.SetValue("Mail", textBox3.Text, RegistryValueKind.String);
                rk.SetValue("Key", textBox4.Text, RegistryValueKind.String);
                textBox4.Text = "****-****-****-****";
                button1.Text = "Passive";
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                label1.ForeColor = Color.Blue;
                label1.Text = "Successfully activated";
                open.app_key = true;
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                button1.Text = "Activate";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                label1.ForeColor = Color.Red;
                string days_not = "??";
                int day = DateTime.Now.DayOfYear;
                int year = DateTime.Now.Year - Convert.ToInt32(rk.GetValue("Year").ToString());
                if (year == 1) day = day + 365;
                else if (year != 0)
                {
                    days_not = "??";
                }
                day = day - Convert.ToInt32(rk.GetValue("DayOfYear").ToString());
                if (day > 30)
                {
                    days_not = "0";
                }
                else
                    days_not = (30-day).ToString();
                label1.Text = "Last " + days_not + " days";
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button_control();
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            button_control();
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            button_control();
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            button_control();
        }
    }
}
