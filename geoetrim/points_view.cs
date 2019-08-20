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
    public partial class points_view : Form
    {
        public points_view()
        {
            InitializeComponent();
        }
        //
        // Fonctions
        //
        void gcp_TEST_Point(float line_length, float line_width, float text_size,
            string text_type, Color text_color, Color line_color, bool write_type, bool write_id)
        {
            float x, y;
            y = (ımageBox1.Height - 5) - (line_length / 2);
            x = 5 + line_length / 2;
            ımageBox1.Refresh();
            Graphics g = ımageBox1.CreateGraphics();
            g.DrawLine(new Pen(line_color, line_width), new PointF(x - (line_length / 2), y), new PointF(x + (line_length / 2), y));
            g.DrawLine(new Pen(line_color, line_width), new PointF(x, y - (line_length / 2)), new PointF(x, y + (line_length / 2)));
            Font ffont = new Font(text_type, text_size);
            string namepoint = "";
            if (write_type == true) namepoint = "GCP";
            if (write_id == true)
            {
                if (write_type == false)
                    namepoint = "427";
                if (write_type == true)
                    namepoint = namepoint + "-427";
            }
            if (namepoint == "-1") namepoint = "?";
            g.DrawString(namepoint, ffont, new SolidBrush(text_color), new PointF(x + (line_length / 2), y - (line_length - 3)));
        }
        void icp_TEST_Point(float line_length, float line_width, float text_size,
       string text_type, Color text_color, Color line_color, bool write_id, bool write_type)
        {
            float x, y;
            y = (ımageBox2.Height - 5) - (line_length / 2);
            x = 5 + line_length / 2;
            ımageBox2.Refresh();
            Graphics g = ımageBox2.CreateGraphics();
            g.DrawLine(new Pen(line_color, line_width), new PointF(x - (line_length / 2), y), new PointF(x + (line_length / 2), y));
            g.DrawLine(new Pen(line_color, line_width), new PointF(x, y - (line_length / 2)), new PointF(x, y + (line_length / 2)));
            Font ffont = new Font(text_type, text_size);
            string namepoint = "";
            if (write_type == true) namepoint = "ICP";
            if (write_id == true)
            {
                if (write_type == false)
                    namepoint = "427";
                if (write_type == true)
                    namepoint = namepoint + "-427";
            }
            if (namepoint == "-1") namepoint = "?";
            g.DrawString(namepoint, ffont, new SolidBrush(text_color), new PointF(x + (line_length / 2), y - (line_length - 3)));
        }
        private void appy_view()
        {
            // for GCP & ICP Collection
            ımageComboBox7.SelectedIndex = ımageComboBox7.FindString(formgcp.title_font_name);
            ımageComboBox8.SelectedIndex = ımageComboBox8.FindString(formgcp.datatable_back_color.Name);
            ımageComboBox9.SelectedIndex = ımageComboBox9.FindString(formgcp.line_select_color.Name);
            ımageComboBox10.SelectedIndex = ımageComboBox10.FindString(formgcp.values_font_name);
            ımageComboBox11.SelectedIndex = ımageComboBox11.FindString(formgcp.img_select_color.Name);

            ımageComboBox15.SelectedIndex = ımageComboBox15.FindString(formgcp.imgbox_backcolor.Name);
            numericUpDown7.Value = formgcp.title_font_size;
            numericUpDown8.Value = formgcp.values_font_size;
            numericUpDown10.Value = formgcp.decimal_points;

            //for main window  
            ımageComboBox13.SelectedIndex = ımageComboBox13.FindString(Form1.select_point_color.Name);
            ımageComboBox14.SelectedIndex = ımageComboBox14.FindString(Form1.mouse_icon_color.Name);

            //gcp for main window
            ımageComboBox1.SelectedIndex = ımageComboBox1.FindString(Form1.gcp_font_name);
            ımageComboBox2.SelectedIndex = ımageComboBox2.FindString(Form1.gcp_text_color.Name);
            ımageComboBox3.SelectedIndex = ımageComboBox3.FindString(Form1.gcp_line_color.Name);
            numericUpDown1.Value = (decimal)Form1.gcp_text_size;
            numericUpDown2.Value = (decimal)Form1.gcp_line_length;
            numericUpDown3.Value = (decimal)Form1.gcp_line_width;
            checkBox1.Checked = Form1.gcp_type_name;
            checkBox2.Checked = Form1.gcp_id_name;

            if ((ımageComboBox1.SelectedItem != null) && (ımageComboBox2.SelectedItem != null) &&
                (ımageComboBox3.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
                gcp_TEST_Point((float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown1.Value,
                    ımageComboBox1.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox1.Checked, checkBox2.Checked);
            }

            //icp for main window
            ımageComboBox4.SelectedIndex = ımageComboBox4.FindString(Form1.icp_font_name);
            ımageComboBox5.SelectedIndex = ımageComboBox5.FindString(Form1.icp_text_color.Name);
            ımageComboBox6.SelectedIndex = ımageComboBox6.FindString(Form1.icp_line_color.Name);
            numericUpDown4.Value = (decimal)Form1.icp_line_width;
            numericUpDown5.Value = (decimal)Form1.icp_line_length;
            numericUpDown6.Value = (decimal)Form1.icp_text_size;
            checkBox3.Checked = Form1.icp_id_name;
            checkBox4.Checked = Form1.icp_type_name;

            if ((ımageComboBox6.SelectedItem != null) && (ımageComboBox5.SelectedItem != null) &&
                (ımageComboBox4.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
                icp_TEST_Point((float)numericUpDown5.Value, (float)numericUpDown4.Value, (float)numericUpDown6.Value,
                    ımageComboBox4.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox4.Checked, checkBox3.Checked);
            }

            // view GeoTransform
            ımageComboBox12.SelectedIndex = ımageComboBox12.FindString(GeoTransform.vektor_back.Name);
            ımageComboBox16.SelectedIndex = ımageComboBox16.FindString(GeoTransform.vektor_canves.Name);
            ımageComboBox17.SelectedIndex = ımageComboBox17.FindString(GeoTransform.wind_back.Name);
            ımageComboBox18.SelectedIndex = ımageComboBox18.FindString(GeoTransform.vektor_color.Name);
            ımageComboBox19.SelectedIndex = ımageComboBox19.FindString(GeoTransform.wind_canves.Name);
            ımageComboBox20.SelectedIndex = ımageComboBox20.FindString(GeoTransform.wind_color.Name);
            ımageComboBox23.SelectedIndex = ımageComboBox23.FindString(GeoTransform.date_back.Name);

            ımageComboBox21.SelectedIndex = ımageComboBox21.FindString(GeoTransform.title_font_name);
            ımageComboBox24.SelectedIndex = ımageComboBox24.FindString(GeoTransform.values_font_name);

            numericUpDown11.Value = GeoTransform.title_font_size;
            numericUpDown9.Value = GeoTransform.values_font_size;
            numericUpDown12.Value = GeoTransform.decimal_points;
        }
        //
        // Events
        //
        private void points_view_Load(object sender, EventArgs e)
        {
            // for here
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
                ımageComboBox2.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox3.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox5.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox6.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox8.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox9.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox11.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox13.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox14.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox15.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox12.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox16.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox17.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox18.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox19.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox20.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
                ımageComboBox23.Items.Add(new ImageComboBoxItem(i, clr[i], new Font("Consolas", 10), i));
            }

            List<string> fonts = new List<string>();
            foreach (FontFamily fnt in FontFamily.Families)
                fonts.Add(fnt.Name);
            for (i = 0; i < fonts.Count; i++)
            {
                ımageComboBox1.Items.Add(new ImageComboBoxItem(fonts[i]));
                ımageComboBox4.Items.Add(new ImageComboBoxItem(fonts[i]));
                ımageComboBox7.Items.Add(new ImageComboBoxItem(fonts[i]));
                ımageComboBox10.Items.Add(new ImageComboBoxItem(fonts[i]));
                ımageComboBox21.Items.Add(new ImageComboBoxItem(fonts[i]));
                ımageComboBox24.Items.Add(new ImageComboBoxItem(fonts[i]));

                ımageComboBox1.Items[i].Font = new Font(fonts[i], 10);
                ımageComboBox4.Items[i].Font = new Font(fonts[i], 10);
                ımageComboBox7.Items[i].Font = new Font(fonts[i], 10);
                ımageComboBox10.Items[i].Font = new Font(fonts[i], 10);
                ımageComboBox21.Items[i].Font = new Font(fonts[i], 10);
                ımageComboBox24.Items[i].Font = new Font(fonts[i], 10);
            }

            appy_view(); // for select
        }
        private void points_view_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Form1.Image_ID > -1)
            {
                Form1 f_main = Application.OpenForms["Form1"] as Form1;
                f_main.GCP_draw(Form1.gcp_collected[Form1.Image_ID]);
                f_main.GCP_draw(Form1.gcp_collected[Form1.Image_ID]);
            }

            if (formgcp.Form_active == true)
            {
                formgcp f_gcp = Application.OpenForms["formgcp"] as formgcp;
                f_gcp.dzy_main();
            }
            if (GeoTransform.form_active == true)
            {
                GeoTransform geo = Application.OpenForms["GeoTransform"] as GeoTransform;
                geo.image_dzyn();
            }
        }
        private void tabPage1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((ımageComboBox1.SelectedItem != null) && (ımageComboBox2.SelectedItem != null) &&
                (ımageComboBox3.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
                gcp_TEST_Point((float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown1.Value,
                    ımageComboBox1.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox1.Checked, checkBox2.Checked);
            }
            if ((ımageComboBox6.SelectedItem != null) && (ımageComboBox5.SelectedItem != null) &&
              (ımageComboBox4.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
                icp_TEST_Point((float)numericUpDown5.Value, (float)numericUpDown4.Value, (float)numericUpDown6.Value,
                    ımageComboBox4.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox4.Checked, checkBox3.Checked);
            }
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox1.SelectedItem != null) && (ımageComboBox2.SelectedItem != null) &&
                (ımageComboBox3.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
                gcp_TEST_Point((float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown1.Value,
                    ımageComboBox1.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox1.Checked, checkBox2.Checked);
            }
            if ((ımageComboBox6.SelectedItem != null) && (ımageComboBox5.SelectedItem != null) &&
                 (ımageComboBox4.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
                icp_TEST_Point((float)numericUpDown5.Value, (float)numericUpDown4.Value, (float)numericUpDown6.Value,
                    ımageComboBox4.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox4.Checked, checkBox3.Checked);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //GCP & ICP Collection window
            formgcp.datatable_back_color = Color.FromName(ımageComboBox8.SelectedItem.ToString());
            formgcp.line_select_color = Color.FromName(ımageComboBox9.SelectedItem.ToString());
            formgcp.img_select_color = Color.FromName(ımageComboBox11.SelectedItem.ToString());

            formgcp.imgbox_backcolor = Color.FromName(ımageComboBox15.SelectedItem.ToString());
            formgcp.title_font_name = ımageComboBox7.SelectedItem.ToString();
            formgcp.values_font_name = ımageComboBox10.SelectedItem.ToString();
            formgcp.title_font_size = (int)numericUpDown7.Value;
            formgcp.values_font_size = (int)numericUpDown8.Value;
            formgcp.decimal_points = (int)numericUpDown10.Value;

            //main windom
            Form1.select_point_color = Color.FromName(ımageComboBox13.SelectedItem.ToString());
            Form1.mouse_icon_color = Color.FromName(ımageComboBox14.SelectedItem.ToString());

            //gcp in main windom
            Form1.gcp_font_name = ımageComboBox1.SelectedItem.ToString();
            Form1.gcp_text_size = (float)numericUpDown1.Value;
            Color clr;
            clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
            Form1.gcp_text_color = clr;
            Form1.gcp_line_length = (float)numericUpDown2.Value;
            Form1.gcp_line_width = (float)numericUpDown3.Value;
            clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
            Form1.gcp_line_color = clr;
            Form1.gcp_type_name = checkBox1.Checked;
            Form1.gcp_id_name = checkBox2.Checked;

            //icp in main windom
            Form1.icp_font_name = ımageComboBox4.SelectedItem.ToString();
            Form1.icp_text_size = (float)numericUpDown6.Value;
            clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
            Form1.icp_text_color = clr;
            Form1.icp_line_length = (float)numericUpDown5.Value;
            Form1.icp_line_width = (float)numericUpDown4.Value;
            clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
            Form1.icp_line_color = clr;
            Form1.icp_type_name = checkBox4.Checked;
            Form1.icp_id_name = checkBox3.Checked;

            // view GeoTransform
            GeoTransform.vektor_back = Color.FromName(ımageComboBox12.SelectedItem.ToString());
            GeoTransform.vektor_canves = Color.FromName(ımageComboBox16.SelectedItem.ToString());
            GeoTransform.wind_back = Color.FromName(ımageComboBox17.SelectedItem.ToString());
            GeoTransform.vektor_color = Color.FromName(ımageComboBox18.SelectedItem.ToString());
            GeoTransform.wind_canves = Color.FromName(ımageComboBox19.SelectedItem.ToString());
            GeoTransform.wind_color = Color.FromName(ımageComboBox20.SelectedItem.ToString());
            GeoTransform.date_back = Color.FromName(ımageComboBox23.SelectedItem.ToString());

            GeoTransform.title_font_name = ımageComboBox21.SelectedItem.ToString();
            GeoTransform.values_font_name = ımageComboBox24.SelectedItem.ToString();

            GeoTransform.title_font_size = (int)numericUpDown11.Value;
            GeoTransform.values_font_size = (int)numericUpDown9.Value;
            GeoTransform.decimal_points = (int)numericUpDown12.Value;

            //////
            Form1 f1 = new Form1();
            f1.view_save();
            formgcp fgcp = new formgcp();
            fgcp.view_save();
            GeoTransform geo = new GeoTransform();
            geo.view_save();
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.view_main_defult();
            formgcp fgcp = new formgcp();
            fgcp.view_gcp_defult();
            GeoTransform geo = new GeoTransform();
            geo.view_defualt();

            appy_view();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //
        // Gcp view
        //
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox1.SelectedItem != null) && (ımageComboBox2.SelectedItem != null) &&
                (ımageComboBox3.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
                gcp_TEST_Point((float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown1.Value,
                    ımageComboBox1.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox1.Checked, checkBox2.Checked);
            }
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox1.SelectedItem != null) && (ımageComboBox2.SelectedItem != null) &&
                (ımageComboBox3.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
                gcp_TEST_Point((float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown1.Value,
                    ımageComboBox1.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox1.Checked, checkBox2.Checked);
            }
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox1.SelectedItem != null) && (ımageComboBox2.SelectedItem != null) &&
                (ımageComboBox3.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
                gcp_TEST_Point((float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown1.Value,
                    ımageComboBox1.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox1.Checked, checkBox2.Checked);
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox1.SelectedItem != null) && (ımageComboBox2.SelectedItem != null) &&
                (ımageComboBox3.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
                gcp_TEST_Point((float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown1.Value,
                    ımageComboBox1.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox1.Checked, checkBox2.Checked);
            }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox1.SelectedItem != null) && (ımageComboBox2.SelectedItem != null) &&
                (ımageComboBox3.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
                gcp_TEST_Point((float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown1.Value,
                    ımageComboBox1.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox1.Checked, checkBox2.Checked);
            }
        }
        private void ımageComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox1.SelectedItem != null) && (ımageComboBox2.SelectedItem != null) &&
                     (ımageComboBox3.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
                gcp_TEST_Point((float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown1.Value,
                    ımageComboBox1.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox1.Checked, checkBox2.Checked);
            }
        }
        private void ımageComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox1.SelectedItem != null) && (ımageComboBox2.SelectedItem != null) &&
      (ımageComboBox3.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
                gcp_TEST_Point((float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown1.Value,
                    ımageComboBox1.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox1.Checked, checkBox2.Checked);
            }
        }
        private void ımageComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox1.SelectedItem != null) && (ımageComboBox2.SelectedItem != null) &&
               (ımageComboBox3.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox2.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox3.SelectedItem.ToString());
                gcp_TEST_Point((float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown1.Value,
                    ımageComboBox1.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox1.Checked, checkBox2.Checked);
            }
        }
        //
        // Icp view
        //
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox6.SelectedItem != null) && (ımageComboBox5.SelectedItem != null) &&
             (ımageComboBox4.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
                icp_TEST_Point((float)numericUpDown5.Value, (float)numericUpDown4.Value, (float)numericUpDown6.Value,
                    ımageComboBox4.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox4.Checked, checkBox3.Checked);
            }
        }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox6.SelectedItem != null) && (ımageComboBox5.SelectedItem != null) &&
             (ımageComboBox4.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
                icp_TEST_Point((float)numericUpDown5.Value, (float)numericUpDown4.Value, (float)numericUpDown6.Value,
                    ımageComboBox4.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox4.Checked, checkBox3.Checked);
            }
        }
        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox6.SelectedItem != null) && (ımageComboBox5.SelectedItem != null) &&
             (ımageComboBox4.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
                icp_TEST_Point((float)numericUpDown5.Value, (float)numericUpDown4.Value, (float)numericUpDown6.Value,
                    ımageComboBox4.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox4.Checked, checkBox3.Checked);
            }


        }
        private void ımageComboBox4_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if ((ımageComboBox6.SelectedItem != null) && (ımageComboBox5.SelectedItem != null) &&
              (ımageComboBox4.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
                icp_TEST_Point((float)numericUpDown5.Value, (float)numericUpDown4.Value, (float)numericUpDown6.Value,
                    ımageComboBox4.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox4.Checked, checkBox3.Checked);
            }
        }
        private void ımageComboBox5_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if ((ımageComboBox6.SelectedItem != null) && (ımageComboBox5.SelectedItem != null) &&
   (ımageComboBox4.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
                icp_TEST_Point((float)numericUpDown5.Value, (float)numericUpDown4.Value, (float)numericUpDown6.Value,
                    ımageComboBox4.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox4.Checked, checkBox3.Checked);
            }
        }
        private void ımageComboBox6_RightToLeftChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox6.SelectedItem != null) && (ımageComboBox5.SelectedItem != null) &&
         (ımageComboBox4.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
                icp_TEST_Point((float)numericUpDown5.Value, (float)numericUpDown4.Value, (float)numericUpDown6.Value,
                    ımageComboBox4.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox4.Checked, checkBox3.Checked);
            }
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox6.SelectedItem != null) && (ımageComboBox5.SelectedItem != null) &&
            (ımageComboBox4.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
                icp_TEST_Point((float)numericUpDown5.Value, (float)numericUpDown4.Value, (float)numericUpDown6.Value,
                    ımageComboBox4.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox4.Checked, checkBox3.Checked);
            }
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if ((ımageComboBox6.SelectedItem != null) && (ımageComboBox5.SelectedItem != null) &&
            (ımageComboBox4.SelectedItem != null))
            {
                Color text_clr, line_clr;
                text_clr = Color.FromName(ımageComboBox5.SelectedItem.ToString());
                line_clr = Color.FromName(ımageComboBox6.SelectedItem.ToString());
                icp_TEST_Point((float)numericUpDown5.Value, (float)numericUpDown4.Value, (float)numericUpDown6.Value,
                    ımageComboBox4.SelectedItem.ToString(), text_clr, line_clr,
                    checkBox4.Checked, checkBox3.Checked);
            }
        }
    }
}
