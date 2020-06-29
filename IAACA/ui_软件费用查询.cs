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

namespace IAACA
{
    public partial class ui_软件费用查询 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_软件明细;
        public ui_软件费用查询()
        {
            InitializeComponent();
        }

        private void ui_软件费用查询_Load(object sender, EventArgs e)
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
                x.UserLayout(panel1, this.Name, cfgfilepath);

                DateTime t = CPublic.Var.getDatetime();
                barEditItem2.EditValue = Convert.ToDateTime(t.ToString("yyyy-MM-dd"));
                barEditItem1.EditValue = Convert.ToDateTime(t.AddMonths(-1).ToString("yyyy-MM-01"));
               
                fun_load();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        private void fun_load()
        {
            //string sql = string.Format(@" select   rb.*,isnull(数量,0) 软件数,isnull(数量*单价,0) as 金额 from 软件单价基础表  rb
            // left join (select   b.软件名称,SUM(a.入库数量)数量 from 生产记录成品入库单明细表  a 
            //             left join  产品软件对应表 b on a.物料编码=b.物料编码 where 生效日期 >='{0}' and 生效日期<'{1}'
            //             and b.软件名称 is not null group by  b.软件名称)x on rb.软件名称=x.软件名称 ", ((DateTime)barEditItem1.EditValue).Date, ((DateTime)barEditItem2.EditValue).Date.AddDays(1));
            string sql = string.Format(@" with t1 as(
 select   b.软件名称,SUM(a.实效数量)数量 from 仓库出入库明细表 a 
 left join  产品软件对应表 b on a.物料编码=b.物料编码
 where 出入库时间 >'{0}' and 出入库时间<'{1}' and 明细类型 ='领料出库' and left(a.物料编码,2)='10' group by  b.软件名称)
 
 ,t2 as (select   b.软件名称,SUM(a.入库数量)数量 from 生产记录成品入库单明细表  a 
  left join  产品软件对应表 b on a.物料编码=b.物料编码 where 生效日期 >='{0}' and 生效日期<='{1}'
  and b.软件名称 is not null group by  b.软件名称)
  ,t3 as(
  select 软件名称,SUM(数量)数量 from(
  select * from t1
  union select * from  t2)x  group by 软件名称)
  
   select   rb.*,isnull(数量,0) 软件数,isnull(数量*单价,0) as 金额 from 软件单价基础表  rb
  left join t3 on rb.软件名称=t3.软件名称", ((DateTime)barEditItem1.EditValue).Date, ((DateTime)barEditItem2.EditValue).Date.AddDays(1));


            dt_软件明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_软件明细;
        }
        //查询
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
        //导出
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
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

        
    }
}
