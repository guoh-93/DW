using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
namespace ERPStock
{
    public partial class ui可用库存查询 : UserControl
    {
        string s_物料编码 = "";
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";

        public ui可用库存查询()
        {
            InitializeComponent();
        }

        public ui可用库存查询(string s_物料)
        {
            InitializeComponent();
            s_物料编码 = s_物料;
        }

        private void ui可用库存查询_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel1, this.Name, cfgfilepath);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void fun_load()
        {
            string sql = string.Format(@"select * from  仓库物料数量表 where 物料编码 = '{0}' 
                                     and 仓库号 in(select 属性字段1 as 仓库号 from 基础数据基础属性表 
                                      where 属性类别 = '仓库类别' and 布尔字段3 = 1)", s_物料编码);
            DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_物料;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
