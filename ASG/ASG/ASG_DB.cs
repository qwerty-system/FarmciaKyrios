using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.Odbc;
using System.Net;

namespace ASG
{
    class ASG_DB
    {
        public static OdbcConnection connectionResult()
        {
            OdbcConnection cnx = new OdbcConnection(Properties.Settings.Default.rute);
            cnx.Open();
            return cnx;
        }
        
    }
}
