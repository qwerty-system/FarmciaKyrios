using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.PowerPacks;
using System.Data.Odbc;


namespace ASG
{
    public partial class frm_loggin : Form
    {
        string nameUser;
        string rolUser;
        string userName;
        string userImage;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        bool[] privilegios = new bool[18];
        public frm_loggin()
        {
            InitializeComponent();
        }
        private void timerActions()
        {
            timer1.Interval = 800;
            pictureBox1.Image = ASG.Properties.Resources.checked2;
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            timer1.Enabled = true;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timerActions();
            timer1.Enabled = false;
            this.Hide();
            frm_menu nuevo = new frm_menu(this, nameUser, rolUser, userName, userImage, privilegios);
            nuevo.Show();
            textBox1.Text = "USUARIO";
            textBox1.ForeColor = Color.DimGray;
            textBox2.UseSystemPasswordChar = false;
            textBox2.Text = "CONTRASEÑA";
            textBox2.ForeColor = Color.DimGray;
            checkBox1.Checked = false;
            pictureBox1.Image = ASG.Properties.Resources.padlock__1_;

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "USUARIO")
            { 
                textBox1.Text = "";
                textBox1.ForeColor = Color.LightGray;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "CONTRASEÑA")
            {
                if (checkBox1.Checked == false)
                {
                    textBox2.UseSystemPasswordChar = true;
                }
                textBox2.Text = "";
                textBox2.ForeColor = Color.LightGray;
            }
           
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                textBox1.Text = "USUARIO";
                textBox1.ForeColor = Color.DimGray;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.UseSystemPasswordChar = false;
                textBox2.Text = "CONTRASEÑA";
                textBox2.ForeColor = Color.DimGray;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                textBox2.UseSystemPasswordChar = false;
            } else
            {
                if (textBox2.Text == "CONTRASEÑA") {
                    textBox2.UseSystemPasswordChar = false;
                } else
                {
                    textBox2.UseSystemPasswordChar = true;
                }

            }
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            string sql = string.Format("SELECT NOMBRE_USUARIO, ESTADO_USUARIO, ID_ROL, ID_USUARIO, imageUser FROM USUARIO WHERE ID_USUARIO  = '{0}' AND PASSWORD_USUARIO = '{1}'", textBox1.Text.Trim(), textBox2.Text.Trim());
            OdbcCommand cmd = new OdbcCommand(sql, conexion);
            OdbcDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                if ((reader.GetBoolean(1) == true))
                {
                    nameUser = reader.GetString(0);
                    rolUser = reader.GetString(2);
                    userName = reader.GetString(3);
                    userImage = reader.GetString(4);
                    getPrivilegios(textBox1.Text.Trim());
                    timerActions();
                    conexion.Close();               
                }
                else
                {
                    MessageBox.Show("TU CUENTA HA SIDO DESACTIVADA!", "INICIO DE SESION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("USUARIO O CONTRASEÑA INCORRECTOS!", "INICIO DE SESION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox1.Text = "USUARIO";
                textBox1.ForeColor = Color.DimGray;
                textBox2.UseSystemPasswordChar = false;
                textBox2.Text = "CONTRASEÑA";
                textBox2.ForeColor = Color.DimGray;
                checkBox1.Checked = false;
                textBox1.Focus();
            }
        }
        private void getPrivilegios(string NombreUsuario)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT APERTURA_CAJA, CIERRE_CAJA, ANULAR_FACTURA, DEVOLUCIONES, VER_REPORTES,ELIMINAR_CLIENTE,INGRESAR_CLIENTE,INGRESO_PROVEEDOR,ELIMINAR_PROVEEDOR,BITACORA,CUENTAS_COBRAR,CUENTAS_PAGAR,INGRESO_USUARIOS,ELIMINAR_USUARIOS,CONSULTAR_MERCADERIA,COMPRAS,TRASLADO_MERCADERIA, KARDEX FROM PRIVILEGIOS_USUARIO WHERE USUARIO_ID_USUARIO  = '{0}' AND ROL_ID_ROL = '{1}';", NombreUsuario, rolUser);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    privilegios[0] = reader.GetBoolean(0);
                    privilegios[1] = reader.GetBoolean(1);
                    privilegios[2] = reader.GetBoolean(2);
                    privilegios[3] = reader.GetBoolean(3);
                    privilegios[4] = reader.GetBoolean(4);
                    privilegios[5] = reader.GetBoolean(5);
                    privilegios[6] = reader.GetBoolean(6);
                    privilegios[7] = reader.GetBoolean(7);
                    privilegios[8] = reader.GetBoolean(8);
                    privilegios[9] = reader.GetBoolean(9);
                    privilegios[10] = reader.GetBoolean(10);
                    privilegios[11] = reader.GetBoolean(11);
                    privilegios[12] = reader.GetBoolean(12);
                    privilegios[13] = reader.GetBoolean(13);
                    privilegios[14] = reader.GetBoolean(14);
                    privilegios[15] = reader.GetBoolean(15);
                    privilegios[16] = reader.GetBoolean(16);
                    privilegios[17] = reader.GetBoolean(17);
                } else
                {
                   // MessageBox.Show("imposbe obenter priiegios");
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {    
                if (e.KeyChar == 13)
                   button1.PerformClick();
        }

        private void frm_loggin_Load(object sender, EventArgs e)
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

        private void frm_loggin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                Application.Exit();
            }
        }

        private void frm_loggin_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_loggin_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_loggin_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox2.Focus();
            } else if(e.KeyData == Keys.Down)
            {
                textBox2.Focus();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var forma = new frm_recuperaPass();
            forma.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            var forma = new frm_aboutSystem();
            forma.ShowDialog();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Up)
            {
                textBox1.Focus();
            }
            else if (e.KeyData == Keys.Down)
            {
                button1.Focus();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
