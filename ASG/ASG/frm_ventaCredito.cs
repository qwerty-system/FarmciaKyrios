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
    public partial class frm_ventaCredito : Form
    {
        double subtotalFactura;
        double descuentoFactura;
        double totalFactura;
        string codigo;
        string cliente;
        string numeroCuenta;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string fecha;
        string monto;
        string numero;
        string codigoCaja;
        public frm_ventaCredito(double subtotal, double descuento, double total, string nit, string nombres, string cuenta, string codigoC)
        {
            InitializeComponent();
            codigoCaja = codigoC;
            subtotalFactura = subtotal;
            descuentoFactura = descuento;
            totalFactura = total;
            codigo = nit;
            cliente = nombres;
            numeroCuenta = cuenta;
            //MessageBox.Show(numeroCuenta);
            iniciaFormulario();
        }
        private void obtienResCaja(string numero)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CAJA SET TOTAL_FACTURADO_CAJA = TOTAL_FACTURADO_CAJA + {0} WHERE ID_CAJA = {1};", numero, codigoCaja);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {

                }
                else
                {
                    MessageBox.Show("IIMPOSIBLE ACTUALIZAR CAJA!", "GESTION CAJA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            
        }
        private void iniciaFormulario()
        {
            label1.Text = string.Format("Q.{0:###,###,###,##0.00##}", subtotalFactura);
            label5.Text = string.Format("Q.{0:###,###,###,##0.00##}", descuentoFactura);
            label7.Text = string.Format("Q.{0:###,###,###,##0.00##}", totalFactura);
            label9.Text = codigo;
            label12.Text = cliente;
            textBox5.Focus();
        }
       
        private void frm_ventaCredito_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_ventaCredito_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;

        }

        private void frm_ventaCredito_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_ventaCredito_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))

            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                if(textBox5.Text != "" && textBox5.Text != "0")
                {
                    button1.PerformClick();
                }
            } else if (e.KeyData == Keys.Space)
            {
                textBox5.Text = ""+ totalFactura;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
         // ABONO DE PAGO DE EL CLIENTE
        private void abonaPagoCliente()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("INSERT INTO PAGO_CLIENTE VALUES (NULL,{0},{1},{2},{3},NOW(),TRUE,TRUE);", numeroCuenta, 0, textBox5.Text, textBox5.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    updateAbono(textBox5.Text.Trim());
                }
                else
                {
                    MessageBox.Show("IMPOSILE ABONAR PAGO DE CLIETE!", "NUEVA VENTA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close(); 
        }
        // ACTUALIZA DISPONIBLE DE EL USAURIO
        private void updateDisponible(string abono)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CLIENTE SET CREDITO_DISPONIBLE = CREDITO_DISPONIBLE + {0} WHERE ID_CLIENTE = '{1}';", abono, codigo);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {

                    var forma = new frm_creditoActualizado();
                    forma.ShowDialog();
                }
                else
                {
                    MessageBox.Show("IMPOSILE ABONAR PAGO DE CLIETE!", "NUEVA VENTA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        // ACTUALIZA LA CANTIDAD DE ABONO EN LA CUENTA PRINCIPAL
        private void updateAbono(string abono)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CUENTA_POR_COBRAR_CLIENTE SET TOTAL_PAGADO = TOTAL_PAGADO + {0} WHERE ID_CUENTA_POR_COBRAR = {1};",abono, numeroCuenta);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    updateDisponible(textBox5.Text);
                }
                else
                {
                    MessageBox.Show("IMPOSILE ABONAR PAGO DE CLIETE!", "NUEVA VENTA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
         // CANCELA LA CUENTA POR QUE EL PAGO SE HA EFECTUADO EN SU TOTALIDAD
        private void cancelaCuenta(string abono)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CUENTA_POR_COBRAR_CLIENTE SET TOTAL_PAGADO = TOTAL_PAGADO + {0}, ESTADO_TUPLA = FALSE WHERE ID_CUENTA_POR_COBRAR = {1};", abono, numeroCuenta);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    updateDisponible(textBox5.Text);
                }
                else
                {
                    MessageBox.Show("IMPOSILE ABONAR PAGO DE CLIETE!", "NUEVA VENTA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
         // INGRESA EL ULTIMO PAGO EEN EL RECORD DE PAGOS PARA CANCELAR LA CUENTA
        private void ingresaPagos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("INSERT INTO PAGO_CLIENTE VALUES (NULL,{0},{1},{2},{3},NOW(),TRUE,TRUE);", numeroCuenta, 0, textBox5.Text, textBox5.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    cancelaCuenta(textBox5.Text.Trim());
                }
                else
                {
                    MessageBox.Show("IMPOSILE ABONAR PAGO DE CLIETE!", "NUEVA VENTA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        // obtiene coddidog para impresion de documentos
        private void obtieneCodigo()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT MAX(ID_PAGO) FROM PAGO_CLIENTE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    numero = reader.GetString(0);
                    fecha = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                    monto = string.Format("Q.{0:###,###,###,##0.00##}", Convert.ToDouble(textBox5.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void contruyeImpresion()
        {
            if (numero!= "" && fecha != "" && monto != "")
            {
               
                frm_reporteAbono frm = new frm_reporteAbono();
                CrystalReport1 cr = new CrystalReport1();
                TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["numerotxt"];
                text.Text = numero;
                TextObject fechas = (TextObject)cr.ReportDefinition.Sections["Section2"].ReportObjects["fecha"];
                fechas.Text = fecha;
                TextObject observacion = (TextObject)cr.ReportDefinition.Sections["Section2"].ReportObjects["observaciones"];
                observacion.Text =textBox1.Text;
                TextObject descripcion = (TextObject)cr.ReportDefinition.Sections["Section3"].ReportObjects["descripcion"];
                descripcion.Text = "PAGO RECIBIDO DEL CLIENTE";
                TextObject montos = (TextObject)cr.ReportDefinition.Sections["Section3"].ReportObjects["monto"];
                montos.Text = monto;
                frm.crystalReportViewer1.ReportSource = cr;
               if(frm.ShowDialog() == DialogResult.OK)
                {
                    button5.PerformClick();
                }
            }
            else
            {
                MessageBox.Show("DEBE DE LLENAR TODOS LOS CAMPOS", "ABONO CLIENTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // PERFORM CLICK DEL BOOTN ARA POCEDER CON EL RESTO DE PROCESOS DE ACTUALIZACION DE CREDITOS
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox5.Text != "" && textBox5.Text != "0")
            {
                if(Convert.ToDouble(textBox5.Text) == totalFactura)
                {
                  DialogResult result  = MessageBox.Show("DESEA CANCELAR EL TOTAL DE LA FACTURA?", "VENTA AL CREDITO", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        obtienResCaja(textBox5.Text);
                        ingresaPagos();
                        obtieneCodigo();
                        DialogResult resultado;
                        resultado = MessageBox.Show("¿DESEA IMPRIMIR UN COMPROBANTE DE PAGO AL CLIENTE?", "ABONO CLIENTE", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (resultado == System.Windows.Forms.DialogResult.Yes)
                        {
                            contruyeImpresion();                           
                        } else
                        {
                            button5.PerformClick();
                        }
                    }
                } else if(Convert.ToDouble(textBox5.Text) > totalFactura)
                {
                    MessageBox.Show("EL VALOR DEL ABONO DEBE SER MENOR O IGUAL AL TOTAL DE LA FACTURA", "VENTA CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    obtienResCaja(textBox5.Text);
                    abonaPagoCliente();
                    obtieneCodigo();
                    DialogResult resultado;
                    resultado = MessageBox.Show("¿DESEA IMPRIMIR UN COMPROBANTE DE PAGO AL CLIENTE?", "ABONO CLIENTE", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (resultado == System.Windows.Forms.DialogResult.Yes)
                    {
                        contruyeImpresion();
                    }
                    else
                    {
                        button5.PerformClick();
                    }
                }
            } else
            {
                MessageBox.Show("INGRESA EL TOTAL DEL ABONO", "NUEVA VENTA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
           
        }

        private void frm_ventaCredito_Load(object sender, EventArgs e)
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
