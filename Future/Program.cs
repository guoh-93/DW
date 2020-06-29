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
//using System.Reflection.Emit;


namespace Future
{
    static class Program
    {
        #region 用户变量
        static string strSQLSrver = "";
        static string strConn = "";
        #endregion

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Form fm = CZMaster.MasterApplication.Start("Future", "FutureMain", "FutureMain.FutureMainFM");
                Application.Run(fm);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }
    }
}
