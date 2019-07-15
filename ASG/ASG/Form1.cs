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
    public partial class Form1 : Form
    {
        string usuario;
        string idSucursal;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public Form1(string nombre, string sucursal)
        {
            InitializeComponent();
            usuario = nombre;
            idSucursal = sucursal;
            cargaTipos();
        }
        private void cargaTipos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT TIPO_SUCURSAL FROM TIPO_SUCURSAL;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    comboBox1.Items.Add(reader.GetString(0));
                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader.GetString(0));
                    }
                }
                else
                {
                    MessageBox.Show("NO EXISTEN TIPOS DE CLIENTES ALMACENADOS!", "GESTION SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                if (textBox9.Text != "" | textBox12.Text != "" | textBox11.Text != "" | comboBox1.Text != "")
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "MANTENIMIENTO SUCURSALES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    }
                }
                else
                {
                    this.Close();
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                button1.PerformClick();
            }
        }
      
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
           if(textBox9.Text != "" | textBox12.Text != "" | textBox11.Text != "" | comboBox1.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "MANTENIMIENTO SUCURSALES", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            } else
            {
                this.Close();
            }
        }
        private bool nombreSucursal(string nombre)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT NOMBRE_SUCURSAL FROM SUCURSAL WHERE NOMBRE_SUCURSAL = '{0}';", nombre);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return false;
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox9.Text != "" && textBox11.Text != "" && textBox12.Text != "" && comboBox1.Text != "")
            {
                if (nombreSucursal(textBox9.Text.Trim()))
                {
                    OdbcConnection conexion = ASG_DB.connectionResult();
                    try
                    {
                        string sql = string.Format("CALL NUEVA_SUCURSAL ('{0}','{1}','{2}','{3}');", comboBox1.Text, textBox9.Text.Trim(), textBox12.Text.Trim(), textBox11.Text.Trim());
                        OdbcCommand cmd = new OdbcCommand(sql, conexion);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            //.Show("SE GENERO LA CUENTA CORRECTAMENTE");
                            var forma = new frm_creditoActualizado();
                            forma.ShowDialog();
                            DialogResult = DialogResult.OK;

                        }
                        else
                        {
                            MessageBox.Show("IMPOSIBLE CREAR NUEVA SUCURSAL!", "AJUSTES", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    conexion.Close();
                } else
                {
                    MessageBox.Show("NO SE PUEDE UTLIZAR EL MISMO NOMBRE PARA LAS SUCURSALES!", "NUEVA SUCURSAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } else
            {
                MessageBox.Show("DEBE DE LLENAR TODOS LOS CAMPOS", "NUEVA SUCURSAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "CENTRAL")
            {
                MessageBox.Show("TENGA EN CUENTA QUE LAS SUCURSALES CENTRALES PODRAN RECIBIR COMPRAS DIRECTAS!", "MANTENNIMIENTO SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBox9_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox9.Text != "")
            {
                if (e.KeyData == Keys.Enter)
                {
                    textBox11.Focus();
                }
            }
        }

        private void textBox11_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox11.Text != "")
            {
                if (e.KeyData == Keys.Enter)
                {
                    textBox12.Focus();
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Red;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Transparent;
        }
    }
}
