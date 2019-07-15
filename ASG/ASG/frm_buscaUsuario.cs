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
    public partial class frm_buscaUsuario : Form
    {
        string usuario;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public frm_buscaUsuario()
        {
            InitializeComponent();
            cargaDatos();
            textBox1.Focus();
        }
       
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
            DialogResult = DialogResult.Cancel;
        }
        private void setDescripcion(int celda)
        {
            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                dataGridView1.Rows[i].Cells[celda].Style.ForeColor = Color.White;
                dataGridView1.Rows[i].Cells[celda].Style.BackColor = Color.FromArgb(207, 136, 65);
            }
        }
        private void setEstado(int celda)
        {
            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[celda].Value.ToString() == "1")
                {
                    dataGridView1.Rows[i].Cells[celda].Value = "ACTIVO";
                }
                else
                {
                    dataGridView1.Rows[i].Cells[celda].Value = "INACTIVO";
                    dataGridView1.Rows[i].Cells[celda].Style.ForeColor = Color.White;
                    dataGridView1.Rows[i].Cells[celda].Style.BackColor = Color.FromArgb(207, 136, 65);
                }
            }
        }
        private void cargaDatos()
        {
            dataGridView1.Rows.Clear();
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_USUARIOS WHERE ID_ROL <> 'ROOT';");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                    while (reader.Read())
                    {

                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5)); 
                        //styleDV(this.dataGridView1);
                    }
                    setEstado(4);
                }
                else
                {
                    MessageBox.Show("NO EXISTEN USUARIOS!", "GESTION USUARIOS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        private void frm_buscaUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }
        internal frm_usuarios.User currentUser
        {
            get
            {
                return new frm_usuarios.User()
                {
                    getUser = usuario
                };
            }
            set
            {
                currentUser.getUser = usuario;
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            if (textBox1.Text != "")
            {
                dataGridView1.Rows.Clear();
                try
                {
                    string sql = string.Format("SELECT * FROM VISTA_USUARIOS WHERE ID_USUARIO LIKE '%{0}%' AND ID_ROL <> 'ROOT';;", textBox1.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                        while (reader.Read())
                        {

                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                            //styleDV(this.dataGridView1);
                        }
                        setEstado(4);
                        //setDescripcion(0);
                    }
                    else
                    {
                        sql = string.Format("SELECT * FROM VISTA_USUARIOS WHERE ID_ROL LIKE '%{0}%' AND ID_ROL <> 'ROOT';", textBox1.Text);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                            while (reader.Read())
                            {

                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                                //styleDV(this.dataGridView1);
                            }
                            setEstado(4);
                            //setDescripcion(1);
                        }
                        else
                        {
                            sql = string.Format("SELECT * FROM VISTA_USUARIOS WHERE NOMBRE_USUARIO LIKE '%{0}%' AND ID_ROL <> 'ROOT';", textBox1.Text);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                                while (reader.Read())
                                {

                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5)); 
                                    //styleDV(this.dataGridView1);
                                }
                                setEstado(4);
                               // setDescripcion(2);
                            }
                            else
                            {
                                sql = string.Format("SELECT * FROM VISTA_USUARIOS WHERE EMAIL LIKE '%{0}%' AND ID_ROL <> 'ROOT';", textBox1.Text);
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                                    while (reader.Read())
                                    {

                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                                        //styleDV(this.dataGridView1);
                                    }
                                    setEstado(4);
                                    //setDescripcion(3);
                                }
                                else
                                {
                                    sql = string.Format("SELECT * FROM VISTA_USUARIOS WHERE LAST_LOGGIN LIKE '%{0}%' AND ID_ROL <> 'ROOT';", textBox1.Text);
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                                        while (reader.Read())
                                        {

                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                                            //styleDV(this.dataGridView1);
                                        }
                                        setEstado(4);
                                        //setDescripcion(5);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION USUARIOS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                cargaDatos();
            }
            conexion.Close();
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                usuario = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                DialogResult = DialogResult.OK;
            }
        }

        private void frm_buscaUsuario_Load(object sender, EventArgs e)
        {
            cargaDatos();
        }

        private void frm_buscaUsuario_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_buscaUsuario_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void label4_MouseUp(object sender, MouseEventArgs e)
        {
           
        }

        private void frm_buscaUsuario_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (dataGridView1.RowCount > 0)
                {
                    usuario = dataGridView1.CurrentRow.Cells[0].Value.ToString();
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
                    usuario = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            cargaDatos();
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
