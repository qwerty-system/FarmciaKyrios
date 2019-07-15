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
    public partial class frm_codigoStock : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string codigo;
        string stock;
        bool state;
        string codigoNuevo;
        public frm_codigoStock(string codigoMatriz, string codigoStock, string descripcion, bool estado)
        {
            InitializeComponent();
            label2.Text = codigoMatriz;
            label5.Text = descripcion;
            label8.Text = codigoStock;
            state = estado;
            codigo = codigoMatriz;
            stock = codigoStock;
            textBox1.Focus();
        }

        private void frm_codigoStock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }
        internal frm_mercaderia.edicionCodigo currentCodigo
        {
            get
            {
                return new frm_mercaderia.edicionCodigo()
                {
                    getNewCode = codigoNuevo
                };
            }
            set
            {
                currentCodigo.getNewCode = codigoNuevo;
            }
        }
        private void frm_codigoStock_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_codigoStock_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_codigoStock_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == Keys.Enter)&& (textBox1.Text != ""))
            {
                
                button1.PerformClick();
            }
        }
        private bool buscaCodigo()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_STOCK FROM MERCADERIA_PRESENTACIONES WHERE ID_STOCK = '{0}';", textBox1.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (buscaCodigo())
            {
                if (state)
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    try
                    {
                        string sql = string.Format("UPDATE MERCADERIA_PRESENTACIONES SET ID_STOCK = '{0}' WHERE ID_STOCK = '{1}';", textBox1.Text.Trim(), stock);
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if (cmd.ExecuteNonQuery() >= 0)
                        {
                            var forma = new frm_creditoActualizado();
                            forma.ShowDialog();
                            DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            MessageBox.Show("NO SE PUDO ACTUALIZAR EL SUBCODIGO!", "MANTENIMIENTO CODIGOS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    conexion.Close();

                } else
                {
                    codigoNuevo = textBox1.Text;
                    DialogResult = DialogResult.OK;
                }
            } else
            {
                MessageBox.Show("EL CODIGO YA EXISTE", "MANTENIMIENTO CODIGOS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void timerActions()
        {
            frm_done frm = new frm_done();
            frm.ShowDialog();
            timer1.Interval = 1500;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void frm_codigoStock_Load(object sender, EventArgs e)
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
