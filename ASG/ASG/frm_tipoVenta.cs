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
    public partial class frm_tipoVenta : Form
    {
        string codigoProducto;
        string descripcionMercaderia;
        string fechaIngreso;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string nombreSucursal;
        string usuarioRol;
        string clasificacionMercaderia;
        bool flag = false;
        ContextMenuStrip mymenu = new ContextMenuStrip();
        public frm_tipoVenta(string codigo,string descripcion, string disponible, string fecha, string sucursal, string rol, string clasificacion)
        {
            InitializeComponent();
            codigoProducto = codigo;
            descripcionMercaderia = descripcion;
            clasificacionMercaderia = clasificacion;
            fechaIngreso = fecha;
            if(clasificacion == "ESPECIAS")
            {
                label13.Text = label13.Text + "Especias";
                pictureBox1.Image = ASG.Properties.Resources.stalk__2_;
            } else
            {
                label13.Text = label13.Text + "Abarrotes";
                pictureBox1.Image = ASG.Properties.Resources.shelf__2_;
            }
            stripMenu();        
            label1.Text = label1.Text + " " + codigoProducto;
            label2.Text = label2.Text + " " + descripcionMercaderia;
            label5.Text = label5.Text + " " + disponible;
            label6.Text = label6.Text + " " + fecha;
            label11.Text = label11.Text + " " + sucursal;
            nombreSucursal = sucursal;
            
            initFormualrio();
                
            usuarioRol = rol;
            if(rol != "ADMINISTRADOR" && rol != "ROOT" && rol != "DATA BASE ADMIN")
            {
                dataGridView1.Columns[5].Visible = false;
            }
           
        }
        private void stripMenu()
        {
            mymenu.Items.Add("Ocultar Fila");
            mymenu.Items[0].Name = "ColHidden";
            mymenu.Items.Add("Ver Sub-Codigos");
            mymenu.Items[1].Name = "ColEdit";
        }
        private void my_menu_Itemclicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (flag == false)
            {
                if (e.ClickedItem.Name == "ColHidden")
                {
                    dataGridView1.Rows[this.dataGridView1.CurrentRow.Index].Visible = false;
                    flag = true;
                }
                else if (e.ClickedItem.Name == "ColEdit")
                {
                    if (dataGridView1.RowCount > 0)
                    {
                        var forma = new frm_alterCodes(dataGridView1.CurrentRow.Cells[0].Value.ToString(), label2.Text + " - " + dataGridView1.CurrentRow.Cells[1].Value.ToString(), 0);
                        forma.ShowDialog();
                    }
                    mymenu.Visible = false;
                    flag = true;

                }
               
            }
        }
        private void initFormualrio()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_STOCK WHERE ID_MERCADERIA = '{0}' AND NOMBRE_SUCURSAL = '{1}';", codigoProducto, nombreSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0),reader.GetString(1), reader.GetString(2), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("{0:###,###,###,##0.00##}", reader.GetString(4)), string.Format("Q.{0:###,###,###,##0.00##}",reader.GetDouble(5)), string.Format("Q.{0:###,###,###,##0.00##}",reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}",reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}",reader.GetDouble(8)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(9)), string.Format("Q.{0:###,###,###,##0.00##}" ,reader.GetDouble(10)), string.Format("{0:###,###,###,##0.00##} %",reader.GetDouble(11)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(12)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(13)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(14)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(15)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("{0:###,###,###,##0.00##}", reader.GetString(4)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(8)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(9)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(11)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(12)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(13)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(14)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(15)));
                        //styleDV(this.dataGridView1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
     private void frm_tipoVenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.D))
            {
                button2.PerformClick();
            }
        }

        private void frm_tipoVenta_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_tipoVenta_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }
        private void frm_tipoVenta_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var forma = new frm_vistaGeneral(codigoProducto, descripcionMercaderia, fechaIngreso,0, usuarioRol);
            forma.ShowDialog();
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                var forma = new frm_alterCodes(dataGridView1.CurrentRow.Cells[0].Value.ToString(), label2.Text + " - " + dataGridView1.CurrentRow.Cells[1].Value.ToString(), 0);
                forma.ShowDialog();
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    mymenu.Show(dataGridView1, new Point(e.X, e.Y));
                    mymenu.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_Itemclicked);
                    mymenu.Enabled = true;
                    flag = false;
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

        private void frm_tipoVenta_Load(object sender, EventArgs e)
        {

        }
    }
}
