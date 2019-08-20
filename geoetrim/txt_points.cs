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
    public partial class txt_points : Form
    {
        public txt_points()
        {
            InitializeComponent();
        }
        private void txt_points_Load(object sender, EventArgs e)
        {
            Form1 form = Application.OpenForms["Form1"] as Form1;
            this.Owner = form;

            int i = 0;
            for (i = 0; i < 12; i++)
                checkedListBox1.SetItemChecked(i, true);
            comboBox1.SelectedIndex = 3;
            comboBox2.SelectedIndex = 0;
            for (i = 0; i < Form1.Imagelist.Count; i++)
                comboBox3.Items.Add(form.treeView1.Nodes[Form1.Project_ID].
                    Nodes[i].Text.Substring(7));
            comboBox3.SelectedIndex = Form1.Image_ID;
        }
        static int down = 0;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (down == 0) pictureBox1.BorderStyle = BorderStyle.FixedSingle;
        }
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.BorderStyle = BorderStyle.None;
            down = 0;
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            down = 1;
            if (checkedListBox1.SelectedItem != null)
            {
                object selected = checkedListBox1.SelectedItem;
                int i = checkedListBox1.SelectedIndex;
                if (i < checkedListBox1.Items.Count-1)
                {
                    bool bl = checkedListBox1.GetItemChecked(i);
                    checkedListBox1.Items.Remove(selected);
                    checkedListBox1.Items.Insert(i + 1, selected);
                    checkedListBox1.SetSelected(i + 1, true);
                    checkedListBox1.SetItemChecked(i + 1, bl);
                }
            }
            else MessageBox.Show("No selected column!");
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.BorderStyle = BorderStyle.None;
            down = 0;
       
        }

        //
        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (down == 0) pictureBox2.BorderStyle = BorderStyle.FixedSingle;
        }
        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.BorderStyle = BorderStyle.None;
            down = 0;
        }
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox2.BorderStyle = BorderStyle.Fixed3D;
            down = 1;
            if (checkedListBox1.SelectedItem != null)
            {
                object selected = checkedListBox1.SelectedItem;
                int i = checkedListBox1.SelectedIndex;
                if (i > 0)
                {
                    bool bl = checkedListBox1.GetItemChecked(i);
                    checkedListBox1.Items.Remove(selected);
                    checkedListBox1.Items.Insert(i - 1, selected);
                    checkedListBox1.SetSelected(i - 1, true);
                    checkedListBox1.SetItemChecked(i - 1, bl);
                }
            }
            else MessageBox.Show("No selected column!");

        }
        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox2.BorderStyle = BorderStyle.None;
            down = 0;
        }
        int col_seq(string text)
        {
            int i =0,index = 0;
            string[] datacol = {"Point ID", "Point Type" ,"Row", "Column", "Easting (X)" ,"Northing (Y)"
                    ,"Height (Z)","Std Row","Std Column" ,"Std X" ,"Std Y" ,"Std Z"};
            for (i = 0; i < 12; i++)
                if (text == datacol[i]) index = i;
                return index;
        }
        private void button2_Click(object sender, EventArgs e)
        {  
            // for selected column(s) and design of column width
            int working_id = comboBox3.SelectedIndex;
            string coor_decimal = "f" + numericUpDown1.Value.ToString();
            string std_decimal = "f" + numericUpDown2.Value.ToString();         
            if ((checkBox1.Checked == false) && (checkBox2.Checked == false))
                MessageBox.Show("Select the point type(s)!");
            else
            {
                int j, i;
                int[] writing_col = new int[0];
                for (i = 0; i < 12; i++)
                {
                    if (checkedListBox1.GetItemChecked(i) == true)
                    {
                        Array.Resize<int>(ref writing_col, writing_col.Length + 1);
                        writing_col[writing_col.Length - 1] = col_seq(checkedListBox1.Items[i].ToString());
                    }
                }
               
                int[] max_col_char = new int[writing_col.Length];
                for (j = 0; j < writing_col.Length; j++)
                {
                    int[] scol = new int[0];
                    if ((checkBox1.Checked == true) && (checkBox2.Checked == false))
                    {
                        scol = new int[Form1.gcp_collected[working_id].Rows.Count];
                        for (i = 0; i < Form1.gcp_collected[working_id].Rows.Count; i++)
                        {
                            if ((writing_col[j] == 0) || (writing_col[j] == 1))
                                scol[i] = Form1.gcp_collected[working_id].Rows[i][writing_col[j]].ToString().ToCharArray().Length;
                            if ((writing_col[j] == 2) || (writing_col[j] == 3) || (writing_col[j] == 4) ||
                               (writing_col[j] == 5) || (writing_col[j] == 6))
                                scol[i] =Convert.ToDouble( Form1.gcp_collected[working_id].Rows[i][writing_col[j]]).
                                    ToString(coor_decimal).ToCharArray().Length;
                            if ((writing_col[j] == 7) || (writing_col[j] == 8) || (writing_col[j] == 9) ||
                            (writing_col[j] == 10) || (writing_col[j] == 11))
                                scol[i] = Convert.ToDouble(Form1.gcp_collected[working_id].Rows[i][writing_col[j]]).
                                    ToString(std_decimal).ToCharArray().Length;
                        }
                    }
                    if ((checkBox1.Checked == false) && (checkBox2.Checked == true))
                    {
                        scol = new int[Form1.icp_collected[working_id].Rows.Count];
                        for (i = 0; i < Form1.icp_collected[working_id].Rows.Count; i++)
                        {
                            if ((writing_col[j] == 0) || (writing_col[j] == 1))
                                scol[i] = Form1.icp_collected[working_id].Rows[i][writing_col[j]].ToString().ToCharArray().Length;
                            if ((writing_col[j] == 2) || (writing_col[j] == 3) || (writing_col[j] == 4) ||
                               (writing_col[j] == 5) || (writing_col[j] == 6))
                                scol[i] = Convert.ToDouble(Form1.icp_collected[working_id].Rows[i][writing_col[j]]).
                                    ToString(coor_decimal).ToCharArray().Length;
                            if ((writing_col[j] == 7) || (writing_col[j] == 8) || (writing_col[j] == 9) ||
                            (writing_col[j] == 10) || (writing_col[j] == 11))
                                scol[i] = Convert.ToDouble(Form1.icp_collected[working_id].Rows[i][writing_col[j]]).
                                    ToString(std_decimal).ToCharArray().Length;
                        }
                    }
                    if ((checkBox1.Checked == true) && (checkBox2.Checked == true))
                    {
                        scol = new int[Form1.gcp_collected[working_id].Rows.Count+
                            Form1.icp_collected[working_id].Rows.Count];
                        for (i = 0; i < Form1.gcp_collected[working_id].Rows.Count; i++)
                        {
                            if ((writing_col[j] == 0) || (writing_col[j] == 1))
                                scol[i] = Form1.gcp_collected[working_id].Rows[i][writing_col[j]].ToString().ToCharArray().Length;
                            if ((writing_col[j] == 2) || (writing_col[j] == 3) || (writing_col[j] == 4) ||
                               (writing_col[j] == 5) || (writing_col[j] == 6))
                                scol[i] = Convert.ToDouble(Form1.gcp_collected[working_id].Rows[i][writing_col[j]]).
                                    ToString(coor_decimal).ToCharArray().Length;
                            if ((writing_col[j] == 7) || (writing_col[j] == 8) || (writing_col[j] == 9) ||
                            (writing_col[j] == 10) || (writing_col[j] == 11))
                                scol[i] = Convert.ToDouble(Form1.gcp_collected[working_id].Rows[i][writing_col[j]]).
                                    ToString(std_decimal).ToCharArray().Length;
                        }
                        for (i = 0; i < Form1.icp_collected[working_id].Rows.Count; i++)
                        {
                            if ((writing_col[j] == 0) || (writing_col[j] == 1))
                                scol[Form1.gcp_collected[working_id].Rows.Count+i] = Form1.icp_collected[working_id]
                                    .Rows[i][writing_col[j]].ToString().ToCharArray().Length;
                            if ((writing_col[j] == 2) || (writing_col[j] == 3) || (writing_col[j] == 4) ||
                               (writing_col[j] == 5) || (writing_col[j] == 6))
                                scol[Form1.gcp_collected[working_id].Rows.Count+i] = Convert.ToDouble(Form1.icp_collected[working_id].
                                    Rows[i][writing_col[j]]).ToString(coor_decimal).ToCharArray().Length;
                            if ((writing_col[j] == 7) || (writing_col[j] == 8) || (writing_col[j] == 9) ||
                            (writing_col[j] == 10) || (writing_col[j] == 11))
                                scol[Form1.gcp_collected[working_id].Rows.Count+i] = Convert.ToDouble(Form1.icp_collected[working_id].
                                    Rows[i][writing_col[j]]).ToString(std_decimal).ToCharArray().Length;
                        }
                    }
                    if(checkBox3.Checked==true)
                    {
                        Array.Resize<int>(ref scol, scol.Length + 1);
                        scol[scol.Length-1]= Form1.gcp_collected[working_id].Columns[writing_col[j]].ToString().ToCharArray().Length;
                    }
                    max_col_char[j] = scol.Max();
                }

                // for "Export to text" part                
                string filepath = textBox1.Text + "/" + textBox2.Text+".gcp";
                StreamWriter sw = File.CreateText(filepath);
                string sequen = comboBox1.SelectedIndex.ToString()+"/";
                string[] list_column = {"Point ID", "Point Type", "Row" ,"Column",
                    "Easting (X)", "Northing (Y)", "Height (Z)", "Std Row", "Std Column",
                    "Std X", "Std Y", "Std Z"};
                if (checkBox3.Checked == true) sequen = sequen + "1/";
                else sequen = sequen = sequen + "0/";
                int code, delta_code;                
                for (i = 0; i < list_column.Length; i++)
                {
                    code = checkedListBox1.Items.IndexOf(list_column[i]);
                    delta_code = 0;                  
                    if (checkedListBox1.GetItemChecked(code) == false) code = -1;
                    else for (j = 0; j < code; j++)
                            if (checkedListBox1.GetItemChecked(j) == false)
                                delta_code = delta_code + 1;
                    code = code - delta_code;
                    sequen = sequen + code.ToString() + "/";
                }
                sw.WriteLine("Import sequence: " + sequen);
                sw.WriteLine("Image file: " + comboBox3.Items[comboBox3.SelectedIndex].ToString());
                sw.WriteLine("");
                string line = "";
                char sep_decimal='.';
                if(comboBox2.SelectedIndex==0) sep_decimal = '.';
                if (comboBox2.SelectedIndex == 1) sep_decimal = ',';
                string col_separate = "     ";
                if (comboBox1.SelectedIndex == 0) col_separate = " ";
                if (comboBox1.SelectedIndex == 1) col_separate = "/";
                if (comboBox1.SelectedIndex == 2) col_separate = "_";
                if (comboBox1.SelectedIndex == 3) col_separate = "\t";
                if (comboBox1.SelectedIndex == 4) col_separate = ";";
                if (checkBox3.Checked == true)
                {
                    line = "";
                    for (j = 0; j < writing_col.Length; j++)
                        line = line + Form1.gcp_collected[0].Columns[writing_col[j]].ColumnName.PadLeft(max_col_char[j]) + col_separate;
                    sw.WriteLine(line);
                }
                    if (checkBox1.Checked == true)
                {
                    for (i = 0; i < Form1.gcp_collected[working_id].Rows.Count; i++)
                    {
                        line = "";
                        for (j = 0; j < writing_col.Length; j++)
                        {
                            if((writing_col[j]==0)|| (writing_col[j] == 1))
                            line = line + Form1.gcp_collected[working_id].Rows[i][writing_col[j]].ToString().PadLeft(max_col_char[j]) + col_separate;
                            if ((writing_col[j] == 2) || (writing_col[j] == 3) || (writing_col[j] == 4) ||
                                (writing_col[j] == 5) || (writing_col[j] == 6))
                                line = line + Convert.ToDouble(Form1.gcp_collected[working_id].Rows[i][writing_col[j]].ToString()).ToString(coor_decimal).
                                    PadLeft(max_col_char[j]).Replace(',', sep_decimal) + col_separate;
                            if ((writing_col[j] == 7) || (writing_col[j] == 8) || (writing_col[j] == 9) ||
                            (writing_col[j] == 10) || (writing_col[j] == 11))
                                line = line + Convert.ToDouble(Form1.gcp_collected[working_id].Rows[i][writing_col[j]].ToString()).ToString(std_decimal).
                                    PadLeft(max_col_char[j]).Replace(',', sep_decimal) + col_separate;
                        }
                      sw.WriteLine(line);
                    }
                }            
                if (checkBox2.Checked == true)
                {
                    for (i = 0; i < Form1.icp_collected[working_id].Rows.Count; i++)
                    {
                        line = "";
                        for (j = 0; j < writing_col.Length; j++)
                        {
                            if ((writing_col[j] == 0) || (writing_col[j] == 1))
                                line = line + Form1.icp_collected[working_id].Rows[i][writing_col[j]].ToString().PadLeft(max_col_char[j]) + col_separate;
                            if ((writing_col[j] == 2) || (writing_col[j] == 3) || (writing_col[j] == 4) ||
                                (writing_col[j] == 5) || (writing_col[j] == 6))
                                line = line + Convert.ToDouble(Form1.icp_collected[working_id].Rows[i][writing_col[j]].ToString()).ToString(coor_decimal).
                                    PadLeft(max_col_char[j]).Replace(',', sep_decimal) + col_separate;
                            if ((writing_col[j] == 7) || (writing_col[j] == 8) || (writing_col[j] == 9) ||
                            (writing_col[j] == 10) || (writing_col[j] == 11))
                                line = line + Convert.ToDouble(Form1.icp_collected[working_id].Rows[i][writing_col[j]].ToString()).ToString(std_decimal).
                                    PadLeft(max_col_char[j]).Replace(',', sep_decimal) + col_separate;
                        }
                        sw.WriteLine(line);
                    }
                }
                sw.Close();
                MessageBox.Show("Done");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }
    }
}
