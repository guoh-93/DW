using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CZMaster;

namespace PLCView
{
    public partial class frm时间数据视图 : UserControl
    {
        public frm时间数据视图()
        {
            InitializeComponent();
        }

        string strconn = "Password=MESSA;Persist Security Info=True;User ID=MESSA;Initial Catalog=自动检测数据;Data Source=218.244.150.177";

        #region   变量

        DataTable dtM;

        DataTable dt_检测标准;

        DataTable dtM_总表;

        DataTable dtM_动作表;

        DataTable dtM_显示;

        DataTable dtM_界面显示 = new DataTable();


        DataTable dtM_检验总;

        DataTable dtM_增加;

        int jcount;

        //检测标准
        string strFindSN = "";  
        //检测标准
        string strFindBZ = ""; 
        //检测结果
        string strFindJG = "";
        //开始时间
        string strFindks = "";
        //结束时间
        string strFindjs = "";
        //SN标准
        string strSNBZ = "";

        //每个电压点的分合闸实际时间
        string time50v = "", time155v = "", time195v = "", time253v = "", time275v = "", time300v = "", time400v = "", time350v="";

        //电压点
        string dy275v = "", dy300v = "", dy350v = "", dy400v = "", dy155v = "", dy50v = "", dy195v = "", dy253v = "";

        string cpsn = "";

        string strbaobiao = "";


        //线程结束标注
        int flag = 0;


        #endregion


        #region   方案


        private void fun_载入全部检测结果动作表()
        {
            string sql = "select * from ABB检测结果动作表";
            dtM = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
        }




        #endregion


        private void frm时间数据视图_Load(object sender, EventArgs e)
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
            LA_jcbzfind.SelectedText = "ABB检测简化流程";
            LA_jgfind.SelectedText = "PASS";
            fun_检测标准();
        }


        //数据显示
        private void fun_界面数据显示()
        {
            dtM_界面显示 = dtM_显示.Clone();

            //所有的SN号
            if (strSNBZ == "" || strSNBZ == "所有SN号")
            {
                if (strFindJG == "PASS")
                {
                    foreach (DataRow r in dtM_显示.Rows)
                    {
                        if (cpsn != r["产品SN号"].ToString())
                        {

                            cpsn = r["产品SN号"].ToString();
                            dtM_界面显示.Rows.Add(r.ItemArray);
                        }
                    }
                    cpsn = "";
                }
                else
                {
                    foreach (DataRow r in dtM_显示.Rows)
                    {
                        dtM_界面显示.Rows.Add(r.ItemArray);
                    }
                }
            }
            //标准的SN号
            if (strSNBZ == "标准SN号")
            {
                if (strFindJG == "PASS")
                {
                    foreach (DataRow r in dtM_显示.Rows)
                    {
                        if (cpsn != r["产品SN号"].ToString())
                        {
                            cpsn = r["产品SN号"].ToString();
                            try
                            {
                                Int64 i = Convert.ToInt64(r["产品SN号"]);
                                if (r["产品SN号"].ToString().Length == 13)
                                {
                                    dtM_界面显示.Rows.Add(r.ItemArray);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        cpsn = "";
                    }
                }
                else
                {
                    foreach (DataRow r in dtM_显示.Rows)
                    {
                        try
                        {
                            Int64 i = Convert.ToInt64(r["产品SN号"]);
                            if (r["产品SN号"].ToString().Length == 13)
                            {
                                dtM_界面显示.Rows.Add(r.ItemArray);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            if (strSNBZ == "非标准SN号")
            {
                foreach (DataRow r in dtM_显示.Rows)
                {
                    if (strFindJG == "PASS")
                    {
                        if (cpsn != r["产品SN号"].ToString())
                        {
                            cpsn = r["产品SN号"].ToString();
                            try
                            {
                                Int64 i = Convert.ToInt64(r["产品SN号"]);
                                if (r["产品SN号"].ToString().Length != 13)
                                {
                                    dtM_界面显示.Rows.Add(r.ItemArray);
                                }
                            }
                            catch
                            {
                                dtM_界面显示.Rows.Add(r.ItemArray);
                                continue;
                            }
                        }
                        cpsn = "";
                    }
                    else
                    {
                        try
                        {
                            Int64 i = Convert.ToInt64(r["产品SN号"]);
                            if (r["产品SN号"].ToString().Length != 13)
                            {
                                dtM_界面显示.Rows.Add(r.ItemArray);
                            }
                        }
                        catch
                        {
                            dtM_界面显示.Rows.Add(r.ItemArray);
                            continue;
                        }
                    }
                }
            }
        }

        //查找检测标准
        private void fun_检测标准()
        {
            string sql = "select * from ABB检测类型主表";
            dt_检测标准 =MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            if (dt_检测标准.Rows.Count > 0)
            {
                foreach (DataRow r in dt_检测标准.Rows)
                {
                    LA_jcbzfind.Properties.Items.Add(r["检测名称"].ToString());
                }
            }
        }



        //查询动作表中的数据


        private List<string> fun_检测结果动作表(string jczGUID,string jccpsn,string baobiao)
        {
            #region

            List<string> list = new List<string>();
            DataRow[] dr = dtM.Select(string.Format("检测总GUID='{0}' and 产品SN号='{1}' and 报表节点='{2}'", jczGUID, jccpsn, baobiao));
            if (dr.Length > 0)
            {
                list.Add((Convert.ToDouble(dr[0]["R1"]) / 1000).ToString());
                list.Add(dr[0]["R5"].ToString());
                return list;
            }
            else
            {
                list.Add("");
                list.Add("");
                return list;
            }

            #endregion

            #region  原方案
            //List<string> list = new List<string>();
            //if (dtM == null)
            //{
            //    dtM = MasterSQL.Get_DataTable("select * from ABB检测结果动作表 where 1<>1", CPublic.Var.geConn("PLC"));
            //}
            //DataRow[] dr = dtM.Select(string.Format("检测总GUID='{0}' and 产品SN号='{1}' and 报表节点='{2}'", jczGUID, jccpsn, baobiao));
            //if (dr.Length > 0)
            //{
            //    list.Add((Convert.ToDouble(dr[0]["R1"]) / 1000).ToString());
            //    list.Add(dr[0]["R5"].ToString());
            //    return list;
            //}
            //else
            //{
            //    string sql = string.Format("select * from ABB检测结果动作表 where 检测总GUID='{0}' and 产品SN号='{1}' and 报表节点='{2}'", jczGUID, jccpsn, baobiao);
            //    dtM_动作表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
            //    if (dtM_动作表.Rows.Count > 0)
            //    {
            //        foreach (DataRow r in dtM_动作表.Rows)
            //        {
            //            dtM.Rows.Add(r.ItemArray);
            //        }

            //        list.Add((Convert.ToDouble(dtM_动作表.Rows[0]["R1"]) / 1000).ToString());
            //        list.Add(dtM_动作表.Rows[0]["R1"].ToString());
            //        return list;
            //    }
            //    else
            //    {
            //        list.Add("");
            //        list.Add("");
            //        return list;
            //    }
            //}

            #endregion

        }

        //重组特定的dt表
        private void fun_结果动作表数据重组()
        {
            try
            {
                List<string> list1 = new List<string>(); //取返回的list
                //ABB检测简化流程
                string sql = string.Format("select * from ABB检测类型主表 where 检测名称='{0}'", strFindBZ);
                DataTable dt_报表类型;
                dt_报表类型 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                strbaobiao = dt_报表类型.Rows[0]["报表类型"].ToString();

                if (strbaobiao == "B")
                {
                    dtM_显示 = new DataTable();
                    dtM_显示.Columns.Add("产品SN号");
                    dtM_显示.Columns.Add("50V实际分闸时间");
                    dtM_显示.Columns.Add("155V实际分闸时间");
                    dtM_显示.Columns.Add("195.5V实际合闸时间");
                    dtM_显示.Columns.Add("253V实际合闸时间");
                    dtM_显示.Columns.Add("275V实际分闸时间");
                    dtM_显示.Columns.Add("300V实际分闸时间");
                    dtM_显示.Columns.Add("400V实际分闸时间");

                    foreach (DataRow r in dtM_总表.Rows)
                    {
                        //50V的实际分闸时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "50");
                        time50v = list1[0];
                        //155V的实际分闸时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "155");
                        time155v = list1[0];
                        //195V的实际合闸时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "195.5");
                        time195v = list1[0];
                        //253V的实际合闸时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "253");
                        time253v = list1[0];
                        //275V的实际分闸时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "275");
                        time275v = list1[0];
                        //300V的实际分闸时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "300");
                        time300v = list1[0];
                        //400V的实际分闸时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "400");
                        time400v = list1[0];

                        if (strFindJG == "PASS")  //结果是PASS的，有可能是没有数据的。
                        {
                            if (time50v == "" || time155v == "" || time195v == "" || time253v == "" || time275v == "" || time300v == "" || time400v == "")
                            {
                                fun_checkpass(r["产品SN号"].ToString());
                            }

                            if (time50v == "" || time155v == "" || time195v == "" || time253v == "" || time275v == "" || time300v == "" || time400v == "")
                            {
                                continue;
                            }
                        }
                        //if (strFindJG == "PASS")
                        //{
                        //    if (time50v == "" || time155v == "" || time195v == "" || time253v == "" || time275v == "" || time300v == "" || time400v == "")
                        //    {
                        //        continue;
                        //    }
                        //}
                        dtM_显示.Rows.Add(r["产品SN号"], time50v, time155v, time195v, time253v, time275v, time300v, time400v);

                        time50v = ""; time155v = ""; time195v = ""; time253v = ""; time275v = ""; time300v = ""; time400v = "";
                    }

                    
                }

                if (strbaobiao == "A")
                {
                    dtM_显示 = new DataTable();
                    dtM_显示.Columns.Add("产品SN号");
                    dtM_显示.Columns.Add("275V实际输出电压");
                    dtM_显示.Columns.Add("275V脱扣时间");
                    dtM_显示.Columns.Add("300V实际输出电压");
                    dtM_显示.Columns.Add("300V脱扣时间");
                    dtM_显示.Columns.Add("350V实际输出电压");
                    dtM_显示.Columns.Add("350V脱扣时间");
                    dtM_显示.Columns.Add("400V实际输出电压");
                    dtM_显示.Columns.Add("400V脱扣时间");
                    dtM_显示.Columns.Add("155V实际输出电压");
                    dtM_显示.Columns.Add("155V脱扣时间");
                    dtM_显示.Columns.Add("50V实际输出电压");
                    dtM_显示.Columns.Add("50V脱扣时间");
                    dtM_显示.Columns.Add("195.5V实际输出电压");
                    dtM_显示.Columns.Add("195.5V合闸时间");
                    dtM_显示.Columns.Add("253V实际输出电压");
                    dtM_显示.Columns.Add("253V合闸时间");

                    foreach (DataRow r in dtM_总表.Rows)
                    {
                        //275V的脱扣电压点，脱扣时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "275");
                        dy275v = list1[1]; time275v = list1[0];
                        //300V的脱扣电压点，脱扣时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "300");
                        dy300v = list1[1]; time300v = list1[0];
                        //350V的脱扣电压点，脱扣时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "350");
                        dy350v = list1[1]; time350v = list1[0];
                        //400V的脱扣电压点，脱扣时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "400");
                        dy400v = list1[1]; time400v = list1[0];
                        //155V的脱扣电压点，脱扣时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "155");
                        dy155v = list1[1]; time155v = list1[0];
                        //50V的脱扣电压点，脱扣时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "50");
                        dy50v = list1[1]; time50v = list1[0];
                        //195.5V的合闸电压点，合闸时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "195");
                        dy195v = list1[1]; time195v = list1[0];
                        //253V的合闸电压点，合闸时间
                        list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "253");
                        dy253v = list1[1]; time253v = list1[0];

                        if (strFindJG == "PASS")
                        {
                            //if (dy50v == "" || dy155v == "" || dy195v == "" || dy253v == "" || dy275v == "" || dy300v == "" || dy350v == "" || dy400v == "")
                            //{
                            //    fun_checkpass(r["产品SN号"].ToString());
                            //}

                            if (time350v=="" || time50v == "" || time155v == "" || time195v == "" || time253v == "" || time275v == "" || time300v == "" || time400v == "")
                            {
                                fun_checkpass(r["产品SN号"].ToString());
                            }


                            if (time350v == "" || time50v == "" || time155v == "" || time195v == "" || time253v == "" || time275v == "" || time300v == "" || time400v == "")
                            {
                                continue;
                            }
                        }


                        dtM_显示.Rows.Add(r["产品SN号"], dy275v, time275v, dy300v, time300v, dy350v, time350v, dy400v, time400v, dy155v, time155v, dy50v, time50v, dy195v, time195v, dy253v, time253v);
                        dy275v = ""; time275v = ""; dy300v = ""; time300v = ""; dy350v = ""; time350v = ""; dy400v = ""; time400v = ""; dy155v = ""; time155v = ""; dy50v = ""; time50v = ""; dy195v = ""; time195v = ""; dy253v = ""; time253v = "";
                    }
                }

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + "  fun_结果动作表数据重组");
            }
        }


    
        //为了找出在设计动作的时候的报表节点的个数
        private void fun_检测组动作子表()
        {
            try
            {
                DataTable dt_动作子表;
                string sql = string.Format("select * from ABB检测组动作子表 where 检测名称='{0}' and 报表节点 <> ''", strFindBZ);
                dt_动作子表 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                if (dt_动作子表.Rows.Count > 0)
                {
                    jcount = dt_动作子表.Rows.Count;   //某一检测标准的报表节点的个数
                }
            }
            catch(Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_检测组动作子表");
            }
        }



        //对PASS的数据进行一个验证查询
        private void fun_checkpass(string jccpsn)
        {
            try
            {
                string sql = string.Format("select * from ABB检测结果总表 where 产品SN号='{0}' and (检测是否通过='未知' or 检测是否通过='PASS')", jccpsn);
                dtM_检验总 = MasterSQL.Get_DataTable(sql, CPublic.Var.geConn("PLC"));
                foreach (DataRow r in dtM_检验总.Rows)
                {
                    string sql1 = string.Format("select * from ABB检测结果动作表 where 检测总GUID='{0}' and 报表节点<>''", r["检测总GUID"].ToString());
                    dtM_增加 = MasterSQL.Get_DataTable(sql1, CPublic.Var.geConn("PLC"));
                    if (dtM_增加.Rows.Count == jcount)   //如果这个报表节点和设计的时候的返回的报表节点个数是一样的话
                    {
                        List<string> list1 = new List<string>(); //取返回的list
                        if (strbaobiao == "B")
                        {
                            //50V的实际分闸时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "50");
                            time50v = list1[0];
                            //155V的实际分闸时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "155");
                            time155v = list1[0];
                            //195V的实际合闸时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "195.5");
                            time195v = list1[0];
                            //253V的实际合闸时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "253");
                            time253v = list1[0];
                            //275V的实际分闸时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "275");
                            time275v = list1[0];
                            //300V的实际分闸时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "300");
                            time300v = list1[0];
                            //400V的实际分闸时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "400");
                            time400v = list1[0];
                        }

                        if (strbaobiao == "A")
                        {
                            //275V的脱扣电压点，脱扣时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "275");
                            dy275v = list1[1]; time275v = list1[0];
                            //300V的脱扣电压点，脱扣时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "300");
                            dy300v = list1[1]; time300v = list1[0];
                            //350V的脱扣电压点，脱扣时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "350");
                            dy350v = list1[1]; time350v = list1[0];
                            //400V的脱扣电压点，脱扣时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "400");
                            dy400v = list1[1]; time400v = list1[0];
                            //155V的脱扣电压点，脱扣时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "155");
                            dy155v = list1[1]; time155v = list1[0];
                            //50V的脱扣电压点，脱扣时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "50");
                            dy50v = list1[1]; time50v = list1[0];
                            //195.5V的合闸电压点，合闸时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "195");
                            dy195v = list1[1]; time195v = list1[0];
                            //253V的合闸电压点，合闸时间
                            list1 = fun_检测结果动作表(r["检测总GUID"].ToString(), r["产品SN号"].ToString(), "253");
                            dy253v = list1[1]; time253v = list1[0];
                        }

                        break;  //找到之后就只要跳出循环
                    }
                }
            }
            catch(Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + "fun_checkpass");
            }
        }



        //先查询检测结果总表的数据
        private void fun_查询检测结果总表()
        {
            try
            {
                string sqlstr = "";
                //产品的SN号 可以模糊查询开头
                if (strFindSN != "")
                {
                    sqlstr = string.Format("产品SN号 like '{0}%' and", strFindSN);
                }
                //产品的检测标准
                if (strFindBZ != "")
                {
                    sqlstr += string.Format(" 检测标准='{0}' and", strFindBZ);
                }
                //产品的检测结果
                if (strFindJG != "")
                {
                    sqlstr += string.Format(" 检测是否通过='{0}' and", strFindJG);
                }
                //检测时间的范围
                if (strFindks != "" && strFindjs != "")
                {
                    sqlstr += string.Format(" convert(datetime,结束检测时间) between '{0}' and '{1}' and", strFindks, strFindjs);
                }

                //如果不为空，说明是有查询条件的
                if (sqlstr != "")
                {
                    sqlstr = " where " + sqlstr.Substring(0, sqlstr.Length - 3);
                }
                sqlstr = string.Format("select * from ABB检测结果总表 {0} order by 产品SN号", sqlstr);
                dtM_总表 = MasterSQL.Get_DataTable(sqlstr, CPublic.Var.geConn("PLC"));
            }
            catch(Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + "  fun_查询检测结果总表");
            }
        }

        //查询检测结果动作表的数据
        private void fun_查询检测结果动作表()
        {
            try
            {
                string sqlstr = "";
                //产品的SN号
                if (strFindSN != "")
                {
                    sqlstr = string.Format("产品SN号 like '{0}%' and", strFindSN);
                }
                //产品的检测标准
                if (strFindBZ != "")
                {
                    sqlstr += string.Format(" 检测标准='{0}' and", strFindBZ);
                }
                //如果不为空，说明是有查询条件的
                if (sqlstr != "")
                {
                    sqlstr = " where " + sqlstr.Substring(0, sqlstr.Length - 3);
                }
                sqlstr = string.Format("select * from ABB检测结果动作表 {0} and 报表节点<>'' order by 产品SN号", sqlstr);
                dtM = MasterSQL.Get_DataTable(sqlstr, CPublic.Var.geConn("PLC"));
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + "  fun_查询检测结果动作表");
            }
        }



        //查询功能
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
             

                strFindSN = LA_snfind.Text;  //产品的SN号 可以进行模糊查询
                strFindBZ = LA_jcbzfind.Text;//检测标准
                strFindJG = LA_jgfind.Text; //检测结果
                strFindks = LA_kssj.Text;  //时间范围   开始时间   
                strFindjs = LA_jssj.Text;  //时间范围   结束时间
                strSNBZ = LA_snbz.Text; //SN的标准
                if (strFindBZ == "")
                    throw new Exception("检测标准不能为空，请选择！");
                if (strFindJG == "")
                    throw new Exception("检测结果不能为空，请选择！");
                if (strFindks != "" && strFindjs != "")  //两个时间都不为空
                {
                    if (Convert.ToDateTime(strFindks) > Convert.ToDateTime(strFindjs))
                        throw new Exception("时间范围选择不正确，第一个时间应小于第二个时间！");
                    strFindjs = Convert.ToDateTime(strFindjs).ToShortDateString() + " 23:59:59";
                }

                fun_检测组动作子表();   //查找某一个标准的报表节点的个数

                //先查询顶层的总表数据
                fun_查询检测结果总表();   //当总表数据不断增加的时候，同时要考虑到查询的速度问题
                if (dtM_总表.Rows.Count <= 0)
                {
                    gv1.ViewCaption = "检测数据记录:(" + "检测标准：" + strFindBZ + " 检测结果：" + strFindJG + " 数量：0)";
                    throw new Exception("查无数据！");
                }

                //动作表的查询，查询动作表。查询动作表需要考虑到查询速度的问题。
                fun_查询检测结果动作表();

                //重组数据显示
                fun_结果动作表数据重组();
                fun_界面数据显示();
                if (dtM_界面显示.Rows.Count <= 0)
                {
                    throw new Exception("查无数据！");
                }
                gc1.DataSource = dtM_界面显示;
                gv1.ViewCaption = "检测数据记录:(" + "检测标准：" + strFindBZ + " 检测结果：" + strFindJG + " 数量：" + dtM_界面显示.Rows.Count.ToString() + ")";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //导出EXECL表的功能
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        gv1.ExportToXlsx(sfd.FileName + ".xls");
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



    }
}
