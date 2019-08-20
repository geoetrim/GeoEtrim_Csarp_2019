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
    public partial class processbarform : Form
    {
        public processbarform()
        {
            InitializeComponent();
        }
        private void processbarform_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
            this.TransparencyKey = Color.White;

            Form1 f = Application.OpenForms["Form1"] as Form1;
            formgcp form = Application.OpenForms["formgcp"] as formgcp;
            GeoTransform trans = Application.OpenForms["GeoTransform"] as GeoTransform;
            this.Owner = f;
            if (formgcp.Form_active == true)
                this.Owner = form;
            if (GeoTransform.form_active == true)
                this.Owner = trans;
        }
    }
}
