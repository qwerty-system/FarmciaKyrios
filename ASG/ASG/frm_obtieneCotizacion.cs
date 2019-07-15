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
    public partial class frm_obtieneCotizacion : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string codigo;
        public frm_obtieneCotizacion(string nombre)
        {
            InitializeComponent();
            cargaSucursales();
            comboBox1.Text = nombre;
            cargaDatos();
        }
        internal frm_facturas.factrua currentFactura
        {
            get
            {
                return new frm_facturas.factrua()
                {
                    codigoFactura = codigo
                };
            }
            set
            {
                currentFactura.codigoFactura = codigo;
            }
        }
        private void cargaDatos()
        {
            dataGridView1.Rows.Clear();
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
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
                    //comboBox1.SelectedIndex = 0;
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

        private void frm_obtieneCotizacion_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_obtieneCotizacion_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_obtieneCotizacion_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_obtieneCotizacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
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

        private void button4_Click(object sender, EventArgs e)
        {
            cargaDatos();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargaDatos();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {

                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    string sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND NOMBRE_CLIENTE LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, true, textBox1.Text.Trim());
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                            //styleDV(this.dataGridView1);
                        }
                    }
                    else
                    {
                        conexion.Close();
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND APELLIDO_CLIENTE LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, true, textBox1.Text.Trim());
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                //styleDV(this.dataGridView1);
                            }
                        }
                        else
                        {
                            conexion.Close();
                            conexion = ASG_DB.connectionResult();
                            sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND TIPO_FACTURA LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, true, textBox1.Text.Trim());
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Clear();
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                    //styleDV(this.dataGridView1);
                                }
                            }
                            else
                            {
                                conexion.Close();
                                conexion = ASG_DB.connectionResult();
                                sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND NOMBRE_USUARIO LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, true, textBox1.Text.Trim());
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Clear();
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                        //styleDV(this.dataGridView1);
                                    }
                                }
                                else
                                {
                                    conexion.Close();
                                    conexion = ASG_DB.connectionResult();
                                    sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND ID_CLIENTE LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, true, textBox1.Text.Trim());
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Clear();
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                        while (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                            //styleDV(this.dataGridView1);
                                        }
                                    }
                                    else
                                    {
                                        conexion.Close();
                                        conexion = ASG_DB.connectionResult();
                                        sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND ID_FACTURA LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, true, textBox1.Text.Trim());
                                        cmd = new OdbcCommand(sql, conexion);
                                        reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            dataGridView1.Rows.Clear();
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                            while (reader.Read())
                                            {
                                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                                //styleDV(this.dataGridView1);
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
            }
            else
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
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = {1} AND FECHA_EMISION_FACTURA LIKE '%{2}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, true, dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                  
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
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

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                codigo = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                DialogResult = DialogResult.OK;
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dataGridView1.RowCount > 0)
                {
                    codigo = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    DialogResult = DialogResult.OK;
                }
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
