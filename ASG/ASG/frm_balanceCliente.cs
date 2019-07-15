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
    public partial class frm_balanceCliente : Form
    {
        string codigoCliente;
        string nombreCliente;
        Point DragCursor;
        Point DragForm;
        bool Dragging;

        public frm_balanceCliente(string codigo, string nombre, string tipo, string limite, string disponible)
        {
            InitializeComponent();
            codigoCliente = codigo;
            nombreCliente = nombre;
            label2.Text = codigoCliente;
            label6.Text = nombreCliente;
            label9.Text = tipo;
            label11.Text = limite;
            label13.Text = disponible;
            cargaFacturas();
        }

        private void frm_balanceCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }
        private void cargaFacturas()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_FACTURAS_CLIENTE WHERE ID_CLIENTE = '{0}' AND TIPO_FACTURA = '{1}' ORDER BY FECHA_EMISION_FACTURA DESC LIMIT 25;", codigoCliente, label9.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                    }
                } else
                {
                    dataGridView1.Rows.Add(" ", "NO EXISTEN DATOS");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaFacturasCredito()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            dataGridView1.Rows.Clear();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_FACTURAS_CLIENTE WHERE  ID_CLIENTE = '{0}' AND TIPO_FACTURA = '{1}' ORDER BY FECHA_EMISION_FACTURA DESC LIMIT 25;", codigoCliente, "CREDITO");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                    }
                }
                else
                {
                    dataGridView1.Rows.Add(" ", "NO EXISTEN DATOS");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaFacturasEfectivo()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            dataGridView1.Rows.Clear();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_FACTURAS_CLIENTE WHERE  ID_CLIENTE = '{0}' AND TIPO_FACTURA = '{1}' ORDER BY FECHA_EMISION_FACTURA DESC LIMIT 25;", codigoCliente, "EFECTIVO");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                    }
                }
                else
                {
                    dataGridView1.Rows.Add(" ", "NO EXISTEN DATOS");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();

        }
        private void frm_balanceCliente_Load(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            //cargaFacturas();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox5.Text == "EFECTIVO")
            {
                cargaFacturasEfectivo();
            } else
            {
              
                cargaFacturasCredito();
            }           
        }

        private void frm_balanceCliente_MouseClick(object sender, MouseEventArgs e)
        {
           
        }

        private void frm_balanceCliente_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_balanceCliente_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_balanceCliente_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
