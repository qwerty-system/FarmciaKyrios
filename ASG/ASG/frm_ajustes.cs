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
    public partial class frm_ajustes : Form
    {
        ContextMenuStrip mymenu_sucursal = new ContextMenuStrip();
        bool flag = false;
        string nameUs;
        string rolUs;
        string usuario;
        string idSucursal;
        Point DragCursor;
        Point DragForm;
        bool Dragging;

        public frm_ajustes(string nameUser, string rolUser, string user, int x,string sucursal)
        {
            InitializeComponent();
            usuario = user;
            nameUs = nameUser;
            rolUs = rolUser;
            idSucursal = sucursal;
            label19.Text = label19.Text + "" + nameUs;
            cargaSucursales();
            stripSucursal();
            cargaTipos();
            cargaPresentacion();
            cargaMonedas();
            cargaConfiguracion();
            if (rolUs != "ROOT" && rolUs != "ADMINISTRADOR")
            {
                mymenu_sucursal.Items[2].Enabled = false;
            } 


        }
       
        private void my_menu_Itemclicked_sucursal(object sender, ToolStripItemClickedEventArgs e)
        {
            if (flag== false)
            {
                if (e.ClickedItem.Name == "ColHidden")
                {
                    dataGridView2.Rows[this.dataGridView2.CurrentRow.Index].Visible = false;
                    flag = true;
                }
                else if (e.ClickedItem.Name == "ColEdit")
                {
                    tabControl1.SelectedTab = tabPage4;
                    textBox2.Text = this.dataGridView2.CurrentRow.Cells[0].Value.ToString();
                    getData(textBox2.Text);
                    mymenu_sucursal.Visible = false;
                    cargaSucursales();
                    flag = true;

                }
                else if (e.ClickedItem.Name == "ColDelete")
                {
                    mymenu_sucursal.Visible = false;
                    button3.PerformClick();
                    mymenu_sucursal.Enabled = false;
                    flag = true;

                }
            }
        }
        private void my_menu_Itemclicked_presentacion(object sender, ToolStripItemClickedEventArgs e)
        {
           
        }
        private void my_menu_Itemclicked_venta(object sender, ToolStripItemClickedEventArgs e)
        {
           
        }
        private void stripSucursal()
        {
            mymenu_sucursal.Items.Add("Ocultar Fila");
            mymenu_sucursal.Items[0].Name = "ColHidden";
            mymenu_sucursal.Items.Add("Editar Sucursal");
            mymenu_sucursal.Items[1].Name = "ColEdit";
            mymenu_sucursal.Items.Add("Eliminar Sucursal");
            mymenu_sucursal.Items[2].Name = "ColDelete";
        }
        private void timerActions()
        {
            frm_done frm = new frm_done();
            frm.ShowDialog();
            timer1.Interval = 1500;
            timer1.Enabled = true;
        }
       
        private void cargaPresentacion()
        {
           
        }
        
        private void cargaTipos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT TIPO_SUCURSAL FROM TIPO_SUCURSAL;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    comboBox1.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader.GetString(0));
                    }
                }
                else
                {
                    MessageBox.Show("NO EXISTEN TIPOS DE CLIENTES ALMACENADOS!", "GESTION SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
            private void cargaSucursales()
            {
            dataGridView2.Rows.Clear();
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM  VISTA_SUCURSALES;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        styleDV(this.dataGridView2);
                    }
                }
                else
                {
                    MessageBox.Show("NO EXISTEN SUCURSALES!", "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void cargaSucursales(string tipo)
        {
            dataGridView2.Rows.Clear();
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM  VISTA_SUCURSALES WHERE NAME_EXP_2 = '{0}';",tipo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        styleDV(this.dataGridView2);
                    }
                }
                else
                {
                    MessageBox.Show("NO EXISTEN SUCURSALES!", "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void setDescripcionv(int celda)
        {
           
        }
        private void setDescripcionp(int celda)
        {
           
        }
      
        private void styleDV(DataGridView data)
        {
            data.RowsDefaultCellStyle.BackColor = Color.LightGray;
            data.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (textBox10.Text != "" | textBox10.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "CONFIGURACION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
                this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void frm_ajustes_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_ajustes_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_ajustes_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void button23_Click(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
           
        }
        private void datoGuardado()
        {
            
        }
        private void button21_Click(object sender, EventArgs e)
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private void button24_Click(object sender, EventArgs e)
        {
            
        }
        private void actualizaPresentacion()
        {
          
        }
        private void actualizaVenta()
        {
           
        }
        private void actualizaMarca()
        {
           
        }
        private void dataGridView4_MouseClick(object sender, MouseEventArgs e)
        {
          
        }

        private void button20_Click(object sender, EventArgs e)
        {
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cargaSucursales();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox5.Text == "CENTRAL")
            {
                cargaSucursales("CENTRAL");
            } else if(comboBox5.Text == "SUCURSALES")
            {
                cargaSucursales("SUCURSALES");
            }
        }

        private void dataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView2.RowCount > 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    mymenu_sucursal.Show(dataGridView2, new Point(e.X, e.Y));
                    mymenu_sucursal.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_Itemclicked_sucursal);
                    mymenu_sucursal.Enabled = true;
                    flag = false;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                dataGridView2.Rows.Clear();
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    string sql = string.Format("SELECT * FROM VISTA_SUCURSALES WHERE ID_SUCURSAL LIKE '%{0}%';", textBox1.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        while (reader.Read())
                        {
                            dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                            styleDV(this.dataGridView2);
                        }
                        setSucursal(0);
                    } else
                    {
                        conexion.Close();
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("SELECT * FROM VISTA_SUCURSALES WHERE NAME_EXP_2 LIKE '%{0}%';", textBox1.Text);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                            while (reader.Read())
                            {
                                dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                styleDV(this.dataGridView2);
                            }
                            setSucursal(1);
                        } else
                        {
                            conexion.Close();
                            conexion = ASG_DB.connectionResult();
                            sql = string.Format("SELECT * FROM VISTA_SUCURSALES WHERE NOMBRE_SUCURSAL LIKE '%{0}%';", textBox1.Text);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                while (reader.Read())
                                {
                                    dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                    styleDV(this.dataGridView2);
                                }
                                setSucursal(2);
                            } else
                            {
                                conexion.Close();
                                conexion = ASG_DB.connectionResult();
                                sql = string.Format("SELECT * FROM VISTA_SUCURSALES WHERE DIRECCION_SUCURSAL LIKE '%{0}%';", textBox1.Text);
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                    while (reader.Read())
                                    {
                                        dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                        styleDV(this.dataGridView2);
                                    }
                                    setSucursal(3);
                                } else
                                {
                                    conexion.Close();
                                    conexion = ASG_DB.connectionResult();
                                    sql = string.Format("SELECT * FROM VISTA_SUCURSALES WHERE TELEFONO_SUCURSAL LIKE '%{0}%';", textBox1.Text);
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                        while (reader.Read())
                                        {
                                            dataGridView2.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                            styleDV(this.dataGridView2);
                                        }
                                        setSucursal(4);
                                    }
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "PREFERENCIAS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                conexion.Close();
            }
            else
            {
                cargaSucursales();
            }
        }
        private void setSucursal(int celda)
        {
            for (int i = 0; i <= dataGridView2.RowCount - 1; i++)
            {
                dataGridView2.Rows[i].Cells[celda].Style.ForeColor = Color.White;
                dataGridView2.Rows[i].Cells[celda].Style.BackColor = Color.FromArgb(244, 101, 36);
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox10.Text != "" | textBox11.Text != "" | textBox12.Text != "" | textBox9.Text != "" )
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "GESTION SUCURSALES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    textBox10.Text = "";
                    textBox11.Text = "";
                    textBox12.Text = "";
                    textBox9.Text = "";
                    textBox2.Text = "";
                    comboBox1.Items.Clear();
                    cargaTipos();
                }
            }
        }
        private void getData(string sucursal)
        {
            if (sucursal != "")
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    string sql = string.Format("SELECT * FROM  VISTA_SUCURSALES WHERE ID_SUCURSAL = {0};", sucursal);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textBox10.Text = reader.GetString(0);
                        textBox9.Text = reader.GetString(2);
                        comboBox1.Text = reader.GetString(1);
                        textBox12.Text = reader.GetString(3);
                        textBox11.Text = reader.GetString(4);
                    }
                    else
                    {
                        MessageBox.Show("NO EXISTEN SUCURSALES!", "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                conexion.Close();
            }
            else
            {
                MessageBox.Show("INGRESE CODIGO VALIDO!", "GESTION SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox2.Text = "";
                textBox2.Focus();
            }
        }
        internal class Sucursales
        {
            public string getSucursal { get; set; }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var forma = new frm_buscaSucursal();
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                textBox2.Text = forma.currentSucursal.getSucursal;
                if (textBox2.Text != "")
                {
                    getData(textBox2.Text);
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button2.PerformClick();
            }
        }
        private bool nombreSucursal(string nombre)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT NOMBRE_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{0}';", nombre);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return true;
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox11.Text != "" & textBox10.Text != ""  & textBox12.Text != "" & textBox9.Text != "" & comboBox1.Text != "" )
            {
                if (nombreSucursal(textBox9.Text.Trim()))
                {
                    try
                    {
                        OdbcConnection conexion = ASG_DB.connectionResult();
                        string sql = string.Format("CALL UPDATE_SUCURSAL({0},'{1}','{2}','{3}','{4}');", textBox10.Text, textBox9.Text, textBox12.Text, textBox11.Text, comboBox1.Text);
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            conexion.Close();
                            timerActions();
                            textBox10.Text = "";
                            textBox11.Text = "";
                            textBox12.Text = "";
                            textBox9.Text = "";
                            textBox2.Text = "";
                            comboBox1.Items.Clear();
                            cargaTipos();
                            cargaSucursales();
                        }
                        else
                        {
                            MessageBox.Show("NO SE GUARDARON LOS DATOS!", "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            conexion.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                } else
                {
                    MessageBox.Show("NO SE PUEDE APLICAR EL NOMBRE DE LA SUCURSAL POR QUE YA EXISTE!", "EDICION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("DEBE LLENAR TODOS LOS DATOS!", "GESTION SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.CurrentRow.Cells[0].Value.ToString() != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA BORRAR LA SUCURSAL? - ESTO AFECTARA EL INVENTARIO" + "\n" + "  " + dataGridView2.CurrentRow.Cells[0].Value.ToString() + " - " + dataGridView2.CurrentRow.Cells[1].Value.ToString(), "GESTION SUCURSALES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    try
                    {
                        string sql = string.Format("UPDATE SUCURSAL SET ESTADO_TUPLA = 0, NOMBRE_SUCURSAL = 'ELIMINADO' WHERE ID_SUCURSAL = {0};", dataGridView2.CurrentRow.Cells[0].Value.ToString());
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            timerActions();
                            cargaSucursales();
                        }
                        else
                        {
                            MessageBox.Show("NO SE ENCONTRO LA SUCURSAL!", "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Error);
                           
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
                    cargaSucursales();
                }
            }
            else
            {
                MessageBox.Show("INGRESE UN CODIGO VALIDO!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Red;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Transparent;
        }

        private void frm_ajustes_Load(object sender, EventArgs e)
        {
            checkBox3.Checked = true;
            checkBox3.Enabled = false;
            checkBox7.Checked = true;
            checkBox7.Enabled = false;
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void button29_Click(object sender, EventArgs e)
        {
         
        }
        

        private void dataGridView4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
         
        }

        private void dataGridView3_MouseClick(object sender, MouseEventArgs e)
        {
           
        }

        private void dataGridView5_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void button31_Click(object sender, EventArgs e)
        {
     
        }

        private void button25_Click(object sender, EventArgs e)
        {
            cargaPresentacion();
        }

        private void button10_Click(object sender, EventArgs e)
        {
           
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void button9_Click(object sender, EventArgs e)
        {
          
        }

        private void button19_Click(object sender, EventArgs e)
        {
           
        }

        private void button8_Click(object sender, EventArgs e)
        {
           
        }

        private void button26_Click(object sender, EventArgs e)
        {
          
        }

        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private void button28_Click(object sender, EventArgs e)
        {
          
        }

        private void button27_Click(object sender, EventArgs e)
        {
           
        }

        private void button32_Click(object sender, EventArgs e)
        {
           
        }

        private void frm_ajustes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if(textBox10.Text != "" | textBox10.Text != "")
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "CONFIGURACION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    }
                } else
                this.Close();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                if (tabControl1.SelectedTab == tabPage2)
                {
                    button9.PerformClick();
                } else if (tabControl1.SelectedTab == tabPage4)
                {
                    button7.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.B))
            {
                if (tabControl1.SelectedTab == tabPage4)
                {
                    button2.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.A))
            {
                if (tabControl1.SelectedTab == tabPage1)
                {
                    button5.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.P))
            {
                if (tabControl1.SelectedTab == tabPage2)
                {
                    button10.PerformClick();
                }
            }
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            var forma = new frm_CambioMoneda(idSucursal,usuario);
            forma.ShowDialog();
            
        }
        private void cargaConfiguracion()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM  CONFIGURACION_SISTEMA WHERE IDCONFIGURACION = 111;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    checkBox1.Checked = reader.GetBoolean(2);
                    checkBox2.Checked = reader.GetBoolean(3);
                    checkBox9.Checked = reader.GetBoolean(4);
                    checkBox4.Checked = reader.GetBoolean(5);
                    checkBox6.Checked = reader.GetBoolean(6);
                    checkBox13.Checked = reader.GetBoolean(7);
                    checkBox14.Checked = reader.GetBoolean(8);
                    checkBox15.Checked = reader.GetBoolean(9);
                }
                else
                {
                    MessageBox.Show("NO EXISTEN CONFIGURACIONES!", "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void cargaMonedas()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM  TIPO_CAMBIO;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetString(0) == "MXN")
                    {
                        label30.Text = label30.Text + " | Cambio Actual - Q." + "" + reader.GetDouble(1);
                        checkBox12.Checked = reader.GetBoolean(2);
                    }
                    else if (reader.GetString(0) == "QUETZALES")
                    {
                        label29.Text = label29.Text + " | Cambio Actual - Q." + "" + reader.GetDouble(1);
                        checkBox10.Checked = reader.GetBoolean(2);
                    }
                    else if (reader.GetString(0) == "DOLARES")
                    {
                        label28.Text = label28.Text + " | Cambio Actual - Q." + "" + reader.GetDouble(1);
                        checkBox11.Checked = reader.GetBoolean(2);
                    }
                    while (reader.Read())
                    {
                        if (reader.GetString(0) == "MXN")
                        {
                            label30.Text = label30.Text + " | Cambio Actual - Q." + "" + reader.GetDouble(1);
                            checkBox12.Checked = reader.GetBoolean(2);
                        } else if (reader.GetString(0) == "QUETZALES")
                        {
                            label29.Text = label29.Text + " | Cambio Actual - Q." + "" + reader.GetDouble(1);
                            checkBox10.Checked = reader.GetBoolean(2);
                        }
                        else if (reader.GetString(0) == "DOLARES")
                        {
                            label28.Text = label28.Text + " | Cambio Actual - Q." + "" + reader.GetDouble(1);
                            checkBox11.Checked = reader.GetBoolean(2);
                        }
                    }
                    
                }
                else
                {
                    MessageBox.Show("NO EXISTEN SUCURSALES!", "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = sql = string.Format("CALL UPDATE_CONFIG({0},{1},{2},{3},{4},{5},{6},{7});",111,checkBox1.Checked, checkBox2.Checked,checkBox4.Checked,checkBox6.Checked,checkBox13.Checked,checkBox14.Checked, checkBox15.Checked);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    timerActions();
                    adressUser.ingresaBitacora(idSucursal, usuario, "ACTUALIZACION DE CONFIGURACION","SYSTEM CONFIG");
                } 
            }
            catch
          (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
             if(checkBox1.Checked == true)
            {
                checkBox1.Checked = false;
            } else
            {
                checkBox1.Checked = true;
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox2.Checked = false;
            }
            else
            {
                checkBox2.Checked = true;
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                checkBox4.Checked = false;
            }
            else
            {
                checkBox4.Checked = true;
            }
        }

        private void label21_Click(object sender, EventArgs e)
        {
            if (checkBox6.Checked == true)
            {
                checkBox6.Checked = false;
            }
            else
            {
                checkBox6.Checked = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {
            if (checkBox13.Checked == true)
            {
                checkBox13.Checked = false;
            }
            else
            {
                checkBox13.Checked = true;
            }
        }

        private void label32_Click(object sender, EventArgs e)
        {
            if (checkBox14.Checked == true)
            {
                checkBox14.Checked = false;
            }
            else
            {
                checkBox14.Checked = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(rolUs == "ADMINISTRADOR" | rolUs == "ROOT")
            {
                var forma = new Form1(usuario, idSucursal);
                if(forma.ShowDialog() == DialogResult.OK)
                {
                    button4.PerformClick();
                }
            } else
            {
                MessageBox.Show("NO POSEE LOS PRIVILEGIOS PARA CREAR UNA NUEVA SUCURSAL", "MANTENIMIENTO SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.Text == "CENTRAL")
            {
                MessageBox.Show("TENGA EN CUENTA QUE LAS SUCURSALES CENTRALES PODRAN RECIBIR COMPRAS DIRECTAS!", "MANTENNIMIENTO SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            var forma = new frm_configPrinter();
            forma.ShowDialog();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
    }
}
