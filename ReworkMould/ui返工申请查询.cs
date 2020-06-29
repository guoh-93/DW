using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ReworkMould
{
    public partial class ui返工申请查询 : UserControl
    {
        #region 
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_mx;

        #endregion

        public ui返工申请查询()
        {
            InitializeComponent();
        }

        private void fun_load()
        {
            DateTime t = Convert.ToDateTime(barEditItem1.EditValue).Date;

            DateTime t1 = Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1);
            string s = string.Format(@"select  a.*,base.物料名称 as 返修产品名称,base.规格型号 as 返修产品型号,数量,x.物料名称 as 目标产品名称,x.规格型号 as 目标产品规格  
                            from 新_返修申请主表 a   left  join 基础数据物料信息表 base on base.物料编码=a.返修产品编码
                            left  join 基础数据物料信息表 x on x.物料编码=a.目标产品编码   where  制单日期>'{0}' and 制单日期<'{1}' ",t,t1);
            dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gridControl1.DataSource = dtM;
        }

        private void fun_mx(string  s)
        {
            string x = string.Format(@" select  * from [新_返修申请子表] where 申请单号='{0}' ",s);
            dt_mx  = CZMaster.MasterSQL.Get_DataTable(x, strcon);
            gridControl2.DataSource = dt_mx;

        }
        private void ui返修申请查询_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime().Date;
                t = t.AddDays(1).AddSeconds(-1);
                barEditItem2.EditValue = t;
                barEditItem1.EditValue = t.AddMonths(-2).Date;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception ex )
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            fun_mx(dr["申请单号"].ToString());
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
            }
            if (CPublic.Var.LocalUserTeam == "管理员权限" || CPublic.Var.LocalUserID == dr["审核人员ID"].ToString())
            {
                barLargeButtonItem5.Enabled = true;
            }
            else
            {
                barLargeButtonItem5.Enabled = false;
            }
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select * from [新_返修申请主表] where 申请单号='{0}'",dr["申请单号"]);

                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (temp.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(temp.Rows[0]["作废"]) == true)
                    {
                        throw new Exception("该单据已作废，不可操作");
                    }
                    if (temp.Rows[0]["审核"].Equals(true))
                    {

                        throw new Exception("已通过审核,不可修改");
                    }
                    else if (temp.Rows[0]["提交审核"].Equals(true))
                    {
                        throw new Exception("已提交审核,不可修改,撤销后再试");
                    }
                }
                else
                {
                    throw new Exception("单据异常,刷新后重试");

                }
                ui返工申请 ui = new ui返工申请(dr);
                CPublic.UIcontrol.Showpage(ui, "返修申请修改");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
          

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select * from [新_返修申请主表] where 申请单号='{0}' and 审核=0",dr["申请单号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (temp.Rows.Count > 0)
                {
                    if(Convert.ToBoolean(temp.Rows[0]["作废"]) == true)
                    {
                        throw new Exception("该单据已作废，不可操作");
                    }
                    if (MessageBox.Show("确认将该条记录取消提交审核", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        temp.Rows[0]["提交审核"] = 0;
                        s = string.Format("select  * from 单据审核申请表 where 关联单号='{0}'", dr["申请单号"]);
                        DataTable dtt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        if(dtt.Rows.Count>0)   dtt.Rows[0].Delete();
                        //SqlConnection conn = new SqlConnection(strcon);
                        //conn.Open();
                        //SqlTransaction ts = conn.BeginTransaction("rwback"); //事务的名称
                        try
                        {
                            //CZMaster.MasterSQL.Save_DataTable(temp, "新_返修申请主表", ts);
                            //CZMaster.MasterSQL.Save_DataTable(dtt, "单据审核申请表", ts);
                            //ts.Commit();
                            string sql = "select * from 新_返修申请主表 where 1<>1";
                            SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                            new SqlCommandBuilder(da);
                            da.Update(temp);
                            sql = "select * from 单据审核申请表 where 1<>1";
                            da = new SqlDataAdapter(sql, strcon);
                            new SqlCommandBuilder(da);
                            dr["提交审核"] = 0;
                            dr.AcceptChanges();
                            MessageBox.Show("撤销成功");
                        }
                        catch (Exception ex)
                        {
                            //ts.Rollback();
                            MessageBox.Show("操作失败" + " " + ex.Message);
                        }
                    }
                }
                else
                {
                    throw new Exception("单据状态已更改,刷新后重试");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 作废ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string s = string.Format("select * from [新_返修申请主表] where 申请单号='{0}'",dr["申请单号"].ToString());
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);

                s = string.Format("select * from [新_返修申请子表] where 申请单号='{0}'", dr["申请单号"].ToString());
                DataTable temp_明细 = CZMaster.MasterSQL.Get_DataTable(s, strcon);

                if (temp.Rows.Count > 0)
                {
                    if (temp.Rows[0]["审核"].Equals(true))
                    {

                        throw new Exception("已通过审核,不可作废");
                    }
                }
                else
                {
                    throw new Exception("单据异常,刷新后重试");
                
                }
                dr["作废"]= temp.Rows[0]["作废"] = 1;
                dr["作废人员"]= temp.Rows[0]["作废人员"] = CPublic.Var.localUserName;
                dr["作废日期"]=temp.Rows[0]["作废日期"] = CPublic.Var.getDatetime();
                if (temp_明细.Rows.Count>0)
                {
                    foreach(DataRow dr_明细 in temp_明细.Rows)
                    {
                        dr_明细["作废"] = true;
                    }
                }

                s = string.Format("select  * from 单据审核申请表 where 关联单号='{0}'", dr["申请单号"]);
                DataTable dtt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (dtt.Rows.Count > 0)
                {
                    dtt.Rows[0]["作废"] = true;
                }   
                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("rwcancel"); //事务的名称
                try
                {
                    string sql = "select * from 新_返修申请主表 where 1<>1";
                    SqlCommand cmd = new SqlCommand(sql, conn, ts);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(temp);
                    sql = "select * from 单据审核申请表 where 1<>1";
                    cmd = new SqlCommand(sql, conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dtt);

                    sql = "select * from 新_返修申请子表 where 1<>1";
                    cmd = new SqlCommand(sql, conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(temp_明细);
                    // CZMaster.MasterSQL.Save_DataTable(dtt, "单据审核申请表", ts);
                    ts.Commit();
                    dr["提交审核"] = 0;
                    dr.AcceptChanges();
                    MessageBox.Show("作废成功");
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    MessageBox.Show("操作失败" + " " + ex.Message);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
          

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();
                gridControl1.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //2020-4-1 增加弃审
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                //判断退料单是否完成
                string sql_退料 = string.Format("select * from 新_返修申请退料子表 where 申请单号 = '{0}' and 完成 = 1 and 作废 = 0", dr["申请单号"].ToString());
                DataTable dt_退料 = CZMaster.MasterSQL.Get_DataTable(sql_退料, strcon);
                
                if (dt_退料.Rows.Count > 0)
                {
                    throw new Exception("该返修单已有料退库，不能弃审");
                }
                string sql_工单 = string.Format("select * from 生产记录生产工单表 where 备注2 = '{0}' and 作废 = 0 ", dr["申请单号"].ToString());
                DataTable dt_工单 = CZMaster.MasterSQL.Get_DataTable(sql_工单, strcon);
                string sql_制令 = string.Format("select * from 生产记录生产制令表 where 备注3 = '{0}' and 关闭 = 0", dr["申请单号"].ToString());
                DataTable dt_制令 = CZMaster.MasterSQL.Get_DataTable(sql_制令, strcon);
                if (dt_工单.Rows.Count>0)
                {
                    //判断待发料单是否已发料
                    string sql_待发料 = string.Format("select * from 生产记录生产工单待领料明细表 where 生产工单号 = '{0}' and 已领数量> 0",dt_工单.Rows[0]["生产工单号"].ToString());
                    DataTable dt_待发料 = CZMaster.MasterSQL.Get_DataTable(sql_待发料, strcon);
                    if (dt_待发料.Rows.Count>0)
                    {
                        throw new Exception("该返修单已有料发料，不能弃审");
                    }
                    if (MessageBox.Show(string.Format("确认弃审单据{0}", dr["申请单号"]), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        //退料单作废
                        string sql_退料_1 = string.Format("select * from 新_返修申请退料子表 where 申请单号 = '{0}' and 完成 = 0 and 作废 = 0", dr["申请单号"].ToString());
                        DataTable dt_退料_1 = CZMaster.MasterSQL.Get_DataTable(sql_退料_1, strcon);
                        if (dt_退料_1.Rows.Count>0)
                        {
                            foreach (DataRow dr_退料 in dt_退料_1.Rows)
                            {
                                dr_退料["作废"] = true;
                            }
                        }
                        //待发料单作废
                        string sql_待发料_1 = string.Format("select * from 生产记录生产工单待领料主表 where 生产工单号 = '{0}' and 完成 = 0 and 关闭 = 0", dt_工单.Rows[0]["生产工单号"].ToString());
                        DataTable dt_待发料_1 = CZMaster.MasterSQL.Get_DataTable(sql_待发料_1, strcon);
                        if (dt_待发料_1.Rows.Count>0)
                        {
                            dt_待发料_1.Rows[0]["关闭"] = true;
                            dt_待发料_1.Rows[0]["关闭时间"] = t;
                            dt_待发料_1.Rows[0]["备注1"] = "返修单弃审";                           
                        }
                        //制令作废
                        dt_制令.Rows[0]["关闭"] = true;
                        dt_制令.Rows[0]["关闭日期"] = t;
                        dt_制令.Rows[0]["关闭人员ID"] = CPublic.Var.LocalUserID;
                        dt_制令.Rows[0]["关闭人员"] = CPublic.Var.localUserName;
                        dt_制令.Rows[0]["备注"] = "返修单弃审";
                        //工单作废
                        dt_工单.Rows[0]["作废"] = true;
                        dt_工单.Rows[0]["关闭"] = true;
                        dt_工单.Rows[0]["关闭日期"] = t;
                        dt_工单.Rows[0]["关闭人员ID"] = CPublic.Var.LocalUserID;
                        dt_工单.Rows[0]["关闭人员"] = CPublic.Var.localUserName;
                        dt_工单.Rows[0]["备注1"] = "返修单弃审";
                        //修改返修单
                        string sql_返修主 = string.Format("select * from 新_返修申请主表 where 申请单号 = '{0}' and 作废 = 0", dr["申请单号"].ToString());
                        DataTable dt_返修 = CZMaster.MasterSQL.Get_DataTable(sql_返修主, strcon);
                        dt_返修.Rows[0]["审核"] = false;
                        dt_返修.Rows[0]["审核人员"] = "";
                        dt_返修.Rows[0]["审核人员ID"] = "";
                        dt_返修.Rows[0]["审核日期"] = DBNull.Value;

                        SqlConnection conn = new SqlConnection(strcon);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("QS");
                        try
                        {
                            SqlCommand cmd = new SqlCommand("select * from 生产记录生产工单表 where 1<>1", conn, ts);
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(dt_工单);

                            cmd = new SqlCommand("select * from 生产记录生产制令表 where 1<>1", conn, ts);
                            da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(dt_制令);

                            cmd = new SqlCommand("select * from 新_返修申请主表 where 1<>1", conn, ts);
                            da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(dt_返修);

                            cmd = new SqlCommand("select * from 生产记录生产工单待领料主表 where 1<>1", conn, ts);
                            da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(dt_待发料_1);

                            cmd = new SqlCommand("select * from 新_返修申请退料子表 where 1<>1", conn, ts);
                            da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(dt_退料_1);
                            MessageBox.Show("弃审成功");
                            ts.Commit();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("弃审失败");
                            ts.Rollback();
                            throw ex;
                        }

                    }

                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                fun_mx(dr["申请单号"].ToString());
                if (CPublic.Var.LocalUserTeam == "管理员权限"||CPublic.Var.LocalUserID == dr["审核人员ID"].ToString())
                {
                    barLargeButtonItem5.Enabled = true;
                }
                else
                {
                    barLargeButtonItem5.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
