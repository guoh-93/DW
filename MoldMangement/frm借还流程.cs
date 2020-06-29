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
    public partial class frm借还流程 : UserControl
    {
        #region 用户变量
        DataTable dt_借还申请表;
        // DataTable dt_仓库物料数量表;
        DataTable dt_仓库;
        DataTable dt_仓库号;
        DataTable dt;
        string sql_ck = "";
        string strconn = CPublic.Var.strConn;
        public static DataTable dt_借还申请表附表;
        CurrencyManager cmM;
        DataRow dr_当前行;
        public static string s_申请批号;
        #endregion

        #region 类自用
        public frm借还流程()
        {
            InitializeComponent();
        }

        private void frm借还流程_Load(object sender, EventArgs e)
        {
            try
            {
                fun_加载();
                fun_仓库();
                //   barEditItem1.EditValue = "未领取物料";
                sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
                dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
                string sql2 = "select * from 仓库出入库明细表 where 1<>1";
                dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql2, strconn);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            gc.DataSource = dt_借还申请表;
            cmM = BindingContext[dt_借还申请表] as CurrencyManager;
        }

        private void fun_仓库()
        {
            dt_仓库号 = new DataTable();
            string sql4 = "select 属性字段1 as 仓库号,属性值 as 仓库名称 from 基础数据基础属性表 where 属性类别 =  '仓库类别'";
            SqlDataAdapter da = new SqlDataAdapter(sql4, strconn);
            da.Fill(dt_仓库号);
            repositoryItemSearchLookUpEdit2.DataSource = dt_仓库号;
            repositoryItemSearchLookUpEdit2.DisplayMember = "仓库号";
            repositoryItemSearchLookUpEdit2.ValueMember = "仓库号";
        }

        #endregion

        #region 数据操作

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
        //显示子表结构
        private void fun_加载()
        {
            dt_借还申请表 = new DataTable();
            string sql = @"select * from 借还申请表 where 作废=0 and 审核=1 and 锁定 = 0 and   借还申请表.申请批号 in (select 借还申请表附表.申请批号 from 借还申请表附表
            left join 人员仓库对应表 on 借还申请表附表.仓库号 = 人员仓库对应表.仓库号 
            where  作废=0 and 已借出数量<申请数量 and  人员仓库对应表.工号='" + CPublic.Var.LocalUserID + "' group by 申请批号)";
            //*and 借还申请表.借还状态 = '未领取物料'*/
            fun_GetDataTable(dt_借还申请表, sql);
            gc.DataSource = dt_借还申请表;

            dt_借还申请表附表 = new DataTable();
            sql = "select * from 借还申请表附表 where 1<>1";
            fun_GetDataTable(dt_借还申请表附表, sql);
            //dt_借还申请表附表.Columns.Add("选择", typeof(bool));
            gc_mx.DataSource = dt_借还申请表附表;
        }

        //        private void fun_刷新()
        //        {
        //            string sql = @"select * from 借还申请表 where 借还申请表.申请批号 in (  select 借还申请表附表.申请批号 from 借还申请表附表
        //            left join 人员仓库对应表 on 借还申请表附表.仓库号 = 人员仓库对应表.仓库号 
        //            where   人员仓库对应表.工号='" + CPublic.Var.LocalUserID + "' group by 申请批号)";
        //            dt_借还申请表.Clear();
        //            fun_GetDataTable(dt_借还申请表, sql);

        //        }

        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //新增
        {
            cmM.EndCurrentEdit();
            gridView1.CloseEditor();
            try
            {

                DataRow dr = dt_借还申请表.NewRow();
                dr["申请日期"] = CPublic.Var.getDatetime().ToShortDateString().ToString();
                dt_借还申请表.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //删除
        {
            cmM = BindingContext[dt_借还申请表] as CurrencyManager;
            cmM.EndCurrentEdit();
            gridView1.CloseEditor();
            try
            {
                DataTable dt = new DataTable();
                string s_申请批号 = dr_当前行["申请批号"].ToString();
                string sql = "select * from 借还申请表附表 where 申请批号='" + s_申请批号 + "'";
                fun_GetDataTable(dt, sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //if (dt.Rows[i][2].ToString().Trim() == "")
                    //{
                    dt.Rows[i].Delete();
                    //}
                }
                //dt.AcceptChanges(); 
                (cmM.Current as DataRowView).Row.Delete();
                string sql2 = "select * from 借还申请表附表 where 1<>1";
                fun_SetDataTable(dt, sql2);
                string sql3 = "select * from 借还申请表 where 1<>1";
                fun_SetDataTable(dt_借还申请表, sql3);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //保存
        {
            cmM.EndCurrentEdit();
            gridView1.CloseEditor();
            try
            {
                string sql = "select * from 借还申请表 where 1<>1";
                fun_SetDataTable(dt_借还申请表, sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //刷新
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //刷新
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

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //关闭
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e) //鼠标单击
        {

            try
            {

                dr_当前行 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s_申请批号 = dr_当前行["申请批号"].ToString();
                dt_借还申请表附表 = new DataTable();
                //                    string sql = @"select 借还申请表附表.*,库存总数,操作数量=申请借用数量 from 借还申请表附表,仓库物料数量表,基础数据物料信息表
                //                    where 借还申请表附表.物料编码=仓库物料数量表.物料编码 and 仓库物料数量表.物料编码=基础数据物料信息表.物料编码 and " + sql_ck + " and 申请批号 = '" + s_申请批号 + "' and 借还申请表附表.借还状态 = '未领取物料'";
                string sql = string.Format(@"select b.*,a.规格型号,a.图纸编号,a.计量单位,(申请数量-已借出数量) as 输入数量,isnull(仓库物料数量表.库存总数,0)库存总数 from 借还申请表附表 b
                                             left join 基础数据物料信息表 a on a.物料编码 = b.物料编码
                                             left join 仓库物料数量表 on 仓库物料数量表.物料编码 = b.物料编码 and 仓库物料数量表.仓库号 = b.仓库号
                                              where b.申请批号 = '" + s_申请批号 + "' and b.申请数量 > b.已借出数量 and b.作废=0");

                fun_GetDataTable(dt_借还申请表附表, sql);
                dt_借还申请表附表.Columns.Add("选择", typeof(bool));

                gc_mx.DataSource = dt_借还申请表附表;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void simpleButton1_Click(object sender, EventArgs e) //确认
        {
            try
            {
                string sql = "select * from 借还申请表附表 where 1<>1";
                fun_SetDataTable(dt_借还申请表附表, sql);
                dr_当前行["借还状态"] = "已领取物料";
                string sql2 = "select * from 借还申请表 where 1<>1";
                fun_SetDataTable(dt_借还申请表, sql2);
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //查看全部
        {
            try
            {
                //fun_刷新();
                dt_借还申请表附表.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e) //输入借用数量获取金额
        {
            if (e.Column.Caption == "操作数量")
            {
                DataRow myDataRow = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                decimal s_单价金额 = Convert.ToDecimal(myDataRow["物料单价"]);
                decimal s_借用数量 = Convert.ToDecimal(myDataRow["申请借用数量"]);
                decimal s_总金额 = s_单价金额 * s_借用数量;
                myDataRow["实际借用金额"] = s_总金额;

            }
        }

        private void 归还物料ToolStripMenuItem_Click(object sender, EventArgs e) //鼠标右击，归还物料
        {
            if (dr_当前行["选择"].ToString() == "False")
            {
                DataRow dr_当前行2 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                s_申请批号 = dr_当前行2["申请批号"].ToString();
                MoldMangement.fm_代还流程 f1 = new MoldMangement.fm_代还流程();
                f1.ShowDialog();
                barLargeButtonItem2_ItemClick(null, null);
            }
            else
            {
                MessageBox.Show("不能对已完成订单进行操作！");
            }
        }

        private void 完成ToolStripMenuItem_Click(object sender, EventArgs e) //鼠标右击，完成
        {
            if (dr_当前行["选择"].ToString() == "False")
            {
                if (MessageBox.Show("该批号有相关物品没有归还，是否确认结束！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    cmM.EndCurrentEdit();
                    gridView1.CloseEditor();
                    try
                    {
                        MoldMangement.fm_手动完成备注 f1 = new MoldMangement.fm_手动完成备注();
                        f1.ShowDialog();
                        if (MoldMangement.fm_手动完成备注.s_状态 == 1)
                        {
                            DataRow dr_当前行2 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                            dr_当前行2["选择"] = true;
                            dr_当前行2["结束日期"] = CPublic.Var.getDatetime();
                            dr_当前行2["借还状态"] = "已归还";
                            dr_当前行2["手动归还原因"] = MoldMangement.fm_手动完成备注.s_手动完成原因;
                            string sql = "select * from 借还申请表 where 1<>1";
                            fun_SetDataTable(dt_借还申请表, sql);
                        }
                        if (MoldMangement.fm_手动完成备注.s_状态 == 0)
                        {
                            MessageBox.Show("已取消修改！");
                        }
                        barLargeButtonItem2_ItemClick(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("不能对已完成订单进行操作！");
            }
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //发货
        {
            if (dt_借还申请表附表.Rows.Count > 0)
            {
                foreach (DataRow dr in dt_借还申请表附表.Rows)
                {
                    decimal s_单价金额 = Convert.ToDecimal(dr["物料单价"]);
                    decimal s_借用数量 = Convert.ToDecimal(dr["申请借用数量"]);
                    decimal s_总金额 = s_单价金额 * s_借用数量;
                    dr["实际借用金额"] = s_总金额;
                }
                foreach (DataRow dr in dt_借还申请表附表.Rows)
                {
                    if (Convert.ToDecimal(dr["申请借用数量"]) < Convert.ToDecimal(dr["实际借用数量"]))
                    {
                        MessageBox.Show("领取物料数量太多！");
                        return;
                    }
                    if (Convert.ToDecimal(dr["库存总数"]) < Convert.ToDecimal(dr["实际借用数量"]))
                    {
                        MessageBox.Show("库存不够！");
                        return;
                    }
                }
                if (dr_当前行["借还状态"].ToString() == "已领取物料" || dr_当前行["借还状态"].ToString() == "已归还")
                {
                    MessageBox.Show("不能重复发货！");
                    return;
                }
                try
                {
                    //foreach (DataRow dr in dt_仓库物料数量表.Rows)//扣库存
                    //{
                    //    foreach (DataRow dr2 in dt_借还申请表附表.Rows)
                    //    {
                    //        if (dr["物料编码"].ToString() == dr2["物料编码"].ToString() && dr2["选择"].ToString() == "False")
                    //        {
                    //            decimal s_库存总数 = Convert.ToDecimal(dr["库存总数"]);
                    //            decimal s_有效总数 = Convert.ToDecimal(dr["有效总数"]);
                    //            decimal s_申请数量 = Convert.ToDecimal(dr2["申请借用数量"]);
                    //            decimal s_差额 = s_库存总数 - s_申请数量;
                    //            decimal s_差额2 = s_有效总数 - s_申请数量;
                    //            dr["库存总数"] = s_差额.ToString();
                    //            dr["有效总数"] = s_差额2.ToString();
                    //            dr2["选择"] = true;
                    //        }
                    //    }
                    //}
                    foreach (DataRow dr in dt_借还申请表附表.Rows)
                    {
                        if (dr["选择"].ToString() == "True")
                        {
                            dr["借还状态"] = "已领取物料";
                        }
                        else
                        {
                            dr["借还状态"] = "未领取物料";
                        }
                        dr["实际借用数量"] = dr["申请借用数量"];
                    }

                    sql_ck = "申请批号明细 not in(";
                    foreach (DataRow dr in dt_借还申请表附表.Rows)
                    {
                        sql_ck = sql_ck + string.Format("'{0}',", dr["申请批号明细"]);

                    }
                    sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";

                    DataTable dt_借还申请表附表1 = new DataTable();
                    string sql4 = "select * from 借还申请表附表 where 申请批号 = '" + dr_当前行["申请批号"].ToString() + "' and " + sql_ck + "and 借还状态 = '未领取物料'";
                    fun_GetDataTable(dt_借还申请表附表1, sql4);
                    if (dt_借还申请表附表1.Rows.Count > 0)
                    {
                        dr_当前行["借还状态"] = "未领取物料";
                    }
                    else
                    {
                        dr_当前行["借还状态"] = "已领取物料";
                    }
                    // fun_保存记录到出入库明细();
                    //-fun_SaveData(dt_仓库物料数量表, dt_借还申请表附表, dt_借还申请表,dt);                
                    MessageBox.Show("操作成功!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请选择申请单！");
            }

            barLargeButtonItem2_ItemClick(null, null);
        }

        private static void fun_SaveData(System.Data.DataTable dt1, System.Data.DataTable dt2, System.Data.DataTable dt3, System.Data.DataTable dt4)
        {
            using (SqlConnection conn = new SqlConnection(CPublic.Var.strConn))
            {
                SqlTransaction transaction = null;
                try
                {
                    ///文件断层使用的数据表
                    System.Data.DataTable dt_MID1 = new System.Data.DataTable();
                    System.Data.DataTable dt_MID2 = new System.Data.DataTable();
                    System.Data.DataTable dt_MID3 = new System.Data.DataTable();
                    System.Data.DataTable dt_MID4 = new System.Data.DataTable();
                    string sql;

                    conn.Open();
                    transaction = conn.BeginTransaction("CFileVibrationTransaction");

                    sql = "select * from 仓库物料数量表 where 1<>1";
                    SqlDataAdapter da_MID1 = new SqlDataAdapter(new SqlCommand(sql, conn, transaction));
                    new SqlCommandBuilder(da_MID1);
                    da_MID1.Fill(dt_MID1);

                    sql = "select * from 借还申请表附表 where 1<>1";
                    SqlDataAdapter da_MID2 = new SqlDataAdapter(new SqlCommand(sql, conn, transaction));
                    new SqlCommandBuilder(da_MID2);
                    da_MID2.Fill(dt_MID2);
                    sql = "select * from 借还申请表 where 1<>1";
                    SqlDataAdapter da_MID3 = new SqlDataAdapter(new SqlCommand(sql, conn, transaction));
                    new SqlCommandBuilder(da_MID3);
                    da_MID2.Fill(dt_MID3);
                    sql = "select * from 仓库出入库明细表 where 1<>1";
                    SqlDataAdapter da_MID4 = new SqlDataAdapter(new SqlCommand(sql, conn, transaction));
                    new SqlCommandBuilder(da_MID4);
                    da_MID2.Fill(dt_MID4);

                    dt_MID1 = dt1;
                    dt_MID2 = dt2;
                    dt_MID3 = dt3;
                    dt_MID4 = dt4;
                    try
                    {
                        da_MID1.Update(dt_MID1);
                        da_MID2.Update(dt_MID2);
                        da_MID3.Update(dt_MID3);
                        da_MID4.Update(dt_MID4);
                        transaction.Commit();

                        //dt_MID1.Clear();
                        //dt_MID2.Clear();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void 当前物品归还ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dr_当前行["选择"].ToString() == "False")
            {
                DataRow dr_当前行2 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                s_申请批号 = dr_当前行2["申请批号"].ToString();
                MoldMangement.fm_当前物品归还代还流程 f1 = new MoldMangement.fm_当前物品归还代还流程();
                f1.ShowDialog();
                barLargeButtonItem2_ItemClick(null, null);
            }
            else
            {
                MessageBox.Show("不能对已完成订单进行操作！");
            }
        }
        private void fun_check()
        {
            //DataView dv = new DataView(dt_借还申请表附表);
            //dv.RowFilter = "选择=1";
            //if (dv.Count == 0) throw new Exception("未选择任何明细");
            DataRow dr1 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql = string.Format("select * from 借还申请表 where 申请批号 = '{0}'",dr1["申请批号"]);
            DataTable dt111 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            if (dt111.Rows.Count > 0)
            {
                if(Convert.ToBoolean(dt111.Rows[0]["锁定"]) == true)
                {
                    throw new Exception("该单据已做弃审申请");
                }
            }
            foreach (DataRow dr in dt_借还申请表附表.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    if (Convert.ToDecimal(dr["输入数量"]) > Convert.ToDecimal(dr["库存总数"])) throw new Exception("物料:" + dr["物料编码"].ToString() + "库存不足");
                    if (Convert.ToDecimal(dr["输入数量"]) > Convert.ToDecimal(dr["申请数量"])) throw new Exception("物料:" + dr["物料编码"].ToString() + "数量超过申请数量");
                    if (dr["仓库号"].ToString().Trim() == "") throw new Exception("物料:" + dr["物料编码"].ToString() + "没有仓库");
                }
            }
        }
        private void fun_save()
        {
          
            string s = "select  * from 仓库出入库明细表  where 1=2";
            DateTime t = CPublic.Var.getDatetime();
            DataTable dt_仓库出入明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);

            foreach (DataRow dr in dt_借还申请表附表.Rows)
            {
                if (dr["选择"].Equals(true))
                {
                    //2018-10-19 申请多少出库多少,之前做的就没有,借用记录表,要查的话估计只能在 仓库出入库明细表和借用申请表 
                    dr["已借出数量"] = Convert.ToDecimal(dr["已借出数量"].ToString()) + Convert.ToDecimal(dr["输入数量"].ToString());
                    if (Convert.ToDecimal(dr["已借出数量"].ToString()) == Convert.ToDecimal(dr["申请数量"].ToString()))
                    {
                        dr["借还状态"] = "已借出";
                        dr["领取完成"] = true;
                    }

                    DataRow dr_cmx = dt_仓库出入明细.NewRow();
                    dr_cmx["GUID"] = System.Guid.NewGuid();
                    dr_cmx["明细类型"] = "借用出库";
                    dr_cmx["单号"] = dr["申请批号"].ToString();
                    dr_cmx["物料编码"] = dr["物料编码"].ToString();
                    dr_cmx["物料名称"] = dr["物料名称"].ToString();
                    dr_cmx["明细号"] = dr["申请批号明细"].ToString();
                    dr_cmx["相关单号"] = dr["申请批号"].ToString();
                    dr_cmx["仓库号"] = dr["仓库号"].ToString();
                    dr_cmx["仓库名称"] = dr["仓库名称"].ToString();
                    dr_cmx["出库入库"] = "出库";
                    dr_cmx["相关单位"] = "";
                    dr_cmx["数量"] = (Decimal)0;
                    dr_cmx["标准数量"] = (Decimal)0;
                    dr_cmx["实效数量"] = Convert.ToDecimal("-" + dr["输入数量"].ToString());
                    dr_cmx["实效时间"] = t;
                    dr_cmx["出入库时间"] = t;
                    dr_cmx["仓库人"] = CPublic.Var.localUserName;

                    ///dt.Rows.Add(dr);
                    dt_仓库出入明细.Rows.Add(dr_cmx);
                }
            }

            //dt_借还申请表
            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            //19-5-21 因为不分仓库，所以可以直接判断，如果分仓库需要再加判断
            DataRow[] ds1 = dt_借还申请表附表.Select(string.Format("领取完成 = 0"));
            if(ds1.Length == 0)
            {
                r["领取完成"] = 1;
            }
            DataView dv = new DataView(dt_借还申请表附表);
            dv.RowFilter = "选择=1";
            DataTable tt = dv.ToTable();

          //  DataTable tt = dt_借还申请表附表.Copy();
            tt.Columns["输入数量"].ColumnName = "数量";
            DataTable dt_库存 = ERPorg.Corg.fun_库存(-1, tt);
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction jyc = conn.BeginTransaction("借用出库");
            try
            {

                string sql = "select * from 借还申请表附表 where 1<>1";
                SqlCommand cmd = new SqlCommand(sql, conn, jyc);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_借还申请表附表);

                sql = "select * from 借还申请表 where 1<>1";
                cmd = new SqlCommand(sql, conn, jyc);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_借还申请表);


                sql = "select * from 仓库出入库明细表  where 1<>1";
                cmd = new SqlCommand(sql, conn, jyc);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_仓库出入明细);

                sql = "select  * from 仓库物料数量表 where 1<>1";
                cmd = new SqlCommand(sql, conn, jyc);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_库存);

                jyc.Commit();
            }
            catch (Exception ex)
            {
                jyc.Rollback();
                throw ex;
            }




        }
        //生效
        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                gridView2.CloseEditor();
                this.BindingContext[dt_借还申请表附表].EndCurrentEdit();
                this.ActiveControl = null;
                fun_check();
                //2018-10-19已经改为 借用出库
                fun_save();
                MessageBox.Show("操作成功");
                barLargeButtonItem2_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }





        }

        private void fun_保存记录到出入库明细()
        {
            try
            {
                DataRow rrr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string sql_1 = string.Format("select * from 人事基础员工表 where 员工号='{0}'", rrr["工号"]);
                DataTable dt_xg = CZMaster.MasterSQL.Get_DataTable(sql_1, strconn);


                DateTime t = CPublic.Var.getDatetime();
                foreach (DataRow r in dt_借还申请表附表.Rows)
                {
                    if (r["选择"].Equals(true))
                    {
                        DataRow dr = dt.NewRow();
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["明细类型"] = "借用出库";
                        dr["单号"] = r["申请批号"].ToString();
                        dr["物料编码"] = r["物料编码"].ToString();
                        dr["物料名称"] = r["物料名称"].ToString();
                        dr["明细号"] = r["申请批号明细"].ToString();
                        dr["出库入库"] = "出库";

                        dr["相关单位"] = dt_xg.Rows[0]["课室"].ToString();
                        dr["数量"] = (Decimal)0;
                        dr["标准数量"] = (Decimal)0;
                        dr["实效数量"] = Convert.ToDecimal("-" + r["实际借用数量"].ToString());
                        dr["实效时间"] = t;
                        dr["出入库时间"] = t;

                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm其他出库_fun_保存出入库明细");
                throw ex;
            }
        }

        #endregion

        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("确定打印？", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                DataTable dt_dy = dt_借还申请表附表.Copy();
                int count = dt_dy.Rows.Count / 14;
                if (dt_dy.Rows.Count % 14 != 0)
                {
                    count++;
                }

                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                this.printDialog1.Document = this.printDocument1;
                DialogResult drt = this.printDialog1.ShowDialog();
                if (drt == DialogResult.OK)
                {
                    string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                    ItemInspection.print_FMS.fun_print_借用出库(dr["申请批号"].ToString(), dt_dy, count, false, PrinterName);
                }
            }
        }

        private void gridView2_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
                if (e.Column.FieldName == "仓库号")
                {
                    dr["仓库号"] = e.Value;
                    DataRow[] ds = dt_仓库号.Select(string.Format("仓库号 = '{0}'", dr["仓库号"]));
                    dr["仓库名称"] = ds[0]["仓库名称"];
                    //dr["仓库名称"] = sr["仓库名称"].ToString();
                    string sql = "select * from 仓库物料数量表 where 物料编码 = '" + dr["物料编码"] + "' and 仓库号 = '" + dr["仓库号"] + "'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    DataTable dt_物料数量 = new DataTable();
                    da.Fill(dt_物料数量);
                    if (dt_物料数量.Rows.Count == 0)
                    {
                        dr["库存总数"] = 0;
                        // dr["有效总数"] = 0;
                        dr["货架描述"] = "";
                    }
                    else
                    {
                        dr["库存总数"] = dt_物料数量.Rows[0]["库存总数"];
                        //  dr["有效总数"] = dt_物料数量.Rows[0]["有效总数"];
                        dr["货架描述"] = dt_物料数量.Rows[0]["货架描述"];//19-9-17解决货架更新
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    gc.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }
    }
}
