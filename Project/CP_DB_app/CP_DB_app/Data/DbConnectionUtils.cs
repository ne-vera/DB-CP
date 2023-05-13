using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_DB_app.Data
{
    public static class DbConnectionUtils
    {
        public static string user = "CLIENT";
        public static string password = "CLIENT";

        public static OracleConnection GetDBConnection()
        {
            string host = "192.168.43.246";
            int port = 1521;
            string sid = "orcl.be.by";

            return DBOracleUtils.GetDBConnection(host, port, sid, user, password);
        }
    }
}
