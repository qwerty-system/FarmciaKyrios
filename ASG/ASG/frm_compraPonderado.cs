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
    public partial class frm_compraPonderado : Form
    {
        double precioAnterior;
        double precioNuevo;
        double existente;
        double ingreso;
        double calculo_existente;
        double calculo_ingreso;
        double precio_ponderado;
        double gasto;
        double subtotal_final;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public frm_compraPonderado(string precioA, string precioN,string cantidadE, string cantidadI)
        {
            InitializeComponent();
            if ((precioA != "") && (cantidadE != ""))
            {
                precioAnterior = Convert.ToDouble(precioA);
                existente = Convert.ToDouble(cantidadE);
                label5.Text = String.Format("Q{0:#,###,###,###.00}", precioAnterior);
                label6.Text = String.Format("{0:#,###,###,###}", existente);
            } else
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
            if((precioAnterior != 0) && (existente != 0))
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
        }
        internal frm_compras.precioPonderado CurrentPrecio
        {
            get
            {
                return new frm_compras.precioPonderado()
                {
                    getPrecio = Convert.ToString(Math.Round(precio_ponderado,2))
                };
            }
            set
            {
                CurrentPrecio.getPrecio = Convert.ToString(Math.Round(precio_ponderado,2));
            }
        }
        internal frm_compras.precioPonderado CurrentGasto
        {
            get
            {
                return new frm_compras.precioPonderado()
                {
                    getGasto = Convert.ToString(Math.Round(gasto, 2))
                };
            }
            set
            {
                CurrentGasto.getGasto = Convert.ToString(Math.Round(gasto, 2));
            }
        }
        internal frm_compras.precioPonderado CurrentSubtotal
        {
            get
            {
                return new frm_compras.precioPonderado()
                {
                    getSubtotal = Convert.ToString(Math.Round(subtotal_final, 2))
                };
            }
            set
            {
                CurrentSubtotal.getSubtotal = Convert.ToString(Math.Round(subtotal_final, 2));
            }
        }
        private void frm_compraPonderado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if ((textBox5.Text != "") && (textBox5.Text != "0"))
            {
                double gastos = Convert.ToDouble(textBox5.Text);
                double importe = gasto / ingreso;
                double precio_final = precioNuevo + importe;
                label13.Text = String.Format("Q{0:#,###,###,###.00}", precio_final);
                calculo_ingreso = precio_final * ingreso;
                label9.Text = String.Format("Q{0:#,###,###,###.00}", calculo_ingreso);
                 if (precioAnterior != 0)
                {
                    precio_ponderado = (calculo_existente + calculo_ingreso) / (existente + ingreso);
                } else
                {
                    precio_ponderado = precio_final;
                }
                label11.Text = String.Format("Q{0:#,###,###,###.00}", precio_ponderado);
                subtotal_final = calculo_ingreso;
                gasto = gastos;
                
            } else if (textBox5.Text == "0")  
            {
                setterForm();
                if (precioAnterior != 0)
                {
                    precio_ponderado = (calculo_existente + calculo_ingreso) / (existente + ingreso);
                }
                else
                {
                    precio_ponderado = Math.Round(((precioAnterior + precioNuevo)/2),2);
                }
                label11.Text = String.Format("Q{0:#,###,###,###.00}", precio_ponderado);
                subtotal_final = calculo_ingreso;
                gasto = Convert.ToDouble(textBox5.Text);

            } else {
                label13.Text = "0";
                label9.Text = "0";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != "") {
                DialogResult = DialogResult.OK;
            }
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

        private void frm_compraPonderado_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_compraPonderado_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_compraPonderado_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_compraPonderado_MouseUp(object sender, MouseEventArgs e)
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
