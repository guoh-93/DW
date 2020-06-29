using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office;
using DevExpress.XtraPrinting;
namespace ERPreport
{
    public partial class ui入库工单BOM : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM = new DataTable();

        public ui入库工单BOM()
        {
            InitializeComponent();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void ui入库工单BOM_Load(object sender, EventArgs e)
        {
            DateTime dtime = CPublic.Var.getDatetime();

            dtime = new DateTime(dtime.Year, dtime.Month,1);

            dateEdit1.EditValue = dtime.AddMonths(-1);

            dateEdit2.EditValue = dtime.AddDays(-1);

 
      
        }
        private void fun_load()
        {

            /* 说要入库工单的物料的BOM  后来又说 是全部 
            select  fx.原ERP物料编号 as 父项编号,fx.物料名称 as 父项名称,fx.n原ERP规格型号 as 父项规格,zx.原ERP物料编号 as 子项编号,zx.物料名称 as 子项名称,zx.n原ERP规格型号 as 子项规格,
                 zx.图纸编号 as 子项图纸编号,b.数量,zx.物料类型 as 子项类型 from  (
                 select  生产工单号,物料编码 ,SUM(入库数量)入库数量 from 生产记录成品入库单明细表  where 生效日期>'{0}' and 生效日期 <'{1}' group by 生产工单号,物料编码)a 
                 left  join 基础数据物料BOM表 b   on a.物料编码=b.产品编码  
                 left join 基础数据物料信息表 fx on fx.物料编码=a.物料编码
                 left  join  基础数据物料信息表 zx on zx.物料编码=b.子项编码 
    
             * */

            string s = string.Format(@"select  fx.物料编码 as 父项编号,fx.物料名称 as 父项名称,fx.规格型号 as 父项规格,zx.物料编码 as 子项编号,zx.物料名称 as 子项名称,zx.规格型号 as 子项规格,
                 zx.图纸编号 as 子项图纸编号,b.数量,zx.物料类型 as 子项类型 from  基础数据物料BOM表 b    
                 left join 基础数据物料信息表 fx on fx.物料编码=b.产品编码
                 left  join  基础数据物料信息表 zx on zx.物料编码=b.子项编码 ");
            dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
 
            gridControl1.DataSource = dtM;

        }



        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_load();

            }
            catch (Exception ex )
            {
                MessageBox.Show("刷新出错,请重试");
 
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //导出
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";

            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new XlsxExportOptions(TextExportMode.Text, false, false);
                gridView1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           

        }
    }
}
