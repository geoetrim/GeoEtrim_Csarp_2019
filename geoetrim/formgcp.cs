using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace GeoEtrim
{
    public partial class formgcp : Form
    {
        //
        // Variables
        //
        public static bool Form_active = false;
        static int editMod_close = 0;// for edit active
        static int[] select_index = new int[0];// this is select row's index
        static string[] select_tybe = new string[0];
        static int select_id_row;

        //  view about veriables
        public static  Color datatable_back_color , line_select_color , imgbox_backcolor , img_select_color;
        public static  string title_font_name , values_font_name;
        public static  int title_font_size , values_font_size , decimal_points;
        //
        // Functions
        //
        public void view_gcp_defult()
        {
            datatable_back_color = Color.FromKnownColor(KnownColor.AppWorkspace);
            line_select_color = Color.Red;
            imgbox_backcolor = Color.FromKnownColor(KnownColor.ControlLight);
            img_select_color = Color.Yellow;
            title_font_name = "Consolas";
            values_font_name = "Consolas";
            title_font_size = 9;
            values_font_size = 8;
            decimal_points = 3;
        }
        public void view_here()
        {
            int i;
            if (File.Exists(Application.StartupPath + "/gcp_view.txt") == true)
            {
                try
                {
                    StreamReader view_m = new StreamReader(Application.StartupPath + "/gcp_view.txt");
                    string[] line = new string[9];
                    for (i = 0; i < 9; i++) line[i] = view_m.ReadLine();
                    view_m.Close();
                    datatable_back_color = Color.FromName(line[0]);
                    line_select_color = Color.FromName(line[1]);
                    imgbox_backcolor = Color.FromName(line[2]);
                    img_select_color = Color.FromName(line[3]);
                    title_font_name = line[4];
                    values_font_name = line[5];
                    title_font_size = Convert.ToInt32(line[6]);
                    values_font_size = Convert.ToInt32(line[7]);
                    decimal_points = Convert.ToInt32(line[8]);
                }
                catch
                {
                    view_gcp_defult();
                }
            }
            else view_gcp_defult();
        } //Ana pencerenin açıldığında ayarlarıı yapması içindir.
        public void view_save()
        {
            StreamWriter wr = File.CreateText(Application.StartupPath + "/gcp_view.txt");
            wr.WriteLine(datatable_back_color.Name);
            wr.WriteLine(line_select_color.Name);
            wr.WriteLine(imgbox_backcolor.Name);
            wr.WriteLine(img_select_color.Name);
            wr.WriteLine(title_font_name);
            wr.WriteLine(values_font_name);
            wr.WriteLine(title_font_size);
            wr.WriteLine(values_font_size);
            wr.WriteLine(decimal_points);
            wr.Close();

        }
        void button_control()// Hangi buttonun ne zaman aktif olacağı ve adının ne olacağını belli etmek içindir[tr].
        {
            if ((editMod_close == 0) && (textBox1.Text != "") && (textBox2.Text != "") && (textBox4.Text != "") && (textBox6.Text != "") &&
             (textBox8.Text != "") && (textBox10.Text != ""))
                button6.Enabled = true;
            else
                button6.Enabled = false;

            if ((editMod_close == 1) && (textBox1.Text != "") && (textBox2.Text != "") && (textBox4.Text != "") && (textBox6.Text != "") &&
           (textBox8.Text != "") && (textBox10.Text != ""))
                button4.Enabled = true;
            else
                button4.Enabled = false;

            if (editMod_close == 0)
            {
                button4.Text = "Edit";
                button4.Enabled = true;
                dataGridView1.Enabled = true;
                button5.Text = "Delete";
            }
            if (editMod_close == 1)
            {
                button4.Text = "OK";
                button5.Text = "Cancel";
                button6.Enabled = false;
                dataGridView1.Enabled = false;
            }
        }
        void clear_page()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox4.Text = "";
            textBox6.Text = "";
            textBox8.Text = "";
            textBox10.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
            textBox7.Text = "";
            textBox9.Text = "";
            textBox11.Text = "";
        }
        Image<Rgb, byte> img_edit_gcp(Image<Rgb, byte> img, int[] edit_row)// for before point editing and deleting
        {
            int i, k = 0;
            float r, c;
            for (i = 0; i < Form1.gcp_collected[Form1.Image_ID].Rows.Count; i++)
            {
                c = Convert.ToSingle(Form1.gcp_collected[Form1.Image_ID].Rows[i][3].ToString()) / (Form1.down_scale - 1);
                r = Convert.ToSingle(Form1.gcp_collected[Form1.Image_ID].Rows[i][2].ToString()) / (Form1.down_scale - 1);
                if (edit_row.Length > 0)
                {
                    if ((edit_row[k] == i))
                    {
                        CvInvoke.Circle(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 12, new Rgb(img_select_color).MCvScalar, 2);
                        CvInvoke.Circle(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 2, new Rgb(Form1.gcp_line_color).MCvScalar, 5);
                        if (edit_row.Length - 1 > k) k = k + 1;
                    }
                    else
                        CvInvoke.Circle(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 2, new Rgb(Form1.gcp_line_color).MCvScalar, 2);

                }
                else
                    CvInvoke.Circle(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 2, new Rgb(Form1.gcp_line_color).MCvScalar, 2);
            }
            return img;
        }
        Image<Rgb, byte> img_edit_icp(Image<Rgb, byte> img, int[] edit_row)// for before point editing and deleting
        {
            int i, k = 0;
            float r, c;
            for (i = 0; i < Form1.icp_collected[Form1.Image_ID].Rows.Count; i++)
            {
                c = Convert.ToSingle(Form1.icp_collected[Form1.Image_ID].Rows[i][3].ToString()) / (Form1.down_scale - 1);
                r = Convert.ToSingle(Form1.icp_collected[Form1.Image_ID].Rows[i][2].ToString()) / (Form1.down_scale - 1);
                if (edit_row.Length > 0)
                {
                    if ((edit_row[k] == i))
                    {
                        CvInvoke.Circle(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 12, new Rgb(img_select_color).MCvScalar, 2);
                        CvInvoke.Circle(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 2, new Rgb(Form1.gcp_line_color).MCvScalar, 5);
                        if (edit_row.Length - 1 > k) k = k + 1;
                    }
                    else
                        CvInvoke.Circle(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 2, new Rgb(Form1.icp_line_color).MCvScalar, 2);
                }
                else
                    CvInvoke.Circle(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 2, new Rgb(Form1.icp_line_color).MCvScalar, 2);
            }
            return img;
        }
        public void img_dzy()// her hangi bir işlem bittikten sonra noktaların dağılımının genel görünümü içindir[tr].
        {
            Bitmap bmp = new Bitmap(Form1.Image_mini);
            Image<Rgb, byte> img = new Image<Rgb, byte>(bmp);
            int i;
            float r, c;
            for (i = 0; i < Form1.gcp_collected[Form1.Image_ID].Rows.Count; i++)
            {
                c = Convert.ToSingle(Form1.gcp_collected[Form1.Image_ID].Rows[i][3].ToString()) / (Form1.down_scale - 1);
                r = Convert.ToSingle(Form1.gcp_collected[Form1.Image_ID].Rows[i][2].ToString()) / (Form1.down_scale - 1);
                CvInvoke.Circle(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 2, new Rgb(Form1.gcp_line_color).MCvScalar, 2);
            }
            for (i = 0; i < Form1.icp_collected[Form1.Image_ID].Rows.Count; i++)
            {
                c = Convert.ToSingle(Form1.icp_collected[Form1.Image_ID].Rows[i][3].ToString()) / (Form1.down_scale - 1);
                r = Convert.ToSingle(Form1.icp_collected[Form1.Image_ID].Rows[i][2].ToString()) / (Form1.down_scale - 1);
                CvInvoke.Circle(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 2, new Rgb(Form1.icp_line_color).MCvScalar, 2);
            }
            pictureBox1.Image = img.Bitmap;
            pictureBox1.Refresh();
        }
        void dzy_form(int id)// for new point and after edit point
        {
            if (Form1.gcp_collected[Form1.Image_ID].Rows.Count != Form1.gcp_count)
                Form1.gcp_collected[Form1.Image_ID].Rows[Form1.gcp_count].Delete();
            if (Form1.icp_collected[Form1.Image_ID].Rows.Count != Form1.icp_count)
                Form1.icp_collected[Form1.Image_ID].Rows[Form1.icp_count].Delete();
            label23.Text = Form1.gcp_collected[Form1.Image_ID].Rows.Count.ToString();
            label25.Text = Form1.icp_collected[Form1.Image_ID].Rows.Count.ToString();

            dataGridView1.Rows[id].Cells[0].Value = textBox1.Text;
            dataGridView1.Rows[id].Cells[1].Value = comboBox1.Text;
            dataGridView1.Rows[id].Cells[2].Value = Convert.ToDouble(textBox2.Text).ToString("f" + decimal_points.ToString());
            dataGridView1.Rows[id].Cells[3].Value = Convert.ToDouble(textBox4.Text).ToString("f" + decimal_points.ToString());
            dataGridView1.Rows[id].Cells[4].Value = Convert.ToDouble(textBox6.Text).ToString("f" + decimal_points.ToString());
            dataGridView1.Rows[id].Cells[5].Value = Convert.ToDouble(textBox8.Text).ToString("f" + decimal_points.ToString());
            dataGridView1.Rows[id].Cells[6].Value = Convert.ToDouble(textBox10.Text).ToString("f" + decimal_points.ToString());
            int[] edit_id = new int[1];
            edit_id[0] = -2;
            img_dzy();
            clear_page();
            button_control();

            Form1 form = Application.OpenForms["Form1"] as Form1;
            form.GCP_draw(Form1.gcp_collected[Form1.Image_ID]);
            form.ICP_draw(Form1.icp_collected[Form1.Image_ID]);
            form.ımageBox1.Refresh();
   
        }
        public void dzy_main()// for first open, change image and export points file
        {
            dataGridView1.BackgroundColor = datatable_back_color;
            pictureBox1.BackColor = imgbox_backcolor;       
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(title_font_name, title_font_size);
            dataGridView1.DefaultCellStyle.Font = new Font(values_font_name, values_font_size);
            dataGridView1.DefaultCellStyle.Format = "f" + decimal_points.ToString();

            int i;
            for (i = 1; i < dataGridView1.ColumnCount; i++)
                dataGridView1.Columns[i].Width = dataGridView1.Size.Width / 7;
            dataGridView1.Columns[0].Width = (dataGridView1.Columns[1].Width / 7) * 6;         
            dataGridView1.Rows.Clear();

            if (Form1.gcp_collected[Form1.Image_ID].Rows.Count != Form1.gcp_count)
                Form1.gcp_collected[Form1.Image_ID].Rows[Form1.gcp_count].Delete();
            if (Form1.icp_collected[Form1.Image_ID].Rows.Count != Form1.icp_count)
                Form1.icp_collected[Form1.Image_ID].Rows[Form1.icp_count].Delete();

            label23.Text = Form1.gcp_collected[Form1.Image_ID].Rows.Count.ToString();
            label25.Text = Form1.icp_collected[Form1.Image_ID].Rows.Count.ToString();
            for (i = 0; i < Form1.gcp_collected[Form1.Image_ID].Rows.Count; i++)
                dataGridView1.Rows.Add(Form1.gcp_collected[Form1.Image_ID].Rows[i].ItemArray);
            for (i = 0; i < Form1.icp_collected[Form1.Image_ID].Rows.Count; i++)
                dataGridView1.Rows.Add(Form1.icp_collected[Form1.Image_ID].Rows[i].ItemArray);
    
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending); // point id'ye göre sırala[tr]. 
      
            editMod_close = 0;
            int[] edit_id = new int[1];
            edit_id[0] = -2;
            img_dzy();
            clear_page();
            button_control();

            // main window
            Form1 form = Application.OpenForms["Form1"] as Form1;
            form.GCP_draw(Form1.gcp_collected[Form1.Image_ID]);
            form.ICP_draw(Form1.icp_collected[Form1.Image_ID]);
            form.ımageBox1.Refresh();

        }
        void point_volue(int id, string gcp_or_icp)
        {
            if (gcp_or_icp == "gcp")
            {
                Form1.gcp_collected[Form1.Image_ID].Rows[id][0] = Convert.ToInt32(textBox1.Text);
                Form1.gcp_collected[Form1.Image_ID].Rows[id][1] = comboBox1.Text;
                Form1.gcp_collected[Form1.Image_ID].Rows[id][2] = Convert.ToSingle(textBox2.Text);
                Form1.gcp_collected[Form1.Image_ID].Rows[id][3] = Convert.ToSingle(textBox4.Text);
                Form1.gcp_collected[Form1.Image_ID].Rows[id][4] = Convert.ToDouble(textBox6.Text);
                Form1.gcp_collected[Form1.Image_ID].Rows[id][5] = Convert.ToDouble(textBox8.Text);
                Form1.gcp_collected[Form1.Image_ID].Rows[id][6] = Convert.ToDouble(textBox10.Text);

                double dvalue = 0;
                double.TryParse(textBox3.Text, out dvalue);
                Form1.gcp_collected[Form1.Image_ID].Rows[id][7] = dvalue;
                double.TryParse(textBox5.Text, out dvalue);
                Form1.gcp_collected[Form1.Image_ID].Rows[id][8] = dvalue;
                double.TryParse(textBox7.Text, out dvalue);
                Form1.gcp_collected[Form1.Image_ID].Rows[id][9] = dvalue;
                double.TryParse(textBox9.Text, out dvalue);
                Form1.gcp_collected[Form1.Image_ID].Rows[id][10] = dvalue;
                double.TryParse(textBox11.Text, out dvalue);
                Form1.gcp_collected[Form1.Image_ID].Rows[id][11] = dvalue;

                Form1.Save_Control = 1;
            }
            if (gcp_or_icp == "icp")
            {
                Form1.icp_collected[Form1.Image_ID].Rows[id][0] = Convert.ToInt32(textBox1.Text);
                Form1.icp_collected[Form1.Image_ID].Rows[id][1] = comboBox1.Text;
                Form1.icp_collected[Form1.Image_ID].Rows[id][2] = Convert.ToSingle(textBox2.Text);
                Form1.icp_collected[Form1.Image_ID].Rows[id][3] = Convert.ToSingle(textBox4.Text);
                Form1.icp_collected[Form1.Image_ID].Rows[id][4] = Convert.ToDouble(textBox6.Text);
                Form1.icp_collected[Form1.Image_ID].Rows[id][5] = Convert.ToDouble(textBox8.Text);
                Form1.icp_collected[Form1.Image_ID].Rows[id][6] = Convert.ToDouble(textBox10.Text);

                double dvalue = 0;
                double.TryParse(textBox3.Text, out dvalue);
                Form1.icp_collected[Form1.Image_ID].Rows[id][7] = dvalue;
                double.TryParse(textBox5.Text, out dvalue);
                Form1.icp_collected[Form1.Image_ID].Rows[id][8] = dvalue;
                double.TryParse(textBox7.Text, out dvalue);
                Form1.icp_collected[Form1.Image_ID].Rows[id][9] = dvalue;
                double.TryParse(textBox9.Text, out dvalue);
                Form1.icp_collected[Form1.Image_ID].Rows[id][10] = dvalue;
                double.TryParse(textBox11.Text, out dvalue);
                Form1.icp_collected[Form1.Image_ID].Rows[id][11] = dvalue;
                Form1.Save_Control = 1;
            }
        }
        bool add_point(out int new_id)
        {
            new_id = -1;
            bool add_accpt = false;
            Form1 form = Application.OpenForms["Form1"] as Form1;
            if (comboBox1.SelectedIndex == 0)
            {
                if (Form1.gcp_collected[Form1.Image_ID].Rows.Count == Form1.gcp_count)
                    Form1.gcp_collected[Form1.Image_ID].Rows.Add(-1, "", 0, 0, 0, 0, 0);
                Form1.gcp_count = Form1.gcp_count + 1;
                try
                {
                    point_volue(Form1.gcp_count - 1, "gcp");
                    add_accpt = true;
                    new_id = Convert.ToInt32(Form1.gcp_collected[Form1.Image_ID].Rows[Form1.gcp_count - 1][0]) + 1;
                }
                catch
                {
                    Form1.gcp_count = Form1.gcp_count - 1;
                    new_id = Convert.ToInt32(Form1.gcp_collected[Form1.Image_ID].Rows[Form1.gcp_count - 1][0]) + 1;
                    MessageBox.Show("Type another point ID!");
                }
            }
            if (comboBox1.SelectedIndex == 1)
            {
                if (Form1.icp_collected[Form1.Image_ID].Rows.Count == Form1.icp_count)
                    Form1.icp_collected[Form1.Image_ID].Rows.Add(-1, "", 0, 0, 0, 0, 0);
                Form1.icp_count = Form1.icp_count + 1;
                try
                {
                    point_volue(Form1.icp_count - 1, "icp");
                    add_accpt = true;
                    new_id = Convert.ToInt32(Form1.icp_collected[Form1.Image_ID].Rows[Form1.icp_count - 1][0]) + 1;
                }
                catch
                {
                    Form1.icp_count = Form1.icp_count - 1;
                    new_id = Convert.ToInt32(Form1.icp_collected[Form1.Image_ID].Rows[Form1.icp_count - 1][0]) + 1;
                    MessageBox.Show("Type another point ID!");
                }
            }
            return add_accpt;
        }
        //
        // For form events
        //
        public formgcp()
        {
            InitializeComponent();
        }
        private void formgcp_Load(object sender, EventArgs e)
        {
            view_here();
            int i;
            Form_active = true;
            Form1 fform = Application.OpenForms["Form1"] as Form1;
            fform.menuStrip1.Enabled = false;
            this.Owner = fform;

            String s = fform.treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Text.Substring(7);
            int start = 0;
            int end = 33;
            label21.Text = "";
            i = 0;
            while ((i * 33) <= s.Length)
            {
                start = i * 33;
                end = 33;
                if (end > s.Length - (i * 33)) end = s.Length - (i * 33);

                label21.Text = label21.Text + s.Substring(start, end) + '\n';
                i = i + 1;
            }

            comboBox1.SelectedIndex = 0;
            dzy_main();
            panel1.Left = 0;
            panel1.Width = this.Width;
            panel1.Top = dataGridView1.Top + dataGridView1.Height;
            panel1.Height = this.Height - panel1.Top;
        }
        private void formgcp_FormClosing(object sender, FormClosingEventArgs e)
        {
            view_save();
            Form_active = false;
            Form1.edit_point_gcp = false;
            Form1.edit_point_icp = false;
            Form1 f = Application.OpenForms["Form1"] as Form1;
            f.menuStrip1.Enabled = true;

            if (Form1.gcp_collected[Form1.Image_ID].Rows.Count != Form1.gcp_count)
                Form1.gcp_collected[Form1.Image_ID].Rows[Form1.gcp_count].Delete();
            if (Form1.icp_collected[Form1.Image_ID].Rows.Count != Form1.icp_count)
                Form1.icp_collected[Form1.Image_ID].Rows[Form1.icp_count].Delete();
        }
        //
        //Main buttons
        //
        private void button4_Click(object sender, EventArgs e)// for point edit
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                bool add_accpt = true;
                int i;
                int id = 0;
                Form1 form = Application.OpenForms["Form1"] as Form1;

                if (editMod_close == 0)
                {
                    select_index = new int[1];
                    select_tybe = new string[1];
                    select_index[0] = dataGridView1.SelectedRows[0].Index;
                    select_tybe[0] = dataGridView1[1, select_index[0]].Value.ToString();

                    select_id_row = select_index[0]; //for datagridview1. this is important.
                    editMod_close = 1;
                    if (Form1.gcp_collected[Form1.Image_ID].Rows.Count != Form1.gcp_count)
                        Form1.gcp_collected[Form1.Image_ID].Rows[Form1.gcp_count].Delete();
                    if (Form1.icp_collected[Form1.Image_ID].Rows.Count != Form1.icp_count)
                        Form1.icp_collected[Form1.Image_ID].Rows[Form1.icp_count].Delete();

                    if (dataGridView1[1, select_index[0]].Value.ToString() == comboBox1.Items[0].ToString())
                        comboBox1.SelectedIndex = 0;
                    if (dataGridView1[1, select_index[0]].Value.ToString() == comboBox1.Items[1].ToString())
                        comboBox1.SelectedIndex = 1;

                    dataGridView1.Rows[select_index[0]].Selected = true;

                    for (i = 1; i < dataGridView1.SelectedRows.Count; i++)
                        dataGridView1.Rows[i].Selected = false;

                    dataGridView1.Rows[select_index[0]].DefaultCellStyle.SelectionBackColor = line_select_color;

                    textBox1.Text = dataGridView1[0, select_index[0]].Value.ToString();
                    textBox2.Text = dataGridView1[2, select_index[0]].Value.ToString();
                    textBox4.Text = dataGridView1[3, select_index[0]].Value.ToString();
                    textBox6.Text = dataGridView1[4, select_index[0]].Value.ToString();
                    textBox8.Text = dataGridView1[5, select_index[0]].Value.ToString();
                    textBox10.Text = dataGridView1[6, select_index[0]].Value.ToString();

                    if (select_tybe[0] == comboBox1.Items[0].ToString())
                    {
                        id = Form1.gcp_collected[Form1.Image_ID].Rows.IndexOf(
                        Form1.gcp_collected[Form1.Image_ID].Rows.Find(
                            Convert.ToInt32(dataGridView1[0, select_index[0]].Value)));

                        textBox3.Text = Form1.gcp_collected[Form1.Image_ID].Rows[id][7].ToString();
                        textBox5.Text = Form1.gcp_collected[Form1.Image_ID].Rows[id][8].ToString();
                        textBox7.Text = Form1.gcp_collected[Form1.Image_ID].Rows[id][9].ToString();
                        textBox9.Text = Form1.gcp_collected[Form1.Image_ID].Rows[id][10].ToString();
                        textBox11.Text = Form1.gcp_collected[Form1.Image_ID].Rows[id][11].ToString();

                        // This codes show the point to be edited in main window.
                        Form1.edit_id = id;
                        Form1.edit_point_gcp = true;
                        form.GCP_draw(Form1.gcp_collected[Form1.Image_ID]);

                        int[] edit_row = new int[1];//This code shows the point to be edited in this window.
                        edit_row[0] = id;//This code shows the point to be edited in this window.
                        Image<Rgb, byte> img = img_edit_gcp(new Image<Rgb, byte>(Form1.Image_mini), edit_row); //This code shows the point to be edited in this window.
                        edit_row[0] = -2;//This code shows the point to be edited in this window.
                        pictureBox1.Image = img_edit_icp(img, edit_row).Bitmap; //This code shows the point to be edited in this window.
                    }
                    if (select_tybe[0] == comboBox1.Items[1].ToString())
                    {
                        id = Form1.icp_collected[Form1.Image_ID].Rows.IndexOf(
                       Form1.icp_collected[Form1.Image_ID].Rows.Find(
                          Convert.ToInt32(dataGridView1[0, select_index[0]].Value)));

                        textBox3.Text = Form1.icp_collected[Form1.Image_ID].Rows[id][7].ToString();
                        textBox5.Text = Form1.icp_collected[Form1.Image_ID].Rows[id][8].ToString();
                        textBox7.Text = Form1.icp_collected[Form1.Image_ID].Rows[id][9].ToString();
                        textBox9.Text = Form1.icp_collected[Form1.Image_ID].Rows[id][10].ToString();
                        textBox11.Text = Form1.icp_collected[Form1.Image_ID].Rows[id][11].ToString();

                        // This codes show the point to be edited in main window.
                        Form1.edit_id = id;
                        Form1.edit_point_icp = true;
                        form.ICP_draw(Form1.icp_collected[Form1.Image_ID]);

                        int[] edit_row = new int[1];//This code shows the point to be edited in this window.
                        edit_row[0] = id;//This code shows the point to be edited in this window.
                        Image<Rgb, byte> img = img_edit_icp(new Image<Rgb, byte>(Form1.Image_mini), edit_row); //This code shows the point to be edited in this window.
                        edit_row[0] = -2;//This code shows the point to be edited in this window.
                        pictureBox1.Image = img_edit_gcp(img, edit_row).Bitmap; //This code shows the point to be edited in this window.
                    }
                }
                else
                {
                    editMod_close = 0;
                    select_index[0] = dataGridView1.SelectedRows[0].Index;
                    dataGridView1.Rows[select_index[0]].DefaultCellStyle.SelectionBackColor = Color.Empty;
                    Form1.edit_point_gcp = false; // Finish editing. For main image
                    Form1.edit_point_icp = false; // Finish editing. For main image
                    if (select_tybe[0] == comboBox1.Items[0].ToString())
                    {
                        id = Form1.gcp_collected[Form1.Image_ID].Rows.IndexOf(
                       Form1.gcp_collected[Form1.Image_ID].Rows.Find(
                           Convert.ToInt32(dataGridView1[0, select_index[0]].Value)));

                        if (comboBox1.SelectedIndex == 0)
                        {
                            try
                            {
                                point_volue(id, "gcp");
                                add_accpt = true;
                            }
                            catch
                            {
                                add_accpt = false;
                                MessageBox.Show("Type another point ID!");
                            }
                        }
                        if (comboBox1.SelectedIndex == 1)
                        {
                            String info = MessageBox.Show("Change point type?", "Warning",
                             MessageBoxButtons.YesNo, MessageBoxIcon.Warning).ToString();
                            if (info == "Yes")
                            {
                                add_accpt = add_point(out i); // "out i" is not important in here.
                                if (add_accpt == true)
                                {
                                    Form1.gcp_collected[Form1.Image_ID].Rows[id].Delete();
                                    Form1.gcp_count = Form1.gcp_count - 1;
                                }
                            }
                            else add_accpt = false;
                        }
                        if (add_accpt == true)
                            dzy_form(select_index[0]);
                        else
                        {
                            img_dzy();
                            clear_page();
                        }
                    }
                    if (select_tybe[0] == comboBox1.Items[1].ToString())
                    {
                        id = Form1.icp_collected[Form1.Image_ID].Rows.IndexOf(
                        Form1.icp_collected[Form1.Image_ID].Rows.Find(
                          Convert.ToInt32(dataGridView1[0, select_index[0]].Value)));

                        if (comboBox1.SelectedIndex == 1)
                        {
                            try
                            {
                                point_volue(id, "icp");
                                add_accpt = true;
                            }
                            catch
                            {
                                add_accpt = false;
                                MessageBox.Show("Type another point ID!");
                            }
                        }
                        if (comboBox1.SelectedIndex == 0)
                        {
                            String info = MessageBox.Show("Change point type?", "Warning",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning).ToString();
                            if (info == "Yes")
                            {
                                add_accpt = add_point(out i); // "out i" is not important in here.
                                if (add_accpt == true)
                                {
                                    Form1.icp_collected[Form1.Image_ID].Rows[id].Delete();
                                    Form1.icp_count = Form1.icp_count - 1;
                                }
                            }
                            else add_accpt = false;
                        }
                        if (add_accpt == true)
                            dzy_form(select_index[0]);
                        else
                        {
                            img_dzy();
                            clear_page();
                        }
                    }
                    Form1.Save_Control = 1;
                    for (i = 0; i < dataGridView1.RowCount; i++)
                        dataGridView1.Rows[i].Selected = false;
                    dataGridView1.Rows[select_id_row].Selected = true;
                }
            }
            else MessageBox.Show("Select only one GCP/ICP!");
            button_control();
        }
        private void button5_Click(object sender, EventArgs e)// for point delete
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (editMod_close == 0)
                {
                    int i;
                    if (Form1.gcp_collected[Form1.Image_ID].Rows.Count != Form1.gcp_count)
                        Form1.gcp_collected[Form1.Image_ID].Rows[Form1.gcp_count].Delete();
                    if (Form1.icp_collected[Form1.Image_ID].Rows.Count != Form1.icp_count)
                        Form1.icp_collected[Form1.Image_ID].Rows[Form1.icp_count].Delete();

                    select_index = new int[dataGridView1.SelectedRows.Count];
                    select_tybe = new string[dataGridView1.SelectedRows.Count];
                    for (i = 0; i < select_index.Length; i++) //for index and tybe
                    {
                        select_index[i] = dataGridView1.SelectedRows[i].Index;
                        select_tybe[i] = dataGridView1[1, select_index[i]].Value.ToString();
                    }
                    for (i = 0; i < select_index.Length; i++) // for point color
                        dataGridView1.Rows[select_index[i]].DefaultCellStyle.SelectionBackColor = line_select_color;

                    int edit_row;
                    int[] edit_gcp_row = new int[0];
                    int[] edit_icp_row = new int[0];
                    for (i = 0; i < select_index.Length; i++)
                    {
                        if (select_tybe[i] == comboBox1.Items[0].ToString())
                        {
                            edit_row = Form1.gcp_collected[Form1.Image_ID].Rows.IndexOf(
                   Form1.gcp_collected[Form1.Image_ID].Rows.Find(Convert.ToInt32
                   (dataGridView1[0, select_index[i]].Value)));

                            Array.Resize<int>(ref edit_gcp_row, edit_gcp_row.Length + 1);
                            edit_gcp_row[edit_gcp_row.Length - 1] = edit_row;
                        }

                        if (select_tybe[i] == comboBox1.Items[1].ToString())
                        {
                            edit_row = Form1.icp_collected[Form1.Image_ID].Rows.IndexOf(
                   Form1.icp_collected[Form1.Image_ID].Rows.Find(
                      Convert.ToInt32(dataGridView1[0, select_index[i]].Value)));

                            Array.Resize<int>(ref edit_icp_row, edit_icp_row.Length + 1);
                            edit_icp_row[edit_icp_row.Length - 1] = edit_row;

                        }
                    }
                    Array.Sort(select_index);
                    Array.Sort(edit_gcp_row);
                    Array.Sort(edit_icp_row);
                    Image<Rgb, byte> img = img_edit_gcp(new Image<Rgb, byte>(Form1.Image_mini), edit_gcp_row);
                    pictureBox1.Image = img_edit_icp(img, edit_icp_row).Bitmap;
                    Array.Reverse(select_index);
                    Array.Reverse(edit_gcp_row);
                    Array.Reverse(edit_icp_row);

                    String info = MessageBox.Show("Delete this " + select_index.Length + " point(s)?", "Warning",
                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning).ToString();
                    if (info == "Yes")
                    {
                        Form1.Save_Control = 1;
                        for (i = 0; i < select_index.Length; i++)
                            dataGridView1.Rows[select_index[i]].DefaultCellStyle.SelectionBackColor = Color.Empty;
                        for (i = 0; i < edit_gcp_row.Length; i++)
                        {
                            Form1.gcp_collected[Form1.Image_ID].Rows[edit_gcp_row[i]].Delete();
                            Form1.gcp_count = Form1.gcp_collected[Form1.Image_ID].Rows.Count;
                        }

                        for (i = 0; i < edit_icp_row.Length; i++)
                        {
                            Form1.icp_collected[Form1.Image_ID].Rows[edit_icp_row[i]].Delete();
                            Form1.icp_count = Form1.icp_collected[Form1.Image_ID].Rows.Count;
                        }
                        for (i = 0; i < select_index.Length; i++)
                            dataGridView1.Rows.RemoveAt(select_index[i]);
                    }
                    if (info == "No")
                        for (i = 0; i < select_index.Length; i++)
                            dataGridView1.Rows[select_index[i]].DefaultCellStyle.SelectionBackColor = Color.Empty;

                    img_dzy();
                }
                else
                {
                    dataGridView1.Rows[select_index[0]].DefaultCellStyle.SelectionBackColor = Color.Empty;
                    editMod_close = 0;
                    img_dzy();
                    clear_page();
                    button_control();
                }
            }
            else MessageBox.Show("Select Row");
        }
        private void button6_Click(object sender, EventArgs e)// for point edit
        {
            int new_id;
            bool add = add_point(out new_id);
            if (add == true)
            {
                dataGridView1.Rows.Add();
                dzy_form(dataGridView1.RowCount - 1);

                int i;
                for (i = 0; i < dataGridView1.SelectedRows.Count; i++)
                    dataGridView1.Rows[dataGridView1.SelectedRows[i].Index].Selected = false;
                dataGridView1.Rows[dataGridView1.RowCount - 1].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[dataGridView1.RowCount - 1].Index;
            }
            textBox1.Text = new_id.ToString();
        }
        //
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)// for select points
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (checkBox1.Checked == true)
                {
                    Form1 form = Application.OpenForms["Form1"] as Form1;
                    form.ımageBox1.SetZoomScale(1, new Point(0, 0));
                    double realx, realy;
                    int hort, vert;
                    realx = Convert.ToDouble(dataGridView1.SelectedRows[0].Cells[3].Value);
                    realy = Convert.ToDouble(dataGridView1.SelectedRows[0].Cells[2].Value);
                    hort = Convert.ToInt32(realx - (form.ımageBox1.Width / 2));
                    if (hort < 0) hort = 0;
                    if (hort > form.ımageBox1.HorizontalScrollBar.Maximum) hort = form.ımageBox1.HorizontalScrollBar.Maximum;
                    form.ımageBox1.HorizontalScrollBar.Value = hort;
                    vert = Convert.ToInt32(realy - (form.ımageBox1.Height / 2));
                    if (vert < 0) vert = 0;
                    if (vert > form.ımageBox1.VerticalScrollBar.Maximum) vert = form.ımageBox1.VerticalScrollBar.Maximum;
                    form.ımageBox1.VerticalScrollBar.Value = vert;

                    form.ımageBox1.Refresh();
                    form.GCP_draw(Form1.gcp_collected[Form1.Image_ID]);
                    form.ICP_draw(Form1.icp_collected[Form1.Image_ID]);
                }
            }
        }     
        //
        // 
        //
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button_control();
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            button_control();
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            button_control();
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            button_control();
        }
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            button_control();
        }
        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            button_control();
        }
        //
        // Key entry in textboxs
        //
        char character_cheak(char character, string text)
        {
            int key = character;
            if (((key >= 47) && (key <= 57)) || (key == 8) || (key == 44))
            {
                if (key == 44)
                {
                    int comma = text.ToCharArray().Count(c => c == ',');
                    if (comma != 0) character = Convert.ToChar(0);
                }
            }
            else character = Convert.ToChar(0);
            return character;
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            int key = e.KeyChar;
            if (((key >= 47) && (key <= 57)) || (key == 8)) e.KeyChar = e.KeyChar;
            else e.KeyChar = Convert.ToChar(0);
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = character_cheak(e.KeyChar, textBox2.Text);
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = character_cheak(e.KeyChar, textBox3.Text);
        }
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = character_cheak(e.KeyChar, textBox4.Text);
        }
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = character_cheak(e.KeyChar, textBox5.Text);
        }
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = character_cheak(e.KeyChar, textBox6.Text);
        }
        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = character_cheak(e.KeyChar, textBox7.Text);
        }
        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = character_cheak(e.KeyChar, textBox8.Text);
        }
        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = character_cheak(e.KeyChar, textBox9.Text);
        }
        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = character_cheak(e.KeyChar, textBox10.Text);
        }
        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = character_cheak(e.KeyChar, textBox11.Text);
        }
        //
        // Others buttons
        //
        private void button1_Click(object sender, EventArgs e)// input file of points
        {
            text_read read = new text_read(textBox12.Text);
            read.ShowDialog();
            dzy_main();
            Form1.Save_Control = 1;
        }
        private void button3_Click(object sender, EventArgs e)// Point file of browse
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Text files (*.txt ,*.gcp) |*.txt; *.gcp; | All files (*.*) | *.*";
            openFileDialog1.ShowDialog();
            textBox12.Text = openFileDialog1.FileName;
        }
        private void button7_Click(object sender, EventArgs e)// for point view
        {
            points_view p = new points_view();
            p.ShowDialog();
        }
        //
        // for this form of size.
        //
        static bool size_accept = false;// form'un genişliğini sadece aşağaya doğru uzatmak için değişkendir.
        static int first_Y;

        static Color line_color = Color.FromKnownColor(KnownColor.ControlLight);
   
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (line_color == Color.FromKnownColor(KnownColor.ControlDark)) line_color = Color.FromKnownColor(KnownColor.ControlLight);
            else line_color = Color.FromKnownColor(KnownColor.ControlDark);
            dataGridView1.Rows[dataGridView1.RowCount - 1].DefaultCellStyle.BackColor = line_color;

        }

        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            line_color = Color.FromKnownColor(KnownColor.ControlLight);
            int i;
            for(i=0;i<dataGridView1.RowCount;i++)
            {
                if (line_color == Color.FromKnownColor(KnownColor.ControlDark)) line_color = Color.FromKnownColor(KnownColor.ControlLight);
                else line_color = Color.FromKnownColor(KnownColor.ControlDark);
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = line_color;
            }
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (size_accept == true)
            {
                int size_dy = e.Y - first_Y;
                if ((this.Height <= 510) && (size_dy < 0))
                    size_dy = 0;
                panel1.Top = panel1.Top + size_dy;
                this.Height = this.Height + size_dy;
            }
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            size_accept = true;
            first_Y = e.Y;
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            size_accept = false;
        }
        //
        string formWindow = "max";
        void sizeform(string wform)
        {
            if (wform == "min")
            {
                this.Size = new Size(350, 330);
                groupBox4.Visible = false;
                groupBox2.Location = groupBox4.Location;
                button5.Enabled = false;
                button4.Enabled = false;
            }
            if (wform == "max")
            {
                this.Size = new Size(747, 640);
                groupBox2.Location = new Point(8, 116);
                groupBox4.Visible = true;
                button5.Enabled = true;
                button4.Enabled = true;
                panel1.Top = dataGridView1.Top + dataGridView1.Height;
            }
        }
        protected override void WndProc(ref Message m)
        {

            if (m.Msg == 0x112 && m.WParam.ToInt32() == 0xf030)
            {
                if (formWindow == "min") formWindow = "max";
                else formWindow = "min";
                sizeform(formWindow);
                return;
            }
            base.WndProc(ref m);
        }
    }
}
