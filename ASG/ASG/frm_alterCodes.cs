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
    public partial class frm_alterCodes : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string codigoMercaderia;
        bool flags;
        ContextMenuStrip mymenu = new ContextMenuStrip();
        public frm_alterCodes(string codigo, string descripcion, int marcador)
        {
            InitializeComponent();
            codigoMercaderia = codigo;
            label1.Text = codigoMercaderia;
            if (descripcion == "")
            {
                label5.Text = "..........";
            } else
                cargaCodigos();
            label5.Text = descripcion;
            if (marcador > 0)
            {      
                stripMenu();
                dataGridView1.Columns[1].ReadOnly = false;
            } else
            {
                button1.Visible = false;
                this.Opacity = 100;
               
            }
        }
        private void stripMenu()
        {
            mymenu.Items.Add("Ocultar Fila");
            mymenu.Items[0].Name = "ColHidden";
            mymenu.Items.Add("Eliminar Codigo Alterno");
            mymenu.Items[1].Name = "ColEdit";
        }
        private void my_menu_ItemclickedE(object sender, ToolStripItemClickedEventArgs e)
        {
            if (flags == false)
            {
                if (e.ClickedItem.Name == "ColHidden")
                {
                    dataGridView1.Rows[this.dataGridView1.CurrentRow.Index].Visible = false;
                    flags = true;
                }
                else if (e.ClickedItem.Name == "ColEdit")
                {
                    deleteCodigo(dataGridView1.CurrentRow.Cells[0].Value.ToString(),dataGridView1.CurrentRow.Cells[1].Value.ToString());
                    flags = true;
                    cargaCodigos();
                }
            }
        }
        private void deleteCodigo(string codigoMer, string codigoAlterno)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE SUBCODIGOS_MERCADERIA SET ESTADO_TUPLA = FALSE WHERE ID_STOCK = '{0}' AND SUBCODIGO = '{1}'", codigoMer, codigoAlterno);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    timerActions();
                }
                else
                {
                    MessageBox.Show("NO SE HA ELIMINADO EL CODIGO ALTERNO!", "GESTION COMPRAS", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void styleDV(DataGridView data)
        {
            data.RowsDefaultCellStyle.BackColor = Color.LightGray;
            data.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
        }
        private void cargaCodigos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM SUBCODIGOS_MERCADERIA WHERE ESTADO_TUPLA = TRUE AND ID_STOCK = '{0}';",codigoMercaderia);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1));
                        styleDV(this.dataGridView1);
                    }
                }
                else
                {
                    label24.Enabled = true;
                    pictureBox1.Enabled = true;
                    label24.Visible = true;
                    pictureBox1.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_alterCodes_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_alterCodes_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_alterCodes_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_alterCodes_Load(object sender, EventArgs e)
        {
            
        }
        private void timerActions()
        {
            frm_done frm = new frm_done();
            frm.ShowDialog();
            timer1.Interval = 1500;
            timer1.Enabled = true;
        }
        private void ingresaCodigos(string codigoAlterno)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL INSERT_ALTER_CODES ('{0}','{1}');", codigoMercaderia, codigoAlterno);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    
                }
            }
            catch (Exception)
            {
                
            }
            conexion.Close();
        }
        private bool enterData(string celdaData)
        {
            if (celdaData == "" | celdaData == " ")
                return false;
            else
                return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0) {
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    if (enterData(dataGridView1.Rows[i].Cells[0].Value.ToString()))
                    {
                        ingresaCodigos(dataGridView1.Rows[i].Cells[1].Value.ToString());
                    }
                }
                var forma = new frm_creditoActualizado();
                forma.ShowDialog();
            }
            else
            {
                MessageBox.Show("INGRESE UN COIDGO ALTERNO!", "GESTION COMPRAS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    mymenu.Show(dataGridView1, new Point(e.X, e.Y));
                    mymenu.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_ItemclickedE);
                    mymenu.Enabled = true;
                    flags = false;
                }
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
           /* if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString() == "") {
                dataGridView1.Rows[e.RowIndex].Cells[0].Value = codigoMercaderia;
            }*/
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].Cells[0].Value = codigoMercaderia;
        }

        private void frm_alterCodes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                button1.PerformClick();  
            }
        }

        private void label5_Click(object sender, EventArgs e)
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
