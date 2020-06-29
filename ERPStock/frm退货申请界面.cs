using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ERPStock
{
    public partial class frm退货申请界面 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dtP;
        DataRow drM = null;
        DataTable dt_物料;
        DataTable dt_客户;
        DataTable dt_出库单;
        bool bl = false;
        //  DataTable dt_出库单;
        string Crm_R_No = "";

        /// <summary>
        /// i_状态=2 为浏览模式
        /// </summary>
        int i_状态 = 0;
        #endregion

        public frm退货申请界面()
        {
            InitializeComponent();
        }
        public frm退货申请界面(DataRow r)
        {
            drM = r;
            InitializeComponent();
            i_状态 = 0;
        }
        public frm退货申请界面(DataRow r, int x)
        {
            drM = r;
            i_状态 = x;
            InitializeComponent();
        }
        private void frm退货申请界面_Load(object sender, EventArgs e)
        {
            try
            {
                time_申请日期.EditValue = CPublic.Var.getDatetime();
                fun_客户();
                fun_物料下拉框();
                fun_载入主表明细();
                op_zt(i_状态);
                gc.DataSource = dtP;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 方法
        private void fun_载入主表明细()
        {
            if (drM == null)
            {
                string sql = "select * from 退货申请主表 where 1<>1";
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
                sql = @"select 退货申请子表.*,a.物料名称,a.规格型号,0 已开票数量,0 累计退货数量,0 未开票数量 from 退货申请子表 
                left join 基础数据物料信息表 a on 退货申请子表.物料编码 = a.物料编码  where 1<>1";
                dtP = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtP);
            }
            else
            {
                string sql = string.Format("select * from 退货申请主表 where 退货申请单号 = '{0}'", drM["退货申请单号"].ToString());
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.Rows[0];
                dataBindHelper1.DataFormDR(drM);
                string sql2 = string.Format(@"select 退货申请子表.*,a.物料名称,a.规格型号,累计退货数量,已开票数量,未开票数量 from 退货申请子表
                left join 基础数据物料信息表 a on 退货申请子表.物料编码 = a.物料编码
                left join 销售记录成品出库单明细表 ckmx  on ckmx.成品出库单明细号=退货申请子表.出库明细号
                where 退货申请单号 = '{0}'", drM["退货申请单号"].ToString());
                dtP = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
                da2.Fill(dtP);
            }
            dtP.Columns.Add("可退货数", typeof(decimal));
            if (drM != null)
            {
                foreach (DataRow dr in dtP.Rows)
                {
                    DataRow[] xr = dt_出库单.Select(string.Format("成品出库单明细号='{0}'", dr["出库明细号"].ToString()));
                    dr["可退货数"] = Convert.ToDecimal(xr[0]["可退货数"]) + Convert.ToDecimal(dr["数量"]);
                }
            }
            // dtP.ColumnChanged += dtP_ColumnChanged;
        }
        /// <summary>
        /// 20-6-3 原本参数为bool 类型区分 生效或保存 现 用提交审核 替代生效 
        /// </summary>
        /// <param name="bl"></param>
        private void fun_保存主表明细(Boolean bl)
        {
            DateTime t = CPublic.Var.getDatetime();
            string str_id = CPublic.Var.LocalUserID;
            string str_name = CPublic.Var.localUserName;


            if (drM["GUID"].ToString() == "")
            {
                drM["GUID"] = System.Guid.NewGuid();
                txt_退货申请单号.Text = string.Format("THSQ{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
                    t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("THSQ", t.Year, t.Month).ToString("0000"));
                drM["退货申请单号"] = txt_退货申请单号.Text;
            }
            drM["操作人员编号"] = str_id;
            drM["操作人员"] = str_name;
            drM["部门编号"] = CPublic.Var.localUser部门编号;
            drM["部门名称"] = CPublic.Var.localUser部门名称;

            // if (checkBox1.Checked) drM["是否再发货"] = true;

            if (bl)
            {
                //drM["生效"] = true;
                //drM["生效人员编号"] = str_id;
                //drM["生效日期"] = t;
                drM["提交审核"] = true;

            }
            drM["完成"] = false;


            dataBindHelper1.DataToDR(drM);
            int i = 1;
            DataRow[] rrr = dtP.Select("POS=max(POS)");

            if (rrr.Length > 0 && rrr[0]["POS"].ToString() != "")
            {
                i = Convert.ToInt32(rrr[0]["POS"]) + 1;
            }
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                if (decimal.Parse(r["数量"].ToString()) < 0)
                {
                    throw new Exception("误输入负数");
                }
                if (r["GUID"].ToString() == "")
                {
                    r["GUID"] = System.Guid.NewGuid();
                    r["退货申请单号"] = drM["退货申请单号"];
                    r["退货申请明细号"] = drM["退货申请单号"].ToString() + "-" + i.ToString("00");
                    r["POS"] = i++;
                }
                //if (bl)
                //{
                //    r["生效"] = true;
                //    r["生效人员编号"] = str_id;
                //    r["生效日期"] = t;
                //}
                r["完成"] = false;
            }
            ////劳务 19-11-21
            //DataTable t_SaleCk = fun_lw(dtP);
            //DataRow[] rr = dtP.Select("完成=0");
            //if (rr.Length == 0)
            //{
            //    drM["完成"] = 1;
            //    drM["完成日期"] = t;
            //    drM["备注1"] = "劳务系统自动完成";
            //}

            DataTable t_spl = ERPorg.Corg.fun_PA("生效", "销售退货", txt_退货申请单号.Text, textBox1.Text);
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {

                string sql = "select * from 退货申请主表 where 1<>1";
                SqlCommand cmd = new SqlCommand(sql, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dtM);


                sql = "select * from 退货申请子表 where 1<>1";
                cmd = new SqlCommand(sql, conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dtP);

                sql = "select * from 单据审核申请表 where 1<>1";
                cmd = new SqlCommand(sql, conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(t_spl);

                //if (t_SaleCk != null && t_SaleCk.Columns.Count > 0)
                //{
                //    sql = "select * from 销售记录成品出库单明细表 where 1<>1";
                //    cmd = new SqlCommand(sql, conn, ts);
                //    da = new SqlDataAdapter(cmd);
                //    new SqlCommandBuilder(da);
                //    da.Update(t_SaleCk);
                //}

                ts.Commit();

            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }
        private DataTable fun_lw(DataTable dt)
        {
            DataTable t_ck = new DataTable();
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow r in dt.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                if (r["物料名称"].ToString().Contains("劳务"))
                {
                    r["完成"] = 1;
                    r["完成日期"] = t;
                    string sql = string.Format("select  * from 销售记录成品出库单明细表 where 成品出库单明细号='{0}'", r["出库明细号"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {
                        da.Fill(t_ck);//这里需要在退货申请的时候做限制 不可以有重复的物料

                        DataRow[] rrx = t_ck.Select(string.Format("成品出库单明细号='{0}'", r["出库明细号"].ToString()));
                        rrx[0]["累计退货数量"] = Convert.ToDecimal(rrx[0]["累计退货数量"]) + Convert.ToDecimal(r["数量"]);
                    }
                    string s_出库单号 = r["出库明细号"].ToString().Split('-')[0];
                    int p = Convert.ToInt32(r["出库明细号"].ToString().Split('-')[1]);
                    DataRow[] tr = t_ck.Select(string.Format("成品出库单明细号='{0}'  and 退货标识<>'是'", s_出库单号 + "-" + p.ToString("00")));
                    //如果退货数量+累计退货数量>出库数量 -已开票数量
                    //那 退货数量+累计退货数量 -（出库数量 -已开票数量） 部分 需要生成负的 出库记录
                    if (Convert.ToDecimal(tr[0]["累计退货数量"]) > Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]))
                    {
                        //成品出库明细
                        DataRow rr = t_ck.NewRow();
                        rr["GUID"] = System.Guid.NewGuid();
                        rr["成品出库单号"] = s_出库单号;
                        int pos = 0;

                        DataRow[] rg = t_ck.Select(string.Format("成品出库单号='{0}'  and 退货标识<>'是'", s_出库单号), "POS desc");
                        pos = Convert.ToInt32(rg[0]["POS"]);

                        //if (tr.Length > 0)
                        //    rr["POS"] = Convert.ToInt32(tr[0]["POS"]) + 1;
                        //else
                        //{
                        string s = string.Format("select  max(pos)POS from 销售记录成品出库单明细表 where 成品出库单号='{0}'", s_出库单号);
                        DataTable tt = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                        pos = Convert.ToInt32(tt.Rows[0]["POS"]) > pos ? Convert.ToInt32(tt.Rows[0]["POS"]) + 1 : pos + 1;
                        rr["POS"] = pos;
                        //}
                        rr["成品出库单明细号"] = s_出库单号 + "-" + rr["POS"].ToString();
                        rr["备注1"] = "退货";
                        rr["退货标识"] = "是";
                        try
                        {
                            rr["销售订单号"] = tr[0]["销售订单号"];
                            rr["销售订单明细号"] = tr[0]["销售订单明细号"];
                            rr["出库通知单号"] = tr[0]["出库通知单号"];
                            rr["出库通知单明细号"] = tr[0]["出库通知单明细号"];
                        }
                        catch
                        { }
                        rr["物料编码"] = r["物料编码"];
                        rr["物料名称"] = r["物料名称"];
                        //那 退货数量+累计退货数量 -（出库数量 -已开票数量） 部分 需要生成负的 出库记录
                        rr["出库数量"] = -Convert.ToDecimal(tr[0]["累计退货数量"]) + Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]);
                        rr["已出库数量"] = -Convert.ToDecimal(tr[0]["累计退货数量"]) + Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]);
                        rr["未开票数量"] = -Convert.ToDecimal(tr[0]["累计退货数量"]) + Convert.ToDecimal(tr[0]["出库数量"]) - Convert.ToDecimal(tr[0]["已开票数量"]);
                        DataTable dt_1 = new DataTable();
                        string sql_1 = string.Format("select  * from 基础数据物料信息表 where 物料编码='{0}'", r["物料编码"]);
                        dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                        rr["计量单位"] = dt_1.Rows[0]["计量单位"];
                        rr["规格型号"] = dt_1.Rows[0]["规格型号"];
                        rr["客户"] = tr[0]["客户"];
                        rr["客户编号"] = tr[0]["客户编号"];
                        rr["生效"] = true;
                        rr["生效日期"] = t;
                        rr["仓库号"] = tr[0]["仓库号"];
                        rr["仓库名称"] = tr[0]["仓库名称"];
                        t_ck.Rows.Add(rr);
                    }
                }
            }
            return t_ck;
        }

        /// <summary>
        /// 弃用 19-4-27
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (e.Column.Caption == "物料编码")
                {
                    DataRow[] ds = dt_物料.Select(string.Format("物料编码 = '{0}'", e.Row["物料编码"]));
                    //e.Row["原ERP物料编号"] = ds[0]["原ERP物料编号"];
                    e.Row["物料名称"] = ds[0]["物料名称"];
                    // e.Row["n原ERP规格型号"] = ds[0]["n原ERP规格型号"];
                    e.Row["规格型号"] = ds[0]["规格型号"];
                    //    e.Row["图纸编号"] = ds[0]["图纸编号"];

                    if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "") return;
                    string sql = string.Format(@"select  zb.销售订单号,成品出库单明细号,y.在退数,ckmx.销售订单明细号,出库通知单明细号,目标客户,base.物料编码,base.物料名称,base.规格型号,出库数量,已开票数量,未开票数量,ckmx.计量单位,  
                                              累计退货数量,mxb.税率,税后单价,税前单价 
                                              from 销售记录成品出库单明细表 ckmx 
                                            left join 基础数据物料信息表 base  on base.物料编码=ckmx.物料编码
                                            left join  ( select 出库明细号,SUM(数量-已入库数量) as   在退数  from 退货申请子表 group by 出库明细号  )   y   on ckmx.成品出库单明细号= y.出库明细号
                                            left join 销售记录销售订单主表 zb  on zb.销售订单号=ckmx.销售订单号
                                            left join 销售记录销售订单明细表 mxb on mxb.销售订单明细号= ckmx.销售订单明细号
                       where 客户编号 = '{0}'and 物料编码='{1}'", searchLookUpEdit1.EditValue.ToString(), e.Row["物料编码"]);
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    dt_出库单 = new DataTable();
                    da.Fill(dt_出库单);
                    cb_关联销售单.Properties.DataSource = dt_出库单;
                    cb_关联销售单.Properties.DisplayMember = "成品出库单明细号";
                    cb_关联销售单.Properties.ValueMember = "成品出库单明细号";
                    searchLookUpEdit2View.PopulateColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //设置状态
        private void op_zt(int status)
        {
            if (status == 2) //浏览模式  不可操作  
            {
                barLargeButtonItem1.Enabled = false;
                barLargeButtonItem4.Enabled = false;
                barLargeButtonItem5.Enabled = false;
                barLargeButtonItem6.Enabled = false;
                button6.Enabled = false;
                panel3.Visible = false;
                panel1.Enabled = false;
                gv.OptionsBehavior.Editable = false;
            }
            else if (status == 1) //已提交审核
            {
                barLargeButtonItem4.Enabled = false;
                barLargeButtonItem5.Enabled = true;
                barLargeButtonItem6.Enabled = false;
                button6.Enabled = false;
                panel3.Enabled = true;

            }
            else if (status == 0) //未提交状态 
            {
                barLargeButtonItem5.Enabled = false;
                barLargeButtonItem4.Enabled = true;
                button6.Enabled = true;
                panel3.Enabled = true;

            }
        }
        private void fun_物料下拉框()
        {
            //string sql = @"select base.物料编码,base.物料名称,base.规格型号,a.仓库号,a.仓库名称,
            //base.图纸编号,isnull(a.库存总数,0)库存总数 from 基础数据物料信息表 base
            //left join 仓库物料数量表 a  on base.物料编码 = a.物料编码";
            //dt_物料 = new DataTable();
            //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //da.Fill(dt_物料);
            //repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            //repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            //repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

            string sql = @"SELECT  SRCode,b.DistributorName,memo,CRMCode FROM `sellreturn_main` a
                left join distributors b on b.DistributorCode = a.DistributorCode  where bl = 0 "; //这里需要增加 字段 ERP已获取的字段
            string strcon_aliyun = string.Format("server={0};User Id={1};password={2};Database={3};CharSet=utf8", CPublic.Var.li_CFG["aliyun_server"].ToString(),
                      CPublic.Var.li_CFG["aliyun_UID"].ToString(), CPublic.Var.li_CFG["aliyun_PWD"].ToString(), CPublic.Var.li_CFG["aliyun_database"].ToString());
            DataTable dt_crm = new DataTable();
            MySqlDataAdapter aa = new MySqlDataAdapter(sql, strcon_aliyun);
            aa.Fill(dt_crm);
            searchLookUpEdit2.Properties.DataSource = dt_crm;
            searchLookUpEdit2.Properties.DisplayMember = "SRCode";
            searchLookUpEdit2.Properties.ValueMember = "SRCode";



        }

        private void fun_客户()
        {
            string sql = string.Format(@"select 客户编号,客户名称 from 客户基础信息表");
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            dt_客户 = new DataTable();
            da.Fill(dt_客户);
            searchLookUpEdit1.Properties.DataSource = dt_客户;
            searchLookUpEdit1.Properties.DisplayMember = "客户编号";
            searchLookUpEdit1.Properties.ValueMember = "客户编号";
        }
        #endregion

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //新增
            try
            {
                time_申请日期.EditValue = CPublic.Var.getDatetime();
                drM = null;
                txt_退货申请单号.Text = ""; ;
                txt_备注.Text = "";
                searchLookUpEdit1.EditValue = "";
                //textBox1.Text = "";
                //cb_关联出库单.EditValue = "";
                cb_关联销售单.EditValue = "";
                fun_物料下拉框();
                fun_载入主表明细();
                gc.DataSource = dtP;
                barLargeButtonItem2.Enabled = true;
                Crm_R_No = "";
                i_状态 = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ztbg()
        {

        }
        private void check()
        {
            foreach (DataRow dr in dtP.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dr["物料编码"].ToString() == "")
                {
                    throw new Exception("有物料编码为空,请检查");
                }
                if (dr["数量"].ToString() == "")
                {
                    throw new Exception("有数量未填,请检查");
                }
                if (decimal.Parse(dr["已开票数量"].ToString()) > 0)
                {
                    bl = true;
                }
                if (decimal.Parse(dr["数量"].ToString()) <= 0)
                {
                    throw new Exception("数量不可小于等于0,请检查");
                }

            }

            string sql = string.Format("select * from 退货申请主表 where 退货申请单号='{0}'", drM["退货申请单号"]);
            DataRow r = CZMaster.MasterSQL.Get_DataRow(sql, strconn);

            if (r != null && Convert.ToBoolean(r["完成"]))
            {
                throw new Exception("单据状态已修改,仓库已入库");

            }


        }
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //生效
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                check();
                if (bl)
                {
                    if (MessageBox.Show("明细中有明细有已开票数量,是否确认", "提醒", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        bl = false;
                    }
                }
                if (!bl)
                {

                    //if (comboBox1.Text != "")
                    //{
                    //    str_thlx = comboBox1.Text;
                    //}
                    //if (MessageBox.Show(string.Format("请确认退货类型是否为 {0} ?", str_thlx), "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    //{

                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    fun_保存主表明细(true);
                    //19-10-29  需要回馈给CRM销售订单号
                    if (Crm_R_No.Trim() != "")
                    {
                        string strcon_aliyun = string.Format("server={0};User Id={1};password={2};Database={3};CharSet=utf8", CPublic.Var.li_CFG["aliyun_server"].ToString(),
                        CPublic.Var.li_CFG["aliyun_UID"].ToString(), CPublic.Var.li_CFG["aliyun_PWD"].ToString(), CPublic.Var.li_CFG["aliyun_database"].ToString());
                        string s = string.Format(" select * from  sellreturn_main  where bl=0 and SRCode='{0}'", Crm_R_No);
                        using (MySqlDataAdapter da = new MySqlDataAdapter(s, strcon_aliyun))
                        {
                            DataTable dt_somain = new DataTable();
                            da.Fill(dt_somain);

                            dt_somain.Rows[0]["bl"] = true;
                            new MySqlCommandBuilder(da);
                            da.Update(dt_somain);
                        }
                    }
                    MessageBox.Show("生效成功");


                    refsh(txt_退货申请单号.Text);

                    barLargeButtonItem2.Enabled = false;
                    //}
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void refsh(string s)
        {
            string sql = string.Format("select * from 退货申请主表 where 退货申请单号 = '{0}'", s);
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            drM = dtM.Rows[0];
            dataBindHelper1.DataFormDR(drM);

            string sql2 = string.Format(@"select 退货申请子表.*,a.物料名称,a.规格型号,累计退货数量,已开票数量,未开票数量 from 退货申请子表
                left join 基础数据物料信息表 a on 退货申请子表.物料编码 = a.物料编码
                left join 销售记录成品出库单明细表 ckmx  on ckmx.成品出库单明细号 = 退货申请子表.出库明细号
                where 退货申请单号 = '{0}'", s);
            dtP = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            da2.Fill(dtP);
            gc.DataSource = dtP;
        }
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            CPublic.UIcontrol.ClosePage();
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue.ToString() == "")
                {
                    textBox1.Text = "";
                    return;
                }

                //根据客户删选销售单
                textBox1.Text = dt_客户.Select(string.Format("客户编号='{0}'", searchLookUpEdit1.EditValue.ToString()))[0]["客户名称"].ToString();

                //if (gv.RowCount == 0) return;
                string sql = string.Format(@"select zb.销售订单号,成品出库单明细号,y.在退数,ckmx.销售订单明细号,出库通知单明细号,目标客户,base.物料编码,base.物料名称,base.规格型号,出库数量,已开票数量,未开票数量,ckmx.计量单位,  
                                             累计退货数量,mxb.税率,税后单价,税前单价,出库数量-累计退货数量-isnull(在退数,0) as 可退货数  from 销售记录成品出库单明细表 ckmx
                                             left join 基础数据物料信息表 base  on base.物料编码=ckmx.物料编码
                                             left join  (select 出库明细号,SUM(数量-已入库数量) as   在退数  from 退货申请子表 a
                                                          left join 退货申请主表 b on a.退货申请单号=b.退货申请单号  where b.审核=1 and  a.作废=0 and b.作废=0  group by 出库明细号   )   y   on ckmx.成品出库单明细号= y.出库明细号
                                             left join 销售记录销售订单明细表 mxb on mxb.销售订单明细号= ckmx.销售订单明细号
                                             left join 销售记录销售订单主表 zb  on zb.销售订单号=mxb.销售订单号
                                             where zb.客户编号 = '{0}' and  出库数量>累计退货数量 order by 未开票数量 ", searchLookUpEdit1.EditValue.ToString());
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dt_出库单 = new DataTable();
                da.Fill(dt_出库单);
                cb_关联销售单.Properties.DataSource = dt_出库单;
                cb_关联销售单.Properties.DisplayMember = "成品出库单明细号";
                cb_关联销售单.Properties.ValueMember = "成品出库单明细号";
                //searchLookUpEdit2View.PopulateColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cb_关联销售单_EditValueChanged(object sender, EventArgs e)
        {
            if (cb_关联销售单.EditValue != null && cb_关联销售单.EditValue.ToString() != "")
            {

                DataRow[] dr = dt_出库单.Select(string.Format("成品出库单明细号='{0}'", cb_关联销售单.EditValue));
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    searchLookUpEdit1.EditValue = dr[0]["客户ID"].ToString();
                }
                DataRow r = dtP.NewRow();
                r["物料编码"] = dr[0]["物料编码"];

                r["出库明细号"] = dr[0]["成品出库单明细号"];
                r["销售明细号"] = dr[0]["销售订单明细号"];
                r["通知单明细号"] = dr[0]["出库通知单明细号"];
                r["物料名称"] = dr[0]["物料名称"];
                r["规格型号"] = dr[0]["规格型号"];
                r["税率"] = dr[0]["税率"];
                r["销售明细"] = cb_关联销售单.EditValue;
                r["税后单价"] = dr[0]["税后单价"];
                r["税前单价"] = dr[0]["税前单价"];
                r["已开票数量"] = dr[0]["已开票数量"];

                r["未开票数量"] = dr[0]["未开票数量"];


                r["累计退货数量"] = dr[0]["累计退货数量"];


                // r["税前单价"] = dr[0]["税前单价"];

                r["可退货数"] = dr[0]["可退货数"];
                dtP.Rows.Add(r);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataRow dr = dtP.NewRow();
            dtP.Rows.Add(dr);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            dr.Delete();
        }

        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {

                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

                decimal taxRate = Convert.ToDecimal(dr["税率"]) / 100 + 1;

                if (e.Column.FieldName == "税后单价")
                {
                    try
                    {
                        decimal dec = Convert.ToDecimal(dr["税后单价"]);
                        decimal dec_q = dec / taxRate;
                        dr["税前单价"] = dec_q;
                        if (dr["数量"] != null && dr["数量"].ToString() != "")
                        {
                            dr["税后金额"] = dec * Convert.ToDecimal(dr["数量"]);
                            dr["税前金额"] = dec_q * Convert.ToDecimal(dr["数量"]);
                        }
                        // dtP.ColumnChanged += dtP_ColumnChanged;
                    }
                    catch (Exception)
                    {
                        //dtP.ColumnChanged += dtP_ColumnChanged;
                        throw new Exception("税后单价格式不正确,请检查");
                    }
                }
                if (e.Column.FieldName == "税前单价")
                {
                    try
                    {
                        // dtP.ColumnChanged -= dtP_ColumnChanged;
                        decimal dec = Convert.ToDecimal(dr["税前单价"]);
                        decimal dec_q = dec * taxRate;
                        dr["税后单价"] = dec_q;
                        if (dr["数量"] != null && dr["数量"].ToString() != "")
                        {
                            dr["税前金额"] = dec * Convert.ToDecimal(dr["数量"]);

                            dr["税后金额"] = dec_q * Convert.ToDecimal(dr["数量"]);
                        }
                        //  dtP.ColumnChanged += dtP_ColumnChanged;
                    }
                    catch (Exception)
                    {
                        // dtP.ColumnChanged += dtP_ColumnChanged;
                        throw new Exception("税后单价格式不正确,请检查");
                    }
                }
                if (e.Column.Caption == "数量")
                {
                    //DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    try
                    {

                        decimal dec_3 = Convert.ToDecimal(dr["可退货数"]);
                        if (Convert.ToDecimal(dr["数量"]) > Convert.ToDecimal(dr["可退货数"]))
                        {

                            dr["数量"] = dr["可退货数"];
                            throw new Exception("输入数量超过了此条出库记录的可退货数量");
                        }
                        // dtP.ColumnChanged -= dtP_ColumnChanged;
                        if (dr["税后单价"] != null && dr["税后单价"].ToString() != "")
                        {
                            decimal dec = Convert.ToDecimal(dr["税后单价"]);
                            dr["税后金额"] = dec * Convert.ToDecimal(dr["数量"]);
                            dr["税前金额"] = dec / taxRate * Convert.ToDecimal(dr["数量"]);
                        }
                        // dtP.ColumnChanged += dtP_ColumnChanged;
                    }
                    catch (Exception ex)
                    {
                        //  dtP.ColumnChanged += dtP_ColumnChanged;
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
        private void gc_Click(object sender, EventArgs e)
        {

        }
        //19-10-29 
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (Crm_R_No != "") throw new Exception(Crm_R_No + "退货单尚未保存,不可多条获取,可刷新后重新获取");

                Crm_R_No = searchLookUpEdit2.EditValue.ToString();

                string strcon_aliyun = string.Format("server={0};User Id={1};password={2};Database={3};CharSet=utf8", CPublic.Var.li_CFG["aliyun_server"].ToString(),
               CPublic.Var.li_CFG["aliyun_UID"].ToString(), CPublic.Var.li_CFG["aliyun_PWD"].ToString(), CPublic.Var.li_CFG["aliyun_database"].ToString());
                string s = string.Format(@"SELECT  a.*,InvoiceNo,SoCode FROM `sellreturn_main` a
                left join somain b on a.CRMCode = b.CRMSoCode where  SRcode = '{0}'", Crm_R_No);
                MySqlDataAdapter da = new MySqlDataAdapter(s, strcon_aliyun);
                DataTable dt_somain = new DataTable();
                da.Fill(dt_somain);
                if (dt_somain.Rows.Count == 0)
                {
                    Crm_R_No = "";
                    throw new Exception("未找到数据,请确认CRM订单号是否正确");
                }
                else
                {
                    if (dt_somain.Rows[0]["InvoiceNo"].ToString() == "")
                    {
                        throw new Exception("该订单尚未出库不可做退货操作,请确认");
                    }
                    string jy = String.Format("Select  * from 退货申请主表 where 备注1='{0}'", Crm_R_No);
                    DataTable jydt = CZMaster.MasterSQL.Get_DataTable(jy, strconn);
                    if (jydt.Rows.Count > 0)
                    {
                        throw new Exception("ERP中已引用过CRM中该单据,不可重复引用");
                    }
                    string str_出库单 = dt_somain.Rows[0]["InvoiceNo"].ToString();
                    string[] ck = str_出库单.Split(',');
                    string xxx = "";
                    foreach (string xs in ck)
                    {
                        xxx += "'" + xs + "',";
                    }
                    if (xxx != "") xxx = xxx.Substring(0, xxx.Length - 1);
                    //可退货清单
                    string thsql = string.Format(@"  select  ckmx.*,isnull(在退数,0)在退数,出库数量-累计退货数量-isnull(在退数,0) as 可退货数,税率,税后单价,税前单价 
                    from 销售记录成品出库单明细表 ckmx
                    left join (select 出库明细号,SUM(数量-已入库数量) as 在退数  from 退货申请子表  group by 出库明细号)y  on ckmx.成品出库单明细号= y.出库明细号
                    left join 销售记录销售订单明细表 dmx on dmx.销售订单明细号=ckmx.销售订单明细号 
                    where 成品出库单号 in ('{0}')  and  出库数量>累计退货数量+isnull(在退数,0) ", str_出库单);
                    DataTable t_thqd = CZMaster.MasterSQL.Get_DataTable(thsql, strconn);


                    drM["备注1"] = Crm_R_No;
                    drM["备注2"] = "CRM处获取的退货数据";
                    searchLookUpEdit1.EditValue = dt_somain.Rows[0]["DistributorCode"].ToString().Trim();

                    s = string.Format(" select InvCode,sum(amount)数量 from  sellreturn_details where  SRCode='{0}' group by SRCode,InvCode  ", Crm_R_No);
                    da = new MySqlDataAdapter(s, strcon_aliyun);
                    DataTable dt_sodetail = new DataTable();
                    da.Fill(dt_sodetail);
                    if (dt_sodetail.Rows.Count == 0)
                    {
                        Crm_R_No = "";
                        throw new Exception("该单号未找到明细,请通知CRM查验该单号数据是否正确");
                    }
                    foreach (DataRow dr in dt_sodetail.Rows)
                    {
                        DataRow[] rrr = t_thqd.Select(string.Format("物料编码='{0}'", dr["InvCode"]));
                        foreach (DataRow r in rrr)
                        {
                            if (Convert.ToDecimal(r["可退货数"]) > Convert.ToDecimal(dr["数量"]))
                            {
                                DataRow dr_add = dtP.NewRow();
                                dr_add["物料编码"] = dr["InvCode"];
                                dr_add["数量"] = dr["数量"];
                                dr_add["出库明细号"] = r["成品出库单明细号"];
                                dr_add["销售明细号"] = r["销售订单明细号"];
                                dr_add["通知单明细号"] = r["出库通知单明细号"];
                                dr_add["已开票数量"] = r["已开票数量"];
                                dr_add["未开票数量"] = r["未开票数量"];


                                dr_add["累计退货数量"] = r["累计退货数量"];
                                dr_add["物料名称"] = r["物料名称"];
                                dr_add["规格型号"] = r["规格型号"];
                                dr_add["税率"] = r["税率"];
                                dr_add["销售明细"] = r["销售订单明细号"];
                                dr_add["税后单价"] = r["税后单价"];
                                dr_add["税前单价"] = r["税前单价"];
                                dr_add["税前金额"] = Convert.ToDecimal(r["税前单价"]) * Convert.ToDecimal(dr["数量"]);
                                dr_add["税后金额"] = Convert.ToDecimal(r["税后单价"]) * Convert.ToDecimal(dr["数量"]);

                                dtP.Rows.Add(dr_add);
                                r["可退货数"] = Convert.ToDecimal(r["可退货数"]) - Convert.ToDecimal(dr["数量"]);
                                continue;
                            }
                            else
                            {
                                DataRow dr_add = dtP.NewRow();
                                dr_add["物料编码"] = dr["InvCode"];
                                dr_add["数量"] = r["可退货数"];
                                dr_add["出库明细号"] = r["成品出库单明细号"];
                                dr_add["销售明细号"] = r["销售订单明细号"];
                                dr_add["通知单明细号"] = r["出库通知单明细号"];
                                dr_add["已开票数量"] = r["已开票数量"];
                                dr_add["未开票数量"] = r["未开票数量"];


                                dr_add["累计退货数量"] = r["累计退货数量"];
                                dr_add["物料名称"] = r["物料名称"];
                                dr_add["规格型号"] = r["规格型号"];
                                dr_add["税率"] = r["税率"];
                                dr_add["销售明细"] = r["销售订单明细号"];
                                dr_add["税后单价"] = r["税后单价"];
                                dr_add["税前单价"] = r["税前单价"];
                                dr_add["税前金额"] = Convert.ToDecimal(r["税前单价"]) * Convert.ToDecimal(r["可退货数"]);
                                dr_add["税后金额"] = Convert.ToDecimal(r["税后单价"]) * Convert.ToDecimal(r["可退货数"]);
                                dtP.Rows.Add(dr_add);
                                r["可退货数"] = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Crm_R_No = "";
                MessageBox.Show(ex.Message);
            }
        }
        //20-6-3提交审核   郭恒
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                check();
                if (bl)
                {
                    if (MessageBox.Show("明细中有明细有已开票数量,是否确认", "提醒", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        bl = false;
                    }
                }
                if (!bl)
                {
                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    fun_保存主表明细(true);
                    ////19-10-29  需要回馈给CRM销售订单号
                    //if (Crm_R_No.Trim() != "")
                    //{
                    //    string strcon_aliyun = string.Format("server={0};User Id={1};password={2};Database={3};CharSet=utf8", CPublic.Var.li_CFG["aliyun_server"].ToString(),
                    //    CPublic.Var.li_CFG["aliyun_UID"].ToString(), CPublic.Var.li_CFG["aliyun_PWD"].ToString(), CPublic.Var.li_CFG["aliyun_database"].ToString());
                    //    string s = string.Format(" select * from  sellreturn_main  where bl=0 and SRCode='{0}'", Crm_R_No);
                    //    using (MySqlDataAdapter da = new MySqlDataAdapter(s, strcon_aliyun))
                    //    {
                    //        DataTable dt_somain = new DataTable();
                    //        da.Fill(dt_somain);
                    //        dt_somain.Rows[0]["bl"] = true;
                    //        new MySqlCommandBuilder(da);
                    //        da.Update(dt_somain);
                    //    }
                    //}
                    i_状态 = 1;//提交状态
                    op_zt(i_状态);
                    MessageBox.Show("提交成功");
                    refsh(txt_退货申请单号.Text);
                    barLargeButtonItem4.Enabled = false;
                    //barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    barLargeButtonItem5.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //20-6-3 保存
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                check();
                if (bl)
                {
                    if (MessageBox.Show("明细中有明细有已开票数量,是否确认", "提醒", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        bl = false;
                    }
                }
                if (!bl)
                {
                    gv.CloseEditor();
                    this.BindingContext[dtP].EndCurrentEdit();
                    fun_保存主表明细(false);
                    MessageBox.Show("保存成功");
                    refsh(txt_退货申请单号.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //20-6-3撤销提交
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
                drM["提交审核"] = false;
                string sql_1 = $"select * from 单据审核申请表 where  审核=0 and 作废=0 and 单据类型='销售退货' and  关联单号 = '{txt_退货申请单号.Text}'";
                DataTable dt_单据审核 = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);
                if (dt_单据审核.Rows.Count > 0) dt_单据审核.Rows[0].Delete();
                Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
                dic.Add("退货申请主表", dtM);
                dic.Add("单据审核申请表", dt_单据审核);
                ERPorg.Corg cg = new ERPorg.Corg();
                cg.save(dic);
                refsh(txt_退货申请单号.Text);

                i_状态 = 0;
                op_zt(i_状态);
                // barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barLargeButtonItem5.Enabled = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
