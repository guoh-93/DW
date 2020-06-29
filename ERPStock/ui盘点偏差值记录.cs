using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraPrinting;
namespace ERPStock
{
    public partial class ui盘点偏差值记录 : UserControl
    {
        string strcon = CPublic.Var.strConn;



        public ui盘点偏差值记录()
        {
            InitializeComponent();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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


        private void fun_load()
        {
            DateTime t=Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1);
            DataTable dtM = new DataTable();
            string sql = string.Format(@"select  [盘点记录表].*,s.库存总数 as 当前库存,(s.库存总数+偏差值) as 调整后库存,base.规格型号,base.物料编码  from [盘点记录表] 
             left join 基础数据物料信息表 base on base.物料编码=[盘点记录表].物料编号
             left join 仓库物料数量明细表 s on s.ItemId=[盘点记录表].itemid  and s.仓库号=盘点记录表.仓库号 where  盘点时间>'{0}' and 盘点时间<'{1}'", barEditItem1.EditValue.ToString(), t);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtM = new DataTable();
                da.Fill(dtM);
            }

            gridControl1.DataSource = dtM;
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void ui盘点偏差值记录_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime().Date;

                DateTime t1 = t.AddMonths(-3);
                 t1 = new DateTime (t1.Year,t1.Month,1);
                 barEditItem1.EditValue = t1;
                 barEditItem2.EditValue = t;






            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                //DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();
                //options.ExportMode = DevExpress.XtraPrinting.XlsExportMode.SingleFile;
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);
                DevExpress.XtraPrinting.XlsxExportOptions options = new XlsxExportOptions(TextExportMode.Text, false, false);
                gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    
    }
}
