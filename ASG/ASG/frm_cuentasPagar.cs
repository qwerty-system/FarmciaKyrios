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
    public partial class frm_cuentasPagar : Form
    {
        string nameUs;
        string rolUs;
        string usuario;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string nombreSucursal;
        ContextMenuStrip mymenu = new ContextMenuStrip();
        bool flag = false;
        public frm_cuentasPagar(string nameUser, string rolUser, string user, string nombre)
        {
            InitializeComponent();
            usuario = user;
            nameUs = nameUser;
            rolUs = rolUser;
            nombreSucursal = nombre;
            label19.Text = label19.Text + "" + nameUs;
            stripMenu();
            cargaDatos();
            mymenu.Items[1].Enabled = false;
            if (rolUs != "ADMINISTRADOR")
            {
                mymenu.Items[1].Enabled = false;

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

                    mymenu.Visible = false;
                    flag = true;

                }
                else if (e.ClickedItem.Name == "ColDelete")
                {
                    mymenu.Visible = false;
                    mymenu.Enabled = false;
                    flag = true;
                    SendKeys.Send("{ENTER}");

                }

            }
        }
        private void stripMenu()
        {
            mymenu.Items.Add("Ocultar Fila");
            mymenu.Items[0].Name = "ColHidden";
            mymenu.Items.Add("Eliminar Cuenta");
            mymenu.Items[1].Name = "ColEdit";
            mymenu.Items.Add("Ver Cartera del Proveedor");
            mymenu.Items[2].Name = "ColDelete";
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void frm_cuentasPagar_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_cuentasPagar_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_cuentasPagar_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_cuentasPagar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }
        private void cargaDatos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_PAGAR WHERE ESTADO_TUPLA = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) ,reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)),reader.GetString(5), reader.GetString(6), reader.GetString(7));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) , reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)),reader.GetString(5), reader.GetString(6), reader.GetString(7));
                    }
                    setDescripcion(6);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaDatosCompletados()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_PAGARF;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), reader.GetString(5), reader.GetString(6), reader.GetString(7));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), reader.GetString(5), reader.GetString(6), reader.GetString(7));
                    }
                    setDescripcion(6);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void setDescripcion(int celda)
        {
            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[celda].Value.ToString() == "0")
                {
                    dataGridView1.Rows[i].Cells[celda].Value = "INACTIVO";

                }
                else
                {
                    dataGridView1.Rows[i].Cells[celda].Value = "ACTIVO";
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            comboBox5.SelectedIndex = 1;
            cargaDatos();
        }

        private void frm_cuentasPagar_Load(object sender, EventArgs e)
        {
            comboBox5.SelectedIndex = 1;
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.Text == "EXISTENTES")
            {
                cargaDatos();
            }
            else
            {
                cargaDatosCompletados();
                dataGridView1.Columns[3].HeaderText = "DOCUMENTOS CANCELADOS";
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                SendKeys.Send("{ENTER}");
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
        private void buscaCompletas()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_PAGARF WHERE ID_PROVEEDOR LIKE '%{0}%';", textBox1.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), reader.GetString(5), reader.GetString(6), reader.GetString(7));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), reader.GetString(5), reader.GetString(6), reader.GetString(7));
                    }
                    setDescripcion(6);

                }
                else
                {
                    conexion.Close();
                    conexion = ASG_DB.connectionResult();
                    sql = string.Format("SELECT * FROM VISTA_PAGARF WHERE NOMBRE_PROVEEDOR LIKE '%{0}%';", textBox1.Text.Trim());
                    cmd = new OdbcCommand(sql, conexion);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), reader.GetString(5), reader.GetString(6), reader.GetString(7));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), reader.GetString(5), reader.GetString(6), reader.GetString(7));
                        }
                        setDescripcion(6);
                    }

                }
            }
            catch (Exception es)
            {
                MessageBox.Show(es.ToString());
            }
        }
        private void buscaExistentes()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_PAGAR WHERE ESTADO_TUPLA = TRUE AND ID_PROVEEDOR LIKE '%{0}%';", textBox1.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), reader.GetString(5), reader.GetString(6), reader.GetString(7));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), reader.GetString(5), reader.GetString(6), reader.GetString(7));
                    }
                    setDescripcion(6);

                }
                else
                {
                    conexion.Close();
                    conexion = ASG_DB.connectionResult();
                    sql = string.Format("SELECT * FROM VISTA_PAGAR WHERE ESTADO_TUPLA = TRUE AND NOMBRE_PROVEEDOR LIKE '%{0}%';", textBox1.Text.Trim());
                    cmd = new OdbcCommand(sql, conexion);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), reader.GetString(5), reader.GetString(6), reader.GetString(7));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), reader.GetString(5), reader.GetString(6), reader.GetString(7));
                        }
                        setDescripcion(6);
                    }
                    
                }
            }
            catch (Exception es)
            {
                MessageBox.Show(es.ToString());
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if (comboBox5.Text == "EXISTENTES")
                {
                    buscaExistentes();
                }
                else
                {
                    buscaCompletas();
                }
            }
            else
            {
                if (comboBox5.Text == "EXISTENTES")
                {
                    button4.PerformClick();
                }
                else
                {
                    cargaDatosCompletados();
                }

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

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                if(e.KeyData == Keys.Enter)
                {
                    bool cast = true;
                    if (comboBox5.Text != "EXISTENTES")
                    {
                        cast = false;
                    }
                    var tester = new frm_balancePagos(dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[2].Value.ToString(), rolUs, cast,nombreSucursal,usuario, dataGridView1.CurrentRow.Cells[7].Value.ToString());
                    if(tester.ShowDialog() == DialogResult.OK)
                    {
                        if (comboBox5.Text == "EXISTENTES")
                        {
                            button4.PerformClick();
                        }
                    }
                }
                cargaDatos();
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
