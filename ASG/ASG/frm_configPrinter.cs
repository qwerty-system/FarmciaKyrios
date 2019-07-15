using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ASG
{
    public partial class frm_configPrinter : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public frm_configPrinter()
        {
            InitializeComponent();
            foreach (String strPrinter in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                comboBox5.Items.Add(strPrinter);
            }
            cargaAjustes();
        }

        private void frm_configPrinter_Load(object sender, EventArgs e)
        {
           
        }
        private void cargaAjustes()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                
                string sql = string.Format("SELECT IMPRESORA, COPIAS, VISTA FROM CONFIGURACION_SISTEMA WHERE IDCONFIGURACION = 111;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    comboBox5.Text = reader.GetString(0);
                    textBox1.Text  = reader.GetString(1);
                    checkBox1.Checked = reader.GetBoolean(2);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_configPrinter_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_configPrinter_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_configPrinter_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_configPrinter_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox5.Text != "")
            {
                if(Convert.ToDouble(textBox1.Text.ToString()) > 0)
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    try
                    {

                        string sql = string.Format("UPDATE CONFIGURACION_SISTEMA SET IMPRESORA = '{0}' , COPIAS = {1} , VISTA = {2} WHERE IDCONFIGURACION = 111 ;", comboBox5.Text.Trim(), textBox1.Text, checkBox1.Checked);
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if ((cmd.ExecuteNonQuery() == 1) || (cmd.ExecuteNonQuery() == 0))
                        {
                            var fomra = new frm_creditoActualizado();
                            fomra.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("IMPOSIBLE GUARDAR CONFIGURACION!", "CONIGURACION SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    conexion.Close();

                } 
                else
                {
                    MessageBox.Show("EL NUMERO DE COPIAS DEBE SER MAYOR A UNO", "CONFIGURACION IMPRESION", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } else
            {
                MessageBox.Show("SELECCIONE UNA IIMPRESORA PARA CONTINUAR", "CONFIGURACION IMPRESION", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' & e.KeyChar <= '9') | (e.KeyChar == 08))

            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
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
