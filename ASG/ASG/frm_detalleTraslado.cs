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
    public partial class frm_detalleTraslado : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public frm_detalleTraslado(string codigo, string usuario, string origen, string destino, string fecha)
        {
            InitializeComponent();
            label7.Text = codigo;
            label8.Text = usuario;
            label9.Text = origen;
            label10.Text = fecha;
            label11.Text = destino;
            cargaDetalle(codigo);
        }

        private void frm_detalleTraslado_KeyDown(object sender, KeyEventArgs e)
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
        private void cargaDetalle(string codigo)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_DETALLE_TRASLADO WHERE ID_TRASLADO = {0};", codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void frm_detalleTraslado_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;

        }

        private void frm_detalleTraslado_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_detalleTraslado_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_MouseHover(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.Red;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.Transparent;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_detalleTraslado_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            frm_reporteVenta frm = new frm_reporteVenta();
            CrystalReport24 cr = new CrystalReport24();
            TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["nombre"];
            text.Text = label8.Text;
            TextObject textf = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["fecha"];
            textf.Text = label10.Text;
            TextObject textt = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["traslado"];
            textt.Text = "Traslado No. " + label7.Text;
            TextObject texto = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["origen"];
            texto.Text = label9.Text;
            TextObject textd = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["destino"];
            textd.Text = label11.Text;
            cr.SetParameterValue("traslado", label7.Text);
            frm.crystalReportViewer1.ReportSource = cr;
            frm.crystalReportViewer1.RefreshReport();
            frm.ShowDialog();
        }
    }
}
