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
    public partial class frm_vistaGeneral : Form
    {

        string descripcionProducto;
        string fechaIngreso;
        string codigoProducto;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public frm_vistaGeneral(string codigo, string descripcion, string fecha, int indexer, string rol)
        {
            InitializeComponent();
            codigoProducto = codigo;
            descripcionProducto = descripcion;
            fechaIngreso = fecha;
            iniciaFormulario();
            obtieneDatos();
            if (indexer > 0)
            {
                this.BackColor = Color.FromArgb(23, 33, 43);
            }
            if(rol != "ADMINISTRADOR" && rol != "ROOT" && rol != "DATA BASE ADMIN")
            {
                dataGridView1.Columns[4].Visible = false;
                this.dataGridView1.Size = new Size(620,247);
                this.dataGridView1.Location = new Point(70,170);
            }
        }
        private void iniciaFormulario()
        {
            // establece propiedades de vista del formulario
            label1.Text = codigoProducto;
            label2.Text = descripcionProducto;
            label6.Text = fechaIngreso;
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       
        private double getTotal()
        {
            if(dataGridView1.RowCount > 0)
            {
                double total = 0;
                for (int i = 0; i<dataGridView1.RowCount; i++)
                {
                    total = total + Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value);
                }
                return total;
            } else
            {
                return 0;
            }
        }
        private void obtieneDatos()
        {
            // funcion para llenar la vista de los productos
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_GENERAL_PRODUCTO WHERE ID_MERCADERIA = '{0}';", codigoProducto);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0),reader.GetString(1), reader.GetString(2), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(3)),string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("Q{0:###,###,###,##0.00##}", reader.GetDouble(4)));
                        //styleDV(this.dataGridView1);
                    }
                    label5.Text = ""+ getTotal();
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void frm_vistaGeneral_Load(object sender, EventArgs e)
        {

        }

        private void frm_vistaGeneral_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_vistaGeneral_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_vistaGeneral_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_vistaGeneral_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }
    }
}
