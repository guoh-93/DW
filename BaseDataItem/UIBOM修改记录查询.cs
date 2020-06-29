using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class UIBOM修改记录查询 : UserControl
    {

        DataTable dtM = new DataTable();
        string strcon = CPublic.Var.strConn;
        public UIBOM修改记录查询()
        {
            InitializeComponent();
            barEditItem1.EditValue = CPublic.Var.getDatetime().Date.AddMonths(-1);
            barEditItem2.EditValue = CPublic.Var.getDatetime().Date;
        }

        private void UIBOM修改记录查询cs_Load(object sender, EventArgs e)
        {

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_search();
        }

        private void fun_search()
        {

            DateTime dtime =Convert.ToDateTime(barEditItem2.EditValue).AddDays(1);
           
             DateTime t = Convert.ToDateTime(barEditItem1.EditValue).Date;
             t = new DateTime(t.Year, t.Month,t.Day);

             string sql = string.Format(@"select 基础数据BOM信息修改记录表.*,a.物料编码 as 成品编号,a.n原ERP规格型号 as 成品规格,b.物料编码 as 更改前子项编码,b.物料名称 as 更改前物料名称
                    ,b.图纸编号 as 更改前子项图号,c.物料编码 as 更改后子项编码,c.物料名称 as 更改后物料名称,c.图纸编号 as 更改后子项图号  from 基础数据BOM信息修改记录表 
                    left   join 基础数据物料信息表 as a on 基础数据BOM信息修改记录表.成品编码=a.物料编码
                    left  join  基础数据物料信息表 as b  on 基础数据BOM信息修改记录表.更改前物料=b.物料编码
                    left  join  基础数据物料信息表 as c on 基础数据BOM信息修改记录表.更改后物料=c.物料编码 
                where 基础数据BOM信息修改记录表.修改日期>'{0}' and 基础数据BOM信息修改记录表.修改日期<='{1}'"
                    ,t.ToString("yyyy-MM-dd"), dtime.ToString("yyyy-MM-dd"));
             dtM = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
             gridControl1.DataSource = dtM;
        
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

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
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                    gridControl1.ExportToXlsx(saveFileDialog.FileName);

                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
          
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
