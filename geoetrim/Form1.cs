using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Diagnostics;

namespace GeoEtrim
{
    public partial class Form1 : Form
    {
        //
        //VARİABLES
        //
        //about mini windows
        static int UpDown = 0, updowndy = 0, leftright, leftrightdx = 0; //This is for the size of windows
        public static double zoom = 1, cnt_zoom;//zoom is zoom scale value. cnt_zooom is control zoom. It is meant to provide usability while zooming with the mouse.
        static Point clickPosition, scrollPosition;//This command to drag with the mouse.
        //
        static int draw = 0;//This command to drag with the mouse.
        public static Image<Gray, byte>[] Main_Bants; // images has got orginal all bants.
        static int followclick = 0; // to scroll the tracking window
        public static int[] main_width = new int[0];
        public static int[] main_height = new int[0];
        public static Bitmap Image_mini; // Tracking windows has got image
        public static int down_scale; //image is decrease rate for trackin windows
        //
        public static int Save_Control = 0; // query to securely close the project
        public static string proje_path; // this is path of proje
        public static List<string> proje_info = new List<string>();
        public static List<string> Imagelist = new List<string>(); //this value is image file path
        public static List<int> bants_img_count = new List<int>();
        public static int Image_ID = -1; //loaded image reference numbers
        static public int Project_ID = -1; // this shows the active project. if there isn't active project, Image must not loading.
        // about point
        static public DataTable[] gcp_collected = new DataTable[0];
        static public int gcp_count = 0;
        static public DataTable[] icp_collected = new DataTable[0];
        static public int icp_count = 0;
        static public int edit_id = 0; // fix point
        static public bool edit_point_gcp = false; // fix point
        static public bool edit_point_icp = false; // fix point
        // about point view in this
        static public float gcp_line_length, icp_line_length, gcp_text_size, icp_text_size, gcp_line_width, icp_line_width;
        static public bool gcp_type_name, icp_type_name, gcp_id_name, icp_id_name;
        static public Color gcp_line_color, gcp_text_color, icp_line_color, icp_text_color, select_point_color, mouse_icon_color;
        static public string gcp_font_name, icp_font_name;
        //
        //FUNCTİONS
        //
        public void zoom_in(Point zoom_position)
        {
            if (zoom > 2)
            {

                zoom = zoom / 1.5;
                zoom = Math.Round(zoom);
            }
            else zoom = zoom / 2;
            ımageBox1.SetZoomScale(zoom, zoom_position);
        }
        public void zoom_out(Point zoom_position)
        {
            if (zoom >= 2)
            {
                zoom = zoom * 1.5;
                zoom = Math.Round(zoom);
            }
            else zoom = zoom * 2;
            ımageBox1.SetZoomScale(zoom, zoom_position);
        }
        public void decrease(Bitmap image)
        {
            Image<Rgb, byte> main_Image = new Image<Rgb, byte>(image);
            double w = main_width[Form1.Image_ID], h = main_height[Form1.Image_ID];
            down_scale = 2;
            while ((w > 350) || (h > 350))
            {
                w = main_width[Form1.Image_ID] / down_scale;
                h = main_height[Form1.Image_ID] / down_scale;
                down_scale = down_scale + 1;
            }
            Image_mini = main_Image.Resize((int)w, (int)h, Inter.Area).Bitmap;
            ımageBox2.Width = Image_mini.Width;
            treeView1.Width = Image_mini.Width;
            panel1.Width = Image_mini.Width;

            ımageBox2.Height = Image_mini.Height;
            treeView1.Height = ımageBox1.Height - panel1.Height - ımageBox2.Height;

            panel1.Top = treeView1.Top + treeView1.Height;
            ımageBox2.Top = treeView1.Top + treeView1.Height + panel1.Height;

            panel2.Left = treeView1.Left + treeView1.Width;
            ımageBox1.Left = treeView1.Left + treeView1.Width + panel2.Width;
            ımageBox1.Width = this.Width - ımageBox1.Left - 20;

            toolStripButton9.Enabled = true;
            toolStripButton10.Enabled = true;
            toolStripButton11.Enabled = true;
            toolStripButton12.Enabled = true;
            toolStripButton13.Enabled = true;
            toolStripButton14.Enabled = true;
            toolStripButton15.Enabled = true;
            toolStripButton16.Enabled = true;
            toolStripButton8.Enabled = true;
            toolStripLabel5.Text = "Active:" + "(img)" + treeView1.Nodes[Project_ID].Nodes[Image_ID].Text.Substring(7);

        }
        public void follow_window()
        {
            double follow_x, follow_y;
            Image<Bgr, byte> img = new Image<Bgr, byte>(Image_mini);
            follow_x = ımageBox1.HorizontalScrollBar.Value;
            follow_y = ımageBox1.VerticalScrollBar.Value;

            Rectangle rect = new Rectangle((int)(follow_x / ((down_scale - 1))), (int)(follow_y / (down_scale - 1)),
                (int)(ımageBox1.Width / ((down_scale - 1) * ımageBox1.ZoomScale)),
                (int)(ımageBox1.Height / ((down_scale - 1) * ımageBox1.ZoomScale)));

            CvInvoke.Rectangle(img, rect, new Bgr(Color.Red).MCvScalar, 1, LineType.EightConnected, 0);
            ımageBox2.Image = img;
            ımageBox2.Refresh();

            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);

        }
        public DataTable point_collumns_creat(DataTable point_table)
        {
            point_table = new DataTable();
            point_table.Columns.Add("PointID", typeof(int));
            point_table.Columns.Add("PointType", typeof(string));
            point_table.Columns.Add("Row", typeof(double));
            point_table.Columns.Add("Column", typeof(double));
            point_table.Columns.Add("X", typeof(double));
            point_table.Columns.Add("Y", typeof(double));
            point_table.Columns.Add("Z", typeof(double));
            point_table.Columns.Add("StdRow", typeof(double));
            point_table.Columns.Add("StdCol", typeof(double));
            point_table.Columns.Add("StdX", typeof(double));
            point_table.Columns.Add("StdY", typeof(double));
            point_table.Columns.Add("StdZ", typeof(double));
            point_table.PrimaryKey = new DataColumn[] { point_table.Columns[0] };
            return point_table;
        }
        public void GCP_draw(DataTable gcp_array)
        {
            if ((Project_ID != -1) && (Image_ID != -1))
            {
                if (treeView1.Nodes[Project_ID].Nodes[Image_ID].Nodes[1].ImageIndex == 19)
                {
                    float[] imgx = new float[gcp_array.Rows.Count];
                    float[] imgy = new float[gcp_array.Rows.Count];
                    int i;
                    Graphics g = ımageBox1.CreateGraphics();
                    for (i = 0; i < gcp_array.Rows.Count; i++)
                    {
                        imgy[i] = Convert.ToSingle(gcp_array.Rows[i][2]) - ımageBox1.VerticalScrollBar.Value;
                        imgx[i] = Convert.ToSingle(gcp_array.Rows[i][3]) - ımageBox1.HorizontalScrollBar.Value;
                        imgx[i] = imgx[i] * Convert.ToSingle(ımageBox1.ZoomScale);
                        imgy[i] = imgy[i] * Convert.ToSingle(ımageBox1.ZoomScale);
                        g.DrawLine(new Pen(gcp_line_color, gcp_line_width), new PointF(imgx[i] - (gcp_line_length / 2), imgy[i]), new PointF(imgx[i] + (gcp_line_length / 2), imgy[i]));
                        g.DrawLine(new Pen(gcp_line_color, gcp_line_width), new PointF(imgx[i], imgy[i] - (gcp_line_length / 2)), new PointF(imgx[i], imgy[i] + (gcp_line_length / 2)));

                        Font ffont = new Font(gcp_font_name, gcp_text_size);
                        string namepoint = "";
                        if (gcp_type_name == true) namepoint = "GCP";
                        if (gcp_id_name == true)
                        {
                            if (gcp_type_name == false)
                                namepoint = gcp_array.Rows[i][0].ToString();
                            if (gcp_type_name == true)
                                namepoint = namepoint + "-" + gcp_array.Rows[i][0].ToString();
                        }
                        if (gcp_array.Rows[i][0].ToString() == "-1") namepoint = "?";
                        g.DrawString(namepoint, ffont, new SolidBrush(gcp_text_color), new PointF(imgx[i] + (gcp_line_length / 2), imgy[i] - (gcp_line_length - 3)));
                    }
                    // this code is for marking the point displaying the point arranged in the main image.
                    if (edit_point_gcp == true) // nokta atma pencesinde editlmek içindir.[tr]
                    {
                        float x_edit;
                        float y_edit;
                        y_edit = Convert.ToSingle(gcp_collected[Image_ID].Rows[edit_id][2]) - ımageBox1.VerticalScrollBar.Value;
                        x_edit = Convert.ToSingle(gcp_collected[Image_ID].Rows[edit_id][3]) - ımageBox1.HorizontalScrollBar.Value;
                        x_edit = x_edit * Convert.ToSingle(ımageBox1.ZoomScale);
                        y_edit = y_edit * Convert.ToSingle(ımageBox1.ZoomScale);
                        g.DrawEllipse(new Pen(new SolidBrush(select_point_color), 5), x_edit - gcp_line_length, y_edit - gcp_line_length, 2 * gcp_line_length, 2 * gcp_line_length);
                    }
                }
            }
        }
        public void ICP_draw(DataTable icp_array)
        {
            if ((Project_ID != -1) && (Image_ID != -1))
            {
                if (treeView1.Nodes[Project_ID].Nodes[Image_ID].Nodes[1].ImageIndex == 19)
                {
                    float[] imgx = new float[icp_array.Rows.Count];
                    float[] imgy = new float[icp_array.Rows.Count];
                    int i;
                    Graphics g = ımageBox1.CreateGraphics();
                    for (i = 0; i < icp_array.Rows.Count; i++)
                    {
                        imgy[i] = Convert.ToSingle(icp_array.Rows[i][2]) - ımageBox1.VerticalScrollBar.Value;
                        imgx[i] = Convert.ToSingle(icp_array.Rows[i][3]) - ımageBox1.HorizontalScrollBar.Value;
                        imgx[i] = imgx[i] * Convert.ToSingle(ımageBox1.ZoomScale);
                        imgy[i] = imgy[i] * Convert.ToSingle(ımageBox1.ZoomScale);
                        g.DrawLine(new Pen(icp_line_color, icp_line_width), new PointF(imgx[i] - (icp_line_length / 2), imgy[i]), new PointF(imgx[i] + (icp_line_length / 2), imgy[i]));
                        g.DrawLine(new Pen(icp_line_color, icp_line_width), new PointF(imgx[i], imgy[i] - (icp_line_length / 2)), new PointF(imgx[i], imgy[i] + (icp_line_length / 2)));

                        Font ffont = new Font(icp_font_name, icp_text_size);
                        string namepoint = "";
                        if (icp_type_name == true) namepoint = "ICP";
                        if (icp_id_name == true)
                        {
                            if (icp_type_name == false)
                                namepoint = icp_array.Rows[i][0].ToString();
                            if (icp_type_name == true)
                                namepoint = namepoint + "-" + icp_array.Rows[i][0].ToString();
                        }
                        if (icp_array.Rows[i][0].ToString() == "-1") namepoint = "?";
                        g.DrawString(namepoint, ffont, new SolidBrush(icp_text_color), new PointF(imgx[i] + (icp_line_length / 2), imgy[i] - (icp_line_length - 3)));
                    }
                    // this code is for marking the point displaying the point arranged in the main image.
                    if (edit_point_icp == true) // nokta atma pencesinde editlmek içindir.[tr]
                    {
                        float x_edit;
                        float y_edit;
                        y_edit = Convert.ToSingle(icp_collected[Image_ID].Rows[edit_id][2]) - ımageBox1.VerticalScrollBar.Value;
                        x_edit = Convert.ToSingle(icp_collected[Image_ID].Rows[edit_id][3]) - ımageBox1.HorizontalScrollBar.Value;
                        x_edit = x_edit * Convert.ToSingle(ımageBox1.ZoomScale);
                        y_edit = y_edit * Convert.ToSingle(ımageBox1.ZoomScale);
                        g.DrawEllipse(new Pen(new SolidBrush(select_point_color), 5), x_edit - icp_line_length, y_edit - icp_line_length, 2 * icp_line_length, 2 * icp_line_length);
                    }
                }
            }
        }

        public void view_main_defult()
        {
            gcp_line_length = 23.0f;
            icp_line_length = 23.0f;
            gcp_text_size = 10.0f;
            icp_text_size = 10.0f;
            gcp_line_width = 2.0f;
            icp_line_width = 2.0f;
            gcp_type_name = true;
            icp_type_name = true;
            gcp_id_name = true;
            icp_id_name = true;
            gcp_line_color = Color.Red;
            gcp_text_color = Color.Red;
            icp_line_color = Color.Aqua;
            icp_text_color = Color.Aqua;
            select_point_color = Color.Yellow;
            mouse_icon_color = Color.Gold;
            gcp_font_name = "Consolas";
            icp_font_name = "Consolas";
        }
        public void view_save()
        {
            StreamWriter wr = File.CreateText(Application.StartupPath + "/main_view.txt");
            wr.WriteLine(gcp_line_length.ToString());
            wr.WriteLine(icp_line_length.ToString());
            wr.WriteLine(gcp_text_size.ToString());
            wr.WriteLine(icp_text_size.ToString());
            wr.WriteLine(gcp_line_width.ToString());
            wr.WriteLine(icp_line_width.ToString());
            wr.WriteLine(gcp_type_name.ToString());
            wr.WriteLine(icp_type_name.ToString());
            wr.WriteLine(gcp_id_name.ToString());
            wr.WriteLine(icp_id_name.ToString());
            wr.WriteLine(gcp_line_color.Name);
            wr.WriteLine(gcp_text_color.Name);
            wr.WriteLine(icp_line_color.Name);
            wr.WriteLine(icp_text_color.Name);
            wr.WriteLine(select_point_color.Name);
            wr.WriteLine(mouse_icon_color.Name);
            wr.WriteLine(gcp_font_name.ToString());
            wr.WriteLine(icp_font_name.ToString());
            wr.Close();
        }
        //
        // form Events
        //
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            formgcp fgcp = new formgcp();
            fgcp.view_here();
            GeoTransform geo = new GeoTransform();
            geo.view_here();
            if (File.Exists(Application.StartupPath + "/main_view.txt") == true)
            {
                try
                {
                    int dat;
                    dat = 0;

                    if (System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.ToString() == ",")
                        dat = 0;
                    else
                        dat = 1;
                    StreamReader view_m = new StreamReader(Application.StartupPath + "/main_view.txt");
                    string[] line = new string[18];
                    int i;
                    for (i = 0; i < 18; i++) line[i] = view_m.ReadLine();
                    view_m.Close();
                    if (dat == 0) gcp_line_length = Convert.ToSingle(line[0].Replace('.', ','));
                    if (dat == 1) gcp_line_length = Convert.ToSingle(line[0].Replace(',', '.'));
                    if (dat == 0) icp_line_length = Convert.ToSingle(line[1].Replace('.', ','));
                    if (dat == 1) icp_line_length = Convert.ToSingle(line[1].Replace(',', '.'));
                    if (dat == 0) gcp_text_size = Convert.ToSingle(line[2].Replace('.', ','));
                    if (dat == 1) gcp_text_size = Convert.ToSingle(line[2].Replace(',', '.'));
                    if (dat == 0) icp_text_size = Convert.ToSingle(line[3].Replace('.', ','));
                    if (dat == 1) icp_text_size = Convert.ToSingle(line[3].Replace(',', '.'));
                    if (dat == 0) gcp_line_width = Convert.ToSingle(line[4].Replace('.', ','));
                    if (dat == 1) gcp_line_width = Convert.ToSingle(line[4].Replace(',', '.'));
                    if (dat == 0) icp_line_width = Convert.ToSingle(line[5].Replace('.', ','));
                    if (dat == 1) icp_line_width = Convert.ToSingle(line[5].Replace(',', '.'));
                    gcp_type_name = Convert.ToBoolean(line[6]);
                    icp_type_name = Convert.ToBoolean(line[7]);
                    gcp_id_name = Convert.ToBoolean(line[8]);
                    icp_id_name = Convert.ToBoolean(line[9]);
                    gcp_line_color = Color.FromName(line[10]);
                    gcp_text_color = Color.FromName(line[11]);
                    icp_line_color = Color.FromName(line[12]);
                    icp_text_color = Color.FromName(line[13]);
                    select_point_color = Color.FromName(line[14]);
                    mouse_icon_color = Color.FromName(line[15]);
                    gcp_font_name = line[16];
                    icp_font_name = line[17];
                }
                catch
                {
                    view_main_defult();
                }
            }
            else
                view_main_defult();

            if (open.System_change == true)
            {
                string Reset_now = MessageBox.Show("Please restart your computer to run the program.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation).ToString();
                if (Reset_now == "Yes")
                    Process.Start("shutdown", "/r /t 0");
                else
                    this.Close();
            }

            clickPosition.X = 0;
            clickPosition.Y = 0;
            scrollPosition.X = 0;
            scrollPosition.Y = 0;

            this.ımageBox1.HorizontalScrollBar.ValueChanged += HorizontalScrollBar_ValueChanged;
            this.ımageBox1.HorizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
            this.ımageBox1.VerticalScrollBar.ValueChanged += VerticalScrollBar_ValueChanged;
            this.ımageBox1.VerticalScrollBar.Scroll += VerticalScrollBar_Scroll;

            treeView1.Top = toolStrip2.Top + toolStrip2.Height;
            ımageBox1.Top = treeView1.Top;
            panel2.Top = treeView1.Top;
            panel2.Left = treeView1.Left + treeView1.Width;
            ımageBox1.Left = panel2.Left + panel2.Width;
            ımageBox1.Width = this.Width - 20 - ımageBox1.Left;

            panel2.Height = toolStrip1.Top - panel2.Top;
            ımageBox1.Height = panel2.Height;
            ımageBox2.Height = panel2.Height / 4;

            ımageBox2.Top = toolStrip1.Top - ımageBox2.Height;
            panel1.Top = ımageBox2.Top - panel1.Height;
            treeView1.Height = panel1.Top - treeView1.Top;
        }
        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (Project_ID != -1)
            {
                string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                openFileDialog1.FileName = FileList[0];
                processbarform pform = new processbarform();
                pform.Show();
                Visual v = new Visual();
                v.open_Image(openFileDialog1.FileName,false,true);
                pform.Close();
            }
            else
                MessageBox.Show("Open project at first!");
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visual v = new Visual();
            int a = v.close_All();
            if (a == 0) e.Cancel = true;
            view_save();
            formgcp fgcp = new formgcp();
            fgcp.view_save();
            GeoTransform geo = new GeoTransform();
            geo.view_save();
        }
        //
        // Head of menu events
        //
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Project file (*.gpr)|*.gpr*";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                Visual v = new Visual();
                v.load_prj(openFileDialog1.FileName);
                toolStripButton7.Enabled = true;
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visual v = new Visual();
            v.Save_prj();
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visual v = new Visual();
            v.New_prj();
        }
        private void OpenimageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Image files |*.*";
            openFileDialog1.ShowDialog();
            processbarform pform = new processbarform();
            pform.Show();
            Visual v = new Visual();
            v.open_Image(openFileDialog1.FileName,false,true);
            pform.Close();
        }
        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visual v = new Visual();
            v.close_prj();
        }
        private void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txt_points f = new txt_points();
            f.Show();
        }
        private void GeoTransformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeoTransform geo = new GeoTransform();
            geo.ShowDialog();
        }
        private void pointsViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            points_view p = new points_view();
            p.ShowDialog();
        }
        private void reputationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Terms t = new Terms();
            t.ShowDialog();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }
        private void registerProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Register r = new Register();
            r.ShowDialog();
        }
        //
        //Main image events
        //
        private void ımageBox1_MouseMove(object sender, MouseEventArgs e)
        {   // This is do not over zooming
            cnt_zoom = 0; // this is very important for MouseWheel events                 
            ımageBox1.MouseWheel += ImageBox1_MouseWheel;

            // (row-col) coordinator
            float realx, realy;
            realx = Convert.ToSingle(ımageBox1.HorizontalScrollBar.Value + (e.X / ımageBox1.ZoomScale));
            realy = Convert.ToSingle(ımageBox1.VerticalScrollBar.Value + (e.Y / ımageBox1.ZoomScale));
            toolStripLabel2.Text = realx.ToString();
            toolStripLabel1.Text = realy.ToString();


            // to drag the Image
            if (draw == 1)
            {
                scrollPosition.X = scrollPosition.X + (clickPosition.X - e.X);
                scrollPosition.Y = scrollPosition.Y + (clickPosition.Y - e.Y);
                clickPosition.X = e.X;
                clickPosition.Y = e.Y;
                if (scrollPosition.X < 0) scrollPosition.X = 0;
                if (scrollPosition.Y < 0) scrollPosition.Y = 0;
                if (scrollPosition.X > ımageBox1.HorizontalScrollBar.Maximum) scrollPosition.X = ımageBox1.HorizontalScrollBar.Maximum;
                if (scrollPosition.Y > ımageBox1.VerticalScrollBar.Maximum) scrollPosition.Y = ımageBox1.VerticalScrollBar.Maximum;
                ımageBox1.HorizontalScrollBar.Value = scrollPosition.X;
                ımageBox1.VerticalScrollBar.Value = scrollPosition.Y;
                ımageBox1.Refresh();
            }

            //gcp form active 
            if (formgcp.Form_active == true) //for drop point whit mouse
            {
                ımageBox1.Refresh();
                Graphics g = ımageBox1.CreateGraphics();
                g.DrawLine(new Pen(mouse_icon_color, gcp_line_width), new PointF(e.X - (gcp_line_length / 2), e.Y), new PointF(e.X + (gcp_line_length / 2), e.Y));
                g.DrawLine(new Pen(mouse_icon_color, gcp_line_width), new PointF(e.X, e.Y - (gcp_line_length / 2)), new PointF(e.X, e.Y + (gcp_line_length / 2)));
            }

            //
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        private void ımageBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                draw = 0;
                ımageBox1.Cursor = Cursors.Default;
            }
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        private void ımageBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // To scroll the image whit mouse.
            if (e.Button == MouseButtons.Middle)
            {
                ımageBox1.Cursor = Cursors.Hand;
                draw = 1;
                clickPosition.X = e.X;
                clickPosition.Y = e.Y;
                scrollPosition.X = ımageBox1.HorizontalScrollBar.Value;
                scrollPosition.Y = ımageBox1.VerticalScrollBar.Value;
            }

            // this is about gcp/icp.
            if (formgcp.Form_active == true)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (gcp_collected[Image_ID].Rows.Count != gcp_count)
                        gcp_collected[Image_ID].Rows[gcp_count].Delete();
                    if (icp_collected[Image_ID].Rows.Count != icp_count)
                        icp_collected[Image_ID].Rows[icp_count].Delete();        
                    
                    float realx, realy;
                    realx = Convert.ToSingle(ımageBox1.HorizontalScrollBar.Value
                        + (e.X / ımageBox1.ZoomScale));
                    realy = Convert.ToSingle(ımageBox1.VerticalScrollBar.Value
                        + (e.Y / ımageBox1.ZoomScale));                     
                    formgcp form = Application.OpenForms["formgcp"] as formgcp;
                   
                    if (form.comboBox1.SelectedIndex == 0)
                    {
                        if (Image_ID != -1)
                        {
                            if (gcp_collected[Image_ID].Rows.Count != 0)
                                if (Convert.ToInt32(gcp_collected[Image_ID].Rows[gcp_collected[Image_ID].Rows.Count - 1][0].ToString()) == -1)
                                    gcp_collected[Image_ID].Rows[gcp_collected[Image_ID].Rows.Count - 1].Delete();
                            if ((realx <= ımageBox1.Image.Size.Width) && (realy <= ımageBox1.Image.Size.Height))
                            {
                                gcp_collected[Image_ID].Rows.Add(-1, "", realy, realx, 0, 0);
                                GCP_draw(gcp_collected[Image_ID]);
                            }
                            else MessageBox.Show("Out of image!");
                        }
                    }
                    if (form.comboBox1.SelectedIndex == 1)
                    {
                        if (Image_ID != -1)
                        {
                            if (icp_collected[Image_ID].Rows.Count != 0)
                                if (Convert.ToInt32(icp_collected[Image_ID].Rows[icp_collected[Image_ID].Rows.Count - 1][0].ToString()) == -1)
                                    icp_collected[Image_ID].Rows[icp_collected[Image_ID].Rows.Count - 1].Delete();
                            if ((realx <= ımageBox1.Image.Size.Width) && (realy <= ımageBox1.Image.Size.Height))
                            {
                                icp_collected[Image_ID].Rows.Add(-1, "", realy, realx, 0, 0);
                                ICP_draw(icp_collected[Image_ID]);
                            }
                            else MessageBox.Show("Out of image!");
                        }
                    }
                    form.img_dzy();
                    Image<Rgb, byte> img_point = new Image<Rgb, byte>((Bitmap)form.pictureBox1.Image);
                    float r = Convert.ToSingle(realy / (down_scale - 1));
                    float c = Convert.ToSingle(realx / (down_scale - 1));
                    CvInvoke.Circle(img_point, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 12, new Rgb(formgcp.img_select_color).MCvScalar, 2);
                    form.pictureBox1.Image = img_point.Bitmap;
                    form.textBox2.Text = realy.ToString();
                    form.textBox4.Text = realx.ToString();
                }
            }

            //
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        private void ImageBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            cnt_zoom = cnt_zoom + 1;
            if (cnt_zoom == 1)
            {
                if (e.Delta > 0) zoom_out(new Point(e.X, e.Y));
                if (e.Delta < 0) zoom_in(new Point(e.X, e.Y));
            }
        }
        private void ımageBox1_MouseLeave(object sender, EventArgs e)
        {
            ımageBox1.Refresh();
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        private void ımageBox1_OnZoomScaleChange(object sender, EventArgs e)
        {
            toolStripLabel3.Text = zoom.ToString();
            follow_window();
        }
        private void HorizontalScrollBar_ValueChanged(object sender, System.EventArgs e)
        {
            follow_window();
        }
        private void VerticalScrollBar_ValueChanged(object sender, System.EventArgs e)
        {
            follow_window();
        }
        private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if ((e.Type == ScrollEventType.LargeIncrement) || (e.Type == ScrollEventType.SmallIncrement)
                || (e.Type == ScrollEventType.ThumbPosition) || (e.Type == ScrollEventType.ThumbTrack))
            {
                if (e.NewValue == ımageBox1.VerticalScrollBar.Maximum -
                         ımageBox1.VerticalScrollBar.LargeChange + 1) e.NewValue = ımageBox1.VerticalScrollBar.Maximum;
            }
        }
        private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if ((e.Type == ScrollEventType.LargeIncrement) || (e.Type == ScrollEventType.SmallIncrement)
                || (e.Type == ScrollEventType.ThumbPosition) || (e.Type == ScrollEventType.ThumbTrack))
            {
                if (e.NewValue == ımageBox1.HorizontalScrollBar.Maximum -
                    ımageBox1.HorizontalScrollBar.LargeChange + 1) e.NewValue = ımageBox1.HorizontalScrollBar.Maximum;
            }

        }
        //
        // Treeview events
        //
        public void Image_ID_change(int img_ID) // an algorithm for image chanege
        {  
            processbarform pform = new processbarform();
            pform.Show();
            gcp_count = gcp_collected[img_ID].Rows.Count;
            icp_count = icp_collected[img_ID].Rows.Count;
            Image_ID = img_ID;
            Visual v = new Visual();
            if (Imagelist[img_ID] == "empty")
                v.open_Image(treeView1.Nodes[Project_ID].Nodes[Image_ID].Text.Substring(7), true, false);
            else
                v.open_Image(Imagelist[Image_ID], false, false);
            
            pform.Close();
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            e.Node.SelectedImageIndex = e.Node.ImageIndex;
        }
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 1)
                if (Image_ID != treeView1.SelectedNode.Index)
                    Image_ID_change(treeView1.SelectedNode.Index);

            if (e.Node.Level == 2)
                if (Image_ID != treeView1.SelectedNode.Parent.Index)
                    Image_ID_change(treeView1.SelectedNode.Parent.Index);

            if (e.Node.Level == 3)
            {
                Form frm = new Form();
                Image<Gray, Byte> gray = new Image<Gray, Byte>(main_width[Form1.Image_ID], main_height[Form1.Image_ID]);
                ImageViewer ımgview = new ImageViewer(); // invisible
                ımgview.StartPosition = FormStartPosition.CenterScreen;
                ımgview.WindowState = FormWindowState.Maximized;
                if ((Image_ID != treeView1.SelectedNode.Parent.Parent.Index))
                    Image_ID_change(treeView1.SelectedNode.Parent.Parent.Index);

                // This is matrix of ımage for Red, Green and Blue /////
                int i;
                for (i = 0; i < e.Node.Parent.Nodes.Count; i++)
                {
                    if (e.Node.Index == i)
                    {
                        gray = Main_Bants[i];
                        ımgview.Text = e.Node.Text;
                    }
                }
                ımgview.Image = gray;
                ımgview.TopMost = true;
                ımgview.Show();
                ımgview.Activate();
                ımageBox1.Refresh();
                ımageBox2.Refresh();
            }

            if (e.Node.Level == 4)
            {
                Form frm = new Form();
                Image<Gray, Byte> gray = new Image<Gray, Byte>(main_width[Form1.Image_ID], main_height[Form1.Image_ID]);
                ImageViewer ımgview = new ImageViewer(); // invisible
                ımgview.StartPosition = FormStartPosition.CenterScreen;
                ımgview.WindowState = FormWindowState.Maximized;
                if ((Image_ID != treeView1.SelectedNode.Parent.Parent.Parent.Index))
                    Image_ID_change(treeView1.SelectedNode.Parent.Parent.Parent.Index);

                // this is Histogram of ımage for Blue, Red and green ////////////              
                HistogramBox histo = new HistogramBox();
                histo.ClearHistogram();
                int i;
                for (i = 0; i < e.Node.Parent.Parent.Nodes.Count; i++)
                {
                    if (e.Node.Parent.Index == i)
                    {
                        gray = Main_Bants[i];
                        frm.Text = e.Node.Text;
                    }
                }
                histo.GenerateHistograms(gray, 256);
                histo.Dock = DockStyle.Fill;
                histo.Refresh();
                frm.TopMost = true;
                frm.Controls.Add(histo);
                frm.Show();
                ımageBox1.Refresh();
                ımageBox2.Refresh();
            }

        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.Node.Level == 0)
                {
                    İnformationToolStripMenuItem.Text = "Proje information";
                    contextMenuStrip1.Show(MousePosition);
                }
                if (e.Node.Level == 1)
                {
                    if (Imagelist[e.Node.Index] != "empty")
                        İnformationToolStripMenuItem.Text = "Image information(GDAL)";
                    else
                        İnformationToolStripMenuItem.Text = "Image information";
                    contextMenuStrip1.Show(MousePosition);
                }

            }
            if ((Project_ID != -1) && (Image_ID != -1))
            {
                if ((e.Node == treeView1.Nodes[Project_ID].Nodes[Image_ID].Nodes[1]))
                {
                    if (e.Node.SelectedImageIndex == 19)
                    {
                        e.Node.SelectedImageIndex = 20;
                        e.Node.ImageIndex = 20;
                    }
                    else
                    {
                        e.Node.SelectedImageIndex = 19;
                        e.Node.ImageIndex = 19;
                    }
                }
            }
            ımageBox1.Refresh();
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        private void treeView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        //
        //Follow window events
        //
        private void ımageBox2_MouseMove(object sender, MouseEventArgs e)
        {   /////////////// Windows of progrsm synchronous /////////////////
            if (ımageBox2.Image != null)
            {
                if (followclick == 1)
                {
                    double dx, dy;
                    dx = e.X - (ımageBox2.Width / 2);
                    dx = (main_width[Form1.Image_ID] * dx) / ımageBox2.Width;
                    dx = (ımageBox1.Image.Bitmap.Width / 2) + dx;
                    dx = dx - (ımageBox1.Size.Width / 2) / ımageBox1.ZoomScale;
                    if (ımageBox1.HorizontalScrollBar.Maximum < dx) dx = ımageBox1.HorizontalScrollBar.Maximum;
                    if (0 > dx) dx = 0;
                    ımageBox1.HorizontalScrollBar.Value = (int)dx;

                    dy = e.Y - (ımageBox2.Height / 2);
                    dy = (main_height[Form1.Image_ID] * dy) / ımageBox2.Height;
                    dy = (ımageBox1.Image.Bitmap.Height / 2) + dy;
                    dy = dy - (ımageBox1.Size.Height / 2) / ımageBox1.ZoomScale;
                    if (ımageBox1.VerticalScrollBar.Maximum < dy) dy = ımageBox1.VerticalScrollBar.Maximum;
                    if (0 > dy) dy = 0;
                    ımageBox1.VerticalScrollBar.Value = (int)dy;
                    ımageBox1.Refresh();
                    if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
                    if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
                }
            }
        }
        private void ımageBox2_MouseDown(object sender, MouseEventArgs e)
        {
            /////////////// Windows of progrsm synchronous /////////////////
            if (ımageBox2.Image != null)
            {
                followclick = 1;
                double dx, dy;
                dx = e.X - (ımageBox2.Width / 2);
                dx = (main_width[Form1.Image_ID] * dx) / ımageBox2.Width;
                dx = (ımageBox1.Image.Bitmap.Width / 2) + dx;
                dx = dx - (ımageBox1.Size.Width / 2) / ımageBox1.ZoomScale;
                if (ımageBox1.HorizontalScrollBar.Maximum < dx) dx = ımageBox1.HorizontalScrollBar.Maximum;
                if (0 > dx) dx = 0;
                ımageBox1.HorizontalScrollBar.Value = (int)dx;

                dy = e.Y - (ımageBox2.Height / 2);
                dy = (main_height[Form1.Image_ID] * dy) / ımageBox2.Height;
                dy = (ımageBox1.Image.Bitmap.Height / 2) + dy;
                dy = dy - (ımageBox1.Size.Height / 2) / ımageBox1.ZoomScale;
                if (ımageBox1.VerticalScrollBar.Maximum < dy) dy = ımageBox1.VerticalScrollBar.Maximum;
                if (0 > dy) dy = 0;
                ımageBox1.VerticalScrollBar.Value = (int)dy;
                ımageBox1.Refresh();
            }
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        private void ımageBox2_MouseUp(object sender, MouseEventArgs e)
        {
            followclick = 0;
        }
        //
        // Up-Down and left-right fonction
        //
        void size_change_left(int dx)
        {
            ımageBox1.Width = ımageBox1.Width - dx;
            ımageBox1.Left = ımageBox1.Left + dx;
            treeView1.Width = treeView1.Width + dx;
            ımageBox2.Width = ımageBox2.Width + dx;
            panel1.Width = panel1.Width + dx;
            panel2.Left = panel2.Left + dx;
        }
        void size_change_up(int dy)
        {
            ımageBox2.Height = ımageBox2.Height - dy;
            ımageBox2.Top = ımageBox2.Top + dy;
            treeView1.Height = treeView1.Height + dy;
            panel1.Top = panel1.Top + dy;
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            UpDown = 1;
            leftright = 1;
            updowndy = e.Y;
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            UpDown = 0;
            leftright = 0;
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            int dy;
            dy = e.Y - updowndy;
            if (UpDown == 1)
            {
                size_change_up(dy);
                size_change_left(-dy);
            }
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            updowndy = 1;
            leftright = 1;
            leftrightdx = e.X;
        }
        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            leftright = 0;
            UpDown = 0;
        }
        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            int dx;
            dx = e.X - leftrightdx;
            if (leftright == 1)
            {
                size_change_left(dx);
                size_change_up(-dx);
            }
        }
        //
        // ToolStrip events
        //
        private void toolStrip1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        private void toolStrip2_MouseMove(object sender, MouseEventArgs e)
        {
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Visual v = new Visual();
            v.New_prj();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Project file (*.gpr)|*.gpr*";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                Visual v = new Visual();
                v.load_prj(openFileDialog1.FileName);
                toolStripButton7.Enabled = true;
            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Visual v = new Visual();
            message_form m = new message_form(message_form.status_value.save);
            m.Show();
            v.Save_prj();
        }
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            treeView1.Nodes[Project_ID].Nodes[Image_ID].BackColor = Color.Red;
            string info;
            info = MessageBox.Show("Delete this image from the project.",
                "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information).ToString();
            if (info == "Yes")
            {
                Visual V = new Visual();
                V.delete_img();
            }
            else
                treeView1.Nodes[Project_ID].Nodes[Image_ID].BackColor = treeView1.BackColor;
        }
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Image files |*.*";
            openFileDialog1.ShowDialog();

            processbarform pform = new processbarform();
            pform.Show();
            checkBox1.Enabled = false;
            trackBar1.Enabled = false;

            Visual v = new Visual();
            v.open_Image(openFileDialog1.FileName, false, true);
            pform.Close();
        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (formgcp.Form_active == false)
            {
                treeView1.Nodes[Project_ID].Nodes[Image_ID].Nodes[1].ImageIndex = 19;
                treeView1.Nodes[Project_ID].Nodes[Image_ID].Nodes[1].SelectedImageIndex = 19;
                formgcp formm = new formgcp();
            
                formm.Show();
            }
        }
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            zoom_out(new Point(ımageBox1.Width / 2, ımageBox1.Height / 2));
        }
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            zoom_in(new Point(ımageBox1.Width / 2, ımageBox1.Height / 2));
        }
        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            zoom = 1;
            ımageBox1.SetZoomScale(1, new Point(0, 0));
            ımageBox1.HorizontalScrollBar.Value = 0;
            ımageBox1.VerticalScrollBar.Value = 0;
            ımageBox1.Refresh();
            ımageBox2.Refresh();
        }
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            ımageBox1.HorizontalScrollBar.Value = 0;
            ımageBox1.Refresh();
            ımageBox2.Refresh();
        }
        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            ımageBox1.HorizontalScrollBar.Value = ımageBox1.HorizontalScrollBar.Maximum;
            ımageBox1.Refresh();
            ımageBox2.Refresh();
        }
        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            ımageBox1.VerticalScrollBar.Value = 0;
            ımageBox1.Refresh();
            ımageBox2.Refresh();
        }
        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            ımageBox1.VerticalScrollBar.Value = ımageBox1.VerticalScrollBar.Maximum;
            ımageBox1.Refresh();
            ımageBox2.Refresh();
        }
        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            double zoom_rate_Width = Math.Round(Convert.ToDouble(ımageBox1.Image.Size.Height) / Convert.ToDouble(ımageBox1.Size.Width), 3);
            double zoom_rate_Height = Math.Round(Convert.ToDouble(ımageBox1.Image.Size.Height) / Convert.ToDouble(ımageBox1.Size.Height), 3);
            if (zoom_rate_Width >= zoom_rate_Height) zoom = zoom_rate_Width;
            else zoom = zoom_rate_Height;
            if (zoom < 1)
            {
                if (zoom_rate_Width <= zoom_rate_Height) zoom = zoom_rate_Width;
                else zoom = zoom_rate_Height;
            }
            ımageBox1.SetZoomScale(1 / zoom, new Point(ımageBox1.Size.Width / 2, ımageBox1.Size.Height / 2));
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Empty_image e_form = new Empty_image();
            e_form.ShowDialog();
        }
        //
        //Extra Events: timer, panel2, Menu and contrast panel
        //

        private void imageToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (Project_ID != -1)
            {
                OpenimageToolStripMenuItem.Enabled = true;
                emptyToolStripMenuItem.Enabled = true;
            }
            else
            {
                OpenimageToolStripMenuItem.Enabled = false;
                emptyToolStripMenuItem.Enabled = false;
            }
            if (Image_ID != -1)  contrastToolStripMenuItem1.Enabled = true;    
            else contrastToolStripMenuItem1.Enabled = false;
        }
        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (Project_ID == -1)
            {
                closeProjectToolStripMenuItem.Enabled = false;
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem1.Enabled = false;
                saveExportToolStripMenuItem.Enabled = false;
                PrintMapToolStripMenuItem.Enabled = false;
            }
            else
            {
                closeProjectToolStripMenuItem.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem1.Enabled = true;
                saveExportToolStripMenuItem.Enabled = true;
                PrintMapToolStripMenuItem.Enabled = true;
            }
        }
        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visual v = new Visual();
            int a = v.close_All();
            if (a == 1)
            {
                Project_ID = -1;
                this.Close();
            }
        }
        private void İnformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Level == 1)
            {
                if (Imagelist[treeView1.SelectedNode.Index] != "empty")
                {
             
                    message_form m = new message_form(message_form.status_value.info_img,
                        treeView1.SelectedNode.Index);
                    m.Show();
                }
                else
                {
                    message_form m = new message_form(message_form.status_value.info_emp,
                        treeView1.SelectedNode.Index);
                    m.Show();
                }
            }
            if (treeView1.SelectedNode.Level == 0)
            {
                message_form m = new message_form(message_form.status_value.info_proje,
                        treeView1.SelectedNode.Index);
                m.Show();
            }
        }

        private void generalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Under construction", "Information" , MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void emptyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Empty_image e_form = new Empty_image();
            e_form.ShowDialog();
        }

        private void contrastToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            panel3.Visible = true;
            Form form = new Form();
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.Controls.Add(panel3);
            form.Text = "Contrast";
            form.StartPosition = FormStartPosition.CenterScreen;
            panel3.Left = 0;
            panel3.Top = 0;
            form.Height = panel3.Height + 40;
            form.Width = panel3.Width + 20;
            form.Icon = this.Icon;
            form.ShowInTaskbar = false;
            form.ShowDialog();
            panel3.Visible = false;
            this.Controls.Add(panel3);
            panel3.BringToFront();
            panel3.Top = treeView1.Top;
        }
        private void collectManuelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (formgcp.Form_active == false)
            {
                formgcp formm = new formgcp();
                formm.Show();
            }
        }
        private void menuStrip1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        private void gCPToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (Image_ID != -1)
            {
                collectManuelToolStripMenuItem.Enabled = true;
                SaveImageToolStripMenuItem.Enabled = true;
            }
            else
            {
                collectManuelToolStripMenuItem.Enabled = false;
                SaveImageToolStripMenuItem.Enabled = false;
            }
        }
        // (Start) for Contrast Panel 
        static int open_close_panel = 0;
        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            if (open_close_panel == 0)
            {
                open_close_panel = 1;
                panel3.Left = this.PointToScreen(toolStripButton16.Bounds.Location).X - 170;
                panel3.Top = toolStrip2.Top + toolStrip2.Height;
                panel3.Height = 0;
                panel3.Visible = true;
                timer1.Enabled = true;
            }
            else
            {
                open_close_panel = 0;
                timer1.Enabled = true;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (open_close_panel == 1)
            {
                if (panel3.Height < 100)
                {
                    panel3.Refresh();
                    Graphics gpanel = panel3.CreateGraphics();
                    gpanel.DrawRectangle(new Pen(Color.Black, 2.0f), new Rectangle(0, 0, panel3.Width, panel3.Height));
                    panel3.Height = panel3.Height + 20;
                }
                else
                {
                    panel3.Refresh();
                    Graphics gpanel = panel3.CreateGraphics();
                    gpanel.DrawRectangle(new Pen(Color.Black, 2.0f), new Rectangle(0, 0, panel3.Width, panel3.Height));
                    timer1.Enabled = false;
                }
            }
            else
            {
                if (panel3.Height > 20)
                {
                    panel3.Refresh();
                    Graphics gpanel = panel3.CreateGraphics();
                    gpanel.DrawRectangle(new Pen(Color.Black, 2.0f), new Rectangle(0, 0, panel3.Width, panel3.Height));
                    panel3.Height = panel3.Height - 20;
                }
                else
                {
                    panel3.Visible = false;
                    panel3.Height = 100;
                    timer1.Enabled = false;
                }
            }
        }
        private void ımageBox1_MouseHover(object sender, EventArgs e)
        {
            if (open_close_panel == 1)
            {
                open_close_panel = 0;
                timer1.Enabled = true;
            }
        }
        private void treeView1_MouseHover(object sender, EventArgs e)
        {
            if (open_close_panel == 1)
            {
                open_close_panel = 0;
                timer1.Enabled = true;
            }
        }
        private void menuStrip1_MouseHover(object sender, EventArgs e)
        {
            if (open_close_panel == 1)
            {
                open_close_panel = 0;
                timer1.Enabled = true;
            }
        }
        private void panel2_MouseHover(object sender, EventArgs e)
        {
            if (open_close_panel == 1)
            {
                open_close_panel = 0;
                timer1.Enabled = true;
            }
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (open_close_panel == 1)
            {
                open_close_panel = 0;
                timer1.Enabled = true;
            }
        }
        // (End) for Contrast Panel 
        //(start) Contrast Events
        static double contrastValue = 1.00; // values about contrast
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                Image<Rgb, byte> contrast_img = new Image<Rgb, byte>(ımageBox1.Image.Bitmap);
                contrast_img._EqualizeHist();
                ımageBox1.Image = contrast_img;
                decrease(contrast_img.Bitmap);
            }
            else
            {
                if (Main_Bants.Length >= 3)
                {
                    Image<Gray, byte>[] color_bants = { Main_Bants[0], Main_Bants[1], Main_Bants[2] };
                    Image<Rgb, byte> main_img = new Image<Rgb, byte>(color_bants);
                    main_img = main_img.ConvertScale<byte>(contrastValue, 0);
                    ımageBox1.Image = main_img;
                    decrease(main_img.Bitmap);
                }
                else
                {
                    Image<Gray, byte> contrast_gray = Main_Bants[0];
                    ımageBox1.Image = contrast_gray.ConvertScale<byte>(contrastValue, 0);
                    decrease(contrast_gray.Bitmap);
                }
            }
            follow_window();
            ımageBox1.Refresh();
            ımageBox2.Refresh();
        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (trackBar1.Value == 0) contrastValue = 0.125;
            if (trackBar1.Value == 1) contrastValue = 0.25;
            if (trackBar1.Value == 2) contrastValue = 0.50;
            if (trackBar1.Value >= 3) contrastValue = ((trackBar1.Value - 3.00d) * 0.50d) + 1.00d;
            label1.Text = contrastValue.ToString();
            if (contrastValue < 1) label2.Text = "Decrease";
            if (contrastValue > 1) label2.Text = "Increase";
            if (contrastValue == 1) label2.Text = "Defualt";
            if (Main_Bants.Length >= 3)
            {
                Image<Gray, byte>[] color_bants = { Main_Bants[0], Main_Bants[1], Main_Bants[2] };
                Image<Rgb, byte> main_img = new Image<Rgb, byte>(color_bants);
                Image<Rgb, byte> contrast_change = main_img.ConvertScale<byte>(contrastValue, 0);
                if (checkBox1.Checked == true) contrast_change._EqualizeHist();
                ımageBox1.Image = contrast_change;
                decrease(contrast_change.Bitmap);
            }
            else
            {
                Image<Gray, byte> contrast_gray = Main_Bants[0].ConvertScale<byte>(contrastValue, 0);
                if (checkBox1.Checked == true) contrast_gray._EqualizeHist();
                ımageBox1.Image = contrast_gray;
                decrease(contrast_gray.Bitmap);
            }
            follow_window();
            ımageBox1.Refresh();
            ımageBox2.Refresh();
            if (Image_ID != -1) GCP_draw(gcp_collected[Image_ID]);
            if (Image_ID != -1) ICP_draw(icp_collected[Image_ID]);
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                checkBox1.Checked = false;
                trackBar1.Enabled = false;
                checkBox1.Enabled = false;
                trackBar1.Value = 3;
            }
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                checkBox1.Checked = false;
                trackBar1.Enabled = false;
                checkBox1.Enabled = false;
                double max_avenge = 1;
                if (Main_Bants.Length >= 3)
                {
                    double[] avange = new double[3];
                    avange[0] = Main_Bants[0].GetAverage().Intensity;
                    avange[1] = Main_Bants[1].GetAverage().Intensity;
                    avange[2] = Main_Bants[2].GetAverage().Intensity;
                    max_avenge = avange.Max();
                }
                if (Main_Bants.Length < 3)
                    max_avenge = Main_Bants[0].GetAverage().Intensity;
                double range = 85.3 / max_avenge;
                if ((range >= 1.50) && (range < 2.0)) trackBar1.Value = 4;
                if ((range >= 2.00) && (range < 2.5)) trackBar1.Value = 5;
                if ((range >= 2.50) && (range < 3.0)) trackBar1.Value = 6;
                if ((range >= 3.00) && (range < 3.5)) trackBar1.Value = 7;
                if ((range >= 3.50) && (range < 4.0)) trackBar1.Value = 8;
                if ((range >= 4.00) && (range < 4.5)) trackBar1.Value = 9;
                if (range >= 4.50) trackBar1.Value = 10;
            }
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                trackBar1.Enabled = true;
                checkBox1.Enabled = true;
            }
        }
        //(end) Contrast Events
    }
}
 