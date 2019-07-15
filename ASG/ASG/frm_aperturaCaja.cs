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
    public partial class frm_aperturaCaja : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string codigoSucursal;
        string nombreUsuario;
        string usuarioSistema;
        public frm_aperturaCaja(string sucursal, string usuario, string nombre)
        {
            InitializeComponent();
            codigoSucursal = sucursal;
            usuarioSistema = usuario;
            nombreUsuario = nombre;
            label15.Text = usuarioSistema;
            label3.Text = nombreUsuario;
            textBox2.Focus();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_aperturaCaja_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_aperturaCaja_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_aperturaCaja_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            aperturaCaja();
            DialogResult = DialogResult.OK;
        }

        private void frm_aperturaCaja_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }
        private void aperturaCaja()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("INSERT INTO CAJA VALUES (NULL,{0},'{1}',NOW(),{2},NOW(),{2},'APERTURADA',TRUE,TRUE);", codigoSucursal,usuarioSistema,textBox2.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    var fomra = new frm_creditoActualizado();
                    fomra.ShowDialog();
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

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))

            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (textBox2.Text != "")
                {
                    button2.PerformClick();
                } else
                {
                    MessageBox.Show("DEBE DE LLENAR TODOS LOS CAMPOS", "APERTURA CAJA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
