using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm生产成品入库单列表 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string strcon = "";

        public frm生产成品入库单列表()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        /// <summary>
        /// 成品入库单的主表
        /// </summary>
        DataTable dt_rukdjzb;

#pragma warning disable IDE1006 // 命名样式
        private void frm生产成品入库单列表_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (CPublic.Var.LocalUserTeam.Contains("管理员") || CPublic.Var.LocalUserID == "admin")
                {
                    barLargeButtonItem7.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                }
                txt_工单.EditValue = "";

                DateTime t = CPublic.Var.getDatetime().Date.AddMonths(-1);
 
                txt_riqitime1.EditValue = Convert.ToDateTime(t.ToString("yyyy-MM-dd"));
                txt_riqitime2.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().Date.ToString("yyyy-MM-dd"));
                txt_rkdanjuzt.EditValue = "已生效";
                fun_getChengPinRKDliebiao();
                //查询
                barLargeButtonItem2_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 数据的查询
        /// </summary>
        private void fun_getChengPinRKDliebiao()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = "";
                ////如果领料单号不为空的话，就以领料单号来查询该领料单
                //if (txt_工单.EditValue.ToString() != "")
                //{
                //    sql = string.Format(" 成品入库单号='{0}' and", txt_工单.EditValue.ToString());
                //}
                //else   //如果领料单号为空的话，那就以时间跟单据状态来进行查询
                //{
                //如果两个时间都不为空的话才已时间进行查询
                if (txt_riqitime1.EditValue != null && txt_riqitime2.EditValue != null && txt_riqitime1.EditValue.ToString() != "" && txt_riqitime2.EditValue.ToString() != "")
                {
                    DateTime dtime = Convert.ToDateTime(txt_riqitime2.EditValue).AddDays(1).AddSeconds(-1);

                    if (Convert.ToDateTime(txt_riqitime1.EditValue) > Convert.ToDateTime(txt_riqitime2.EditValue))
                        throw new Exception("第一个是起始时间，不能大于终止时间，请重新选择时间");
                    sql = sql + string.Format(" 录入日期>='{0}' and 录入日期<='{1}' and", txt_riqitime1.EditValue, dtime);
                }
                //单据状态
                if (txt_rkdanjuzt.EditValue.ToString() == "未生效")  //生效
                {
                    sql = sql + string.Format(" 生效=0 and");
                }
                if (txt_rkdanjuzt.EditValue.ToString() == "已生效")
                {
                    sql = sql + string.Format(" 生效=1 and");
                }
                //if (txt_rkdanjuzt.EditValue.ToString() == "已作废") //作废
                //{
                //    sql = sql + string.Format(" 作废=1 and");
                //}
                //if (txt_rkdanjuzt.EditValue.ToString() == "未作废")
                //{
                //    sql = sql + string.Format(" 作废=0 and");
                //}
                //if (txt_rkdanjuzt.EditValue.ToString() == "未完成")  //完成
                //{
                //    sql = sql + string.Format(" 完成=0 and");
                //}
                //if (txt_rkdanjuzt.EditValue.ToString() == "已完成")
                //{
                //    sql = sql + string.Format(" 完成=1 and");
                //}
                //}

                //入库人员工号
                ////视图权限
                //DataTable dt_仓库人员 = ERPorg.Corg.fun_hr("仓库", CPublic.Var.LocalUserID);

                //if (CPublic.Var.LocalUserTeam != "管理员" && dt_仓库人员.Rows.Count>0)
                //{

                //        sql += " ( ";
                //        foreach (DataRow r in dt_仓库人员.Rows)
                //        {
                //            sql += "入库人员工号 = '" + r["工号"].ToString().Trim() + "' or ";
                //        }
                //        sql = sql.Substring(0, sql.Length - 3);
                //        sql = sql + " ) ";
                //        sql += " and";

                //}

                sql = " where " + sql.Substring(0, sql.Length - 3);
                sql = string.Format("select * from 生产记录成品入库单主表 {0}", sql);
                SqlDataAdapter da;
                dt_rukdjzb = new DataTable();
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_rukdjzb);
                gc_rukd.DataSource = dt_rukdjzb;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SearchData");
                throw ex;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_search_工单()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select * from 生产记录成品入库单主表 where 成品入库单号 in 
            (select  [成品入库单号]  from [生产记录成品入库单明细表] where 生产工单号 like '%{0}%')", txt_工单.EditValue.ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dt_rukdjzb = new DataTable();

                da.Fill(dt_rukdjzb);
                gc_rukd.DataSource = dt_rukdjzb;
            }
        }
        #region
        //查询操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_getChengPinRKDliebiao();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //新增界面
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                frm成品入库明细 frm = new frm成品入库明细();
                CPublic.UIcontrol.AddNewPage(frm, "成品入库");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //查找工单号
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (txt_工单.EditValue != null && txt_工单.EditValue.ToString() != "")
                {
                    fun_search_工单();
                }
                else
                {
                    throw new Exception("未输入工单号");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //关闭界面
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

        #endregion

        //明细的查询
        private void 查询明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r = (this.BindingContext[dt_rukdjzb].Current as DataRowView).Row;
                //if (r["生效"].Equals(true))
                //{
                //    MessageBox.Show("该成品入库单已生效");
                //}
                //else
                //{
                DataRow dr = gv_rukd.GetDataRow(gv_rukd.FocusedRowHandle);
                //ERPproduct.frm成品入库单列表视图 frm = new frm成品入库单列表视图(dr["成品入库单号"].ToString());
                //CPublic.UIcontrol.AddNewPage(frm, "成品入库明细");
                ERPproduct.frm成品入库单列表视图 frm = new frm成品入库单列表视图(dr["成品入库单号"].ToString());
                CPublic.UIcontrol.AddNewPage(frm, "生产成品入库明细");
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_rukd_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gv_rukd.GetDataRow(gv_rukd.FocusedRowHandle);
            fun_load_明细(dr["成品入库单号"].ToString());
 
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load_明细(string str_成品入库单号)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = string.Format(@"select 生产记录成品入库单明细表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号 from 生产记录成品入库单明细表 
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录成品入库单明细表.物料编码 
                where 成品入库单号='{0}'", str_成品入库单号);
                DataTable dt = new DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                gridControl1.DataSource = dt;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_rukd_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gc_rukd_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_rukd.GetFocusedRowCellValue(gv_rukd.FocusedColumn));
                e.Handled = true;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDialog1.Document = this.printDocument1;
            DialogResult dr = this.printDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {

                string str_p = this.printDocument1.PrinterSettings.PrinterName;

                DataRow r = gv_rukd.GetDataRow(gv_rukd.FocusedRowHandle);
                string sql = string.Format(@"select  成品入库单号,a.生产工单号,a.生产检验单号,原ERP物料编号,车间,入库数量,送检数量,a.物料名称,入库人员  
                  ,a.生效日期    from  生产记录成品入库单明细表 a 
               left join 生产记录生产检验单主表 b on b.生产检验单号=a.生产检验单号 
               left join 基础数据物料信息表 c on c.物料编码=a.物料编码
               where 成品入库单号 ='{0}'", r["成品入库单号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ItemInspection.print_FMS.fun_成品入库(dt, str_p);
                }
            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv_rukd.GetDataRow(gv_rukd.FocusedRowHandle);
            DateTime t = CPublic.Var.getDatetime().Date;
            t = new DateTime(t.Year, t.Month, 1);

            try
            {
                if (Convert.ToDateTime(dr["生效日期"]) < t) throw new Exception("往月数据不可撤销");
                if (MessageBox.Show("确定撤回该单据？请核对。", "提示!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    ERPorg.Corg cg = new ERPorg.Corg();
                    DataSet ds = cg.back_ruk(dr["成品入库单号"].ToString());

                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction bts = conn.BeginTransaction("生产入库撤回");
                    try
                    {
                        SqlCommand cmm = new SqlCommand("select * from 生产记录生产制令表 where 1<>1", conn, bts);
                        SqlDataAdapter da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[0]);

                        cmm = new SqlCommand("select * from 生产记录生产工单表  where 1<>1", conn, bts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[1]);


                        cmm = new SqlCommand("select  * from 生产记录生产检验单主表 where 1=2", conn, bts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[2]);

                        cmm = new SqlCommand("select  * from  生产记录成品入库单主表 where 1=2", conn, bts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[3]);

                        cmm = new SqlCommand("select  * from  生产记录成品入库单明细表 where 1=2", conn, bts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[4]);

                        cmm = new SqlCommand("select  * from 仓库出入库明细表 where 1=2 ", conn, bts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[5]);


                        cmm = new SqlCommand("select  * from 仓库物料数量表 where 1=2", conn, bts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(ds.Tables[6]);

                        bts.Commit();
                        MessageBox.Show("撤回成功");
                        dt_rukdjzb.Rows.Remove(dr);
                        DataRow drr = gv_rukd.GetDataRow(gv_rukd.FocusedRowHandle);
                        fun_load_明细(drr["成品入库单号"].ToString());
                    }

                    catch (Exception ex)
                    {
                        bts.Rollback();
                        throw new Exception(ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_rukd_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv_rukd.GetDataRow(gv_rukd.FocusedRowHandle);
                fun_load_明细(dr["成品入库单号"].ToString());
            }
            catch  
            {
                

            }
        }
    }
}
