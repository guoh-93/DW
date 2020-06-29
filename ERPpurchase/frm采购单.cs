using CZMaster;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class frm采购单 : UserControl
    {

        string strcon = "";
        string str_打印机;
        string cfgfilepath = "";
        public frm采购单()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        #region  变量
        //采购单主表的dt
        DataTable dt_采购单列表;
        //采购单单号
        string strcgdh = "";
        //操作员
        DataTable view_权限;
        //日期
        DateTime dttime1;
        DateTime dttime2;
        //单据状态
        string strdjzt = "";
        //采购单明细表
        DataTable dt_明细;
        DataTable dt_采购单明细;

        #endregion

        private void frm采购单_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(xtraTabControl1, this.Name, cfgfilepath);

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
                //找到进入系统的操作员的信息
                //string sql = string.Format("select * from 人事基础员工表 where 员工号='{0}'", CPublic.Var.LocalUserID);
                //dt_操作员 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                //19-4-4 
                view_权限 = ERPorg.Corg.fun_hr("采购", CPublic.Var.LocalUserID);

                //采购单号先赋空值
                //txt_cgdh.EditValue = "";
                //给一个默认的日期选择区间
                txt_riqi1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"));
                txt_riqi2.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                txt_djzt.EditValue = "已生效";

                fun_显示列();
                fun_查询采购单列表();


                gc1.DataSource = dt_采购单列表;


                fun_查询采购单明细();
                this.gv1.BestFitColumns();
                // DateTime dt4 = System.DateTime.Now;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //根据采购单主表的信息查询采购单的明细
        private void 查询明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_采购单列表.Rows.Count <= 0)
                    throw new Exception("无数据可以查询！");
                DataRow r = (this.BindingContext[dt_采购单列表].Current as DataRowView).Row; //选中一行
                if (r["生效"].ToString() != "True")
                {
                    ERPpurchase.frm采购单明细 fm = new frm采购单明细(r);
                    CPublic.UIcontrol.AddNewPage(fm, "采购明细");
                }
                if (r["生效"].ToString() == "True")
                {
                    frm采购单明细视图 fm = new frm采购单明细视图(r["采购单号"].ToString());
                    CPublic.UIcontrol.AddNewPage(fm, "采购明细视图");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_查询采购单明细()
        {
            try
            {


                string strsql = "";
                //  strcgdh = txt_cgdh.EditValue.ToString();   //采购单号
                if (txt_riqi1.EditValue != null && txt_riqi2.EditValue != null)
                {
                    dttime1 = Convert.ToDateTime(txt_riqi1.EditValue);  //日期开始范围
                    dttime2 = Convert.ToDateTime(txt_riqi2.EditValue).AddDays(1).AddSeconds(-1);  //日期结束范围
                    if (dttime1 > dttime2)
                        throw new Exception("第一个时间不能大于第二个时间！");
                }
                strdjzt = txt_djzt.EditValue.ToString();  //单据状态
                string Strsql = "";
                //如果采购单号不为空:只以采购单号作为条件进行查询
                if (strcgdh != "")
                {
                    Strsql = Strsql + string.Format("采购记录采购单明细表.采购单号='{0}' and", strcgdh);
                }


                //如果采购单号是空的，才以其他的查询条件查询
                if (strcgdh == "")
                {

                    //如果单据状态不为空
                    if (strdjzt != "")
                    {
                        if (strdjzt == "已生效")
                        {
                            //如果两个时间文本都不为空
                            if (dttime1.ToString().Substring(0, 1) != "0" && dttime2.ToString().Substring(0, 1) != "0")
                            {
                                Strsql = Strsql + string.Format(" x.生效日期 >= '{0}' and x.生效日期<= '{1}' and", dttime1, dttime2);
                            }
                            Strsql = Strsql + string.Format(" x.生效=1 and x.作废=0 and");
                        }
                        if (strdjzt == "未生效")
                            Strsql = Strsql + string.Format(" x.生效=0 and x.作废=0 and");
                        if (strdjzt == "已作废")
                            Strsql = Strsql + string.Format(" x.作废=1 and");
                        if (strdjzt == "未作废")
                            Strsql = Strsql + string.Format(" x.作废=0 and");
                        if (strdjzt == "已完成")
                        {
                            //如果两个时间文本都不为空
                            if (dttime1.ToString().Substring(0, 1) != "0" && dttime2.ToString().Substring(0, 1) != "0")
                            {
                                Strsql = Strsql + string.Format(" x.生效日期 >= '{0}' and x.生效日期<= '{1}' and", dttime1, dttime2);
                            }
                            Strsql = Strsql + string.Format(" x.明细完成=1 and x.作废=0 and");

                        }

                        if (strdjzt == "未完成")
                        {  //如果两个时间文本都不为空
                            if (dttime1.ToString().Substring(0, 1) != "0" && dttime2.ToString().Substring(0, 1) != "0")
                            {
                                Strsql = Strsql + string.Format(" x.生效日期 >= '{0}' and x.生效日期<= '{1}' and", dttime1, dttime2);
                            }
                            Strsql = Strsql + string.Format(" x.明细完成=0 and x.作废=0 and");

                        }

                    }

                    if (CPublic.Var.LocalUserTeam == "开发部权限")
                    {
                        Strsql += "  采购单类型='开发采购' and";
                    }
                }
                ////视图权限
                ////DataTable dt_采购人员 = ERPorg.Corg.fun_hr("采购", CPublic.Var.LocalUserID);
                //if (view_权限.Rows.Count > 0)  //有这个操作员的情况下
                //{
                //    Strsql += " (";
                //            foreach (DataRow r in view_权限.Rows)
                //            {
                //                Strsql += " x.员工号 = '" + r["工号"].ToString().Trim() + "' or ";
                //            }
                //            Strsql = Strsql.Substring(0, Strsql.Length - 3)+")";

                //            Strsql += " and";

                //}
                //else if (CPublic.Var.localUserName != "管理员权限")
                //{
                //    throw new Exception("未配置此界面视图权限,请确认");
                //}
                if (Strsql != "")
                {
                    Strsql = " where " + Strsql.Substring(0, Strsql.Length - 3);
                }
                strsql = Strsql;

                strsql = string.Format(@"select x.GUID,x.采购明细号,x.物料编码,x.物料名称,x.供应商,x.规格型号,x.未税单价,x.单价,x.备注,x.采购数量,x.明细完成
                ,x.明细完成日期,x.到货日期,x.作废,x.生效日期,采购供应商备注 as 供应商备注,已送检数,x.作废人员ID,x.作废人员,x.备注9
                ,x.未税金额,x.金额,isnull(已入库数,0) 已入库数, case when x.明细完成日期 is null then  convert(bit,0) else convert(bit,1)  end as 入库完成,采购记录采购单主表.采购单类型,采购价,新数据
                from 采购记录采购单明细表 x 
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = x.物料编码 
                left join 采购记录采购单主表 on 采购记录采购单主表.采购单号 = x.采购单号
                left join ( select 采购明细号,sum(已入库数)已入库数 from 采购记录采购单检验主表 group by 采购明细号 )y on y.采购明细号=x.采购明细号 {0}", strsql);
                dt_采购单明细 = MasterSQL.Get_DataTable(strsql, CPublic.Var.strConn);
                gcP.DataSource = dt_采购单明细;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void fun_显示列()
        {
            if (!CPublic.Var.LocalUserTeam.Contains("管理员") && !CPublic.Var.LocalUserTeam.Contains("采购") && !CPublic.Var.LocalUserTeam.Contains("财务"))
            {
                gridColumn2.Visible = false;
                gridColumn8.Visible = false;
                gridColumn7.Visible = false;
                gridColumn18.Visible = false;
                gridColumn19.Visible = false;
                gridColumn21.Visible = false;
                gridColumn22.Visible = false;
                gv1.OptionsMenu.EnableColumnMenu = false;
                gvP.OptionsMenu.EnableColumnMenu = false;
                gvP.OptionsCustomization.AllowQuickHideColumns = false;
                gv1.OptionsCustomization.AllowQuickHideColumns = false;

                gv1.RowClick -= gv1_RowClick;
                gvP.RowCellClick -= gvP_RowCellClick;
                barLargeButtonItem10.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }

        //根据查询条件查询采购单列表
        private void fun_查询采购单列表()
        {
            try
            {
                // strcgdh = txt_cgdh.EditValue.ToString();   //采购单号
                if (txt_riqi1.EditValue != null && txt_riqi2.EditValue != null)
                {
                    dttime1 = Convert.ToDateTime(txt_riqi1.EditValue);  //日期开始范围
                    dttime2 = Convert.ToDateTime(txt_riqi2.EditValue).AddDays(1).AddSeconds(-1);  //日期结束范围
                    if (dttime1 > dttime2)
                        throw new Exception("第一个时间不能大于第二个时间！");
                }
                strdjzt = txt_djzt.EditValue.ToString();  //单据状态
                string Strsql = "";
                //如果采购单号不为空:只以采购单号作为条件进行查询
                if (strcgdh != "")
                {
                    Strsql = Strsql + string.Format("采购单号='{0}' and", strcgdh);
                }

                //如果采购单号是空的，才以其他的查询条件查询
                if (strcgdh == "")
                {
                    //如果两个时间文本都不为空
                    if (dttime1.ToString().Substring(0, 1) != "0" && dttime2.ToString().Substring(0, 1) != "0")
                    {
                        Strsql = Strsql + string.Format(" 录入日期 >= '{0}' and 录入日期<= '{1}' ", dttime1, dttime2);
                    }
                    //如果单据状态不为空
                    if (strdjzt != "")
                    {
                        if (strdjzt == "已生效")
                            Strsql = Strsql + string.Format("and 生效='True' ");
                        if (strdjzt == "未生效")
                            Strsql = Strsql + string.Format(" and 生效='False' ");
                        if (strdjzt == "已作废")
                            Strsql = Strsql + string.Format("and 作废='True' ");
                        if (strdjzt == "未作废")
                            Strsql = Strsql + string.Format("and 作废='False' ");
                        if (strdjzt == "已完成")
                            Strsql = Strsql + string.Format("and 完成='True' ");
                        if (strdjzt == "未完成")
                            Strsql = Strsql + string.Format("and 完成='False' ");
                    }
                }
                if (CPublic.Var.LocalUserTeam == "开发部权限")
                {
                    Strsql += "  and 采购单类型='开发采购' ";

                }
                //视图权限
                // DataTable dt_采购人员 = ERPorg.Corg.fun_hr("采购", CPublic.Var.LocalUserID);
                //if (view_权限.Rows.Count > 0)  //有这个操作员的情况下
                //{
                //    Strsql += " ( 员工号='' or ";

                //    foreach (DataRow r in view_权限.Rows)
                //    {
                //        Strsql += " 员工号 = '" + r["工号"].ToString().Trim() + "' or ";
                //    }
                //    Strsql = Strsql.Substring(0, Strsql.Length - 3);

                //    Strsql += ")";

                //}
                //else if (CPublic.Var.localUserName != "管理员权限")
                //{
                //    throw new Exception("未配置此界面视图权限,请确认");
                //}


                if (Strsql != "")
                {
                    Strsql = " where " + Strsql;
                }
                Strsql = string.Format("select * from 采购记录采购单主表 {0}", Strsql);
                dt_采购单列表 = MasterSQL.Get_DataTable(Strsql, CPublic.Var.strConn);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_查询采购单列表");
                throw ex;
            }
        }

        //明细表删除
        private void fun_明细表删除(DataRow r)
        {
            string sql = string.Format("select * from 采购记录采购单明细表 where 采购单号='{0}'", r["采购单号"].ToString());
            dt_明细 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            foreach (DataRow r1 in dt_明细.Rows)
            {
                r1.Delete();
            }
            MasterSQL.Save_DataTable(dt_明细, "采购记录采购单明细表", CPublic.Var.strConn);
        }

        //明细生效
        private void fun_明细表生效(DataRow r)
        {
            string sql = string.Format("select * from 采购记录采购单明细表 where 采购单号='{0}'", r["采购单号"].ToString());
            dt_明细 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            foreach (DataRow r1 in dt_明细.Rows)
            {
                r1["生效"] = "已生效";
                r1["生效日期"] = System.DateTime.Now;
            }
            MasterSQL.Save_DataTable(dt_明细, "采购记录采购单明细表", CPublic.Var.strConn);
        }

        //明细作废
        private void fun_明细表作废(DataRow r)
        {
            string sql = string.Format("select * from 采购记录采购单明细表 where 采购单号='{0}'", r["采购单号"].ToString());
            dt_明细 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            foreach (DataRow r1 in dt_明细.Rows)
            {
                r1["作废"] = "已作废";
                r1["作废日期"] = System.DateTime.Now;
            }
            MasterSQL.Save_DataTable(dt_明细, "采购记录采购单明细表", CPublic.Var.strConn);
        }

        #region   界面的一下操作
        //根据查询条件查询采购单的列表
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                fun_查询采购单列表(); //查询
                fun_查询采购单明细();
                //if (dt_采购单列表.Rows.Count <= 0)
                //{
                //    gc1.DataSource = dt_采购单列表;
                //    throw new Exception("查无数据！");
                //}
                gc1.DataSource = dt_采购单列表;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增采购单，跳出采购单的新增界面
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                frm采购单明细 fm = new frm采购单明细();
                CPublic.UIcontrol.AddNewPage(fm, "采购明细");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //采购单的删除操作，未生效的单价可以进行删除操作
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt_采购单列表.Rows.Count <= 0)
                    throw new Exception("没有采购单可以删除，请先查询需要删除的采购单！");
                DataRow r = (this.BindingContext[dt_采购单列表].Current as DataRowView).Row;
                if (r["生效"].ToString() == "1")   //如果单据是已生效的是不可以删除的
                    throw new Exception("该采购单是已生效的采购单，是不可以删除的！");
                if (MessageBox.Show(string.Format("你确实要删除采购单号为\"{0}\"的采购单吗？", r["采购单号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_明细表删除(r);
                    r.Delete();
                    MasterSQL.Save_DataTable(dt_采购单列表, "采购记录采购单主表", CPublic.Var.strConn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //采购单的生效操作
        private void barLargeButtonItem4_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt_采购单列表.Rows.Count <= 0)
                    throw new Exception("没有采购单可以进行生效操作，请先查询需要生效的采购单！");
                DataRow r = (this.BindingContext[dt_采购单列表].Current as DataRowView).Row;
                if (r["生效"].ToString() == "已生效")
                    throw new Exception("该采购单是已生效采购单，不能再次生效！");
                if (r["发送"].ToString() != "已发送")
                    throw new Exception("该采购单还不是已发送采购单，不能进行生效操作！\n请先查询进入明细界面，进行发送");
                if (r["作废"].ToString() == "已作废")
                    throw new Exception("该采购单是已经作废的采购单，是不能进行生效操作的！");
                if (r["停用"].ToString() == "已停用")
                    throw new Exception("该采购单是已经停用的采购单，是不能进行生效操作的！");
                r["生效"] = "已生效";
                r["生效日期"] = System.DateTime.Now;
                fun_明细表生效(r);
                MasterSQL.Save_DataTable(dt_采购单列表, "采购记录采购单主表", CPublic.Var.strConn);
                MessageBox.Show("采购单生效成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //采购单的作废操作
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt_采购单列表.Rows.Count <= 0)
                    throw new Exception("没有采购单可以进行作废操作，请先查询需要作废的采购单！");
                DataRow r = (this.BindingContext[dt_采购单列表].Current as DataRowView).Row;
                if (r["生效"].ToString() != "已生效")
                    throw new Exception("该采购单还没有生效，不能进行作废操作！");
                if (MessageBox.Show(string.Format("你确定要作废采购单\"{0}\"", r["采购单号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    r["作废"] = "已作废";
                    r["作废日期"] = CPublic.Var.getDatetime();
                    fun_明细表作废(r);
                    MasterSQL.Save_DataTable(dt_采购单列表, "采购记录采购单主表", CPublic.Var.strConn);
                    MessageBox.Show("采购单作废成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //清空单号的操作:清空单号之后，恢复日期和单单据状态的查询条件
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // txt_cgdh.EditValue = "";
                fun_查询采购单列表();
                gc1.DataSource = dt_采购单列表;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //关闭按钮
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        #endregion

        //双击查询采购明细
        private void gv1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc1, new Point(e.X, e.Y));

                    DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);


                    if (dr["采购单类型"].ToString() != "委外采购")
                    {
                        跳转ToolStripMenuItem.Visible = false;

                    }
                    else
                    {
                        跳转ToolStripMenuItem.Visible = true;
                        if (!Convert.ToBoolean(dr["生效"]) || Convert.ToBoolean(dr["作废"]))
                        {
                            跳转ToolStripMenuItem.Enabled = false;
                        }


                    }


                }
                if (e.Clicks == 2)
                {
                    if (dt_采购单列表.Rows.Count <= 0)
                        throw new Exception("无数据可以查询！");
                    DataRow r = (this.BindingContext[dt_采购单列表].Current as DataRowView).Row; //选中一行
                    if (r["生效"].ToString() != "True")
                    {
                        ERPpurchase.frm采购单明细 fm = new frm采购单明细(r);
                        CPublic.UIcontrol.AddNewPage(fm, "采购明细");
                    }
                    else
                    {
                        frm采购单明细视图 fm = new frm采购单明细视图(r["采购单号"].ToString());
                        CPublic.UIcontrol.AddNewPage(fm, "采购明细视图");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gc1_Click(object sender, EventArgs e)
        {

        }

        //private void gv1_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Control & e.KeyCode == Keys.C)
        //    {
        //        Clipboard.SetDataObject(gv1.GetFocusedRowCellValue(gv1.FocusedColumn));
        //        e.Handled = true;
        //    }
        //}

        //private void gvP_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Control & e.KeyCode == Keys.C)
        //    {
        //        Clipboard.SetDataObject(gvP.GetFocusedRowCellValue(gvP.FocusedColumn));
        //        e.Handled = true;
        //    }
        //}

        //private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        //{
        //    if (e.Info.IsRowIndicator && e.RowHandle > -1)
        //    {
        //        e.Info.DisplayText = (e.RowHandle + 1).ToString();
        //    }
        //}

        //private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        //{
        //    if (e.Info.IsRowIndicator && e.RowHandle > -1)
        //    {
        //        e.Info.DisplayText = (e.RowHandle + 1).ToString();
        //    }
        //}

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
                if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
                {
                    gc1.ExportToXlsx(saveFileDialog.FileName);
                }
                else
                {
                    gcP.ExportToXlsx(saveFileDialog.FileName);
                }
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //打印
        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            if (MessageBox.Show(string.Format("你确定要打印采购单{0}", dr["采购单号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                this.printDialog1.Document = this.printDocument1;

                DialogResult result = this.printDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {

                    str_打印机 = this.printDocument1.PrinterSettings.PrinterName;

                    Thread thDo;
                    thDo = new Thread(Dowork);
                    thDo.IsBackground = true;
                    thDo.Start();
                }
            }
        }
        public void Dowork()
        {

            try
            {
                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);

                ItemInspection.print_FMS.fun_采购单(dr["采购单号"].ToString(), str_打印机);
            }
            catch (Exception)
            {


            }
        }
        //
        string strcon_FS = CPublic.Var.geConn("FS");
        private void 预览审核文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);

                if (dr["文件GUID"] == null || dr["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以查看，请先上传文件");
                }
                //string type = dr["后缀"].ToString();

                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //string fileName = foldPath + CPublic.Var.getDatetime().ToString("yyyy-MM-dd") + "T" + CPublic.Var.getDatetime().ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + dr["文件"].ToString();

                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {
                }
                CFileTransmission.CFileClient.strCONN = strcon_FS;
                CFileTransmission.CFileClient.Receiver(dr["文件GUID"].ToString(), fileName);
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void 下载审核文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                //string s = string.Format("select * from 单据审核申请表 where 关联单号='{0}'", dr["采购单号"].ToString());
                //DataTable tt = new DataTable();
                //using (SqlDataAdapter a = new SqlDataAdapter(s, strcon))
                //{
                //    a.Fill(tt);
                //    if (tt.Rows.Count > 0)
                //    {
                //        if (tt.Rows[0]["文件地址"] == null || tt.Rows[0]["文件地址"].ToString() == "")
                //        {
                //            throw new Exception("该记录没有审核文件");
                //        }
                //        SaveFileDialog save = new SaveFileDialog();
                //        string sql = string.Format("select * from 采购供应商表 where   供应商ID='{0}'", dr["供应商ID"].ToString());
                //        using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                //        {
                //            DataTable t = new DataTable();
                //            da.Fill(t);
                //            if (t.Rows.Count > 0)
                //            {
                //                save.FileName = t.Rows[0]["供应商简码"].ToString();

                //            }
                //            save.FileName = save.FileName + dr["采购单号"].ToString() + ".pdf";
                //            // save.FileName = tt.Rows[0]["文件地址"].ToString() + "." + dr["后缀"].ToString();
                //            if (save.ShowDialog() == DialogResult.OK)
                //            {
                //                string strConn_FS = CPublic.Var.geConn("FS");
                //                CFileTransmission.CFileClient.strCONN = strConn_FS;


                //                CFileTransmission.CFileClient.Receiver(tt.Rows[0]["文件地址"].ToString(), save.FileName);
                //                MessageBox.Show("文件下载成功！");
                //            }
                //        }
                //    }
                //    else
                //    {
                //        throw new Exception("未找到对应的审核记录");
                //    }
                //}

                if (dr == null)
                {
                    throw new Exception("请重新选择采购订单！");
                }
                if (dr["文件GUID"] == null || dr["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以查看，请先上传文件");
                }
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "(*.jpg,*.png,*.jpeg,*.bmp,*.gif)|*.jgp;*.png;*.jpeg;*.bmp;*.gif|All files(*.*)|*.*";
                save.FileName = dr["文件"].ToString() + "." + dr["后缀"].ToString();
                //save.FileName = drm["文件名"].ToString();

                if (save.ShowDialog() == DialogResult.OK)
                {
                    CFileTransmission.CFileClient.strCONN = strcon_FS;
                    CFileTransmission.CFileClient.Receiver(dr["文件GUID"].ToString(), save.FileName);
                    MessageBox.Show("文件下载成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 撤销提交ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                string sql = string.Format("select  * from 采购记录采购单主表 where 审核=1 and 采购单号='{0}'", dr["采购单号"]);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (t.Rows.Count > 0)
                {
                    throw new Exception("该单据审核人已审核,不可撤销");
                }
                sql = string.Format(@"update 采购记录采购单主表 set 待审核=0 where 采购单号='{0}'  
                                     update  单据审核申请表 set 作废=1 where 关联单号 ='{0}' and 审核=0  and 作废=0 and 单据类型='采购单' ", dr["采购单号"]);
                CZMaster.MasterSQL.ExecuteSQL(sql, strcon);
                dr["待审核"] = false;
                dr.AcceptChanges();
                MessageBox.Show("已撤销");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void gvP_ColumnPositionChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cfgfilepath != "")
        //        {
        //            gvP.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}_2.xml", this.Name));
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //private void gv1_ColumnPositionChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cfgfilepath != "")
        //        {
        //            gv1.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}_1.xml", this.Name));
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        private void 委外明细补料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
            ui委外补料 ui = new ui委外补料(dr["采购明细号"].ToString(), dr["物料编码"].ToString());
            CPublic.UIcontrol.Showpage(ui, "委外补料");
        }
        private void gvP_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                if (dr == null) return;
                if (e != null && e.Button == MouseButtons.Right)
                {

                    if (dr["采购单类型"].ToString() != "委外采购")
                    {
                        委外明细补料ToolStripMenuItem.Visible = false;
                        委外明细退料ToolStripMenuItem.Visible = false;

                    }
                    contextMenuStrip2.Show(gcP, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 委外明细退料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
            ui委外补料 ui = new ui委外补料(dr["采购明细号"].ToString(), dr["物料编码"].ToString(), true);
            CPublic.UIcontrol.Showpage(ui, "委外退料");
        }

        //private void gvP_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        //{
        //    try
        //    {

        //        if (cfgfilepath != "")
        //        {
        //            gvP.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}_1.xml", this.Name));
        //        }

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr2 = gv1.GetDataRow(gv1.FocusedRowHandle);
            string str_采购单 = dr2["采购单号"].ToString();

            string sql = string.Format(@"select 采购记录采购单主表.*,供应商传真 from 采购记录采购单主表,采购供应商表 
                        where  采购记录采购单主表.供应商ID=采购供应商表.供应商ID and 采购单号='{0}'", str_采购单);
            System.Data.DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
            sql = string.Format(@"select 采购记录采购单明细表.*,原ERP物料编号 from 采购记录采购单明细表,基础数据物料信息表 
            where 采购记录采购单明细表.物料编码=基础数据物料信息表.物料编码  and 采购单号='{0}' order by 原ERP物料编号", str_采购单);
            System.Data.DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            //  frm采购报表打印 frm = new frm采购报表打印(dr["采购明细号"].ToString(), dr["物料编码"].ToString(), true);
            ERPreport.采购单 form = new ERPreport.采购单(dr, dt);
            form.ShowDialog();

        }/////新打印

        private void 历史采购价ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "ERPStock.dll"));//dr["dll全路径"] = "动态载入dll.dll"
                Type outerForm = outerAsm.GetType("ERPStock.fm空窗体", false);//动态载入dll.UI动态载入窗体
                Form fm = (Form)Activator.CreateInstance(outerForm);

                Type outerui = outerAsm.GetType("ERPStock.UI过往采购单价查询", false);//动态载入dll.UI动态载入窗体
                object[] r = new object[1];
                r[0] = dr["物料编码"].ToString();
                UserControl ui = Activator.CreateInstance(outerui, r) as UserControl;

                fm.Controls.Add(ui);
                ui.Dock = DockStyle.Fill;
                fm.Text = "历史采购价";
                fm.Size = new System.Drawing.Size(1200, 550);
                fm.StartPosition = FormStartPosition.CenterScreen;
                fm.ShowDialog();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void 跳转ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            StockCore.ui材料出库查询 ui = new StockCore.ui材料出库查询(dr["采购单号"].ToString());
            CPublic.UIcontrol.Showpage(ui, "材料出库申请查询");



        }
    }
}
