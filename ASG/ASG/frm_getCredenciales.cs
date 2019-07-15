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
    public partial class frm_getCredenciales : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        bool flagSesion;
        string user;
        public frm_getCredenciales(string value)
        {
            InitializeComponent();
            user = value;
        }

        private void frm_getCredenciales_Load(object sender, EventArgs e)
        {

        }
        private void timerActions()
        {
            timer1.Interval = 800;
            timer1.Enabled = true;
            pictureBox1.Image = ASG.Properties.Resources.checked2;
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            DialogResult = DialogResult.OK;
        }
        private void frm_getCredenciales_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_getCredenciales_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_getCredenciales_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                if (textBox2.Text == "CONTRASEÑA")
                {
                    textBox2.UseSystemPasswordChar = false;
                }
                else
                {
                    textBox2.UseSystemPasswordChar = true;
                }

            }
        }
        internal frm_usuarios.getCredencials currenSesion
        {
            get
            {
                return new frm_usuarios.getCredencials()
                {
                    getSesion = flagSesion
                };
            }
            set
            {
                currenSesion.getSesion = flagSesion;
            }


        }
        private void frm_getCredenciales_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            string sql = string.Format("SELECT NOMBRE_USUARIO, ESTADO_USUARIO, ID_ROL, ID_USUARIO FROM USUARIO WHERE ID_USUARIO  = '{0}' AND PASSWORD_USUARIO = '{1}'", textBox1.Text.Trim(), textBox2.Text.Trim());
            OdbcCommand cmd = new OdbcCommand(sql, conexion);
            OdbcDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                if ((reader.GetBoolean(1) == true))
                {
                    
                    if((reader.GetString(2) == "ADMINISTRADOR") |(reader.GetString(3) == user))
                    {
                        flagSesion = true;
                        timerActions();
                    }
                    else
                    {
                        MessageBox.Show("INGRESE LOS CREDENCIALES DE SU CUENTA O DE UN ADMINISTRADOR!", "INICIO DE SESION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            timerActions();
            timer1.Enabled = false;
            textBox1.Text = "USUARIO";
            textBox1.ForeColor = Color.DimGray;
            textBox2.UseSystemPasswordChar = false;
            textBox2.Text = "CONTRASEÑA";
            textBox2.ForeColor = Color.DimGray;
            checkBox1.Checked = false;
            pictureBox1.Image = ASG.Properties.Resources.padlock__1_;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "USUARIO")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.LightGray;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "USUARIO";
                textBox1.ForeColor = Color.DimGray;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                button1.PerformClick();
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

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.UseSystemPasswordChar = false;
                textBox2.Text = "CONTRASEÑA";
                textBox2.ForeColor = Color.DimGray;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                if (textBox1.Text != "")
                {
                    textBox2.Focus();
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
