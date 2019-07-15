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
    public partial class frm_userDetails : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        string userDetails;
        public frm_userDetails(string user)
        {
            InitializeComponent();
            userDetails = user;
            cargaDatos();
        }
        private void getPrivilegios(string id_usuario, string id_rol)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM PRIVILEGIOS_USUARIO WHERE USUARIO_ID_USUARIO = '{0}' AND ROL_ID_ROL = '{1}';", id_usuario, id_rol);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    checkBox34.Checked = reader.GetBoolean(2);
                    checkBox32.Checked = reader.GetBoolean(3);
                    checkBox33.Checked = reader.GetBoolean(4);
                    checkBox31.Checked = reader.GetBoolean(5);
                    checkBox30.Checked = reader.GetBoolean(6);
                    checkBox29.Checked = reader.GetBoolean(7);
                    checkBox28.Checked = reader.GetBoolean(8);
                    checkBox27.Checked = reader.GetBoolean(9);
                    checkBox26.Checked = reader.GetBoolean(10);
                    checkBox25.Checked = reader.GetBoolean(11);
                    checkBox24.Checked = reader.GetBoolean(12);
                    checkBox23.Checked = reader.GetBoolean(13);
                    checkBox22.Checked = reader.GetBoolean(14);
                    checkBox21.Checked = reader.GetBoolean(15);
                    checkBox20.Checked = reader.GetBoolean(16);
                    checkBox19.Checked = reader.GetBoolean(17);
                    checkBox18.Checked = reader.GetBoolean(18);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION USUARIOS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private void cargaDatos()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT * FROM VISTA_USUARIOS WHERE ID_USUARIO = '{0}';",userDetails);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    label1.Text = reader.GetString(0);
                    string rol = reader.GetString(1);
                    label5.Text = rol;
                    label2.Text = reader.GetString(3);
                    label6.Text = ""+reader.GetString(5);
                    getPrivilegios(userDetails,rol);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION USUARIOS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_userDetails_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_userDetails_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_userDetails_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_userDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void frm_userDetails_Load(object sender, EventArgs e)
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
    }
}
