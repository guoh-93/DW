using CZMaster;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class frm采购送检单界面 : UserControl
    {
        /// <summary>
        /// 数据库的连接字符串
        /// </summary>
        string strcon = "";
        DateTime time = CPublic.Var.getDatetime().Date;
        /// <summary>
        /// 采购送检单号
        /// </summary>
        string strSongjianDan = "";
        DataTable dt_检验员;
        /// <summary>
        /// 这个标记 供应商送检数量是否大于采购数量10%
        /// true 要记录该供应商的 信息
        /// </summary>
        bool bl = false;

        public frm采购送检单界面()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        public frm采购送检单界面(string songjiandan)
        {
            strSongjianDan = songjiandan;
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }



        /// <summary>
        /// 送检单
        /// </summary>
        string strSongjian = "";

        /// <summary>
        /// 操作的行
        /// </summary>
        DataRow drm = null;

        /// <summary>
        /// 送检的采购单列表
        /// </summary>
        DataTable dt_sjPurchaseList;

        /// <summary>
        /// 采购送检单的明细表
        /// </summary>
        DataTable dt_songjianMx;

        /// <summary>
        /// 采购送检单的主表
        /// </summary>
        DataTable dt_songjianZb;

        /// <summary>
        /// 供应商的DT
        /// </summary>
        DataTable dt_gys;

        /// <summary>
        /// 送检人员
        /// </summary>
        DataTable dt_people;

        /// <summary>
        /// 采购权限的dt
        /// </summary>
        DataTable dt_cgquanxian;

        DataView dv;

        /// <summary>
        /// 基础数据的准备
        /// </summary>
        private void fun_getBaseData()
        {
            try
            {
                string sql = "";
                sql = "select 供应商ID,供应商名称,供应商负责人,供应商电话 from 采购供应商表 order by 供应商ID";
                dt_gys = MasterSQL.Get_DataTable(sql, strcon);
                dt_gys.Rows.Add(dt_gys.NewRow());
                txt_gysID.Properties.DataSource = dt_gys;
                txt_gysID.Properties.ValueMember = "供应商ID";
                txt_gysID.Properties.DisplayMember = "供应商ID";

                sql = "select 员工号,姓名,手机,部门 from 人事基础员工表  where 在职状态='在职' order by 员工号";
                dt_people = MasterSQL.Get_DataTable(sql, strcon);
                txt_songjianrenID.Properties.DataSource = dt_people;
                txt_songjianrenID.Properties.ValueMember = "员工号";
                txt_songjianrenID.Properties.DisplayMember = "员工号";

                //采购权限
                dt_cgquanxian = ERPorg.Corg.fun_hr("采购", CPublic.Var.LocalUserID);
                fun_默认检验员();
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_getBaseData");
                throw ex;
            }
        }

        //选择员工，变化姓名
        private void txt_songjianrenID_EditValueChanged(object sender, EventArgs e)
        {

        }


        private void fun_ChangeLoadPurchaseList()
        {
            try
            {
                if (txt_gysID.EditValue.ToString() != "")
                {
                    dv = new DataView(dt_sjPurchaseList);
                    dv.RowFilter = string.Format("供应商ID={0}", txt_gysID.EditValue.ToString());
                }
                else
                {
                    dv = new DataView(dt_sjPurchaseList);
                }



            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_ChangeLoadPurchaseList");
                throw ex;
            }
        }


        /// <summary>
        /// 载入采购单列表:已生效的采购单
        /// </summary>
        private void fun_load采购单列表()
        {
            try
            {
                SqlDataAdapter da;
                string sql = "";
                dt_sjPurchaseList = new DataTable();
                DateTime time = CPublic.Var.getDatetime();
                //string ss = "('',";
                //if (dt_cgquanxian.Rows.Count > 0)
                //{
                //    foreach (DataRow r in dt_cgquanxian.Rows)
                //    {
                //        ss = ss + "'" + r["工号"].ToString() + "'" + ',';
                //    }
                //    ss = ss.Substring(0, ss.Length - 1) + ")";
                //}
                //else
                //{
                //    ss = "('')";

                //}

                //19-10-10 
                string s_add = "";

                if (CPublic.Var.LocalUserTeam == "开发部权限" || CPublic.Var.localUser部门名称.Contains("开发"))
                {

                    s_add = " and  采购单类型='开发采购' ";

                }
                //2020-3-31
                string s0 = txt_gysID.EditValue.ToString();
                DateTime time1 = time.AddMonths(-3).Date;
                DateTime time2 = time.Date;
                string s_补条件 = "and 明细类型 not in('借用出库','拆单申请出库','形态转换出库')";
                //and 采购单类型  in ('','资产采购','委外加工','普通采购','计划类型','委外采购','开发采购')  
                if (txt_gysID.EditValue.ToString() != "")
                {
                    sql = $@"select cmx.*,参考量,存货分类编码,采购单类型,采购供应商备注,库存总数,季度用量  from 采购记录采购单明细表 cmx
                                                left join    基础数据物料信息表 base on base.物料编码=cmx.物料编码
                                            left join  采购记录采购单主表 cz on cz.采购单号=cmx.采购单号     
                                          left join 仓库物料数量表 kc on kc.物料编码=cmx.物料编码  and kc.仓库号=cmx.仓库号
                                    left   join  (select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库' {s_补条件}  and  出入库时间>'{time1}' and 
                                    出入库时间<'{time2}'  group by 物料编码)a on  cmx.物料编码=a.物料编码 
                                left  join 计划池物料参考量表   on 计划池物料参考量表.物料编码=cmx.物料编码
                             where cmx.生效=1 and cmx.明细完成 = 0 
                          and cmx.采购数量>cmx.已送检数 and cmx.作废 = 0   and cz.生效=1 and cz.作废=0 
                           and cmx.供应商ID='{s0}'  {s_add} ";
                }
                else
                {
                    // and 采购单类型  in ('','资产采购','委外加工','普通采购','计划类型','委外采购') 
                    sql = $@"select cmx.*,参考量,存货分类编码,采购单类型 ,采购供应商备注,库存总数,季度用量 from 采购记录采购单明细表 cmx
                                             left join  基础数据物料信息表 base on base.物料编码=cmx.物料编码
                                             left join  采购记录采购单主表 cz on cz.采购单号=cmx.采购单号 
                                         left join 仓库物料数量表 kc on kc.物料编码=cmx.物料编码   and kc.仓库号=cmx.仓库号
                                         left   join  (select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库' {s_补条件} and  出入库时间>'{time1}' and 
                                                 出入库时间<'{time2}'  group by 物料编码)a on  cmx.物料编码=a.物料编码 
                                      left  join 计划池物料参考量表 on 计划池物料参考量表.物料编码=cmx.物料编码
                                             where cmx.生效=1 and cmx.明细完成 = 0
                                             and cmx.作废 = 0   and cz.生效=1 and cz.作废=0   
                                             and cmx.采购数量>cmx.已送检数   {s_add} order by 到货日期 ";
                }
                //string sql_补 = "";
                //if (CPublic.Var.localUserName != "admin" && CPublic.Var.LocalUserTeam != "管理员权限")
                //{
                //    string aaa = CPublic.Var.localUser部门编号;

                //        sql_补 = " and (cmx.采购部门ID = '000107' or cmx.采购部门ID = '00010302' or cmx.采购部门ID='') order by 到货日期";
                //    sql = sql + sql_补;

                //}
                da = new SqlDataAdapter(sql, strcon);

                da.Fill(dt_sjPurchaseList);
                //dt_sjPurchaseList = MasterSQL.Get_DataTable(sql, strcon);  

                DateTime t2 = CPublic.Var.getDatetime();
                dt_sjPurchaseList.Columns.Add("选择", typeof(bool));
                dt_sjPurchaseList.Columns.Add("未送检数");
                // dt_sjPurchaseList.Columns.Add("开票税率");

                //把采购单明细表中选中过的赋值为true
                foreach (DataRow r in dt_sjPurchaseList.Rows)
                {
                    r["未送检数"] = Convert.ToDecimal(r["采购数量"]) - Convert.ToDecimal(r["已送检数"]);
                    if (dt_songjianMx != null)
                    {
                        DataRow[] dr = dt_songjianMx.Select(string.Format("采购单明细号='{0}'", r["采购明细号"].ToString()));
                        if (dr.Length > 0)
                        {
                            r["选择"] = true;
                            dr[0]["可送检数量"] = Convert.ToDecimal(r["采购数量"]) - Convert.ToDecimal(r["已送检数"]);
                        }
                    }
                }
                if (dt_songjianMx != null)
                {
                    for (int i = dt_songjianMx.Rows.Count - 1; i >= 0; i--)
                    {
                        if (dt_songjianMx.Rows[i].RowState == DataRowState.Deleted) continue;
                        DataRow[] dr = dt_sjPurchaseList.Select(string.Format("采购明细号='{0}'", dt_songjianMx.Rows[i]["采购单明细号"].ToString()));
                        if (dr.Length <= 0)
                        {
                            dt_songjianMx.Rows[i].Delete();
                        }
                    }
                    //foreach (DataRow r in dt_songjianMx.Rows)
                    //{
                    //    if (r.RowState == DataRowState.Deleted) continue;
                    //    DataRow[] dr = dt_sjPurchaseList.Select(string.Format("采购明细号='{0}'", r["采购单明细号"].ToString()));
                    //    if (dr.Length <= 0)
                    //    {
                    //        r.Delete();
                    //    }
                    //}
                }
                DateTime t3 = CPublic.Var.getDatetime();

                gc_purchaselist.DataSource = dt_sjPurchaseList;
                //dt_sjPurchaseList.ColumnChanged += dt_sjPurchaseList_ColumnChanged;

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_load采购单列表");
                throw ex;
            }

        }
        private void frm采购送检单界面_Load(object sender, EventArgs e)
        {
            try
            {

                fun_getBaseData();
                if (strSongjianDan == "")
                {
                    txt_songjianrenID.EditValue = CPublic.Var.LocalUserID;
                    txt_songjiantime.EditValue = CPublic.Var.getDatetime();
                    txt_gysID.EditValue = "";   ///这里会触发   fun_load采购单列表();
                    string sql = "";
                    //主表
                    sql = "select * from 采购记录采购送检单主表 where 1<>1";
                    dt_songjianZb = MasterSQL.Get_DataTable(sql, strcon);
                    //明细表
                    sql = "select * from 采购记录采购送检单明细表 where 1<>1";
                    dt_songjianMx = MasterSQL.Get_DataTable(sql, strcon);
                    //dt_songjianMx.Columns.Add("输入送检数量");
                    dt_songjianMx.Columns.Add("可送检数量");
                    dt_songjianMx.Columns.Add("原ERP物料编号");
                    dt_songjianMx.Columns.Add("存货分类编码");
                    dt_songjianMx.Columns.Add("采购单类型");


                    // dt_songjianMx.Columns.Add("开票税率");


                    drm = dt_songjianZb.NewRow();
                    gc_songjianmx.DataSource = dt_songjianMx;


                }
                else
                {
                    fun_searchData(strSongjianDan);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_默认检验员()
        {
            string sql = "select 员工号,姓名 from 人事基础员工表  where 课室='品管部'  and 在职状态='在职'";
            dt_检验员 = new DataTable();
            dt_检验员 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            searchLookUpEdit1.Properties.DataSource = dt_检验员;
            searchLookUpEdit1.Properties.DisplayMember = "员工号";
            searchLookUpEdit1.Properties.ValueMember = "员工号";

        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                textBox2.Text = dt_检验员.Select(string.Format("员工号='{0}'", searchLookUpEdit1.EditValue))[0]["姓名"].ToString();

            }
        }
        /// <summary>
        /// 数据的查询
        /// </summary>
        /// <param name="sjID">送检单号</param>
        private void fun_searchData(string sjID)
        {
            try
            {
                string sql = "";
                //查找采购送检单的主表的记录
                sql = string.Format("select * from 采购记录采购送检单主表 where 送检单号='{0}'", sjID);
                dt_songjianZb = MasterSQL.Get_DataTable(sql, strcon);
                if (dt_songjianZb.Rows.Count > 0)
                {
                    drm = dt_songjianZb.Rows[0];
                }
                dataBindHelper1.DataFormDR(drm);
                //查找采购送检单的明细表的记录
                sql = string.Format("select * from 采购记录采购送检单明细表 where 送检单号='{0}'   ", sjID);
                dt_songjianMx = MasterSQL.Get_DataTable(sql, strcon);
                // dt_songjianMx.Columns.Add("输入送检数量");
                dt_songjianMx.Columns.Add("可送检数量");
                // dt_songjianMx.Columns.Add("开票税率");

                dt_songjianMx.Columns.Add("原ERP物料编号");

                dt_songjianMx.Columns.Add("存货分类编码");
                dt_songjianMx.Columns.Add("采购单类型");

                gc_songjianmx.DataSource = dt_songjianMx;
                fun_load采购单列表();

                ////供应商ID和送检人ID就变成不可编辑的
                //txt_gysID.Enabled = false;
                //txt_songjianrenID.Enabled = false;
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_searchData");
                throw ex;
            }
        }

        void dt_sjPurchaseList_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            //try
            //{
            //    if (e.Column.ColumnName == "选择")
            //    {
            //        int count = 0;
            //        DataRow dr = gv_purchaselist.GetDataRow(gv_purchaselist.FocusedRowHandle);
            //        foreach (DataRow r in dt_sjPurchaseList.Rows)
            //        {
            //            if (r["选择"].Equals(true))
            //            {

            //                if (dt_songjianMx.Rows.Count > 0)
            //                {
            //                    DataRow[] dr1 = dt_songjianMx.Select(string.Format("采购单明细号='{0}'", r["采购明细号"].ToString()));
            //                    if (dr1.Length > 0)
            //                    {
            //                        //continue;
            //                    }
            //                }
            //                //else
            //                //{
            //                    DataRow r2 = dt_songjianMx.NewRow();
            //                    r2["采购单号"] = r["采购单号"];
            //                    r2["采购单明细号"] = r["采购明细号"];
            //                    r2["物料编码"] = r["物料编码"];
            //                    r2["物料名称"] = r["物料名称"];
            //                    r2["规格型号"] = r["规格型号"];
            //                    r2["供应商ID"] = r["供应商ID"];
            //                    r2["供应商"] = r["供应商"];
            //                    r2["供应商负责人"] = r["供应商负责人"];
            //                    r2["供应商电话"] = r["供应商电话"];
            //                    r2["物料编码"] = r["物料编码"];
            //                    r2["物料名称"] = r["物料名称"];
            //                    r2["图纸编号"] = r["图纸编号"];
            //                    r2["图纸版本"] = r["图纸版本"];
            //                    r2["规格型号"] = r["规格型号"];
            //                    r2["采购数量"] = r["采购数量"];
            //                    r2["税率"] = r["税率"];
            //                    r2["未税单价"] = r["未税单价"];
            //                    r2["单价"] = r["单价"];
            //                    r2["未税金额"] = r["未税金额"];
            //                    r2["金额"] = r["金额"];
            //                    r2["可送检数量"] = Convert.ToDecimal(r["采购数量"]) - Convert.ToDecimal(r["已送检数"]);
            //                    dt_songjianMx.Rows.Add(r2);
            //                    txt_gysID.EditValue = r["供应商ID"].ToString();
            //                    biaoji = 1;
            //                //}
            //            }
            //            else
            //            {
            //                if (dt_songjianMx.Rows.Count > 0)
            //                {
            //                    DataRow[] dr1 = dt_songjianMx.Select(string.Format("采购单明细号='{0}'", r["采购明细号"].ToString()));
            //                    foreach (DataRow t in dr1)
            //                    {
            //                        t.Delete();
            //                    }
            //                    //txt_gysID.EditValue = "";
            //                }
            //                count++;
            //            }
            //        }
            //        if (count == dt_sjPurchaseList.Rows.Count)
            //        {
            //            txt_gysID.EditValue = "";
            //        }
            //        else
            //        {

            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }


        #region 调用的方法
        //新增的方法
        private void fun_Newsongjiandan()
        {
            try
            {
                string str_供应商ID = "";
                int flag = 0;
                if (txt_gysID.EditValue.ToString() != "")
                {
                    str_供应商ID = txt_gysID.EditValue.ToString();
                    if (gv_purchaselist.RowCount >= 1)
                    {
                        flag = 1;
                    }
                }

                drm = dt_songjianZb.NewRow();
                dataBindHelper1.DataFormDR(drm);

                txt_songjianrenID.EditValue = CPublic.Var.LocalUserID;
                if (dt_songjianMx != null)
                {
                    dt_songjianMx.Clear();
                }
                //找到该明细号。
                fun_load采购单列表();
                if (flag == 1)
                {
                    txt_gysID.EditValue = str_供应商ID;
                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_Newsongjiandan");
                throw ex;
            }
        }

        /// <summary>
        /// 检测主表的数据的准确性
        /// </summary>
        private void fun_checkMainData()
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                if (drm["GUID"] == DBNull.Value)
                {
                    drm["GUID"] = System.Guid.NewGuid();
                    if (txt_songjiandan.Text == "")
                    {
                        txt_songjiandan.Text = string.Format("SJ{0}{1:00}{2:00}{3:0000}", t.Year, t.Month,
                        t.Day, CPublic.CNo.fun_得到最大流水号("SJ", t.Year, t.Month, t.Day));
                    }
                    drm["创建日期"] = t;
                    dt_songjianZb.Rows.Add(drm);
                }
                strSongjian = txt_songjiandan.Text;
                if (txt_gysID.EditValue.ToString() == "")
                {
                    txt_gysID.EditValue = dt_songjianMx.Rows[0]["供应商ID"].ToString();
                }

                drm["操作人员ID"] = CPublic.Var.LocalUserID;
                drm["操作人员"] = CPublic.Var.localUserName;
                drm["修改日期"] = t;
                dataBindHelper1.DataToDR(drm);
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_checkMainData");
                MessageBox.Show(ex.Message + "主表");
            }
        }

        /// <summary>
        /// 检查明细表的数据
        /// </summary>
        private void fun_checkTheDetail()
        {
            try
            {
                int pos = 0;
                foreach (DataRow r in dt_songjianMx.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    if (r["GUID"] == DBNull.Value)
                    {
                        r["GUID"] = System.Guid.NewGuid();
                    }
                    r["送检单号"] = strSongjian;
                    r["POS"] = ++pos;
                    r["送检单明细号"] = r["送检单号"].ToString() + "-" + pos.ToString("00");
                    r["送检人员ID"] = txt_songjianrenID.EditValue.ToString();
                    r["送检人员"] = txt_songjianrenName.Text;
                    r["送检日期"] = CPublic.Var.getDatetime();
                    if (r["送检数量"].ToString() == "")
                        throw new Exception(string.Format("明细号\"{0}\"输入的送检数量不能为空，请填写！", r["采购单明细号"].ToString()));
                    try
                    {
                        decimal dcm = Convert.ToDecimal(r["送检数量"]);
                    }
                    catch
                    {
                        throw new Exception(string.Format("明细号\"{0}\"输入的送检数量数数字，请检查！", r["采购单明细号"].ToString()));
                    }
                    //17/5/27 重新加回来  17/8/25 改为可大于采购数量 至多110%     但是 超过记录详细信息
                    //19-10-15 制定最小起订量  不能超过 采购单送检
                    decimal dec_送检数 = Convert.ToDecimal(r["送检数量"]);
                    decimal dec_可送检数 = Convert.ToDecimal(r["可送检数量"]);
                    decimal dec_采购数 = Convert.ToDecimal(r["采购数量"]);

                    if (dec_送检数 > dec_可送检数)
                    {
                        ///2019-10-15 
                        throw new Exception(string.Format("明细号\"{0}\"输入送检数量不可超出采购数量", r["采购单明细号"].ToString()));

                        //if ((dec_送检数 - dec_可送检数) / dec_采购数 > (decimal)0.1)
                        //{
                        //    throw new Exception(string.Format("明细号\"{0}\"输入送检数量不可超出采购数量10% ", r["采购单明细号"].ToString()));
                        //}
                        //else
                        //{
                        //    // 需要记录供应商送检数量超出采购单数量信息
                        //    bl = true;
                        //}
                    }
                    r["操作人员ID"] = CPublic.Var.LocalUserID;
                    r["操作人员"] = CPublic.Var.localUserName;
                }
                //给主表添加数据
                drm["物料编码"] = dt_songjianMx.Rows[0]["物料编码"].ToString();
                drm["物料名称"] = dt_songjianMx.Rows[0]["物料名称"].ToString();
                drm["送检数量"] = dt_songjianMx.Rows[0]["送检数量"].ToString();
                drm["采购单号"] = dt_songjianMx.Rows[0]["采购单号"].ToString();
                drm["采购单明细号"] = dt_songjianMx.Rows[0]["采购单明细号"].ToString();
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_checkTheDetail");

                //MessageBox.Show(ex.Message+"明细");
                throw new Exception(ex.Message);

            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        private void fun_SaveData()
        {
            try
            {
                string sql = "select * from 采购记录采购送检单主表 where  1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_songjianZb);
                }
                string sql_1 = "select * from 采购记录采购送检单明细表 where  1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strcon))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_songjianMx);
                }

                //MasterSQL.Save_DataTable(dt_songjianZb, "采购记录采购送检单主表", strcon);

                //MasterSQL.Save_DataTable(dt_songjianMx, "采购记录采购送检单明细表", strcon);

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_SaveData");
                throw ex;
            }
        }

        #endregion


        #region 界面操作
        //刷新操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //if (drm.RowState == DataRowState.Added)
                //{
                if (txt_gysID.Text == "")
                {
                    fun_load采购单列表();
                }
                else
                {
                    frm采购送检单界面_Load(null, null);
                }
                //fun_Newsongjiandan();
                //fun_load采购单列表();

                //}
                //else
                //{
                //    fun_searchData(drm["送检单号"].ToString());
                //}          
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_Newsongjiandan();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataView dv_check = new DataView(dt_songjianMx);
                dv_check.RowStateFilter = DataViewRowState.CurrentRows;
                if (dv_check.Count <= 0)
                    throw new Exception("没有送检明细不能进行保存操作！");
                if (dt_songjianMx.Rows == null || dt_songjianMx.Rows.Count <= 0)
                    throw new Exception("没有送检明细不能进行保存操作！");
                ////代办事项
                //gv_purchaselist.CloseEditor();
                //this.BindingContext[dt_sjPurchaseList].EndCurrentEdit();
                //送检单明细
                gv_songjianmx.CloseEditor();
                this.BindingContext[dt_songjianMx].EndCurrentEdit();
                fun_checkMainData();
                fun_checkTheDetail();

                fun_SaveData();
                fun_searchData(drm["送检单号"].ToString());
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //关闭界面
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        #endregion

        #region  生效操作

        private void fun_生效送检单()
        {
            try
            {
                DataTable dt_物料超出 = new DataTable();
                DataTable dt_检验记录 = new DataTable();
                string s = "select  * from 采购记录采购单检验主表 where 1=2";
                dt_检验记录 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DateTime t = CPublic.Var.getDatetime();
                //采购送检单主表的字段生效
                drm["生效"] = 1;
                drm["生效人员ID"] = CPublic.Var.LocalUserID;
                drm["生效人员"] = CPublic.Var.localUserName;
                drm["生效日期"] = t;
                //采购送检单明细表字段的生效
                foreach (DataRow r in dt_songjianMx.Rows)
                {
                    r["生效"] = 1;
                    r["生效人员ID"] = CPublic.Var.LocalUserID;
                    r["生效人员"] = CPublic.Var.localUserName;
                    r["生效日期"] = t;
                    string ss = t.Year.ToString().Substring(2, 2);
                    //采购单明细中的已送检数需要相加起来
                    DataRow[] dr = dt_sjPurchaseList.Select(string.Format("选择=True and 采购明细号='{0}'", r["采购单明细号"].ToString()));
                    if (dr.Length > 0)
                    {
                        dr[0]["已送检数"] = Convert.ToDecimal(dr[0]["已送检数"]) + Convert.ToDecimal(r["送检数量"]);
                        if (Convert.ToDecimal(dr[0]["已送检数"]) >= Convert.ToDecimal(dr[0]["采购数量"]))
                        { dr[0]["明细完成"] = 1; }
                    }
                    //2019-10-10 增加 开发采购 的 也自动生成检验单
                    if (r["存货分类编码"].ToString().Substring(0, 2) == "02" || r["存货分类编码"].ToString().Substring(0, 2) == "30" || r["采购单类型"].ToString() == "开发采购") //低值易耗品 不需要再检验
                    {
                        DataRow r_检验 = dt_检验记录.NewRow();
                        r_检验["GUID"] = Guid.NewGuid().ToString();
                        r_检验["检验记录单号"] = string.Format("IC{0}{1:00}{2:00}{3:0000}", ss, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("IC", t.Year, t.Month)); ;
                        r_检验["送检单号"] = r["送检单号"];
                        r_检验["检验日期"] = t;
                        r_检验["送检单明细号"] = r["送检单明细号"];
                        r_检验["采购单号"] = r["采购单号"];
                        r_检验["采购明细号"] = r["采购单明细号"];
                        //  r["操作员"] = strCurrUser;
                        r_检验["操作员"] = CPublic.Var.localUserName;
                        //增加的部分
                        r_检验["采购数量"] = r["采购数量"];
                        r_检验["税率"] = r["税率"];
                        r_检验["未税单价"] = r["未税单价"];
                        r_检验["单价"] = r["单价"];
                        r_检验["未税金额"] = r["未税金额"];
                        r_检验["金额"] = r["金额"];
                        r_检验["价格核实"] = r["价格核实"];
                        r_检验["是否急单"] = r["是否急单"];
                        r_检验["操作员ID"] = CPublic.Var.LocalUserID;
                        r_检验["产品名称"] = r["物料名称"];
                        r_检验["产品编号"] = r["物料编码"];
                        r_检验["规格型号"] = r["规格型号"];
                        r_检验["送检数量"] = r["送检数量"];
                        r_检验["已检数量"] = r["送检数量"];


                        r_检验["供应商编号"] = txt_gysID.Text;
                        r_检验["供应商名称"] = r["供应商"];

                        r_检验["检验员ID"] = "无需检验";
                        r_检验["检验员"] = "无需检验";

                        r_检验["送检人ID"] = CPublic.Var.LocalUserID;
                        r_检验["送检人"] = CPublic.Var.localUserName;

                        r_检验["检验结果"] = "免检";
                        dt_检验记录.Rows.Add(r_检验);
                        decimal dec = 0;
                        if (r["已检验数"].ToString() != "") dec = Convert.ToDecimal(r["已检验数"]);
                        r["已检验数"] = dec + Convert.ToDecimal(r["送检数量"]);
                        //if (Convert.ToDecimal(r["已检验数"]) == Convert.ToDecimal(dr[0]["采购数量"]))
                        //{
                        r["检验完成"] = 1;
                        //}
                        r["备注4"] = "免检";

                        drm["确认到货日期"] = t;
                    }
                }
                if (bl)
                {
                    dt_物料超出 = fun_供应商_物料超出(dt_songjianMx);
                }
                SqlDataAdapter da;
                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("sjdsx");
                SqlCommand cmd_zb = new SqlCommand("select * from 采购记录采购送检单主表 where 1<>1", conn, ts);
                SqlCommand cmd_mx = new SqlCommand("select * from 采购记录采购送检单明细表 where 1<>1", conn, ts);
                SqlCommand cmd_cgmx = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);

                try
                {   //送检单主表的生效
                    da = new SqlDataAdapter(cmd_zb);
                    new SqlCommandBuilder(da);
                    da.Update(dt_songjianZb);
                    //送检单明细表的生效
                    da = new SqlDataAdapter(cmd_mx);
                    new SqlCommandBuilder(da);
                    da.Update(dt_songjianMx);
                    //采购单明细表
                    da = new SqlDataAdapter(cmd_cgmx);
                    new SqlCommandBuilder(da);
                    da.Update(dt_sjPurchaseList);

                    if (dt_物料超出.Columns.Count > 0)
                    {
                        cmd_mx = new SqlCommand("select * from 采购记录供应商送检超出记录表 where 1<>1", conn, ts);
                        da = new SqlDataAdapter(cmd_mx);
                        new SqlCommandBuilder(da);
                        da.Update(dt_物料超出);
                    }
                    if (dt_检验记录.Rows.Count > 0)
                    {
                        cmd_mx = new SqlCommand(s, conn, ts);
                        da = new SqlDataAdapter(cmd_mx);
                        new SqlCommandBuilder(da);
                        da.Update(dt_检验记录);
                    }
                    ts.Commit();
                    bl = false;
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    bl = false;
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_生效送检单");
                throw ex;
            }
        }
        /// <summary>
        /// 这里还只是一条 如果改成多条一起送检 需要 修改
        /// </summary>
        /// <param name="dtx"></param>
        /// <returns></returns>
        private DataTable fun_供应商_物料超出(DataTable dtx)
        {
            DataTable dt = new DataTable();
            string sql = "select  *  from 采购记录供应商送检超出记录表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dt);
                DataRow dr = dt.NewRow();
                dr["采购单明细号"] = dtx.Rows[0]["采购单明细号"];
                dr["采购数量"] = dtx.Rows[0]["采购数量"];
                dr["送检总量"] = Convert.ToDecimal(dtx.Rows[0]["送检数量"]) + Convert.ToDecimal(dtx.Rows[0]["采购数量"]) - Convert.ToDecimal(dtx.Rows[0]["可送检数量"]);
                dr["供应商ID"] = dtx.Rows[0]["供应商ID"];
                dr["物料编码"] = dtx.Rows[0]["物料编码"];
                dr["日期"] = CPublic.Var.getDatetime();


                dt.Rows.Add(dr);

            }

            return dt;

        }
        private void fun_存默认检验员()
        {
            DataRow r = gv_purchaselist.GetDataRow(gv_purchaselist.FocusedRowHandle);
            string sql = string.Format("select * from 采购记录采购检验默认人员表 where 物料编码 = '{0}'", r["物料编码"]);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["检验员工号"].ToString() == searchLookUpEdit1.EditValue.ToString())
                {

                }
                else
                {
                    dt.Rows[0]["默认检验员"] = textBox2.Text;
                    dt.Rows[0]["检验员工号"] = searchLookUpEdit1.EditValue;

                    new SqlCommandBuilder(da);
                    da.Update(dt);
                }
            }
            else
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                dr["物料编码"] = r["物料编码"];
                dr["物料名称"] = r["物料名称"];
                dr["默认检验员"] = textBox2.Text;

                dr["检验员工号"] = searchLookUpEdit1.EditValue;
                new SqlCommandBuilder(da);
                da.Update(dt);
            }

        }
        //生效操作
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv_songjianmx.CloseEditor();
                this.BindingContext[dt_songjianMx].EndCurrentEdit();

                DataView dv_check = new DataView(dt_songjianMx);
                dv_check.RowStateFilter = DataViewRowState.CurrentRows;
                if (dv_check.Count <= 0)
                    throw new Exception("没有送检明细是不能进行生效操作的！");


                //  DataRow dr = gv_purchaselist.GetDataRow(gv_purchaselist.FocusedRowHandle);
                DataRow[] ds = dt_sjPurchaseList.Select("选择 = 1");
                DataRow dr = ds[0];
                if (dr["采购单类型"].ToString() == "委外采购") //若为委外 需对应发料单完成才可送检 18-7-7 送检总数不可超过 采购数量
                {
                    //改为取发料最小情况  19-6-20
                    string s = string.Format("select a.* from 其他出入库申请子表 a left join 其他出入库申请主表 b on a.出入库申请单号 = b.出入库申请单号   where a.备注='{0}'  and b.作废 = 0", dr["采购明细号"]);
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    DataRow[] r = t.Select(string.Format("已完成数量=min(已完成数量)"));
                    if (r.Length > 0)
                    {
                        decimal dec = Convert.ToDecimal(r[0]["已完成数量"]) / (Convert.ToDecimal(r[0]["数量"]) / Convert.ToDecimal(dr["采购数量"]));
                        if (Convert.ToDecimal(dt_songjianMx.Rows[0]["送检数量"]) > dec) throw new Exception(string.Format("已发料况只允许到货数量为{0}", dec));
                    }
                    else
                    {
                        throw new Exception("该条委外采购记录没有相应的委外发料申请单,请确认");
                    }
                    //if (t.Rows.Count > 0) throw new Exception("原料尚未发出,不可送检");

                    if (Convert.ToDecimal(dt_songjianMx.Rows[0]["送检数量"]) > Convert.ToDecimal(dt_songjianMx.Rows[0]["可送检数量"]))
                        throw new Exception("委外采购送检数量不可超过采购量");
                }
                if (dt_songjianMx.Rows == null || dt_songjianMx.Rows.Count <= 0)
                    throw new Exception("没有送检明细是不能进行生效操作的！");

                //生效的时候也要做保存的操作的检查，因为生效包括了保存
                if (MessageBox.Show(string.Format("确认生成送检单？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_checkMainData(); //主表的检查

                    fun_checkTheDetail(); //明细表的检查
                    fun_生效送检单(); //生效送检单
                    if (textBox2.Text != "")
                    {
                        fun_存默认检验员();
                    }


                    MessageBox.Show("生效成功！");
                    //生效成功后调用新增
                    fun_Newsongjiandan();
                    txt_songjiandan.Text = "";


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        //代办事项的选择列的实时响应
        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            gv_purchaselist.CloseEditor();
            this.BindingContext[dt_sjPurchaseList].EndCurrentEdit();
            try
            {
                //if (e.Column.ColumnName == "选择")  
                {
                    int count = 0; int count_行数 = dt_sjPurchaseList.Rows.Count;
                    DataRow dr = gv_purchaselist.GetDataRow(gv_purchaselist.FocusedRowHandle);
                    string sql_检验员 = string.Format("select *  from 采购记录采购检验默认人员表 where 物料编码='{0}' ", dr["物料编码"]);
                    DataTable dt = new DataTable();
                    dt = CZMaster.MasterSQL.Get_DataTable(sql_检验员, strcon);
                    if (dt.Rows.Count > 0 && dt.Rows[0]["检验员工号"].ToString().Trim() != "")
                    {
                        searchLookUpEdit1.EditValue = dt.Rows[0]["检验员工号"];
                    }
                    // 12/14


                    foreach (DataRow r in dt_sjPurchaseList.Rows)
                    {
                        if (r["选择"].Equals(true))
                        {
                            if (dt_songjianMx.Rows.Count > 0)
                            {
                                DataRow[] dr1 = dt_songjianMx.Select(string.Format("采购单明细号='{0}'", r["采购明细号"].ToString()));
                                if (dr1.Length > 0)
                                {
                                    continue;
                                }
                            }
                            DataRow r2 = dt_songjianMx.NewRow();
                            r2["采购单号"] = r["采购单号"];
                            r2["采购单明细号"] = r["采购明细号"];
                            r2["物料编码"] = r["物料编码"];
                            r2["存货分类编码"] = r["存货分类编码"];
                            r2["采购单类型"] = r["采购单类型"];
                            r2["送检单类型"] = "到货";
                            r2["物料名称"] = r["物料名称"];
                            r2["规格型号"] = r["规格型号"];
                            r2["供应商ID"] = r["供应商ID"];
                            r2["供应商"] = r["供应商"];
                            r2["供应商负责人"] = r["供应商负责人"];
                            r2["供应商电话"] = r["供应商电话"];
                            r2["物料编码"] = r["物料编码"];
                            r2["物料名称"] = r["物料名称"];
                            r2["图纸编号"] = r["图纸编号"];
                            r2["图纸版本"] = r["图纸版本"];
                            r2["规格型号"] = r["规格型号"];
                            r2["采购数量"] = r["采购数量"];
                            r2["税率"] = r["税率"];
                            r2["未税单价"] = r["未税单价"];
                            r2["单价"] = r["单价"];
                            r2["未税金额"] = r["未税金额"];
                            r2["金额"] = r["金额"];
                            r2["送检数量"] = r2["可送检数量"] = Convert.ToDecimal(r["采购数量"]) - Convert.ToDecimal(r["已送检数"]);
                            dt_songjianMx.Rows.Add(r2);
                            txt_gysID.EditValue = r["供应商ID"].ToString();

                            //if (Convert.ToInt32(r["税率"]) == 17)
                            //{
                            //    MessageBox.Show("选中采购明细税率为17,请确认是否要更改,如要更改在下面列表中更改开票税率,含税单价会自动计算。");
                            //}

                        }
                        else
                        {
                            if (dt_songjianMx.Rows.Count > 0)
                            {
                                DataRow[] dr1 = dt_songjianMx.Select(string.Format("采购单明细号='{0}'", r["采购明细号"].ToString()));
                                foreach (DataRow t in dr1)
                                {
                                    t.Delete();
                                }
                            }
                            count++;
                        }
                    }
                    if (count == count_行数)
                    {
                        txt_gysID.EditValue = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //代办事项的选择列的互斥
        private void repositoryItemCheckEdit1_EditValueChanged(object sender, EventArgs e)
        {
            foreach (DataRow r in dt_sjPurchaseList.Rows)
            {
                if (r["选择"].Equals(true))
                {
                    r["选择"] = false;
                }
            }
        }

        #region 右键
        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                DataRow dr = gv_purchaselist.GetDataRow(gv_purchaselist.FocusedRowHandle);
                if (dr["采购单类型"].ToString() == "委外采购" && CPublic.Var.LocalUserTeam != "管理员权限")
                {
                    throw new Exception("委外采购单暂不允许使用此功能");
                }
                if (MessageBox.Show(string.Format("是否要关闭采购明细{0}", dr["采购明细号"]), "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (Convert.ToDecimal(dr["未送检数"]) == Convert.ToDecimal(dr["采购数量"]))
                    {
                        string sql = string.Format("select * from 采购记录采购单明细表 where 采购明细号 = '{0}'", dr["采购明细号"]);
                        SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dt.Rows[0]["作废"] = 1;
                        dt.Rows[0]["作废人员"] = CPublic.Var.localUserName;
                        dt.Rows[0]["作废人员ID"] = "右击关闭";
                        dt.Rows[0]["作废日期"] = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");                     //在途量也要去掉
                        //在途量也要去掉
                        new SqlCommandBuilder(da);
                        da.Update(dt);
                        StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);

                        fun_load采购单列表();
                    }
                    else
                    {
                        string sql = string.Format("select * from 采购记录采购单明细表 where 采购明细号 = '{0}'", dr["采购明细号"]);
                        SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dt.Rows[0]["明细完成"] = 1;
                        dt.Rows[0]["明细完成日期"] = CPublic.Var.getDatetime();
                        dt.Rows[0]["作废人员"] = CPublic.Var.localUserName;
                        dt.Rows[0]["作废人员ID"] = "右击明细完成";
                        //在途量也要去掉
                        new SqlCommandBuilder(da);
                        da.Update(dt);
                        StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);

                        fun_load采购单列表();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_purchaselist_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            //判断右键菜单是否可用
            DataRow dr = gv_purchaselist.GetDataRow(gv_purchaselist.FocusedRowHandle);
            if (dr == null) return;
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc_purchaselist, new Point(e.X, e.Y));
            }
        }
        #endregion
        //修改采购单
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow r = gv_purchaselist.GetDataRow(gv_purchaselist.FocusedRowHandle);


            frm采购单明细视图 fm = new frm采购单明细视图(r["采购单号"].ToString());
            CPublic.UIcontrol.AddNewPage(fm, "采购明细视图");

        }

        private void gv_purchaselist_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_purchaselist.GetFocusedRowCellValue(gv_purchaselist.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gv_songjianmx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_songjianmx.GetFocusedRowCellValue(gv_songjianmx.FocusedColumn));
                e.Handled = true;
            }
        }

        private void txt_gysID_EditValueChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (txt_gysID.EditValue == null)
                    txt_gysID.EditValue = "";
                DataRow[] dr = dt_gys.Select(string.Format("供应商ID='{0}'", txt_gysID.EditValue.ToString()));
                if (dr.Length > 0)
                {
                    txt_gysName.Text = dr[0]["供应商名称"].ToString();
                    txt_gysfzr.Text = dr[0]["供应商负责人"].ToString();
                    txt_gysdianhua.Text = dr[0]["供应商电话"].ToString();
                }
                else
                {
                    txt_gysName.Text = "";
                    txt_gysfzr.Text = "";
                    txt_gysdianhua.Text = "";
                }

                //明细中，如果与当前的供应商不一致。就删除
                if (dt_songjianMx != null)
                {
                    DataRow[] dr1 = dt_songjianMx.Select(string.Format("供应商ID='{0}'", txt_gysID.EditValue.ToString()));
                    if (dr1.Length <= 0)
                    {   //针对新增的删除
                        foreach (DataRow t in dt_songjianMx.Rows)
                        {
                            if (t.RowState == DataRowState.Added)
                            {
                                t.AcceptChanges();
                                t.Delete();
                            }
                        }
                        dt_songjianMx.AcceptChanges();
                        //针对非新增的删除
                        foreach (DataRow t in dt_songjianMx.Rows)
                        {
                            t.Delete();
                        }
                    }
                }
                fun_load采购单列表();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txt_songjianrenID_EditValueChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (txt_songjianrenID.EditValue == null)
                    txt_songjianrenID.EditValue = "";
                DataRow[] dr = dt_people.Select(string.Format("员工号='{0}'", txt_songjianrenID.EditValue.ToString()));
                if (dr.Length > 0)
                {
                    txt_songjianrenName.Text = dr[0]["姓名"].ToString();
                }
                else
                {
                    txt_songjianrenName.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gv_purchaselist_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem3_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                saveFileDialog.FileName = "采购明细送检待办事项";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(DevExpress.XtraPrinting.TextExportMode.Text);
                    //saveFileDialog.FileName = ((DateTime)bar_前.EditValue).ToString() + ((DateTime)bar_后.EditValue).AddDays(1).AddSeconds(-1).ToString();
                    gc_purchaselist.ExportToXlsx(saveFileDialog.FileName, options);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_songjianmx_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void 完成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv_purchaselist.GetDataRow(gv_purchaselist.FocusedRowHandle);
                if (dr["采购单类型"].ToString() == "委外采购" && CPublic.Var.LocalUserTeam != "管理员权限")
                {
                    throw new Exception("委外采购单暂不允许使用此功能");
                }


                if (Convert.ToDecimal(dr["未送检数"]) == Convert.ToDecimal(dr["采购数量"]))
                {
                    throw new Exception("尚未送检,可选择关闭该明细");
                }
                if (MessageBox.Show(string.Format("是否不送检采购单:{0} 剩余数量的物料", dr["采购明细号"]), "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string sql = string.Format("select * from 采购记录采购单明细表 where 采购明细号 = '{0}'", dr["采购明细号"]);
                    SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dt.Rows[0]["明细完成"] = 1;
                    dt.Rows[0]["明细完成日期"] = CPublic.Var.getDatetime();
                    dt.Rows[0]["作废人员ID"] = "右击明细完成";
                    //在途量也要去掉
                    new SqlCommandBuilder(da);
                    da.Update(dt);
                    StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);

                    fun_load采购单列表();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_purchaselist_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {


                if (gv_purchaselist.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                string s = gv_purchaselist.GetRowCellValue(e.RowHandle, "到货日期").ToString();
                if (s != null && s != "")
                {
                    DateTime t = Convert.ToDateTime(gv_purchaselist.GetRowCellValue(e.RowHandle, "到货日期"));
                    if (DateTime.Compare(t, time) == 0)
                    {
                        e.Appearance.BackColor = Color.Yellow;

                    }
                    else if (DateTime.Compare(t, time) < 0)
                    {
                        e.Appearance.BackColor = Color.Pink;

                    }
                }
                if (e.Column.Caption == "季度用量")
                {
                    DataRow dr = gv_purchaselist.GetDataRow(e.RowHandle);
                    try
                    {
                        if ((Convert.ToDecimal(e.CellValue) * (decimal)0.3) > Convert.ToDecimal(dr["库存总数"]))
                        {
                            e.Appearance.BackColor = Color.BurlyWood;

                        }
                    }
                    catch
                    {
                    }

                }
                if (e.Column.Caption == "库存总数")
                {
                    DataRow dr = gv_purchaselist.GetDataRow(e.RowHandle);
                    try
                    {
                        if (Convert.ToDecimal(dr["参考量"]) > Convert.ToDecimal(dr["库存总数"]))
                        {
                            e.Appearance.BackColor = Color.Red;

                        }
                    }
                    catch
                    {
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void searchLookUpEdit1_EditValueChanged_1(object sender, EventArgs e)
        {
            DataRow[] dr = dt_检验员.Select(string.Format("员工号='{0}'", searchLookUpEdit1.EditValue));
            if (dr.Length > 0)
            {
                textBox2.Text = dr[0]["姓名"].ToString();
            }
        }



        private void gv_songjianmx_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "开票税率")
                {
                    DataRow r = gv_songjianmx.GetDataRow(e.RowHandle);
                    if (e.Value.ToString() != "17" && e.Value.ToString() != "16")
                    {

                        gv_songjianmx.SetFocusedRowCellValue("开票税率", 17);
                        throw new Exception("输入税率不正确");
                    }
                    else
                    {

                        decimal dec = Convert.ToDecimal(r["未税单价"]) * (decimal)(1 + Convert.ToDecimal(e.Value) / 100);
                        r["单价"] = dec;
                        r["税率"] = Convert.ToDecimal(e.Value);
                        //原先送检单明细上金额 未税金额 都是 采购单上的金额 ，不是根据 送检数量重新计算的。先 更改 18-4-20
                        //检验单上单价金额 税率 都是延续的送检单明细上信息
                        r["金额"] = dec * Convert.ToDecimal(r["送检数量"]);
                        r["未税金额"] = Convert.ToDecimal(r["未税单价"]) * Convert.ToDecimal(r["送检数量"]);

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
