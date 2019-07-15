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
    public partial class frm_BuscaProducto : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string nombreSucursal;
        string idSucursal;
        public frm_BuscaProducto(string sucursal, string codigo, string rol)
        {
            InitializeComponent();
            cargacuentasCobrar();
            nombreSucursal = sucursal;
            idSucursal = codigo;
            if(rol == "ADMINISTRADOR" | rol == "DATA BASE ADMIN" | rol == "ROOT")
            {
                cargacuentasPagar();
                label3.Visible = true;
                label1.Visible = true;
            } else
            {
                label3.Visible = false;
                label1.Visible = false;
            }
            productosAgotados();
            cotizacionesPendinetes();
            
        }
        private void frm_BuscaProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }
        private void frm_BuscaProducto_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }
        private void frm_BuscaProducto_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }
        private void frm_BuscaProducto_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_BuscaProducto_Load(object sender, EventArgs e)
        {

        }
        private void cargacuentasCobrar()
        { 
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT COUNT(ID_CLIENTE) FROM CUENTA_POR_COBRAR_CLIENTE WHERE ESTADO_TUPLA = TRUE AND ESTADO_CUENTA = TRUE;" );
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetDouble(0) <= 0)
                    {
                        label2.Text = "0";
                    }
                    else
                        label2.Text = string.Format("{0:#,###,###,###,###}", reader.GetDouble(0));
                }
                else
                {
                    label2.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargacuentasPagar()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT COUNT(ID_PROVEEDOR) FROM CUENTA_POR_PAGAR WHERE ESTADO_TUPLA = TRUE AND ESTADO_CUENTA = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetDouble(0) <= 0)
                    {
                        label1.Text = "0";
                    } else
                    label1.Text = string.Format("{0:#,###,###,###,###}", reader.GetDouble(0));
                } else
                {
                    label1.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void productosAgotados()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT COUNT(ID_MERCADERIA) FROM MERCADERIA WHERE ESTADO_TUPLA = TRUE AND TOTAL_UNIDADES = 0 AND ID_SUCURSAL = {0};",idSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetDouble(0) <= 0)
                    {
                        label5.Text = "0";
                    }
                    else
                        label5.Text = string.Format("{0:#,###,###,###,###}", reader.GetDouble(0));
                }
                else
                {
                    label5.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cotizacionesPendinetes()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT COUNT(ID_FACTURA) FROM FACTURA WHERE ESTADO_TUPLA = TRUE AND ESTADO_FACTURA = 0 AND ID_SUCURSAL = {0};", idSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetDouble(0) <= 0)
                    {
                        label9.Text = "0";
                    }
                    else
                        label9.Text = string.Format("{0:#,###,###,###,###}", reader.GetDouble(0));
                }
                else
                {
                    label9.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
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
