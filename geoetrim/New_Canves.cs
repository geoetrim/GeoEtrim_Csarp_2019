using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GeoEtrim
{
    public partial class New_Canves : Form
    {   
        public New_Canves()
        {
            InitializeComponent();
        }        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "") && (textBox2.Text != ""))
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "") && (textBox2.Text != ""))
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument proje = new XmlDocument();
                XmlDeclaration xmlDeclaration = proje.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = proje.DocumentElement;
                proje.InsertBefore(xmlDeclaration, root);
             
                root = proje.CreateElement("Project");
                root.SetAttribute("Project_name", textBox2.Text+ ".gpr");
                root.SetAttribute("Information", textBox3.Text);
                Form1.proje_info.Add(textBox3.Text);
                proje.AppendChild(root);

                Form1.proje_path =textBox1.Text + "/" + textBox2.Text+ ".gpr";
                proje.Save(Form1.proje_path);

                Form1 form = Application.OpenForms["Form1"] as Form1;
                TreeView form1treeview = form.Controls["treeView1"] as TreeView;
                TreeNode node = new TreeNode("Project: " + textBox2.Text.ToString()+ ".gpr");
                Form1.Project_ID = form1treeview.Nodes.Count;
                form1treeview.Nodes.Add(node);
                form1treeview.SelectedNode = node;
   
                int i = 0;
                for (i = 0; i < form1treeview.Nodes.Count; i++)
                {
                    form1treeview.Nodes[i].ImageIndex = 1;
                }
                form1treeview.Nodes[Form1.Project_ID].ImageIndex = 0;

                form.toolStripButton3.Enabled = true;
                form.toolStripButton5.Enabled = true;
                form.toolStripButton7.Enabled = true;
                          
                this.Close();
            }
            catch(Exception error)
            {
                MessageBox.Show(error.Message,"ERROR",MessageBoxButtons.OK,MessageBoxIcon.Error);

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }
    }
}
