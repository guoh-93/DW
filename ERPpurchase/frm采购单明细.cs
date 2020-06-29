using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CZMaster;
using System.Data.SqlClient;
using System.Threading;
using System.Reflection;
using System.IO;

namespace ERPpurchase
{
    public partial class frm采购单明细 : UserControl
    {

        #region 变量
        //操作行drm
        DataRow drm;

        //部门表的dt
        DataTable dt_部门表;

        //员工表的dt
        DataTable dt_员工表;

        //采购单主表dt
        DataTable dt_采购单主表;

        //采购单主表副本
        DataTable dt_采购单;

        //采购单明细表dt
        DataTable dt_采购单明细;

        //采购供应商表
        DataTable dt_供应商表;

        //物料编码表
        DataTable dt_物料编码;

        //产品金额对照表
        DataTable dt_产品金额对照;



        //获取采购订单号
        string strCGDDH = "";

        //订单的未税金额
        decimal ddwsje = 0;

        //中间drmm
        DataRow drmm;

        DataTable t_cs;
        string supplierID = "";
        //经办人
        string strjbr = "";
        bool mxbool = false;
        //税率
        decimal shlv = 0;

        //供应商ID
        string strgysid = "";
        string 相关单号;
        //供应商
        string strgys = "";
        string cfgfilepath = "";
        //单据的总金额
        decimal djzje = 0;

        //单据的税金
        decimal djshuijin = 0;

        //数据库连接字符串
        string strcon = CPublic.Var.strConn;
        DataTable dt_stock;
        string str_打印机 = "";
        Boolean bl_istj = false;
        string str_单据状态 = "";
        bool bl_主计划池 = false;

        DataRow dr_ww; //如果有委外采购的记录 记录出入库申请主表 datarow
        #endregion

        public frm采购单明细()
        {
            InitializeComponent();

        }
        public frm采购单明细(int x)
        {
            InitializeComponent();

        }

        public frm采购单明细(DataRow r)
        {
            drmm = r;

            InitializeComponent();
            if (Convert.ToBoolean(drmm["待审核"]))
            {
                string sql = string.Format("select * from 单据审核申请表 where 关联单号 = '{0}'", drmm["采购单号"].ToString());
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                DataTable dt1111 = new DataTable();
                da.Fill(dt1111);
                if (dt1111.Rows[0]["待审核人ID"].ToString() == CPublic.Var.LocalUserID)
                {
                    bl_istj = false;
                    barLargeButtonItem9.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
                else
                {
                    bl_istj = true;
                }
            }
            if (Convert.ToBoolean(drmm["订单原件"]))
            {
                checkBox1.Checked = true;
                button2.Enabled = true;
                button5.Enabled = true;
            }


        }

        public frm采购单明细(object t, object c)
        {
            相关单号 = (string)t;
            mxbool = (Boolean)c;

            // drmm = (DataRow)r;
            InitializeComponent();
            // supplierID = suppID;
        }


        public frm采购单明细(DataTable t, string suppID)
        {
            t_cs = t;
            InitializeComponent();
            supplierID = suppID;
        }

        public frm采购单明细(DataTable t, string suppID,bool s_1 = true)
        {
            
            InitializeComponent();
            t_cs = t;
            supplierID = suppID;
            bl_主计划池 = s_1;
        }

        //下拉框的选择项
        private void fun_下拉框的数据()
        {
            //人事基础部门表
            try
            {
                string sql_ddfs = "select * from 基础数据基础属性表 where 属性类别='订单方式'";
                DataTable dt_ddfs = MasterSQL.Get_DataTable(sql_ddfs, CPublic.Var.strConn);
                foreach (DataRow r in dt_ddfs.Rows)
                {
                    txt_ddfs.Properties.Items.Add(r["属性值"].ToString());
                }
                string sql = "select  属性字段1 as 仓库号,属性值 as 仓库名称  from  基础数据基础属性表 where 属性类别='仓库类别' and 布尔字段1=1";
                dt_stock = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                repositoryItemGridLookUpEdit1.DataSource = dt_stock;
                repositoryItemGridLookUpEdit1.DisplayMember = "仓库号";
                repositoryItemGridLookUpEdit1.ValueMember = "仓库号";
                sql = "select 部门编号,部门名称,领导姓名 from 人事基础部门表";
                dt_部门表 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                foreach (DataRow r in dt_部门表.Rows)
                {
                    txt_bumenid.Properties.Items.Add(r["部门编号"].ToString());
                }
                searchLookUpEdit2.Properties.DataSource = dt_部门表;
                searchLookUpEdit2.Properties.DisplayMember = "部门编号";
                searchLookUpEdit2.Properties.ValueMember = "部门编号";


                ////人事基础人员表
                //try
                //{
                //    string sql1 = "select 员工号,姓名,手机,部门 from 人事基础员工表 where 在职状态='在职' ";
                //    dt_员工表 = MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                //    foreach (DataRow r in dt_员工表.Rows)
                //    {
                //        txt_cgygh.Properties.Items.Add(r["员工号"].ToString());
                //    }
                //    searchLookUpEdit3.Properties.DataSource = dt_员工表;
                //    searchLookUpEdit3.Properties.DisplayMember = "员工号";
                //    searchLookUpEdit3.Properties.ValueMember = "员工号";
                //}
                //catch (Exception ex)
                //{
                //    CZMaster.MasterLog.WriteLog(ex.Message + " fun_下拉框的数据人员表");
                //    throw new Exception(ex.Message);
                //}

                //采购供应商表

                string sql2 = "select 供应商ID,供应商名称,供应商负责人,供应商电话,交期,税率 from 采购供应商表 where 供应商状态 = '在用' order by 供应商ID";
                dt_供应商表 = MasterSQL.Get_DataTable(sql2, CPublic.Var.strConn);
                //foreach (DataRow r in dt_供应商表.Rows)
                //{
                //    txt_gysid.Properties.Items.Add(r["供应商ID"].ToString());
                //}
                searchLookUpEdit1.Properties.DataSource = dt_供应商表;
                searchLookUpEdit1.Properties.DisplayMember = "供应商ID";
                searchLookUpEdit1.Properties.ValueMember = "供应商ID";
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_下拉框的采购供应商表");
                throw new Exception(ex.Message);
            }
            //数量单位的下拉框
            //2018-9-19
            //string sql5 = string.Format("select * from 基础数据基础属性表 where 属性类别='数量单位' or 属性类别='税率'");
            string s = string.Format("select 属性值 from 基础数据基础属性表 where   属性类别='税率'");
            DataTable temp;
            temp = MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
            foreach (DataRow r in temp.Rows)
            {
                //if (r["属性类别"].ToString() == "数量单位")
                //{
                //    repositoryItemComboBox1.Items.Add(r["属性值"].ToString());
                //}
                //if (r["属性类别"].ToString() == "税率")
                //{
                txt_cgshlv.Properties.Items.Add(r["属性值"].ToString());
                //}
            }
            string zy = "and base.在研 = 0";
            if (CPublic.Var.LocalUserTeam == "开发部权限")
            {
                zy = "";

            }
            s = @"select base.物料编码,base.物料名称,base.供应商编号,base.规格型号,base.图纸编号,base.计量单位,base.计量单位编码,base.标准单价,
                        a.仓库号,a.仓库名称,base.图纸版本,isnull(a.库存总数,0)库存总数,isnull(a.有效总数,0)有效总数,isnull(a.在途量,0)在途量,采购供应商备注 as 供应商备注,新数据
                       , base.仓库号 as 默认仓库号,base.仓库名称 as 默认仓库 from 基础数据物料信息表 base left join 仓库物料数量表 a  on base.物料编码 = a.物料编码  and a.仓库号=base.仓库号
                        where (base.可购=1 or 委外=1)  and base.停用= 0 " + zy; //布尔字段1 位是否 纳入可用量
            dt_物料编码 = MasterSQL.Get_DataTable(s, strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料编码;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
        }
        private void frm采购单明细_Load(object sender, EventArgs e)
        {
            try
            {
                //2019-10-9 
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";

                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(panel1, this.Name, cfgfilepath);
                txt_采购单类型.Properties.Items.Clear();
                txt_采购单类型.Properties.Items.Add("普通采购");
                txt_采购单类型.Properties.Items.Add("委外采购");

                if (CPublic.Var.LocalUserTeam == "管理员权限")
                {

                    txt_采购单类型.Properties.Items.Add("开发采购");
                }
                else if (CPublic.Var.LocalUserTeam == "开发部权限")
                {
                    txt_采购单类型.Properties.Items.Clear();
                    txt_采购单类型.Properties.Items.Add("开发采购");
                    txt_bumenid.Text = CPublic.Var.localUser部门编号;
                    txt_采购单类型.Text = "开发采购";
                    txt_cgbumen.Text = CPublic.Var.localUser部门名称;

                }


                label22.Text = "";
                txt_cggs.Text = "南京东屋电气有限公司";
                txt_lrrqi.EditValue = CPublic.Var.getDatetime();
                repositoryItemSearchLookUpEdit1.PopupFormSize = new Size(1200, 300);
                txt_cgjhri.EditValue = CPublic.Var.getDatetime();
                textBox1.Text = CPublic.Var.LocalUserID;
                txt_cgjbr.Text = CPublic.Var.localUserName;
                txt_cgshlv.Text = "0";
                shlv = Convert.ToDecimal(txt_cgshlv.Text) / 100;
                txt_cgjbr.Text = CPublic.Var.localUserName;
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
                fun_下拉框的数据();
                if (bl_istj)
                {
                    string s = string.Format("select * from 采购记录采购单主表 where 采购单号='{0}'", drmm["采购单号"].ToString());
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (t.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(t.Rows[0]["审核"]))
                        {
                            str_单据状态 = "已审核";
                            label20.Visible = true;
                            label20.Text = str_单据状态;
                        }
                        else
                        {
                            str_单据状态 = "审核中";
                            label20.Visible = true;
                            label20.Text = str_单据状态;
                            barLargeButtonItem10.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        }
                    }
                    fun_编辑();
                }
                if (t_cs != null && t_cs.Columns.Count > 0)
                {
                    //采购单主表的dt
                    string sql = "select * from 采购记录采购单主表 where 1<>1";
                    dt_采购单主表 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                    drm = dt_采购单主表.NewRow();
                    drm["GUID"] = System.Guid.NewGuid().ToString();
                    dt_采购单主表.Rows.Add(drm);  //new出drm就把drm加到里面去
                    txt_采购单类型.Properties.Items.Add("计划类型");
                    txt_采购单类型.Text = "计划类型";
                    //txt_采购单类型.Enabled = false;
                    //明细表
                    string sql1 = @"select 采购记录采购单明细表.*,新数据 from 采购记录采购单明细表,基础数据物料信息表
                    where 采购记录采购单明细表.物料编码 =基础数据物料信息表.物料编码 and 1<>1";
                    dt_采购单明细 = MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                    foreach (DataRow dr in t_cs.Rows)
                    {
                        DataRow tr = dt_采购单明细.NewRow();
                        tr["物料编码"] = dr["物料编码"];
                        tr["物料名称"] = dr["物料名称"];
                        tr["规格型号"] = dr["规格型号"];
                        tr["仓库号"] = dr["仓库号"];
                        tr["仓库名称"] = dr["仓库名称"];
                        if (bl_主计划池)
                        {
                           
                            tr["采购数量"] = Convert.ToDecimal(dr["通知采购数量"])-Convert.ToDecimal(dr["已转采购数量"]);
                            tr["备注9"] = dr["计划通知单明细号"];
                            if (dr["预计来料日期"] == DBNull.Value|| Convert.ToDateTime(dr["预计来料日期"]).ToString() == "")
                            {
                                tr["到货日期"] = Convert.ToDateTime(dr["需求来料日期"]);
                            }
                            else
                            {
                                tr["到货日期"] = Convert.ToDateTime(dr["预计来料日期"]);
                            }
                           
                        }
                        else
                        {
                            tr["采购数量"] = dr["参考数量"];
                        }
                       
                        dt_采购单明细.Rows.Add(tr);
                    }
                    searchLookUpEdit1.EditValue = supplierID;

                    Recal();

                    gc2.DataSource = dt_采购单明细;
                }
                else if (drmm == null && mxbool == false)  //如果drm是null的话，表示的是直接进入的这个界面
                {
                    //采购单主表的dt
                    string sql = "select * from 采购记录采购单主表 where 1<>1";
                    dt_采购单主表 = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
                    drm = dt_采购单主表.NewRow();
                    drm["GUID"] = System.Guid.NewGuid().ToString();
                    dt_采购单主表.Rows.Add(drm);  //new出drm就把drm加到里面去
                    //明细表
                    string sql1 = @"select 采购记录采购单明细表.*,新数据 from 采购记录采购单明细表,基础数据物料信息表
                                    where 采购记录采购单明细表.物料编码 =基础数据物料信息表.物料编码 and 1<>1";
                    dt_采购单明细 = MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
                    gc2.DataSource = dt_采购单明细;
                    //税率默认16


                    //  txt_cgygh.Text = CPublic.Var.LocalUserID;


                    // searchLookUpEdit3.EditValue = CPublic.Var.LocalUserID;
                }
                else if (drmm != null && mxbool == false)   //如果drm不是null的话，表示的是通过查询进入的这个界面
                {
                    fun_采购单明细查询(drmm["GUID"].ToString());
                    if (drmm["待审核"].Equals(true))
                    {
                        barLargeButtonItem9.Enabled = false;

                    }
                }
                else
                {
                    fun_采购单明细查询(相关单号.ToString());
                    //if (drmm["待审核"].Equals(true))
                    //{
                    //    barLargeButtonItem9.Enabled = false;

                    //}
                }
                //dt_采购单明细.ColumnChanged += dt_采购单明细_ColumnChanged;

                //补开功能  不应开着的  要即时关掉   //4/14号 先开放权限  6/7
                //if (CPublic.Var.LocalUserID == "8404")
                //{
                //    txt_采购单类型.Properties.Items.Add("补开采购");
                //}

                //if (checkBox1.Checked == true)
                //{
                //    button4.Enabled = true;
                //    button2.Enabled = true;
                //    button5.Enabled = true;
                //}
                //else
                //{
                //    button4.Enabled = false;
                //    button2.Enabled = false;
                //    button5.Enabled = false;
                //}

                string sql1111 = string.Format("select * from 采购记录采购单主表 where GUID='{0}'", 相关单号);
                DataTable ttt = CZMaster.MasterSQL.Get_DataTable(sql1111, strcon);
                if (ttt.Rows.Count > 0)
                {
                    if (ttt.Rows[0]["文件GUID"].ToString() != "")
                    {
                        checkBox1.Checked = true;
                        button2.Enabled = true;
                        button4.Enabled = true;
                        button5.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //把采购单的明细查询出来
        private void fun_采购单明细查询(string str_GUID)
        {
            try
            {
                string sql = string.Format("select * from 采购记录采购单主表 where GUID='{0}'", str_GUID);
                dt_采购单主表 = MasterSQL.Get_DataTable(sql, strcon);
                drm = dt_采购单主表.NewRow();
                if (dt_采购单主表.Rows.Count > 0)
                    drm = dt_采购单主表.Rows[0];
                dataBindHelper1.DataFormDR(drm);  //把查询出来的值赋值到文本框中去

                shlv = Convert.ToDecimal(txt_cgshlv.Text);
                //查询采购单明细表，更加采购单号查询
                string sql1 = string.Format(@"select 采购记录采购单明细表.*,原ERP物料编号,新数据 from 采购记录采购单明细表,基础数据物料信息表
                            where 采购记录采购单明细表.物料编码=基础数据物料信息表.物料编码 and 采购单号='{0}' order by 采购POS", drm["采购单号"].ToString());
                dt_采购单明细 = MasterSQL.Get_DataTable(sql1, strcon);
                foreach (DataRow r in dt_采购单明细.Rows)
                {
                    DataRow[] dr1 = dt_物料编码.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString().Trim()));
                    if (dr1.Length <= 0)
                    {
                        string sql_num = string.Format("select * from 仓库物料数量表 where 物料编码='{0}'", r["物料编码"].ToString());
                        DataRow r_num = MasterSQL.Get_DataRow(sql_num, strcon);
                        if (r_num != null)
                        {
                            dt_物料编码.Rows.Add(r["物料编码"].ToString(), "", r["物料名称"], r["规格型号"], "", r_num["库存总数"], r_num["有效总数"], r_num["在途量"]);
                        }
                        else
                        {
                            dt_物料编码.Rows.Add(r["物料编码"], "", r["物料名称"]);
                        }
                    }
                }
                gc2.DataSource = dt_采购单明细;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_采购单明细查询");
            }
        }

        #region  check数据部分

        //检查主表的保存数据是否正确
        private void fun_check主表的数据()
        {

            //采购计划日期的检查
            if (txt_cgjhri.Text == "")
                txt_cgjhri.EditValue = DBNull.Value;
            //税率的检查
            if (txt_cgshlv.Text == "")
                throw new Exception("请填写税率，税率不能为空！");
            try
            {
                int i = Convert.ToInt32(txt_cgshlv.Text);
            }
            catch
            {
                throw new Exception("税率是数字，请检查！");
            }
            //供应商的电话检查
            if (searchLookUpEdit1.EditValue == null && searchLookUpEdit1.EditValue.ToString() == "")
                throw new Exception("请选择供应商编号，供应商编号不能为空！");
            //供应商的检查
            if (txt_cggys.Text == "")
                throw new Exception("供应商不能为空，请检查！");
            //经办的员工号
            //if (txt_cgygh.Text == "")
            //    throw new Exception("员工号不能为空，请选择！");
            //经办人
            //if (txt_cgjbr.Text == "")
            //    throw new Exception("经办人不能为空，请检查！");
            //采购公司
            if (txt_cggs.Text == "")
                throw new Exception("采购公司不能为空，请检查！");
            //录入日期
            if (txt_lrrqi.Text == "")
                throw new Exception("录入日期不能为空，请检查！");

            if (txt_采购单类型.Text == "")
            {
                throw new Exception("采购单类型不可为空！");
            }
            //{
            //    if(drm["文件GUID"].ToString() == "")
            //    {
            //        throw new Exception("订单原件必须上传");
            //    }

            ////}
            //if (checkBox1.Checked)
            //{
            //    if(drm["文件GUID"].ToString() == "")
            //    {
            //        throw new Exception("订单原件未上传,若不上传原件，请去除勾选");
            //    }
            //}
            if (drm["文件GUID"].ToString() != "")
            {
                drm["订单原件"] = true;
            }
            else
            {
                drm["订单原件"] = false;
            }


            DataTable dt_基础 = new DataTable();
            DateTime tp = CPublic.Var.getDatetime();

            tp = new DateTime(tp.Year, tp.Month, tp.Day);
            int pos = 1;
            foreach (DataRow r in dt_采购单明细.Rows)
            {
                
                if (r.RowState == DataRowState.Deleted) continue;
                if (r["备注9"].ToString().Trim() == "") r["备注9"] = "";
                DataRow[] dr_cfx = dt_采购单明细.Select(string.Format("物料编码='{0}' and 备注9 = '{1}' ", r["物料编码"].ToString(), r["备注9"].ToString().Trim()));
                if (dr_cfx.Length > 1)
                    throw new Exception(string.Format("物料编码\"{0}\"有重复，只允许有一个！", r["物料编码"].ToString()));
                if (r["备注9"].ToString() == "")
                {
                    DataRow[] dr_1 = dt_采购单明细.Select(string.Format("物料编码='{0}' and 备注9 <>'' ", r["物料编码"].ToString()));
                    if (dr_1.Length>0)
                    {
                        if (r["备注"].ToString().Trim() == "") throw new Exception("物料编码："+r["物料编码"]+"新增备注必填");
                    }
                }
                //采购单号
                string sql = string.Format("select * from 基础数据物料信息表  where 物料编码 = '{0}'", r["物料编码"].ToString());
                dt_基础 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt_基础.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(dt_基础.Rows[0]["委外"]) == true)
                    {
                        if (Convert.ToBoolean(dt_基础.Rows[0]["可购"]) && txt_采购单类型.Text != "委外采购")
                        {
                            if (MessageBox.Show(string.Format("确认该物料为{0}么？", txt_采购单类型.Text), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {

                            }
                            else
                            {
                                throw new Exception("已取消");
                            }
                        }
                        if (!Convert.ToBoolean(dt_基础.Rows[0]["可购"]) && txt_采购单类型.Text != "委外采购")
                        {
                            throw new Exception(string.Format("物料编码\"{0}\"是委外物料并且不可购，请重新选择！", r["物料编码"].ToString()));
                        }
                    }
                }
                if (r["仓库号"].ToString() == "")
                {
                    throw new Exception(string.Format("物料编码\"{0}\"仓库号为空,请选择仓库", r["物料编码"].ToString()));
                }

                //赋值:明细表与主表相同的，只要直接赋值
                r["供应商ID"] = searchLookUpEdit1.EditValue.ToString();
                r["供应商"] = txt_cggys.Text;
                r["供应商负责人"] = txt_gysfzr.Text;
                r["供应商电话"] = txt_gysdh.Text;
                //r["员工号"] = txt_cgygh.Text;
                r["采购人"] = txt_cgjbr.Text;
                r["采购部门ID"] = txt_bumenid.Text;
                r["采购部门"] = txt_cgbumen.Text;
                r["税率"] = txt_cgshlv.Text;

                //物料编码的检查：物料编码是由订单号+采购顺序的POS
                if (r["物料编码"].ToString() == "")
                    throw new Exception("请选择物料编码，物料编码不能为空！");
                //采购数量的检查
                if (r["采购数量"].ToString() == "")
                    throw new Exception("请填写采购数量，采购数量不能为空！");
                try
                {
                    double d = Convert.ToDouble(r["采购数量"]);
                }
                catch
                {
                    throw new Exception("采购数量是数字，请检查一下，重新填写！");
                }

                try
                {
                    double d = Convert.ToDouble(r["单价"]);
                    if (d <= 0)
                    {
                        throw new Exception("单价不能小于0！");
                    }

                }
                catch
                {
                    throw new Exception("请检查单价");
                }
                r["采购价"] = r["单价"];
                //到货日期的检查
                if (r["到货日期"].ToString() == "")
                    throw new Exception("请选择到货日期，到货日期不能为空！");

                if (Convert.ToDateTime(r["到货日期"]) < tp)
                {
                    throw new Exception("到货日期小于今天");

                }
                r["预计到货日期"] = r["到货日期"];

                r["操作员ID"] = CPublic.Var.LocalUserID;
                r["操作员"] = CPublic.Var.localUserName;
            }




            if (txt_caigousn.Text == "")
            {
                DateTime t = CPublic.Var.getDatetime();
                txt_caigousn.Text = string.Format("PO{0}{1:00}{2:00}{3:0000}", t.Year, t.Month,
                    t.Day, CPublic.CNo.fun_得到最大流水号("PO", t.Year, t.Month));
            }

            strCGDDH = txt_caigousn.Text.ToString();
            foreach (DataRow trr in dt_采购单明细.Rows)
            {
                if (trr.RowState == DataRowState.Deleted) continue;
                trr["采购单号"] = strCGDDH;

                trr["采购明细号"] = strCGDDH + "-" + pos.ToString("00");

                trr["采购POS"] = pos++;  //采购POS
            }


            DataView dv = new DataView(dt_采购单主表);
            dv.RowStateFilter = DataViewRowState.Added;
            if (dv.Count > 0)
            {
                if (txt_cgshhje.Text == "")
                {
                    txt_cgshhje.Text = "0";
                    txt_cgshuijin.Text = "0";
                }
                if (djzje != Convert.ToDecimal(txt_cgshhje.Text))
                {
                    txt_cgshhje.Text = djzje.ToString("#0.####");
                }
            }




        }

        //检查采购单明细
        private void fun_check采购单明细()
        {
            DateTime t = CPublic.Var.getDatetime();
            t = new DateTime(t.Year, t.Month, t.Day);
            int pos = 1;
            foreach (DataRow r in dt_采购单明细.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                DataRow[] dr_cfx = dt_采购单明细.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
                if (dr_cfx.Length > 1)
                    throw new Exception(string.Format("物料编码\"{0}\"有重复，只允许有一个！", r["物料编码"].ToString()));
                //采购单号
                r["采购单号"] = strCGDDH;
                if (pos.ToString().Length == 1)  //采购明细号：采购单号-采购POS
                {
                    r["采购明细号"] = strCGDDH + "-0" + pos.ToString();
                }
                else
                {
                    r["采购明细号"] = strCGDDH + "-" + pos.ToString();
                }
                r["采购POS"] = pos++;  //采购POS
                //赋值:明细表与主表相同的，只要直接赋值
                r["供应商ID"] = searchLookUpEdit1.EditValue.ToString();
                r["供应商"] = txt_cggys.Text;
                r["供应商负责人"] = txt_gysfzr.Text;
                r["供应商电话"] = txt_gysdh.Text;
                // r["员工号"] = txt_cgygh.Text;
                r["采购人"] = txt_cgjbr.Text;
                r["采购部门ID"] = txt_bumenid.Text;
                r["采购部门"] = txt_cgbumen.Text;
                r["税率"] = txt_cgshlv.Text;

                //物料编码的检查：物料编码是由订单号+采购顺序的POS
                if (r["物料编码"].ToString() == "")
                    throw new Exception("请选择物料编码，物料编码不能为空！");
                //采购数量的检查
                if (r["采购数量"].ToString() == "")
                    throw new Exception("请填写采购数量，采购数量不能为空！");
                try
                {
                    double d = Convert.ToDouble(r["采购数量"]);
                }
                catch
                {
                    throw new Exception("采购数量是数字，请检查一下，重新填写！");
                }

                try
                {
                    double d = Convert.ToDouble(r["单价"]);
                    if (d <= 0)
                    {
                        throw new Exception("单价不能小于0！");
                    }

                }
                catch
                {
                    throw new Exception("请检查单价");
                }
                r["采购价"] = r["单价"];
                //到货日期的检查
                if (r["到货日期"].ToString() == "")
                    throw new Exception("请选择到货日期，到货日期不能为空！");
                if (Convert.ToDateTime(r["到货日期"]) < t)
                {
                    throw new Exception("到货日期小于今天");

                }

                r["操作员ID"] = CPublic.Var.LocalUserID;
                r["操作员"] = CPublic.Var.localUserName;
            }





        }

        #endregion

        #region  选择物料编码 带出物料名称和单价

        //采购明细，选择物料编码，物料名称和单价同时带出来
        private void repositoryItemSearchLookUpEdit1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //DevExpress.XtraEditors.SearchLookUpEdit su = gv2.ActiveEditor as DevExpress.XtraEditors.SearchLookUpEdit;

                //DataRow r = (gv2.ActiveEditor as DevExpress.XtraEditors.SearchLookUpEdit).Properties.View.GetFocusedDataRow();

                //if (r != null)
                //{
                //    foreach (DataRow r2 in dt_产品金额对照.Rows)  //带出采购物料的单价 先去产品金额对照表拿单价
                //    {
                //        if (r2["产品编号"].ToString().Trim() == r["物料编码"].ToString().Trim() && Convert.ToInt32(r2["采购价格"])!=0)
                //        {


                //            (this.BindingContext[dt_采购单明细].Current as DataRowView).Row["单价"] = r2["采购价格"];
                //        }
                //    }

                //    foreach (DataRow r1 in dt_物料编码.Rows)  //带出物料名称
                //    {
                //        if (r1["物料编码"].ToString().Trim() == r["物料编码"].ToString().Trim())
                //        {
                //            (this.BindingContext[dt_采购单明细].Current as DataRowView).Row["物料名称"] = r1["物料名称"];
                //            (this.BindingContext[dt_采购单明细].Current as DataRowView).Row["数量单位"] = r1["计量单位"];
                //            if ((this.BindingContext[dt_采购单明细].Current as DataRowView).Row["单价"].ToString() == "")
                //            {
                //                (this.BindingContext[dt_采购单明细].Current as DataRowView).Row["单价"] = r1["标准单价"];
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region  供应商信息的选择

        //供应商变化，带出供应商的负责人，供应商的电话
        //private void txt_gysid_SelectedValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        foreach (DataRow r in dt_供应商表.Rows)
        //        {
        //            if (txt_gysid.Text == r["供应商ID"].ToString())
        //            {
        //                txt_cggys.Text = r["供应商名称"].ToString();  //供应商名称
        //                txt_gysfzr.Text = r["供应商负责人"].ToString();
        //                txt_gysdh.Text = r["供应商电话"].ToString();
        //                txt_cgshlv.EditValue = r["税率"].ToString();
        //                if (txt_gysid.Text != "")
        //                {
        //                    label22.Text = string.Format("当前供应商的交期为{0}", r["交期"].ToString());
        //                }
        //                else
        //                {
        //                    label22.Text = "";
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //文本变化事件
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue != null)
                {


                    DataRow[] r = dt_供应商表.Select(string.Format("供应商ID='{0}'", searchLookUpEdit1.EditValue.ToString()));
                    if (r.Length > 0)
                    {

                        //txt_gysid.Text = r[0]["供应商ID"].ToString();
                        txt_cggys.Text = r[0]["供应商名称"].ToString();  //供应商名称
                        txt_gysfzr.Text = r[0]["供应商负责人"].ToString();
                        txt_gysdh.Text = r[0]["供应商电话"].ToString();
                        txt_cgshlv.Text = r[0]["税率"].ToString();
                        label22.Text = string.Format("当前供应商的交期为{0}", r[0]["交期"].ToString());

                        if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "" && dt_采购单明细 != null)
                        {
                            foreach (DataRow dr in dt_采购单明细.Rows)
                            {
                                if (dr.RowState == DataRowState.Deleted) continue;
                                if (dr["物料编码"] == null || dr["物料编码"].ToString() == "") continue;

                                dr["采购价"] = dr["单价"] = ERPorg.Corg.ReacqPP(searchLookUpEdit1.EditValue.ToString(), dr["物料编码"].ToString());

                                fun_金额的变化();

                            }


                        }
                    }
                    else
                    {
                        label22.Text = "";
                    }
                }
                else
                {
                    txt_cggys.Text = "";

                    txt_gysfzr.Text = "";
                    txt_gysdh.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region   经办人信息选择

        ////员工编号，改变的话，姓名也随之变化
        //private void txt_cgygh_SelectedValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        foreach (DataRow r in dt_员工表.Rows)
        //        {
        //            if (txt_cgygh.Text == r["员工号"].ToString())
        //            {
        //                txt_cgjbr.Text = r["姓名"].ToString();  //经办人姓名
        //                txt_jbrtel.Text = r["手机"].ToString(); //经办人电话
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}


        ////经办人信息的变化
        //private void searchLookUpEdit3_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        //{
        //    try
        //    {
        //        foreach (DataRow r in dt_员工表.Rows)
        //        {
        //            if (searchLookUpEdit3.EditValue != null)
        //            {
        //                if (searchLookUpEdit3.EditValue.ToString() == r["员工号"].ToString())
        //                {
        //                    txt_cgygh.Text = r["员工号"].ToString(); //员工号
        //                    txt_cgjbr.Text = r["姓名"].ToString();   //姓名
        //                    txt_jbrtel.Text = r["手机"].ToString();   //手机号
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}

        #endregion

        #region 采购部门信息选择

        //部门的信息选择
        private void txt_bumenid_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {

                DataRow[] tr = dt_部门表.Select(string.Format("部门编号='{0}'", txt_bumenid.Text));
                if (tr.Length > 0) txt_cgbumen.Text = tr[0]["部门名称"].ToString();
                //foreach (DataRow r in dt_部门表.Rows)
                //{
                //    if (txt_bumenid.Text == r["部门编号"].ToString())
                //    {
                //        txt_cgbumen.Text = r["部门名称"].ToString();
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void searchLookUpEdit2_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                foreach (DataRow r in dt_部门表.Rows)
                {
                    if (searchLookUpEdit2.EditValue != null)
                    {
                        if (searchLookUpEdit2.EditValue.ToString() == r["部门编号"].ToString())
                        {
                            txt_bumenid.Text = r["部门编号"].ToString();
                            txt_cgbumen.Text = r["部门名称"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region   当改变明细的采购数量和和单价的时候，单据的金额随之改变

        private void fun_金额的变化()
        {
            Decimal s = 0;
            shlv = Convert.ToDecimal(txt_cgshlv.Text) / (decimal)100;


            foreach (DataRow r in dt_采购单明细.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;



                if (r["采购数量"].ToString() != "" && r["单价"].ToString() != "" )
                {
                    // r["金额"] = (Convert.ToDecimal(r["采购数量"]) * Convert.ToDecimal(r["单价"])).ToString("0.000000");  //金额
                  
                    if (Convert.ToDecimal(r["采购数量"])!=0 && r["金额"].ToString() != "" &&  Convert.ToDecimal(r["单价"]) == 0)
                    {
                        r["单价"] = (Convert.ToDecimal(r["金额"]) / Convert.ToDecimal(r["采购数量"]));
                    }
                    else
                    {
                        r["金额"] = (Convert.ToDecimal(r["采购数量"]) * Convert.ToDecimal(r["单价"]));  //金额
                    }


                    if (shlv == 0)
                    {
                        r["税金"] = 0;   //计算税金
                        r["未税单价"] = r["单价"];
                        r["未税金额"] = r["金额"];
                    }
                    else
                    {
                        //r["税金"] = ((Convert.ToDecimal(r["金额"]) / (1 + shlv)) * shlv).ToString("0.000000");   //计算税金
                        //r["未税单价"] = (Convert.ToDecimal(r["单价"]) / (1 + shlv)).ToString("0.000000");
                        //r["未税金额"] = (Convert.ToDecimal(r["金额"]) / (1 + shlv)).ToString("0.000000");

                        r["税金"] = ((Convert.ToDecimal(r["金额"]) / (1 + shlv)) * shlv);   //计算税金
                        r["未税单价"] = (Convert.ToDecimal(r["单价"]) / (1 + shlv));
                        r["未税金额"] = (Convert.ToDecimal(r["金额"]) / (1 + shlv));
                    }

                    s += Convert.ToDecimal(r["金额"]);
                    r["未完成数量"] = r["采购数量"];
                }

                else if ((r["采购数量"] == DBNull.Value || r["采购数量"].ToString() == "") && r["单价"].ToString() != "")
                {
                    if (shlv == 0)
                    {
                        r["税金"] = 0;   //计算税金
                        r["未税单价"] = r["单价"];
                        r["未税金额"] = r["金额"];
                    }
                    else
                    {

                        //r["税金"] = ((Convert.ToDecimal(r["金额"]) / (1 + shlv)) * shlv);   //计算税金
                        r["未税单价"] = (Convert.ToDecimal(r["单价"]) / (1 + shlv));

                    }

                }

            }
            txt_cgshhje.Text = s.ToString("#0.####");
            ddwsje = s / (1 + shlv);
            if (shlv == 0)
            {
                txt_cgshuijin.Text = "0";  //计算税金
            }
            else
            {
                txt_cgshuijin.Text = ((s / (1 + shlv)) * shlv).ToString("#0.##");  //计算税金

            }
            djzje = s;
        }

        //填写采购数量引起明细中的 金额  税金  未税金额 变化   主表中的未税金额  总金额 税金的变化
        void dt_采购单明细_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            //if (e.Column.ColumnName == "物料编码")
            //{
            //    DataRow [] dr=dt_物料编码.Select(string.Format("物料编码='{0}'",e.Row["物料编码"].ToString()));
            //    if (dr.Length > 0)
            //    {
            //        e.Row["物料名称"] = dr[0]["物料名称"];
            //        e.Row["数量单位"] = dr[0]["计量单位"];
            //        e.Row["规格型号"] = dr[0]["规格"];
            //        e.Row["图纸编号"] = dr[0]["图纸编号"];

            //        e.Row["图纸版本"]=dr[0]["图纸版本"];


            //        e.Row["仓库ID"] = dr[0]["仓库号"];
            //        e.Row["仓库名称"] = dr[0]["仓库名称"];


            //        DataRow[] dr1 = dt_产品金额对照.Select(string.Format("产品编号='{0}'", e.Row["物料编码"].ToString()));
            //        if (dr1.Length > 0)
            //        {
            //            if (Convert.ToInt32(dr1[0]["采购价格"]) != 0)
            //            {
            //                e.Row["单价"] = dr1[0]["采购价格"];
            //            }
            //            else
            //            {
            //                e.Row["单价"] = dr[0]["标准单价"];
            //            }

            //        }
            //        else
            //        {
            //            e.Row["单价"] = dr[0]["标准单价"];
            //        }
            //        if (txt_gysid.EditValue == null || txt_gysid.EditValue.ToString() == "")
            //        {
            //            txt_gysid.EditValue = dr[0]["供应商编号"].ToString().Trim();
            //        }
            //    }

            //}



            //if (e.Column.ColumnName == "采购数量" || e.Column.ColumnName == "单价")
            //{
            //    fun_金额的变化();
            //}
        }

        #endregion

        #region  相关调用的方法

        //新增采购单
        private void fun_新增采购主表数据()
        {
            try
            {
                drm = dt_采购单主表.NewRow();  //新建一行
                dataBindHelper1.DataFormDR(drm); //把新建的行赋值到文本框
                drm["GUID"] = System.Guid.NewGuid().ToString();
                dt_采购单主表.Rows.Add(drm);
                //txt_cgshlv.Text = "17";  //税率要有默认的17
                //txt_cgygh.Text = CPublic.Var.LocalUserID;  //默认当前操作员的ID
                txt_cgjbr.Text = CPublic.Var.localUserName;  //默认当前操作员 
                txt_cggs.Text = "南京东屋电气有限公司";
                searchLookUpEdit1.EditValue = "";
                txt_lrrqi.EditValue = CPublic.Var.getDatetime();
                txt_cgjhri.EditValue = CPublic.Var.getDatetime();
                //如果采购单明细不为Null
                if (dt_采购单明细 != null)
                {
                    dt_采购单明细.Clear();
                    gc2.DataSource = dt_采购单明细;
                }

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_新增采购主表数据");
                throw new Exception(ex.Message);
            }
        }

        //新增明细的方法
        private void fun_新增采购单明细()
        {
            try
            {
                DataRow r = dt_采购单明细.NewRow();
                dt_采购单明细.Rows.Add(r);
                gv2.FocusedRowHandle = gv2.LocateByDisplayText(0, gridColumn28, "");
                DateTime t = CPublic.Var.getDatetime().Date.AddDays(7);
                if (dt_采购单明细.Rows.Count > 1)
                {
                    r["到货日期"] = dt_采购单明细.Rows[0]["到货日期"]; //若第一条有时间 跟着第一条
                }
                else
                {
                    r["到货日期"] = t;//默认七天后

                }
                r["GUID"] = System.Guid.NewGuid().ToString();

                //dt_采购单明细.ColumnChanged += dt_采购单明细_ColumnChanged;

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_新增采购单明细");
            }
        }
        /// <summary>
        /// 为开发票 补做的 采购单 不入库
        /// </summary>
        private void fun_save_辅助表()
        {
            try
            {
                //主表数据的检查无误完成之后:该单号是否具有
                string sql = string.Format("select * from 采购记录采购单辅助主表 where 采购单号='{0}'", txt_caigousn.Text);
                dt_采购单 = MasterSQL.Get_DataTable(sql, strcon);
                if (dt_采购单.Rows.Count <= 0)
                {
                    dataBindHelper1.DataToDR(drm);
                    drm["采购单号"] = strCGDDH;   //采购单号
                    drm["未税金额"] = ddwsje;
                    //采购单的创建日期
                    drm["创建日期"] = CPublic.Var.getDatetime();
                    drm["修改日期"] = CPublic.Var.getDatetime();
                    drm["操作员ID"] = CPublic.Var.LocalUserID;
                    drm["操作员"] = CPublic.Var.localUserName;
                }
                else  //如果是修改的情况之下
                {
                    dataBindHelper1.DataToDR(drm);
                    drm["未税金额"] = ddwsje;
                    drm["修改日期"] = CPublic.Var.getDatetime();
                    drm["操作员ID"] = CPublic.Var.LocalUserID;
                    drm["操作员"] = CPublic.Var.localUserName;
                }

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("辅助pur"); //事务的名称
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购单辅助主表 where 1<>1", conn, ts);
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购明细辅助表 where 1<>1", conn, ts);

                try
                {
                    SqlDataAdapter da;
                    da = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da);
                    da.Update(dt_采购单主表);

                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_采购单明细);
                    ts.Commit();
                }
                catch
                {
                    ts.Rollback();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_save数据");
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        ///     为开发票 补做的 采购单 不入库
        /// </summary>
        /// <param name="strGUID"></param>
        private void fun_采购明细辅助查询(string strGUID)
        {
            try
            {
                string sql = string.Format("select * from 采购记录采购单辅助主表 where GUID='{0}'", strGUID);
                dt_采购单主表 = MasterSQL.Get_DataTable(sql, strcon);
                drm = dt_采购单主表.NewRow();
                if (dt_采购单主表.Rows.Count > 0)
                    drm = dt_采购单主表.Rows[0];
                dataBindHelper1.DataFormDR(drm);  //把查询出来的值赋值到文本框中去

                shlv = Convert.ToDecimal(txt_cgshlv.Text);
                //查询采购单明细表，更加采购单号查询
                string sql1 = string.Format(@"select 采购记录采购明细辅助表.*,原ERP物料编号,新数据 from 采购记录采购明细辅助表,基础数据物料信息表
                where 采购记录采购明细辅助表.物料编码=基础数据物料信息表.物料编码 and 采购单号='{0}' order by 采购POS", drm["采购单号"].ToString());
                dt_采购单明细 = MasterSQL.Get_DataTable(sql1, strcon);
                foreach (DataRow r in dt_采购单明细.Rows)
                {
                    DataRow[] dr1 = dt_物料编码.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString().Trim()));
                    if (dr1.Length <= 0)
                    {
                        string sql_num = string.Format("select * from 仓库物料数量表 where 物料编码='{0}'", r["物料编码"].ToString());
                        DataRow r_num = MasterSQL.Get_DataRow(sql_num, strcon);
                        if (r_num != null)
                        {
                            dt_物料编码.Rows.Add(r["物料编码"].ToString(), "", r["物料名称"], r["规格型号"], "", r_num["库存总数"], r_num["有效总数"], r_num["在途量"]);
                        }
                        else
                        {
                            dt_物料编码.Rows.Add(r["物料编码"], "", r["物料名称"]);
                        }
                    }
                }
                gc2.DataSource = dt_采购单明细;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_采购单明细查询");
            }
        }
        //数据的保存
        private void fun_save数据()
        {
            try
            {
                DateTime time = CPublic.Var.getDatetime();
                //主表数据的检查无误完成之后:该单号是否具有
                string sql = string.Format("select * from 采购记录采购单主表 where 采购单号='{0}'", txt_caigousn.Text);
                dt_采购单 = MasterSQL.Get_DataTable(sql, strcon);
                if (dt_采购单.Rows.Count <= 0)
                {
                    dataBindHelper1.DataToDR(drm);
                    drm["采购单号"] = strCGDDH;   //采购单号
                    drm["未税金额"] = ddwsje;
                    //采购单的创建日期
                    drm["创建日期"] = time;
                    drm["录入日期"] = time;
                    drm["操作员ID"] = CPublic.Var.LocalUserID;
                    drm["操作员"] = CPublic.Var.localUserName;
                    drm["员工号"] = CPublic.Var.LocalUserID;
                    drm["部门编号"] = CPublic.Var.localUser部门编号;
                    drm["采购部门"] = CPublic.Var.localUser部门名称;
                }
                else  //如果是修改的情况之下
                {
                    dataBindHelper1.DataToDR(drm);
                    drm["未税金额"] = ddwsje;
                    drm["修改日期"] = time;
                    drm["操作员ID"] = CPublic.Var.LocalUserID;
                    drm["操作员"] = CPublic.Var.localUserName;
                }
                if (txt_采购单类型.Text.Trim() == "")
                {
                    drm["采购单类型"] = "普通采购";
                }
                //生成审核申请单
                //DataTable dt = ERPorg.Corg.fun_PA("生效","采购单", txt_caigousn.Text, txt_cggys.Text);


                








                DataTable dt_采购计划明细 = new DataTable();

                if (bl_主计划池)
                {
                    string sql_计划需求 = "select * from 主计划计划通知单明细 where 生效 = 1  order by 需求来料日期";
                    dt_采购计划明细 = CZMaster.MasterSQL.Get_DataTable(sql_计划需求, strcon);
                    decimal ii = 0;
                    foreach (DataRow dr_采购明细 in dt_采购单明细.Rows)
                    {
                        if (dr_采购明细.RowState == DataRowState.Deleted) continue;
                        ii = Convert.ToDecimal(dr_采购明细["采购数量"]);
                        if (dr_采购明细["备注9"].ToString() !="")
                        {
                            DataRow[] dr_计划明细 = dt_采购计划明细.Select(string.Format("计划通知单明细号 = '{0}'", dr_采购明细["备注9"]));
                            if (dr_计划明细.Length > 0)
                            {
                                if (Convert.ToDecimal(dr_计划明细[0]["通知采购数量"])- Convert.ToDecimal(dr_计划明细[0]["已转采购数量"]) < Convert.ToDecimal(dr_采购明细["采购数量"]))
                                {
                                    throw new Exception("采购数量超过通知数量，请确认");
                                }
                            }
                        }                                                                  
                    }
                }

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, ts);
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);
                SqlCommand cmd2 = new SqlCommand("select * from 主计划计划通知单明细 where 1<>1", conn, ts);

                try
                {
                    SqlDataAdapter da;
                    da = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da);
                    da.Update(dt_采购单主表);

                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_采购单明细);
                    if (bl_主计划池)
                    {
                        da = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(da);
                        da.Update(dt_采购计划明细);
                    }

                    //cmd = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
                    //da = new SqlDataAdapter(cmd);
                    //new SqlCommandBuilder(da);
                    //da.Update(dt);


                    ts.Commit();
                    MessageBox.Show("保存成功！");
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_save数据");
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region  界面的一些操作

        //新增主表的功能
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataView dv = new DataView(dt_采购单主表);
                dv.RowStateFilter = DataViewRowState.Added;
                if (dv.Count > 0)      //如果这个drm是新增的
                {
                    if (MessageBox.Show("当前采购单是新增状态，还没有保存，如果不需保存，请点击确定！", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        drm.Delete();
                        fun_新增采购主表数据();
                    }
                }
                else
                {
                    fun_新增采购主表数据();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //采购单明细记录的新增
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {

                gv2.CloseEditor();
                this.BindingContext[dt_采购单明细].EndCurrentEdit();
                fun_新增采购单明细();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //保存数据:先保存子表数据，金额才能相加。
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv2.CloseEditor();
                this.BindingContext[dt_采购单明细].EndCurrentEdit();
                this.ActiveControl = null;
                //检测采购单号：采购单号不需要手动输入

                fun_金额的变化();

                //if (txt_采购单类型.Text == "委外采购")
                //{
                //    if (dt_采购单明细.DefaultView.Count > 1)
                //        throw new Exception("委外采购只能添加一条明细");
                //}
                fun_check主表的数据();  //检查采购单主表的数据
                                   //if (dt_采购单明细.Rows.Count > 0)
                                   //{

                //   // fun_check采购单明细(); //检查采购明细的数据
                //}
                fun_save数据();
                //数据保存之后，重新查询，强行加载一遍
                fun_采购单明细查询(drm["GUID"].ToString());
                


                bl_主计划池 = false;
                //searchLookUpEdit1.EditValue = "";

                ////采购供应商表
                //try
                //{
                //    string sql2 = "select 供应商ID,供应商名称,供应商负责人,供应商电话,交期,税率 from 采购供应商表 where 供应商状态 = '在用' order by 供应商ID";
                //    dt_供应商表 = MasterSQL.Get_DataTable(sql2, CPublic.Var.strConn);
                //    foreach (DataRow r in dt_供应商表.Rows)
                //    {
                //        txt_gysid.Properties.Items.Add(r["供应商ID"].ToString());
                //    }
                //    searchLookUpEdit1.Properties.DataSource = dt_供应商表;
                //    searchLookUpEdit1.Properties.DisplayMember = "供应商ID";
                //    searchLookUpEdit1.Properties.ValueMember = "供应商ID";
                //}
                //catch (Exception ex)
                //{
                //    CZMaster.MasterLog.WriteLog(ex.Message + " fun_下拉框的采购供应商表");
                //    throw new Exception(ex.Message);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //明细删除
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_采购单明细.Rows.Count <= 0)
                    throw new Exception("没有采购单明细可以删除！");
                DataRow rr = (this.BindingContext[dt_采购单明细].Current as DataRowView).Row;
                rr.Delete();

                Decimal s = 0;
                shlv = Convert.ToDecimal(txt_cgshlv.Text) / 100;
                foreach (DataRow r in dt_采购单明细.Rows)
                {

                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["金额"].ToString() != "")
                        s += Convert.ToDecimal(r["金额"]);
                }
                txt_cgshhje.Text = s.ToString("#0.####"); //总金额
                ddwsje = s / (1 + shlv);
                txt_cgshuijin.Text = ((s / (1 + shlv)) * shlv).ToString("#0.####");  //计算税金
                djzje = s;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //删除操作：删除没有生效的采购单，包括明细表
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataView dv = new DataView(dt_采购单主表);
                dv.RowStateFilter = DataViewRowState.Added;
                if (dv.Count > 0)
                {
                    drm.Delete();
                    foreach (DataRow r in dt_采购单明细.Rows)
                    {
                        if (Convert.ToDecimal(r["已送检数"]) > 0)
                        {
                            throw new Exception("已送检的采购单不可删除");
                        }
                        r.Delete();
                    }
                }
                else
                {
                    if (MessageBox.Show(string.Format("你确定要删除采购单号为\"{0}\"的采购单及其明细数据吗？", drm["采购单号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        SqlConnection conn = new SqlConnection(strcon);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("drmdelete");
                        try
                        {   //删除主表的数据
                            drm.Delete();
                            MasterSQL.Save_DataTable(dt_采购单主表, "采购记录采购单主表", strcon);
                            foreach (DataRow r in dt_采购单明细.Rows)  //删除该采购单号明细表的数据
                            {
                                r.Delete();
                            }
                            MasterSQL.Save_DataTable(dt_采购单明细, "采购记录采购单明细表", strcon);
                        }
                        catch
                        {
                            ts.Rollback();
                        }
                        fun_新增采购主表数据(); //新增主表的数据
                        MessageBox.Show("删除成功！");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //界面关闭
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion
        //税率的变化事件  //税率如果变化的话
        private void txt_cgshlv_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txt_cgshlv.Text != "")
                {
                    shlv = Convert.ToDecimal(txt_cgshlv.Text);
                    if (dt_采购单明细 != null && dt_采购单明细.Rows.Count > 0)
                    {
                        fun_金额的变化();

                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //采购生效 减去相应计划池数量
        // 没用了 17-10-7
        private void fun_减计划池()
        {
            DataTable dt = new DataTable();

            foreach (DataRow dr in dt_采购单明细.Rows)
            {
                if (dt.Rows.Count == 0)
                {
                    string sql = string.Format("select  * from [采购记录采购计划表] where 物料编码='{0}'", dr["物料编码"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                    {
                        da.Fill(dt);
                    }
                }

                DataRow[] r = dt.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                if (r.Length == 0)     //加载所有 需要操作的数据
                {
                    string sql = string.Format("select  * from [采购记录采购计划表] where 物料编码='{0}'", dr["物料编码"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                    {
                        da.Fill(dt);
                    }
                    DataRow[] rr = dt.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (rr.Length > 0)
                    {
                        decimal dec = Convert.ToDecimal(rr[0]["未完成采购数量"]) - Convert.ToDecimal(dr["采购数量"]);
                        if (dec > 0)
                        {
                            rr[0]["未完成采购数量"] = dec;
                        }
                        else
                        {
                            rr[0]["未完成采购数量"] = 0;
                        }
                    }
                }
                else
                {
                    decimal dec = Convert.ToDecimal(r[0]["未完成采购数量"]) - Convert.ToDecimal(dr["采购数量"]);
                    if (dec > 0)
                    {
                        r[0]["未完成采购数量"] = dec;
                    }
                    else
                    {
                        r[0]["未完成采购数量"] = 0;
                    }
                }
                string sql_1 = "select * from 采购记录采购计划表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strcon))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                }
            }
        }

        /// <summary>
        /// 为开票补开采购单 不入库
        /// </summary>

        private void fun_采购辅助单生效()
        {
            try
            {
                drm["生效"] = 1;  //生效主单
                drm["生效人员ID"] = CPublic.Var.LocalUserID;
                drm["生效人员"] = CPublic.Var.localUserName; //生效人员
                drm["生效日期"] = CPublic.Var.getDatetime();  //生效日期
                drm["修改日期"] = CPublic.Var.getDatetime();

                foreach (DataRow r in dt_采购单明细.Rows)
                {
                    r["生效"] = 1;
                    r["生效人员ID"] = CPublic.Var.LocalUserID;
                    r["生效人员"] = CPublic.Var.localUserName;
                    r["生效日期"] = CPublic.Var.getDatetime();
                }

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购单辅助主表 where 1<>1", conn, ts);
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购明细辅助表 where 1<>1", conn, ts);
                try
                {
                    //MasterSQL.Save_DataTable(dt_采购单主表, "采购记录采购单主表", ts);
                    SqlDataAdapter da;
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_采购单主表);

                    da = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da);
                    da.Update(dt_采购单明细);
                    //MasterSQL.Save_DataTable(dt_采购单明细, "采购记录采购单明细表", ts);
                    ts.Commit();
                }
                catch
                {
                    ts.Rollback();
                }

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_采购单生效");
                throw new Exception("生效失败");
            }
        }
        /// <summary>
        /// 补开采购 直接生成入库单
        /// </summary>
        private void fun_直接生成入库单()
        {

            DataTable dt = new DataTable();
            DataTable dt_mx = new DataTable();
            string sql = "select  * from L采购记录采购单入库主表L where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(dt);
            string sql_mx = "select  * from L采购记录采购单入库明细L where 1<>1";
            da = new SqlDataAdapter(sql_mx, strcon);
            da.Fill(dt_mx);
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow r in dt_采购单明细.Rows)     //
            {
                DataRow dr = dt.NewRow();

                dr["GUID"] = System.Guid.NewGuid();
                dr["入库单号"] = string.Format("PC{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PC", t.Year, t.Month));
                dr["修改日期"] = t;
                if (dr["操作员ID"].ToString() == "")
                {
                    dr["操作员ID"] = CPublic.Var.LocalUserID;
                    dr["操作员"] = CPublic.Var.localUserName;
                }
                dr["录入日期"] = t;
                dr["供应商ID"] = searchLookUpEdit1.EditValue.ToString();
                dr["供应商"] = txt_cggys.Text;
                dr["供应商负责人"] = txt_gysfzr.Text;
                dr["供应商电话"] = txt_gysdh.Text;

                dr["生效"] = true;
                dr["创建日期"] = t;
                dt.Rows.Add(dr);
                int pos = 1;

                DataRow dr_mx = dt_mx.NewRow();
                if (dr_mx.RowState == DataRowState.Deleted) continue;
                if (dr_mx["GUID"] == DBNull.Value)
                {
                    dr_mx["GUID"] = System.Guid.NewGuid();
                }
                dr_mx["入库单号"] = dr["入库单号"]; //入库单号
                dr_mx["入库POS"] = 1;
                dr_mx["入库明细号"] = dr["入库单号"].ToString() + "-" + pos.ToString("00");


                dr_mx["录入日期"] = t;
                if (dr_mx["操作员ID"].ToString() == "")
                {
                    dr_mx["操作员ID"] = CPublic.Var.LocalUserID;
                    dr_mx["操作员"] = CPublic.Var.localUserName;
                }

                dr_mx["入库量"] = r["采购数量"];
                dr_mx["入库量"] = r["采购数量"];
                dr_mx["采购数量"] = r["采购数量"];
                dr_mx["采购单号"] = r["采购单号"];
                dr_mx["采购单明细号"] = r["采购明细号"];
                dr_mx["物料编码"] = r["物料编码"];
                dr_mx["物料名称"] = r["物料名称"];
                dr_mx["图纸编号"] = r["图纸编号"];
                dr_mx["规格型号"] = r["规格型号"];
                dr_mx["未税单价"] = r["未税单价"];
                dr_mx["单价"] = r["单价"];
                dr_mx["税率"] = Convert.ToInt32(txt_cgshlv.Text);
                dr_mx["未税金额"] = r["未税金额"];
                dr_mx["金额"] = r["金额"];
                dr_mx["供应商ID"] = searchLookUpEdit1.EditValue.ToString();
                dr_mx["供应商"] = txt_cggys.Text;
                dr_mx["供应商负责人"] = txt_gysfzr.Text;
                dr_mx["供应商电话"] = txt_gysdh.Text;
                dr_mx["生效"] = true;
                dr_mx["入库量"] = r["采购数量"];
                dr_mx["价格核实"] = false;

                dt_mx.Rows.Add(dr_mx);
            }

            CZMaster.MasterSQL.Save_DataTable(dt, "L采购记录采购单入库主表L", strcon);
            CZMaster.MasterSQL.Save_DataTable(dt_mx, "L采购记录采购单入库明细L", strcon);

        }

        /// <summary>
        /// 开发采购也要开票  为了开票那边区别开发采购和 补开采购的区别 送检单号赋为1
        /// </summary>
        private void fun_直接生成入库单_2()
        {

            DataTable dt = new DataTable();
            DataTable dt_mx = new DataTable();
            string sql = "select  * from 采购记录采购单入库主表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(dt);
            string sql_mx = "select  * from 采购记录采购单入库明细 where 1<>1";
            da = new SqlDataAdapter(sql_mx, strcon);
            da.Fill(dt_mx);
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow r in dt_采购单明细.Rows)     //
            {
                if (r.RowState == DataRowState.Deleted) continue;

                DataRow dr = dt.NewRow();

                dr["GUID"] = System.Guid.NewGuid();
                dr["入库单号"] = string.Format("PC{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PC", t.Year, t.Month));
                dr["修改日期"] = t;
                dr["操作员ID"] = CPublic.Var.LocalUserID;
                dr["操作员"] = CPublic.Var.localUserName;
                dr["录入日期"] = t;
                dr["供应商ID"] = searchLookUpEdit1.EditValue.ToString();
                dr["供应商"] = txt_cggys.Text;
                dr["供应商负责人"] = txt_gysfzr.Text;
                dr["供应商电话"] = txt_gysdh.Text;

                dr["生效"] = true;
                dr["创建日期"] = t;
                dt.Rows.Add(dr);
                int pos = 1;

                DataRow dr_mx = dt_mx.NewRow();
                if (dr_mx["GUID"] == DBNull.Value)
                {
                    dr_mx["GUID"] = System.Guid.NewGuid();
                }
                dr_mx["入库单号"] = dr["入库单号"]; //入库单号
                dr_mx["入库POS"] = 1;
                dr_mx["入库明细号"] = dr["入库单号"].ToString() + "-" + pos.ToString("00");


                dr_mx["录入日期"] = t;

                dr_mx["操作员ID"] = CPublic.Var.LocalUserID;
                dr_mx["操作员"] = CPublic.Var.localUserName;
                dr_mx["入库量"] = r["采购数量"];
                dr_mx["入库量"] = r["采购数量"];
                dr_mx["采购数量"] = r["采购数量"];
                dr_mx["采购单号"] = r["采购单号"];
                dr_mx["采购单明细号"] = r["采购明细号"];
                dr_mx["送检单号"] = "1"; //只为在采购开票可区分补开采购和 开发采购

                dr_mx["物料编码"] = r["物料编码"];
                dr_mx["物料名称"] = r["物料名称"];
                dr_mx["图纸编号"] = r["图纸编号"];
                dr_mx["规格型号"] = r["规格型号"];
                dr_mx["未税单价"] = r["未税单价"];
                dr_mx["单价"] = r["单价"];

                dr_mx["税率"] = Convert.ToInt32(txt_cgshlv.Text);
                dr_mx["未税金额"] = r["未税金额"];
                dr_mx["金额"] = r["金额"];
                dr_mx["供应商ID"] = searchLookUpEdit1.EditValue.ToString();
                dr_mx["供应商"] = txt_cggys.Text;
                dr_mx["供应商负责人"] = txt_gysfzr.Text;
                dr_mx["供应商电话"] = txt_gysdh.Text;
                dr_mx["生效"] = true;
                dr_mx["入库量"] = r["采购数量"];
                dr_mx["价格核实"] = false;

                dt_mx.Rows.Add(dr_mx);
            }

            CZMaster.MasterSQL.Save_DataTable(dt, "采购记录采购单入库主表", strcon);
            CZMaster.MasterSQL.Save_DataTable(dt_mx, "采购记录采购单入库明细", strcon);

        }

        /// <summary>
        /// 保存生成 审核单
        /// </summary>
        /// 
        //private DataTable fun_PA(string str_采购单号)
        //{
        //    DataRow r_upper = ERPorg.Corg.fun_hr_upper("采购单", CPublic.Var.LocalUserID);
        //    if (r_upper == null)
        //    {
        //        throw new Exception("未找到你的上级审核人员");
        //    }
        //    DataTable dt_申请;
        //    string s = string.Format("select * from  单据审核申请表 where 关联单号='{0}'", str_采购单号);
        //    dt_申请 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
        //    DateTime t = CPublic.Var.getDatetime();
        //    string str_pa = "";
        //    if (dt_申请.Rows.Count == 0)
        //    {
        //        str_pa = string.Format("AP{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("AP", t.Year, t.Month));
        //        // 申请主表记录
        //        DataRow r_z = dt_申请.NewRow();
        //        r_z["审核申请单号"] = str_pa;
        //        r_z["关联单号"] = txt_caigousn.Text;
        //        r_z["相关单位"] = txt_cggys.Text;
        //        r_z["单据类型"] = "采购单";
        //        decimal dec = Convert.ToDecimal(txt_cgshhje.Text);
        //        r_z["总金额"] = dec;
        //        r_z["申请人ID"] = CPublic.Var.LocalUserID;
        //        r_z["申请人"] = CPublic.Var.localUserName;
        //        r_z["申请时间"] = t;
        //        r_z["待审核人ID"] = r_upper["工号"];
        //        r_z["待审核人"] = r_upper["姓名"];
        //        dt_申请.Rows.Add(r_z);
        //    }
        //    else
        //    {
        //        str_pa = dt_申请.Rows[0]["审核申请单号"].ToString();
        //        decimal dec = Convert.ToDecimal(txt_cgshhje.Text);
        //        dt_申请.Rows[0]["总金额"] = dec;
        //        dt_申请.Rows[0]["相关单位"] = txt_cggys.Text;
        //        dt_申请.Rows[0]["待审核人ID"] = r_upper["工号"];
        //        dt_申请.Rows[0]["待审核人"] = r_upper["姓名"];
        //        dt_申请.Rows[0]["申请时间"] = t;
        //        dt_申请.Rows[0]["申请人ID"] = CPublic.Var.LocalUserID;
        //        dt_申请.Rows[0]["申请人"] = CPublic.Var.localUserName;
        //    }

        //    return dt_申请;
        //}




        private void fun_采购单生效()
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                string str_id = CPublic.Var.LocalUserID;
                string str_name = CPublic.Var.localUserName;
                drm["生效"] = 1;  //生效主单
                drm["生效人员ID"] = str_id;
                drm["生效人员"] = str_name; //生效人员
                drm["生效日期"] = t;  //生效日期
                drm["修改日期"] = t;

                foreach (DataRow r in dt_采购单明细.Rows)
                {
                    r["生效"] = 1;
                    r["生效人员ID"] = str_id;
                    r["生效人员"] = str_name;
                    r["生效日期"] = t;
                }




                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, ts);
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);
                try
                {

                    SqlDataAdapter da;
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_采购单主表);

                    da = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da);
                    da.Update(dt_采购单明细);

                    ts.Commit();
                }
                catch
                {
                    ts.Rollback();
                }
                #region 17-10-8 注释掉 已无用
                //foreach (DataRow r in dt_采购单明细.Rows)
                //{
                //    StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(), true);

                //    Decimal de = 0;
                //    if (r["明细类型"].ToString() == "MRP类型" && Convert.ToDecimal(r["采购数量"]) > Convert.ToDecimal(r["计划采购量"]))
                //    {
                //        de = Convert.ToDecimal(r["计划采购量"]);
                //    }
                //    else
                //    {
                //        de = Convert.ToDecimal(r["采购数量"]);
                //    }
                //    if (r["采购计划明细号"].ToString() != "")
                //    {
                //        StockCore.StockCorer.fun_采购单生效(r["物料编码"].ToString(), Convert.ToDecimal(r["采购数量"]), de, r["明细类型"].ToString(), r["采购计划明细号"].ToString(), strcon);
                //    }
                //    else
                //    {
                //        string sql1 = string.Format("select * from 采购记录采购计划表 where 物料编码='{0}'", r["物料编码"].ToString());
                //        DataTable dt_计划明细号;
                //        dt_计划明细号 = MasterSQL.Get_DataTable(sql1, strcon);
                //        if (dt_计划明细号.Rows.Count > 0)
                //        {
                //            StockCore.StockCorer.fun_采购单生效(r["物料编码"].ToString(), Convert.ToDecimal(r["采购数量"]), de, r["明细类型"].ToString(), dt_计划明细号.Rows[0]["采购计划明细号"].ToString(), strcon);
                //        }
                //    }
                //}
                #endregion
                foreach (DataRow dr in dt_采购单明细.Rows)
                {
                    StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
                }
                fun_新增采购主表数据();  //生效单据之后，新增状态
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_采购单生效");
                throw new Exception("生效失败");
            }
        }

        private void fun_check生效()
        {
            foreach (DataRow r in dt_采购单明细.Rows)
            {
                //物料是否有效
                string sql_物料是否有效 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                DataTable dt_基础物料 = new DataTable();
                dt_基础物料 = MasterSQL.Get_DataTable(sql_物料是否有效, strcon);
                if (dt_基础物料.Rows.Count == 0)
                {
                    throw new Exception(string.Format("物料'{0}'无效，基础数据物料信息表中不存在该物料信息", r["物料编码"].ToString()));
                }
                //物料是否初始化
                string sql_物料是否初始化 = string.Format("select * from 仓库物料表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                DataTable dt_物料是否初始化 = new DataTable();
                dt_物料是否初始化 = MasterSQL.Get_DataTable(sql_物料是否初始化, strcon);
                if (dt_物料是否初始化.Rows.Count == 0)
                {
                    throw new Exception(string.Format("物料'{0}'无效，仓库物料表中不存在该物料信息", r["物料编码"].ToString()));
                }
            }

        }
        private void fun_开发采购生效()
        {
            DateTime t = CPublic.Var.getDatetime();
            drm["生效"] = 1;  //生效主单
            drm["生效人员ID"] = CPublic.Var.LocalUserID;
            drm["生效人员"] = CPublic.Var.localUserName; //生效人员
            drm["生效日期"] = t;  //生效日期
            drm["修改日期"] = t;

            foreach (DataRow r in dt_采购单明细.Rows)
            {
                r["明细完成日期"] = t;

                r["生效"] = 1;
                r["生效人员ID"] = CPublic.Var.LocalUserID;
                r["生效人员"] = CPublic.Var.localUserName;
                r["生效日期"] = t;
            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
            SqlCommand cmd = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, ts);
            SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);
            try
            {

                SqlDataAdapter da;
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);

                da.Update(dt_采购单主表);

                da = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da);

                da.Update(dt_采购单明细);

                ts.Commit();
            }
            catch
            {
                ts.Rollback();
            }

            fun_新增采购主表数据();
        }
        /// <summary>
        /// 这边不用生效 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //生效操作
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                //保存代码
                fun_金额的变化();
                fun_check主表的数据();  //检查采购单主表的数据
                if (dt_采购单明细.Rows.Count > 0)
                {
                    gv2.CloseEditor();
                    this.BindingContext[dt_采购单明细].EndCurrentEdit();
                    fun_check采购单明细(); //检查采购明细的数据
                }


                if (txt_采购单类型.EditValue != null && txt_采购单类型.EditValue.ToString().Trim() == "补开采购")
                {
                    fun_save_辅助表();
                    fun_采购明细辅助查询(drm["GUID"].ToString());

                }
                else
                {
                    fun_save数据();
                    fun_采购单明细查询(drm["GUID"].ToString());

                }
                //DataView dv = new DataView(dt_采购单主表);
                //dv.RowStateFilter = DataViewRowState.Added;
                //if (dv.Count > 0)
                //    throw new Exception("该采购单是新增的采购单，还没有保存，不能进行生效操作！");
                if (dt_采购单明细.Rows.Count <= 0)
                    throw new Exception("该采购单不具备明细项，不能进行生效操作，请先新增并保存明细！");
                //DataView dv1 = new DataView(dt_采购单明细);
                //dv1.RowStateFilter = DataViewRowState.Added;
                //if (dv1.Count > 0)
                //    throw new Exception("采购单明细有新增，不能进行生效操作，请先保存明细！");
                //fun_check生效();

                //外协采购生成待领料记录


                if (txt_采购单类型.EditValue.ToString() == "外协" && txt_采购单类型.EditValue != null)
                {
                    fun_外协();
                }

                if (txt_采购单类型.EditValue != null && txt_采购单类型.Text.Trim() == "开发采购")
                {
                    fun_开发采购生效();
                    fun_直接生成入库单_2();
                }
                else if (txt_采购单类型.EditValue != null && txt_采购单类型.Text.Trim() == "补开采购")
                {
                    fun_采购辅助单生效();
                    fun_直接生成入库单();
                    fun_新增采购主表数据();

                }
                else
                {
                    fun_采购单生效();
                    //fun_减计划池();
                }

                MessageBox.Show("生效成功！");

                if (MessageBox.Show("是否打印采购单？", "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
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
                //searchLookUpEdit1.SelectionStart = -1;
                searchLookUpEdit1.EditValue = "";

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                MessageBox.Show(ex.Message + "生效失败");
            }
        }

        string str_外协待领 = "";
        /// <summary>
        /// 弃用
        /// </summary>
        private void fun_外协待领料单号()
        {
            DateTime t = CPublic.Var.getDatetime();
            str_外协待领 = string.Format("WXDL{0}{1:D2}{2:D4}", t.Year, t.Month,
                    CPublic.CNo.fun_得到最大流水号("WXDL", t.Year, t.Month));
        }
        /// <summary>
        /// 弃用
        /// </summary>
        private void fun_外协()
        {
            fun_外协待领料单号();
            string sql_主表 = "select * from  采购记录外协采购待领料主表 where 1<>1";
            DataTable dt_主表 = new DataTable();
            SqlDataAdapter da_主表 = new SqlDataAdapter(sql_主表, strcon);
            da_主表.Fill(dt_主表);
            string sql_明细 = "select * from 采购记录外协采购待领料明细表  where 1<>1";
            DataTable dt_明细 = new DataTable();
            SqlDataAdapter da_明细 = new SqlDataAdapter(sql_明细, strcon);
            da_明细.Fill(dt_明细);
            foreach (DataRow r in dt_采购单明细.Rows)
            {
                DataRow dr = dt_主表.NewRow();
                dt_主表.Rows.Add(dr);
                dr["待领料单号"] = str_外协待领;
                dr["采购单号"] = strCGDDH;
                dr["产品编码"] = r["物料编码"];
                dr["产品名称"] = r["物料名称"];
                dr["外协数量"] = Convert.ToDecimal(r["采购数量"]);
                dr["规格型号"] = r["规格型号"];
                //dr["原规格型号"] = r[""];
                dr["图纸编号"] = r["图纸编号"];

                //dr["图纸版本"] = r["图纸版本"];


                dr["制单人员"] = CPublic.Var.localUserName;
                dr["制单人员工号"] = CPublic.Var.LocalUserID;
                //dr["领料仓库号"] = "";
                //dr["领料仓库名称"] = "";
                //dr["库位号"] = "";
                //dr["库位名称"] = "";
                //dr["领料人"] = "";
                //dr["领料人工号"] = "";
                dr["供应商"] = txt_cggys.Text;
                dr["供应商ID"] = searchLookUpEdit1.EditValue.ToString();
                dr["供应商负责人"] = txt_gysfzr.Text;
                dr["供应商电话"] = txt_gysdh.Text;
                dr["录入日期"] = CPublic.Var.getDatetime();
                //生成待领料明细
                string sql_BOM = string.Format("select * from 基础数据物料BOM表 where 产品编码 = '{0}'", r["物料编码"].ToString());
                DataTable dt_BOM = new DataTable();
                SqlDataAdapter da_BOM = new SqlDataAdapter(sql_BOM, strcon);
                da_BOM.Fill(dt_BOM);
                int i = 1;
                foreach (DataRow rr in dt_BOM.Rows)
                {
                    DataRow drr = dt_明细.NewRow();
                    dt_明细.Rows.Add(drr);
                    drr["待领料单号"] = str_外协待领;
                    drr["待领料单明细号"] = str_外协待领 + "-0" + i.ToString();
                    drr["采购单号"] = strCGDDH;
                    drr["物料编码"] = rr["子项编码"];
                    drr["物料名称"] = rr["子项名称"];
                    drr["BOM数量"] = Convert.ToDecimal(rr["数量"]);
                    drr["待领料总量"] = Convert.ToDecimal(rr["数量"]) * Convert.ToDecimal(r["采购数量"]);
                    drr["已领数量"] = 0;
                    drr["未领数量"] = Convert.ToDecimal(rr["数量"]) * Convert.ToDecimal(r["采购数量"]);
                    drr["制单人员"] = CPublic.Var.localUserName;
                    drr["制单人员工号"] = CPublic.Var.LocalUserID;
                    drr["创建日期"] = CPublic.Var.getDatetime();
                    i++;
                }
            }
            new SqlCommandBuilder(da_主表);
            new SqlCommandBuilder(da_明细);
            da_明细.Update(dt_明细);
            da_主表.Update(dt_主表);
        }
        //采购单 打印
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        public void Dowork()
        {

            ItemInspection.print_FMS.fun_采购单(strCGDDH, str_打印机);

        }

        //private void gv2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        //{
        //if (e.Column.FieldName == "物料编码")
        //{
        //    DataRow drr = gv2.GetDataRow(gv2.FocusedRowHandle);
        //    DataRow[] dr = dt_物料编码.Select(string.Format("物料编码='{0}'", e.Value.ToString()));
        //    if (dr.Length > 0)
        //    {
        //        drr["物料名称"] = dr[0]["物料名称"];
        //        drr["数量单位"] = dr[0]["计量单位"];
        //        drr["规格型号"] = dr[0]["规格"];
        //        drr["图纸编号"] = dr[0]["图纸编号"];
        //        drr["新数据"] = dr[0]["新数据"];
        //        drr["原ERP物料编号"] = dr[0]["原ERP物料编号"];
        //        drr["图纸版本"] = dr[0]["图纸版本"];


        //        drr["仓库ID"] = dr[0]["仓库号"];
        //        drr["仓库名称"] = dr[0]["仓库名称"];
        //        if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
        //        {
        //            searchLookUpEdit1.EditValue = dr[0]["供应商编号"].ToString().Trim();
        //        }
        //        string ss = "";
        //        //即默认供应商仍然为空时  searchLookUpEdit1.EditValue还是"" 则不需要供应商ID 限制
        //        if (searchLookUpEdit1.EditValue.ToString() != "")
        //        {
        //            ss = string.Format("and 供应商ID='{0}'", searchLookUpEdit1.EditValue.ToString());
        //        }
        //        string sql4 = string.Format(@"select * from 采购供应商物料单价表 where 物料编码='{0}' {1}", e.Value.ToString(), ss);
        //        dt_产品金额对照 = MasterSQL.Get_DataTable(sql4, CPublic.Var.strConn);
        //        DataRow[] dr1 = dt_产品金额对照.Select(string.Format("物料编码='{0}'", drr["物料编码"].ToString()));
        //        if (dr1.Length > 0)
        //        {
        //            if (Convert.ToDouble(dr1[0]["单价"]) != 0)
        //            {
        //                drr["单价"] = dr1[0]["单价"];
        //                drr["采购价"] = dr1[0]["单价"];

        //            }
        //            else
        //            {
        //                MessageBox.Show("该物料在此供应商价目表中单价为0");
        //                drr["采购价"] = 0;
        //                drr["单价"] = 0;
        //                //drr["采购价"] = dr[0]["标准单价"];
        //                //drr["单价"] = dr[0]["标准单价"];
        //            }
        //            if (dr1.Length == 1 && searchLookUpEdit1.EditValue.ToString() == "") //若无默认供应商情况下 价目表中仅有一家有此物料 赋值
        //            {
        //                searchLookUpEdit1.EditValue = dr1[0]["供应商ID"].ToString();
        //            }
        //        }
        //        else    //18-3-21 号 改为只从价目表中取，若价目表中没有则 为0  提示 价目表中没有 先去维护
        //        {
        //            drr["采购价"] = 0;
        //            drr["单价"] = 0;
        //            MessageBox.Show("该物料在此供应商价目表中没有维护单价");
        //            //drr["采购价"] = dr[0]["标准单价"];
        //            //drr["单价"] = dr[0]["标准单价"];
        //        }
        //    }
        //}
        //    if (e.Column.FieldName == "采购数量" || e.Column.FieldName == "单价")
        //    {
        //        fun_金额的变化();
        //    }
        //}
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("是否确认刷新，刷新后界面将清空", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                drm = dt_采购单主表.NewRow();  //新建一行
                dt_采购单主表.Rows.Add(drm);
                dataBindHelper1.DataFormDR(drm); //把新建的行赋值到文本框
                                                 //t_cs = null; drmm = null;
                                                 //frm采购单明细_Load(null, null);
                dt_采购单明细.Clear();
                bl_istj = false;
                fun_编辑();
                checkBox1.Checked = false;
                button2.Enabled = false;
                button5.Enabled = false;
                barLargeButtonItem10.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                txt_采购单类型.Enabled = true;
                //txt_采购单类型.Properties.Items.Contains("计划类型");
                //txt_采购单类型.Properties.Items.Remove("计划类型");
                //2019-10-9 
                txt_采购单类型.Properties.Items.Clear();
                txt_采购单类型.Properties.Items.Add("普通采购");
                txt_采购单类型.Properties.Items.Add("委外采购");
                if (CPublic.Var.LocalUserTeam == "管理员权限")
                {
                    txt_采购单类型.Properties.Items.Add("开发采购");
                }
                else if (CPublic.Var.LocalUserTeam == "开发部权限")
                {
                    txt_采购单类型.Properties.Items.Clear();
                    txt_采购单类型.Properties.Items.Add("开发采购");
                    txt_bumenid.Text = CPublic.Var.localUser部门编号;
                    txt_采购单类型.Text = "开发采购";
                    txt_cgbumen.Text = CPublic.Var.localUser部门名称;
                }
                textBox1.Text = CPublic.Var.LocalUserID;
                txt_cggs.Text = "南京东屋电气有限公司";
                bl_主计划池 = false;
            }
        }

        private void Recal()
        {
            try
            {
                foreach (DataRow dr in dt_采购单明细.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    try
                    {
                        decimal dec = ERPorg.Corg.ReacqPP(searchLookUpEdit1.EditValue.ToString(), dr["物料编码"].ToString());
                        dr["采购价"] = dr["单价"] = ERPorg.Corg.ReacqPP(searchLookUpEdit1.EditValue.ToString(), dr["物料编码"].ToString());
                        fun_金额的变化();
                    }
                    catch
                    {
                        dr["采购价"] = dr["单价"] = 0;

                    }

                }
            }
            catch
            {


            }

        }

        //重新获取价目表单价
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                Recal();
                barLargeButtonItem3_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //作废 
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("确认作废该采购单？", "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (txt_caigousn.Text.Trim() != "")
                {
                    string s = string.Format("select  * from  采购记录采购单主表 where 作废=0 and 采购单号='{0}'", txt_caigousn.Text);
                    DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0]["作废"] = true;
                        dt.Rows[0]["作废人员ID"] = CPublic.Var.LocalUserID;
                        dt.Rows[0]["作废人员"] = CPublic.Var.localUserName;
                        dt.Rows[0]["作废日期"] = CPublic.Var.getDatetime();
                        //new SqlCommandBuilder(da);
                        //da.Update(dt);
                        s = string.Format("select  * from  单据审核申请表 where 关联单号='{0}'", txt_caigousn.Text);
                        //using (SqlDataAdapter a = new SqlDataAdapter(s, strcon))
                        //{
                        //    DataTable tt = new DataTable();
                        //    a.Fill(tt);
                        //    if (tt.Rows.Count > 0)
                        //    {
                        //        tt.Rows[0]["作废"] = 1;
                        //        new SqlCommandBuilder(a);
                        //        a.Update(tt);
                        //    }

                        //}
                        DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        if (tt.Rows.Count > 0)
                        {
                            tt.Rows[0]["作废"] = 1;
                        }
                        s = string.Format("select * from 采购记录采购单明细表 where 作废 = 0 and 采购单号 = '{0}'", txt_caigousn.Text);
                        DataTable ttt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        if (ttt.Rows.Count > 0)
                        {
                            foreach(DataRow dr in ttt.Rows)
                            {
                                dr["作废"] = 1;
                                dr["作废人员"] = CPublic.Var.localUserName;
                                dr["作废人员ID"] = CPublic.Var.LocalUserID;
                            }
                        }
                        //s = "select * from 主计划计划通知单明细";
                        //DataTable dt_计划通知 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        SqlConnection conn = new SqlConnection(strcon);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("作废"); //事务的名称
                        SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, ts);
                        SqlCommand cmd = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);
                        SqlCommand cmd2 = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);

                        try
                        {
                            SqlDataAdapter da;
                            da = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da);
                            da.Update(dt);

                            if (ttt.Rows.Count > 0)
                            {
                                da = new SqlDataAdapter(cmd);
                                new SqlCommandBuilder(da);
                                da.Update(ttt);
                            }
                            
                            if (tt.Rows.Count>0)
                            {
                                da = new SqlDataAdapter(cmd2);
                                new SqlCommandBuilder(da);
                                da.Update(tt);
                            }

                            //cmd = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
                            //da = new SqlDataAdapter(cmd);
                            //new SqlCommandBuilder(da);
                            //da.Update(dt);


                            ts.Commit();
                            MessageBox.Show("已作废");
                            simpleButton2.Enabled = false;
                            simpleButton1.Enabled = false;
                            simpleButton3.Enabled = false;
                            barLargeButtonItem5.Enabled = false;
                            barLargeButtonItem3.Enabled = false;
                            barLargeButtonItem6.Enabled = false;
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw new Exception(ex.Message);
                        }

                     
                    }
                    else
                    {
                        MessageBox.Show("该单子已作废");

                    }
                    


                }
            }
        }
        //提交审核该采购单
        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                //fun_check采购单明细();
                fun_check主表的数据();
                if (txt_caigousn.Text.Trim() != "")
                {
                    string s = string.Format("select  * from  采购记录采购单主表 where 作废=0 and 生效=0 and 采购单号='{0}'", txt_caigousn.Text);
                    SqlDataAdapter da = new SqlDataAdapter(s, strcon);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0]["待审核"] = true;
                        dataBindHelper1.DataToDR(dt.Rows[0]);
                        fun_check主表的数据();


                        DataSet ds = new DataSet();
                        if (txt_采购单类型.Text.Trim() == "委外采购")
                        {
                            //if (dt_采购单明细.DefaultView.Count > 1) throw new Exception("委外采购只能添加一条明细");
                            //2018/5/22  委外加工采购  生成委外领料单（其他出库申请，委外加工 类型，备注为采购单号,明细备注为 采购单明细号）
                            ds = fun_wwll();
                        }
                        DataTable t_审核 = ERPorg.Corg.fun_PA("生效", "采购单", txt_caigousn.Text, txt_cggys.Text);
                        SqlConnection conn = new SqlConnection(strcon);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("psh"); //事务的名称
                        try
                        {
                            SqlCommand cmd = new SqlCommand("select * from 采购记录采购单主表 where 1<>1", conn, ts);
                            SqlDataAdapter aa = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(aa);
                            aa.Update(dt);
                            cmd = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);
                            aa = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(aa);
                            aa.Update(dt_采购单明细);
                            cmd = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
                            aa = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(aa);
                            aa.Update(t_审核);
                            if (ds.Tables.Count > 1)
                            {
                                cmd = new SqlCommand("select * from 其他出入库申请主表 where 1<>1", conn, ts);
                                aa = new SqlDataAdapter(cmd);
                                new SqlCommandBuilder(aa);
                                aa.Update(ds.Tables[0]);
                                cmd = new SqlCommand("select * from 其他出入库申请子表 where 1<>1", conn, ts);
                                aa = new SqlDataAdapter(cmd);
                                new SqlCommandBuilder(aa);
                                aa.Update(ds.Tables[1]);

                            }

                            ts.Commit();
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw new Exception(ex.Message + "提交审核错误,检查委外BOM后重新提交");
                        }

                        MessageBox.Show("已提交审核");
                        bl_主计划池 = false;
                        if (MessageBox.Show("是否要打印合同？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {

                            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                            Type outerForm = outerAsm.GetType("ERPreport.采购合同", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                            object[] drr = new object[3];
                            DataRow drrr = dt.Rows[0];
                            drr[0] = drrr;
                            drr[1] = dt_采购单明细;
                            drr[2] = comboBox1.Text.ToString();


                            //   drr[2] = dr["出入库申请单号"].ToString();
                            Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                            //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                            ui.ShowDialog();


                        }





                        barLargeButtonItem10.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        bl_istj = true;
                        fun_编辑();
                        if (txt_采购单类型.Text.Trim() == "委外采购")
                        {

                            StockCore.frm其它出入库申请 frm = new StockCore.frm其它出入库申请(dr_ww);
                            CPublic.UIcontrol.ShowPage_withoutclosebutton(frm, "委外发料单确认");
                        }






                        //drm = dt_采购单主表.NewRow();  //新建一行
                        //dt_采购单主表.Rows.Add(drm);
                        //dataBindHelper1.DataFormDR(drm); //把新建的行赋值到文本框

                        //txt_cggs.Text = "南京东屋电气有限公司";

                        //txt_cgshlv.Text = "";  //

                        ////txt_cgygh.Text = CPublic.Var.LocalUserID;  //默认当前操作员的ID
                        //txt_cgjbr.Text = CPublic.Var.localUserName;  //默认当前操作员 
                        //drm["GUID"] = System.Guid.NewGuid().ToString();
                        ////dt_采购单主表.Rows.Add(drm);

                        //if (dt_采购单明细 != null)
                        //{
                        //    dt_采购单明细 = dt_采购单明细.Clone();
                        //    gc2.DataSource = dt_采购单明细;
                        //}
                    }
                    else
                    {
                        MessageBox.Show("保存成功后再提交审核");
                    }


                }
                else
                {
                    throw new Exception("先保存后审核");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        /// <summary>
        /// 根据=委外BOM 生成领料单
        /// 如果采购单撤销提交审核之后再审核,这里需改为先找有没有 如果有 找出来修改，没有新增
        /// </summary>
        /// <returns></returns>
        private DataSet fun_wwll()
        {
            DataSet ds = new DataSet();
            string s = string.Format("select  * from  其他出入库申请主表 where 备注='{0}'", txt_caigousn.Text);
            DataTable dt_主 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DateTime t = CPublic.Var.getDatetime();

            if (dt_主.Rows.Count == 0)
            {
                string s申请_no = string.Format("DWLS{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
               t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("DWLS", t.Year, t.Month).ToString("0000"));  //19-9-1 QWSQ
                DataRow dr_申请主 = dt_主.NewRow();
                dr_申请主["GUID"] = System.Guid.NewGuid();
                dr_申请主["出入库申请单号"] = s申请_no;
                dr_申请主["申请日期"] = t;
                dr_申请主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_申请主["操作人员"] = CPublic.Var.localUserName;
                dr_申请主["备注"] = txt_caigousn.Text;//关联采购单号
                dr_申请主["申请类型"] = "材料出库";  // 2019-9-1 修正
                dr_申请主["单据类型"] = "材料出库";  // 2019-9-1 修正
                dr_申请主["原因分类"] = "委外加工";
                dt_主.Rows.Add(dr_申请主);
            }
            else
            {
                dt_主.Rows[0]["申请日期"] = t;

            }
            dr_ww = dt_主.Rows[0];
            string ss_no = dt_主.Rows[0]["出入库申请单号"].ToString();
            s = string.Format("select  * from  其他出入库申请子表 where 出入库申请单号='{0}'", ss_no);
            DataTable dt_子 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (dt_子.Rows.Count >= 0)
            {
                int count = dt_子.Rows.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    dt_子.Rows[i].Delete();
                }
            }
            int pos = 1;
            foreach (DataRow r in dt_采购单明细.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;

                //                s = string.Format(@"SELECT  base.产品编号 as 父项编号, 子项编号 ,[数量],b.物料编码,b.物料名称,b.规格型号,组,优先级,base.仓库号,base.仓库名称 FROM [委外加工BOM表] base
                //                                   left join 基础数据物料信息表 a on base.产品编号=a.物料编码
                //                                   left join 基础数据物料信息表 b on base.子项编号 =b.物料编码 where a.物料编码='{0}'", r["物料编码"]);
                s = string.Format(@"SELECT  base.产品编码 as 父项编号, 子项编码 ,[数量],b.物料编码,b.物料名称,b.规格型号,组,优先级,base.仓库号,base.仓库名称 FROM 基础数据物料BOM表 base
                                   left join 基础数据物料信息表 a on base.产品编码=a.物料编码
                                  left join 基础数据物料信息表 b on base.子项编码 =b.物料编码 where a.物料编码='{0}'", r["物料编码"]);
                using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                {
                    DataTable temp = new DataTable();
                    da.Fill(temp);
                    if (temp.Rows.Count > 0)
                    {
                        foreach (DataRow dr in temp.Rows)
                        {
                            //采购明细间 可能BOM中存在同一子项 不合并 每条申请明细都关联一条采购明细
                            DataRow dr_子 = dt_子.NewRow();
                            dr_子["GUID"] = System.Guid.NewGuid();
                            dr_子["出入库申请单号"] = ss_no;
                            dr_子["出入库申请明细号"] = ss_no + "-" + pos.ToString("00");
                            dr_子["POS"] = pos;
                            dr_子["物料编码"] = dr["物料编码"].ToString();
                            dr_子["物料名称"] = dr["物料名称"].ToString();
                            dr_子["仓库号"] = dr["仓库号"].ToString();
                            dr_子["仓库名称"] = dr["仓库名称"].ToString();
                            //19-12-23新增bom数量
                            dr_子["委外bom数量"] = dr["数量"];
                            dr_子["数量"] = Convert.ToDecimal(r["采购数量"]) * Convert.ToDecimal(dr["数量"]);
                            dr_子["规格型号"] = dr["规格型号"].ToString();
                            dr_子["备注"] = r["采购明细号"];
                            dr_子["委外备注1"] = dr["组"]; //组
                            dr_子["委外备注2"] = dr["优先级"]; //优先级
                            dt_子.Rows.Add(dr_子);
                            pos++;
                        }
                    }
                    else
                    {
                        throw new Exception(r["物料编码"].ToString() + "没有BOM");
                    }
                }
            }

            ds.Tables.Add(dt_主);
            ds.Tables.Add(dt_子);
            return ds;

        }

        //private void repositoryItemSearchLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        //{
        //    try
        //    {

        //        DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
        //        DataRow drr = gv2.GetDataRow(gv2.FocusedRowHandle);

        //        drr["物料编码"] = d["物料编码"];
        //        drr["计量单位编码"] = d["计量单位编码"];
        //        drr["物料名称"] = d["物料名称"];
        //        drr["计量单位"] = d["计量单位"];
        //        drr["规格型号"] = d["规格型号"];
        //        drr["图纸编号"] = d["图纸编号"];
        //        drr["新数据"] = d["新数据"];
        //        drr["图纸版本"] = d["图纸版本"];
        //        drr["仓库号"] = d["仓库号"];
        //        drr["仓库名称"] = d["仓库名称"];
        //        if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
        //        {
        //            searchLookUpEdit1.EditValue = d["供应商编号"].ToString().Trim();
        //        }
        //        string ss = "";
        //        //即默认供应商仍然为空时  searchLookUpEdit1.EditValue还是"" 则不需要供应商ID 限制
        //        if (searchLookUpEdit1.EditValue.ToString() != "")
        //        {
        //            ss = string.Format("and 供应商ID='{0}'", searchLookUpEdit1.EditValue.ToString());
        //        }
        //        string sql4 = string.Format(@"select * from 采购供应商物料单价表 where 物料编码='{0}' {1}", d["物料编码"].ToString(), ss);
        //        dt_产品金额对照 = MasterSQL.Get_DataTable(sql4, strcon);
        //        DataRow[] dr1 = dt_产品金额对照.Select(string.Format("物料编码='{0}'", drr["物料编码"].ToString()));
        //        if (dr1.Length > 0)
        //        {
        //            if (Convert.ToDouble(dr1[0]["单价"]) != 0)
        //            {
        //                drr["单价"] = dr1[0]["单价"];
        //                drr["采购价"] = dr1[0]["单价"];

        //            }
        //            else
        //            {
        //                //MessageBox.Show("该物料在此供应商价目表中单价为0");
        //                drr["采购价"] = 0;
        //                drr["单价"] = 0;
        //                //drr["采购价"] = dr[0]["标准单价"];
        //                //drr["单价"] = dr[0]["标准单价"];
        //            }
        //            if (dr1.Length == 1 && searchLookUpEdit1.EditValue.ToString() == "") //若无默认供应商情况下 价目表中仅有一家有此物料 赋值
        //            {
        //                searchLookUpEdit1.EditValue = dr1[0]["供应商ID"].ToString();
        //            }
        //        }
        //        else    //18-3-21 号 改为只从价目表中取，若价目表中没有则 为0  提示 价目表中没有 先去维护
        //        {
        //            drr["采购价"] = 0;
        //            drr["单价"] = 0;
        //           // MessageBox.Show("该物料在此供应商价目表中没有维护单价");
        //            //drr["采购价"] = dr[0]["标准单价"];
        //            //drr["单价"] = dr[0]["标准单价"];
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}

        //private void repositoryItemSearchLookUpEdit1View_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        //{
        //    DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
        //    DataRow drr = gv2.GetDataRow(gv2.FocusedRowHandle);

        //    drr["物料编码"] = d["物料编码"];

        //    drr["物料名称"] = d["物料名称"];
        //    drr["计量单位"] = d["计量单位"];
        //    //drr["计量单位编码"] = d["计量单位编码"];

        //    drr["规格型号"] = d["规格型号"];
        //    drr["图纸编号"] = d["图纸编号"];
        //    drr["新数据"] = d["新数据"];
        //    //drr["原ERP物料编号"] = d["原ERP物料编号"];
        //    drr["图纸版本"] = d["图纸版本"];
        //    drr["仓库号"] = d["仓库号"];
        //    drr["仓库名称"] = d["仓库名称"];
        //    if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
        //    {
        //        searchLookUpEdit1.EditValue = d["供应商编号"].ToString().Trim();
        //    }
        //    string ss = "";
        //    //即默认供应商仍然为空时  searchLookUpEdit1.EditValue还是"" 则不需要供应商ID 限制
        //    if (searchLookUpEdit1.EditValue.ToString() != "")
        //    {
        //        ss = string.Format("and 供应商ID='{0}'", searchLookUpEdit1.EditValue.ToString());
        //    }
        //    string sql4 = string.Format(@"select * from 采购供应商物料单价表 where 物料编码='{0}' {1}", d["物料编码"].ToString(), ss);
        //    dt_产品金额对照 = MasterSQL.Get_DataTable(sql4, strcon);
        //    DataRow[] dr1 = dt_产品金额对照.Select(string.Format("物料编码='{0}'", drr["物料编码"].ToString()));
        //    if (dr1.Length > 0)
        //    {
        //        if (Convert.ToDouble(dr1[0]["单价"]) != 0)
        //        {
        //            drr["单价"] = dr1[0]["单价"];
        //            drr["采购价"] = dr1[0]["单价"];

        //        }
        //        else
        //        {
        //            MessageBox.Show("该物料在此供应商价目表中单价为0");
        //            drr["采购价"] = 0;
        //            drr["单价"] = 0;
        //            //drr["采购价"] = dr[0]["标准单价"];
        //            //drr["单价"] = dr[0]["标准单价"];
        //        }
        //        if (dr1.Length >= 1 && searchLookUpEdit1.EditValue.ToString() == "") //若无默认供应商情况下 价目表中仅有一家有此物料 赋值
        //        {
        //            searchLookUpEdit1.EditValue = dr1[0]["供应商ID"].ToString();
        //        }
        //    }
        //    else    //18-3-21 号 改为只从价目表中取，若价目表中没有则 为0  提示 价目表中没有 先去维护
        //    {
        //        drr["采购价"] = 0;
        //        drr["单价"] = 0;
        //        MessageBox.Show("该物料在此供应商价目表中没有维护单价");
        //        //drr["采购价"] = dr[0]["标准单价"];
        //        //drr["单价"] = dr[0]["标准单价"];
        //    }

        //}



        private void gv2_CellValueChanged_1(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "采购数量" || e.Column.FieldName == "单价")
            {
                if (Convert.ToDecimal( e.Value) ==0)
                {
                    gv2.GetDataRow(e.RowHandle)["金额"]=0;
                }
                fun_金额的变化();
            }
            if (e.Column.FieldName == "金额")
            {
                fun_金额变化();
            }
            else if (e.Column.FieldName == "物料编码")
            {
                try
                {

                    // DataRow d = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

                    DataRow drr = gv2.GetDataRow(gv2.FocusedRowHandle);
                    drr["物料编码"] = e.Value;
                    DataRow[] ds = dt_物料编码.Select(string.Format("物料编码 = '{0}'", drr["物料编码"]));

                    drr["物料编码"] = ds[0]["物料编码"];
                    drr["计量单位编码"] = ds[0]["计量单位编码"];
                    drr["物料名称"] = ds[0]["物料名称"];
                    drr["计量单位"] = ds[0]["计量单位"];
                    drr["规格型号"] = ds[0]["规格型号"];
                    drr["图纸编号"] = ds[0]["图纸编号"];
                    drr["新数据"] = ds[0]["新数据"];
                    drr["图纸版本"] = ds[0]["图纸版本"];
                    drr["仓库号"] = ds[0]["仓库号"];
                    drr["仓库名称"] = ds[0]["仓库名称"];
                    if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                    {
                        searchLookUpEdit1.EditValue = ds[0]["供应商编号"].ToString().Trim();
                    }
                    string ss = "";
                    //即默认供应商仍然为空时  searchLookUpEdit1.EditValue还是"" 则不需要供应商ID 限制
                    if (searchLookUpEdit1.EditValue.ToString() != "")
                    {
                        ss = string.Format("and 供应商ID='{0}'", searchLookUpEdit1.EditValue.ToString());
                    }
                    string sql4 = string.Format(@"select * from 采购供应商物料单价表 where 物料编码='{0}' {1}", ds[0]["物料编码"].ToString(), ss);
                    dt_产品金额对照 = MasterSQL.Get_DataTable(sql4, strcon);
                    DataRow[] dr1 = dt_产品金额对照.Select(string.Format("物料编码='{0}'", drr["物料编码"].ToString()));
                    if (dr1.Length > 0)
                    {
                        if (Convert.ToDouble(dr1[0]["单价"]) != 0)
                        {
                            drr["单价"] = dr1[0]["单价"];
                            drr["采购价"] = dr1[0]["单价"];

                        }
                        else
                        {
                            //MessageBox.Show("该物料在此供应商价目表中单价为0");
                            drr["采购价"] = 0;
                            drr["单价"] = 0;

                            //drr["采购价"] = dr[0]["标准单价"];
                            //drr["单价"] = dr[0]["标准单价"];
                        }
                        if (dr1.Length == 1 && searchLookUpEdit1.EditValue.ToString() == "") //若无默认供应商情况下 价目表中仅有一家有此物料 赋值
                        {
                            searchLookUpEdit1.EditValue = dr1[0]["供应商ID"].ToString();
                        }
                    }
                    else    //18-3-21 号 改为只从价目表中取，若价目表中没有则 为0  提示 价目表中没有 先去维护
                    {
                        drr["采购价"] = 0;
                        drr["单价"] = 0;

                        // MessageBox.Show("该物料在此供应商价目表中没有维护单价");
                        //drr["采购价"] = dr[0]["标准单价"];
                        //drr["单价"] = dr[0]["标准单价"];
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (e.Column.FieldName == "仓库号")
            {
                DataRow drr = gv2.GetDataRow(gv2.FocusedRowHandle);
                DataRow[] r = dt_stock.Select(string.Format("仓库号='{0}'", drr["仓库号"]));
                drr["仓库名称"] = r[0]["仓库名称"];
            }
        }

        private void fun_金额变化()
        {
            try
            {
                Decimal s = 0;
                shlv = Convert.ToDecimal(txt_cgshlv.Text) / (decimal)100;


                foreach (DataRow r in dt_采购单明细.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;



                    if (r["采购数量"].ToString() != "" && r["金额"].ToString() != "")
                    {
                        // r["金额"] = (Convert.ToDecimal(r["采购数量"]) * Convert.ToDecimal(r["单价"])).ToString("0.000000");  //金额
                        r["单价"] = (Convert.ToDecimal(r["金额"]) / Convert.ToDecimal(r["采购数量"]));  //金额

                        if (shlv == 0)
                        {
                            r["税金"] = 0;   //计算税金
                            r["未税单价"] = r["单价"];
                            r["未税金额"] = r["金额"];
                        }
                        else
                        {
                            //r["税金"] = ((Convert.ToDecimal(r["金额"]) / (1 + shlv)) * shlv).ToString("0.000000");   //计算税金
                            //r["未税单价"] = (Convert.ToDecimal(r["单价"]) / (1 + shlv)).ToString("0.000000");
                            //r["未税金额"] = (Convert.ToDecimal(r["金额"]) / (1 + shlv)).ToString("0.000000");

                            r["税金"] = ((Convert.ToDecimal(r["金额"]) / (1 + shlv)) * shlv);   //计算税金
                            r["未税单价"] = (Convert.ToDecimal(r["单价"]) / (1 + shlv));
                            r["未税金额"] = (Convert.ToDecimal(r["金额"]) / (1 + shlv));
                        }

                        s += Convert.ToDecimal(r["金额"]);
                        r["未完成数量"] = r["采购数量"];
                    }

                    else if ((r["采购数量"] == DBNull.Value || r["采购数量"].ToString() == "") && r["单价"].ToString() != "")
                    {
                        if (shlv == 0)
                        {
                            r["税金"] = 0;   //计算税金
                            r["未税单价"] = r["单价"];
                            r["未税金额"] = r["金额"];
                        }
                        else
                        {

                            //r["税金"] = ((Convert.ToDecimal(r["金额"]) / (1 + shlv)) * shlv);   //计算税金
                            r["未税单价"] = (Convert.ToDecimal(r["单价"]) / (1 + shlv));

                        }

                    }

                }
                txt_cgshhje.Text = s.ToString("#0.####");
                ddwsje = s / (1 + shlv);
                if (shlv == 0)
                {
                    txt_cgshuijin.Text = "0";  //计算税金
                }
                else
                {
                    txt_cgshuijin.Text = ((s / (1 + shlv)) * shlv).ToString("#0.##");  //计算税金

                }
                djzje = s;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void infolink()
        {
            DateTime t = CPublic.Var.getDatetime().Date.AddDays(7);
            foreach (DataRow dr in dt_采购单明细.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                try
                {
                    if (dr["仓库号"].ToString() == "")
                    {
                        DataRow[] r = dt_物料编码.Select(string.Format("物料编码='{0}'", dr["物料编码"]));

                        dr["到货日期"] = t;
                        dr["新数据"] = r[0]["新数据"].ToString();
                        dr["物料名称"] = r[0]["物料名称"].ToString();
                        dr["计量单位"] = r[0]["计量单位"].ToString();
                        dr["计量单位编码"] = r[0]["计量单位编码"].ToString();

                        dr["规格型号"] = r[0]["规格型号"].ToString();
                        // dr["特殊备注"] = r[0]["特殊备注"].ToString();
                        dr["仓库号"] = r[0]["默认仓库号"].ToString();
                        dr["仓库名称"] = r[0]["默认仓库"].ToString();
                    }
                     
                    Recal();
                }
                catch (Exception ex)
                {

                }

            }

        }

        private void gv2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (gv2.FocusedColumn.Caption == "物料编码") infolink();
            }
        }
        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("该采购单是否确认撤销提交？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = string.Format("select * from 采购记录采购单主表 where 采购单号 = '{0}'", txt_caigousn.Text);
                    SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                    DataTable dt_撤销 = new DataTable();
                    da.Fill(dt_撤销);
                    sql = string.Format("select * from 单据审核申请表  where 关联单号 = '{0}' and  操作类型='生效' and 作废=0 and 审核=0 ", txt_caigousn.Text);
                    da = new SqlDataAdapter(sql, strcon);
                    DataTable dt_审核申请 = new DataTable();
                    da.Fill(dt_审核申请);
                    if (bl_istj)
                    {
                        if (Convert.ToBoolean(dt_撤销.Rows[0]["审核"]))
                        {
                            throw new Exception("采购单已审核，请联系审核人弃审！");
                        }
                        else
                        {
                            if (dt_撤销.Rows.Count > 0)
                            {
                                if (Convert.ToBoolean(dt_撤销.Rows[0]["待审核"]))
                                {
                                    dt_撤销.Rows[0]["待审核"] = 0;
                                    if (dt_审核申请.Rows.Count > 0)
                                    {
                                        dt_审核申请.Rows[0].Delete();
                                    }
                                    sql = "select * from 单据审核申请表 where 1<>1";
                                    da = new SqlDataAdapter(sql, strcon);
                                    new SqlCommandBuilder(da);
                                    da.Update(dt_审核申请);
                                    sql = "select * from 采购记录采购单主表 where 1<>1";
                                    da = new SqlDataAdapter(sql, strcon);
                                    new SqlCommandBuilder(da);
                                    da.Update(dt_撤销);
                                    MessageBox.Show("撤销成功");
                                    bl_istj = false;
                                    fun_编辑();
                                    //drm = dt_撤销.Rows[0];
                                   
                                    //drm.AcceptChanges();
                                    fun_采购单明细查询(dt_撤销.Rows[0]["GUID"].ToString());


                                    barLargeButtonItem10.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_编辑()
        {
            try
            {
                if (bl_istj)
                {
                    string s = string.Format("select * from 采购记录采购单主表 where 采购单号='{0}'", txt_caigousn.Text);
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (t.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(t.Rows[0]["审核"]))
                        {

                            str_单据状态 = "已审核";
                            label20.Visible = true;
                            label20.Text = str_单据状态;

                        }
                        else
                        {
                            str_单据状态 = "审核中";
                            label20.Visible = true;
                            label20.Text = str_单据状态;
                        }
                    }
                }
                else
                {
                    str_单据状态 = "";
                    label20.Visible = false;
                }
                //barLargeButtonItem4.Enabled = !bl_istj;
                barLargeButtonItem3.Enabled = !bl_istj;
                barLargeButtonItem9.Enabled = !bl_istj;
                barLargeButtonItem6.Enabled = !bl_istj;
                txt_ddfs.Enabled = !bl_istj;
                txt_cgjhri.Enabled = !bl_istj;
                txt_cgshlv.Enabled = !bl_istj;
                txt_cgshhje.Enabled = !bl_istj;
                searchLookUpEdit1.Enabled = !bl_istj;
                txt_cggys.Enabled = !bl_istj;
                txt_cgyy.Enabled = !bl_istj;
                txt_采购单类型.Enabled = !bl_istj;
                txt_cgyt.Enabled = !bl_istj;
                simpleButton1.Enabled = !bl_istj;
                simpleButton2.Enabled = !bl_istj;
                gv2.OptionsBehavior.Editable = !bl_istj;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bl_istj)
            {
                fun_编辑();
                string s = string.Format("select * from 采购记录采购单主表 where 采购单号='{0}'", txt_caigousn.Text);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (t.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(t.Rows[0]["审核"]))
                    {

                        str_单据状态 = "已审核";
                        label20.Visible = true;
                        label20.Text = str_单据状态;

                    }
                    else
                    {
                        str_单据状态 = "审核中";
                        label20.Visible = true;
                        label20.Text = str_单据状态;
                    }
                }



            }
            else
            {
                str_单据状态 = "";
                label20.Visible = false;


            }
        }

        private void barLargeButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.采购合同", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                object[] drr = new object[5];
                string sql = "select * from 采购记录采购单主表 where 1<>1";
                // DataRow drrr = CZMaster.MasterSQL.Get_DataRow(sql,strcon);
                DataTable drwq = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                DataRow drrr = drwq.NewRow();
                drwq.Rows.Add(drrr);
                dataBindHelper1.DataToDR(drrr);
                drr[0] = drm;
                drr[1] = dt_采购单明细;
                drr[2] = comboBox1.Text.ToString();
                string str_含税 = Math.Round(Convert.ToDecimal(txt_cgshhje.Text), 2, MidpointRounding.AwayFromZero).ToString();
                string str_不含税 = Math.Round(Convert.ToDecimal(txt_cgshhje.Text) - Convert.ToDecimal(txt_cgshuijin.Text), 2, MidpointRounding.AwayFromZero).ToString();
                ERPorg.Corg cg = new ERPorg.Corg();
                drr[3] = cg.NumToChinese(str_含税);//含税金额
                drr[4] = cg.NumToChinese(str_不含税); //不含税
                                                   //   drr[2] = dr["出入库申请单号"].ToString();
                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                ui.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 过往采购价ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
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

        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
            if (dr == null) return;
            if (dr["备注9"].ToString() != "")
            {
                gridColumn28.OptionsColumn.AllowEdit = false;
                gridColumn28.OptionsColumn.ReadOnly = true;
            }
            else
            {
                gridColumn28.OptionsColumn.AllowEdit = true;
                gridColumn28.OptionsColumn.ReadOnly = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc2, new Point(e.X, e.Y));
                gv2.CloseEditor();
                contextMenuStrip1.Tag = gv2;

            }
            
        }

        private void 物料详细明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "ERPStock.dll"));
                Type outerForm = outerAsm.GetType("ERPStock.frm仓库物料数量明细", false);

                object[] r = new object[2];
                r[0] = dr["物料编码"].ToString();
                r[1] = dr["仓库号"].ToString();
                UserControl ui = Activator.CreateInstance(outerForm, r) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "物料明细查询");

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void 采购送检明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "ERPpurchase.dll"));
                Type outerForm = outerAsm.GetType("ERPpurchase.frm采购送检单列表", false);

                object[] r = new object[1];
                r[0] = dr["物料编码"].ToString();

                UserControl ui = Activator.CreateInstance(outerForm, r) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "送检明细查询");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }



        //private void checkBox1_CheckedChanged(object sender, EventArgs e)
        //{
        //    //if (checkBox1.Checked == true)
        //    //{
        //    //    button4.Enabled = true;
        //    //    button2.Enabled = true;
        //    //    button5.Enabled = true;
        //    //}
        //    //else
        //    //{
        //    //    button4.Enabled = false;
        //    //    button2.Enabled = false;
        //    //    button5.Enabled = false;
        //    //}
        //}

        string strcon_FS = CPublic.Var.geConn("FS");
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (drm == null)
                {
                    throw new Exception("请先新增采购订单！");
                }
                OpenFileDialog open = new OpenFileDialog();
                if (open.ShowDialog() == DialogResult.OK)
                {
                    FileInfo info = new FileInfo(open.FileName);      //判定上传文件的大小
                                                                      //long maxlength = info.Length;
                                                                      //if (maxlength > 1024 * 1024 * 8)
                                                                      //{
                                                                      //    throw new Exception("上传的文件不能超过1M，请重新选择后再上传！");//drM
                                                                      //}
                    MasterFileService.strWSDL = CPublic.Var.strWSConn;
                    CFileTransmission.CFileClient.strCONN = strcon_FS;

                    string type = "";
                    //type = pathName.Substring(pathName.LastIndexOf("."), pathName.Length - pathName.LastIndexOf(".")).Replace(".", "");
                    int s = Path.GetFileName(open.FileName).LastIndexOf(".") + 1;
                    type = Path.GetFileName(open.FileName).Substring(s, Path.GetFileName(open.FileName).Length - s);

                    string strguid = "";  //记录系统自动返回的GUID
                    strguid = CFileTransmission.CFileClient.sendFile(open.FileName);
                    drm["文件GUID"] = strguid;
                    drm["订单原件"] = true;
                    drm["文件"] = Path.GetFileName(open.FileName);
                    drm["上传时间"] = CPublic.Var.getDatetime();
                    drm["后缀"] = type;
                    if (drm["采购单号"].ToString() != "")
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter("select * from 采购记录采购单主表 where 1<>1", strcon))
                        {
                            new SqlCommandBuilder(da);
                            da.Update(dt_采购单主表);
                        }

                    }
                    MessageBox.Show("上传成功！");
                    checkBox1.Checked = true;
                    button2.Enabled = true;
                    button5.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (drm == null)
                {
                    throw new Exception("请重新选择采购订单！");
                }
                if (drm["文件GUID"] == null || drm["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以查看，请先上传文件");
                }

                SaveFileDialog save = new SaveFileDialog();
                // save.Filter = "(*.jpg,*.png,*.jpeg,*.bmp,*.gif)|*.jgp;*.png;*.jpeg;*.bmp;*.gif|All files(*.*)|*.*";
                save.FileName = drm["文件"].ToString() + "." + drm["后缀"].ToString();
                //save.FileName = drm["文件名"].ToString();

                if (save.ShowDialog() == DialogResult.OK)
                {
                    CFileTransmission.CFileClient.strCONN = strcon_FS;
                    CFileTransmission.CFileClient.Receiver(drm["文件GUID"].ToString(), save.FileName);
                    MessageBox.Show("文件下载成功！");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {

                if (drm["文件GUID"] == null || drm["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以查看，请先上传文件");
                }
                //string type = dr["后缀"].ToString();

                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //string fileName = foldPath + CPublic.Var.getDatetime().ToString("yyyy-MM-dd") + "T" + CPublic.Var.getDatetime().ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + drm["文件"].ToString();

                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {
                }
                CFileTransmission.CFileClient.strCONN = strcon_FS;
                CFileTransmission.CFileClient.Receiver(drm["文件GUID"].ToString(), fileName);
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        //引用预订单 20-1-8  16：30
        private void simpleButton4_Click(object sender, EventArgs e)
        {

        }

        private void txt_cgshhje_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //判断按键是不是要输入的类型。
                if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                    e.Handled = true;

                //小数点的处理。
                if ((int)e.KeyChar == 46)                           //小数点
                {
                    if (txt_cgshhje.Text.Length <= 0)
                        e.Handled = true;   //小数点不能在第一位
                    else
                    {
                        float f;
                        float oldf;
                        bool b1 = false, b2 = false;
                        b1 = float.TryParse(txt_cgshhje.Text, out oldf);
                        b2 = float.TryParse(txt_cgshhje.Text + e.KeyChar.ToString(), out f);
                        if (b2 == false)
                        {
                            if (b1 == true)
                                e.Handled = true;
                            else
                                e.Handled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void txt_采购单类型_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (txt_采购单类型.Text == null || txt_采购单类型.Text == "" || txt_采购单类型.Text == "普通采购")
        //        {
        //            checkBox1.Checked = true;
        //        }
        //        else
        //        {
        //            checkBox1.Checked = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //    }
        //}


    }
}
