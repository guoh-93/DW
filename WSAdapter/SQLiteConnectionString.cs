using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
//using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CZMaster
{
    public static class SQLiteConnectionString
    {
        public static string strDir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

        private static Dictionary<string, string> li_CFG = null; 

        public static string GetConnectionString(string path)
        {
            return GetConnectionString(path, null);
        }
        public static string GetConnectionString(string path, string password)
        {
            if (string.IsNullOrEmpty(password))
                return "Data Source=" + path;
            return "Data Source=" + path + ";Password=" + password;
        }
        public static string GetValueFormCFG(string Key)
        {
            fun_初始化参数();
            try
            {
                return li_CFG[Key];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void fun_初始化参数()
        {
            try
            {
                if (li_CFG == null)
                {
                    li_CFG = new Dictionary<string, string>();
                    string sKey = "MES_keys";
                    string path = strDir + "\\conn.cfg";
                    DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
                    DES.Key = ASCIIEncoding.UTF8.GetBytes(sKey);
                    DES.IV = ASCIIEncoding.UTF8.GetBytes(sKey);
                    ICryptoTransform desencrypt = DES.CreateDecryptor();
                    byte[] data = File.ReadAllBytes(path);
                    byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                    MemoryStream ms = new MemoryStream(result);
                    DataTable dtM = new DataTable();
                    dtM.ReadXml(ms);
                    foreach (DataRow r in dtM.Rows)
                    {
                        li_CFG.Add(r[0].ToString(), r[1].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
