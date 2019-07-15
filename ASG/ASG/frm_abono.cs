﻿using System;
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
using CrystalDecisions.Shared;


namespace ASG
{
    public partial class frm_abono : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        double totalFactura;
        string numeroCuenta;
        string codigoCliente;
        bool tipoAbono;
        string nombreSucursal;
        string usuarioNombre;
        public frm_abono(string cuenta, double total, string codigo, bool estado, string sucursal, string usuario)
        {
            InitializeComponent();
            numeroCuenta = cuenta;
            totalFactura = total;
            codigoCliente = codigo;
            nombreSucursal = sucursal;
            usuarioNombre = usuario;
            tipoAbono = estado;
            label15.Text = "" + total;

        }

        private void frm_abono_KeyDown(object sender, KeyEventArgs e)
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

        private void frm_abono_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_abono_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_abono_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void updateDisponible(string abono)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CLIENTE SET CREDITO_DISPONIBLE = CREDITO_DISPONIBLE + {0} WHERE ID_CLIENTE = '{1}';", abono, codigoCliente);
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
        private void updateAbono(string abono)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CUENTA_POR_COBRAR_CLIENTE SET TOTAL_PAGADO = TOTAL_PAGADO + {0} WHERE ID_CUENTA_POR_COBRAR = {1};", abono, numeroCuenta);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    updateDisponible(textBox5.Text);
                }
                else
                {
                    MessageBox.Show("IMPOSILE ABONAR PAGO DE CLIENTE!", "NUEVA VENTA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                //MessageBox.Show(""+totalFactura);
                string sql = string.Format("UPDATE CUENTA_POR_COBRAR_CLIENTE SET TOTAL_PAGADO = TOTAL_PAGADO + {0}, ESTADO_TUPLA = FALSE WHERE ID_CUENTA_POR_COBRAR = {1};", abono, numeroCuenta);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    updateDisponible(textBox5.Text);
                }
                else
                {
                    MessageBox.Show("IMPOSILE ABONAR PAGO DE CLIENTE!", "NUEVA VENTA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
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
                    label9.Text = reader.GetString(0);
                    textBox2.Text = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                    textBox6.Text = string.Format("Q.{0:###,###,###,##0.00##}", Convert.ToDouble(textBox5.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (tipoAbono)
            {
                if (textBox5.Text != "" && textBox5.Text != "0")
                {
                    if (Convert.ToDouble(textBox5.Text) == totalFactura)
                    {
                        DialogResult result = MessageBox.Show("DESEA CANCELAR EL TOTAL DE LA FACTURA?", "VENTA AL CREDITO", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            ingresaPagos();
                            obtieneCodigo();
                            textBox3.Text = "PAGO EFECTUADO POR TOTAL FACTURA";
                            adressUser.ingresaBitacora(nombreSucursal, usuarioNombre, "ABONO - TOTAL FACTURA", codigoCliente);
                            button1.Enabled = false;
                            button3.Enabled = false;
                        }
                    }
                    else if (Convert.ToDouble(textBox5.Text) > totalFactura)
                    {
                        MessageBox.Show("EL VALOR DEL ABONO DEBE SER MENOR O IGUAL AL TOTAL DE LA FACTURA", "VENTA CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        abonaPagoCliente();
                        obtieneCodigo();
                        textBox3.Text = "PAGO PARCIAL EFECTUADO DE FACTURA";
                        adressUser.ingresaBitacora(nombreSucursal, usuarioNombre, "ABONO - PARCIAL", codigoCliente);
                        button1.Enabled = false;
                        button3.Enabled = false;
                    }
                }
                else
                {
                    MessageBox.Show("INGRESO ELTOTAL DEL ABONO", "NUEVA VENTA - CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } else
            {
                if ((Convert.ToDouble(textBox5.Text) > totalFactura) || (Convert.ToDouble(textBox5.Text) < totalFactura))
                {
                    MessageBox.Show("EL VALOR DEL ABONO DEBE IGUAL AL TOTAL DE LA FACTURA", "VENTA CREDITO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                } else
                {
                    obtieneCodigo();
                    textBox3.Text = "PAGO TOTAL POR EL VALOR DE LAS FACTURAS";
                    button1.Enabled = false;
                    button3.Enabled = false;
                    var forma = new frm_creditoActualizado();
                    forma.ShowDialog();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == 's') || (e.KeyChar == 'S'))
            {
                textBox5.Text = label15.Text;
            }
            if ((e.KeyChar >= '0' & e.KeyChar <= '9') || (e.KeyChar == 08) || (e.KeyChar == 46))

            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if(textBox5.Text != "")
            {
                double cast = totalFactura - Convert.ToDouble(textBox5.Text);
                label2.Text = "" + cast;
            }
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (textBox5.Text != "")
                {
                    button1.PerformClick();
                    button2.Focus();
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox6.Text != "" && textBox3.Text != "" && textBox2.Text != "")
            {
                if (textBox4.Text == "") textBox4.Text = "   ";
                frm_reporteAbono frm = new frm_reporteAbono();
                CrystalReport1 cr = new CrystalReport1();
                TextObject text = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["numerotxt"];
                text.Text = label9.Text;
                TextObject fecha = (TextObject)cr.ReportDefinition.Sections["Section2"].ReportObjects["fecha"];
                fecha.Text = textBox2.Text;
                TextObject observacion = (TextObject)cr.ReportDefinition.Sections["Section2"].ReportObjects["observaciones"];
                observacion.Text = textBox4.Text;
                TextObject descripcion = (TextObject)cr.ReportDefinition.Sections["Section3"].ReportObjects["descripcion"];
                descripcion.Text = textBox3.Text;
                TextObject monto = (TextObject)cr.ReportDefinition.Sections["Section3"].ReportObjects["monto"];
                monto.Text = textBox6.Text;
                frm.crystalReportViewer1.ReportSource = cr;
                frm.Show();
            } else
            {
                MessageBox.Show("DEBE DE LLENAR TODOS LOS CAMPOS", "ABONO CLIENTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            textBox2.Text = dateTimePicker1.Value.ToString("yyyy-MM-dd");
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void frm_abono_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
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
