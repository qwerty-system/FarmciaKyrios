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
    public partial class frm_proveedores : Form
    {
        ContextMenuStrip mymenu = new ContextMenuStrip();
        bool flag = false;
        string nameUs;
        string rolUs;
        string usuario;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        int estado = 0;
        string proveedor;
        string idSucursal;
        bool index;
        bool[] privilegios;
        public frm_proveedores(string nameUser, string rolUser, string user, int x, string sucursal, bool indexer, bool[] privilegio)
        {
            InitializeComponent();
            usuario = user;
            nameUs = nameUser;
            rolUs = rolUser;
            index = indexer;
            this.privilegios = privilegio;
            idSucursal = sucursal;
            stripMenu();
            label19.Text = label19.Text + "" + nameUs;
            cargaDatos();
            estado = x;
            pictureBox1.Visible = false;
            if(index)
                tabControl1.SelectedTab = tabPage2;
            if((rolUs != "ADMINISTRADOR")| (privilegios[8] != true))
            {
                mymenu.Items[2].Enabled = false;
                
            }
            if(privilegios[8] != true)
            {
                tabControl1.TabPages.Remove(this.tabPage3);
                button3.Enabled = false;
            }
            if (privilegios[7] != true)
            {
                tabControl1.TabPages.Remove(this.tabPage2);
                mymenu.Items[1].Enabled = false;
            }
        }
        internal class Proveedor
        {
            public string getProveedor{ get; set; }
        }
        private void stripMenu()
        {
            mymenu.Items.Add("Ocultar Fila");
            mymenu.Items[0].Name = "ColHidden";
            mymenu.Items.Add("Editar Proveedor");
            mymenu.Items[1].Name = "ColEdit";
            mymenu.Items.Add("Eliminar Proveedor");
            mymenu.Items[2].Name = "ColDelete";
          
        }
        private void timerActions()
        {
            frm_done frm = new frm_done();
            frm.ShowDialog();
            timer1.Interval = 1500;
            timer1.Enabled = true;
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
                    textBox9.Text = this.dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    getData(textBox9.Text);
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
                    flag = true;
                   
                }
            }
        }
        private void setDescripcion(int celda)
        {
            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                dataGridView1.Rows[i].Cells[celda].Style.ForeColor = Color.White;
                dataGridView1.Rows[i].Cells[celda].Style.BackColor = Color.FromArgb(244, 101, 36);
            }
        }
       
        private void cargaDatos()
        {
            dataGridView1.Rows.Clear();
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE NOMBRE_PROVEEDOR <> 'N/A';");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        //styleDV(this.dataGridView1);
                    }
                }
                else
                {
                    MessageBox.Show("NO EXISTEN PROVEEDORES!", "GESTION PROVEEDORES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            if (textBox2.Text != "" || textBox7.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "GESTION PROVEEDORES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

        private void frm_proveedores_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_proveedores_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_proveedores_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cargaDatos();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            if (textBox1.Text != "")
            {
                dataGridView1.Rows.Clear();
                try
                {
                    string sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE ID_PROVEEDOR LIKE '%{0}%' AND NOMBRE_PROVEEDOR <> 'N/A';", textBox1.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {

                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                            //styleDV(this.dataGridView1);
                        }
                        //setDescripcion(0);
                    }
                    else
                    {
                        sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE NOMBRE_PROVEEDOR LIKE '%{0}%'  AND NOMBRE_PROVEEDOR <> 'N/A'; ", textBox1.Text);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                //styleDV(this.dataGridView1);
                            }
                            //setDescripcion(1);
                        }
                        else
                        {
                            sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE EMAIL_PROVEEDOR LIKE '%{0}%'  AND NOMBRE_PROVEEDOR <> 'N/A'; ", textBox1.Text);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                    //styleDV(this.dataGridView1);
                                }
                               // setDescripcion(2);
                            }
                            else
                            {
                                sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE DIRECCION_PROVEEDOR LIKE '%{0}%'  AND NOMBRE_PROVEEDOR <> 'N/A'; ", textBox1.Text);
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                        //styleDV(this.dataGridView1);
                                    }
                                    //setDescripcion(3);
                                }
                                else
                                {
                                    sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE TELEFONO_PROVEEDOR LIKE '%{0}%'  AND NOMBRE_PROVEEDOR <> 'N/A'; ", textBox1.Text);
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                        while (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                            //styleDV(this.dataGridView1);
                                        }
                                        //setDescripcion(4);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION PROVEEDORES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentRow.Cells[0].Value.ToString() != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA BORRAR EL PROVEEDOR?" + "\n" + "  " + this.dataGridView1.CurrentRow.Cells[0].Value.ToString() + " - " + this.dataGridView1.CurrentRow.Cells[1].Value.ToString(), "GESTION PROVEEDORES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    try
                    {
                        string sql = string.Format("CALL ELIMINAR_PROVEEDOR('{0}');", this.dataGridView1.CurrentRow.Cells[0].Value.ToString());
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            adressUser.ingresaBitacora(idSucursal, usuario, "ELIMINACION PROVEEDOR", this.dataGridView1.CurrentRow.Cells[0].Value.ToString());
                            timerActions();
                            cargaDatos();
                        }
                        else
                        {
                            MessageBox.Show("NO SE ENCONTRO EL PROVEEDOR!", "GESTION PROVEEDORES", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("INGRESE CODIGO VALIDO!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Focus();
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }
        private void datoGuardado()
        {
            timerActions();
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox13.Text = "";
            textBox12.Text = "";
            textBox11.Text = "";
            textBox10.Text = "";
            textBox9.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            cargaDatos();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" & textBox3.Text != "" & textBox4.Text != "" & textBox5.Text != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = sql = string.Format("CALL INGRESO_PROVEEDOR('{0}','{1}','{2}','{3}','{4}');", textBox2.Text.Trim(), textBox6.Text.Trim(), textBox3.Text.Trim(), textBox4.Text.Trim(), textBox5.Text.Trim());
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        adressUser.ingresaBitacora(idSucursal, usuario, "INGRESO PROVEEDOR",textBox2.Text.Trim());
                        conexion.Close();
                        datoGuardado();
                        if (index)
                            DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("NO SE GUARDARON LOS DATOS!", "GESTION PROVEEDORES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                MessageBox.Show("DEBE LLENAR TODOS LOS DATOS!", "GESTION PROVEEDORES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            textBox2.Focus();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT ID_PROVEEDOR FROM PROVEEDOR WHERE ID_PROVEEDOR = '{0}' AND ESTADO_TUPLA = TRUE;", textBox2.Text);
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

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox10.Text != "" | textBox13.Text != "" | textBox12.Text != "" | textBox11.Text != "" | textBox7.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "GESTION PROVEEDORES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    textBox10.Text = "";
                    textBox13.Text = "";
                    textBox12.Text = "";
                    textBox11.Text = "";
                    textBox9.Text = "";
                    textBox6.Text = "";
                    textBox7.Text = "";
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" | textBox3.Text != "" | textBox4.Text != "" | textBox5.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "GESTION PROVEEDORES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var forma = new frm_buscaProveedor(0, nameUs, rolUs, usuario, idSucursal, privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                textBox9.Text = forma.currentProveedor.getProveedor;
                textBox7.Text = textBox9.Text;
                if (textBox7.Text != "")
                {
                    getData(textBox9.Text);
                }
            }

        }
        private void getData(string code)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            if (textBox9.Text != "")
            {
                try
                {
                    string sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE ID_PROVEEDOR = '{0}';", code);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textBox7.Text = reader.GetString(0);
                        textBox10.Text = reader.GetString(1);
                        textBox13.Text = reader.GetString(3);
                        textBox12.Text = reader.GetString(4);
                        textBox11.Text = reader.GetString(2);
                        
                    }
                    else
                    {
                        MessageBox.Show("EL PROVEEDOR NO EXISTE!", "GESTION PROVEEDORES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        textBox9.Text = "";
                        textBox9.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION PROVEEDORES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("INGRESE CODIGO VALIDO!", "GESTION PROVEEDORES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox9.Text = "";
                textBox9.Focus();
            }
            conexion.Close();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox10.Text != "" & textBox13.Text != "" & textBox12.Text != "" & textBox11.Text != "" & textBox9.Text != "" & textBox7.Text != "" & textBox9.Text != "")
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = sql = string.Format("CALL UPDATE_PROVEEDOR('{0}','{1}','{2}','{3}','{4}','{5}');", textBox9.Text.Trim(), textBox7.Text.Trim(),textBox10.Text.Trim(), textBox11.Text.Trim(), textBox13.Text.Trim(), textBox12.Text.Trim());
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        adressUser.ingresaBitacora(idSucursal, usuario, "EDICION PROVEEDOR", textBox7.Text.Trim());
                        conexion.Close();
                        datoGuardado();
                    }
                    else
                    {
                        MessageBox.Show("NO SE GUARDARON LOS DATOS!", "GESTION PROVEEDORES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                MessageBox.Show("DEBE LLENAR TODOS LOS DATOS!", "GESTION PROVEEDORES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button8.PerformClick();
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

        private void button1_Click(object sender, EventArgs e)
        {

        }
        internal frm_compras.Proveedor CurrentProveedor
        {
            get
            {
                return new frm_compras.Proveedor()
                {
                    getProveedor = proveedor
                };
            }
            set
            {
                CurrentProveedor.getProveedor = proveedor;
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {            
                if (estado == 1)
                {
                    proveedor = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void frm_proveedores_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if(textBox2.Text != "" || textBox7.Text != "")
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "GESTION PROVEEDORES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                    button7.PerformClick();
                } else if (tabControl1.SelectedTab == tabPage2)
                {
                    button2.PerformClick();
                }
            }
        }

        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox4.Focus();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox6.Focus();
            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox5.Focus();
            }
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox3.Focus();
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
        private bool codigoRepetido(string code)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_PROVEEDOR FROM PROVEEDOR WHERE ID_PROVEEDOR =  '{0}' AND ESTADO_TUPLA = TRUE;", code);
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
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
            return true;
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
                    textBox6.Focus();
                    codex = false;
                }

            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                e.Handled = true;
                button9.PerformClick();
                textBox6.Focus();
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                if (textBox4.Text == "") {
                    e.Handled = true;
                    textBox4.Text = "N/A";
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
                    textBox3.Focus();
                }
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                if (textBox3.Text == "")
                {
                    e.Handled = true;
                    textBox3.Text = "N/A";
                }
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button2.PerformClick();
            }
        }

        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                if (textBox13.Text == "")
                {
                    e.Handled = true;
                    textBox13.Text = "N/A";
                    textBox12.Focus();
                }
            }
        }

        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                if (textBox12.Text == "")
                {
                    e.Handled = true;
                    textBox12.Text = "N/A";
                    textBox11.Focus();
                }
            }
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                if (textBox11.Text == "")
                {
                    e.Handled = true;
                    textBox11.Text = "N/A";
                }
            }
        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox10.Focus();
            }
        }

        private void textBox10_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox13.Focus();
            }
        }

        private void textBox13_KeyDown(object sender, KeyEventArgs e)
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
                textBox11.Focus();
            }
        }

        private void textBox11_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button7.PerformClick();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if ((textBox9.Text != "") && (textBox7.Text != ""))
            {
                MessageBox.Show("SE CAMBIARA EL ID DEL CLIENTE!", "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                bool codex = true;
                while (codex)
                {
                    string temp = createCode(9);
                    if (codigoRepetido(temp))
                    {
                        textBox7.Text = temp;
                        codex = false;
                    }

                }
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            var forma = new frm_buscaProveedor(0, nameUs, rolUs, usuario, idSucursal, privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
               
            }
        }

        private void frm_proveedores_Load(object sender, EventArgs e)
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
    }
}
