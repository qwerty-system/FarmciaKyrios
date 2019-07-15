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
    public partial class frm_eCompra : Form
    {
        string compraActual;
        double total;
        double descuento;
        double totalFinal;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string usuario;
        string idSucursal;
        string proveedorCompra;
        public frm_eCompra(string compra, string sucursal, string user, string proveedor)
        {
            InitializeComponent();
            idSucursal = sucursal;
            usuario = user;
            proveedorCompra = proveedor;
            compraActual = compra;
            cargaCompras();
            label9.Text = compra;
            getTotal();
            calculosFormulario();
            textBox5.Focus();
        }
        private void timerActions()
        {
            frm_done frm = new frm_done();
            frm.ShowDialog();
            timer1.Interval = 1500;
            timer1.Enabled = true;
        }
        private void calculosFormulario()
        {
            label7.Text = string.Format("Q.{0:###,###,###,##0.00##}", total);
            totalFinal = total;
        }
        private void guardaCompra()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL CIERRA_COMPRA ({0},{1},{2},{3});",compraActual, total, descuento, totalFinal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    adressUser.ingresaBitacora(idSucursal, usuario, "COMPRA GENERADA", compraActual);
                    timerActions();
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("NO SE PUDO GUARDAR NUEVA ORDEN DE COMPRA!", "GESTION COMPRAS", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void getTotal()
        {
            if (compraActual != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = sql = string.Format("SELECT SUBTOTAL FROM CONTROL_COMPRA WHERE ID_COMPRA = {0};", compraActual);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        total = 0;
                        total = reader.GetDouble(0);
                        while (reader.Read())
                        {
                            total += reader.GetDouble(0);
                        }
                        label5.Text = string.Format("Q.{0:###,###,###,##0.00##}", total);
                    }
                }
                catch
              (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void cargaCompras()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_CONTROL_COMPRA WHERE ID_COMPRA = {0};",compraActual);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                        //styleDV(this.dataGridView1);
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
        
        private void frm_eCompra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                button1.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.P))
            {
                button2.PerformClick();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            guardaCompra();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if ((textBox5.Text != "") && (textBox5.Text != "0"))
            {
                descuento = total * (Convert.ToDouble(textBox5.Text)/ 100);
                label6.Text = string.Format("Q.{0:###,###,###,##0.00##}", descuento);
                totalFinal = total - descuento;
                label7.Text = string.Format("Q.{0:###,###,###,##0.00##}", totalFinal);
            }
            else
            {
                totalFinal = total;
                label7.Text = string.Format("Q.{0:###,###,###,##0.00##}", total);
                label6.Text = "0";
            }
           
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button1.PerformClick();
            } else if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_eCompra_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_eCompra_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_eCompra_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_eCompra_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var forma = new frm_creaCuenta(compraActual,idSucursal,usuario, proveedorCompra);
            if(forma.ShowDialog() == DialogResult.OK)
            {
               DialogResult = DialogResult.OK;
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

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
