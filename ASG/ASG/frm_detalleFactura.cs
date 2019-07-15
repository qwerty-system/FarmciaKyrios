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
    public partial class frm_detalleFactura : Form
    {
        string idFactura;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string nombre;
        string direccion;
        string codigo;
        string vendedor;
        string totalF;
        string terminos;
        int indexer;
        public frm_detalleFactura(string factura, string subtotal, string descuento, string total, string cliente, string fecha, bool estado, string vendedorC, string terminosC, string direccionC, string codigoC, int index, bool state)
        {
            InitializeComponent();
            if (state)
            {
                idFactura = factura;
                totalF = total;
                nombre = cliente;
                indexer = index;
                direccion = direccionC;
                vendedor = vendedorC;
                codigo = codigoC;
                label20.Text = codigo;
                label16.Text = direccion;
                label18.Text = vendedor;
                terminos = terminosC;
                label22.Text = terminos;
                label9.Text = factura;
                label5.Text = subtotal;
                label1.Text = descuento;
                label7.Text = total;
                label8.Text = cliente;
                label15.Text = fecha;
            } else
            {
                idFactura = factura;
                totalF = total;
                nombre = cliente;
                direccion = direccionC;
               
                codigo = codigoC;
                label9.Text = factura;
                indexer = index;
                codigo = codigoC;
                label8.Text = cliente;
                label20.Text = codigo;
                terminos = terminosC;
                label22.Text = terminos;
               // button3.Enabled = false;
                cargaDatosDB();
            }
            
            if (estado)
            {
                cargaDetalle();
            } else
            {
                cargaDetalleProveedor();
            }
           
        }
        private void cargaDatosDB()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT DIRECCION_CLIENTE FROM CLIENTE WHERE ID_CLIENTE = '{0}' ;", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    label16.Text = reader.GetString(0);
                    direccion = label16.Text;
                    cargaFactura();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void cargaFactura()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT SUBTOTAL_FACTURA, DESCUENTO_FACTURA, TOTAL_FACTURA, (SELECT NOMBRE_USUARIO FROM USUARIO WHERE ID_USUARIO = VENDEDOR),FECHA_EMISION_FACTURA FROM FACTURA WHERE ID_FACTURA  = '{0}' ;", idFactura);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    label5.Text = string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(0));
                    label1.Text = string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(1));
                    label7.Text = string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(2));
                    label18.Text = reader.GetString(3);
                    vendedor = label18.Text;
                    label15.Text = reader.GetString(4);
                    totalF = label7.Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void cargaDetalleProveedor()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT D.ID_MERCADERIA, D.DESCRIPCION_DETALLE, D.CANTIDAD_DET_FACTURA, D.SUBTOTAL_DETALLE,D.DESCUENTO_DET_FACTURA,D.TOTAL_DET_FACTURA FROM DETALLE_COTIZACION D WHERE ID_FACTURA = '{0}' AND D.ESTADO_TUPLA = TRUE;", idFactura);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("{0:###,###,###,##0.00##} %", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                        //styleDV(this.dataGridView1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }

        private void cargaDetalle()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT D.ID_MERCADERIA, D.DESCRIPCION_DETALLE, D.CANTIDAD_DET_FACTURA, D.SUBTOTAL_DETALLE,D.DESCUENTO_DET_FACTURA,D.TOTAL_DET_FACTURA FROM DET_FACTURA D WHERE ID_FACTURA = '{0}' AND D.ESTADO_TUPLA = TRUE;", idFactura);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1),  reader.GetString(2), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(3)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)));
                        //styleDV(this.dataGridView1);
                    }
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }

        private void frm_detalleFactura_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.P))
            {
                button3.PerformClick();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_detalleFactura_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_detalleFactura_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_detalleFactura_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void frm_detalleFactura_Load(object sender, EventArgs e)
        {

        }
        private void contruyeImpresion()
        {
            DataSetVenta ds = new DataSetVenta();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                ds.Tables[0].Rows.Add
                    (new object[]
                    {
                                    dataGridView1[2,i].Value.ToString(),
                                    dataGridView1[1,i].Value.ToString(),
                                    dataGridView1[3,i].Value.ToString(),
                                    dataGridView1[5,i].Value.ToString()
                    });
            }
            frm_reporteVenta frm = new frm_reporteVenta();
            CrystalReport4 cr = new CrystalReport4();
            cr.SetDataSource(ds);
            TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["numero"];
            text.Text = label9.Text;
            TextObject textn = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["nombre"];
            textn.Text = nombre;
            TextObject textd = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["direccion"];
            textd.Text = direccion;
            TextObject textc = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["codigo"];
            textc.Text = codigo;
            TextObject textv = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["vendedor"];
            textv.Text = vendedor;
            TextObject textt = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["terminos"];
            textt.Text = terminos;
            TextObject texttotal = (TextObject)cr.ReportDefinition.Sections["Section4"].ReportObjects["total"];
            texttotal.Text = totalF;
            TextObject texttotaletra = (TextObject)cr.ReportDefinition.Sections["Section5"].ReportObjects["letters"];
            numberToString nt = new numberToString();
            string temp = totalF;
            temp = temp.Replace("Q.", "");
            texttotaletra.Text = nt.enletras(temp) + " Q.";
            frm.crystalReportViewer1.ReportSource = cr;
            frm.ShowDialog();
        }
        private void contruyeCotizacionCliente()
        {
            DataSetVenta ds = new DataSetVenta();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                ds.Tables[0].Rows.Add
                    (new object[]
                    {
                                    dataGridView1[2,i].Value.ToString(),
                                    dataGridView1[1,i].Value.ToString(),
                                    dataGridView1[3,i].Value.ToString(),
                                    dataGridView1[5,i].Value.ToString()
                    });
            }
            frm_reporteVenta frm = new frm_reporteVenta();
            CrystalReport9 cr = new CrystalReport9();
            cr.SetDataSource(ds);
            TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["numero"];
            text.Text = label9.Text;
            TextObject textn = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["nombre"];
            textn.Text = nombre;
            TextObject textd = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["direccion"];
            textd.Text = direccion;
            TextObject textc = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["codigo"];
            textc.Text = codigo;
            TextObject textv = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["vendedor"];
            textv.Text = vendedor;
            TextObject textt = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["terminos"];
            textt.Text = terminos;
            TextObject texttotal = (TextObject)cr.ReportDefinition.Sections["Section4"].ReportObjects["total"];
            texttotal.Text = totalF;
            TextObject texttotaletra = (TextObject)cr.ReportDefinition.Sections["Section5"].ReportObjects["letters"];
            numberToString nt = new numberToString();
            string temp = totalF;
            temp = temp.Replace("Q.", "");
            texttotaletra.Text = nt.enletras(temp) + " Q.";
            frm.crystalReportViewer1.ReportSource = cr;
            frm.ShowDialog();
        }
        private void contruyeCotizacionProveedor()
        {
            DataSetVenta ds = new DataSetVenta();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                ds.Tables[0].Rows.Add
                    (new object[]
                    {
                                    dataGridView1[2,i].Value.ToString(),
                                    dataGridView1[1,i].Value.ToString(),
                                    dataGridView1[3,i].Value.ToString(),
                                    dataGridView1[5,i].Value.ToString()
                    });
            }
            frm_reporteVenta frm = new frm_reporteVenta();
            CrystalReport10 cr = new CrystalReport10();
            cr.SetDataSource(ds);
            TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["numero"];
            text.Text = label9.Text;
            TextObject textn = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["nombre"];
            textn.Text = nombre;
            TextObject textd = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["direccion"];
            textd.Text = direccion;
            TextObject textc = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["codigo"];
            textc.Text = codigo;
            TextObject textv = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["vendedor"];
            textv.Text = vendedor;
            TextObject textt = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["terminos"];
            textt.Text = terminos;
            TextObject texttotal = (TextObject)cr.ReportDefinition.Sections["Section4"].ReportObjects["total"];
            texttotal.Text = totalF;
            TextObject texttotaletra = (TextObject)cr.ReportDefinition.Sections["Section5"].ReportObjects["letters"];
            numberToString nt = new numberToString();
            string temp = totalF;
            temp = temp.Replace("Q.", "");
            texttotaletra.Text = nt.enletras(temp) + " Q.";
            frm.crystalReportViewer1.ReportSource = cr;
            frm.ShowDialog();
        }
        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                if (indexer == 0)
                {
                    contruyeImpresion();
                } else if(indexer == 1)
                {
                    contruyeCotizacionCliente();
                } else 
                {
                    contruyeCotizacionProveedor();
                }
            } else
            {
                MessageBox.Show("IMPOSILE IMPRIMIR FACTURA VACIA!", "FACTURAS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
