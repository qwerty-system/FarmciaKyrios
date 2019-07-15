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
using System.Globalization;

namespace ASG
{
    public partial class frm_clientes : Form
    {
       
        ContextMenuStrip mymenu = new ContextMenuStrip();
        bool flag = false;
        string nameUs;
        string rolUs;
        string usuario;
        string idSucursal;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        bool indexer;
        bool[] privilegios;
        public frm_clientes(string nameUser, string rolUser, string user, string sucursal, bool index, string nit, string nombre, bool[] privilegio)
        {
            InitializeComponent();
            usuario = user;
            nameUs = nameUser;
            rolUs = rolUser;
            this.privilegios = privilegio;
            idSucursal = sucursal;
            cargaDatos();
            stripMenu();
            cargaTipos();
            comboBox1.SelectedIndex = 1;
            pictureBox1.Visible = false;
            indexer = index;
            label19.Text = label19.Text +""+ nameUs;
            if(index)
            {
                tabControl1.SelectedTab = tabPage2;              
            }
            if((rolUs != "ADMINISTRADOR") | (privilegios[5] != true))
            {
                mymenu.Items[2].Enabled = false;
            }
            if(privilegios[6] != true)
            {
                tabControl1.TabPages.Remove(this.tabPage2);
            }
            if (privilegios[5] != true)
            {
                tabControl1.TabPages.Remove(this.tabPage3);
                mymenu.Items[1].Enabled = false;
                button3.Enabled = false;
            }

        }
        Boolean permitir = true;
        public bool solonumerosLimite(int code)
        {
            bool resultado;

            if (code == 46 && textBox6.Text.Contains("."))//se evalua si es punto y si es punto se rebiza si ya existe en el textbox
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
        public bool solonumerosLimiteEditar(int code)
        {
            bool resultado;

            if (code == 46 && textBox13.Text.Contains("."))//se evalua si es punto y si es punto se rebiza si ya existe en el textbox
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
        public bool solonumerosDias(int code)
        {
            bool resultado;
            if (code == 46 && textBox6.Text.Contains("."))//se evalua si es punto y si es punto se rebiza si ya existe en el textbox
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
        public bool solonumerosDiasEditar(int code)
        {
            bool resultado;
            if (code == 46 && textBox17.Text.Contains("."))//se evalua si es punto y si es punto se rebiza si ya existe en el textbox
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
        private void stripMenu()
        {
            mymenu.Items.Add("Ocultar Fila");
            mymenu.Items[0].Name = "ColHidden";
            mymenu.Items.Add("Editar Cliente");
            mymenu.Items[1].Name = "ColEdit";
            mymenu.Items.Add("Eliminar Cliente");
            mymenu.Items[2].Name = "ColDelete";
            mymenu.Items.Add("Ver Balance del Cliente");
            mymenu.Items[3].Name = "ColView";
        }
        private void cargaTipos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT TIPO FROM TIPO_CLIENTE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    comboBox1.Items.Add(reader.GetString(0));
                    comboBox2.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader.GetString(0));
                        comboBox2.Items.Add(reader.GetString(0));
                    }
                }
                else
                {
                    MessageBox.Show("NO EXISTEN TIPOS DE CLIENTES ALMACENADOS!", "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
       
        private void cargaDatos()
        {
            dataGridView1.Rows.Clear();
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_CLIENTES;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                        //styleDV(this.dataGridView1);
                    }
                }
                else
                {
                    MessageBox.Show("NO EXISTEN CLIENTES!", "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox1.Text = "";
                    textBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void frm_clientes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if(textBox2.Text != "" || textBox3.Text != "" || textBox4.Text != "" || textBox10.Text != "" || textBox9.Text != "" || textBox12.Text != "")
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "GESTION CLIENTES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    }
                } else
                this.Close();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.B))
            {
                if (tabControl1.SelectedTab == tabPage3)
                {
                    button8.PerformClick();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                if (tabControl1.SelectedTab == tabPage3)
                {
                    button10.PerformClick();
                } else if (tabControl1.SelectedTab == tabPage2)
                {
                    button2.PerformClick();
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            cargaDatos();
        }
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox5.Text  == "EFECTIVO")
            {
                cargaSeleccion("EFECTIVO");
            } else if (comboBox5.Text == "CREDITO")
            {
                cargaSeleccion("CREDITO");
            }
        }
        private void setDescripcion(int celda)
        {
            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                dataGridView1.Rows[i].Cells[celda].Style.ForeColor = Color.White;
                dataGridView1.Rows[i].Cells[celda].Style.BackColor = Color.FromArgb(207, 136, 65);
            }
        }
        private void cargaSeleccion(string tipo)
        {
            dataGridView1.Rows.Clear();
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE TIPO = '{0}';",tipo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                        //styleDV(this.dataGridView1);
                    }
                    //setDescripcion(4);
                }
                else
                {
                    MessageBox.Show("NO EXISTEN CLIENTES!", "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox1.Text = "";
                    textBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
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
                    textBox14.Text = this.dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    getData(textBox14.Text.Trim());
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
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            if (textBox1.Text != "")
            {
                try
                {
                    string sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE ID_CLIENTE LIKE '%{0}%';", textBox1.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                            //styleDV(this.dataGridView1);
                        }
                        //setDescripcion(0);
                    }
                    else
                    {
                        sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE NOMBRE_CLIENTE LIKE '%{0}%';", textBox1.Text);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                ////styleDV(this.dataGridView1);
                            }
                            //setDescripcion(1);
                        }
                        else
                        {
                            sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE APELLIDO_CLIENTE LIKE '%{0}%';", textBox1.Text);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Clear();
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                    //styleDV(this.dataGridView1);
                                }
                                //setDescripcion(1);
                            }
                            else
                            {
                                sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE TELEFONO_CLIENTE LIKE '%{0}%';", textBox1.Text);
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Clear();
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                        //styleDV(this.dataGridView1);
                                    }
                                    //setDescripcion(3);
                                }
                                else
                                {
                                    sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE TIPO LIKE '%{0}%';", textBox1.Text);
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Clear();
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                        while (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                            //styleDV(this.dataGridView1);
                                        }
                                       // setDescripcion(4);
                                    }
                                    else
                                    {
                                        sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE LIMITE_CREDITO LIKE '%{0}%';", textBox1.Text);
                                        cmd = new OdbcCommand(sql, conexion);
                                        reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            dataGridView1.Rows.Clear();
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                            while (reader.Read())
                                            {
                                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                                //styleDV(this.dataGridView1);
                                            }
                                            //setDescripcion(5);
                                        }
                                        else
                                        {
                                            sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE DIAS_CREDITO LIKE '%{0}%';", textBox1.Text);
                                            cmd = new OdbcCommand(sql, conexion);
                                            reader = cmd.ExecuteReader();
                                            if (reader.Read())
                                            {
                                                dataGridView1.Rows.Clear();
                                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                                while (reader.Read())
                                                {
                                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                                    //styleDV(this.dataGridView1);
                                                }
                                               // setDescripcion(6);
                                            } else
                                            {
                                                sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE DIRECCION_CLIENTE LIKE '%{0}%';", textBox1.Text);
                                                cmd = new OdbcCommand(sql, conexion);
                                                reader = cmd.ExecuteReader();
                                                if (reader.Read())
                                                {
                                                    dataGridView1.Rows.Clear();
                                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                                    while (reader.Read())
                                                    {
                                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                                        //styleDV(this.dataGridView1);
                                                    }
                                                    //setDescripcion(6);
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
            }
            else
            {
                cargaDatos();
            }
            conexion.Close();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "EFECTIVO")
            {
                label9.Enabled = false;
                label10.Enabled = false;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox6.Text = "0";
                textBox7.Text = "0";
            }
            else if (comboBox1.Text == "CREDITO")
            {
                label9.Enabled = true;
                label10.Enabled = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox6.Text = "";
                textBox7.Text = "";

            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT ID_CLIENTE FROM CLIENTE WHERE ID_CLIENTE = '{0}';", textBox2.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {

                        pictureBox1.Image = ASG.Properties.Resources.delete__2_;
                    }
                    else
                    {
                        pictureBox1.Visible = true;
                        pictureBox1.Image = ASG.Properties.Resources.checked2;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            } else
            {
                pictureBox1.Visible = false;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" | textBox3.Text != "" | textBox4.Text != "" | textBox5.Text != "" | textBox6.Text != "" | textBox7.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "GESTION CLIENTES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                    textBox6.Text = "";
                    textBox7.Text = "";
                    textBox16.Text = "";
                    comboBox1.Items.Clear();
                    comboBox2.Items.Clear();
                    cargaTipos();
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
        private void datoGuardado()
        {
            timerActions();
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox16.Text = "";
            comboBox1.Items.Clear();
            cargaDatos();
            cargaTipos();
        }
        private void datoGuardadoedit()
        {
            timerActions();
            textBox10.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
            textBox13.Text = "";
            textBox9.Text = "";
            textBox8.Text = "";
            comboBox2.Items.Clear();
            cargaDatos();
            cargaTipos();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "EFECTIVO")
            {
                guardaEfectivo();
            } else
            {
                guardaCredito();
            }
            if (indexer)
            {
                DialogResult = DialogResult.OK;
            }
        }
        private void guardaEfectivo()
        {
            if (textBox2.Text != "" & textBox3.Text != "" & textBox4.Text != "" & textBox5.Text != "" & textBox16.Text != "" & comboBox1.Text != "" & textBox7.Text != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT ID_CLIENTE FROM CLIENTE WHERE ID_CLIENTE = '{0}';", textBox2.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        conexion.Close();
                        adressUser.ingresaBitacora(idSucursal, usuario, "INGRESO CLIENTE", textBox2.Text.Trim());
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("CALL UPDATE_CLIENTE('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8});", textBox2.Text.Trim(), textBox2.Text.Trim(), textBox3.Text.Trim(), textBox4.Text.Trim(), textBox16.Text.Trim(), comboBox1.Text, textBox5.Text.Trim(), 0, 0);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        datoGuardado();

                    }
                    else
                    {
                        conexion.Close();
                        adressUser.ingresaBitacora(idSucursal, usuario, "INGRESO CLIENTE", textBox2.Text.Trim());
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("CALL INGRESO_CLIENTE('{0}','{1}','{2}','{3}','{4}',{5},{6},'{7}');", textBox2.Text.Trim(), textBox3.Text.Trim(), textBox4.Text.Trim(), textBox16.Text.Trim(), textBox5.Text.Trim(), 0, 0, comboBox1.Text);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        datoGuardado();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("DEBE LLENAR TODOS LOS DATOS!", "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            textBox2.Focus();
            pictureBox1.Visible = false;
        }
        private void guardaCredito()
        {
            if (textBox2.Text != "" & textBox3.Text != "" & textBox4.Text != "" & textBox5.Text != "" & textBox6.Text != "" & comboBox1.Text != "" & textBox7.Text != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT ID_CLIENTE FROM CLIENTE WHERE ID_CLIENTE = '{0}';", textBox2.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        conexion.Close();
                        adressUser.ingresaBitacora(idSucursal, usuario, "INGRESO CLIENTE", textBox2.Text.Trim());
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("CALL UPDATE_CLIENTE('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8});", textBox2.Text.Trim(), textBox2.Text.Trim(), textBox3.Text.Trim(), textBox4.Text.Trim(), textBox16.Text.Trim(), comboBox1.Text, textBox5.Text.Trim(), textBox7.Text.Trim(), textBox6.Text.Trim());
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        datoGuardado();

                    }
                    else
                    {
                        conexion.Close();
                        adressUser.ingresaBitacora(idSucursal, usuario, "INGRESO CLIENTE", textBox2.Text.Trim());
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("CALL INGRESO_CLIENTE('{0}','{1}','{2}','{3}','{4}',{5},{6},'{7}');", textBox2.Text.Trim(), textBox3.Text.Trim(), textBox4.Text.Trim(), textBox16.Text.Trim(), textBox5.Text.Trim(), textBox6.Text.Trim(), textBox7.Text.Trim(), comboBox1.Text);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        datoGuardado();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("DEBE LLENAR TODOS LOS DATOS!", "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            textBox2.Focus();
            pictureBox1.Visible = false;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentRow.Cells[0].Value.ToString() != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA BORRAR EL CLIENTE?" + "\n" + "  " + this.dataGridView1.CurrentRow.Cells[0].Value.ToString() + " - " + this.dataGridView1.CurrentRow.Cells[1].Value.ToString(), "GESTION CLIENTES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    try
                    {
                        string sql = string.Format("CALL ELIMINAR_CLIENTE('{0}');", this.dataGridView1.CurrentRow.Cells[0].Value.ToString());
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            adressUser.ingresaBitacora(idSucursal, usuario, "ELIMINACION CLIENTE", this.dataGridView1.CurrentRow.Cells[0].Value.ToString());
                            timerActions();
                            cargaDatos();
                        }
                        else
                        {
                            MessageBox.Show("NO SE ENCONTRO EL CLIENTE!", "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBox1.Text = "";
                            textBox1.Focus();
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
                    button4.PerformClick();
                }
            }
            else
            {
                MessageBox.Show("INGRESE UN NIT VALIDO!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Focus();
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            var forma = new frm_buscaCliente(nameUs,rolUs,usuario,idSucursal, privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                textBox14.Text = forma.currentCliente.getCliente;
                textBox10.Text = textBox14.Text;
                if (textBox10.Text != "")
                {
                    getData(textBox14.Text);
                    textBox7.Focus();
                }
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox11.Text != "" & textBox10.Text != "" & textBox13.Text != "" & textBox12.Text != "" & textBox9.Text != "" & comboBox2.Text != "" & textBox8.Text != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("CALL UPDATE_CLIENTE('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7});", textBox14.Text, textBox11.Text, textBox10.Text, textBox13.Text, comboBox2.Text, textBox12.Text, textBox8.Text, textBox9.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        conexion.Close();
                        //insertBitacorau();
                        datoGuardadoedit();
                        textBox14.Text = "";
                        textBox14.Focus();
                    }
                    else
                    {
                        MessageBox.Show("NO SE GUARDARON LOS DATOS!", "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        conexion.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("DEBE LLENAR TODOS LOS DATOS!", "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "EFECTIVO")
            {
                label12.Enabled = false;
                label11.Enabled = false;
                textBox9.Enabled = false;
                textBox8.Enabled = false;
                textBox9.Text = "0";
                textBox8.Text = "0";
            }
            else if (comboBox2.Text == "CREDITO")
            {
                label12.Enabled = true;
                label11.Enabled = true;
                textBox9.Enabled = true;
                textBox8.Enabled = true;
                textBox9.Text = "";
                textBox8.Text = "";
            }
        }
        private void frm_clientes_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }
        private void pictureBox4_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox3_Click_1(object sender, EventArgs e)
        {
            if (textBox2.Text != "" || textBox3.Text != "" || textBox4.Text != "" || textBox10.Text != "" || textBox9.Text != "" || textBox12.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "GESTION CLIENTES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
                this.Close();
        }

        private void frm_clientes_MouseUp_1(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_clientes_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox14_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                getData(textBox14.Text);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox10.Text != "" | textBox11.Text != "" | textBox13.Text != "" | textBox12.Text != "" | textBox9.Text != "" | textBox8.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "GESTION CLIENTES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    textBox10.Text = "";
                    textBox11.Text = "";
                    textBox13.Text = "";
                    textBox12.Text = "";
                    textBox9.Text = "";
                    textBox8.Text = "";
                    textBox14.Text = "";
                    comboBox1.Items.Clear();
                    comboBox2.Items.Clear();
                    cargaTipos();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox14_DoubleClick(object sender, EventArgs e)
        {
            var forma = new frm_buscaCliente(nameUs,rolUs,usuario,idSucursal, privilegios);
            forma.Show();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))

            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }*/
            e.Handled = solonumerosLimite(Convert.ToInt32(e.KeyChar)); //llamada a la funcion que evalua que tecla es aceptada
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*if ((e.KeyChar >= '0' & e.KeyChar <= '9')|| (e.KeyChar == 08))

            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }*/
            e.Handled = solonumerosDias(Convert.ToInt32(e.KeyChar));
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox3.Focus();
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox4.Focus();
            }
        }

        private void textBox16_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox5.Focus();
            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
               
                textBox16.Focus();
            }
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                comboBox1.Focus();
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
          /*  if(textBox7.Text != "" && textBox7.Text != "0")
            {
                DateTime today = DateTime.Now;
                DateTime answer = today.AddDays(Convert.ToDouble(textBox7.Text));

            } else
            {
                textBox17.Text = "";
            } */
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                e.Handled = true;
                button9.PerformClick();
                textBox3.Focus();
            }
        }
        private bool codigoRepetido(string code)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CLIENTE FROM CLIENTE WHERE ID_CLIENTE =  '{0}' AND ESTADO_TUPLA = TRUE;", code);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //MessageBox.Show("el codigo ya existe");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        private void button9_Click(object sender, EventArgs e)
        {
            bool codex = true;
            while (codex)
            {
                string temp = createCode(9);
                if (codigoRepetido(temp))
                {
                    textBox2.Text = temp;
                    textBox3.Focus();
                    codex = false;
                }

            }
        }

        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox7.Focus();
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

        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button2.PerformClick();
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

        private void button6_Click_1(object sender, EventArgs e)
        {
            if ((textBox10.Text != "") &&(textBox14.Text != ""))
            {
                MessageBox.Show("SE CAMBIARA EL ID DEL CLIENTE!" , "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                bool codex = true;
                while (codex)
                {
                    string temp = createCode(9);
                    if (codigoRepetido(temp))
                    {
                        textBox10.Text = temp;
                        codex = false;
                    }

                }
            } 
        }
        internal class Cliente
        {
            public string getCliente { get; set; }
        }
        private void getData(string cliente)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_CLIENTE_EDIT WHERE ID_CLIENTE = '{0}';",cliente);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    textBox10.Text = reader.GetString(0);
                    textBox9.Text = reader.GetString(1);
                    textBox12.Text = reader.GetString(2);
                    textBox8.Text = reader.GetString(3);
                    textBox11.Text = reader.GetString(4);
                    comboBox2.Text = reader.GetString(5);
                    textBox13.Text = "" + reader.GetDouble(6);
                    textBox17.Text = "" + reader.GetDouble(7);
                    textBox15.Text = "" + reader.GetDouble(8);
                    textBox18.Text = "" +(Convert.ToDouble(textBox13.Text) - Convert.ToDouble(textBox15.Text));
                    
                }
                else
                {
                    MessageBox.Show("NO EXISTEN CLIENTES!", "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox1.Text = "";
                    textBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text == "EFECTIVO")
            {
                editaEfectivo();
            }
            else
            {
                if(Convert.ToDouble(textBox13.Text) >= Convert.ToDouble(textBox18.Text))
                {
                    double limites = Convert.ToDouble(textBox13.Text.Trim());
                    double disponibles = limites - Convert.ToDouble(textBox18.Text.Trim());
                    editaCredito(limites, disponibles);
                } else
                {
                    MessageBox.Show("EL LIMITE NO PUEDE SER MENOR A LO ADEUDADO POR EL CLIENTE", "EDICION CLIENTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            if (textBox10.Text != "" | textBox9.Text != "" | textBox12.Text != "" | textBox8.Text != "" | textBox11.Text != "" | textBox17.Text != "" | textBox13.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "GESTION CLIENTES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    textBox8.Text = "";
                    textBox9.Text = "";
                    textBox10.Text = "";
                    textBox11.Text = "";
                    textBox12.Text = "";
                    textBox13.Text = "";
                    textBox17.Text = "";
                    textBox14.Text = "";
                    comboBox1.Items.Clear();
                    comboBox2.Items.Clear();
                    cargaTipos();
                }
            }
        }
        private void editaEfectivo()
        {
            if (textBox10.Text != "" & textBox9.Text != "" & textBox12.Text != "" & textBox8.Text != "" & textBox11.Text != "" & comboBox2.Text != "" & textBox13.Text != "" & textBox17.Text != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT ID_CLIENTE FROM CLIENTE WHERE ID_CLIENTE = '{0}';", textBox14.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        conexion.Close();
                        adressUser.ingresaBitacora(idSucursal, usuario, "EDICION CLIENTE",textBox10.Text.Trim());
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("CALL UPDATE_CLIENTE('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8},{9});", textBox14.Text.Trim(), textBox10.Text.Trim(), textBox9.Text.Trim(), textBox12.Text.Trim(), textBox8.Text.Trim(), comboBox2.Text, textBox11.Text.Trim(), 0, 0,0);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        datoGuardados();
                    }    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("DEBE LLENAR TODOS LOS DATOS!", "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            textBox14.Focus();
        }
        private void editaCredito(double limite, double disponible)
        {
            if (textBox10.Text != "" & textBox9.Text != "" & textBox12.Text != "" & textBox8.Text != "" & textBox11.Text != "" & comboBox2.Text != "" & textBox13.Text != "" & textBox17.Text != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT ID_CLIENTE FROM CLIENTE WHERE ID_CLIENTE = '{0}';", textBox14.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        conexion.Close();
                        adressUser.ingresaBitacora(idSucursal, usuario, "EDICION CLIENTE", textBox10.Text.Trim());
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("CALL UPDATE_CLIENTE('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8},{9});", textBox14.Text.Trim(), textBox10.Text.Trim(), textBox9.Text.Trim(), textBox12.Text.Trim(), textBox8.Text.Trim(), comboBox2.Text, textBox11.Text.Trim(), textBox17.Text.Trim(), limite, disponible);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        datoGuardados();
                    }       
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("DEBE LLENAR TODOS LOS DATOS!", "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            
        }
        private void datoGuardados()
        {
            timerActions();
            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
            textBox13.Text = "";
            textBox17.Text = "";
            textBox14.Text = "";
            textBox18.Text = "";
            textBox15.Text = "";
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            cargaTipos();
            cargaDatos();
        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox2.Text == "EFECTIVO")
            {
                label17.Enabled = false;
                label22.Enabled = false;
                textBox13.Enabled = false;
                textBox17.Enabled = false;
                textBox17.Text = "0";
                textBox13.Text = "0";
            }
            else if (comboBox2.Text == "CREDITO")
            {
                label17.Enabled = true;
                label22.Enabled = true;
                textBox13.Enabled = true;
                textBox17.Enabled = true;
                textBox17.Text = "";
                textBox13.Text = "";
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var forma = new frm_buscaCliente(nameUs,rolUs,usuario,idSucursal, privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                
            }
        }

        private void textBox10_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox9.Focus();
            }
        }

        private void textBox9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox12.Focus();
            }
        }

        private void textBox12_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox8.Focus();
            }
        }

        private void textBox8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox11.Focus();
            }
        }

        private void textBox11_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBox13_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox17.Focus();
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

        private void textBox17_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button10.PerformClick();
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

        private void textBox16_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                
                if (textBox16.Text == "") {
                    e.Handled = true;
                    textBox16.Text = "N/A";
                    textBox5.Focus();
                }
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                
                if (textBox5.Text == "")
                {
                    e.Handled = true;
                    textBox5.Text = "N/A";
                    comboBox1.Focus();
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
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

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                var forma = new frm_balanceCliente(dataGridView1.CurrentRow.Cells[0].Value.ToString(),dataGridView1.CurrentRow.Cells[1].Value.ToString(),dataGridView1.CurrentRow.Cells[4].Value.ToString(), dataGridView1.CurrentRow.Cells[5].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString());
                forma.ShowDialog();
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dataGridView1.RowCount > 0)
                {
                    var forma = new frm_balanceCliente(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[4].Value.ToString(), dataGridView1.CurrentRow.Cells[5].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString());
                    forma.ShowDialog();
                }
            }
        }

        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
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

        private void frm_clientes_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
    }
}
