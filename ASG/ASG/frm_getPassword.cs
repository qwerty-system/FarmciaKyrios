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
using System.Text.RegularExpressions;

namespace ASG
{
    public partial class frm_getPassword : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string temp;
        string nombreUsuario;
        string pass;
        public frm_getPassword()
        {
            InitializeComponent();
        }

        private void frm_getPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                this.Close();
        }

        private void frm_getPassword_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;

        }

        private void frm_getPassword_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_getPassword_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void composeMail(string direccionMail)
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.To.Add(direccionMail);
            msg.Subject = "Recuperación de Contraseña";
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Body = "Hola" + " " +nombreUsuario+" "+"actualmente has recuperado tu contraseña" + "\n"+ "Tu Contraseña es:"+" "+ pass;

            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.From = new System.Net.Mail.MailAddress("help.qwertyco@outlook.com");

            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();
            cliente.Credentials = new System.Net.NetworkCredential("help.qwertyco@outlook.com", "//Qwertyco//50%04");
            cliente.Port = 587;
            cliente.EnableSsl = true;

            cliente.Host = "smtp-mail.outlook.com";

            try
            {
                cliente.Send(msg);
                var forma = new frm_sent();
                forma.Show();
                button1.Enabled = false;
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "")
            {
                if ((textBox2.BackColor != Color.OrangeRed) && (textBox2.Text == temp)) {
                    composeMail(textBox2.Text.Trim());
                } else
                {
                    MessageBox.Show("HA OCURRIDO UN ERROR!", "RECUPERACION USUARIO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } 
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(textBox3.Text != "")
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    string sql = string.Format("SELECT EMAIL, NOMBRE_USUARIO, PASSWORD_USUARIO FROM USUARIO WHERE ID_USUARIO = '{0}';", textBox3.Text.Trim());
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        temp = reader.GetString(0);
                        nombreUsuario = reader.GetString(1);
                        pass = reader.GetString(2);
                        textBox1.Text = temp;
                        textBox2.Focus();

                    } else
                    {
                        MessageBox.Show("USUARIO NO EXISTE!", "RECUPERACION DE CUENTA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                } catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                conexion.Close();
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                button8.PerformClick();
            }
        }
        private Boolean emailValidate(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.BackColor == Color.OrangeRed)
            {
                textBox2.BackColor = Color.White;
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                if (!emailValidate(textBox2.Text))
                {
                    textBox2.BackColor = Color.OrangeRed;
                    textBox2.ForeColor = Color.White;
                }
                else
                {
                    textBox2.BackColor = Color.White;
                    textBox2.ForeColor = Color.Black;
                }
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                button1.PerformClick();
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
