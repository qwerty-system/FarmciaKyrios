using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data.Odbc;
using System.Windows.Forms;

namespace ASG
{
    class adressUser
    {
        public static string adressIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    System.Security.Principal.WindowsIdentity user = System.Security.Principal.WindowsIdentity.GetCurrent();
                    localIP = localIP + " / " + user.Name;
                    return localIP;
                }
            }
            return null;
        }
        public static void ingresaBitacora (string sucursal,string usuario,string operacion,string dato)
        {
            OdbcConnection conexion = ASG_DB.connectionResult();
            try
            {
                string sql = string.Format("CALL UPDATE_BITACORA ({0},'{1}','{2}','{3}','{4}');", sucursal,usuario,operacion,dato,adressIP());
                OdbcCommand cmd = new OdbcCommand(sql, conexion);
                if (cmd.ExecuteNonQuery() == 1)
                {
                   
                }
                else
                {
                    MessageBox.Show("NO SE PUDO ACTUALIZAR LA BITACORA!", "GESTION BITACORA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexion.Close();
        }
        public static void generaNotificacion(string sucursal)
        {

            NotifyIcon notifyIcon1 = new NotifyIcon();
            NotifyIcon notifyIcon = notifyIcon1;
            notifyIcon.BalloonTipTitle = "Traslado de Mercaderia";
            notifyIcon.BalloonTipText = "Se ha trasladado mercaderia a la sucursal " + sucursal;
            notifyIcon.ShowBalloonTip(0);

        }
    }
}
