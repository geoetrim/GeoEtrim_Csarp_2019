using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoEtrim
{
    public partial class GeoTrans : Form
    {
        public GeoTrans()
        {
            InitializeComponent();
        }
        static DataTable point_table = new DataTable();
        void data_disign(int id)
        {
            point_table = new DataTable();
            point_table.Columns.Add("Point ID", typeof(int));
            point_table.Columns.Add("Point Tybe", typeof(string));
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
            point_table.Columns.Add("Vrn", typeof(double));
            point_table.Columns.Add("Vcn", typeof(double));
            point_table.Columns.Add("Rn + Vrn", typeof(double));
            point_table.Columns.Add("Cn + Vcn", typeof(double));
            point_table.Columns.Add("Row Back Norm", typeof(double));
            point_table.Columns.Add("Column Back Norm", typeof(double));
            point_table.Columns.Add("Outlier", typeof(bool));
            int i;
            for (i = 0; i < Form1.gcp_points[id].Rows.Count; i++)
                point_table.Rows.Add(Form1.gcp_points[id].Rows[i].ItemArray);
            for (i = 0; i < Form1.icp_points[id].Rows.Count; i++)
                point_table.Rows.Add(Form1.icp_points[id].Rows[i].ItemArray);
            for (i = 0; i < point_table.Rows.Count; i++)
                dataGridView1.Rows.Add();
            dataGridView1.Rows[0].Cells[5].Value = "5";
                 
            

        }
        private void GeoTrans_Load(object sender, EventArgs e)
        {
            comboBox2.DataSource = Form1.Imagelist;
            comboBox2.SelectedIndex = Form1.Image_ID;
            comboBox3.SelectedIndex = 0;
            int colwidth = dataGridView1.Width / 10;
            int i;
            for(i=0;i<10;i++)
            dataGridView1.Columns[i].Width = colwidth;
            data_disign(Form1.Image_ID);
        }

        private void dataGridView1_SizeChanged(object sender, EventArgs e)
        {
            int colwidth = dataGridView1.Width / 10;
            int i;
            for (i = 0; i < 10; i++)
                dataGridView1.Columns[i].Width = colwidth;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if((comboBox3.SelectedIndex==3)||(comboBox3.SelectedIndex == 4))
            {
                label7.Enabled = true;
                numericUpDown2.Enabled = true;
            }
            else
            {
                label7.Enabled = false;
                numericUpDown2.Enabled = false;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
    }
}
