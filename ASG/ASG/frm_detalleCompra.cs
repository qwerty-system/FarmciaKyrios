using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Odbc;

namespace ASG
{
    public partial class frm_detalleCompra : Form
    {
        string compraActual;
        string subTotal;
        string descuento;
        string total;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public frm_detalleCompra(string compra, string stotal, string desc, string totalf, bool state)
        {
            InitializeComponent();
            if (state)
            {
                compraActual = compra;
                subTotal = stotal;
                descuento = desc;
                total = totalf;
                label9.Text = compra;

                label5.Text = subTotal;
                label11.Text = descuento;
                label7.Text = total;
            } else
            {
                label9.Text = compra;
                compraActual = compra;
                cargaDB();
            }
            

            cargaCompras();
        }
        private void cargaDB()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT subtotal_compra, descuento, total_final from compra WHERE ID_COMPRA = {0};", compraActual);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    label5.Text = "Q." + reader.GetDouble(0);
                    label11.Text = "Q." + reader.GetDouble(1);
                    label7.Text = "Q." + reader.GetDouble(2);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaCompras()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView3.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_CONTROL_COMPRA WHERE ID_COMPRA = {0};", compraActual);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView3.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                    while (reader.Read())
                    {
                        dataGridView3.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                        //styleDV(this.dataGridView3);
                    }
                }
                else
                {
                    MessageBox.Show("NO EXISTEN COMPRAS!", "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    DialogResult = DialogResult.Cancel;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void frm_detalleCompra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_detalleCompra_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_detalleCompra_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_detalleCompra_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
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
