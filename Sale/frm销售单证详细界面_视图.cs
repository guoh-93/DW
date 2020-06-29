using CZMaster;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class frm销售单证详细界面_视图 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        /// <summary>
        /// 明细表
        /// </summary>
        DataTable dtP;
        DataTable dtP_副 = null;
        DataTable dt_差异_增 = null;
        DataTable dt_差异_减 = null;
        DataTable dtM = new DataTable();
        DataRow drM = null;
        string str_销售订单号 = "";

        DataTable dt_物料信息 = new DataTable();

        DataTable dt_生产 = new DataTable();
        DataTable dt_采购 = new DataTable();
        DataTable dt_物料数量 = new DataTable();

        /// <summary>
        /// 传过来的dt采购，由计算生成
        /// </summary>
        DataTable dt1_采购 = null;
        /// <summary>
        /// 传过来的dt生产，由计算生成
        /// </summary>
        DataTable dt2_生产 = null;
        /// <summary>
        /// 传过来的dt_物料数量，生效MRP3种数量
        /// </summary>
        DataTable dt3 = null;
        /// <summary>
        /// 传过来的dM，用于区分明细是否以计算 
        /// </summary>
        DataTable dt4 = null;
        /// <summary>
        /// 由 采购记录采购计划表 读取
        /// </summary>
        DataTable dtM_采购;
        /// <summary>
        /// 由 生产记录生产计划表 读取
        /// </summary>
        DataTable dtM_生产;
        int POS = 0;
        string cfgfilepath = "";
        #endregion

        #region 自用类
        public frm销售单证详细界面_视图(DataRow dr, string s_销售订单号)
        {
            InitializeComponent();
            //drM = dr;
            str_销售订单号 = s_销售订单号;
            fun_载入主表();

        }

        public frm销售单证详细界面_视图(string s_销售订单号)
        {
            //没用到
            InitializeComponent();
            str_销售订单号 = s_销售订单号;
            fun_载入主表();
        }

        private void frm销售单证详细界面_视图_Load(object sender, EventArgs e)
        {
            try
            {

                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
                {

                    gvP.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }
                fun_载入主表();
                dataBindHelper1.DataFormDR(drM);
                fun_载入明细();

                fun_物料下拉框();
                //dtP.ColumnChanged += dtP_ColumnChanged;
                #region 判断界面按钮是否可用
                foreach (DataRow r in dtP.Rows)
                {
                    if (r["明细完成"].ToString() == "True")
                    {
                        barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }
                    //if (r_x["已计算"].ToString() == "False")
                    //{
                    //    barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    //}
                }
                if (drM["作废"].ToString() == "True" || drM["完成"].ToString() == "True")
                {
                    barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barLargeButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    button3.Visible = false;
                    button1.Visible = false;
                    button5.Location = new Point(3, 4);


                    gvP.OptionsBehavior.Editable = false;
                }
                if (drM["生效"].ToString() == "False")
                {
                    barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
                if (checkBox1.Checked == true)
                {
                    button4.Enabled = true;
                    button2.Enabled = true;
                }
                else
                {
                    button4.Enabled = false;
                    button2.Enabled = false;
                }
                #endregion
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售订单视图");
            }
        }
        /// <summary>
        /// 不用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtP_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.Text) / (Decimal)100);
            if (e.Column.ColumnName == "物料编码")
            {
                string ss = e.Row["物料编码"].ToString();
                DataRow[] ds = dt_物料信息.Select(string.Format("物料编码 = '{0}'", ss));
                try
                {
                    e.Row["物料名称"] = ds[0]["物料名称"].ToString();
                    e.Row["计量单位"] = ds[0]["计量单位"].ToString();
                    e.Row["n原ERP规格型号"] = ds[0]["n原ERP规格型号"].ToString();
                    e.Row["规格型号"] = ds[0]["规格"].ToString();
                    e.Row["原ERP物料编号"] = ds[0]["原ERP物料编号"].ToString();
                    try
                    {
                        //合约金额
                        e.Row["税后单价"] = fun_明细金额(ds[0]).ToString("0.000000");
                        e.Row["税前单价"] = (fun_明细金额(ds[0]) / ((Decimal)1 + dec税率)).ToString("0.000000");
                    }
                    catch
                    {
                        //产品标准单价
                        e.Row["税后单价"] = (Convert.ToDecimal(ds[0]["标准单价"])).ToString("0.000000");
                        e.Row["税前单价"] = (Convert.ToDecimal(ds[0]["标准单价"]) / ((Decimal)1 + dec税率)).ToString("0.000000");
                    }
                }
                catch { }
            }
        }
        private void fun_check()
        {
            int i_total = 0;
            int i_finish = 0;

            foreach (DataRow r in dtP.Rows)
            {
                i_total++;

                if (Convert.ToDecimal(r["数量"]) <= 0)
                {
                    throw new Exception(string.Format("数量不能小于等于0,物料:{0}", r["物料编码"]).ToString());
                }
                if (Convert.ToDecimal(r["数量"]) == Convert.ToDecimal(r["完成数量"]))
                {
                    r["明细完成"] = true;
                    r["明细完成日期"] = CPublic.Var.getDatetime();
                }
                if (r["明细完成"].Equals(true))
                {
                    i_finish++;
                }

            }
            if (i_finish == i_total)  // 主表赋完成
            {
                drM["完成"] = true;
                drM["完成日期"] = CPublic.Var.getDatetime();
                drM["完成人员"] = CPublic.Var.localUserName;
                drM["完成人员ID"] = CPublic.Var.LocalUserID;
            }
            if (drM["文件GUID"].ToString() != "")
            {
                drM["订单原件"] = true;
            }
            else
            {
                drM["订单原件"] = false;
            }
        }
        private Decimal fun_明细金额(DataRow r)
        {
            //有合同价使用合同价
            string sql = string.Format("select * from 产品对应关系表 where 客户编号 = '{0}' and 产品编号 = '{1}'", txt_客户编号.Text.ToString(), r["物料编码"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            decimal de_金额 = 0;
            if (Convert.ToDecimal(dt.Rows[0]["税后价格"]) > (Decimal)0)
            {
                de_金额 = Convert.ToDecimal(dt.Rows[0]["税后价格"]);
            }
            else
            {
                de_金额 = Convert.ToDecimal(dt.Rows[0]["税前价格"]) * Convert.ToDecimal(1.17);
            }
            return de_金额;
        }

        private void gvP_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Value.ToString() != "")
            {
                Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.Text) / (Decimal)100);
                DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                if (e.Column.Caption == "税后单价" || e.Column.Caption == "数量")
                {
                    if (Convert.ToDecimal(dr["税后单价"]) >= (Decimal)0)
                    {
                        //if (e.Row["税前单价"] == DBNull.Value || Convert.ToDecimal(e.Row["税前单价"]) != (Convert.ToDecimal(e.Row["税后单价"]) / ((Decimal)1 + dec税率)))
                        {
                            dr["税前单价"] = (Convert.ToDecimal(dr["税后单价"]) / ((Decimal)1 + dec税率)).ToString("0.000000");

                        }
                    }
                    fun_明细金额变化();
                }
                if (e.Column.Caption == "税前单价")
                {
                    if (Convert.ToDecimal(dr["税前单价"]) >= (Decimal)0)
                    {
                        //if (dr["税后单价"] == DBNull.Value || Convert.ToDecimal(dr["税前单价"]) != (Convert.ToDecimal(dr["税后单价"]) / ((Decimal)1 + dec税率)))
                        {
                            dr["税后单价"] = (Convert.ToDecimal(dr["税前单价"]) * ((Decimal)1 + dec税率)).ToString("0.000000");
                        }
                    }
                    fun_明细金额变化();
                }
            }
        }

        //计算明细金额，以及总金额
        private void fun_明细金额变化(Boolean blErr = false)
        {
            System.Decimal sum = 0;
            System.Decimal sum1 = 0;
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted || r["关闭"].Equals(true))
                {
                    continue;
                }

                try
                {

                    r["税后金额"] = ((Decimal)r["税后单价"] * (Decimal)r["数量"]).ToString("0.000000");
                    sum += (Decimal)r["税后金额"];
                    r["税前金额"] = ((Decimal)r["税前单价"] * (Decimal)r["数量"]).ToString("0.000000");
                    sum1 += (Decimal)r["税前金额"];
                }
                catch
                {
                    if (blErr)
                    {
                        throw new Exception(string.Format("{0}的单价或物料出错！", r["物料名称"].ToString()));
                    }
                }
            }
            txt_税前金额.Text = sum1.ToString();
            txt_税后金额.Text = sum.ToString();
        }

        private void txt_金额_TextChanged(object sender, EventArgs e)
        {
            //if (txt_税后金额.Text.ToString() != "")
            //{
            //    Decimal de_金额 = Convert.ToDecimal(txt_税后金额.Text.ToString());
            //    try
            //    {
            //        Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.Text) / (Decimal)100);
            //        txt_税前金额.Text = (de_金额 / ((Decimal)1 + dec税率)).ToString("0.000000");
            //    }
            //    catch
            //    {
            //        txt_税前金额.Text = "0.000000";
            //    }
            //}
            //else
            //{
            //    txt_税前金额.Text = "0.000000";
            //}
        }
        #endregion

        #region 载入订单
        private void fun_载入主表()
        {
            try
            {
                string sql = string.Format("select * from 销售记录销售订单主表 where 销售订单号 = '{0}'", str_销售订单号);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                drM = dtM.Rows[0];

                bool bl = false;

                if (CPublic.Var.LocalUserTeam == "管理员权限")
                {
                    bl = true;
                }
                else
                {
                    sql = $"select 在职状态,部门编号 from 人事基础员工表 where 员工号 = '{drM["生效人员ID"].ToString()}'";
                    DataTable dt_人事 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_人事.Rows.Count > 0)
                    {
                        if (dt_人事.Rows[0]["在职状态"].ToString() == "离职")
                        {
                            if (dt_人事.Rows[0]["部门编号"].ToString() == CPublic.Var.localUser部门编号)
                            {
                                bl = true;
                            }
                        }
                        else
                        {
                            if (drM["生效人员ID"].ToString() == CPublic.Var.LocalUserID)
                            {
                                bl = true;
                            }
                        }
                    }
                }
                               
                barLargeButtonItem6.Enabled = bl;



                if (drM["文件GUID"].ToString() != "")
                {
                    checkBox1.Checked = true;
                }
                else
                {
                    checkBox1.Checked = false;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面_fun_载入主表");
            }
        }

        private void fun_载入明细()
        {
            try
            {

                //smx.作废 = 0 19-12-25 取消
                string sql = string.Format(@"select smx.*,base.物料类型,可售 from 销售记录销售订单明细表 smx
                left join 基础数据物料信息表 base on base.物料编码 = smx.物料编码 
                left join 仓库物料数量表 kc on kc.物料编码=smx.物料编码 and kc.仓库号=smx.仓库号
                where     销售订单号 = '{0}'", str_销售订单号);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    dtP = new DataTable();
                    da.Fill(dtP);
                    gcP.DataSource = dtP;
                    dtP_副 = dtP.Clone();
                    foreach (DataRow r in dtP.Rows)
                    {
                        DataRow dr = dtP_副.NewRow();
                        dtP_副.Rows.Add(dr);
                        dr.ItemArray = r.ItemArray;
                    }
                }
                sql = string.Format("select max(POS)POS from 销售记录销售订单明细表 where 销售订单号 = '{0}'", str_销售订单号);
                SqlDataAdapter daa = new SqlDataAdapter(sql, strconn);
                DataTable dt = new DataTable();
                daa.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    POS = Convert.ToInt32(dt.Rows[0]["POS"]);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单界面_fun_载入明细");
            }
        }
        #endregion

        #region 界面按钮 关闭界面
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        #region GH 右键查看制令明细

        private void gvP_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gcP, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看关联制令ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ERPSale.UI关联制令界面 frm = new ERPSale.UI关联制令界面(drM);
            CPublic.UIcontrol.AddNewPage(frm, "有关制令明细");
        }
        #endregion

        #region 作废
        private void fun_保存订单()
        {
            try
            {
                string sql = string.Format("select * from 销售记录销售订单主表 where GUID = '{0}'", drM["GUID"]);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dtM = new DataTable();
                da.Fill(dtM);
                dtM.Rows[0]["作废"] = 1;
                dtM.Rows[0]["作废人员"] = CPublic.Var.localUserName;
                dtM.Rows[0]["作废人员ID"] = CPublic.Var.LocalUserID;
                dtM.Rows[0]["作废日期"] = CPublic.Var.getDatetime();
                dtM.Rows[0]["完成"] = 1;
                new SqlCommandBuilder(da);
                da.Update(dtM);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单视图界面保存订单");
                throw new Exception(ex.Message);
            }
        }
        private void fun_保存明细()
        {
            try
            {
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    r["作废"] = 1;
                    r["作废日期"] = CPublic.Var.getDatetime();
                    r["明细完成"] = 1;
                    r["总完成"] = 1;
                }
                string sql = "select * from 销售记录销售订单明细表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dtP);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单视图界面保存明细");
                throw new Exception("保存失败！");
            }
        }

        private void fun_保存作废记录()
        {
            try
            {
                string sql_作废记录 = "select * from 销售记录订单作废记录表 where 1<>1";
                DataTable dt_作废记录 = new DataTable();
                SqlDataAdapter da_作废记录 = new SqlDataAdapter(sql_作废记录, strconn);
                da_作废记录.Fill(dt_作废记录);
                foreach (DataRow r in dtP.Rows)
                {
                    DataRow dr = dt_作废记录.NewRow();
                    dt_作废记录.Rows.Add(dr);
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["销售订单明细号"] = r["销售订单明细号"];
                    //1.是否计算过
                    if (r["已计算"].ToString().Trim() == "true")
                    {
                        dr["是否计算过"] = 1;
                        //2.是否转制令
                        string sql = string.Format("select * from 生产记录生产制令子表 where 销售订单明细号 = '{0}'", r["销售订单明细号"].ToString());
                        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                        DataTable dt2 = new DataTable();
                        da.Fill(dt2);
                        if (dt2.Rows.Count > 0)
                        {
                            dr["是否转制令"] = 1;
                            foreach (DataRow rr in dt2.Rows)
                            {
                                dr["制令数量"] = Convert.ToDecimal(rr["数量"]) + Convert.ToDecimal(dr["制令数量"]);
                                //3.是否转工单
                                sql = string.Format("select * from 生产记录生产工单表 where 生产制令单号 = '{0}'", rr["生产制令单号"].ToString());
                                da = new SqlDataAdapter(sql, strconn);
                                DataTable dt3 = new DataTable();
                                da.Fill(dt3);
                                if (dt3.Rows.Count > 0)
                                {
                                    dr["是否转工单"] = 1;
                                    //4.是否领料
                                    sql = string.Format("select * from 生产记录生产工单待领料明细表 where 生产制令单号 = '{0}' and 未领数量 <= 0", rr["生产制令单号"].ToString());
                                    da = new SqlDataAdapter(sql, strconn);
                                    DataTable dt4 = new DataTable();
                                    da.Fill(dt4);
                                    if (dt4.Rows.Count > 0)
                                    {
                                        dr["是否领料"] = 1;
                                        foreach (DataRow rrr in dt3.Rows)
                                        {
                                            //5.是否入库
                                            sql = string.Format("select * from 生产记录成品入库单明细表 where 生产工单号 = '{0}'", rrr["生产工单号"].ToString());
                                            da = new SqlDataAdapter(sql, strconn);
                                            DataTable dt5 = new DataTable();
                                            da.Fill(dt5);
                                            if (dt5.Rows.Count > 0)
                                            {
                                                dr["是否入库"] = 1;
                                            }
                                            else
                                            {
                                                dr["是否入库"] = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        dr["是否领料"] = 0;
                                        dr["是否入库"] = 0;
                                    }
                                }
                                else
                                {
                                    dr["是否转工单"] = 0;
                                    dr["是否领料"] = 0;
                                    dr["是否入库"] = 0;
                                }
                            }
                        }
                        else
                        {
                            dr["是否转制令"] = 0;
                            dr["制令数量"] = 0;
                            dr["是否转工单"] = 0;
                            dr["是否领料"] = 0;
                            dr["是否入库"] = 0;
                        }
                    }
                    else
                    {
                        dr["是否计算过"] = 0;
                        dr["是否转制令"] = 0;
                        dr["制令数量"] = 0;
                        dr["是否转工单"] = 0;
                        dr["是否领料"] = 0;
                        dr["是否入库"] = 0;
                    }
                }
                //保存记录
                new SqlCommandBuilder(da_作废记录);
                da_作废记录.Update(dt_作废记录);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售订单-作废");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("是否要作废此订单？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //fun_保存作废记录();
                fun_保存订单();
                fun_保存明细();
                //生效
                //受订量变化，有效总量变化
                foreach (DataRow r in dtP.Rows)
                {
                    StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(), r["仓库号"].ToString(), true);
                }
                fun_保存作废记录();
                MessageBox.Show("作废完成");

            }
        }
        #endregion

        #region 关闭订单
        private void fun_保存订单_关闭()
        {
            try
            {
                string sql = string.Format("select * from 销售记录销售订单主表 where GUID = '{0}'", drM["GUID"]);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dtM = new DataTable();
                da.Fill(dtM);
                dtM.Rows[0]["关闭"] = 1;
                dtM.Rows[0]["完成"] = 1;
                dtM.Rows[0]["完成人员"] = CPublic.Var.localUserName;
                dtM.Rows[0]["完成人员ID"] = CPublic.Var.LocalUserID;
                dtM.Rows[0]["完成日期"] = CPublic.Var.getDatetime();
                new SqlCommandBuilder(da);
                da.Update(dtM);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单视图界面 关闭保存订单");
                throw new Exception(ex.Message);
            }
        }
        private void fun_保存明细_关闭()
        {
            try
            {
                foreach (DataRow r in dtP.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    if (r["明细完成"].ToString().ToLower() == "false")
                    {
                        r["关闭"] = 1;
                    }
                    r["明细完成"] = 1;
                    r["明细完成日期"] = CPublic.Var.getDatetime();
                    r["总完成"] = 1;
                    r["总完成日期"] = CPublic.Var.getDatetime();
                    //MRP库存锁定量 减少
                    string l = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", r["物料编码"]);
                    DataTable t = new DataTable();
                    SqlDataAdapter a = new SqlDataAdapter(l, strconn);
                    a.Fill(t);
                    new SqlCommandBuilder(a);
                    if (t.Rows.Count > 0)
                    {
                        t.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(t.Rows[0]["MRP库存锁定量"]) - Convert.ToDecimal(r["关闭数量"]);
                    }
                    a.Update(t);
                }
                string sql = "select * from 销售记录销售订单明细表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dtP);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单视图界面 关闭保存明细");
                throw new Exception("保存失败！");
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow[] rr = dtP.Select(string.Format("完成数量>0"));
                if (rr.Length > 0)
                {
                    throw new Exception("已有明细有过销售出库记录，不可关闭整张订单");
                }

                if (MessageBox.Show("是否要关闭此订单？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        dtP.Columns.Add("关闭数量");
                    }
                    catch { }
                    ERPSale.frm关闭订单窗口 fm = new ERPSale.frm关闭订单窗口(drM, dtP);
                    fm.ShowDialog();
                    if (fm.blResult == true)
                    {
                        //主表和明细表 完成 = 1
                        fun_保存订单_关闭();
                        fun_保存明细_关闭();
                        //受订量变化，有效总量变化
                        foreach (DataRow r in dtP.Rows)
                        {
                            StockCore.StockCorer.fun_物料数量_实际数量(r["物料编码"].ToString(), r["仓库号"].ToString(), true);
                        }
                        MessageBox.Show("订单已关闭");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 导出TXT
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //drM  dr_传
                string file = string.Format(@"C://{0}_{1}.txt", str_销售订单号, System.DateTime.Today.ToString("yyyy-MM-dd"));
                string strSoNo = string.Format("{0}{1}{2}{3}", DateTime.Now.Year.ToString().Substring(2, 2), DateTime.Now.Month.ToString("00"),
                    DateTime.Now.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("XSKP", DateTime.Now.Year, DateTime.Now.Month).ToString("000"));
                string content = strSoNo + "," + dtP.Rows.Count + "," + txt_客户名称.Text + ",,,,";
                foreach (DataRow dr in dtP.Rows)
                {
                    content = content + Environment.NewLine + dr["物料名称"].ToString() + "," + dr["规格型号"].ToString() + "," + dr["计量单位"].ToString()
                        + "," + dr["数量"].ToString() + "," + dr["税后单价"].ToString() + "," + txt_税率.Text.ToString() + ",1601,0";
                }
                if (File.Exists(file) == true)
                {
                    System.IO.File.WriteAllText(file, content);
                }
                else
                {
                    FileStream myFs = new FileStream(file, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs);
                    mySw.Write(content);
                    mySw.Close();
                    myFs.Close();
                }
                MessageBox.Show("已完成导出！");
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");

            }
        }
        #endregion

        #region 上传下载
        string strcon_FS = CPublic.Var.geConn("FS");
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (drM == null)
                {
                    throw new Exception("请先新增销售订单！");
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
                    string strguid = "";  //记录系统自动返回的GUID
                    strguid = CFileTransmission.CFileClient.sendFile(open.FileName);
                    drM["文件GUID"] = strguid;
                    drM["文件"] = Path.GetFileName(open.FileName);
                    drM["上传时间"] = System.DateTime.Now;
                    MessageBox.Show("上传成功！");
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
                if (drM == null)
                {
                    throw new Exception("请重新选择销售订单！");
                }

                SaveFileDialog save = new SaveFileDialog();
                save.FileName = drM["文件"].ToString();
                if (save.ShowDialog() == DialogResult.OK)
                {
                    CFileTransmission.CFileClient.strCONN = strcon_FS;
                    CFileTransmission.CFileClient.Receiver(drM["文件GUID"].ToString(), save.FileName);
                    MessageBox.Show("下载成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                button4.Enabled = true;
                button2.Enabled = true;
            }
            else
            {
                button4.Enabled = false;
                button2.Enabled = false;
            }
        }
        #endregion

        #region 修改订单生效
        string[] sd = null;
        private void fun_获取主表信息()
        {
            drM["客户订单号"] = txt_客户订单号.Text;
            drM["修改日期"] = CPublic.Var.getDatetime();
            drM["销售备注"] = txt_销售备注.Text;
            drM["税前金额"] = txt_税前金额.Text;
            drM["税后金额"] = txt_税后金额.Text;
            //int i = 1;
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                r["销售订单号"] = textBox1.Text;
                //r_x["POS"] = i++;
                if (r["销售订单明细号"].ToString() == "")
                {  //r_x["销售订单明细号"] = textBox1.Text + r_x["物料编码"].ToString();
                    r["销售订单明细号"] = textBox1.Text + "-" + (++POS).ToString("00");
                    r["POS"] = POS;

                }
                decimal dec_已通知 = 0;
                string s = string.Format("select  sum(出库数量)已通知数量 from 销售记录销售出库通知单明细表 where 作废=0 and 销售订单明细号='{0}'", r["销售订单明细号"].ToString());
                using (SqlDataAdapter da = new SqlDataAdapter(s, strconn))
                {
                    DataTable temp = new DataTable();
                    da.Fill(temp);
                    if (temp.Rows.Count > 0 && temp.Rows[0][0].ToString() != "")
                    {
                        dec_已通知 = Convert.ToDecimal(temp.Rows[0][0]);
                    }

                    r["已通知数量"] = dec_已通知;
                }


                decimal dec_未完成数量 = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["完成数量"]);
                if (dec_未完成数量 < 0)
                {
                    throw new Exception("已完成数量大于即将要修改的数量,请确认");
                }
                if (dec_未完成数量 == 0)
                {
                    r["明细完成"] = true;
                    r["明细完成日期"] = CPublic.Var.getDatetime();

                }
                r["未完成数量"] = dec_未完成数量;

                decimal dec_未通知数量 = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(r["已通知数量"]);
                if (dec_未通知数量 < 0)
                {
                    throw new Exception("已通知数量大于即将要修改的数量,请确认");
                }
                r["未通知数量"] = dec_未通知数量;
                if (r["含税销售价"].ToString() == "" || Convert.ToDecimal(r["含税销售价"].ToString()) == 0)
                {
                    r["含税销售价"] = r["税后单价"];
                }
                r["生效"] = true;

                if (r["生效日期"] == DBNull.Value)
                {
                    r["生效日期"] = CPublic.Var.getDatetime();
                    r["修改日期"] = CPublic.Var.getDatetime();
                }
                else
                {
                    DateTime dt = new DateTime(1900, 1, 1);

                    DateTime t = Convert.ToDateTime(r["生效日期"]);//&&  DateTime.Compare(t,dt)==0
                    if (r["生效日期"] == null || r["生效日期"].ToString() == "" || DateTime.Compare(t, dt) == 0)
                    {
                        r["生效日期"] = CPublic.Var.getDatetime();

                    }
                    r["修改日期"] = CPublic.Var.getDatetime();
                }

                string str = r["生效日期"].ToString();
            }
        }

        private void fun_物料下拉框()
        {
            try
            {

                string sql2 = @"select base.物料名称,base.物料编码,base.规格型号,kc.货架描述,
               base.计量单位,base.标准单价,base.特殊备注,kc.有效总数,kc.库存总数,kc.在制量,kc.受订量
              from 基础数据物料信息表 base  left join 仓库物料数量表 kc on base.物料编码 = kc.物料编码
           where (base.内销=1 or base.外销=1) and base.停用=0";
                SqlDataAdapter da = new SqlDataAdapter(sql2, strconn);
                da.Fill(dt_物料信息);

                repositoryItemSearchLookUpEdit1.PopupFormSize = new Size(1400, 400);
                repositoryItemSearchLookUpEdit1.DataSource = dt_物料信息;
                repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
                repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_对比前后订单()
        {
            sd = new string[dtP.Rows.Count + dtP_副.Rows.Count];
            int i = 0;
            //dr_传:修改之后的 dtP_副：修改之前的
            dt_差异_增 = dtP.Clone();
            dt_差异_减 = dtP.Clone();
            foreach (DataRow r in dtP.Rows)
            {
                if (r.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                sd[i] = r["物料编码"].ToString();
                if (r["已计算"].ToString().ToLower() == "false")
                {
                    continue;
                }

                i++;
                DataRow[] ds = dtP_副.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                if (ds.Length > 0)
                {
                    //查看是否修改过
                    if (Convert.ToDecimal(r["数量"]) > Convert.ToDecimal(ds[0]["数量"]))
                    {
                        DataRow dr = dt_差异_增.NewRow();
                        dt_差异_增.Rows.Add(dr);
                        dr.ItemArray = r.ItemArray;
                        dr["数量"] = Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(ds[0]["数量"]);
                        dr["未完成数量"] = Convert.ToDecimal(dr["数量"]);
                        dr["未通知数量"] = Convert.ToDecimal(dr["数量"]);

                        r["未完成数量"] = Convert.ToDecimal(r["未完成数量"]) + Convert.ToDecimal(dr["数量"]);
                        r["未通知数量"] = Convert.ToDecimal(r["未通知数量"]) + Convert.ToDecimal(dr["数量"]);
                        r["修改日期"] = System.DateTime.Now;
                    }
                    if (Convert.ToDecimal(r["数量"]) < Convert.ToDecimal(ds[0]["数量"]))
                    {
                        DataRow dr = dt_差异_减.NewRow();
                        dt_差异_减.Rows.Add(dr);
                        dr.ItemArray = r.ItemArray;
                        dr["数量"] = Convert.ToDecimal(ds[0]["数量"]) - Convert.ToDecimal(r["数量"]);
                        dr["未完成数量"] = Convert.ToDecimal(dr["数量"]);
                        dr["未通知数量"] = Convert.ToDecimal(dr["数量"]);

                        r["未完成数量"] = Convert.ToDecimal(r["未完成数量"]) - Convert.ToDecimal(dr["数量"]);
                        r["未通知数量"] = Convert.ToDecimal(r["未通知数量"]) - Convert.ToDecimal(dr["数量"]);
                        r["修改日期"] = System.DateTime.Now;
                    }

                }
                else
                {
                    //该行为新增
                    DataRow dr = dt_差异_增.NewRow();
                    dt_差异_增.Rows.Add(dr);
                    dr.ItemArray = r.ItemArray;
                    r["未完成数量"] = Convert.ToDecimal(dr["数量"]);
                    r["未通知数量"] = Convert.ToDecimal(dr["数量"]);
                    dr["未完成数量"] = Convert.ToDecimal(dr["数量"]);
                    dr["未通知数量"] = Convert.ToDecimal(dr["数量"]);
                    r["修改日期"] = System.DateTime.Now;

                }
            }
            foreach (DataRow rr in dtP_副.Rows)
            {
                if (rr.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                if (rr["已计算"].ToString().ToLower() == "false")
                {
                    continue;
                }

                DataRow[] ds = dtP.Select(string.Format("物料编码 = '{0}'", rr["物料编码"].ToString()));
                if (ds.Length > 0)
                {
                    //查看是否修改过
                    //上一个循环已经比对过
                }
                else
                {
                    sd[i] = rr["物料编码"].ToString();
                    i++;
                    //该行为删除
                    DataRow dr = dt_差异_减.NewRow();
                    dt_差异_减.Rows.Add(dr);
                    dr.ItemArray = rr.ItemArray;
                    dr["数量"] = Convert.ToDecimal(rr["数量"]);
                    dr["未完成数量"] = Convert.ToDecimal(dr["数量"]);
                    dr["未通知数量"] = Convert.ToDecimal(dr["数量"]);
                }
            }
        }
        private void fun_主表状态()
        {
            string sql = string.Format("select * from  销售记录销售订单明细表 where 销售订单号='{0}' and 明细完成=0 and 关闭=0 ", textBox1.Text);  //找未完成的
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt.Rows.Count == 0)
            {
                string sql_1 = string.Format("update 销售记录销售订单主表 set 完成=1 where  销售订单号='{0}'", textBox1.Text);
                CZMaster.MasterSQL.ExecuteSQL(sql_1, strconn);
            }


        }
        private void fun_MRP分析并保存()
        {
            #region 生产采购
            try
            {
                dt_生产 = new DataTable();
                dt_采购 = new DataTable();
                dt_生产.Columns.Add("物料编码");
                dt_生产.Columns.Add("物料名称");
                dt_生产.Columns.Add("规格型号");
                dt_生产.Columns.Add("图纸编号");
                dt_生产.Columns.Add("特殊备注");
                dt_生产.Columns.Add("原规格型号");
                dt_生产.Columns.Add("POS");
                dt_生产.Columns.Add("物料类型");
                dt_生产.Columns.Add("层级");
                dt_生产.Columns.Add("订单数量", typeof(Decimal));
                dt_生产.Columns.Add("欠缺数量", typeof(Decimal));
                dt_生产.Columns.Add("上级物料");
                dt_生产.Columns.Add("已计算");

                dt_采购.Columns.Add("物料编码");
                dt_采购.Columns.Add("物料名称");
                dt_采购.Columns.Add("规格型号");
                dt_采购.Columns.Add("图纸编号");
                dt_采购.Columns.Add("物料类型");
                dt_采购.Columns.Add("物料数量", typeof(Decimal));
                dt_采购.Columns.Add("仓库参考数量", typeof(Decimal));
                dt_采购.Columns.Add("总需数量", typeof(Decimal));
            }
            catch { }
            #endregion

            if (dt_差异_增.Rows.Count == 0) { }
            else
            {
                int i = 0;
                foreach (DataRow r in dt_差异_增.Rows)
                {
                    r["POS"] = i++;
                }

                ERPSale.frm销售明细分析界面 fm = new ERPSale.frm销售明细分析界面();

                string sql_物料数量 = string.Format(@"select 仓库物料数量表.*,可售 from 仓库物料数量表,基础数据物料信息表
                                    where 仓库物料数量表.物料编码 =基础数据物料信息表.物料编码 and  1<>1");
                SqlDataAdapter da_物料数量 = new SqlDataAdapter(sql_物料数量, strconn);
                da_物料数量.Fill(dt_物料数量);

                DataSet dset = new DataSet();
                dset = fm.fun_生成表一和表二(dt_生产, dt_采购, dt_物料数量, dt_差异_增, 1);
                foreach (DataTable t in dset.Tables)
                {
                    if (t.TableName == "采购计划")
                    {
                        dt_采购 = t;
                    }
                    if (t.TableName == "生产计划")
                    {
                        dt_生产 = t;
                    }
                    if (t.TableName == "物料数量")
                    {
                        dt_物料数量 = t;
                    }
                }

                dt1_采购 = dt_采购;
                dt2_生产 = dt_生产;
                dt3 = dt_物料数量;
                dt4 = dt_差异_增;
                try
                {
                    dt1_采购.Columns.Add("库存有效数量");
                    dt1_采购.Columns.Add("序号");
                    dt1_采购.Columns.Add("单位");
                    dt1_采购.Columns.Add("图纸编号1");
                    dt2_生产.Columns.Add("库存有效数量");
                    dt2_生产.Columns.Add("序号");
                    dt2_生产.Columns.Add("单位");
                }
                catch { }

                fun_载入_MRP();
                fun_保存_MRP();

                foreach (DataRow r in dt4.Rows)
                {
                    DataRow[] ds = dtP.Select(string.Format("物料编码 = '{0}'", r["物料编码"]));
                    if (ds.Length > 0)
                    {
                        foreach (DataRow rr in ds)
                        {
                            rr["已计算"] = true;
                        }
                    }
                }
            }
        }

        private void fun_处理删除或减少()//减少量为n
        {
            DataTable dt_半成品 = new DataTable();
            dt_半成品.Columns.Add("物料编码");
            dt_半成品.Columns.Add("数量");

            if (dt_差异_减.Rows.Count == 0) { }
            else
            {
                foreach (DataRow dr in dt_差异_减.Rows)
                {
                    #region 成品
                    //1.查询计划表数量是否大于减少数（删除也认为是减少数）
                    //2.计划表数量减少（最多到0），MRP计划生产量减少（最多到0），MRP库存锁定量减少
                    string sql_计划池 = string.Format("select * from 生产记录生产计划表 where 物料编码 = '{0}' and 生产计划类型 = 'MRP类型' and 未生成数量 > 0", dr["物料编码"].ToString());
                    DataTable dt_计划池 = new DataTable();
                    SqlDataAdapter da_计划池 = new SqlDataAdapter(sql_计划池, strconn);
                    da_计划池.Fill(dt_计划池);

                    string sql_物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", dr["物料编码"].ToString());
                    DataTable dt_物料数量 = new DataTable();
                    SqlDataAdapter da_物料数量 = new SqlDataAdapter(sql_物料数量, strconn);
                    da_物料数量.Fill(dt_物料数量);

                    if (dt_计划池.Rows.Count > 0)
                    {
                        //1
                        if (Convert.ToDecimal(dt_计划池.Rows[0]["未生成数量"]) >= Convert.ToDecimal(dr["数量"]))
                        {
                            dt_计划池.Rows[0]["未生成数量"] = Convert.ToDecimal(dt_计划池.Rows[0]["未生成数量"]) - Convert.ToDecimal(dr["数量"]);
                            dt_计划池.Rows[0]["计划数量"] = Convert.ToDecimal(dt_计划池.Rows[0]["计划数量"]) - Convert.ToDecimal(dr["数量"]);
                            dt_物料数量.Rows[0]["MRP计划生产量"] = Convert.ToDecimal(dt_物料数量.Rows[0]["MRP计划生产量"]) - Convert.ToDecimal(dr["数量"]);
                        }
                        //2
                        if (Convert.ToDecimal(dt_计划池.Rows[0]["未生成数量"]) < Convert.ToDecimal(dr["数量"]))
                        {
                            dt_物料数量.Rows[0]["MRP计划生产量"] = Convert.ToDecimal(dt_物料数量.Rows[0]["MRP计划生产量"]) - Convert.ToDecimal(dt_计划池.Rows[0]["未生成数量"]);
                            dt_物料数量.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt_物料数量.Rows[0]["MRP库存锁定量"]) -
                                (Convert.ToDecimal(dr["数量"]) - Convert.ToDecimal(dt_计划池.Rows[0]["未生成数量"]));
                            dt_计划池.Rows[0]["未生成数量"] = 0;
                            dt_计划池.Rows[0]["计划数量"] = 0;
                        }
                    }
                    //3
                    if (dt_计划池.Rows.Count == 0)
                    {
                        dt_物料数量.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt_物料数量.Rows[0]["MRP库存锁定量"]) - Convert.ToDecimal(dr["数量"]);
                    }

                    if (Convert.ToDecimal(dt_物料数量.Rows[0]["MRP库存锁定量"]) < 0)
                    {
                        dt_物料数量.Rows[0]["MRP库存锁定量"] = 0;
                    }

                    //保存
                    new SqlCommandBuilder(da_计划池);
                    new SqlCommandBuilder(da_物料数量);
                    da_计划池.Update(dt_计划池);
                    da_物料数量.Update(dt_物料数量);
                    #endregion

                    #region 采购
                    string ssql = string.Format("select * from 基础数据物料BOM表 where 产品编码 = '{0}'", dr["物料编码"].ToString());
                    DataTable dt_BOM = new DataTable();
                    SqlDataAdapter da_BOM = new SqlDataAdapter(ssql, strconn);
                    da_BOM.Fill(dt_BOM);

                    foreach (DataRow r in dt_BOM.Rows)
                    {
                        string sql_采购池 = string.Format("select * from 采购记录采购计划表 where 物料编码 = '{0}' and 采购计划类型 = 'MRP类型' and 未完成采购数量 > 0", r["子项编码"].ToString());
                        DataTable dt_采购池 = new DataTable();
                        SqlDataAdapter da_采购池 = new SqlDataAdapter(sql_采购池, strconn);
                        da_采购池.Fill(dt_采购池);

                        string sql_采购物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", r["子项编码"].ToString());
                        DataTable dt_采购物料数量 = new DataTable();
                        SqlDataAdapter da_采购物料数量 = new SqlDataAdapter(sql_采购物料数量, strconn);
                        da_采购物料数量.Fill(dt_采购物料数量);

                        if (dt_采购池.Rows.Count > 0)
                        {
                            //1
                            if (Convert.ToDecimal(dt_采购池.Rows[0]["未完成采购数量"]) >= Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]))
                            {
                                dt_采购池.Rows[0]["未完成采购数量"] = Convert.ToDecimal(dt_采购池.Rows[0]["未完成采购数量"]) - Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]);
                                dt_采购池.Rows[0]["数量"] = Convert.ToDecimal(dt_采购池.Rows[0]["数量"]) - Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]);
                                dt_采购物料数量.Rows[0]["MRP计划采购量"] = Convert.ToDecimal(dt_采购物料数量.Rows[0]["MRP计划采购量"]) - Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]);
                            }
                            //2
                            if (Convert.ToDecimal(dt_采购池.Rows[0]["未完成采购数量"]) < Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]))
                            {
                                dt_采购物料数量.Rows[0]["MRP计划采购量"] = Convert.ToDecimal(dt_采购物料数量.Rows[0]["MRP计划采购量"]) - Convert.ToDecimal(dt_采购池.Rows[0]["未完成采购数量"]);
                                dt_采购物料数量.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt_采购物料数量.Rows[0]["MRP库存锁定量"]) -
                                    (Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(dt_采购池.Rows[0]["未完成采购数量"]));
                                dt_采购池.Rows[0]["未完成采购数量"] = 0;
                                dt_采购池.Rows[0]["数量"] = 0;
                            }
                        }
                        //3
                        if (dt_采购池.Rows.Count == 0)
                        {
                            //半成品
                            string sql_ = string.Format("select 物料类型 from 基础数据物料信息表 where 物料编码 = '{0}'", r["子项编码"].ToString());
                            DataTable dt_ = new DataTable();
                            SqlDataAdapter da_ = new SqlDataAdapter(sql_, strconn);
                            da_.Fill(dt_); try
                            {
                                //if (dt_.Rows[0]["物料类型"].ToString() == "半成品")
                                //{
                                //DataRow rrr = dt_半成品.NewRow();
                                //dt_半成品.Rows.Add(rrr);
                                //rrr["物料编码"] = r_x["子项编码"];
                                //rrr["数量"] = r_x["数量"];
                                //fun_重用(dt_半成品);
                                //}
                                //else
                                {
                                    //原材料
                                    dt_采购物料数量.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt_采购物料数量.Rows[0]["MRP库存锁定量"]) - Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(dr["数量"]);
                                }
                            }
                            catch (Exception ex)
                            {

                                throw new Exception(ex.StackTrace + ex.TargetSite + ex.Source);
                            }
                        }

                        if (Convert.ToDecimal(dt_采购物料数量.Rows[0]["MRP库存锁定量"]) < 0)
                        {
                            dt_采购物料数量.Rows[0]["MRP库存锁定量"] = 0;
                        }

                        //保存
                        new SqlCommandBuilder(da_采购池);
                        new SqlCommandBuilder(da_采购物料数量);
                        da_采购池.Update(dt_采购池);
                        da_采购物料数量.Update(dt_采购物料数量);
                    }
                    #endregion
                }
            }
            //后果 在第二种和无计划池并全转制令的情况下，会多做库存
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                gvP.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();

                fun_明细金额变化();

                fun_获取主表信息();
                fun_事务_保存();


                //刷新受订量
                foreach (DataRow dr in dtP.Rows)
                {

                    StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
                }
                frm销售单证详细界面_视图_Load(null, null);
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_事务_保存()
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");
            try
            {
                {
                    string sql = "select * from 销售记录销售订单明细表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtP);
                    }
                }
                {
                    string sql = "select * from 销售记录销售订单主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                    }
                }

                ts.Commit();
                fun_主表状态();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }

        private void fun_重用(DataTable dt_半成品)
        {
            foreach (DataRow dr in dt_半成品.Rows)
            {
                #region 成品
                //1.查询计划表数量是否大于减少数（删除也认为是减少数）
                //2.计划表数量减少（最多到0），MRP计划生产量减少（最多到0），MRP库存锁定量减少
                string sql_计划池 = string.Format("select * from 生产记录生产计划表 where 物料编码 = '{0}' and 生产计划类型 = 'MRP类型' and 未生成数量 > 0", dr["物料编码"].ToString());
                DataTable dt_计划池 = new DataTable();
                SqlDataAdapter da_计划池 = new SqlDataAdapter(sql_计划池, strconn);
                da_计划池.Fill(dt_计划池);

                string sql_物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", dr["物料编码"].ToString());
                DataTable dt_物料数量 = new DataTable();
                SqlDataAdapter da_物料数量 = new SqlDataAdapter(sql_物料数量, strconn);
                da_物料数量.Fill(dt_物料数量);

                if (dt_计划池.Rows.Count > 0)
                {
                    //1
                    if (Convert.ToDecimal(dt_计划池.Rows[0]["未生成数量"]) >= Convert.ToDecimal(dr["数量"]))
                    {
                        dt_计划池.Rows[0]["未生成数量"] = Convert.ToDecimal(dt_计划池.Rows[0]["未生成数量"]) - Convert.ToDecimal(dr["数量"]);
                        dt_计划池.Rows[0]["计划数量"] = Convert.ToDecimal(dt_计划池.Rows[0]["计划数量"]) - Convert.ToDecimal(dr["数量"]);
                        dt_物料数量.Rows[0]["MRP计划生产量"] = Convert.ToDecimal(dt_物料数量.Rows[0]["MRP计划生产量"]) - Convert.ToDecimal(dr["数量"]);
                    }
                    //2
                    if (Convert.ToDecimal(dt_计划池.Rows[0]["未生成数量"]) < Convert.ToDecimal(dr["数量"]))
                    {
                        dt_物料数量.Rows[0]["MRP计划生产量"] = Convert.ToDecimal(dt_物料数量.Rows[0]["MRP计划生产量"]) - Convert.ToDecimal(dt_计划池.Rows[0]["未生成数量"]);
                        dt_物料数量.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt_物料数量.Rows[0]["MRP库存锁定量"]) -
                            (Convert.ToDecimal(dr["数量"]) - Convert.ToDecimal(dt_计划池.Rows[0]["未生成数量"]));
                        dt_计划池.Rows[0]["未生成数量"] = 0;
                        dt_计划池.Rows[0]["计划数量"] = 0;
                    }
                }
                //3
                if (dt_计划池.Rows.Count == 0)
                {
                    dt_物料数量.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt_物料数量.Rows[0]["MRP库存锁定量"]) - Convert.ToDecimal(dr["数量"]);
                }

                if (Convert.ToDecimal(dt_物料数量.Rows[0]["MRP库存锁定量"]) < 0)
                {
                    dt_物料数量.Rows[0]["MRP库存锁定量"] = 0;
                }

                //保存
                new SqlCommandBuilder(da_计划池);
                new SqlCommandBuilder(da_物料数量);
                da_计划池.Update(dt_计划池);
                da_物料数量.Update(dt_物料数量);
                #endregion

                #region 采购
                string ssql = string.Format("select * from 基础数据物料BOM表 where 产品编码 = '{0}'", dr["物料编码"].ToString());
                DataTable dt_BOM = new DataTable();
                SqlDataAdapter da_BOM = new SqlDataAdapter(ssql, strconn);
                da_BOM.Fill(dt_BOM);

                foreach (DataRow r in dt_BOM.Rows)
                {
                    string sql_采购池 = string.Format("select * from 采购记录采购计划表 where 物料编码 = '{0}' and 采购计划类型 = 'MRP类型' and 未完成采购数量 > 0", r["子项编码"].ToString());
                    DataTable dt_采购池 = new DataTable();
                    SqlDataAdapter da_采购池 = new SqlDataAdapter(sql_采购池, strconn);
                    da_采购池.Fill(dt_采购池);

                    string sql_采购物料数量 = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}'", r["子项编码"].ToString());
                    DataTable dt_采购物料数量 = new DataTable();
                    SqlDataAdapter da_采购物料数量 = new SqlDataAdapter(sql_采购物料数量, strconn);
                    da_采购物料数量.Fill(dt_采购物料数量);

                    if (dt_采购池.Rows.Count > 0)
                    {
                        //1
                        if (Convert.ToDecimal(dt_采购池.Rows[0]["未完成采购数量"]) >= Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]))
                        {
                            dt_采购池.Rows[0]["未完成采购数量"] = Convert.ToDecimal(dt_采购池.Rows[0]["未完成采购数量"]) - Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]);
                            dt_采购池.Rows[0]["数量"] = Convert.ToDecimal(dt_采购池.Rows[0]["数量"]) - Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]);
                            dt_采购物料数量.Rows[0]["MRP计划采购量"] = Convert.ToDecimal(dt_采购物料数量.Rows[0]["MRP计划采购量"]) - Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]);
                        }
                        //2
                        if (Convert.ToDecimal(dt_采购池.Rows[0]["未完成采购数量"]) < Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]))
                        {
                            dt_采购物料数量.Rows[0]["MRP计划采购量"] = Convert.ToDecimal(dt_采购物料数量.Rows[0]["MRP计划采购量"]) - Convert.ToDecimal(dt_采购池.Rows[0]["未完成采购数量"]);
                            dt_采购物料数量.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt_采购物料数量.Rows[0]["MRP库存锁定量"]) -
                                (Convert.ToDecimal(dr["数量"]) * Convert.ToDecimal(r["数量"]) - Convert.ToDecimal(dt_采购池.Rows[0]["未完成采购数量"]));
                            dt_采购池.Rows[0]["未完成采购数量"] = 0;
                            dt_采购池.Rows[0]["数量"] = 0;
                        }
                    }
                    //3
                    if (dt_采购池.Rows.Count == 0)
                    {
                        //半成品
                        string sql_ = string.Format("select 物料类型 from 基础数据物料信息表 where 物料编码 = '{0}'", r["子项编码"].ToString());
                        DataTable dt_ = new DataTable();
                        SqlDataAdapter da_ = new SqlDataAdapter(sql_, strconn);
                        da_.Fill(dt_); try
                        {
                            if (dt_.Rows[0]["物料类型"].ToString() == "半成品")
                            {
                                DataRow rrr = dt_半成品.NewRow();
                                dt_半成品.Rows.Add(rrr);
                                rrr["物料编码"] = r["子项编码"];
                                rrr["数量"] = r["数量"];
                                fun_重用(dt_半成品);
                            }
                            else
                            {
                                //原材料
                                dt_采购物料数量.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(dt_采购物料数量.Rows[0]["MRP库存锁定量"]) - Convert.ToDecimal(r["数量"]) * Convert.ToDecimal(dr["数量"]);
                            }
                        }
                        catch
                        {
                            throw new Exception("请查看该物料是否存在");
                        }
                    }

                    if (Convert.ToDecimal(dt_采购物料数量.Rows[0]["MRP库存锁定量"]) < 0)
                    {
                        dt_采购物料数量.Rows[0]["MRP库存锁定量"] = 0;
                    }

                    //保存
                    new SqlCommandBuilder(da_采购池);
                    new SqlCommandBuilder(da_采购物料数量);
                    da_采购池.Update(dt_采购池);
                    da_采购物料数量.Update(dt_采购物料数量);
                }
                #endregion
            }
        }
        #endregion

        #region 新增删除明细
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = dtP.NewRow();
                if (dtP.Rows.Count > 0)
                {
                    dr["送达日期"] = dtP.Rows[0]["送达日期"];
                }
                dr["GUID"] = System.Guid.NewGuid();
                dr["客户编号"] = txt_客户编号.Text;
                dr["客户"] = txt_客户名称.Text;
                dr["完成数量"] = 0;
                dr["已通知数量"] = 0;
                dr["税率"] = txt_税率.Text.ToString();
                dr["修改日期"] = CPublic.Var.getDatetime();
                dtP.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单视图界面新增明细");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r = gvP.GetDataRow(gvP.FocusedRowHandle);
                r.Delete();
                fun_明细金额变化();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单视图界面删除明细");
            }
        }
        #endregion

        #region MRP保存计划
        private void fun_载入_MRP()
        {
            string sql = "select * from 采购记录采购计划表";
            dtM_采购 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM_采购);

            string sql2 = "select * from 生产记录生产计划表";
            dtM_生产 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            da2.Fill(dtM_生产);
        }

        private void fun_记录缺料情况()
        {
            try
            {
                string str_销售订单号 = "";
                foreach (DataRow rr in dt4.Rows)
                {
                    str_销售订单号 = rr["销售订单号"].ToString() + "|";
                }
                str_销售订单号 = str_销售订单号.Substring(0, str_销售订单号.Length - 1);

                DataTable dt_缺料 = new DataTable();
                string sql = "select * from 销售订单分析缺料记录表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_缺料);
                foreach (DataRow r in dt1_采购.Rows)
                {
                    if (Convert.ToDecimal(r["物料数量"]) > 0)
                    {
                        DataRow dr = dt_缺料.NewRow();
                        dt_缺料.Rows.Add(dr);
                        dr["GUID"] = System.Guid.NewGuid() + "修改";
                        dr["销售订单号"] = str_销售订单号;
                        dr["物料编码"] = r["物料编码"];
                        dr["物料名称"] = r["物料名称"];
                        dr["数量"] = r["物料数量"];
                        dr["日期"] = System.DateTime.Now;
                    }
                }

                new SqlCommandBuilder(da);
                da.Update(dt_缺料);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm采购_生产计划弹窗界面_fun_记录缺料情况");
            }
        }

        private void fun_保存_采购计划()
        {
            try
            {
                foreach (DataRow r in dt1_采购.Rows)
                {
                    DataRow[] ds = dtM_采购.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString().Trim()));
                    if (ds.Length > 0)
                    {
                        ds[0]["数量"] = Convert.ToDecimal(ds[0]["数量"]) + Convert.ToDecimal(r["物料数量"]);
                        if (Convert.ToDecimal(ds[0]["未完成采购数量"]) < 0)
                        {
                            ds[0]["未完成采购数量"] = 0;
                        }

                        ds[0]["未完成采购数量"] = Convert.ToDecimal(ds[0]["未完成采购数量"]) + Convert.ToDecimal(r["物料数量"]);
                        ds[0]["总需数量"] = Convert.ToDecimal(ds[0]["总需数量"]) + Convert.ToDecimal(r["总需数量"]);
                    }
                    else
                    {
                        DataRow dr = dtM_采购.NewRow();
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["采购计划类型"] = "MRP类型";
                        dr["物料编码"] = r["物料编码"].ToString().Trim();
                        dr["物料名称"] = r["物料名称"].ToString();
                        dr["规格型号"] = r["规格型号"].ToString();
                        dr["图纸编号"] = r["图纸编号"].ToString();
                        dr["数量"] = Convert.ToDecimal(r["物料数量"]);
                        dr["已生成采购数量"] = (Decimal)0;
                        dr["未完成采购数量"] = Convert.ToDecimal(r["物料数量"]);
                        dr["日期"] = System.DateTime.Now;
                        dr["采购计划明细号"] = "MRP_PS_" + r["物料编码"].ToString();
                        dr["操作人员"] = CPublic.Var.localUserName;
                        dr["操作人员ID"] = CPublic.Var.LocalUserID;
                        dr["年"] = DateTime.Now.Year;
                        dr["月"] = DateTime.Now.Month;
                        dr["总需数量"] = Convert.ToDecimal(r["总需数量"]);
                        dr["是否生成"] = "否";
                        dtM_采购.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm采购_生产计划弹窗界面_fun_保存_采购计划");
                throw ex;
            }
        }

        private void fun_保存_生产计划()
        {
            try
            {
                foreach (DataRow r in dt2_生产.Rows)
                {
                    DataRow[] ds = dtM_生产.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString().Trim()));
                    if (ds.Length > 0)
                    {
                        if (r["欠缺数量"] == null)
                        {
                            r["欠缺数量"] = 0;
                        }

                        if (r["欠缺数量"] != null && r["欠缺数量"].ToString() == "")
                        {
                            r["欠缺数量"] = 0;
                        }

                        ds[0]["计划数量"] = Convert.ToDecimal(ds[0]["计划数量"]) + Convert.ToDecimal(r["欠缺数量"]);
                        ds[0]["未生成数量"] = Convert.ToDecimal(ds[0]["未生成数量"]) + Convert.ToDecimal(r["欠缺数量"]);
                        ds[0]["规格型号"] = r["规格型号"].ToString();
                        ds[0]["原规格型号"] = r["原规格型号"].ToString();
                        ds[0]["日期"] = System.DateTime.Now;
                    }
                    else
                    {
                        string sql11 = string.Format("select 产品线 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                        DataTable dt11 = new DataTable();
                        SqlDataAdapter da11 = new SqlDataAdapter(sql11, strconn);
                        da11.Fill(dt11);

                        string sql = string.Format(@"select 基础数据物料类型表.* from 基础数据物料类型表 left join 基础数据物料信息表 
                        on 基础数据物料类型表.物料类型名称 = 基础数据物料信息表.大类 or 基础数据物料类型表.物料类型名称 = 基础数据物料信息表.小类
                        where 基础数据物料信息表.物料编码 = '{0}'", r["物料编码"].ToString().Trim());
                        DataTable dtt = new DataTable();
                        SqlDataAdapter ad = new SqlDataAdapter(sql, strconn);
                        ad.Fill(dtt);

                        DataRow dr = dtM_生产.NewRow();
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["生产计划类型"] = "MRP类型";
                        dr["物料编码"] = r["物料编码"].ToString();
                        dr["物料名称"] = r["物料名称"].ToString();
                        dr["规格型号"] = r["规格型号"].ToString();
                        dr["原规格型号"] = r["原规格型号"].ToString();
                        dr["图纸编号"] = r["图纸编号"].ToString();
                        dr["特殊备注"] = r["特殊备注"].ToString();
                        try
                        {
                            dr["生产线"] = dt11.Rows[0]["产品线"].ToString();
                        }
                        catch { }
                        dr["物料类型"] = r["物料类型"].ToString();
                        if (r["欠缺数量"] == DBNull.Value)
                        {
                            r["欠缺数量"] = 0;
                        }
                        dr["计划数量"] = Convert.ToDecimal(r["欠缺数量"]);
                        dr["已生成数量"] = (Decimal)0;
                        dr["未生成数量"] = Convert.ToDecimal(r["欠缺数量"]);
                        dr["日期"] = System.DateTime.Now;

                        string str = "";
                        foreach (DataRow rr in dtt.Rows)
                        {
                            if (rr["类型级别"].ToString() == "小类")
                            {
                                if (rr["计划员"].ToString() != "")
                                {
                                    str = rr["计划员"].ToString();
                                }
                            }
                            if (rr["类型级别"].ToString() == "大类")
                            {
                                if (rr["计划员"].ToString() != "")
                                {
                                    if (str == "")
                                    {
                                        str = rr["计划员"].ToString();
                                    }
                                }
                            }
                        }
                        dr["生产计划单号"] = "MRP_PP_" + r["物料编码"].ToString() + "_" + str;
                        //dr["操作人员"] = CPublic.Var.localUserName;
                        dr["操作人员ID"] = str;

                        dtM_生产.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm采购_生产计划弹窗界面_fun_保存_生产计划");
                throw ex;
            }
        }

        private void fun_保存_MRP()
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");

            fun_保存_采购计划();
            fun_记录缺料情况();
            fun_保存_生产计划();
            string sql_采购 = "select * from 采购记录采购计划表 where 1<>1";
            SqlCommand cmd_采购 = new SqlCommand(sql_采购, conn, ts);
            SqlDataAdapter da_采购 = new SqlDataAdapter(cmd_采购);
            new SqlCommandBuilder(da_采购);

            string sql_生产 = "select * from 生产记录生产计划表 where 1 <> 1";
            SqlCommand cmd_生产 = new SqlCommand(sql_生产, conn, ts);
            SqlDataAdapter da_生产 = new SqlDataAdapter(cmd_生产);
            new SqlCommandBuilder(da_生产);

            string sql_MRP = "select * from 仓库物料数量表 where 1<>1";
            SqlCommand cmd_MRP = new SqlCommand(sql_MRP, conn, ts);
            SqlDataAdapter da_MRP = new SqlDataAdapter(cmd_MRP);
            new SqlCommandBuilder(da_MRP);
            foreach (DataRow r in dt1_采购.Rows)
            {
                try
                {
                    DataRow[] ds = dt3.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                    ds[0]["MRP计划采购量"] = Convert.ToDecimal(ds[0]["MRP计划采购量"]) + Convert.ToDecimal(r["物料数量"]);
                }
                catch { }
            }
            foreach (DataRow r in dt2_生产.Rows)
            {
                try
                {
                    DataRow[] ds = dt3.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                    ds[0]["MRP计划生产量"] = Convert.ToDecimal(ds[0]["MRP计划生产量"]) + Convert.ToDecimal(r["欠缺数量"]);
                }
                catch { }
            }

            try
            {
                da_采购.Update(dtM_采购);
                da_生产.Update(dtM_生产);
                da_MRP.Update(dt3);
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
        }

        #endregion

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                if (Convert.ToDecimal(dr["已通知数量"]) == 0)
                {
                    if (MessageBox.Show("是否确认关闭本条明细", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (Convert.ToDecimal(dr["完成数量"]) > 0)
                        {
                            throw new Exception("已有货物发出,不可关闭该条明细,可修改数量");
                        }
                        dr["关闭"] = 1;
                    }
                }
                else
                {

                    if (MessageBox.Show("是否确认完成该明细剩余不发货了", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (Convert.ToDecimal(dr["完成数量"]) == 0)
                        {
                            throw new Exception("尚未有货物发出,可尝试直接关闭");
                        }
                        dr["明细完成"] = dr["总完成"] = 1;
                        dr["明细完成日期"] = dr["总完成日期"] = CPublic.Var.getDatetime();

                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 维护箱贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
            string str = txt_客户订单号.Text;
            if (txt_客户名称.Text != "")
            {

                ERPSale.fm销售合同箱贴数据维护 fm = new ERPSale.fm销售合同箱贴数据维护(str, dr);
                fm.ShowDialog();

                if (fm.bl) //有箱贴
                {
                    dr["是否有箱贴"] = true;
                    dr.AcceptChanges();
                }
            }
            else
            {
                MessageBox.Show("先选择客户再进行箱贴维护");
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
                if (MessageBox.Show("是否确认完成该明细剩余不发货了", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (Convert.ToDecimal(dr["完成数量"]) == 0)
                    {
                        throw new Exception("尚未有货物发出,可尝试直接关闭");
                    }
                    dr["明细完成"] = dr["总完成"] = 1;
                    dr["明细完成日期"] = dr["总完成日期"] = CPublic.Var.getDatetime();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void repositoryItemSearchLookUpEdit1View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.Text) / (Decimal)100);
            DataRow dr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            DataRow r_focus = gvP.GetDataRow(gvP.FocusedRowHandle);
            if (r_focus == null) return;

            try
            {
                r_focus["物料名称"] = dr["物料名称"].ToString();
                r_focus["计量单位"] = dr["计量单位"].ToString();
                r_focus["规格型号"] = dr["规格型号"].ToString();
                r_focus["规格型号"] = dr["规格"].ToString();
                r_focus["仓库名称"] = dr["仓库名称"].ToString();
                try
                {
                    //合约金额
                    r_focus["税后单价"] = fun_明细金额(dr).ToString("0.000000");
                    r_focus["税前单价"] = (fun_明细金额(dr) / ((Decimal)1 + dec税率)).ToString("0.000000");
                }
                catch
                {
                    //产品标准单价
                    r_focus["税后单价"] = (Convert.ToDecimal(dr["标准单价"])).ToString("0.000000");
                    r_focus["税前单价"] = (Convert.ToDecimal(dr["标准单价"]) / ((Decimal)1 + dec税率)).ToString("0.000000");
                }
            }
            catch { }
        }

        private void repositoryItemSearchLookUpEdit1View_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            Decimal dec税率 = Convert.ToDecimal(Convert.ToInt32(txt_税率.Text) / (Decimal)100);
            DataRow dr = (sender as DevExpress.XtraGrid.Views.Grid.GridView).GetDataRow(e.RowHandle);
            DataRow r_focus = gvP.GetDataRow(gvP.FocusedRowHandle);

            try
            {
                r_focus["物料名称"] = dr["物料名称"].ToString();
                r_focus["计量单位"] = dr["计量单位"].ToString();
                r_focus["规格型号"] = dr["规格型号"].ToString();
                r_focus["规格型号"] = dr["规格"].ToString();
                r_focus["仓库名称"] = dr["仓库名称"].ToString();
                try
                {
                    //合约金额
                    r_focus["税后单价"] = fun_明细金额(dr).ToString("0.000000");
                    r_focus["税前单价"] = (fun_明细金额(dr) / ((Decimal)1 + dec税率)).ToString("0.000000");
                }
                catch
                {
                    //产品标准单价
                    r_focus["税后单价"] = (Convert.ToDecimal(dr["标准单价"])).ToString("0.000000");
                    r_focus["税前单价"] = (Convert.ToDecimal(dr["标准单价"]) / ((Decimal)1 + dec税率)).ToString("0.000000");
                }
            }
            catch { }

        }

        private void gvP_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gvP.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gvP_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gvP.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                //bool bl_弃审 = false;
                string sql = string.Format("select * from 销售记录销售订单主表 where 销售订单号 = '{0}'", textBox1.Text);
                DataTable dt_撤销 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);



                sql = string.Format("select * from 销售记录销售订单明细表 where 销售订单号 = '{0}'", textBox1.Text);
                DataTable dt_撤销明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                //sql = string.Format("select * from 单据审核申请表 where 关联单号 = '{0}'", textBox1.Text);
                //DataTable dt_审核申请 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                //sql = string.Format("select * from 单据审核日志表 where 审核申请单号 = '{0}'", textBox1.Text);
                //DataTable dt_审核日志 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                //sql = "select 产品编码,子项编码 from 基础数据物料BOM表";
                //DataTable dt_BOM = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                //sql = string.Format("select * from 生产记录生产制令子表 where 销售订单号 = '{0}'", textBox1.Text);
                //DataTable dt_制令子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                sql = string.Format(@"select a.* from 销售记录销售出库通知单明细表  a 
                                    left join 销售记录销售出库通知单主表 b  on a.出库通知单号 = b.出库通知单号
                                    where 销售订单明细号 like '%{0}%'  and a.作废 = 0 and b.作废 = 0", textBox1.Text);
                DataTable dt_出库通知 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt_出库通知.Rows.Count > 0)
                {
                    throw new Exception("该订单存在发货通知单，不能弃审");
                }

                if (dt_撤销.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(dt_撤销.Rows[0]["锁定"]) == true)
                    {
                        throw new Exception("该单据已锁定，请确认");
                    }
                    //string sql_采购 = string.Format(@"select mx.采购单号,mx.物料编码,mx.物料名称,mx.规格型号,zb.录入日期 from 采购记录采购单明细表 mx
                    //                left join 采购记录采购单主表 zb on zb.采购单号 = mx.采购单号 where  zb.生效=1 and 录入日期>'{0}'", dt_撤销.Rows[0]["审核日期"]);
                    //DataTable t = new DataTable();
                    //foreach (DataRow dr in dt_撤销明细.Rows)
                    //{
                    //    t = ERPorg.Corg.billofM(t, dr["物料编码"].ToString(), true, dt_BOM);
                    //}
                    //string s = "and 物料编码 in (";
                    //foreach (DataRow dr in t.Rows)
                    //{
                    //    s += "'" + dr["子项编码"].ToString() + "',";

                    //}
                    //s = s.Substring(0, s.Length - 1) + ")";
                    //sql_采购 = sql_采购 + s;
                    //DataTable dt_采购 = CZMaster.MasterSQL.Get_DataTable(sql_采购, strconn);

                    //if (Convert.ToBoolean(dt_撤销.Rows[0]["完成"]))
                    //{
                    //    throw new Exception("该销售单已发货，不能弃审");
                    //}

                    //if (dt_制令子.Rows.Count > 0)
                    //{
                    //    bl_弃审 = true;
                    //}
                    //if (dt_采购.Rows.Count > 0)
                    //{
                    //    bl_弃审 = true;
                    //}
                    if (MessageBox.Show(string.Format("是否申请弃审？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        DataTable dt_审核 = new DataTable();
                        dt_审核 = ERPorg.Corg.fun_PA("弃审", "销售单弃审申请", textBox1.Text, dt_撤销.Rows[0]["销售部门"].ToString());
                        dt_撤销.Rows[0]["锁定"] = true;

                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                        SqlCommand cmd = new SqlCommand("select * from 销售记录销售订单主表 where 1<>1", conn, ts);
                        SqlCommand cmd1 = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);

                        try
                        {

                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(dt_撤销);
                            da = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da);
                            da.Update(dt_审核);
                            ts.Commit();
                            MessageBox.Show("弃审申请成功");
                        }
                        catch
                        {
                            ts.Rollback();
                        }
                    }

                    //else
                    //{
                    //    if (MessageBox.Show(string.Format("该销售单是否确认弃审？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    //    {

                    //        if (Convert.ToBoolean(dt_撤销.Rows[0]["审核"]))
                    //        {

                    //            dt_撤销.Rows[0]["审核"] = 0;
                    //            dt_撤销.Rows[0]["审核人员"] = "";
                    //            dt_撤销.Rows[0]["审核人员ID"] = "";
                    //            dt_撤销.Rows[0]["待审核"] = 0;
                    //            dt_撤销.Rows[0]["生效"] = 0;
                    //            dt_撤销.Rows[0]["生效人员"] = "";
                    //            dt_撤销.Rows[0]["生效人员ID"] = "";
                    //            //dt_撤销.Rows[0]["生效日期"] = DBNull.Value;
                    //            foreach (DataRow dr_明细 in dt_撤销明细.Rows)
                    //            {
                    //                dr_明细["生效"] = 0;
                    //            }

                    //            if (dt_审核申请.Rows.Count > 0)
                    //            {
                    //                dt_审核申请.Rows[0].Delete();
                    //            }
                    //            //if (dt_审核日志.Rows.Count > 0)
                    //            //{
                    //            //    dt_审核日志.Rows[0].Delete();
                    //            //}

                    //            SqlConnection conn = new SqlConnection(strconn);
                    //            conn.Open();
                    //            SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                    //            SqlCommand cmd = new SqlCommand("select * from 销售记录销售订单主表 where 1<>1", conn, ts);
                    //            SqlCommand cmd1 = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
                    //            SqlCommand cmd2 = new SqlCommand("select * from 销售记录销售订单明细表 where 1<>1", conn, ts);
                    //            try
                    //            {

                    //                SqlDataAdapter da = new SqlDataAdapter(cmd);
                    //                new SqlCommandBuilder(da);
                    //                da.Update(dt_撤销);
                    //                da = new SqlDataAdapter(cmd1);
                    //                new SqlCommandBuilder(da);
                    //                da.Update(dt_审核申请);
                    //                da = new SqlDataAdapter(cmd2);
                    //                new SqlCommandBuilder(da);
                    //                da.Update(dt_撤销明细);
                    //                ts.Commit();
                    //            }
                    //            catch
                    //            {
                    //                ts.Rollback();
                    //            }
                    //            MessageBox.Show("弃审成功");
                    //            barLargeButtonItem6.Enabled = false;
                    //            CPublic.UIcontrol.ClosePage();
                    //            frm销售单证详细界面 fm = new frm销售单证详细界面(dt_撤销.Rows[0]["销售订单号"], dt_撤销.Rows[0], dt_撤销);
                    //            fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    //            CPublic.UIcontrol.AddNewPage(fm, "销售订单");
                    //        }
                    //    }
                    //}




                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 明细完成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                DataRow drM = (this.BindingContext[gcP.DataSource].Current as DataRowView).Row;

                string s_ck = string.Format("select * from 销售记录销售出库通知单明细表 where 销售订单明细号='{0}' and 作废=0", drM["销售订单明细号"]);
                DataTable dt_ck = CZMaster.MasterSQL.Get_DataTable(s_ck, strconn);
                foreach (DataRow dr in dt_ck.Rows)
                {
                    if (bool.Parse(dr["完成"].ToString()) == false)
                    {
                        throw new Exception("当前明细有未完成的出库通知单");
                    }

                }

                if (MessageBox.Show(string.Format("确认完成该条明细？"), "询问!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DateTime t = CPublic.Var.getDatetime();
                    string sql = string.Format(" select * from 销售记录销售订单明细表 where 销售订单号='{0}' ", drM["销售订单号"]);
                    DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                    DataRow[] dr_xxx = dt_mx.Select(string.Format("销售订单明细号='{0}'", drM["销售订单明细号"].ToString()));
                    if (dr_xxx.Length > 0)
                    {
                        dr_xxx[0]["明细完成"] = true;
                        dr_xxx[0]["明细完成日期"] = t;
                        //2019-12-19   增加
                        dr_xxx[0]["备注3"] = "完成人：" + CPublic.Var.localUserName;
                    }
                    int i = 0;
                    foreach (DataRow dr in dt_mx.Rows)
                    {
                        if (bool.Parse(dr["明细完成"].ToString()) == true)
                        {
                            i++;
                        }
                    }
                    string s_z = string.Format("select * from 销售记录销售订单主表 where 销售订单号='{0}'", drM["销售订单号"]);
                    DataTable dt_z = CZMaster.MasterSQL.Get_DataTable(s_z, strconn);

                    if (i == dt_mx.Rows.Count)
                    {
                        foreach (DataRow dr in dt_z.Rows)
                        {
                            dr["完成"] = true;
                            dr["完成日期"] = t;
                        }
                        foreach (DataRow dr_x in dt_mx.Rows)
                        {
                            dr_x["总完成"] = 1;
                            dr_x["总完成日期"] = t;
                        }
                    }
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("生效");
                    try
                    {
                        {
                            sql = "select * from 销售记录销售订单明细表 where 1<>1";
                            SqlCommand cmd = new SqlCommand(sql, conn, ts);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                new SqlCommandBuilder(da);
                                da.Update(dt_mx);
                            }
                        }
                        {
                            sql = "select * from 销售记录销售订单主表 where 1<>1";
                            SqlCommand cmd = new SqlCommand(sql, conn, ts);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                new SqlCommandBuilder(da);
                                da.Update(dt_z);
                            }
                        }

                        ts.Commit();
                        drM["明细完成"] = 1;
                        drM["明细完成日期"] = t;
                        drM.AcceptChanges();

                        MessageBox.Show("完成成功");
                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw ex;
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
