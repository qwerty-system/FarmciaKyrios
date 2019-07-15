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
    public partial class frm_Precios : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string idSucursal;
        string nombreSucursal;
        string rolUs;
        string codigo;
        public frm_Precios(string codigoSucursal, bool estado, string nombre, string rol)
        {
            InitializeComponent();
            // FALSO SI VIENE DE UN FORM
            idSucursal = codigoSucursal;
            nombreSucursal = nombre;
            rolUs = rol;
            if (rol != "ADMINISTRADOR")
            {
                dataGridView1.Columns[4].Visible = false;
                dataGridView1.Columns[10].Visible = false;
                dataGridView1.Columns[11].Visible = false;
                dataGridView1.Columns[12].Visible = false;
                dataGridView1.Columns[13].Visible = false;
                dataGridView1.Columns[14].Visible = false;
            }
        }

        private void frm_Precios_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.B))
            {
                button4.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.D))
            {
                button1.PerformClick();
            }
        }

        private void frm_Precios_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_Precios_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }
        private void obtieneStock(string codigo, string sucursal)
        {
            try
            {
                dataGridView1.Rows.Clear();
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_STOCK WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", codigo, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("{0:#,###,###,###,###}", reader.GetDouble(3)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(8)), string.Format("{0:###,###,###,##0.00##} ", reader.GetDouble(9)), string.Format("{0:###,###,###,##0.00##} ", reader.GetDouble(10)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(11)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(12)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(13)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(14)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(15)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("{0:#,###,###,###,###}", reader.GetDouble(3)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(8)), string.Format("{0:###,###,###,##0.00##} ", reader.GetDouble(9)), string.Format("{0:###,###,###,##0.00##} ", reader.GetDouble(10)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(11)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(12)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(13)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(14)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(15)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void obtieneProducto(string codigo, string sucursal)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT M.ID_MERCADERIA, M.ID_SUCURSAL, M.TOTAL_UNIDADES, M.DESCRIPCION,M.FECHA_INGRESO FROM MERCADERIA M WHERE ID_MERCADERIA = '{0}' AND M.ID_SUCURSAL = (SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL ='{1}' AND ESTADO_TUPLA = TRUE LIMIT 1);", codigo, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    label10.Text = reader.GetString(0);
                    label5.Text = "" + reader.GetString(2);
                    label2.Text = reader.GetString(3);
                    label11.Text = reader.GetString(4);
                    obtieneStock(codigo, sucursal);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void frm_Precios_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var Tempforma = new frm_buscaEdicion(nombreSucursal, rolUs);
            if (Tempforma.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.Rows.Clear();
               // textBox11.Text = Tempforma.CurrentMercaderia.getMercaderia;
                obtieneProducto(Tempforma.CurrentMercaderia.getMercaderia, Tempforma.CurrentSucursal.getSucursal);
            }
        }
        private void buscaSubcodigos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_STOCK FROM SUBCODIGOS_MERCADERIA WHERE SUBCODIGO = '{0}' AND ESTADO_TUPLA = TRUE;", textBox11.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    textBox11.Text = reader.GetString(0);
                    buscaCodigos(textBox11.Text);
                } else
                {
                    buscaCodigos(textBox11.Text);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void buscaCodigos(string codigoMercaderia)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT M.ID_MERCADERIA, (SELECT NOMBRE_SUCURSAL FROM SUCURSAL WHERE ID_SUCURSAL = M.ID_SUCURSAL) FROM MERCADERIA_PRESENTACIONES M WHERE ESTADO_TUPLA = TRUE AND ID_SUCURSAL = {0} AND ID_STOCK = '{1}';", idSucursal, codigoMercaderia);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string codigo = reader.GetString(0);
                    string sucursal = reader.GetString(1);
                    textBox11.Text = "";
                    textBox11.Focus();
                    obtieneProducto(codigo, sucursal);
                    if (dataGridView1.RowCount > 0)
                    {
                        bool flags = true;
                        for (int i = 0; i < dataGridView1.RowCount && flags; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[0].Value.ToString() == codigoMercaderia)
                            {
                                flags = false;
                                this.dataGridView1.CurrentCell = this.dataGridView1[dataGridView1.Rows[i].Index, dataGridView1.Rows[i].Index];
                            }
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            if(textBox11.Text == "")
            {
                label2.Text = "";
                label5.Text = "";
                dataGridView1.Rows.Clear();
            }
        }

        private void textBox11_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox11.Text != "")
            {
                if (e.KeyData == Keys.Enter)
                {
                    buscaSubcodigos();
                }
            } if(e.KeyData == Keys.Down)
            {
                if(dataGridView1.RowCount > 0)
                {
                    dataGridView1.Focus();
                }
            }
        }

        private void frm_Precios_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var forma = new frm_vistaGeneral(label10.Text,label2.Text,label11.Text,1, rolUs);
            forma.ShowDialog();
        }
        internal frm_menu.codigoMercaderia CurrrentCodigo
        {
            get
            {
                return new frm_menu.codigoMercaderia()
                {
                    getProducto = codigo
                };
            }
            set
            {
                CurrrentCodigo.getProducto = codigo;
            }
        }
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
           if(dataGridView1.RowCount > 0)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    DialogResult result;
                    result = MessageBox.Show("¿DESEA AGREGAR EL PRODUCTO A LA VENTA?", "CONSULTA DE PRODUCTOS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        codigo = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        DialogResult = DialogResult.OK;   
                    }
                }
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                SendKeys.Send("{ENTER}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                dataGridView1.Focus();
                SendKeys.Send("{ENTER}");
            }
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
