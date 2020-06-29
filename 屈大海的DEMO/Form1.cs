using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 屈大海的DEMO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
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



            DataTable dt = new DataTable();
            dt.Columns.Add("A");
            dt.Columns.Add("b");
            dt.Columns.Add("c");

            for (int i = 0; i <= 10; i++)
            {
                dt.Rows.Add("1", "2", "3");
                dt.Rows.Add("2", "2", "3");
                dt.Rows.Add("3", "2", "3");
                dt.Rows.Add("4", "2", "3");
                dt.Rows.Add("5", "2", "3");
            }

            //dt.Rows.Add("2", "5", "2", "3", "5", "2", "3", "5", "2", "3", "5", "2", "3", "5", "2", "3", "5", "2", "3", "5", "2", "3");

            gcM.DataSource = dt;
            //dt.Columns[0 = "33333";
            CZMaster.DevGridControlHelper.Helper(this);
            //CZMaster.DevGridControlHelper.Helper(gvM);

        }

        int i = 1;
        private void gvM_ColumnChanged(object sender, EventArgs e)
        {

        }

        private void gvM_ColumnPositionChanged(object sender, EventArgs e)
        {
            i++;
            textBox1.Text = i.ToString();
            gvM.SaveLayoutToXml("C:\\123.xml", DevExpress.Utils.OptionsLayoutBase.FullLayout);
        }

        private void gvM_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            i++;
            textBox1.Text = i.ToString();
            gvM.SaveLayoutToXml("C:\\123.xml",DevExpress.Utils.OptionsLayoutBase.FullLayout);

        }

        private void gcM_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                gvM.RestoreLayoutFromXml("C:\\123.xml");
            }
            catch
            {

            }
        }
    }
}
