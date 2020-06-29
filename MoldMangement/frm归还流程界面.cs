using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MoldMangement
{
    public partial class frm归还流程界面 : UserControl
    {
        DataTable dt_仓库物料数量表;
        DataRow dr_借还;
        DataTable dt_仓库;
        string sql_ck = "";
        string strSoNo = "";
        string strconn = CPublic.Var.strConn;
        DataTable dt_借还申请表附表;
        DataTable dt_借还申请表;
        CurrencyManager cmM;
        #region 原来的
        // public static DataTable dt_借还申请表附表;
        //public static DataRow dr_当前行;
        //public static string s_申请批号;
        //public static DataTable dt_借还申请表;

        //public static int i_保留记录1 = 0;
        // public static DataTable dt_保留;

        #endregion

        public frm归还流程界面()
        {
            InitializeComponent();
        }

        private void frm归还流程界面_Load(object sender, EventArgs e)
        {
            try
            {
                fun_加载();
                dt_借还申请表附表 = new DataTable();
                string sql = "select * from 借还申请表附表 where 1<>1";
                fun_GetDataTable(dt_借还申请表附表, sql);
                sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
                dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            gc3.DataSource = dt_借还申请表;
            cmM = BindingContext[dt_借还申请表] as CurrencyManager;
        }
        private void fun_加载()
        {
            dt_借还申请表 = new DataTable();
            //            string sql = @"select * from 借还申请表 where 借还申请表.申请批号 in (select 借还申请表附表.申请批号 from 借还申请表附表
            //            left join 人员仓库对应表 on 借还申请表附表.仓库名称 = 人员仓库对应表.仓库名称 
            //            where   人员仓库对应表.工号='" + CPublic.Var.LocalUserID + "' group by 申请批号) and 借还申请表.借还状态 = '已领取物料'";
            //2018-10-23
            string sql = @"select * from 借还申请表 where 借还申请表.申请批号 in (select 借还申请表附表.申请批号 from 借还申请表附表
            left join 人员仓库对应表 on 借还申请表附表.仓库名称 = 人员仓库对应表.仓库名称 
            where   人员仓库对应表.工号='" + CPublic.Var.LocalUserID + "' and  借还状态 = '已借出'  group by 申请批号) and 归还=0  ";
            fun_GetDataTable(dt_借还申请表, sql);
            gc3.DataSource = dt_借还申请表;
        }

        private void fun_GetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
        }

        private void fun_SetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }

        private void gridView3_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 1)
            {
                try
                {
                    //判断右键菜单是否可用
                    if (e != null && e.Button == MouseButtons.Right)
                    {
                        contextMenuStrip2.Show(gc3, new Point(e.X, e.Y));
                    }
                    DataRow dr_当前行 = gv3.GetDataRow(gv3.FocusedRowHandle);
                    string s_申请批号 = dr_当前行["申请批号"].ToString();
                    dt_借还申请表附表 = new DataTable();
                    string sql = string.Format(@"select b.*,a.图纸编号,a.计量单位,仓库物料数量表.库存总数 from 借还申请表附表 b
                 left join 基础数据物料信息表 a on a.物料编码 = b.物料编码
                 left join 仓库物料数量表 on 仓库物料数量表.物料编码 = b.物料编码 and 仓库物料数量表.仓库号 = b.仓库号
                 where 申请批号 ='" + s_申请批号 + "' and 归还完成=0");
                    //                  string sql = @"select 借还申请表附表.*,库存总数,操作数量=申请借用数量,基础数据物料信息表.规格型号,基础数据物料信息表.图纸编号,基础数据物料信息表.仓库号,基础数据物料信息表.计量单位 
                    //                  from 借还申请表附表,仓库物料数量表,基础数据物料信息表
                    //                  where 借还申请表附表.物料编码=仓库物料数量表.物料编码 and 仓库物料数量表.物料编码=基础数据物料信息表.物料编码 and " + sql_ck + " and 申请批号 = '" + s_申请批号 + "'";
                    fun_GetDataTable(dt_借还申请表附表, sql);
                    gc2.DataSource = dt_借还申请表附表;
                    dt_借还申请表附表.Columns.Add("选择", typeof(bool));
                    dt_借还申请表附表.Columns.Add("录入归还数量", typeof(decimal));

                    //dt_仓库物料数量表 = new DataTable();
                    //foreach (DataRow dr in dt_借还申请表附表.Rows)
                    //{
                    //    string s_物料编码 = dr["物料编码"].ToString();
                    //    string s_仓库号 = dr["仓库号"].ToString();
                    //    string sql3 = "select * from 仓库物料数量表 where 物料编码 = '" + s_物料编码 + "' and 仓库号 = '" + s_仓库号 + "'";
                    //    fun_GetDataTable(dt_仓库物料数量表, sql3);
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc3, new Point(e.X, e.Y));
                gv3.CloseEditor();
                this.BindingContext[dt_借还申请表].EndCurrentEdit();

            }
        }
        #region 弃用
        private void 当前物品归还ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DataRow dr_当前行 = gv3.GetDataRow(gv3.FocusedRowHandle);
            //if (dr_当前行["选择"].ToString() == "False")
            //{
            //    s_申请批号 = dr_当前行["申请批号"].ToString();
            //    MoldMangement.fm_当前物品归还代还流程 f1 = new MoldMangement.fm_当前物品归还代还流程();

            //    f1.ShowDialog();
            //}
            //else
            //{
            //    MessageBox.Show("不能对已完成订单进行操作！");
            //}
        }

        private void 其他物品归还ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (dr_当前行["选择"].ToString() == "False")
            //{
            //    s_申请批号 = dr_当前行["申请批号"].ToString();
            //    MoldMangement.fm_代还流程 f1 = new MoldMangement.fm_代还流程();
            //    f1.ShowDialog();
            //    barLargeButtonItem3_ItemClick(null, null);
            //}
            //else
            //{
            //    MessageBox.Show("不能对已完成订单进行操作！");
            //}
        }
        #endregion
        #region   2018-10-23 已注释
        private void 结束工单ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //if (dr_当前行["选择"].ToString() == "False")
            //{
            //if (MessageBox.Show("该批号有相关物品没有归还，是否确认结束！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            //{
            //    cmM.EndCurrentEdit();
            //    gv3.CloseEditor();
            //    try
            //    {
            //        MoldMangement.fm_手动完成备注 f1 = new MoldMangement.fm_手动完成备注();
            //        f1.ShowDialog();
            //        if (MoldMangement.fm_手动完成备注.s_状态 == 1)
            //        {
            //            DataRow dr_当前行2 = gv3.GetDataRow(gv3.FocusedRowHandle);
            //            dr_当前行2["选择"] = true;
            //            dr_当前行2["结束日期"] = CPublic.Var.getDatetime();
            //            dr_当前行2["借还状态"] = "已归还";
            //            dr_当前行2["手动归还原因"] = MoldMangement.fm_手动完成备注.s_手动完成原因;
            //            dr_当前行2["手动完成备注"] = MoldMangement.fm_手动完成备注.bts;
            //            dr_当前行2["备注名称"] = MoldMangement.fm_手动完成备注.ss;
            //            string sql = "select * from 借还申请表 where 1<>1";
            //            fun_SetDataTable(dt_借还申请表, sql);
            //        }
            //        if (MoldMangement.fm_手动完成备注.s_状态 == 0)
            //        {
            //            MessageBox.Show("已取消修改！");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
            //}
            //else
            //{
            //    MessageBox.Show("不能对已完成订单进行操作！");
            //}
            barLargeButtonItem3_ItemClick(null, null);
        }
        #endregion
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv3.CloseEditor();
                this.BindingContext[dt_借还申请表].EndCurrentEdit();
                gv2.CloseEditor();
                this.BindingContext[dt_借还申请表附表].EndCurrentEdit();

                DataSet ds = fun_save();
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction st = conn.BeginTransaction("归还");
                try
                {
                    SqlCommand cmd = new SqlCommand("select  * from 借还申请表 where 1=2", conn, st);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[0]);
                    cmd = new SqlCommand("select  * from 借还申请表附表 where 1=2", conn, st);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[1]);
                    cmd = new SqlCommand("select  * from 借还申请表归还记录 where 1=2", conn, st);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[2]);
                    cmd = new SqlCommand("select  * from 仓库出入库明细表 where 1=2", conn, st);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[3]);
                    cmd = new SqlCommand("select  * from 仓库物料数量表 where 1=2", conn, st);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(ds.Tables[4]);
                    st.Commit();
                    MessageBox.Show("归还操作成功");
                    barLargeButtonItem3_ItemClick(null, null);

                }
                catch (Exception ex)
                {
                    st.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            #region 2018-10-23 原来的 已注释
            // int i = 0;
            //foreach (DataRow dr in dt_借还申请表.Rows)
            //{
            //    if (dr["选取"].ToString() == "True")
            //    {
            //        i = 1;
            //        break;
            //    }
            //}
            //if (i == 1)
            //{
            //    MoldMangement.fm_批量归还界面 f1 = new MoldMangement.fm_批量归还界面();
            //    f1.ShowDialog();
            //    barLargeButtonItem3_ItemClick(null, null);
            //}
            //if (i == 0)
            //{
            //    MessageBox.Show("请勾选要归还的批号");
            //}
            #endregion
        }
        /// <summary>
        ///  借还申请表,借还申请表附表,归还记录,出入库明细，库存表
        /// </summary>
        /// <returns></returns>
        private DataSet fun_save()
        {
            DateTime t = CPublic.Var.getDatetime();
            DataSet ds = new DataSet();
            string s = "select  * from 借还申请表归还记录 where 1=2";
            DataTable dt_returnRecord = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            DataRow r_主 = gv3.GetDataRow(gv3.FocusedRowHandle);
            s = "select  * from 仓库出入库明细表  where 1=2";
            DataTable dt_inventoryFlow = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            //2018-10-23 改为按单归还，可分批,借还申请附表 '已归还数量',归还状态;借还申请表 归还状态;借还申请表归还记录、库存表、仓库出入库明细表
            string str_returnRecord = string.Format("RA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
              t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RA", t.Year, t.Month).ToString("0000"));
            int i = 1;
            foreach (DataRow dr in dt_借还申请表附表.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    dr["归还数量"] = Convert.ToDecimal(dr["归还数量"]) + Convert.ToDecimal(dr["录入归还数量"]);
                    if (Convert.ToDecimal(dr["归还数量"]) == Convert.ToDecimal(dr["已借出数量"]))  // 这边 不大于check中会检查。
                    {
                        dr["归还完成"] = 1;
                        dr["归还日期"] = t;
                    }
                    DataRow r = dt_returnRecord.NewRow();
                    r["guid"] = System.Guid.NewGuid();
                    r["申请批号"] = str_returnRecord;
                    r["申请批号明细"] = str_returnRecord + "-" + i.ToString("00");
                    r["借用申请明细号"] = dr["申请批号明细"];
                    r["物料名称"] = dr["物料名称"];
                    r["物料编码"] = dr["物料编码"];
                    r["归还日期"] = t;
                    r["规格型号"] = dr["规格型号"];
                    r["货架描述"] = dr["货架描述"];
                    r["仓库号"] = dr["仓库号"];
                    r["仓库名称"] = dr["仓库名称"];
                    r["归还数量"] = dr["录入归还数量"];
                    r["归还操作人"] = CPublic.Var.localUserName;
                    dt_returnRecord.Rows.Add(r);

                    DataRow r_inventoryFlow = dt_inventoryFlow.NewRow();
                    r_inventoryFlow["GUID"] = System.Guid.NewGuid();
                    r_inventoryFlow["明细类型"] = "归还入库";
                    r_inventoryFlow["单号"] = str_returnRecord;
                    r_inventoryFlow["物料名称"] = dr["物料名称"];
                    r_inventoryFlow["物料编码"] = dr["物料编码"];

                    string x = CPublic.Var.localUser课室编号;
                    if (x == "") x = CPublic.Var.localUser部门编号;
                    // x = string.Format("select  * from 人事基础部门表 where 部门编号='{0}'", x);
                    r_inventoryFlow["相关单位"] = r_主["相关单位"];
                    //DataRow r_depart = CZMaster.MasterSQL.Get_DataRow(x,strconn);
                    //r_inventoryFlow["相关单位"] = r_depart["部门名称"];
                    r_inventoryFlow["明细号"] = str_returnRecord + "-" + i.ToString("00");
                    r_inventoryFlow["出库入库"] = "入库";
                    r_inventoryFlow["仓库号"] = dr["仓库号"];
                    r_inventoryFlow["仓库名称"] = dr["仓库名称"];
                    r_inventoryFlow["单位"] = dr["计量单位"];
                    r_inventoryFlow["实效数量"] = dr["录入归还数量"];
                    r_inventoryFlow["实效时间"] = r_inventoryFlow["出入库时间"] = t;
                    r_inventoryFlow["相关单号"] = dr["申请批号明细"];
                    dt_inventoryFlow.Rows.Add(r_inventoryFlow);

                    i++;
                }

            }
            DataView dv = new DataView(dt_借还申请表附表);
            dv.RowFilter = "选择=1";
            DataTable tt = dv.ToTable();

            tt.Columns["录入归还数量"].ColumnName = "数量";
            DataTable dt_库存 = ERPorg.Corg.fun_库存(1, tt);
            //还需要一个借还申请表的 状态
            bool bl = true;

            foreach (DataRow rr in dt_借还申请表附表.Rows)
            {
                if (rr["归还完成"].Equals(false)) { bl = false; break; }
            }
            if (bl)  //如果上面判断成功，则bl=true 并且dt_借还申请表附表.rows.count 就是此次完成条目 
            {
                s = string.Format("select count(*) from 借还申请表附表 where 归还完成=0 and 申请批号='{0}' ", r_主["申请批号"]);
                DataRow r = CZMaster.MasterSQL.Get_DataRow(s, strconn);
                if (Convert.ToInt32(r[0]) < dt_借还申请表附表.Rows.Count)
                {
                    bl = false;
                }
            }
            r_主["归还"] = bl;
            r_主["归还日期"] = t;
            ds.Tables.Add(dt_借还申请表);
            ds.Tables.Add(dt_借还申请表附表);
            ds.Tables.Add(dt_returnRecord);
            ds.Tables.Add(dt_inventoryFlow);
            ds.Tables.Add(dt_库存);
            return ds;
        }
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gv3.CloseEditor();
            this.BindingContext[gc3].EndCurrentEdit();
            if (MessageBox.Show("该批号有相关物品没有归还，是否确认结束！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                cmM.EndCurrentEdit();
                gv2.CloseEditor();
                try
                {
                    MoldMangement.fm_手动完成备注 f1 = new MoldMangement.fm_手动完成备注();
                    f1.ShowDialog();
                    if (MoldMangement.fm_手动完成备注.s_状态 == 1)
                    {
                        foreach (DataRow dr in dt_借还申请表.Rows)
                        {
                            if (dr["选取"].ToString() == "True")
                            {
                                dr["选择"] = true;
                                dr["结束日期"] = CPublic.Var.getDatetime();
                                dr["借还状态"] = "已归还";
                                dr["手动归还原因"] = MoldMangement.fm_手动完成备注.s_手动完成原因;
                                dr["手动完成备注"] = MoldMangement.fm_手动完成备注.bts;
                                dr["备注名称"] = MoldMangement.fm_手动完成备注.ss;
                            }
                        }
                        string sql = "select * from 借还申请表 where 1<>1";
                        fun_SetDataTable(dt_借还申请表, sql);
                    }
                    if (MoldMangement.fm_手动完成备注.s_状态 == 0)
                    {
                        MessageBox.Show("已取消修改！");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                MessageBox.Show("不能对已完成订单进行操作！");
            }
            barLargeButtonItem3_ItemClick(null, null);
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
                dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
                dt_借还申请表.Clear();
                dt_借还申请表附表.Clear();
                fun_加载();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        #region 已弃用
        private void timer1_Tick(object sender, EventArgs e)
        {
            //timer1.Stop();
            //if (MoldMangement.fm_批量归还界面.i_保留记录 == 1)
            //{
            //    dt_保留 = MoldMangement.fm_批量归还界面.dt_借还申请表附表.Copy();
            //    i_保留记录1 = 1;
            //    MoldMangement.fm_批量归还界面.i_保留记录 = 0;
            //    //timer1.Stop();
            //}
            //timer1.Start();

        }
        #endregion
        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Caption == "选取")
            {
                gv3.CloseEditor();
                this.BindingContext[gc3].EndCurrentEdit();
                decimal dec_单价 = 0;
                decimal dec_总金额 = 0;
                foreach (DataRow dr in dt_借还申请表.Rows)
                {
                    if (dr["选取"].ToString() == "True")
                    {
                        dec_单价 = Convert.ToDecimal(dr["总金额"]);
                        dec_总金额 += dec_单价;
                    }
                }
                barMdiChildrenListItem1.Caption = dec_总金额.ToString();
            }
        }

        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void 外销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dr_借还 = gv3.GetDataRow(gv3.FocusedRowHandle);
                //返回ds.tables[0]归还记录明细，ds.tables[1]仓库出入库明细,ds.tables[2]归还关联
                //保存ds_借还,dt_借用申请表，dt_借用申请表附表
                DataSet ds_借还 = fun_归还("借用转销售", dr_借还);

                DataSet ds_外销 = new DataSet();
                MoldMangement.fm_归还转外销 fm = new fm_归还转外销(dr_借还, ds_借还.Tables[1], ds_外销,ds_借还.Tables[0]);
                //fm.Dock = System.Windows.Forms.DockStyle.Fill;
                fm.ShowDialog();
                if (fm.flag)
                {
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction thrk = conn.BeginTransaction("归还转销售");
                    try
                    {
                        string sql1 = "select * from 借还申请表 where 1<>1";
                        SqlCommand cmd1 = new SqlCommand(sql1, conn, thrk);
                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_借还申请表);

                        sql1 = "select * from 借还申请表附表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_借还申请表附表);

                        sql1 = "select * from 借还申请表归还记录 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(ds_借还.Tables[0]);

                        //sql1 = "select * from 借还申请批量归还关联 where 1<>1";
                        //cmd1 = new SqlCommand(sql1, conn, thrk);
                        //da1 = new SqlDataAdapter(cmd1);
                        //new SqlCommandBuilder(da1);
                        //da1.Update(ds_借还.Tables[2]);

                        sql1 = "select * from 仓库出入库明细表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(ds_借还.Tables[1]);

                        sql1 = "select * from 销售记录销售订单主表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[0]);

                        sql1 = "select * from 销售记录销售出库通知单主表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[1]);

                        sql1 = "select * from 销售记录成品出库单主表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[2]);

                        sql1 = "select * from 销售记录销售订单明细表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[3]);
                        sql1 = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[4]);

                        sql1 = "select * from 销售记录成品出库单明细表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[5]);



                        thrk.Commit();
                        MessageBox.Show("归还转外销成功");
                    }
                    catch (Exception ex)
                    {
                        thrk.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        private void 耗用ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dr_借还 = gv3.GetDataRow(gv3.FocusedRowHandle);
                DataSet ds_借还 = fun_归还("借用转耗用", dr_借还);//返回ds.tables[0]归还记录明细，ds.tables[1]仓库出入库明细,ds.tables[2]归还关联
                //保存ds_借还,dt_借用申请表，dt_借用申请表附表
                //dr_借还["相关单位"]
                DataSet ds_qt = fun_qt(ds_借还.Tables[0], ds_借还.Tables[1], dr_借还["相关单位"].ToString());// 传入归还记录表,返回ds.tables[0]dt_其他出入库申请主,ds.tables[1]其他出库主,ds.tables[2]其他出入库申请子,ds.tables[3]dt_其他出库子,ds.tables[4]dt_仓库出入库明细
                //保存ds_qt
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction thrk = conn.BeginTransaction("归还转耗用");

                try
                {
                    string sql1 = "select * from 借还申请表 where 1<>1";
                    SqlCommand cmd1 = new SqlCommand(sql1, conn, thrk);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_借还申请表);

                    sql1 = "select * from 借还申请表附表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_借还申请表附表);

                    sql1 = "select * from 借还申请表归还记录 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_借还.Tables[0]);

                    //sql1 = "select * from 借还申请批量归还关联 where 1<>1";
                    //cmd1 = new SqlCommand(sql1, conn, thrk);
                    //da1 = new SqlDataAdapter(cmd1);
                    //new SqlCommandBuilder(da1);
                    //da1.Update(ds_借还.Tables[2]);

                    sql1 = "select * from 其他出入库申请主表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_qt.Tables[0]);

                    sql1 = "select * from 其他出库主表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_qt.Tables[1]);

                    sql1 = "select * from 其他出入库申请子表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_qt.Tables[2]);

                    sql1 = "select * from 其他出库子表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_qt.Tables[3]);

                    sql1 = "select * from 仓库出入库明细表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_借还.Tables[1]);

                    thrk.Commit();
                    MessageBox.Show("归还转耗用成功");
                    barLargeButtonItem3_ItemClick(null, null);

                }
                catch (Exception ex)
                {
                    thrk.Rollback();
                    throw ex;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //这边有问题10-24 需要修改
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt_归还记录"></param>
        /// <param name="dt_仓库出入库明细"></param>
        /// <param name="s_相关单位"></param>
        /// <returns></returns>
        private DataSet fun_qt(DataTable dt_归还记录, DataTable dt_仓库出入库明细, string s_相关单位)
        {
            DataSet ds = new DataSet();
            DateTime t = CPublic.Var.getDatetime();
            try
            {
                DataTable dt_其他出入库申请主;
                DataTable dt_其他出入库申请子;
                DataTable dt_其他出库主;
                DataTable dt_其他出库子;
                string sql_s = "select * from 其他出入库申请主表 where 1<>1";
                dt_其他出入库申请主 = CZMaster.MasterSQL.Get_DataTable(sql_s, strconn);
                sql_s = "select * from 其他出入库申请子表 where 1<>1";
                dt_其他出入库申请子 = CZMaster.MasterSQL.Get_DataTable(sql_s, strconn);
                sql_s = "select * from 其他出库主表 where 1<>1";
                dt_其他出库主 = CZMaster.MasterSQL.Get_DataTable(sql_s, strconn);
                sql_s = "select * from 其他出库子表 where 1<>1";
                dt_其他出库子 = CZMaster.MasterSQL.Get_DataTable(sql_s, strconn);

                string s_其他出入库申请单号 = string.Format("QWSQ{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
              t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QWSQ", t.Year, t.Month).ToString("0000"));
                string s_其他出库单号 = string.Format("QT{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
              t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("QT", t.Year, t.Month).ToString("0000"));

                DataRow dr_其他出入库申请主 = dt_其他出入库申请主.NewRow();
                dt_其他出入库申请主.Rows.Add(dr_其他出入库申请主);
                dr_其他出入库申请主["GUID"] = System.Guid.NewGuid();
                dr_其他出入库申请主["出入库申请单号"] = s_其他出入库申请单号;
                dr_其他出入库申请主["申请日期"] = t;
                dr_其他出入库申请主["申请类型"] = "其他出库";
                dr_其他出入库申请主["备注"] = "借出转耗用";
                dr_其他出入库申请主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_其他出入库申请主["操作人员"] = CPublic.Var.localUserName;
                dr_其他出入库申请主["生效"] = true;
                dr_其他出入库申请主["生效日期"] = t;
                dr_其他出入库申请主["生效人员编号"] = CPublic.Var.LocalUserID;
                dr_其他出入库申请主["完成"] = true;
                dr_其他出入库申请主["完成日期"] = t;
                dr_其他出入库申请主["原因分类"] = "耗用";
                dr_其他出入库申请主["备注1"] = dt_归还记录.Rows[0]["借用申请明细号"].ToString().Split('-')[0];

                ds.Tables.Add(dt_其他出入库申请主);

                DataRow dr_其他出库主 = dt_其他出库主.NewRow();
                dt_其他出库主.Rows.Add(dr_其他出库主);
                dr_其他出库主["GUID"] = System.Guid.NewGuid();
                dr_其他出库主["其他出库单号"] = s_其他出库单号;
                dr_其他出库主["出库类型"] = "其他出库";
                dr_其他出库主["操作人员编号"] = CPublic.Var.LocalUserID;
                dr_其他出库主["操作人员"] = CPublic.Var.localUserName;
                dr_其他出库主["出库日期"] = t;
                dr_其他出库主["生效"] = true;
                dr_其他出库主["生效日期"] = t;
                dr_其他出库主["创建日期"] = t;
                dr_其他出库主["出入库申请单号"] = s_其他出入库申请单号;
                ds.Tables.Add(dt_其他出库主);

                int i = 1;
                foreach (DataRow rr in dt_归还记录.Rows)
                {
                    DataRow dr_其他出入库申请子 = dt_其他出入库申请子.NewRow();
                    dt_其他出入库申请子.Rows.Add(dr_其他出入库申请子);
                    dr_其他出入库申请子["GUID"] = System.Guid.NewGuid();
                    dr_其他出入库申请子["出入库申请单号"] = s_其他出入库申请单号;
                    dr_其他出入库申请子["POS"] = i;
                    dr_其他出入库申请子["出入库申请明细号"] = s_其他出入库申请单号 + "-" + i.ToString("00");
                    dr_其他出入库申请子["物料编码"] = rr["物料编码"];
                    //dr_其他出入库申请子["原ERP物料编号"] = dr_借用明细["原ERP物料编号"];
                    dr_其他出入库申请子["物料名称"] = rr["物料名称"];
                    dr_其他出入库申请子["数量"] = rr["归还数量"];
                    //dr_其他出入库申请子["n原ERP规格型号"] = dr_借用明细["n原ERP规格型号"];
                    dr_其他出入库申请子["备注"] = rr["借用申请明细号"];
                    dr_其他出入库申请子["生效"] = true;
                    dr_其他出入库申请子["生效日期"] = t;
                    dr_其他出入库申请子["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_其他出入库申请子["完成"] = true;
                    dr_其他出入库申请子["完成日期"] = t;
                    dr_其他出入库申请子["仓库号"] = rr["仓库号"];
                    dr_其他出入库申请子["仓库名称"] = rr["仓库名称"];
                    dr_其他出入库申请子["货架描述"] = rr["货架描述"];

                    DataRow dr_其他出库子 = dt_其他出库子.NewRow();
                    dt_其他出库子.Rows.Add(dr_其他出库子);
                    dr_其他出库子["物料编码"] = rr["物料编码"];
                    //dr_其他出库子["原ERP物料编号"] = dr_借用明细["原ERP物料编号"];
                    dr_其他出库子["物料名称"] = rr["物料名称"];
                    dr_其他出库子["数量"] = rr["归还数量"];
                    //dr_其他出库子["n原ERP规格型号"] = dr_借用明细["n原ERP规格型号"];
                    dr_其他出库子["规格型号"] = rr["规格型号"];
                    // dr_其他出库子["图纸编号"] = rr["图纸编号"];
                    dr_其他出库子["其他出库单号"] = s_其他出库单号;
                    dr_其他出库子["POS"] = i;
                    dr_其他出库子["其他出库明细号"] = s_其他出库单号 + "-" + i++.ToString("00");
                    dr_其他出库子["GUID"] = System.Guid.NewGuid();
                    dr_其他出库子["备注"] = rr["借用申请明细号"];
                    dr_其他出库子["生效"] = true;
                    dr_其他出库子["生效日期"] = t;
                    dr_其他出库子["生效人员编号"] = CPublic.Var.LocalUserID;
                    dr_其他出库子["完成"] = true;
                    dr_其他出库子["完成日期"] = t;
                    dr_其他出库子["完成人员编号"] = CPublic.Var.LocalUserID;
                    dr_其他出库子["出入库申请单号"] = s_其他出入库申请单号;
                    dr_其他出库子["出入库申请明细号"] = dr_其他出入库申请子["出入库申请明细号"];

                    DataRow dr_仓库出入库明细 = dt_仓库出入库明细.NewRow();
                    dt_仓库出入库明细.Rows.Add(dr_仓库出入库明细);
                    dr_仓库出入库明细["GUID"] = System.Guid.NewGuid();
                    dr_仓库出入库明细["明细类型"] = "其他出库";
                    dr_仓库出入库明细["单号"] = s_其他出库单号;
                    dr_仓库出入库明细["物料编码"] = rr["物料编码"];
                    dr_仓库出入库明细["物料名称"] = rr["物料名称"];
                    dr_仓库出入库明细["明细号"] = dr_其他出库子["其他出库明细号"];
                    dr_仓库出入库明细["出库入库"] = "出库";
                    dr_仓库出入库明细["实效数量"] = "-" + rr["归还数量"];
                    dr_仓库出入库明细["实效时间"] = t;
                    dr_仓库出入库明细["出入库时间"] = t;
                    dr_仓库出入库明细["相关单号"] = s_其他出入库申请单号;
                    dr_仓库出入库明细["仓库号"] = rr["仓库号"];
                    dr_仓库出入库明细["仓库名称"] = rr["仓库名称"];
                    dr_仓库出入库明细["相关单位"] = s_相关单位;
                }
                ds.Tables.Add(dt_其他出入库申请子);
                ds.Tables.Add(dt_其他出库子);
                //ds.Tables.Add(dt_仓库出入库明细);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return ds;
        }

        private DataSet fun_归还(string ss, DataRow dr_借还)
        {
            DataSet ds = new DataSet();


            DateTime t = CPublic.Var.getDatetime();
            DataTable dt_归还表;
            //   DataTable dt_归还关联表;
            DataTable dt_仓库出入库明细;
            string sql_归还 = "select * from 借还申请表归还记录 where 1<>1";
            dt_归还表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);
            //sql_归还 = "select * from 借还申请批量归还关联 where 1<>1";
            //dt_归还关联表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);

            sql_归还 = "select * from 仓库出入库明细表 where 1<>1";
            dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);

            string s_归还单号 = string.Format("RA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"), t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RA", t.Year, t.Month).ToString("0000"));
            //string s_归还单号111 = string.Format("RA{0}",CPublic.CNo.fun_得到最大流水号("RA").ToString("0000"));
            int i = 1;
            foreach (DataRow dr in dt_借还申请表附表.Rows)
            {
                //dt_借还申请表附表 只显示未归还记录

                DataRow dr_归还 = dt_归还表.NewRow();
                dt_归还表.Rows.Add(dr_归还);
                dr_归还["guid"] = System.Guid.NewGuid();
                dr_归还["申请批号"] = s_归还单号;
                dr_归还["申请批号明细"] = s_归还单号 + "-" + i++.ToString("00");
                dr_归还["借用申请明细号"] = dr["申请批号明细"];
                dr_归还["计量单位"] = dr["计量单位"];
                dr_归还["计量单位编码"] = dr["计量单位编码"];

                dr_归还["物料编码"] = dr["物料编码"];
                dr_归还["物料名称"] = dr["物料名称"];
                dr_归还["规格型号"] = dr["规格型号"];
                dr_归还["仓库号"] = dr["仓库号"];
                dr_归还["仓库名称"] = dr["仓库名称"];
                dr_归还["备注"] = ss+"自动生成记录";
                decimal dec = Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["归还数量"]);
                dr_归还["归还数量"] = dec;
                dr_归还["归还日期"] = t;
                dr_归还["货架描述"] = dr["货架描述"];
                dr_归还["归还操作人"] = CPublic.Var.localUserName;

                dr["归还日期"] = t;
                dr["归还完成"] = 1;
                dr["借还状态"] = "已归还";
               
                dr["归还数量"] = dr["申请数量"];


                DataRow dr_仓库出入库明细 = dt_仓库出入库明细.NewRow();
                dt_仓库出入库明细.Rows.Add(dr_仓库出入库明细);
                dr_仓库出入库明细["GUID"] = System.Guid.NewGuid();
                dr_仓库出入库明细["明细类型"] = "归还入库";
                dr_仓库出入库明细["单号"] = s_归还单号;
                dr_仓库出入库明细["物料编码"] = dr["物料编码"];
                dr_仓库出入库明细["物料名称"] = dr["物料名称"];
                dr_仓库出入库明细["明细号"] = dr_归还["申请批号明细"];
                dr_仓库出入库明细["出库入库"] = "入库";
                dr_仓库出入库明细["实效数量"] = dec;
                dr_仓库出入库明细["实效时间"] = t;
                dr_仓库出入库明细["出入库时间"] = t;
                dr_仓库出入库明细["相关单号"] = dr_归还["借用申请明细号"];
                dr_仓库出入库明细["相关单位"] = dr_借还["相关单位"];
                dr_仓库出入库明细["仓库号"] = dr["仓库号"];
                dr_仓库出入库明细["仓库名称"] = dr["仓库名称"];
                dr_仓库出入库明细["单位"] = dr["计量单位"];
                //DataRow dr_归还关联 = dt_归还关联表.NewRow();
                //dt_归还关联表.Rows.Add(dr_归还关联);
                //dr_归还关联["关联批号"] = dr_借还["申请批号"];
                //dr_归还关联["归还批号"] = s_归还单号;
                //ds.Tables.Add(dt_归还关联表);
            }
            dr_借还["归还日期"] = t;
            dr_借还["归还"] = true;
            //dr_借还["借还状态"] = "已归还";
            dr_借还["手动归还原因"] = ss;

            ds.Tables.Add(dt_归还表);
            ds.Tables.Add(dt_仓库出入库明细);

            return ds;
        }

        private void 赠送ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dr_借还 = gv3.GetDataRow(gv3.FocusedRowHandle);
                //返回ds.tables[0]归还记录明细，ds.tables[1]归还关联,ds.tables[2]
                DataSet ds_借还 = fun_归还("借用转赠送", dr_借还);
                //保存ds_借还,dt_借用申请表，dt_借用申请表附表
                //返回ds.tables[0]dt_销售订单主表dt_，ds.tables[1]出库通知单主表,ds.tables[2]dt_成品出库单主表,ds.tables[3]dt_销售订单明细表，
                //ds.tables[4]dt_出库通知单明细表,ds.tables[5]dt_成品出库单明细表,ds.tables[6]dt_仓库出入库明细
                DataSet ds_zs = fun_赠送(ds_借还.Tables[0], ds_借还.Tables[1]);
                //保存ds_zs
                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction thrk = conn.BeginTransaction("归还转赠送");
                try
                {
                    string sql1 = "select * from 借还申请表 where 1<>1";
                    SqlCommand cmd1 = new SqlCommand(sql1, conn, thrk);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_借还申请表);

                    sql1 = "select * from 借还申请表附表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_借还申请表附表);

                    sql1 = "select * from 借还申请表归还记录 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_借还.Tables[0]);

                    //sql1 = "select * from 借还申请批量归还关联 where 1<>1";
                    //cmd1 = new SqlCommand(sql1, conn, thrk);
                    //da1 = new SqlDataAdapter(cmd1);
                    //new SqlCommandBuilder(da1);
                    //da1.Update(ds_借还.Tables[2]);

                    sql1 = "select * from 销售记录销售订单主表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[0]);

                    sql1 = "select * from 销售记录销售出库通知单主表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[1]);

                    sql1 = "select * from 销售记录成品出库单主表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[2]);

                    sql1 = "select * from 销售记录销售订单明细表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[3]);
                    sql1 = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[4]);

                    sql1 = "select * from 销售记录成品出库单明细表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_zs.Tables[5]);

                    sql1 = "select * from 仓库出入库明细表 where 1<>1";
                    cmd1 = new SqlCommand(sql1, conn, thrk);
                    da1 = new SqlDataAdapter(cmd1);
                    new SqlCommandBuilder(da1);
                    da1.Update(ds_借还.Tables[1]);

                    thrk.Commit();
                    MessageBox.Show("归还转赠送成功");

                    barLargeButtonItem3_ItemClick(null, null);
                }
                catch (Exception ex)
                {
                    thrk.Rollback();
                    throw ex;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private DataSet fun_赠送(DataTable dt_归还记录, DataTable dt_仓库出入库明细)
        {
            DataSet ds = new DataSet();
            DateTime t = CPublic.Var.getDatetime();

            DataTable dt_销售订单主表;
            DataTable dt_销售订单明细表;
            DataTable dt_出库通知单主表;
            DataTable dt_出库通知单明细表;
            DataTable dt_成品出库单主表;
            DataTable dt_成品出库单明细表;
            DataTable dt_客户;
            string s = "select * from 销售记录销售订单主表 where 1<>1";
            dt_销售订单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录销售订单明细表 where 1<>1";
            dt_销售订单明细表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录销售出库通知单主表 where 1<>1";
            dt_出库通知单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录销售出库通知单明细表 where 1<>1";
            dt_出库通知单明细表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = string.Format("select * from 客户基础信息表 where 客户名称 = '{0}'", dr_借还["相关单位"]);
            dt_客户 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录成品出库单主表 where 1<>1";
            dt_成品出库单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录成品出库单明细表 where 1<>1";
            dt_成品出库单明细表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            //s = "select * from 仓库出入库明细表 where 1<>1";
            //dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);


            string s_销售单号 = string.Format("SO{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
          t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", t.Year, t.Month).ToString("0000"));
            string s_出库通知单号 = string.Format("SK{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
          t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SK", t.Year, t.Month).ToString("0000"));
            string s_成品出库单号 = string.Format("SA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
          t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SA", t.Year, t.Month).ToString("0000"));

            DataRow dr_销售订单主 = dt_销售订单主表.NewRow();
            dt_销售订单主表.Rows.Add(dr_销售订单主);
            dr_销售订单主["GUID"] = System.Guid.NewGuid();
            dr_销售订单主["销售订单号"] = s_销售单号;
            dr_销售订单主["录入人员"] = CPublic.Var.localUserName;
            dr_销售订单主["录入人员ID"] = CPublic.Var.LocalUserID;
            dr_销售订单主["待审核"] = true;
            dr_销售订单主["审核"] = true;
            dr_销售订单主["备注1"] = dt_仓库出入库明细.Rows[0]["相关单号"].ToString().Split('-')[0]; //记录借用申请单号


            if (dt_客户.Rows.Count > 0)
            {
                dr_销售订单主["客户编号"] = dt_客户.Rows[0]["客户编号"];
                dr_销售订单主["客户名"] = dr_借还["相关单位"];
                dr_销售订单主["税率"] = dt_客户.Rows[0]["税率"];
                dr_销售订单主["业务员"] = dt_客户.Rows[0]["业务员"];
                //dr_销售订单主["客户名"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_销售订单主["日期"] = t;
            dr_销售订单主["销售备注"] = "借出转赠送";

            dr_销售订单主["税前金额"] = 0;
            dr_销售订单主["税后金额"] = 0;
            dr_销售订单主["生效"] = true;
            dr_销售订单主["生效日期"] = t;
            dr_销售订单主["生效人员"] = CPublic.Var.localUserName;
            dr_销售订单主["生效人员ID"] = CPublic.Var.LocalUserID;

            dr_销售订单主["创建日期"] = t;
            dr_销售订单主["修改日期"] = t;
            dr_销售订单主["完成"] = true;
            dr_销售订单主["完成日期"] = t;
            ds.Tables.Add(dt_销售订单主表);

            DataRow dr_出库通知单主 = dt_出库通知单主表.NewRow();
            dt_出库通知单主表.Rows.Add(dr_出库通知单主);
            dr_出库通知单主["GUID"] = System.Guid.NewGuid();
            dr_出库通知单主["出库通知单号"] = s_出库通知单号;
            if (dt_客户.Rows.Count > 0)
            {
                dr_出库通知单主["客户编号"] = dt_客户.Rows[0]["客户编号"];
                dr_出库通知单主["客户名"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_出库通知单主["出库日期"] = t;
            dr_出库通知单主["创建日期"] = t;
            dr_出库通知单主["修改日期"] = t;
            dr_出库通知单主["操作员ID"] = CPublic.Var.LocalUserID;
            dr_出库通知单主["操作员"] = CPublic.Var.localUserName;
            dr_出库通知单主["生效"] = true;
            dr_出库通知单主["生效日期"] = t;
            ds.Tables.Add(dt_出库通知单主表);

            DataRow dr_成品出库主 = dt_成品出库单主表.NewRow();
            dt_成品出库单主表.Rows.Add(dr_成品出库主);
            dr_成品出库主["GUID"] = System.Guid.NewGuid();
            dr_成品出库主["成品出库单号"] = s_成品出库单号;
            dr_成品出库主["操作员ID"] = CPublic.Var.LocalUserID;
            dr_成品出库主["操作员"] = CPublic.Var.localUserName;
            if (dt_客户.Rows.Count > 0)
            {
                dr_成品出库主["客户"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_成品出库主["日期"] = t;
            dr_成品出库主["创建日期"] = t;
            dr_成品出库主["修改日期"] = t;
            dr_成品出库主["生效"] = true;
            dr_成品出库主["生效日期"] = t;
            ds.Tables.Add(dt_成品出库单主表);

            int i = 1;
            foreach (DataRow dr in dt_归还记录.Rows)
            {
                DataRow dr_saleDetail = dt_销售订单明细表.NewRow();
                dt_销售订单明细表.Rows.Add(dr_saleDetail);
                dr_saleDetail["GUID"] = System.Guid.NewGuid();
                dr_saleDetail["销售订单号"] = s_销售单号;
                dr_saleDetail["POS"] = i;
                dr_saleDetail["销售订单明细号"] = s_销售单号 + "-" + i.ToString("00");
                dr_saleDetail["物料编码"] = dr["物料编码"];
                dr_saleDetail["数量"] = dr["归还数量"];
                dr_saleDetail["完成数量"] = dr["归还数量"];
                dr_saleDetail["未完成数量"] = 0;
                dr_saleDetail["已通知数量"] = dr["归还数量"];
                dr_saleDetail["未通知数量"] = 0;
                dr_saleDetail["物料名称"] = dr["物料名称"];
                //dr_销售订单子["n原ERP规格型号"] = dr["n原ERP规格型号"];
                dr_saleDetail["规格型号"] = dr["规格型号"];
                // dr_销售订单子["图纸编号"] = dr["图纸编号"];
                dr_saleDetail["仓库号"] = dr["仓库号"];
                dr_saleDetail["仓库名称"] = dr["仓库名称"];
                dr_saleDetail["计量单位"] = dr["计量单位"];
               // dr_saleDetail["销售备注"] = "借出转赠送";
                dr_saleDetail["税前金额"] = 0;
                dr_saleDetail["税后金额"] = 0;
                dr_saleDetail["税前单价"] = 0;
                dr_saleDetail["税后单价"] = 0;
                dr_saleDetail["送达日期"] = t;
                if (dt_客户.Rows.Count > 0)
                {
                    dr_saleDetail["客户编号"] = dt_客户.Rows[0]["客户编号"];
                    dr_saleDetail["客户"] = dt_客户.Rows[0]["客户名称"];
                }
                dr_saleDetail["生效"] = true;
                dr_saleDetail["生效日期"] = t;
                dr_saleDetail["明细完成"] = true;
                dr_saleDetail["明细完成日期"] = t;
                dr_saleDetail["总完成"] = true;
                dr_saleDetail["总完成日期"] = t;
                dr_saleDetail["已计算"] = true;
                dr_saleDetail["录入人员ID"] = CPublic.Var.LocalUserID;
                dr_saleDetail["含税销售价"] = 0;

                DataRow dr_stockOutNotice = dt_出库通知单明细表.NewRow();
                dt_出库通知单明细表.Rows.Add(dr_stockOutNotice);
                dr_stockOutNotice["GUID"] = System.Guid.NewGuid();
                dr_stockOutNotice["出库通知单号"] = s_出库通知单号;
                dr_stockOutNotice["POS"] = i;
                dr_stockOutNotice["出库通知单明细号"] = s_出库通知单号 + "-" + i.ToString("00");
                dr_stockOutNotice["销售订单明细号"] = dr_saleDetail["销售订单明细号"];
                dr_stockOutNotice["物料编码"] = dr["物料编码"];
                dr_stockOutNotice["物料名称"] = dr["物料名称"];
                dr_stockOutNotice["出库数量"] = dr["归还数量"];
                dr_stockOutNotice["规格型号"] = dr["规格型号"];
                //dr_stockOutNotice["图纸编号"] = dr["图纸编号"];
                dr_stockOutNotice["操作员ID"] = CPublic.Var.LocalUserID;
                dr_stockOutNotice["操作员"] = CPublic.Var.localUserName;
                dr_stockOutNotice["生效"] = true;
                dr_stockOutNotice["生效日期"] = t;
                dr_stockOutNotice["完成"] = true;
                dr_stockOutNotice["完成日期"] = t;
                dr_stockOutNotice["计量单位"] = dr["计量单位"];
                dr_stockOutNotice["销售备注"] = "借出转赠送";
       

                if (dt_客户.Rows.Count > 0)
                {
                    dr_stockOutNotice["客户"] = dt_客户.Rows[0]["客户名称"];
                    dr_stockOutNotice["客户编号"] = dt_客户.Rows[0]["客户编号"];
                }
                dr_stockOutNotice["已出库数量"] = dr["归还数量"];
                dr_stockOutNotice["未出库数量"] = 0;
                //dr_出库通知单明细["n原ERP规格型号"] = dr["n原ERP规格型号"];

                DataRow dr_stockOutDetaail = dt_成品出库单明细表.NewRow();
                dt_成品出库单明细表.Rows.Add(dr_stockOutDetaail);
                dr_stockOutDetaail["GUID"] = System.Guid.NewGuid();
                dr_stockOutDetaail["成品出库单号"] = s_成品出库单号;
                dr_stockOutDetaail["POS"] = i;
                dr_stockOutDetaail["成品出库单明细号"] = s_成品出库单号 + "-" + i++.ToString("00");
                dr_stockOutDetaail["销售订单号"] = s_销售单号;
                dr_stockOutDetaail["销售订单明细号"] = dr_saleDetail["销售订单明细号"];
                dr_stockOutDetaail["出库通知单号"] = s_出库通知单号;
                dr_stockOutDetaail["出库通知单明细号"] = dr_stockOutNotice["出库通知单明细号"];
                dr_stockOutDetaail["物料编码"] = dr["物料编码"];
                dr_stockOutDetaail["物料名称"] = dr["物料名称"];
                dr_stockOutDetaail["出库数量"] = dr["归还数量"];
                dr_stockOutDetaail["已出库数量"] = dr["归还数量"];
                dr_stockOutDetaail["未开票数量"] = dr["归还数量"];
                dr_stockOutDetaail["规格型号"] = dr["规格型号"];
                dr_stockOutNotice["计量单位"] = dr["计量单位"];
                dr_stockOutNotice["销售备注"] = "借出转赠送";
                //dr_stockOutDetaail["图纸编号"] = dr["图纸编号"];
                if (dt_客户.Rows.Count > 0)
                {
                    dr_stockOutDetaail["客户"] = dt_客户.Rows[0]["客户名称"];
                    dr_stockOutDetaail["客户编号"] = dt_客户.Rows[0]["客户编号"];
                }
                dr_stockOutDetaail["仓库号"] = dr["仓库号"];
                dr_stockOutDetaail["仓库名称"] = dr["仓库名称"];
                dr_stockOutDetaail["生效"] = true;
                dr_stockOutDetaail["生效日期"] = t;
                //dr_成品出库明细["n原ERP规格型号"] = dr["n原ERP规格型号"];

                DataRow dr_stockcrmx = dt_仓库出入库明细.NewRow();
                dt_仓库出入库明细.Rows.Add(dr_stockcrmx);
                dr_stockcrmx["GUID"] = System.Guid.NewGuid();
                dr_stockcrmx["明细类型"] = "销售出库";
                dr_stockcrmx["单号"] = s_成品出库单号;
                dr_stockcrmx["物料编码"] = dr["物料编码"];
                dr_stockcrmx["物料名称"] = dr["物料名称"];
                dr_stockcrmx["明细号"] = dr_stockOutDetaail["成品出库单明细号"];
                dr_stockcrmx["出库入库"] = "出库";
                dr_stockcrmx["实效数量"] = "-" + dr["归还数量"];
                dr_stockcrmx["实效时间"] = t;
                dr_stockcrmx["出入库时间"] = t;
                dr_stockcrmx["相关单号"] = dr_saleDetail["销售订单明细号"];
                dr_stockcrmx["仓库号"] = dr["仓库号"];
                dr_stockcrmx["仓库名称"] = dr["仓库名称"];
                dr_stockcrmx["相关单位"] = dr_借还["相关单位"];
                dr_stockcrmx["单位"] = dr["计量单位"];


            }
            ds.Tables.Add(dt_销售订单明细表);
            ds.Tables.Add(dt_出库通知单明细表);
            ds.Tables.Add(dt_成品出库单明细表);
            //ds.Tables.Add(dt_仓库出入库明细);               

            return ds;
        }


        //private void fun_保存主表明细(bool bl)
        //{
        //    DateTime t = CPublic.Var.getDatetime();
        //    string sql_kh = string.Format("select * from 客户基础信息表 where 客户编号 = '" + dr_借还["借用人员"] + "'");
        //    DataTable dt_客户 = CZMaster.MasterSQL.Get_DataTable(sql_kh, strconn);

        //    try
        //    {
        //        string sql = "select * from 销售记录销售订单主表 where 1<>1";
        //        DataTable dt_销售主单 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
        //        DataRow dr_销售主单 = dt_销售主单.NewRow();
        //        dt_销售主单.Rows.Add(dr_销售主单);
        //        strSoNo = string.Format("SO{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
        //       t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", t.Year, t.Month, t.Day).ToString("0000"));
        //        if (dr_销售主单["GUID"].ToString() == "")
        //        {
        //            dr_销售主单["GUID"] = System.Guid.NewGuid();
        //            dr_销售主单["销售订单号"] = strSoNo.ToString();
        //            dr_销售主单["录入人员"] = CPublic.Var.localUserName;
        //            dr_销售主单["录入人员ID"] = CPublic.Var.LocalUserID;
        //            dr_销售主单["日期"] = CPublic.Var.getDatetime();
        //            dr_销售主单["创建日期"] = CPublic.Var.getDatetime();
        //        }
        //        if (bl == true)
        //        {
        //            dr_销售主单["生效"] = true;
        //            dr_销售主单["生效日期"] = CPublic.Var.getDatetime();
        //            dr_销售主单["生效人员"] = CPublic.Var.localUserName;
        //            dr_销售主单["生效人员ID"] = CPublic.Var.LocalUserID;
        //        }
        //        dr_销售主单["修改日期"] = CPublic.Var.getDatetime();
        //        dr_销售主单["作废"] = false;
        //        dr_销售主单["完成"] = true;
        //        dr_销售主单["完成日期"] = CPublic.Var.getDatetime();
        //        dr_销售主单["计划是否确认"] = true;
        //        if (dt_客户.Rows.Count > 0)
        //        {
        //            dr_销售主单["客户编号"] = dt_客户.Rows[0]["客户编号"].ToString();
        //            dr_销售主单["客户名"] = dt_客户.Rows[0]["客户名称"].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    try
        //    {
        //        string sql = "select * from 销售记录销售订单明细表 where 1<>1";
        //        DataTable dt_销售附表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

        //        int i = 1;
        //        DataRow[] ds = dt_借还申请表附表.Select(string.Format("申请批号 = '{0}'", dr_借还["申请批号"].ToString()));
        //        DataTable dt_基础;
        //        foreach (DataRow dr in ds)
        //        {
        //            string sql_基础 = string.Format("select * from 基础数据物料信息表 where 物料编码 = '" + dr["物料编码"] + "'");
        //            dt_基础 = CZMaster.MasterSQL.Get_DataTable(sql_基础, strconn);
        //            DataRow dr_销售附表 = dt_销售附表.NewRow();
        //            dt_销售附表.Rows.Add(dr_销售附表);
        //            dr_销售附表["GUID"] = System.Guid.NewGuid();
        //            dr_销售附表["POS"] = i.ToString();
        //            dr_销售附表["销售订单明细号"] = strSoNo.ToString() + "-" + i.ToString("00");
        //            dr_销售附表["物料编码"] = dr["物料编码"].ToString();
        //            dr_销售附表["物料名称"] = dr["物料名称"].ToString();
        //            dr_销售附表["数量"] = dr["实际借用数量"].ToString();
        //            dr_销售附表["n原ERP规格型号"] = dr["n原ERP规格型号"].ToString();
        //            if (dt_客户.Rows.Count > 0)
        //            {
        //                dr_销售附表["客户编号"] = dt_客户.Rows[0]["客户编号"].ToString();
        //                dr_销售附表["客户"] = dt_客户.Rows[0]["客户名称"].ToString();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}





    }
}
