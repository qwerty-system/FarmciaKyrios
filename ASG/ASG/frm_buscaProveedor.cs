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
    public partial class frm_buscaProveedor : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string proveedor;
        int typeData;
        string nameUs;
        string rolUs;
        string usuario;
        string idSucursal;
        bool[] privilegios;
        public frm_buscaProveedor(int indexer, string nameUser, string rolUser, string user, string sucursal, bool[] privilegio)
        {
            InitializeComponent();
            typeData = indexer;
            cargaDatos();
            usuario = user;
            nameUs = nameUser;
            rolUs = rolUser;
            this.privilegios = privilegio;
            idSucursal = sucursal;
        }
        private void cargaDatos()
        {
            dataGridView1.Rows.Clear();
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_PROVEEDORES;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
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
            this.Close();
        }

        private void frm_buscaProveedor_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_buscaProveedor_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_buscaProveedor_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }
       
        private void button4_Click(object sender, EventArgs e)
        {
            cargaDatos();
        }

        private void frm_buscaProveedor_KeyDown(object sender, KeyEventArgs e)
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
        internal frm_proveedores.Proveedor currentProveedor
        {
            get
            {
                return new frm_proveedores.Proveedor()
                {
                    getProveedor = proveedor
                };
            }
            set
            {
                currentProveedor.getProveedor = proveedor;
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (typeData == 0) {
                    proveedor = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    DialogResult = DialogResult.OK;
                } else if (typeData == 1)
                {
                    proveedor = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                    DialogResult = DialogResult.OK;
                }
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
                    string sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE ID_PROVEEDOR LIKE '%{0}%';", textBox1.Text);
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
                        setDescripcion(0);
                    }
                    else
                    {
                        sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE NOMBRE_PROVEEDOR LIKE '%{0}%' ; ", textBox1.Text);
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
                            setDescripcion(1);
                        }
                        else
                        {
                            sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE EMAIL_PROVEEDOR LIKE '%{0}%' ; ", textBox1.Text);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                                    ////styleDV(this.dataGridView1);
                                }
                                setDescripcion(2);
                            }
                            else
                            {
                                sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE DIRECCION_PROVEEDOR LIKE '%{0}%' ; ", textBox1.Text);
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
                                    setDescripcion(3);
                                }
                                else
                                {
                                    sql = string.Format("SELECT * FROM VISTA_PROVEEDORES WHERE TELEFONO_PROVEEDOR LIKE '%{0}%' ; ", textBox1.Text);
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
                                        setDescripcion(4);
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
        private void setDescripcion(int celda)
        {
            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                dataGridView1.Rows[i].Cells[celda].Style.ForeColor = Color.White;
                dataGridView1.Rows[i].Cells[celda].Style.BackColor = Color.FromArgb(244, 101, 36);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (dataGridView1.RowCount > 0)
                {
                    if (typeData == 0)
                    {
                        proveedor = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        DialogResult = DialogResult.OK;
                    }
                    else if (typeData == 1)
                    {
                        proveedor = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                        DialogResult = DialogResult.OK;
                    }
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

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var forma = new frm_proveedores(nameUs, rolUs, usuario, 1, idSucursal,true, privilegios);
            forma.ShowDialog();
            if (forma.DialogResult == DialogResult.OK)
            {
                button4.PerformClick();
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

        private void button2_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
    }
}
