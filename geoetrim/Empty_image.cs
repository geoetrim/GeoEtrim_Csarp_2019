using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageComboBox;

namespace GeoEtrim
{
    public partial class Empty_image : Form
    {
        public static bool active_form = false;
        public static int emp_Width=100, emp_Height=100;
        static public Color empty_page_clr = Color.Black;
        public Empty_image()
        {
            InitializeComponent();
        }
        private void Empty_image_Load(object sender, EventArgs e)
        {
            active_form = true;
            Form1 ff = new Form1();
            this.Icon = ff.Icon;
            string[] clr = Enum.GetNames(typeof(KnownColor));
            int i;
            for (i = 0; i < clr.Length; i++)
            {
                Bitmap img_icon = new Bitmap(16, 16);
                Graphics g = Graphics.FromImage(img_icon);
                g.Clear(Color.FromName(clr[i].ToString()));
                ımageList1.Images.Add(img_icon);
            }
            for (i = 0; i < clr.Length; i++)
            {
                ımageComboBox1.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
            }
            ımageComboBox1.SelectedIndex = ımageComboBox1.FindString(empty_page_clr.Name);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            text_read txt_read = new text_read(textBox2.Text);
            empty_page_clr =Color.FromName(ımageComboBox1.Items[ımageComboBox1.SelectedIndex].Text);
            txt_read.ShowDialog();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "") && (textBox2.Text != ""))
                button1.Enabled = true;
            else
                button1.Enabled = false;
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "") && (textBox2.Text != ""))
                button1.Enabled = true;
            else
                button1.Enabled = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            textBox2.Text = openFileDialog1.FileName;
        }
        private void Empty_image_FormClosing(object sender, FormClosingEventArgs e)
        {
            active_form = false;

       
        }
                
    }
}
