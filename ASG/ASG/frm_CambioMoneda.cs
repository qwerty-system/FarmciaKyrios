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
    public partial class frm_CambioMoneda : Form
    {
        string idSucursal;
        string usuario;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public frm_CambioMoneda(string sucursal, string user)
        {
            InitializeComponent();
            idSucursal = sucursal;
            usuario = user;
            cargaMonedas();
            
        }
        
        private void cargaMonedas()
        {
            dataGridView1.Rows.Clear();
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM  TIPO_CAMBIO;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1));
                        //styleDV(this.dataGridView1);
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
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_CambioMoneda_Load(object sender, EventArgs e)
        {
            cargaMonedas();
            dataGridView1.Columns[1].ReadOnly = false;
            dataGridView1.Rows[2].Cells[1].ReadOnly = true;
        }
        private void timerActions()
        {
            frm_done frm = new frm_done();
            frm.ShowDialog();
            timer1.Interval = 1500;
            timer1.Enabled = true;
        }
        private void guardaMonedas(string idMoneda, string monto)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = sql = string.Format("UPDATE TIPO_CAMBIO SET TIPO_CAMBIO = {0} WHERE IDMONEDA = '{1}';", monto,idMoneda);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    adressUser.ingresaBitacora(idSucursal, usuario,"ACTUALIZACION TIPO CAMBIO", idMoneda);
                }
            }
            catch
          (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                guardaMonedas(dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString());
            }
            var forma = new frm_creditoActualizado();
            forma.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_CambioMoneda_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_CambioMoneda_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_CambioMoneda_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 1)

            {
                if (Char.IsNumber(e.KeyChar) || e.KeyChar == (Char)Keys.Back || e.KeyChar == '.')
                    e.Handled = false;
                else
                    e.Handled = true;
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 1)
            {

                TextBox txt = e.Control as TextBox;

                if (txt != null)

                {
                    txt.KeyPress -= new KeyPressEventHandler(dataGridView1_KeyPress);

                    txt.KeyPress += new KeyPressEventHandler(dataGridView1_KeyPress);
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
