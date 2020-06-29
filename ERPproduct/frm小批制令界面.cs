using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ERPproduct
{
    public partial class frm小批制令界面 : UserControl
    {
        public frm小批制令界面()
        {
            InitializeComponent();
            strconn = CPublic.Var.strConn;
        }

        #region 成员
        //数据库连接字符串


        string cfgfilepath = "";
        //有变化的做保存
        string strconn = "";
        DataRow drr = null;
        string str_制令 = "";
        string str_制令单 = "";
        DataTable dt_视图权限;
        public Boolean a;
        DataTable dt_proZLysx;
        bool flag = false;   //用以标记是否是是改制工单  
        DataTable dt_计划池; //用以减去计划池相应数量
        #endregion

        #region 自用类

        public frm小批制令界面(DataRow r, string str)
        {
            InitializeComponent();
            strconn = CPublic.Var.strConn;
            drr = r;
            str_制令 = str;
        }


        public frm小批制令界面(string str_制令单号)
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


        #endregion

        #region  变量

        /// <summary>
        /// 生产制令的主表
        /// </summary>
        DataTable dt_proZL;

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
        /// 勾选的用于生效制令的DT
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
                       base.图纸编号,车间编号,库存总数,新数据
                       from 基础数据物料信息表 base,仓库物料数量表 kc
                where   base.物料编码=kc.物料编码 and base.自制=1 and base.停用=0 and kc.仓库号  
                in (select  属性字段1 as 仓库号 from  基础数据基础属性表 where 属性类别='仓库类别' and 布尔字段1=1 )";//base.物料类型<>'原材料'
                dt_wuliao = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_wuliao);
                repositoryItemSearchLookUpEdit1.DataSource = dt_wuliao;
                // rsl.PopulateColumns();
                repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
                repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";


                fun_search3();


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

                string sql = "";
                SqlDataAdapter da;
                if (str_制令单 != "")
                {
                    if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserTeam == "管理员权限")
                    {
                        sql = string.Format(@"select sczl.*,库存总数,新数据  from 生产记录生产制令表 sczl
                                               left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                                left join  仓库物料数量表 kc   on    仓库物料数量表.物料编码= sczl.物料编码
                                                where sczl.生产制令单号='{0}' and sczl.关闭 = 0                 
                                                and sczl.生效 = 0 and sczl.完成 = 0  and sczl.仓库号=kc.仓库号 ", str_制令单);

                    }
                    else  //未排单
                    {
                        sql = string.Format(@"select sczl.*,库存总数,新数据  from 生产记录生产制令表  sczl
                                            left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                             left join  仓库物料数量表 kc  on    kc.物料编码= sczl.物料编码 and sczl.仓库号=kc.仓库号
                                                where sczl.生产制令单号='{0}' and sczl.关闭 = 0                 
                                                and sczl.生效 = 0 and sczl.完成 = 0   and sczl.生产制令类型='小批试制' ", str_制令单, CPublic.Var.LocalUserID);
                    }
                    da = new SqlDataAdapter(sql, strconn);
                    dt_proZL = new DataTable();
                    da.Fill(dt_proZL);
                    dt_proZL.Columns.Add("选择", typeof(bool));
                    dt_proZL.Columns.Add("反馈备注");

                }
                else
                {
                    if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserTeam == "管理员权限")
                    {
                        sql = @"select sczl.* ,库存总数,新数据  from 生产记录生产制令表 sczl 
                                 left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                             left join  仓库物料数量表 kc  on    kc.物料编码= sczl.物料编码 
                                                where sczl.生效 = 0 and sczl.完成 = 0  and sczl.关闭 = 0 and sczl.仓库号=kc.仓库号 and sczl.生产制令类型='小批试制' ";
                    }

                    else  //未排单
                    {//sql = "select * from 生产记录生产制令表 where 生效=0 and 完成=0";
                        sql = string.Format(@"select sczl.* ,库存总数,新数据  from 生产记录生产制令表 sczl
                                         left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码  
                                         left join  仓库物料数量表 kc  on kc.物料编码= sczl.物料编码   and sczl.仓库号=kc.仓库号
                                        where sczl.生效 = 0 and sczl.完成 = 0  and sczl.关闭 = 0 
                                          and sczl.生产制令类型='小批试制'", CPublic.Var.LocalUserID);
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
                    dt_proZL.Columns.Add("选择", typeof(bool));
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
                        dt_wuliao.Rows.Add(r["物料编码"], r["新数据"], r["物料名称"], r["物料类型"], r["规格型号"], r["图纸编号"], r["生产车间"], r["仓库号"], r["仓库名称"]);
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
                SqlDataAdapter da;
                string sql = "";
                if (date_前.EditValue != null && date_前.EditValue.ToString() != "" && date_后.EditValue != null && date_后.EditValue.ToString() != "")
                {
                    sql = string.Format("and sczl.生效日期 >= '{0}' and sczl.生效日期 <= '{1}'", Convert.ToDateTime(date_前.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(date_后.EditValue).AddDays(1).ToString("yyyy-MM-dd"));
                }
                if (CPublic.Var.LocalUserID == "admin" || CPublic.Var.LocalUserTeam == "公司高管权限")//|| CPublic.Var.LocalUserID == "2101" || CPublic.Var.LocalUserID == "2233" || CPublic.Var.LocalUserID == "4136" || CPublic.Var.LocalUserID == "2106"
                {
                    sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数,新数据  from 生产记录生产制令表 sczl
        left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
        left join  仓库物料数量表 kc   on    kc.物料编码= sczl.物料编码
        left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  on sczl.生产制令单号=a.生产制令单号
       where  sczl.关闭=0 and sczl.未排单数量>0 and sczl.生效 = 1 and sczl.关闭=0 and sczl.完成=0   and sczl.生效日期 >= '2016-12-1' and  sczl.仓库号=kc.仓库号 {0} and sczl.生产制令类型='小批试制' ", sql);
                }
                else
                {
                    sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数,新数据   from 生产记录生产制令表 sczl
                                           left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                              left join  仓库物料数量表 kc on    kc.物料编码= sczl.物料编码
                           left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                                    on sczl.生产制令单号=a.生产制令单号
                                 where  sczl.生效 = 1 and  (操作人员ID='{0}'  or 生产制令类型='销售备库') and  sczl.未排单数量>0  
                                    and sczl.生效日期 >= '2016-12-1' and sczl.关闭=0 and sczl.完成=0   and sczl.仓库号=kc.仓库号  {1} and sczl.生产制令类型='小批试制' ", CPublic.Var.LocalUserID, sql);
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
                r["生产制令类型"] = "小批试制";
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
                string str_name = CPublic.Var.localUserName;
                string str_id = CPublic.Var.LocalUserID;
                foreach (DataRow r in dt_proZL.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["生产制令类型"].ToString() == "")
                        throw new Exception("生产制令类型不能为空，请选择！");
                    if (r["物料编码"].ToString() == "")
                        throw new Exception("物料编码不能为空，请选择！");
                    if (r["制令数量"].ToString() == "")
                        throw new Exception("制令数量不能为空，请填写！");
                    r["未排单数量"] = r["制令数量"];
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
                        //  r["项目名称"]=repositoryItemSearchLookUpEdit3
                        r["日期"] = t;
                        r["制单人员"] = str_name;
                        r["制单人员ID"] = str_id;
                    }

                    //DataRow[] t = dt_proZLdetail.Select(string.Format("生产制令单号='{0}'", r["生产制令单号"].ToString()));
                    //foreach (DataRow r1 in t)
                    //{
                    //    r["销售订单明细号"] = r["销售订单明细号"].ToString() + r1["销售订单明细号"] + "|";
                    //    r["销售订单号"] = r["销售订单号"].ToString() + r1["销售订单号"] + "|";
                    //}

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
                SqlDataAdapter da;
                string sql = "select * from 生产记录生产制令表 where 1<>1";
                da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_proZL);
                //制令明细表
                sql = "select * from 生产记录生产制令子表 where 1<>1";
                da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_proZLdetail);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region   界面操作


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
                foreach (DataRow r in dt_proZLdetail.Rows)
                {
                    if (r.RowState == DataRowState.Added)
                        throw new Exception(string.Format("生产制令单号\"{0}\"中的销售订单明细号\"{1}\"是新增的,\n请先执行保存操作,或者删除明细操作后，再生效", r["生产制令单号"].ToString(), r["销售订单明细号"].ToString()));
                }
                //物料是否有效
                foreach (DataRow r in dt_SXZL.Rows)
                {
                    if (r["预完工日期"].ToString() == "")
                        throw new Exception("请填写预完工日期");
                    //物料是否有效
                    string sql_物料是否有效 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                    DataTable dt_基础物料 = new DataTable();
                    dt_基础物料 = CZMaster.MasterSQL.Get_DataTable(sql_物料是否有效, CPublic.Var.strConn);
                    if (dt_基础物料.Rows.Count == 0)
                    {
                        throw new Exception(string.Format("物料'{0}'无效，基础数据物料信息表中不存在该物料信息", r["物料编码"].ToString()));
                    }

                    //物料是否初始化
                    //string sql_物料是否初始化 = string.Format("select * from 仓库物料表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                    //DataTable dt_物料是否初始化 = new DataTable();
                    //dt_物料是否初始化 = CZMaster.MasterSQL.Get_DataTable(sql_物料是否初始化, CPublic.Var.strConn);
                    //if (dt_物料是否初始化.Rows.Count == 0)
                    //{
                    //    throw new Exception(string.Format("物料'{0}'无效，仓库物料表中不存在该物料信息", r["物料编码"].ToString()));
                    //}
                    str = str + StockCore.StockCorer.fun_flag(r["物料编码"].ToString(), false);

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
                    string sql = string.Format("select * from 生产记录生产制令子表 where 生产制令单号='{0}'", r["生产制令单号"].ToString());
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
                            r["生效日期"] = CPublic.Var.getDatetime(); ;
                        }
                    }
                }

                DataTable dt_关联物料 = fun_关联物料(dtx);

                SqlDataAdapter da;
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("ZLSX");
                SqlCommand cmd2 = new SqlCommand("select * from 销售记录销售订单明细表 where 1<>1", conn, ts);

                //SqlCommand cmd_计划池 = new SqlCommand("select * from 生产记录生产计划表 where 1<>1", conn, ts);
                SqlCommand cmd = new SqlCommand("select * from 生产记录生产制令表 where 1<>1", conn, ts);
                SqlCommand cmd1 = new SqlCommand("select * from 生产记录生产制令子表 where 1<>1", conn, ts);
                SqlCommand cmd3 = new SqlCommand("select * from 计划人员关联物料表 where 1<>1", conn, ts);

                try
                {
                    if (flag == false)
                    {
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_proZL);


                    }
                    da = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da);
                    da.Update(dt_proZLdetail);

                    da = new SqlDataAdapter(cmd2);
                    new SqlCommandBuilder(da);
                    da.Update(t);
                    da = new SqlDataAdapter(cmd3);
                    new SqlCommandBuilder(da);
                    da.Update(dt_关联物料);

                    cmd3 = new SqlCommand("select * from 采购记录采购送检单明细表 where 1<>1", conn, ts);
                    da = new SqlDataAdapter(cmd3);
                    new SqlCommandBuilder(da);
                    da.Update(dt_待检);

                    ts.Commit();
                }
                catch
                {
                    ts.Rollback();
                    throw new Exception("生效失败");
                }

                //17/9/19   弃用
                //foreach (DataRow r in dt_SXZL.Rows)
                //{
                //    ////刷新在制量
                //    //StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(), true);
                //    //改变已完成数量，未完成数量
                //    Decimal de = 0;
                //    if (Convert.ToDecimal(r["制令数量"]) > Convert.ToDecimal(r["计划生产量"]))
                //        de = Convert.ToDecimal(r["计划生产量"]);
                //    else
                //        de = Convert.ToDecimal(r["制令数量"]);
                //  //  StockCore.StockCorer.fun_生产制令_生效(r["物料编码"].ToString(), Convert.ToDecimal(r["制令数量"]), de, r["生产制令类型"].ToString(), r["生产计划单号"].ToString(), strconn);
                //    ////计算待领量，并保存  刷新未领量
                //    //StockCore.StockCorer.fun_生产制令_待领料(r["生产制令单号"].ToString(), strconn);
                //}
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
        /// <summary>
        /// Check the list to be checked
        /// </summary>

        #endregion

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

        #endregion

        #region 行变化，着色时间，行点击事件，右键查看BOM
        //选中行位置的变化事件
#pragma warning disable IDE1006 // 命名样式
        private void gv_sczlmain_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

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
        private void gv_sczlmain_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
            if (dr != null)
            {
                string sql = string.Format(@"select 生产记录生产制令子表.*,[销售记录销售订单明细表].备注,反馈备注,原ERP物料编号 from 生产记录生产制令子表,销售记录销售订单明细表,基础数据物料信息表
    
                                        where 生产记录生产制令子表.销售订单明细号 =销售记录销售订单明细表.销售订单明细号  and 生产记录生产制令子表.物料编码=基础数据物料信息表.物料编码

                                            and  生产制令单号='{0}'", dr["生产制令单号"].ToString().Trim());
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    dt_dispalymx = new DataTable();
                    da.Fill(dt_dispalymx);
                    gc_关联订单.DataSource = dt_dispalymx;
                }

                if (e != null && e.Button == MouseButtons.Right)
                {
                    //contextMenuStrip2.Show(gc_未生效制令, new Point(e.X, e.Y));
                    gv_未生效制令.CloseEditor();
                    this.BindingContext[dt_proZL].EndCurrentEdit();

                }
                if (dr.RowState != DataRowState.Added)
                {


                    foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_未生效制令.Columns)
                    {
                        if (dc.FieldName != "选择" && dc.FieldName != "备注" && dc.FieldName != "预完工日期" && dc.FieldName != "加急状态" && dc.FieldName != "反馈备注" && dc.FieldName != "项目号" && dc.FieldName != "项目名称" && dc.FieldName != "物料编码" && dc.FieldName != "制令数量")
                        {
                            dc.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            dc.OptionsColumn.AllowEdit = true;
                        }
                    }
                }
                else
                {
                    foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_未生效制令.Columns)
                    {
                        if (dc.FieldName != "预完工日期" && dc.FieldName != "选择" && dc.FieldName != "备注" && dc.FieldName != "反馈备注"
                            && dc.FieldName != "制令数量" && dc.FieldName != "物料编码" && dc.FieldName != "加急状态")
                        {
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
                foreach (DataRow r in dt_制令对应未领量.Rows)
                {
                    // StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(), true);
                }
            }
        }
        #endregion

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
        private void 改制工单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_choseZLSX();
                fun_checkZLSX();
                DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                frm改制工单 fm = new frm改制工单(dr);
                fm.ShowDialog();
                if (fm.a.Equals(true))
                {
                    flag = true;
                    fun_Shengxiao();
                    MessageBox.Show("生效成功");
                    barLargeButtonItem1_ItemClick(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            if (e.Button == MouseButtons.Left)
            {
                int[] dr = gv_未生效制令.GetSelectedRows();
                if (dr.Length > 1)
                {
                    for (int i = 0; i < dr.Length; i++)
                    {
                        DataRow r = gv_未生效制令.GetDataRow(dr[i]);
                        if (r["选择"].Equals(true))
                        {
                            r["选择"] = 0;

                        }
                        else
                        {
                            r["选择"] = 1;
                        }

                    }
                    //gridView1.FocusedRowHandle = dr[dr.Length - 1];
                    gv_未生效制令.MoveBy(dr[dr.Length - 1]);
                }
            }
        }



        private void 查看工单状态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
            ERPproduct.frm查看制令相关工单的状态 fm = new ERPproduct.frm查看制令相关工单的状态(dr["生产制令单号"].ToString());
            CPublic.UIcontrol.AddNewPage(fm, "工单状态查询");
        }



        private void 修改制令ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
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

                    }
                    if (fm.de_现 != 0)
                    {

                        dr["未排单数量"] = dr["制令数量"] = fm.de_现;
                        gv_已生效制令.CloseEditor();
                        this.BindingContext[dt_proZLysx].EndCurrentEdit();
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
                            da.Update(dt_proZLysx);
                            xgzl.Commit();
                            MessageBox.Show("修改成功");
                        }
                        catch (Exception)
                        {
                            xgzl.Rollback();
                            throw;
                        }



                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


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




#pragma warning disable IDE1006 // 命名样式
        private void rsl_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void rsl_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
        }















        #region 事件

#pragma warning disable IDE1006 // 命名样式
        private void fun_search3()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select 项目名称,项目号 from 基础信息项目管理表";
            DataTable dt_item = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            repositoryItemSearchLookUpEdit3.DataSource = dt_item;
            // rsl.PopulateColumns();
            repositoryItemSearchLookUpEdit3.DisplayMember = "项目名称";
            repositoryItemSearchLookUpEdit3.ValueMember = "项目号";



        }
#pragma warning disable IDE1006 // 命名样式
        private void frm研发制令界面_Load(object sender, EventArgs e)
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


                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "1")))
                {

                    gv_未生效制令.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "1"));
                }
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "2")))
                {

                    gv_关联订单.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "2"));
                }
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "3")))
                {

                    gv_已生效制令.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "3"));
                }

                // fun_search3();
                fun_searchMaterial();
                fun_loadsczlMain();
                fun_load已生效制令();

                if (drr != null)
                {
                    DataRow[] r = dt_proZL.Select(string.Format("生产制令单号='{0}' ", str_制令));

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




#pragma warning disable IDE1006 // 命名样式
        private void simpleButton3_Click_2(object sender, EventArgs e)
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
                // fm关联销售明细选择 fm = new fm关联销售明细选择(dt_proZLdetail, r["物料编码"].ToString(),r["生产制令单号"].ToString());
                fm关联销售明细选择 fm = new fm关联销售明细选择(dt_dispalymx, r["物料编码"].ToString(), r["生产制令单号"].ToString());

                fm.ShowDialog();
                if (fm.dt != null)
                {
                    dt_dispalymx = fm.dt;
                    gc_关联订单.DataSource = dt_dispalymx;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
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
        }//刷新

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (ActiveControl != null && ActiveControl.GetType().Equals(gc_已生效制令.GetType()))
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();



                    DevExpress.XtraGrid.GridControl gc = (ActiveControl) as DevExpress.XtraGrid.GridControl;

                    gc.ExportToXlsx(saveFileDialog.FileName);



                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {

                MessageBox.Show("若要导出请先选中要导出的表格");
            }
        }//导出

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
        }//新增

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
        }//删除

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
                barLargeButtonItem1_ItemClick(null, null);
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }//保存

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
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
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }//生效

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
        }//关闭制令

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }//关闭



#pragma warning disable IDE1006 // 命名样式
        private void gv_已生效制令_ColumnFilterChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //保存界面设置
            if (cfgfilepath != "")
            {
                gv_已生效制令.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "3"));
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_已生效制令_ColumnPositionChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //保存界面设置
            if (cfgfilepath != "")
            {
                gv_已生效制令.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "3"));
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_已生效制令_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

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
        private void gv_未生效制令_ColumnFilterChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //保存界面设置
                if (cfgfilepath != "")
                {
                    gv_未生效制令.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "1"));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_CellValueChanging_1(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            // /try
            //{
            //if (e.Column.FieldName == "项目号")
            //{

            //    string sql = string.Format("select * from 基础信息项目管理表 where 项目号='{0}'",e.Value.ToString());
            //    DataRow  dr = CZMaster.MasterSQL.Get_DataRow(sql,strconn);


            // }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_ColumnPositionChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                //保存界面设置
                if (cfgfilepath != "")
                {
                    gv_未生效制令.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "1"));
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
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

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_关联订单_ColumnFilterChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //保存界面设置
            if (cfgfilepath != "")
            {
                gv_关联订单.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "2"));
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_关联订单_ColumnPositionChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //保存界面设置
            if (cfgfilepath != "")
            {
                gv_关联订单.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name + "2"));
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_关联订单_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_关联订单.GetFocusedRowCellValue(gv_关联订单.FocusedColumn));
                e.Handled = true;
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load已生效制令();
            gv_已生效制令.ViewCaption = "未排单生效制令";
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
                sql = string.Format(@"select sczl.*,a.已转工单数,库存总数,新数据  from 生产记录生产制令表 sczl
                                                left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                     left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                                              on sczl.生产制令单号=a.生产制令单号
                                            left join 仓库物料数量表 kc on  kc.物料编码= sczl.物料编码
                                                where /*生产记录生产制令表.关闭=0 and*/    sczl.生效日期 >= '2016-12-1'and  sczl.仓库号=kc.仓库号 {0}and sczl.生产制令类型='小批试制' ", sql);
            }
            else
            {
                sql = string.Format(@"select sczl.*,a.已转工单数,库存总数,新数据  from 生产记录生产制令表 sczl
            left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
               left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
               on sczl.生产制令单号=a.生产制令单号
              left join 仓库物料数量表 kc on  kc.物料编码= sczl.物料编码
               where  /*生产记录生产制令表.关闭=0 and*/ sczl.生效日期 >= '2016-12-1' {0} and sczl.仓库号=kc.仓库号 and sczl.生产制令类型='小批试制' ", sql, CPublic.Var.LocalUserID);
                //and  操作人员ID='{1}' 
            }




            da = new SqlDataAdapter(sql, strconn);
            DataTable dt_proZLysx = new DataTable();
            da.Fill(dt_proZLysx);
            gc_已生效制令.DataSource = dt_proZLysx;



        }

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
                sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数 from 生产记录生产制令表  sczl
                    left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                    left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                    on sczl.生产制令单号=a.生产制令单号 left join 仓库物料数量表 kc on   kc.物料编码= sczl.物料编码
                      where sczl.生产制令单号 in ( select 生产制令单号   from 生产记录生产工单表 
                      where  生产记录生产工单表.生效=1 and 生产记录生产工单表.关闭 =0  and  生产记录生产工单表.检验完成=0 group by 生产制令单号)    
                     and sczl.关闭=0 and sczl.仓库号=kc.仓库号   and sczl.生效日期 >= '2016-12-1' {0} and sczl.生产制令类型='小批试制' ", sql);
            }
            else
            {
                sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数 from 生产记录生产制令表 sczl
                          left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                         left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                          on sczl.生产制令单号=a.生产制令单号  left join 仓库物料数量表 kc on  kc.物料编码= sczl.物料编码
                        where sczl.生产制令单号 in ( select 生产制令单号   from 生产记录生产工单表 
                             where   生产记录生产工单表.生效=1 and  生产记录生产工单表.关闭 =0  and 生产记录生产工单表.检验完成=0 group by 生产制令单号)    
                             and sczl.关闭=0  and sczl.生效日期 >= '2016-12-1' {0} and sczl.仓库号=kc.仓库号 and   操作人员ID='{1}' and sczl.生产制令类型='小批试制'", sql, CPublic.Var.LocalUserID);
            }




            da = new SqlDataAdapter(sql, strconn);
            DataTable dt_proZLysx = new DataTable();
            da.Fill(dt_proZLysx);
            gc_已生效制令.DataSource = dt_proZLysx;
        }

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
                sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数, 库存总数 from 生产记录生产制令表 sczl 
                         left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                                 left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                          on sczl.生产制令单号=a.生产制令单号  left join 仓库物料数量表 kc on  kc.物料编码= sczl.物料编码
                           where sczl.生产制令单号 in ( select  生产记录生产工单表.生产制令单号  from 生产记录生产工单表
                         left join  生产记录生产制令表 sczl on sczl.生产制令单号=生产记录生产工单表.生产制令单号 
                           where 生产记录生产工单表.关闭=0 and 检验完成=1 and 生产记录生产工单表.完成=0 and sczl.完成=0  and sczl.生效日期 >='2016-12-1' {0} group by 生产记录生产工单表.生产制令单号) 
                    and sczl.关闭=0  and sczl.生效日期 >='2016-12-1' and sczl.仓库号=kc.仓库号 {0} and sczl.生产制令类型='小批试制' ", sql);
            }
            else
            {
                sql = string.Format(@"select sczl.*,isnull(a.已转工单数,0)已转工单数,库存总数 from 生产记录生产制令表 sczl
                          left join  基础数据物料信息表 base on  base.物料编码=sczl.物料编码
                            left join (select sum(生产数量) as 已转工单数,生产制令单号  from  生产记录生产工单表 where 关闭= 0  group by 生产制令单号) a  
                            on sczl.生产制令单号=a.生产制令单号  left join 仓库物料数量表 kc on  kc.物料编码= sczl.物料编码
                                where sczl.生产制令单号 in (select  生产记录生产工单表.生产制令单号  from 生产记录生产工单表
         left join  生产记录生产制令表 sczl on sczl.生产制令单号=生产记录生产工单表.生产制令单号 
                           where 生产记录生产工单表.关闭=0 and 检验完成=1 and 生产记录生产工单表.完成=0 and sczl.完成=0  and sczl.生效日期 >='2016-12-1' {0} group by 生产记录生产工单表.生产制令单号) 
                            and sczl.生效日期 >= '2016-12-1' and sczl.关闭=0 {0} and sczl.仓库号=kc.仓库号 and  sczl.操作人员ID='{1}'and sczl.生产制令类型='小批试制'", sql, CPublic.Var.LocalUserID);
            }
            da = new SqlDataAdapter(sql, strconn);
            DataTable dt_proZLysx = new DataTable();
            da.Fill(dt_proZLysx);
            gc_已生效制令.DataSource = dt_proZLysx;

        }







        #endregion

        private void 查看物料BOMToolStripMenuItem_Click_1(object sender, EventArgs e)
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

        private void 查看工单状态ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
            ERPproduct.frm查看制令相关工单的状态 fm = new ERPproduct.frm查看制令相关工单的状态(dr["生产制令单号"].ToString());
            CPublic.UIcontrol.AddNewPage(fm, "工单状态查询");
        }

        private void 修改制令ToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            try
            {
                gv_已生效制令.CloseEditor();
                this.BindingContext[dt_proZLysx].EndCurrentEdit();
                DataRow dr = gv_已生效制令.GetDataRow(gv_已生效制令.FocusedRowHandle);
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

                    }
                    if (fm.de_现 != 0)
                    {
                        DataTable t_save = dt_proZLysx.Clone();
                        t_save.ImportRow(dr);
                        t_save.Rows[0]["未排单数量"] = t_save.Rows[0]["制令数量"] = fm.de_现;

                        // dr["未排单数量"] = dr["制令数量"] = fm.de_现;


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

                            string s = string.Format(@"select sczl.*,a.已转工单数,库存总数,新数据  from 生产记录生产制令表 sczl
            left join  基础数据物料信息表 base on  base.物料编码 = sczl.物料编码
               left join(select sum(生产数量) as 已转工单数, 生产制令单号  from 生产记录生产工单表 where 关闭 = 0 group by 生产制令单号) a
               on sczl.生产制令单号 = a.生产制令单号
              left join 仓库物料数量表 kc on  kc.物料编码 = sczl.物料编码 and sczl.仓库号 = kc.仓库号
               where   sczl.生产制令单号='{0}'  ", dr["生产制令单号"]);
                            DataRow rr = CZMaster.MasterSQL.Get_DataRow(s, strconn);
                            dr.ItemArray = rr.ItemArray;
                            dr.AcceptChanges();
                        }
                        catch (Exception)
                        {
                            xgzl.Rollback();
                            throw;
                        }



                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void 查看BOMToolStripMenuItem_Click_1(object sender, EventArgs e)
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

        private void 查看过往制令ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DataRow r = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);

            UI查看制令列表 ui = new UI查看制令列表(r["物料编码"].ToString().Trim());
            CPublic.UIcontrol.AddNewPage(ui, "过往制令列表");
        }

        private void 过往通知出库记录ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DataRow r = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);

            UI查看出库通知明细 ui = new UI查看出库通知明细(r["物料编码"].ToString().Trim());
            CPublic.UIcontrol.AddNewPage(ui, "过往通知出库记录");
        }

        private void 改制工单ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                gv_未生效制令.CloseEditor();
                this.BindingContext[dt_proZL].EndCurrentEdit();
                fun_choseZLSX();
                fun_checkZLSX();
                DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
                frm改制工单 fm = new frm改制工单(dr);
                fm.ShowDialog();
                if (fm.a.Equals(true))
                {
                    flag = true;
                    fun_Shengxiao();
                    MessageBox.Show("生效成功");
                    barLargeButtonItem1_ItemClick(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 修改制令ToolStripMenuItem_Click_1(object sender, EventArgs e)
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
        private void rsl_RowCellClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
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
                rr["特殊备注"] = dr["特殊备注"];
                rr["仓库号"] = dr["仓库号"];
                rr["仓库名称"] = dr["仓库名称"];
                rr["新数据"] = dr["新数据"];
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void rsl_RowClick_1(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
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
                rr["特殊备注"] = dr["特殊备注"];
                rr["仓库号"] = dr["仓库号"];
                rr["仓库名称"] = dr["仓库名称"];
                rr["新数据"] = dr["新数据"];
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gv_未生效制令.GetDataRow(gv_未生效制令.FocusedRowHandle);
            if (dr != null)
            {
                string sql = string.Format(@"select 生产记录生产制令子表.*,[销售记录销售订单明细表].备注,反馈备注,原ERP物料编号 from 生产记录生产制令子表,销售记录销售订单明细表,基础数据物料信息表
    
                                        where 生产记录生产制令子表.销售订单明细号 =销售记录销售订单明细表.销售订单明细号  and 生产记录生产制令子表.物料编码=基础数据物料信息表.物料编码

                                            and  生产制令单号='{0}'", dr["生产制令单号"].ToString().Trim());
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
                if (dr.RowState != DataRowState.Added)
                {


                    foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_未生效制令.Columns)
                    {
                        if (dc.FieldName != "选择" && dc.FieldName != "备注" && dc.FieldName != "预完工日期" && dc.FieldName != "加急状态" && dc.FieldName != "反馈备注")
                        {
                            dc.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            dc.OptionsColumn.AllowEdit = true;
                        }
                    }
                }
                else
                {
                    foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gv_未生效制令.Columns)
                    {
                        if (dc.FieldName != "预完工日期" && dc.FieldName != "选择" && dc.FieldName != "备注" && dc.FieldName != "反馈备注"
                            && dc.FieldName != "制令数量" && dc.FieldName != "物料编码" && dc.FieldName != "加急状态")
                        {
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
        private void gv_未生效制令_MouseUp_1(object sender, MouseEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Button == MouseButtons.Left)
            {
                int[] dr = gv_未生效制令.GetSelectedRows();
                if (dr.Length > 1)
                {
                    for (int i = 0; i < dr.Length; i++)
                    {
                        DataRow r = gv_未生效制令.GetDataRow(dr[i]);
                        if (r["选择"].Equals(true))
                        {
                            r["选择"] = 0;

                        }
                        else
                        {
                            r["选择"] = 1;
                        }

                    }
                    //gridView1.FocusedRowHandle = dr[dr.Length - 1];
                    gv_未生效制令.MoveBy(dr[dr.Length - 1]);
                }
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_未生效制令_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
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
        private void rsl_RowClick_3(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
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
                rr["特殊备注"] = dr["特殊备注"];
                rr["仓库号"] = dr["仓库号"];
                rr["仓库名称"] = dr["仓库名称"];
                rr["新数据"] = dr["新数据"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



    }
}
