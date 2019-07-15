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
    public partial class frm_getPresentacion : Form
    {
        string presentacion;
        string cantidad;
        public frm_getPresentacion()
        {
            InitializeComponent();
        }

        private void frm_getPresentacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }
        internal frm_compras.Presentaciones CurrentCantidad
        {
            get
            {
                return new frm_compras.Presentaciones
                {
                    getCantidad = cantidad
                };
            }
            set
            {
                CurrentCantidad.getCantidad = cantidad;
            }
        }
        internal frm_compras.Presentaciones CurrentPresentacion
        {
            get
            {
                return new frm_compras.Presentaciones
                {
                    getPresentacion = presentacion
                };
            }
            set
            {
                CurrentPresentacion.getPresentacion = presentacion;
            }
        }

        private void button8_Enter(object sender, EventArgs e)
        {
            button8.BackColor = Color.DimGray;
        }

        private void button1_Enter(object sender, EventArgs e)
        {
            button1.BackColor = Color.DimGray;
        }

        private void button2_Enter(object sender, EventArgs e)
        {
            button2.BackColor = Color.DimGray;
        }

        private void button3_Enter(object sender, EventArgs e)
        {
            button3.BackColor = Color.DimGray;
        }

        private void button4_Enter(object sender, EventArgs e)
        {
            button4.BackColor = Color.DimGray;
        }

        private void button5_Enter(object sender, EventArgs e)
        {
            button5.BackColor = Color.DimGray;
        }

        private void button8_Leave(object sender, EventArgs e)
        {
            button8.BackColor = Color.Transparent;
        }

        private void button1_Leave(object sender, EventArgs e)
        {
            button1.BackColor = Color.Transparent;
        }

        private void button2_Leave(object sender, EventArgs e)
        {
            button2.BackColor = Color.Transparent;
        }

        private void button3_Leave(object sender, EventArgs e)
        {
            button3.BackColor = Color.Transparent;
        }

        private void button4_Leave(object sender, EventArgs e)
        {
            button4.BackColor = Color.Transparent;
        }

        private void button5_Leave(object sender, EventArgs e)
        {
            button5.BackColor = Color.Transparent;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            presentacion = button8.Text;
            cantidad = "1";
            DialogResult = DialogResult.OK;
        }

        private void button6_Enter(object sender, EventArgs e)
        {
            button6.BackColor = Color.DimGray;
        }

        private void button6_Leave(object sender, EventArgs e)
        {
            button6.BackColor = Color.Transparent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            presentacion = button1.Text;
            cantidad = "0";
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            presentacion = button2.Text;
            cantidad = "6";
            DialogResult = DialogResult.OK;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            presentacion = button3.Text;
            cantidad = "12";
            DialogResult = DialogResult.OK;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            presentacion = button4.Text;
            cantidad = "0";
            DialogResult = DialogResult.OK;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            presentacion = button5.Text;
            cantidad = "0";
            DialogResult = DialogResult.OK;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            presentacion = button6.Text;
            cantidad = "0";
            DialogResult = DialogResult.OK;
        }
    }
}
