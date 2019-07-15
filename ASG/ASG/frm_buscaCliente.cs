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
    public partial class frm_buscaCliente : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string cliente;
        string nameUs;
        string rolUs;
        string usuario;
        string idSucursal;
        bool[] privilegios;
        public frm_buscaCliente(string nameUser, string rolUser, string user, string sucursal, bool[] privilegio)
        {
            InitializeComponent();
            cargaDatos();
            usuario = user;
            nameUs = nameUser;
            rolUs = rolUser;
            idSucursal = sucursal;
            this.privilegios = privilegio;
            if(privilegios[6] != true)
            {
                button2.Enabled = false;
            }
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
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                        styleDV(this.dataGridView1);
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
        private void styleDV(DataGridView data)
        {
            data.RowsDefaultCellStyle.BackColor = Color.LightGray;
            data.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            cargaDatos();
        }

        private void frm_buscaCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.A))
            {
                button2.PerformClick();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_buscaCliente_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_buscaCliente_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
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
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                            styleDV(this.dataGridView1);
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
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                styleDV(this.dataGridView1);
                            }
                            // setDescripcion(1);
                        }
                        else
                        {
                            sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE APELLIDO_CLIENTE LIKE '%{0}%';", textBox1.Text);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Clear();
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                    styleDV(this.dataGridView1);
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
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                        styleDV(this.dataGridView1);
                                    }
                                    //setDescripcion(2);
                                }
                                else
                                {
                                    sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE TIPO LIKE '%{0}%';", textBox1.Text);
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Clear();
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                        while (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                            styleDV(this.dataGridView1);
                                        }
                                        //setDescripcion(3);
                                    }
                                    else
                                    {
                                        sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE LIMITE_CREDITO LIKE '%{0}%';", textBox1.Text);
                                        cmd = new OdbcCommand(sql, conexion);
                                        reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            dataGridView1.Rows.Clear();
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                            while (reader.Read())
                                            {
                                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                                styleDV(this.dataGridView1);
                                            }
                                            // setDescripcion(4);
                                        }
                                        else
                                        {
                                            sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE DIAS_CREDITO LIKE '%{0}%';", textBox1.Text);
                                            cmd = new OdbcCommand(sql, conexion);
                                            reader = cmd.ExecuteReader();
                                            if (reader.Read())
                                            {
                                                dataGridView1.Rows.Clear();
                                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                                while (reader.Read())
                                                {
                                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), reader.GetString(9));
                                                    styleDV(this.dataGridView1);
                                                }
                                                //setDescripcion(5);
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
        internal frm_clientes.Cliente currentCliente
        {
            get
            {
                return new frm_clientes.Cliente()
                {
                    getCliente = cliente
                };
            }
            set
            {
                currentCliente.getCliente = cliente;
            }
        }
        private void frm_buscaCliente_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                cliente = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                DialogResult = DialogResult.OK;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (dataGridView1.RowCount > 0)
                {
                   cliente = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    DialogResult = DialogResult.OK;
                }
            }

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

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dataGridView1.RowCount > 0)
                {
                    cliente = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void frm_buscaCliente_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var forma = new frm_clientes(nameUs, rolUs, usuario, idSucursal, true, "", "", privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                button4.PerformClick();
            }
        }
        private void cargaSeleccion(string tipo)
        {
            dataGridView1.Rows.Clear();
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_CLIENTES WHERE TIPO = '{0}';", tipo);
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
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.Text == "EFECTIVO")
            {
                cargaSeleccion("EFECTIVO");
            }
            else if (comboBox5.Text == "CREDITO")
            {
                cargaSeleccion("CREDITO");
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
