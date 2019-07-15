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
    public partial class frm_busca : Form
    {
        string mercaderiaObtenida;
        int clasificacion;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string sucursalDestino;
        string descripcion;
        string cantidadTotal;
        string precioProducto;
        string proveedorMercaderia;
        public frm_busca(int x, string sucursal, string usuarioRol)
        {
            InitializeComponent();
            textBox1.Focus();
            sucursalDestino = sucursal;
            label2.Text = label2.Text + " " + sucursalDestino;
            clasificacion = x;
            if (x == 1)  {
                cargaMercaderia(x);
            } else if (x == 3)
            {
                cargaMercaderiatotal();
            }
            else if(x == 2)
            {
                cargaMercaderia(x);
            }
            if(usuarioRol != "ADMINISTRADOR")
            {
                dataGridView1.Columns[2].Visible = false;
            }
        }

        private void frm_busca_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }
        internal frm_trasladoLotes.CargaProductos CurrentProductos
        {
            get
            {
                return new frm_trasladoLotes.CargaProductos
                {
                    CodigoProducto = mercaderiaObtenida,
                    Descripcion = descripcion,
                    SucursalProducto = sucursalDestino,
                    TotalExistencia = cantidadTotal,
                    precioCompra = precioProducto,
                    proveedor = proveedorMercaderia
                };
            }
            set
            {
                CurrentProductos.CodigoProducto = mercaderiaObtenida;
                CurrentProductos.Descripcion = descripcion;
                CurrentProductos.SucursalProducto = sucursalDestino;
                CurrentProductos.TotalExistencia = cantidadTotal;
            }
        }
        private void cargaMercaderia(int x)
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE ID_CLASIFICACION = {0} AND ID_SUCURSAL = (SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{1}' LIMIT 1);", x, sucursalDestino);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                        //styleDV(this.dataGridView1);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void cargaMercaderiatotal()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE ID_SUCURSAL = (SELECT ID_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{0}' LIMIT 1);", sucursalDestino);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                        //styleDV(this.dataGridView1);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            if(clasificacion <= 2)
            {
                cargaMercaderia(clasificacion);
            }  else
            {
                cargaMercaderiatotal();
            }          
        }

        private void frm_busca_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (clasificacion <=2)
            {
                try
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    string sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE ID_MERCADERIA LIKE '%{0}%' AND ID_CLASIFICACION = {1} AND NOMBRE_SUCURSAL = '{2}';", textBox1.Text, clasificacion, sucursalDestino);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                            //styleDV(this.dataGridView1);
                        }
                    }
                    else
                    {
                        conexion.Close();
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE DESCRIPCION LIKE '%{0}%' AND ID_CLASIFICACION = {1} AND NOMBRE_SUCURSAL = '{2}';", textBox1.Text, clasificacion, sucursalDestino);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                //styleDV(this.dataGridView1);
                            }
                        }
                        else
                        {
                            conexion.Close();
                            conexion = ASG_DB.connectionResult();
                            sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE NOMBRE_PROVEEDOR LIKE '%{0}%' AND ID_CLASIFICACION = {1} AND NOMBRE_SUCURSAL = '{2}';", textBox1.Text, clasificacion, sucursalDestino);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Clear();
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                    //styleDV(this.dataGridView1);
                                }
                            }
                            else
                            {
                                conexion.Close();
                                conexion = ASG_DB.connectionResult();
                                sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE PRECIO_COMPRA LIKE '%{0}%' AND ID_CLASIFICACION = {1} AND NOMBRE_SUCURSAL = '{2}';", textBox1.Text, clasificacion, sucursalDestino);
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Clear();
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                        //styleDV(this.dataGridView1);
                                    }
                                }
                                else
                                {
                                    conexion.Close();
                                    conexion = ASG_DB.connectionResult();
                                    sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE TOTAL_UNIDADES LIKE '%{0}%' AND ID_CLASIFICACION = {1} AND NOMBRE_SUCURSAL = '{2}';", textBox1.Text, clasificacion, sucursalDestino);
                                    cmd = new OdbcCommand(sql, conexion);
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        dataGridView1.Rows.Clear();
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                        while (reader.Read())
                                        {
                                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                            //styleDV(this.dataGridView1);
                                        }
                                    }
                                    else
                                    {
                                        cargaMercaderia(clasificacion);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception es)
                {
                    MessageBox.Show(es.ToString());
                }
            }
            else
            {
                buscaMercaderia();
            }
        }
        private void buscaMercaderia()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE ID_MERCADERIA LIKE '%{0}%' AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, sucursalDestino);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                        //styleDV(this.dataGridView1);
                    }
                }
                else
                {
                    conexion.Close();
                    conexion = ASG_DB.connectionResult();
                    sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE DESCRIPCION LIKE '%{0}%'  AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, sucursalDestino);
                    cmd = new OdbcCommand(sql, conexion);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                            //styleDV(this.dataGridView1);
                        }
                    }
                    else
                    {
                        conexion.Close();
                        conexion = ASG_DB.connectionResult();
                        sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE NOMBRE_PROVEEDOR LIKE '%{0}%' AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, sucursalDestino);
                        cmd = new OdbcCommand(sql, conexion);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                //styleDV(this.dataGridView1);
                            }
                        }
                        else
                        {
                            conexion.Close();
                            conexion = ASG_DB.connectionResult();
                            sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE PRECIO_COMPRA LIKE '%{0}%'  AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, sucursalDestino);
                            cmd = new OdbcCommand(sql, conexion);
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                dataGridView1.Rows.Clear();
                                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                    //styleDV(this.dataGridView1);
                                }
                            }
                            else
                            {
                                conexion.Close();
                                conexion = ASG_DB.connectionResult();
                                sql = string.Format("SELECT * FROM VISTA_MERCADERIA WHERE TOTAL_UNIDADES LIKE '%{0}%' AND NOMBRE_SUCURSAL = '{1}';", textBox1.Text, sucursalDestino);
                                cmd = new OdbcCommand(sql, conexion);
                                reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    dataGridView1.Rows.Clear();
                                    dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                    while (reader.Read())
                                    {
                                        dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(2), reader.GetString(1), string.Format("Q.{0:###,###,###,##0.00##}", reader.GetDouble(4)), string.Format("{0:#,###,###,###,###}", reader.GetDouble(5)));
                                        ////styleDV(this.dataGridView1);
                                    }
                                }
                                else
                                {
                                    cargaMercaderiatotal();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception es)
            {
                MessageBox.Show(es.ToString());
            }
        }
        internal frm_compras.Mercaderia CurrentMercaderia
        {
            get
            {
                return new frm_compras.Mercaderia()
                {
                    getMercaderia = mercaderiaObtenida
                };
            }
            set
            {
                CurrentMercaderia.getMercaderia = mercaderiaObtenida;
            }
        }
        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                if (clasificacion == 3)
                {
                  
                    if (dataGridView1.RowCount > 0)
                    {
                        mercaderiaObtenida = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        descripcion = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                        cantidadTotal = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                        precioProducto = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                        proveedorMercaderia = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                        DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                   
                    if (dataGridView1.RowCount > 0)
                    {
                        mercaderiaObtenida = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        DialogResult = DialogResult.OK;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
            {
               
                if (dataGridView1.RowCount > 0)
                {
                    if (clasificacion == 3)
                    {

                        if (dataGridView1.RowCount > 0)
                        {
                            mercaderiaObtenida = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                            descripcion = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                            cantidadTotal = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                            precioProducto = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                            proveedorMercaderia = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                            DialogResult = DialogResult.OK;
                        }
                    }
                    else
                    {

                        if (dataGridView1.RowCount > 0)
                        {
                            mercaderiaObtenida = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                            DialogResult = DialogResult.OK;
                        }
                    }
                }
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_busca_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_busca_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_busca_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
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

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (clasificacion == 3)
                {

                    if (dataGridView1.RowCount > 0)
                    {
                        mercaderiaObtenida = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        descripcion = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                        cantidadTotal = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                        precioProducto = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                        proveedorMercaderia = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                        DialogResult = DialogResult.OK;
                    }
                }
                else
                {

                    if (dataGridView1.RowCount > 0)
                    {
                        mercaderiaObtenida = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        DialogResult = DialogResult.OK;
                    }
                }
            }
        }

        private void pictureBox3_MouseHover(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.Red; pictureBox3.BackColor = Color.Red;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.Transparent;
        }
    }
}
