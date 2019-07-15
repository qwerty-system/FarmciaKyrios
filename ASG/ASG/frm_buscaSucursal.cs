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
    public partial class frm_buscaSucursal : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string sucursal;
        public frm_buscaSucursal()
        {
            InitializeComponent();
            cargaSucursales();
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
                        //styleDV(this.dataGridView2);
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
        private void frm_buscaSucursal_Load(object sender, EventArgs e)
        {

        }

        private void frm_buscaSucursal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_buscaSucursal_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_buscaSucursal_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_buscaSucursal_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }
        internal frm_ajustes.Sucursales currentSucursal
        {
            get
            {
                return new frm_ajustes.Sucursales()
                {
                    getSucursal = sucursal
                };
            }
            set
            {
                currentSucursal.getSucursal = sucursal;
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
                            //styleDV(this.dataGridView2);
                        }
                       // setSucursal(0);
                    }
                    else
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
                                //styleDV(this.dataGridView2);
                            }
                          //  setSucursal(1);
                        }
                        else
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
                                    //styleDV(this.dataGridView2);
                                }
                               // setSucursal(2);
                            }
                            else
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
                                        //styleDV(this.dataGridView2);
                                    }
                                   // setSucursal(3);
                                }
                                else
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
                                            //styleDV(this.dataGridView2);
                                        }
                                       // setSucursal(4);
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

        private void dataGridView2_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView2.RowCount > 0)
            {
                sucursal = dataGridView2.CurrentRow.Cells[0].Value.ToString();
                DialogResult = DialogResult.OK;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (dataGridView2.RowCount > 0)
                {
                    sucursal = dataGridView2.CurrentRow.Cells[0].Value.ToString();
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Down)
            {
                if (dataGridView2.RowCount > 0)
                {
                    dataGridView2.Focus();
                    if (dataGridView2.RowCount > 1)
                        this.dataGridView2.CurrentCell = this.dataGridView2[1, dataGridView2.CurrentRow.Index + 1];
                }
            }
        }

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dataGridView2.RowCount > 0)
                {
                    sucursal = dataGridView2.CurrentRow.Cells[0].Value.ToString();
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
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
    }
}
