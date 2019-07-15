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
    public partial class frm_preciosMercaderia : Form
    {
        string precio;
        bool estado = false;
        public frm_preciosMercaderia(string stock, string sucursal,string rol, bool estate)
        {
            InitializeComponent();
            estado = estate;
            cargaPrecios(stock, sucursal);
            if(rol != "ADMINISTRADOR")
            {
                dataGridView1.Rows[0].Visible = false;
                dataGridView1.Columns[2].Visible = false;
                dataGridView1.Size = new Size(275,235);
                dataGridView1.Location = new Point(79, 83);
            }
        }
        private void cargaPrecios(string idStock, string idSucursal)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_PRECIOS WHERE ID_STOCK = '{0}' AND NOMBRE_SUCURSAL = '{1}';", idStock, idSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add("PRECIO COSTO", reader.GetString(0), 0);
                    dataGridView1.Rows.Add("PRECIO VENTA 1", reader.GetString(1), reader.GetString(2));
                    dataGridView1.Rows.Add("PRECIO VENTA 2", reader.GetString(3), reader.GetString(4));
                    dataGridView1.Rows.Add("PRECIO VENTA 3", reader.GetString(5), reader.GetString(6));
                    dataGridView1.Rows.Add("PRECIO VENTA 4", reader.GetString(7), reader.GetString(8));
                    dataGridView1.Rows.Add("PRECIO VENTA 5", reader.GetString(9), reader.GetString(10));
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        
        }
        private void frm_preciosMercaderia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if (dataGridView1.RowCount > 0)
                {
                    if (estado)
                        precio = dataGridView1.Rows[1].Cells[1].Value.ToString();
                    else
                        precio = dataGridView1.Rows[0].Cells[1].Value.ToString();
                    DialogResult = DialogResult.OK;
                    SendKeys.Send("{ENTER}");
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (estado)
                   precio = dataGridView1.Rows[1].Cells[1].Value.ToString();
                else
                    precio = dataGridView1.Rows[0].Cells[1].Value.ToString();
                DialogResult = DialogResult.OK;
                SendKeys.Send("{ENTER}");
            }
            else
            {
                this.Close();
            }
            
        }
        internal frm_venta.preciosProductos currentPrecio
        {
            get
            {
                return new frm_venta.preciosProductos()
                {
                    getPrice = precio
                };
            }
            set
            {
                currentPrecio.getPrice = precio;
            }

        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dataGridView1.RowCount > 0)
                {
                    precio = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
           
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                precio = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                DialogResult = DialogResult.OK;
                SendKeys.Send("{ENTER}");
            }
        }

        private void frm_preciosMercaderia_Load(object sender, EventArgs e)
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
