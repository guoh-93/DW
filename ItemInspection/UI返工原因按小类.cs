using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ItemInspection
{
    public partial class UI返工原因按小类 : UserControl
    {
        #region

        DataTable dt_产品;
        string strcon = CPublic.Var.strConn;
        #endregion 
        public UI返工原因按小类()
        {
            InitializeComponent();

            barEditItem2.EditValue = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
            barEditItem1.EditValue = CPublic.Var.getDatetime().AddMonths(-1).ToString("yyyy-MM-dd");
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void fun_load()
        {

            string sql_1 = string.Format(@"select a.小类,返工原因,SUM(数量)as 数量  from 
                        (select 成品检验检验记录返工表.*,基础数据物料信息表.小类 ,生产记录生产检验单主表.原规格型号 from 成品检验检验记录返工表,生产记录生产检验单主表,基础数据物料信息表 
                         where 成品检验检验记录返工表.生产检验单号 = 生产记录生产检验单主表.生产检验单号  and 生产记录生产检验单主表.检验日期>='{0}'
                          and 基础数据物料信息表.物料编码 =生产记录生产检验单主表.物料编码 and 生产记录生产检验单主表.检验日期<='{1}')a  
                        group by a.小类,返工原因 order by 数量 ",
                   barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            DataTable dt_mx = new DataTable();
            dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
            gridControl1.DataSource = dt_mx;

        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql = string.Format(@"select 生产工单号,返工数量,生产记录生产检验单主表.原规格型号,大类,小类,生产记录生产检验单主表.负责人员,生产记录生产检验单主表.检验日期 
                                from 成品检验检验记录返工表,生产记录生产检验单主表,基础数据物料信息表 where 基础数据物料信息表.物料编码=生产记录生产检验单主表.物料编码 and
                                 成品检验检验记录返工表.生产检验单号=生产记录生产检验单主表.生产检验单号  and   检验日期>='{0}' and 检验日期<='{1}'
                                and 基础数据物料信息表.小类='{2}' and 返工原因='{3}' order by 返工数量", Convert.ToDateTime(barEditItem1.EditValue), Convert.ToDateTime(barEditItem2.EditValue).AddDays(1)
                                 , dr["小类"], dr["返工原因"].ToString().Trim());
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl2.DataSource = dt;
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
    }
}
