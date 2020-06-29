using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace ItemInspection
{
    public partial class ui一次合格率_按工单 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dtM = new DataTable();

        public ui一次合格率_按工单()
        {
            InitializeComponent();
        }
        //导出
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
           
                gridControl1.ExportToXlsx(saveFileDialog.FileName);
                MessageBox.Show("导出成功");
            }

        }
        //查询
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        private void  fun_search()
        {
            DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue).Date;
            DateTime t2 = Convert.ToDateTime(barEditItem2.EditValue).Date;

            string sql =string.Format(@"select yy.*,base.物料名称,base.规格型号,最后一次检验时间 from(
  select *, 一次合格数 / 送检数量 as 总检一次合格率, (一次合格数 + 重检合格数) / 送检数量 as 总检总合格率 from(
  select  生产工单号, 物料编码, sum(送检数量)送检数量, sum(合格数量)一次合格数, SUM(重检合格数)重检合格数  from 生产记录生产检验单主表
  where 检验日期 > '{0}' and 检验日期 < '{1}'  group by  生产工单号, 物料编码) zj)yy
  left join 基础数据物料信息表 base on base.物料编码 = yy.物料编码   
 left join (select  生产工单号,MAX(生效日期) as 最后一次检验时间 from 生产记录生产检验单主表 group by 生产工单号)jy on jy.生产工单号=yy.生产工单号
 order by 生产工单号", t1,t2);
            dtM = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
            gridControl1.DataSource = dtM;

        }

        private void ui一次合格率_按工单_Load(object sender, EventArgs e)
        {
            DateTime time = CPublic.Var.getDatetime().Date;
            barEditItem1.EditValue = Convert.ToDateTime(time.AddMonths(-1).ToString("yyyy-MM-dd"));
            barEditItem2.EditValue = Convert.ToDateTime(time.ToString("yyyy-MM-dd"));
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(panel1, this.Name, cfgfilepath);


        }
    }
}
