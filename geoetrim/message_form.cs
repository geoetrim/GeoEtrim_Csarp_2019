using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSGeo.GDAL;

namespace GeoEtrim
{
    public partial class message_form : Form
    {
        public enum status_value
        {
            save,
            info_img,
            info_emp,
            info_proje,
        };
        static status_value situations = status_value.save;
        static int id;
        /// <summary>
        /// How should the form be opened?
        /// </summary>
        /// <param name="status">situations: save, info...</param>
        /// <param name="img_prj_no">which image or project information</param>
        public message_form(status_value status, int img_prj_no = -1) 
        {
            id = img_prj_no;
            situations = status;
            InitializeComponent();
        }
        private void message_form_Shown(object sender, EventArgs e)
        {
            Form1 f = Application.OpenForms["Form1"] as Form1;
            if (situations == status_value.save)
            {
                this.Text = "";
                this.ControlBox = false;
                timer1.Enabled = true;
                this.Size = new Size(140, 140);
                pictureBox1.Visible = true;
                label5.Visible = true;
                richTextBox1.Visible = false;
                dataGridView1.Visible = false;
                label1.Visible = false;
                label2.Visible = false;
            }
            if ((situations == status_value.info_img) || (situations == status_value.info_emp))
            {
                this.Text = "Information";
                this.ControlBox = true;
                timer1.Enabled = false;              
                this.Size = new Size(360, 440);
                pictureBox1.Visible = false;
                label5.Visible = false;
                dataGridView1.Visible = false;
                label1.Visible = false;
                label2.Visible = false;
                richTextBox1.Visible = true;
                richTextBox1.Location = new Point(0, 0);
                richTextBox1.Text = "";
                if ((id == -1) || (Form1.Project_ID == -1))
                    richTextBox1.Text = "no selected image or project!";
                else
                {
                    if (situations == status_value.info_img)
                    {
                        Gdal.AllRegister();
                        Dataset data_igdal = Gdal.Open(Form1.Imagelist[id],
                            Access.GA_ReadOnly);
                        richTextBox1.Text = Gdal.GDALInfo(data_igdal, null);
                    }
                    else richTextBox1.Text = "Image name: " +
                            f.treeView1.Nodes[Form1.Project_ID].Nodes[id].Text.Substring(7)
                            + "\n" + "Back color: " + Empty_image.empty_page_clr.Name
                            + "\n" + "Bands: 0"
                            + "\n" + "Page width: " + Form1.main_width[id].ToString()
                            + "\n" + "Page height: " + Form1.main_height[id].ToString()
                            + "\n" + "GCP count: " + Form1.gcp_collected[id].Rows.Count.ToString()
                            + "\n" + "ICP count: " + Form1.icp_collected[id].Rows.Count.ToString();
                }
            }
            if (situations == status_value.info_proje)
            {
                this.Text = "Information";
                this.ControlBox = true;
                timer1.Enabled = false;
                this.Size = new Size(645, 275);
                pictureBox1.Visible =false;
                label5.Visible = false;
                richTextBox1.Visible = false;
                dataGridView1.Visible = true;
                label1.Visible = true;
                label2.Visible = true;
                label1.Text = "Proje name: " + f.treeView1.Nodes[id].Text.Substring(9);
                label2.Text = "Proje Information: " + Form1.proje_info[id];
                int i;
                for (i = 0; i < Form1.Imagelist.Count; i++)
                {
                    dataGridView1.Rows.Add(f.treeView1.Nodes[id].Nodes[i].Text.Substring(7),
                        Form1.bants_img_count[i].ToString(), Form1.main_width[i].ToString(),
                          Form1.main_height[i].ToString(),Form1.gcp_collected[i].Rows.Count,
                          Form1.icp_collected[i].Rows.Count);
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
        private void message_form_Load(object sender, EventArgs e)
        {
            Form1 f = Application.OpenForms["Form1"] as Form1;
            formgcp form = Application.OpenForms["formgcp"] as formgcp;
            this.Owner = f;
            if (formgcp.Form_active == true)
                this.Owner = form;
        }
    }
}
