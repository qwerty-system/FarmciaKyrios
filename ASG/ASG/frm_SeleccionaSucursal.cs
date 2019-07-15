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
    
    public partial class frm_SeleccionaSucursal : Form
    {
        int tipoImpresion;
        public frm_SeleccionaSucursal(int estado)
        {
            InitializeComponent();
            tipoImpresion = estado;
            cargaSucursales();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void cargaSucursales()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT NOMBRE_SUCURSAL FROM SUCURSAL WHERE ESTADO_TUPLA = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    comboBox2.Items.Clear();
                    // comboBox4.Items.Clear();
                    comboBox2.Items.Add(reader.GetString(0));

                    //comboBox4.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader.GetString(0));
                        //comboBox4.Items.Add(reader.GetString(0));
                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            if(comboBox2.Text != "")
            {
                if (tipoImpresion == 0)
                {
                    frm_reporteVenta frm = new frm_reporteVenta();
                    CrystalReport13 cr = new CrystalReport13();
                    TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["sucursal"];
                    text.Text = comboBox2.Text;
                    cr.SetParameterValue("sucursal", comboBox2.Text);
                    frm.crystalReportViewer1.ReportSource = cr;
                    frm.crystalReportViewer1.RefreshReport();
                    frm.ShowDialog();
                } else if(tipoImpresion == 1)
                {
                    frm_reporteVenta frm = new frm_reporteVenta();
                    CrystalReport8 cr = new CrystalReport8();
                    TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["sucursal"];
                    text.Text = comboBox2.Text;
                    cr.SetParameterValue("sucursal", comboBox2.Text);
                    frm.crystalReportViewer1.ReportSource = cr;
                    frm.crystalReportViewer1.RefreshReport();
                    frm.ShowDialog();
                }
                else if (tipoImpresion == 2)
                {
                    frm_reporteVenta frm = new frm_reporteVenta();
                    CrystalReport19 cr = new CrystalReport19();
                    TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["sucursal"];
                    text.Text = comboBox2.Text;
                    cr.SetParameterValue("sucursal", comboBox2.Text);
                    frm.crystalReportViewer1.ReportSource = cr;
                    frm.crystalReportViewer1.RefreshReport();
                    frm.ShowDialog();
                }
                else if (tipoImpresion == 3)
                {
                    frm_reporteVenta frm = new frm_reporteVenta();
                    CrystalReport25 cr = new CrystalReport25();
                    TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["sucursal"];
                    text.Text = comboBox2.Text;
                    cr.SetParameterValue("sucursal", comboBox2.Text);
                    frm.crystalReportViewer1.ReportSource = cr;
                    frm.crystalReportViewer1.RefreshReport();
                    frm.ShowDialog();
                }
            }
        }

        private void frm_SeleccionaSucursal_KeyDown(object sender, KeyEventArgs e)
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
