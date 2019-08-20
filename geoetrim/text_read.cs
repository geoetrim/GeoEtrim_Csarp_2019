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

namespace GeoEtrim
{
    public partial class text_read: Form
    {
        static string file;
        public text_read(string text_file)
        {      
            file = text_file;
            InitializeComponent();
        }
       
        private void text_read_Load(object sender, EventArgs e)
        {
            StreamReader reader = new StreamReader(file);
            string line = reader.ReadLine(); ;
            string[] column_line;
            int i, row, dat;
            dat = 0;
            row = 0;
            FileInfo exten = new FileInfo(file);
            string file_exten = exten.Extension;
            char line_split = '\t';
            int[] sequen = new int[14];
            sequen[1] = 0;
            sequen[2] = 0;
            sequen[3] = 1;
            sequen[4] = 2;
            sequen[5] = 3;
            sequen[6] = 4;
            sequen[7] = 5;
            sequen[8] = 6;
            sequen[9] = 7;
            sequen[10] = 8;
            sequen[11] = 9;
            sequen[12] = 10;
            sequen[13] = 11;
            if (file_exten==".gcp")
            {
                column_line = line.Substring(16, line.Length - 17).Split('/');
                for (i = 0; i < 14; i++)
                    sequen[i] = Convert.ToInt32(column_line[i]);
                if (sequen[0] == 0) line_split = ' ';
                if (sequen[0] == 1) line_split = '/';
                if (sequen[0] == 2) line_split = '_';
                if (sequen[0] == 3) line_split = '\t';
                if (sequen[0] == 4) line_split = ';';
                line = reader.ReadLine();
                line = reader.ReadLine();
                line = reader.ReadLine();
                if (sequen[1] == 1) line = reader.ReadLine();
            }
            
            while (line != null)
            {
                column_line = line.Split(line_split);
                dataGridView1.Rows.Add();
                if (System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.ToString() == ",")
                    dat = 0;
                else
                    dat = 1;
                for (i = 0; i < 12; i++)
                {
                    if (sequen[2 + i] > -1)
                    {
                        if (dat == 0) dataGridView1.Rows[row].Cells[i].Value = column_line[sequen[2 + i]].Replace(" ", "").Replace('.', ',');
                        if (dat == 1) dataGridView1.Rows[row].Cells[i].Value = column_line[sequen[2 + i]].Replace(" ", "").Replace(',', '.');
                    }
                    else
                    {
                        if (i == 0) dataGridView1.Rows[row].Cells[i].Value = 100 + row;
                        if (i == 1) dataGridView1.Rows[row].Cells[i].Value = "Control";
                        if ((i != 0) && (i != 1)) dataGridView1.Rows[row].Cells[i].Value = 0;
                    }
                }
                row = row + 1;
                line = reader.ReadLine();
            }
            
            if (Empty_image.active_form == false)
                label21.Text = Form1.Imagelist[Form1.Image_ID];
            else
            {
                Empty_image empty = Application.OpenForms["Empty_image"] as Empty_image;
                label21.Text = empty.textBox1.Text;
            }
            label3.Text = row.ToString();
           
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int i, j;

            if (Empty_image.active_form == true)
            {
                Empty_image empty = Application.OpenForms["Empty_image"] as Empty_image;
                double[] r = new double[dataGridView1.RowCount];
                double[] c = new double[dataGridView1.RowCount];
                for (i = 0; i < dataGridView1.RowCount; i++)
                {
                    r[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value.ToString());
                    c[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value.ToString());
                }
                if (GeoTransform.form_active == false)
                {
                    Empty_image.emp_Width = Convert.ToInt32(c.Max() + 100);
                    Empty_image.emp_Height = Convert.ToInt32(r.Max() + 100);
                }
                else
                {
                    Empty_image.emp_Width = 1100;
                    Empty_image.emp_Height = 1100;
                }
                Visual v = new Visual();
                v.open_Image(empty.textBox1.Text, true, true);
                empty.Close();
            }
            for (i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[1].Value.ToString() == "Control")
                {
                    Form1.gcp_collected[Form1.Image_ID].Rows.Add("-2");
                    Form1.gcp_collected[Form1.Image_ID].Rows[Form1.gcp_count][0] = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                    Form1.gcp_collected[Form1.Image_ID].Rows[Form1.gcp_count][1] = dataGridView1.Rows[i].Cells[1].Value;
                    for (j = 2; j < 12; j++)
                        Form1.gcp_collected[Form1.Image_ID].Rows[Form1.gcp_count][j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value);
                    Form1.gcp_count = Form1.gcp_count + 1;
                }
                if (dataGridView1.Rows[i].Cells[1].Value.ToString() == "Check")
                {
                    Form1.icp_collected[Form1.Image_ID].Rows.Add("-2");
                    Form1.icp_collected[Form1.Image_ID].Rows[Form1.icp_count][0] = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                    Form1.icp_collected[Form1.Image_ID].Rows[Form1.icp_count][1] = dataGridView1.Rows[i].Cells[1].Value;
                    for (j = 2; j < 12; j++)
                        Form1.icp_collected[Form1.Image_ID].Rows[Form1.icp_count][j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value);
                    Form1.icp_count = Form1.icp_count + 1;
                }
            }
            if (GeoTransform.form_active == true)
            {
                GeoTransform geo = Application.OpenForms["GeoTransform"] as GeoTransform;
                geo.scale_control = 1;
                geo.load_dzyn();

            }
            this.Close();
        }
    }
}
