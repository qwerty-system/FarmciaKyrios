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
using MySql.Data.MySqlClient;

namespace ASG
{
    public partial class frm_menu : Form
    {
        string nameUser;
        string rolUser;
        string userName;
        bool flag = true;
        string idSucursal;
        frm_loggin principal;
        bool inicioSesion = true;
        string nombreSucursal;
        string codigoCaja;
        string userImage;
        bool[] privilegios;
        string codigo;
        public frm_menu(frm_loggin previo, string name, string rol, string user, string image, bool[] privilegio)
        {
            InitializeComponent();
            nameUser = name;
            this.privilegios = privilegio;
            rolUser = rol;
            userName = user;
            userImage = image;
            label2.Text = nameUser;
            label3.Text = rolUser;
            principal = previo;
            timer2.Interval = 600;
            timer2.Enabled = true;
           /* vScrollBar1.Maximum = panel4.Size.Height - panel5.Size.Height;
            vScrollBar1.Visible = (panel5.Size.Height >= panel4.Size.Height) ? false : true;*/
            //panel4.AutoScroll = true;
            panel5.AutoScroll = true;
            panel5.HorizontalScroll.Visible = false;
            panel4.HorizontalScroll.Visible = false;
            this.MouseWheel += new MouseEventHandler(Panel4_MouseWheel);
            setterFormulario();
            setterCaja();

        }
        private void setterCaja()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE CAJA SET ESTADO_TUPLA = FALSE  WHERE APERTURA_CAJA < CURDATE();");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() >= 1 || cmd.ExecuteNonQuery() == 0)
                {
                    //.Show("SE GENERO LA CUENTA CORRECTAMENTE");
                   
                }
                else
                {
                    MessageBox.Show("LAS CAJAS DEL DIA ANTERIOR NO PUDIERON SER CERRADAS!", "GETION CAJA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void getbyName()
        {
            try
            {
                OdbcConnection conexion = ASG_DB.connectionResult();
                string sql = string.Format("SELECT NOMBRE_SUCURSAL FROM SUCURSAL WHERE ID_SUCURSAL  = {0} AND ESTADO_TUPLA = TRUE;", idSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    nombreSucursal = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void setterFormulario()
        {
            privilegiosUsuario();
            if (rolUser != "ADMINISTRADOR" && rolUser != "DATA BASE ADMIN" && rolUser != "ROOT")
            {
                button11.Enabled = false;
                button13.Enabled = false;
                button19.Enabled = false;
                button20.Enabled = false;
                button17.Enabled = false;
                button18.Enabled = false;
                button8.Enabled = false;
            }
            if (userImage != " " && userImage != "0" && userImage != "")
            {
                pictureBox1.ImageLocation = userImage;
            }
        }
        private void privilegiosUsuario()
        {          
            for (int i = 0; i<18; i++)
            {
                switch (i)
                {
                    case 2 :
                        if (privilegios[i] != true)
                           button6.Enabled = false;
                        break;
                    case 4:
                        if (privilegios[i] != true)
                            button10.Enabled = false;
                        break;
                    case 6:
                        if (privilegios[i] != true)
                            button2.Enabled = false;
                        break;
                    case 9:
                        if (privilegios[i] != true)
                            button12.Enabled = false;
                        break;
                    case 7:
                        if (privilegios[i] != true)
                            button3.Enabled = false;
                        break;
                    case 10:
                        if (privilegios[i] != true)
                            button16.Enabled = false;
                        break;
                    case 11:
                        if (privilegios[i] != true)
                            button14.Enabled = false;
                        break;
                    case 14:
                        if (privilegios[i] != true)
                            button4.Enabled = false;
                        break;
                    case 15:
                        if (privilegios[i] != true)
                            button5.Enabled = false;
                        break;
                }
            }
        }
        private void Panel5_MouseWheel(object sender, MouseEventArgs e)
        {
           /* panel5.Focus();
            panel4.Location = new Point(0, -(vScrollBar1.Value));*/
        }
        private void lastLoggin()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("UPDATE USUARIO SET LAST_LOGGIN = NOW() WHERE ID_USUARIO = '{0}';",userName);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void frm_menu_Load(object sender, EventArgs e)
        {
            /*this.WindowState = FormWindowState.Maximized;
            this.Size = SystemInformation.PrimaryMonitorMaximizedWindowSize;*/
           
        }

        private void frm_menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (flag == true)
            {
                DialogResult dialogo = MessageBox.Show("¿ESTA SEGURO QUE DESEA CERRAR EL PROGRAMA?",
                   "CERRAR PROGRAMA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogo == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    flag = false;
                    e.Cancel = false;
                    adressUser.ingresaBitacora(idSucursal, userName, "CIERRE DE SESION", "CIERRE APLICACION");
                    Application.Exit();
                }
            }
        }
        private void button13_Click(object sender, EventArgs e)
        {
            frm_ajustes frm = new frm_ajustes(nameUser, rolUser, userName,0, idSucursal);
            frm.Show(); ;
        }
        private void button15_Click(object sender, EventArgs e)
        {
            adressUser.ingresaBitacora(idSucursal, userName, "CIERRE DE SESION", "DESLOGEO USUARIO");
            principal.Show();
            flag = false;
            this.Close();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Text = DateTime.Now.ToLongTimeString();
            label1.Text = DateTime.Now.ToLongDateString();
        }
        private void aperturaCaja()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("INSERT INTO CAJA VALUES (NULL,{0},'{1}',NOW(),{2},NOW(),{2},'APERTURADA',TRUE,TRUE);", idSucursal, userName, 0);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    obtieneCaja();
                }
                else
                {
                    MessageBox.Show("NO SE PUDO GENERAR NUEVA ORDEN DE COMPRA!", "CAJA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        private void obtieneCaja()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CAJA FROM CAJA WHERE ID_SUCURSAL = {0} AND ID_USUARIO = '{1}' AND ESTADO_TUPLA = TRUE AND ESTADO_CAJA = TRUE;", idSucursal, userName);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    codigoCaja = reader.GetString(0);
                }
                else
                {
                    aperturaCaja();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "GESTION MERCADERIA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();
        }
        private bool obtieneAjustes()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT CAJA FROM CONFIGURACION_SISTEMA WHERE IDCONFIGURACION = 111 AND CAJA = TRUE;");
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    conexion.Close();
                    return true;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return false;
        }
        private bool revisaCaja()
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT ID_CAJA FROM CAJA WHERE ID_USUARIO = '{0}' AND ID_SUCURSAL = {1} AND ESTADO_TUPLA = TRUE;", userName, idSucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    conexion.Close();
                    return true;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
            return false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
           
            if (obtieneAjustes())
            {
                if (!revisaCaja())
                {
                   if(privilegios[0] == true)
                    {
                        var forma = new frm_aperturaCaja(idSucursal, userName, nameUser);
                        if (forma.ShowDialog() == DialogResult.OK)
                        {
                            obtieneCaja();
                            frm_venta frm = new frm_venta(nameUser, rolUser, userName, idSucursal, nombreSucursal, codigoCaja, privilegios);
                            if (codigo != "")
                            {
                                frm.textBox11.Text = codigo;
                                frm.textBox11.Focus();
                                SendKeys.Send("{ENTER}");
                            }
                            frm.Show();
                        }
                    } else
                    {
                        DialogResult result;
                        result = MessageBox.Show("ACTUALMENTE NO CUENTAS CON LOS PRIVILEGIOS PARA APERTURAR UNA CAJA ¿DESEA INGRESAR CREDENCIALES DE ADMINISTRADOR?", "TRASLADO DE MERCADERIA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            var forma = new frm_getCredenciales(null);
                            if(forma.ShowDialog() == DialogResult.OK)
                            {
                                if(forma.currenSesion.getSesion == true)
                                {
                                    var formas = new frm_aperturaCaja(idSucursal, userName, nameUser);
                                    if (formas.ShowDialog() == DialogResult.OK)
                                    {
                                        obtieneCaja();
                                        frm_venta frm = new frm_venta(nameUser, rolUser, userName, idSucursal, nombreSucursal, codigoCaja, privilegios);
                                        if (codigo != "")
                                        {
                                            frm.textBox11.Text = codigo;
                                            frm.textBox11.Focus();
                                            SendKeys.Send("{ENTER}");
                                        }
                                        frm.Show();
                                    }
                                }
                            }
                        }
                    }
                   

                }
                else
                {
                    obtieneCaja();
                    frm_venta frm = new frm_venta(nameUser, rolUser, userName, idSucursal, nombreSucursal, codigoCaja, privilegios);
                    if (codigo != "")
                    {
                        frm.textBox11.Text = codigo;
                        frm.textBox11.Focus();
                        SendKeys.Send("{ENTER}");
                    }
                    frm.Show();
                }
            }
            else
            {
                obtieneCaja();
                frm_venta frm = new frm_venta(nameUser, rolUser, userName, idSucursal, nombreSucursal, codigoCaja,privilegios);
                if (codigo != "")
                {
                    frm.textBox11.Text = codigo;
                    frm.textBox11.Focus();
                    SendKeys.Send("{ENTER}");
                }
                frm.Show();
            }
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            frm_clientes frm = new frm_clientes(nameUser, rolUser, userName, idSucursal, false, null, null, privilegios);
            frm.Show(); ;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            frm_proveedores frm = new frm_proveedores(nameUser, rolUser, userName,0,idSucursal,false, privilegios);
            frm.Show();
        }
        private void button12_Click(object sender, EventArgs e)
        {
            frm_bitacora frm = new frm_bitacora(nameUser, rolUser, userName);
            frm.Show();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            frm_compras frm = new frm_compras(nameUser, rolUser, userName, idSucursal, privilegios);
            frm.Show();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            frm_mercaderia frm = new frm_mercaderia(nameUser, rolUser, userName, idSucursal,nombreSucursal, privilegios);
            frm.Show();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            frm_usuarios frm = new frm_usuarios(nameUser, rolUser, userName, privilegios);
            frm.Show();
        }
        private void button10_Click(object sender, EventArgs e)
        {
            frm_estadisticas frm = new frm_estadisticas(nameUser, rolUser, userName,nombreSucursal);
            frm.Show();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (obtieneAjustes())
            {
                if (!revisaCaja())
                {
                    var forma = new frm_aperturaCaja(idSucursal, userName, nameUser);
                    if (forma.ShowDialog() != DialogResult.OK)
                    {

                    }
                    else
                    {
                        obtieneCaja();
                        frm_devoluciones frm = new frm_devoluciones(nameUser, rolUser, userName, idSucursal, codigoCaja);
                        frm.Show();
                    }

                }
                else
                {
                    obtieneCaja();
                    frm_devoluciones frm = new frm_devoluciones(nameUser, rolUser, userName, idSucursal, codigoCaja);
                    frm.Show();
                }
            }
            else
            {
                obtieneCaja();
                frm_devoluciones frm = new frm_devoluciones(nameUser, rolUser, userName, idSucursal, codigoCaja);
                frm.Show();
            }
           
        }
        private void button8_Click(object sender, EventArgs e)
        {
            frm_caja frm = new frm_caja(nameUser, rolUser, userName, idSucursal, nombreSucursal, privilegios[1]);
            frm.Show();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            frm_envios frm = new frm_envios(nameUser, rolUser, userName, idSucursal, nombreSucursal);
            frm.Show();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            frm_facturas frm = new frm_facturas(nameUser, rolUser, userName, idSucursal, nombreSucursal, privilegios);
            frm.Show();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            getSucursal();
        }
        private void getSucursal()
        {
            var forma = new frm_getSucursal();
            if (forma.ShowDialog() == DialogResult.OK)
            {
                idSucursal = forma.CurrentSucursal.getSucursal;
                setSucursal(idSucursal);
                getNotification();
                getbyName();
            }
        }
        private void getNotification()
        {
            if (inicioSesion)
            {
                notifyIcon1.BalloonTipTitle = notifyIcon1.BalloonTipTitle + " " + nameUser;
                notifyIcon1.ShowBalloonTip(0);
                inicioSesion = false;
            }
        }
        private void setSucursal (string sucursal)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("SELECT NOMBRE_SUCURSAL FROM  SUCURSAL WHERE ID_SUCURSAL = {0};", sucursal);
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                OdbcDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    label5.Text = reader.GetString(0);
                   
                    lastLoggin();
                    adressUser.ingresaBitacora(sucursal,userName,"INICIO DE SESION","PASSWORD_USER");
                }
                else
                {
                    MessageBox.Show("NO EXISTEN SUCURSALES!", "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FALLO LA CONEXION CON LA BASE DE DATOS!" + "\n" + ex.ToString(), "SUCURSALES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            conexion.Close();

        }
            internal class Sucursal
        {
            public string getSucursal { get; set; }
        }
        internal class codigoMercaderia
        {
            public string getProducto { get; set; }
        }
        private void frm_menu_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.S))
            {
                button5.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.N))
            {
                button1.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.C))
            {
                button2.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.P))
            {
                button3.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.M))
            {
                button4.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.D))
            {
                button6.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.F))
            {
                button7.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.R))
            {
                button14.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.E))
            {
                button10.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.B))
            {
                var forma = new frm_Precios(idSucursal, false, nombreSucursal, rolUser);
                if (forma.ShowDialog() == DialogResult.OK)
                {
                    codigo = forma.CurrrentCodigo.getProducto;
                    button1.PerformClick();
                    codigo = "";
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.F5))
            {
                timer2.Enabled = true;
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.J))
            {
                button16.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.A))
            {
                button8.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.T))
            {
                button9.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.U))
            {
                button11.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.X))
            {
                button12.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.G))
            {
                button13.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.L))
            {
                button15.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.I))
            {
                button17.PerformClick();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.O))
            {
                notificaciones();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            //panel4.Location = new Point(0, -(vScrollBar1.Value));
        }

        private void panel5_Resize(object sender, EventArgs e)
        {
          
        }

        private void panel4_Resize(object sender, EventArgs e)
        {
           /* vScrollBar1.Maximum = panel5.Size.Height - panel4.Size.Height;
            vScrollBar1.Visible = (panel4.Size.Height >= panel5.Size.Height) ? false : true;*/
        }

        private void button18_Click(object sender, EventArgs e)
        {
            var forma = new frm_Soporte();
            forma.ShowDialog();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            var forma = new frm_aboutSystem();
            forma.ShowDialog();
        }

        private void panel4_Resize_1(object sender, EventArgs e)
        {
            /*vScrollBar1.Maximum = panel4.Size.Height - panel5.Size.Height;
            vScrollBar1.Visible = (panel5.Size.Height >= panel4.Size.Height) ? false : true;*/
        }

        private void button14_Click(object sender, EventArgs e)
        {
            frm_cuentasPagar frm = new frm_cuentasPagar(nameUser, rolUser, userName, idSucursal);
            frm.Show();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            frm_cuentasCobrar frm = new frm_cuentasCobrar(nameUser, rolUser, userName, idSucursal);
            frm.Show();
        }
        private void Panel4_MouseWheel(object sender, MouseEventArgs e)
        {
            panel4.Focus();
        }
        private void panel4_MouseEnter(object sender, EventArgs e)
        {
            //panel4.Focus();
        }
        private void notificaciones()
        {
            var forma = new frm_BuscaProducto(nombreSucursal, idSucursal, rolUser);
            forma.ShowDialog();
        }
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            notificaciones();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            notificaciones();
        }

        private void guardaArchivo()
        {
            
            SaveFileDialog Guardar = new SaveFileDialog();
            Guardar.Filter = "sql(*.sql)|*.sql|sql(*.sql)|*.sql";
           
            if (Guardar.ShowDialog() == DialogResult.OK)
            {
               
                
            }
        }
        private void button19_Click(object sender, EventArgs e)
        {
            var forma = new frm_copiaDB(userName, idSucursal);
            forma.ShowDialog();
        }
      
        private void button20_Click(object sender, EventArgs e)
        {
            var forma = new frm_informacion(userName, idSucursal);
            forma.ShowDialog();
        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
