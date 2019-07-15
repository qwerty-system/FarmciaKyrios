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
    public partial class frm_cambioVenta : Form
    {
        public frm_cambioVenta()
        {
            InitializeComponent();
            timer1.Interval = 2000;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_cambioVenta_Load(object sender, EventArgs e)
        {

        }
    }
}
