using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CZMaster;
using System.Runtime.InteropServices;
using System.IO;
namespace ERPpurchase
{
    public partial class UI采购开票 : UserControl
    {
        #region 变量
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string strconn = CPublic.Var.strConn;

        /// <summary>
        /// 供应商信息
        /// </summary>
        DataTable dt_1;

        /// <summary>
        /// 员工信息
        /// </summary>
        DataTable dt_员工;


        DataTable dt_开票列表;
        DataView dv_开票列表;
        string str_供应商ID;
        bool bl_x = false;//标识是否已按供应商筛选
        #endregion


        string StrKpHao = "";

        /// <summary>
        /// 采购入库的dt
        /// </summary>
        DataTable dt_入库;

        /// <summary>
        /// 勾选的开票通知明细
        /// </summary>
        DataTable dt_开票通知明细;

        /// <summary>
        /// 操作的行
        /// </summary>
        DataRow drm;

        /// <summary>
        /// 开票通知单的主表
        /// </summary>
        DataTable dt_开票通知主表;

        string strKptzd = "";
        string cfgfilepath = "";
        #region 加载

        public UI采购开票()
        {
            InitializeComponent();
        }

        public UI采购开票(string kph)
        {
            StrKpHao = kph;
            InitializeComponent();
        }

        /// <summary>
        /// 员工信息表
        /// </summary>
        private void fun_load员工信息()
        {
            SqlDataAdapter da;
            string sql = "select 员工号,姓名,手机,部门 from 人事基础员工表";
            dt_员工 = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_员工);
            txt_kprgh.Properties.DataSource = dt_员工;
            txt_kprgh.Properties.ValueMember = "员工号";
            txt_kprgh.Properties.DisplayMember = "员工号";
        }



        /// <summary>
        /// 供应商表
        /// </summary>
        private void fun_load_供应商信息()
        {
            string str = CPublic.Var.LocalUserID;
            string sql = "select * from 采购供应商表";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dt_1 = new DataTable();
                da.Fill(dt_1);
                txt_gysbh.Properties.DataSource = dt_1;
                txt_gysbh.Properties.DisplayMember = "供应商ID";
                txt_gysbh.Properties.ValueMember = "供应商ID";
            }
        }

        //选择供应商后 带出供应商信息 以及 筛选 左边gridcontrol 的 信息
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txt_gysbh.EditValue == null)
                    txt_gysbh.EditValue = "";
                DataRow[] dr = dt_1.Select(string.Format("供应商ID='{0}'", txt_gysbh.EditValue.ToString()));
                if (dr.Length > 0)
                {
                    txt_gysmc.Text = dr[0]["供应商名称"].ToString();
                    txt_gysfzr.Text = dr[0]["供应商负责人"].ToString();
                    txt_gysdh.Text = dr[0]["供应商电话"].ToString();
                }
                else
                {
                    txt_gysmc.Text = "";
                    txt_gysfzr.Text = "";
                    txt_gysdh.Text = "";
                }
                fun_load采购入库明细();
                if (dt_开票通知明细 != null)
                {
                    DataRow[] t = dt_开票通知明细.Select(string.Format("供应商ID='{0}'", txt_gysbh.EditValue.ToString()));
                    if (t.Length <= 0)
                    {
                        dt_开票通知明细.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UI采购开票_Load(object sender, EventArgs e)
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
                x.UserLayout(this.splitContainer1, this.Name, cfgfilepath);

                fun_load员工信息();
                txt_kprgh.EditValue = CPublic.Var.LocalUserID;
                fun_load_供应商信息();
                txt_gysbh.EditValue = "";
                txt_kptime.EditValue = CPublic.Var.getDatetime();
                if (StrKpHao == "")
                {
                    SqlDataAdapter da;
                    string sql = "";
                    //开票通知单主表
                    sql = "select * from 采购记录采购开票通知单主表 where 1<>1";
                    dt_开票通知主表 = new DataTable();
                    da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt_开票通知主表);
                    //开票通知单明细表
                    sql = @"select 采购记录采购开票通知单明细表.*,采购供应商备注
                                from 采购记录采购开票通知单明细表,基础数据物料信息表 
                                where  采购记录采购开票通知单明细表.物料编码=基础数据物料信息表.物料编码   and  1<>1";
                    dt_开票通知明细 = new DataTable();
                    da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt_开票通知明细);
                    dt_开票通知明细.Columns.Add("可开票数量");
                    gcM.DataSource = dt_开票通知明细;
                    drm = dt_开票通知主表.NewRow();
                    fun_load采购入库明细();
                }
                else
                {
                    fun_查询数据(StrKpHao);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //数据信息的查询
        private void fun_查询数据(string getDanhao)
        {
            try
            {
                SqlDataAdapter da;
                string sql = "";
                sql = string.Format("select * from 采购记录采购开票通知单主表 where 开票通知单号='{0}'", getDanhao);
                da = new SqlDataAdapter(sql, strconn);
                dt_开票通知主表 = new DataTable();
                da.Fill(dt_开票通知主表);
                drm = dt_开票通知主表.NewRow();
                if (dt_开票通知主表.Rows.Count > 0)
                {
                    drm = dt_开票通知主表.Rows[0];
                    dataBindHelper1.DataFormDR(drm);

                }
                sql = string.Format(@"select 采购记录采购开票通知单明细表.* from 采购记录采购开票通知单明细表,基础数据物料信息表 
                            where 采购记录采购开票通知单明细表.物料编码=基础数据物料信息表.物料编码 and 开票通知单号='{0}'", getDanhao);
                da = new SqlDataAdapter(sql, strconn);
                dt_开票通知明细 = new DataTable();
                da.Fill(dt_开票通知明细);
                dt_开票通知明细.Columns.Add("可开票数量");

                 fun_load采购入库明细();
                gcM.DataSource = dt_开票通知明细;

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_查询数据");
                throw ex;
            }
        }

        /// <summary>
        /// 加载入库开票的代办列表
        /// </summary>
        private void fun_load采购入库明细()
        {
            try
            {
                SqlDataAdapter da;
                string sql = "";
                dt_入库 = new DataTable();
                string s_add = "";
                if (CPublic.Var.LocalUserTeam == "开发部权限" || CPublic.Var.localUser部门名称.Contains("开发"))
                {
                    s_add = " and 采购单类型='开发采购'";
                }
                else if (CPublic.Var.LocalUserTeam != "管理员权限")
                {
                    s_add = " and (采购单类型<>'开发采购' or 采购单类型 is null) ";
                }


                if (txt_gysbh.EditValue.ToString() != "")
                {
                    sql = string.Format(@"select crmx.*,czb.采购单类型,采购供应商备注,a.已选择,cmx.生效人员 as 经办人,入库量*crmx.单价 as 入库金额,已开票量 ,isnull(xx,0)xx     from 采购记录采购单入库明细 crmx
                       left join  基础数据物料信息表 on 基础数据物料信息表.物料编码 =crmx.物料编码 
                       left join  (select 入库明细号,  CONVERT(bit,1)  as 已选择,SUM(isnull(开票数量,0))xx    from  采购记录采购开票通知单明细表  group by 入库明细号)a 
                       on a.入库明细号=crmx.入库明细号 
                      left join 采购记录采购单明细表 cmx on   cmx.采购明细号=crmx.采购单明细号
                      left join 采购记录采购单主表  czb on czb.采购单号=cmx.采购单号
                       where  crmx.作废=0 and crmx.生效=1 and abs(crmx.入库量) >abs(isnull(xx,0))
                         
                       and crmx.供应商ID='{0}' {1}", txt_gysbh.EditValue.ToString(), s_add);
                    bl_x = true;
                    str_供应商ID = "";
                }
                else
                {
                    sql = string.Format(@"  select crmx.*,czb.采购单类型,采购供应商备注,a.已选择,cmx.生效人员 as 经办人,入库量*crmx.单价 as 入库金额,已开票量,isnull(xx,0)xx     from 采购记录采购单入库明细 crmx
                    left join  基础数据物料信息表 on 基础数据物料信息表.物料编码 =crmx.物料编码
                    left join  (select 入库明细号,CONVERT(bit,1)  as 已选择,SUM(isnull(开票数量,0))xx   from  采购记录采购开票通知单明细表  group by 入库明细号)a 
                      on a.入库明细号=crmx.入库明细号  
                      left join 采购记录采购单明细表 cmx on   cmx.采购明细号=crmx.采购单明细号
                     left join 采购记录采购单主表  czb on czb.采购单号=cmx.采购单号
                    where crmx.作废=0 and  crmx.生效=1 and abs(crmx.入库量) > abs(isnull(xx,0)) {0}", s_add);
                    bl_x = false;
                }
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_入库);
                if (txt_kptzdh.Text != "")
                {
                    sql = string.Format(@"  select crmx.*,czb.采购单类型,采购供应商备注,a.已选择,cmx.生效人员 as 经办人,入库量*crmx.单价 as 入库金额,已开票量,isnull(xx,0)xx    from 采购记录采购单入库明细 crmx
                    left join  基础数据物料信息表 on 基础数据物料信息表.物料编码 =crmx.物料编码
                    left join  (select 入库明细号,CONVERT(bit,1)  as 已选择,SUM(isnull(开票数量,0))xx   from  采购记录采购开票通知单明细表 where 开票通知单号<>'{0}'group by 入库明细号)a 
                      on a.入库明细号=crmx.入库明细号  
                      left join 采购记录采购单明细表 cmx on   cmx.采购明细号=crmx.采购单明细号
                     left join 采购记录采购单主表  czb on czb.采购单号=cmx.采购单号
                    where crmx.作废=0 and  crmx.生效=1 and crmx.入库明细号 in (select 入库明细号 from 采购记录采购开票通知单明细表 where 开票通知单号='{0}') {1}", txt_kptzdh.Text, s_add);
              
                    DataTable tt = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
                    foreach(DataRow ff in  tt.Rows)
                    {
                       DataRow []ur= dt_入库.Select(string.Format("入库明细号='{0}'",ff["入库明细号"]));
                        if (ur.Length > 0) dt_入库.Rows.Remove(ur[0]);

                        dt_入库.ImportRow(ff);

                    }
                }
                //string sql_补 = "";
                //if (txt_gysbh.EditValue.ToString() != "")
                //{
                //    sql_补 = string.Format(@"select crmx.*,采购供应商备注,a.已选择,入库量*crmx.单价 as 入库金额 from L采购记录采购单入库明细L crmx
                //                        left join  基础数据物料信息表 on 基础数据物料信息表.物料编码 =crmx.物料编码 
                //                        left join  (select 入库明细号, CONVERT(bit,1)  as 已选择  from  采购记录采购开票通知单明细表 where 生效=0 group by 入库明细号)a 
                //                        on a.入库明细号=crmx.入库明细号 
                //                        where  crmx.作废=0 and crmx.生效=1 
                //                        and abs(crmx.入库量)>abs(crmx.已开票量) and crmx.供应商ID='{0}'", txt_gysbh.EditValue.ToString());
                //}
                //else
                //{
                //    sql_补 = string.Format(@"select crmx.*,采购供应商备注,a.已选择,入库量*crmx.单价 as 入库金额    from  L采购记录采购单入库明细L crmx
                //            left join  基础数据物料信息表 on 基础数据物料信息表.物料编码 =crmx.物料编码 
                //            left join  (select 入库明细号, CONVERT(bit,1)  as 已选择  from  采购记录采购开票通知单明细表 where 生效=0 group by 入库明细号)a 
                //            on a.入库明细号=crmx.入库明细号                    
                //            where   crmx.作废=0 and crmx.生效=1 
                //            and abs(crmx.入库量)>abs(crmx.已开票量) ");
                //}
                //da = new SqlDataAdapter(sql_补, strconn);
                //da.Fill(dt_入库);
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dt_入库.Columns.Add(dc);


                if (txt_shuilv.Text == "") txt_shuilv.Text = "0";
                //把采购单明细表中选中过的赋值为true
                foreach (DataRow r in dt_入库.Rows)
                {
                    if (dt_开票通知明细 != null)
                    {
                        DataRow[] dr = dt_开票通知明细.Select(string.Format("入库明细号='{0}'", r["入库明细号"].ToString().Trim()));
                        if (dr.Length > 0)
                        {
                            r["选择"] = true;
                            dr[0]["可开票数量"] = Convert.ToDecimal(r["入库量"]) - Convert.ToDecimal(r["xx"]);
                            if (Convert.ToDecimal(txt_shuilv.Text) != Convert.ToDecimal(dr[0]["税率"]))
                            {
                                txt_shuilv.Text = Convert.ToDecimal(dr[0]["税率"]).ToString();
                            }
                        }

                    }

                }

                if (dt_开票通知明细 != null)
                {
                    foreach (DataRow r in dt_开票通知明细.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted) continue;
                        DataRow[] dr = dt_入库.Select(string.Format("入库明细号='{0}'", r["入库明细号"].ToString()));
                        if (dr.Length <= 0)
                        {
                            r.Delete();

                        }
                    }
                }

                gridControl1.DataSource = dt_入库;
                //dt_入库.ColumnChanged += dt_入库_ColumnChanged;
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_load采购单列表");
                throw ex;
            }
        }
        #endregion



        /// <summary>
        /// 勾选开票的入库选项
        /// 18-7-26 弃用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dt_入库_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (e.Column.ColumnName == "选择")
                {
                    int count = 0;
                    foreach (DataRow r in dt_入库.Rows)
                    {
                        //if (r["选择"].Equals(true) && r["价格核实"].Equals(false))
                        //{
                        //    r["选择"] = false;
                        //    throw new Exception("该入库明细价格尚未核实，请先进行价格核实，再进行开票！");
                        //}
                        if (r["选择"].Equals(true))
                        {
                            DataRow[] dr = dt_开票通知明细.Select(string.Format("入库明细号='{0}'", r["入库明细号"].ToString()));
                            if (dr.Length > 0)
                            {
                                continue;
                            }
                            DataRow r1 = dt_开票通知明细.NewRow();
                            //    r1["原ERP物料编号"] = r["原ERP物料编号"];

                            r1["采购单号"] = r["采购单号"];
                            r1["采购单明细号"] = r["采购单明细号"];
                            r1["送检单号"] = r["送检单号"];
                            r1["送检单明细号"] = r["送检单明细号"];
                            r1["检验记录单号"] = r["检验记录单号"];
                            r1["入库单号"] = r["入库单号"];
                            r1["入库明细号"] = r["入库明细号"];
                            r1["物料编码"] = r["物料编码"];
                            r1["物料名称"] = r["物料名称"];
                            r1["规格型号"] = r["规格型号"];
                            //  r1["原规格型号"] = r["n原ERP规格型号"];

                            //r1["规格型号"] = r["规格型号"];
                            r1["图纸编号"] = r["图纸编号"];
                            r1["BOM版本"] = r["BOM版本号"];
                            r1["入库数量"] = r["入库量"];
                            r1["供应商ID"] = r["供应商ID"];
                            r1["供应商名称"] = r["供应商"];
                            r1["供应商负责人"] = r["供应商负责人"];
                            r1["供应商电话"] = r["供应商电话"];
                            r1["可开票数量"] = Convert.ToDecimal(r["入库量"]) - Convert.ToDecimal(r["已开票量"]);
                            r1["开票数量"] = Convert.ToDecimal(r["入库量"]) - Convert.ToDecimal(r["已开票量"]);

                            r1["采购数量"] = r["采购数量"];
                            //if (Convert.ToDecimal(r["税率"]) == 0)
                            //{
                            //    r1["税率"] =17;
                            //    r["税率"] = 17;
                            //}
                            //else
                            //{
                            r1["税率"] = r["税率"];
                            //}
                            r1["单价"] = r["单价"];
                            r1["金额"] = Math.Round(Convert.ToDecimal(r["单价"]) * Convert.ToDecimal(r["入库量"]),2,MidpointRounding.AwayFromZero);
                            r1["未税单价"] = Math.Round(Convert.ToDecimal(r["单价"]) / (1 + Convert.ToDecimal(r["税率"]) / 100), 2, MidpointRounding.AwayFromZero);
                            r1["未税金额"] = Math.Round(Convert.ToDecimal(r1["金额"]) / (1 + Convert.ToDecimal(r["税率"]) / 100), 2, MidpointRounding.AwayFromZero);
                            r1["税金"] = Convert.ToDecimal(r1["金额"]) - Convert.ToDecimal(r1["未税金额"]);


                            r1["价格核实"] = r["价格核实"];
                            r1["是否急单"] = r["是否急单"];

                            dt_开票通知明细.Rows.Add(r1);
                            txt_gysbh.EditValue = r["供应商ID"].ToString();
                        }
                        else
                        {
                            DataRow[] dr = dt_开票通知明细.Select(string.Format("入库明细号='{0}'", r["入库明细号"].ToString()));
                            if (dr.Length > 0)
                            {
                                foreach (DataRow t in dr)
                                {
                                    t.Delete();
                                }
                            }
                            count++;
                        }
                    }
                    if (count == dt_入库.Rows.Count)
                    {
                        txt_gysbh.EditValue = "";
                    }
                    txt_shuihoujine.Text = "0.00";
                    txt_shuijin.Text = "0.00";
                    txt_shuiqianjine.Text = "0.00";

                    foreach (DataRow r in dt_开票通知明细.Rows)
                    {
                        if (r.RowState == DataRowState.Deleted) continue;
                        //税率
                        txt_shuilv.Text = r["税率"].ToString();
                        //税后金额
                        txt_shuihoujine.Text =(Convert.ToDecimal(txt_shuihoujine.Text) + Convert.ToDecimal(r["金额"])).ToString();
                        //税前金额
                        txt_shuiqianjine.Text = Math.Round(Convert.ToDecimal(txt_shuihoujine.Text) / (1 + Convert.ToDecimal(r["税率"]) / 100),2,MidpointRounding.AwayFromZero).ToString();
                        //税金
                        txt_shuijin.Text = (Convert.ToDecimal(txt_shuihoujine.Text) - Convert.ToDecimal(txt_shuiqianjine.Text)).ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region 调用的方法

        //新增的方法
        private void fun_Newkaipiaotongzhi()
        {
            try
            {
                strKptzd = "";
                StrKpHao = "";
                drm = dt_开票通知主表.NewRow();
                drm["录入日期"] = CPublic.Var.getDatetime();
                dataBindHelper1.DataFormDR(drm);
                txt_kprgh.EditValue = CPublic.Var.LocalUserID;
                bl_x = false;
                str_供应商ID = "";
                if (dt_开票通知明细 != null)
                {
                    dt_开票通知明细.Clear();
                }
                //找到该明细号。
                fun_load采购入库明细();
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_Newsongjiandan");
                throw ex;
            }
        }

        /// <summary>
        /// 检查开票通知单的主表
        /// </summary>
        private void fun_check主表()
        {
            try
            {   //GUID判别是否是新增的
                DateTime t = CPublic.Var.getDatetime();
                if (drm["GUID"] == DBNull.Value)
                {
                    drm["GUID"] = System.Guid.NewGuid();
                    drm["创建日期"] = t;
                    dt_开票通知主表.Rows.Add(drm);
                }
                if (txt_kptzdh.Text == "")
                {
                   StrKpHao= txt_kptzdh.Text = string.Format("KP{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("KP", t.Year, t.Month));

                }
                strKptzd = txt_kptzdh.Text;  //开票通知单号
                if (drm["操作人员ID"].ToString() == "")
                {
                    drm["操作人员ID"] = CPublic.Var.LocalUserID;
                    drm["操作人员"] = CPublic.Var.localUserName;
                }
                drm["修改日期"] = t;
                if (drm["部门名称"].ToString()=="")  drm["部门名称"] = CPublic.Var.localUser部门名称;
                dataBindHelper1.DataToDR(drm);
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_check主表");
                throw ex;
            }
        }

        /// <summary>
        /// 检查开票通知单的明细表
        /// </summary>
        private void fun_check明细表()
        {
            try
            {
                int pos = 0;
                //DataRow[] ds = dt_入库.Select("选择 = 1");
                //DataTable dt_核销 = new DataTable();
                //foreach(DataRow dr in ds)
                //{
                //    if(dr["采购单类型"].ToString() == "委外采购")
                //    {
                //        string sql = string.Format("select * from 委外核销明细表 where 入库单号 = '{0}' ",dr["入库单号"]);
                //        dt_核销 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                //        if(dt_核销.Rows.Count == 0)
                //        {
                //            throw new Exception("有委外产品未核销，不能生效");
                //        }
                //    }
                //}
                foreach (DataRow r in dt_开票通知明细.Rows)
                {
                    // 11-24 
                    //if (r["选择"].Equals(true) && r["价格核实"].Equals(false))
                    //{
                    //    r["选择"] = false;
                    //    throw new Exception("有入库明细价格尚未核实，请先进行价格核实，再进行开票！");
                    //}
                    //
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["GUID"] == DBNull.Value)
                    {
                        r["GUID"] = System.Guid.NewGuid();
                    }
                    r["开票通知单号"] = strKptzd;
                    r["POS"] = ++pos;
                    r["通知单明细号"] = strKptzd + "-" + pos.ToString();
                    r["录入日期"] = txt_kptime.EditValue;

                    if (r["操作人员ID"].ToString() == "")
                    {

                        r["开票人ID"] = txt_kprgh.EditValue.ToString();
                        r["开票人"] = txt_kprgh.EditValue.ToString();
                        r["操作人员ID"] = CPublic.Var.LocalUserID;
                        r["操作人员"] = CPublic.Var.localUserName;
                    }
                    // DataRow[] rr = dt_入库.Select(string.Format("入库明细号='{0}'", r["入库明细号"]));

                    if (Convert.ToDecimal(r["可开票数量"])>0 &&  Convert.ToDecimal(r["开票数量"]) > Convert.ToDecimal(r["可开票数量"]))
                    {
                        throw new Exception(string.Format("'{0}'开票数量大于可开票量", r["入库明细号"]));

                    }
                    if (Convert.ToDecimal(r["可开票数量"]) <0 && Convert.ToDecimal(r["开票数量"]) < Convert.ToDecimal(r["可开票数量"]))
                    {
                        throw new Exception(string.Format("'{0}'开票数量大于可开票量", r["入库明细号"]));

                    }
                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_checkfun_check明细表");
                throw ex;
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        private void fun_Save数据()
        {
            try
            {

                foreach (DataRow r in dt_开票通知明细.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    //默认 折扣1   
                    r["折扣后含税单价"] = r["单价"];
                    r["折扣后含税金额"] = r["金额"];
                    r["折扣后不含税单价"] = r["未税单价"];
                    r["折扣后不含税金额"] = r["未税金额"];
                }

                SqlDataAdapter da;
                string sql = "";
                sql = "select * from 采购记录采购开票通知单主表 where 1<>1";
                da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_开票通知主表);


                sql = "select * from 采购记录采购开票通知单明细表 where 1<>1";
                da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_开票通知明细);
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_Save数据");
                throw ex;
            }
        }


        #endregion


        #region 界面的相关操作

        //刷新的操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                //UI采购开票_Load(null, null);
                if (drm.RowState == DataRowState.Added)
                {
                    fun_Newkaipiaotongzhi();
                }
                else
                {
                    fun_查询数据(txt_kptzdh.Text);
                }
                button4_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增操作
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_Newkaipiaotongzhi();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存操作
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                DataView dv_mx = new DataView(dt_开票通知明细);
                dv_mx.RowStateFilter = DataViewRowState.CurrentRows;
                if (dv_mx.Count <= 0 || dt_开票通知明细.Rows.Count <= 0)
                    throw new Exception("没有开票通知明细，不可进行保存！");
                fun_check主表();
                gvM.CloseEditor();
                this.BindingContext[dt_开票通知明细].EndCurrentEdit();
                fun_check明细表();
                fun_Save数据();
                fun_查询数据(drm["开票通知单号"].ToString());
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //界面的关闭
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        #endregion


        #region  采购开票通知单的生效操作



        private void fun_生效开票通知单()
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                drm["生效"] = 1;
                if (drm["生效人员ID"].ToString() == "")
                {
                    drm["生效人员ID"] = CPublic.Var.LocalUserID;
                    drm["生效人员"] = CPublic.Var.localUserName;

                    drm["生效日期"] = t;
                }
                drm["折扣前总金额"] = drm["总金额"];
                drm["折扣前未税金额"] = drm["未税金额"];

                dataBindHelper1.DataToDR(drm);
                foreach (DataRow r in dt_开票通知明细.Rows)
                {

                    if (r.RowState == DataRowState.Deleted) continue;
                    //if (r.RowState == DataRowState.Deleted) continue;
                    r["生效"] = 1;
                    if (r["生效人员ID"].ToString() == "")
                    {
                        r["生效人员ID"] = CPublic.Var.LocalUserID;
                        r["生效人员"] = CPublic.Var.localUserName;
                        r["生效日期"] = t;
                    }
                    //默认 折扣1   
                    r["折扣后含税单价"] = r["单价"];
                    r["折扣后含税金额"] = r["金额"];
                    r["折扣后不含税单价"] = r["未税单价"];
                    r["折扣后不含税金额"] = r["未税金额"];
                    DataRow[] dr = dt_入库.Select(string.Format("入库明细号='{0}'", r["入库明细号"].ToString()));
                    if (dr.Length > 0)
                    {
                        dr[0]["已开票量"] = Convert.ToDecimal(dr[0]["已开票量"]) + Convert.ToDecimal(r["开票数量"]);
                    }
                }
                //  dt_入库分开两个datatable保存
               // DataTable dt_辅助 = dt_入库.Clone();

                //for (int i = 0; i < dt_入库.Rows.Count; i++)
                //{
                //    if (dt_入库.Rows[i]["送检单号"].ToString() == "" || dt_入库.Rows[i]["送检单号"].ToString().Substring(0, 2) == "MO")   //需存入 L L 表内的记录
                //    {
                //        dt_辅助.ImportRow(dt_入库.Rows[i]);
                //        dt_入库.Rows.Remove(dt_入库.Rows[i]);
                //        i--;

                //    }

                //}


                //DataView dv = new DataView(dt_入库);
                //dv.RowFilter = "送检单号<>''";
                //DataTable dt = dv.ToTable();
                //dt.AcceptChanges();
                //foreach (DataRow r in dt.Rows)
                //{
                //    r.SetModified();
                //}

                //DataView dv_辅助 = new DataView(dt_入库);
                //dv_辅助.RowFilter = "送检单号=''";
                //DataTable dt_辅助 = dv_辅助.ToTable();
                //dt_辅助.AcceptChanges();

                //foreach (DataRow r in dt_辅助.Rows)
                //{
                //    r.SetModified();
                //}
                SqlDataAdapter da;
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("kpsw");
                SqlCommand cmd = new SqlCommand("select * from 采购记录采购开票通知单主表 where 1<>1", conn, ts);
                SqlCommand cmd1 = new SqlCommand("select * from 采购记录采购开票通知单明细表 where 1<>1", conn, ts);
                SqlCommand cmd2 = new SqlCommand("select * from 采购记录采购单入库明细 where 1<>1", conn, ts);
                //SqlCommand cmd3 = new SqlCommand("select * from L采购记录采购单入库明细L where 1<>1", conn, ts);

                try
                {
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_开票通知主表);

                    da = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da);
                    da.Update(dt_开票通知明细);

                    da = new SqlDataAdapter(cmd2);
                    new SqlCommandBuilder(da);
                    da.Update(dt_入库);
                    //da = new SqlDataAdapter(cmd3);
                    //new SqlCommandBuilder(da);
                    //da.Update(dt_辅助);

                    ts.Commit();

                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw new Exception(ex.Message);
                }

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_生效开票通知单");
                throw ex;
            }
        }

        //开票通知单的生效操作
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if(MessageBox.Show(string.Format("确认开票信息是否正确？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataView dv_mx = new DataView(dt_开票通知明细);
                    dv_mx.RowStateFilter = DataViewRowState.CurrentRows;
                    if (dv_mx.Count <= 0 || dt_开票通知明细.Rows.Count <= 0)
                        throw new Exception("没有开票通知明细，不可进行生效！");

                    foreach (DataRow dr in dt_开票通知明细.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;
                        if (dr["价格核实"].Equals(false))
                        {
                            throw new Exception("尚有明细价格未核实,请先核实单价。");
                        }

                    }
                    fun_check主表();
                    fun_check明细表();
                    ERPorg.Corg cg = new ERPorg.Corg();

                    if (cg.price_changed(dt_开票通知明细))
                    {
                        if (MessageBox.Show(string.Format("请注意此张开票通知单需提供价格异动单,是否继续?"), "提醒!", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        {
                            throw new Exception("已取消操作");
                        }

                    }
                    fun_生效开票通知单();
                    fun_Newkaipiaotongzhi();
                    MessageBox.Show("生效成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion


        //勾选代办事项，实时响应
        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_入库].EndCurrentEdit();
                DataRow dr_FR = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr_FR["选择"].Equals(true))
                {
                    DataRow[] rr = dt_开票通知明细.Select(string.Format("入库明细号='{0}'", dr_FR["入库明细号"]));
                    if (rr.Length == 0)
                    {
                        DataRow r1 = dt_开票通知明细.NewRow();
                        // r1["原ERP物料编号"] = dr_FR["原ERP物料编号"];
                        r1["物料编码"] = dr_FR["物料编码"];

                        r1["采购单号"] = dr_FR["采购单号"];
                        r1["采购单明细号"] = dr_FR["采购单明细号"];
                        r1["送检单号"] = dr_FR["送检单号"];
                        r1["送检单明细号"] = dr_FR["送检单明细号"];
                        r1["检验记录单号"] = dr_FR["检验记录单号"];
                        r1["入库单号"] = dr_FR["入库单号"];
                        r1["入库明细号"] = dr_FR["入库明细号"];
                        r1["物料编码"] = dr_FR["物料编码"];
                        r1["物料名称"] = dr_FR["物料名称"];
                        r1["规格型号"] = dr_FR["规格型号"];
                        //r1["原规格型号"] = dr_FR["n原ERP规格型号"];

                        //r1["规格型号"] = r["规格型号"];
                        r1["图纸编号"] = dr_FR["图纸编号"];
                        r1["BOM版本"] = dr_FR["BOM版本号"];
                        r1["入库数量"] = dr_FR["入库量"];
                        r1["供应商ID"] = dr_FR["供应商ID"];
                        r1["供应商名称"] = dr_FR["供应商"];
                        r1["供应商负责人"] = dr_FR["供应商负责人"];
                        r1["供应商电话"] = dr_FR["供应商电话"];
                        r1["可开票数量"] = Convert.ToDecimal(dr_FR["入库量"]) - Convert.ToDecimal(dr_FR["xx"]);
                        r1["开票数量"] = Convert.ToDecimal(dr_FR["入库量"]) - Convert.ToDecimal(dr_FR["xx"]);

                        r1["采购数量"] = dr_FR["采购数量"];
                        //if (Convert.ToDecimal(r["税率"]) == 0)
                        //{
                        //    r1["税率"] =17;
                        //    r["税率"] = 17;
                        //}
                        //else
                        //{
                        r1["税率"] = dr_FR["税率"];
                        //}
                        r1["单价"] = dr_FR["单价"];
                        r1["金额"] = Math.Round(Convert.ToDecimal(dr_FR["单价"]) * Convert.ToDecimal(r1["开票数量"]),2,MidpointRounding.AwayFromZero);
                        r1["未税单价"] = Math.Round(Convert.ToDecimal(dr_FR["单价"]) / (1 + Convert.ToDecimal(dr_FR["税率"]) / 100), 2, MidpointRounding.AwayFromZero);
                        r1["未税金额"] = Math.Round(Convert.ToDecimal(r1["金额"]) / (1 + Convert.ToDecimal(dr_FR["税率"]) / 100), 2, MidpointRounding.AwayFromZero);
                        r1["税金"] = Convert.ToDecimal(r1["金额"]) - Convert.ToDecimal(r1["未税金额"]);


                        r1["价格核实"] = dr_FR["价格核实"];
                        r1["是否急单"] = dr_FR["是否急单"];

                        dt_开票通知明细.Rows.Add(r1);
                
                        txt_gysbh.EditValue = dr_FR["供应商ID"].ToString();
                  

                    }
                }
                else
                {

                    DataRow[] dr = dt_开票通知明细.Select(string.Format("入库明细号='{0}'", dr_FR["入库明细号"].ToString()));
                    if (dr.Length > 0) dr[0].Delete();
                }

                if (dt_开票通知明细.Rows.Count == 0)
                {
                    txt_gysbh.EditValue = "";
                    txt_shuilv.Text = "0";
                }
                txt_shuihoujine.Text = "0.00";
                txt_shuijin.Text = "0.00";
                txt_shuiqianjine.Text = "0.00";

                foreach (DataRow r in dt_开票通知明细.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    //税率
                    //if (txt_shuilv.Text == "" || Convert.ToDecimal(txt_shuilv.Text) == 0)
                    //{
                    txt_shuilv.Text = r["税率"].ToString();
                    //}
                    //税后金额
                    txt_shuihoujine.Text = ((Convert.ToDecimal(txt_shuihoujine.Text) + Convert.ToDecimal(r["金额"]))).ToString();
                    //税前金额
                    txt_shuiqianjine.Text = Math.Round(Convert.ToDecimal(txt_shuihoujine.Text) / (1 + Convert.ToDecimal(r["税率"]) / (decimal)100),2,MidpointRounding.AwayFromZero).ToString();
                    //税金
                    txt_shuijin.Text = (Convert.ToDecimal(txt_shuihoujine.Text) - Convert.ToDecimal(txt_shuiqianjine.Text)).ToString();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void Txt_gysbh_EditValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        //价格核实的右键操作
        private void 价格核实ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);  //获取当前行的写法
                //if (r["价格核实"].Equals(true))
                //{
                //    if (MessageBox.Show("价格已经核实过了，还需要再次核实吗？如果需要请确定！", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                //    {
                //        frm采购价格核实界面 frm = new frm采购价格核实界面(r["入库明细号"].ToString());
                //        CPublic.UIcontrol.AddNewPage(frm, "采购价格核实界面");
                //    }

                //}
                //else
                //{

                //    frm采购价格核实界面 frm = new frm采购价格核实界面(r["入库明细号"].ToString());
                //    CPublic.UIcontrol.AddNewPage(frm, "采购价格核实界面");
                gridView1.CloseEditor();
                this.ActiveControl = null;
                DataView dv = new DataView(dt_入库);
                dv.RowFilter = "选择=1";
                DataTable dt = dv.ToTable();

                dt.AcceptChanges();

                if (StrKpHao != "")
                {
                    frm采购价格核实界面 ui = new frm采购价格核实界面(dt.Copy(), dt_开票通知明细);
                    //CPublic.UIcontrol.AddNewPage(frm, "采购价格核实界面");
                    Form1 fm = new Form1();
                    fm.Controls.Add(ui);
                    ui.Dock = DockStyle.Fill;
                    fm.StartPosition = FormStartPosition.CenterParent;
                    fm.Size = new System.Drawing.Size(1500, 900);
                    fm.Text = "价格核实";
                    fm.ShowDialog();
                    if (ui.bl)
                    {
                        // barLargeButtonItem2_ItemClick(null, null);
                        fun_load采购入库明细();
                        foreach(DataRow dr in dt_入库.Rows)
                        {
                            dr["选择"] = false;
                        }
                    
                        for (int i= dt_开票通知明细.Rows.Count-1;i>=0;i--)
                        {
                            dt_开票通知明细.Rows[i].Delete();
                        }
                        gcM.DataSource = dt_开票通知明细;
                        foreach (DataRow dr in ui.dt_已核价.Rows)
                        {
                            DataRow[] r_rk = dt_入库.Select(string.Format("入库明细号='{0}'", dr["入库明细号"]));
                            r_rk[0]["选择"] = true;
                            gridView1.FocusedRowHandle = gridView1.LocateByDisplayText(0, gridColumn38, dr["入库明细号"].ToString());
                            repositoryItemCheckEdit1_CheckedChanged(null, null);
                            DataRow[] x = dt_开票通知明细.Select(string.Format("入库明细号='{0}'", dr["入库明细号"]));
                            //   x[0]["单价"] = dr["单价"];
                            x[0]["开票数量"] = dr["入库量"];
                        }
                        barLargeButtonItem2_ItemClick(null, null);
                        button4_Click(null, null);
                    }
                }
                else
                {
                    frm采购价格核实界面 ui = new frm采购价格核实界面(dt.Copy());
                    //CPublic.UIcontrol.AddNewPage(frm, "采购价格核实界面");
                    Form1 fm = new Form1();
                    fm.Controls.Add(ui);
                    ui.Dock = DockStyle.Fill;
                    fm.StartPosition = FormStartPosition.CenterParent;
                    fm.Size = new System.Drawing.Size(1500, 900);
                    fm.Text = "价格核实";
                    fm.ShowDialog();

                    if (ui.bl)
                    {
                        fun_load采购入库明细();
                        foreach (DataRow dr in dt_入库.Rows)
                        {
                            dr["选择"] = false;
                        }
                        dt_开票通知明细 = dt_开票通知明细.Clone();
                        gcM.DataSource = dt_开票通知明细;
         
                        foreach (DataRow dr in ui.dt_已核价.Rows)
                        {
                            DataRow[] r_rk = dt_入库.Select(string.Format("入库明细号='{0}'", dr["入库明细号"]));
                            r_rk[0]["选择"] = true;
                            gridView1.FocusedRowHandle = gridView1.LocateByDisplayText(0, gridColumn38, dr["入库明细号"].ToString());
                            repositoryItemCheckEdit1_CheckedChanged(null, null);
                            DataRow []x  =dt_开票通知明细.Select(string.Format("入库明细号='{0}'", dr["入库明细号"]));
           
                            x[0]["开票数量"] = dr["入库量"];

                            x[0]["税率"] = dr["税率"];
                            x[0]["未税单价"] = dr["未税单价"];
                            x[0]["折扣后含税单价"] = dr["单价"];
                            x[0]["折扣后不含税单价"] = dr["未税单价"];
                            x[0]["折扣后不含税金额"] = dr["未税金额"];
                            x[0]["折扣后含税金额"] = dr["金额"];
                            x[0]["金额"] = dr["金额"];
                            x[0]["未税金额"] = dr["未税金额"];
                            x[0]["税金"] = Convert.ToDecimal(dr["金额"]) - Convert.ToDecimal(dr["未税金额"]);
 

                        }
                        button4_Click(null, null);
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                DataRow r = gridView1.GetDataRow(e.RowHandle);
                if (r == null) return;
                if (r["价格核实"].Equals(true))
                {
                    e.Appearance.BackColor = Color.FromArgb(85, 180, 100);
                }
                if (r["已选择"].Equals(true))
                {
                    e.Appearance.BackColor = Color.Yellow;
                }
                if (r["已选择"].Equals(true) && r["价格核实"].Equals(true))
                {
                    e.Appearance.BackColor = Color.FromArgb(85, 180, 100);

                    e.Appearance.BackColor2 = Color.Yellow;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UI采购开票_Enter(object sender, EventArgs e)
        {
            //            try
            //            {
            //                fun_load员工信息();
            //                txt_kprgh.EditValue = CPublic.Var.LocalUserID;
            //                fun_load_供应商信息();
            //                txt_gysbh.EditValue = "";
            //                txt_kptime.EditValue = System.DateTime.Now;
            //                if (StrKpHao == "")
            //                {
            //                    SqlDataAdapter da;
            //                    string sql = "";
            //                    //开票通知单主表
            //                    sql = "select * from 采购记录采购开票通知单主表 where 1<>1";
            //                    dt_开票通知主表 = new DataTable();
            //                    da = new SqlDataAdapter(sql, strconn);
            //                    da.Fill(dt_开票通知主表);
            //                    //开票通知单明细表
            //                    sql = @"select 采购记录采购开票通知单明细表.*,原ERP物料编号,基础数据物料信息表.n原ERP规格型号 as 原规格型号 from 采购记录采购开票通知单明细表,基础数据物料信息表 
            //                                where  采购记录采购开票通知单明细表.物料编码=基础数据物料信息表.物料编码   and  1<>1";
            //                    dt_开票通知明细 = new DataTable();
            //                    da = new SqlDataAdapter(sql, strconn);
            //                    da.Fill(dt_开票通知明细);
            //                    dt_开票通知明细.Columns.Add("可开票数量");
            //                    gcM.DataSource = dt_开票通知明细;
            //                    drm = dt_开票通知主表.NewRow();
            //                    fun_load采购入库明细();
            //                }
            //                else
            //                {
            //                    fun_查询数据(StrKpHao);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message);
            //            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";//txt_gysmc
                saveFileDialog.FileName = txt_gysmc.Text;
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    gcM.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(String Name); //调用win api将指定名称的打印机设置为默认打印机
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDialog1.Document = this.printDocument1;
            DialogResult dr = this.printDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                //Get the Copy times
                int nCopy = this.printDocument1.PrinterSettings.Copies;
                //Get the number of Start Page
                int sPage = this.printDocument1.PrinterSettings.FromPage;
                //Get the number of End Page
                int ePage = this.printDocument1.PrinterSettings.ToPage;
                string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                SetDefaultPrinter(PrinterName);
                decimal dec_含税金额 = 0;
                decimal dec_含税金额总 = 0;
                decimal dec_不含税金额 = 0;
                decimal dec_不含税金额总 = 0;
                foreach (DataRow dr2 in dt_开票通知明细.Rows)
                {
                    dec_不含税金额 = Convert.ToDecimal(dr2["未税金额"]);
                    dec_不含税金额总 += dec_不含税金额;
                    dec_含税金额 = Convert.ToDecimal(dr2["金额"]);
                    dec_含税金额总 += dec_含税金额;
                }
                DataView dv = new DataView(dt_开票通知明细);
                dv.Sort = "送检单号";
                DataTable dt_dy = dv.ToTable();
                ItemInspection.print_FMS.fun_print_采购开票清单(dt_dy, txt_gysmc.Text, false, dec_不含税金额总, dec_含税金额总);

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";//txt_gysmc
                saveFileDialog.FileName = txt_gysmc.Text;
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }



        private void 清除开票ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridView1.CloseEditor();
            this.BindingContext[dt_入库].EndCurrentEdit();
            DataView dv = new DataView(dt_入库);
            dv.RowFilter = "选择=1";
            DataTable dt_删 = dv.ToTable();

            if (MessageBox.Show(string.Format("确认清除这些需开票记录？共{0}条记录", dt_删.Rows.Count), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                foreach (DataRow dr in dt_删.Rows)
                {
                    string sql = string.Format(@"update  [采购记录采购单入库明细]  set 作废=1,作废人员ID='不开票关闭了',作废人员='{0}',作废日期='{1}' 
                       where 入库明细号='{2}' ", CPublic.Var.localUserName, CPublic.Var.getDatetime(), dr["入库明细号"]);
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);

                }
                foreach (DataRow dr in dt_删.Rows)
                {
                    string sql = string.Format(@"update  [L采购记录采购单入库明细L]  set 作废=1,作废人员ID='不开票关闭了',作废人员='{0}',作废日期='{1}' 
                       where 入库明细号='{2}' ", CPublic.Var.localUserName, CPublic.Var.getDatetime(), dr["入库明细号"]);
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);

                }
            }
            fun_load采购入库明细();
            button4_Click(null, null);
        }
         

        private void txt_gysbh_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txt_gysbh.EditValue == null)
                    txt_gysbh.EditValue = "";
                DataRow[] dr = dt_1.Select(string.Format("供应商ID='{0}'", txt_gysbh.EditValue.ToString()));
                if (dr.Length > 0)
                {
                    txt_gysmc.Text = dr[0]["供应商名称"].ToString();
                    txt_gysfzr.Text = dr[0]["供应商负责人"].ToString();
                    txt_gysdh.Text = dr[0]["供应商电话"].ToString();
                }
                else
                {
                    txt_gysmc.Text = "";
                    txt_gysfzr.Text = "";
                    txt_gysdh.Text = "";
                }
                
                
                  fun_load采购入库明细();





                if (dt_开票通知明细 != null)
                {
                    DataRow[] t = dt_开票通知明细.Select(string.Format("供应商ID='{0}'", txt_gysbh.EditValue.ToString()));
                    if (t.Length <= 0)
                    {
                        //dt_开票通知明细.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";//txt_gysmc
            saveFileDialog.FileName = txt_gysmc.Text;
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                gridControl1.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            decimal dec_税前 = 0;
            decimal dec_税后 = 0;

            foreach (DataRow r in dt_开票通知明细.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;

                dec_税前 = dec_税前 + Convert.ToDecimal(r["未税金额"]);

                dec_税后 = dec_税后 + Convert.ToDecimal(r["金额"]);
            }
            //税后金额
            txt_shuihoujine.Text = dec_税后.ToString("0.00");
            //税前金额
            txt_shuiqianjine.Text = dec_税前.ToString("0.00");
            //税金
            txt_shuijin.Text = (Convert.ToDecimal(txt_shuihoujine.Text) - Convert.ToDecimal(txt_shuiqianjine.Text)).ToString("0.00");

        }
        private void fun_核实单据价格()
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_入库].EndCurrentEdit();
                gvM.CloseEditor();
                this.BindingContext[dt_开票通知明细].EndCurrentEdit();
                DataRow r = (this.BindingContext[dt_开票通知明细].Current as DataRowView).Row;
                string sql = "";
                string str_表名 = "";
                //
                foreach (DataRow dr in dt_开票通知明细.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    else if (dr.RowState == DataRowState.Added)
                    {
                        continue;
                    }
                    else
                    {
                        dr["价格核实"] = true;
                    }

                    string sql_1 = string.Format("select  * from  L采购记录采购单入库明细L where 采购单明细号='{0}' ", dr["采购单明细号"]);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strconn))
                    {
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            str_表名 = "L采购记录采购单入库明细L";
                        }
                        else
                        {
                            str_表名 = "采购记录采购单入库明细";

                        }
                    }
                    //
                    DataTable dt_入库明细;

                    sql = string.Format("select *  from {0} where 采购单明细号='{1}'", str_表名, dr["采购单明细号"].ToString());

                    dt_入库明细 = MasterSQL.Get_DataTable(sql, strconn);

                    foreach (DataRow r1 in dt_入库明细.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted)
                        {
                            continue;
                        }

                        r1["价格核实"] = true;

                    }

                    MasterSQL.Save_DataTable(dt_入库明细, str_表名, strconn);
                }
                MasterSQL.Save_DataTable(dt_开票通知明细, "采购记录采购开票通知单明细表", strconn);
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_核实单据价格");
                throw ex;
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
        //批量价格核实
        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认批量核实所选记录？核实过后不可再核实,请确认", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                barLargeButtonItem5_ItemClick(null, null);
                fun_核实单据价格();
                fun_查询数据(txt_kptzdh.Text);
                button4_Click(null, null);
            }
        }

        private void txt_kprgh_EditValueChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (txt_kprgh.EditValue == null)
                    txt_kprgh.EditValue = "";
                DataRow[] dr = dt_员工.Select(string.Format("员工号='{0}'", txt_kprgh.EditValue.ToString()));
                if (dr.Length > 0)
                {
                    txt_kprName.Text = dr[0]["姓名"].ToString();
                }
                else
                {
                    txt_kprName.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem9_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";//txt_gysmc

            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {

                decimal dec_含税金额 = 0;
                decimal dec_含税金额总 = 0;
                decimal dec_不含税金额 = 0;
                decimal dec_不含税金额总 = 0;
                foreach (DataRow dr2 in dt_开票通知明细.Rows)
                {
                    dec_不含税金额 = Convert.ToDecimal(dr2["未税金额"]);
                    dec_不含税金额总 += dec_不含税金额;
                    dec_含税金额 = Convert.ToDecimal(dr2["金额"]);
                    dec_含税金额总 += dec_含税金额;
                }
                DataView dv = new DataView(dt_开票通知明细);
                dv.Sort = "送检单号";
                DataTable dt_dy = dv.ToTable();
                ItemInspection.print_FMS.fun_print_采购开票清单(dt_dy, txt_gysmc.Text, false, dec_不含税金额总, dec_含税金额总, saveFileDialog.FileName);

                MessageBox.Show("ok");
            }
        }

        private void gvM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }
        //全选
        private void button6_Click(object sender, EventArgs e)
        {
            //点击全选按钮  如果 未有过供应商筛选条件  会触发供应商下拉框值变化事件  导致第一条赋值为true后会 重新加载 然后第一条的勾选没有了
            //所以先给值 让供应商下拉框事件先触发 先重新加载该供应商的数据
            if (gridView1.DataRowCount > 0)
                txt_gysbh.EditValue = gridView1.GetDataRow(0)["供应商ID"];
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                gridView1.GetDataRow(i)["选择"] = true;
                gridView1.FocusedRowHandle = gridView1.LocateByDisplayText(0, gridColumn38, gridView1.GetDataRow(i)["入库明细号"].ToString());
                repositoryItemCheckEdit1_CheckedChanged(null, null);
            }
        }
        private void gridControl1_Click(object sender, EventArgs e)
        {
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null) return;
                gvM.FocusedRowHandle = gvM.LocateByDisplayText(0, gridColumn36, dr["入库明细号"].ToString());
            }
            catch (Exception)
            {
            }
        }

        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            decimal dec_含税金额 = 0;
            decimal dec_含税金额总 = 0;
            decimal dec_不含税金额 = 0;
            decimal dec_不含税金额总 = 0;
            foreach (DataRow dr2 in dt_开票通知明细.Rows)
            {
                dec_不含税金额 = Convert.ToDecimal(dr2["未税金额"]);
                dec_不含税金额总 += dec_不含税金额;
                dec_含税金额 = Convert.ToDecimal(dr2["金额"]);
                dec_含税金额总 += dec_含税金额;
            }
            DataView dv = new DataView(dt_开票通知明细);
            dv.Sort = "送检单号";
            DataTable dt_dy = dv.ToTable();

            //  frm采购报表打印 frm = new frm采购报表打印(dr["采购明细号"].ToString(), dr["物料编码"].ToString(), true);
            ERPreport.采购开票 form = new ERPreport.采购开票(dt_dy, dec_不含税金额总, dec_含税金额总);
            form.ShowDialog();




        }

        private void gvM_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "开票数量")
                {
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    dr["金额"] = Math.Round(Convert.ToDecimal(dr["单价"]) * Convert.ToDecimal(e.Value), 6);
                    dr["未税金额"] = Math.Round(Convert.ToDecimal(dr["未税单价"]) * Convert.ToDecimal(e.Value), 6);

                    button4_Click(null, null);
                }
            }
            catch
            {

            }

           
        }

        private void txt_shuiqianjine_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txt_shuijin.Text == "") txt_shuijin.Text = "0";
                if (txt_shuiqianjine.Text == "") txt_shuiqianjine.Text = "0";
                if (txt_shuihoujine.Text == "") txt_shuihoujine.Text = "0";


                txt_shuijin.Text = (Convert.ToDecimal(txt_shuihoujine.Text) - Convert.ToDecimal(txt_shuiqianjine.Text)).ToString();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void txt_shuihoujine_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //判断按键是不是要输入的类型。
                if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                    e.Handled = true;

                //小数点的处理。
                if ((int)e.KeyChar == 46)                           //小数点
                {
                    if (txt_shuihoujine.Text.Length <= 0)
                        e.Handled = true;   //小数点不能在第一位
                    else
                    {
                        float f;
                        float oldf;
                        bool b1 = false, b2 = false;
                        b1 = float.TryParse(txt_shuihoujine.Text, out oldf);
                        b2 = float.TryParse(txt_shuihoujine.Text + e.KeyChar.ToString(), out f);
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

        private void txt_shuiqianjine_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //判断按键是不是要输入的类型。
                if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                    e.Handled = true;

                //小数点的处理。
                if ((int)e.KeyChar == 46)                           //小数点
                {
                    if (txt_shuiqianjine.Text.Length <= 0)
                        e.Handled = true;   //小数点不能在第一位
                    else
                    {
                        float f;
                        float oldf;
                        bool b1 = false, b2 = false;
                        b1 = float.TryParse(txt_shuiqianjine.Text, out oldf);
                        b2 = float.TryParse(txt_shuiqianjine.Text + e.KeyChar.ToString(), out f);
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
    }
}
