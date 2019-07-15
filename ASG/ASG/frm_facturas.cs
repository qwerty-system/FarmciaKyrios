using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASG
{
    public partial class frm_facturas : Form
    {
        ContextMenuStrip mymenu = new ContextMenuStrip();
        string nameUs;
        string rolUs;
        string usuario;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        bool flag = false;
        string idSucursal;
        string nombreSucursal;
        bool[] privilegios;
        public frm_facturas(string nameUser, string rolUser, string user, string sucursal, string nombre, bool[] privilegio)
        {
            InitializeComponent();
            usuario = user;
            nameUs = nameUser;
            rolUs = rolUser;
            idSucursal = sucursal;
            this.privilegios = privilegio;
            nombreSucursal = nombre;
            stripMenu();
            label19.Text = label19.Text + "" + nameUs;
            comboBox5.SelectedIndex = 1;
            cargaSucursales();
            comboBox1.Text = nombreSucursal;
            cargaDatos();
            cargaUsuarios();
            if(privilegios[3] != true)
            {
                button3.Enabled = false;
                mymenu.Items[1].Enabled = false;              
                tabControl1.TabPages.Remove(this.tabPage3);
            }
            if(rolUs != "ADMINISTRADOR" & rolUs != "ROOT" & rolUs != "DATA BASE ADMIN")
            {
                tabControl1.TabPages.Remove(this.tabPage3);
                mymenu.Items[2].Enabled = false;
            }
        }
        private void stripMenu()
        {
            mymenu.Items.Add("Ocultar Fila");
            mymenu.Items[0].Name = "ColHidden";
            mymenu.Items.Add("Editar Factura");
            mymenu.Items[1].Name = "ColEdit";
            mymenu.Items.Add("Eliminar Factura");
            mymenu.Items[2].Name = "ColDelete";
            mymenu.Items.Add("Ver Detalle de Factura");
            mymenu.Items[3].Name = "ColView";
           
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
                    tabControl1.SelectedTab = tabPage3;
                    //textBox14.Text = this.dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    //getData(textBox14.Text.Trim());
                    mymenu.Visible = false;
                    flag = true;

                }
                else if (e.ClickedItem.Name == "ColDelete")
                {
                    mymenu.Visible = false;
                    mymenu.Enabled = false;
                    flag = true;
                    button3.PerformClick();
                }
                else if (e.ClickedItem.Name == "ColView")
                {
                    mymenu.Visible = false;
                    mymenu.Enabled = false;
                    SendKeys.Send("{ENTER}");
                    flag = true;
                   
                }
            }
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void cargaDatos()
        {
            dataGridView1.Rows.Clear();
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_FACTURAS WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} ORDER BY FECHA_EMISION_FACTURA DESC LIMIT 65;", comboBox1.Text,comboBox5.SelectedIndex);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1),reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)),reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
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
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void frm_facturas_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_facturas_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_facturas_MouseUp(object sender, MouseEventArgs e)
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

        private void frm_facturas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if(textBox14.Text != "" || textBox10.Text!= "")
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "FACTURAS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    }
                }
                this.Close();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.B))
            {
                if (tabControl1.SelectedTab == tabPage3)
                {
                    button8.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.C))
            {
                if (tabControl1.SelectedTab == tabPage3)
                {
                    button6.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                if (tabControl1.SelectedTab == tabPage3)
                {
                    button10.PerformClick();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cargaDatos();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargaDatos();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargaDatos();
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

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

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                var forma = new frm_detalleFactura(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[8].Value.ToString(), dataGridView1.CurrentRow.Cells[9].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(),dataGridView1.CurrentRow.Cells[2].Value.ToString(),dataGridView1.CurrentRow.Cells[1].Value.ToString(),true, dataGridView1.CurrentRow.Cells[7].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[11].Value.ToString(), dataGridView1.CurrentRow.Cells[10].Value.ToString(),0, true);
                forma.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Down)
            {
                if (dataGridView1.RowCount > 0)
                {
                    dataGridView1.Focus();
                    if (dataGridView1.RowCount > 1)
                        this.dataGridView1.CurrentCell = this.dataGridView1[1, dataGridView1.CurrentRow.Index + 1];
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {

                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    string sql = string.Format("SELECT * FROM VISTA_FACTURAS WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND NOMBRE_CLIENTE LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, comboBox5.SelectedIndex, textBox1.Text.Trim());
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                            //styleDV(this.dataGridView1);
                        }
                    }
                    else
                    {
                        conexion.Close();
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("SELECT * FROM VISTA_FACTURAS WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND APELLIDO_CLIENTE LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, comboBox5.SelectedIndex, textBox1.Text.Trim());
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                //styleDV(this.dataGridView1);
                            }
                        }
                        else
                        {
                            conexion.Close();
                            conexion = ASG_DB.connectionResult();
                            sql = string.Format("SELECT * FROM VISTA_FACTURAS WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND TIPO_FACTURA LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, comboBox5.SelectedIndex, textBox1.Text.Trim());
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Clear();
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                    //styleDV(this.dataGridView1);
                                }
                            }
                            else
                            {
                                conexion.Close();
                                conexion = ASG_DB.connectionResult();
                                sql = string.Format("SELECT * FROM VISTA_FACTURAS WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND NOMBRE_USUARIO LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, comboBox5.SelectedIndex, textBox1.Text.Trim());
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Clear();
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                        //styleDV(this.dataGridView1);
                                    }
                                }
                                else
                                {
                                    conexion.Close();
                                    conexion = ASG_DB.connectionResult();
                                    sql = string.Format("SELECT * FROM VISTA_FACTURAS WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND TOTAL_FACTURA LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, comboBox5.SelectedIndex, textBox1.Text.Trim());
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Clear();
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                        while (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                            //styleDV(this.dataGridView1);
                                        }
                                    } else
                                    {
                                        conexion.Close();
                                        conexion = ASG_DB.connectionResult();
                                        sql = string.Format("SELECT * FROM VISTA_FACTURAS WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND ID_CLIENTE LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, comboBox5.SelectedIndex, textBox1.Text.Trim());
                                        cmd = new OdbcCommand(sql, conexion);
                                        reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            dataGridView1.Rows.Clear();
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                            while (reader.Read())
                                            {
                                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                                //styleDV(this.dataGridView1);
                                            }
                                        } else
                                        {
                                            conexion = ASG_DB.connectionResult();
                                            sql = string.Format("SELECT * FROM VISTA_FACTURAS WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND ID_FACTURA LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, comboBox5.SelectedIndex, textBox1.Text.Trim());
                                            cmd = new OdbcCommand(sql, conexion);
                                            reader = cmd.ExecuteReader();
                                            if (reader.Read())
                                            {
                                                dataGridView1.Rows.Clear();
                                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                                while (reader.Read())
                                                {
                                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                                    //styleDV(this.dataGridView1);
                                                }
                                            } else
                                            {
                                                conexion = ASG_DB.connectionResult();
                                                sql = string.Format("SELECT * FROM VISTA_FACTURAS WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND DESCUENTO_FACTURA LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, comboBox5.SelectedIndex, textBox1.Text.Trim());
                                                cmd = new OdbcCommand(sql, conexion);
                                                reader = cmd.ExecuteReader();
                                                if (reader.Read())
                                                {
                                                    dataGridView1.Rows.Clear();
                                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                                    while (reader.Read())
                                                    {
                                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                                        //styleDV(this.dataGridView1);
                                                    }
                                                } else
                                                {
                                                    conexion = ASG_DB.connectionResult();
                                                    sql = string.Format("SELECT * FROM VISTA_FACTURAS_PRODUCTOS WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND DESCRIPCION_DETALLE LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, comboBox5.SelectedIndex, textBox1.Text.Trim());
                                                    cmd = new OdbcCommand(sql, conexion);
                                                    reader = cmd.ExecuteReader();
                                                    if (reader.Read())
                                                    {
                                                        dataGridView1.Rows.Clear();
                                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                                        while (reader.Read())
                                                        {
                                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
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

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                conexion.Close();
            } else
            {
                button4.PerformClick();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_FACTURAS WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND FECHA_EMISION_FACTURA LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, comboBox5.SelectedIndex, dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {

                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
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

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dataGridView1.RowCount > 0)
                {
                    var forma = new frm_detalleFactura(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[8].Value.ToString(), dataGridView1.CurrentRow.Cells[9].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(), dataGridView1.CurrentRow.Cells[2].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString(),true, dataGridView1.CurrentRow.Cells[7].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[11].Value.ToString(), dataGridView1.CurrentRow.Cells[10].Value.ToString(),0, true);
                    forma.ShowDialog();
                }
            }
        }
        private void timerActions()
        {
            frm_done frm = new frm_done();
            frm.ShowDialog();
            timer1.Interval = 1500;
            timer1.Enabled = true;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR LA FACTURA? TOME EN CUENTA QUE LOS PRODUCTOS Y EL EFECTIVO NO SERAN TRASLADADOS", "GESTION FACTURAS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    if (existeCuenta(dataGridView1.CurrentRow.Cells[0].Value.ToString()))
                    {
                        MessageBox.Show("NO SE PUEDE ELIMINAR LA FACTURA DEBIDO A QUE ESTA ALIDADA A UNA CUENTA POR COBRAR", "FACTURAS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    } else
                    {
                        AnulaFactura();
                    }
                }
                
            }
        }
        private bool existeCuenta(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_FACTURA FROM CUENTA_POR_COBRAR_CLIENTE WHERE ESTADO_CUENTA = TRUE AND ESTADO_TUPLA = TRUE AND ID_FACTURA = '{0}';", codigo);
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
        private void AnulaFactura()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE FACTURA SET ESTADO_TUPLA = FALSE WHERE ID_FACTURA = '{0}';", dataGridView1.CurrentRow.Cells[0].Value.ToString());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    timerActions();
                    button4.PerformClick();
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
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }
        internal class factrua
        {
            public string codigoFactura { get; set; }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var forma = new frm_editaVenta(nombreSucursal);
             if(forma.ShowDialog() == DialogResult.OK)
            {
                textBox14.Text = forma.currentFactura.codigoFactura;
                getData(forma.currentFactura.codigoFactura);
            }
        }
        private void getData(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_FACTURAS WHERE ID_FACTURA = '{0}';", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    textBox10.Text = reader.GetString(0);
                    textBox3.Text = reader.GetString(1);
                    textBox9.Text = reader.GetString(2);
                    textBox12.Text = reader.GetString(3);
                    comboBox3.Text = reader.GetString(8);
                    textBox2.Text = reader.GetString(13);                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
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
                    comboBox3.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        //comboBox5.Items.Add(reader.GetString(0));
                        comboBox3.Items.Add(reader.GetString(0));
                    }
                    //comboBox5.SelectedIndex = 0;

                }
                else
                {
                    comboBox3.Items.Add("NO EXISTEN VENDEDORES");
                    comboBox3.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            if(textBox14.Text == "")
            {
                textBox10.Text = "";
                textBox3.Text = "";
                textBox9.Text = "";
                textBox12.Text = "";
                comboBox3.SelectedIndex = -1;
                textBox2.Text = "";
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value <= DateTime.Today)
            {
                textBox3.Text = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            }  else
            {
                MessageBox.Show("LA FECHA NO PUEDE SER MAYOR", "GESTION FACTURAS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var forma = new frm_buscaCliente(nameUs, rolUs, usuario, idSucursal, privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                textBox2.Text = forma.currentCliente.getCliente;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    
                    string sql = string.Format("SELECT * FROM VISTA_CLIENTE_VENTA WHERE ID_CLIENTE = '{0}';", textBox2.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textBox2.Text = reader.GetString(0);
                        textBox9.Text = reader.GetString(1);
                        textBox12.Text = reader.GetString(2);
                    }
                    else
                    {
                        textBox2.Text = "";
                        textBox9.Text = "";
                        textBox12.Text = "";
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
                textBox2.Text = "";
                textBox9.Text = "";
                textBox12.Text = "";
            }
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox10.Text != "" || textBox3.Text != "" || textBox9.Text != "" || textBox12.Text != "" || textBox2.Text != "")
            {
                DialogResult result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "TRASLADO DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    textBox10.Text = "";
                    textBox3.Text = "";
                    textBox9.Text = "";
                    textBox12.Text = "";
                    comboBox3.SelectedIndex = -1;
                    textBox2.Text = "";
                }
            }
        }
        
        private void button10_Click(object sender, EventArgs e)
        {
            if (textBox10.Text != "" && textBox3.Text != "" && textBox9.Text != "" && textBox12.Text != "" && textBox2.Text != "")
            {
                DialogResult result = MessageBox.Show("¿ESTA SEGURO QUE DESEA GUARDAR LOS CAMBIOS?", "TRASLADO DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    try
                    {
                        string sql = string.Format("CALL EDITA_MINIMO_FACTURA ('{0}','{1}','{2}','{3}');", textBox10.Text.Trim(),textBox3.Text,textBox2.Text,comboBox3.Text);
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            timerActions();
                            textBox10.Text = "";
                            textBox3.Text = "";
                            textBox9.Text = "";
                            textBox12.Text = "";
                            textBox14.Text = "";
                            comboBox3.SelectedIndex = -1;
                            textBox2.Text = "";
                            button4.PerformClick();
                        }
                        else
                        {
                            MessageBox.Show("IIMPOSIBLE GUARDAR CAMBIOS EN LA FACTURA!", "GESTION FACTURAS", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    } catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    conexion.Close();
                }
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}
