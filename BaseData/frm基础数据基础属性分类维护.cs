using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CPublic;
using CZMaster;



namespace BaseData
{
    public partial class frm基础数据基础属性分类维护 : UserControl
    {

        #region 公有成员

        #endregion


        #region  私有成员

        /// <summary>
        /// 主表:属性分类大类表
        /// </summary>
        DataTable dtM;

        CurrencyManager cmM;

        #endregion


        #region  类加载


        public frm基础数据基础属性分类维护()
        {
            InitializeComponent();
        }

        private void frm基础数据基础属性分类维护_Load(object sender, EventArgs e)
        {
            try
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
                ////DevExpress.XtraReports.Localization.ReportLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraReportsLocalizationCHS();
                ////DevExpress.XtraRichEdit.Localization.XtraRichEditLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraRichEditLocalizationCHS();
                ////DevExpress.XtraRichEdit.Localization.RichEditExtensionsLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraRichEditExtensionsLocalizationCHS();
                ////DevExpress.XtraScheduler.Localization.SchedulerLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSchedulerLocalizationCHS();
                ////DevExpress.XtraScheduler.Localization.SchedulerExtensionsLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSchedulerExtensionsLocalizationCHS();
                ////DevExpress.XtraSpellChecker.Localization.SpellCheckerLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraSpellCheckerLocalizationCHS();
                ////DevExpress.XtraTreeList.Localization.TreeListLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraTreeListLocalizationCHS();
                ////DevExpress.XtraVerticalGrid.Localization.VGridLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraVerticalGridLocalizationCHS();
                ////DevExpress.XtraWizard.Localization.WizardLocalizer.Active = new DevExpress.LocalizationCHS.DevExpressXtraWizardLocalizationCHS();
                #endregion
                fun_主数据读取();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }         
        }

        #endregion


        #region 其他数据处理

        /// <summary>
        /// 输入数据合法性的检查
        /// </summary>
        private void fun_Check()
        {
           foreach (DataRow r in dtM.Rows)
          {
              if (r.RowState == DataRowState.Deleted) continue; //遇到已删除的就跳过
              //属性值不能为空
              if (r["属性值"].ToString() == "")
             {
                throw new Exception("属性值不能为空，请检查！");
             }

             //属性值不能够重复
             DataRow[] dr = dtM.Select(string.Format("属性值='{0}'", r["属性值"].ToString()));
             if (dr.Length >= 2)
             {
                 throw new Exception("属性值有重复，请检查！");
             }

             //如果POS不输入，默认为0
             if (r["POS"].ToString() == "")
             {
                 r["POS"] = 0;
             }

             //排列顺序只能输入数字
             int i = 0;
             try
             {
                 i = int.Parse(r["POS"].ToString());
             }
             catch
             {
                 throw new Exception("排列顺序只能输入数字，请检查！");
             }



           }
        }


        #endregion


        #region 数据库的读取与保存

        /// <summary>
        /// 数据的载入读取
        /// </summary>
        private void fun_主数据读取()
        {
            string sql = "select * from 基础数据基础属性分类表 order by POS";
            dtM=MasterSQL.Get_DataTable(sql,CPublic.Var.strConn);
            cmM=this.BindingContext[dtM] as CurrencyManager;
            gcM.DataSource = dtM;
        }

        /// <summary>
        /// 新增或修改之后的数据保存
        /// </summary>
        private void fun_数据保存()
        {
            //POS的排列顺序
            DataView dv =new DataView(dtM);
            dv.Sort = "POS";
            int i = 0;
            foreach (DataRowView drv in dv)
            {
                DataRow r = drv.Row;
                r["POS"] = i++;
            }

            MasterSQL.Save_DataTable(dtM, "基础数据基础属性分类表", CPublic.Var.strConn);
        }

        #endregion


        #region  界面操作


        /// <summary>
        /// 刷新操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_主数据读取();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }     
        }

        /// <summary>
        /// 新增的操作:新增一行的操作，处于ADD状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
       {       
            gvM.CloseEditor();
            cmM.EndCurrentEdit();
            try
            {
                cmM.AddNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }    
       }

        /// <summary>
        /// 删除操作：将要删除的行打上delete标记，需要执行保存操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvM.CloseEditor();
            cmM.EndCurrentEdit();
            DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;
            try
            {
                if (MessageBox.Show(string.Format("你确定要删除\"{0}\" 吗", r["属性值"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 数据的保存操作：新增后的保存，修改后的保存，删除后的保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvM.CloseEditor();
            cmM.EndCurrentEdit();
            try
            {
                fun_Check();
                fun_数据保存();
                fun_主数据读取();
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }       
        }


        #endregion

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }


    }
}
