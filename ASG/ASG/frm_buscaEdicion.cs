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
    public partial class frm_buscaEdicion : Form
    {
        string mercaderiaObtenida;
        string sucursal;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string nombreSucursal;
        public frm_buscaEdicion(string sucursal, string rol)
        {
            InitializeComponent();
            cargaSucursales();
            nombreSucursal = sucursal;
            comboBox2.Text = nombreSucursal;
            cargaMercaderia();
            if(rol != "ADMINISTRADOR")
            {
                dataGridView1.Columns[3].Visible = false;
                dataGridView1.Columns[2].Visible = false;
                dataGridView1.Columns[0].Width = 200;
                dataGridView1.Columns[1].Width = 300;
                dataGridView1.Columns[4].Width = 200;
                this.dataGridView1.Size = new Size(660,331);
                this.dataGridView1.Location = new Point(45, 135);
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
                    comboBox2.Items.Clear();
                   // comboBox4.Items.Clear();
                    comboBox2.Items.Add(reader.GetString(0));

                    //comboBox4.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader.GetString(0));
                        //comboBox4.Items.Add(reader.GetString(0));
                    }
                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void cargaMercaderia()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE NOMBRE_SUCURSAL = '{0}';", comboBox2.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                        //styleDV(this.dataGridView1);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        internal frm_compras.Mercaderia CurrentMercaderia
        {
            get
            {
                return new frm_compras.Mercaderia()
                {
                    getMercaderia = mercaderiaObtenida
                };
            }
            set
            {
                CurrentMercaderia.getMercaderia = mercaderiaObtenida;
            }
        }
      
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        internal frm_mercaderia.Sucursal CurrentSucursal
        {
            get
            {
                return new frm_mercaderia.Sucursal()
                {
                    getSucursal = sucursal
                };
            }
            set
            {
                CurrentSucursal.getSucursal = sucursal;
            }
        }
        private void frm_buscaEdicion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_buscaEdicion_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_buscaEdicion_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_buscaEdicion_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                mercaderiaObtenida = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                sucursal = comboBox2.Text.Trim();
                DialogResult = DialogResult.OK;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cargaMercaderia();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (dataGridView1.RowCount > 0)
                {
                    mercaderiaObtenida = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    sucursal = comboBox2.Text.Trim();
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
             try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE ID_MERCADERIA LIKE '%{0}%' AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, comboBox2.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                            //styleDV(this.dataGridView1);
                        }
                    }
                    else
                    {
                        conexion.Close();
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE DESCRIPCION LIKE '%{0}%' AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, comboBox2.Text);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                //styleDV(this.dataGridView1);
                            }
                        }
                        else
                        {
                            conexion.Close();
                            conexion = ASG_DB.connectionResult();
                            sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE NOMBRE_PROVEEDOR LIKE '%{0}%' AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, comboBox2.Text);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Clear();
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                    //styleDV(this.dataGridView1);
                                }
                            }
                            else
                            {
                                conexion.Close();
                                conexion = ASG_DB.connectionResult();
                                sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE PRECIO_COMPRA LIKE '%{0}%' AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, comboBox2.Text);
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Clear();
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                        //styleDV(this.dataGridView1);
                                    }
                                }
                                else
                                {
                                    conexion.Close();
                                    conexion = ASG_DB.connectionResult();
                                    sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE TOTAL_UNIDADES LIKE '%{0}%' AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, comboBox2.Text);
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Clear();
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                        while (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                            ///styleDV(this.dataGridView1);
                                        }
                                    }
                                    
                                }
                            }
                        }
                    }
                }
                catch (Exception es)
                {
                    MessageBox.Show(es.ToString());
                }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nombreSucursal != comboBox2.Text)
            {
                MessageBox.Show("LA MERCADERIA A LA QUE ACCEDERAS SE ENCUENTRA EN OTRA SUCURSAL!", "NUEVA VENTA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
                try
                {
                    dataGridView1.Rows.Clear();
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE NOMBRE_SUCURSAL = '{0}';", comboBox2.Text);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {

                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                            //styleDV(this.dataGridView1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
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
                        mercaderiaObtenida = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        sucursal = comboBox2.Text.Trim();
                        DialogResult = DialogResult.OK;
                }
            }
        }

        private void frm_buscaEdicion_Load(object sender, EventArgs e)
        {

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
    }
}
