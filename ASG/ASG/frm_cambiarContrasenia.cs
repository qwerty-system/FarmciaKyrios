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
    public partial class frm_cambiarContrasenia : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string usuario;

        public frm_cambiarContrasenia(string user)
        {
            InitializeComponent();
            usuario = user;
        }

        private void frm_cambiarContrasenia_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                if (textBox3.Text != "")
                {
                    DialogResult result;
                    result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "USUARIOS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    }
                } else
                {
                    this.Close();
                }
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if(textBox3.Text != "")
            {
                DialogResult result;
                result = MessageBox.Show("¿ESTA SEGURO QUE DESEA CANCELAR LA OPERACION?", "USUARIOS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }else
            {
                this.Close();
            }
        }

        private void frm_cambiarContrasenia_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_cambiarContrasenia_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_cambiarContrasenia_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT PASSWORD_USUARIO FROM USUARIO WHERE ID_USUARIO = '{0}' AND PASSWORD_USUARIO = '{1}';", usuario, textBox3.Text.Trim());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    button1.Visible = true;
                    textBox1.Visible = true;
                    label4.Visible = true;
                    textBox1.Focus();
                } else
                {
                    MessageBox.Show("LA CONTRASEÑA INGRESADA NO COINCIDE", "GESTION USUARIOS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (textBox3.Text != "")
                {
                    button8.PerformClick();
                } else
                {
                    MessageBox.Show("INGRESE UNA CONTRASEÑA VALIDA!", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
               
            }
            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (textBox1.Text != "")
                {
                    button1.PerformClick();
                } else
                {
                    MessageBox.Show("INGRESE UNA CONTRASEÑA VALIDA!", "TRASLADOS MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "")
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                try
                {
                    string sql = string.Format("UPDATE USUARIO SET PASSWORD_USUARIO = '{0}' WHERE ID_USUARIO = '{1}';", textBox1.Text.Trim(), usuario);
                    OdbcCommand cmd = new OdbcCommand(sql, conexion);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        var forma = new frm_creditoActualizado();
                        forma.ShowDialog();
                        button1.Visible = false;
                        textBox1.Visible = false;
                        label4.Visible = false;
                        button8.Enabled = false;
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
