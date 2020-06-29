using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace 赵峰的DEMO
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
            //Application.Run(new fm修改规格());
            //Application.Run(new Fm批量初始化());
            //Application.Run(new fm待审核改已审核());
            //Application.Run(new fm模具信息());
            //Application.Run(new fm导入库存());
            //Application.Run(new frm修改库存锁定量());
            //Application.Run(new frm修改仓库名称());
            //Application.Run(new frm修改人事员工());
            //Application.Run(new 通过原材料查询销售单中的成品());
            //Application.Run(new frm可售原材料());
            //Application.Run(new frm链接MySql());
            //Application.Run(new frm改正MRP三个量());
            Application.Run(new Form3());
        }
    }
}
