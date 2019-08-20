using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.IO;

namespace GeoEtrim
{
    public partial class GeoTransform : Form
    {
        public GeoTransform()
        {
            InitializeComponent();
        }
        //
        // Variables
        //
        static DataTable point_total_table = new DataTable();
        static DataTable point_check_table = new DataTable();
        static double mr, mc,vrow_total,vcol_total,col_max,col_min,row_max,row_min;
        static double[] mo = new double[1];
        static int iteration = 0;
        static bool new_data_active = true;
        static bool data_clear_active = false;

        public static bool form_active = false;
        List<int> par_valid_rslt_size = new List<int>();
        //  view about veriables
        public static Color vektor_back, vektor_canves, vektor_color, wind_back, wind_canves, wind_color, date_back;
        public static string title_font_name, values_font_name;
        public static int title_font_size, values_font_size, decimal_points;

        //
        // Visual interface functions
        //
        public void image_dzyn()
        {

            pictureBox1.BackColor = vektor_back;
            pictureBox2.BackColor = wind_back;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(title_font_name, title_font_size);
            dataGridView1.DefaultCellStyle.Font = new Font(values_font_name, values_font_size);
            dataGridView1.DefaultCellStyle.Format = "f" + decimal_points.ToString();

            int i;
            Bitmap bmp = new Bitmap(image_mini.Width + 100, image_mini.Height + 100);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(new SolidBrush(vektor_canves), 0, 0, bmp.Width, bmp.Height);
            if (checkBox2.Checked == true)
                g.DrawImage(image_mini, new Point(50, 50));
            else
                g.DrawRectangle(new Pen(Color.Black, 2), 50, 50, image_mini.Width, image_mini.Height);

            Image<Rgb, byte> img = new Image<Rgb, byte>(bmp.Width, bmp.Height, new Rgb(wind_canves));
            float r, c, r2, c2, vr, vc;
            g.DrawLine(new Pen(Color.Black, Convert.ToSingle(numericUpDown4.Value)), 50, 25,
                50 + Convert.ToSingle(numericUpDown3.Value) * Convert.ToSingle(numericUpDown8.Value)
                , 25);
            string s = numericUpDown8.Value.ToString() + " Pixel";
            g.DrawString(s, new Font("Consolas", 9), new SolidBrush(Color.Black),
                60 + Convert.ToSingle(numericUpDown3.Value) * Convert.ToSingle(numericUpDown8.Value), 25);
            float max_row = 1, max_col = 1;
            float max_row_2 = 1, max_col_2 = 1;
            if (point_check_table.Rows.Count != 0)
            {
                if (scale_control == 1)
                {
                    float[] array_points = new float[point_check_table.Rows.Count];
                    for (i = 0; i < point_check_table.Rows.Count; i++)
                        array_points[i] = Convert.ToSingle(point_check_table.Rows[i][2].ToString());
                    max_row = 1000 / array_points.Max();
                    for (i = 0; i < point_check_table.Rows.Count; i++)
                        array_points[i] = Convert.ToSingle(point_check_table.Rows[i][3].ToString());
                    max_col = 1000 / array_points.Max();
                }
            }

            if (point_check_table.Rows.Count != 0)
            {
                if (point_check_table.Rows[1][21].ToString() != "")
                {
                    if (scale_control == 1)
                    {
                        float[] array_points = new float[point_check_table.Rows.Count];
                        for (i = 0; i < point_check_table.Rows.Count; i++)
                            array_points[i] = Convert.ToSingle(point_check_table.Rows[i][21].ToString());
                        max_row_2 = 1000 / array_points.Max();
                        for (i = 0; i < point_check_table.Rows.Count; i++)
                            array_points[i] = Convert.ToSingle(point_check_table.Rows[i][22].ToString());
                        max_col_2 = 1000 / array_points.Max();
                    }
                }
            }
            for (i = 0; i < point_check_table.Rows.Count; i++)
            {
              
                r = 50 + Convert.ToSingle(point_check_table.Rows[i][2].ToString())* max_row / (down_Scale - 1);
                c = 50 + Convert.ToSingle(point_check_table.Rows[i][3].ToString())* max_col / (down_Scale - 1);
                float scl_line = Convert.ToSingle(numericUpDown7.Value);
                g.FillEllipse(new SolidBrush(vektor_color), c - (scl_line / 2), r - (scl_line / 2), scl_line, scl_line);
                if (point_check_table.Rows[i][21].ToString() != "")
                {
                
                    r2 = (50 + Convert.ToSingle(point_check_table.Rows[i][21].ToString())*max_row_2 / (down_Scale - 1));
                    c2 = (50 + Convert.ToSingle(point_check_table.Rows[i][22].ToString())*max_col_2 / (down_Scale - 1));
                    vr = (r2 - r) * Convert.ToSingle(numericUpDown3.Value); // for vector scale
                    vc = (c2 - c) * Convert.ToSingle(numericUpDown3.Value); // for vector scale
                    r2 = vr + r;
                    c2 = vc + c;
                    g.DrawLine(new Pen(vektor_color, Convert.ToSingle(numericUpDown4.Value)),
                        c, r, c2, r2);

                    vr = (vr / Convert.ToSingle(numericUpDown3.Value)) * Convert.ToSingle(numericUpDown5.Value);
                    vc = (vc / Convert.ToSingle(numericUpDown3.Value)) * Convert.ToSingle(numericUpDown5.Value);
                    CvInvoke.Line(img, new Point(Convert.ToInt32(bmp.Width / 2), Convert.ToInt32(bmp.Height / 2)),
                        new Point(Convert.ToInt32((bmp.Width / 2) + vc), Convert.ToInt32((bmp.Height / 2) + vr)),
                        new Rgb(wind_color).MCvScalar, Convert.ToInt32(numericUpDown6.Value));
                }
            }
            pictureBox1.Image = bmp;
            pictureBox2.Image = img.Bitmap;

        }
        public void view_save()
        {
            StreamWriter wr = File.CreateText(Application.StartupPath + "/GeoTransform_view.txt");
            wr.WriteLine(vektor_back.Name);
            wr.WriteLine(vektor_canves.Name);
            wr.WriteLine(vektor_color.Name);
            wr.WriteLine(wind_back.Name);
            wr.WriteLine(wind_canves.Name);
            wr.WriteLine(wind_color.Name);
            wr.WriteLine(date_back.Name);
            wr.WriteLine(title_font_name);
            wr.WriteLine(values_font_name);
            wr.WriteLine(title_font_size);
            wr.WriteLine(values_font_size);
            wr.WriteLine(decimal_points);
            wr.Close();
        }
        public void view_here()
        {
            int i;
            if (File.Exists(Application.StartupPath + "/GeoTransform_view.txt") == true)
            {
                try
                {
                    StreamReader view_m = new StreamReader(Application.StartupPath + "/GeoTransform_view.txt");
                    string[] line = new string[12];
                    for (i = 0; i < 12; i++) line[i] = view_m.ReadLine();
                    view_m.Close();
                    vektor_back = Color.FromName(line[0]);
                    vektor_canves = Color.FromName(line[1]);
                    vektor_color = Color.FromName(line[2]);
                    wind_back = Color.FromName(line[3]);
                    wind_canves = Color.FromName(line[4]);
                    wind_color = Color.FromName(line[5]);
                    date_back = Color.FromName(line[6]);
                    title_font_name = line[7];
                    values_font_name = line[8];
                    title_font_size = Convert.ToInt32(line[9]);
                    values_font_size = Convert.ToInt32(line[10]);
                    decimal_points = Convert.ToInt32(line[11]);
                }
                catch
                {
                    view_defualt();
                }
            }
            else view_defualt();
        }
        public void view_defualt()
        {
            vektor_back = Color.FromKnownColor(KnownColor.ControlLight);
            vektor_canves = Color.White;
            vektor_color = Color.Red;
            wind_back= Color.FromKnownColor(KnownColor.ControlLight);
            wind_canves = Color.White;
            wind_color = Color.BlueViolet;
            date_back = Color.FromKnownColor(KnownColor.AppWorkspace);
            title_font_name = "Consolas";
            values_font_name = "Consolas";
            title_font_size = 9;
            values_font_size = 8;
            decimal_points = 3;
    }
        /// <summary>
        /// For total data
        /// </summary>
        /// <param name="image_id">Form1 of image is id </param>
        void data_total(int image_id)
        {

            new_data_active = true;
            point_total_table.Rows.Clear();
            dataGridView1.Rows.Clear();
            int  i;
        
                for (i = 0; i < Form1.gcp_collected[image_id].Rows.Count; i++)
                {
               
                    point_total_table.Rows.Add(Form1.gcp_collected[image_id].Rows[i].ItemArray);
                    point_total_table.Rows[i][12] = true;
                }
                for (i = 0; i < Form1.icp_collected[image_id].Rows.Count; i++)
                {
                    point_total_table.Rows.Add(Form1.icp_collected[image_id].Rows[i].ItemArray);
                    point_total_table.Rows[i][12] = false;
                }
           
                for (i = 0; i < point_total_table.Rows.Count; i++)
                {
                    dataGridView1.Rows.Add();
                    if ((Boolean)point_total_table.Rows[i][12] == true)
                        dataGridView1.Rows[i].Cells[0].Value = true;
                    else dataGridView1.Rows[i].Cells[0].Value = false;
                    dataGridView1.Rows[i].Cells[1].Value = point_total_table.Rows[i][0];
                    dataGridView1.Rows[i].Cells[2].Value = point_total_table.Rows[i][1];
                    dataGridView1.Rows[i].Cells[4].Value = point_total_table.Rows[i][2];
                    dataGridView1.Rows[i].Cells[6].Value = point_total_table.Rows[i][3];
                    dataGridView1.Rows[i].Cells[8].Value = point_total_table.Rows[i][4];
                    dataGridView1.Rows[i].Cells[9].Value = point_total_table.Rows[i][5];
                    dataGridView1.Rows[i].Cells[10].Value = point_total_table.Rows[i][6];
                }
                new_data_active = false;
                check_dzyn();
            
        }
        void check_dzyn()
        {
            int i, j;
            for (i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true)
                    point_total_table.Rows[i][12] = true;
                else point_total_table.Rows[i][12] = false;
            }
            point_check_table.Rows.Clear();
            j = 0;
            for (i = 0; i < point_total_table.Rows.Count; i++)
            {
                if ((Boolean)point_total_table.Rows[i][12]== true)
                {
                    point_check_table.Rows.Add();
                    point_check_table.Rows[j][0] = point_total_table.Rows[i][0];
                    point_check_table.Rows[j][1] = point_total_table.Rows[i][1];
                    point_check_table.Rows[j][2] = point_total_table.Rows[i][2];
                    point_check_table.Rows[j][3] = point_total_table.Rows[i][3];
                    point_check_table.Rows[j][4] = point_total_table.Rows[i][4];
                    point_check_table.Rows[j][5] = point_total_table.Rows[i][5];
                    point_check_table.Rows[j][6] = point_total_table.Rows[i][6];
                    point_check_table.Rows[j][7] = point_total_table.Rows[i][7];
                    point_check_table.Rows[j][8] = point_total_table.Rows[i][8];
                    point_check_table.Rows[j][9] = point_total_table.Rows[i][9];
                    point_check_table.Rows[j][10] = point_total_table.Rows[i][10];
                    point_check_table.Rows[j][11] = point_total_table.Rows[i][11];
                    j = j + 1;
                }
            }

            // Normalize
            double[] columns_array = new double[point_check_table.Rows.Count];
            if (point_check_table.Rows.Count > 0)
            {
                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < point_check_table.Rows.Count; j++)
                        columns_array[j] = Convert.ToDouble(point_check_table.Rows[j][i + 2].ToString());
                    if (i == 0)
                    {
                        row_max = columns_array.Max();
                        row_min = columns_array.Min();
                    }
                    if (i == 1)
                    {
                        col_max = columns_array.Max();
                        col_min = columns_array.Min();
                    }
                    columns_array = Normalize(columns_array, columns_array.Max(), columns_array.Min());
                    for (j = 0; j < point_check_table.Rows.Count; j++)
                        point_check_table.Rows[j][i + 12] = columns_array[j];
                }
            }
            mr = 0;
            mc = 0;
            mo[mo.Length - 1] = 0;
            vrow_total = 0;
            vcol_total = 0;

            label15.Text = ((char)177).ToString() + mr.ToString();
            label16.Text = ((char)177).ToString() + mc.ToString();
            label17.Text = ((char)177).ToString() + mo[mo.Length - 1].ToString();
            label18.Text = vrow_total.ToString();
            label19.Text = vcol_total.ToString();
            label22.Text = degreeoffreedom().ToString();
            image_dzyn();
        }
        void data_clear()
        {
            if (data_clear_active == true)
            {
                int i;
                for (i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Cells[3].Value = "";
                    dataGridView1.Rows[i].Cells[5].Value = "";
                    dataGridView1.Rows[i].Cells[7].Value = "";
                }
                data_clear_active = false;
            }

        }
        int degreeoffreedom() //For Degree of freedom
        {
            int result = 0;
            int u = 0;
            if (comboBox3.SelectedIndex == 0) u = 4;// Similarity (Helmert)
            if (comboBox3.SelectedIndex == 1) u = 6;// Affine
            if (comboBox3.SelectedIndex == 2)// Polinomial
            {
                if (comboBox5.SelectedIndex == 0) u = 6;// first degree (Affine)
                if (comboBox5.SelectedIndex == 1) u = 12;// second degree (bilinear)
                if (comboBox5.SelectedIndex == 2) u = 20;// third degree (cubic)
                if (comboBox5.SelectedIndex == 3) u = 30;// fourth degree (quartic)
                if (comboBox5.SelectedIndex == 4) u = 42;//fifth degree (quintic)
            }
            if (comboBox3.SelectedIndex == 3)// Affine projection
            {
                if (comboBox5.SelectedIndex == 0) u = 8;
                if (comboBox5.SelectedIndex == 1) u = 14;
                if (comboBox5.SelectedIndex == 2) u = 12;
            }
            if (comboBox3.SelectedIndex == 4) u = 8;// Projective
            if (comboBox3.SelectedIndex == 5) u = 11;// DLT
            if (comboBox3.SelectedIndex == 6)// RFM
            {
                if (comboBox5.SelectedIndex == 0) u = 14;// first degree
                if (comboBox5.SelectedIndex == 1) u = 38;// second degree
                if (comboBox5.SelectedIndex == 2) u = 78;// third degree
            }
            result =2 * point_check_table.Rows.Count - u;
            return result;
        }
        //
        // Processing functions
        //

        bool[] blunder(double apriori_sigma, double mo, double mon, Matrix<double> P,
            Matrix<double> A, Matrix<double> v, Matrix<double> Qxx)
        {

            bool[] blunder_result = new bool[A.Rows];
            double scale = mo / mon;
            double sigma_user = apriori_sigma / scale;
            Matrix<double> Qll = new Matrix<double>(P.Rows, P.Cols);
            Matrix<double> Qvv = new Matrix<double>(P.Rows, P.Cols);
            Matrix<double> T_normal = new Matrix<double>(P.Rows, 1);
            CvInvoke.Invert(P, Qll, Emgu.CV.CvEnum.DecompMethod.Cholesky);
            Qvv = Qll - A * Qxx * A.Transpose();
            int i;
            for (i = 0; i < blunder_result.Length; i++)
                blunder_result[i] = false;
            //  Normal distribution
            if (comboBox1.SelectedIndex == 0)
            {
                for (i = 0; i < point_check_table.Rows.Count; i++)
                    T_normal[i, 0] = Math.Abs(v[i, 0]) / (sigma_user * Math.Sqrt(Qvv[i, i]));
                double min_Tn, max_Tn;
                Point min_Tn_line, max_Tn_line;
                T_normal.MinMax(out min_Tn, out max_Tn, out min_Tn_line, out max_Tn_line);
                if (max_Tn > sigma_user * 3)
                {
                    if (max_Tn_line.Y % 2 == 0)
                    {
                        blunder_result[max_Tn_line.Y] = true;
                        blunder_result[max_Tn_line.Y + 1] = true;
                    }
                    else
                    {
                        blunder_result[max_Tn_line.Y] = true;
                        blunder_result[max_Tn_line.Y - 1] = true;
                    }
                }
               
            }

            // T-student distribution
            if (comboBox1.SelectedIndex == 1)
            {
                double alfa = 0.05 / A.Rows;
                if (alfa <= 0.001) alfa = 0.001;
                double T_dist =  alglib.invstudenttdistribution(degreeoffreedom() - 1,(1-alfa/2));
                double[] So = new double[A.Rows];
                double[] T_student = new double[A.Rows];
                double vTv = (v.Transpose() * v)[0, 0];
                for (i = 0; i < A.Rows; i++)
                {
                    So[i] = Math.Sqrt((vTv - (v[i, 0] * v[i, 0] / Qvv[i, i])) / (degreeoffreedom() - 1));
                    T_student[i] = Math.Abs(v[i, 0]) / (So[i] * Math.Sqrt(Qvv[i, i]));

                    if (T_student[i] <= T_dist) blunder_result[i] = false;
                    else blunder_result[i] = true;
                }
            }
            // Teu distribution
            // Coordinate pair
            return blunder_result;
        }
        bool[] par_valid(double mon, Matrix<double> A, Matrix<double> dx, Matrix<double> Qxx)
        {
            bool[] par_valid = new bool[dx.Rows];
            int i;
            double[] m = new double[Qxx.Rows];
            double[] t = new double[dx.Rows];
            for (i = 0; i < Qxx.Rows; i++)
                m[i] = mon * Math.Sqrt(Qxx[i, i]);


            double T = alglib.invstudenttdistribution(degreeoffreedom(), 0.995);

            for (i = 0; i < dx.Rows; i++)
            {
                t[i] = Math.Abs(dx[i, 0]) / m[i];
                if (t[i] <= T)
                    par_valid[i] = false;
                else
                    par_valid[i] = true;
            }
            return par_valid;
        }
        double[] Normalize(double[] ref_array, double max_value, double min_value)
        {
            double[] new_matrix = new double[ref_array.Length];
            int i;
            for (i = 0; i < ref_array.Length; i++)        
                new_matrix[i] =  (2*ref_array[i] - max_value - min_value) / (max_value - min_value) ;
            return new_matrix;            
        }
        double[] Back_Normalize(double[] ref_array, double max_value, double min_value)
        {
            double[] new_matrix = new double[ref_array.Length];
            int i;
            for (i = 0; i < ref_array.Length; i++)
                new_matrix[i] = (ref_array[i]*(max_value - min_value) + max_value + min_value)/2;
            return new_matrix;
        }
        void XYZ(out double[] X, out double[] Y, out double[] Z, out double[] row, out double[] col)
        {
            int i;
            X = new double[point_check_table.Rows.Count];
            Y = new double[point_check_table.Rows.Count];
            Z = new double[point_check_table.Rows.Count];
            row = new double[point_check_table.Rows.Count];
            col = new double[point_check_table.Rows.Count];
            if (checkBox1.Checked == true)
            {
                for (i = 0; i < point_check_table.Rows.Count; i++)
                {
                    X[i] = Convert.ToDouble(point_check_table.Rows[i][14]);
                    Y[i] = Convert.ToDouble(point_check_table.Rows[i][15]);
                    Z[i] = Convert.ToDouble(point_check_table.Rows[i][16]);

                    row[i] = Convert.ToDouble(point_check_table.Rows[i][12]);
                    col[i] = Convert.ToDouble(point_check_table.Rows[i][13]);
                }
            }
            else
            {
                for (i = 0; i < point_check_table.Rows.Count; i++)
                {
                    X[i] = Convert.ToDouble(point_check_table.Rows[i][4]);
                    Y[i] = Convert.ToDouble(point_check_table.Rows[i][5]);
                    Z[i] = Convert.ToDouble(point_check_table.Rows[i][6]);

                    row[i] = Convert.ToDouble(point_check_table.Rows[i][2]);
                    col[i] = Convert.ToDouble(point_check_table.Rows[i][3]);
                }
            }
            if (iteration != 0)
            {
                for (i = 0; i < point_check_table.Rows.Count; i++)
                {
                    row[i] = Convert.ToDouble(point_check_table.Rows[i][19]);
                    col[i] = Convert.ToDouble(point_check_table.Rows[i][20]);
                }
            }
        }
        Matrix<double> jacobian()
        {
            // Row, Col, x, y, z
            Matrix<double> A = null;
            int i, j;
            double[] X, Y, Z, row, col;
            XYZ(out X, out Y, out Z, out row, out col);

            // Mat models
            if (comboBox3.SelectedIndex == 0) //Similarity (Helmert)
            {
                A = new Matrix<double>(2 * point_check_table.Rows.Count, 4);
                A.SetZero();
                for (i = 0; i < A.Rows; i += 2)
                {
                    j = i / 2;
                    A[i, 0] = 1.0d;
                    A[i, 1] = X[j];
                    A[i, 2] = -Y[j];
                    // Between zero
                    A[i + 1, 1] = Y[j];
                    A[i + 1, 2] = X[j];
                    A[i + 1, 3] = 1.0d;
                }
            }
            if (comboBox3.SelectedIndex == 1) //Affine
            {
                A = new Matrix<double>(2 * point_check_table.Rows.Count, 6);
                A.SetZero();
                for (i = 0; i < A.Rows; i += 2)
                {
                    j = i / 2;
                    A[i, 0] = 1.0d;
                    A[i, 1] = X[j];
                    A[i, 2] = Y[j];
                    // Between zero
                    A[i + 1, 3] = 1.0d;
                    A[i + 1, 4] = X[j];
                    A[i + 1, 5] = Y[j];
                }
            }
            if (comboBox3.SelectedIndex == 2) //Polinomial
            {
                if (comboBox5.SelectedIndex == 0) // first degree (Affine)
                {
                    A = new Matrix<double>(2 * point_check_table.Rows.Count, 6);
                    A.SetZero();
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        // Between zero
                        A[i + 1, 3] = 1.0d;
                        A[i + 1, 4] = X[j];
                        A[i + 1, 5] = Y[j];
                    }
                }
                if (comboBox5.SelectedIndex == 1)// second degree (bilinear)
                {
                    A = new Matrix<double>(2 * point_check_table.Rows.Count, 12);
                    A.SetZero();
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        A[i, 3] = X[j] * Y[j];
                        A[i, 4] = X[j] * X[j];
                        A[i, 5] = Y[j] * Y[j];
                        // Between zero
                        A[i + 1, 6] = 1.0d;
                        A[i + 1, 7] = X[j];
                        A[i + 1, 8] = Y[j];
                        A[i + 1, 9] = X[j] * Y[j];
                        A[i + 1, 10] = X[j] * X[j];
                        A[i + 1, 11] = Y[j] * Y[j];
                    }
                }
                if (comboBox5.SelectedIndex == 2)// third degree (cubic)
                {
                    A = new Matrix<double>(2 * point_check_table.Rows.Count, 20);
                    A.SetZero();
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        A[i, 3] = X[j] * Y[j];
                        A[i, 4] = X[j] * X[j];
                        A[i, 5] = Y[j] * Y[j];
                        A[i, 6] = X[j] * X[j] * Y[j];
                        A[i, 7] = X[j] * Y[j] * Y[j];
                        A[i, 8] = X[j] * X[j] * X[j];
                        A[i, 9] = Y[j] * Y[j] * Y[j];
                        // Between zero
                        A[i + 1, 10] = 1.0d;
                        A[i + 1, 11] = X[j];
                        A[i + 1, 12] = Y[j];
                        A[i + 1, 13] = X[j] * Y[j];
                        A[i + 1, 14] = X[j] * X[j];
                        A[i + 1, 15] = Y[j] * Y[j];
                        A[i + 1, 16] = X[j] * X[j] * Y[j];
                        A[i + 1, 17] = X[j] * Y[j] * Y[j];
                        A[i + 1, 18] = X[j] * X[j] * X[j];
                        A[i + 1, 19] = Y[j] * Y[j] * Y[j];
                    }
                }
                if (comboBox5.SelectedIndex == 3)
                {
                    A = new Matrix<double>(2 * point_check_table.Rows.Count, 30);// fourth degree (quartic)
                    A.SetZero();
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        A[i, 3] = X[j] * Y[j];
                        A[i, 4] = X[j] * X[j];
                        A[i, 5] = Y[j] * Y[j];
                        A[i, 6] = X[j] * X[j] * Y[j];
                        A[i, 7] = X[j] * Y[j] * Y[j];
                        A[i, 8] = X[j] * X[j] * X[j];
                        A[i, 9] = Y[j] * Y[j] * Y[j];
                        A[i, 10] = Y[j] * Y[j] * Y[j] * X[j];
                        A[i, 11] = X[j] * X[j] * X[j] * Y[j];
                        A[i, 12] = X[j] * X[j] * X[j] * X[j];
                        A[i, 13] = Y[j] * Y[j] * Y[j] * Y[j];
                        A[i, 14] = Y[j] * Y[j] * X[j] * X[j];
                        // Between zero
                        A[i + 1, 15] = 1.0d;
                        A[i + 1, 16] = X[j];
                        A[i + 1, 17] = Y[j];
                        A[i + 1, 18] = X[j] * Y[j];
                        A[i + 1, 19] = X[j] * X[j];
                        A[i + 1, 20] = Y[j] * Y[j];
                        A[i + 1, 21] = X[j] * X[j] * Y[j];
                        A[i + 1, 22] = X[j] * Y[j] * Y[j];
                        A[i + 1, 23] = X[j] * X[j] * X[j];
                        A[i + 1, 24] = Y[j] * Y[j] * Y[j];
                        A[i + 1, 25] = Y[j] * Y[j] * Y[j] * X[j];
                        A[i + 1, 26] = X[j] * X[j] * X[j] * Y[j];
                        A[i + 1, 27] = X[j] * X[j] * X[j] * X[j];
                        A[i + 1, 28] = Y[j] * Y[j] * Y[j] * Y[j];
                        A[i + 1, 29] = Y[j] * Y[j] * X[j] * X[j];
                    }
                }
                if (comboBox5.SelectedIndex == 4)
                {
                    A = new Matrix<double>(2 * point_check_table.Rows.Count, 42);// fifth degree (quintic)
                    A.SetZero();
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        A[i, 3] = X[j] * Y[j];
                        A[i, 4] = X[j] * X[j];
                        A[i, 5] = Y[j] * Y[j];
                        A[i, 6] = X[j] * X[j] * Y[j];
                        A[i, 7] = X[j] * Y[j] * Y[j];
                        A[i, 8] = X[j] * X[j] * X[j];
                        A[i, 9] = Y[j] * Y[j] * Y[j];
                        A[i, 10] = Y[j] * Y[j] * Y[j] * X[j];
                        A[i, 11] = X[j] * X[j] * X[j] * Y[j];
                        A[i, 12] = X[j] * X[j] * X[j] * X[j];
                        A[i, 13] = Y[j] * Y[j] * Y[j] * Y[j];
                        A[i, 14] = Y[j] * Y[j] * X[j] * X[j];
                        A[i, 15] = Y[j] * Y[j] * X[j] * X[j] * X[j];
                        A[i, 16] = Y[j] * Y[j] * Y[j] * X[j] * X[j];
                        A[i, 17] = X[j] * X[j] * X[j] * X[j] * X[j];
                        A[i, 18] = Y[j] * Y[j] * Y[j] * Y[j] * Y[j];
                        A[i, 19] = Y[j] * X[j] * X[j] * X[j] * X[j];
                        A[i, 20] = Y[j] * Y[j] * Y[j] * Y[j] * X[j];
                        // Between zero
                        A[i + 1, 21] = 1.0d;
                        A[i + 1, 22] = X[j];
                        A[i + 1, 23] = Y[j];
                        A[i + 1, 24] = X[j] * Y[j];
                        A[i + 1, 25] = X[j] * X[j];
                        A[i + 1, 26] = Y[j] * Y[j];
                        A[i + 1, 27] = X[j] * X[j] * Y[j];
                        A[i + 1, 28] = X[j] * Y[j] * Y[j];
                        A[i + 1, 29] = X[j] * X[j] * X[j];
                        A[i + 1, 30] = Y[j] * Y[j] * Y[j];
                        A[i + 1, 31] = Y[j] * Y[j] * Y[j] * X[j];
                        A[i + 1, 32] = X[j] * X[j] * X[j] * Y[j];
                        A[i + 1, 33] = X[j] * X[j] * X[j] * X[j];
                        A[i + 1, 34] = Y[j] * Y[j] * Y[j] * Y[j];
                        A[i + 1, 35] = Y[j] * Y[j] * X[j] * X[j];
                        A[i + 1, 36] = Y[j] * Y[j] * X[j] * X[j] * X[j];
                        A[i + 1, 37] = Y[j] * Y[j] * Y[j] * X[j] * X[j];
                        A[i + 1, 38] = X[j] * X[j] * X[j] * X[j] * X[j];
                        A[i + 1, 39] = Y[j] * Y[j] * Y[j] * Y[j] * Y[j];
                        A[i + 1, 40] = Y[j] * X[j] * X[j] * X[j] * X[j];
                        A[i + 1, 41] = Y[j] * Y[j] * Y[j] * Y[j] * X[j];
                    }
                }
            }
            if (comboBox3.SelectedIndex == 3)// Affine projection
            {
                if (comboBox5.SelectedIndex == 0)
                {
                    A = new Matrix<double>(2 * point_check_table.Rows.Count, 8);
                    A.SetZero();
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        A[i, 3] = Z[j];
                        // Between zero
                        A[i + 1, 4] = 1.0d;
                        A[i + 1, 5] = X[j];
                        A[i + 1, 6] = Y[j];
                        A[i + 1, 7] = Z[j];
                    }
                }
                if (comboBox5.SelectedIndex == 1)
                {
                    A = new Matrix<double>(2 * point_check_table.Rows.Count, 14);
                    A.SetZero();
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        A[i, 3] = Z[j];
                        A[i, 4] = Z[j] * X[j];
                        A[i, 5] = Z[j] * Y[j];
                        A[i, 6] = X[j] * X[j];
                        // Between zero
                        A[i + 1, 7] = 1.0d;
                        A[i + 1, 8] = X[j];
                        A[i + 1, 9] = Y[j];
                        A[i + 1, 10] = Z[j];
                        A[i + 1, 11] = Z[j] * X[j];
                        A[i + 1, 12] = Z[j] * Y[j];
                        A[i + 1, 13] = X[j] * Y[j];
                    }
                }
                if (comboBox5.SelectedIndex == 2)
                {
                    A = new Matrix<double>(2 * point_check_table.Rows.Count, 12);
                    A.SetZero();
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        A[i, 3] = Z[j];
                        A[i, 4] = Z[j] * X[j];
                        A[i, 5] = Z[j] * Y[j];
                        // Between zero
                        A[i + 1, 6] = 1.0d;
                        A[i + 1, 7] = X[j];
                        A[i + 1, 8] = Y[j];
                        A[i + 1, 9] = Z[j];
                        A[i + 1, 10] = Z[j] * X[j];
                        A[i + 1, 11] = Z[j] * Y[j];
                    }
                }
            }
            if (comboBox3.SelectedIndex == 4)//Projective
            {
                A = new Matrix<double>(2 * point_check_table.Rows.Count, 8);
                A.SetZero();
                for (i = 0; i < A.Rows; i += 2)
                {
                    j = i / 2;
                    A[i, 0] = 1.0d;
                    A[i, 1] = X[j];
                    A[i, 2] = Y[j];
                    // Between zero
                    A[i, 6] = -row[j] * X[j];
                    A[i, 7] = -row[j] * Y[j];
                    // Between zero
                    A[i + 1, 3] = 1.0d;
                    A[i + 1, 4] = X[j];
                    A[i + 1, 5] = Y[j];
                    A[i + 1, 6] = -col[j] * X[j];
                    A[i + 1, 7] = -col[j] * Y[j];
                }
            }
            if (comboBox3.SelectedIndex == 5)// DLT
            {
                A = new Matrix<double>(2 * point_check_table.Rows.Count, 11);
                A.SetZero();
                for (i = 0; i < A.Rows; i += 2)
                {
                    j = i / 2;
                    A[i, 0] = 1.0d;
                    A[i, 1] = X[j];
                    A[i, 2] = Y[j];
                    A[i, 3] = Z[j];
                    A[i, 4] = -row[j] * X[j];
                    A[i, 5] = -row[j] * Y[j];
                    A[i, 6] = -row[j] * Z[j];
                    // Between zero
                    A[i + 1, 4] = -col[j] * X[j];
                    A[i + 1, 5] = -col[j] * Y[j];
                    A[i + 1, 6] = -col[j] * Z[j];
                    A[i + 1, 7] = 1.0d;
                    A[i + 1, 8] = X[j];
                    A[i + 1, 9] = Y[j];
                    A[i + 1, 10] = Z[j];
                }
            }
            if (comboBox3.SelectedIndex == 6)
            {
                if (comboBox5.SelectedIndex == 0)
                {
                    A = new Matrix<double>(2 * point_check_table.Rows.Count, 14);
                    A.SetZero();
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = Y[j];
                        A[i, 2] = X[j];
                        A[i, 3] = Z[j];
                        A[i, 4] = -row[j] * Y[j];
                        A[i, 5] = -row[j] * X[j];
                        A[i, 6] = -row[j] * Z[j];
                        // Between zero
                        A[i + 1, 7] = 1.0d;
                        A[i + 1, 8] = Y[j];
                        A[i + 1, 9] = X[j];
                        A[i + 1, 10] = Z[j];
                        A[i + 1, 11] = -col[j] * Y[j];
                        A[i + 1, 12] = -col[j] * X[j];
                        A[i + 1, 13] = -col[j] * Z[j];
                    }
                }
                if (comboBox5.SelectedIndex == 1)
                {
                    A = new Matrix<double>(2 * point_check_table.Rows.Count, 38);
                    A.SetZero();
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = Y[j];
                        A[i, 2] = X[j];
                        A[i, 3] = Z[j];
                        A[i, 4] = Y[j] * X[j];
                        A[i, 5] = Y[j] * Z[j];
                        A[i, 6] = X[j] * Z[j];
                        A[i, 7] = Y[j] * Y[j];
                        A[i, 8] = X[j] * X[j];
                        A[i, 9] = Z[j] * Z[j];
                        A[i, 10] = -row[j] * Y[j];
                        A[i, 11] = -row[j] * X[j];
                        A[i, 12] = -row[j] * Z[j];
                        A[i, 13] = -row[j] * Y[j] * X[j];
                        A[i, 14] = -row[j] * Y[j] * Z[j];
                        A[i, 15] = -row[j] * X[j] * Z[j];
                        A[i, 16] = -row[j] * Y[j] * Y[j];
                        A[i, 17] = -row[j] * X[j] * X[j];
                        A[i, 18] = -row[j] * Z[j] * Z[j];
                        // Between zero
                        A[i + 1, 19] = 1.0d;
                        A[i + 1, 20] = Y[j];
                        A[i + 1, 21] = X[j];
                        A[i + 1, 22] = Z[j];
                        A[i + 1, 23] = Y[j] * X[j];
                        A[i + 1, 24] = Y[j] * Z[j];
                        A[i + 1, 25] = X[j] * Z[j];
                        A[i + 1, 26] = Y[j] * Y[j];
                        A[i + 1, 27] = X[j] * X[j];
                        A[i + 1, 28] = Z[j] * Z[j];
                        A[i + 1, 29] = -col[j] * Y[j];
                        A[i + 1, 30] = -col[j] * X[j];
                        A[i + 1, 31] = -col[j] * Z[j];
                        A[i + 1, 32] = -col[j] * Y[j] * X[j];
                        A[i + 1, 33] = -col[j] * Y[j] * Z[j];
                        A[i + 1, 34] = -col[j] * X[j] * Z[j];
                        A[i + 1, 35] = -col[j] * Y[j] * Y[j];
                        A[i + 1, 36] = -col[j] * X[j] * X[j];
                        A[i + 1, 37] = -col[j] * Z[j] * Z[j];
                    }
                }
                if (comboBox5.SelectedIndex == 2)
                {
                    A = new Matrix<double>(2 * point_check_table.Rows.Count, 78);
                    A.SetZero();
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = Y[j];
                        A[i, 2] = X[j];
                        A[i, 3] = Z[j];
                        A[i, 4] = Y[j] * X[j];
                        A[i, 5] = Y[j] * Z[j];
                        A[i, 6] = X[j] * Z[j];
                        A[i, 7] = Y[j] * Y[j];
                        A[i, 8] = X[j] * X[j];
                        A[i, 9] = Z[j] * Z[j];
                        A[i, 10] = Y[j] * X[j] * Z[j];
                        A[i, 11] = Y[j] * Y[j] * Y[j];
                        A[i, 12] = Y[j] * X[j] * X[j];
                        A[i, 13] = Y[j] * Z[j] * Z[j];
                        A[i, 14] = Y[j] * Y[j] * X[j];
                        A[i, 15] = X[j] * X[j] * X[j];
                        A[i, 16] = X[j] * Z[j] * Z[j];
                        A[i, 17] = Y[j] * Y[j] * Z[j];
                        A[i, 18] = X[j] * X[j] * Z[j];
                        A[i, 19] = Z[j] * Z[j] * Z[j];
                        A[i, 20] = -row[j] * Y[j];
                        A[i, 21] = -row[j] * X[j];
                        A[i, 22] = -row[j] * Z[j];
                        A[i, 23] = -row[j] * Y[j] * X[j];
                        A[i, 24] = -row[j] * Y[j] * Z[j];
                        A[i, 25] = -row[j] * X[j] * Z[j];
                        A[i, 26] = -row[j] * Y[j] * Y[j];
                        A[i, 27] = -row[j] * X[j] * X[j];
                        A[i, 28] = -row[j] * Z[j] * Z[j];
                        A[i, 29] = -row[j] * Y[j] * X[j] * Z[j];
                        A[i, 30] = -row[j] * Y[j] * Y[j] * Y[j];
                        A[i, 31] = -row[j] * Y[j] * X[j] * X[j];
                        A[i, 32] = -row[j] * Y[j] * Z[j] * Z[j];
                        A[i, 33] = -row[j] * Y[j] * Y[j] * X[j];
                        A[i, 34] = -row[j] * X[j] * X[j] * X[j];
                        A[i, 35] = -row[j] * X[j] * Z[j] * Z[j];
                        A[i, 36] = -row[j] * Y[j] * Y[j] * Z[j];
                        A[i, 37] = -row[j] * X[j] * X[j] * Z[j];
                        A[i, 38] = -row[j] * Z[j] * Z[j] * Z[j];
                        // Between zero
                        A[i + 1, 39] = 1.0d;
                        A[i + 1, 40] = Y[j];
                        A[i + 1, 41] = X[j];
                        A[i + 1, 42] = Z[j];
                        A[i + 1, 43] = Y[j] * X[j];
                        A[i + 1, 44] = Y[j] * Z[j];
                        A[i + 1, 45] = X[j] * Z[j];
                        A[i + 1, 46] = Y[j] * Y[j];
                        A[i + 1, 47] = X[j] * X[j];
                        A[i + 1, 48] = Z[j] * Z[j];
                        A[i + 1, 49] = Y[j] * X[j] * Z[j];
                        A[i + 1, 50] = Y[j] * Y[j] * Y[j];
                        A[i + 1, 51] = Y[j] * X[j] * X[j];
                        A[i + 1, 52] = Y[j] * Z[j] * Z[j];
                        A[i + 1, 53] = Y[j] * Y[j] * X[j];
                        A[i + 1, 54] = X[j] * X[j] * X[j];
                        A[i + 1, 55] = X[j] * Z[j] * Z[j];
                        A[i + 1, 56] = Y[j] * Y[j] * Z[j];
                        A[i + 1, 57] = X[j] * X[j] * Z[j];
                        A[i + 1, 58] = Z[j] * Z[j] * Z[j];
                        A[i + 1, 59] = -col[j] * Y[j];
                        A[i + 1, 60] = -col[j] * X[j];
                        A[i + 1, 61] = -col[j] * Z[j];
                        A[i + 1, 62] = -col[j] * Y[j] * X[j];
                        A[i + 1, 63] = -col[j] * Y[j] * Z[j];
                        A[i + 1, 64] = -col[j] * X[j] * Z[j];
                        A[i + 1, 65] = -col[j] * Y[j] * Y[j];
                        A[i + 1, 66] = -col[j] * X[j] * X[j];
                        A[i + 1, 67] = -col[j] * Z[j] * Z[j];
                        A[i + 1, 68] = -col[j] * Y[j] * X[j] * Z[j];
                        A[i + 1, 69] = -col[j] * Y[j] * Y[j] * Y[j];
                        A[i + 1, 70] = -col[j] * Y[j] * X[j] * X[j];
                        A[i + 1, 71] = -col[j] * Y[j] * Z[j] * Z[j];
                        A[i + 1, 72] = -col[j] * Y[j] * Y[j] * X[j];
                        A[i + 1, 73] = -col[j] * X[j] * X[j] * X[j];
                        A[i + 1, 74] = -col[j] * X[j] * Z[j] * Z[j];
                        A[i + 1, 75] = -col[j] * Y[j] * Y[j] * Z[j];
                        A[i + 1, 76] = -col[j] * X[j] * X[j] * Z[j];
                        A[i + 1, 77] = -col[j] * Z[j] * Z[j] * Z[j];
                    }
                }
            }
            return A;
        }
        void adj(Matrix<double> A)
        {
            iteration = 0;
            Matrix<double> L = new Matrix<double>(A.Rows, 1);
            Matrix<double> l = new Matrix<double>(A.Rows, 1);
            Matrix<double> v = new Matrix<double>(A.Rows, 1);
            Matrix<double> Vr = new Matrix<double>(A.Rows / 2, 1);
            Matrix<double> Vc = new Matrix<double>(A.Rows / 2, 1);
            Matrix<double> lo = new Matrix<double>(A.Rows, 1);
            Matrix<double> P = new Matrix<double>(A.Rows, A.Rows);
            P.SetIdentity();// Unit matrix.     
            lo.SetZero();

            double[] row, col, X, Y, Z;
            double[] v_row = new double[A.Rows / 2];
            double[] v_col = new double[A.Rows / 2];

            int i, j, n, u;
            n = A.Rows / 2;
            u = 0;
            if (comboBox3.SelectedIndex == 0) u = 4;// Similarity (Helmert)
            if (comboBox3.SelectedIndex == 1) u = 6;// Affine
            if (comboBox3.SelectedIndex == 2)// Polinomial
            {
                if (comboBox5.SelectedIndex == 0) u = 6;// first degree (Affine)
                if (comboBox5.SelectedIndex == 1) u = 12;// second degree (bilinear)
                if (comboBox5.SelectedIndex == 2) u = 20;// third degree (cubic)
                if (comboBox5.SelectedIndex == 3) u = 30;// fourth degree (quartic)
                if (comboBox5.SelectedIndex == 4) u = 42;// fifth degree (quintic)
            }
            if (comboBox3.SelectedIndex == 3)// Affine projection
            {
                if (comboBox5.SelectedIndex == 0) u = 8;
                if (comboBox5.SelectedIndex == 1) u = 14;
                if (comboBox5.SelectedIndex == 2) u = 12;
            }
            if (comboBox3.SelectedIndex == 4) u = 8;// Projective
            if (comboBox3.SelectedIndex == 5) u = 11;// DLT
            if (comboBox3.SelectedIndex == 6) // RFM
            {
                if (comboBox5.SelectedIndex == 0) u = 14;// first degree
                if (comboBox5.SelectedIndex == 1) u = 38;// second degree
                if (comboBox5.SelectedIndex == 2) u = 78;// third degree
            }

            Matrix<double> Qxx = new Matrix<double>(u, u);
            Matrix<double> dx = new Matrix<double>(u, 1);
            Matrix<double> Px = new Matrix<double>(u, 1);
            Px.SetZero();

            run_iteration:// Iterative solution

            // Iteration start 
            CvInvoke.Invert((A.Transpose() * P * A), Qxx, Emgu.CV.CvEnum.DecompMethod.Cholesky);
            XYZ(out X, out Y, out Z, out row, out col);
            j = 0;
            for (i = 0; i < A.Rows; i += 2)
            {
                L[i, 0] = row[j];
                L[i + 1, 0] = col[j];
                j = j + 1;
            }

        
            l = L - lo;//

            Px = Px + dx;// for iteration
            lo = A * Px;// for iteration

            // Transactions
            dx = Qxx * A.Transpose() * l;
            v = (A * dx) - l;


            j = 0;
            for (i = 0; i < v.Rows; i += 2)// for vr and vc
            {
                v_row[j] = v[i, 0];
                point_check_table.Rows[j][17] = v_row[j];
                v_col[j] = v[i + 1, 0];
                point_check_table.Rows[j][18] = v_col[j];

                j = j + 1;
            }
            for (i = 0; i < v_row.Length; i++)// for row+vr and col+vc
            {
                v_row[i] = row[i] + v_row[i];
                point_check_table.Rows[i][19] = v_row[i];

                v_col[i] = v_col[i] + col[i];
                point_check_table.Rows[i][20] = v_col[i];
            }         
            if (checkBox1.Checked == true)// for back Normalize
            {
                v_row = Back_Normalize(v_row, row_max, row_min);
                v_col = Back_Normalize(v_col, col_max, col_min);
            }           
            for (i = 0; i < v_row.Length; i++)// Results
            {
                point_check_table.Rows[i][21] = v_row[i];
                point_check_table.Rows[i][22] = v_col[i];
            }

            iteration = iteration + 1;// for iteration
            Array.Resize<double>(ref mo, mo.Length + 1);// for iteration

            
            for (i = 0; i < point_check_table.Rows.Count; i++)//there are vr and vc for mo. Döngüsellerde ilk döngüyle olan vr ve vc lazım 
            {
                Vr[i, 0] = Convert.ToDouble(point_check_table.Rows[i][21].ToString()) -
                   Convert.ToDouble(point_check_table.Rows[i][2].ToString());

                Vc[i, 0] = Convert.ToDouble(point_check_table.Rows[i][22].ToString()) -
                  Convert.ToDouble(point_check_table.Rows[i][3].ToString());
            }
             
            j = 0;
            for (i = 0; i < dataGridView1.RowCount; i++) 
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true)
                {
                    dataGridView1.Rows[i].Cells[5].Value = Vr[j, 0].ToString("0.00");
                    dataGridView1.Rows[i].Cells[7].Value = Vc[j, 0].ToString("0.00");
                    j = j + 1;
                }
                else
                {
                    dataGridView1.Rows[i].Cells[5].Value = "--";
                    dataGridView1.Rows[i].Cells[7].Value = "--";
                }
            }

            double vrTvr = (Vr.Transpose() * Vr)[0, 0];
            double vcTvc = (Vc.Transpose() * Vc)[0, 0];
            double vTv = vrTvr + vcTvc;
            mo[mo.Length - 1] = vTv / (2 * n - u);
            mo[mo.Length - 1] = Math.Sqrt(mo[mo.Length - 1]);
            mr = Math.Sqrt(vrTvr / (n - (u / 2)));
            mc = Math.Sqrt(vcTvc / (n - (u / 2)));
            vcol_total = 0;
            vrow_total = 0;
            for (i = 0; i < Vr.Rows; i++)
            {
                vrow_total = Vr[i, 0] + vrow_total;
                vcol_total = Vc[i, 0] + vcol_total;
            }
            double mon;
            if (checkBox1.Checked == true)
            {
                for (i = 0; i < point_check_table.Rows.Count; i++)
                {
                    Vr[i, 0] = Convert.ToDouble(point_check_table.Rows[i][19].ToString()) -
                       Convert.ToDouble(point_check_table.Rows[i][12].ToString());

                    Vc[i, 0] = Convert.ToDouble(point_check_table.Rows[i][20].ToString()) -
                      Convert.ToDouble(point_check_table.Rows[i][13].ToString());
                }
                vrTvr = (Vr.Transpose() * Vr)[0, 0];
                vcTvc = (Vc.Transpose() * Vc)[0, 0];
                vTv = vrTvr + vcTvc;
                mon = Math.Sqrt(vTv / (2 * n - u));
            }
            else mon = mo[mo.Length - 1];

            // Blunder test
            bool[] blunder_rslt = blunder((double)numericUpDown9.Value, mo[mo.Length - 1]
                , mon, P, A, v, Qxx);
            j = 0;
            int k, p;
            p = 0;
            for (i = 0; i < point_check_table.Rows.Count; i++)
            {
                j = i * 2;
                k = i + p;
                while (Convert.ToBoolean(dataGridView1.Rows[k].Cells[0].Value) == false)
                {
                    dataGridView1.Rows[k].Cells[3].Value = "--";
                    p = p + 1;
                    k = i + p;
                }
                if (blunder_rslt[j] == true) dataGridView1.Rows[k].Cells[3].Value = "Row";
                if (blunder_rslt[j + 1] == true) dataGridView1.Rows[k].Cells[3].Value = "Column";
                if ((blunder_rslt[j] == true) && (blunder_rslt[j + 1] == true))
                    dataGridView1.Rows[k].Cells[3].Value = "Row & Column";
                if ((blunder_rslt[j] == false) && (blunder_rslt[j + 1] == false))
                 dataGridView1.Rows[k].Cells[3].Value = System.Text.RegularExpressions.Regex.Unescape("\\u2713");
            }

            // Validation of parameters/coefficients
            bool[] par_valid_rslt = par_valid(mon, A, dx, Qxx);
            par_valid_rslt_size = new List<int>();  
            for (i = 0; i < par_valid_rslt.Length; i++)
                if (par_valid_rslt[i] == false)
                    par_valid_rslt_size.Add(i + 1);
            
            label33.Visible = true;
            if (par_valid_rslt_size.Count == 0)
            {
                label33.ForeColor = Color.Black;
                label33.Text = "All parameters are valid.";
                label33.Cursor = Cursors.Default;
            }
            else
            {
                label33.ForeColor = Color.Blue;
                label33.Text = "Same parameters are unvalid.";
                label33.Cursor = Cursors.Hand;
            }

            //for DLT and Projective iteration
            if ((comboBox3.SelectedIndex == 5) || (comboBox3.SelectedIndex == 4) || (comboBox3.SelectedIndex == 6))// iteration limit
            {
                double diff_mo = mo[mo.Length - 1] - mo[mo.Length - 2];           
                if ((diff_mo > Convert.ToDouble(numericUpDown1.Value) || (iteration == 1)))
                {
                    if (iteration < Convert.ToInt32(numericUpDown2.Value))
                    {
                        A = jacobian();
                        goto run_iteration;
                    }
                    else MessageBox.Show("Maximum iteration reached." + "\n" + "Num. of iteration= "
                        + iteration.ToString() + "\n" + (char)916 + "mo (pix)= " + diff_mo.ToString());
                }
                else MessageBox.Show("Threshold achieved." + "\n" + "Num. of iteration= "
                      + iteration.ToString() + "\n" + (char)916 + "mo (pix)= " + diff_mo.ToString());
            }
            image_dzyn();



        }
        static Bitmap image_mini = new Bitmap(100, 100);
        int down_Scale;
        public int scale_control = 0; 
   
        //
        // Events
        //
        private void GeoTransform_Load(object sender, EventArgs e)
        {
           
            view_here();
            form_active = true;
            new_data_active = true;
            Form1 form = Application.OpenForms["Form1"] as Form1;
            this.Owner = form;
        
            point_total_table = new DataTable();
            point_total_table.Columns.Add("Point ID", typeof(int));                       // 0
            point_total_table.Columns.Add("Point Type", typeof(string));                  // 1
            point_total_table.Columns.Add("Row", typeof(double));                         // 2
            point_total_table.Columns.Add("Column", typeof(double));                      // 3
            point_total_table.Columns.Add("X", typeof(double));                           // 4
            point_total_table.Columns.Add("Y", typeof(double));                           // 5
            point_total_table.Columns.Add("Z", typeof(double));                           // 6
            point_total_table.Columns.Add("Std Row", typeof(double));                     // 7
            point_total_table.Columns.Add("Std Col", typeof(double));                     // 8
            point_total_table.Columns.Add("Std X", typeof(double));                       // 9
            point_total_table.Columns.Add("Std Y", typeof(double));                       // 10
            point_total_table.Columns.Add("Std Z", typeof(double));                       // 11
            point_total_table.Columns.Add("check", typeof(Boolean));                      // 12


            point_check_table = new DataTable();
            point_check_table.Columns.Add("Point ID", typeof(int));                       // 0
            point_check_table.Columns.Add("Point Type", typeof(string));                  // 1
            point_check_table.Columns.Add("Row", typeof(double));                         // 2
            point_check_table.Columns.Add("Column", typeof(double));                      // 3
            point_check_table.Columns.Add("X", typeof(double));                           // 4
            point_check_table.Columns.Add("Y", typeof(double));                           // 5
            point_check_table.Columns.Add("Z", typeof(double));                           // 6
            point_check_table.Columns.Add("Std Row", typeof(double));                     // 7
            point_check_table.Columns.Add("Std Col", typeof(double));                     // 8
            point_check_table.Columns.Add("Std X", typeof(double));                       // 9
            point_check_table.Columns.Add("Std Y", typeof(double));                       // 10
            point_check_table.Columns.Add("Std Z", typeof(double));                       // 11
            point_check_table.Columns.Add("Row Norm", typeof(double));                    // 12
            point_check_table.Columns.Add("Col Norm", typeof(double));                    // 13
            point_check_table.Columns.Add("X Norm", typeof(double));                      // 14
            point_check_table.Columns.Add("Y Norm", typeof(double));                      // 15
            point_check_table.Columns.Add("Z Norm", typeof(double));                      // 16
            point_check_table.Columns.Add("vrn or vr", typeof(double));                   // 17
            point_check_table.Columns.Add("vcn or vc", typeof(double));                   // 18
            point_check_table.Columns.Add("rn + vrn or r + vr ", typeof(double));         // 19
            point_check_table.Columns.Add("cn + vcn or c + vc", typeof(double));          // 20
            point_check_table.Columns.Add("Row adjusted and renormalized or r + vr", typeof(double));// 21
            point_check_table.Columns.Add("Column adjusted and renormalized or c + vc", typeof(double));// 22
            point_check_table.Columns.Add("Outlier", typeof(bool));                       // 23

            int i;
            dataGridView1.Top = groupBox3.Height + groupBox4.Height + 30;
            dataGridView1.Height = this.Height - dataGridView1.Top - 50;
            dataGridView1.Width = this.Width - 50;
            groupBox2.Width = dataGridView1.Width + dataGridView1.Left - groupBox2.Left;

            int cell_windth = (dataGridView1.Width - 30) / (dataGridView1.ColumnCount - 1);
            for (i = 1; i == dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].Width = cell_windth;
                dataGridView1.Columns[i].FillWeight = cell_windth;
            }
 
            mo = new double[1];
            mr = 0;
            mc = 0;
            mo[0] = 0;
            vrow_total = 0;
            vcol_total = 0;
            iteration = 0;
       

            comboBox3.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;

     
            dataGridView1.Columns[5].HeaderText = ((char)177).ToString() + " vr";
            dataGridView1.Columns[7].HeaderText = ((char)177).ToString() + " vc";

            if (Form1.Imagelist.Count != 0)
                load_dzyn();
        
        }
        public void load_dzyn()
        {
            comboBox2.Items.Clear();
            int i;
            int colwidth = (dataGridView1.Width - 50) / 10;
            image_mini = Form1.Image_mini;
            down_Scale = Form1.down_scale;
            comboBox1.SelectedIndex = 0;
            Form1 form = Application.OpenForms["Form1"] as Form1;
            for (i = 0; i < Form1.Imagelist.Count; i++)
                comboBox2.Items.Add(form.treeView1.Nodes[Form1.Project_ID].
                    Nodes[i].Text.Substring(7));

            comboBox2.SelectedIndex = Form1.Image_ID;
            data_total(comboBox2.SelectedIndex);
            for (i = 1; i < 11; i++)
                dataGridView1.Columns[i].Width = colwidth;
            label29.Text = Form1.gcp_collected[Form1.Image_ID].Rows.Count.ToString();
            label31.Text = Form1.icp_collected[Form1.Image_ID].Rows.Count.ToString();
            image_dzyn();

        }

        private void GeoTransform_FormClosed(object sender, FormClosedEventArgs e)
        {
            form_active = false;
            scale_control = 0;
            Visual v = new Visual();
            v.delete_img();
            view_save();
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            image_dzyn();
        }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            image_dzyn();
        }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            image_dzyn();
        }
        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            image_dzyn();
        }
        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            image_dzyn();
        }

        private void button5_Click(object sender, EventArgs e)
        {

            Empty_image emp = new Empty_image();
            emp.ShowDialog();
     
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            image_dzyn();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            points_view p = new points_view();
            p.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Under construction", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static Color line_color = Color.FromKnownColor(KnownColor.ControlLight);
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (line_color == Color.FromKnownColor(KnownColor.ControlDark)) line_color = Color.FromKnownColor(KnownColor.ControlLight);
            else line_color = Color.FromKnownColor(KnownColor.ControlDark);
            dataGridView1.Rows[dataGridView1.RowCount - 1].DefaultCellStyle.BackColor = line_color;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            data_clear();
            image_dzyn();
            if (comboBox1.SelectedIndex == 1) numericUpDown9.Enabled = false;
            else numericUpDown9.Enabled = true ;
        }
        private void label33_Click(object sender, EventArgs e)
        {
            if (par_valid_rslt_size.Count != 0)
            {
                string size_texts=par_valid_rslt_size[0].ToString();
                int i;
                for (i = 1; i < par_valid_rslt_size.Count; i++)
                    size_texts = size_texts + ", " + par_valid_rslt_size[i].ToString();
                MessageBox.Show("Parameter id: "+size_texts, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            line_color = Color.FromKnownColor(KnownColor.ControlLight);
            int i;
            for (i = 0; i < dataGridView1.RowCount; i++)
            {
                if (line_color == Color.FromKnownColor(KnownColor.ControlDark)) line_color = Color.FromKnownColor(KnownColor.ControlLight);
                else line_color = Color.FromKnownColor(KnownColor.ControlDark);
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = line_color;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            data_clear();
            image_dzyn();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (new_data_active == false)
            {
                Form1 f = Application.OpenForms["Form1"] as Form1;
                f.Image_ID_change(comboBox2.SelectedIndex);
                data_total(comboBox2.SelectedIndex);
            }
            
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            data_clear();
            image_dzyn();
            if ((comboBox3.SelectedIndex == 4) || (comboBox3.SelectedIndex == 5) || (comboBox3.SelectedIndex == 6))//
            {
                label7.Enabled = true;
                numericUpDown2.Enabled = true;
                label6.Enabled = true;
                numericUpDown1.Enabled = true;
            }
            else
            {
                label7.Enabled = false;
                numericUpDown2.Enabled = false;
                label6.Enabled = false;
                numericUpDown1.Enabled = false;
            }
            if ((comboBox3.SelectedIndex == 2) || (comboBox3.SelectedIndex == 3) || (comboBox3.SelectedIndex == 6))
            {

                label21.Enabled = true;
                comboBox5.Enabled = true;
                if (comboBox3.SelectedIndex == 2)
                {
                    comboBox5.Items.Clear();
                    int i;
                    comboBox5.Items.Add("1 (Affine 2D)");
                    for (i = 2; i <= 5; i++)
                        comboBox5.Items.Add(i.ToString());
                    comboBox5.SelectedIndex = 0;
                }
                if (comboBox3.SelectedIndex == 3)
                {
                    comboBox5.Items.Clear();
                    comboBox5.Items.Add("Generic affine projection");
                    comboBox5.Items.Add("AP for OrbView - 3");
                    comboBox5.Items.Add("AP for IKONOS & QuickBird");
                    comboBox5.SelectedIndex = 0;
                }
                if (comboBox3.SelectedIndex == 6)
                {
                    comboBox5.Items.Clear();
                    comboBox5.Items.Add("1");
                    comboBox5.Items.Add("2");
                    comboBox5.Items.Add("3");
                    comboBox5.SelectedIndex = 0;
                }

            }
            else
            {
                label21.Enabled = false;
                comboBox5.Enabled = false;
            }

            // For Degree of freedom
            label22.Text = degreeoffreedom().ToString();
        }
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            // For Degree of freedom
            data_clear();
            image_dzyn();
            label22.Text = degreeoffreedom().ToString();
        }      
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((new_data_active == false) && (e.ColumnIndex == 0))
            {
                data_clear();
                check_dzyn();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            mo = new double[1];
            mr = 0;
            mc = 0;
            mo[0] = 0;
            vrow_total = 0;
            vcol_total = 0;
            iteration = 0;
            check_dzyn();
            data_clear();

            Matrix<double> A = jacobian();
            adj(A);
            data_clear_active = true;

            label15.Text = ((char)177).ToString() + mr.ToString("0.00");
            label16.Text = ((char)177).ToString() + mc.ToString("0.00");
            label17.Text = ((char)177).ToString() + mo[mo.Length - 1].ToString("0.00");
            label18.Text = vrow_total.ToString("0.00");
            label19.Text = vcol_total.ToString("0.00");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            data_clear();
            check_dzyn();
            pictureBox1.Refresh();
            pictureBox2.Refresh();
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            image_dzyn();
        }
    }
}