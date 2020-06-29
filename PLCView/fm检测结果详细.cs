using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CZMaster;

namespace PLCView
{
    public partial class fm检测结果详细 : Form
    {

        DataRow drm;
        DataTable dtM;
        DataTable dtM_动作;

        Dictionary<string,string> dic=new Dictionary<string,string>();
       
        public fm检测结果详细(DataRow r)
        {
            InitializeComponent();
            this.drm = r;
        }




        private void fm检测结果详细_Load(object sender, EventArgs e)
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
                //多行输入
                gv1.ShownEditor += gv1_ShownEditor;
                gc1.EditorKeyUp += gv1_EditorKeyUp;
                //fun_主数据();
                //fun_加载结果();
                //fun_结果显示();
                fun_查询();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region  多行输入

        void gv1_EditorKeyUp(object sender, KeyEventArgs e)
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


        private void fun_查询()
        {
            string sql=string.Format("select * from ABB检测结果动作表 where 检测总GUID='{0}' and 产品SN号='{1}' order by 检测组POS,动作POS",drm["检测总GUID"].ToString(),drm["产品SN号"].ToString());
            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            foreach (DataRow r in dtM.Rows)
            {
                r["R1"] = Convert.ToDouble(r["R1"]) / 1000;
            }


            gc1.DataSource = dtM;
        }



        //查看的某一条主数据
        private void fun_主数据()
        {
            cpmc.Text = drm["产品名称"].ToString();
            cpsn.Text = drm["产品SN号"].ToString();
            cplx.Text = drm["产品类型"].ToString();
            cpcx.Text = drm["产品产线"].ToString();
            czy.Text = drm["操作员"].ToString();
            kssj.Text = drm["开始检测时间"].ToString();
            jssj.Text = drm["结束检测时间"].ToString();
            zsj.Text = drm["检测总时间"].ToString();
            jtmc.Text = drm["机台名称"].ToString();
            gztmc.Text = drm["工作台名称"].ToString();
            jcjg.Text = drm["检测是否通过"].ToString();
            cwdz.Text = drm["错误动作"].ToString();
        }

        //数据库的结果
        private void fun_加载结果()
        {
            string sql = string.Format("select * from ABB检测结果子表 where 产品SN号='{0}'", drm["产品SN号"].ToString());
            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            dtM.Columns.Add("输入参数");
            dtM.Columns.Add("实际返回");
        }

        private void fun_加载动作表()
        {
            string sql = "select * from ABB动作表";
            dtM_动作 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }



        //显示的效果
        private void fun_结果显示()
        {
            foreach (DataRow r in dtM.Rows)
            {
                


            }



            //dtM_显示 = new DataTable();
            //dtM_显示.Columns.Add("产品SN号");
            //dtM_显示.Columns.Add("产品线");
            //dtM_显示.Columns.Add("动作ID");
            //dtM_显示.Columns.Add("动作说明");
            //dtM_显示.Columns.Add("动作大类");
            //dtM_显示.Columns.Add("是否检测动作");
            //dtM_显示.Columns.Add("动作描述");
            //dtM_显示.Columns.Add("动作参数个数");
            //dtM_显示.Columns.Add("动作参数说明");
            //dtM_显示.Columns.Add("备注");
            //dtM_显示.Columns.Add("检测结果");
            //dtM_显示.Columns.Add("检测时间");
            //dtM_显示.Columns.Add("结果要求");

            //foreach (DataRow r in dtM.Rows)
            //{
            //    string jieguo = r["R1"].ToString() + @":" + r["VR1"].ToString() +" "+ r["R2"].ToString() + @":" + r["VR2"].ToString()+" "+ r["R3"].ToString() + @":" + r["VR3"].ToString()+" "+ r["R4"].ToString() + @":" + r["VR4"].ToString() +" "+ r["R5"].ToString() + @":" + r["VR5"].ToString();
            //    string jieguo1 = r["R6"].ToString() + @":" + r["VR6"].ToString() + " " + r["R7"].ToString() + @":" + r["VR7"].ToString() + " " + r["R8"].ToString() + @":" + r["VR8"].ToString() + " " + r["R9"].ToString() + @":" + r["VR9"].ToString() + " " + r["R10"].ToString() + @":" + r["VR10"].ToString();
            //    string jieguo2 = jieguo + jieguo1;
            //    dtM_显示.Rows.Add(r["产品SN号"], r["产品线"], r["动作ID"], r["动作说明"], r["动作大类"], r["是否检测动作"], r["动作描述"], r["动作参数个数"], r["动作参数说明"], r["备注"], r["检测结果"], r["检测时间"],jieguo2);   
            //}
            //gc1.DataSource = dtM_显示;
        }

        //动作错误日志
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //fm动作错误日志 fm = new fm动作错误日志();
            //fm.ShowDialog();
            frm多机台查看 fm = new frm多机台查看();
            fm.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // DataRow r = (this.BindingContext[dtM_显示].Current as DataRowView).Row;
             //fm分解动作子表 fm = new fm分解动作子表(r);
             //fm.ShowDialog();
        }    

 

    }
}
