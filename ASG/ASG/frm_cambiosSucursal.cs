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
 
    public partial class frm_cambiosSucursal : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string codigoMercaderia;
        public frm_cambiosSucursal(string mercaderia, string sucursal)
        {
            InitializeComponent();
            codigoMercaderia = mercaderia;
            cargaSucursales(mercaderia);
            getPresentaciones(mercaderia, sucursal);
        }

        private void frm_cambiosSucursal_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            } else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                button2.PerformClick();
            }
        }

        private void frm_cambiosSucursal_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_cambiosSucursal_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_cambiosSucursal_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }
        private void cargaSucursales(string mercaderia)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
               
                string sql = string.Format("SELECT * FROM VISTA_SUCURSALES_PRECIOS WHERE ID_MERCADERIA = '{0}';",mercaderia);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void frm_cambiosSucursal_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = false;

            dataGridView4.Columns[0].ReadOnly = false;
            dataGridView4.Columns[1].ReadOnly = true;
            dataGridView4.Columns[2].ReadOnly = true;
            dataGridView4.Columns[3].ReadOnly = true;
            dataGridView4.Columns[4].ReadOnly = true;
            dataGridView4.Columns[5].ReadOnly = true;
            dataGridView4.Columns[6].ReadOnly = true;
            dataGridView4.Columns[7].ReadOnly = true;
            dataGridView4.Columns[8].ReadOnly = true;
            dataGridView4.Columns[9].ReadOnly = true;
            dataGridView4.Columns[10].ReadOnly = true;
            dataGridView4.Columns[11].ReadOnly = true;
            dataGridView4.Columns[12].ReadOnly = true;
            dataGridView4.Columns[13].ReadOnly = true;
            dataGridView4.Columns[14].ReadOnly = true;
            dataGridView4.Columns[15].ReadOnly = true;
            dataGridView4.Columns[16].ReadOnly = true;

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void updateStock(string mercaderia, string stock, string descipcion, string cantidadP, string total, string cantM, string p1, string p2, string p3, string p4, string p5, string p6, string u1, string u2, string u3, string u4, string u5, string sucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL UPDATE_SUCURSALES_PRESENTACIONES ('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17});", mercaderia,stock, descipcion, cantidadP, total, cantM, p1, p2, p3, p4, p5, p6, u1, u2, u3, u4, u5, sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //MessageBox.Show("SE ACTUALIZA TIPO VENTA");    
                }
                else
                {
                    //MessageBox.Show("fallo update stock");
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private bool sucursalSeleccionada()
        {
            for(int i = 0; i<dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null)
                {
                    if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[2].Value.ToString()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (sucursalSeleccionada())
            {
                if ((dataGridView1.RowCount > 0) && (dataGridView4.RowCount > 0))
                {
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[2].Value != null)
                        {
                            if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[2].Value.ToString()))
                            {
                                for (int j = 0; j < dataGridView4.RowCount; j++)
                                {
                                    if (dataGridView4.Rows[j].Cells[0].Value != null)
                                    {
                                        if (Convert.ToBoolean(dataGridView4.Rows[j].Cells[0].Value.ToString()))
                                        {
                                            //MessageBox.Show(dataGridView1.Rows[i].Cells[1].Value.ToString() + " PRESENTACION -> " + dataGridView4.Rows[j].Cells[2].Value.ToString());
                                            updateStock(codigoMercaderia, dataGridView4.Rows[j].Cells[1].Value.ToString(), dataGridView4.Rows[j].Cells[2].Value.ToString(), dataGridView4.Rows[j].Cells[3].Value.ToString(), dataGridView4.Rows[j].Cells[4].Value.ToString(), dataGridView4.Rows[j].Cells[5].Value.ToString(), dataGridView4.Rows[j].Cells[6].Value.ToString(), dataGridView4.Rows[j].Cells[7].Value.ToString(), dataGridView4.Rows[j].Cells[8].Value.ToString(), dataGridView4.Rows[j].Cells[9].Value.ToString(), dataGridView4.Rows[j].Cells[10].Value.ToString(), dataGridView4.Rows[j].Cells[11].Value.ToString(), dataGridView4.Rows[j].Cells[12].Value.ToString(), dataGridView4.Rows[j].Cells[13].Value.ToString(), dataGridView4.Rows[j].Cells[14].Value.ToString(), dataGridView4.Rows[i].Cells[15].Value.ToString(), dataGridView4.Rows[j].Cells[16].Value.ToString(), dataGridView1.Rows[i].Cells[0].Value.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var forma = new frm_creditoActualizado();
                    forma.ShowDialog();
                    button2.Enabled = false;
                }
                else
                {
                    MessageBox.Show("DEBEN DE EXISTIR DATOS PARA APLICAR LOS PRECIOS!", "PRECIOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } else
            {
                MessageBox.Show("NO HA SELECCIONADO NINGUNA SUCURSAL!", "PRECIOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void getPresentaciones(string mercaderia, string nombreSucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_STOCK WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", mercaderia, nombreSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView4.Rows.Add(true,reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                    while (reader.Read())
                    {
                        dataGridView4.Rows.Add(true,reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14), reader.GetString(15));
                        //styleDV(this.dataGridView4);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var forma = new  frm_informacionTraslado();
            forma.ShowDialog();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
