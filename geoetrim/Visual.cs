using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using OSGeo.GDAL;
using System.Threading;
   
namespace GeoEtrim
{
    class Visual
    {
        //
        // Private fonksyon
        //
        void Button_control()
        {
            Form1 form = Application.OpenForms["Form1"] as Form1;
            if (Form1.Project_ID == -1)
            {
                form.toolStripButton3.Enabled = false;// Save proje sortcut
                form.toolStripButton5.Enabled = false;// Empty image sortcut
                form.toolStripButton7.Enabled = false;// Add image sortcut
            }
            else
            {
                form.toolStripButton3.Enabled = true;// Save proje sortcut
                form.toolStripButton5.Enabled = true;// Empty image sortcut
                form.toolStripButton7.Enabled = true;// Add image sortcut
            }

            if (Form1.Image_ID == -1)
            {
                form.toolStripButton6.Enabled = false;// Delete image sortcut          
                form.toolStripButton8.Enabled = false;// Add points sortcut
                form.toolStripButton9.Enabled = false;// Zoom Out sortcut
                form.toolStripButton10.Enabled = false;// Zoom in sortcut
                form.toolStripButton11.Enabled = false;// Zoom fix sortcut
                form.toolStripButton12.Enabled = false;// Must left sortcut
                form.toolStripButton13.Enabled = false;// Must Right sortcut
                form.toolStripButton14.Enabled = false;// Top sortcut
                form.toolStripButton15.Enabled = false;// Buttom sortcut
                form.toolStripButton16.Enabled = false;// Contrast sortcut
                form.toolStripButton17.Enabled = false;// image fix sortcut
            }
            else
            {
                form.toolStripButton6.Enabled = true;// Delete image sortcut          
                form.toolStripButton8.Enabled = true;// Add point sortcut
                form.toolStripButton9.Enabled = true;// Zoom Out sortcut
                form.toolStripButton10.Enabled = true;// Zoom in sortcut
                form.toolStripButton11.Enabled = true;// Zoom fix sortcut
                form.toolStripButton12.Enabled = true;// Must left sortcut
                form.toolStripButton13.Enabled = true;// Must Right sortcut
                form.toolStripButton14.Enabled = true;// Top sortcut
                form.toolStripButton15.Enabled = true;// Buttom sortcut
                form.toolStripButton16.Enabled = true;// Contrast sortcut
                form.toolStripButton17.Enabled = true;// image fix sortcut
            }
        }
        void Load(string open)
        {
            Clearprj();

            Form1 form = Application.OpenForms["Form1"] as Form1;
            XmlDocument proje = new XmlDocument();
            proje.Load(open);
            XmlNode root = proje.LastChild;
            form.treeView1.Nodes.Add("Project: " + root.Attributes["Project_name"].Value.ToString());
            form.treeView1.Nodes[0].ImageIndex = 0;
            Form1.proje_path = open;
            Form1.Project_ID = 0;
            form.treeView1.SelectedNode = form.treeView1.Nodes[Form1.Project_ID];
            Form1.proje_info.Add(root.Attributes["Information"].Value.ToString());
            Button_control();
            int i = 0;
            for (i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlNode Image_xml = root.ChildNodes[i];
                int bants = Convert.ToInt32(Image_xml.Attributes["bants"].Value);
                Form1.Imagelist.Add(Image_xml.Attributes["href"].Value.ToString());
                Form1.Image_ID = Form1.Image_ID + 1;
                Array.Resize<int>(ref Form1.main_width, Form1.main_width.Length + 1);
                Array.Resize<int>(ref Form1.main_height, Form1.main_height.Length + 1);
                Form1.main_width[Form1.Image_ID] = Convert.ToInt32(Image_xml.Attributes["width"].Value);
                Form1.main_height[Form1.Image_ID] = Convert.ToInt32(Image_xml.Attributes["height"].Value);

                treenodes_add_img(Image_xml.Attributes["name"].Value.ToString(), bants, true);
                Form1.bants_img_count.Add(bants);
                int j;
                for (j = 0; j < Image_xml.ChildNodes.Count; j++)
                {
                    XmlNode point = Image_xml.ChildNodes[j];
                    if ("Control" == point.Attributes[1].Value)
                    {
                        Array.Resize<DataTable>(ref Form1.gcp_collected, Form1.gcp_collected.Length + 1);
                        Form1.gcp_collected[i].Rows.Add(point.Attributes[0].Value, point.Attributes[1].Value
                            , point.Attributes[2].Value, point.Attributes[3].Value, point.Attributes[4].Value,
                            point.Attributes[5].Value, point.Attributes[6].Value, point.Attributes[7].Value,
                            point.Attributes[8].Value, point.Attributes[9].Value, point.Attributes[10].Value,
                            point.Attributes[11].Value);
                        Form1.gcp_count = Form1.gcp_count + 1;
                    }
                    if ("Check" == point.Attributes[1].Value)
                    {
                        Array.Resize<DataTable>(ref Form1.icp_collected, Form1.icp_collected.Length + 1);
                        Form1.icp_collected[i].Rows.Add(point.Attributes[0].Value, point.Attributes[1].Value
                            , point.Attributes[2].Value, point.Attributes[3].Value, point.Attributes[4].Value,
                            point.Attributes[5].Value, point.Attributes[6].Value, point.Attributes[7].Value,
                            point.Attributes[8].Value, point.Attributes[9].Value, point.Attributes[10].Value,
                            point.Attributes[11].Value);
                        Form1.icp_count = Form1.icp_count + 1;
                    }
                }
            }
            Form1.Image_ID = -1; // Because treeview of nodes is not close 
            Form1.Save_Control = 0;
            form.treeView1.Refresh();
            form.treeView1.Nodes[0].Expand();
          
        }
        void Clearprj()
        {
            Form1 form = Application.OpenForms["Form1"] as Form1;
            form.treeView1.Nodes.Clear();
            form.ımageBox1.Image = null;
            form.ımageBox2.Image = null;
            Form1.main_height = new int[0];
            Form1.main_width = new int[0];
            Form1.Image_ID = -1;
            Form1.Image_mini = null;
            Form1.Project_ID = -1;
            Form1.Imagelist = new List<string>();
            Form1.proje_path = null;
            Form1.Save_Control = 0;
            form.ımageBox1.HorizontalScrollBar.Visible = false;
            form.ımageBox1.VerticalScrollBar.Visible = false;
            form.ımageBox1.Refresh();

            Button_control();
            form.toolStripLabel5.Text = "Active: None";

            Form1.gcp_count = 0;
            Form1.gcp_collected = new DataTable[0];
            Form1.icp_count = 0;
            Form1.icp_collected = new DataTable[0];

        }
        void treenodes_add_img(string image_name, int bands, bool GCP_ICP_new)
        {
            Form1 form = Application.OpenForms["Form1"] as Form1;
            TreeView treeView1 = form.Controls["treeView1"] as TreeView;

            string[] bands_name = {"Red band", "Red histogram","Green band", "Green histogram","Blue band", "Blue histogram",
            "NIR band", "NIR histogram"};
            Color[] bands_color = { Color.Red, Color.Green, Color.Blue, Color.Orange };
            treeView1.Nodes[Form1.Project_ID].Nodes.Add("Image: " + image_name);

            if (bands == 1) treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].ImageIndex = 18;
            else treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].ImageIndex = 3;

            if (bands == 0)
                treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes.Add("Band (null)");
            if (bands == 1)
                treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes.Add("Band");
            if (bands > 1)
                treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes.Add("Bands");

            treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].ImageIndex = 4;

            treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes.Add("GCP/ICP");
            treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[1].ImageIndex = 19;

            if (bands == 1)
            {
                treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes.Add("Gray band");
                treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[0].ForeColor = Color.DarkGray;
                treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[0].ImageIndex = 6;
                treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[0].Nodes.Add("Gray histogram");
                treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[0].Nodes[0].ForeColor = Color.DarkGray;
                treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[0].Nodes[0].ImageIndex = 7;
            }
            if (bands >= 3)
            {
                int i;
                for (i = 0; i < bands; i++)
                {
                    if (i < 4)
                    {
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes.Add(bands_name[i * 2]);
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[i].ForeColor = bands_color[i];
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[i].ImageIndex = 8 + i * 2;
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[i].Nodes.Add(bands_name[(i * 2) + 1]);
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[i].Nodes[0].ForeColor = bands_color[i];
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[i].Nodes[0].ImageIndex = 9 + i * 2;
                    }
                    if (i >= 4)
                    {
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes.Add("Band " + (i + 1).ToString());
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[i].ForeColor = Color.Black;
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[i].ImageIndex = 16;
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[i].Nodes.Add("Band " + (i + 1).ToString() + " Histogram");
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[i].Nodes[0].ForeColor = Color.Black;
                        treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID].Nodes[0].Nodes[i].Nodes[0].ImageIndex = 17;
                    }
                }
            }
            ////////// For Gcp_data ////////////
            if (GCP_ICP_new == true)
            {
                Array.Resize<DataTable>(ref Form1.gcp_collected, Form1.gcp_collected.Length + 1);
                Form1.gcp_collected[Form1.Image_ID] = form.point_collumns_creat(Form1.gcp_collected[Form1.Image_ID]);
                Array.Resize<DataTable>(ref Form1.icp_collected, Form1.icp_collected.Length + 1);
                Form1.icp_collected[Form1.Image_ID] = form.point_collumns_creat(Form1.icp_collected[Form1.Image_ID]);
            }
        }
        //
        // Public fonksyon
        //
        public void Save_prj()
        {
            Form1 form = Application.OpenForms["Form1"] as Form1;
            TreeView treeView1 = form.Controls["treeView1"] as TreeView;

            XmlDocument proje = new XmlDocument();
            proje.Load(Form1.proje_path);
            XmlNode root = proje.LastChild;

            int i;
            for (i = root.ChildNodes.Count - 1; i >= 0; i--) root.ChildNodes[i].ParentNode.RemoveChild(root.ChildNodes[i]);

            for (i = 0; i < Form1.Imagelist.Count; i++)
            {
                XmlElement image = proje.CreateElement("Image");
                image.SetAttribute("name", treeView1.Nodes[Form1.Project_ID].Nodes[i].Text.Substring(7));
                image.SetAttribute("href", Form1.Imagelist[i]);
                image.SetAttribute("bants", treeView1.Nodes[Form1.Project_ID].Nodes[i].
                    Nodes[0].Nodes.Count.ToString());
                image.SetAttribute("width", Form1.main_width[i].ToString());
                image.SetAttribute("height", Form1.main_height[i].ToString());
                image.SetAttribute("id", i.ToString());
                root.AppendChild(image);
                int j, k;
                for (j = 0; j < Form1.gcp_collected[i].Rows.Count; j++)
                {
                    XmlElement gcp_xml = proje.CreateElement("gcp");
                    for (k = 0; k < 12; k++)
                        gcp_xml.SetAttribute(Form1.gcp_collected[i].Columns[k].ColumnName, Form1.gcp_collected[i].Rows[j][k].ToString());
                    image.AppendChild(gcp_xml);
                }
                for (j = 0; j < Form1.icp_collected[i].Rows.Count; j++)
                {
                    XmlElement icp_xml = proje.CreateElement("icp");
                    for (k = 0; k < 12; k++)
                        icp_xml.SetAttribute(Form1.icp_collected[i].Columns[k].ColumnName, Form1.icp_collected[i].Rows[j][k].ToString());
                    image.AppendChild(icp_xml);
                }
            }

            proje.Save(Form1.proje_path);
            Form1.Save_Control = 0;
        }
        public void New_prj()
        {
            New_Canves f = new New_Canves();
            if (Form1.Project_ID != -1)
            {
                if (Form1.Save_Control == 1)
                {
                    string info;
                    info = MessageBox.Show("Project Changing" + "\n" + "\n" +
                        "Save this project?", "Information",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information).ToString();
                    if (info == "Yes")
                    {
                        Save_prj();
                        Clearprj();
                        f.ShowDialog();
                    }
                    if (info == "No")
                    {
                        Clearprj();
                        f.ShowDialog();
                    }
                }
                else
                {
                    string info;
                    info = MessageBox.Show("Project closing" + "\n" + "\n" +
                        "Active project will be closed. Would you like to continue?", "Information",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information).ToString();
                    if (info == "Yes")
                    {
                        Clearprj();
                        f.ShowDialog();
                    }

                }
            }
            else f.ShowDialog();
        }
        public int close_All()
        {
            int a = 1;
            Form1 f = Application.OpenForms["Form1"] as Form1;
            if (Form1.Save_Control == 1)
            {
                string info;
                info = MessageBox.Show("Project Changing" + "\n" + "\n" +
                    "Save this project?", "Information",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information).ToString();
                if (info == "Yes") Save_prj();
                if (info == "Cancel") a = 0;
            }
            return a;
        }
        public void close_prj()
        {
            if (Form1.Save_Control == 1)
            {
                string info;
                info = MessageBox.Show("Project Changing" + "\n" + "\n" +
                    "", "Information",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information).ToString();
                if (info == "Yes")
                {
                    Save_prj();
                    Clearprj();
                }
                if (info == "No") Clearprj();
            }
            else Clearprj();
        }
        public void load_prj(string open)
        {
            Form1 f = Application.OpenForms["Form1"] as Form1;
            if (Form1.Save_Control == 1)
            {
                string info;
                info = MessageBox.Show("Project Changing" + "\n" + "\n" +
                    "Save this project?", "Information",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information).ToString();
                if (info == "Yes")
                {
                    Save_prj();
                    Load(open);
                }
                if (info == "No") Load(open);
            }
            else Load(open);
        }
   
        /// <summary>
        /// this method is for opening or changing the image.
        /// </summary>
        /// <param name="href_open">image filename</param>
        /// <param name="empty">true=no image / false=have image</param>
        /// <param name="new_img">true= new image / false= change image</param>
        public void open_Image(string href_open, bool empty, bool new_img)
        {
            try
            {
                OpenFileDialog opendio = new OpenFileDialog();
                opendio.FileName = href_open;
                href_open = opendio.SafeFileName;
                Form1 form = Application.OpenForms["Form1"] as Form1;
                int bit = 0, channels = 0;
                string format = null;
                if (empty != true) imginfo(opendio.FileName, out bit, out channels, out format);
                form.ımageBox1.Image = null;
                form.ımageBox2.Image = null;
                Bitmap mini_bmp= null;
                if (new_img == true)
                {
                    Form1.Image_ID = form.treeView1.Nodes[Form1.Project_ID].Nodes.Count;
                    Array.Resize<int>(ref Form1.main_width, Form1.main_width.Length + 1);
                    Array.Resize<int>(ref Form1.main_height, Form1.main_height.Length + 1);
                    Form1.gcp_count = 0;
                    Form1.icp_count = 0;
                }
                if (empty == true)
                {
                    if (new_img == true)
                    {
                        Form1.main_width[Form1.Image_ID] = Empty_image.emp_Width;
                        Form1.main_height[Form1.Image_ID] = Empty_image.emp_Height;
                    }

                    Image<Rgb, byte> main_Image = main_Image = new Image<Rgb, byte>(Form1.main_width[Form1.Image_ID],
                        Form1.main_height[Form1.Image_ID], new Rgb(Empty_image.empty_page_clr));  
                    form.ımageBox1.Image = main_Image;  
                    Form1.Main_Bants = main_Image.Split();
                    mini_bmp =new Bitmap(main_Image.Bitmap);
                    opendio.FileName = "empty";
                }
                else
                {
                    if (bit == 16)
                    {                
                        if (channels >= 3)
                        {
                            Image<Gray, ushort>[] bit_bants_16 = bantsimg_16(opendio.FileName);
                            Form1.Main_Bants = new Image<Gray, Byte>[channels];
                            int i;
                            for (i = 0; i < channels; i++)
                                Form1.Main_Bants[i] = bit_bants_16[i].Convert<Gray, byte>();
                            Image<Gray, byte>[] rgb = { Form1.Main_Bants[0], Form1.Main_Bants[1], Form1.Main_Bants[2] };
                            Image<Rgb, byte> main_Image = new Image<Rgb, byte>(rgb);
                            form.ımageBox1.Image = main_Image;
                            mini_bmp = new Bitmap(main_Image.Bitmap);
                        }
                        if (channels < 3)
                        {
                            Image<Gray, ushort>[] bit_bants_16 = bantsimg_16(opendio.FileName);
                            Form1.Main_Bants = new Image<Gray, Byte>[channels];
                            int i;              
                            for (i = 0; i < channels; i++)
                                Form1.Main_Bants[i] = bit_bants_16[i].Convert<Gray, byte>();
                            form.ımageBox1.Image = Form1.Main_Bants[0];
                            mini_bmp = new Bitmap(Form1.Main_Bants[0].Bitmap);
                        }
                    }
                    if (bit == 8)
                    {
                        if (channels >= 3)
                        {
                            Form1.Main_Bants = bantsimg_8(opendio.FileName);
                            Image<Gray, byte>[] rgb = { Form1.Main_Bants[0], Form1.Main_Bants[1], Form1.Main_Bants[2] };
                            Image<Rgb, byte> main_Image = new Image<Rgb, byte>(rgb);
                            form.ımageBox1.Image = main_Image;
                            mini_bmp = new Bitmap(main_Image.Bitmap);
                        }
                        if (channels < 3)
                        {
                            Form1.Main_Bants = bantsimg_8(opendio.FileName);
                            form.ımageBox1.Image = Form1.Main_Bants[0];            
                            mini_bmp = new Bitmap(Form1.Main_Bants[0].Bitmap);
                        }
                    }
                }
                if(new_img==true)
                {
                    Form1.bants_img_count.Add(channels);
                    treenodes_add_img(href_open, channels, true);
                    form.treeView1.SelectedNode = form.treeView1.Nodes[Form1.Project_ID].Nodes[Form1.Image_ID];
                    Form1.Imagelist.Add(opendio.FileName);
                }
                Form1.main_width[Form1.Image_ID] = Form1.Main_Bants[0].Width;
                Form1.main_height[Form1.Image_ID] = Form1.Main_Bants[0].Height;
                form.decrease(mini_bmp);
                Form1.zoom = 1;
                Form1.cnt_zoom = 0;
                form.follow_window();
                form.ımageBox1.HorizontalScrollBar.Value = 0;
                form.ımageBox1.VerticalScrollBar.Value = 0;
                form.zoom_in(new Point(0, 0));
                form.zoom_out(new Point(0, 0));
                form.radioButton1.Checked = true;
                Form1.Save_Control = 1;
                Button_control();
                if (formgcp.Form_active == true)
                {
                    formgcp ff = Application.OpenForms["formgcp"] as formgcp;
                    ff.dzy_main();
                }
            }
            catch (Exception mistake)
            {
                MessageBox.Show(mistake.Message.ToString());
            }
          
        }
        public void delete_img()
        {
            int delete_img = Form1.Image_ID;
            int i;
            for (i = delete_img; i < Form1.Imagelist.Count - 1; i++)
            {
                Form1.Imagelist[i] = Form1.Imagelist[i + 1];
                Form1.bants_img_count[i] = Form1.bants_img_count[i + 1];

                Form1.gcp_collected[i] = Form1.gcp_collected[i + 1];
                Form1.icp_collected[i] = Form1.icp_collected[i + 1];
                Form1.main_width[i] = Form1.main_width[i + 1];
                Form1.main_height[i] = Form1.main_height[i + 1];
            }
            Form1.Imagelist.RemoveAt(Form1.Imagelist.Count - 1);
            Form1.bants_img_count.RemoveAt(Form1.Imagelist.Count);

            Array.Resize<DataTable>(ref Form1.gcp_collected, Form1.Imagelist.Count);
            Array.Resize<DataTable>(ref Form1.icp_collected, Form1.Imagelist.Count);
            Array.Resize<int>(ref Form1.main_width, Form1.Imagelist.Count);
            Array.Resize<int>(ref Form1.main_height, Form1.Imagelist.Count);
            Form1.gcp_count = Form1.gcp_count - 1;
            Form1.icp_count = Form1.icp_count - 1;

            Form1 form = Application.OpenForms["Form1"] as Form1;
            TreeView form1treeview = form.Controls["treeView1"] as TreeView;
            form1treeview.Nodes.Clear();
            form.ımageBox1.Image = null;
            form.ımageBox2.Image = null;
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.FileName = Form1.proje_path;
            TreeNode node = new TreeNode("Project: " + openFile.SafeFileName);
            Form1.Project_ID = form1treeview.Nodes.Count;
            form1treeview.Nodes.Add(node);
            form1treeview.SelectedNode = node;
            for (i = 0; i < form1treeview.Nodes.Count; i++)
            {
                form1treeview.Nodes[i].ImageIndex = 1;
            }
            form1treeview.Nodes[Form1.Project_ID].ImageIndex = 0;

            for (i = 0; i < Form1.Imagelist.Count; i++)
            {
                Form1.Image_ID = i;
                openFile.FileName = Form1.Imagelist[i];

                treenodes_add_img(openFile.SafeFileName, Form1.bants_img_count[i], false);
            }
            form.treeView1.Nodes[0].Expand();
            Form1.Image_ID = -1;
            Form1.Save_Control = 1;

            Button_control();
        }
        //
        // GDL of Code
        //
        void imginfo(string open, out int bit, out int channels, out string format)
        {
            bit = -1;
            channels = 0;
            format = "";
            Gdal.AllRegister();

            Dataset data_igdal = Gdal.Open(open, Access.GA_ReadOnly);
            Band[] bants_gdal = new Band[data_igdal.RasterCount];
            channels = data_igdal.RasterCount;
            format = data_igdal.GetDriver().ShortName;

            bants_gdal[0] = data_igdal.GetRasterBand(1);
            if (bants_gdal[0].DataType == DataType.GDT_Byte) bit = 8;
            if (bants_gdal[0].DataType == DataType.GDT_UInt16) bit = 16;
            if (bants_gdal[0].DataType == DataType.GDT_Int16) bit = 16;
            data_igdal.Dispose();
            bants_gdal[0].Dispose();
        }
        static int max_progress;
        void firstwork()// procesbar is code for image open. For other form.
        {
            processbarform pform = Application.OpenForms["processbarform"] as processbarform;
            pform.progressBar1.Maximum = max_progress;
        }
        void secondwork()// procesbar is code for image open. For other form.
        {
            processbarform pform = Application.OpenForms["processbarform"] as processbarform;
            if (pform.progressBar1.Value + 1 == pform.progressBar1.Maximum)
                pform.progressBar1.Value = pform.progressBar1.Maximum;
            else
                pform.progressBar1.Value = pform.progressBar1.Value + 1;
        }
        Image<Gray, byte>[] bantsimg_8(string open)
        {
            Thread thr = new Thread(new ThreadStart(firstwork));
            Gdal.AllRegister();
            Dataset data_igdal = Gdal.Open(open, Access.GA_ReadOnly);
            Band[] bants_gdal = new Band[data_igdal.RasterCount];
            int rows = data_igdal.RasterXSize;
            int cols = data_igdal.RasterYSize;
            max_progress = cols * data_igdal.RasterCount;
            thr.Start();
            thr.Join();

            int band_count = data_igdal.RasterCount;
            int i, j, bantindeks, k;
            i = 0;
            int[] rasterValues_gdal = new int[rows * cols];
            Byte[,,] byte_img = new byte[cols, rows, 1];
            Image<Gray, byte>[] bantsimg = new Image<Gray, byte>[data_igdal.RasterCount];
            for (bantindeks = 0; bantindeks < data_igdal.RasterCount; bantindeks++)
            {
                k = 0;
                bants_gdal[bantindeks] = data_igdal.GetRasterBand(bantindeks + 1);
                bants_gdal[bantindeks].ReadRaster(0, 0, rows, cols, rasterValues_gdal, rows, cols, 0, 0);
                for (i = 0; i < cols; i++)
                {
                    for (j = 0; j < rows; j++)
                    {
                        byte_img[i, j, 0] = Convert.ToByte(rasterValues_gdal[k]);
                        k = k + 1;
                    }
                    thr = new Thread(new ThreadStart(secondwork));
                    thr.Start();
                    thr.Join();
                }
                bantsimg[bantindeks] = new Image<Gray, byte>(byte_img);
            }
            return bantsimg;
        }
        Image<Gray, ushort>[] bantsimg_16(string open)
        {
            Thread thr = new Thread(new ThreadStart(firstwork));
            Gdal.AllRegister();
            Dataset data_igdal = Gdal.Open(open, Access.GA_ReadOnly);
            Band[] bants_gdal = new Band[data_igdal.RasterCount];
            int rows = data_igdal.RasterXSize;
            int cols = data_igdal.RasterYSize;
            max_progress = cols * data_igdal.RasterCount;
            thr.Start();
            thr.Join();
            int band_count = data_igdal.RasterCount;
            int i, j, bantindeks, k;

            Image<Gray, ushort>[] bantsimg = new Image<Gray, ushort>[data_igdal.RasterCount];
            for (bantindeks = 0; bantindeks < data_igdal.RasterCount; bantindeks++)
            {
                k = 0;
                bants_gdal[bantindeks] = data_igdal.GetRasterBand(bantindeks + 1);
                int[] rasterValues_gdal = new int[rows * cols];
                ushort[,,] ushort_img = new ushort[cols, rows, 1];
                bants_gdal[bantindeks].ReadRaster(0, 0, rows, cols, rasterValues_gdal, rows, cols, 0, 0);
                for (i = 0; i < cols; i++)
                {
                    for (j = 0; j < rows; j++)
                    {
                        ushort_img[i, j, 0] = Convert.ToUInt16(rasterValues_gdal[k]);
                        k = k + 1;
                    }
                    thr = new Thread(new ThreadStart(secondwork));
                    thr.Start();
                    thr.Join();
                }
                bantsimg[bantindeks] = new Image<Gray, ushort>(ushort_img);
            }
            return bantsimg;
        }
    }
}
