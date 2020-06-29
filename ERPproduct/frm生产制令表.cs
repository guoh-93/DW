using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ERPproduct
{
    public partial class frm生产制令表 : UserControl
    {
        #region 成员
        //数据库连接字符串
        string strconn = "";
        DataRow drr = null;
        string str_制令 = "";
        string str_制令单 = "";
        DataTable dt_视图权限;
        public Boolean a;
        DataTable dt_proZLysx;
        bool flag = false;   //用以标记是否是是改制工单  
        DataTable dt_计划池; //用以减去计划池相应数量
        string cfgfilepath = "";
        DataTable dt_班组;
        //有变化的做保存
        #endregion

        #region 自用类
        public frm生产制令表()
        {
            InitializeComponent();
            strconn = CPublic.Var.strConn;
        }

        public frm生产制令表(DataRow r, string str)
        {
            InitializeComponent();
            strconn = CPublic.Var.strConn;
            drr = r;
            str_制令 = str;
        }


        public frm生产制令表(string str_制令单号)
        {
            InitializeComponent();
            str_制令单 = str_制令单号;
            strconn = CPublic.Var.strConn;
            panel2.Visible = false;
            barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        //private void UserLayout(Control x)
        //{
        //    foreach (Control c in x.Controls)
        //    {
        //        if (c is DevExpress.XtraGrid.GridControl)
        //        {
        //            DevExpress.XtraGrid.GridControl g = (c as DevExpress.XtraGrid.GridControl);
        //            if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name + g.MainView.Name)))
        //            {

        //                g.MainView.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + g.MainView.Name));
        //            }
        //        }
        //        if (c.HasChildren)
        //        {
        //            UserLayout(c);
        //        }

        //    }
        //}
#pragma warning disable IDE1006 // 命名样式
        private void frm生产制令表_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
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
                x.UserLayout(this.xtraTabControl1, this.Name, cfgfilepath);


                string s = "select  产品编码,子项编码  from 基础数据物料BOM表 ";
                dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                //foreach (DevExpress.XtraGrid.Columns.GridColumn item in gv_未生效制令.Columns)
                //{
                //    item.OptionsFilter.Reset();   //筛选条件设置为包含  
                //}
                //foreach (DevExpress.XtraGrid.Columns.GridColumn item in gv_已生效制令.Columns)
                //{
                //    item.OptionsFilter.Reset();   //筛选条件设置为包含  
                //}

                //date_前.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().AddDays(-7).ToString("yyyy-MM-dd"));
                //date_后.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().ToString("yyyy-MM-dd"));

                fun_searchMaterial();
                fun_loadsczlMain();
                fun_load已生效制令();

                if (drr != null)
                {
                    DataRow[] r = dt_proZL.Select(string.Format("生产制令单号='{0}'", str_制令));

                    r[0]["选择"] = true;
                    gv_未生效制令.Focus();
                    gv_未生效制令.FocusedRowHandle = gv_未生效制令.LocateByDisplayText(0, gridColumn2, str_制令);
                    gv_未生效制令.SelectRow(gv_未生效制令.FocusedRowHandle);
                }
                if (gv_未生效制令.RowCount > 0)
                {
                    gv_未生效制令.GetDataRow(0)["选择"] = false;
                }
                gv_sczlmain_RowCellClick_1(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region  变量

        /// <summary>
        /// 生产制令的主表
        /// </summary>
        DataTable dt_proZL;

        DataTable dt_bom;

        /// <summary>
        /// 生产制令的明细
        /// </summary>
        DataTable dt_proZLdetail;

        /// <summary>
        /// 物料信息表
        /// </summary>
        DataTable dt_wuliao;

        /// <summary>
        /// 用作界面显示的明细
        /// </summary>
        DataTable dt_dispalymx;

        /// <summary>
        /// 勾选的用于生效制令的集合
        /// </summary>
        DataTable dt_SXZL;

        #endregion

        #region 类加载
        //查找物料的信息填充下拉框
#pragma warning disable IDE1006 // 命名样式
        private void fun_searchMaterial()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                SqlDataAdapter da;
                string sql = @"select base.物料编码,base.物料名称,特殊备注,base.规格型号,kc.仓库号,kc.仓库名称,
                  base.图纸编号,车间编号,库存总数,新数据,b_班组编号 as 班组ID,b_班组名称 as 班组
                 from 基础数据物料信息表 base,仓库物料数量表 kc
                 where   base.物料编码=kc.物料编码 and base.自制=1 and  base.仓库号  in (select  属性字段1 as 仓库号
                 from  基础数据基础属性表 where 属性类别='仓库类别' and 布尔字段1=1 )";//base.物料类型<>'原材料'
                dt_wuliao = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_wuliao);
                repositoryItemSearchLookUpEdit1.DataSource = dt_wuliao;
                // rsl.PopulateColumns();
                repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
                repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";

                sql = "  select  属性字段1 as 班组编号,属性值 as 班组 from  基础数据基础属性表  where 属性类别='班组'";
                dt_班组 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                repositoryItemGridLookUpEdit2.DataSource = dt_班组;
                repositoryItemGridLookUpEdit2.DisplayMember = "班组编号";
                repositoryItemGridLookUpEdit2.ValueMember = "班组编号";

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_searchMaterial");
                throw new Exception(ex.Message);
            }

        }

        //载入未生效的生产制令表
#pragma warning disable IDE1006 // 命名样式
        private void fun_loadsczlMain()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                string sql = "";
                SqlDataAdapter da;
                if (str_制令单 != "")
                {
                    if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserTeam == "管理员权限")
                    {
                        sql = string.Format(@"select sczl.*,库存总数,新数据,拼板数量  from 生产记录生产制令表 sczl
                                               left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                                left join  仓库物料数量表 kc   on    kc.物料编码= sczl.物料编码 and sczl.仓库号=kc.仓库号
                                                where sczl.生产制令单号='{0}' and sczl.关闭 = 0                 
                                                and sczl.生效 = 0 and sczl.完成 = 0  and sczl.生产制令类型<>'研发样品' ", str_制令单);
                    }
                    else  //未排单
                    {
                        sql = string.Format(@"select sczl.*,库存总数,新数据,拼板数量   from 生产记录生产制令表  sczl
                                            left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                             left join  仓库物料数量表 kc  on    kc.物料编码= sczl.物料编码 and sczl.仓库号=kc.仓库号
                                                where sczl.生产制令单号='{0}' and sczl.关闭 = 0                 
                                                and sczl.生效 = 0 and sczl.完成 = 0   and sczl.生产制令类型!='研发样品' ", str_制令单, CPublic.Var.LocalUserID);
                        //and 操作人员ID ='{1}'   19-5-5 先去掉
                    }
                    da = new SqlDataAdapter(sql, strconn);
                    dt_proZL = new DataTable();
                    da.Fill(dt_proZL);
                    DataColumn dc = new DataColumn("选择", typeof(bool));
                    dc.DefaultValue = false;
                    dt_proZL.Columns.Add(dc);
                    dt_proZL.Columns.Add("反馈备注");

                }
                else
                {
                    if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserTeam == "管理员权限")
                    {
                        sql = string.Format(@"select sczl.* ,库存总数,新数据 ,拼板数量  from 生产记录生产制令表 sczl 
                                 left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                             left join  仓库物料数量表 kc  on    kc.物料编码= sczl.物料编码 and sczl.仓库号=kc.仓库号
                                                where sczl.生效 = 0 and sczl.完成 = 0  and sczl.关闭 = 0   and sczl.生产制令类型 !='{0}' ", "研发样品");
                    }

                    else  //未排单
                    {//sql = "select * from 生产记录生产制令表 where 生效=0 and 完成=0";
                        sql = string.Format(@"select sczl.* ,库存总数,新数据,拼板数量   from 生产记录生产制令表 sczl
                                         left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码  
                                         left join  仓库物料数量表 kc  on kc.物料编码= sczl.物料编码   and sczl.仓库号=kc.仓库号
                                        where sczl.生效 = 0 and sczl.完成 = 0  and sczl.关闭 = 0 
                                          and sczl.生产制令类型!='研发样品'", CPublic.Var.LocalUserID);
                        //and (操作人员ID ='{0}') 
                    }
                    //sql += " and ( ";
                    //foreach (DataRow r in dt_视图权限.Rows)
                    //{
                    //    sql += "操作人员ID = '" + r["工号"].ToString().Trim() + "' or ";
                    //}
                    //sql = sql.Substring(0, sql.Length - 3);
                    //sql = sql + " ) ";

                    da = new SqlDataAdapter(sql, strconn);
                    dt_proZL = new DataTable();
                    da.Fill(dt_proZL);

                    DataColumn dc = new DataColumn("选择", typeof(bool));
                    dc.DefaultValue = false;
                    dt_proZL.Columns.Add(dc);
                    dt_proZL.Columns.Add("反馈备注");

                }
                //制令子表
                sql = @"select 生产记录生产制令子表.*,反馈备注 from 生产记录生产制令子表,销售记录销售订单明细表
                where 生产记录生产制令子表.销售订单明细号=销售记录销售订单明细表.销售订单明细号 and   1<>1";
                da = new SqlDataAdapter(sql, strconn);
                dt_proZLdetail = new DataTable();
                dt_SXZL = new DataTable();
                da.Fill(dt_proZLdetail);

                //把下拉框dt没有的数据增加到里面去
                foreach (DataRow r in dt_proZL.Rows)
                {
                    DataRow[] drr1 = dt_wuliao.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
                    if (drr1.Length <= 0)
                    {
                        dt_wuliao.Rows.Add(r["物料编码"], r["物料名称"], r["特殊备注"], r["规格型号"], r["仓库号"], r["仓库名称"], r["图纸编号"], r["生产车间"], r["库存总数"], r["新数据"]);
                    }
                }
                gc_未生效制令.DataSource = dt_proZL;
                gc_关联订单.DataSource = dt_proZLdetail;
                //dt_proZL.ColumnChanged += dt_proZL_ColumnChanged;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_loadsczlMain");
                throw new Exception(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_load已生效制令()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                SqlDataAdapter da;
                string sql = "";
                if (date_前.EditValue != null && date_前.EditValue.ToString() != "" && date_后.EditValue != null && date_后.EditValue.ToString() != "")
                {
                    sql = string.Format("and sczl.生效日期 >= '{0}' and sczl.生效日期 <= '{1}'", Convert.ToDateTime(date_前.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(date_后.EditValue).AddDays(1).ToString("yyyy-MM-dd"));
                }
                if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserTeam == "公司高管权限")//|| CPublic.Var.LocalUserID == "2101" || CPublic.Var.LocalUserID == "2233" || CPublic.Var.LocalUserID == "4136" || CPublic.Var.LocalUserID == "2106"
                {
                    sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数,新数据,isnull(aaa.完工数量,0)完工数量,拼板数量  from 生产记录生产制令表 sczl
        left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
        left join  仓库物料数量表 kc   on    kc.物料编码= sczl.物料编码 and  sczl.仓库号=kc.仓库号
        left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  on sczl.生产制令单号=a.生产制令单号
        left join  (select  生产制令单号,sum(完工数量)完工数量  from (
               select  生产制令单号, case when 完工=1 then 生产数量 else 部分完工数 end as 完工数量   from 生产记录生产工单表)a   
               group by 生产制令单号) aaa  on aaa.生产制令单号 = sczl.生产制令单号
       where  sczl.关闭=0 and sczl.未排单数量>0 and sczl.生效 = 1 and sczl.关闭=0   and sczl.生效日期 >= '2017-12-1' {0} and sczl.生产制令类型!='研发样品' ", sql);
                }
                else
                {
                    sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数,新数据,isnull(aaa.完工数量,0)完工数量,拼板数量   from 生产记录生产制令表 sczl
                                           left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                              left join  仓库物料数量表 kc on    kc.物料编码= sczl.物料编码 and sczl.仓库号=kc.仓库号
                           left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                                    on sczl.生产制令单号=a.生产制令单号
                           left join  (select  生产制令单号,sum(完工数量)完工数量  from (
                           select  生产制令单号, case when 完工=1 then 生产数量 else 部分完工数 end as 完工数量   from 生产记录生产工单表)a   
               group by 生产制令单号) aaa  on aaa.生产制令单号 = sczl.生产制令单号
                                 where  sczl.生效 = 1  and  sczl.未排单数量>0  
                                    and sczl.生效日期 >= '2018-12-1' and sczl.关闭=0   {1} and sczl.生产制令类型<>'研发样品' ", CPublic.Var.LocalUserID, sql);
                    //and(操作人员ID = '{0}'  or 生产制令类型 = '销售备库')
                }



                //sql += " and ( ";
                //foreach (DataRow r in dt_视图权限.Rows)
                //{
                //    sql += "操作人员ID = '" + r["工号"].ToString().Trim() + "' or ";
                //}
                //sql = sql.Substring(0, sql.Length - 3);
                //sql = sql + " )";
                da = new SqlDataAdapter(sql, strconn);
                dt_proZLysx = new DataTable();
                da.Fill(dt_proZLysx);
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dt_proZLysx.Columns.Add(dc);
                gc_已生效制令.DataSource = dt_proZLysx;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }





        //void dt_proZL_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Column.ColumnName == "原ERP物料编号")
        //        {
        //            DataRow[] dr = dt_wuliao.Select(string.Format("物料编码='{0}'", e.Row["物料编码"].ToString()));
        //            if (dr.Length > 0)
        //            {
        //                e.Row["物料名称"] = dr[0]["物料名称"];
        //                e.Row["规格型号"] = dr[0]["规格型号"];
        //                e.Row["图纸编号"] = dr[0]["图纸编号"];
        //                e.Row["客户ID"] = dr[0]["客户"];
        //                e.Row["客户名称"] = dr[0]["客户名称"];
        //                e.Row["生产车间"] = dr[0]["车间编号"];
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);

                if (e.Column.FieldName == "物料编码")
                {
                    string v_number = "";

                    DataTable dt_x = new DataTable();
                    dt_x = ERPorg.Corg.billofM(dt_x, dr["物料编码"].ToString(), true, dt_bom);
                    if (dt_x.Rows.Count > 0)
                    {
                        foreach (DataRow drr in dt_x.Rows)
                        {
                            string sql1 = string.Format(@"  SELECT 文件名, 物料号, 版本 FROM 程序版本维护表 WHERE 版本 = (SELECT MAX(版本) FROM    程序版本维护表 where  物料号 ='{0}' and 停用='0' ) and 物料号 = '{0}' and 停用='0'  ", drr["子项编码"]);
                            DataRow dr_banbe = CZMaster.MasterSQL.Get_DataRow(sql1, strconn);
                            if (dr_banbe != null)
                            {
                                if (dr_banbe["文件名"].ToString() != "")
                                {
                                    if (v_number == "")
                                    {
                                        v_number = v_number + dr_banbe["文件名"].ToString();
                                    }
                                    else
                                    {
                                        v_number = v_number + ";" + dr_banbe["文件名"].ToString();
                                    }
                                    //  break;
                                }
                            }
                        }
                    }

                    dr["版本备注"] = v_number.ToString();



                }
                else if (e.Column.FieldName == "班组ID")
                {
                    DataRow[] rr = dt_班组.Select(string.Format("班组编号='{0}'", e.Value));
                    if (rr.Length > 0) dr["班组"] = rr[0]["班组"];

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }






            //try
            //{
            //    if (e.Column.FieldName == "物料编码")
            //    {
            //        DataRow[] dr = dt_wuliao.Select(string.Format("物料编码='{0}'", e.Value));
            //        if (dr.Length > 0)
            //        {
            //            DataRow rr = gv_未生效制令.GetDataRow(e.RowHandle);
            //            rr["物料名称"] = dr[0]["物料名称"];
            //            rr["规格型号"] = dr[0]["规格型号"];
            //            rr["图纸编号"] = dr[0]["图纸编号"];
            //            rr["原规格型号"] = dr[0]["n原ERP规格型号"];
            //            rr["物料编码"] = dr[0]["物料编码"];
            //            rr["生产车间"] = dr[0]["车间编号"];
            //            rr["库存总数"] = dr[0]["库存总数"];
            //            rr["特殊备注"] = dr[0]["特殊备注"];
            //            
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

        }
        #endregion

        #region 调用的相关方法

        //新增行
#pragma warning disable IDE1006 // 命名样式
        private void fun_AddNewRow()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow r = dt_proZL.NewRow();
                r["生产制令类型"] = "标准类型";
                r["加急状态"] = "正常";
                dt_proZL.Rows.Add(r);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_AddNewRow");
                throw new Exception(ex.Message);
            }
        }

        //生效减去计划池的数量    11/14     gh  //如果是MRP 类型就不用  调用这个减计划池 
        //最新的 计划采购界面 就不用 计划池了  生产记录生产计划表 也不用了

#pragma warning disable IDE1006 // 命名样式
        private DataTable fun_减计划池()
#pragma warning restore IDE1006 // 命名样式
        {
            DataTable dt = new DataTable();

            foreach (DataRow dr in dt_proZL.Rows)
            {
                if (dr["选择"].Equals(true) && dr["生产制令类型"].ToString() == "标准类型")
                {

                    if (dt.Rows.Count == 0)
                    {
                        string sql = string.Format("select  * from [生产记录生产计划表] where 物料编码='{0}'", dr["物料编码"]);
                        using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                        {
                            da.Fill(dt);
                        }

                    }

                    DataRow[] r = dt.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (r.Length == 0)     //加载所有 需要操作的数据
                    {
                        string sql = string.Format("select  * from [生产记录生产计划表] where 物料编码='{0}'", dr["物料编码"]);
                        using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                        {
                            da.Fill(dt);
                        }
                        DataRow[] rr = dt.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (rr.Length > 0)
                        {
                            decimal dec = Convert.ToDecimal(rr[0]["计划数量"]) - Convert.ToDecimal(dr["制令数量"]);
                            if (dec > 0)
                            {
                                rr[0]["未生成数量"] = rr[0]["计划数量"] = dec;

                            }
                            else
                            {
                                rr[0]["未生成数量"] = rr[0]["计划数量"] = 0;
                            }
                        }

                    }
                    else
                    {

                        decimal dec = Convert.ToDecimal(r[0]["计划数量"]) - Convert.ToDecimal(dr["制令数量"]);
                        if (dec > 0)
                        {
                            r[0]["未生成数量"] = r[0]["计划数量"] = dec;
                        }
                        else
                        {
                            r[0]["未生成数量"] = r[0]["计划数量"] = 0;
                        }
                    }


                }
            }

            return dt;
        }

        //检测制令明细的数据的合法性
#pragma warning disable IDE1006 // 命名样式
        private void fun_checkSaveMXData()
#pragma warning restore IDE1006 // 命名样式
        {
            //try
            //{
            //    string strzld = "";
            //    int pos = 1;
            //    foreach (DataRow r in dt_proZLdetail.Rows)
            //    {
            //      // if(str)

            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        //检查保存制令的数据的合法性
#pragma warning disable IDE1006 // 命名样式
        private void fun_checkSaveZLData()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                string str_id = CPublic.Var.LocalUserID;
                string str_name = CPublic.Var.localUserName;
                foreach (DataRow r in dt_proZL.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;


                    if (r["生产制令类型"].ToString() == "")
                        throw new Exception("生产制令类型不能为空，请选择！");
                    if (r["物料编码"].ToString() == "")
                        throw new Exception("物料编码不能为空，请选择！");
                    if (r["制令数量"].ToString() == "")
                        throw new Exception("制令数量不能为空，请填写！");

                    string sql_新 = string.Format("select 子项编码 from 基础数据物料BOM表 where 产品编码='{0}'", r["物料编码"].ToString());
                    DataTable dt_x = CZMaster.MasterSQL.Get_DataTable(sql_新, strconn);
                    if (dt_x.Rows.Count == 0)
                    {
                        throw new Exception(string.Format("物料'{0}'尚未有BOM", r["物料编码"].ToString()));
                    }


                    r["未排单数量"] = r["制令数量"];

                    r["预完工日期"] = r["预计完工日期"];
 
                    try
                    {
                        decimal dd = Convert.ToDecimal(r["制令数量"]);
                    }
                    catch
                    {
                        throw new Exception("制令数量应该是数字，请重新填写！");
                    }

                    //如果GUID是空的说明是新增的
                    if (r["GUID"].ToString().Trim() == "")
                    {
                        r["操作人员"] = str_name;
                        r["操作人员ID"] = str_id;

                        r["GUID"] = System.Guid.NewGuid();

                        r["生产制令单号"] = string.Format("PM{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                            CPublic.CNo.fun_得到最大流水号("PM", t.Year, t.Month));

                        r["日期"] = t;
                        r["制单人员"] = str_name;
                        r["制单人员ID"] = str_id;
                    }
 

                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_checkSaveZLData");
                throw new Exception(ex.Message);
            }
        }

        //数据的保存
#pragma warning disable IDE1006 // 命名样式
        private void fun_SaveData()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {   //制令主表
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("制令保存");
                try
                {
                    string sql = "select * from 生产记录生产制令表 where 1<>1";
                    SqlCommand cmm = new SqlCommand(sql, conn, ts);
                    SqlDataAdapter da = new SqlDataAdapter(cmm);
                    new SqlCommandBuilder(da);
                    da.Update(dt_proZL);
                    //制令明细表
                    sql = "select * from 生产记录生产制令子表 where 1<>1";
                    cmm = new SqlCommand(sql, conn, ts);

                    da = new SqlDataAdapter(cmm);
                    new SqlCommandBuilder(da);
                    da.Update(dt_proZLdetail);
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
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region   界面操作

        //刷新操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_loadsczlMain();
                fun_load已生效制令();
                fun_searchMaterial();
                //button2.Text = "显示所有";
                gv_已生效制令.ViewCaption = "未排单生效制令";

                if (gv_未生效制令.RowCount > 0)
                {
                    gv_未生效制令.GetDataRow(0)["选择"] = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_AddNewRow();
                gv_未生效制令.Focus();
                gv_未生效制令.FocusedRowHandle = gv_未生效制令.LocateByDisplayText(0, gridColumn2, "");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //删除操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dt_proZL == null || dt_proZL.Rows.Count <= 0)
                    throw new Exception("没有生产制令可以删除！");
                DataRow r = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                if (r.RowState != DataRowState.Added)
                {
                    if (r["生产制令类型"].ToString() == "MRP类型")
                        throw new Exception("MRP类型的生产制令是不允许删除的！");
                    if (MessageBox.Show(string.Format("请确定要删除生产制令单号为\"{0}\"的生产制令吗？", r["生产制令单号"].ToString()), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {   //删除该明细
                        foreach (DataRow r1 in dt_proZLdetail.Rows)
                        {
                            r1.Delete();
                        }
                        r.Delete();
                    }
                }
                else
                {
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                MessageBox.Show("删除失败,刷新重试");
            }
        }

        //保存操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_checkSaveZLData();
                fun_SaveData();
                int index = 0;
                int x = 0;

                if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                {
                    index = gv_未生效制令.FocusedRowHandle;

                }
                else
                {
                    index = gv_已生效制令.FocusedRowHandle;
                    x = 1;
                }
                barLargeButtonItem1_ItemClick(null, null);
                if (x == 0)
                {
                    gv_未生效制令.FocusedRowHandle = index;
                    gv_未生效制令.SelectRow(index);
                }
                else
                {
                    gv_已生效制令.FocusedRowHandle = index;
                    gv_已生效制令.SelectRow(index);

                }

                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //关闭界面
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (MessageBox.Show(string.Format("是否确认关闭此界面"), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                ERPorg.Corg.FlushMemory();
                CPublic.UIcontrol.ClosePage();
            }
        }

        #endregion

        #region  生效制令

        //选择生效的制令
#pragma warning disable IDE1006 // 命名样式
        private void fun_choseZLSX()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                
                dt_SXZL = dt_proZL.Clone();
                foreach (DataRow r in dt_proZL.Rows)
                {
                    if (r["选择"].Equals(true))
                    {

                        if (r.RowState == DataRowState.Added)
                        {
                            throw new Exception(string.Format("勾选的生产制令单号\"{0}\",是新增的，尚未保存，如要生效，请先保存！", r["生产制令单号"].ToString()));
                        }
                        dt_SXZL.Rows.Add(r.ItemArray);

                    }
                }

                //}
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_choseZLSX");
                throw new Exception(ex.Message);
            }
        }

        //检查生效的制令的有效性
#pragma warning disable IDE1006 // 命名样式
        private void fun_checkZLSX()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {


                if (dt_SXZL.Rows.Count <= 0)
                    throw new Exception("请勾选需要生效的生产制令单！");
                //检查勾选的是否有明细,无明细是不能生效的。
                //foreach (DataRow r in dt_SXZL.Rows)
                //{
                //    DataRow[] dr = dt_proZLdetail.Select(string.Format("生产制令单号='{0}'", r["生产制令单号"].ToString()));
                //    if (dr.Length <= 0)
                //        throw new Exception(string.Format("生产制令单号\"{0}\"，无制令明细，不可生效！", r["生产制令单号"].ToString()));
                //}
                //循环制令子表检测有没有新增的没保存的
                string str = "";
                //20-1-8 
                string ss = "";
                ERPorg.Corg cg = new ERPorg.Corg();
                foreach (DataRow r in dt_proZLdetail.Rows)
                {
                    if (r.RowState == DataRowState.Added)
                        throw new Exception(string.Format("生产制令单号\"{0}\"中的销售订单明细号\"{1}\"是新增的,\n请先执行保存操作,或者删除明细操作后，再生效", r["生产制令单号"].ToString(), r["销售订单明细号"].ToString()));
                }
                foreach (DataRow r in dt_proZL.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;

                    if (r["选择"].Equals(true))
                    {
                        if (r["预完工日期"] == null)
                            r["预完工日期"] = r["预计完工日期"];
                        if (r["预计完工日期"] == null)
                            r["预计完工日期"] = r["预完工日期"];
                    }
                }

                //物料是否有效
                foreach (DataRow r in dt_SXZL.Rows)
                {
                    if (r["生产车间"].ToString() == "") throw new Exception("生产车间为空");

                    if (r["预计完工日期"].ToString() == "")
                        throw new Exception("请填写预完工日期");
                    //if (r["预完工日期"].ToString() == "")
                    //    throw new Exception("请填写预完工日期");

                    if (r["班组ID"].ToString() == "")
                        throw new Exception("请选择班组");
                    //物料是否有效
                    if (r["仓库号"].ToString() == "")
                        throw new Exception("有制令的生产入库仓库没有值请检查确认后录入");

                    string sql_物料是否有效 = string.Format("select 物料编码 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                    DataTable dt_基础物料 = new DataTable();
                    dt_基础物料 = CZMaster.MasterSQL.Get_DataTable(sql_物料是否有效, CPublic.Var.strConn);
                    if (dt_基础物料.Rows.Count == 0)
                    {
                        throw new Exception(string.Format("物料'{0}'无效，基础数据物料信息表中不存在该物料信息", r["物料编码"].ToString()));
                    }
                    str = str + StockCore.StockCorer.fun_flag(r["物料编码"].ToString(), false);
                    bool bl_停产 = cg.determ_stop_product(r["物料编码"].ToString());
                    if (bl_停产)
                    {
                        if (ss != "") ss += "," + r["物料编码"].ToString();
                        else ss += r["物料编码"].ToString();
                    }
                }
                if (ss != "")
                {
                    ss += "子项中有已停产或将停产物料,是否确认继续生效制令";
                    if (MessageBox.Show(ss, "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    { }
                    else
                    {
                        throw new Exception("已取消");
                    }
                }
                if (str.Trim() != "")
                {
                    throw new Exception(str);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_checkZLSX");
                throw new Exception(ex.Message);
            }

        }

        //生效选择的制令
#pragma warning disable IDE1006 // 命名样式
        private void fun_Shengxiao()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataTable t = new DataTable();
                dt_proZLdetail = new DataTable();
                DataView dv = new DataView(dt_proZL);
                dv.RowFilter = "选择=1";
                DataTable dtx = dv.ToTable();
                DataTable dt_billofM = new DataTable();
                string s = "select  产品编码,子项编码  from 基础数据物料BOM表 ";
                DataTable dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                foreach (DataRow r in dtx.Rows)
                {
                    //if (r["选择"].Equals(true))
                    //{
                    string sql = string.Format(@"select  zl.*,smx.物料编码 as 销售物料编码,smx.物料名称 as 销售产品名称,smx.规格型号 as 销售产品型号 from 生产记录生产制令子表 zl
                     left join 销售记录销售订单明细表 smx  on zl.销售订单明细号 = smx.销售订单明细号 where 生产制令单号='{0}'", r["生产制令单号"].ToString());
                    using (SqlDataAdapter da_1 = new SqlDataAdapter(sql, strconn))
                    {
                        da_1.Fill(dt_proZLdetail);

                        DataRow[] x = dt_proZLdetail.Select(string.Format("生产制令单号='{0}'", r["生产制令单号"].ToString()));
                        if (x.Length > 0)
                        {
                            x[0]["计划确认日期"] = r["预完工日期"];
                        }
                    }
                    dt_billofM = ERPorg.Corg.billofM(dt_billofM, r["物料编码"].ToString(), false, dt_bom);
                    //}
                }
                //所有待检验物料
                s = @"select * from 采购记录采购送检单明细表    where 检验完成=0  and 作废 = 0";
                DataTable dt_待检 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                foreach (DataRow dr in dt_待检.Rows)
                {
                    if (dt_billofM.Select(string.Format("子项编码='{0}'", dr["物料编码"])).Length > 0)
                    {
                        dr["是否急单"] = true;
                    }
                }

                foreach (DataRow r in dt_proZLdetail.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    string str = string.Format("select * from 销售记录销售订单明细表 where 生效=1 and 关闭=0 and 销售订单明细号='{0}'", r["销售订单明细号"].ToString());
                    using (SqlDataAdapter a = new SqlDataAdapter(str, strconn))
                    {
                        a.Fill(t);
                        DataRow[] xx = t.Select(string.Format("销售订单明细号='{0}'", r["销售订单明细号"].ToString()));
                        if (xx.Length > 0)
                        {

                            xx[0]["计划确认日期"] = r["计划确认日期"];
                        }
                    }
                }
              
                //计划人员关联物料表(如果已有则不加)
                DataTable dt_关联物料 = fun_关联物料(dtx);

                Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
                //生效时给相应的字段赋值
                if (flag == false)
                {
                    foreach (DataRow r in dt_proZL.Rows)
                    {
                        if (r["选择"].Equals(true))
                        {
                            r["生效"] = 1;
                            r["生效人员ID"] = CPublic.Var.LocalUserID;
                            r["生效人员"] = CPublic.Var.localUserName;
                            r["生效日期"] = CPublic.Var.getDatetime();
                            r["预计完工日期"] = r["预完工日期"];
                        }
                    }
                    dic.Add("生产记录生产制令表", dt_proZL);
                }
            
                dic.Add("生产记录生产制令子表", dt_proZLdetail);
                dic.Add("销售记录销售订单明细表", t);
                dic.Add("计划人员关联物料表", dt_关联物料);
                dic.Add("采购记录采购送检单明细表", dt_待检);


                ERPorg.Corg cg = new ERPorg.Corg();
                cg.save(dic);
                #region 20-6-17 替换成上面的 已注释
                //SqlDataAdapter da;
                //SqlConnection conn = new SqlConnection(strconn);
                //conn.Open();
                //SqlTransaction ts = conn.BeginTransaction("ZLSX");
                //SqlCommand cmd2 = new SqlCommand("select * from 销售记录销售订单明细表 where 1<>1", conn, ts);

                ////SqlCommand cmd_计划池 = new SqlCommand("select * from 生产记录生产计划表 where 1<>1", conn, ts);
                //SqlCommand cmd = new SqlCommand("select * from 生产记录生产制令表 where 1<>1", conn, ts);
                //SqlCommand cmd1 = new SqlCommand("select * from 生产记录生产制令子表 where 1<>1", conn, ts);
                //SqlCommand cmd3 = new SqlCommand("select * from 计划人员关联物料表 where 1<>1", conn, ts);

                //try
                //{
                //    if (flag == false)
                //    {
                //        da = new SqlDataAdapter(cmd);
                //        new SqlCommandBuilder(da);
                //        da.Update(dt_proZL);


                //    }
                //    da = new SqlDataAdapter(cmd1);
                //    new SqlCommandBuilder(da);
                //    da.Update(dt_proZLdetail);

                //    da = new SqlDataAdapter(cmd2);
                //    new SqlCommandBuilder(da);
                //    da.Update(t);
                //    da = new SqlDataAdapter(cmd3);
                //    new SqlCommandBuilder(da);
                //    da.Update(dt_关联物料);

                //    cmd3 = new SqlCommand("select * from 采购记录采购送检单明细表 where 1<>1", conn, ts);
                //    da = new SqlDataAdapter(cmd3);
                //    new SqlCommandBuilder(da);
                //    da.Update(dt_待检);

                //    ts.Commit();
                //}
                //catch
                //{
                //    ts.Rollback();
                //    throw new Exception("生效失败");
                //}

                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //throw new Exception(ex.Message);
            }
        }

        //生效制令  计划人员关联物料表如果没有该计划员关联这条制令 存记录
#pragma warning disable IDE1006 // 命名样式
        private DataTable fun_关联物料(DataTable dt_mx)
#pragma warning restore IDE1006 // 命名样式
        {
            DataTable dt = new DataTable();
            string str_工号 = CPublic.Var.LocalUserID;
            foreach (DataRow dr in dt_mx.Rows)
            {
                string sql = string.Format("select  *  from 计划人员关联物料表 where 工号='{0}' and 物料编码='{1}'", str_工号, dr["物料编码"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    da.Fill(dt);
                    DataRow[] r = dt.Select(string.Format("工号='{0}' and 物料编码='{1}'", str_工号, dr["物料编码"].ToString()));
                    if (r.Length == 0)
                    {
                        DataRow rx = dt.NewRow();
                        rx["工号"] = str_工号;
                        rx["物料编码"] = dr["物料编码"].ToString();

                        dt.Rows.Add(rx);
                    }
                }
            }
            return dt;
        }

      
        //生效操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                this.ActiveControl = null;
                if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                {
                    gv_未生效制令.CloseEditor();
                    this.BindingContext[dt_proZL].EndCurrentEdit();
                    fun_choseZLSX();
                    fun_checkZLSX();
                    //dt_计划池 = new DataTable();
                    // dt_计划池 = fun_减计划池();  // 2017/6-19弃用
                    fun_Shengxiao();
                    //减去 计划池里对应的量
                    MessageBox.Show("生效成功");
                    barLargeButtonItem1_ItemClick(null, null);
                }
                else //已生效制令 转工单
                {
                    gv_已生效制令.CloseEditor();
                    this.BindingContext[dt_proZLysx].EndCurrentEdit();
                    DataView dv = new DataView(dt_proZLysx);
                    dv.RowFilter = "选择=1";
                    if (dv.Count == 0) throw new Exception("未选择任何明细");
                    ERPorg.Corg cg = new ERPorg.Corg();
                    string ss = "";
                    foreach(DataRow dr in dv.ToTable().Rows)
                    {
                        bool bl_停产 = cg.determ_stop_product(dr["物料编码"].ToString());
                        if (bl_停产)
                        {
                            if (ss != "") ss += "," + dr["物料编码"].ToString();
                            else ss += dr["物料编码"].ToString();
                        }
                    }
                    if(ss!="")
                    {
                        ss += "子项中有已停产或将停产物料,是否确认继续转工单";
                        if (MessageBox.Show(ss, "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        { }
                        else
                        {
                            throw new Exception("已取消");
                        }
                    }
                    if (MessageBox.Show(string.Format("确认选择{0}条制令自动转成已生效工单", dv.Count)
                    , "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        string s = "select  * from 生产记录生产工单表 where 1=2";
                        DataTable dt_save工单 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                        //直接生成工单 并生效 制令的 已排单数量=制令数量
                        fun_check_转工单(dv.ToTable());
                        DataTable dt_已生效制令 = dv.ToTable();
                        dt_已生效制令.AcceptChanges();
                        foreach (DataRow dr in dt_已生效制令.Rows)
                        {
                            //if (Convert.ToBoolean(dr["选择"]))
                            //{
                            dr["已排单数量"] = dr["制令数量"];
                            dr["未排单数量"] = 0;
                            if (dr["预完工日期"] == null)
                                dr["预完工日期"] = dr["预计完工日期"];
                            if (dr["预计完工日期"] == null)
                                dr["预计完工日期"] = dr["预完工日期"];
                            DataRow r = fun_自动转工单(dr);
                            dt_save工单.ImportRow(r);
                            //}
                        }

                        DataTable temp = dt_save工单.Copy();
                        DataSet ds = fun_自动生效工单(temp);
                        //19-11-21 有些物料仓库物料信息表中没有记录 需要添加进去 如果没有影响未领 导致计划池不准
                        DataTable dt_kc = StockCore.StockCorer.KCRecord(temp);

                        DataSet dss = null;
                        if (CPublic.Var.localUser部门名称 != "生产二厂") //19-11-14正式库还没启用 现有是研发部内部电脑做的服务器 二厂连不上
                        {
                            ERPorg.Corg xx = new ERPorg.Corg();
                            dss = xx.fun_SN(dt_save工单);
                        }



                        string sql_baocun = "select * from 生产记录生产工单表  where 1<>1";
                        string sql_制令数量 = "select * from 生产记录生产制令表 where 1<>1";
                        string sql_待主 = "select * from 生产记录生产工单待领料主表 where 1<>1";
                        string sql_待明细 = "select * from 生产记录生产工单待领料明细表 where 1<>1";
                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("工单生效");
                        try
                        {
                            SqlCommand cmm_0 = new SqlCommand(sql_baocun, conn, ts);
                            SqlCommand cmm_1 = new SqlCommand(sql_制令数量, conn, ts);
                            SqlCommand cmm_2 = new SqlCommand(sql_待主, conn, ts);
                            SqlCommand cmm_3 = new SqlCommand(sql_待明细, conn, ts);

                            SqlDataAdapter da_cun = new SqlDataAdapter(cmm_0);
                            SqlDataAdapter da_制令数量 = new SqlDataAdapter(cmm_1);
                            SqlDataAdapter da_待主 = new SqlDataAdapter(cmm_2);
                            SqlDataAdapter da_待明细 = new SqlDataAdapter(cmm_3);

                            new SqlCommandBuilder(da_cun);
                            new SqlCommandBuilder(da_制令数量);
                            new SqlCommandBuilder(da_待主);
                            new SqlCommandBuilder(da_待明细);
                            //da_cun.Update(dt_save工单);
                            //da_cun.Update(dss.Tables[2]); 

                            da_制令数量.Update(dt_已生效制令);
                            da_待主.Update(ds.Tables[0]);
                            da_待明细.Update(ds.Tables[1]);

                            if (dss != null)
                            {
                                new SqlCommandBuilder(da_cun);
                                da_cun.Update(dss.Tables[2]);

                                sql_baocun = "select * from Print_ShareLockInfo where 1=2 ";
                                cmm_0 = new SqlCommand(sql_baocun, conn, ts);
                                da_cun = new SqlDataAdapter(cmm_0);
                                new SqlCommandBuilder(da_cun);
                                da_cun.Update(dss.Tables[1]);
                            }
                            else
                            {
                                new SqlCommandBuilder(da_cun);
                                da_cun.Update(dt_save工单);
                            }
                            sql_baocun = "select * from 仓库物料数量表 where 1=2 ";
                            cmm_0 = new SqlCommand(sql_baocun, conn, ts);
                            da_cun = new SqlDataAdapter(cmm_0);
                            new SqlCommandBuilder(da_cun);
                            da_cun.Update(dt_kc);



                            ts.Commit();
                            s = "exec FourNum ";
                            CZMaster.MasterSQL.ExecuteSQL(s, strconn);
                            ///2019-10-16  这边要保存另一个数据库  目前我不知道怎么两个数据用类似事务的方式一起保存 
                            if (dss != null)
                            {
                                string str_BQ = CPublic.Var.geConn("BQ");
                                CZMaster.MasterSQL.Save_DataTable(dss.Tables[0], "ShareLockInfo", str_BQ);
                            }
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw new Exception("工单生效失败");
                        }
                        int index = 0;
                        int x = 0;

                        if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                        {
                            index = gv_未生效制令.FocusedRowHandle;
                        }
                        else
                        {
                            index = gv_已生效制令.FocusedRowHandle;
                            x = 1;
                        }
                        //MessageBox.Show("转工单成功");
                        barLargeButtonItem1_ItemClick(null, null);
                        if (MessageBox.Show("转工单成功,是否跳转至工单界面？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            ERPproduct.frm工单生效选择 frm = new frm工单生效选择();
                            CPublic.UIcontrol.Showpage(frm, "工单生效界面");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        #endregion


        private void fun_check_转工单(DataTable dt)
        {
            foreach (DataRow r in dt_proZLdetail.Rows)
            {
                if (r.RowState == DataRowState.Added)
                    throw new Exception(string.Format("生产制令单号\"{0}\"中的销售订单明细号\"{1}\"是新增的,\n请先执行保存操作,或者删除明细操作后，再生效", r["生产制令单号"].ToString(), r["销售订单明细号"].ToString()));
            }
            foreach (DataRow dr in dt.Rows)
            {
                //if(dr["版本备注"].ToString().Trim()=="")
                //    throw new Exception(string.Format("制令{0} 无版本备注不可转工单",dr["生产制令单号"].ToString()));
                if (Convert.ToDecimal(dr["已排单数量"]) > 0)
                    throw new Exception("已有已排单数量的制令不可使用此功能");
                if (Convert.ToBoolean(dr["关闭"]) || Convert.ToBoolean(dr["完成"]))
                    throw new Exception(string.Format("制令:{0}已关闭或已完成", dr["生产制令单号"].ToString()));
                if (dr["生产车间"].ToString() == "") throw new Exception("生产车间为空");
                if (dr["预完工日期"].ToString() == "")
                    throw new Exception("请填写预完工日期");
                else
                {
                    DateTime t = CPublic.Var.getDatetime().Date;
                    if (t > Convert.ToDateTime(dr["预完工日期"]).Date)
                    {
                        throw new Exception("预完工日期不可小于当天");
                    }

                }
                if (dr["班组ID"].ToString() == "")
                    throw new Exception("请选择班组");
                //物料是否有效
                if (dr["仓库号"].ToString() == "")
                    throw new Exception("有制令的生产入库仓库没有值请检查确认后录入");
                string sql_物料是否有效 = string.Format("select 物料编码 from 基础数据物料信息表 where 物料编码 = '{0}'", dr["物料编码"].ToString());
                DataTable dt_基础物料 = new DataTable();
                dt_基础物料 = CZMaster.MasterSQL.Get_DataTable(sql_物料是否有效, CPublic.Var.strConn);
                if (dt_基础物料.Rows.Count == 0)
                {
                    throw new Exception(string.Format("物料'{0}'无效，基础数据物料信息表中不存在该物料信息", dr["物料编码"].ToString()));
                }
                string sq = string.Format(@"select bz.*,bx.审核 from   基础数据BOM修改主表  bz 
                 left join   单据审核申请表 bx on bz.BOM修改单号=bx.关联单号   
                 where bz.产品编码 = '{0}' and bx.审核=0 and bz.作废=0 and bx.作废=0", dr["物料编码"].ToString());

                DataTable dt_ss = CZMaster.MasterSQL.Get_DataTable(sq, strconn);
                if (dt_ss.Rows.Count > 0)
                {
                    if (MessageBox.Show(string.Format("{0}物料有正在修改的BOM未审核，请确认继续？", dr["物料编码"].ToString())
                        , "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        throw new Exception("已取消");
                    }
                }

                string sx = string.Format(@"select  count(*)xx from 基础数据物料BOM表 a
                                   left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
                                   where  base.停用 =1  and base.物料编码='{0}'  ", dr["物料编码"].ToString());
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(sx, strconn);
                if (Convert.ToInt32(temp.Rows[0]["xx"]) > 0)
                {
                    throw new Exception(string.Format("物料:'{0}'BOM中存在停用物料,请确认", dr["物料编码"].ToString()));
                }
            }

        }


        #region 明细的操作

        //明细回传值处理
#pragma warning disable IDE1006 // 命名样式
        private void fun_detailDeal(DataTable dt, string danhao)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                dt_proZLdetail = dt_proZLdetail.Clone();
                //勾选返回的dt
                foreach (DataRow r in dt.Rows)
                {
                    DataRow r_zlzb = dt_proZLdetail.NewRow();
                    r_zlzb["GUID"] = System.Guid.NewGuid();
                    r_zlzb["生产制令单号"] = danhao;
                    r_zlzb["销售订单明细号"] = r["销售订单明细号"];
                    r_zlzb["销售订单号"] = r["销售订单号"];
                    r_zlzb["物料编码"] = r["物料编码"];


                    r_zlzb["物料名称"] = r["物料名称"];
                    r_zlzb["客户"] = r["客户"];
                    r_zlzb["送达日期"] = r["送达日期"];
                    r_zlzb["规格型号"] = r["规格型号"];
                    r_zlzb["图纸编号"] = r["图纸编号"];
                    r_zlzb["数量"] = r["数量"];
                    r_zlzb["计量单位"] = r["计量单位"];
                    r_zlzb["销售备注"] = r["备注"];

                    dt_proZLdetail.Rows.Add(r_zlzb);
                }
                dt_dispalymx = dt_proZLdetail.Copy();

                gc_关联订单.DataSource = dt_dispalymx;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_detailDeal");
                throw new Exception(ex.Message);
            }
        }

        //明细的新增
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dt_proZL.Rows.Count <= 0)
                    throw new Exception("无生产制令，不可新增明细！");
                DataRow r = (this.BindingContext[dt_proZL].Current as DataRowView).Row;
                if (r.RowState == DataRowState.Added)
                    throw new Exception("你选中的生产制令是新增的，还没有保存，请先保存生产制令！");
                //// fm关联销售明细选择 fm = new fm关联销售明细选择(dt_proZLdetail, r["物料编码"].ToString(),r["生产制令单号"].ToString());
                //fm关联销售明细选择 fm = new fm关联销售明细选择(dt_dispalymx, r["物料编码"].ToString(), r["生产制令单号"].ToString());

                //fm.ShowDialog();
                //if (fm.dt != null)
                //{
                //    dt_dispalymx = fm.dt;
                //    gc_关联订单.DataSource = dt_dispalymx;
                //}

                //20-6-12 更新 
                //这边是跳过去后重新选 有就更新数据 没有就新增  删除让他在这边界面自己删除

                Assembly outerAsm1 = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "ERPStock.dll"));
                Type outerForm1 = outerAsm1.GetType("ERPStock.fm空窗体", false);
                Form fm = (Form)Activator.CreateInstance(outerForm1);

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ReworkMould.dll")));
                Type outerForm = outerAsm.GetType("ReworkMould.ui_选择关联销售单", false);
                object[] drr = new object[1];
                drr[0] =r["物料编码"].ToString();
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;


          
                fm.Controls.Add(ui);
                ui.Dock = DockStyle.Fill;
                fm.Text = "关联销售明细";
                fm.Size = new System.Drawing.Size(1200, 550);
                fm.StartPosition = FormStartPosition.CenterScreen;
                fm.ShowDialog();

                bool flag = Convert.ToBoolean(outerForm.GetField("flag").GetValue(ui));
                DataTable dt_xsmx = outerForm.GetField("dt_xsmx").GetValue(ui) as DataTable;
                //DataTable dt_ydd_mx = outerForm.GetField("dt_ydd_mx").GetValue(ui) as DataTable;

                if (flag && dt_xsmx.Rows.Count > 0)
                {
                    foreach (DataRow dr_mx in  dt_xsmx.Rows)
                    {
                        DataRow[] dr_p = dt_dispalymx.Select(string.Format("销售订单明细号 = '{0}' ", dr_mx["销售订单明细号"].ToString()  ));
                        if (dr_p.Length > 0)
                        {
                            dr_p[0]["物料编码"] = dr_mx["物料编码"];
                            dr_p[0]["销售物料编码"] = dr_mx["物料编码"];
                            dr_p[0]["销售产品名称"] = dr_mx["物料名称"];
                            dr_p[0]["销售产品型号"] = dr_mx["规格型号"];


                            dr_p[0]["物料名称"] = dr_mx["物料名称"];
                            dr_p[0]["客户"] = dr_mx["客户"];
                            dr_p[0]["规格型号"] = dr_mx["规格型号"];
                            dr_p[0]["送达日期"] = Convert.ToDateTime(dr_mx["预计发货日期"]);
                            dr_p[0]["数量"] = Convert.ToDecimal(dr_mx["数量"]);
                            dr_p[0]["销售备注"] = dr_mx["备注"];
                        }
                        else
                        {
                            DataRow dr_1 = dt_dispalymx.NewRow();
                            dt_dispalymx.Rows.Add(dr_1);
                            dr_1["GUID"] = System.Guid.NewGuid() ;
                            dr_1["生产制令单号"] = r["生产制令单号"];
                            dr_1["销售物料编码"] = dr_mx["物料编码"];
                            dr_1["销售产品名称"] = dr_mx["物料名称"];
                            dr_1["销售产品型号"] = dr_mx["规格型号"];
                            dr_1["销售订单明细号"] = dr_mx["销售订单明细号"];
                            dr_1["销售订单号"] = dr_mx["销售订单号"];
                            dr_1["物料编码"] = dr_mx["物料编码"];
                            dr_1["物料名称"] = dr_mx["物料名称"];
                            dr_1["客户"] = dr_mx["客户"];
                            dr_1["规格型号"] = dr_mx["规格型号"];
                            dr_1["送达日期"] = Convert.ToDateTime(dr_mx["预计发货日期"]);
                            dr_1["数量"] = Convert.ToDecimal(dr_mx["数量"]);
                            dr_1["销售备注"] = dr_mx["备注"];
                        }
 
                    }
                }
                gc_关联订单.DataSource = dt_dispalymx;
 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //明细的删除
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dt_dispalymx == null || dt_dispalymx.Rows.Count <= 0)
                    throw new Exception("无明细可以删除，请先新增明细！");
                //DataRow r = (this.BindingContext[dt_dispalymx].Current as DataRowView).Row;
                DataRow r = gv_关联订单.GetDataRow(gv_关联订单.FocusedRowHandle);
                if (MessageBox.Show(string.Format("你确定要删除明细号为\"{0}\"的明细吗？", r["销售订单明细号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow[] dr = dt_dispalymx.Select(string.Format("销售订单明细号='{0}'", r["销售订单明细号"].ToString()));
                    if (dr.Length > 0)
                    {
                        dr[0].Delete();
                    }
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region 行变化，着色时间，行点击事件，右键查看BOM


#pragma warning disable IDE1006 // 命名样式
        private void gv_sczlmain_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (gv_未生效制令.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                if (gv_未生效制令.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "加急")
                {
                    e.Appearance.BackColor = Color.Red;
                    e.Appearance.BackColor2 = Color.Red;
                }
                if (gv_未生效制令.GetRowCellValue(e.RowHandle, "加急状态").ToString() == "急")
                {
                    e.Appearance.BackColor = Color.Yellow;
                    e.Appearance.BackColor2 = Color.Yellow;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
        }
        // 2.29 根据要求 
#pragma warning disable IDE1006 // 命名样式
        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
                if (dr == null) return;
                string sql = string.Format(@"  select  zl.*,smx.物料编码 as 销售物料编码,smx.物料名称 as 销售产品名称,smx.规格型号 as 销售产品型号,smx.备注,smx.表头备注 from 生产记录生产制令子表 zl
            left join [V_制令关联] smx  on zl.销售订单明细号=smx.销售订单明细号
               where 生产制令单号='{0}'", dr["生产制令单号"].ToString().Trim());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gc_关联订单.DataSource = dt;
                }
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc_已生效制令, new Point(e.X, e.Y));
                    gv_已生效制令.CloseEditor();
                    this.BindingContext[dt_proZLysx].EndCurrentEdit();


                }
            }
            catch  
            {
 
            }
         
        }
#pragma warning disable IDE1006 // 命名样式
        private void gv_sczlmain_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
            if (dr != null)
            {
                //if (gv_未生效制令.FocusedColumn.FieldName=="选择")
                //{
                //    if (dr["选择"] == null|| dr["选择"]==DBNull.Value) dr["选择"] = true;
                //    else dr["选择"] = !Convert.ToBoolean(dr["选择"]);
                //}

                string sql = string.Format(@"select  zl.*,smx.物料编码 as 销售物料编码,smx.物料名称 as 销售产品名称,smx.规格型号 as 销售产品型号,smx.备注,smx.表头备注  from 生产记录生产制令子表 zl
           left join [V_制令关联] smx  on zl.销售订单明细号=smx.销售订单明细号
           where  生产制令单号='{0}'", dr["生产制令单号"].ToString().Trim());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    dt_dispalymx = new DataTable();
                    da.Fill(dt_dispalymx);
                    gc_关联订单.DataSource = dt_dispalymx;
                }

                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip2.Show(gc_未生效制令, new Point(e.X, e.Y));
                    gv_未生效制令.CloseEditor();
                    this.BindingContext[dt_proZL].EndCurrentEdit();

                }
                if (dr.RowState != DataRowState.Added)// 不是新增的行 
                {


                    foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_未生效制令.Columns)
                    {
                        if (dc.FieldName != "选择" &&  dc.FieldName != "计划自用备注" && dc.FieldName != "备注" && dc.FieldName != "预计完工日期" && dc.FieldName != "预完工日期" && dc.FieldName != "加急状态" && dc.FieldName != "反馈备注" && dc.FieldName != "班组ID")
                        {
                            dc.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            dc.OptionsColumn.AllowEdit = true;
                        }
                    }
                }
                else //新增的行 
                {
                    foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_未生效制令.Columns)
                    {
                        if (dc.FieldName != "预完工日期" && dc.FieldName != "预计完工日期" && dc.FieldName != "选择" && dc.FieldName != "备注" && dc.FieldName != "反馈备注"
                           && dc.FieldName != "计划自用备注"  && dc.FieldName != "制令数量" && dc.FieldName != "物料编码" && dc.FieldName != "加急状态" && dc.FieldName != "班组ID")
                        {
                            //以上字段可编辑
                            dc.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            dc.OptionsColumn.AllowEdit = true;
                        }
                    }
                }
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void gv_sczlmain_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow r = gv_未生效制令.GetDataRow(e.RowHandle);
            if (r == null)
            {
                return;
            }
            else if (r["加急状态"].ToString().Trim() == "急")
            {
                e.Appearance.BackColor = Color.Yellow;
            }
            else if (r["加急状态"].ToString().Trim() == "加急")
            {
                e.Appearance.BackColor = Color.Red;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void gridView2_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow r = gv_已生效制令.GetDataRow(e.RowHandle);
            if (r == null)
            {
                return;
            }
            else if (r["加急状态"].ToString().Trim() == "急")
            {
                e.Appearance.BackColor = Color.Yellow;
            }
            else if (r["加急状态"].ToString().Trim() == "加急")
            {
                e.Appearance.BackColor = Color.Red;
            }
        }

        private void 查看物料BOMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow r = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
            decimal dec;
            if (r["制令数量"] != DBNull.Value && r["制令数量"].ToString() != "")
            {
                dec = Convert.ToDecimal(r["制令数量"].ToString());
            }
            else
            {
                dec = 1;
            }
            ERPproduct.UI物料BOM详细数量 frm = new UI物料BOM详细数量(r["物料编码"].ToString().Trim(), dec, r["生产制令单号"].ToString());
            CPublic.UIcontrol.AddNewPage(frm, "详细数量");
        }
        #endregion

        #region 关闭制令
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                this.ActiveControl = null;
                DateTime t1 = CPublic.Var.getDatetime();
                //dt_proZL.Columns.Add("关闭制令");
                //第一步，制令关闭
                if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                {
                    if (MessageBox.Show(string.Format("是否确认关闭该制令"), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                        if (dr.RowState == DataRowState.Added)
                        {
                            dr.Delete();
                        }
                        else
                        {
                            dr["关闭"] = true;
                            dr["关闭日期"] = t1;
                            dr["关闭人员ID"] = CPublic.Var.LocalUserID;
                            dr["关闭人员"] = CPublic.Var.localUserName;
                            fun_保存并刷新过(false);
                            dt_proZL.Rows.Remove(dr);
                        }
                    }
                }
                else
                {
                    if (xtraTabControl1.SelectedTabPage == xtraTabPage2)
                    {
                        DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
                        //先判断有没有完成
                        if (Convert.ToBoolean(dr["完成"]))
                        {
                            throw new Exception("该制令已经完成，不能关闭");
                        }
                        if (Convert.ToBoolean(dr["关闭"]))
                        {
                            throw new Exception("该制令已经关闭，不能关闭");
                        }
                        //再判断有没有转过工单，关闭还没转工单的数量 
                        if (Convert.ToDecimal(dr["已排单数量"]) == 0)
                        {
                            dr["关闭"] = true;
                            dr["关闭日期"] = t1;
                            dr["关闭人员ID"] = CPublic.Var.LocalUserID;
                            dr["关闭人员"] = CPublic.Var.localUserName;
                            fun_保存并刷新过(true);
                        }
                        else  //已转过工单 关闭
                        {
                            dr["完成"] = true;
                            dr["完成日期"] = t1;

                            fun_保存并刷新过(true);
                        }

                    }
                }

                // barLargeButtonItem1_ItemClick(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bl">表示是已生效制令还是未生效制令</param>
        private void fun_保存并刷新过(Boolean bl)
#pragma warning restore IDE1006 // 命名样式
        {
            SqlDataAdapter da;
            string sql = "select * from 生产记录生产制令表 where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            if (bl)
            {
                da.Update(dt_proZLysx);
                DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
                StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
            }
            else
            {
                da.Update(dt_proZL);
                DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_刷新受订量(DataRow dr)
#pragma warning restore IDE1006 // 命名样式
        {
            if (xtraTabControl1.SelectedTabPage == xtraTabPage2)
            {
                string sql_查找BOM = string.Format("select * from 基础数据物料BOM表 where 产品编码 = '{0}' and 主辅料 = '主料'", dr["物料编码"].ToString());
                DataTable dt_查找BOM = new DataTable();
                SqlDataAdapter da_查找BOM = new SqlDataAdapter(sql_查找BOM, strconn);
                da_查找BOM.Fill(dt_查找BOM);

                string sql_制令对应未领量 = string.Format("select * from 生产记录生产制令单待领料表 where 生产制令单号 = '{0}'", dr["生产制令单号"].ToString());
                DataTable dt_制令对应未领量 = new DataTable();
                SqlDataAdapter da_制令对应未领量 = new SqlDataAdapter(sql_制令对应未领量, strconn);
                da_制令对应未领量.Fill(dt_制令对应未领量);

                //待领料表中去掉关闭的量
                foreach (DataRow r in dt_制令对应未领量.Rows)
                {
                    DataRow[] ds = dt_查找BOM.Select(string.Format("子项编码 = '{0}'", r["物料编码"].ToString()));
                    r["未领数量"] = Convert.ToDecimal(r["未领数量"]) - Convert.ToDecimal(ds[0]["数量"]) * Convert.ToDecimal(dr["未排单数量"]);
                    r["关闭"] = true;
                    r["关闭日期"] = CPublic.Var.getDatetime();
                    r["关闭人员ID"] = CPublic.Var.LocalUserID;
                    r["关闭人员"] = CPublic.Var.localUserName;
                }
                new SqlCommandBuilder(da_制令对应未领量);
                da_制令对应未领量.Update(dt_制令对应未领量);

                //刷新未领量
                //foreach (DataRow r in dt_制令对应未领量.Rows)
                //{
                //    // StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(), true);
                //}
            }
        }
        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton3_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //制令明细表

            string sql = "select * from 生产记录生产制令子表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_dispalymx);  //dt_proZLdetail
            dt_dispalymx.AcceptChanges();

            MessageBox.Show("保存成功");
            gv_sczlmain_RowCellClick_1(null, null);

        }

        #region 右键菜单
        private void 查看BOMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow r = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
            decimal dec;
            if (r["制令数量"] != DBNull.Value && r["制令数量"].ToString() != "")
            {
                dec = Convert.ToDecimal(r["制令数量"].ToString());
            }
            else
            {
                dec = 1;
            }
            ERPproduct.UI物料BOM详细数量 frm = new UI物料BOM详细数量(r["物料编码"].ToString().Trim(), dec, r["生产制令单号"].ToString());
            CPublic.UIcontrol.AddNewPage(frm, "详细数量");
        }

        private void 查看过往制令ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow r = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);

            UI查看制令列表 ui = new UI查看制令列表(r["物料编码"].ToString().Trim());
            CPublic.UIcontrol.AddNewPage(ui, "过往制令列表");
        }

        private void 过往通知出库记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow r = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);

            UI查看出库通知明细 ui = new UI查看出库通知明细(r["物料编码"].ToString().Trim());
            CPublic.UIcontrol.AddNewPage(ui, "过往通知出库记录");
        }
        #endregion

        #region 改制工单
        //弃用
        private void 改制工单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    gv_未生效制令.CloseEditor();
            //    this.BindingContext[dt_proZL].EndCurrentEdit();
            //    fun_choseZLSX();
            //    fun_checkZLSX();
            //    DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
            //    frm改制工单 fm = new frm改制工单(dr);
            //    fm.ShowDialog();
            //    if (fm.a.Equals(true))
            //    {
            //        flag = true;
            //        fun_Shengxiao();
            //        MessageBox.Show("生效成功");
            //        barLargeButtonItem1_ItemClick(null, null);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }
        #endregion
#pragma warning disable IDE1006 // 命名样式
        public void fun_check制令(DataRow dr)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format("select * from 生产记录生产制令表 where 生产制令单号='{0}'", dr["生产制令单号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToDecimal(dt.Rows[0]["制令数量"]) != Convert.ToDecimal(dr["制令数量"])
                        || dt.Rows[0]["备注"].ToString() != dr["备注"].ToString() || dt.Rows[0]["预完工日期"].ToString() != dr["预完工日期"].ToString())
                    {
                        throw new Exception("制令已被修改，刷新后重试");
                    }
                }
                else
                {
                    throw new Exception("该制令已删除,刷新后重试");
                }

            }

        }
        private void 修改制令ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                foreach (DataRow r in dt_proZL.Rows)
                {
                    if (r.RowState == DataRowState.Added)
                        throw new Exception(string.Format("有新增未保存的制令,先执行保存操作,或者删除明细操作后再修改制令"));
                }
                DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                fun_check制令(dr);
                frm修改制令 fm = new frm修改制令(dr);
                fm.ShowDialog();
                if (fm.flag)
                {
                    for (int i = 0; i < gv_关联订单.RowCount; i++)
                    {

                        gv_关联订单.GetDataRow(i)["计划确认日期"] = dr["预完工日期"];
                    }
                    dr["预计完工日期"] = dr["预完工日期"];
                }
                if (fm.de_现 != 0)
                {
                    dr["制令数量"] = fm.de_现;
                    dr["未排单数量"] = fm.de_现;
                    gv_已生效制令.CloseEditor();
                    this.BindingContext[dt_proZL].EndCurrentEdit();
                    DataTable dt_销售明细 = new DataTable();
                    for (int i = 0; i < gv_关联订单.RowCount; i++)
                    {
                        string str = string.Format("select * from 销售记录销售订单明细表 where 生效=1 and 关闭=0 and 销售订单明细号='{0}'", gv_关联订单.GetDataRow(i)["销售订单明细号"].ToString());
                        using (SqlDataAdapter a = new SqlDataAdapter(str, strconn))
                        {
                            a.Fill(dt_销售明细);
                            DataRow[] xx = dt_销售明细.Select(string.Format("销售订单明细号='{0}'", gv_关联订单.GetDataRow(i)["销售订单明细号"].ToString()));
                            if (xx.Length > 0)
                            {

                                xx[0]["计划确认日期"] = gv_关联订单.GetDataRow(i)["计划确认日期"];
                            }

                        }
                    }
                    SqlDataAdapter dda;
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction xgwzl = conn.BeginTransaction("修改未生效制令");
                    try
                    {

                        SqlCommand cmd2 = new SqlCommand("select * from 销售记录销售订单明细表 where 1<>1", conn, xgwzl);
                        dda = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(dda);
                        dda.Update(dt_销售明细);

                        string sql_1 = "select * from 生产记录生产制令表 where 1<>1";
                        cmd2 = new SqlCommand(sql_1, conn, xgwzl);
                        dda = new SqlDataAdapter(cmd2);
                        new SqlCommandBuilder(dda);
                        dda.Update(dt_proZL);
                        xgwzl.Commit();
                        MessageBox.Show("修改成功");
                    }
                    catch (Exception)
                    {
                        xgwzl.Rollback();
                        throw;
                    }

                }


            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_MouseUp(object sender, MouseEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    int[] dr = gv_未生效制令.GetSelectedRows();
            //    if (dr.Length > 1)
            //    {
            //        for (int i = 0; i < dr.Length; i++)
            //        {
            //            DataRow r = gv_未生效制令.GetDataRow(dr[i]);
            //            if (r["选择"].Equals(true))
            //            {
            //                r["选择"] = 0;

            //            }
            //            else
            //            {
            //                r["选择"] = 1;
            //            }

            //        }
            //        //gridView1.FocusedRowHandle = dr[dr.Length - 1];
            //        gv_未生效制令.MoveBy(dr[dr.Length - 1]);
            //    }
            //}
        }

#pragma warning disable IDE1006 // 命名样式
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //显示所有制令
            gv_已生效制令.ViewCaption = "所有生效制令";

            SqlDataAdapter da;
            string sql = "";
            if (date_前.EditValue != null && date_前.EditValue.ToString() != "" && date_后.EditValue != null && date_后.EditValue.ToString() != "")
            {
                sql = string.Format("and sczl.生效日期 >= '{0}' and sczl.生效日期 <= '{1}'", Convert.ToDateTime(date_前.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(date_后.EditValue).AddDays(1).ToString("yyyy-MM-dd"));
            }
            if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserID == "2101" || CPublic.Var.LocalUserID == "2233" || CPublic.Var.LocalUserID == "4136" || CPublic.Var.LocalUserID == "2106")
            {
                sql = string.Format(@"select sczl.*,a.已转工单数,库存总数,新数据,isnull(aaa.完工数量,0)完工数量,拼板数量   from 生产记录生产制令表 sczl
                                                left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                     left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                                              on sczl.生产制令单号=a.生产制令单号
                                            left join 仓库物料数量表 kc on  kc.物料编码= sczl.物料编码 and sczl.仓库号=kc.仓库号
                                            left join  (select  生产制令单号,sum(完工数量)完工数量  from (
                                            select  生产制令单号, case when 完工=1 then 生产数量 else 部分完工数 end as 完工数量   from 生产记录生产工单表)a   
                                             group by 生产制令单号) aaa  on aaa.生产制令单号 = sczl.生产制令单号
                                                where /*生产记录生产制令表.关闭=0 and*/    sczl.生效日期 >= '2016-12-1'  {0}and sczl.生产制令类型!='研发样品' ", sql);
            }
            else
            {
                sql = string.Format(@"select sczl.*,a.已转工单数,库存总数,新数据,isnull(aaa.完工数量,0)完工数量 ,拼板数量  from 生产记录生产制令表 sczl
            left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
               left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
               on sczl.生产制令单号=a.生产制令单号
              left join 仓库物料数量表 kc on  kc.物料编码= sczl.物料编码 and sczl.仓库号=kc.仓库号
              left join  (select  生产制令单号,sum(完工数量)完工数量  from (
                                            select  生产制令单号, case when 完工=1 then 生产数量 else 部分完工数 end as 完工数量   from 生产记录生产工单表)a   
                                             group by 生产制令单号) aaa  on aaa.生产制令单号 = sczl.生产制令单号
               where  /*生产记录生产制令表.关闭=0 and*/ sczl.生效日期 >= '2016-12-1' {0}  and sczl.生产制令类型!='研发样品' ", sql, CPublic.Var.LocalUserID);
                //and  操作人员ID='{1}' 
            }




            da = new SqlDataAdapter(sql, strconn);
            dt_proZLysx = new DataTable();
            da.Fill(dt_proZLysx);
            DataColumn dc = new DataColumn("选择", typeof(bool));
            dc.DefaultValue = false;
            dt_proZLysx.Columns.Add(dc);
            gc_已生效制令.DataSource = dt_proZLysx;




        }

        private void 查看工单状态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
            ERPproduct.frm查看制令相关工单的状态 fm = new ERPproduct.frm查看制令相关工单的状态(dr["生产制令单号"].ToString());
            CPublic.UIcontrol.AddNewPage(fm, "工单状态查询");
        }



#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load已生效制令();
            gv_已生效制令.ViewCaption = "未排单生效制令";

        }
        //在产未检验 已生效制令
#pragma warning disable IDE1006 // 命名样式
        private void button3_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            gv_已生效制令.ViewCaption = "在产制令";

            SqlDataAdapter da;
            string sql = "";
            if (date_前.EditValue != null && date_前.EditValue.ToString() != "" && date_后.EditValue != null && date_后.EditValue.ToString() != "")
            {
                sql = string.Format("and sczl.生效日期 >= '{0}' and sczl.生效日期 <= '{1}'", Convert.ToDateTime(date_前.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(date_后.EditValue).AddDays(1).ToString("yyyy-MM-dd"));
            }
            if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserID == "2101" || CPublic.Var.LocalUserID == "2233" || CPublic.Var.LocalUserID == "4136" || CPublic.Var.LocalUserID == "2106")
            {
                sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数,isnull(aaa.完工数量,0)完工数量,拼板数量  from 生产记录生产制令表  sczl
                    left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                    left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                    on sczl.生产制令单号=a.生产制令单号 left join 仓库物料数量表 kc on   kc.物料编码= sczl.物料编码 and sczl.仓库号=kc.仓库号
                    left join  (select  生产制令单号,sum(完工数量)完工数量  from (
                                            select  生产制令单号, case when 完工=1 then 生产数量 else 部分完工数 end as 完工数量   from 生产记录生产工单表)a   
                                             group by 生产制令单号) aaa  on aaa.生产制令单号 = sczl.生产制令单号
                      where sczl.生产制令单号 in ( select 生产制令单号   from 生产记录生产工单表 
                      where  生产记录生产工单表.生效=1 and 生产记录生产工单表.关闭 =0  and  生产记录生产工单表.检验完成=0 group by 生产制令单号)    
                     and sczl.关闭=0    and sczl.生效日期 >= '2017-12-1' {0}and sczl.生产制令类型!='研发样品' ", sql);
            }
            else
            {
                sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数,isnull(aaa.完工数量,0)完工数量,拼板数量  from 生产记录生产制令表 sczl
                          left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                          left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                          on sczl.生产制令单号=a.生产制令单号  left join 仓库物料数量表 kc on  kc.物料编码= sczl.物料编码  and sczl.仓库号=kc.仓库号
                          left join  (select  生产制令单号,sum(完工数量)完工数量  from (
                                            select  生产制令单号, case when 完工=1 then 生产数量 else 部分完工数 end as 完工数量   from 生产记录生产工单表)a   
                                             group by 生产制令单号) aaa  on aaa.生产制令单号 = sczl.生产制令单号
                        where sczl.生产制令单号 in ( select 生产制令单号   from 生产记录生产工单表 
                             where   生产记录生产工单表.生效=1 and  生产记录生产工单表.关闭 =0  and 生产记录生产工单表.检验完成=0 group by 生产制令单号)    
                             and sczl.关闭=0  and sczl.生效日期 >= '2017-12-1' {0} and sczl.生产制令类型!='研发样品'", sql, CPublic.Var.LocalUserID);
                //and   操作人员ID='{1}'
            }




            da = new SqlDataAdapter(sql, strconn);
            DataTable dt_proZLysx = new DataTable();
            da.Fill(dt_proZLysx);
            DataColumn dc = new DataColumn("选择", typeof(bool));
            dc.DefaultValue = false;
            dt_proZLysx.Columns.Add(dc);
            gc_已生效制令.DataSource = dt_proZLysx;
        }
        //已检未入库
#pragma warning disable IDE1006 // 命名样式
        private void button4_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            gv_已生效制令.ViewCaption = "已检未入库制令";

            SqlDataAdapter da;
            string sql = "";
            if (date_前.EditValue != null && date_前.EditValue.ToString() != "" && date_后.EditValue != null && date_后.EditValue.ToString() != "")
            {
                sql = string.Format("and sczl.生效日期 >= '{0}' and sczl.生效日期 <= '{1}'", Convert.ToDateTime(date_前.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(date_后.EditValue).AddDays(1).ToString("yyyy-MM-dd"));
            }
            if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserID == "2101" || CPublic.Var.LocalUserID == "2233" || CPublic.Var.LocalUserID == "4136" || CPublic.Var.LocalUserID == "2106")
            {
                sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数, 库存总数,isnull(aaa.完工数量,0)完工数量,拼板数量  from 生产记录生产制令表 sczl 
                         left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                 left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                          on sczl.生产制令单号=a.生产制令单号  left join 仓库物料数量表 kc on  kc.物料编码= sczl.物料编码 and sczl.仓库号=kc.仓库号
                          left join  (select  生产制令单号,sum(完工数量)完工数量  from (
               select  生产制令单号, case when 完工=1 then 生产数量 else 部分完工数 end as 完工数量   from 生产记录生产工单表)a   
               group by 生产制令单号) aaa  on aaa.生产制令单号 = sczl.生产制令单号
                          where sczl.生产制令单号 in ( select  生产记录生产工单表.生产制令单号  from 生产记录生产工单表
                         left join  生产记录生产制令表 sczl on sczl.生产制令单号=生产记录生产工单表.生产制令单号 
                           where 生产记录生产工单表.关闭=0 and 检验完成=1 and 生产记录生产工单表.完成=0 and sczl.完成=0  and sczl.生效日期 >='2017-12-1' {0} group by 生产记录生产工单表.生产制令单号) 
                    and sczl.关闭=0  and sczl.生效日期 >='2017-12-1'  {0} and sczl.生产制令类型!='研发样品'", sql);
            }
            else
            {
                sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数,isnull(aaa.完工数量,0)完工数量,拼板数量  from 生产记录生产制令表 sczl
                          left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                            left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                            on sczl.生产制令单号=a.生产制令单号  left join 仓库物料数量表 kc on  kc.物料编码= sczl.物料编码  and sczl.仓库号=kc.仓库号
                            left join  (select  生产制令单号,sum(完工数量)完工数量  from (
               select  生产制令单号, case when 完工=1 then 生产数量 else 部分完工数 end as 完工数量   from 生产记录生产工单表)a   
               group by 生产制令单号) aaa  on aaa.生产制令单号 = sczl.生产制令单号                                
where sczl.生产制令单号 in (select  生产记录生产工单表.生产制令单号  from 生产记录生产工单表
         left join  生产记录生产制令表 sczl on sczl.生产制令单号=生产记录生产工单表.生产制令单号 
                           where 生产记录生产工单表.关闭=0 and 检验完成=1 and 生产记录生产工单表.完成=0 and sczl.完成=0  and sczl.生效日期 >='2017-12-1' {0} group by 生产记录生产工单表.生产制令单号) 
                            and sczl.生效日期 >= '2017-12-1' and sczl.关闭=0 {0}   and sczl.生产制令类型!='研发样品'", sql, CPublic.Var.LocalUserID);
                // and sczl.操作人员ID = '{1}'
            }
            da = new SqlDataAdapter(sql, strconn);
            DataTable dt_proZLysx = new DataTable();
            da.Fill(dt_proZLysx);
            DataColumn dc = new DataColumn("选择", typeof(bool));
            dc.DefaultValue = false;
            dt_proZLysx.Columns.Add(dc);
            gc_已生效制令.DataSource = dt_proZLysx;
        }

        private void 修改制令ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);

            try
            {
                DateTime time = CPublic.Var.getDatetime();
                gv_已生效制令.CloseEditor();
                this.BindingContext[dt_proZLysx].EndCurrentEdit();


                decimal dec_原 = 0;
                dec_原 = Convert.ToDecimal(dr["制令数量"]);
                if (dr["已转工单数"] != DBNull.Value && Convert.ToDecimal(dr["已转工单数"]) > 0)
                {

                    throw new Exception("已转工单不允许修改");
                }
                else
                {
                    frm修改制令 fm = new frm修改制令(dr);
                    fm.ShowDialog();
                    if (fm.flag)
                    {
                        for (int i = 0; i < gv_关联订单.RowCount; i++)
                        {
                            gv_关联订单.GetDataRow(i)["计划确认日期"] = dr["预完工日期"];
                        }
                        dr["预计完工日期"] = dr["预完工日期"];
                    }
                    if (fm.de_现 != 0)
                    {
                        DataTable t_save = dt_proZLysx.Clone();
                        t_save.ImportRow(dr);

                        t_save.Rows[0]["未排单数量"] = t_save.Rows[0]["制令数量"] = fm.de_现;
                        //dr["未排单数量"] = dr["制令数量"] = fm.de_现;
                        string s_制令 = dr["生产制令单号"].ToString();
                        if (s_制令.Contains("-") && fm.de_现 != dec_原) //包含 - 的是 分批转的需要更改数量
                        {
                            string[] xx = s_制令.Split('-');
                            string qql = string.Format(@"select sczl.*,a.已转工单数,库存总数,新数据,拼板数量   from 生产记录生产制令表 sczl
            left join  基础数据物料信息表 base on  base.物料编码 = sczl.物料编码
               left join(select sum(生产数量) as 已转工单数, 生产制令单号  from 生产记录生产工单表 where 关闭 = 0 group by 生产制令单号) a
               on sczl.生产制令单号 = a.生产制令单号
              left join 仓库物料数量表 kc on  kc.物料编码 = sczl.物料编码 and sczl.仓库号 = kc.仓库号
               where   sczl.生产制令单号='{0}'  ", xx[0]);
                            //加载原制令并且要是未生效的 
                            DataTable t = CZMaster.MasterSQL.Get_DataTable(qql, strconn);
                            if (t.Rows.Count == 0)
                            {
                                if (MessageBox.Show("原制令已生效或关闭,修改后数量将不影响原单据？是否继续", "警告!", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    throw new Exception("已取消操作");
                                }
                            }
                            else
                            {
                                decimal dec_total = Convert.ToDecimal(t.Rows[0]["制令数量"]) + dec_原;
                                if (fm.de_现 > dec_total) throw new Exception("已超过可修改范围");
                                else if (fm.de_现 == dec_total)
                                {
                                    if (MessageBox.Show("原制令数量已剩0,将自动关闭原单？是否继续", "警告!",
                                        MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                    {
                                        throw new Exception("已取消操作");
                                    }
                                    else
                                    {
                                        //原制令数量=0  自动关闭
                                        t.Rows[0]["制令数量"] = 0;
                                        t.Rows[0]["关闭"] = 1;
                                        t.Rows[0]["关闭人员"] = CPublic.Var.localUserName;
                                        t.Rows[0]["关闭人员ID"] = CPublic.Var.LocalUserID;
                                        t.Rows[0]["关闭日期"] = time;
                                        t_save.ImportRow(t.Rows[0]);
                                    }

                                }
                                else    //修改数量比原来小 那么 原制令数量及 
                                {
                                    t.Rows[0]["制令数量"] = dec_total - fm.de_现;
                                    t_save.ImportRow(t.Rows[0]);

                                }


                            }
                        }
                        DataTable dt_销售明细 = new DataTable();
                        for (int i = 0; i < gv_关联订单.RowCount; i++)
                        {
                            string str = string.Format("select * from 销售记录销售订单明细表 where 生效=1 and 关闭=0 and 销售订单明细号='{0}'", gv_关联订单.GetDataRow(i)["销售订单明细号"].ToString());
                            using (SqlDataAdapter a = new SqlDataAdapter(str, strconn))
                            {
                                a.Fill(dt_销售明细);
                                DataRow[] xx = dt_销售明细.Select(string.Format("销售订单明细号='{0}'", gv_关联订单.GetDataRow(i)["销售订单明细号"].ToString()));
                                if (xx.Length > 0)
                                {
                                    xx[0]["计划确认日期"] = gv_关联订单.GetDataRow(i)["计划确认日期"];
                                }
                            }
                        }
                        SqlDataAdapter da;
                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction xgzl = conn.BeginTransaction("修改制令");
                        try
                        {
                            SqlCommand cmd2 = new SqlCommand("select * from 销售记录销售订单明细表 where 1<>1", conn, xgzl);
                            da = new SqlDataAdapter(cmd2);
                            new SqlCommandBuilder(da);
                            da.Update(dt_销售明细);

                            string sql_1 = "select * from 生产记录生产制令表 where 1<>1";
                            cmd2 = new SqlCommand(sql_1, conn, xgzl);
                            da = new SqlDataAdapter(cmd2);
                            new SqlCommandBuilder(da);
                            da.Update(t_save);
                            xgzl.Commit();
                            MessageBox.Show("修改成功");

                            //                string s = string.Format(@"select sczl.*,a.已转工单数,库存总数,新数据,拼板数量   from 生产记录生产制令表 sczl
                            //left join  基础数据物料信息表 base on  base.物料编码 = sczl.物料编码
                            //   left join(select sum(生产数量) as 已转工单数, 生产制令单号  from 生产记录生产工单表 where 关闭 = 0 group by 生产制令单号) a
                            //   on sczl.生产制令单号 = a.生产制令单号
                            //  left join 仓库物料数量表 kc on  kc.物料编码 = sczl.物料编码 and sczl.仓库号 = kc.仓库号
                            //   where   sczl.生产制令单号='{0}'  ", dr["生产制令单号"]);
                            //                DataRow rr = CZMaster.MasterSQL.Get_DataRow(s, strconn);
                            //                dr.ItemArray = rr.ItemArray;
                            //                dr.AcceptChanges();
                            frsh_单条(dr);
                            if (s_制令.Contains("-"))
                            {
                                string[] xx = s_制令.Split('-');

                                DataRow[] gr = dt_proZL.Select(string.Format("生产制令单号='{0}'", xx[0]));
                                if (gr.Length > 0)
                                {
                                    frsh_单条(gr[0]);

                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            xgzl.Rollback();
                            frsh_单条(dr);

                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                frsh_单条(dr);
                MessageBox.Show(ex.Message);
            }


        }
        //单条刷新已生效 未生效制令 
        private void frsh_单条(DataRow dr)
        {
            string s = string.Format(@"select sczl.*,a.已转工单数,库存总数,新数据,拼板数量   from 生产记录生产制令表 sczl
            left join  基础数据物料信息表 base on  base.物料编码 = sczl.物料编码
               left join(select sum(生产数量) as 已转工单数, 生产制令单号  from 生产记录生产工单表 where 关闭 = 0 group by 生产制令单号) a
               on sczl.生产制令单号 = a.生产制令单号
              left join 仓库物料数量表 kc on  kc.物料编码 = sczl.物料编码 and sczl.仓库号 = kc.仓库号
               where   sczl.生产制令单号='{0}'  ", dr["生产制令单号"]);
            DataRow rr = CZMaster.MasterSQL.Get_DataRow(s, strconn);
            dr.ItemArray = rr.ItemArray;
            dr.AcceptChanges();

        }


#pragma warning disable IDE1006 // 命名样式
        private void panel4_Paint(object sender, PaintEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            gv_sczlmain_RowCellClick_1(null, null);
        }




        //点击导出
        [DllImport("user32.dll")]

        public static extern int GetFocus();

        ///获取 当前拥有焦点的控件

        private Control GetFocusedControl()

        {
            Control c = null;

            // string focusedControl = null;

            IntPtr handle = (IntPtr)GetFocus();


            if (handle == null)
                this.FindForm().KeyPreview = true;

            else
            {

                c = Control.FromHandle(handle);
            }


            return c;


        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //if (ActiveControl != null && ActiveControl.GetType().Equals(gc_已生效制令.GetType()))
            //{

            //    SaveFileDialog saveFileDialog = new SaveFileDialog();
            //    saveFileDialog.Title = "导出Excel";
            //    saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            //    DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            //    if (dialogResult == DialogResult.OK)
            //    {
            //        DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
            //        DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;
            //        gc.ExportToXlsx(saveFileDialog.FileName);
            //        DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("若要导出请先选中要导出的表格");
            //}

            Control c = GetFocusedControl();

            if (c != null && c.GetType().Equals(gc_未生效制令.GetType()))

            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                 
                saveFileDialog.Title = "导出Excel";

                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";

                DialogResult dialogResult = saveFileDialog.ShowDialog(this);

                if (dialogResult == DialogResult.OK)

                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();


                    DevExpress.XtraGrid.GridControl gc = (c) as DevExpress.XtraGrid.GridControl;


                    gc.ExportToXlsx(saveFileDialog.FileName);

                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }

            }

            else
            {

                MessageBox.Show("若要导出请先选中要导出的表格(鼠标点一下表格)");
            }

        }
#pragma warning disable IDE1006 // 命名样式
        private void gv_已生效制令_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }











#pragma warning disable IDE1006 // 命名样式
        private void rsl_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void rsl_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);

                DataRow rr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                rr["物料名称"] = dr["物料名称"];
                rr["规格型号"] = dr["规格型号"];
                rr["图纸编号"] = dr["图纸编号"];
                rr["物料编码"] = dr["物料编码"];
                rr["生产车间"] = dr["车间编号"];
                rr["库存总数"] = dr["库存总数"];
                //DataTable t = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
                //if (t.Rows.Count > 0)
                //{
                //    rr["生产车间"] = t.Rows[0]["生产车间"];
                //}
                rr["特殊备注"] = dr["特殊备注"];
                rr["仓库号"] = dr["仓库号"];
                rr["仓库名称"] = dr["仓库名称"];

                rr["班组ID"] = dr["班组ID"];
                rr["班组"] = dr["班组"];
                rr["新数据"] = dr["新数据"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //DataView dv = new DataView();
            //   dv.ToTable


            DataTable dt2 = (DataTable)gc_未生效制令.DataSource;

            foreach (DataRow dr in dt2.Rows)
            {

                string v_number = "";
                DataTable dt_x = new DataTable();
                dt_x = ERPorg.Corg.billofM(dt_x, dr["物料编码"].ToString(), true, dt_bom);
                //  dt_x = ERPorg.Corg.billofM(dt_x, "05019901000009", true, dt_bom);
                if (dt_x.Rows.Count > 0)
                {
                    foreach (DataRow drr in dt_x.Rows)
                    {
                        string sql1 = string.Format(@"SELECT 文件名, 物料号, 版本 FROM 程序版本维护表 WHERE 版本 = (SELECT MAX(版本) FROM    程序版本维护表 where  物料号 ='{0}'and 停用='0'  ) 
                       and 物料号 = '{0}'and 停用='0'  ", drr["子项编码"]);
                        DataRow dr_banbe = CZMaster.MasterSQL.Get_DataRow(sql1, strconn);
                        if (dr_banbe != null)
                        {
                            if (dr_banbe["文件名"].ToString() != "")
                            {
                                if (v_number == "")
                                {
                                    v_number = v_number + dr_banbe["文件名"].ToString();
                                }
                                else
                                {
                                    v_number = v_number + ";" + dr_banbe["文件名"].ToString();
                                }
                                //break;
                            }
                        }
                    }
                }
                dr["版本备注"] = v_number.ToString();
            }

            SqlDataAdapter da;
            string sql = "select * from 生产记录生产制令表 where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt2);
            int index = 0;

            index = gv_未生效制令.FocusedRowHandle;

            barLargeButtonItem1_ItemClick(null, null);

            gv_未生效制令.FocusedRowHandle = index;
            gv_未生效制令.SelectRow(index);




        }

        private void 查看料况ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
            ui制令料况查询 ui = new ui制令料况查询(dr);
            CPublic.UIcontrol.Showpage(ui, "制令料况");
        }

        private void 查看料况ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
            ui制令料况查询 ui = new ui制令料况查询(dr);
            CPublic.UIcontrol.Showpage(ui, "制令料况");
        }

        private void gv_已生效制令_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gv_已生效制令.GetRow(e.RowHandle) == null)
                {
                    return;
                }


                if (Convert.ToBoolean(gv_已生效制令.GetRowCellValue(e.RowHandle, "完成")))
                {
                    e.Appearance.BackColor = Color.GreenYellow;

                }


            }
            catch
            {
            }
        }


        private void barLargeButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ui未完成制令缺料情况 ui = new ui未完成制令缺料情况();
            CPublic.UIcontrol.Showpage(ui, "未完成制令缺料情况");
        }
        /// <summary>
        /// 19-8-20 改 分批制令自动转工单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 分批生效制令ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);

                if (MessageBox.Show(string.Format("确定分批生效制令'{0}'的部分数量？", dr["生产制令单号"]), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    工单部分完工填写数量界面 frm = new 工单部分完工填写数量界面();
                    frm.Text = "分批生效制令";
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.ShowDialog();
                    if (frm.bl && frm.in_部分完工数 > 0)
                    {
                        DateTime time = CPublic.Var.getDatetime();
                        string s = "select  * from 生产记录生产制令表 where 1<>1";
                        DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                        if (Convert.ToDecimal(dr["制令数量"]) > frm.in_部分完工数)
                        {
                            s = string.Format("select * from 生产记录生产制令表 where 生产制令单号='{0}'", dr["生产制令单号"]);
                            using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                            {
                                da.Fill(t);
                                t.Rows[0]["制令数量"] = t.Rows[0]["未排单数量"] = Convert.ToDecimal(t.Rows[0]["制令数量"]) - frm.in_部分完工数;
                                /// 备注4 用来存放制令分批转的流水号
                                if (t.Rows[0]["备注4"].ToString() == "") t.Rows[0]["备注4"] = 0;
                                t.Rows[0]["备注4"] = Convert.ToDecimal(t.Rows[0]["备注4"]) + 1;
                                DataRow r_new = t.NewRow();
                                r_new["生产制令类型"] = dr["生产制令类型"];
                                r_new["生产计划单号"] = dr["生产计划单号"];
                                r_new["物料编码"] = dr["物料编码"];
                                r_new["物料名称"] = dr["物料名称"];
                                r_new["规格型号"] = dr["规格型号"];
                                r_new["生产车间"] = dr["生产车间"];
                                r_new["预开工日期"] = dr["预开工日期"];
                                r_new["预完工日期"] = dr["预完工日期"];
                                r_new["预计完工日期"] = dr["预完工日期"];

                                r_new["备注"] = dr["备注"];

                                r_new["班组ID"] = dr["班组ID"];
                                r_new["班组"] = dr["班组"];


                                r_new["仓库号"] = dr["仓库号"];
                                r_new["仓库名称"] = dr["仓库名称"];
                                r_new["项目号"] = dr["项目号"];
                                r_new["版本备注"] = dr["版本备注"];
                                r_new["生效人员"] = r_new["制单人员"] = r_new["操作人员"] = CPublic.Var.localUserName;
                                r_new["生效人员ID"] = r_new["制单人员ID"] = r_new["操作人员ID"] = CPublic.Var.LocalUserID;
                                r_new["GUID"] = System.Guid.NewGuid();
                                r_new["制令数量"] = frm.in_部分完工数;
                                r_new["未排单数量"] = frm.in_部分完工数; /// 19-8-22需要改成0  然后工单自动生效
                                r_new["加急状态"] = dr["加急状态"];
                                r_new["生效"] = 1;
                                r_new["日期"] = time;
                                r_new["生效日期"] = time;
                                //r_new["生产制令单号"] = string.Format("PM{0}{1:00}{2:00}{3:0000}", time.Year, time.Month, time.Day,
                                //CPublic.CNo.fun_得到最大流水号("PM", time.Year, time.Month));
                                r_new["生产制令单号"] = dr["生产制令单号"].ToString() + "-" + Convert.ToInt32(t.Rows[0]["备注4"]).ToString("00");

                                t.Rows.Add(r_new);
                                s = string.Format("select * from 生产记录生产制令子表 where 生产制令单号='{0}'", dr["生产制令单号"]);
                                DataTable t_mx = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                                DataTable t_mx_cpy = t_mx.Copy();
                                foreach (DataRow r in t_mx_cpy.Rows)
                                {
                                    DataRow p = t_mx.NewRow();
                                    p["GUID"] = System.Guid.NewGuid();
                                    p["生产制令单号"] = r_new["生产制令单号"];
                                    p["销售订单明细号"] = r["销售订单明细号"];
                                    p["销售订单号"] = r["销售订单号"];
                                    p["物料编码"] = r["物料编码"];
                                    p["物料名称"] = r["物料名称"];
                                    p["客户"] = r["客户"];
                                    p["送达日期"] = r["送达日期"];
                                    p["规格型号"] = r["规格型号"];
                                    p["数量"] = r["数量"];
                                    p["计划确认日期"] = r["计划确认日期"];
                                    p["销售备注"] = r["销售备注"];
                                    t_mx.Rows.Add(p);
                                }
                                //2019-8-21 不要放在未生效节点 放到 已生效制令的 转工单操作 并且 已生效制令修改数量后需要跟未生效的制令联动
                                // DataTable dt_gd= fun_自动转工单(r_new);
                                try
                                {   //制令主表
                                    SqlConnection conn = new SqlConnection(strconn);
                                    conn.Open();
                                    SqlTransaction ts = conn.BeginTransaction("制令保存");
                                    try
                                    {
                                        string sql = "select * from 生产记录生产制令表 where 1<>1";
                                        SqlCommand cmm = new SqlCommand(sql, conn, ts);
                                        SqlDataAdapter dax = new SqlDataAdapter(cmm);
                                        new SqlCommandBuilder(dax);
                                        dax.Update(t);
                                        //制令明细表
                                        sql = "select * from 生产记录生产制令子表 where 1<>1";
                                        cmm = new SqlCommand(sql, conn, ts);

                                        dax = new SqlDataAdapter(cmm);
                                        new SqlCommandBuilder(dax);
                                        dax.Update(t_mx);

                                        //sql = "select * from 生产记录生产工单表 where 1<>1";
                                        //cmm = new SqlCommand(sql, conn, ts);
                                        //dax = new SqlDataAdapter(cmm);
                                        //new SqlCommandBuilder(dax);
                                        //dax.Update(dt_gd);

                                        ts.Commit();

                                        MessageBox.Show("成功");
                                        barLargeButtonItem1_ItemClick(null, null);
                                    }
                                    catch (Exception ex)
                                    {
                                        ts.Rollback();
                                        throw new Exception(ex.Message);
                                    }

                                }
                                catch (Exception ex)
                                {

                                    throw new Exception(ex.Message);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("数量超过可分批数量");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        /// <summary>
        ///分批转制令自动转成工单
        ///工单子表 没有用 不添记录了
        ///19-8-22 还需要自动生效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private DataRow fun_自动转工单(DataRow dr_new制令)
        {
            DateTime tt = CPublic.Var.getDatetime();
            string ss = tt.Year.ToString().Substring(2, 2);
            string yyyy = tt.Year.ToString();
            string mm = tt.Month.ToString("00");
            string dd = tt.Day.ToString("00");

            string s = "select  * from 生产记录生产工单表 where 1=2";
            DataTable dt_工单 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            string s_版本 = string.Format(@" with temp_bom(产品编码,子项编码,仓库号,仓库名称,wiptype,子项类型,数量,bom类型,bom_level ) as
                                            (select  产品编码,子项编码,仓库号,仓库名称,WIPType,子项类型,数量,bom类型,1 as level from 基础数据物料BOM表 
                                             where 产品编码='{0}'
                                           union all 
                                           select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level+1  from 基础数据物料BOM表 a
                                           inner join temp_bom b on a.产品编码=b.子项编码 
                                           ) 
                                           select 子项编码,子项名称,文件名 from (
                                              select  产品编码,fx.物料名称 as  产品名称,子项编码,base.物料名称 as 子项名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称,
                                              bom_level,base.规格型号 as 子项规格,isnull(文件名,'')文件名  from  temp_bom a
                                          left join 基础数据物料信息表 base  on base.物料编码=a.子项编码
                                          left join 基础数据物料信息表  fx  on fx.物料编码=a.产品编码
                                           left join   (  select  a.* from 程序版本维护表 a
                                          inner join (select  物料号,MAX(版本)maxbb from 程序版本维护表 where 停用=0  group by 物料号) b 
                                          on a.物料号=b.物料号 and  a.版本=b.maxbb ) bb on bb.物料号 =子项编码 
                                          group by 产品编码,fx.物料名称 ,子项编码,base.物料名称 ,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称, bom_level,base.规格型号,文件名
                                           ) aaaa group by 子项编码,子项名称,文件名
                                          ", dr_new制令["物料编码"]);
            DataTable dt_Bomm = CZMaster.MasterSQL.Get_DataTable(s_版本, strconn);
            //MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
            //DataTable dt_SaleOrder = RBQ.SelectGroupByInto("", dt_Bomm, " 子项编码,子项名称,文件名 ", "", "子项编码,子项名称,文件名");
            string sql11 = string.Format(@"   SELECT 文件名, 物料号, 版本 FROM 程序版本维护表 WHERE 版本 = (SELECT MAX(版本) FROM    程序版本维护表 where  物料号 ='{0}' and 停用='0' ) and 物料号 = '{0}'  and 停用='0' ", dr_new制令["物料编码"]);
            DataRow dr_1 = CZMaster.MasterSQL.Get_DataRow(sql11, strconn);
            string v_number = "";
            if (dr_1 != null)
            {
                v_number = dr_1["文件名"].ToString();
            }

            foreach (DataRow drr in dt_Bomm.Rows)
            {


                if (drr["文件名"].ToString() != "")
                {
                    if (v_number == "")
                    {
                        v_number = v_number + drr["文件名"].ToString();
                    }
                    else
                    {
                        v_number = v_number + ";" + drr["文件名"].ToString();
                    }


                }
            }


            string strMoNo = string.Format("MO{0}{1:D2}{2:00}{3:0000}", ss, mm, dd, CPublic.CNo.fun_得到最大流水号("MO", Convert.ToInt32(yyyy), Convert.ToInt32(mm), Convert.ToInt32(dd)));
            DataRow drrr = dt_工单.NewRow(); //
            drrr["生产工单号"] = strMoNo;
            drrr["生产工单类型"] = dr_new制令["生产制令类型"];
            drrr["加急状态"] = dr_new制令["加急状态"];
            drrr["预计开工日期"] = tt.Date;
            drrr["预计完工日期"] = dr_new制令["预完工日期"];

            drrr["GUID"] = System.Guid.NewGuid();
            drrr["生产制令单号"] = dr_new制令["生产制令单号"];
            drrr["物料编码"] = dr_new制令["物料编码"];
            drrr["版本备注"] = v_number;
            drrr["物料名称"] = dr_new制令["物料名称"];
            drrr["规格型号"] = dr_new制令["规格型号"];
            drrr["仓库号"] = dr_new制令["仓库号"];
            drrr["仓库名称"] = dr_new制令["仓库名称"];
            drrr["特殊备注"] = dr_new制令["特殊备注"];
            drrr["备注1"] = dr_new制令["备注"];
            //     drrr["作废"] = 0;
            // drrr["工时备注"] = dr_new制令["工时备注"];
            string x = string.Format("select 工时 from 基础数据物料信息表 where 物料编码='{0}'", dr_new制令["物料编码"].ToString());
            DataTable tx = CZMaster.MasterSQL.Get_DataTable(x, strconn);
            if (Convert.ToDecimal(tx.Rows[0]["工时"]) > 0)
            {
                // drrr["工时"] = Convert.ToDecimal(dr["生产数量"]) / Convert.ToDecimal(dr["工时定额"]);
                drrr["工时"] = Convert.ToDecimal(dr_new制令["制令数量"]) * Convert.ToDecimal(tx.Rows[0]["工时"]); // 19-7-30 

            }

            drrr["生产数量"] = dr_new制令["制令数量"];
            drrr["班组ID"] = dr_new制令["班组ID"];
            drrr["班组"] = dr_new制令["班组"];
            drrr["未检验数量"] = drrr["生产数量"];
            drrr["图纸编号"] = dr_new制令["图纸编号"];
            drrr["生产车间"] = dr_new制令["生产车间"];
            x = string.Format("select 属性值, 属性字段1  from 基础数据基础属性表 where 属性类别 = '生产车间' and 属性字段1='{0}'", dr_new制令["生产车间"]);
            tx = CZMaster.MasterSQL.Get_DataTable(x, strconn);
            if (tx.Rows.Count > 0 && tx.Rows[0][0] != null && tx.Rows[0][0].ToString() != "")
                drrr["车间名称"] = tx.Rows[0]["属性值"];


            drrr["制单人员ID"] = CPublic.Var.LocalUserID;
            drrr["制单人员"] = CPublic.Var.localUserName;
            drrr["制单日期"] = tt;
            drrr["生效"] = 1;
            drrr["生效人"] = CPublic.Var.localUserName;
            drrr["生效人ID"] = CPublic.Var.LocalUserID;
            drrr["生效日期"] = tt;
            dt_工单.Rows.Add(drrr);
            return drrr;

        }

        private DataSet fun_自动生效工单(DataTable dtX)
        {
            DataTable dt_MIcach = new DataTable(); //原料库存缓存
            DataSet ds = new DataSet();
            DataTable dt_刷新数量 = new DataTable();
            DataSet ds_back = new DataSet();
            ds.Tables.Add();
            ds.Tables.Add();
            ds.Tables.Add(dt_刷新数量);
            ds.Tables.Add(dt_MIcach);
            DateTime time = CPublic.Var.getDatetime();

            ds.DataSetName = "x";
            ds.Tables[0].TableName = "ds0";
            ds.Tables[1].TableName = "ds1";
            ds.Tables[2].TableName = "list_原料刷新";
            ds.Tables[3].TableName = "list_库存缓存";
            foreach (DataRow drM in dtX.Rows)
            {
                string x = string.Format(@"select 产品编码,a.物料编码,库存总数,有效总数,b.计量单位 as bom单位,单位换算标识,单位换算标识 from 仓库物料数量表 a
                        left  join 基础数据物料信息表 base on base.物料编码=a.物料编码
                        Left  join 基础数据物料BOM表 b on a.物料编码=b.子项编码 and a.仓库号=b.仓库号 where   产品编码='{0}'", drM["物料编码"].ToString());

                DataTable temp = CZMaster.MasterSQL.Get_DataTable(x, strconn);
                if (dt_MIcach.Columns.Count == 0)
                {
                    dt_MIcach = CZMaster.MasterSQL.Get_DataTable(x, strconn);
                }
                else
                {
                    DataRow[] r = dt_MIcach.Select(string.Format("产品编码='{0}'", drM["物料编码"].ToString()));
                    if (r.Length == 0) //该成品未加载过
                    {
                        foreach (DataRow dr in temp.Rows)
                        {
                            if (dt_MIcach.Select(string.Format("物料编码='{0}'", dr["物料编码"].ToString())).Length == 0) //dt_MIcach 里先找有没有  没有就添进去
                            {
                                dt_MIcach.ImportRow(dr);
                                if (dr["单位换算标识"].Equals(true)) //
                                {
                                    string ss = string.Format("select  * from 计量单位换算表 where 物料编码='{0}'", dr["物料编码"]);
                                    using (SqlDataAdapter da = new SqlDataAdapter(ss, strconn))
                                    {
                                        DataTable dt = new DataTable();
                                        da.Fill(dt);
                                        DataRow[] r1 = dt.Select(string.Format("计量单位='{0}'", dr["bom单位"].ToString().Trim()));
                                        DataRow[] r2 = dt.Select(string.Format("计量单位='{0}'", dr["库存单位"].ToString().Trim()));
                                        decimal dec = Convert.ToDecimal(r2[0]["换算率"]) / Convert.ToDecimal(r1[0]["换算率"]);
                                        //DataRow []rr=  dt.Select(string.Format("计量单位='{0}'", dr["库存单位"].ToString().Trim()));
                                        dr["有效总数"] = dec * Convert.ToDecimal(dr["有效总数"]);
                                        dr["库存总数"] = dec * Convert.ToDecimal(dr["库存总数"]);
                                    }
                                }
                            }

                        }
                    }
                }
                ds.Tables.RemoveAt(3);
                ds.Tables.Add(dt_MIcach);
                ds.Tables[3].TableName = "list_库存缓存";


                // drM["预计完工日期"] = dateEdit2.EditValue;


                // 更改对应制令号的数量 已排单和未排单数量  in 生产记录生产制令表 


                DataTable dt_temp = drM.Table.Clone();
                dt_temp.TableName = "drm";
                dt_temp.ImportRow(drM);

                string str_待领料单号 = string.Format("DL{0}{1:00}{2:0000}",
                                        time.Year, time.Month,
                                        CPublic.CNo.fun_得到最大流水号("DL", time.Year, time.Month));



                ds = StockCore.StockCorer.fun_lld(ds, dt_temp, CPublic.Var.LocalUserID, CPublic.Var.localUserName
                    , "", "", "", "", str_待领料单号);

                dt_MIcach = ds.Tables[3];
            }
            return ds;

        }


        private void barLargeButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {       //   dv.ToTable


            DataTable dt2 = (DataTable)gc_已生效制令.DataSource;

            DataRow drM = (this.BindingContext[gc_已生效制令.DataSource].Current as DataRowView).Row;


            string v_number = "";

            DataTable dt_x = new DataTable();
            dt_x = ERPorg.Corg.billofM(dt_x, drM["物料编码"].ToString(), true, dt_bom);

            //  dt_x = ERPorg.Corg.billofM(dt_x, "05019901000009", true, dt_bom);
            if (dt_x.Rows.Count > 0)
            {
                foreach (DataRow drr in dt_x.Rows)
                {
                    string sql1 = string.Format(@"  SELECT 文件名, 物料号, 版本 FROM 程序版本维护表 WHERE 版本 = (SELECT MAX(版本) FROM    程序版本维护表 where  物料号 ='{0}'and 停用='0'  ) and 物料号 = '{0}'and 停用='0'  ", drr["子项编码"]);
                    DataRow dr_banbe = CZMaster.MasterSQL.Get_DataRow(sql1, strconn);
                    if (dr_banbe != null)
                    {
                        if (dr_banbe["文件名"].ToString() != "")
                        {
                            if (v_number == "")
                            {
                                v_number = v_number + dr_banbe["文件名"].ToString();
                            }
                            else
                            {
                                v_number = v_number + ";" + dr_banbe["文件名"].ToString();
                            }
                            //break;
                        }

                    }
                }


                drM["版本备注"] = v_number.ToString();


            }

            SqlDataAdapter da;
            string sql = "select * from 生产记录生产制令表 where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt2);

            int index = 0;

            index = gv_已生效制令.FocusedRowHandle;

            barLargeButtonItem1_ItemClick(null, null);
            gv_已生效制令.FocusedRowHandle = index;
            gv_已生效制令.SelectRow(index);



        }

        private void barLargeButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("是否确认撤销生效"), "提示？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    this.ActiveControl = null;

                    if (xtraTabControl1.SelectedTabPage == xtraTabPage2)
                    {
                        DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
                        //先判断有没有完成
                        if (Convert.ToBoolean(dr["完成"]))
                        {
                            throw new Exception("该制令已经完成，不能作废");
                        }
                        if (Convert.ToBoolean(dr["关闭"]))
                        {
                            throw new Exception("该制令已经关闭，不能作废");
                        }
                        //再判断有没有转过工单，关闭还没转工单的数量 
                        if (Convert.ToDecimal(dr["已排单数量"]) == 0)
                        {
                            DataTable t = new DataTable();
                           
                            string sql = string.Format(@"select  zl.*,smx.物料编码 as 销售物料编码,smx.物料名称 as 销售产品名称,smx.规格型号 as 销售产品型号 from 生产记录生产制令子表 zl
                             left join 销售记录销售订单明细表 smx  on zl.销售订单明细号 = smx.销售订单明细号 where 生产制令单号='{0}'", dr["生产制令单号"].ToString());
                            using (SqlDataAdapter da_1 = new SqlDataAdapter(sql, strconn))
                            {
                                da_1.Fill(dt_proZLdetail);

                                DataRow[] x = dt_proZLdetail.Select(string.Format("生产制令单号='{0}'", dr["生产制令单号"].ToString()));
                                if (x.Length > 0)
                                {
                                    x[0]["计划确认日期"] = DBNull.Value;
                                }
                            }
                         
                            foreach (DataRow r in dt_proZLdetail.Rows)
                            {
                                if (r.RowState == DataRowState.Deleted) continue;
                                string str = string.Format("select * from 销售记录销售订单明细表 where 生效=1 and 关闭=0 and 销售订单明细号='{0}'", r["销售订单明细号"].ToString());
                                using (SqlDataAdapter a = new SqlDataAdapter(str, strconn))
                                {
                                    a.Fill(t);
                                    DataRow[] xx = t.Select(string.Format("销售订单明细号='{0}'", r["销售订单明细号"].ToString()));
                                    if (xx.Length > 0)
                                    {

                                        xx[0]["计划确认日期"] = DBNull.Value;
                                    }
                                }
                            }
                            //生效时给相应的字段赋值
                            dr["生效"] = false;
                            dr["生效人员ID"] = "";
                            dr["生效人员"] = "";
                            //dr["生效日期"] = DBNull.Value;
                            string s_制令号 = dr["生产制令单号"].ToString();
                            DateTime time = CPublic.Var.getDatetime();
                            if (s_制令号.Contains("-")) //数量需要回到原制单数量上去 并且该记录作废 //制令本号 撤回 不需要额外操作
                            {
                                string[] xx = s_制令号.Split('-');
                                DataRow[] r = dt_proZLysx.Select(string.Format("生产制令单号='{0}'", xx[0].Trim()));
                                if (r.Length > 0)
                                {
                                    r[0]["制令数量"] = Convert.ToDecimal(r[0]["制令数量"]) + Convert.ToDecimal(dr["制令数量"]);
                                    dr["关闭"] = true;
                                    dr["关闭人员"] = CPublic.Var.localUserName;
                                    dr["关闭人员ID"] = CPublic.Var.LocalUserID;
                                }
                                else  //不在dt_proZLysx 中 需要往这里面加这一条数据 因为dt_proZLysx是 已生效制令列表 
                                {
                                    string sl = string.Format(@"select sczl.*,isnull(a.已转工单数, 0)已转工单数,库存总数,新数据,isnull(aaa.完工数量, 0)完工数量,拼板数量 from 生产记录生产制令表 sczl
                                           left join  基础数据物料信息表 base on  base.物料编码 = sczl.物料编码
                                            left join  仓库物料数量表 kc on kc.物料编码 = sczl.物料编码 and sczl.仓库号 = kc.仓库号
                                   left join(select sum(生产数量) as 已转工单数,生产制令单号 from  生产记录生产工单表 where 关闭 = 0  group by 生产制令单号) a
                                   on sczl.生产制令单号 = a.生产制令单号
                                   left join(select 生产制令单号, sum(完工数量) 完工数量  from (
                                   select 生产制令单号, case when 完工 = 1 then 生产数量 else 部分完工数 end as 完工数量   from 生产记录生产工单表)a
                                    group by 生产制令单号) aaa on aaa.生产制令单号 = sczl.生产制令单号
                                     where sczl.生产制令单号 = '{0}'", xx[0].Trim());  ///保证和dt_proZLysx 相同表结构
                                    using (SqlDataAdapter daa = new SqlDataAdapter(sl, strconn))
                                    {
                                        daa.Fill(dt_proZLysx);
                                    }
                                    r = dt_proZLysx.Select(string.Format("生产制令单号='{0}'", xx[0].Trim()));
                                    r[0]["制令数量"] = Convert.ToDecimal(r[0]["制令数量"]) + Convert.ToDecimal(dr["制令数量"]);
                                    dr["关闭"] = true;
                                    dr["关闭日期"] = time;

                                    dr["关闭人员"] = CPublic.Var.localUserName;
                                    dr["关闭人员ID"] = CPublic.Var.LocalUserID;
                                }
                            }


                            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
                            //生效时给相应的字段赋值
                            if (flag == false)
                            {
                                
                                dic.Add("生产记录生产制令表", dt_proZLysx);
                            }
                            dic.Add("生产记录生产制令子表", dt_proZLdetail);
                            dic.Add("销售记录销售订单明细表", t);
    
                            ERPorg.Corg cg = new ERPorg.Corg();
                            cg.save(dic);
                            MessageBox.Show("撤销成功");
                            barLargeButtonItem1_ItemClick(null, null);

                            //SqlDataAdapter da;
                            //SqlConnection conn = new SqlConnection(strconn);
                            //conn.Open();
                            //SqlTransaction ts = conn.BeginTransaction("ZLSX");
                            //SqlCommand cmd2 = new SqlCommand("select * from 销售记录销售订单明细表 where 1<>1", conn, ts);

                            ////SqlCommand cmd_计划池 = new SqlCommand("select * from 生产记录生产计划表 where 1<>1", conn, ts);
                            //SqlCommand cmd = new SqlCommand("select * from 生产记录生产制令表 where 1<>1", conn, ts);
                            //SqlCommand cmd1 = new SqlCommand("select * from 生产记录生产制令子表 where 1<>1", conn, ts);
                            ////SqlCommand cmd3 = new SqlCommand("select * from 计划人员关联物料表 where 1<>1", conn, ts);

                            //try
                            //{
                            //    if (flag == false)
                            //    {
                            //        da = new SqlDataAdapter(cmd);
                            //        new SqlCommandBuilder(da);
                            //        da.Update(dt_proZLysx);


                            //    }
                            //    da = new SqlDataAdapter(cmd1);
                            //    new SqlCommandBuilder(da);
                            //    da.Update(dt_proZLdetail);

                            //    da = new SqlDataAdapter(cmd2);
                            //    new SqlCommandBuilder(da);
                            //    da.Update(t);
                  
                            //    ts.Commit();
                            //    MessageBox.Show("撤销成功");
                            //    barLargeButtonItem1_ItemClick(null, null);
                            //}
                            //catch (Exception ex)
                            //{
                            //    ts.Rollback();
                            //    throw new Exception(ex.Message + "撤销失败");
                            //}
                        }
                        else
                        {
                            throw new Exception("该制令有已转工单，不能撤回");
                        }


                    }
                }
                else
                {
                    MessageBox.Show("选择的记录不正确");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page.Name == "xtraTabPage2")
            {
                barLargeButtonItem5.Caption = "转工单";
                barLargeButtonItem13.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                barLargeButtonItem5.Caption = "生效";
                barLargeButtonItem13.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }



        //private void 关闭制令ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (MessageBox.Show("确定关闭该制令单？请核对。", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
        //    {

        //        DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
        //    }
        //}
    }
}
