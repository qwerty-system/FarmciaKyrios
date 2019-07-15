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
    public partial class frm_getProducto : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string idMercaderia;
        string idSucursal;
        string idStock;
        string descripcion;
        string precio;
        string cantidad;
        string precio_costo;
        public frm_getProducto(string idProducto, string sucursal, string rol)
        {
            InitializeComponent();
            idMercaderia = idProducto;
            idSucursal = sucursal;
            obtieneProducto();
            if(rol != "ADMINISTRADOR")
            {
                dataGridView1.Columns[4].Visible = false;
                dataGridView1.Columns[10].Visible = false;
                dataGridView1.Columns[11].Visible = false;
                dataGridView1.Columns[12].Visible = false;
                dataGridView1.Columns[13].Visible = false;
                dataGridView1.Columns[14].Visible = false;
            }
        }
       
        private void obtieneStock()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_STOCK WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", idMercaderia, idSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("{0:#,###,###,###,###}", reader.GetDouble(3)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(8)), string.Format("{0:###,###,###,##0.00##} ", reader.GetDouble(9)), string.Format("{0:###,###,###,##0.00##} ", reader.GetDouble(10)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(11)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(12)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(13)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(14)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(15)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("{0:#,###,###,###,###}", reader.GetDouble(3)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(8)), string.Format("{0:###,###,###,##0.00##} ", reader.GetDouble(9)), string.Format("{0:###,###,###,##0.00##} ", reader.GetDouble(10)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(11)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(12)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(13)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(14)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(15)));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void obtieneProducto()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT M.ID_MERCADERIA, M.ID_SUCURSAL, M.TOTAL_UNIDADES, M.DESCRIPCION FROM MERCADERIA M WHERE ID_MERCADERIA = '{0}' AND M.ID_SUCURSAL = (SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL ='{1}' AND ESTADO_TUPLA = TRUE LIMIT 1);", idMercaderia, idSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    label2.Text = reader.GetString(3);
                    label5.Text = ""+reader.GetString(2);
                    obtieneStock();
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void cargaPresentaciones()
        {

        }
        private void frm_getProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_getProducto_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_getProducto_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_getProducto_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_getProducto_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (dataGridView1.RowCount > 0)
                {
                    idStock = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    if (dataGridView1.CurrentRow.Cells[1].Value.ToString() != "MEDIDA")
                    {
                        descripcion = label2.Text + " - " + dataGridView1.CurrentRow.Cells[1].Value.ToString() + " ( " + dataGridView1.CurrentRow.Cells[2].Value.ToString() + " )";
                    }
                    else
                    {
                        descripcion = label2.Text + " - " + dataGridView1.CurrentRow.Cells[1].Value.ToString();
                    }
                    precio = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                    cantidad = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                    precio_costo = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                    DialogResult = DialogResult.OK;
                }
            }
        }
         internal frm_venta.Producto currentProducto
        {
            get
            {
                return new frm_venta.Producto()
                {
                    getStock = idStock,
                    getDescripcion = descripcion,
                    currentSucursal = idSucursal,
                    getPrice = precio,
                    getCpp = cantidad,
                    getProducto = idMercaderia,
                    getPC = precio_costo                  
                };
            }
            set
            {
                currentProducto.getProducto = idMercaderia;
            }
        }
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                idStock = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                if (dataGridView1.CurrentRow.Cells[1].Value.ToString() != "MEDIDA")
                {
                    descripcion = label2.Text + " - " + dataGridView1.CurrentRow.Cells[1].Value.ToString() + " ( " + dataGridView1.CurrentRow.Cells[2].Value.ToString() + " )";
                }
                else
                {
                descripcion = label2.Text + " - " + dataGridView1.CurrentRow.Cells[1].Value.ToString();
                }
                precio = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                cantidad = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                precio_costo = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                DialogResult = DialogResult.OK;
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
