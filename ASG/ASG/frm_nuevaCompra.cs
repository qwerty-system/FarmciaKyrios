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
    public partial class frm_nuevaCompra : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string usuarioSucursal;
        string nombreSucursal;
        string codigo;
        string proveedor = "";
        string nameUs;
        string rolUs;
        string usuario;
        bool[] privilegios;
        string idSucursal;
        public frm_nuevaCompra(string nameUser, string rolUser, string user, string sucursal, bool[] privilegio)
        {
            InitializeComponent();
            cargaSucursales();
            usuario = user;
            nameUs = nameUser;
            rolUs = rolUser;
            idSucursal = sucursal;
            this.privilegios = privilegio;
            cargaProveedores();
            usuarioSucursal = user;
            textBox9.Text = getCompra();
        }
        private void cargaProveedores()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT NOMBRE_PROVEEDOR FROM PROVEEDOR WHERE ESTADO_TUPLA = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void cargaSucursales()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT NOMBRE_SUCURSAL FROM SUCURSAL WHERE ID_TIPO =  1 AND ESTADO_TUPLA = TRUE;");
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void nuevaCompra(string code, string sucursal, string proveedor)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL NUEVA_COMPRA ({0},'{1}','{2}','{3}');", code, usuarioSucursal,sucursal, proveedor);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    codigo = textBox9.Text.Trim();
                    nombreSucursal = comboBox1.Text.Trim();
                    proveedor = comboBox2.Text;
                    var forma = new frm_creditoActualizado();
                    forma.ShowDialog();
                    DialogResult = DialogResult.OK;
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
        private bool codigoCompra(string code)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_COMPRA FROM COMPRA WHERE ID_COMPRA =  '{0}' AND ESTADO_TUPLA = TRUE;", code);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //MessageBox.Show("EL CODIGO YA EXITSTE");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
            return true;
        }
        public string createCodecompra(int longitud)
        {
            string caracteres = "1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < longitud--)
            {
                res.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            return res.ToString();
        }
        private string getCompra()
        {
            bool codex = true;
            while (codex)
            {
                string temp = createCodecompra(7);
                if (codigoCompra(temp))
                {
                    codex = false;
                    return temp;
                }

            }
            return null;
        }
        private void frm_nuevaCompra_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                if (textBox1.Text != "" | textBox9.Text != "" | comboBox1.Text != "")
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "NUEVA COMPRA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    }
                }
                else
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "NUEVA COMPRA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    }
                }
            } else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.A))
            {
                button1.PerformClick();
            }
        }

        private void frm_nuevaCompra_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_nuevaCompra_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        internal frm_compras.nuevaCompra currentCompra
        {
            get
            {
                return new frm_compras.nuevaCompra()
                {
                    getCompra = codigo,
                    getSucursal = nombreSucursal,
                    getProveedor = proveedor
                };
            }
            set
            {
                currentCompra.getCompra = codigo;
                currentCompra.getProveedor = proveedor;
            }
        }
        private void frm_nuevaCompra_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_nuevaCompra_Load(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now;
            textBox1.Text = today.ToString("yyyy-MM-dd");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox9.Text = getCompra();
        }
        
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "" | textBox9.Text != "" | comboBox1.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "NUEVA COMPRA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            } else
            {
                this.Close();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = dateTimePicker1.Value.ToString("yyyy-MM-dd");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" & textBox9.Text != "" & comboBox1.Text != "" & comboBox2.Text != "")
            {
                nuevaCompra(textBox9.Text.Trim(), comboBox1.Text.Trim(), comboBox2.Text.Trim());
            }
            else
            {
                MessageBox.Show("DEBE DE COMPLETAR LOS DATOS", "NUEVA COMPRA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            proveedor = comboBox2.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var forma = new frm_buscaProveedor(1, nameUs, rolUs, usuario, idSucursal, privilegios);

            if (forma.ShowDialog() == DialogResult.OK)
            {
                comboBox2.Items.Clear();
                cargaProveedores();
                comboBox2.Text = forma.currentProveedor.getProveedor;

            }
        }
    }
}
