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
    public partial class frm_cierreCaja : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string codigoCaja;
        string montoCaja;
        public frm_cierreCaja(string codigo, string nombre, string usuario, string monto)
        {
            InitializeComponent();
            label3.Text = nombre;
            label15.Text = usuario;
            label6.Text = monto;
            codigoCaja = codigo;
            montoCaja = monto;
        }
        private void cierraCaja(string codigo, string descripcion)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CAJA SET ESTADO_TUPLA = FALSE, OBSERVACIONES_CAJA = '{0}' WHERE ID_CAJA = {1};",descripcion, codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    var forma = new frm_creditoActualizado();
                    forma.ShowDialog();
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("IMPOSILBE CERRAR CAJA!", "GESTION CAJA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void frm_cierreCaja_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_cierreCaja_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_cierreCaja_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_cierreCaja_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox2.Text != "")
            {
                cierraCaja(codigoCaja,textBox2.Text);

            } else
            {
                cierraCaja(codigoCaja, "CIERRE DE CAJA CORRECTO");
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                button2.PerformClick();
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
