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
using CrystalDecisions.CrystalReports.Engine;

namespace ASG
{
    public partial class frm_venta : Form
    {
        string nameUs;
        string rolUs;
        string usuario;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string idSucursal;
        double dias;
        string nombreSucursal;
        double total = 0;
        double subtotal = 0;
        double descuento = 0;
        string numeroCuenta;
        bool preferenciaVenta = false;
        string codigoCaja;
        bool flags = false;
        bool existente = false;
        bool cotizacion = false;
        string monto_factura;
        ContextMenuStrip mymenuS = new ContextMenuStrip();
        bool[] privilegios;
        public frm_venta(string nameUser, string rolUser, string user, string sucursal, string sucursalNombre, string codigoCajaExterno, bool[] privilegio)
        {
            InitializeComponent();
            usuario = user;
            nameUs = nameUser;
            rolUs = rolUser;
            this.privilegios = privilegio;
            codigoCaja = codigoCajaExterno;
            idSucursal = sucursal;
            nombreSucursal = sucursalNombre;
            label19.Text = label19.Text + "" + nameUs;
            cargaUsuarios();
            inicializaFactura();
            descuento = 0;
            stripMenuS();
            textBox11.Focus();
            if(privilegios[3] != true)
            {
                button1.Enabled = false;
            }
            
        }
       
        // FIN DE INICILIZACION
        private void stripMenuS()
        {
            mymenuS.Items.Add("Eliminar Producto de la Factura");
            mymenuS.Items[0].Name = "ColHidden";

        }
        
       
        private void cargaUsuarios()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT NOMBRE_USUARIO FROM USUARIO WHERE ESTADO_TUPLA = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //comboBox5.Items.Add(reader.GetString(0));
                    comboBox1.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        //comboBox5.Items.Add(reader.GetString(0));
                        comboBox1.Items.Add(reader.GetString(0));
                    }
                    //comboBox5.SelectedIndex = 0;
                    comboBox1.Text = nameUs;
                }
                else
                {
                    comboBox1.Items.Add("NO EXISTEN VENDEDORES");
                    comboBox1.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void inicializaFactura()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT FACTURA_DEFECTO FROM CONFIGURACION_SISTEMA;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetBoolean(0) == true)
                    {
                        iniciaDefault();

                    }
                    else
                        textBox2.Focus();
                }
                else
                {
                    textBox2.Focus();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
       
        
        private void frm_venta_Load(object sender, EventArgs e)
        {
            label9.Text = getCode();
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = false;
            dataGridView1.Columns[2].ReadOnly = false;
            dataGridView1.Columns[3].ReadOnly = false;
            dataGridView1.Columns[4].ReadOnly = false;
            dataGridView1.Columns[5].ReadOnly = false;
            //radioButton1.Checked = true;
            comboBox5.SelectedIndex = 0;
           


            //checkBox2.Checked = true;

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "VENTA RAPIDA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void frm_venta_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_venta_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_venta_MouseUp(object sender, MouseEventArgs e)
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

        private void frm_venta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if (dataGridView1.RowCount > 0)
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "VENTA RAPIDA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    }
                }
                else
                {
                    this.Close();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.E))
            {
                button1.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.C))
            {
                button7.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.D))
            {
                button6.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.B))
            {
                button4.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                button2.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.A))
            {
                var forma = new frm_buscaCliente(nameUs, rolUs, usuario, idSucursal, privilegios);
                forma.ShowDialog();
                if (forma.DialogResult == DialogResult.OK)
                {
                    textBox2.Text = forma.currentCliente.getCliente;
                }


            }
            else if (e.KeyData == Keys.F5)
            {
                if (preferenciaVenta)
                {
                    preferenciaVenta = false;
                    var forma = new frm_ventaNormal();
                    forma.Show();
                }
                else
                {
                    preferenciaVenta = true;
                    var forma = new frm_cambioVenta();
                    forma.Show();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.Q))
            {
                var forma = new frm_Precios(idSucursal, true, nombreSucursal, rolUs);
                 if(forma.ShowDialog() == DialogResult.OK)
                {
                    textBox11.Text = forma.CurrrentCodigo.getProducto;
                    textBox11.Focus();
                    SendKeys.Send("{ENTER}");
                }
            }     
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                var forma = new frm_buscaCliente(nameUs,rolUs,usuario,idSucursal, privilegios);
                forma.ShowDialog();
                if (forma.DialogResult == DialogResult.OK)
                {
                    textBox2.Text = forma.currentCliente.getCliente;
                }
            }
        }
        internal class Vendedor
        {
            public string getVendedor { get; set; }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT ID_CLIENTE, NOMBRE_CLIENTE, APELLIDO_CLIENTE, DIRECCION_CLIENTE, ID_TIPO_CLIENTE, DIAS_CREDITO FROM CLIENTE WHERE ID_CLIENTE = '{0}' AND ESTADO_TUPLA = TRUE;", textBox2.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textBox1.Text = reader.GetString(1) + " " + reader.GetString(2);
                        textBox3.Text = reader.GetString(3);
                        string typeOF = reader.GetString(4);
                        dias = reader.GetDouble(5);
                        iniciaVenta();
                        if (typeOF == "1001")
                        {
                            comboBox5.SelectedIndex = 1;
                        }
                        else
                        {
                            comboBox5.SelectedIndex = 0;
                        }
                        //textBox5.Focus();
                    }
                    else
                    {
                        textBox1.Text = "";
                        textBox3.Text = "";
                        comboBox5.SelectedIndex = 0;
                        textBox4.Text = "";
                        textBox6.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                textBox1.Text = "";
                textBox3.Text = "";
                comboBox5.SelectedIndex = 0;
                textBox4.Text = "";
                textBox6.Text = "";
            }
        }
        private void iniciaVenta()
        {
            DateTime today = DateTime.Now;
            textBox4.Text = today.ToString("yyyy-MM-dd");
            DateTime answer = today.AddDays(dias);
            textBox6.Text = answer.ToString("yyyy-MM-dd");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var forma = new frm_clientes(nameUs, rolUs, usuario, idSucursal, true, textBox2.Text, textBox1.Text, privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                string temp = textBox2.Text;
                textBox2.Text = "";
                temp = textBox2.Text;
            }
        }
        private void iniciaDefault()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT ID_CLIENTE, NOMBRE_CLIENTE, APELLIDO_CLIENTE, DIRECCION_CLIENTE, ID_TIPO_CLIENTE, DIAS_CREDITO FROM CLIENTE WHERE ID_CLIENTE = 'C/F' AND ESTADO_TUPLA = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    textBox2.Text = reader.GetString(0);
                    textBox1.Text = reader.GetString(1) + " " + reader.GetString(2);
                    textBox3.Text = reader.GetString(3);
                    string typeOF = reader.GetString(4);
                    dias = reader.GetDouble(5);
                    textBox10.Text = "0";
                    textBox4.Text = "0";
                    textBox6.Text = "0";
                    textBox5.Text = "0";
                    textBox9.Text = "N/A";
                    iniciaVenta();
                    if (typeOF == "1001")
                    {
                        comboBox5.SelectedIndex = 1;
                    }
                    else
                    {
                        comboBox5.SelectedIndex = 0;
                    }

                    //textBox5.Focus();
                }
                else
                {
                    textBox1.Text = "";
                    textBox3.Text = "";
                    comboBox5.SelectedIndex = 0;
                    textBox4.Text = "";
                    textBox6.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            iniciaDefault();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void textBox8_Enter(object sender, EventArgs e)
        {

        }

        private void textBox8_Leave(object sender, EventArgs e)
        {
          /*  if (textBox8.Text == "")
                textBox8.Text = "   ";*/
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var forma = new frm_clientes(nameUs, rolUs, usuario, idSucursal, true, textBox2.Text, textBox1.Text, privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                string temp = textBox2.Text;
                textBox2.Text = "";
                temp = textBox2.Text;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            var forma = new frm_buscaCliente(nameUs, rolUs, usuario, idSucursal, privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                textBox2.Text = forma.currentCliente.getCliente;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            iniciaDefault();
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT * FROM VISTA_CLIENTE_VENTA WHERE ID_CLIENTE = '{0}';", textBox2.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textBox2.Text = reader.GetString(0);
                        textBox1.Text = reader.GetString(1) + " " + reader.GetString(2);
                        textBox3.Text = reader.GetString(3);
                        //textBox5.Text = "" + reader.GetDouble(4);
                        //textBox9.Text = "" + reader.GetDouble(5);
                        //textBox10.Text = "" + reader.GetInt32(6);
                        //dias = Convert.ToDouble(textBox10.Text); ;
                        string typeOF = reader.GetString(7);
                        iniciaVenta();
                        comboBox5.Text = "EFECTIVO";
                        //textBox5.Focus();
                        textBox11.Focus();
                    }
                    else
                    {
                        textBox1.Text = "";
                        textBox3.Text = "";
                        comboBox5.SelectedIndex = 0;
                        textBox4.Text = "";
                        textBox6.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                textBox1.Text = "";
                textBox3.Text = "";
                comboBox5.SelectedIndex = 0;
                textBox4.Text = "";
                textBox6.Text = "";
            }
        }

        private void getCreditos(string codigoCliente)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_CLIENTE_VENTA WHERE ID_CLIENTE = '{0}';", textBox2.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    textBox2.Text = reader.GetString(0);
                    textBox1.Text = reader.GetString(1) + " " + reader.GetString(2);
                    textBox3.Text = reader.GetString(3);
                    textBox5.Text = "" + reader.GetDouble(4);
                    textBox9.Text = "" + reader.GetDouble(5);
                    textBox10.Text = "" + reader.GetInt32(6);
                    dias = Convert.ToDouble(textBox10.Text); ;
                    string typeOF = reader.GetString(7);
                    iniciaVenta();
                    if (typeOF == "1001")
                    {
                        comboBox5.SelectedIndex = 1;
                    }
                    else
                    {
                        comboBox5.SelectedIndex = 0;
                    }
                    //textBox5.Focus();
                }
                else
                {
                    textBox1.Text = "";
                    textBox3.Text = "";
                    comboBox5.SelectedIndex = 0;
                    textBox4.Text = "";
                    textBox6.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.Text == "EFECTIVO")
            {
                if (textBox5.Text != "" | textBox10.Text != "" | textBox9.Text != "")
                {
                    textBox10.Text = "0";
                    textBox5.Text = "0";
                    textBox9.Text = "N/A";
                    DateTime today = DateTime.Now;
                    textBox4.Text = today.ToString("yyyy-MM-dd");
                    DateTime answer = today.AddDays(0);
                    textBox6.Text = answer.ToString("yyyy-MM-dd");
                }
            }
            else
            {
                if (textBox2.Text != "")
                {
                    getCreditos(textBox2.Text);
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void button4_Click(object sender, EventArgs e)
        {
            var Tempforma = new frm_buscaEdicion(nombreSucursal, rolUs);
            if (Tempforma.ShowDialog() == DialogResult.OK)
            {
                textBox11.Text = Tempforma.CurrentMercaderia.getMercaderia;
                var forma = new frm_getProducto(Tempforma.CurrentMercaderia.getMercaderia, Tempforma.CurrentSucursal.getSucursal, rolUs);
                if(forma.ShowDialog() == DialogResult.OK)
                {
                    // AGREGA A FACTURA
                    getRepetitivo(forma.currentProducto.getStock, forma.currentProducto.getProducto, forma.currentProducto.getDescripcion, forma.currentProducto.getPrice, forma.currentProducto.currentSucursal, forma.currentProducto.getCpp, forma.currentProducto.getPC);
                    //localizaCursor();
                    getTotales();
                    totalFactura();
                    textBox11.Text = "";
                    textBox11.Focus();
                    //dataGridView1.Rows.Add(forma.currentProducto.getProducto,forma.currentProducto.getDescripcion,1,forma.currentProducto.getPrice,0,0,forma.currentProducto.currentSucursal, forma.currentProducto.getStock);
                }
            } else
            {
                textBox11.Text = "";
                textBox11.Focus();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var forma = new frm_clientes(nameUs, rolUs, usuario, idSucursal, true, textBox2.Text, textBox1.Text, privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                string temp = textBox2.Text;
                textBox2.Text = "";
                textBox2.Text = temp;
            }
        }
        
        private bool existeMercaderia(string idMercaderia)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT ID_MERCADERIA FROM MERCADERIA WHERE ID_MERCADERIA = '{0}' AND ID_SUCURSAL = {1};", idMercaderia, idSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    conexion.Close();
                    return true;
                }
                else
                {
                    conexion = ASG_DB.connectionResult();
                    sql = string.Format("SELECT ID_MERCADERIA FROM MERCADERIA_PRESENTACIONES WHERE ID_STOCK = '{0}' AND ID_SUCURSAL = {1};", idMercaderia, idSucursal);
                    cmd = new OdbcCommand(sql, conexion);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textBox11.Text = reader.GetString(0);
                        conexion.Close();
                        return true;
                    }
                    else
                    {
                        sql = string.Format("SELECT ID_STOCK FROM SUBCODIGOS_MERCADERIA WHERE SUBCODIGO = '{0}' AND ESTADO_TUPLA = TRUE;", textBox11.Text.Trim());
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            string temp = reader.GetString(0);
                            conexion = ASG_DB.connectionResult();
                            sql = string.Format("SELECT ID_MERCADERIA FROM MERCADERIA_PRESENTACIONES WHERE ID_STOCK = '{0}' AND ID_SUCURSAL = {1};", temp, idSucursal);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                textBox11.Text = reader.GetString(0);
                                conexion.Close();
                                return true;
                            }
                        }
                        else
                        {
                            conexion.Close();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return false;
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (dataGridView1.CurrentCell.ColumnIndex <=1)
                {
                    if (e.KeyData == Keys.Delete)
                    {
                        if (!cotizacion)
                        {
                            eliminaExistente();

                        }
                        else
                        {
                            elimimaCotizacion();
                        }
                    }
                }
            }
        }
        private void eliminaExistente()
        {
            if (dataGridView1.RowCount > 0)
            {
                mymenuS.Visible = false;
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR EL PRODUCTO DE LA FACTURA?", "NUEVA VENTA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    if (!existente)
                    {
                        dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                        totalFactura();
                    }
                    else
                    {
                        MessageBox.Show("EL PRODUCTO SERA DEVUELTO AL INVENTARIO", "FACTURA EXISTENTE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (actualizaDetalle(dataGridView1.CurrentRow.Cells[9].Value.ToString()))
                        {

                            double cantidad = Convert.ToDouble(dataGridView1.CurrentRow.Cells[2].Value.ToString()) * Convert.ToDouble(dataGridView1.CurrentRow.Cells[8].Value.ToString());
                            algoritmoRecalculoMenor(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(), cantidad);
                            dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                            totalFactura();
                            timerActions();


                        }
                        else
                        {
                            MessageBox.Show("EL PRODUCTO NO PUDO SER ELIMINADO DE LA FACTURA", "FACTURA EXISTENTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
               
            }
        }
        private void elimimaCotizacion()
        {
            DialogResult result;
            mymenuS.Visible = false;
            result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR EL PRODUCTO DE LA COTIZACION?", "NUEVA VENTA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                if (actualizaDetalle(dataGridView1.CurrentRow.Cells[9].Value.ToString()))
                {
                    dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                    totalFactura();
                    timerActions();
                } else
                {
                    MessageBox.Show("EL PRODUCTO NO PUDO SER ELIMINADO DE LA FACTURA", "COTIZACION", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void my_menu_ItemclickedS(object sender, ToolStripItemClickedEventArgs e)
        {
            if (flags == false)
            {
                if (e.ClickedItem.Name == "ColHidden")
                {
                    if (!cotizacion)
                    {
                        eliminaExistente();

                    } else
                    {
                        elimimaCotizacion();
                    }
                    flags = true;
                }

            }
        }
        private bool actualizaDetalle(string detalle)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE DET_FACTURA SET ESTADO_TUPLA = FALSE WHERE ID_DET_FACTURA = {0};", detalle);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;      
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return false;
        }
        private void devuelveInventario()
        {

        }
        private void obtienePorFacturacion()
        {
            if (existeMercaderia(textBox11.Text.Trim()))
            {
                var forma = new frm_getProducto(textBox11.Text.Trim(), nombreSucursal, rolUs);
                if (forma.ShowDialog() == DialogResult.OK)
                {
                    // AGREGA A FACTURA
                    textBox11.Text = "";
                    getRepetitivo(forma.currentProducto.getStock, forma.currentProducto.getProducto, forma.currentProducto.getDescripcion, forma.currentProducto.getPrice, forma.currentProducto.currentSucursal, forma.currentProducto.getCpp, forma.currentProducto.getPC);
                    localizaCursor();
                    //dataGridView1.Rows.Add(forma.currentProducto.getProducto,forma.currentProducto.getDescripcion,1,forma.currentProducto.getPrice,0,0,forma.currentProducto.currentSucursal, forma.currentProducto.getStock);
                }
            }
        }
        private void textBox11_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if(textBox11.Text != "")
                {
                    if (preferenciaVenta)
                    {
                        obtienePorFacturacion();
                    } else
                    {
                        buscaSubcodigos();
                    }
                } else
                {
                    if (preferenciaVenta)
                    {
                        button4.PerformClick();
                    }
                }
            } else if (e.KeyData == Keys.Down)
            {
                if(dataGridView1.RowCount > 0)
                {
                    dataGridView1.Focus();
                }
            } else if (e.KeyData == Keys.Right)
            {
                if (dataGridView1.RowCount > 0)
                {
                    dataGridView1.CurrentCell = dataGridView1[2, dataGridView1.RowCount - 1];
                    dataGridView1.Focus();
                }
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
                    ventaRapida();
                } else
                {
                    ventaRapida();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void ventaRapida()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_STOCK_VENTA WHERE ID_STOCK = '{0}' AND NOMBRE_SUCURSAL = '{1}';", textBox11.Text.Trim(), nombreSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string stockventa = reader.GetString(0);
                    string codigoproducto = reader.GetString(1);
                    string decproducto = reader.GetString(2);
                    string descstock = reader.GetString(3);
                    string precioventa = reader.GetString(4);
                    string sucursal = reader.GetString(5);
                    string cpp = reader.GetString(6);
                    string precioCosto = reader.GetString(7);
                    string castingDescripcion = decproducto + " - " + descstock + " (" + cpp + ")";
                    getRepetitivo(stockventa, codigoproducto, castingDescripcion, precioventa, sucursal,cpp, precioCosto);
                    getTotales();
                    totalFactura();
                    textBox11.Text = "";
                    textBox11.Focus();
                    conexion.Close();
                } else
                {
                    textBox11.Text = "";
                    textBox11.Focus();
                    //MessageBox.Show("NO FUNCINO VENGTA DE STOCK");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void getTotales()
        {
            if( dataGridView1.RowCount > 0)
            {
                for(int i = 0; i<dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells[3].Value != null)
                    {
                        if (Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value.ToString()) > 0)
                        {
                            double cantidad = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value.ToString());
                            double precio = Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value.ToString());
                            double descuento = (Convert.ToDouble(dataGridView1.CurrentRow.Cells[4].Value.ToString()) / 100) * precio;
                            precio = precio - descuento;
                            double total = cantidad * precio;
                            Math.Round(total, 2);
                            dataGridView1.Rows[i].Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                           
                        }
                        else
                        {
                            double cantidad = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value.ToString());
                            double precio = Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value.ToString());
                            double total = cantidad * precio;
                            Math.Round(total, 2);
                            dataGridView1.Rows[i].Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                           
                        }
                    }
                }
            }
        }
        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
          if(dataGridView1.CurrentCell.ColumnIndex <= 1)
            {
                dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystroke;
            } else
            {
                dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            }
           // dataGridView1.BeginEdit(true);
        }

        // CURSOR EN DATA GRID VIEW PARA LUEGO DE ESCANEAR
        private void localizaCursor()
        {
            int columna = dataGridView1.CurrentCell.ColumnIndex;
            int fila = dataGridView1.CurrentCell.RowIndex;

           
           
                if ((dataGridView1.RowCount == 1) || (dataGridView1.RowCount == 0))
                {
                    dataGridView1.CurrentCell = dataGridView1[2, fila];
                    dataGridView1.Focus();
                } else
                {
                    if (fila + 1 != dataGridView1.RowCount)
                    {
                        dataGridView1.CurrentCell = dataGridView1[2, fila + 1];
                        dataGridView1.Focus();
                    }
                }
            
           
        }
            
        // EVENTO QUE CONTROLA LOS ENTERS EN EL DATAGRIDVIEW
        void text_KeyUp(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = false;
            if (e.KeyCode == Keys.Enter)
            {
                int columna = dataGridView1.CurrentCell.ColumnIndex;
                int fila = dataGridView1.CurrentCell.RowIndex;
                if (columna < 6)
                {
                    if ((dataGridView1.RowCount == 1))
                    {

                        dataGridView1.CurrentCell = dataGridView1[columna + 1, fila];
                        // dataGridView1.Focus();

                    }
                    else
                    {
                        if (dataGridView1.CurrentCell.ColumnIndex == 1)
                        {
                            if (dataGridView1.RowCount == fila)
                            {
                                dataGridView1.CurrentCell = dataGridView1[columna + 1, fila];
                                //dataGridView1.Focus();
                            }
                            else if (fila == 0 && dataGridView1.RowCount > fila)
                            {
                                dataGridView1.CurrentCell = dataGridView1[columna + 1, fila];
                            }
                            else
                            {
                                dataGridView1.CurrentCell = dataGridView1[columna + 1, fila - 1];
                            }
                        } else if(columna == 3)
                        {
                            
                            if (dataGridView1.RowCount == fila)
                            {
                                dataGridView1.CurrentCell = dataGridView1[columna + 1, fila];
                                //dataGridView1.Focus();
                            }
                            else if (dataGridView1.CurrentRow.Index == 0 && dataGridView1.RowCount > fila)
                            {
                                dataGridView1.CurrentCell = dataGridView1[columna + 1, fila];
                            }
                            else
                            {
                                dataGridView1.CurrentCell = dataGridView1[columna + 1, fila];
                            }
                        }
                        else if (columna == 2)
                        {

                            if (dataGridView1.RowCount == fila)
                            {
                                dataGridView1.CurrentCell = dataGridView1[columna + 1, fila];
                                //dataGridView1.Focus();
                            }
                            else if (fila == 0 && dataGridView1.RowCount > fila)
                            {
                                dataGridView1.CurrentCell = dataGridView1[columna + 1, fila];
                            }
                            else
                            {
                                dataGridView1.CurrentCell = dataGridView1[columna + 1, fila];
                            }
                        }
                    }
                }
            }
        }
        private void dgr_KeyPress_NumericTester(object sender, KeyPressEventArgs e)
        {

            if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))
            {
                if (dataGridView1.CurrentCell.ColumnIndex > 1)
                {
                    e.Handled = false;
                }
            }
            else
            {
               
                if (dataGridView1.CurrentCell.ColumnIndex > 1)
                {
                    if ((e.KeyChar == 's') | (e.KeyChar == 'S'))
                    {
                        textBox11.Text = "";
                        textBox11.Focus();
                    }
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }

        }
        // BUSCAR DESDE EL APARTADO DE CODIGO EN EL DGV
        private void dgr_KeyPress_SearchObject(object sender, KeyPressEventArgs e)
        {

            if (dataGridView1.CurrentCell.ColumnIndex == 0)
            {
                if ((e.KeyChar == 'B') || (e.KeyChar == 'b'))
                {
                    e.Handled = false;
                    e.KeyChar = (char)13;
                    var Tempforma = new frm_buscaEdicion(nombreSucursal, rolUs);
                    if (Tempforma.ShowDialog() == DialogResult.OK)
                    {
                        textBox11.Text = Tempforma.CurrentMercaderia.getMercaderia;
                        var forma = new frm_getProducto(Tempforma.CurrentMercaderia.getMercaderia, Tempforma.CurrentSucursal.getSucursal, rolUs);
                        if (forma.ShowDialog() == DialogResult.OK)
                        {
                            //AGREGA AL DETALLE DE LA FACTURA
                            getRepetitivo(forma.currentProducto.getStock, forma.currentProducto.getProducto, forma.currentProducto.getDescripcion, forma.currentProducto.getPrice, forma.currentProducto.currentSucursal, forma.currentProducto.getCpp, forma.currentProducto.getPC);
                            localizaCursor();
                            //dataGridView1.Rows.Add(forma.currentProducto.getProducto, forma.currentProducto.getDescripcion, 1, forma.currentProducto.getPrice, 0, 0, forma.currentProducto.currentSucursal, forma.currentProducto.getStock);
                        }
                    }
                }
            }
        }
        // COLOCA PRECIOS DE EL PRODUCTO 
        private void dgr_KeyPress_preciosObject(object sender, KeyPressEventArgs e)
        {

            if (dataGridView1.CurrentCell.ColumnIndex == 3)
            {
                if (dataGridView1.CurrentRow.Cells[6].Value != null && dataGridView1.CurrentRow.Cells[6].Value != null)
                {
                    if ((e.KeyChar == 'p') || (e.KeyChar == 'P'))
                    {
                        e.Handled = false;
                        e.KeyChar = (char)13;                     
                        var forma = new frm_preciosMercaderia(dataGridView1.CurrentRow.Cells[7].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(), rolUs, true);
                        if (forma.ShowDialog() == DialogResult.OK)
                        {
                            dataGridView1.CurrentRow.Cells[3].Value = forma.currentPrecio.getPrice;         
                        }
                    }
                    else if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))
                    {
                        if (dataGridView1.CurrentCell.ColumnIndex > 1)
                        {
                            e.Handled = false;
                        }
                    }
                    else
                    {
                        if (dataGridView1.CurrentCell.ColumnIndex > 1)
                        {
                            e.Handled = true;
                        }
                        else
                        {
                            e.Handled = false;
                        }
                    }
                }
            }
        }
      
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
          
            if (dataGridView1.CurrentCell.ColumnIndex > 1)
            {
               if (dataGridView1.CurrentCell.ColumnIndex == 3)
                {
                  
                    e.Control.KeyPress += new KeyPressEventHandler(dgr_KeyPress_preciosObject);
                   
                }
                else
                    e.Control.KeyPress += new KeyPressEventHandler(dgr_KeyPress_NumericTester);
            }
            else if (dataGridView1.CurrentCell.ColumnIndex == 0)
            {

                e.Control.KeyPress += new KeyPressEventHandler(dgr_KeyPress_SearchObject);
            }

           DataGridViewTextBoxEditingControl dText = (DataGridViewTextBoxEditingControl)e.Control;
            dText.KeyUp -= new KeyEventHandler(text_KeyUp);
                dText.KeyUp += new KeyEventHandler(text_KeyUp);
         
        }
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex > 1)
            {
                if (dataGridView1.CurrentRow.Cells[e.ColumnIndex].Value != null)
                {
                    if(dataGridView1.CurrentCell.ColumnIndex == 2)
                    {
                        if (Convert.ToDouble(dataGridView1.CurrentRow.Cells[4].Value.ToString()) > 0)
                        {
                            double cantidad = Convert.ToDouble(dataGridView1.CurrentRow.Cells[e.ColumnIndex].Value.ToString());
                            double precio = Convert.ToDouble(dataGridView1.CurrentRow.Cells[3].Value.ToString());
                            double descuento = (Convert.ToDouble(dataGridView1.CurrentRow.Cells[4].Value.ToString())/100) * precio;
                            precio = precio - descuento;
                            double total = cantidad * precio;
                            Math.Round(total, 2);
                            dataGridView1.CurrentRow.Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                            totalFactura();
                        }
                        else
                        {
                            double cantidad = Convert.ToDouble(dataGridView1.CurrentRow.Cells[e.ColumnIndex].Value.ToString());
                            double precio = Convert.ToDouble(dataGridView1.CurrentRow.Cells[3].Value.ToString());
                            double total = cantidad * precio;
                            Math.Round(total, 2);
                            dataGridView1.CurrentRow.Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                            totalFactura();
                        }

                    } else if(dataGridView1.CurrentCell.ColumnIndex == 3)
                    {
                        if (Convert.ToDouble(dataGridView1.CurrentRow.Cells[3].Value.ToString()) < Convert.ToDouble(dataGridView1.CurrentRow.Cells[10].Value.ToString()))
                        {
                            MessageBox.Show("EL PRECIO INGRESADO ES MENOR AL PRECIO COSTO", "PRECIO PRODUCTOS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            var forma = new frm_preciosMercaderia(dataGridView1.CurrentRow.Cells[7].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(), rolUs, true);
                            if (forma.ShowDialog() == DialogResult.OK)
                            {
                                dataGridView1.CurrentRow.Cells[3].Value = forma.currentPrecio.getPrice;
                               // dataGridView1.CurrentCell = dataGridView1[4, e.RowIndex];
                            }
                        }
                        else
                        {

                            if (Convert.ToDouble(dataGridView1.CurrentRow.Cells[4].Value.ToString()) > 0)
                            {
                                double cantidad = Convert.ToDouble(dataGridView1.CurrentRow.Cells[2].Value.ToString());
                                double precio = Convert.ToDouble(dataGridView1.CurrentRow.Cells[e.ColumnIndex].Value.ToString());
                                double descuento = (Convert.ToDouble(dataGridView1.CurrentRow.Cells[4].Value.ToString()) / 100) * precio;
                                precio = precio - descuento;
                                double total = cantidad * precio;
                                Math.Round(total, 2);
                                dataGridView1.CurrentRow.Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                                totalFactura();
                            }
                            else
                            {
                                double cantidad = Convert.ToDouble(dataGridView1.CurrentRow.Cells[2].Value.ToString());
                                double precio = Convert.ToDouble(dataGridView1.CurrentRow.Cells[e.ColumnIndex].Value.ToString());
                                double total = cantidad * precio;
                                Math.Round(total, 2);
                                dataGridView1.CurrentRow.Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                                totalFactura();
                            }
                        }
                        
                    }
                    else if (dataGridView1.CurrentCell.ColumnIndex == 4)
                    {
                        double cantidad = Convert.ToDouble(dataGridView1.CurrentRow.Cells[2].Value.ToString());
                        double precio = Convert.ToDouble(dataGridView1.CurrentRow.Cells[3].Value.ToString());
                        double descuento = (Convert.ToDouble(dataGridView1.CurrentRow.Cells[e.ColumnIndex].Value.ToString()) / 100) * precio;
                        precio = precio - descuento;
                        double total = cantidad * precio;               
                        dataGridView1.CurrentRow.Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                        totalFactura();
                    }
                }
                else
                {
                    if (e.ColumnIndex > 1)
                        dataGridView1.CurrentCell.Value = "0";
                }
            }
        }

        private void dataGridView1_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
           
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
           
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (textBox2.Text != "")
                {
                    textBox1.Focus();
                }
                else
                {
                    button3.PerformClick();
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var forma = new frm_obtieneCotizacion(nombreSucursal);
            if (forma.ShowDialog() == DialogResult.OK)
            {
                cotizacion = true;
                existente = false;
                monto_factura = "";
                totalFactura();
                iniciaDefault();
               // comboBox5.Enabled = true;
                CargaExistente(forma.currentFactura.codigoFactura);
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            var forma = new frm_editaVenta(nombreSucursal);
            if (forma.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = "";
                monto_factura = "";
                existente = true;
                cotizacion = false;
                dataGridView1.Rows.Clear();
                totalFactura();
                iniciaDefault();
                CargaExistente(forma.currentFactura.codigoFactura);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var forma = new frm_duplicaFactura();
            if (forma.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = "";
                existente = false;
                cotizacion = false;
                monto_factura = "";
                dataGridView1.Rows.Clear();
                totalFactura();
                iniciaDefault();
                label9.Text = getCode();
               //comboBox5.Enabled = true;
                duplicaFactura(forma.currentFactura.codigoFactura);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (textBox1.Text != "")
                {
                    textBox3.Focus();
                }
              
            }
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (textBox3.Text != "")
                {
                    comboBox1.Focus();
                }

            }
        }
        // CRA CODIGOS DE TIPO ENTERO
        public string createCuenta(int longitud)
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
        // CREA UN ID FACTURA DE MANERA ALETAORIO TIPO STRING
        public string createCode(int longitud)
        {
            string caracteres = "ABCDEF1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < longitud--)
            {
                res.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            return res.ToString();
        }
        // FUNCION QUE VJERIFICA SI LA CUENTA YA FUE CREADA
        private bool codigoCuenta(string code)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CUENTA_POR_COBRAR FROM CUENTA_POR_COBRAR_CLIENTE WHERE ID_CUENTA_POR_COBRAR =  {0};", code);
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
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "FACTURACION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
            return true;
        }
        // FUNCION QUE VERIFICA SI EL ID DE FACTURA YA FUE GENERADO 
        private bool codigoFactura(string code)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_FACTURA FROM FACTURA WHERE ID_FACTURA =  '{0}';", code);
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
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "FACTURACION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
            return true;
        }
        // FUNCION QUE GENERA CODIGO A APRTIR DE QUE NO EXISTA EN EL CODIGO DE LA DB
        private string getCode()
        {
            bool codex = true;
            while (codex)
            {
                string temp = createCode(7);
                if (codigoFactura(temp))
                {
                    codex = false;
                    return temp;
                }
            }
            return null;
        }
        // FUNCION QUE GENERA CODIGO DE CUENTA POR COBRAR
        private string getCuenta()
        {
            bool codex = true;
            while (codex)
            {
                string temp = createCuenta(7);
                if (codigoCuenta(temp))
                {
                    codex = false;
                    return temp;
                }
            }
            return null;
        }
        // CLASE INTERNA QUE PERMITE EL RETORNO DE VALORES BUSCADOS EN LOS PRODUCTOS
        internal class Producto
        {
            public string getProducto { get; set; }
            public string getStock { get; set; }
            public string currentSucursal { get; set; }
            public string getDescripcion { get; set; }
            public string getPrice { get; set; }
            public string getCpp { get; set; }
            public string getPC { get; set; }
        }
        // CLASE INTERNA PARA PODER EDITAR PRECIOS
        internal class preciosProductos
        { 
            public string getPrice { get; set; }
        }
        // AUMENTA LA CANTIDAD DE LA PRESENTACION SI ES LA MISMA
        private void getRepetitivo(string idStock, string producto, string descripcion, string precio, string sucursal, string cpp, string precioCosto)
        {
            bool state = true;
            if (dataGridView1.RowCount > 0)
            {            
                    for (int i = 0; i < dataGridView1.RowCount && state; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[7].Value != null)
                        {
                            if (dataGridView1.Rows[i].Cells[7].Value.ToString() == idStock)
                            {
                                state = false;
                                dataGridView1.Rows[i].Cells[2].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value) + 1;
                               dataGridView1.CurrentCell = dataGridView1[1, dataGridView1.RowCount - 1];

                        }
                        }
                    }
                    if (state)
                    {
                        dataGridView1.Rows.Add(producto, descripcion, 1, string.Format("{0:###,###,###,##0.00##}", precio), 0, 0, sucursal, idStock,cpp, null,precioCosto);
                        dataGridView1.CurrentCell = dataGridView1[1, dataGridView1.RowCount - 1];
                }
               
            } else
            {
                dataGridView1.Rows.Add(producto, descripcion, 1, string.Format("{0:###,###,###,##0.00##}", precio), 0, 0, sucursal, idStock,cpp,null,precioCosto);
                dataGridView1.CurrentCell = dataGridView1[1, dataGridView1.RowCount - 1];
            }
        }

        private void textBox11_Enter(object sender, EventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                dataGridView1.CurrentCell = dataGridView1[1, dataGridView1.RowCount-1];
            }
        }
        // FUNCION QUE OBTIEN EL TOTAL DE LA FACTURA EN TIEMPO REAL
        private void totalFactura()
        {
            if(dataGridView1.RowCount > 0)
            {
                subtotal = 0;
                for(int i = 0; i<dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells[5].Value != null)
                    {
                        subtotal =  subtotal + Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value.ToString());
                    }
                }
                subtotal =  Math.Round(subtotal, 2, MidpointRounding.ToEven);
                label14.Text = string.Format("{0:###,###,###,##0.00##}", subtotal);
                if(textBox8.Text != "" && textBox8.Text != "0")
                {
                    descuento = subtotal * (Convert.ToDouble(textBox8.Text)/100);
                    total = subtotal - descuento;
                    Math.Round(descuento, 2);
                    Math.Round(subtotal, 2);
                    Math.Round(total, 2);
                    total = Math.Round(total, 2, MidpointRounding.ToEven);
                    label13.Text = string.Format("{0:###,###,###,##0.00##}",total);
                } else
                {
                    total = subtotal;
                    Math.Round(subtotal, 2);
                    Math.Round(total, 2);
                    subtotal = Math.Round(total, 2, MidpointRounding.ToEven);
                    total = Math.Round(total, 2, MidpointRounding.ToEven);
                    label13.Text = string.Format("{0:###,###,###,##0.00##}", subtotal);
                }
            } else
            {
                total = 0;
                subtotal = 0;
                descuento = 0;
                label14.Text = "";
                label13.Text = "";
                textBox8.Text = "";
            }
        }
        private void actualizaTotal(string codigo, double monto)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CAJA SET TOTAL_FACTURADO_CAJA = TOTAL_FACTURADO_CAJA + {0} WHERE ID_CAJA = '{1}';", monto, codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //.Show("SE GENERO LA CUENTA CORRECTAMENTE");
                    /*var forma = new frm_creditoActualizado();
                    forma.ShowDialog();*/
                }
                else
                {
                    MessageBox.Show("IMPOSIBLE ACTUALIZAR MONTO DE CAJA!", "CAJA", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                string sql = string.Format("INSERT INTO DET_CAJA VALUES (NULL,{0},'{1}','VENTA',TRUE)", codigoCaja,label9.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("DETALLE DE CAJA GUARDADO GURDADA");
                    conexion.Close();
                    if (comboBox5.Text == "EFECTIVO")
                    {
                        actualizaTotal(codigoCaja, total);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private bool existeFactura(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_FACTURA FROM FACTURA WHERE ID_FACTURA = '{0}' AND ESTADO_TUPLA = TRUE;", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("La factura existe");
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
        
        private bool guardaFactura(string descripcion)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL NUEVA_FACTURA ('{0}','{1}','{2}',{3},'{4}',{5},{6},{7},'{8}','{9}');", label9.Text.Trim(),comboBox1.Text,textBox6.Text,idSucursal,textBox2.Text.Trim(),subtotal,descuento,total,comboBox5.Text,descripcion);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("FACTURA GURDADA");
                    guardaCaja();
                    conexion.Close();
                    return true;
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return false;
        }

        private void guardaFactura()
        {
            if (textBox2.Text != "" && textBox1.Text != "" && textBox3.Text != "" && dataGridView1.RowCount > 0)
            {
                if (!existeFactura(label9.Text.Trim()))
                {
                    if (guardaFactura("VENTA AL " + comboBox5.Text))
                    {
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            double castingTotal = Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value.ToString());
                            OdbcConnection conexion = ASG_DB.connectionResult();
                            try
                            {
                                string sql = string.Format("INSERT INTO DET_FACTURA VALUES (NULL,'{0}','{1}','{2}',(SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{3}' LIMIT 1),'{4}',{5},{6},{7},{8},TRUE);", label9.Text, dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[7].Value.ToString(), dataGridView1.Rows[i].Cells[6].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString(), dataGridView1.Rows[i].Cells[2].Value.ToString(), dataGridView1.Rows[i].Cells[4].Value.ToString(), Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value.ToString()), castingTotal);
                                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                                if (cmd.ExecuteNonQuery() == 1)
                                {
                                    double cantidad = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(dataGridView1.Rows[i].Cells[8].Value.ToString());
                                    algoritmoRecalculo(dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[6].Value.ToString(), cantidad);
                                   // MessageBox.Show("DETALLE GUARDADO CON EXITO");
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
                else
                {
                    if (existente)
                    {
                        revisaFactura();
                        //MessageBox.Show("la factua existe");
                        if (updateFactura("VENTA AL " + comboBox5.Text))
                        {
                            for (int i = 0; i < dataGridView1.RowCount; i++)
                            {
                                double castingTotal = Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value.ToString());
                                OdbcConnection conexion = ASG_DB.connectionResult();
                                if (dataGridView1.Rows[i].Cells[9].Value != null)
                                {
                                    
                                    try
                                    {
                                        string sql = string.Format("SELECT * FROM VISTA_DETALLE_FACTURA WHERE ID_DET_FACTURA = {0};", dataGridView1.Rows[i].Cells[9].Value.ToString());
                                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                                        OdbcDataReader reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            //MessageBox.Show("SE ENCONTRO EL ID STOCK YA EXISTE");
                                            double cantidades = reader.GetDouble(3);
                                            string codigoStock = reader.GetString(8);
                                           
      
                                            double cantidadPresentacion = reader.GetDouble(9);
                                            double cantidadExistnete = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(dataGridView1.Rows[i].Cells[8].Value.ToString());
                                            double cantidadAplicada = cantidades * cantidadPresentacion;
                                            string codigoDetalle = ""+reader.GetString(10);
                                            //MessageBox.Show("Existe en DB" + cantidadExistnete);
                                            //MessageBox.Show("Existe en Memoria" + cantidadAplicada);
                                            if (cantidadExistnete > cantidadAplicada)
                                            {
                                                //MessageBox.Show("actualiza detalle que aplicada es mayor a DB");
                                                double cantidad = cantidadExistnete - cantidadAplicada;
                                                sql = string.Format("CALL ACTUALIZA_STOCK ({0},'{1}','{2}',{3},{4},{5},{6});", codigoDetalle, label9.Text, dataGridView1.Rows[i].Cells[1].Value.ToString(), dataGridView1.Rows[i].Cells[2].Value.ToString(), dataGridView1.Rows[i].Cells[4].Value.ToString(), dataGridView1.Rows[i].Cells[3].Value.ToString(), dataGridView1.Rows[i].Cells[5].Value.ToString());
                                                cmd = new OdbcCommand(sql, conexion);
                                                if (cmd.ExecuteNonQuery() == 1)
                                                {

                                                    // MessageBox.Show("DIFERENCIAL" + cantidad);
                                                    algoritmoRecalculoMenor(dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[6].Value.ToString(), cantidad);
                                                    // MessageBox.Show("DETALLE GUARDADO CON EXITO ACTUALIZADO");
                                                }
                                                else
                                                {
                                                    //MessageBox.Show("imposible guardar detalle");
                                                }
                                            }
                                            else if (cantidadExistnete < cantidadAplicada)
                                            {
                                                //MessageBox.Show("actualiza detalle que menor devolviendo al stok es emnor a DB");
                                                double cantidad = cantidadAplicada - cantidadExistnete;
                                                sql = string.Format("CALL ACTUALIZA_STOCK ({0},'{1}','{2}',{3},{4},{5},{6});", dataGridView1.Rows[i].Cells[9].Value.ToString(), label9.Text, dataGridView1.Rows[i].Cells[1].Value.ToString(), dataGridView1.Rows[i].Cells[2].Value.ToString(), dataGridView1.Rows[i].Cells[4].Value.ToString(), dataGridView1.Rows[i].Cells[3].Value.ToString(), castingTotal);
                                                cmd = new OdbcCommand(sql, conexion);
                                                if (cmd.ExecuteNonQuery() == 1)
                                                {

                                                    //MessageBox.Show("DIFERENCIAL" + cantidad);
                                                    algoritmoRecalculoMenor(dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[6].Value.ToString(), cantidad);
                                                    //MessageBox.Show("DETALLE GUARDADO CON EXITO ACTUALIZADO");
                                                }
                                                else
                                                {
                                                   // MessageBox.Show("imposible guardar detalle");
                                                }
                                            }
                                            conexion.Close();
                                        }
                                        else
                                        {
                                           
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.ToString());
                                    }
                                    conexion.Close();

                                } else
                                {
                                    
                                   //MessageBox.Show("gaurdar nuevo detalle");// GUARDAR EL STOCK EN LA FACTURA
                                    string sql = string.Format("INSERT INTO DET_FACTURA VALUES (NULL,'{0}','{1}','{2}',(SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{3}' LIMIT 1),'{4}',{5},{6},{7},{8},TRUE);", label9.Text, dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[7].Value.ToString(), dataGridView1.Rows[i].Cells[6].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString(), dataGridView1.Rows[i].Cells[2].Value.ToString(), dataGridView1.Rows[i].Cells[4].Value.ToString(), Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value.ToString()), castingTotal);
                                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                                    if (cmd.ExecuteNonQuery() == 1)
                                    {
                                        double cantidad = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(dataGridView1.Rows[i].Cells[8].Value.ToString());
                                        algoritmoRecalculo(dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[6].Value.ToString(), cantidad);
                                        //MessageBox.Show("DETALLE GUARDADO CON EXITO");
                                    }
                                }
                            }
                        }
                        
                    }
                    else if (cotizacion)
                    {
                        //MessageBox.Show("es un cotizacion");
                        if (updateCotizacion("VENTA AL " + comboBox5.Text))
                        {
                            for (int i = 0; i < dataGridView1.RowCount; i++)
                            {
                                double castingTotal = Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value.ToString());
                                OdbcConnection conexion = ASG_DB.connectionResult();
                                if (dataGridView1.Rows[i].Cells[9].Value != null)
                                {

                                    try
                                    {
                                        string sql = string.Format("SELECT * FROM VISTA_DETALLE_FACTURA WHERE ID_DET_FACTURA = {0};", dataGridView1.Rows[i].Cells[9].Value.ToString());
                                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                                        OdbcDataReader reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            //MessageBox.Show("SE ENCONTRO EL ID STOCK YA EXISTE");
                                            double cantidades = reader.GetDouble(3);
                                            string codigoStock = reader.GetString(8);
                                            double cantidadPresentacion = reader.GetDouble(9);
                                            double cantidadExistnete = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(dataGridView1.Rows[i].Cells[8].Value.ToString());
                                            double cantidadAplicada = cantidades * cantidadPresentacion;
                                            //MessageBox.Show("Existe en DB" + cantidadExistnete);
                                           // MessageBox.Show("Existe en Memoria" + cantidadAplicada);
                                            if (cantidadExistnete > cantidadAplicada)
                                            {
                                                //MessageBox.Show("actualiza detalle que aplicada es mayor a DB");
                                                double cantidad = cantidadExistnete - cantidadAplicada;
                                                sql = string.Format("CALL ACTUALIZA_STOCK ({0},'{1}','{2}',{3},{4},{5},{6});", codigoStock, label9.Text, dataGridView1.Rows[i].Cells[1].Value.ToString(), dataGridView1.Rows[i].Cells[2].Value.ToString(), dataGridView1.Rows[i].Cells[4].Value.ToString(), dataGridView1.Rows[i].Cells[3].Value.ToString(), castingTotal);
                                                cmd = new OdbcCommand(sql, conexion);
                                                if (cmd.ExecuteNonQuery() == 1)
                                                {

                                                    //MessageBox.Show("DIFERENCIAL" + cantidad);
                                                    algoritmoRecalculo(dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[6].Value.ToString(), cantidad);
                                                    //MessageBox.Show("DETALLE GUARDADO CON EXITO ACTUALIZADO");
                                                }
                                                else
                                                {
                                                    //MessageBox.Show("imposible guardar detalle");
                                                }
                                            }
                                            else if (cantidadExistnete < cantidadAplicada)
                                            {
                                                //MessageBox.Show("actualiza detalle que menor devolviendo al stok es emnor a DB");
                                                double cantidad = cantidadAplicada - cantidadExistnete;
                                                sql = string.Format("CALL ACTUALIZA_STOCK ({0},'{1}','{2}',{3},{4},{5},{6});", codigoStock, label9.Text, dataGridView1.Rows[i].Cells[1].Value.ToString(), dataGridView1.Rows[i].Cells[2].Value.ToString(), dataGridView1.Rows[i].Cells[4].Value.ToString(), dataGridView1.Rows[i].Cells[3].Value.ToString(), castingTotal);
                                                cmd = new OdbcCommand(sql, conexion);
                                                if (cmd.ExecuteNonQuery() == 1)
                                                {

                                                    //MessageBox.Show("DIFERENCIAL" + cantidad);
                                                    algoritmoRecalculoMenor(dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[6].Value.ToString(), cantidad);
                                                    //MessageBox.Show("DETALLE GUARDADO CON EXITO ACTUALIZADO");
                                                }
                                                else
                                                {
                                                   // MessageBox.Show("imposible guardar detalle");
                                                }
                                            }

                                            conexion.Close();
                                        }
                                        else
                                        {

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

                                    //MessageBox.Show("gaurdar nuevo detalle");// GUARDAR EL STOCK EN LA FACTURA
                                    string sql = string.Format("INSERT INTO DET_FACTURA VALUES (NULL,'{0}','{1}','{2}',(SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{3}' LIMIT 1),'{4}',{5},{6},{7},{8},TRUE);", label9.Text, dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[7].Value.ToString(), dataGridView1.Rows[i].Cells[6].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString(), dataGridView1.Rows[i].Cells[2].Value.ToString(), dataGridView1.Rows[i].Cells[4].Value.ToString(), Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value.ToString()), castingTotal);
                                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                                    if (cmd.ExecuteNonQuery() == 1)
                                    {
                                        double cantidad = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(dataGridView1.Rows[i].Cells[8].Value.ToString());
                                        algoritmoRecalculo(dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[6].Value.ToString(), cantidad);
                                        //MessageBox.Show("DETALLE GUARDADO CON EXITO");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void reiniciaFormulario()
        {
            textBox2.Text = "";
            textBox11.Text = "";
            dataGridView1.Rows.Clear();
            totalFactura();
            iniciaDefault();
            cotizacion = false;
            existente = false;
            label9.Text = getCode();
            monto_factura = "";
            checkBox1.Checked = false;
        }
        private bool updateCotizacion(string descripcion)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL ACTUALIZA_COTIZACION ('{0}','{1}','{2}',{3},'{4}',{5},{6},{7},'{8}','{9}');", label9.Text.Trim(), comboBox1.Text, textBox6.Text, idSucursal, textBox2.Text.Trim(), subtotal, descuento, total, comboBox5.Text, descripcion);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("FACTURA GURDADA DE EXISTENCIA");
                    guardaCaja();
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
        private bool obtienSumCaja(double numero)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CAJA SET TOTAL_FACTURADO_CAJA = TOTAL_FACTURADO_CAJA - {0} WHERE ID_CAJA = {1};", numero, codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                   // MessageBox.Show("CJA ACTUALIZADA");
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
        private bool obtienResCaja(double numero)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CAJA SET TOTAL_FACTURADO_CAJA = TOTAL_FACTURADO_CAJA + {0} WHERE ID_CAJA = {1};", numero, codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
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
        private bool updateFactura(string descripcion)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL ACTUALIZA_FACTURA ('{0}','{1}','{2}',{3},'{4}',{5},{6},{7},'{8}','{9}');", label9.Text.Trim(), comboBox1.Text, textBox6.Text, idSucursal, textBox2.Text.Trim(), subtotal, descuento, total, comboBox5.Text, descripcion);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("FACTURA GURDADA DE EXISTENCIA");                 
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
       
        private bool obtieneConfig()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT IMPRESION_DEFECTO FROM CONFIGURACION_SISTEMA WHERE IDCONFIGURACION = 111 AND CAJA = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    bool temp = reader.GetBoolean(0);
                    return temp;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return false;
        }
        private void contruyeImpresion()
        {
            DataSetVenta ds = new DataSetVenta();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                ds.Tables[0].Rows.Add
                    (new object[]
                    {
                                    dataGridView1[2,i].Value.ToString(),
                                    dataGridView1[1,i].Value.ToString(),
                                    dataGridView1[3,i].Value.ToString(),
                                    dataGridView1[5,i].Value.ToString()
                    });
            }
            frm_reporteVenta frm = new frm_reporteVenta();
            CrystalReport4 cr = new CrystalReport4();
            cr.SetDataSource(ds);
            TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["numero"];
            text.Text = label9.Text;
            TextObject textn = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["nombre"];
            textn.Text = textBox1.Text;
            TextObject textd = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["direccion"];
            textd.Text = textBox3.Text;
            TextObject textc = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["codigo"];
            textc.Text = textBox2.Text;
            TextObject textv = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["vendedor"];
            textv.Text = comboBox1.Text;
            TextObject textt = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["terminos"];
            textt.Text = comboBox5.Text;
            TextObject texttotal = (TextObject)cr.ReportDefinition.Sections["Section4"].ReportObjects["total"];
            texttotal.Text = label13.Text;
            numberToString c = new numberToString();
            TextObject texttotaleter = (TextObject)cr.ReportDefinition.Sections["Section5"].ReportObjects["letters"];
            texttotaleter.Text = c.enletras(label13.Text) + " Q.";
            frm.crystalReportViewer1.ReportSource = cr;
            if (poseeVista())
            {
                frm.ShowDialog();
                
            }
            else
            {
                imprimePropiedades(cr);
            }

        }
        private void imprimePropiedades(CrystalReport4 rpt)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT IMPRESORA, COPIAS FROM CONFIGURACION_SISTEMA WHERE IDCONFIGURACION = 111;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    rpt.PrintOptions.PrinterName = reader.GetString(0);
                    rpt.PrintToPrinter(reader.GetInt32(1), false, 0, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private bool poseeVista()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT VISTA FROM CONFIGURACION_SISTEMA WHERE IDCONFIGURACION = 111;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    bool temp = reader.GetBoolean(0);
                    return temp;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return false;
        }
        private bool poseeCuenta()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_FACTURA FROM CUENTA_POR_COBRAR_CLIENTE WHERE ID_FACTURA = '{0}' AND ESTADO_TUPLA = TRUE;", label9.Text);
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
        private double totalAbonadoCliente()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT TOTAL_PAGADO FROM CUENTA_POR_COBRAR_CLIENTE WHERE ID_FACTURA = '{0}';", label9.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                       double temp = reader.GetDouble(0);
                        //MessageBox.Show("" + temp);
                      
                        return Convert.ToDouble(temp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return 0;
        }
        private void actualizaCreditosClientes()
        {
            if (totalAbonadoCliente() > 0)
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    double temp = Convert.ToDouble(monto_factura) - totalAbonadoCliente();
                   // MessageBox.Show(""+temp);
                    string sql = string.Format("UPDATE CLIENTE SET CREDITO_DISPONIBLE = CREDITO_DISPONIBLE + {0}  WHERE ID_CLIENTE = (SELECT ID_CLIENTE FROM FACTURA WHERE ID_FACTURA = '{1}');", temp,label9.Text );
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        //MessageBox.Show("SE ACTUALIZO CORRECTAMENTE EL CREDITO");  
                    }
                    else
                    {
                        MessageBox.Show("IMPOSIBE ACTUALIZAR CREDITO DEL CLIENTE!", "CUENTA POR COBRAR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                conexion.Close();
                
            } else
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    string sql = string.Format("UPDATE CLIENTE SET CREDITO_DISPONIBLE = CREDITO_DISPONIBLE + {0}  WHERE  ID_CLIENTE = (SELECT ID_CLIENTE FROM FACTURA WHERE ID_FACTURA = '{1}');", monto_factura, label9.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                       // MessageBox.Show("SE ACTUALIZO CORRECTAMENTE EL CREDITO");
                    }
                    else
                    {
                        MessageBox.Show("IMPOSIBE ACTUALIZAR CREDITO DEL CLIENTE!", "CUENTA POR COBRAR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                conexion.Close();
            }
        }
        private bool anulaCuenta()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CUENTA_POR_COBRAR_CLIENTE SET ESTADO_CUENTA = FALSE WHERE ID_FACTURA = '{0}';", label9.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    actualizaCreditosClientes();
                    return true;
                }
                else
                {
                    MessageBox.Show("IMPOSIBE ANULAR CUENTA POR COBRAR!", "CUENTA POR COBRAR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return false;
        }
        
        
        private void revisaFactura()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT TIPO_FACTURA FROM FACTURA WHERE ID_FACTURA = '{0}';", label9.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                   string temp = reader.GetString(0);    
                    if (temp != comboBox5.Text.Trim())
                    {
                        if(comboBox5.Text == "CREDITO")
                        {
                        } else if (comboBox5.Text == "EFECTIVO")
                        {
                            // REVISAR CUENTA DEL CLIENTE PARA AJUSTAR LOS CREDITOS
                           // MessageBox.Show("SE CAMBIO DE EFFECTIVO A CREDITO");
                            if (poseeCuenta())
                            {                               
                                DialogResult result;
                                result = MessageBox.Show("LA FACTURA ACTUALMENTE POSEE UNA CUENTA POR COBRAR, LA CUENTA POR COBRAR SE ELIMINARA!", "FACTURA EXISTENTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                if (result == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (anulaCuenta())
                                    {
                                        // CREAR ALGORITMO PARA CALCULAR REPISIOCN DE CREDITOS
                                        MessageBox.Show("LOS DATOS DE LA CUENTA POR COBRAR Y CREDITO DEL CLIENTE SE HAN ACTUALIZADO!", "FACTURA EXISTENTE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        ajustaCaja();
                                    }
                                }
                            } else
                            {
                                ajustaCaja();
                            }
                        }
                    } else
                    {
                        // NO CAMBIO EL TIPO DE FACTURA
                        //MessageBox.Show("SE MANTIENE tipo factura");
                        ajustaCaja();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private void ajustaCaja()
        {
            if (cajaFactura())
            {

                if (Convert.ToDouble(monto_factura) > Convert.ToDouble(label13.Text))
                {

                    if (!obtienSumCaja(Convert.ToDouble(monto_factura) - Convert.ToDouble(label13.Text)))
                    {
                        MessageBox.Show("LA FACTURA SELECCIONADA PERTENECE A OTRA CAJA, LOS CAMBIOS SE EFECTUARAN EN LA CAJA QUE CORRESPONDE A LA FACTURA, LA CAJA ACTUAL NO SERA AFECTADA!", "FACTURA EXISTENTE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (!obtienResCaja(Convert.ToDouble(label13.Text) - Convert.ToDouble(monto_factura)))
                    {
                        MessageBox.Show("LA FACTURA SELECCIONADA PERTENECE A OTRA CAJA, LOS CAMBIOS SE EFECTUARAN EN LA CAJA QUE CORRESPONDE A LA FACTURA, LA CAJA ACTUAL NO SERA AFECTADA!", "FACTURA EXISTENTE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("LA FACTURA SELECCIONADA PERTENECE A OTRA CAJA, LOS CAMBIOS SE EFECTUARAN EN LA CAJA QUE CORRESPONDE A LA FACTURA, LA CAJA ACTUAL NO SERA AFECTADA!", "FACTURA EXISTENTE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private bool cajaFactura()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_FACTURA FROM DET_CAJA WHERE ID_FACTURA = '{0}' AND ID_CAJA = {1};", label9.Text, codigoCaja);
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
        private void button2_Click_2(object sender, EventArgs e)
        {
            
            if(dataGridView1.RowCount > 0)
            {
                if (comboBox5.Text == "EFECTIVO")
                {
                    guardaFactura();
                    adressUser.ingresaBitacora(idSucursal, usuario, "FACTURA EFFECTIVO", label9.Text);
                    var forma = new frm_totalFactura(subtotal, descuento, total);
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        if (checkBox1.Checked == true)
                        {
                            contruyeImpresion();
                        }
                        else
                        {
                            if (obtieneConfig())
                            {
                                DialogResult result;
                                result = MessageBox.Show("¿DESEA IMPRIMIR LA FACTURA COMO UN ENVIO?", "NUEVA VENTA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == System.Windows.Forms.DialogResult.Yes)
                                {
                                    contruyeImpresion();
                                }
                            }
                        }

                        reiniciaFormulario();
                    }
                }
                else
                {
                    if (total > Convert.ToDouble(textBox9.Text))
                    {
                        DialogResult result;
                        result = MessageBox.Show("¿EL TOTAL DE LA FACTURA ES MAYOR AL LIMITE DE CREDITO, DESEA CONTINUAR? " + "\n" + " EL CREDITO DEL CLIENTE SE ACTUALIZARA", "FACTURACION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            guardaFactura();
                            adressUser.ingresaBitacora(idSucursal, usuario, "FACTURA CREDITO", label9.Text);
                            actualizaCreditos();
                            creanuevaCuenta();
                            adressUser.ingresaBitacora(idSucursal, usuario, "CUENTA POR COBRAR", textBox2.Text);
                            actualizaDisponibleCredito();
                            var forma = new frm_ventaCredito(subtotal, descuento, total, textBox2.Text.Trim(), textBox1.Text.Trim(), numeroCuenta, codigoCaja);
                            if (forma.ShowDialog() == DialogResult.OK)
                            {
                                if (checkBox1.Checked == true)
                                {
                                    contruyeImpresion();
                                }
                                else
                                {
                                    if (obtieneConfig())
                                    {
                                        DialogResult resultado;
                                        resultado = MessageBox.Show("¿DESEA IMPRIMIR LA FACTURA COMO UN ENVIO?", "NUEVA VENTA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                        if (result == System.Windows.Forms.DialogResult.Yes)
                                        {
                                            contruyeImpresion();
                                        }
                                    }
                                }
                                reiniciaFormulario();
                            }
                        }
                    }
                    else
                    {
                        guardaFactura();

                        adressUser.ingresaBitacora(idSucursal, usuario, "FACTURA CREDITO", label9.Text);
                        creanuevaCuenta();

                        adressUser.ingresaBitacora(idSucursal, usuario, "CUENTA POR COBRAR", textBox2.Text);
                        actualizaDisponibleCredito();

                        var forma = new frm_ventaCredito(subtotal, descuento, total, textBox2.Text.Trim(), textBox1.Text.Trim(), numeroCuenta, codigoCaja);
                        if (forma.ShowDialog() == DialogResult.OK)
                        {
                            if (checkBox1.Checked == true)
                            {
                                contruyeImpresion();
                            }
                            else
                            {
                                if (obtieneConfig())
                                {
                                    DialogResult result;
                                    result = MessageBox.Show("¿DESEA IMPRIMIR LA FACTURA COMO UN ENVIO?", "NUEVA VENTA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (result == System.Windows.Forms.DialogResult.Yes)
                                    {
                                        contruyeImpresion();
                                    }
                                }
                            }

                            reiniciaFormulario();
                        }
                    }
                }
            } else
            {
                MessageBox.Show("NO HA INGRESADO NINGUN PROUDCTO!", "VENTA RAPIDA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void textBox8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button2.PerformClick();
            }
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            
            if (textBox8.Text != "" && textBox8.Text != "0")
            {
                descuento = subtotal * (Convert.ToDouble(textBox8.Text) / 100);
                Math.Round(descuento, 2);
                Math.Round(subtotal, 2);
                Math.Round(total, 2);
                total = subtotal - descuento;
                label13.Text = string.Format("{0:###,###,###,##0.00##}", total);
            } else
            {
                descuento = 0;
            }            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" || textBox1.Text != "" || textBox3.Text != "" || dataGridView1.RowCount > 0)
            {
                textBox2.Text = "";
                existente = false;
                cotizacion = false;
                dataGridView1.Rows.Clear();
                totalFactura();
                iniciaDefault();
                comboBox5.Enabled = true;

            }
        }
        
         // COMMIT EN THE DB PARA GUARDAR TODOS LOS DATOS YA EXACTOS!
        private void guardaDatos()
        {
            if(dataGridView2.RowCount > 0)
            {
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    try
                    {
                        string sql = string.Format("UPDATE MERCADERIA_PRESENTACIONES SET TOTAL_PRESENTACIONES = {0} WHERE ID_STOCK = '{1}'  AND ESTADO_TUPLA = TRUE AND ID_SUCURSAL = {2};", dataGridView2.Rows[i].Cells[2].Value.ToString(), dataGridView2.Rows[i].Cells[0].Value.ToString(), dataGridView2.Rows[i].Cells[3].Value.ToString());
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            //MessageBox.Show("ACTUALIZADO");
                        } else
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
         // ACTUALIZA EL DISPONIBLE GENERAL
        private bool actualizaDisponible(double restante , string codigo, string sucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE MERCADERIA SET TOTAL_UNIDADES = {0} WHERE ID_MERCADERIA = '{1}' AND ID_SUCURSAL = {2};",restante, codigo, sucursal);
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
        private void inventarioDexMenor(double disponible, double cantidad, string tipo, string codigo, string sucursal)
        {
            if (dataGridView2.RowCount > 0)
            {
                double restante = disponible + cantidad;
                if (restante > 0)
                {
                    for (int i = 0; i < dataGridView2.RowCount; i++)
                    {
                        if (tipo == "1")
                        {
                            dataGridView2.Rows[i].Cells[2].Value = Math.Truncate((restante / Convert.ToDouble(dataGridView2.Rows[i].Cells[1].Value.ToString())));
                        }
                        else
                        {
                            dataGridView2.Rows[i].Cells[2].Value = Math.Round(restante / Convert.ToDouble(dataGridView2.Rows[i].Cells[1].Value.ToString()), 2);
                        }
                    }
                    //dataGridView2.Visible = true;
                    if (actualizaDisponible(restante, codigo, sucursal))
                    {
                        guardaDatos();
                        dataGridView2.Rows.Clear();
                    }
                    //MessageBox.Show("data convertida exactamente");
                }
                else
                {
                    for (int i = 0; i < dataGridView2.RowCount; i++)
                    {
                        dataGridView2.Rows[i].Cells[2].Value = 0;
                    }
                    // dataGridView2.Visible = true;
                    if (actualizaDisponible(restante, codigo, sucursal))
                    {
                        guardaDatos();
                        dataGridView2.Rows.Clear();
                    }
                    // MessageBox.Show("data convertida exactamente");
                }
            }
        }
        // RECALCULA LAS PRESENTACIONES DISPONIBLE SDESPUES DE LA VENTA
        private void inventarioDex(double disponible, double cantidad, string tipo, string codigo, string sucursal)
        {
           if(dataGridView2.RowCount > 0)
            {
                double restante = disponible - cantidad;
                if(restante > 0)
                {
                    for(int i = 0; i<dataGridView2.RowCount; i++)
                    {
                        if (tipo == "1")
                        {
                            dataGridView2.Rows[i].Cells[2].Value = Math.Truncate( (restante / Convert.ToDouble(dataGridView2.Rows[i].Cells[1].Value.ToString()) )  );
                        } else
                        {
                            dataGridView2.Rows[i].Cells[2].Value = Math.Round(restante / Convert.ToDouble(dataGridView2.Rows[i].Cells[1].Value.ToString()),2);
                        }
                    }
                    //dataGridView2.Visible = true;
                    if (actualizaDisponible(restante, codigo, sucursal))
                    {
                        guardaDatos();
                        dataGridView2.Rows.Clear();
                    }
                    //MessageBox.Show("data convertida exactamente");
                } else
                {
                    for(int i =0; i<dataGridView2.RowCount; i++)
                    {
                        dataGridView2.Rows[i].Cells[2].Value = 0;
                    }
                   // dataGridView2.Visible = true;
                    if (actualizaDisponible(restante, codigo, sucursal))
                    {
                        guardaDatos();
                        dataGridView2.Rows.Clear();
                    }
                   // MessageBox.Show("data convertida exactamente");
                }
            }
        }
        private void recalculaStockMenor(string clasificacion, double disponible, string sucursal, string producto, double cantidad)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_STOCK, CANTIDAD_POR_PRESENTACION, TOTAL_PRESENTACIONES, ID_SUCURSAL FROM MERCADERIA_PRESENTACIONES WHERE ID_MERCADERIA = '{0}' AND ID_SUCURSAL = {1} AND ESTADO_TUPLA = TRUE;", producto, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView2.Rows.Clear();
                    dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                    }
                    inventarioDexMenor(disponible, cantidad, clasificacion, producto, sucursal);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        // CARGA LOS DATOS EN UN DATAGRIDVIEW TEMPORAL PARA HACER LOS CALCULOS
        private void recalculaStock(string clasificacion, double disponible,  string sucursal, string producto, double cantidad )
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_STOCK, CANTIDAD_POR_PRESENTACION, TOTAL_PRESENTACIONES, ID_SUCURSAL FROM MERCADERIA_PRESENTACIONES WHERE ID_MERCADERIA = '{0}' AND ID_SUCURSAL = {1} AND ESTADO_TUPLA = TRUE;", producto, sucursal );
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView2.Rows.Clear();                  
                    dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                    }
                    inventarioDex(disponible,cantidad,clasificacion,producto,sucursal);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void algoritmoRecalculoMenor(string codigo, string sucursal, double cantidad)
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
                    recalculaStockMenor(reader.GetString(0), reader.GetDouble(1), reader.GetString(2), codigo, cantidad);
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
        // BUSCA LOS DATOS D ELA MERCADERIA A ACTUALIZAR
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
                } else
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
        // FUNCION QUE ACTUALIZA CREDITO DE LOS CIENTES SI LA VENTA ES MAYOR 
        private void actualizaCreditos()
        {
            double limiteCredito;
            
                double limite = Convert.ToDouble(textBox9.Text);
                double disponible = Convert.ToDouble(textBox9.Text);
                double importe = total - limite;
                limiteCredito = Convert.ToDouble(textBox5.Text);
                limiteCredito = limiteCredito + importe;                 
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CLIENTE SET LIMITE_CREDITO = {0}, CREDITO_DISPONIBLE = {1} WHERE ID_CLIENTE  = '{2}'", limiteCredito, 0,textBox2.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("EL CREDITO DEL CLINETE SE ACTUALIZO CON EXITO!", "NUEVA VENTA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("IMPOSIBLE ACTUALIZAR EL LIMITE DE CREDITO DEL CLIENTE!", "VENTA - LIMITE CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
         /// RESTA EL DISPNIBE DEL CREDITO 
        private void actualizaDisponibleCredito()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                double disponible = 0;
                if (total > Convert.ToDouble(textBox9.Text))
                {
                   disponible = 0;
                } else
                {
                    disponible = Convert.ToDouble(textBox9.Text) - Convert.ToDouble(label13.Text);
                }
                string sql = string.Format("UPDATE CLIENTE SET CREDITO_DISPONIBLE  = {0} WHERE ID_CLIENTE  = '{1}';", disponible, textBox2.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if( (cmd.ExecuteNonQuery() == 1 )|| (cmd.ExecuteNonQuery() == 0))
                {
                    //MessageBox.Show("SE resto corrctamente el credito");
                }
                else
                {
                    MessageBox.Show("IMPOSIBLE ACTUALIZAR CREDITO DEL CLIENTE!", "NUEVA VENTA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }

        // FUNCION QUE CREA UNA NUEVA CUNETA POR COBRAR DEL CLIENTE
        private void creanuevaCuenta()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                numeroCuenta = getCuenta();
               // MessageBox.Show(numeroCuenta);
                string sql = string.Format("INSERT INTO CUENTA_POR_COBRAR_CLIENTE VALUES ({0},'{1}','{2}',{3},'{4}',TRUE,TRUE);", numeroCuenta,textBox2.Text.Trim(),label9.Text, 0,textBox6.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                   // MessageBox.Show("SE CREO LA CUENTA");
                }
                else
                {
                    MessageBox.Show("NO SE PUDO CREAR CUENTA POR COBRAR!", "NUEVA VENTA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void timerActions()
        {
            frm_done frm = new frm_done();
            frm.ShowDialog();
            timer1.Interval = 1500;
            timer1.Enabled = true;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }
        private void CargaExistente(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CLIENTE, ID_FACTURA, TIPO_FACTURA FROM FACTURA WHERE ID_FACTURA  = '{0}';", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                   // MessageBox.Show("FACTURA DUPLICADA");
                    textBox2.Text = reader.GetString(0);
                    label9.Text = reader.GetString(1);
                    comboBox5.Text = reader.GetString(2);
                    duplicaDetalle(label9.Text);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        //  DUPLICA FACTURA EXISTENTE 
        private void duplicaFactura(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CLIENTE, ID_FACTURA FROM FACTURA WHERE ID_FACTURA  = '{0}';", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //MessageBox.Show("FACTURA DUPLICADA");
                    textBox2.Text = reader.GetString(0);
                    duplicaDetalle(reader.GetString(1));
                }

            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
       // DUPLICA DETALLE DE FACTURA
       private void duplicaDetalle(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_DETALLE_FACTURA WHERE ID_FACTURA  = '{0}';", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5),reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                    }
                    totalFactura();
                    if (existente)
                    {
                        monto_factura = label13.Text;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        // EDITA FACTURA EXISTENTE
        private void editaFactura(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CLIENTE, ID_FACTURA FROM FACTURA WHERE ID_FACTURA  = '{0}';", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    MessageBox.Show("FACTURA DUPLICADA");
                    textBox2.Text = reader.GetString(0);
                    duplicaDetalle(reader.GetString(1));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        // DUPLICA DETALLE DE FACTURA EXISTENTE
        private void editaDetalle(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_DETALLE_FACTURA WHERE ID_FACTURA  = '{0}';", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9),reader.GetString(10));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10));
                    }
                    totalFactura();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (dataGridView1.CurrentCell.ColumnIndex == 3)
                {
                    var forma = new frm_preciosMercaderia(dataGridView1.CurrentRow.Cells[7].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(), rolUs, true);
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        dataGridView1.CurrentRow.Cells[3].Value = forma.currentPrecio.getPrice;
                        dataGridView1.CurrentCell = dataGridView1[4, e.RowIndex];
                    }
                }
                
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var forma = new frm_buscaCliente(nameUs, rolUs, usuario, idSucursal, privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                textBox2.Text = forma.currentCliente.getCliente;
            }
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    mymenuS.Show(dataGridView1, new Point(e.X, e.Y));
                    mymenuS.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_ItemclickedS);
                    mymenuS.Enabled = true;
                    flags = false;
                }
            }
        }
    }
}
  
