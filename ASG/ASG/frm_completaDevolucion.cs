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
    public partial class frm_completaDevolucion : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string idSucursal;
        string codigoFactura;
        string codigoCaja;
        string nombreUsuario;
        string usuario;
        string totalFactura;
        public frm_completaDevolucion(string factura, string user, string sucursal, string cliente, string nombre, string total)
        {

            InitializeComponent();
            idSucursal = sucursal;
            codigoFactura = factura;
            nombreUsuario = nombre;
            totalFactura = total;
            label6.Text = cliente;
            label3.Text = factura;
            usuario = user;
            radioButton2.Checked = true;
            DateTime today = DateTime.Now;
            textBox5.Text = today.ToString("yyyy-MM-dd");
            if (obtieneAjustes())
            {
                if (!revisaCaja())
                {
                    var forma = new frm_aperturaCaja(idSucursal, user, nombreUsuario);
                    if (forma.ShowDialog() != DialogResult.OK)
                    {
                        this.Enabled = false;
                    }
                    obtieneCaja();
                }
                else
                {
                    obtieneCaja();
                }
            }
            else
            {
                obtieneCaja();
            }
        }
        private void aperturaCaja()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("INSERT INTO CAJA VALUES (NULL,{0},'{1}',NOW(),{2},NOW(),{2},'APERTURADA',TRUE,TRUE);", idSucursal, usuario, 0);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    obtieneCaja();
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
        private void obtieneCaja()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CAJA FROM CAJA WHERE ID_SUCURSAL = {0} AND ID_USUARIO = '{1}' AND ESTADO_TUPLA = TRUE AND ESTADO_CAJA = TRUE;", idSucursal, usuario);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    codigoCaja = reader.GetString(0);
                }
                else
                {
                    aperturaCaja();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private bool revisaCaja()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CAJA FROM CAJA WHERE ID_USUARIO = '{0}' AND ID_SUCURSAL = {1} AND ESTADO_TUPLA = TRUE;", usuario, idSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    conexion.Close();
                    return true;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return false;
        }
        private bool obtieneAjustes()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT CAJA FROM CONFIGURACION_SISTEMA WHERE IDCONFIGURACION = 111 AND CAJA = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    conexion.Close();
                    return true;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return false;
        }
        private void ingresaDevolucion(string factura, string tipo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("INSERT INTO DEVOLUCION VALUES (NULL,'{0}', '{1}',NOW(),TRUE);",tipo, factura);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    var fomra = new frm_creditoActualizado();
                    fomra.ShowDialog();
                    adressUser.ingresaBitacora(idSucursal, usuario, "DEVOLUCION FACTURA", codigoFactura);
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("IMPOSIBLE GUARDAR DEVOLUCION!", "ANULACION FACTURA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void guardaCaja()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE DET_CAJA SET ESTADO_TUPLA = FALSE WHERE ID_FACTURA = '{0}' AND ID_CAJA = {1};",  codigoFactura, codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                  //  MessageBox.Show("DETALLE DE CAJA GUARDADO GURDADA");
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void descuentaDevolucion()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CAJA SET TOTAL_FACTURADO_CAJA = TOTAL_FACTURADO_CAJA - {0} WHERE ID_CAJA = {1} ;", totalFactura, codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    guardaCaja();
                }
                else
                {
                    MessageBox.Show("IMPOSIBLE ACTUALIZAR EL MONTO DE LA CAJA!", "GESTION COMPRAS", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
           
                ingresaDevolucion(codigoFactura, radioButton2.Text);
                descuentaDevolucion();
            
        }

        private void frm_completaDevolucion_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_completaDevolucion_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_completaDevolucion_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_completaDevolucion_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_completaDevolucion_Load(object sender, EventArgs e)
        {

        }
    }
}
