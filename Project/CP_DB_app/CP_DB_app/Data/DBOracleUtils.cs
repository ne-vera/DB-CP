using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_DB_app.Data
{
    internal class DBOracleUtils
    {
        public static OracleConnection GetDBConnection(string host, int port, String sid, String user, String password)
        {
            Console.WriteLine("Getting Connection ...");

            string connString = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = "
                + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = "
                + sid + ")));Password=" + password + ";User ID=" + user + ";Connection Timeout = 60000";

            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = connString;
            return conn;
        }
    }
}
