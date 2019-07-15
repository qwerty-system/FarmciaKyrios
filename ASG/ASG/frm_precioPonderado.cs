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
    public partial class frm_precioPonderado : Form
    {
        double precioAnterior;
        double precioNuevo;
        double existente;
        double ingreso;
        double calculo_existente;
        double calculo_ingreso;
        double precio_ponderado;
        double subtotal_final;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public frm_precioPonderado(string precioA, string precioN, string cantidadE, string cantidadI)
        {
            InitializeComponent();
            if ((precioA != "") && (cantidadE != ""))
            {
                precioAnterior = Convert.ToDouble(precioA);
                existente = Convert.ToDouble(cantidadE);
                label5.Text = String.Format("Q{0:#,###,###,###.00}", precioAnterior);
                label6.Text = String.Format("{0:#,###,###,###}", existente);
            }
            else
            {
                precioAnterior = 0;
                existente = 0;
            }
            precioNuevo = Convert.ToDouble(precioN);
            ingreso = Convert.ToDouble(cantidadI);
            label15.Text = String.Format("Q{0:#,###,###,###.00}", precioNuevo);
            label14.Text = String.Format("{0:#,###,###,###}", ingreso);
            setterForm();
        }
        private void setterForm()
        {
            if ((precioAnterior != 0) && (existente != 0))
            {
                calculo_existente = Math.Round((precioAnterior * existente), 2);
                label7.Text = String.Format("Q{00:#,###,###,###.00}", calculo_existente);
                label19.Text = String.Format("{0:#,###,###,###}", existente + ingreso);
            }
            else
            {
                label19.Text = String.Format("{000:#,###,###,###}", ingreso);
            }
            calculo_ingreso = ingreso * precioNuevo;
            subtotal_final = calculo_ingreso;

            label9.Text = String.Format("Q{00:#,###,###,###.00}", calculo_ingreso);
            if (precioAnterior != 0)
            {
                precio_ponderado = (calculo_existente + calculo_ingreso) / (existente + ingreso);
                label11.Text = String.Format("Q{00:#,###,###,###.00}", precio_ponderado);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            DialogResult = DialogResult.OK;
        }
        internal frm_compras.precioPonderado CurrentPrecio
        {
            get
            {
                return new frm_compras.precioPonderado()
                {
                    getPrecio = Convert.ToString(Math.Round(precio_ponderado, 2))
                };
            }
            set
            {
                CurrentPrecio.getPrecio = Convert.ToString(Math.Round(precio_ponderado, 2));
            }
        }

        private void frm_precioPonderado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_precioPonderado_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_precioPonderado_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_precioPonderado_MouseUp(object sender, MouseEventArgs e)
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
