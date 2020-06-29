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
    public partial class frm检测机台表维护 : UserControl
    {
        public frm检测机台表维护()
        {
            InitializeComponent();
        }

        #region  变量

        DataTable dtM;

        DataTable dtP;

        #endregion


        #region   类加载

        private void frm检测机台表维护_Load(object sender, EventArgs e)
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
                fun_load();  //主数据加载
                fun_加载机台类型();
                repositoryItemSearchLookUpEdit1.DataSource = dtP;
                repositoryItemSearchLookUpEdit1.DisplayMember = "机台类型";
                repositoryItemSearchLookUpEdit1.ValueMember = "机台类型";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        #endregion


        #region   数据检查

        //数据检查部分
        private void fun_check()
        {
            foreach (DataRow r in dtM.Rows)
            {
                //机台名称的检查
                if (r["机台名称"].ToString() == "")
                    throw new Exception("机台名称不能为空，请检查填写！");
                DataRow[] dr = dtM.Select(string.Format("机台名称='{0}'", r["机台名称"]));
                if (dr.Length > 1)
                    throw new Exception("机台名称有重复，请检查重新填写！");
                //是否使用进行检查
                if (r["使用"].ToString() == "")
                    throw new Exception("是否使用值为空，请检查填写！填写格式：1为使用，0为不使用！");

                try
                {
                    int i = Convert.ToInt32(r["使用"].ToString());
                    if (i != 0 && i != 1)
                        throw new Exception("是否使用的值为0或1，1为使用，0为不使用！请检查！");
                }
                catch
                {
                    throw new Exception("是否使用的值只能填写1或者0，1为使用，0为不使用，请检查！");
                }
            }
        }


        #endregion



        #region   加载数据

        //表数据加载进来
        private void fun_load()
        {
            string sql = "select * from 检测机台表";
            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            gc1.DataSource = dtM;
        }

        //机台类型加载进来 即机台能够做的操作
        private void fun_加载机台类型()
        {
            string sql = "select * from 检测机台类型";
            dtP = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }
        
        #endregion


        #region  界面操作调用

        //新增
        private void fun_新增()
        {
            DataRow dr = dtM.NewRow();
            dtM.Rows.Add(dr);
        }


        //保存
        private void fun_保存()
        {
            MasterSQL.Save_DataTable(dtM, "检测机台表", CPublic.Var.geConn("PLC"));
        }

        #endregion


        #region    界面的操作
        //刷新操作
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

        //新增的操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                (this.BindingContext[dtM] as CurrencyManager).EndCurrentEdit();
                gv1.CloseEditor();
                fun_新增();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //删除操作
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                (this.BindingContext[dtM] as CurrencyManager).EndCurrentEdit();
                gv1.CloseEditor();
                DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;
                if (MessageBox.Show(string.Format("你确定要删除机台名称为\"{0}\"的机台数据吗？", r["机台名称"].ToString()), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存操作
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                (this.BindingContext[dtM] as CurrencyManager).EndCurrentEdit();
                gv1.CloseEditor();
                fun_check();
                fun_保存();
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion


        
    }
}
