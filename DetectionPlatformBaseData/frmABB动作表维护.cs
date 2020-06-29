using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CPublic;
using CZMaster;

namespace DetectionPlatformBaseData
{
    public partial class frmABB动作表维护 : UserControl
    {


        #region 变量

        DataTable dtP;
        CurrencyManager cmM;


        #endregion

        #region 类加载

        public frmABB动作表维护()
        {
            InitializeComponent();
        }


        private void frmABB动作表维护_Load(object sender, EventArgs e)
        {
            //try
            //{
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
                fun_load();
                cmM = this.BindingContext[dtP] as CurrencyManager;

                //多行输入的两个事件
                gvM.ShownEditor += gvM_ShownEditor;
                gcM.EditorKeyUp += gcM_EditorKeyUp;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        #endregion

        #region 数据检查

        #region 多行输入  

        void gcM_EditorKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && (gvM.ActiveEditor is DevExpress.XtraEditors.MemoEdit))
            {
                gvM.CloseEditor();
                gvM.RefreshData();
                gvM.ShowEditor();
            }
        }

        void gvM_ShownEditor(object sender, EventArgs e)
        {
            if (gvM.ActiveEditor is DevExpress.XtraEditors.MemoEdit)
            {
                DevExpress.XtraEditors.MemoEdit me = gvM.ActiveEditor as DevExpress.XtraEditors.MemoEdit;
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



        //数据检查
        private void fun_check()
        {
            //数据项检查
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                if (r["动作ID"].ToString() == "")
                    throw new Exception("动作编号有空值，请检查！");
                DataRow[] r1 = dtP.Select(string.Format("动作ID='{0}'", r["动作ID"].ToString()));
                if (r1.Length > 1)
                    throw new Exception("动作ID有重复，检查之后重新填写！");
                //try
                //{
                //    int i = Convert.ToInt32(r["动作ID"].ToString());
                //}
                //catch
                //{
                //    throw new Exception("动作编号需要是数字！");
                //}
                if (r["动作说明"].ToString() == "")
                    throw new Exception("动作说明有空值，请检查！");
                if (r["动作大类"].ToString() == "")
                    throw new Exception("动作大类有空值，请检查！");
                try
                {
                    int i = Convert.ToInt32(r["动作参数个数"].ToString());
                }
                catch
                {
                    throw new Exception("动作参数是数字，表示该动作有几个参数，请检查！");
                }

            }
        }



        #endregion

        #region 数据载入及操作

        //载入表中的数据
        private void fun_load()
        {
            string sql = "select * from ABB动作表";
            dtP = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            gcM.DataSource = dtP;
        }

        //新增一行
        private void add_NewRow()
        {
            cmM.AddNew();
        }

        //保存
        private void fun_save()
        {
            MasterSQL.Save_DataTable(dtP, "ABB动作表", CPublic.Var.geConn("PLC"));
        }


        #endregion

        #region   界面相关操作


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


        //新增操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gvM.CloseEditor();
                cmM.EndCurrentEdit(); 
                add_NewRow();
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
                gvM.CloseEditor();
                cmM.EndCurrentEdit();
                DataRow r = (this.BindingContext[dtP].Current as DataRowView).Row;
                if (MessageBox.Show(string.Format("你确定要删除动作ID是\"{0}\"的动作？", r["动作ID"].ToString()), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    r.Delete();
                   // MessageBox.Show("删除成功！记得点保存！");
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
                gvM.CloseEditor();
                cmM.EndCurrentEdit();
                fun_check();
                fun_save();
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
