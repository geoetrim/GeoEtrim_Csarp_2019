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

namespace GeoEtrim
{
    public partial class GeoTrans : Form
    {
        public GeoTrans()
        {
            InitializeComponent();
        }
        static DataTable point_table = new DataTable();
        static double mr, mc,vrow_total,vcol_total,col_max,col_min,row_max,row_min;
        static double[] mo = new double[1];
        static int iteration = 0;
        static int[] check_control;
        static int first_open = 0;
        
        //
        // Visual interface functions
        //
        void data_disign(int id)
        {
            first_open = 0;
            point_table.Rows.Clear();
            dataGridView1.Rows.Clear();
            int j, i;
            for (i = 0; i < Form1.gcp_collected[id].Rows.Count; i++)
                point_table.Rows.Add(Form1.gcp_collected[id].Rows[i].ItemArray);
                
            for (i = 0; i < Form1.icp_collected[id].Rows.Count; i++)
                point_table.Rows.Add(Form1.icp_collected[id].Rows[i].ItemArray);

            for (i = 0; i < point_table.Rows.Count; i++)
            {
                dataGridView1.Rows.Add();
                if (check_control[i] == 1)
                    dataGridView1.Rows[i].Cells[0].Value = true;
                else dataGridView1.Rows[i].Cells[0].Value = false;
                dataGridView1.Rows[i].Cells[1].Value = point_table.Rows[i][0];
                dataGridView1.Rows[i].Cells[2].Value = point_table.Rows[i][1].ToString();
                dataGridView1.Rows[i].Cells[3].Value = point_table.Rows[i][23].ToString();
                dataGridView1.Rows[i].Cells[4].Value = point_table.Rows[i][2];
                dataGridView1.Rows[i].Cells[5].Value = point_table.Rows[i][17];
                dataGridView1.Rows[i].Cells[6].Value = point_table.Rows[i][3];
                dataGridView1.Rows[i].Cells[7].Value = point_table.Rows[i][18];
                dataGridView1.Rows[i].Cells[8].Value = point_table.Rows[i][4];
                dataGridView1.Rows[i].Cells[9].Value = point_table.Rows[i][5];
                dataGridView1.Rows[i].Cells[10].Value = point_table.Rows[i][6];
            }
           
            for (i = 0; i < dataGridView1.RowCount; i++)
                if (check_control[dataGridView1.RowCount - i - 1] == 0)
                    point_table.Rows[dataGridView1.RowCount - i - 1].Delete(); //işaretsizleri silmek için(tr)

            // Normalize
            double[] columns_array = new double[point_table.Rows.Count];
            if (point_table.Rows.Count > 0)
            {
                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < point_table.Rows.Count; j++)
                        columns_array[j] = Convert.ToDouble(point_table.Rows[j][i + 2].ToString());

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

                    for (j = 0; j < point_table.Rows.Count; j++)
                        point_table.Rows[j][i + 12] = columns_array[j];
                }
            }
            mr = 0;
            mc = 0;
            mo[mo.Length - 1] = 0;
            vrow_total = 0;
            vcol_total = 0;
            // dataGridView2.DataSource = point_table;
            label15.Text = ((char)177).ToString() + mr.ToString();
            label16.Text = ((char)177).ToString() + mc.ToString();
            label17.Text = ((char)177).ToString() + mo[mo.Length - 1].ToString();
            label18.Text =  vrow_total.ToString();
            label19.Text =  vcol_total.ToString();
            label22.Text = degreeoffreedom().ToString();

            image_dzyn();
            first_open = 1;
           
        }
        void check_dzyn()
        {
            int i;
            for (i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true)
                    check_control[i] = 1;
                else check_control[i] = 0;
            }
            data_disign(comboBox2.SelectedIndex);
        }
        void image_dzyn()
        {            
            int i;
            Bitmap bmp = new Bitmap(Form1.Image_mini);
            if (checkBox2.Checked == false)
                bmp = (new Image<Rgb, byte>(bmp.Width, bmp.Height, new Rgb(255, 255, 255))).Bitmap;
            Image<Rgb, byte> img = new Image<Rgb, byte>(bmp);
            Image<Rgb, byte> img2 = new Image<Rgb, byte>(bmp.Width,bmp.Height,new Rgb(Color.White));
            float r, c,r2,c2,vr,vc;
            
            for (i = 0; i < point_table.Rows.Count; i++)
           {
                    r = Convert.ToSingle(point_table.Rows[i][2].ToString()) / (Form1.down_scale - 1);
                    c = Convert.ToSingle(point_table.Rows[i][3].ToString()) / (Form1.down_scale - 1);
                    CvInvoke.Circle(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), Convert.ToInt32( numericUpDown7.Value), new Rgb(Color.Red).MCvScalar,
                        Convert.ToInt32(numericUpDown7.Value)+1);
                if (point_table.Rows[i][21].ToString() != "")
                {
                    r2 = (Convert.ToSingle(point_table.Rows[i][21].ToString()) / (Form1.down_scale - 1));
                    c2 = (Convert.ToSingle(point_table.Rows[i][22].ToString()) / (Form1.down_scale - 1));
                    vr = (r2 - r)* Convert.ToSingle(numericUpDown3.Value); // for vector scale
                    vc = (c2 - c)* Convert.ToSingle(numericUpDown3.Value); // for vector scale
                    r2 = vr + r;
                    c2 = vc + c;
                    CvInvoke.Line(img, new Point(Convert.ToInt32(c), Convert.ToInt32(r)), 
                        new Point(Convert.ToInt32(c2), Convert.ToInt32(r2)), new Rgb(Color.Red).MCvScalar, Convert.ToInt32(numericUpDown4.Value));

                    vr = (vr / Convert.ToSingle(numericUpDown3.Value)) * Convert.ToSingle(numericUpDown5.Value);
                    vc = (vc / Convert.ToSingle(numericUpDown3.Value)) * Convert.ToSingle(numericUpDown5.Value);
                    CvInvoke.Line(img2, new Point(Convert.ToInt32(bmp.Width / 2), Convert.ToInt32(bmp.Height / 2)),
                        new Point(Convert.ToInt32((bmp.Width / 2) + vc), Convert.ToInt32((bmp.Height / 2) + vr)),
                        new Rgb(Color.BlueViolet).MCvScalar, Convert.ToInt32(numericUpDown6.Value));
                }
            }
            pictureBox1.Image = img.Bitmap;
            pictureBox2.Image = img2.Bitmap;
        }
        int degreeoffreedom() //For Degree of freedom
        {
            int result = 0;
            int u = 0;
            if (comboBox3.SelectedIndex == 0) u = 4; //Similarity (Helmert)
            if (comboBox3.SelectedIndex == 1) u = 6; // Affine
            if (comboBox3.SelectedIndex == 2)// Polinomial
            {
                if (comboBox5.SelectedIndex == 0) u = 6; // first degree (Affine)
                if (comboBox5.SelectedIndex == 1) u = 12; // second degree (bilinear)
                if (comboBox5.SelectedIndex == 2) u = 20; // third degree (cubic)
                if (comboBox5.SelectedIndex == 3) u = 30; // fourth degree (quartic)
                if (comboBox5.SelectedIndex == 4) u = 42; //fifth degree (quintic)
            }
            if (comboBox3.SelectedIndex == 3) //Affine projection
            {
                if (comboBox5.SelectedIndex == 0) u = 8;
                if (comboBox5.SelectedIndex == 1) u = 14;
                if (comboBox5.SelectedIndex == 2) u = 12;
            }
            if (comboBox3.SelectedIndex == 4) u = 8; // Projective
            if (comboBox3.SelectedIndex == 5) u = 11;//DLT
            result = point_table.Rows.Count - u;
            return result;
        }

        //
        // Processing functions
        //
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
            X = new double[point_table.Rows.Count];
            Y = new double[point_table.Rows.Count];
            Z = new double[point_table.Rows.Count];
            row = new double[point_table.Rows.Count];
            col = new double[point_table.Rows.Count];
            if (checkBox1.Checked == true)
            {
                for (i = 0; i < point_table.Rows.Count; i++)
                {
                    X[i] = Convert.ToDouble(point_table.Rows[i][14]);
                    Y[i] = Convert.ToDouble(point_table.Rows[i][15]);
                    Z[i] = Convert.ToDouble(point_table.Rows[i][16]);

                    row[i] = Convert.ToDouble(point_table.Rows[i][12]);
                    col[i] = Convert.ToDouble(point_table.Rows[i][13]);
                }
            }
            else
            {
                for (i = 0; i < point_table.Rows.Count; i++)
                {
                    X[i] = Convert.ToDouble(point_table.Rows[i][4]);
                    Y[i] = Convert.ToDouble(point_table.Rows[i][5]);
                    Z[i] = Convert.ToDouble(point_table.Rows[i][6]);

                    row[i] = Convert.ToDouble(point_table.Rows[i][2]);
                    col[i] = Convert.ToDouble(point_table.Rows[i][3]);
                }
            }
            if (iteration != 0)
            {
                for (i = 0; i < point_table.Rows.Count; i++)
                {
                    row[i] = Convert.ToDouble(point_table.Rows[i][19]);
                    col[i] = Convert.ToDouble(point_table.Rows[i][20]);
                }
            }
        }
        Matrix<double> jacobian()
        {
            // Row, Col, x, y, z
            Matrix<double> A = null;
            int i, j;
            double[] X , Y, Z, row, col;
            XYZ(out X, out Y, out Z, out row, out col);
         
            // Mat models
            int k;
            if (comboBox3.SelectedIndex == 0) //Similarity (Helmert)
            {
                A = new Matrix<double>(2 * point_table.Rows.Count, 4);
                
                for (i = 0; i < A.Rows; i += 2)
                {
                    j = i / 2;
                    A[i, 0] = 1.0d;
                    A[i, 1] = X[j];
                    A[i, 2] = -Y[j];
                    A[i, 3] = 0.0d;
                    A[i + 1, 0] = 0.0d;
                    A[i + 1, 1] = Y[j];
                    A[i + 1, 2] = X[j];
                    A[i + 1, 3] = 1.0d; 
                }
            }
            if (comboBox3.SelectedIndex == 1) //Affine
            {
                A = new Matrix<double>(2 * point_table.Rows.Count, 6);
                
                for (i = 0; i < A.Rows; i += 2)
                {
                    j = i / 2;
                    A[i, 0] = 1.0d;
                    A[i, 1] = X[j];
                    A[i, 2] = Y[j];
                    for (k = 0; k < 3; k++) // for zero values
                    {
                        A[i, k + 3] = 0;
                        A[i + 1, k] = 0;
                    }
                    A[i + 1, 3] = 1.0d;
                    A[i + 1, 4] = X[j];
                    A[i + 1, 5] = Y[j];
                }
            }
            if (comboBox3.SelectedIndex == 2) //Polinomial
            {
                
                if (comboBox5.SelectedIndex == 0) // first degree (Affine)
                {
                    A = new Matrix<double>(2 * point_table.Rows.Count, 6);
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        for (k = 0; k < 3; k++) // for zero values
                        {
                            A[i, k + 3] = 0;
                            A[i + 1, k] = 0;
                        }
                        A[i + 1, 3] = 1.0d;
                        A[i + 1, 4] = X[j];
                        A[i + 1, 5] = Y[j];
                    }
                }
                if (comboBox5.SelectedIndex == 1)// second degree (bilinear)
                {
                    A = new Matrix<double>(2 * point_table.Rows.Count, 12);
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        A[i, 3] = X[j] * Y[j];
                        A[i, 4] = X[j] * X[j];
                        A[i, 5] = Y[j] * Y[j];
                        for (k = 0; k < 6; k++)// for zero values
                        {
                            A[i, k + 6] = 0;
                            A[i + 1, k] = 0;
                        }
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
                    A = new Matrix<double>(2 * point_table.Rows.Count, 20);
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
                        for (k = 0; k < 10; k++) // for zero values
                        {
                            A[i, k + 10] = 0;
                            A[i + 1, k] = 0;
                        }
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
                    A = new Matrix<double>(2 * point_table.Rows.Count, 30);// fourth degree (quartic)
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
                        for (k = 0; k < 15; k++)  // for zero values
                        {
                            A[i, k + 15] = 0;
                            A[i + 1, k] = 0;
                        }
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
                    A = new Matrix<double>(2 * point_table.Rows.Count, 42);// fifth degree (quintic)
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
                        for (k = 0; k < 21; k++) // for zero values
                        {
                            A[i, k + 21] = 0;
                            A[i + 1, k] = 0;
                        }
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
                    A = new Matrix<double>(2 * point_table.Rows.Count, 8);
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        A[i, 3] = Z[j];
                        for (k = 0; k < 4; k++) // for zero values
                        {
                            A[i, k + 4] = 0;
                            A[i + 1, k] = 0;
                        }
                        A[i + 1, 4] = 1.0d;
                        A[i + 1, 5] = X[j];
                        A[i + 1, 6] = Y[j];
                        A[i + 1, 7] = Z[j];
                    }
                }
                if (comboBox5.SelectedIndex == 1)
                {
                    A = new Matrix<double>(2 * point_table.Rows.Count, 14);
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
                        for (k = 0; k < 7; k++) // for zero values
                        {
                            A[i, k + 7] = 0;
                            A[i + 1, k] = 0;
                        }
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
                    A = new Matrix<double>(2 * point_table.Rows.Count, 12);
                    for (i = 0; i < A.Rows; i += 2)
                    {
                        j = i / 2;
                        A[i, 0] = 1.0d;
                        A[i, 1] = X[j];
                        A[i, 2] = Y[j];
                        A[i, 3] = Z[j];
                        A[i, 4] = Z[j] * X[j];
                        A[i, 5] = Z[j] * Y[j];
                        for (k = 0; k < 6; k++) // for zero values
                        {
                            A[i, k + 6] = 0;
                            A[i + 1, k] = 0;
                        }
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
                A = new Matrix<double>(2 * point_table.Rows.Count, 8);
                
                for (i = 0; i < A.Rows; i += 2)
                {
                    j = i / 2;
                    A[i, 0] = 1.0d;
                    A[i, 1] = X[j];
                    A[i, 2] = Y[j];
                    for (k = 0; k < 3; k++) // for zero values
                        A[i, k + 3] = 0;
                    A[i, 6] = -row[j] * X[j];
                    A[i, 7] = -row[j] * Y[j];
                    for (k = 0; k < 3; k++) // for zero values
                        A[i + 1, k] = 0;
                    A[i + 1, 3] = 1.0d;
                    A[i + 1, 4] = X[j];
                    A[i + 1, 5] = Y[j];
                    A[i + 1, 6] = -col[j] * X[j];
                    A[i + 1, 7] = -col[j] * Y[j];
                }
            }
            if (comboBox3.SelectedIndex == 5)//DLT
            {
                A = new Matrix<double>(2 * point_table.Rows.Count, 11);
                
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
                    for (k = 0; k < 4; k++) // for zero values
                    {
                        A[i, k + 7] = 0;
                        A[i + 1, k] = 0;
                    }
                    A[i + 1, 4] = -col[j] * X[j];
                    A[i + 1, 5] = -col[j] * Y[j];
                    A[i + 1, 6] = -col[j] * Z[j];
                    A[i + 1, 7] = 1.0d;
                    A[i + 1, 8] = X[j];
                    A[i + 1, 9] = Y[j];
                    A[i + 1, 10] = Z[j];
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

            int i, j, n, u;
            double[] row,col, x, y, z;

            double[] v_row = new double[A.Rows / 2];
            double[] v_col = new double[A.Rows / 2];
            Matrix<double> ATA_inv = new Matrix<double>(1, 1);
            Matrix<double> dx;
            n = A.Rows / 2;
            u = 0;
           
            if (comboBox3.SelectedIndex == 0) u = 4; //Similarity (Helmert)
            if (comboBox3.SelectedIndex == 1) u = 6; // Affine
            if (comboBox3.SelectedIndex == 2)// Polinomial
            {
                if (comboBox5.SelectedIndex == 0) u = 6; // first degree (Affine)
                if (comboBox5.SelectedIndex == 1) u = 12; // second degree (bilinear)
                if (comboBox5.SelectedIndex == 2) u = 20; // third degree (cubic)
                if (comboBox5.SelectedIndex == 3) u = 30; // fourth degree (quartic)
                if (comboBox5.SelectedIndex == 4) u = 42; //fifth degree (quintic)
            }
            if (comboBox3.SelectedIndex == 3) //Affine projection
            {
                if (comboBox5.SelectedIndex == 0) u = 8;
                if (comboBox5.SelectedIndex == 1) u = 14;
                if (comboBox5.SelectedIndex == 2) u = 12;
            }
            if (comboBox3.SelectedIndex == 4) u = 8; // Projective
            if (comboBox3.SelectedIndex == 5) u = 11;//DLT
            ATA_inv = new Matrix<double>(u, u);
            dx = new Matrix<double>(u, 1);
            Matrix<double> Px = new Matrix<double>(u, 1);

            run_iteration:  // Iterative solution
            CvInvoke.Invert((A.Transpose() * A), ATA_inv, Emgu.CV.CvEnum.DecompMethod.Cholesky);
            j = 0;
            XYZ(out x, out y, out z, out row, out col);
            for (i = 0; i < A.Rows; i += 2)
            {
                L[i, 0] = row[j];
                L[i + 1, 0] = col[j];
                j=j + 1;
            }
            
            if (iteration != 0)
            {
                Px = Px + dx;
                lo = A * Px;
                l = L - lo;
            }
            else l = L;
  
            //transactions
            dx = ATA_inv * A.Transpose() * l;
            v = (A * dx) - l;

            j = 0;
            for (i = 0; i < v.Rows; i += 2)
            {
                v_row[j] = v[i, 0];
                point_table.Rows[j][17] = v_row[j];
                v_col[j] = v[i + 1, 0];
                point_table.Rows[j][18] = v_col[j];

                j = j + 1;
            }
            for (i = 0; i < v_row.Length; i++) 
            {
                v_row[i] = v_row[i] + row[i];
                point_table.Rows[i][19] = v_row[i];

                v_col[i] = v_col[i] + col[i];
                point_table.Rows[i][20] = v_col[i];
            }

            // Result
            if (checkBox1.Checked == true)
            {
                v_row = Back_Normalize(v_row, row_max, row_min);
                v_col = Back_Normalize(v_col, col_max, col_min);
            }
            for (i = 0; i < v_row.Length; i++)
            {
                point_table.Rows[i][21] = v_row[i];
                point_table.Rows[i][22] = v_col[i];
               
            }
            //////// new max and min for iteration
            row_max = v_row.Max();
            row_min = v_row.Min();

            col_max = v_col.Max();
            col_min = v_col.Min();

            iteration = iteration + 1;
            Array.Resize<double>(ref mo, mo.Length + 1); // for DLT and Projective
     
            for (i = 0; i < point_table.Rows.Count; i++)
            {
                Vr[i, 0] = Convert.ToDouble(point_table.Rows[i][21].ToString()) -
                    Convert.ToDouble(point_table.Rows[i][2].ToString());
                
                Vc[i, 0] = Convert.ToDouble(point_table.Rows[i][22].ToString()) -
                   Convert.ToDouble(point_table.Rows[i][3].ToString());
               
            }
            j = 0;
            for (i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true)
                {
                    dataGridView1.Rows[i].Cells[5].Value = Vr[j, 0].ToString("0.00");
                    dataGridView1.Rows[i].Cells[7].Value = Vc[j, 0].ToString("0.00");
                    j = j +1;
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
            //for DLT and Projective iteration
            
            if ((comboBox3.SelectedIndex == 5) || (comboBox3.SelectedIndex == 4))// iteration limit
            {
                double diff_mo = mo[mo.Length - 1] - mo[mo.Length - 2];
                diff_mo = Math.Abs(diff_mo);
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
        private void GeoTrans_Load(object sender, EventArgs e)
        {
            point_table = new DataTable();
            point_table.Columns.Add("Point ID", typeof(int));
            point_table.Columns.Add("Point Type", typeof(string));
            point_table.Columns.Add("Row", typeof(float));
            point_table.Columns.Add("Column", typeof(float));
            point_table.Columns.Add("X", typeof(double));
            point_table.Columns.Add("Y", typeof(double));
            point_table.Columns.Add("Z", typeof(double));
            point_table.Columns.Add("Std Row", typeof(double));
            point_table.Columns.Add("Std Col", typeof(double));
            point_table.Columns.Add("Std X", typeof(double));
            point_table.Columns.Add("Std Y", typeof(double));
            point_table.Columns.Add("Std Z", typeof(double));
            point_table.Columns.Add("Row Norm", typeof(double));
            point_table.Columns.Add("Col Norm", typeof(double));
            point_table.Columns.Add("X Norm", typeof(double));
            point_table.Columns.Add("Y Norm", typeof(double));
            point_table.Columns.Add("Z Norm", typeof(double));
            point_table.Columns.Add("Vrn&Vr", typeof(double));
            point_table.Columns.Add("Vcn&Vc", typeof(double));
            point_table.Columns.Add("Rn + Vrn&Vr", typeof(double));
            point_table.Columns.Add("Cn + Vcn&Vr", typeof(double));
            point_table.Columns.Add("Row Back Norm&Rn + Vr", typeof(double));
            point_table.Columns.Add("Col Back Norm&Cn + Vc", typeof(double));
            point_table.Columns.Add("Outlier", typeof(bool));
        
     
            int i;
            dataGridView1.Top = groupBox3.Height+30;
            dataGridView1.Height = this.Height - dataGridView1.Top - 50;
            dataGridView1.Width = this.Width - 50;
            groupBox2.Width = dataGridView1.Width - groupBox2.Left;
          
          
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

            comboBox2.DataSource = Form1.Imagelist;
            comboBox2.SelectedIndex = Form1.Image_ID;
            comboBox3.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;

            int colwidth = (dataGridView1.Width - 50) / 10;

            for (i = 1; i < 11; i++)
                dataGridView1.Columns[i].Width = colwidth;

            if ((Form1.gcp_id != 0) || (Form1.icp_id != 0))
            {
                check_control = new int[Form1.gcp_id + Form1.icp_id];

                for (i = 0; i < Form1.gcp_id; i++)
                    check_control[i] = 1;
                for (i = Form1.gcp_id; i <check_control.Length; i++)
                    check_control[i] = 0;
                
                data_disign(comboBox2.SelectedIndex);
            }

            dataGridView1.Columns[5].HeaderText = ((char)177).ToString() + " vr";
            dataGridView1.Columns[7].HeaderText = ((char)177).ToString() + " vc";
        
            first_open = 1;
        }
        private void GeoTrans_FormClosed(object sender, FormClosedEventArgs e)
        {
            first_open = 0;
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

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            // For Degree of freedom
            label22.Text = degreeoffreedom().ToString();
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if((comboBox3.SelectedIndex==4)||(comboBox3.SelectedIndex == 5))//
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
            if ((comboBox3.SelectedIndex == 2) || (comboBox3.SelectedIndex == 3))
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
                    label21.Enabled = true;
                    comboBox5.Enabled = true;
                }
                if (comboBox3.SelectedIndex == 3)
                {
                    comboBox5.Items.Clear();
                    comboBox5.Items.Add("Generic affine projection");
                    comboBox5.Items.Add("AP for OrbView - 3");
                    comboBox5.Items.Add("AP for IKONOS & QuickBird");
                    comboBox5.SelectedIndex = 0;
                }
            }
            else
            {
                label21.Enabled = false;
                comboBox5.Enabled = false;
            }

            // For Degree of freedom
           label22.Text=degreeoffreedom().ToString();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (first_open == 1)
            {
                Form1 f = Application.OpenForms["Form1"] as Form1;
                f.Image_ID_change(comboBox2.SelectedIndex);

                if ((Form1.gcp_id != 0) || (Form1.icp_id != 0))
                {
                    point_table.Rows.Clear();
                    dataGridView1.Rows.Clear();
                    check_control = new int[Form1.gcp_id + Form1.icp_id];
                    int i = 0;
                    for (i = 0; i < Form1.gcp_id; i++)
                        check_control[i] = 1;
                    for (i = Form1.gcp_id; i < check_control.Length; i++)
                        check_control[i] = 0;

                    data_disign(comboBox2.SelectedIndex);
                }
                else
                {
                    dataGridView1.Rows.Clear();
                    point_table.Rows.Clear();
                }
            }
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((first_open == 1) && (e.ColumnIndex==0))
                check_dzyn();
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            image_dzyn();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            image_dzyn();
            pictureBox1.Refresh();
            pictureBox2.Refresh();
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

            
            Matrix<double> A = jacobian();
            adj(A);

            label15.Text = ((char)177).ToString() + mr.ToString("0.00");
            label16.Text = ((char)177).ToString() + mc.ToString("0.00");
            label17.Text = ((char)177).ToString() + mo[mo.Length - 1].ToString("0.00");
            label18.Text = vrow_total.ToString("0.00");
            label19.Text = vcol_total.ToString("0.00");
        }
    }
}