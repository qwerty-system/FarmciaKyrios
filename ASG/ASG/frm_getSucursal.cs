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
    public partial class frm_getSucursal : Form
    {
        string sucursalObtenida;
        public frm_getSucursal()
        {
            InitializeComponent();
            cargaSucursales();
        }
        private void cargaSucursales()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM  VISTA_SUCURSALES;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), reader.GetString(3), reader.GetString(4));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), reader.GetString(3), reader.GetString(4));
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
        private void pictureBox3_MouseHover(object sender, EventArgs e)
        {
            //pictureBox3.BackColor = Color.Red;
        }
        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
           // pictureBox3.BackColor = Color.Transparent;
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            
        }
        private void button8_Click(object sender, EventArgs e)
        {
            
            sucursalObtenida = label1.Text;
            this.Close();
            DialogResult = DialogResult.OK;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            sucursalObtenida = label2.Text;
            this.Close();
            DialogResult = DialogResult.OK;
        }
        internal frm_menu.Sucursal CurrentSucursal
        {
            get
            {
                return new frm_menu.Sucursal()
                {
                    getSucursal = sucursalObtenida
                };
            }
            set
            {
                CurrentSucursal.getSucursal = sucursalObtenida;
            }
        }

        private void frm_getSucursal_Load(object sender, EventArgs e)
        {

        }

        private void button8_DragEnter(object sender, DragEventArgs e)
        {
            
        }

        private void button8_MouseEnter(object sender, EventArgs e)
        {
           
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (sucursalObtenida != "")
                {
                    sucursalObtenida = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    this.Close();
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("SELECCIONE UNA SUCURSAL!", "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                if(e.KeyData == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    if (sucursalObtenida != "")
                    {
                        sucursalObtenida = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        this.Close();
                        DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("SELECCIONE UNA SUCURSAL!", "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }
    }
}
