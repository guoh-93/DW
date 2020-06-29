using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CZMaster;


namespace PLCView
{
    public partial class frm检测结果 : UserControl
    {
        public frm检测结果()
        {
            InitializeComponent();
        }

        #region   变量

        /// <summary>
        /// 检测结果主表
        /// </summary>
        DataTable dtM;

        #endregion


        #region   类加载

        private void frm检测结果_Load(object sender, EventArgs e)
        {
            #region gridcontrol汉化代码
            //DevExpress.Accessibility.AccLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressUtilsLocalizationCHS();
            ////DevExpress.XtraBars.Localization.BarLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraBarsLocalizationCHS();
            ////DevExpress.XtraCharts.Localization.ChartLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraChartsLocalizationCHS();
            //DevExpress.XtraEditors.Controls.Localizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraEditorsLocalizationCHS();
            //DevExpress.XtraGrid.Localization.GridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraGridLocalizationCHS();
            //DevExpress.XtraLayout.Localization.LayoutLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraLayoutLocalizationCHS();
            ////DevExpress.XtraNavBar.NavBarLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraNavBarLocalizationCHS();
            ////DevExpress.XtraPivotGrid.Localization.PivotGridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraPivotGridLocalizationCHS();
            //DevExpress.XtraPrinting.Localization.PreviewLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraPrintingLocalizationCHS();
            //DevExpress.XtraReports.Localization.ReportLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraReportsLocalizationCHS();
            //DevExpress.XtraRichEdit.Localization.XtraRichEditLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraRichEditLocalizationCHS();
            //DevExpress.XtraRichEdit.Localization.RichEditExtensionsLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraRichEditExtensionsLocalizationCHS();
            //DevExpress.XtraScheduler.Localization.SchedulerLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSchedulerLocalizationCHS();
            //DevExpress.XtraScheduler.Localization.SchedulerExtensionsLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSchedulerExtensionsLocalizationCHS();
            //DevExpress.XtraSpellChecker.Localization.SpellCheckerLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSpellCheckerLocalizationCHS();
            //DevExpress.XtraTreeList.Localization.TreeListLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraTreeListLocalizationCHS();
            //DevExpress.XtraVerticalGrid.Localization.VGridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraVerticalGridLocalizationCHS();
            //DevExpress.XtraWizard.Localization.WizardLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraWizardLocalizationCHS();
            #endregion

            try
            {
                barEditItem1.EditValue = "";
                barEditItem2.EditValue = "";
                barEditItem3.EditValue = "";


               // fun_加载结果();


                //多行输入的方法
                gv1.ShownEditor += gv1_ShownEditor;
                gc1.EditorKeyUp += gc1_EditorKeyUp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        void gc1_EditorKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && (gv1.ActiveEditor is DevExpress.XtraEditors.MemoEdit))
            {
                gv1.CloseEditor();
                gv1.RefreshData();
                gv1.ShowEditor();
            }
        }

        void gv1_ShownEditor(object sender, EventArgs e)
        {
            if (gv1.ActiveEditor is DevExpress.XtraEditors.MemoEdit)
            {
                DevExpress.XtraEditors.MemoEdit me = gv1.ActiveEditor as DevExpress.XtraEditors.MemoEdit;
                try
                {
                    me.SelectionStart = me.Text.Length;
                }
                catch
                {
                }
            }
        }

        #endregion


        #region   数据加载

        //加载结果主表
        private void fun_加载结果()
        {
            try
            {
                string sql = "select * from ABB检测结果主表";
                dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                gc1.DataSource = dtM;
            }
            catch
            {
            }
        }

        #endregion


        #region   界面的操作

        //产看详细数据
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;
                fm检测结果详细 fm = new fm检测结果详细(r);
                fm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //双击产看详细的数据
        private void gc1_DoubleClick(object sender, EventArgs e)
        {
            DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;
            fm检测结果详细 fm = new fm检测结果详细(r);
            fm.ShowDialog();
        }

        #endregion

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);
  
                //gc1.ExportToExcelOld(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //查询功能
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (barEditItem2.EditValue.ToString() != "")
                {
                    string sql = string.Format("select * from ABB检测结果总表 where 检测标准='{0}' and 检测是否通过='{1}'", barEditItem2.EditValue.ToString(), barEditItem3.EditValue.ToString());
                    dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                }


                if (barEditItem1.EditValue.ToString() != "" && barEditItem2.EditValue.ToString() != "")
                {
                    string sql = string.Format("select * from ABB检测结果总表 where 产品SN号='{0}' and 检测标准='{1}' and 检测是否通过='{2}'", barEditItem1.EditValue.ToString(), barEditItem2.EditValue.ToString(), barEditItem3.EditValue.ToString());
                    dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                }

                if (barEditItem1.EditValue.ToString() == "" && barEditItem2.EditValue.ToString() == "" && barEditItem3.EditValue.ToString()!="")
                {
                    string sql = string.Format("select * from ABB检测结果总表 where 检测是否通过='{0}'", barEditItem3.EditValue.ToString());
                    dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                }

                gc1.DataSource = dtM;

            }
            catch
            {



            }





        }











    }
}
