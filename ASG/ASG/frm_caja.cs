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
    public partial class frm_caja : Form
    {
        string nameUs;
        string rolUs;
        string usuario;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string idSucursal;
        string sucursal;
        bool flag = false;
        ContextMenuStrip mymenu = new ContextMenuStrip();
        public frm_caja(string nameUser, string rolUser, string user, string codigoSuc, string nombre, bool caja)
        {
            InitializeComponent();
            usuario = user;
            nameUs = nameUser;
            rolUs = rolUser;
            idSucursal = codigoSuc;
            sucursal = nombre;
            label19.Text = label19.Text + "" + nameUs;
            cargaSucursales();
            comboBox1.Text = nombre;
            cargaActuales();
            stripMenu();         
            setterForm();
            if (caja != true)
            {
                mymenu.Items[2].Visible = false;
            }
        }
        private void setterForm()
        {
            if (rolUs != "ADMINISTRADOR")
            {
                mymenu.Items[1].Visible = false;
            }
           
        }
        private void stripMenu()
        {
            mymenu.Items.Add("Ocultar Fila");
            mymenu.Items[0].Name = "ColHidden";
            mymenu.Items.Add("Eliminar Caja");
            mymenu.Items[1].Name = "ColEdit";
            mymenu.Items.Add("Cerrar Caja");
            mymenu.Items[2].Name = "ColView";
            mymenu.Items.Add("Ver Detalle de Caja");
            mymenu.Items[3].Name = "ColDet";
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
                    DialogResult result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR LA CAJA?", "GESTION CAJA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        eliminaCaja(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                    }
                   
                    flag = true;

                }
                else if (e.ClickedItem.Name == "ColView")
                {
                   
                    mymenu.Visible = false;
                    mymenu.Enabled = false;
                    DialogResult result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CERRAR LA CAJA?", "GESTION CAJA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        var forma = new frm_cierreCaja(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[7].Value.ToString(), dataGridView1.CurrentRow.Cells[5].Value.ToString());
                         if(forma.ShowDialog() == DialogResult.OK)
                        {
                            button4.PerformClick();
                        }
                    }
                    flag = true;

                }
                else if (e.ClickedItem.Name == "ColDet")
                {
                    mymenu.Visible = false;
                    mymenu.Enabled = false;
                    if (dataGridView1.RowCount > 0)
                    {
                        bool cast = false;
                        if (comboBox5.Text == "CERRADAS")
                        {
                            cast = true;
                        }
                        var forma = new frm_detalleCaja(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[7].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[8].Value.ToString(),
                            dataGridView1.CurrentRow.Cells[2].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[4].Value.ToString(), rolUs, dataGridView1.CurrentRow.Cells[5].Value.ToString(), cast, dataGridView1.CurrentRow.Cells[9].Value.ToString());
                        forma.ShowDialog();
                    }
                    flag = true;

                }
            }
        }
        private void cierraCaja(string codigoCaja)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CAJA SET ESTADO_TUPLA = FALSE WHERE ID_CAJA = {0}", codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    var forma = new frm_done();
                    forma.ShowDialog();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO GENERAR NUEVA ORDEN DE COMPRA!", "GESTION COMPRAS", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void eliminaCaja(string codigoCaja)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CAJA SET ESTADO_CAJA = FALSE, ESTADO_TUPLA = FALSE WHERE ID_CAJA = {0}", codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    var forma = new frm_done();
                    forma.ShowDialog();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO GENERAR NUEVA ORDEN DE COMPRA!", "GESTION COMPRAS", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                    dataGridView1.Rows[i].Cells[celda].Value = "CERRADA";
                }
                else
                {
                    dataGridView1.Rows[i].Cells[celda].Value = "ACTIVA";
                }
            }
        }
        private void cargaCerradas()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_CAJASF WHERE APERTURA_CAJA  >= curdate() AND NOMBRE_SUCURSAL = '{0}' ORDER BY APERTURA_CAJA DESC;", comboBox1.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {

                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                    }
                    setDescripcion(6);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void cargaActuales()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_CAJAS WHERE ESTADO_TUPLA = TRUE AND APERTURA_CAJA  >= curdate() AND NOMBRE_SUCURSAL = '{0}' ORDER BY APERTURA_CAJA DESC;", comboBox1.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8),reader.GetString(9));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8),reader.GetString(9));
                    }
                    setDescripcion(6);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void frm_caja_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_caja_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_caja_MouseUp(object sender, MouseEventArgs e)
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

        private void frm_caja_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_caja_Load(object sender, EventArgs e)
        {
            comboBox5.SelectedIndex = 1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cargaActuales();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox5.Text == "ACTIVAS")
            {
                cargaActuales();
                mymenu.Items[2].Enabled = true;
            } else
            {
                cargaCerradas();
                mymenu.Items[2].Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.Text == "ACTIVAS")
            {
                cargaActuales();
            }
            else
            {
                cargaCerradas();
            }
        }

        private void buscaActuales()
        {
            if (textBox1.Text != "")
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    dataGridView1.Rows.Clear();
                    string sql = string.Format("SELECT * FROM VISTA_CAJAS WHERE ESTADO_TUPLA = TRUE AND APERTURA_CAJA  >= curdate() AND NOMBRE_SUCURSAL = '{0}' AND NOMBRE_USUARIO LIKE '%{1}%' ORDER BY APERTURA_CAJA DESC;", comboBox1.Text, textBox1.Text.Trim());
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                        }
                        setDescripcion(6);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            } else
            {
                cargaActuales();
            }
        }
        private void buscaCompletas()
        {
            if (textBox1.Text != "")
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    dataGridView1.Rows.Clear();
                    string sql = string.Format("SELECT * FROM VISTA_CAJASF WHERE  APERTURA_CAJA  >= curdate() AND NOMBRE_SUCURSAL = '{0}' AND NOMBRE_USUARIO LIKE '%{1}%' ORDER BY APERTURA_CAJA DESC;", comboBox1.Text, textBox1.Text.Trim());
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                        }
                        setDescripcion(6);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            } else
            {
                cargaCerradas();
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox5.Text == "ACTIVAS")
            {
                buscaActuales();
            }
            else
            {
                buscaCompletas();
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
            if (e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dataGridView1.RowCount > 0)
                {
                    bool cast = false;
                    if(comboBox5.Text == "CERRADAS")
                    {
                        cast = true;
                    }
                    var forma = new frm_detalleCaja(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[7].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[8].Value.ToString(), 
                        dataGridView1.CurrentRow.Cells[2].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[4].Value.ToString(),rolUs, dataGridView1.CurrentRow.Cells[5].Value.ToString(),cast, dataGridView1.CurrentRow.Cells[9].Value.ToString());
                    forma.ShowDialog();
                }
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SendKeys.Send("{ENTER}");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (comboBox5.Text == "ACTIVAS")
            {
                cargaActualesFecha();
            }

            else
            {
                cargaCerradasFecha();
            }
        }
        private void cargaActualesFecha()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_CAJAS WHERE ESTADO_TUPLA = TRUE AND APERTURA_CAJA  like '%{0}%' AND NOMBRE_SUCURSAL = '{1}' ORDER BY APERTURA_CAJA DESC;", dateTimePicker1.Value.ToString("yyyy-MM-dd"),comboBox1.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                    }
                    setDescripcion(6);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void cargaCerradasFecha()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_CAJASF WHERE APERTURA_CAJA LIKE '%{0}%' AND NOMBRE_SUCURSAL = '{1}' ORDER BY APERTURA_CAJA DESC;", dateTimePicker1.Value.ToString("yyyy-MM-dd"),comboBox1.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {

                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                    }
                    setDescripcion(6);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
