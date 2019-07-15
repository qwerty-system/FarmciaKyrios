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
    public partial class frm_creaCuenta : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string compraActual;
        double total;
        double descuento;
        double totalFinal;
        string usuario;
        string idSucursal;
        public frm_creaCuenta(string compra, string sucursal, string user, string proveedor)
        {
            InitializeComponent();
            idSucursal = sucursal;
            usuario = user;
            compraActual = compra;
            label15.Text = compra;
           
            getTotal();
            calculosFormulario();
            cargaProveedor();
            comboBox5.Text = proveedor;
            comboBox5.Enabled = false;
            textBox5.Focus();
            textBox2.Text = "COMPRA CREDITO";
            textBox1.Text = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            textBox5.Focus();

        }
        private void cargaProveedor()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT NOMBRE_PROVEEDOR FROM PROVEEDOR WHERE ESTADO_TUPLA = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    comboBox5.Items.Add(reader.GetString(0));
                   // comboBox5.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        comboBox5.Items.Add(reader.GetString(0));
                        //comboBox1.Items.Add(reader.GetString(0));
                    }
                    //comboBox5.SelectedIndex = 0;
                    //comboBox1.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("NO EXISTEN PROVEEDORES!", "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                        label3.Text = string.Format("Q.{0:###,###,###,##0.00##}", total);
                    }
                }
                catch
              (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void calculosFormulario()
        {
            label7.Text = string.Format("Q.{0:###,###,###,##0.00##}", total);
            totalFinal = total;
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_creaCuenta_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_creaCuenta_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_creaCuenta_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_creaCuenta_KeyDown(object sender, KeyEventArgs e)
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
                descuento = total * (Convert.ToDouble(textBox5.Text) / 100);
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
        private void guardaCompra()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL CIERRA_COMPRA ({0},{1},{2},{3});", compraActual, total, descuento, totalFinal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    var fomra = new frm_creditoActualizado();
                    fomra.ShowDialog();
                    adressUser.ingresaBitacora(idSucursal, usuario, "COMPRA CREDITO", compraActual);
                    //timerActions();
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
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button2.PerformClick();
            }
            else if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        private bool codigostock(string code)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CUENTA_POR_PAGAR FROM CUENTA_POR_PAGAR WHERE ID_CUENTA_POR_PAGAR =  '{0}' AND ESTADO_TUPLA = TRUE AND ESTADO_CUENTA = TRUE;", code);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // MessageBox.Show("el codigo ya existe");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
            return true;
        }
        public string createCode(int longitud)
        {
            string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < longitud--)
            {
                res.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            return res.ToString();
        }
        private string getCode()
        {
            bool codex = true;
            while (codex)
            {
                string temp = createCode(10);
                if (codigostock(temp))
                {
                    codex = false;
                    return temp;
                }

            }
            return null;
        }
        private void guardaCuenta()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string codigoCuenta = getCode();
                string sql = string.Format("INSERT INTO CUENTA_POR_PAGAR VALUES ('{0}',(SELECT ID_PROVEEDOR FROM PROVEEDOR WHERE NOMBRE_PROVEEDOR = '{1}' AND ESTADO_TUPLA = 1 LIMIT 1),{2},'{3}',0,'{4}',{5},{6},{7},TRUE,TRUE);", codigoCuenta, comboBox5.Text, compraActual, textBox2.Text.Trim(),dateTimePicker1.Value.ToString("yyyy-MM-dd") ,total, descuento, totalFinal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //.Show("SE GENERO LA CUENTA CORRECTAMENTE");
                    guardaCompra();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO GENERAR NUEVA ORDEN DE COMPRA!", "GESTION COMPRAS", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if ((comboBox5.Text != "") && (textBox2.Text != "") && (textBox1.Text != ""))
            {
                guardaCuenta();
            }
            else
            {
                MessageBox.Show("DEBE DE LLENAR TODOS LOS CAMPOS", "COMPRA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = dateTimePicker1.Value.ToString("yyyy-MM-dd");
        }

        private void frm_creaCuenta_Load(object sender, EventArgs e)
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
