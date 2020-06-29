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
    public partial class frm不合格品信息 : UserControl
    {
        public frm不合格品信息()
        {
            InitializeComponent();
        }

        #region   变量

        string strCPSN = "";
        string strJCBZ = "";
        string strSNBZ = "";
        string strJTMC = "";
        string strTime1 = "";
        string strTime2 = "";



        #endregion



        DataTable dtM;

        DataTable dt_动作;

        private void frm不合格品信息_Load(object sender, EventArgs e)
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

            LA_chanpinsn.Text = "";
            LA_cpjcbz.Text = "";
            LA_cpsnbz.Text = "";
            LA_jcjtmc.Text = "";
            LA_cuotime1.Text = "";
            LA_cuotime2.Text = "";



            try
            {
                fun_加载动作表();
                fun_load不合格数据();
                fun_load参数组合();
                //多行的
                gv1.ShownEditor += gv1_ShownEditor;
                gc1.EditorKeyUp += gc1_EditorKeyUp;
                gc1.DataSource = dtM;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region  多行输入的方法

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


        private void fun_加载动作表()
        {
            string sql = "select * from ABB动作表";
            dt_动作 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }


        //不合格数据信息全部加载进来
        private void fun_load不合格数据()
        {
            string sql = "select * from ABB不合格产品表";
            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            dtM.Columns.Add("输入参数");
            dtM.Columns.Add("实际返回");
        }

        //把参数组合起来
        private void fun_load参数组合()
        {
            foreach (DataRow r in dtM.Rows)
            {
                DataRow[] dr = dt_动作.Select(string.Format("动作ID='{0}'", r["出错动作ID"]));
                if (dr.Length > 0)
                {
                    for (int i = 1; i <= Convert.ToInt32(dr[0]["动作参数个数"]); i++)
                    {
                        r["输入参数"] += dr[0]["P" + i + ""] + "：" + r["P" + i + ""] + " ";
                      //  r["实际返回"] += dr[0]["R" + i + ""] + "：" + r["R" + i + ""] + " ";
                    }

                    for (int j = 1; j <= Convert.ToInt32(dr[0]["结果返回个数"]); j++)
                    {
                        r["实际返回"] += dr[0]["R" + j + ""] + "：" + r["R" + j + ""] + " ";
                    }
                    //r["输入参数"] = dr[0]["P1"] + "：" + r["P1"] + " " + dr[0]["P2"] + "：" + r["P2"] + " " + dr[0]["P3"] + "：" + r["P3"] + " " + dr[0]["P4"] + "：" + r["P4"] + " " + dr[0]["P5"] + "：" + r["P5"] + " " + dr[0]["P6"] + "：" + r["P6"] + " " + dr[0]["P7"] + "：" + r["P7"] + " " + dr[0]["P8"] + "：" + r["P8"] + " " + dr[0]["P9"] + "：" + r["P9"] + " " + dr[0]["P10"] + "：" + r["P10"];
                    //r["实际返回"] = dr[0]["R1"] + "：" + r["R1"] + " " + dr[0]["R2"] + "：" + r["R2"] + " " + dr[0]["R3"] + "：" + r["R3"] + " " + dr[0]["R4"] + "：" + r["R4"] + " " + dr[0]["R5"] + "：" + r["R5"] + " " + dr[0]["R6"] + "：" + r["R6"] + " " + dr[0]["R7"] + "：" + r["R7"] + " " + dr[0]["R8"] + "：" + r["R8"] + " " + dr[0]["R9"] + "：" + r["R9"] + " " + dr[0]["R10"] + "：" + r["R10"];
                }
            }
        }


        //EXCEL表的导出
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "导出Excel";
            fileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = fileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                gc1.ExportToXlsx(fileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void fun_sn查询()
        {
            string sql =string.Format("select * from ABB不合格产品表 where 产品SN号='{0}'",LA_cpsn.EditValue.ToString());
            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            dtM.Columns.Add("输入参数");
            dtM.Columns.Add("实际返回");
        }


        //按照产品的SN号进行查询
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (LA_cpsn.EditValue == null)
                {
                    LA_cpsn.EditValue = "";
                }
                if (LA_cpsn.EditValue.ToString() == "")
                    throw new Exception("请输入产品SN号进行查询！");
                fun_sn查询();
                fun_load参数组合();
                gc1.DataSource = dtM;
                //DataRow[] dr1 = dtM.Select(string.Format("产品SN号='{0}'", LA_cpsn.EditValue.ToString()));
                //if (dr1.Length > 0)
                //{
                //    gc1.DataSource = dr1;
                //}
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //重新刷新
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load不合格数据();
                fun_load参数组合();
            }
            catch
            {

            }
        }



        private void fun_find不合格数据()
        {
            string strsql = "";

            if (strCPSN != "")
            {
                strsql = string.Format("产品SN号='{0}'", strCPSN);
            }

            if (strJCBZ != "")
            {
                strsql = string.Format(" and 检测大类='{0}'", strJCBZ);
            }

            if (strSNBZ != "")
            {
                if (strSNBZ == "标准SN号")
                {
                    
                }

                if (strSNBZ == "非标准SN号")
                {

                }

                if (strSNBZ == "所有SN号")
                {

                }

                strsql=string.Format(" and ");
            }

            if (strJTMC != "")
            {
                strsql = string.Format(" and 机台名称='{0}'", strJTMC);
            }

            if (strTime1 != "" && strTime2 != "")
            {
                strsql = string.Format(" and 出错时间 between {0} and {2}", strTime1, strTime2);
            }

         

           



        }


        //查询SN号
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            strCPSN = LA_chanpinsn.Text;
            strJCBZ = LA_cpjcbz.Text;
            strSNBZ = LA_cpsnbz.Text;
            strJTMC = LA_jcjtmc.Text;
            strTime1 = LA_cuotime1.EditValue.ToString();
            strTime2 = LA_cuotime2.EditValue.ToString();




        }



    }
}
