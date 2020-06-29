using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Data.SqlClient;
namespace CPublic
{
    public class Var
    {
        #region 全局变量

        /// <summary>
        /// 当前用户名
        /// </summary>
        public static string localUserName = "";

        public static string localUser部门编号 = "";
        public static string localUser组织关系 = "";
        public static string localUser课室编号 = "";
        public static string localUser工号简码 = "";
        public static string localUser部门名称 = "";

        /// <summary>
        /// 当前用户ID
        /// </summary>
        public static string LocalUserID = "";

        /// <summary>
        /// 用户权限组
        /// </summary>
        public static string LocalUserTeam = "";

        public static string FileServerConnString = "";
        /// <summary>
        /// 服务器名
        /// </summary>
        public static string ServerName = "";
        /// <summary>
        /// 服务器IP
        /// </summary>
        public static string ServerIP = "";
        /// <summary>
        /// 服务器端口
        /// </summary>
        public static int ServerListenPort = 0;
        /// <summary>
        /// 通道名称
        /// </summary>
        public static String ChannelName = null; 

        #endregion
        
        #region 当前路径
        public static string strDir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
          
        #endregion 

        #region 私有变量
        /// <summary>
        /// 数据库连接串
        /// </summary>
        private static string strconn = "";

        /// <summary>
        /// 数据库连接串_FC
        /// </summary>
        private static string strconn_fc = "";

        /// <summary>
        /// web service 连接地址
        /// </summary>
        private static string strWSconn = "";

        /// <summary>
        /// web service 连接地址
        /// </summary>
        private static string strconn_fs = "";


        public static Dictionary<string, string> li_CFG = null; 
        #endregion

        #region 参数

        public static string strConn
        {
            get
            {
                if (li_CFG == null) li_CFG = fun_初始化参数( "conn.cfg", "MES_keys");

                if (strconn == "")
                {
                    strconn = string.Format("Password={0};Persist Security Info=True;User ID={1};Initial Catalog={2};Data Source={3};Pooling=true;Max Pool Size=40000;Min Pool Size=0",
                        li_CFG["PWD"], li_CFG["UID"], li_CFG["DataBase"], li_CFG["SQLServer"]);
                    ServerName = li_CFG["SQLServerName"].ToString();
                    ServerIP = li_CFG["SQLServer"].ToString();
                    try
                    {
                        ServerListenPort = int.Parse(li_CFG["ServerPort"].ToString());
                    }
                    catch { }
                }
                return strconn;
            }
        }
        /// <summary>
        /// 得到其它数据库访问地址
        /// </summary>
        /// <param name="DBflag"></param>
        /// <returns></returns>
        public static string geConn(string DBflag)
        {

            if (li_CFG == null) li_CFG = fun_初始化参数("conn.cfg", "MES_keys");
                //if (strconn_fc == "")
                //{
                    try
                    {
                        strconn_fc = string.Format("Password={0};Persist Security Info=True;User ID={1};Initial Catalog={2};Data Source={3};Pooling=true;Max Pool Size=40000;Min Pool Size=0",
                                             li_CFG[string.Format("PWD_{0}", DBflag)], li_CFG[string.Format("UID_{0}", DBflag)], li_CFG[string.Format("DataBase_{0}", DBflag)], li_CFG[string.Format("SQLServer_{0}", DBflag)]);
                        ServerName = li_CFG["SQLServerName"].ToString();
                        ServerIP = li_CFG["SQLServer"].ToString();
                    }
                    catch 
                    {
                        
                     
                    }
                 
                //}
                return strconn_fc;
            
        }
       /// <summary>
        /// web service 连接地址
       /// </summary>
        public static string strWSConn
        {
            get
            {
                if (li_CFG == null) li_CFG = fun_初始化参数("conn.cfg", "MES_keys");

                return li_CFG["WS"];
            }
        }
        public static DateTime getDatetime()
        {
            try
            {
                //放到CP里
                string sql = "select getdate() as time";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn);
                da.Fill(dt);
                return (DateTime)(dt.Rows[0]["time"]);
            }
            catch
            {
                return System.DateTime.Now;
            }
        }
        #endregion

        #region fun_初始化参数

        private static Dictionary<string, string> fun_初始化参数( string connPath, string strKey)
        {
            try
            {
                    Dictionary<string, string>  liCFG = new Dictionary<string, string>();
                    string sKey = strKey;
                    string path = string.Format("{0}\\{1}", strDir, connPath);
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
                        liCFG.Add(r[0].ToString(), r[1].ToString());
                    }
             
                    return liCFG;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 其它全局变量_路径
        static string baseDir = "";
        
        /// <summary>
        /// 得到当前路径
        /// </summary>
        public static string BaseDir
        {
            get
            {
                if (baseDir == "")
                {
                    baseDir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName); 
                }
                return baseDir;
            }
            
        }

        /// <summary>
        /// 得到当前某路径，并可以设置是否要消除当前路径中的所有文件
        /// </summary>
        /// <param name="dir">要追加的路径</param>
        /// <param name="blClear">是否要清除</param>
        /// <returns></returns>
        public static string GetCurrCoustDir(string dir,Boolean blClear = true)
        {
            dir = BaseDir + "\\" + dir;
            if (blClear)
            {
                try
                {
                    System.IO.Directory.Delete(dir, true);

                }
                catch { }
            }
            try
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            catch { }
            return dir;
        }

        #endregion



    }
}
