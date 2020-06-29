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
namespace ERPpurchase
{
    public partial class ui拒收核销明细查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        public ui拒收核销明细查询()
        {
            InitializeComponent();
        }

        private void ui拒收核销明细查询_Load(object sender, EventArgs e)
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

                DateTime t = CPublic.Var.getDatetime();
                barEditItem1.EditValue = t.Date.AddMonths(-2);
                barEditItem3.EditValue = t;
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue).Date;
            DateTime t2 = Convert.ToDateTime(barEditItem3.EditValue).Date.AddDays(1).AddSeconds(-1);
            if (t1 > t2)
            {
                throw new Exception("第一个时间不能大于第二个时间！");
            }
            string sql = string.Format(@"select a.*,b.物料名称,b.规格型号 from 采购拒收核销记录表 a left join 基础数据物料信息表 b on a.物料编码=b.物料编码 where 核销时间 >='{0}' and 核销时间 <='{1}'",t1,t2);
            DataTable dt_核销 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_核销;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
