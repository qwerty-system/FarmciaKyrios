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
using CrystalDecisions.CrystalReports.Engine;

namespace ASG
{
    public partial class frm_detalleCaja : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string codigoCaja;
        string nombreUsuario;
        string usuarioSistema;
        string sucursalCodigo;
        string sucursalNombre;
        string fechaApertura;
        string montoApertura;
        bool flag = false;
        ContextMenuStrip mymenu = new ContextMenuStrip();
        public frm_detalleCaja(string caja, string usuario, string nombre, string codigosucursal, string nombresucursal, string fecha, string monto, string rol, string totalFacturado, bool estado, string observacion)
        {
            InitializeComponent();
            codigoCaja = caja;
            usuarioSistema = usuario;
            nombreUsuario = nombre;
            sucursalCodigo = codigosucursal;
            sucursalNombre = nombresucursal;
            fechaApertura = fecha;
            montoApertura = monto;
            label7.Text = totalFacturado;
            setterForm();
            cargaFacturas();
            cargaCreditos();
            cargaDatos();
            stripMenu();
            if (estado)
            {
                label17.Visible = true;
                label17.Text = observacion;
                label18.Visible = true;
                mymenu.Items[1].Enabled = false;
            }

        }
        private void stripMenu()
        {
            mymenu.Items.Add("Ocultar Fila");
            mymenu.Items[0].Name = "ColHidden";
            mymenu.Items.Add("Eliminar Detalle");
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

                    mymenu.Visible = false;
                    DialogResult result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR EL DETALLE DE LA CAJA?", "GESTION CAJA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        eliminaDetalle(dataGridView1.CurrentRow.Cells[6].Value.ToString(), dataGridView1.CurrentRow.Cells[5].Value.ToString());
                    }

                    flag = true;

                }
              
            }
        }
        private void eliminaDetalle(string codigos, string montos)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE DET_CAJA SET ESTADO_TUPLA = FALSE WHERE ID_DET_CAJA = {0};", codigos);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //.Show("SE GENERO LA CUENTA CORRECTAMENTE");
                    actualizaTotal(montos);
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
        private void actualizaTotal( string monto)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CAJA SET TOTAL_FACTURADO_CAJA = TOTAL_FACTURADO_CAJA - {0} WHERE ID_CAJA = {1};", monto, codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //.Show("SE GENERO LA CUENTA CORRECTAMENTE");
                    var forma = new frm_creditoActualizado();
                    forma.ShowDialog();
                    cargaCreditos();
                    cargaFacturas();
                    cargaDatos();
                }
                else
                {
                    MessageBox.Show("IMPOSIBLE ACTUALIZAR MONTO DE CAJA!", "CAJA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaDatos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("select * from caja_facturas where id_caja = {0};", codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0),reader.GetString(1) + " " +reader.GetString(2),reader.GetString(3), reader.GetString(4), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(7)),reader.GetString(8));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void setterForm()
        {
            label8.Text = nombreUsuario;
            label3.Text = fechaApertura;
            label1.Text = montoApertura;
        }
        private void frm_detalleCaja_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.P))
            {
                button8.PerformClick();
            }
            
        }
        private void cargaFacturas()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("select sum(total_factura) from caja_facturas  where tipo_factura = 'EFECTIVO' and id_caja = {0};", codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.IsDBNull(0))
                    {
                        label6.Text = string.Format("Q.{0:###,###,###,##0.00##}", 0);
                    }
                    else
                    {
                        label6.Text = string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(0));
                    }
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaCreditos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                
                string sql = string.Format("select sum(total_factura) from caja_facturas where tipo_factura = 'CREDITO' and  id_caja = {0};", codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.IsDBNull(0))
                    {
                        label10.Text = string.Format("Q.{0:###,###,###,##0.00##}", 0);
                    } else
                    {
                        label10.Text = string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(0));
                    }
                    
                    conexion.Close();
                }
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void frm_detalleCaja_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_detalleCaja_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_detalleCaja_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void cargaEfectivo()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("select * from caja_facturas where id_caja = {0} and tipo_factura = 'EFECTIVO';", codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {

                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaCredito()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("select * from caja_facturas where id_caja = {0} and tipo_factura = 'CREDITO';", codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1) + " " + reader.GetString(2), reader.GetString(3), reader.GetString(4), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(6)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox5.Text == "EFECTIVO")
            {
                cargaEfectivo();
            } else if(comboBox5.Text == "CREDITO")
            {
                cargaCredito();
            } else
            {
                cargaDatos();
            }
        }

        private void frm_detalleCaja_Load(object sender, EventArgs e)
        {

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
        private void contruyeImpresion()
        {
            DataSet2 ds = new DataSet2();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                ds.Tables[0].Rows.Add
                    (new object[]
                    {
                                    dataGridView1[0,i].Value.ToString(),
                                    dataGridView1[1,i].Value.ToString(),
                                    dataGridView1[2,i].Value.ToString(),
                                    dataGridView1[6,i].Value.ToString()
                    });
            }
            frm_reporteCaja frm = new frm_reporteCaja();
            CrystalReport3 cr = new CrystalReport3();
            cr.SetDataSource(ds);
            TextObject textn = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["nombre"];
            textn.Text = label8.Text;
            TextObject textd = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["fecha"];
            textd.Text = label3.Text;
            TextObject textc = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["monto"];
            textc.Text = label1.Text;
            TextObject textv = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["obser"];
            if (label17.Text != "")
            {
                textv.Text = label17.Text;
            } else
            {
                textv.Text = "NO SE HA CERRADO LA CAJA";
            }
            // totales del reporte //
            TextObject texe = (TextObject)cr.ReportDefinition.Sections["Section4"].ReportObjects["efectivo"];
            texe.Text = label6.Text;
            TextObject texc = (TextObject)cr.ReportDefinition.Sections["Section4"].ReportObjects["credito"];
            texc.Text = label10.Text;
            TextObject textt = (TextObject)cr.ReportDefinition.Sections["Section5"].ReportObjects["caja"];
            textt.Text = label7.Text;
            frm.crystalReportViewer1.ReportSource = cr;
            frm.ShowDialog();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                contruyeImpresion();
            }
            else
            {
                MessageBox.Show("NO HAY DATOS A IMPRIMIR", "GESTION CAJA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label14_Click(object sender, EventArgs e)
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

