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
    public partial class frm_cotizacionesEdicion : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string cotizacion;
        string codigo;
        string tipo;
        string direccion;
        string nombre;
        string vendedor;
        string terminos;
        string fecha;
        
        public frm_cotizacionesEdicion(bool tipoCotizacion, string nombreSucursal)
        {
            InitializeComponent();
            cargaSucursales();
            comboBox1.Text = nombreSucursal;
            comboBox2.Text = "CLIENTES";
            if (tipoCotizacion)
            {
                cargaCotizaciones();
            } else
            {
                cargaCotizacionesProveedor();
            }
        }
        private void cargaCotizacionesProveedor()
        {

            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        //styleDV(this.dataGridView1);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
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
                    comboBox1.Items.Clear();
                    comboBox1.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader.GetString(0));
                    }
                    //comboBox1.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void cargaCotizaciones()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        //styleDV(this.dataGridView1);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void frm_cotizacionesEdicion_Load(object sender, EventArgs e)
        {

        }
        internal frm_mercaderia.Cotizacion currentCotizacion
        {
            get
            {
                return new frm_mercaderia.Cotizacion()
                {
                    getCodigoCliente = codigo,
                    getDireccion = direccion,
                    getNombre = nombre,
                    getFecha = fecha,
                    getTerminos = terminos,
                    getTipo = tipo,
                    getVendedor = vendedor,
                    getNumeroCotizacion = cotizacion
                };
            }
            set
            {
                currentCotizacion.getNumeroCotizacion = cotizacion;
            }

        }
        private void frm_cotizacionesEdicion_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_cotizacionesEdicion_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_cotizacionesEdicion_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_cotizacionesEdicion_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "CLIENTES")
            {
                cargaCotizaciones();
            }
            else if (comboBox1.Text == "PROVEEDORES")
            {
                cargaCotizacionesProveedor();
            }
        }
        private void cargaCotizacionesFecha()
        {

            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND FECHA_EMISION_FACTURA LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        //styleDV(this.dataGridView1);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void cargaCotizacionesProveedorFecha()
        {

            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND FECHA_EMISION_FACTURA LIKE  '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        //styleDV(this.dataGridView1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "CLIENTES")
            {
                cargaCotizacionesFecha();
            }
            else if (comboBox1.Text == "PROVEEDORES")
            {
                cargaCotizacionesProveedorFecha();
            }
        }
        private void BuscaCotizacionesProveedor()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND ID_FACTURA LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                        //styleDV(this.dataGridView1);
                    }
                }
                else
                {
                    dataGridView1.Rows.Clear();
                    sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND FECHA_EMISION_FACTURA LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                    cmd = new OdbcCommand(sql, conexion);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                            //styleDV(this.dataGridView1);
                        }
                    }
                    else
                    {
                        dataGridView1.Rows.Clear();
                        sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND NOMBRE_PROVEEDOR LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                //styleDV(this.dataGridView1);
                            }
                        }
                        else
                        {
                            dataGridView1.Rows.Clear();
                            sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND ID_PROVEEDOR LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                    //styleDV(this.dataGridView1);
                                }
                            }
                            else
                            {
                                dataGridView1.Rows.Clear();
                                sql = string.Format("SELECT * FROM VISTA_COTIZACION_PROVEEDOR WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND NOMBRE_USUARIO LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)));
                                        //styleDV(this.dataGridView1);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void buscaCotizaciones()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                dataGridView1.Rows.Clear();
                string sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND ID_FACTURA LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        //styleDV(this.dataGridView1);
                    }
                }
                else
                {
                    dataGridView1.Rows.Clear();
                    sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND FECHA_EMISION_FACTURA LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                    cmd = new OdbcCommand(sql, conexion);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                            //styleDV(this.dataGridView1);
                        }
                    }
                    else
                    {
                        dataGridView1.Rows.Clear();
                        sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND NOMBRE_CLIENTE LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                //styleDV(this.dataGridView1);
                            }
                        }
                        else
                        {
                            dataGridView1.Rows.Clear();
                            sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND APELLIDO_CLIENTE LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                    //styleDV(this.dataGridView1);
                                }
                            }
                            else
                            {
                                dataGridView1.Rows.Clear();
                                sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND ID_CLIENTE LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                        //styleDV(this.dataGridView1);
                                    }
                                }
                                else
                                {
                                    dataGridView1.Rows.Clear();
                                    sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND NOMBRE_USUARIO LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                        while (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                            //styleDV(this.dataGridView1);
                                        }
                                    }
                                    else
                                    {
                                        dataGridView1.Rows.Clear();
                                        sql = string.Format("SELECT * FROM VISTA_COTIZACION WHERE NOMBRE_SUCURSAL  = '{0}' AND ESTADO_TUPLA = TRUE AND ID_CLIENTE LIKE '%{1}%' ORDER BY FECHA_EMISION_FACTURA DESC;", comboBox1.Text, textBox1.Text.Trim());
                                        cmd = new OdbcCommand(sql, conexion);
                                        reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                            while (reader.Read())
                                            {
                                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2) + " " + reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(7)), reader.GetString(8), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(10)), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(11)), reader.GetString(13), reader.GetString(14));
                                                //styleDV(this.dataGridView1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION CLIENTES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
            private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "CLIENTES")
            {
                cargaCotizaciones();
            }
            else if (comboBox2.Text == "PROVEEDORES")
            {
                cargaCotizacionesProveedor();
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if (comboBox2.Text == "CLIENTES")
                {
                    buscaCotizaciones();
                }
                else if (comboBox2.Text == "PROVEEDORES")
                {
                    BuscaCotizacionesProveedor();
                }
            }
            else
            {
                if (comboBox2.Text == "CLIENTES")
                {
                    cargaCotizaciones();
                }
                else if (comboBox2.Text == "PROVEEDORES")
                {
                    cargaCotizacionesProveedor();
                }
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                cotizacion = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                codigo = dataGridView1.CurrentRow.Cells[10].Value.ToString();
                nombre = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                direccion = dataGridView1.CurrentRow.Cells[11].Value.ToString();
                vendedor = dataGridView1.CurrentRow.Cells[7].Value.ToString();
                terminos = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                fecha = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                tipo = comboBox2.Text;
                DialogResult = DialogResult.OK;
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                if (dataGridView1.RowCount > 0)
                {
                    cotizacion = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    codigo = dataGridView1.CurrentRow.Cells[10].Value.ToString();
                    nombre = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                    direccion = dataGridView1.CurrentRow.Cells[11].Value.ToString();
                    vendedor = dataGridView1.CurrentRow.Cells[7].Value.ToString();
                    terminos = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                    fecha = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                    tipo = comboBox2.Text;
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Down)
            {
                if (dataGridView1.RowCount > 0)
                {
                    dataGridView1.Focus();
                    if (dataGridView1.RowCount > 1)
                        this.dataGridView1.CurrentCell = this.dataGridView1[1, dataGridView1.CurrentRow.Index + 1];
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
    }
}
