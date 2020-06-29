using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using 郭恒的DEMO;

namespace 郭恒的Demo
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");//zh-CN
            // Application.Run(new fmbandedview());
           // Application.Run(new 中英文版本());

            Application.Run(new 测试下拉 ());
            //Application.Run(new 校验码2());

            //Application.Run(new 补检验记录());
            //Application.Run(new 存货核算和成本核算());
            // Application.Run(new fmU8委外发料());
            //Application.Run(new sync_u8_借出借用单());


            //  Application.Run(new 导入用友BOM());

            //Application.Run(new fm校验未入库倒冲物料及数量());





        }
    }
}
