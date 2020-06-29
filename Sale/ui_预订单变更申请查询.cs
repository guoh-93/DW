using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ERPSale
{
    public partial class ui_预订单变更申请查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        public ui_预订单变更申请查询()
        {
            InitializeComponent();
        }

        private void ui_预订单变更申请查询_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this.xtraTabControl1, this.Name, cfgfilepath);
                DateTime t = CPublic.Var.getDatetime();
                barEditItem2.EditValue = t.Date.AddDays(1).AddSeconds(-1);
                barEditItem1.EditValue = t.Date.AddDays(-15);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            throw new NotImplementedException();
        }
    }
}
