using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Data.OleDb;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Security.Principal;
using System.Runtime.Remoting;

namespace CZMaster
{

    public class TcpClientConnector
    {
        ///   <summary>   
        ///   在指定时间内尝试连接指定主机上的指定端口。   
        ///   </summary>   
        ///   <param   name="hostname">要连接到的远程主机的   DNS   名。</param>   
        ///   <param   name="port">要连接到的远程主机的端口号。</param>   
        ///   <param   name="millisecondsTimeout">要等待的毫秒数，或   -1   表示无限期等待。</param>   
        ///   <returns>已连接的一个   TcpClient   实例。</returns>   
        ///   <remarks>本方法可能抛出的异常与   TcpClient   的构造函数重载之一   
        ///   public   TcpClient(string,   int)   相同，并若指定的等待时间是个负数且不等于   
        ///   -1，将会抛出   ArgumentOutOfRangeException。</remarks>   
        public static TcpClient Connect(string hostname, int port, int millisecondsTimeout)
        {
            ConnectorState cs = new ConnectorState();
            cs.Hostname = hostname;
            cs.Port = port;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectThreaded), cs);
            if (cs.Completed.WaitOne(millisecondsTimeout, false))
            {
                if (cs.TcpClient != null) return cs.TcpClient;
                throw cs.Exception;
            }
            else
            {
                cs.Abort();
                throw new SocketException(11001);   //   cannot   connect   
            }
        }

        private static void ConnectThreaded(object state)
        {
            ConnectorState cs = (ConnectorState)state;
            cs.Thread = Thread.CurrentThread;
            try
            {
                TcpClient tc = new TcpClient(cs.Hostname, cs.Port);
                if (cs.Aborted)
                {
                    try { tc.GetStream().Close(); }
                    catch { }
                    try { tc.Close(); }
                    catch { }
                }
                else
                {
                    cs.TcpClient = tc;
                    cs.Completed.Set();
                }
            }
            catch (Exception e)
            {
                cs.Exception = e;
                cs.Completed.Set();
            }
        }

        private class ConnectorState
        {
            public string Hostname;
            public int Port;
            public volatile Thread Thread;
            public readonly ManualResetEvent Completed = new ManualResetEvent(false);
            public volatile TcpClient TcpClient;
            public volatile Exception Exception;
            public volatile bool Aborted;
            public void Abort()
            {
                if (Aborted != true)
                {
                    Aborted = true;
                    try { Thread.Abort(); }
                    catch { }
                }
            }
        }
    }



    public class MasterApplication
    {

        #region 用户变量
        static string strSQLSrver = "";
        static string strConn = "";
        static string strAppName = "";
        #endregion

        /// <summary>
        /// Master专用的程序启动函数，主要包括程序更新版本功能
        /// </summary>
        /// <param name="appName">应用程序标识</param>
        /// <param name="strModuleName">启动模块名</param>
        /// <param name="fmName">启动窗体全名</param>
        /// <returns></returns>
        public static Form Start(string appName,string strModuleName,string fmName)
        {
            strAppName = appName;
            bool flag;
            Mutex mutex;
            mutex = new Mutex(true, string.Format("{0}.HAS.Run#",appName), out flag);
            if (flag)
            {
                try
                {
                    //更新程序模块
                    try
                    {
                        fun_初始化参数();
                    }
                    catch { }
                    bool blUP = true;
                    if (IsAdministrator() == false)
                    {
                        Debug.WriteLine("当前用户不是管理员");
                        if (IsAllESS() == false)
                        {
                            Debug.WriteLine("当前用户没有程序目录权限,不需要启动更新机制");
                            blUP = false;
                        }
                        else
                        {
                            Debug.WriteLine("当前用户有程序目录权限,可以启动更新机制");
                        }
                    }

                    fun_写入注册表();
                    if (strSQLSrver != "" && blUP == true)
                    {
                        try
                        {
                            using (System.Net.Sockets.TcpClient tc = TcpClientConnector.Connect(strSQLSrver, 1433, 5000))
                            {
                                tc.Close();
                                DSSupdata();
                            }
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine("没有找到服务器,程序不自动升级版本");
                        }
                    }
                }
                catch
                {
                }
            }
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                string Dir = System.AppDomain.CurrentDomain.BaseDirectory;

                Assembly ass = Assembly.LoadFrom(Dir + strModuleName + ".dll");
                Type tp = ass.GetType(fmName);

                System.Diagnostics.Debug.WriteLine(tp.ToString());
                return  Activator.CreateInstance(tp) as System.Windows.Forms.Form;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 注册表操作
        //去除网上证书校验。重启后有效。对程序启动速度特别慢的电脑有效。
        private static void fun_写入注册表()
        {
            try
            {
                RegistryKey hklm = Registry.CurrentUser;
                RegistryKey software = hklm.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\WinTrust\Trust Providers\Software Publishing", true);
                software.SetValue("State", 0x00023e00);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 读取参数用
        private static void fun_初始化参数()
        {
            string strDir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
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
            Dictionary<string, string> li = new Dictionary<string, string>();
            foreach (DataRow r in dtM.Rows)
            {
                li.Add(r[0].ToString(), r[1].ToString());
            }
            strSQLSrver = li["SQLServer"];
            strConn = string.Format("Provider=SQLOLEDB.1;Password={0};Persist Security Info=True;User ID={1};Initial Catalog={2};Data Source={3};Pooling=true;Max Pool Size=40000;Min Pool Size=0",
                        li["PWD"], li["UID"], li["DataBase"], li["SQLServer"]);

        }
        #endregion

        #region 数据更新用
        private static void DSSupdata()
        {
            try
            {   
                string sql = "select moduleName,ver,hash,fileName,moduleDesc from moduleData";
                DataTable dtCurrM = new DataTable();
                string str_dt_moduleVer = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + string.Format("\\{0}\\moduleVer.cfg",strAppName);
                try
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(str_dt_moduleVer));
                }
                catch { }
                if (File.Exists(str_dt_moduleVer))
                {
                    dtCurrM.ReadXml(str_dt_moduleVer);
                }
                DataTable dtM = new DataTable();
                using (OleDbDataAdapter da = new OleDbDataAdapter(sql, strConn))
                {
                    da.Fill(dtM);
                    Boolean blSeccess = true;
                    foreach (DataRow r1 in dtM.Rows)
                    {
                        Boolean blUP = false;
                        Boolean blOtherDir = false;
                        try
                        {
                            //得到标准的文件路径
                            string strFile = r1["fileName"].ToString();
                            if (strFile.IndexOf("%system64%") == 0)
                            {
                                blOtherDir = true;
                                if (IntPtr.Size == 4)
                                {
                                    continue;
                                }
                                strFile = strFile.Replace("%system64%", Environment.GetFolderPath(Environment.SpecialFolder.System));
                            }
                            if (strFile.IndexOf("%system32%") == 0)
                            {
                                blOtherDir = true;
                                if (IntPtr.Size == 4)
                                {
                                    strFile = strFile.Replace("%system32%", Environment.GetFolderPath(Environment.SpecialFolder.System));
                                }
                                else
                                {
                                    strFile = strFile.Replace("%system32%", System.IO.Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System)) + "\\SysWOW64");
                                }
                            }

                            if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(strFile)) == false)
                            {
                                strFile = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\" +
                                    r1["fileName"].ToString();
                            }
                            sql = string.Format("moduleName = '{0}'", r1["moduleName"].ToString());
                            if (dtCurrM.Rows.Count == 0)
                            {
                                blUP = true;
                            }
                            else
                            {
                                DataRow[] rs = dtCurrM.Select(sql);
                                if (rs.Length > 0)
                                {
                                    if (System.IO.File.Exists(strFile) == false || (int)r1["ver"] > (int)rs[0]["ver"])
                                    {
                                        blUP = true;
                                    }
                                }
                                else
                                {
                                    blUP = true;
                                }
                            }
                            if (blUP)
                            {
                                sql = string.Format("select * from moduleData where moduleName = '{0}'", r1["moduleName"].ToString());
                                DataTable dt = new DataTable();
                                using (OleDbDataAdapter da1 = new OleDbDataAdapter(sql, strConn))
                                {
                                    da1.Fill(dt);
                                }
                                try
                                {
                                    System.IO.File.Delete(strFile);
                                    System.IO.File.WriteAllBytes(strFile, (byte[])dt.Rows[0]["moduleData"]);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                    if (blOtherDir == true)
                                    {
                                        //如果是系统目录。不要处理
                                    }
                                    else
                                    {

                                        blSeccess = false;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            blSeccess = false;
                        }
                    }
                    if (blSeccess)
                    {
                        dtM.TableName = "dt_moduleVer";
                        dtM.WriteXml(str_dt_moduleVer, XmlWriteMode.WriteSchema);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region 管理员及权限相关
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        public static bool IsAllESS()
        {
            try
            {
                string Dir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                string SF = Dir + "\\" + System.Guid.NewGuid().ToString();
                System.IO.File.WriteAllText(SF, "AAAA");
                System.IO.File.Delete(SF);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

    }
}
