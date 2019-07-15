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
    public partial class frm_devoluciones : Form
    {
        string nameUs;
        string rolUs;
        string usuario;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string idSucursal;
        string totalFactura;
        bool flag = false;
       // bool flags = false;
        string codigoCaja;
        ContextMenuStrip mymenu = new ContextMenuStrip();
        ContextMenuStrip mymenuS = new ContextMenuStrip();
        public frm_devoluciones(string nameUser, string rolUser, string user, string codigo, string codigoCajaExterno)
        {
            InitializeComponent();
            usuario = user;
            nameUs = nameUser;
            rolUs = rolUser;
            codigoCaja = codigoCajaExterno;
            idSucursal = codigo;
            stripMenu();
            stripMenuS();
            label19.Text = label19.Text + "" + nameUs;
            cargaDatos();
            
            if (rolUs != "ADMINISTRADOR")
            {
                mymenu.Items[1].Enabled = false;
                //button3.Enabled = false;
            }
        }
        private void eliminaDevolucion(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE DEVOLUCION SET ESTADO_TUPLA = FALSE WHERE ID_DEVOLUCION = {0};",codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    var forma = new frm_done();
                    forma.ShowDialog();
                    cargaDatos();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO ANULAR LA FACTURA!", "ANULAR FACTURAS", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void my_menu_Itemclicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (flag == false)
            {
                if (e.ClickedItem.Name == "ColHidden")
                {
                    dataGridView1.Rows[this.dataGridView1.CurrentRow.Index].Visible = false;
                    flag = true;
                }
                else if (e.ClickedItem.Name == "ColEdit")
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR LA DEVOLUCION?", "DEVOLUCIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        eliminaDevolucion(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                    }                   
                    mymenu.Visible = false;
                    flag = true;

                }
            }
        }
        private void my_menu_ItemclickedS(object sender, ToolStripItemClickedEventArgs e)
        {
            /*if (flags == false)
            {
                if (e.ClickedItem.Name == "ColHidden")
                {
                    dataGridView2.Rows.RemoveAt(dataGridView2.CurrentRow.Index);
                    flags = true;
                }
                
            }*/
        }
        private void stripMenu()
        {
            mymenu.Items.Add("Ocultar Fila");
            mymenu.Items[0].Name = "ColHidden";
            mymenu.Items.Add("Eliminar Devolucion");
            mymenu.Items[1].Name = "ColEdit";
        }
        private void stripMenuS()
        {
            mymenuS.Items.Add("Eliminar de Devolucion");
            mymenuS.Items[0].Name = "ColHidden";
           
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (dataGridView2.RowCount > 0 || textBox5.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "DEVOLUCIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
                this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void frm_devoluciones_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_devoluciones_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_devoluciones_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_devoluciones_MouseUp_1(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_devoluciones_Load(object sender, EventArgs e)
        {
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
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
        private bool AnulaFactura(string numeroFactura)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE FACTURA SET ESTADO_TUPLA = FALSE WHERE ID_FACTURA = '{0}';", numeroFactura);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("IMPOSIBE ANULAR FACTURA!", "FACTURAS", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return false;
        }
        private void frm_devoluciones_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if(dataGridView2.RowCount > 0 || textBox5.Text != "")
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "ANULACION FACTURA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    }
                } else
                this.Close();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.B))
            {
                if (tabControl1.SelectedTab == tabPage1)
                {
                    button6.PerformClick();
                }
               
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                if (tabControl1.SelectedTab == tabPage1)
                {
                    button3.PerformClick();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
        private void duplicaFactura(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT F.ID_CLIENTE, F.ID_FACTURA, F.FECHA_EMISION_FACTURA, (SELECT NOMBRE_CLIENTE FROM CLIENTE WHERE ID_CLIENTE = F.ID_CLIENTE ), (SELECT APELLIDO_CLIENTE FROM CLIENTE WHERE ID_CLIENTE = F.ID_CLIENTE ), F.TIPO_FACTURA, F.ID_SUCURSAL, F.SUBTOTAL_FACTURA, F.DESCUENTO_FACTURA, F.TOTAL_FACTURA FROM FACTURA F WHERE ID_FACTURA  = '{0}';", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // MessageBox.Show("FACTURA DUPLICADA");
                    textBox4.Text = reader.GetString(0);
                    duplicaDetalle(reader.GetString(1));
                    textBox2.Text = reader.GetString(2);
                    textBox3.Text = reader.GetString(3) + " " + reader.GetString(4);
                    textBox6.Text = reader.GetString(5);
                    cargaSucursales(reader.GetString(6));
                    label11.Text = string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7));
                    label8.Text = string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8));
                    totalFactura = "" + reader.GetDouble(9);
                    label12.Text = string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(9));
                   
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaSucursales(string sucursal)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT NOMBRE_SUCURSAL FROM SUCURSAL WHERE ESTADO_TUPLA = TRUE AND ID_SUCURSAL = {0};", sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    comboBox5.Items.Add(reader.GetString(0));
                    comboBox5.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void duplicaDetalle(string codigo )
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_DETALLE_FACTURA WHERE ID_FACTURA  = '{0}';", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView2.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                    }
                    //totalFactura();
                } else
                {
                    MessageBox.Show("IMPOSIBLE OBTENER DATOS DE LA FACTURA");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private bool actualizaDisponible(double restante, string codigo, string sucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE MERCADERIA SET TOTAL_UNIDADES = {0} WHERE ID_MERCADERIA = '{1}' AND ID_SUCURSAL = {2};", restante, codigo, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    // MessageBox.Show("IMPOSIBLE ACTUAIZAR DISPONIBLE!", "NUEVA VENTA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return false;
        }
        private void guardaDatos()
        {
            if (dataGridView3.RowCount > 0)
            {
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    try
                    {
                        string sql = string.Format("UPDATE MERCADERIA_PRESENTACIONES SET TOTAL_PRESENTACIONES = {0} WHERE ID_STOCK = '{1}'  AND ESTADO_TUPLA = TRUE AND ID_SUCURSAL = {2};", dataGridView3.Rows[i].Cells[2].Value.ToString(), dataGridView3.Rows[i].Cells[0].Value.ToString(), dataGridView3.Rows[i].Cells[3].Value.ToString());
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            //MessageBox.Show("ACTUALIZADO");
                        }
                        else
                        {
                            //MessageBox.Show("imposible actualizar");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    conexion.Close();
                }
            }
        }
        private void inventarioDex(double disponible, double cantidad, string tipo, string codigo, string sucursal)
        {
            if (dataGridView3.RowCount > 0)
            {
                double restante = disponible + cantidad;
                if (restante > 0)
                {
                    for (int i = 0; i < dataGridView3.RowCount; i++)
                    {
                        if (tipo == "1")
                        {
                            dataGridView3.Rows[i].Cells[2].Value = Math.Truncate((restante / Convert.ToDouble(dataGridView3.Rows[i].Cells[1].Value.ToString())));
                        }
                        else
                        {
                            dataGridView3.Rows[i].Cells[2].Value = Math.Round(restante / Convert.ToDouble(dataGridView3.Rows[i].Cells[1].Value.ToString()), 2);
                        }
                    }
                    //dataGridView2.Visible = true;
                    if (actualizaDisponible(restante, codigo, sucursal))
                    {
                        guardaDatos();
                        dataGridView3.Rows.Clear();
                    }
                    //MessageBox.Show("data convertida exactamente");
                }
                else
                {
                    for (int i = 0; i < dataGridView3.RowCount; i++)
                    {
                        dataGridView3.Rows[i].Cells[2].Value = 0;
                    }
                    // dataGridView2.Visible = true;
                    if (actualizaDisponible(restante, codigo, sucursal))
                    {
                        guardaDatos();
                        dataGridView3.Rows.Clear();
                    }
                    // MessageBox.Show("data convertida exactamente");
                }
            }
        }
        // FUNCION QUE PERMITE RECALCULAR EL STOCK DE UN PRODUCTO QUE ENTRA A INVENTARIO POR DEVOLUCION
        private void recalculaStock(string clasificacion, double disponible, string sucursal, string producto, double cantidad)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_STOCK, CANTIDAD_POR_PRESENTACION, TOTAL_PRESENTACIONES, ID_SUCURSAL FROM MERCADERIA_PRESENTACIONES WHERE ID_MERCADERIA = '{0}' AND ID_SUCURSAL = {1} AND ESTADO_TUPLA = TRUE;", producto, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView3.Rows.Clear();
                    dataGridView3.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                    while (reader.Read())
                    {
                        dataGridView3.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                    }
                    inventarioDex(disponible, cantidad, clasificacion, producto, sucursal);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void algoritmoRecalculo(string codigo, string sucursal, double cantidad)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CLASIFICACION, TOTAL_UNIDADES, ID_SUCURSAL FROM MERCADERIA WHERE ID_MERCADERIA = '{0}' AND ID_SUCURSAL = (SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{1}' LIMIT 1);", codigo, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //MessageBox.Show(reader.GetString(0) + "  " +reader.GetDouble(1) + " " +reader.GetString(1));
                    recalculaStock(reader.GetString(0), reader.GetDouble(1), reader.GetString(2), codigo, cantidad);
                }
                else
                {
                    // MessageBox.Show("erroneo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        // FUNCION QUE OBTIENE LOS SUB-CODIGOS / CPP / DISPONIBLE
        private void actualizaStock()
        {
            if(dataGridView2.RowCount > 0)
            {
                for(int i = 0; i<dataGridView2.RowCount; i++)
                {
                    double cantidad = Convert.ToDouble(dataGridView2.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(dataGridView2.Rows[i].Cells[8].Value.ToString());
                    algoritmoRecalculo(dataGridView2.Rows[i].Cells[0].Value.ToString(), dataGridView2.Rows[i].Cells[6].Value.ToString(), cantidad);
                }
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            var forma = new frm_duplicaFactura();
            if (forma.ShowDialog() == DialogResult.OK)
            {
                limpiaFormulario();
                textBox5.Text = forma.currentFactura.codigoFactura;
                dataGridView2.Rows.Clear();
                duplicaFactura(forma.currentFactura.codigoFactura);
            }
        }
      
        private void limpiaFormulario()
        {
            textBox5.Text = "";
            textBox4.Text = "";
            textBox2.Text = "";
            textBox6.Text = "";
            textBox3.Text = "";
            comboBox5.Items.Clear();
            dataGridView2.Rows.Clear();
            label11.Text = "";
            label8.Text = "";
            label12.Text = "";

        }
      
        private void button3_Click(object sender, EventArgs e)
        {
            if ((dataGridView2.RowCount > 0) &&(textBox5.Text != ""))
            {
                DialogResult result = MessageBox.Show("SE AGREGARA EL PRODUCTO AL INVENTARIO ¿DESEA CONTINUAR?", "AULACION FACTURAS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    actualizaStock();
                    var xcode = new frm_done();
                    xcode.ShowDialog();
                    if (AnulaFactura(textBox5.Text))
                    {


                        var forma = new frm_completaDevolucion(textBox5.Text.Trim(), usuario, idSucursal, textBox3.Text, nameUs, totalFactura);
                        if (forma.ShowDialog() == DialogResult.OK)
                        {
                            limpiaFormulario();
                        }
                    }

                } else
                {
                    var forma = new frm_completaDevolucion(textBox5.Text.Trim(),usuario,idSucursal,textBox3.Text,nameUs, totalFactura);
                     if(forma.ShowDialog() == DialogResult.OK)
                    {
                        limpiaFormulario();
                    }
                }
            } else
            {
                MessageBox.Show("DEBE DE LLENAR LOS DATOS DE LA DEVOLUCION", "DEVOLUCIONES", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "DEVLUCIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                limpiaFormulario();
            }

        }
        private void cargaDatos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_DEVOLUCIONES ORDER BY FECHA_DEVOLUCION DESC;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1),reader.GetString(2) + " " +reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
       
        private void button4_Click_1(object sender, EventArgs e)
        {
            cargaDatos();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_DEVOLUCIONES WHERE FECHA_DEVOLUCION LIKE '%{0}%' ORDER BY FECHA_DEVOLUCION DESC;", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    dataGridView1.Rows.Clear();
                    string sql = string.Format("SELECT * FROM VISTA_DEVOLUCIONES WHERE ID_FACTURA LIKE '%{0}%' ORDER BY FECHA_DEVOLUCION DESC;", textBox1.Text.Trim());
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                        }
                    }
                    else
                    {
                        dataGridView1.Rows.Clear();
                        sql = string.Format("SELECT * FROM VISTA_DEVOLUCIONES WHERE NOMBRE_CLIENTE LIKE '%{0}%' ORDER BY FECHA_DEVOLUCION DESC;", textBox1.Text.Trim());
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                            }
                        }
                        else
                        {
                            dataGridView1.Rows.Clear();
                            sql = string.Format("SELECT * FROM VISTA_DEVOLUCIONES WHERE APELLIDO_CLIENTE LIKE '%{0}%' ORDER BY FECHA_DEVOLUCION DESC;", textBox1.Text.Trim());
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                }
                            }
                            else
                            {
                                dataGridView1.Rows.Clear();
                                sql = string.Format("SELECT * FROM VISTA_DEVOLUCIONES WHERE TIPO_DEVOLUCION LIKE '%{0}%' ORDER BY FECHA_DEVOLUCION DESC;", textBox1.Text.Trim());
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                    }
                                } else
                                {
                                    dataGridView1.Rows.Clear();
                                    sql = string.Format("SELECT * FROM VISTA_DEVOLUCIONES WHERE FECHA_DEVOLUCION LIKE '%{0}%' ORDER BY FECHA_DEVOLUCION DESC;", textBox1.Text.Trim());
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                        while (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                        }
                                    } else
                                    {
                                        dataGridView1.Rows.Clear();
                                        sql = string.Format("SELECT * FROM VISTA_DEVOLUCIONES WHERE SUBTOTAL_FACTURA LIKE '%{0}%' ORDER BY FECHA_DEVOLUCION DESC;", textBox1.Text.Trim());
                                        cmd = new OdbcCommand(sql, conexion);
                                        reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                            while (reader.Read())
                                            {
                                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                            }
                                        } else
                                        {
                                            dataGridView1.Rows.Clear();
                                            sql = string.Format("SELECT * FROM VISTA_DEVOLUCIONES WHERE DESCUENTO_FACTURA LIKE '%{0}%' ORDER BY FECHA_DEVOLUCION DESC;", textBox1.Text.Trim());
                                            cmd = new OdbcCommand(sql, conexion);
                                            reader = cmd.ExecuteReader();
                                            if (reader.Read())
                                            {
                                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                                while (reader.Read())
                                                {
                                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                                }
                                            } else
                                            {
                                                dataGridView1.Rows.Clear();
                                                sql = string.Format("SELECT * FROM VISTA_DEVOLUCIONES WHERE TOTAL_FACTURA LIKE '%{0}%' ORDER BY FECHA_DEVOLUCION DESC;", textBox1.Text.Trim());
                                                cmd = new OdbcCommand(sql, conexion);
                                                reader = cmd.ExecuteReader();
                                                if (reader.Read())
                                                {
                                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                                    while (reader.Read())
                                                    {
                                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                                    }
                                                } else
                                                {
                                                    dataGridView1.Rows.Clear();
                                                    sql = string.Format("SELECT * FROM VISTA_DEVOLUCIONES WHERE VENDEDOR LIKE '%{0}%' ORDER BY FECHA_DEVOLUCION DESC;", textBox1.Text.Trim());
                                                    cmd = new OdbcCommand(sql, conexion);
                                                    reader = cmd.ExecuteReader();
                                                    if (reader.Read())
                                                    {
                                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                                        while (reader.Read())
                                                        {
                                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
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
            else
            {
                cargaDatos();
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    mymenu.Show(dataGridView1, new Point(e.X, e.Y));
                    mymenu.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_Itemclicked);
                    mymenu.Enabled = true;
                    flag = false;
                }
            }
        }

        private void dataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView2.RowCount > 0)
            {
              /*  if (e.Button == MouseButtons.Right)
                {
                    mymenuS.Show(dataGridView2, new Point(e.X, e.Y));
                    mymenuS.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_ItemclickedS);
                    mymenuS.Enabled = true;
                    flags = false;
                }*/
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Delete)
            {

            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                var fomra = new frm_detalleFactura(dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[5].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(), dataGridView1.CurrentRow.Cells[7].Value.ToString(), dataGridView1.CurrentRow.Cells[2].Value.ToString(), dataGridView1.CurrentRow.Cells[4].Value.ToString(),true, dataGridView1.CurrentRow.Cells[9].Value.ToString(), dataGridView1.CurrentRow.Cells[8].Value.ToString(),null, dataGridView1.CurrentRow.Cells[10].Value.ToString(), 0,false);
                fomra.ShowDialog();
            }
        }
    }
}
