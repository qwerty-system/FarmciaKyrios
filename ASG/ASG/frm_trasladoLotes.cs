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
    public partial class frm_trasladoLotes : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string usuario;
        bool flagsc;
        string idSucursal;
        ContextMenuStrip mymenuS = new ContextMenuStrip();
        public frm_trasladoLotes(string user, string sucursal)
        {
            InitializeComponent();
            usuario = user;
            idSucursal = sucursal;
            cargaSucursales();
            stripMenuSC();
            
        }
        private void stripMenuSC()
        {
            mymenuS.Items.Add("Eliminar Producto de la Cotizacion");
            mymenuS.Items[0].Name = "ColHidden";
        }
        private void cargaSucursales()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT NOMBRE_SUCURSAL FROM SUCURSAL WHERE ESTADO_TUPLA = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                   
                    comboBox1.Items.Clear();
                    comboBox1.Items.Add(reader.GetString(0)); 
                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void cargaSucursalesDestino()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT ID_SUCURSAL, NOMBRE_SUCURSAL FROM SUCURSAL WHERE ESTADO_TUPLA = TRUE AND NOMBRE_SUCURSAL <> '{0}';", comboBox1.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), false);
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), false);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
       private void frm_trasladoLotes_KeyDown(object sender, KeyEventArgs e)
       {
            if(e.KeyData == Keys.Escape)
            {
                if((dataGridView4.RowCount > 0) | (comboBox1.Text != ""))
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA SALIR, LOS DATOS DEL TRASLADO SE PERDERAN?", "TRASLADO DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    } else
                    {
                        this.Close();
                    }
                } else
                {
                    this.Close();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.B))
            {
                button3.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                button2.PerformClick();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "TRASLADO DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    dataGridView4.Rows.Clear();
                    dataGridView1.Rows.Clear();
                    comboBox1.SelectedIndex = -1;
                    comboBox1.Enabled = true;
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var forma = new  frm_informacionTrasladoGenral();
            forma.ShowDialog();
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void frm_trasladoLotes_Load(object sender, EventArgs e)
        {
            dataGridView4.Columns[0].ReadOnly = true;
            dataGridView4.Columns[1].ReadOnly = true;
            dataGridView4.Columns[2].ReadOnly = true;
            dataGridView4.Columns[3].ReadOnly = true;
            dataGridView4.Columns[4].ReadOnly = true;
            dataGridView4.Columns[5].ReadOnly = false;

            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = false;
        }

        private void frm_trasladoLotes_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;

        }

        private void frm_trasladoLotes_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_trasladoLotes_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }
        private bool existeProducto(string codigo)
        {
            for (int i = 0; i< dataGridView4.RowCount; i++)
            {
                if(dataGridView4.Rows[i].Cells[0].Value.ToString() == codigo)
                {
                    return false;
                }
            }
            return true;
        }
        private bool sucursalSeleccionada()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null)
                {
                    if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[2].Value.ToString()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                var forma = new frm_busca(3,comboBox1.Text,"ADMINISTRADOR");
                if (forma.ShowDialog() == DialogResult.OK)
                {
                    if (existeProducto(forma.CurrentProductos.CodigoProducto))
                    {                      
                        dataGridView4.Rows.Add(forma.CurrentProductos.CodigoProducto, forma.CurrentProductos.SucursalProducto,forma.CurrentProductos.Descripcion, forma.CurrentProductos.precioCompra.Replace("Q.",""),forma.CurrentProductos.TotalExistencia, 0, forma.CurrentProductos.proveedor);
                        this.dataGridView4.CurrentCell = this.dataGridView4[5, dataGridView4.RowCount - 1];
                        dataGridView4.Focus();
                    } else
                    {
                        MessageBox.Show("EL PRODUCTO YA HA SIDO SELECCIONADO!", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            } else
            {
                MessageBox.Show("SELECCIONE UNA SUCURSAL PARA BUSCAR LOS PRODUCTOS A TRASLADAR!", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.Text != "")
            {
                comboBox1.Enabled = false;
                cargaSucursalesDestino();
            }
        }
        internal class CargaProductos
        {
            public string CodigoProducto { get; set; }
            public string Descripcion { get; set; }
            public string SucursalProducto { get; set; }
            public string TotalExistencia { get; set; }
            public string precioCompra { get; set; }
            public string proveedor { get; set; }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView4_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value != null)
            {
                if(e.ColumnIndex == 5)
                {
                    if(Convert.ToDouble(dataGridView4.CurrentRow.Cells[5].Value.ToString()) > Convert.ToDouble(dataGridView4.CurrentRow.Cells[4].Value.ToString()))
                    {
                        MessageBox.Show("LA CANTIDAD A TRASLADAR NO PUEDE SER MAYOR A LA EXISTENCIA!", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dataGridView4.CurrentRow.Cells[5].Value = "0";
                    }
                }
            } else
            {
                dataGridView4.CurrentCell.Value = "0";
            }
        }
        private bool validacionesTraslados()
        {
            for(int i = 0; i<dataGridView4.RowCount; i++)
            {
                if(dataGridView4.Rows[i].Cells[5].Value!= null)
                {
                    if (Convert.ToDouble(dataGridView4.Rows[i].Cells[5].Value.ToString()) <= 0)
                    {
                        MessageBox.Show("LA CANTIDAD A TRASLADAR DE: " + "\"" + dataGridView4.Rows[i].Cells[2].Value.ToString() + "\"" + " DEBE SER MAYOR A 0!", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            return true;
        }
        private void presentacionesIngreso(string mercaderia, string stock, string descipcion, string cantidadP, string total, string cantM, string p1, string p2, string p3, string p4, string p5, string p6, string u1, string u2, string u3, string u4, string u5, string sucursal)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = sql = string.Format("SELECT ID_STOCK FROM MERCADERIA_PRESENTACIONES WHERE ID_STOCK = '{0}' AND ID_SUCURSAL = (SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{1}' LIMIT 1);", stock, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    conexion.Close();
                    conexion = ASG_DB.connectionResult();
                    sql = sql = string.Format("CALL UPDATE_STOCK ('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},'{17}');", stock, mercaderia, descipcion, cantidadP, total, cantM, p1, p2, p3, p4, p5, p6, u1, u2, u3, u4, u5, sucursal);
                    cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        //MessageBox.Show("SE ACTUALIZA TIPO VENTA");
                        conexion.Close();
                    }
                    else
                    {
                        // MessageBox.Show("fallo update stock");
                    }

                }
                else
                {

                    conexion.Close();
                    conexion = ASG_DB.connectionResult();
                    sql = sql = string.Format("CALL INGRESA_STOCK_TRASLADO ('{0}','{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17});", stock, mercaderia, sucursal, descipcion, cantidadP, total, cantM, p1, p2, p3, p4, p5, p6, u1, u2, u3, u4, u5);
                    cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        //  MessageBox.Show("SE CREA TIPO VENTA TIPO VENTA");
                        conexion.Close();
                    }
                    else
                    {
                        // MessageBox.Show("fallo primer ingeso");
                    }
                }
            }
            catch
          (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void actualizaMercaderia(string codigo, string precioC, double totalU, string sucursal)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("CALL UPDATE_MERCADERIA_TRASLADO ('{0}',{1},{2},'{3}');", codigo, precioC, totalU, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("ACTUALIZO MERCADERIA DE TRASLADO");
                    for (int i = 0; i < dataGridView3.RowCount; i++)
                    {
                        presentacionesIngreso(codigo, dataGridView3.Rows[i].Cells[0].Value.ToString(), dataGridView3.Rows[i].Cells[1].Value.ToString(), dataGridView3.Rows[i].Cells[2].Value.ToString(), dataGridView3.Rows[i].Cells[3].Value.ToString(), dataGridView3.Rows[i].Cells[4].Value.ToString(), dataGridView3.Rows[i].Cells[5].Value.ToString(), dataGridView3.Rows[i].Cells[6].Value.ToString(), dataGridView3.Rows[i].Cells[7].Value.ToString(), dataGridView3.Rows[i].Cells[8].Value.ToString(), dataGridView3.Rows[i].Cells[9].Value.ToString(), dataGridView3.Rows[i].Cells[10].Value.ToString(), dataGridView3.Rows[i].Cells[11].Value.ToString(), dataGridView3.Rows[i].Cells[12].Value.ToString(), dataGridView3.Rows[i].Cells[13].Value.ToString(), dataGridView3.Rows[i].Cells[14].Value.ToString(), dataGridView3.Rows[i].Cells[15].Value.ToString(), sucursal);
                    }
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void actualizaMercaderiaExistente(string codigo, string precioC, double totalU, string sucursal)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("CALL UPDATE_MERCADERIA_TRASLADO ('{0}',{1},{2},'{3}');", codigo, precioC, totalU, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("ACTUALIZO MERCADERIA DE TRASLADO");
                    for (int i = 0; i < dataGridView5.RowCount; i++)
                    {
                        presentacionesIngreso(codigo, dataGridView5.Rows[i].Cells[0].Value.ToString(), dataGridView5.Rows[i].Cells[1].Value.ToString(), dataGridView5.Rows[i].Cells[2].Value.ToString(), dataGridView5.Rows[i].Cells[3].Value.ToString(), dataGridView5.Rows[i].Cells[4].Value.ToString(), dataGridView5.Rows[i].Cells[5].Value.ToString(), dataGridView5.Rows[i].Cells[6].Value.ToString(), dataGridView5.Rows[i].Cells[7].Value.ToString(), dataGridView5.Rows[i].Cells[8].Value.ToString(), dataGridView5.Rows[i].Cells[9].Value.ToString(), dataGridView5.Rows[i].Cells[10].Value.ToString(), dataGridView5.Rows[i].Cells[11].Value.ToString(), dataGridView5.Rows[i].Cells[12].Value.ToString(), dataGridView5.Rows[i].Cells[13].Value.ToString(), dataGridView5.Rows[i].Cells[14].Value.ToString(), dataGridView5.Rows[i].Cells[15].Value.ToString(), sucursal);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void ingresaMercaderia(string CodigoM, string NombreSucursal, string Proveedor, string Descripcion, string PrecioCompra, double TotalUnidades )
          {
              OdbcConnection conexion = ASG_DB.connectionResult();
              try
              {
                  string sql = string.Format("CALL INGRESA_MERCADERIA_SUCURSAL ('{0}','{1}','{2}','{3}',{4},{5},TRUE);", CodigoM, NombreSucursal, Proveedor, Descripcion, PrecioCompra, TotalUnidades);
                  OdbcCommand cmd = new OdbcCommand(sql, conexion);
                  if (cmd.ExecuteNonQuery() == 1)
                  {                     
                      // MessageBox.Show("INGRESO CORRECTO MERCADERIA A SUCURSAL NUEVA");
                      for (int i = 0; i < dataGridView5.RowCount; i++)
                      {
                          presentacionesIngreso(CodigoM, dataGridView5.Rows[i].Cells[0].Value.ToString(), dataGridView5.Rows[i].Cells[1].Value.ToString(), dataGridView5.Rows[i].Cells[2].Value.ToString(), dataGridView5.Rows[i].Cells[3].Value.ToString(), dataGridView5.Rows[i].Cells[4].Value.ToString(), dataGridView5.Rows[i].Cells[5].Value.ToString(), dataGridView5.Rows[i].Cells[6].Value.ToString(), dataGridView5.Rows[i].Cells[7].Value.ToString(), dataGridView5.Rows[i].Cells[8].Value.ToString(), dataGridView5.Rows[i].Cells[9].Value.ToString(), dataGridView5.Rows[i].Cells[10].Value.ToString(), dataGridView5.Rows[i].Cells[11].Value.ToString(), dataGridView5.Rows[i].Cells[12].Value.ToString(), dataGridView5.Rows[i].Cells[13].Value.ToString(), dataGridView5.Rows[i].Cells[14].Value.ToString(), dataGridView5.Rows[i].Cells[15].Value.ToString(), NombreSucursal);
                      }
                                          
                  }
              }
              catch (Exception ex)
              {
                  MessageBox.Show(ex.ToString());
              }
              conexion.Close();
          }
        private void ingresaEspecias(string CodigoM, string NombreSucursal, string Proveedor, string Descripcion, string PrecioCompra, double TotalUnidades)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL INGRESA_ESPECIAS_SUCURSAL ('{0}','{1}','{2}','{3}',{4},{5},TRUE);", CodigoM, NombreSucursal, Proveedor, Descripcion, PrecioCompra, TotalUnidades);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    // MessageBox.Show("INGRESO CORRECTO MERCADERIA A SUCURSAL NUEVA");
                    for (int i = 0; i < dataGridView5.RowCount; i++)
                    {
                        presentacionesIngreso(CodigoM, dataGridView5.Rows[i].Cells[0].Value.ToString(), dataGridView5.Rows[i].Cells[1].Value.ToString(), dataGridView5.Rows[i].Cells[2].Value.ToString(), dataGridView5.Rows[i].Cells[3].Value.ToString(), dataGridView5.Rows[i].Cells[4].Value.ToString(), dataGridView5.Rows[i].Cells[5].Value.ToString(), dataGridView5.Rows[i].Cells[6].Value.ToString(), dataGridView5.Rows[i].Cells[7].Value.ToString(), dataGridView5.Rows[i].Cells[8].Value.ToString(), dataGridView5.Rows[i].Cells[9].Value.ToString(), dataGridView5.Rows[i].Cells[10].Value.ToString(), dataGridView5.Rows[i].Cells[11].Value.ToString(), dataGridView5.Rows[i].Cells[12].Value.ToString(), dataGridView5.Rows[i].Cells[13].Value.ToString(), dataGridView5.Rows[i].Cells[14].Value.ToString(), dataGridView5.Rows[i].Cells[15].Value.ToString(),NombreSucursal);
                    }
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private bool existenciaProducto(string codigoProducto, string nombreSucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {               
                string sql = string.Format("SELECT ID_MERCADERIA FROM VISTA_MERCADERIA WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", codigoProducto, nombreSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
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
        private int tipoTraslado(string codigoProducto, string nombreSucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CLASIFICACION FROM VISTA_MERCADERIA WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", codigoProducto, nombreSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return Convert.ToInt32(reader.GetInt32(0)); 
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return 1;
        }
        private void getSobrante(string tipoTraslado, double sobranteTraslado)
        {
            if (tipoTraslado == "1")
            {
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    dataGridView3.Rows[i].Cells[3].Value = Math.Truncate(sobranteTraslado / Convert.ToDouble(dataGridView3.Rows[i].Cells[2].Value.ToString()));
                }
            }
            else if (tipoTraslado == "2")
            {
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    dataGridView3.Rows[i].Cells[3].Value = Math.Round(sobranteTraslado/ Convert.ToDouble(dataGridView3.Rows[i].Cells[2].Value.ToString()), 2);
                }
            }
        }
        private void duplicaTraslado()
        {
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                dataGridView5.Rows.Add(dataGridView3.Rows[i].Cells[0].Value, dataGridView3.Rows[i].Cells[1].Value, dataGridView3.Rows[i].Cells[2].Value, 0, dataGridView3.Rows[i].Cells[4].Value,
                dataGridView3.Rows[i].Cells[5].Value, dataGridView3.Rows[i].Cells[6].Value, dataGridView3.Rows[i].Cells[7].Value, dataGridView3.Rows[i].Cells[8].Value, dataGridView3.Rows[i].Cells[9].Value,
                dataGridView3.Rows[i].Cells[10].Value, dataGridView3.Rows[i].Cells[11].Value, dataGridView3.Rows[i].Cells[12].Value, dataGridView3.Rows[i].Cells[13].Value, dataGridView3.Rows[i].Cells[14].Value, dataGridView3.Rows[i].Cells[15].Value);                    
            }
           
        }
        private void getDisponiblesIingresoActualizacion(string tipoTraslado, double cantidadTotal)
        {
            if (tipoTraslado == "1")
            {

                for (int i = 0; i < dataGridView5.RowCount; i++)
                {
                    dataGridView5.Rows[i].Cells[3].Value = Math.Truncate(cantidadTotal / Convert.ToDouble(dataGridView5.Rows[i].Cells[2].Value.ToString()));
                }

            }
            else if (tipoTraslado == "2")
            {
                for (int i = 0; i < dataGridView5.RowCount; i++)
                {
                    dataGridView5.Rows[i].Cells[3].Value = Math.Round(cantidadTotal / Convert.ToDouble(dataGridView5.Rows[i].Cells[2].Value.ToString()), 2);
                }
            }
        }
        private void getDisponiblesIingreso(string tipoTraslado, double cantidadTrasladar)
        {
            if (tipoTraslado == "1")
            {

                for (int i = 0; i < dataGridView5.RowCount; i++)
                {
                    dataGridView5.Rows[i].Cells[3].Value = Math.Truncate( cantidadTrasladar / Convert.ToDouble(dataGridView5.Rows[i].Cells[2].Value.ToString()));
                }

            }
            else if (tipoTraslado == "2")
            {
                for (int i = 0; i < dataGridView5.RowCount; i++)
                {
                    dataGridView5.Rows[i].Cells[3].Value = Math.Round(cantidadTrasladar / Convert.ToDouble(dataGridView5.Rows[i].Cells[2].Value.ToString()), 2);
                }
            }
        }
        private void getdataTraslado(string mercaderia, string sucursal, string TrasladoTipo, double sobrante)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {               
                string sql = string.Format("SELECT * FROM VISTA_STOCK WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", mercaderia, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView3.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                    while (reader.Read())
                    {
                        dataGridView3.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                        //styleDV(this.dataGridView3);
                    }
                    getSobrante(TrasladoTipo, sobrante);
                    duplicaTraslado();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void obtieneDatosExistentes(string mercaderia, string sucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_STOCK WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", mercaderia, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView5.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                    while (reader.Read())
                    {
                        dataGridView5.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                        //styleDV(this.dataGridView3);
                    }   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void getdataTrasladoExiste(string mercaderia, string sucursal, string TrasladoTipo, double sobrante)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {               
                string sql = string.Format("SELECT * FROM VISTA_STOCK WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", mercaderia, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView3.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                    while (reader.Read())
                    {
                        dataGridView3.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                        //styleDV(this.dataGridView3);
                    }
                    getSobrante(TrasladoTipo, sobrante);                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private double CantidadExistenteProducto(string mercaderia, string sucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT TOTAL_UNIDADES FROM VISTA_MERCADERIA WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", mercaderia, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return reader.GetDouble(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return 0;
        }
        public string createCodeInt(int longitud)
        {
            string caracteres = "1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < longitud--)
            {
                res.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            return res.ToString();
        }
        private bool codigoTraslado(string code)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_TRASLADO FROM TRASLADOS WHERE ID_TRASLADO =  {0};", code);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //MessageBox.Show("EL CODIGO YA EXITSTE");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "TRASLADO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
            return true;
        }
        private string getCodeTraslado()
        {
            bool codex = true;
            while (codex)
            {
                string temp = createCodeInt(5);
                if (codigoTraslado(temp))
                {
                    codex = false;
                    return temp;
                }
            }
            return null;
        }
        private void ingresaTraslado(string codeTraslado, string destino)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL NUEVO_TRASLADO ({0},'{1}','{2}','{3}',{4});", codeTraslado, usuario, comboBox1.Text, destino, dataGridView4.RowCount);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    adressUser.ingresaBitacora(idSucursal, usuario, "TRASLADO A - " + destino, codeTraslado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void ingresoProductosTraslado(string traslado, string mercaderia, string sucursal, double cantidad)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL INGRESA_PRODUCTOS_TRASLADO ({0},'{1}','{2}',{3});", traslado, mercaderia, sucursal, cantidad);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private int getDisponibleTraslado(string mercaderia, string sucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT TOTAL_UNIDADES FROM MERCADERIA WHERE ID_MERCADERIA =  '{0}' AND ID_SUCURSAL = (SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{1}' LIMIT 1);", mercaderia, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //MessageBox.Show("EL CODIGO YA EXITSTE");
                    return reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "TRASLADO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
            return 0;
        }
        private void button2_Click(object sender, EventArgs e)
        {
          if(dataGridView4.RowCount > 0)
          {
                if (sucursalSeleccionada())
                {
                    if (validacionesTraslados())
                    {
                        string temp = "";
                        for (int i = 0; i<dataGridView1.RowCount; i++)
                        {
                            // TOMA PRIMER SUCURSAL SELECCIONADA                          
                            temp = getCodeTraslado();
                           
                            if (dataGridView1.Rows[i].Cells[2].Value != null)
                            {
                                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[2].Value.ToString()))
                                {
                                    ingresaTraslado(temp, dataGridView1.Rows[i].Cells[1].Value.ToString());
                                    for (int j = 0; j < dataGridView4.RowCount; j++)
                                    {
                                        // RECORRE TODOS LOS PROUCTOS SELECCIONADOS

                                        if (existenciaProducto(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString()))
                                        {

                                            double disponible = getDisponibleTraslado(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString()) - Convert.ToDouble(dataGridView4.Rows[j].Cells[5].Value.ToString());
                                            double cantidadAplicada = Convert.ToDouble(dataGridView4.Rows[j].Cells[5].Value.ToString());
                                            //MessageBox.Show("EL PRODUCTO YA EXISTE EN LA SUCURSAL SE ACTUARIARA DISPONBILE");
                                            ingresoProductosTraslado(temp, dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString(), cantidadAplicada);
                                            if (tipoTraslado(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString()) == 1)
                                            {
                                                // TRASLADO DE ABARROTES QUE YA EXISTEN
                                                //MessageBox.Show("TRASLADO DE ABARROTES");
                                                getdataTrasladoExiste(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString(), "1", disponible);
                                                obtieneDatosExistentes(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString());
                                                double cantidad = CantidadExistenteProducto(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString()) + cantidadAplicada;
                                                //MessageBox.Show("" + CantidadExistenteProducto(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString()));
                                                getDisponiblesIingresoActualizacion("1", cantidad);
                                                actualizaMercaderia(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[3].Value.ToString(), disponible, dataGridView4.Rows[j].Cells[1].Value.ToString());
                                                actualizaMercaderiaExistente(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[3].Value.ToString(), cantidad, dataGridView1.Rows[i].Cells[1].Value.ToString());
                                                dataGridView3.Rows.Clear();
                                                dataGridView5.Rows.Clear();
                                            }
                                            else if (tipoTraslado(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString()) == 2)
                                            {
                                                // TRASLADO DE ESPECIAS QUE YA EXISTEN
                                                //MessageBox.Show("TRASLADO DE ESPECIAS");
                                                getdataTrasladoExiste(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString(), "2", disponible);
                                                obtieneDatosExistentes(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString());
                                                double cantidad = CantidadExistenteProducto(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString()) + cantidadAplicada;
                                                //MessageBox.Show("" + CantidadExistenteProducto(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString()));
                                                getDisponiblesIingresoActualizacion("2", cantidad);
                                                actualizaMercaderia(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[3].Value.ToString(), disponible, dataGridView4.Rows[j].Cells[1].Value.ToString());
                                                actualizaMercaderiaExistente(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[3].Value.ToString(), cantidad, dataGridView1.Rows[i].Cells[1].Value.ToString());
                                                dataGridView3.Rows.Clear();
                                                dataGridView5.Rows.Clear();
                                            }
                                        }
                                        else
                                        {
                                            //MessageBox.Show("EL PRODUCTO SERA INGRESADO POR PRIMERA VEZ");
                                            double disponible = getDisponibleTraslado(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString()) - Convert.ToDouble(dataGridView4.Rows[j].Cells[5].Value.ToString());
                                            double cantidadAplicada = Convert.ToDouble(dataGridView4.Rows[j].Cells[5].Value.ToString());
                                            ingresoProductosTraslado(temp, dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString(), cantidadAplicada);
                                            if (tipoTraslado(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString()) == 1)
                                            {
                                                // INGRESO NUEVO DE ABARROTTES QUE NO EXISTEN
                                                //MessageBox.Show("TRASLADO DE ABARROTES");
                                                getdataTraslado(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString(), "1", disponible);
                                                getDisponiblesIingreso("1", cantidadAplicada);
                                                ingresaMercaderia(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString(), dataGridView4.Rows[j].Cells[6].Value.ToString(), dataGridView4.Rows[j].Cells[2].Value.ToString(), dataGridView4.Rows[j].Cells[3].Value.ToString(), cantidadAplicada);
                                                actualizaMercaderia(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[3].Value.ToString(), disponible, dataGridView4.Rows[j].Cells[1].Value.ToString());
                                                dataGridView3.Rows.Clear();
                                                dataGridView5.Rows.Clear();
                                            }
                                            else if (tipoTraslado(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString()) == 2)
                                            {
                                                // INGRESO NUEVO DE ESPECIAS QUE NO EXISTEN
                                                // MessageBox.Show("TRASLADO DE ESPECIAS");
                                                getdataTraslado(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[1].Value.ToString(), "2", disponible);
                                                getDisponiblesIingreso("2", cantidadAplicada);
                                                ingresaEspecias(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString(), dataGridView4.Rows[j].Cells[6].Value.ToString(), dataGridView4.Rows[j].Cells[2].Value.ToString(), dataGridView4.Rows[j].Cells[3].Value.ToString(), cantidadAplicada);
                                                actualizaMercaderia(dataGridView4.Rows[j].Cells[0].Value.ToString(), dataGridView4.Rows[j].Cells[3].Value.ToString(), disponible, dataGridView4.Rows[j].Cells[1].Value.ToString());
                                                dataGridView3.Rows.Clear();
                                                dataGridView5.Rows.Clear();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        var fomra = new frm_done();
                        fomra.ShowDialog();
                        dataGridView1.Rows.Clear();
                        dataGridView4.Rows.Clear();
                        comboBox1.SelectedIndex = -1;
                        comboBox1.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("NO HA SELECCIONADO UNA SUCURSAL DE DESTINO!", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
          } else
            {
                MessageBox.Show("DEBE SELECCIONAR AL MENOS UN PRODUCTO PARA TRASLADAR!", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
        private void my_menu_ItemclickedSC(object sender, ToolStripItemClickedEventArgs e)
        {
            if (flagsc == false)
            {
                if (e.ClickedItem.Name == "ColHidden")
                {
                    mymenuS.Visible = false;
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR EL PRODUCTO DEL TRASLADO?", "TRASLADO DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        dataGridView4.Rows.RemoveAt(dataGridView4.CurrentRow.Index);
                    }
                    flagsc = true;
                }

            }
        }
        private void dataGridView4_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView4.RowCount > 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    mymenuS.Show(dataGridView4, new Point(e.X, e.Y));
                    mymenuS.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_ItemclickedSC);
                    mymenuS.Enabled = true;
                    flagsc = false;
                }
            }
        }

        private void dataGridView4_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Delete)
            {
                if(dataGridView4.RowCount > 0)
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR EL PRODUCTO DEL TRASLADO?", "TRASLADO DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        dataGridView4.Rows.RemoveAt(dataGridView4.CurrentRow.Index);
                    }                   
                }
            }
        }
    }
}
