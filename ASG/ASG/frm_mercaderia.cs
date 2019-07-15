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
using BarcodeLib;
using System.Drawing.Imaging;
using CrystalDecisions.CrystalReports.Engine;

namespace ASG
{
    public partial class frm_mercaderia : Form
    {
        ContextMenuStrip mymenu = new ContextMenuStrip();
        ContextMenuStrip mymenus = new ContextMenuStrip();
        ContextMenuStrip mymenuS = new ContextMenuStrip();
        ContextMenuStrip mymenuSV = new ContextMenuStrip();
        ContextMenuStrip mymenuTS = new ContextMenuStrip();
        string nameUs;
        string rolUs;
        string usuario;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        bool flag = true;
        bool flagT = true;
        string idSucursal;
        string tipoMercaderia;
        string tipoTraslado;
        double disponible;
        bool flags = false;
        string nombreSucursal;
        bool preferenciaVenta = false;
        double total = 0;
        double subtotal = 0;
        double descuento = 0;
        bool flagsc = false;
        bool flagst = false;
        bool tipoCotizacion;
        bool existente = false;
        bool[] privilegios;
        public frm_mercaderia(string nameUser, string rolUser, string user, string sucursal, string nombre, bool[] privilegio)
        {
            InitializeComponent();
            usuario = user;
            label19.Text = label19.Text + nameUser;
            nameUs = nameUser;
            rolUs = rolUser;
            nombreSucursal = nombre;
            idSucursal = sucursal;
            this.privilegios = privilegio;
            stripMenu();
            stripMenuS();
            stripMenuSC();
            stripMenuST();
            stripMenuT();
            cargaSucursales();
            comboBox2.Text = nombreSucursal;
            comboBox9.Text = nombreSucursal;
            cargaMercaderia();
            cargaCotizaciones();
            comboBox10.SelectedIndex = 0;
            cargaProveedor();
            disponible = 0;
            cargaUsuarios();
            cargaTraslados();
            
            if (rolUs != "ADMINISTRADOR" && rolUs != "ROOT" && rolUs != "DATA BASE ADMIN")
            {
                dataGridView1.Columns[2].Visible = false;
                dataGridView1.Columns[5].Visible = false;
                tabControl1.TabPages.Remove(this.tabPage2);             
                mymenu.Items[1].Enabled = false;
                mymenu.Items[3].Enabled = false;
                mymenu.Items[4].Enabled = false;
                mymenuSV.Items[2].Enabled = false;
                mymenuTS.Items[1].Enabled = false;
                mymenu.Items[6].Enabled = false;
                button29.Enabled = false;
            }
            if(privilegios[16] != true)
            {
                tabControl1.TabPages.Remove(this.tabPage4);
                tabControl1.TabPages.Remove(this.tabPage7);
            }
            if(privilegios[17] != true)
            {
                tabControl1.TabPages.Remove(this.tabPage3);
            }
        }
       
        private void stripMenu()
        {
            mymenu.Items.Add("Ocultar Fila");
            mymenu.Items[0].Name = "ColHidden";
            mymenu.Items.Add("Editar Mercaderia");
            mymenu.Items[1].Name = "ColEdit";
            mymenu.Items.Add("Eliminar Mercaderia");
            mymenu.Items[2].Name = "ColDelete";
            mymenu.Items[2].Enabled = false;
            mymenu.Items.Add("Activar / Desactivar Mercaderia");
            mymenu.Items[3].Name = "ColDesac";
            mymenu.Items.Add("Ver Formas de Venta");
            mymenu.Items[4].Name = "ColForma";
            mymenu.Items.Add("Vista General del Producto");
            mymenu.Items[5].Name = "ColGeneral";
            mymenu.Items.Add("Ver Kardex");
            mymenu.Items[6].Name = "ColKardex";        

        }
        private void stripMenuSC()
        {
            mymenuS.Items.Add("Eliminar Producto de la Cotizacion");
            mymenuS.Items[0].Name = "ColHidden";
        }
        private void stripMenuST()
        {
            mymenuSV.Items.Add("Ocultar Fila");
            mymenuSV.Items[0].Name = "ColHidden";
            mymenuSV.Items.Add("Editar Cotización");
            mymenuSV.Items[1].Name = "ColEdit";
            mymenuSV.Items.Add("Eliminar Cotización");
            mymenuSV.Items[2].Name = "ColDelete";
            mymenuSV.Items.Add("Ver Detalle de Cotización");
            mymenuSV.Items[3].Name = "ColView";
            mymenuSV.Items.Add("Imprimir Cotizacion");
            mymenuSV.Items[4].Name = "ColImp";
        }
        private void stripMenuT()
        {
            mymenuTS.Items.Add("Ocultar Fila");
            mymenuTS.Items[0].Name = "ColHidden";
            mymenuTS.Items.Add("Eliminar Traslado");
            mymenuTS.Items[1].Name = "ColEdit";
            mymenuTS.Items.Add("Ver Detalle de Traslado");
            mymenuTS.Items[2].Name = "ColDelete";         
        }
        private void my_menu_ItemclickedST(object sender, ToolStripItemClickedEventArgs e)
        {
            if (flagst == false)
            {
                if (e.ClickedItem.Name == "ColHidden")
                {
                    if (dataGridView7.RowCount > 0)
                    {
                        flagst = false;
                        dataGridView7.Rows[this.dataGridView7.CurrentRow.Index].Visible = false;
                        mymenuSV.Visible = false;
                        
                    } 
                }
                else if (e.ClickedItem.Name == "ColEdit")
                {
                    flagst = false;
                    mymenuSV.Visible = false;
                }
                else if (e.ClickedItem.Name == "ColDelete")
                {
                    flagst = false;
                    button29.PerformClick();
                    mymenuSV.Visible = false;
                }
                else if (e.ClickedItem.Name == "ColView")
                {
                    flagst = false;
                    mymenuSV.Visible = false;
                }

            }
        }
        private bool actualizaDetalleProveedor(string detalle)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE DETALLE_COTIZACION SET ESTADO_TUPLA = FALSE WHERE ID_DET_FACTURA = {0};", detalle);
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
            return false;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return false;
        }
        private void my_menu_ItemclickedSC(object sender, ToolStripItemClickedEventArgs e)
        {
            if (flagsc == false)
            {
                if (e.ClickedItem.Name == "ColHidden")
                {
                    if (dataGridView6.RowCount > 0)
                    {
                        if (comboBox7.Text == "CLIENTES")
                        {
                            mymenuS.Visible = false;
                            DialogResult result;
                            result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR EL PRODUCTO DE LA COTIZACION?", "COTIZACIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == System.Windows.Forms.DialogResult.Yes)
                            {
                                if (!existente)
                                {
                                    dataGridView6.Rows.RemoveAt(dataGridView6.CurrentRow.Index);
                                    totalFactura();
                                }
                                else
                                {
                                    if (actualizaDetalle(dataGridView6.CurrentRow.Cells[9].Value.ToString()))
                                    {
                                        dataGridView6.Rows.RemoveAt(dataGridView6.CurrentRow.Index);
                                        totalFactura();
                                        timerActions();
                                    }
                                    else
                                    {
                                        MessageBox.Show("EL PRODUCTO NO PUDO SER ELIMINADO DE LA FACTURA", "COTIZACIONES EXISTENTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }

                        }
                        else if (comboBox7.Text == "PROVEEDORES")
                        {
                            mymenuS.Visible = false;
                            DialogResult result;
                            result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR EL PRODUCTO DE LA COTIZACION?", "COTIZACIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == System.Windows.Forms.DialogResult.Yes)
                            {
                                if (!existente)
                                {
                                    dataGridView6.Rows.RemoveAt(dataGridView6.CurrentRow.Index);
                                    totalFactura();
                                }
                                else
                                {
                                    if (actualizaDetalleProveedor(dataGridView6.CurrentRow.Cells[9].Value.ToString()))
                                    {

                                        dataGridView6.Rows.RemoveAt(dataGridView6.CurrentRow.Index);
                                        totalFactura();
                                        timerActions();
                                    }
                                    else
                                    {
                                        MessageBox.Show("EL PRODUCTO NO PUDO SER ELIMINADO DE LA COTIZACION", "COTIZACIONES EXISTENTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }

                        }
                        flagsc = true;
                    }
                }

            }
        }
        private void stripMenuS()
        {
            mymenus.Items.Add("Ocultar Fila");
            mymenus.Items[0].Name = "ColHidden";
            mymenus.Items.Add("Eliminar Tipo de Venta");
            mymenus.Items[1].Name = "ColEdit";
            mymenus.Items.Add("Editar Sub-Código");
            mymenus.Items[2].Name = "ColSub";
            mymenus.Items.Add("Agregar Sub-Código");
            mymenus.Items[3].Name = "ColSubA";
        }

        internal class edicionCodigo
        {
            public string getNewCode { get; set; }
        }
        // MOUSE CLICK EVENTO DE LOS CODIGOS EN EDITAR MERCADERIA
        private void my_menu_ItemclickedS(object sender, ToolStripItemClickedEventArgs e)
        {
            if (flags == false)
            {
                if (e.ClickedItem.Name == "ColHidden")
                {
                    mymenus.Visible = false;
                    dataGridView4.Rows[this.dataGridView4.CurrentRow.Index].Visible = false;
                    flags = true;
                }
                else if (e.ClickedItem.Name == "ColEdit")
                {
                    mymenus.Visible = false;
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR LA PRESENTACION DEL PRODUCTO?", "EDICION DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        deleteDatagridE();
                    }
                    flags = true;
                }
                else if (e.ClickedItem.Name == "ColSub")
                {
                    mymenus.Visible = false;
                    if (existeSubCodigo())
                    {
                        var forma = new frm_codigoStock(textBox2.Text.Trim(), dataGridView4.CurrentRow.Cells[0].Value.ToString(), textBox1.Text.Trim(), true);
                        if (forma.ShowDialog() == DialogResult.OK)
                        {
                            getdataEdit(textBox3.Text.Trim(), textBox23.Text.Trim());
                        }
                    } else
                    {
                        var forma = new frm_codigoStock(textBox2.Text.Trim(), dataGridView4.CurrentRow.Cells[0].Value.ToString(), textBox1.Text.Trim(), false);
                        if (forma.ShowDialog() == DialogResult.OK)
                        {
                            dataGridView4.CurrentRow.Cells[0].Value = forma.currentCodigo.getNewCode;
                        }

                    }
                    flags = true;
                }
                else if (e.ClickedItem.Name == "ColSubA")
                {  //AGREGAR SUBCOIGOS AL LAS PRESENTACIONES //
                    mymenus.Visible = false;
                    if (existeSubCodigo())
                    {
                        if (textBox2.Text.Trim() != "" && textBox2.BackColor != Color.OrangeRed)
                        {
                            var forma = new frm_alterCodes(dataGridView4.CurrentRow.Cells[0].Value.ToString(), dataGridView4.CurrentRow.Cells[1].Value.ToString(), 1);
                            forma.ShowDialog();
                        }
                    } else
                    {
                        presentacionesIngreso(textBox2.Text, dataGridView4.CurrentRow.Cells[0].Value.ToString(), dataGridView4.CurrentRow.Cells[1].Value.ToString(), dataGridView4.CurrentRow.Cells[2].Value.ToString(), dataGridView4.CurrentRow.Cells[3].Value.ToString(), dataGridView4.CurrentRow.Cells[4].Value.ToString(), dataGridView4.CurrentRow.Cells[5].Value.ToString(), dataGridView4.CurrentRow.Cells[6].Value.ToString(), dataGridView4.CurrentRow.Cells[7].Value.ToString(), dataGridView4.CurrentRow.Cells[8].Value.ToString(), dataGridView4.CurrentRow.Cells[9].Value.ToString(), dataGridView4.CurrentRow.Cells[10].Value.ToString(), dataGridView4.CurrentRow.Cells[11].Value.ToString(), dataGridView4.CurrentRow.Cells[12].Value.ToString(), dataGridView4.CurrentRow.Cells[13].Value.ToString(), dataGridView4.CurrentRow.Cells[14].Value.ToString(), dataGridView4.CurrentRow.Cells[15].Value.ToString(), textBox23.Text.Trim());
                        if (textBox2.Text.Trim() != "" && textBox2.BackColor != Color.OrangeRed)
                        {
                            var forma = new frm_alterCodes(dataGridView4.CurrentRow.Cells[0].Value.ToString(), dataGridView4.CurrentRow.Cells[1].Value.ToString(), 1);
                            forma.ShowDialog();
                        }
                    }
                    flags = true;
                }


            }
        }
        private bool existeSubCodigo()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = sql = string.Format("SELECT ID_STOCK FROM MERCADERIA_PRESENTACIONES WHERE ID_STOCK = '{0}' AND ID_SUCURSAL = (SELECT NOMBRE_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{1}');", dataGridView4.CurrentRow.Cells[0].Value.ToString(), textBox23.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return true;
                }
                
            }
            catch
          (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return false;
        }

        private void deleteDatagridE()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = sql = string.Format("SELECT ID_STOCK FROM MERCADERIA_PRESENTACIONES WHERE ID_STOCK = '{0}';", dataGridView4.CurrentRow.Cells[0].Value.ToString());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // MessageBox.Show("se encontro en stock");
                    conexion.Close();
                    conexion = ASG_DB.connectionResult();
                    sql = sql = string.Format("UPDATE MERCADERIA_PRESENTACIONES SET ESTADO_TUPLA = FALSE, ID_STOCK = '{0}' WHERE ID_STOCK = '{1}';", getCode(),dataGridView4.CurrentRow.Cells[0].Value.ToString());
                    cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        dataGridView4.Rows.RemoveAt(dataGridView4.CurrentRow.Index);
                    }
                }
                else
                {
                    dataGridView4.Rows.RemoveAt(dataGridView4.CurrentRow.Index);
                }
            }
            catch
          (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
           
                if ((dataGridView4.RowCount > 0) || (dataGridView3.RowCount > 0) || (dataGridView5.RowCount > 0) || (dataGridView6.RowCount > 0))
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox3_MouseHover(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.Red;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.Transparent;
        }
        private void generaBarcode(string barString)
        {
            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
            Codigo.IncludeLabel = true;
            pictureBox1.Image = Codigo.Encode(BarcodeLib.TYPE.CODE128, barString);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        }
        private void frm_mercaderia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if ((dataGridView4.RowCount > 0) || (dataGridView3.RowCount > 0) || (dataGridView5.RowCount > 0) || (dataGridView6.RowCount > 0))
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            else if (e.KeyData == Keys.F5)
            {
                if (tabControl1.SelectedTab == tabPage5)
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
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.B))
            {
                if (tabControl1.SelectedTab == tabPage5)
                {
                    button22.PerformClick();
                }
                else if (tabControl1.SelectedTab == tabPage4)
                {
                    button10.PerformClick();
                }
                else if (tabControl1.SelectedTab == tabPage3)
                {
                    button5.PerformClick();
                }
                else if (tabControl1.SelectedTab == tabPage2)
                {
                    button3.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                if (tabControl1.SelectedTab == tabPage5)
                {
                    button24.PerformClick();
                }
                else if (tabControl1.SelectedTab == tabPage4)
                {
                    button14.PerformClick();
                }
                else if (tabControl1.SelectedTab == tabPage2)
                {
                    button13.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.A))
            {
                if (tabControl1.SelectedTab == tabPage5)
                {
                    button21.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.E))
            {
                if (tabControl1.SelectedTab == tabPage5)
                {
                    button35.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.D))
            {
                if (tabControl1.SelectedTab == tabPage5)
                {
                    button34.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.L))
            {
                if (tabControl1.SelectedTab == tabPage4)
                {
                    button36.PerformClick();
                } else if (tabControl1.SelectedTab == tabPage2)
                {
                    button33.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.T))
            {
                if (tabControl1.SelectedTab == tabPage2)
                {
                    if ((textBox2.Text != "") && (textBox1.Text != ""))
                    {
                        dataGridView4.Rows.Add(getCode(), "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        //styleDV(this.dataGridView1);
                        if (dataGridView4.RowCount == 1)
                        {
                            this.dataGridView4.CurrentCell = this.dataGridView1[1, dataGridView4.CurrentRow.Index];

                        }
                        else
                        {
                            this.dataGridView4.CurrentCell = this.dataGridView4[1, dataGridView4.RowCount - 1];
                        }
                        dataGridView4.Focus();
                    }
                    else
                    {
                        MessageBox.Show("NO SE HA CREADO AUN EL PRODUCTO!", "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                } else if (tabControl1.SelectedTab == tabPage4)
                {
                    button15.PerformClick();
                }
            }
        }

        private void frm_mercaderia_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_mercaderia_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }
        private void frm_mercaderia_MouseUp(object sender, MouseEventArgs e)
        { 
            Dragging = false;
        }
        
        private void timerActions()
        {
            frm_done frm = new frm_done();
            frm.ShowDialog();
            timer1.Interval = 1500;
            timer1.Enabled = true;
        }
        private void activaMercaderia()
        {
            if (this.dataGridView1.CurrentRow.Cells[0].Value.ToString() != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ACTIVAR EL PRODUCTO?" + "\n" + "  " + this.dataGridView1.CurrentRow.Cells[0].Value.ToString() + " - " + this.dataGridView1.CurrentRow.Cells[1].Value.ToString(), "GESTION MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    try
                    {
                        string sql = string.Format("UPDATE MERCADERIA SET ACTIVO = TRUE WHERE ID_MERCADERIA = '{0}';", this.dataGridView1.CurrentRow.Cells[0].Value.ToString());
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            adressUser.ingresaBitacora(idSucursal, usuario, "ACTIVACION PRODUCTO", this.dataGridView1.CurrentRow.Cells[0].Value.ToString());
                            timerActions();
                            cargaClasificacion(comboBox1.Text);
                        }
                        else
                        {
                           // MessageBox.Show("NO SE ENCONTRO LA MERCADERIA!", "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                    button17.PerformClick();
                }
            }
            
        }
        private void desactivaMercaderia()
        {
            if (this.dataGridView1.CurrentRow.Cells[0].Value.ToString() != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA DESACTIVAR EL PRODUCTO?" + "\n" + "  " + this.dataGridView1.CurrentRow.Cells[0].Value.ToString() + " - " + this.dataGridView1.CurrentRow.Cells[1].Value.ToString(), "GESTION MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    try
                    {
                        string sql = string.Format("update mercaderia set activo = false where id_mercaderia = '{0}'; ", this.dataGridView1.CurrentRow.Cells[0].Value.ToString());
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            adressUser.ingresaBitacora(idSucursal, usuario, "DESACTIVACION PRODUCTO", this.dataGridView1.CurrentRow.Cells[0].Value.ToString());
                            timerActions();
                            cargaClasificacion(comboBox1.Text);
                        }
                        else
                        {
                            //MessageBox.Show("NO SE ENCONTRO LA MERCADERIA!", "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            
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
                    button17.PerformClick();
                }
            }
           
        }
        private void my_menu_ItemclickedE(object sender, ToolStripItemClickedEventArgs e)
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
                    mymenu.Visible = false;
                    tabControl1.SelectedTab = tabPage2;
                    textBox3.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    textBox23.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
                    getdataEdit(textBox3.Text.Trim(), textBox23.Text.Trim());
                    flag = true;
                }
                else if (e.ClickedItem.Name == "ColDelete")
                {
                    flag = true;
                    mymenu.Visible = false;
                    button11.PerformClick();
                }
                else if (e.ClickedItem.Name == "ColDesac")
                {
                    flag = true;
                    mymenu.Visible = false;
                    if (dataGridView1.CurrentRow.Cells[7].Value.ToString() == "ACTIVO")
                    {
                        desactivaMercaderia();
                    }
                    else
                    {
                        activaMercaderia();
                    }
                   
                }
                else if (e.ClickedItem.Name == "ColForma")
                {
                    flag = true;
                    mymenu.Visible = false;
                    if (dataGridView1.RowCount > 0)
                    {       
                        var forma = new frm_tipoVenta(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[8].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(),rolUs, dataGridView1.CurrentRow.Cells[4].Value.ToString());
                        forma.ShowDialog();
                    }     
                }
                else if (e.ClickedItem.Name == "ColKardex")
                {
                    tabControl1.SelectedTab = tabPage3;
                    flag = true;
                    textBox6.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    textBox28.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
                    getKardex(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString());
                }
                else if (e.ClickedItem.Name == "ColGeneral")
                {
                    flag = true;
                    mymenu.Visible = false;
                    var forma = new frm_vistaGeneral(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[8].Value.ToString(),1, rolUs);
                    forma.ShowDialog();
                }
            }
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
                    comboBox2.Items.Clear();
                    comboBox4.Items.Clear();
                    comboBox9.Items.Clear();
                    comboBox12.Items.Clear();
                    comboBox2.Items.Add(reader.GetString(0));
                    comboBox4.Items.Add(reader.GetString(0));
                    comboBox9.Items.Add(reader.GetString(0));
                    comboBox12.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader.GetString(0));
                        comboBox4.Items.Add(reader.GetString(0));
                        comboBox9.Items.Add(reader.GetString(0));
                        comboBox12.Items.Add(reader.GetString(0));
                    }
                   
                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void setDescripcion(int celda)
        {
            for (int i = 0; i <= dataGridView1.RowCount-1; i++)
            {
                if (dataGridView1.Rows[i].Cells[celda].Value.ToString() == "0")
                {
                    dataGridView1.Rows[i].Cells[celda].Value = "INACTIVO";
                    dataGridView1.Rows[i].Cells[celda].Style.ForeColor = Color.White;
                    dataGridView1.Rows[i].Cells[celda].Style.BackColor = Color.OrangeRed;
                }
                else
                {
                    dataGridView1.Rows[i].Cells[celda].Value = "ACTIVO";
                }
            }
        }
        private void setDescripcionTraslado(int celda)
        {
            for (int i = 0; i <= dataGridView8.RowCount - 1; i++)
            {
                if (dataGridView8.Rows[i].Cells[celda].Value.ToString() == "0")
                {
                    dataGridView8.Rows[i].Cells[celda].Value = "NO RECIBIDO";
                    dataGridView8.Rows[i].Cells[celda].Style.ForeColor = Color.White;
                    dataGridView8.Rows[i].Cells[celda].Style.BackColor = Color.OrangeRed;
                }
                else
                {
                    dataGridView8.Rows[i].Cells[celda].Value = "RECIBIDO";
                }
            }
        }
        private void setDisponible(int celda)
        {
            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                if ((dataGridView1.Rows[i].Cells[celda].Value.ToString() == "") | ((dataGridView1.Rows[i].Cells[celda].Value.ToString() == "0")))
                {
                    dataGridView1.Rows[i].Cells[celda].Value = "0";
                    dataGridView1.Rows[i].Cells[celda].Style.ForeColor = Color.White;
                    dataGridView1.Rows[i].Cells[celda].Style.BackColor = Color.FromArgb(207, 136, 65);
                } else if ((Convert.ToDouble(dataGridView1.Rows[i].Cells[celda].Value.ToString()) < 0))
                {
                   
                    dataGridView1.Rows[i].Cells[celda].Style.ForeColor = Color.White;
                    dataGridView1.Rows[i].Cells[celda].Style.BackColor = Color.FromArgb(38, 39, 48);
                }
            }
        }
        private void cargaMercaderia()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C  WHERE NOMBRE_SUCURSAL = '{0}' LIMIT 25;",comboBox2.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                        //styleDV(this.dataGridView1);
                    }
                    setDescripcion(7);
                    setDisponible(3);
                    //label18.Text = string.Format("{0:#,###,###,###,###}", dataGridView1.RowCount);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void button17_Click(object sender, EventArgs e)
        {
            cargaMercaderia();
        }
        private void cargaSegunda(string sucursal)
        {
            try
            {
                dataGridView1.Rows.Clear();
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE NOMBRE_SUCURSAL = '{0}' LIMIT 25;",sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {

                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                        //styleDV(this.dataGridView1);
                    }
                    setDescripcion(7);
                    setDisponible(3);
                    //label18.Text = string.Format("{0:#,###,###,###,###}", dataGridView1.RowCount);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void cargaClasificacion(string clasificacion)
        {
            try
            {
                dataGridView1.Rows.Clear();
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE TIPO_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}' LIMIT 25;",clasificacion,comboBox2.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {

                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                        //styleDV(this.dataGridView1);
                    }
                    setDescripcion(7);
                    setDisponible(3);
                    //label18.Text = string.Format("{0:#,###,###,###,###}", dataGridView1.RowCount);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void cargaTotal (string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format(" SELECT COUNT( distinct ID_MERCADERIA) FROM MERCADERIA WHERE ESTADO_TUPLA = TRUE AND ID_SUCURSAL = (SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{0}' AND ESTADO_TUPLA = TRUE LIMIT 1);",codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {


                    label59.Text = reader.GetString(0);

                }
                else
                {
                    label59.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "ESTADISTICAS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargaTotal(comboBox2.Text);
            cargaSegunda(comboBox2.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.Text == "TODOS")
            {
                cargaMercaderia();
            } else
            {
                cargaClasificacion(comboBox1.Text);
            }
        }

        private void frm_mercaderia_Load(object sender, EventArgs e)
        {
            //comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            dataGridView2.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView4.Columns[0].ReadOnly = true;
            dataGridView5.Columns[0].ReadOnly = true;
            dataGridView4.Columns[1].ReadOnly = false;
            dataGridView4.Columns[2].ReadOnly = false;
            dataGridView4.Columns[3].ReadOnly = false;
            dataGridView4.Columns[4].ReadOnly = false;
            dataGridView4.Columns[5].ReadOnly = false;
            dataGridView4.Columns[6].ReadOnly = false;
            dataGridView4.Columns[7].ReadOnly = false;
            textBox23.ReadOnly = true;
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if ((e.Button == MouseButtons.Right))
                {
                    mymenu.Show(dataGridView1, new Point(e.X, e.Y));
                    mymenu.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_ItemclickedE);
                    mymenu.Enabled = true;
                    flag = false;
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (rolUs == "ADMINISTRADOR")
            {
                if (dataGridView1.RowCount > 0)
                {
                    var forma = new frm_tipoVenta(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[8].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(), rolUs, dataGridView1.CurrentRow.Cells[4].Value.ToString());
                    forma.ShowDialog();
                }
            }
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
        private bool codigostock(string code)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_STOCK FROM MERCADERIA_PRESENTACIONES WHERE ID_STOCK =  '{0}' AND ESTADO_TUPLA = TRUE;", code);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return false;
                } else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
            return false;
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
        
       
        private void getEspecias(int numColum)
        {
            if (numColum == 2)
            {
                if (dataGridView4.CurrentRow.Cells[numColum].Value.ToString() != "0")
                {
                    double cantidad = Convert.ToDouble(textBox5.Text);
                    dataGridView4.CurrentRow.Cells[3].Value = Math.Round(cantidad / Convert.ToDouble(dataGridView4.CurrentRow.Cells[numColum].Value.ToString()),2);
                    dataGridView4.CurrentRow.Cells[5].Value = Convert.ToDouble(dataGridView4.CurrentRow.Cells[numColum].Value.ToString()) * Convert.ToDouble(textBox5.Text);
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if ((textBox2.Text != "") && (textBox1.Text != ""))
            {
                dataGridView4.Rows.Add(getCode(), "", 0, 0, 0, 0, 0, 0, 0, 0, 0,0,0,0,0,0);
                //styleDV(this.dataGridView1);
                if (dataGridView4.RowCount == 1)
                {
                    this.dataGridView4.CurrentCell = this.dataGridView1[1, dataGridView4.CurrentRow.Index];

                }
                else
                {
                    this.dataGridView4.CurrentCell = this.dataGridView4[1, dataGridView4.RowCount - 1];
                }
                dataGridView4.Focus();

            }
            else
            {
                MessageBox.Show("NO SE HA CREADO AUN EL PRODUCTO!", "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentRow.Cells[0].Value != null)
            {
               if(dataGridView1.CurrentRow.Cells[7].Value.ToString() == "ACTIVO")
               {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA DESACTIVAR EL PRODUCTO?" + "\n" + "  " + this.dataGridView1.CurrentRow.Cells[0].Value.ToString() + " - " + this.dataGridView1.CurrentRow.Cells[1].Value.ToString(), "GESTION MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        OdbcConnection conexion = ASG_DB.connectionResult();
                        try
                        {
                            string sql = string.Format("UPDATE MERCADERIA SET ACTIVO = FALSE WHERE ID_MERCADERIA = '{0}';", this.dataGridView1.CurrentRow.Cells[0].Value.ToString());
                            OdbcCommand cmd = new OdbcCommand(sql, conexion);
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                adressUser.ingresaBitacora(idSucursal, usuario, "PRODUCTO DESACTIVADO", this.dataGridView1.CurrentRow.Cells[0].Value.ToString());
                                timerActions();
                                cargaMercaderia();
                            }
                            else
                            {
                                MessageBox.Show("NO SE ENCONTRO LA MERCADERIA!", "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                        button17.PerformClick();
                    }
               } else
                {
                    MessageBox.Show("LA MERCADERIA SE ENCUENTRA INACTIVA!", "MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {           
                var forma = new frm_edicionProducto( nombreSucursal, rolUs);
                if (forma.ShowDialog() == DialogResult.OK)
                {
                    textBox3.Text = forma.CurrentMercaderia.getMercaderia;
                    textBox2.Text = forma.CurrentMercaderia.getMercaderia;
                    textBox23.Text = forma.CurrentSucursal.getSucursal;
                    textBox25.Text = "";
                    textBox26.Text = "";
                
                if (textBox2.Text != "") {
                    getdataEdit(textBox3.Text.Trim(), textBox23.Text.Trim());
                    textBox1.Focus();
                }

                }        
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
                    while (reader.Read())
                    {
                        comboBox5.Items.Add(reader.GetString(0));
                    }
                    comboBox5.SelectedIndex = 0;
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
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_MERCADERIA FROM MERCADERIA WHERE ID_MERCADERIA = '{0}';",textBox2.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (textBox2.Text.Trim() == textBox3.Text.Trim())
                    {
                        textBox2.BackColor = Color.LightGreen;
                        textBox2.ForeColor = Color.Black;
                    }
                    else
                    {
                        textBox2.BackColor = Color.OrangeRed;
                        textBox2.ForeColor = Color.White;
                    }
                }
                else
                {
                    textBox2.BackColor = Color.LightGreen;
                    textBox2.ForeColor = Color.Black;
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }

        private void dataGridView4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView4.CurrentCell.ColumnIndex > 1)

            {
                if (Char.IsNumber(e.KeyChar) || e.KeyChar == (Char)Keys.Back || e.KeyChar == '.')
                    e.Handled = false;
                else
                    e.Handled = true;
            }
        }
        private void dgr_KeyPress_NumericTesterdgv4(object sender, KeyPressEventArgs e)
        {

            if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))
            {
                if (dataGridView4.CurrentCell.ColumnIndex > 1)
                {
                    e.Handled = false;
                }

            }
            else
            {
                if (dataGridView4.CurrentCell.ColumnIndex > 1)
                {
                   
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }

        }
        private void dgr_KeyPress_NumericTesterTraslado(object sender, KeyPressEventArgs e)
        {

            if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))
            {
                if (dataGridView5.CurrentCell.ColumnIndex > 1)
                {
                    e.Handled = false;
                }

            }
            else
            {
                if (dataGridView5.CurrentCell.ColumnIndex > 1)
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }

        }
        private void dataGridView4_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView4.CurrentCell.ColumnIndex > 1)
            {
                e.Control.KeyPress += new KeyPressEventHandler(dgr_KeyPress_NumericTesterdgv4);
            }

        }
        private void getCantidades(int numColum)
        {
            if (textBox4.Text != "")
            {
                if (numColum == 2)
                {
                    if (dataGridView4.CurrentRow.Cells[numColum].Value.ToString() != "0")
                    {
                        double cantidad = Convert.ToDouble(textBox4.Text);
                        dataGridView4.CurrentRow.Cells[3].Value = Math.Truncate(cantidad / Convert.ToDouble(dataGridView4.CurrentRow.Cells[numColum].Value.ToString()));
                        dataGridView4.CurrentRow.Cells[5].Value = Convert.ToDouble(dataGridView4.CurrentRow.Cells[numColum].Value.ToString()) * Convert.ToDouble(textBox5.Text); ;
                    }
                }
            }
        }
        private void dataGridView4_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
          
            
        }

        private void dataGridView4_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value != null)
            {
                if ((e.ColumnIndex == 6) | (e.ColumnIndex == 7) | (e.ColumnIndex == 8) | (e.ColumnIndex == 9) | (e.ColumnIndex == 10))
                {
                    if (dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString() != "0")
                    {
                        if (Convert.ToDouble(dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString()) < Convert.ToDouble(dataGridView4.CurrentRow.Cells[5].Value.ToString()))
                        {
                            MessageBox.Show("EL PRECIO DE VENTA APLICADO ES MENOR AL PRECIO COSTO!", "INGRESO DE MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dataGridView4.CurrentRow.Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(207, 136, 65);
                            dataGridView4.CurrentRow.Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                        }
                        else
                        {
                            dataGridView4.CurrentRow.Cells[e.ColumnIndex].Style.ForeColor = Color.Black;
                            dataGridView4.CurrentRow.Cells[e.ColumnIndex].Style.BackColor = dataGridView4.CurrentRow.DefaultCellStyle.BackColor;
                        }
                        double diferencial = Convert.ToDouble(dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString()) - Convert.ToDouble(dataGridView4.CurrentRow.Cells[5].Value.ToString());
                        double porcentaje = Math.Round(diferencial / Convert.ToDouble(dataGridView4.CurrentRow.Cells[5].Value.ToString()), 4) * 100;
                        if (e.ColumnIndex == 6)
                        {
                            dataGridView4.CurrentRow.Cells[11].Value = porcentaje;
                        }
                        else if (e.ColumnIndex == 7)
                        {
                            dataGridView4.CurrentRow.Cells[12].Value = porcentaje;
                        }
                        else if (e.ColumnIndex == 8)
                        {
                            dataGridView4.CurrentRow.Cells[13].Value = porcentaje;
                        }
                        else if (e.ColumnIndex == 9)
                        {
                            dataGridView4.CurrentRow.Cells[14].Value = porcentaje;
                        }
                        else if (e.ColumnIndex == 10)
                        {
                            dataGridView4.CurrentRow.Cells[15].Value = porcentaje;
                        }
                    }
                }
                else if ((e.ColumnIndex == 11) | (e.ColumnIndex == 12) | (e.ColumnIndex == 13) | (e.ColumnIndex == 14) | (e.ColumnIndex == 15))
                {
                    if ((Convert.ToDouble(dataGridView4.CurrentRow.Cells[5].Value.ToString()) > 0))
                    {
                        double precio = Convert.ToDouble(dataGridView4.CurrentRow.Cells[5].Value.ToString());
                        precio = precio + (precio * (Convert.ToDouble(dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString()) / 100));
                        precio = Math.Round(precio, 4);
                        if (e.ColumnIndex == 11)
                        {
                            dataGridView4.CurrentRow.Cells[6].Value = precio;
                        }
                        else if (e.ColumnIndex == 12)
                        {
                            dataGridView4.CurrentRow.Cells[7].Value = precio;
                        }
                        else if (e.ColumnIndex == 13)
                        {
                            dataGridView4.CurrentRow.Cells[8].Value = precio;
                        }
                        else if (e.ColumnIndex == 14)
                        {
                            dataGridView4.CurrentRow.Cells[9].Value = precio;
                        }
                        else if (e.ColumnIndex == 15)
                        {
                            dataGridView4.CurrentRow.Cells[10].Value = precio;
                        }
                    }
                }
                if ((e.ColumnIndex == 2) && (tipoMercaderia == "1"))
                {
                    if (dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString() != "0")
                    {
                        if (textBox26.Text == "")
                        {
                            double cantidad = Convert.ToDouble(textBox4.Text);
                            dataGridView4.CurrentRow.Cells[3].Value = Math.Truncate(cantidad / Convert.ToDouble(dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString()));
                            dataGridView4.CurrentRow.Cells[5].Value = Convert.ToDouble(dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString()) * Convert.ToDouble(textBox5.Text);
                        }
                        else
                        {
                            double cantidad = Convert.ToDouble(textBox26.Text);
                            dataGridView4.CurrentRow.Cells[3].Value = Math.Truncate(cantidad / Convert.ToDouble(dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString()));
                            dataGridView4.CurrentRow.Cells[5].Value = Convert.ToDouble(dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString()) * Convert.ToDouble(textBox5.Text);
                        }
                    }
                }
                else if ((e.ColumnIndex == 2) && (tipoMercaderia == "2"))
                {
                    if (dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString() != "0")
                    {
                        if (textBox26.Text == "")
                        {
                            double cantidad = Convert.ToDouble(textBox4.Text);
                            dataGridView4.CurrentRow.Cells[3].Value = Math.Round(cantidad / Convert.ToDouble(dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString()),2);
                            dataGridView4.CurrentRow.Cells[5].Value = Convert.ToDouble(dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString()) * Convert.ToDouble(textBox5.Text);
                        }
                        else
                        {
                            double cantidad = Convert.ToDouble(textBox26.Text);
                            dataGridView4.CurrentRow.Cells[3].Value = Math.Truncate(cantidad / Convert.ToDouble(dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString()));
                            dataGridView4.CurrentRow.Cells[5].Value = Convert.ToDouble(dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value.ToString()) * Convert.ToDouble(textBox5.Text);
                        }
                    }
                }
            }
            else
            {
                if (e.ColumnIndex == 1)
                {
                    dataGridView4.CurrentCell.Value = "INGRESE UNA PRESENTACION!";
                }
                else if(e.ColumnIndex == 0)
                {
                    dataGridView4.CurrentCell.Value = getCode();
                }
            }
        }
        private bool Revisa_precios()
        {
            for (int i = 0; i < dataGridView4.RowCount; i++)
            {
                for (int j = 6; j < 11; j++)
                {
                    if (dataGridView4.Rows[i].Cells[j].Value.ToString() != "0")
                    {
                        if (Convert.ToDouble(dataGridView4.Rows[i].Cells[j].Value.ToString()) < Convert.ToDouble(dataGridView4.Rows[i].Cells[5].Value.ToString()))
                        {
                            MessageBox.Show("EL PRECIO APLICADO EN UNA PRESENTACION ES MENOR AL PRECIO COSTO!", "INGRESO MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private void dataGridView4_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex > 1)
            {
                double newInteger;
                if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
                {
                    dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value = "0";
                }
                if (!double.TryParse(e.FormattedValue.ToString(), out newInteger) || newInteger < 0)
                {
                    dataGridView4.CurrentRow.Cells[e.ColumnIndex].Value = "0";
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Image imgFinal = (Image)pictureBox1.Image.Clone();
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Title = "Guardar codigo de Barra";
                saveFileDialog1.AddExtension = true;
                saveFileDialog1.Filter = "Image PNG (*.png)|*.png";
                saveFileDialog1.ShowDialog();
                if (!string.IsNullOrEmpty(saveFileDialog1.FileName))
                {
                    imgFinal.Save(saveFileDialog1.FileName, ImageFormat.Png);
                }
                imgFinal.Dispose();
            }
            else
            {
                MessageBox.Show("NO SE HA GENERADO CODIGO DE BARRA!", "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var forma = new frm_buscaProveedor(1, nameUs, rolUs, usuario, idSucursal, privilegios);
            if (forma.ShowDialog() == DialogResult.OK)
            {
                comboBox5.Items.Clear();
                cargaProveedor();
                comboBox5.Text = forma.currentProveedor.getProveedor;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var forma = new frm_buscaEdicion(nombreSucursal, rolUs);
            if (forma.ShowDialog() == DialogResult.OK)
            {
                textBox6.Text = forma.CurrentMercaderia.getMercaderia;
                textBox28.Text = forma.CurrentSucursal.getSucursal;
                getKardex(textBox6.Text.Trim(), textBox28.Text.Trim());
            }
        }
        private void getKardex(string mercaderia, string nombreSucursal)
        {
            if (textBox6.Text != "")
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    
                    string sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE ID_MERCADERIA = '{0}'  AND NOMBRE_SUCURSAL = '{1}';", mercaderia, nombreSucursal);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView2.Rows.Clear();
                        textBox6.Text = reader.GetString(0);
                        textBox9.Text = reader.GetString(1);
                        textBox7.Text = reader.GetString(2);
                        textBox11.Text = "" + reader.GetDouble(4);
                        textBox10.Text = "" + reader.GetDouble(5);
                        textBox12.Text = reader.GetString(9);
                        conexion = ASG_DB.connectionResult();
                        cargaComprasKardex(textBox6.Text.Trim(),textBox28.Text.Trim());
                        cargaFacturas(textBox6.Text.Trim(), textBox28.Text.Trim());
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
                textBox6.Text = "";
                textBox7.Text = "";
                textBox10.Text = "";
                textBox11.Text = "";
                textBox12.Text = "";
                textBox28.Text = "";
            }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            var forma = new frm_buscaEdicion(nombreSucursal, rolUs);
            if (forma.ShowDialog() == DialogResult.OK)
            {
                textBox14.Text = forma.CurrentMercaderia.getMercaderia;
                textBox24.Text = forma.CurrentSucursal.getSucursal;
                if (textBox14.Text != "")
                {
                    getdataTraslado(textBox14.Text.Trim(), textBox24.Text.Trim());
                    groupBox8.Text = textBox24.Text;
                }
            }
        }
        private void getdataEdit(string mercaderia, string nombreSucursal)
        {
            if (textBox3.Text != "")
            {
                try
                {
                    dataGridView4.Rows.Clear();
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT * FROM VISTA_MERCADERIA_EDICION WHERE ID_MERCADERIA = '{0}'  AND NOMBRE_SUCURSAL = '{1}';", mercaderia, nombreSucursal);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        generaBarcode(reader.GetString(0));
                        textBox2.Text = reader.GetString(0);
                        comboBox5.Text = reader.GetString(1);
                        textBox1.Text = reader.GetString(2);
                       
                        textBox5.Text = "" + reader.GetDouble(3);
                        textBox4.Text = "" + reader.GetDouble(4);
                        if (reader.GetBoolean(5))
                        {
                            checkBox2.Checked = true;
                        }
                        else
                        {
                            checkBox2.Checked = false;
                        }
                        tipoMercaderia = reader.GetString(6);
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("SELECT * FROM VISTA_STOCK WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", mercaderia, nombreSucursal);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {

                            dataGridView4.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                            while (reader.Read())
                            {
                                dataGridView4.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                                //styleDV(this.dataGridView4);
                            }
                        }
                    }
                    else
                    {
                        comboBox5.Items.Clear();
                        textBox1.Text = "";
                        dataGridView4.Rows.Clear();
                        cargaProveedor();
                        textBox3.Text = "";
                        textBox4.Text = "";
                        textBox5.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                comboBox5.Items.Clear();
                textBox1.Text = "";
                dataGridView4.Rows.Clear();
                cargaProveedor();
            }
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if ((comboBox1.SelectedIndex != 2))
            {
                buscaEspecificacion();
            } else
            {
                buscaTotales();
            }
        }
        private void buscaEspecificacion()
        {
            if ((textBox8.Text != ""))
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE ID_MERCADERIA LIKE '%{0}%' AND TIPO_MERCADERIA = '{1}' AND NOMBRE_SUCURSAL = '{2}';", textBox8.Text, comboBox1.Text, comboBox2.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                            //styleDV(this.dataGridView1);
                        }
                        setDescripcion(7);
                        setDisponible(3);
                    }
                    else
                    {
                        conexion.Close();
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE DESCRIPCION LIKE '%{0}%' AND TIPO_MERCADERIA = '{1}' AND NOMBRE_SUCURSAL = '{2}';", textBox8.Text, comboBox1.Text, comboBox2.Text);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                //styleDV(this.dataGridView1);
                            }
                            setDescripcion(7);
                            setDisponible(3);
                        }
                        else
                        {
                            conexion.Close();
                            conexion = ASG_DB.connectionResult();
                            sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE NOMBRE_PROVEEDOR LIKE '%{0}%' AND TIPO_MERCADERIA = '{1}'  AND NOMBRE_SUCURSAL = '{2}';", textBox8.Text, comboBox1.Text, comboBox2.Text);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Clear();
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                    //styleDV(this.dataGridView1);
                                }
                                setDescripcion(7);
                                setDisponible(3);
                            }
                            else
                            {
                                conexion.Close();
                                conexion = ASG_DB.connectionResult();
                                sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE PRECIO_COMPRA LIKE '%{0}%' AND TIPO_MERCADERIA = '{1}' AND NOMBRE_SUCURSAL = '{2}';", textBox8.Text, comboBox1.Text, comboBox2.Text);
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Clear();
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                        //styleDV(this.dataGridView1);
                                    }
                                    setDescripcion(7);
                                    setDisponible(3);
                                }
                                else
                                {
                                    conexion.Close();
                                    conexion = ASG_DB.connectionResult();
                                    sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE TOTAL_UNIDADES LIKE '%{0}%' AND TIPO_MERCADERIA = '{1}' AND NOMBRE_SUCURSAL = '{2}';", textBox8.Text, comboBox1.Text, comboBox2.Text);
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Clear();
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                        while (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                            //styleDV(this.dataGridView1);
                                        }
                                        setDescripcion(7);
                                        setDisponible(3);
                                    }
                                    
                                }
                            }
                        }
                    }
                }
                catch (Exception es)
                {
                    MessageBox.Show(es.ToString());
                }
            }
            else
            {
                cargaClasificacion(comboBox1.Text);
            }
        }
        private void buscaTotales()
        {
            if ((textBox8.Text != ""))
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE ID_MERCADERIA LIKE '%{0}%'  AND NOMBRE_SUCURSAL = '{1}';", textBox8.Text, comboBox2.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                            //styleDV(this.dataGridView1);
                        }
                        setDescripcion(7);
                        setDisponible(3);
                    }
                    else
                    {
                        conexion.Close();
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE DESCRIPCION LIKE '%{0}%'  AND NOMBRE_SUCURSAL = '{1}';", textBox8.Text, comboBox2.Text);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                //styleDV(this.dataGridView1);
                            }
                            setDescripcion(7);
                            setDisponible(3);
                        }
                        else
                        {
                            conexion.Close();
                            conexion = ASG_DB.connectionResult();
                            sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE NOMBRE_PROVEEDOR LIKE '%{0}%'  AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, comboBox2.Text);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Clear();
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                    //styleDV(this.dataGridView1);
                                }
                                setDescripcion(7);
                                setDisponible(3);
                            }
                            else
                            {
                                conexion.Close();
                                conexion = ASG_DB.connectionResult();
                                sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE PRECIO_COMPRA LIKE '%{0}%'  AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, comboBox2.Text);
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Clear();
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                        //styleDV(this.dataGridView1);
                                    }
                                    setDescripcion(7);
                                    setDisponible(3);
                                }
                                else
                                {
                                    conexion.Close();
                                    conexion = ASG_DB.connectionResult();
                                    sql = string.Format("SELECT * FROM VISTA_MERCADERIA_C WHERE TOTAL_UNIDADES LIKE '%{0}%'  AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, comboBox2.Text);
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Clear();
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                        while (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(4), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(6)), reader.GetString(2), reader.GetString(3), reader.GetString(1), reader.GetString(7), reader.GetString(8));
                                            //styleDV(this.dataGridView1);
                                        }
                                        setDescripcion(7);
                                        setDisponible(3);
                                    }
                                    else
                                    {
                                        cargaMercaderia();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception es)
                {
                    MessageBox.Show(es.ToString());
                }
            }
            else
            {
                cargaMercaderia();
            }
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
           /*if (e.KeyChar == 13)
            {
                getdataEdit(textBox3.Text.Trim());
            }*/
        }
        private void getdataTraslado(string mercaderia, string sucursal)
        {
            if (textBox14.Text != "")
            {
                try
                {
                    dataGridView3.Rows.Clear();
                    dataGridView5.Rows.Clear();

                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE ID_MERCADERIA = '{0}'  AND NOMBRE_SUCURSAL = '{1}';", mercaderia, sucursal);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        comboBox4.SelectedIndex = -1;
                        textBox27.Text = "0";
                        textBox20.Text = "";
                        textBox18.Text = reader.GetString(0);
                        textBox19.Text = reader.GetString(1);
                        textBox17.Text = reader.GetString(2);
                        textBox15.Text = "" + reader.GetDouble(4);
                        //textBox21.Text = "" + reader.GetDouble(4);
                        textBox16.Text = "" + reader.GetDouble(5);
                        tipoTraslado = reader.GetString(6);
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("SELECT * FROM VISTA_STOCK WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", mercaderia, sucursal);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {

                            dataGridView3.Rows.Add(reader.GetString(0),reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                            while (reader.Read())
                            {
                                dataGridView3.Rows.Add(reader.GetString(0),reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                                //styleDV(this.dataGridView3);
                            }
                        }
                    }
                    else
                    {
                        textBox14.Text = "";
                        textBox18.Text = "";
                        textBox17.Text = "";
                        textBox19.Text = "";
                        textBox15.Text = "";
                        textBox16.Text = "";
                        dataGridView3.Rows.Clear();
                        dataGridView5.Rows.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                textBox14.Text = "";
                textBox18.Text = "";
                textBox17.Text = "";
                textBox19.Text = "";
                textBox15.Text = "";
                textBox16.Text = "";
                dataGridView3.Rows.Clear();
                dataGridView5.Rows.Clear();
            }
        }
        private void getDisponibles()
        {
            if (tipoTraslado == "1")
            {
                
                    for (int i = 0; i < dataGridView5.RowCount; i++)
                    {
                        dataGridView5.Rows[i].Cells[3].Value = Math.Truncate((Convert.ToDouble(textBox20.Text) + disponible) / Convert.ToDouble(dataGridView5.Rows[i].Cells[2].Value.ToString()));
                    }
                
            }
            else if(tipoTraslado == "2")
            {
                for (int i = 0; i < dataGridView5.RowCount; i++)
                {
                    dataGridView5.Rows[i].Cells[3].Value = Math.Round((Convert.ToDouble(textBox20.Text) + disponible) / Convert.ToDouble(dataGridView5.Rows[i].Cells[2].Value.ToString()),2);
                }
            }
        }
        private void getprecioCosto()
        {
            if (textBox21.Text != "")
            {
                for (int i = 0; i< dataGridView5.RowCount; i++)
                {
                    dataGridView5.Rows[i].Cells[6].Value = Convert.ToDouble(textBox21.Text) * Convert.ToDouble(dataGridView5.Rows[i].Cells[2].Value.ToString());
                }
            }
        }
        private void button15_Click(object sender, EventArgs e)
        {
            button15.Focus();
            if ((textBox20.Text != "0") && (textBox20.Text != "") &&(textBox20.Text != " ") &&(comboBox4.Text != ""))
            {
                
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    
                
                    if (!presentacionRepetida(dataGridView3.Rows[i].Cells[1].Value.ToString()))
                    {                  
                        dataGridView5.Rows.Add(dataGridView3.Rows[i].Cells[0].Value, dataGridView3.Rows[i].Cells[1].Value, dataGridView3.Rows[i].Cells[2].Value, 0, dataGridView3.Rows[i].Cells[4].Value,
                        dataGridView3.Rows[i].Cells[5].Value, dataGridView3.Rows[i].Cells[6].Value, dataGridView3.Rows[i].Cells[7].Value, dataGridView3.Rows[i].Cells[8].Value, dataGridView3.Rows[i].Cells[9].Value,
                        dataGridView3.Rows[i].Cells[10].Value, dataGridView3.Rows[i].Cells[11].Value, dataGridView3.Rows[i].Cells[12].Value, dataGridView3.Rows[i].Cells[13].Value, dataGridView3.Rows[i].Cells[14].Value, dataGridView3.Rows[i].Cells[15].Value);
                        button15.Enabled = false;
                        getDisponibles();
                        getSobrante();
                        getprecioCosto();
                        
                    }
                    
                }     
            } else
            {
                MessageBox.Show("DEBE INGRESAR CANTIDAD A TRASLADAR!", "TRASLADO DE MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void textBox20_KeyPress(object sender, KeyPressEventArgs e)
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
        private bool presentacionRepetida(string presentacion)
        {
            if (dataGridView1.RowCount > 0) {
                for (int i = 0; i < dataGridView5.RowCount; i++)
                {
                    if (presentacion == dataGridView5.Rows[i].Cells[1].Value.ToString())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void dataGridView3_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView3.RowCount>0)
            {
                if ((textBox20.Text != "0") && (textBox20.Text != "") && (textBox20.Text != " "))
                {
                    if (!presentacionRepetida(dataGridView3.CurrentRow.Cells[1].Value.ToString()))
                    {
                        dataGridView5.Rows.Add(dataGridView3.CurrentRow.Cells[0].Value, dataGridView3.CurrentRow.Cells[1].Value, dataGridView3.CurrentRow.Cells[2].Value, 0, dataGridView3.CurrentRow.Cells[4].Value,
                                    dataGridView3.CurrentRow.Cells[5].Value, dataGridView3.CurrentRow.Cells[6].Value, dataGridView3.CurrentRow.Cells[7].Value, dataGridView3.CurrentRow.Cells[8].Value, dataGridView3.CurrentRow.Cells[9].Value,
                                    dataGridView3.CurrentRow.Cells[10].Value, dataGridView3.CurrentRow.Cells[11].Value, dataGridView3.CurrentRow.Cells[12].Value, dataGridView3.CurrentRow.Cells[13].Value, dataGridView3.CurrentRow.Cells[14].Value, dataGridView3.CurrentRow.Cells[15].Value);
                        getDisponibles();
                        getSobrante();
                        getprecioCosto();
                    }
                    else
                    {
                        MessageBox.Show("LA PRESENTACION YA EXISTE!", "TRASLADO DE MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                } else
                {
                    MessageBox.Show("DEBE INGRESAR CANTIDAD A TRASLADAR!", "TRASLADO DE MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        private void cleanTraslado()
        {
            textBox14.Text = "";
            textBox18.Text = "";
            textBox17.Text = "";
            textBox19.Text = "";
            textBox15.Text = "";
            textBox16.Text = "";
            textBox20.Text = "";
            textBox22.Text = "";
            textBox21.Text = "";
            textBox24.Text = "";
            comboBox4.SelectedIndex = -1;
            button15.Enabled = true;
            dataGridView3.Rows.Clear();
            dataGridView5.Rows.Clear();
            tipoTraslado = "";
            button17.PerformClick();
            textBox27.Text = "";
        }
        private void button18_Click(object sender, EventArgs e)
        {
            if (textBox14.Text != "" | textBox18.Text != "" | textBox17.Text != "" | textBox19.Text != "" | textBox16.Text != "" | textBox15.Text != "" | dataGridView5.RowCount >0 | dataGridView3.RowCount > 0)
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "TRASLADO DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    textBox14.Text = "";
                    textBox18.Text = "";
                    textBox17.Text = "";
                    textBox19.Text = "";
                    textBox15.Text = "";
                    textBox16.Text = "";
                    textBox20.Text = "";
                    textBox22.Text = "";
                    textBox21.Text = "";
                    textBox24.Text = "";
                    textBox27.Text = "";
                    comboBox4.SelectedIndex = -1;
                    button15.Enabled = true;
                    dataGridView3.Rows.Clear();
                    dataGridView5.Rows.Clear();
                    tipoTraslado = "";
                    textBox23.Text = "";
                }
            }
        }
        private void getSobrante()
        {
            if (tipoTraslado == "1")
            {
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    dataGridView3.Rows[i].Cells[3].Value = Math.Truncate(Convert.ToDouble(textBox22.Text) / Convert.ToDouble(dataGridView3.Rows[i].Cells[2].Value.ToString()));
                }
            }
            else if(tipoTraslado == "2")
            {
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    dataGridView3.Rows[i].Cells[3].Value = Math.Round(Convert.ToDouble(textBox22.Text) / Convert.ToDouble(dataGridView3.Rows[i].Cells[2].Value.ToString()),2);
                }
            }
        }
        private void textBox20_Leave(object sender, EventArgs e)
        {
            if((textBox20.Text != "") &&(textBox20.Text != "0") &&(textBox20.Text != " "))
            {
                if(Convert.ToDouble(textBox20.Text)> Convert.ToDouble(textBox16.Text))
                {
                    MessageBox.Show("LA CANTIDAD A TRANSFERIR NO PUEDE SER MAYOR QUE EL EXISTENTE!", "TRASLADO DE MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox20.Text = "";
                }
                else
                {
                    textBox22.Text = ""+(Convert.ToDouble(textBox16.Text) - Convert.ToDouble(textBox20.Text));
                }
            }
            if ((dataGridView5.RowCount > 0) && (textBox20.Text != "0") && (textBox20.Text != " "))
            {
                getDisponibles();
                getSobrante();
            }
        }

        private void textBox21_KeyPress(object sender, KeyPressEventArgs e)
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
       
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (textBox24.Text != "" & textBox18.Text != "") {
                if (comboBox4.Text == textBox24.Text)
                {
                    comboBox4.SelectedIndex = -1;
                    MessageBox.Show("DEBE SELECCIONAR UNA SUCURSAL DISTINTA", "TRASLADO DE MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    cargaExistente(textBox18.Text.Trim(), comboBox4.Text);
                    groupBox10.Text = comboBox4.Text;
                }
            }
        }
        internal class Sucursal
        {
            public string getSucursal { get; set; }

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

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox4.Focus();
            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button2.Focus();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
           
            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox25.Focus();
            }
        }
        private void clenEdicion()
        {
            textBox3.Text = "";
            textBox2.Text = "";
            textBox1.Text = "";
            textBox5.Text = "";
            textBox4.Text = "";
            textBox23.Text = "";
            dataGridView4.Rows.Clear();
            pictureBox1.Image = null;
            textBox2.BackColor = Color.White;
            textBox2.ForeColor = Color.Black;
            textBox25.Text = "";
            textBox26.Text = "";
            button17.PerformClick();
            comboBox5.SelectedIndex = -1;
        }
        private void button19_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" | textBox1.Text!= "" | dataGridView4.RowCount > 0) {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "TRASLADO DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    textBox3.Text = "";
                    textBox2.Text = "";
                    textBox1.Text = "";
                    textBox5.Text = "";
                    textBox4.Text = "";
                    textBox23.Text = "";
                    dataGridView4.Rows.Clear();
                    pictureBox1.Image = null;
                    textBox2.BackColor = Color.White;
                    textBox2.ForeColor = Color.Black;
                    textBox25.Text = "";
                    textBox26.Text = "";
                    comboBox5.SelectedIndex = -1;
                }
            }

        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (textBox26.Text != "" && textBox26.Text != "0")
            {
                // si existe cantidad a aplicar
                if (textBox25.Text != "" && textBox25.Text !="0")
                {
                    // si existe un nuevo precio
                    string precio_ponderado;
                    var forma = new frm_precioPonderado(textBox5.Text, textBox25.Text, textBox26.Text, textBox26.Text);
                    forma.ShowDialog();
                    if (forma.DialogResult == DialogResult.OK)
                    {
                        precio_ponderado = forma.CurrentPrecio.getPrecio;
                        for (int i = 0; i < dataGridView4.RowCount; i++)
                        {
                            dataGridView4.Rows[i].Cells[5].Value = Math.Round(Convert.ToDouble(dataGridView4.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(precio_ponderado), 2);
                        }
                    }
                    else if (forma.DialogResult == DialogResult.Yes)
                    {
                        for (int i = 0; i < dataGridView4.RowCount; i++)
                        {
                            dataGridView4.Rows[i].Cells[5].Value = Math.Round(Convert.ToDouble(dataGridView4.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(textBox25.Text), 2);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("NO EXISTEN UN NUEVO PRECIO DE REFERENCIA", "EDICION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } else
            {
                // aplicar otra formula ponderado con existencias y nuevos precios
                if (textBox25.Text != "" && textBox25.Text != "0")
                // si existe un nuevo precio
                { 
                string precio_ponderado;
                var forma = new frm_precioPonderado(textBox5.Text, textBox25.Text, textBox4.Text, textBox4.Text);
                forma.ShowDialog();
                if (forma.DialogResult == DialogResult.OK)
                {
                    precio_ponderado = forma.CurrentPrecio.getPrecio;
                    for (int i = 0; i < dataGridView4.RowCount; i++)
                    {
                        dataGridView4.Rows[i].Cells[5].Value = Math.Round(Convert.ToDouble(dataGridView4.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(precio_ponderado), 2);
                    }
                }
                else if (forma.DialogResult == DialogResult.Yes)
                {
                    for (int i = 0; i < dataGridView4.RowCount; i++)
                    {
                        dataGridView4.Rows[i].Cells[5].Value = Math.Round(Convert.ToDouble(dataGridView4.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(textBox25.Text), 2);
                    }
                }
                } else
                {
                    MessageBox.Show("NO EXISTEN UN NUEVO PRECIO DE REFERENCIA", "EDICION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void textBox25_KeyPress(object sender, KeyPressEventArgs e)
        {
            /* if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))

             {
                 e.Handled = false;
             }
             else
             {
                 e.Handled = true;
             }*/
            e.Handled = solonumeros(Convert.ToInt32(e.KeyChar));
        }

        private void textBox26_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))

            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }*/
            e.Handled = solonumerosCantidad(Convert.ToInt32(e.KeyChar));
        }
        Boolean permitir = true;
        public bool solonumeros(int code)
        {
            bool resultado;
            if (code == 46 && textBox25.Text.Contains("."))//se evalua si es punto y si es punto se rebiza si ya existe en el textbox
            {
                resultado = true;
            }
            else if ((((code >= 48) && (code <= 57)) || (code == 8) || code == 46)) //se evaluan las teclas validas
            {
                resultado = false;
            }
            else if (!permitir)
            {
                resultado = permitir;
            }
            else
            {
                resultado = true;
            }
            return resultado;
        }

        public bool solonumerosCantidad(int code)
        {
            bool resultado;
            if (code == 46 && textBox26.Text.Contains("."))//se evalua si es punto y si es punto se rebiza si ya existe en el textbox
            {
                resultado = true;
            }
            else if ((((code >= 48) && (code <= 57)) || (code == 8) || code == 46)) //se evaluan las teclas validas
            {
                resultado = false;
            }
            else if (!permitir)
            {
                resultado = permitir;
            }
            else
            {
                resultado = true;
            }
            return resultado;
        }
        private void textBox25_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox26.Focus();
            } else if (e.KeyData == Keys.S)
            {
                textBox25.Text = textBox5.Text;
                textBox26.Focus();
            }
            bool paste = (Convert.ToInt32(e.KeyData) == (Convert.ToInt32(Keys.Control) | Convert.ToInt32(Keys.V)));
            bool copy = (Convert.ToInt32(e.KeyData) == (Convert.ToInt32(Keys.Control) | Convert.ToInt32(Keys.C)));
            if (paste || copy)
            {
                permitir = false;
            }
            else
            {
                permitir = true;
            }
        }

        private void textBox26_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button2.Focus();
            }
            bool paste = (Convert.ToInt32(e.KeyData) == (Convert.ToInt32(Keys.Control) | Convert.ToInt32(Keys.V)));
            bool copy = (Convert.ToInt32(e.KeyData) == (Convert.ToInt32(Keys.Control) | Convert.ToInt32(Keys.C)));
            if (paste || copy)
            {
                permitir = false;
            }
            else
            {
                permitir = true;
            }

        }

        private void dataGridView5_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView5.CurrentRow.Cells[e.ColumnIndex].Value != null)
            {
                if ((e.ColumnIndex == 6) | (e.ColumnIndex == 7) | (e.ColumnIndex == 8) | (e.ColumnIndex == 9) | (e.ColumnIndex == 10))
                {
                    if (dataGridView5.CurrentRow.Cells[e.ColumnIndex].Value.ToString() != "0")
                    {
                        double diferencial = Convert.ToDouble(dataGridView5.CurrentRow.Cells[e.ColumnIndex].Value.ToString()) - Convert.ToDouble(dataGridView5.CurrentRow.Cells[5].Value.ToString());
                        double porcentaje = Math.Round(diferencial / Convert.ToDouble(dataGridView5.CurrentRow.Cells[5].Value.ToString()), 4) * 100;
                        if (e.ColumnIndex == 6)
                        {
                            dataGridView5.CurrentRow.Cells[11].Value = porcentaje;
                        }
                        else if (e.ColumnIndex == 7)
                        {
                            dataGridView5.CurrentRow.Cells[12].Value = porcentaje;
                        }
                        else if (e.ColumnIndex == 8)
                        {
                            dataGridView5.CurrentRow.Cells[13].Value = porcentaje;
                        }
                        else if (e.ColumnIndex == 9)
                        {
                            dataGridView5.CurrentRow.Cells[14].Value = porcentaje;
                        }
                        else if (e.ColumnIndex == 10)
                        {
                            dataGridView5.CurrentRow.Cells[15].Value = porcentaje;
                        } 
                    }
                }
                else if ((e.ColumnIndex == 11) | (e.ColumnIndex == 12) | (e.ColumnIndex == 13) | (e.ColumnIndex == 14) | (e.ColumnIndex == 15))
                {
                    if ((Convert.ToDouble(dataGridView5.CurrentRow.Cells[5].Value.ToString()) > 0))
                    {
                        double precio = Convert.ToDouble(dataGridView5.CurrentRow.Cells[5].Value.ToString());
                        precio = precio + (precio * (Convert.ToDouble(dataGridView5.CurrentRow.Cells[e.ColumnIndex].Value.ToString()) / 100));
                        precio = Math.Round(precio, 4);
                        if (e.ColumnIndex == 11)
                        {
                            dataGridView5.CurrentRow.Cells[6].Value = precio;
                        }
                        else if (e.ColumnIndex == 12)
                        {
                            dataGridView5.CurrentRow.Cells[7].Value = precio;
                        }
                        else if (e.ColumnIndex == 13)
                        {
                            dataGridView5.CurrentRow.Cells[8].Value = precio;
                        }
                        else if (e.ColumnIndex == 14)
                        {
                            dataGridView5.CurrentRow.Cells[9].Value = precio;
                        }
                        else if (e.ColumnIndex == 15)
                        {
                            dataGridView5.CurrentRow.Cells[10].Value = precio;
                        }
                    }
                }
                else if (e.ColumnIndex == 2)
                {
                    getDisponibles();
                }

            } else
            {
                if (e.ColumnIndex > 1)
                    dataGridView5.CurrentCell.Value = "0";
                else dataGridView5.CurrentCell.Value = "INGRESE UNA PRESENTACION!";
            }
           
            
        }
      
        private bool existenciasStock(string codigoProducto, string nombreSucursal)
        {
            return true; 
        }
        private bool existenciaProducto(string codigoProducto, string nombreSucursal)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
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
            return false;
        }
        private int existeSucursales(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = sql = string.Format("SELECT COUNT(ID_SUCURSAL) FROM MERCADERIA WHERE ID_MERCADERIA = '{0}';",codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return 0;
        }
        private void sucursalesDisponibles(string codigo, string sucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = sql = string.Format("SELECT ID_SUCURSAL, TOTAL_UNIDADES FROM MERCADERIA WHERE ID_MERCADERIA = '{0}' AND ESTADO_TUPLA = TRUE AND ID_SUCURSAL <> (SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{1}');", codigo, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView9.Rows.Add(reader.GetString(0), reader.GetString(1));
                    while (reader.Read())
                    {
                        dataGridView9.Rows.Add(reader.GetString(0), reader.GetString(1));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
                    } else
                    {
                       // MessageBox.Show("fallo update stock");
                    }

                }
                else
                {
                    // VERIFICAR SI EXISTE EN MAS SUCURSALES
                    conexion.Close();
                    if(existeSucursales(mercaderia) > 1)
                    {
                        DialogResult result;
                        result = MessageBox.Show("EL PRODUCTO EDITADO SE ENCUENTRA EN MAS DE UNA SUCURSAL" + "\n" + "¿DESEA CREAR LA NUEVA PRESENTACION:  \"" + descipcion + " " + "(" + cantidadP + ")" + "\" EN LAS DE MAS SUCURSALES?" + "\n" + "SI NO GUARDA LA PRESENTACION EN EL RESTO DE SUCURSALES, PODRA HACERLO MANUALMENTE EDITANDO EL PRODUCTO EN CADA SUCURSAL","EDICION MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                           // MessageBox.Show("SE DIJO QUE SI");
                            sucursalesDisponibles(mercaderia, sucursal);
                            if (dataGridView9.RowCount > 0)
                            {
                                //MessageBox.Show("HAY DATO EN DATAGRIDVIEWI");
                                for (int i = 0; i<dataGridView9.RowCount; i++)
                                {
                                    if (tipoMercaderia == "1")
                                    {
                                        // ES ABARROTES INGRESO
                                        //MessageBox.Show("SE DIJO QUE SI");
                                        double temp = Math.Truncate( Convert.ToDouble(dataGridView9.Rows[i].Cells[1].Value.ToString()) / Convert.ToDouble(cantidadP));
                                        conexion = ASG_DB.connectionResult();
                                        sql = sql = string.Format("CALL INGRESA_STOCK_TRASLADO_ACTUALIZACION ('{0}','{1}',{2},'{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17});", stock, mercaderia, dataGridView9.Rows[i].Cells[0].Value.ToString(), descipcion, cantidadP, temp, cantM, p1, p2, p3, p4, p5, p6, u1, u2, u3, u4, u5);
                                        cmd = new OdbcCommand(sql, conexion);
                                        if (cmd.ExecuteNonQuery() == 1)
                                        {
                                            //  MessageBox.Show("SE CREA TIPO VENTA TIPO VENTA");
                                            conexion.Close();
                                        }
                                       
                                    }
                                    else if(tipoMercaderia == "2")
                                    {
                                        //MessageBox.Show("SE ENTRO EN QUE ES ESPECIAS");
                                        double temp2 = Math.Round(Convert.ToDouble(dataGridView9.Rows[i].Cells[1].Value.ToString()) / Convert.ToDouble(cantidadP), 2);
                                        conexion = ASG_DB.connectionResult();
                                        sql = sql = string.Format("CALL INGRESA_STOCK_TRASLADO_ACTUALIZACION ('{0}','{1}',{2},'{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17});", stock, mercaderia, dataGridView9.Rows[i].Cells[0].Value.ToString(), descipcion, cantidadP, temp2, cantM, p1, p2, p3, p4, p5, p6, u1, u2, u3, u4, u5);
                                        cmd = new OdbcCommand(sql, conexion);
                                        if (cmd.ExecuteNonQuery() == 1)
                                        {
                                            //  MessageBox.Show("SE CREA TIPO VENTA TIPO VENTA");
                                            conexion.Close();
                                        }
                                    }
                                }
                            }
                        }                        
                    }
                    conexion = ASG_DB.connectionResult();
                    sql = sql = string.Format("CALL INGRESA_STOCK_TRASLADO ('{0}','{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17});", stock, mercaderia,sucursal ,descipcion, cantidadP, total, cantM, p1, p2, p3, p4, p5, p6, u1, u2, u3, u4, u5);
                    cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                      //  MessageBox.Show("SE CREA TIPO VENTA TIPO VENTA");
                        conexion.Close();
                    } else
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
        private void actualizaMercaderia()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("CALL UPDATE_MERCADERIA_TRASLADO ('{0}',{1},{2},'{3}');", textBox18.Text.Trim(),textBox15.Text.Trim(),textBox22.Text.Trim(),textBox24.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("ACTUALIZO MERCADERIA DE TRASLADO");
                    for (int i = 0; i < dataGridView3.RowCount; i++)
                    {
                        presentacionesIngreso(textBox18.Text, dataGridView3.Rows[i].Cells[0].Value.ToString(), dataGridView3.Rows[i].Cells[1].Value.ToString(), dataGridView3.Rows[i].Cells[2].Value.ToString(), dataGridView3.Rows[i].Cells[3].Value.ToString(), dataGridView3.Rows[i].Cells[4].Value.ToString(), dataGridView3.Rows[i].Cells[5].Value.ToString(), dataGridView3.Rows[i].Cells[6].Value.ToString(), dataGridView3.Rows[i].Cells[7].Value.ToString(), dataGridView3.Rows[i].Cells[8].Value.ToString(), dataGridView3.Rows[i].Cells[9].Value.ToString(), dataGridView3.Rows[i].Cells[10].Value.ToString(), dataGridView3.Rows[i].Cells[11].Value.ToString(), dataGridView3.Rows[i].Cells[12].Value.ToString(), dataGridView3.Rows[i].Cells[13].Value.ToString(), dataGridView3.Rows[i].Cells[14].Value.ToString(), dataGridView3.Rows[i].Cells[15].Value.ToString(), textBox24.Text.Trim());
                    }
                    timerActions();
                    cleanTraslado();

                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void ingresaMercaderia()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("CALL INGRESA_MERCADERIA_SUCURSAL ('{0}','{1}','{2}','{3}',{4},{5},TRUE);", textBox18.Text.Trim(), comboBox4.Text, textBox19.Text, textBox17.Text, textBox15.Text, textBox20.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    adressUser.ingresaBitacora(idSucursal, usuario, "TRASLADO A - " + comboBox4.Text, textBox18.Text.Trim());
                   // MessageBox.Show("INGRESO CORRECTO MERCADERIA A SUCURSAL NUEVA");
                    for (int i = 0; i < dataGridView5.RowCount; i++)
                    {
                        presentacionesIngreso(textBox18.Text, dataGridView5.Rows[i].Cells[0].Value.ToString(), dataGridView5.Rows[i].Cells[1].Value.ToString(), dataGridView5.Rows[i].Cells[2].Value.ToString(), dataGridView5.Rows[i].Cells[3].Value.ToString(), dataGridView5.Rows[i].Cells[4].Value.ToString(), dataGridView5.Rows[i].Cells[5].Value.ToString(), dataGridView5.Rows[i].Cells[6].Value.ToString(), dataGridView5.Rows[i].Cells[7].Value.ToString(), dataGridView5.Rows[i].Cells[8].Value.ToString(), dataGridView5.Rows[i].Cells[9].Value.ToString(), dataGridView5.Rows[i].Cells[10].Value.ToString(), dataGridView5.Rows[i].Cells[11].Value.ToString(), dataGridView5.Rows[i].Cells[12].Value.ToString(), dataGridView5.Rows[i].Cells[13].Value.ToString(), dataGridView5.Rows[i].Cells[14].Value.ToString(), dataGridView5.Rows[i].Cells[15].Value.ToString(), comboBox4.Text.Trim());
                    }
                    conexion.Close();
                    adressUser.generaNotificacion(comboBox4.Text);
                   actualizaMercaderia();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void actualizaTraslado()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("CALL UPDATE_MERCADERIA_TRASLADOS ('{0}',{1},{2},'{3}');", textBox18.Text.Trim(), textBox15.Text.Trim(), textBox20.Text.Trim(), comboBox4.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("ACTUALIZO MERCADERIA DE TRASLADO");
                    for (int i = 0; i < dataGridView5.RowCount; i++)
                    {
                        presentacionesIngreso(textBox18.Text, dataGridView5.Rows[i].Cells[0].Value.ToString(), dataGridView5.Rows[i].Cells[1].Value.ToString(), dataGridView5.Rows[i].Cells[2].Value.ToString(), dataGridView5.Rows[i].Cells[3].Value.ToString(), dataGridView5.Rows[i].Cells[4].Value.ToString(), dataGridView5.Rows[i].Cells[5].Value.ToString(), dataGridView5.Rows[i].Cells[6].Value.ToString(), dataGridView5.Rows[i].Cells[7].Value.ToString(), dataGridView5.Rows[i].Cells[8].Value.ToString(), dataGridView5.Rows[i].Cells[9].Value.ToString(), dataGridView5.Rows[i].Cells[10].Value.ToString(), dataGridView5.Rows[i].Cells[11].Value.ToString(), dataGridView5.Rows[i].Cells[12].Value.ToString(), dataGridView5.Rows[i].Cells[13].Value.ToString(), dataGridView5.Rows[i].Cells[14].Value.ToString(), dataGridView5.Rows[i].Cells[15].Value.ToString(), comboBox4.Text.Trim());
                    }
                    conexion.Close();
                    adressUser.generaNotificacion(comboBox4.Text);
                    actualizaMercaderia();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void ingresaEspecias()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("CALL INGRESA_ESPECIAS_SUCURSAL ('{0}','{1}','{2}','{3}',{4},{5},TRUE);", textBox18.Text.Trim(), comboBox4.Text, textBox19.Text, textBox17.Text, textBox15.Text, textBox20.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    adressUser.ingresaBitacora(idSucursal, usuario, "TRASLADO A - " + comboBox4.Text, textBox18.Text.Trim());
                    //MessageBox.Show("INGRESO CORRECTO MERCADERIA A SUCURSAL NUEVA");
                    for (int i = 0; i < dataGridView5.RowCount; i++)
                    {
                        presentacionesIngreso(textBox18.Text, dataGridView5.Rows[i].Cells[0].Value.ToString(), dataGridView5.Rows[i].Cells[1].Value.ToString(), dataGridView5.Rows[i].Cells[2].Value.ToString(), dataGridView5.Rows[i].Cells[3].Value.ToString(), dataGridView5.Rows[i].Cells[4].Value.ToString(), dataGridView5.Rows[i].Cells[5].Value.ToString(), dataGridView5.Rows[i].Cells[6].Value.ToString(), dataGridView5.Rows[i].Cells[7].Value.ToString(), dataGridView5.Rows[i].Cells[8].Value.ToString(), dataGridView5.Rows[i].Cells[9].Value.ToString(), dataGridView5.Rows[i].Cells[10].Value.ToString(), dataGridView5.Rows[i].Cells[11].Value.ToString(), dataGridView5.Rows[i].Cells[12].Value.ToString(), dataGridView5.Rows[i].Cells[13].Value.ToString(), dataGridView5.Rows[i].Cells[14].Value.ToString(), dataGridView5.Rows[i].Cells[15].Value.ToString(), comboBox4.Text.Trim());
                    }
                    conexion.Close();
                    adressUser.generaNotificacion(comboBox4.Text);
                    actualizaMercaderia();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void ingresoProductosTraslado(string traslado)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL INGRESA_PRODUCTOS_TRASLADO ({0},'{1}','{2}',{3});", traslado, textBox18.Text, textBox24.Text, textBox20.Text);
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
        private void ingresaTraslado()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string temp = getCodeTraslado();
                string sql = string.Format("CALL NUEVO_TRASLADO ({0},'{1}','{2}','{3}',{4});", temp, usuario, textBox24.Text, comboBox4.Text, 1);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    adressUser.ingresaBitacora(idSucursal, usuario, "TRASLADO A - " + comboBox4.Text, temp);
                    ingresoProductosTraslado(temp); 
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void trasladoAbarrotes()
        {
            try
            {
                if (!existenciaProducto(textBox18.Text.Trim(),comboBox4.Text))
                {
                    ingresaTraslado();
                    ingresaMercaderia();                                    
                } else
                {
                    // MessageBox.Show("EL PRODUCTO EXISTE");
                    ingresaTraslado();
                    actualizaTraslado();                   
                }

            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void button14_Click(object sender, EventArgs e)
        {
            if(textBox18.Text != "" && textBox20.Text != "" && textBox15.Text != "" && textBox16.Text != "" && textBox22.Text != "" & comboBox4.Text != "" & dataGridView3.RowCount >0 & dataGridView5.RowCount > 0)
            {
                if (tipoTraslado == "1")
                {
                    trasladoAbarrotes();
                    disponible = 0;

                } else if (tipoTraslado == "2")
                {
                    trasladoEspecias();
                    disponible = 0;
                }
            } else
            {
                MessageBox.Show("DEBE DE LLENAR TODOS LOS CAMPOS", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void trasladoEspecias()
        {
            try
            {
                if (!existenciaProducto(textBox18.Text.Trim(), comboBox4.Text))
                {
                    ingresaTraslado();
                    ingresaEspecias();
                }
                else
                {
                    // MessageBox.Show("EL PRODUCTO EXISTE");
                    ingresaTraslado();
                    actualizaTraslado();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void button20_Click(object sender, EventArgs e)
        {
            if (dataGridView5.RowCount > 0) { 
            if (textBox21.Text != "" & textBox21.Text != "0")
            {
                getprecioCosto();
            } else
            {
                MessageBox.Show("INGRESE PRECIO VALIDO", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            } else
            {
                MessageBox.Show("DEBE SELECCIONAR EL PRODUCTO Y TIPOS DE VENTA A TRASLADAR!", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void cargaExistente(string mercaderia, string sucursal)
        {
            try
            { 
                dataGridView5.Rows.Clear();
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE ID_MERCADERIA = '{0}'  AND NOMBRE_SUCURSAL = '{1}';", mercaderia, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {  
                    disponible  = reader.GetDouble(5);
                    textBox27.Text = ""+disponible;
                    conexion = ASG_DB.connectionResult();
                    sql = string.Format("SELECT * FROM VISTA_STOCK WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", mercaderia, sucursal);
                    cmd = new OdbcCommand(sql, conexion);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {

                        dataGridView5.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                        while (reader.Read())
                        {
                            dataGridView5.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                            //styleDV(this.dataGridView5);
                        }
                    }
                } else
                {
                    textBox27.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView4_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView4.RowCount > 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    mymenus.Show(dataGridView4, new Point(e.X, e.Y));
                    mymenus.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_ItemclickedS);
                    mymenus.Enabled = true;
                    flags = false;
                }
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox1.Focus();

            }
            
        }

        private void textBox26_Leave(object sender, EventArgs e)
        {
            if (textBox26.Text != "")
            {
                if ((Convert.ToDouble(textBox26.Text) > 0) && (textBox26.Text != "") && (dataGridView4.RowCount > 0))
                {
                   
                        for (int i = 0; i < dataGridView4.RowCount; i++)
                        {
                            dataGridView4.Rows[i].Cells[3].Value = Math.Truncate(Convert.ToDouble(textBox26.Text) / Convert.ToDouble(dataGridView4.Rows[i].Cells[2].Value));
                        }
                    
                }
            }
        }
        private void generalizaMercaderia(string precio)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("CALL UPDATE_MERCADERIA_GENERAL ('{0}','{1}','{2}','{3}',{4});", textBox3.Text.Trim(), textBox2.Text.Trim(), textBox1.Text.Trim(), comboBox5.Text, precio);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("SE EDITO CORRECTAMENTE LA MERCADERIA DE TRASLADO");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void estadoMercaderia()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            { 
                string sql = string.Format("UPDATE MERCADERIA SET ACTIVO = {0} WHERE ID_MERCADERIA = '{1}' AND ID_SUCURSAL = (SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{2}' LIMIT 1);", Convert.ToBoolean(checkBox2.Checked),textBox2.Text.Trim(), textBox23.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1 || cmd.ExecuteNonQuery() == 0)
                {
                    
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void editaMercaderiaSucursal(string precio, string unidades)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("CALL UPDATE_MERCADERIA_EDICION ('{0}','{1}',{2},'{3}','{4}',{5},'{6}');", textBox3.Text.Trim(), textBox2.Text.Trim(), precio, textBox1.Text.Trim(), comboBox5.Text, unidades, textBox23.Text.Trim() ,Convert.ToBoolean(checkBox2.Checked));
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1 || cmd.ExecuteNonQuery() == 0)
                {
                   
                    //MessageBox.Show("SE EDITO CORRECTAMENTE LA MERCADERIA DE TRASLADO");
                    for (int i = 0; i < dataGridView4.RowCount; i++)
                    {
                        presentacionesIngreso(textBox2.Text, dataGridView4.Rows[i].Cells[0].Value.ToString(), dataGridView4.Rows[i].Cells[1].Value.ToString(), dataGridView4.Rows[i].Cells[2].Value.ToString(), dataGridView4.Rows[i].Cells[3].Value.ToString(), dataGridView4.Rows[i].Cells[4].Value.ToString(), dataGridView4.Rows[i].Cells[5].Value.ToString(), dataGridView4.Rows[i].Cells[6].Value.ToString(), dataGridView4.Rows[i].Cells[7].Value.ToString(), dataGridView4.Rows[i].Cells[8].Value.ToString(), dataGridView4.Rows[i].Cells[9].Value.ToString(), dataGridView4.Rows[i].Cells[10].Value.ToString(), dataGridView4.Rows[i].Cells[11].Value.ToString(), dataGridView4.Rows[i].Cells[12].Value.ToString(), dataGridView4.Rows[i].Cells[13].Value.ToString(), dataGridView4.Rows[i].Cells[14].Value.ToString(), dataGridView4.Rows[i].Cells[15].Value.ToString(), textBox23.Text.Trim());
                    }
                    generalizaMercaderia(precio);
                   
                    conexion.Close();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void editaMercaderia(string precio, string unidades)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("CALL UPDATE_MERCADERIA_EDICION ('{0}','{1}',{2},'{3}','{4}',{5},'{6}');", textBox3.Text.Trim(), textBox2.Text.Trim(), precio, textBox1.Text.Trim(), comboBox5.Text, unidades, textBox23.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1 || cmd.ExecuteNonQuery() == 0)
                {
                    estadoMercaderia();
                    //MessageBox.Show("SE EDITO CORRECTAMENTE LA MERCADERIA DE TRASLADO");
                    for (int i = 0; i < dataGridView4.RowCount; i++)
                    {
                        presentacionesIngreso(textBox2.Text, dataGridView4.Rows[i].Cells[0].Value.ToString(), dataGridView4.Rows[i].Cells[1].Value.ToString(), dataGridView4.Rows[i].Cells[2].Value.ToString(), dataGridView4.Rows[i].Cells[3].Value.ToString(), dataGridView4.Rows[i].Cells[4].Value.ToString(), dataGridView4.Rows[i].Cells[5].Value.ToString(), dataGridView4.Rows[i].Cells[6].Value.ToString(), dataGridView4.Rows[i].Cells[7].Value.ToString(), dataGridView4.Rows[i].Cells[8].Value.ToString(), dataGridView4.Rows[i].Cells[9].Value.ToString(), dataGridView4.Rows[i].Cells[10].Value.ToString(), dataGridView4.Rows[i].Cells[11].Value.ToString(), dataGridView4.Rows[i].Cells[12].Value.ToString(), dataGridView4.Rows[i].Cells[13].Value.ToString(), dataGridView4.Rows[i].Cells[14].Value.ToString(), dataGridView4.Rows[i].Cells[15].Value.ToString(),textBox23.Text.Trim());
                    }
                    generalizaMercaderia(precio);
                    timerActions();
                    clenEdicion();
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void button13_Click(object sender, EventArgs e)
        {
            if (Revisa_precios())
            {
                if (textBox2.Text != "" && textBox2.Text != "" && textBox23.Text != "" && comboBox5.Text != "" && dataGridView4.RowCount > 0)
                {
                    try
                    {
                        if (textBox2.BackColor != Color.OrangeRed)
                        {
                            string precio;
                            string unidades;
                            if (textBox25.Text != "")
                            {
                                precio = textBox25.Text;
                            }
                            else
                            {
                                precio = textBox5.Text;
                            }
                            if (textBox26.Text != "")
                            {
                                unidades = textBox26.Text;
                            }
                            else
                            {
                                unidades = textBox4.Text;
                            }

                            editaMercaderia(precio, unidades);
                            textBox3.Focus();
                        }
                        else
                        {
                            MessageBox.Show("EL CODIGO YA EXISTE", "EDICION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("DEBE DE LLENAR TODOS LOS CAMPOS", "EDICION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                e.Handled = true;
                textBox2.Text = textBox3.Text;
                textBox1.Focus();
            }
        }
        private void presentacionesEdicion(string mercaderia, string stock, string descipcion, string cantidadP, string total, string cantM, string p1, string p2, string p3, string p4, string u1, string u2, string u3)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = sql = string.Format("SELECT ID_STOCK FROM MERCADERIA_PRESENTACIONES WHERE ID_STOCK = '{0}';", stock);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    conexion.Close();
                    conexion = ASG_DB.connectionResult();
                    sql = sql = string.Format("CALL UPDATE_STOCK ('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12});", stock, mercaderia, descipcion, cantidadP, total, cantM, p1, p2, p3, p4, u1, u2, u3);
                    cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        //MessageBox.Show("SE ACTUALIZA TIPO VENTA");

                        conexion.Close();
                    }

                }
                else
                {

                    conexion.Close();
                    conexion = ASG_DB.connectionResult();
                    sql = sql = string.Format("CALL INGRESA_STOCK ('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12});", stock, mercaderia, descipcion, cantidadP, total, cantM, p1, p2, p3, p4, u1, u2, u3);
                    cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        // MessageBox.Show("SE CREA TIPO VENTA TIPO VENTA");

                        conexion.Close();
                    }

                }
            }
            catch
          (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dataGridView4_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
           
        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (rolUs == "ADMINISTRADOR")
                {
                    e.SuppressKeyPress = true;
                    if (dataGridView1.RowCount > 0)
                    {
                        var forma = new frm_tipoVenta(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[8].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(), rolUs, dataGridView1.CurrentRow.Cells[4].Value.ToString());
                        forma.ShowDialog();
                    }
                }
            }
        }

        private void textBox8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dataGridView1.RowCount > 0)
                {
                    var forma = new frm_tipoVenta(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[8].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(),rolUs, dataGridView1.CurrentRow.Cells[4].Value.ToString());
                    forma.ShowDialog();
                }
            } else if (e.KeyData == Keys.Down)
                {
                if (dataGridView1.RowCount > 0)
                {
                    if (e.KeyData == Keys.Down)
                    {
                        if (dataGridView1.RowCount > 0)
                        {
                            dataGridView1.Focus();
                            if (dataGridView1.RowCount > 1)
                                this.dataGridView1.CurrentCell = this.dataGridView1[1, dataGridView1.CurrentRow.Index + 1];
                        }
                    }
                }
                }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(textBox6.Text!= "" || textBox28.Text != "" || textBox7.Text != "" || textBox9.Text != "" || textBox10.Text != "" || textBox11.Text != "" || textBox12.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "KARDEX DE PRODUCTOS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    textBox6.Text = "";
                    textBox7.Text = "";
                    textBox10.Text = "";
                    textBox11.Text = "";
                    textBox12.Text = "";
                    textBox28.Text = "";
                    textBox9.Text = "";
                    dataGridView2.Rows.Clear();
                }
            }
        }
        private void cargaComprasKardex(string codigo, string nombreSucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_KARDEX_COMPRA WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}' AND FECHA_COMPRA >= date_sub(curdate(), interval 1 month)  ORDER BY FECHA_COMPRA DESC;", codigo, nombreSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView2.Rows.Add(reader.GetString(0),reader.GetString(1),"COMPRA",reader.GetString(2), reader.GetString(3), string.Format("{0:#,###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), "COMPRA", reader.GetString(2), reader.GetString(3), string.Format("{0:#,###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaTraslados()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView8.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_TRASLADOS ORDER BY FECHA_TRASLADO DESC LIMIT 80;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                    while (reader.Read())
                    {
                        dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                    }
                    setDescripcionTraslado(6);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaFacturas(string codigo, string nombreSucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_KARDEX_FACTURA WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}' AND FECHA_EMISION_FACTURA >= date_sub(curdate(), interval 1 month)  ORDER BY FECHA_EMISION_FACTURA DESC;", codigo, nombreSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), "FACTURA - ENVIO", reader.GetString(2), reader.GetString(3), string.Format("{0:#,###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), "FACTURA - ENVIO", reader.GetString(2), reader.GetString(3), string.Format("{0:#,###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }

        private void dataGridView4_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView4.CurrentCell.ColumnIndex <=1)
            {
                dataGridView4.EditMode = DataGridViewEditMode.EditOnKeystroke;
            }
            else
            {
                dataGridView4.EditMode = DataGridViewEditMode.EditOnEnter;
            }
        }

        private void dataGridView4_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(dataGridView4.RowCount > 0)
            {
                generaBarcode(dataGridView4.CurrentRow.Cells[0].Value.ToString());
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if((textBox6.Text != "") && (textBox28.Text != ""))
            {
                if (comboBox6.Text != "")
                {
                    dataGridView2.Rows.Clear();
                    cargaFacturasTimepo(textBox6.Text, textBox28.Text, comboBox6.Text);
                    cargaComprasKardexTiempo(textBox6.Text, textBox28.Text, comboBox6.Text);
                } else
                {
                    MessageBox.Show("DEBE ELEGIR UN RANGO PARA COTEJAR LOS DATOS", "KARDEX PRODUCTOS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void cargaFacturasTimepo(string codigo, string nombreSucursal, string tiempo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                
                string sql = string.Format("SELECT * FROM VISTA_KARDEX_FACTURA WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}' AND FECHA_EMISION_FACTURA <= date_sub(curdate(), interval {2} month)  ORDER BY FECHA_EMISION_FACTURA DESC;", codigo, nombreSucursal, tiempo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), "FACTURA", reader.GetString(2), string.Format("{0:#,###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), "FACTURA", reader.GetString(2), string.Format("{0:#,###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaComprasKardexTiempo(string codigo, string nombreSucursal, string tiempo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                
                string sql = string.Format("SELECT * FROM VISTA_KARDEX_COMPRA WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}' AND FECHA_COMPRA >= date_sub(curdate(), interval {2} month)  ORDER BY FECHA_COMPRA DESC;", codigo, nombreSucursal, tiempo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), "COMPRA", reader.GetString(2), string.Format("{0:#,###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), "COMPRA", reader.GetString(2), string.Format("{0:#,###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }



        ///      APARTADO DE COTIZACIONES OJO NO PONER NADA ARRIB AQUI EMPOIEZA EL APARTAO DE COTIZACIONES
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox7.Text == "CLIENTES")
            {
                textBox30.Text = "";
                textBox29.Text = "";
                textBox13.Text = "";
                label42.Text = "Codigo Cliente:";
                label41.Text = "Cliente:";
                tipoCotizacion = true;
                label44.Text = getCodeFactura();
            } else if(comboBox7.Text == "PROVEEDORES")
            {
                if (rolUs != "ADMINISTRADOR" & rolUs != "ROOT" & rolUs != "DATA BASE ADMIN")
                {
                    MessageBox.Show("ACTUALMENTE NO POSEE LOS PRIVILEGIOS PARA HACER UNA COTIZACION AL PROVEEDOR!", "COTIZACIONES", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    comboBox7.Text = "CLIENTES";
                }
                else
                {
                    textBox30.Text = "";
                    textBox29.Text = "";
                    textBox13.Text = "";
                    label41.Text = "Proveedor:";
                    label42.Text = "Codigo Proveed.:";
                    label44.Text = getCodeFacturaProveedor();
                    tipoCotizacion = false;
                }
            }
        }
        private void datosCliente(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT NOMBRE_CLIENTE, APELLIDO_CLIENTE, DIRECCION_CLIENTE FROM CLIENTE WHERE ID_CLIENTE  = '{0}';", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    textBox29.Text = reader.GetString(0) + " " + reader.GetString(1);
                    textBox13.Text = reader.GetString(2);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void datosProveedor(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT NOMBRE_PROVEEDOR, DIRECCION_PROVEEDOR FROM PROVEEDOR WHERE ID_PROVEEDOR  = '{0}';", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    textBox29.Text = reader.GetString(0);
                    textBox13.Text = reader.GetString(1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void button21_Click(object sender, EventArgs e)
        {
            if (comboBox7.Text == "CLIENTES")
            {
                var forma = new frm_buscaCliente(nameUs, rolUs, usuario, idSucursal, privilegios);
                forma.ShowDialog();
                if (forma.DialogResult == DialogResult.OK)
                {
                    textBox30.Text = forma.currentCliente.getCliente;
                    datosCliente(forma.currentCliente.getCliente);
                    comboBox8.SelectedIndex = 2;
                    DateTime today = DateTime.Now;
                    textBox31.Text = today.ToString("yyyy-MM-dd");
                    
                    textBox32.Focus();
                }
            }
            else if(comboBox7.Text == "PROVEEDORES")
            {
                var forma = new frm_buscaProveedor(0,nameUs, rolUs, usuario, idSucursal, privilegios);
                forma.ShowDialog();
                if (forma.DialogResult == DialogResult.OK)
                {
                    textBox30.Text = forma.currentProveedor.getProveedor;
                    datosProveedor(forma.currentProveedor.getProveedor);
                    comboBox8.SelectedIndex = 1;
                    DateTime today = DateTime.Now;
                    textBox31.Text = today.ToString("yyyy-MM-dd");
                    
                    textBox32.Focus();
                    
                }
            } else
            {
                MessageBox.Show("NO HA SELECCIONADO TIPO DE COTIZACION", "COTIZACIONES", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void cargaUsuarios()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT NOMBRE_USUARIO FROM USUARIO WHERE ESTADO_TUPLA = TRUE AND ESTADO_USUARIO  = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    comboBox3.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        comboBox3.Items.Add(reader.GetString(0));
                    }
                    comboBox3.Text = nameUs;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void textBox32_Enter(object sender, EventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {
                dataGridView6.CurrentCell = dataGridView6[1, dataGridView6.RowCount - 1];
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
                        textBox32.Text = reader.GetString(0);
                        conexion.Close();
                        return true;
                    }
                    else
                    {
                        sql = string.Format("SELECT ID_STOCK FROM SUBCODIGOS_MERCADERIA WHERE SUBCODIGO = '{0}' AND ESTADO_TUPLA = TRUE;", textBox32.Text.Trim());
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
                                textBox32.Text = reader.GetString(0);
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
        private void obtienePorFacturacion()
        {
            if (existeMercaderia(textBox32.Text.Trim()))
            {
                var forma = new frm_getProducto(textBox32.Text.Trim(), nombreSucursal, rolUs);
                if (forma.ShowDialog() == DialogResult.OK)
                {
                    // AGREGA A FACTURA
                    textBox32.Text = "";
                    getRepetitivo(forma.currentProducto.getStock, forma.currentProducto.getProducto, forma.currentProducto.getDescripcion, forma.currentProducto.getPrice, forma.currentProducto.currentSucursal, forma.currentProducto.getCpp, forma.currentProducto.getPC);
                    localizaCursor();
                    //dataGridView1.Rows.Add(forma.currentProducto.getProducto,forma.currentProducto.getDescripcion,1,forma.currentProducto.getPrice,0,0,forma.currentProducto.currentSucursal, forma.currentProducto.getStock);
                }
            }
        }
        // CURSOR EN DATA GRID VIEW PARA LUEGO DE ESCANEAR
        private void localizaCursor()
        {
            int columna = dataGridView6.CurrentCell.ColumnIndex;
            int fila = dataGridView6.CurrentCell.RowIndex;


            if (columna == 1)
            {
                if ((dataGridView6.RowCount == 1) || (dataGridView6.RowCount == 0))
                {
                    dataGridView6.CurrentCell = dataGridView6[2, fila];
                    dataGridView6.Focus();
                }
                else
                {
                    if (fila + 1 != dataGridView6.RowCount)
                    {
                        dataGridView6.CurrentCell = dataGridView6[2, fila + 1];
                        dataGridView6.Focus();
                    }
                }
            }

        }
        // FUNCION QUE OBTIEN EL TOTAL DE LA FACTURA EN TIEMPO REAL
        private void totalFactura()
        {
            if (dataGridView6.RowCount > 0)
            {
                subtotal = 0;
                for (int i = 0; i < dataGridView6.RowCount; i++)
                {
                    if (dataGridView6.Rows[i].Cells[5].Value != null)
                    {
                        subtotal = subtotal + Convert.ToDouble(dataGridView6.Rows[i].Cells[5].Value.ToString());
                    }
                }
                subtotal = Math.Round(subtotal, 2, MidpointRounding.ToEven);
                label50.Text = string.Format("{0:###,###,###,##0.00##}", subtotal);
                if (textBox33.Text != "" && textBox33.Text != "0")
                {
                    descuento = subtotal * (Convert.ToDouble(textBox33.Text) / 100);
                    total = subtotal - descuento;
                    Math.Round(descuento, 2);
                    Math.Round(subtotal, 2);
                    Math.Round(total, 2);
                    total = Math.Round(total, 2, MidpointRounding.ToEven);
                    label51.Text = string.Format("{0:###,###,###,##0.00##}", total);
                }
                else
                {
                    total = subtotal;
                    Math.Round(subtotal, 2);
                    Math.Round(total, 2);
                    total = Math.Round(total, 2, MidpointRounding.ToEven);
                    label51.Text = string.Format("{0:###,###,###,##0.00##}", subtotal);
                }
            }
            else
            {
                total = 0;
                subtotal = 0;
                descuento = 0;
                label14.Text = "";
                label13.Text = "";
                textBox8.Text = "";
            }
        }
        private void getTotales()
        {
            if (dataGridView6.RowCount > 0)
            {
                for (int i = 0; i < dataGridView6.RowCount; i++)
                {
                    if (dataGridView6.Rows[i].Cells[3].Value != null)
                    {
                        if (Convert.ToDouble(dataGridView6.Rows[i].Cells[4].Value.ToString()) > 0)
                        {
                            double cantidad = Convert.ToDouble(dataGridView6.Rows[i].Cells[2].Value.ToString());
                            double precio = Convert.ToDouble(dataGridView6.Rows[i].Cells[3].Value.ToString());
                            double descuento = (Convert.ToDouble(dataGridView6.CurrentRow.Cells[4].Value.ToString()) / 100) * precio;
                            precio = precio - descuento;
                            double total = cantidad * precio;
                            Math.Round(total, 2);
                            dataGridView6.Rows[i].Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);

                        }
                        else
                        {
                            double cantidad = Convert.ToDouble(dataGridView6.Rows[i].Cells[2].Value.ToString());
                            double precio = Convert.ToDouble(dataGridView6.Rows[i].Cells[3].Value.ToString());
                            double total = cantidad * precio;
                            Math.Round(total, 2);
                            dataGridView6.Rows[i].Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);

                        }
                    }
                }
            }
        }
        private void ventaRapida()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_STOCK_VENTA WHERE ID_STOCK = '{0}' AND NOMBRE_SUCURSAL = '{1}';", textBox32.Text.Trim(), nombreSucursal);
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
                    if (!tipoCotizacion)
                    {
                        precioventa = precioCosto;
                    }
                    string castingDescripcion = decproducto + " - " + descstock + " (" + cpp + ")";
                    getRepetitivo(stockventa, codigoproducto, castingDescripcion, precioventa, sucursal, cpp, precioCosto);
                    getTotales();
                    totalFactura();
                    textBox32.Text = "";
                    textBox32.Focus();
                    conexion.Close();
                }
                else
                {
                    //MessageBox.Show("NO FUNCINO VENGTA DE STOCK");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        // AUMENTA LA CANTIDAD DE LA PRESENTACION SI ES LA MISMA
        private void getRepetitivo(string idStock, string producto, string descripcion, string precio, string sucursal, string cpp, string precioCosto)
        {
            bool state = true;
            if (dataGridView6.RowCount > 0)
            {
                for (int i = 0; i < dataGridView6.RowCount && state; i++)
                {
                    if (dataGridView6.Rows[i].Cells[7].Value != null)
                    {
                        if (dataGridView6.Rows[i].Cells[7].Value.ToString() == idStock)
                        {
                            state = false;
                            dataGridView6.Rows[i].Cells[2].Value = Convert.ToDouble(dataGridView6.Rows[i].Cells[2].Value) + 1;
                        }
                    }
                }
                if (state)
                {
                    dataGridView6.Rows.Add(producto, descripcion, 1, string.Format("{0:###,###,###,##0.00##}", precio), 0, 0, sucursal, idStock, cpp,null,precioCosto);
                    dataGridView6.CurrentCell = dataGridView6[1, dataGridView6.RowCount - 1];
                }
            }
            else
            {
                dataGridView6.Rows.Add(producto, descripcion, 1, string.Format("{0:###,###,###,##0.00##}", precio), 0, 0, sucursal, idStock, cpp,null,precioCosto);
                dataGridView6.CurrentCell = dataGridView6[1, dataGridView6.RowCount - 1];
            }
        }
        private void buscaSubcodigos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_STOCK FROM SUBCODIGOS_MERCADERIA WHERE SUBCODIGO = '{0}' AND ESTADO_TUPLA = TRUE;", textBox32.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    textBox32.Text = reader.GetString(0);
                    ventaRapida();
                }
                else
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
        private void textBox32_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (textBox32.Text != "")
                {
                    if (preferenciaVenta)
                    {
                        obtienePorFacturacion();
                    }
                    else
                    {
                        buscaSubcodigos();  
                    }
                }
                else
                {
                    if (preferenciaVenta)
                    {
                        button22.PerformClick();
                    }
                }
            }
            else if (e.KeyData == Keys.Down)
            {
                if (dataGridView6.RowCount > 0)
                {
                    dataGridView6.Focus();
                }
            }
            else if (e.KeyData == Keys.Right)
            {
                if (dataGridView6.RowCount > 0)
                {
                    dataGridView6.CurrentCell = dataGridView6[2, dataGridView6.RowCount - 1];
                    dataGridView6.Focus();
                }
            }
        }

        private void textBox33_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox33_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button24.PerformClick();
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
          if(comboBox7.Text != "")
            {
                var Tempforma = new frm_buscaEdicion(nombreSucursal, rolUs);
                if (Tempforma.ShowDialog() == DialogResult.OK)
                {
                    textBox11.Text = Tempforma.CurrentMercaderia.getMercaderia;
                    var forma = new frm_getProducto(Tempforma.CurrentMercaderia.getMercaderia, Tempforma.CurrentSucursal.getSucursal, rolUs);
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        // AGREGA A FACTURA
                        if (tipoCotizacion)
                        {
                            getRepetitivo(forma.currentProducto.getStock, forma.currentProducto.getProducto, forma.currentProducto.getDescripcion, forma.currentProducto.getPrice, forma.currentProducto.currentSucursal, forma.currentProducto.getCpp, forma.currentProducto.getPC);
                        }
                        else
                        {


                            getRepetitivo(forma.currentProducto.getStock, forma.currentProducto.getProducto, forma.currentProducto.getDescripcion, forma.currentProducto.getPC, forma.currentProducto.currentSucursal, forma.currentProducto.getCpp, forma.currentProducto.getPC);
                        }
                       // localizaCursor();
                        getTotales();
                        totalFactura();
                        textBox32.Text = "";
                        textBox32.Focus();
                        //dataGridView1.Rows.Add(forma.currentProducto.getProducto,forma.currentProducto.getDescripcion,1,forma.currentProducto.getPrice,0,0,forma.currentProducto.currentSucursal, forma.currentProducto.getStock);
                    }
                }
            } else
            {
                MessageBox.Show("NO HA SELECCIONADO NINGUN TIPO DE COTIZACION", "COTIZACIONES", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView6_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView6.CurrentCell.ColumnIndex <=1)
            {
                dataGridView6.EditMode = DataGridViewEditMode.EditOnKeystroke;
            }
            else
            {
                dataGridView6.EditMode = DataGridViewEditMode.EditOnEnter;
            }
        }

        private void dataGridView6_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView6.CurrentCell.ColumnIndex > 1)
            {
                if (dataGridView6.CurrentRow.Cells[e.ColumnIndex].Value != null)
                {
                    if (dataGridView6.CurrentCell.ColumnIndex == 2)
                    {
                        if (Convert.ToDouble(dataGridView6.CurrentRow.Cells[4].Value.ToString()) > 0)
                        {
                            double cantidad = Convert.ToDouble(dataGridView6.CurrentRow.Cells[e.ColumnIndex].Value.ToString());
                            double precio = Convert.ToDouble(dataGridView6.CurrentRow.Cells[3].Value.ToString());
                            double descuento = (Convert.ToDouble(dataGridView6.CurrentRow.Cells[4].Value.ToString()) / 100) * precio;
                            precio = precio - descuento;
                            double total = cantidad * precio;
                            Math.Round(total, 2);
                            dataGridView6.CurrentRow.Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                            totalFactura();
                        }
                        else
                        {
                            double cantidad = Convert.ToDouble(dataGridView6.CurrentRow.Cells[e.ColumnIndex].Value.ToString());
                            double precio = Convert.ToDouble(dataGridView6.CurrentRow.Cells[3].Value.ToString());
                            double total = cantidad * precio;
                            Math.Round(total, 2);
                            dataGridView6.CurrentRow.Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                            totalFactura();
                        }

                    }
                    else if (dataGridView6.CurrentCell.ColumnIndex == 3)
                    {
                        if (Convert.ToDouble(dataGridView6.CurrentRow.Cells[3].Value.ToString()) < Convert.ToDouble(dataGridView6.CurrentRow.Cells[10].Value.ToString()) && tipoCotizacion == true)
                        {
                            MessageBox.Show("EL PRECIO APLICADO A:" + " " + dataGridView6.CurrentRow.Cells[1].Value.ToString() + " " + "  ES MENOR AL PRECIO COSTO!", "COTIZACIONES", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            var forma = new frm_preciosMercaderia(dataGridView6.CurrentRow.Cells[7].Value.ToString(), dataGridView6.CurrentRow.Cells[6].Value.ToString(), rolUs, tipoCotizacion);
                            if (forma.ShowDialog() == DialogResult.OK)
                            {
                                dataGridView6.CurrentRow.Cells[3].Value = forma.currentPrecio.getPrice;
                                //dataGridView6.CurrentCell = dataGridView6[4, e.RowIndex];
                            }
                        }
                        else
                        {
                            if (Convert.ToDouble(dataGridView6.CurrentRow.Cells[4].Value.ToString()) > 0)
                            {
                                double cantidad = Convert.ToDouble(dataGridView6.CurrentRow.Cells[2].Value.ToString());
                                double precio = Convert.ToDouble(dataGridView6.CurrentRow.Cells[e.ColumnIndex].Value.ToString());
                                double descuento = (Convert.ToDouble(dataGridView6.CurrentRow.Cells[4].Value.ToString()) / 100) * precio;
                                precio = precio - descuento;
                                double total = cantidad * precio;
                                Math.Round(total, 2);
                                dataGridView6.CurrentRow.Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                                totalFactura();

                            }
                            else
                            {

                                
                                    double cantidad = Convert.ToDouble(dataGridView6.CurrentRow.Cells[2].Value.ToString());
                                    double precio = Convert.ToDouble(dataGridView6.CurrentRow.Cells[e.ColumnIndex].Value.ToString());
                                    double total = cantidad * precio;
                                    Math.Round(total, 2);
                                    dataGridView6.CurrentRow.Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                                    totalFactura();
                                

                            }
                        }

                    }
                    else if (dataGridView6.CurrentCell.ColumnIndex == 4)
                    {
                        double cantidad = Convert.ToDouble(dataGridView6.CurrentRow.Cells[2].Value.ToString());
                        double precio = Convert.ToDouble(dataGridView6.CurrentRow.Cells[3].Value.ToString());
                        double descuento = (Convert.ToDouble(dataGridView6.CurrentRow.Cells[e.ColumnIndex].Value.ToString()) / 100) * precio;
                        precio = precio - descuento;
                        double total = cantidad * precio;
                        dataGridView6.CurrentRow.Cells[5].Value = string.Format("{0:###,###,###,##0.00##}", total);
                        totalFactura();
                    }
                }
                else
                {
                    if (e.ColumnIndex > 1)
                        dataGridView6.CurrentCell.Value = "0";
                }
            }
        }

        private void dataGridView6_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {
                if (dataGridView6.CurrentCell.ColumnIndex == 3)
                {
                    var forma = new frm_preciosMercaderia(dataGridView6.CurrentRow.Cells[7].Value.ToString(), dataGridView6.CurrentRow.Cells[6].Value.ToString(), rolUs, tipoCotizacion);
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        dataGridView6.CurrentRow.Cells[3].Value = forma.currentPrecio.getPrice;
                        dataGridView6.CurrentCell = dataGridView6[4, e.RowIndex];
                    }
                }

            }
        }
        // EVENTO QUE CONTROLA LOS ENTERS EN EL DATAGRIDVIEW
        void text_KeyUp(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = false;
            if (e.KeyCode == Keys.Enter)
            {
                int columna = dataGridView6.CurrentCell.ColumnIndex;
                int fila = dataGridView6.CurrentCell.RowIndex;
                if (columna < 6)
                {
                    if ((dataGridView6.RowCount == 1))
                    {

                        dataGridView6.CurrentCell = dataGridView6[columna + 1, fila];
                        // dataGridView1.Focus();

                    }
                    else
                    {
                        if (dataGridView6.CurrentCell.ColumnIndex == 1)
                        {
                            if (dataGridView6.RowCount == fila)
                            {
                                dataGridView6.CurrentCell = dataGridView6[columna + 1, fila];
                                //dataGridView1.Focus();
                            }
                            else if (fila == 0 && dataGridView6.RowCount > fila)
                            {
                                dataGridView6.CurrentCell = dataGridView6[columna + 1, fila];
                            }
                            else
                            {
                                dataGridView6.CurrentCell = dataGridView6[columna + 1, fila - 1];
                            }
                        }
                        else if (columna == 3)
                        {

                            if (dataGridView6.RowCount == fila)
                            {
                                dataGridView6.CurrentCell = dataGridView6[columna + 1, fila];
                                //dataGridView1.Focus();
                            }
                            else if (dataGridView6.CurrentRow.Index == 0 && dataGridView6.RowCount > fila)
                            {
                                dataGridView6.CurrentCell = dataGridView6[columna + 1, fila];
                            }
                            else
                            {
                                dataGridView6.CurrentCell = dataGridView6[columna + 1, fila];
                            }
                        }
                        else if (columna == 2)
                        {

                            if (dataGridView6.RowCount == fila)
                            {
                                dataGridView6.CurrentCell = dataGridView6[columna + 1, fila];
                                //dataGridView1.Focus();
                            }
                            else if (fila == 0 && dataGridView6.RowCount > fila)
                            {
                                dataGridView6.CurrentCell = dataGridView6[columna + 1, fila];
                            }
                            else
                            {
                                dataGridView6.CurrentCell = dataGridView6[columna + 1, fila];
                            }
                        }
                    }
                }
            }
        }
        // BUSCAR DESDE EL APARTADO DE CODIGO EN EL DGV
        private void dgr_KeyPress_SearchObject(object sender, KeyPressEventArgs e)
        {

            if (dataGridView6.CurrentCell.ColumnIndex == 0)
            {
                if ((e.KeyChar == 'B') || (e.KeyChar == 'b'))
                {
                    e.Handled = false;
                    e.KeyChar = (char)13;
                    var Tempforma = new frm_buscaEdicion(nombreSucursal, rolUs);
                    if (Tempforma.ShowDialog() == DialogResult.OK)
                    {
                        textBox32.Text = Tempforma.CurrentMercaderia.getMercaderia;
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
        private void dgr_KeyPress_NumericTester(object sender, KeyPressEventArgs e)
        {

            if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))
            {
                if (dataGridView6.CurrentCell.ColumnIndex > 1)
                {
                    e.Handled = false;
                }
            }
            else
            {
                if (dataGridView6.CurrentCell.ColumnIndex > 1)
                {
                    if ((e.KeyChar == 's') | (e.KeyChar == 'S'))
                    {
                        textBox32.Text = "";
                        textBox32.Focus();
                    }
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }

        }
        // COLOCA PRECIOS DE EL PRODUCTO 
        private void dgr_KeyPress_preciosObject(object sender, KeyPressEventArgs e)
        {

            if (dataGridView6.CurrentCell.ColumnIndex == 3)
            {
                if (dataGridView6.CurrentRow.Cells[6].Value != null && dataGridView6.CurrentRow.Cells[6].Value != null)
                {
                    if ((e.KeyChar == 'p') || (e.KeyChar == 'P'))
                    {
                        e.Handled = false;
                        e.KeyChar = (char)13;
                        var forma = new frm_preciosMercaderia(dataGridView6.CurrentRow.Cells[7].Value.ToString(), dataGridView6.CurrentRow.Cells[6].Value.ToString(), rolUs, tipoCotizacion);
                        if (forma.ShowDialog() == DialogResult.OK)
                        {
                            dataGridView6.CurrentRow.Cells[3].Value = forma.currentPrecio.getPrice;
                        }
                    }
                    else if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))
                    {
                        if (dataGridView6.CurrentCell.ColumnIndex > 1)
                        {
                            e.Handled = false;
                        }
                    }
                    else
                    {
                        if (dataGridView6.CurrentCell.ColumnIndex > 1)
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
        private void dataGridView6_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

            if (dataGridView6.CurrentCell.ColumnIndex > 1)
            {
                if (dataGridView6.CurrentCell.ColumnIndex == 3)
                {

                    e.Control.KeyPress += new KeyPressEventHandler(dgr_KeyPress_preciosObject);

                }
                else
                    e.Control.KeyPress += new KeyPressEventHandler(dgr_KeyPress_NumericTester);
            }
            else if (dataGridView6.CurrentCell.ColumnIndex == 0)
            {

               // e.Control.KeyPress += new KeyPressEventHandler(dgr_KeyPress_SearchObject);
            }

           
        }

        private void textBox33_TextChanged(object sender, EventArgs e)
        {

            if (textBox33.Text != "" && textBox33.Text != "0")
            {
                descuento = subtotal * (Convert.ToDouble(textBox33.Text) / 100);
                Math.Round(descuento, 2);
                Math.Round(subtotal, 2);
                Math.Round(total, 2);
                total = subtotal - descuento;
                label51.Text = string.Format("{0:###,###,###,##0.00##}", total);
            }
            else
            {
                descuento = 0;
            }
        }
        // FUNCION QUE GENERA CODIGO A APRTIR DE QUE NO EXISTA EN EL CODIGO DE LA DB
        private string getCodeFactura()
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
        private string getCodeFacturaProveedor()
        {
            bool codex = true;
            while (codex)
            {
                string temp = createCode(7);
                if (codigoFacturaProveedor(temp))
                {
                    codex = false;
                    return temp;
                }
            }
            return null;
        }
        private bool codigoFacturaProveedor(string code)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_FACTURA FROM COTIZACION_PROVEEDOR WHERE ID_FACTURA =  '{0}';", code);
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
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "COTIZACION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
            return true;
        }
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
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "COTIZACION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
            return true;
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
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            textBox31.Text = dateTimePicker1.Value.ToString("yyyy-MM-dd");
        }
        private bool guardaFacturaProveedor(string descripcion)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL NUEVA_COTIZACION_PROVEEDOR ('{0}','{1}',NOW(),{2},'{3}',{4},{5},{6},'{7}','{8}');", label44.Text.Trim(), comboBox3.Text, idSucursal, textBox30.Text.Trim(), subtotal, descuento, total, descripcion, descripcion);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if ((cmd.ExecuteNonQuery() == 1))
                {
                   // MessageBox.Show("COTIZACION_PROVEEDOR GUARDADA");
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
                string sql = string.Format("CALL NUEVA_COTIZACION_CLIENTE ('{0}','{1}',NOW(),{2},'{3}',{4},{5},{6},'{7}','{8}');", label44.Text.Trim(), comboBox3.Text, idSucursal, textBox30.Text.Trim(), subtotal, descuento, total,descripcion, descripcion);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if ((cmd.ExecuteNonQuery() == 1))
                {
                    //MessageBox.Show("COTIZACION_PROVEEDOR GUARDADA");
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
        private void limpiaCotizacio()
        {
            textBox30.Text = "";
            textBox29.Text = "";
            textBox13.Text = "";
            comboBox7.SelectedIndex = -1;
            comboBox8.SelectedIndex = -1;
            textBox31.Text = "";
            textBox32.Text = "";
            label44.Text = "";
            dataGridView6.Rows.Clear();
            subtotal = 0;
            descuento = 0;
            total = 0;
            textBox33.Text = "";
            label50.Text = "";
            label51.Text = "";
            tipoCotizacion = true;
            existente = false;
            label44.Text = getCodeFactura();
        }
        private void contruyeCotizacionCliente()
        {
            DataSetVenta ds = new DataSetVenta();
            for (int i = 0; i < dataGridView6.RowCount; i++)
            {
                ds.Tables[0].Rows.Add
                    (new object[]
                    {
                                    dataGridView6[2,i].Value.ToString(),
                                    dataGridView6[1,i].Value.ToString(),
                                    dataGridView6[3,i].Value.ToString(),
                                    dataGridView6[5,i].Value.ToString()
                    });
            }
            frm_reporteVenta frm = new frm_reporteVenta();
            CrystalReport9 cr = new CrystalReport9();
            cr.SetDataSource(ds);
            TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["numero"];
            text.Text = label44.Text;
            TextObject textn = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["nombre"];
            textn.Text = textBox29.Text;
            TextObject textd = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["direccion"];
            textd.Text = textBox13.Text;
            TextObject textc = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["codigo"];
            textc.Text = textBox30.Text;
            TextObject textv = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["vendedor"];
            textv.Text = comboBox3.Text;
            TextObject textt = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["terminos"];
            textt.Text = comboBox8.Text;
            TextObject texttotal = (TextObject)cr.ReportDefinition.Sections["Section4"].ReportObjects["total"];
            texttotal.Text = label51.Text;
            TextObject texttotaletra = (TextObject)cr.ReportDefinition.Sections["Section5"].ReportObjects["letters"];
            numberToString nt = new numberToString();
            texttotaletra.Text = nt.enletras(label51.Text) + " Q.";
            frm.crystalReportViewer1.ReportSource = cr;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                var forma = new frm_done();
                forma.ShowDialog();
                limpiaCotizacio();
            }
        }
        private void contruyeCotizacionProveedor()
        {
            DataSetVenta ds = new DataSetVenta();
            for (int i = 0; i < dataGridView6.RowCount; i++)
            {
                ds.Tables[0].Rows.Add
                    (new object[]
                    {
                                    dataGridView6[2,i].Value.ToString(),
                                    dataGridView6[1,i].Value.ToString(),
                                    dataGridView6[3,i].Value.ToString(),
                                    dataGridView6[5,i].Value.ToString()
                    });
            }
            frm_reporteVenta frm = new frm_reporteVenta();
            CrystalReport10 cr = new CrystalReport10();
            cr.SetDataSource(ds);
            TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["numero"];
            text.Text = label44.Text;
            TextObject textn = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["nombre"];
            textn.Text = textBox29.Text;
            TextObject textd = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["direccion"];
            textd.Text = textBox13.Text;
            TextObject textc = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["codigo"];
            textc.Text = textBox30.Text;
            TextObject textv = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["vendedor"];
            textv.Text = comboBox3.Text;
            TextObject textt = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["terminos"];
            textt.Text = comboBox8.Text;
            TextObject texttotal = (TextObject)cr.ReportDefinition.Sections["Section4"].ReportObjects["total"];
            texttotal.Text = label51.Text;
            TextObject texttotaletra = (TextObject)cr.ReportDefinition.Sections["Section5"].ReportObjects["letters"];
            numberToString nt = new numberToString();
            texttotaletra.Text = nt.enletras(label51.Text) + " Q.";
            frm.crystalReportViewer1.ReportSource = cr;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                var forma = new frm_done();
                forma.ShowDialog();
                limpiaCotizacio();
            }
        }
        private bool existeFacturaProveedor(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_FACTURA FROM COTIZACION_PROVEEDOR WHERE ID_FACTURA = '{0}';", codigo);
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
        private bool existeFactura(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_FACTURA FROM FACTURA WHERE ID_FACTURA = '{0}';", codigo);
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
        private void button24_Click(object sender, EventArgs e)
        {
            if(comboBox7.Text != ""  && textBox30.Text != "" && comboBox3.Text != "" && dataGridView6.RowCount > 0) {
                if (comboBox7.Text == "CLIENTES")
                {
                    if (!existeFactura(label44.Text))
                    {
                        if (guardaFactura(comboBox8.Text))
                        {
                            for (int i = 0; i < dataGridView6.RowCount; i++)
                            {
                                double castingTotal = Convert.ToDouble(dataGridView6.Rows[i].Cells[5].Value.ToString());
                                OdbcConnection conexion = ASG_DB.connectionResult();
                                try
                                {
                                    string sql = string.Format("INSERT INTO DET_FACTURA VALUES (NULL,'{0}','{1}','{2}',(SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{3}' LIMIT 1),'{4}',{5},{6},{7},{8},TRUE);", label44.Text, dataGridView6.Rows[i].Cells[0].Value.ToString(), dataGridView6.Rows[i].Cells[7].Value.ToString(), dataGridView6.Rows[i].Cells[6].Value.ToString(), dataGridView6.Rows[i].Cells[1].Value.ToString(), dataGridView6.Rows[i].Cells[2].Value.ToString(), dataGridView6.Rows[i].Cells[4].Value.ToString(), Convert.ToDouble(dataGridView6.Rows[i].Cells[3].Value.ToString()), castingTotal);
                                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                                    if (cmd.ExecuteNonQuery() == 1)
                                    {

                                        //MessageBox.Show("DETALLE GUARDADO CON EXITO - COTIZACION CLIENTE");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
                                conexion.Close();

                            }
                            if (checkBox1.Checked == true)
                            {
                                contruyeCotizacionCliente();
                            }
                            else
                            {
                                DialogResult result;
                                result = MessageBox.Show("¿DESEA IMPRIMIR LA COTIZACION AL CLIENTE?", "COTIZACIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == System.Windows.Forms.DialogResult.Yes)
                                {
                                    contruyeCotizacionCliente();
                                }
                                else
                                {
                                    var forma = new frm_done();
                                    forma.ShowDialog();
                                    limpiaCotizacio();
                                }
                            }

                        }
                    } else
                    {
                        if (updateFactura(comboBox8.Text))
                        {
                            
                            for (int i = 0; i < dataGridView6.RowCount; i++)
                            {
                                double castingTotal = Convert.ToDouble(dataGridView6.Rows[i].Cells[5].Value.ToString());
                                OdbcConnection conexion = ASG_DB.connectionResult();
                                if (dataGridView6.Rows[i].Cells[9].Value != null)
                                {

                                    try
                                    {
                                        string sql = string.Format("SELECT * FROM VISTA_DETALLE_FACTURA WHERE ID_DET_FACTURA = {0};", dataGridView6.Rows[i].Cells[9].Value.ToString());
                                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                                        OdbcDataReader reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            double cantidad = Convert.ToDouble(dataGridView6.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(dataGridView6.Rows[i].Cells[8].Value.ToString());
                                            sql = string.Format("CALL ACTUALIZA_STOCK ({0},'{1}','{2}',{3},{4},{5},{6});", dataGridView6.Rows[i].Cells[9].Value.ToString(), label44.Text, dataGridView6.Rows[i].Cells[1].Value.ToString(), dataGridView6.Rows[i].Cells[2].Value.ToString(), dataGridView6.Rows[i].Cells[4].Value.ToString(), dataGridView6.Rows[i].Cells[3].Value.ToString(), dataGridView6.Rows[i].Cells[5].Value.ToString());
                                            cmd = new OdbcCommand(sql, conexion);
                                            if (cmd.ExecuteNonQuery() == 1)
                                            {

                                                // MessageBox.Show("DIFERENCIAL" + cantidad); 
                                                // MessageBox.Show("DETALLE GUARDADO CON EXITO ACTUALIZADO");
                                            }
                                            else
                                            {
                                                //MessageBox.Show("imposible guardar detalle");
                                            }
                                           
                                        }
                                        else
                                        {
                                            sql = string.Format("INSERT INTO DET_FACTURA VALUES (NULL,'{0}','{1}','{2}',(SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{3}' LIMIT 1),'{4}',{5},{6},{7},{8},TRUE);", label44.Text, dataGridView6.Rows[i].Cells[0].Value.ToString(), dataGridView6.Rows[i].Cells[7].Value.ToString(), dataGridView6.Rows[i].Cells[6].Value.ToString(), dataGridView6.Rows[i].Cells[1].Value.ToString(), dataGridView6.Rows[i].Cells[2].Value.ToString(), dataGridView6.Rows[i].Cells[4].Value.ToString(), Convert.ToDouble(dataGridView6.Rows[i].Cells[3].Value.ToString()), castingTotal);
                                            cmd = new OdbcCommand(sql, conexion);
                                            if (cmd.ExecuteNonQuery() == 1)
                                            {

                                                //MessageBox.Show("DETALLE GUARDADO CON EXITO");
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.ToString());
                                    }
                                    

                                }
                                else
                                {
                                    string sql = string.Format("INSERT INTO DET_FACTURA VALUES (NULL,'{0}','{1}','{2}',(SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{3}' LIMIT 1),'{4}',{5},{6},{7},{8},TRUE);", label44.Text, dataGridView6.Rows[i].Cells[0].Value.ToString(), dataGridView6.Rows[i].Cells[7].Value.ToString(), dataGridView6.Rows[i].Cells[6].Value.ToString(), dataGridView6.Rows[i].Cells[1].Value.ToString(), dataGridView6.Rows[i].Cells[2].Value.ToString(), dataGridView6.Rows[i].Cells[4].Value.ToString(), Convert.ToDouble(dataGridView6.Rows[i].Cells[3].Value.ToString()), castingTotal);
                                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                                    if (cmd.ExecuteNonQuery() == 1)
                                    {

                                        //MessageBox.Show("DETALLE GUARDADO CON EXITO");
                                    }
                                    //MessageBox.Show("gaurdar nuevo detalle");// GUARDAR EL STOCK EN LA FACTURA

                                }
                                conexion.Close();
                            }
                            if (checkBox1.Checked == true)
                            {
                                contruyeCotizacionCliente();
                            }
                            else
                            {
                                DialogResult result;
                                result = MessageBox.Show("¿DESEA IMPRIMIR LA COTIZACION AL CLIENTE?", "COTIZACIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == System.Windows.Forms.DialogResult.Yes)
                                {
                                    contruyeCotizacionCliente();
                                }
                                else
                                {
                                    var forma = new frm_done();
                                    forma.ShowDialog();
                                    limpiaCotizacio();
                                }
                            }

                        }
                    }
                }
                else if (comboBox7.Text == "PROVEEDORES")
                {
                   // MessageBox.Show("proveedores encongtro");
                    if (!existeFacturaProveedor(label44.Text))
                    {
                        if (guardaFacturaProveedor(comboBox8.Text))
                        {
                            for (int i = 0; i < dataGridView6.RowCount; i++)
                            {
                                double castingTotal = Convert.ToDouble(dataGridView6.Rows[i].Cells[5].Value.ToString());
                                OdbcConnection conexion = ASG_DB.connectionResult();
                                try
                                {
                                    string sql = string.Format("INSERT INTO DETALLE_COTIZACION VALUES (NULL,'{0}','{1}','{2}',(SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{3}' LIMIT 1),'{4}',{5},{6},{7},{8},TRUE);", label44.Text, dataGridView6.Rows[i].Cells[0].Value.ToString(), dataGridView6.Rows[i].Cells[7].Value.ToString(), dataGridView6.Rows[i].Cells[6].Value.ToString(), dataGridView6.Rows[i].Cells[1].Value.ToString(), dataGridView6.Rows[i].Cells[2].Value.ToString(), dataGridView6.Rows[i].Cells[4].Value.ToString(), Convert.ToDouble(dataGridView6.Rows[i].Cells[3].Value.ToString()), castingTotal);
                                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                                    if (cmd.ExecuteNonQuery() == 1)
                                    {

                                        // MessageBox.Show("DETALLE GUARDADO CON EXITO - COTIZACION proveedor");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
                                conexion.Close();

                            }
                            if (checkBox1.Checked == true)
                            {
                                contruyeCotizacionProveedor();
                            }
                            else
                            {
                                DialogResult result;
                                result = MessageBox.Show("¿DESEA IMPRIMIR LA COTIZACION AL PROVEEDOR?", "COTIZACIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == System.Windows.Forms.DialogResult.Yes)
                                {
                                    contruyeCotizacionProveedor();
                                }
                                else
                                {
                                    var forma = new frm_done();
                                    forma.ShowDialog();
                                    limpiaCotizacio();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("IMPOSIBLE GURDAR LA COTIZACION AL PROVEEDOR", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    } else
                    {
                       // MessageBox.Show("la factura existe");
                        if (updateFacturaProveedor(comboBox8.Text))
                        {
                           // MessageBox.Show("SE ACTUALIZO ENCABEZADO");
                            for (int i = 0; i < dataGridView6.RowCount; i++)
                            {
                                double castingTotal = Convert.ToDouble(dataGridView6.Rows[i].Cells[5].Value.ToString());
                                OdbcConnection conexion = ASG_DB.connectionResult();
                                if (dataGridView6.Rows[i].Cells[9].Value != null)
                                {

                                    try
                                    {
                                        string sql = string.Format("SELECT * FROM VISTA_DETALLE_PROVEEDOR WHERE ID_DET_FACTURA = {0};", dataGridView6.Rows[i].Cells[9].Value.ToString());
                                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                                        OdbcDataReader reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            double cantidad = Convert.ToDouble(dataGridView6.Rows[i].Cells[2].Value.ToString()) * Convert.ToDouble(dataGridView6.Rows[i].Cells[8].Value.ToString());
                                            sql = string.Format("CALL ACTUALIZA_STOCK_PROVEEDOR ({0},'{1}','{2}',{3},{4},{5},{6});", dataGridView6.Rows[i].Cells[9].Value.ToString(), label44.Text, dataGridView6.Rows[i].Cells[1].Value.ToString(), dataGridView6.Rows[i].Cells[2].Value.ToString(), dataGridView6.Rows[i].Cells[4].Value.ToString(), dataGridView6.Rows[i].Cells[3].Value.ToString(), dataGridView6.Rows[i].Cells[5].Value.ToString());
                                            cmd = new OdbcCommand(sql, conexion);
                                            if (cmd.ExecuteNonQuery() == 1)
                                            {

                                                // MessageBox.Show("DIFERENCIAL" + cantidad); 
                                                // MessageBox.Show("DETALLE GUARDADO CON EXITO ACTUALIZADO");
                                            }
                                            else
                                            {
                                                //MessageBox.Show("imposible guardar detalle");
                                            }

                                        }
                                        else
                                        {
                                            sql = string.Format("INSERT INTO DETALLE_COTIZACION VALUES (NULL,'{0}','{1}','{2}',(SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{3}' LIMIT 1),'{4}',{5},{6},{7},{8},TRUE);", label44.Text, dataGridView6.Rows[i].Cells[0].Value.ToString(), dataGridView6.Rows[i].Cells[7].Value.ToString(), dataGridView6.Rows[i].Cells[6].Value.ToString(), dataGridView6.Rows[i].Cells[1].Value.ToString(), dataGridView6.Rows[i].Cells[2].Value.ToString(), dataGridView6.Rows[i].Cells[4].Value.ToString(), Convert.ToDouble(dataGridView6.Rows[i].Cells[3].Value.ToString()), castingTotal);
                                            cmd = new OdbcCommand(sql, conexion);
                                            if (cmd.ExecuteNonQuery() == 1)
                                            {

                                                //MessageBox.Show("DETALLE GUARDADO CON EXITO");
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.ToString());
                                    }


                                }
                                else
                                {
                                    string sql = string.Format("INSERT INTO DETALLE_COTIZACION VALUES (NULL,'{0}','{1}','{2}',(SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{3}' LIMIT 1),'{4}',{5},{6},{7},{8},TRUE);", label44.Text, dataGridView6.Rows[i].Cells[0].Value.ToString(), dataGridView6.Rows[i].Cells[7].Value.ToString(), dataGridView6.Rows[i].Cells[6].Value.ToString(), dataGridView6.Rows[i].Cells[1].Value.ToString(), dataGridView6.Rows[i].Cells[2].Value.ToString(), dataGridView6.Rows[i].Cells[4].Value.ToString(), Convert.ToDouble(dataGridView6.Rows[i].Cells[3].Value.ToString()), castingTotal);
                                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                                    if (cmd.ExecuteNonQuery() == 1)
                                    {

                                        //MessageBox.Show("DETALLE GUARDADO CON EXITO");
                                    }
                                    //MessageBox.Show("gaurdar nuevo detalle");// GUARDAR EL STOCK EN LA FACTURA

                                }
                                conexion.Close();
                            }
                            if (checkBox1.Checked == true)
                            {
                                contruyeCotizacionCliente();
                            }
                            else
                            {
                                DialogResult result;
                                result = MessageBox.Show("¿DESEA IMPRIMIR LA COTIZACION AL CLIENTE?", "COTIZACIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == System.Windows.Forms.DialogResult.Yes)
                                {
                                    contruyeCotizacionCliente();
                                }
                                else
                                {
                                    var forma = new frm_done();
                                    forma.ShowDialog();
                                    limpiaCotizacio();
                                }
                            }

                        } else
                        {
                            MessageBox.Show("IMPOSIBLE ACTUALIZAR COTIZACION EXISTENTE");
                        }
                    }
                } 
            }
            else
            {
                MessageBox.Show("DEBE DE LLENAR TODOS LOS CAMPOS", "NUEVA COTIZACION", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "COTIZACIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                limpiaCotizacio();
              
            }
        }

        private void dataGridView6_KeyDown(object sender, KeyEventArgs e)
        {
          if(dataGridView6.RowCount > 0)
            {
                if (e.KeyData == Keys.Delete)
                {
                    if (comboBox7.Text == "CLIENTES")
                    {
                        mymenuS.Visible = false;
                        DialogResult result;
                        result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR EL PROUDCTO DE LA COTIZACION?", "COTIZACIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (!existente)
                            {
                                dataGridView6.Rows.RemoveAt(dataGridView6.CurrentRow.Index);
                                totalFactura();
                            }
                            else
                            {
                                if (actualizaDetalle(dataGridView6.CurrentRow.Cells[9].Value.ToString()))
                                {
                                    dataGridView6.Rows.RemoveAt(dataGridView6.CurrentRow.Index);
                                    totalFactura();
                                    timerActions();
                                }
                                else
                                {
                                    MessageBox.Show("EL PRODUCTO NO PUDO SER ELIMINADO DE LA FACTURA", "COTIZACIONES EXISTENTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }

                    }
                    else if (comboBox7.Text == "PROVEEDORES")
                    {
                        mymenuS.Visible = false;
                        DialogResult result;
                        result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR EL PROUDCTO DE LA COTIZACION?", "COTIZACIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (!existente)
                            {
                                dataGridView6.Rows.RemoveAt(dataGridView6.CurrentRow.Index);
                                totalFactura();
                            }
                            else
                            {
                                if (actualizaDetalleProveedor(dataGridView6.CurrentRow.Cells[9].Value.ToString()))
                                {

                                    dataGridView6.Rows.RemoveAt(dataGridView6.CurrentRow.Index);
                                    totalFactura();
                                    timerActions();
                                }
                                else
                                {
                                    MessageBox.Show("EL PRODUCTO NO PUDO SER ELIMINADO DE LA COTIZACION", "COTIZACIONES EXISTENTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }

                    }
                }
            }
        }

        private void dataGridView6_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    mymenuS.Show(dataGridView6, new Point(e.X, e.Y));
                    mymenuS.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_ItemclickedSC);
                    mymenuS.Enabled = true;
                    flagsc = false;
                }
            }
        }
        private void cargaCotizaciones()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView7.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)),reader.GetString(13),reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        //styleDV(this.dataGridView1);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void cargaCotizacionesProveedor()
        {

            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView7.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) , reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)),reader.GetString(13),reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)),reader.GetString(13),reader.GetString(14));
                        //styleDV(this.dataGridView1);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        // APARTADO DE COITZACIONES AQUI EMPIEZA
        private void button30_Click(object sender, EventArgs e)
        {
            cargaCotizaciones();
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox10.Text == "CLIENTES")
            {
                cargaCotizaciones();
            }
            else if (comboBox10.Text == "PROVEEDORES")
            {
                cargaCotizacionesProveedor();
            }
        }
        
        private void button29_Click(object sender, EventArgs e)
        {
            if(dataGridView7.RowCount > 0)
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR LA COTIZACION?", "COTIZACIONES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {

                    if (comboBox10.Text == "CLIENTES")
                    {
                        eliminaCotizacionCliente(dataGridView7.CurrentRow.Cells[0].Value.ToString());
                    }
                    else if (comboBox10.Text == "PROVEEDORES")
                    {
                        eliminaCotizacionProveedor(dataGridView7.CurrentRow.Cells[0].Value.ToString());
                    }
                    button30.PerformClick();
                }
            }
        }
        private void eliminaCotizacionCliente(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE FACTURA SET ESTADO_FACTURA = 1 , ESTADO_TUPLA = FALSE WHERE ID_FACTURA = '{0}';", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //.Show("SE GENERO LA CUENTA CORRECTAMENTE");
                    var forma = new frm_done();
                    forma.ShowDialog();
                }
                else
                {
                    MessageBox.Show("IMPOSIBLE ELIMINAR LA COTIZACION!", "COTIZACIONES", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void eliminaCotizacionProveedor(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE COTIZACION_PROVEEDOR SET ESTADO_FACTURA = 1 WHERE ID_FACTURA = '{0}';",codigo );
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //.Show("SE GENERO LA CUENTA CORRECTAMENTE");
                    var forma = new frm_done();
                    forma.ShowDialog();
                }
                else
                {
                    MessageBox.Show("IMPOSIBLE ELIMINAR LA COTIZACION!", "COTIZACIONES", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox10.Text == "CLIENTES")
            {
                cargaCotizaciones();
            }
            else if (comboBox10.Text == "PROVEEDORES")
            {
                cargaCotizacionesProveedor();
            }
        }
        private void cargaCotizacionesProveedorFecha()
        {

            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView7.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND FECHA_EMISION_FACTURA LIKE  '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, dateTimePicker2.Value.ToString("yyyy-MM-dd"));
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        //styleDV(this.dataGridView1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void cargaCotizacionesFecha()
        {

            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView7.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND FECHA_EMISION_FACTURA LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, dateTimePicker2.Value.ToString("yyyy-MM-dd"));
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        //styleDV(this.dataGridView1);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (comboBox10.Text == "CLIENTES")
            {
                cargaCotizacionesFecha();
            }
            else if (comboBox10.Text == "PROVEEDORES")
            {
                cargaCotizacionesProveedorFecha();
            }
        }

        private void dataGridView7_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView7.RowCount > 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    mymenuSV.Show(dataGridView7, new Point(e.X, e.Y));
                    mymenuSV.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_ItemclickedST);
                    mymenuSV.Enabled = true;
                    flagst = false;
                }
            }
        }

        private void textBox34_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dataGridView7.RowCount > 0)
                {
                   
                }
            }
            else if (e.KeyData == Keys.Down)
            {
                if (dataGridView7.RowCount > 0)
                {
                    if (e.KeyData == Keys.Down)
                    {
                        if (dataGridView7.RowCount > 0)
                        {
                            dataGridView7.Focus();
                            if (dataGridView7.RowCount > 1)
                                this.dataGridView7.CurrentCell = this.dataGridView7[1, dataGridView7.CurrentRow.Index + 1];
                        }
                    }
                }
            }
        }

        private void dataGridView7_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(dataGridView7.RowCount > 0)
            {
                if (comboBox10.Text == "CLIENTES")
                {
                    var forma = new frm_detalleFactura(dataGridView7.CurrentRow.Cells[0].Value.ToString(), dataGridView7.CurrentRow.Cells[8].Value.ToString(), dataGridView7.CurrentRow.Cells[9].Value.ToString(), dataGridView7.CurrentRow.Cells[6].Value.ToString(), dataGridView7.CurrentRow.Cells[2].Value.ToString(), dataGridView7.CurrentRow.Cells[1].Value.ToString(), true, dataGridView7.CurrentRow.Cells[7].Value.ToString(), dataGridView7.CurrentRow.Cells[3].Value.ToString(), dataGridView7.CurrentRow.Cells[11].Value.ToString(), dataGridView7.CurrentRow.Cells[10].Value.ToString(),1, true);
                    forma.ShowDialog();
                } else if(comboBox10.Text == "PROVEEDORES")
                {
                    var forma = new frm_detalleFactura(dataGridView7.CurrentRow.Cells[0].Value.ToString(), dataGridView7.CurrentRow.Cells[8].Value.ToString(), dataGridView7.CurrentRow.Cells[9].Value.ToString(), dataGridView7.CurrentRow.Cells[6].Value.ToString(), dataGridView7.CurrentRow.Cells[2].Value.ToString(), dataGridView7.CurrentRow.Cells[1].Value.ToString(), false, dataGridView7.CurrentRow.Cells[7].Value.ToString(), dataGridView7.CurrentRow.Cells[3].Value.ToString(), dataGridView7.CurrentRow.Cells[11].Value.ToString(), dataGridView7.CurrentRow.Cells[10].Value.ToString(),2, true);
                    forma.ShowDialog();
                }
            }
        }

        private void textBox34_TextChanged(object sender, EventArgs e)
        {
            if (textBox34.Text != "")
            {
                if (comboBox10.Text == "CLIENTES")
                {
                    buscaCotizaciones();
                }
                else if (comboBox10.Text == "PROVEEDORES")
                {
                    BuscaCotizacionesProveedor();
                }
            } else
            {
                if (comboBox10.Text == "CLIENTES")
                {
                    cargaCotizaciones();
                }
                else if (comboBox10.Text == "PROVEEDORES")
                {
                    cargaCotizacionesProveedor();
                }
            }
        }
        private void buscaCotizaciones()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView7.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND ID_FACTURA LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        //styleDV(this.dataGridView1);
                    }
                } else
                {
                    dataGridView7.Rows.Clear();
                    sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND FECHA_EMISION_FACTURA LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                    cmd = new OdbcCommand(sql, conexion);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        while (reader.Read())
                        {
                            dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                            //styleDV(this.dataGridView1);
                        }
                    } else
                    {
                        dataGridView7.Rows.Clear();
                        sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND NOMBRE_CLIENTE LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                            while (reader.Read())
                            {
                                dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                //styleDV(this.dataGridView1);
                            }
                        } else
                        {
                            dataGridView7.Rows.Clear();
                            sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND APELLIDO_CLIENTE LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                while (reader.Read())
                                {
                                    dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                    //styleDV(this.dataGridView1);
                                }
                            } else
                            {
                                dataGridView7.Rows.Clear();
                                sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND ID_CLIENTE LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                    while (reader.Read())
                                    {
                                        dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                        //styleDV(this.dataGridView1);
                                    }
                                } else
                                {
                                    dataGridView7.Rows.Clear();
                                    sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND NOMBRE_USUARIO LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                        while (reader.Read())
                                        {
                                            dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                            //styleDV(this.dataGridView1);
                                        }
                                    } else
                                    {
                                        dataGridView7.Rows.Clear();
                                        sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND ID_CLIENTE LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                                        cmd = new OdbcCommand(sql, conexion);
                                        reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                            while (reader.Read())
                                            {
                                                dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                                //styleDV(this.dataGridView1);
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
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "COTIZACIONES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void BuscaCotizacionesProveedor()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView7.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND ID_FACTURA LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                    while (reader.Read())
                    {
                        dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                        //styleDV(this.dataGridView1);
                    }
                } else
                {
                    dataGridView7.Rows.Clear();
                    sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND FECHA_EMISION_FACTURA LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                    cmd = new OdbcCommand(sql, conexion);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                        while (reader.Read())
                        {
                            dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                            //styleDV(this.dataGridView1);
                        }
                    } else
                    {
                        dataGridView7.Rows.Clear();
                        sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND NOMBRE_PROVEEDOR LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                            while (reader.Read())
                            {
                                dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                //styleDV(this.dataGridView1);
                            }
                        } else
                        {
                            dataGridView7.Rows.Clear();
                            sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND ID_PROVEEDOR LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                while (reader.Read())
                                {
                                    dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                    //styleDV(this.dataGridView1);
                                }
                            } else
                            {
                                dataGridView7.Rows.Clear();
                                sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND NOMBRE_USUARIO LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox9.Text, textBox34.Text.Trim());
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                    while (reader.Read())
                                    {
                                        dataGridView7.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                        //styleDV(this.dataGridView1);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }

        private void dataGridView7_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dataGridView7.RowCount > 0)
                {
                    if (comboBox10.Text == "CLIENTES")
                    {
                        var forma = new frm_detalleFactura(dataGridView7.CurrentRow.Cells[0].Value.ToString(), dataGridView7.CurrentRow.Cells[8].Value.ToString(), dataGridView7.CurrentRow.Cells[9].Value.ToString(), dataGridView7.CurrentRow.Cells[6].Value.ToString(), dataGridView7.CurrentRow.Cells[2].Value.ToString(), dataGridView7.CurrentRow.Cells[1].Value.ToString(), true, dataGridView7.CurrentRow.Cells[7].Value.ToString(), dataGridView7.CurrentRow.Cells[3].Value.ToString(), dataGridView7.CurrentRow.Cells[11].Value.ToString(), dataGridView7.CurrentRow.Cells[10].Value.ToString(),1, true);
                        forma.ShowDialog();
                    }
                    else if (comboBox10.Text == "PROVEEDORES")
                    {
                        var forma = new frm_detalleFactura(dataGridView7.CurrentRow.Cells[0].Value.ToString(), dataGridView7.CurrentRow.Cells[8].Value.ToString(), dataGridView7.CurrentRow.Cells[9].Value.ToString(), dataGridView7.CurrentRow.Cells[6].Value.ToString(), dataGridView7.CurrentRow.Cells[2].Value.ToString(), dataGridView7.CurrentRow.Cells[1].Value.ToString(), false, dataGridView7.CurrentRow.Cells[7].Value.ToString(), dataGridView7.CurrentRow.Cells[3].Value.ToString(), dataGridView7.CurrentRow.Cells[11].Value.ToString(), dataGridView7.CurrentRow.Cells[10].Value.ToString(),2, true);
                        forma.ShowDialog();
                    }
                }
            }
        }

        private void dataGridView5_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView5.CurrentCell.ColumnIndex >1)
            {
                e.Control.KeyPress += new KeyPressEventHandler(dgr_KeyPress_NumericTesterTraslado);
            }
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((textBox6.Text != "") && (textBox28.Text != ""))
            {
                if(comboBox11.Text == "COMPRAS")
                {
                    dataGridView2.Rows.Clear();
                    cargaComprasKardex(textBox6.Text.Trim(), textBox28.Text.Trim());
                } else
                {
                    dataGridView2.Rows.Clear();
                    cargaFacturas(textBox6.Text.Trim(), textBox28.Text.Trim());
                }
            } else
            {
                MessageBox.Show("NO HA SELECCIONADO AUN UN PRODUCTO", "KARDEX PRODUCTOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lineShape2_Click(object sender, EventArgs e)
        {

        }

        private void label54_Click(object sender, EventArgs e)
        {

        }
        private void duplicaDetalle(string codigo)
        {
            if (comboBox7.Text == "CLIENTES")
            {
                dataGridView6.Rows.Clear();
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    string sql = string.Format("SELECT * FROM VISTA_DETALLE_FACTURA WHERE ID_FACTURA  = '{0}';", codigo);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView6.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                        while (reader.Read())
                        {
                            dataGridView6.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                        }
                       
                    }
                    totalFactura();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                conexion.Close();
            }
        }
        private void duplicaDetalleProveedor(string codigo)
        {
           
                dataGridView6.Rows.Clear();
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    string sql = string.Format("SELECT * FROM VISTA_DETALLE_PROVEEDOR WHERE ID_FACTURA = '{0}';", codigo);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView6.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                        while (reader.Read())
                        {
                            dataGridView6.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11));
                        }

                    }
                    totalFactura();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                conexion.Close();
            
           
        }
        private void button33_Click(object sender, EventArgs e)
        {
            
           
        }
        internal class Cotizacion
        {
            public string getTipo { get; set; }
            public string getCodigoCliente { get; set; }
            public string getNombre { get; set; }
            public string getDireccion { get; set; }
            public string getVendedor { get; set; }
            public string getTerminos { get; set; }
            public string getFecha { get; set; }
            public string getNumeroCotizacion { get; set; }
        }
        private bool updateFacturaProveedor(string descripcion)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL ACTUALIZA_FACTURA_PROVEEDOR ('{0}','{1}','{2}',{3},'{4}',{5},{6},{7},'{8}','{9}');", label44.Text.Trim(), comboBox3.Text, textBox31.Text, idSucursal, textBox30.Text.Trim(), subtotal, descuento, total, comboBox8.Text, comboBox8.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if ((cmd.ExecuteNonQuery() == 1) |(cmd.ExecuteNonQuery() == 0))
                {

                    //MessageBox.Show("FACTURA GURDADA DE EXISTENCIA");
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
                string sql = string.Format("CALL ACTUALIZA_FACTURA ('{0}','{1}','{2}',{3},'{4}',{5},{6},{7},'{8}','{9}');", label44.Text.Trim(), comboBox3.Text, textBox31.Text, idSucursal, textBox30.Text.Trim(), subtotal, descuento, total, comboBox8.Text, comboBox8.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if ((cmd.ExecuteNonQuery() == 1) | (cmd.ExecuteNonQuery() == 0))
                {
                   
                    //MessageBox.Show("FACTURA GURDADA DE EXISTENCIA");
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

        private void button35_Click(object sender, EventArgs e)
        {

            
                var forma = new frm_cotizacionesEdicion(true, nombreSucursal);
                if (forma.ShowDialog() == DialogResult.OK)
                {
                    comboBox7.Text = forma.currentCotizacion.getTipo;
                    textBox30.Text = forma.currentCotizacion.getCodigoCliente;
                    textBox29.Text = forma.currentCotizacion.getNombre;
                    textBox13.Text = forma.currentCotizacion.getDireccion;
                    comboBox3.Text = forma.currentCotizacion.getVendedor;
                    comboBox8.Text = forma.currentCotizacion.getTerminos;
                    textBox31.Text = forma.currentCotizacion.getFecha;
                    label44.Text = forma.currentCotizacion.getNumeroCotizacion;
                existente = true;

                if (forma.currentCotizacion.getTipo == "CLIENTES")
                    {
                        duplicaDetalle(label44.Text);
                        label42.Text = "Codigo Cliente:";
                        label41.Text = "Cliente:";
                        tipoCotizacion = true;
                    }
                    else
                    {
                        label41.Text = "Proveedor:";
                        label42.Text = "Codigo Proveed.:";
                        tipoCotizacion = false;
                        duplicaDetalleProveedor(label44.Text);

                    }
                }
            
        }

        private void button34_Click(object sender, EventArgs e)
        {
            existente = false;
            var forma = new frm_cotizacionesEdicion(true, nombreSucursal);
            if (forma.ShowDialog() == DialogResult.OK)
            {
                comboBox7.Text = forma.currentCotizacion.getTipo;
                textBox30.Text = forma.currentCotizacion.getCodigoCliente;
                textBox29.Text = forma.currentCotizacion.getNombre;
                textBox13.Text = forma.currentCotizacion.getDireccion;
                comboBox3.Text = forma.currentCotizacion.getVendedor;
                comboBox8.Text = forma.currentCotizacion.getTerminos;
                textBox31.Text = forma.currentCotizacion.getFecha;
                

                if (forma.currentCotizacion.getTipo == "CLIENTES")
                {
                    duplicaDetalle(forma.currentCotizacion.getNumeroCotizacion);
                    label42.Text = "Codigo Cliente:";
                    label41.Text = "Cliente:";
                    label44.Text = getCodeFactura();
                    tipoCotizacion = true;
                }
                else
                {
                    label41.Text = "Proveedor:";
                    label42.Text = "Codigo Proveed.:";
                    tipoCotizacion = false;
                    label44.Text = getCodeFacturaProveedor();
                    duplicaDetalleProveedor(forma.currentCotizacion.getNumeroCotizacion);
                   
                }
            }

        }

        private void dataGridView6_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView4_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (tipoMercaderia == "1")
            {
                if (e.ColumnIndex == 1)
                {
                    int numColumn = dataGridView4.CurrentCell.ColumnIndex;
                    int numRow = dataGridView4.CurrentCell.RowIndex;
                    var forma = new frm_getPresentacion();

                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        dataGridView4.CurrentRow.Cells[numColumn].Value = forma.CurrentPresentacion.getPresentacion;
                        dataGridView4.CurrentRow.Cells[2].Value = forma.CurrentCantidad.getCantidad;
                        this.dataGridView4.CurrentCell = this.dataGridView4[2, dataGridView4.CurrentRow.Index];
                        getCantidades(2);
                    }
                }
            } else
            {
                if (e.ColumnIndex == 1)
                {
                    int numColumn = dataGridView4.CurrentCell.ColumnIndex;
                    int numRow = dataGridView4.CurrentCell.RowIndex;
                    var forma = new frm_especias();
                    if (forma.ShowDialog() == DialogResult.OK)
                    {
                        dataGridView4.CurrentRow.Cells[numColumn].Value = forma.CurrentPresentacion.getPresentacion;
                        dataGridView4.CurrentRow.Cells[2].Value = forma.CurrentCantidad.getCantidad;
                        this.dataGridView4.CurrentCell = this.dataGridView4[2, dataGridView4.CurrentRow.Index];
                        dataGridView1.Focus();
                        getEspecias(2);

                    }
                }
            }
        }

        private void dataGridView4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                if (dataGridView4.RowCount > 0)
                {
                     DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA PRESENTACION DEL PRODUCTO?", "INGRESO DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        deleteDatagridE();
                    }
                }
            } else if (e.KeyData == Keys.Enter)
            {
                if (tipoMercaderia == "1")
                {
                    if (dataGridView4.CurrentCell.ColumnIndex == 1)
                    {
                        int numColumn = dataGridView4.CurrentCell.ColumnIndex;
                        int numRow = dataGridView4.CurrentCell.RowIndex;
                        var forma = new frm_getPresentacion();

                        if (forma.ShowDialog() == DialogResult.OK)
                        {
                            dataGridView4.CurrentRow.Cells[numColumn].Value = forma.CurrentPresentacion.getPresentacion;
                            dataGridView4.CurrentRow.Cells[2].Value = forma.CurrentCantidad.getCantidad;
                            this.dataGridView4.CurrentCell = this.dataGridView4[2, dataGridView4.CurrentRow.Index];
                            getCantidades(2);
                        }
                    }
                }
                else
                {
                    if (dataGridView4.CurrentCell.ColumnIndex == 1)
                    {
                        int numColumn = dataGridView4.CurrentCell.ColumnIndex;
                        int numRow = dataGridView4.CurrentCell.RowIndex;
                        var forma = new frm_especias();
                        if (forma.ShowDialog() == DialogResult.OK)
                        {
                            dataGridView4.CurrentRow.Cells[numColumn].Value = forma.CurrentPresentacion.getPresentacion;
                            dataGridView4.CurrentRow.Cells[2].Value = forma.CurrentCantidad.getCantidad;
                            this.dataGridView4.CurrentCell = this.dataGridView4[2, dataGridView4.CurrentRow.Index];
                            dataGridView1.Focus();
                            getEspecias(2);

                        }
                    }
                }
            }

        }
        private int getSucursales(string mercaderiaC)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            int temp = 0;
            try
            {
                string sql = string.Format("SELECT COUNT(NOMBRE_SUCURSAL) FROM VISTA_SUCURSALES_PRECIOS WHERE ID_MERCADERIA = '{0}';", mercaderiaC);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    temp = reader.GetInt32(0);
                    return temp;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return temp;
        }
        private void button33_Click_1(object sender, EventArgs e)
        {
           if(getSucursales(textBox2.Text)> 1)
           {
                if (dataGridView4.RowCount > 0)
                {

                    if (Revisa_precios())
                    {
                        if (textBox2.Text != "" && textBox2.Text != "" && textBox23.Text != "" && comboBox5.Text != "" && dataGridView4.RowCount > 0)
                        {
                            try
                            {
                                if (textBox2.BackColor != Color.OrangeRed)
                                {
                                    string precio;
                                    string unidades;
                                    if (textBox25.Text != "")
                                    {
                                        precio = textBox25.Text;
                                    }
                                    else
                                    {
                                        precio = textBox5.Text;
                                    }
                                    if (textBox26.Text != "")
                                    {
                                        unidades = textBox26.Text;
                                    }
                                    else
                                    {
                                        unidades = textBox4.Text;
                                    }

                                    editaMercaderiaSucursal(precio, unidades);
                                    var forma = new frm_cambiosSucursal(textBox3.Text, textBox23.Text);
                                    forma.ShowDialog();
                                }
                                else
                                {
                                    MessageBox.Show("EL CODIGO YA EXISTE", "EDICION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        else
                        {
                            MessageBox.Show("DEBE DE LLENAR TODOS LOS CAMPOS", "EDICION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("SELECCIONE UN PRODUCTO PARA APLICAR PRECIOS EN DISTINTAS SUCURSALES!", "EDICION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
           {
                MessageBox.Show("PARA APLICAR EL PRECIO EL PRODUCTO DEBE DE EXISTIR EN OTRAS SUCURSALES!", "EDICION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView4_ImeModeChanged(object sender, EventArgs e)
        {

        }

        private void button36_Click(object sender, EventArgs e)
        {
            var forma = new frm_trasladoLotes(usuario, idSucursal);
            forma.ShowDialog();
        }

        private void button41_Click(object sender, EventArgs e)
        {
            comboBox12.SelectedIndex = -1;
            comboBox13.SelectedIndex = -1;
            cargaTraslados();
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox13.Text == "RECIBIDO")
            {
                cargaTrasladosEstado(1);
               
            } else
            {
                cargaTrasladosEstado(0);
            }
        }
        private void cargaTrasladosEstado(int estado)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView8.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_TRASLADOS WHERE ESTADO_TRASLADO = {0} ORDER BY FECHA_TRASLADO DESC LIMIT 80;", estado);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {                            
                    dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                    while (reader.Read())
                    {
                        dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                    }
                    setDescripcionTraslado(6);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox12.Text != "")
            {
                cargaTrasladosSucursal(comboBox12.Text);
            }
        }
        private void cargaTrasladosSucursal(string sucursalTraslado)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView8.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_TRASLADOS WHERE TRASLADO_DE = '{0}' ORDER BY FECHA_TRASLADO DESC LIMIT 80;", sucursalTraslado);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                    while (reader.Read())
                    {
                        dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                    }
                    setDescripcionTraslado(6);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView8.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_TRASLADOS WHERE FECHA_TRASLADO LIKE '%{0}%' ORDER BY FECHA_TRASLADO DESC LIMIT 80;", dateTimePicker3.Value.ToString("yyyy-MM-dd"));
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                    while (reader.Read())
                    {
                        dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                    }
                    setDescripcionTraslado(6);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }

        private void dataGridView8_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView8.RowCount > 0)
            {
                if ((e.Button == MouseButtons.Right))
                {
                    mymenuTS.Show(dataGridView8, new Point(e.X, e.Y));
                    mymenuTS.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_ItemclickedT);
                    mymenuTS.Enabled = true;
                    flagT = false;
                }
            }
        }

        private void button40_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR EL TRASLADO?", "TRASLADO DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    string codigoCuenta = getCode();
                    string sql = string.Format("UPDATE TRASLADOS SET ESTADO_TUPLA = FALSE WHERE ID_TRASLADO = {0};", dataGridView8.CurrentRow.Cells[0].Value.ToString());
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        var forma = new frm_done();
                        forma.ShowDialog();
                        button41.PerformClick();
                    }
                    else
                    {
                        MessageBox.Show("NO SE PUDO ELIMINAR EL TRASLADO DE LA MERCADERIA!", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                conexion.Close();
            }
        }
        private void my_menu_ItemclickedT(object sender, ToolStripItemClickedEventArgs e)
        {
            if (flagT == false)
            {
                if (e.ClickedItem.Name == "ColHidden")
                {
                    dataGridView8.Rows[this.dataGridView8.CurrentRow.Index].Visible = false;
                    flagT = true;
                }
                else if (e.ClickedItem.Name == "ColEdit")
                {
                    mymenuTS.Visible = false;
                    button40.PerformClick();
                    flagT = true;
                }
                else if (e.ClickedItem.Name == "ColDelete")
                {
                    flagT = true;
                    mymenuTS.Visible = false;
                    var forma = new frm_detalleTraslado(dataGridView8.CurrentRow.Cells[0].Value.ToString(), dataGridView8.CurrentRow.Cells[2].Value.ToString(), dataGridView8.CurrentRow.Cells[3].Value.ToString(), dataGridView8.CurrentRow.Cells[4].Value.ToString(), dataGridView8.CurrentRow.Cells[1].Value.ToString());
                    forma.ShowDialog();

                }
            }
        }

        private void textBox35_TextChanged(object sender, EventArgs e)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            if (textBox35.Text != "")
            {
                try
                {
                    string sql = string.Format("SELECT * FROM VISTA_TRASLADOS WHERE ID_TRASLADO LIKE '%{0}%' ORDER BY FECHA_TRASLADO DESC;", textBox35.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView8.Rows.Clear();
                        dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                        while (reader.Read())
                        {
                            dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                        }
                        setDescripcionTraslado(6);
                    } else
                    {
                        sql = string.Format("SELECT * FROM VISTA_TRASLADOS WHERE FECHA_TRASLADO LIKE '%{0}%' ORDER BY FECHA_TRASLADO DESC;", textBox35.Text);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView8.Rows.Clear();
                            dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                            while (reader.Read())
                            {
                                dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                            }
                            setDescripcionTraslado(6);
                        } else
                        {
                            sql = string.Format("SELECT * FROM VISTA_TRASLADOS WHERE NOMBRE_USUARIO LIKE '%{0}%' ORDER BY FECHA_TRASLADO DESC;", textBox35.Text);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView8.Rows.Clear();
                                dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                                while (reader.Read())
                                {
                                    dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                                }
                                setDescripcionTraslado(6);
                            } else
                            {
                                sql = string.Format("SELECT * FROM VISTA_TRASLADOS WHERE TRASLADO_DE LIKE '%{0}%' ORDER BY FECHA_TRASLADO DESC;", textBox35.Text);
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView8.Rows.Clear();
                                    dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                                    while (reader.Read())
                                    {
                                        dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                                    }
                                    setDescripcionTraslado(6);
                                } else
                                {
                                    sql = string.Format("SELECT * FROM VISTA_TRASLADOS WHERE TRASLADO_DESTINO LIKE '%{0}%' ORDER BY FECHA_TRASLADO DESC;", textBox35.Text);
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView8.Rows.Clear();
                                        dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                                        while (reader.Read())
                                        {
                                            dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                                        }
                                        setDescripcionTraslado(6);
                                    } else
                                    {
                                        sql = string.Format("SELECT * FROM VISTA_TRASLADOS WHERE NUMERO_PRODUCTOS LIKE '%{0}%' ORDER BY FECHA_TRASLADO DESC;", textBox35.Text);
                                        cmd = new OdbcCommand(sql, conexion);
                                        reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            dataGridView8.Rows.Clear();
                                            dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                                            while (reader.Read())
                                            {
                                                dataGridView8.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6));
                                            }
                                            setDescripcionTraslado(6);
                                        }
                                    }
                                }
                            }
                        }
                    }
                } catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            } else
            {
                button41.PerformClick();
            }
            conexion.Close();
        }

        private void dataGridView8_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(dataGridView8.RowCount > 0)
            {
                var forma = new frm_detalleTraslado(dataGridView8.CurrentRow.Cells[0].Value.ToString(), dataGridView8.CurrentRow.Cells[2].Value.ToString(), dataGridView8.CurrentRow.Cells[3].Value.ToString(), dataGridView8.CurrentRow.Cells[4].Value.ToString(), dataGridView8.CurrentRow.Cells[1].Value.ToString());
                forma.ShowDialog();
            }
        }

        private void dataGridView8_KeyDown(object sender, KeyEventArgs e)
        {
            if(dataGridView8.RowCount > 0)
            {
                button40.PerformClick();
            }
        }
    }
}
