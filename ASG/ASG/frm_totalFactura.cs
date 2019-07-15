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
    public partial class frm_totalFactura : Form
    {
        double subtotalFactura;
        double descuentoFactura;
        double totalFactura;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public frm_totalFactura(double subtotal, double descuento, double total)
        {
            InitializeComponent();
            subtotalFactura = subtotal;
            descuentoFactura = descuento;
            totalFactura = total;
            iniciaFormulario();          
        }
        private void iniciaFormulario()
        {
            label1.Text = string.Format("Q.{0:###,###,###,##0.00##}", subtotalFactura);
            label5.Text = string.Format("Q.{0:###,###,###,##0.00##}", descuentoFactura);
            label7.Text = string.Format("Q.{0:###,###,###,##0.00##}", totalFactura);
            textBox5.Focus();
        }
        private void frm_totalFactura_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_totalFactura_Load(object sender, EventArgs e)
        {

        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))

            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                button5.PerformClick();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Text != "" && textBox5.Text != "0")
            {
                double recibido = Convert.ToDouble(textBox5.Text.Trim());
                double cambio = recibido - totalFactura;
                label12.Text = string.Format("Q.{0:###,###,###,##0.00##}",cambio);
            } else
            {
                label12.Text = "";
            }
        }

        private void frm_totalFactura_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_totalFactura_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;

        }

        private void frm_totalFactura_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_MouseHover(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.Red;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.Transparent;
        }
    }
}
