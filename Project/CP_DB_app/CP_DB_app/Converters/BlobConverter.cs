using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_DB_app.Converters
{
    public static class BlobConverter
    {
        public static byte[] ConvertToBlob(string fileName)
        {
            FileStream fls;
            fls = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] blob = new byte[fls.Length];
            fls.Read(blob, 0, System.Convert.ToInt32(fls.Length));
            fls.Close();
            return blob;
        }
    }
}
