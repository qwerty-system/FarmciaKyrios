using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASG
{
    public partial class frm_reporteVenta : Form
    {
        public frm_reporteVenta()
        {
            InitializeComponent();
        }

        private void frm_reporteVenta_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void frm_reporteVenta_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_reporteVenta_Load(object sender, EventArgs e)
        {

        }
    }
}
