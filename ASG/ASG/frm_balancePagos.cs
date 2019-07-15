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
    public partial class frm_balancePagos : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        double totalAdeudado;
        string rolUsuario;
        string codigoProveedor;
        string nombreProveedor;
        bool flag = false;
        bool estado = false;
        string nombreSucursal;
        string nombreUsuario;
        string direccion;
        ContextMenuStrip mymenu = new ContextMenuStrip();
        public frm_balancePagos(string codigo, string nombre, string rol, bool status, string sucursal, string usuario, string direc)
        {
            InitializeComponent();
            rolUsuario = rol;
            estado = status;
            direccion = direc;
            codigoProveedor = codigo;
            nombreProveedor = nombre;
            nombreSucursal = sucursal;
            nombreUsuario = usuario;
            label2.Text = codigoProveedor;
            label6.Text = nombreProveedor;
            stripMenu();
           
            if (rolUsuario != "ADMINISTRADOR")
            {
                mymenu.Items[1].Enabled = false;
            }
            if ((dataGridView1.RowCount == 1))
            {
                button1.Enabled = false;
            }
            if (estado)
                cargaDatos();
            else
            {
                //MessageBox.Show("cancelados");
                cargaDatosCancelados();
                button8.Enabled = false;
                button1.Enabled = false;
            }

        }
        private void eliminaCompra()
        {
            DialogResult result = MessageBox.Show("¿ESTA SEGURO QUE DESEA ELIMINAR LA COMPRA DE LA CUENTA POR PAGAR?", "BALANCE PROVEEDORES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    string sql = string.Format("UPDATE CUENTA_POR_PAGAR SET ESTADO_CUENTA = FALSE WHERE ID_CUENTA_POR_PAGAR = '{0}';", dataGridView1.CurrentRow.Cells[6].Value.ToString());
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        var forma = new frm_creditoActualizado();
                        forma.ShowDialog();
                        cargaDatos();
                    }
                    else
                    {
                        MessageBox.Show("IMPOSIBLE ELIMINAR FACTURA!", "GESTION COMPRAS", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                conexion.Close();
            }
        }
        private void cargaDatosCancelados()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM  VISTA_CUENTA_PAGARF WHERE ID_PROVEEDOR = '{0}' ORDER BY FECHA_COMPRA DESC;", codigoProveedor);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {

                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6));
                    }
                    totalDeuda();
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
                string sql = string.Format("SELECT * FROM VISTA_CUENTA_PAGAR WHERE ID_PROVEEDOR = '{0}' ORDER BY FECHA_COMPRA DESC;", codigoProveedor);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                   
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6));
                    }
                    totalDeuda();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void stripMenu()
        {
            mymenu.Items.Add("Ocultar Fila");
            mymenu.Items[0].Name = "ColHidden";
            mymenu.Items.Add("Elimitar Compra");
            mymenu.Items[1].Name = "ColEdit";
            mymenu.Items.Add("Ver Abonos al Proveedor");
            mymenu.Items[2].Name = "ColDelete";
            mymenu.Items.Add("Ver Detalle de Compra");
            mymenu.Items[3].Name = "ColAs";

        }
        private void totalDeuda()
        {
            if (dataGridView1.RowCount > 0)
            {
                totalAdeudado = 0;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    totalAdeudado = totalAdeudado + (Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value.ToString()) - Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value.ToString()));
                }
                label16.Text = string.Format("Q.{0:###,###,###,##0.00##}", totalAdeudado);
            }
        }
        private void frm_balancePagos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                DialogResult = DialogResult.OK;
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.P))
            {
                button3.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.D))
            {
                button4.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.T))
            {
                button1.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.N))
            {
                button8.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.A))
            {
                button2.PerformClick();
            }
        }

        private void frm_balancePagos_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_balancePagos_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_balancePagos_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
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
                    if (dataGridView1.RowCount > 0)
                    {
                        eliminaCompra();
                    }
                    flag = true;

                }
                else if (e.ClickedItem.Name == "ColDelete")
                {
                    mymenu.Visible = false;
                    mymenu.Enabled = false;
                    flag = true;
                    button2.PerformClick();
                }
                else if (e.ClickedItem.Name == "ColAs")
                {
                    mymenu.Visible = false;
                    mymenu.Enabled = false;

                    flag = true;
                    if (dataGridView1.RowCount > 0)
                    {
                        var forma = new frm_detalleCompra(dataGridView1.CurrentRow.Cells[0].Value.ToString(), null, null, null, false);
                        forma.ShowDialog();
                    }
                }

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

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                double cast = Convert.ToDouble(dataGridView1.CurrentRow.Cells[4].Value.ToString()) - Convert.ToDouble(dataGridView1.CurrentRow.Cells[5].Value.ToString());
                //MessageBox.Show("" + cast);
                var forma = new frm_abonoProveedor(dataGridView1.CurrentRow.Cells[6].Value.ToString(), cast, codigoProveedor, true,nombreSucursal,nombreUsuario);
                if (forma.ShowDialog() == DialogResult.OK)
                {
                    cargaDatos();
                    if (dataGridView1.RowCount == 0)
                    {
                        DialogResult = DialogResult.OK;

                    }
                }
            }
        }
        private void cancelaCuenta(double abono, string numeroCuenta)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CUENTA_POR_PAGAR SET  ESTADO_TUPLA = FALSE WHERE ID_CUENTA_POR_PAGAR = '{1}';", abono, numeroCuenta);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {

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
        private void updateAbono(double abonoCuenta, string numeroCuenta)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                //MessageBox.Show("" + abonoCuenta);
                string sql = string.Format("CALL ACTUALIZA_CUENTAP({0},'{1}')", abonoCuenta, numeroCuenta);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    cancelaCuenta(abonoCuenta, numeroCuenta);
                }
                else
                {
                    MessageBox.Show("IMPOSILE ABONAR PAGO AL PROVEEDOR!", "CUENTAS POR PAGAR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void abonaPagoCliente(string numeroCuenta, double abono)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("INSERT INTO PAGO_CUENTA VALUES (NULL,'{0}',{1},{2},{3},NOW(),TRUE);", numeroCuenta, abono, 0, abono);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    updateAbono(abono, numeroCuenta);
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
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR EL TOTAL DE TODAS LAS COMPRAS?", "CUENTA POR PAGAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
               
                var forma = new frm_abonoProveedor(null, totalAdeudado, codigoProveedor, false,nombreSucursal,nombreUsuario);
                if (forma.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {

                        double cast = (Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value.ToString()) - Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value.ToString()));
                        abonaPagoCliente(dataGridView1.Rows[i].Cells[6].Value.ToString(), cast);
                    }
                    cargaDatos();
                    adressUser.ingresaBitacora(nombreSucursal, nombreUsuario, "PAGO TOTAL", codigoProveedor);
                   
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                var forma = new frm_recordAbonos(label2.Text, label6.Text, dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString(),
                    dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(), estado,
                    dataGridView1.CurrentRow.Cells[4].Value.ToString(), rolUsuario,nombreSucursal,nombreUsuario);
                if (forma.ShowDialog() == DialogResult.OK)
                {
                    cargaDatos();
                }
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (estado)
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    dataGridView1.Rows.Clear();
                    string sql = string.Format("SELECT * FROM  VISTA_CUENTA_PAGAR WHERE ID_PROVEEDOR = '{0}' AND FECHA_COMPRA LIKE '%{1}%' ORDER BY FECHA_COMPRA DESC;", codigoProveedor, dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {

                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6));
                        }
                        totalDeuda();
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
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    dataGridView1.Rows.Clear();
                    string sql = string.Format("SELECT * FROM  VISTA_CUENTA_PAGARF WHERE ID_PROVEEDOR = '{0}' AND FECHA_COMPRA LIKE '%{1}%' ORDER BY FECHA_COMPRA DESC;", codigoProveedor, dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {

                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:###,###,###,##0.00##}", reader.GetDouble(5)), reader.GetString(6));
                        }
                        totalDeuda();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                conexion.Close();
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
        }
        private void contruyeImpresion()
        {
            DataSet1 ds = new DataSet1();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                ds.Tables[0].Rows.Add
                    (new object[]
                    {
                                    dataGridView1[0,i].Value.ToString(),
                                    dataGridView1[1,i].Value.ToString(),
                                    dataGridView1[2,i].Value.ToString(),
                                    dataGridView1[4,i].Value.ToString(),
                                    dataGridView1[5,i].Value.ToString()
                    });
            }
            frm_reporteProvedorBalance frm = new frm_reporteProvedorBalance();
            CrystalReport7 cr = new CrystalReport7();
            cr.SetDataSource(ds);
            TextObject textn = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["nombre"];
            textn.Text = label6.Text;
            TextObject textd = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["codigo"];
            textd.Text = label2.Text;
            TextObject textdirec = (TextObject)cr.ReportDefinition.Sections["Section1"].ReportObjects["direccion"];
            textdirec.Text = direccion;
            TextObject textotal = (TextObject)cr.ReportDefinition.Sections["Section5"].ReportObjects["total"];
            textotal.Text = label16.Text;
            frm.crystalReportViewer1.ReportSource = cr;
            frm.ShowDialog();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                contruyeImpresion();
            }
            else
            {
                MessageBox.Show("NO HAY DATOS A IMPRIMIR", "CUENTAS POR COBRAR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                button2.PerformClick();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                var forma = new frm_detalleCompra(dataGridView1.CurrentRow.Cells[0].Value.ToString(), null, null, null, false);
                forma.ShowDialog();
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

        private void frm_balancePagos_Load(object sender, EventArgs e)
        {

        }
    }
}
