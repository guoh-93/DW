using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace StockCore
{
    public partial class ECR变更申请审核 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_审核;
        public ECR变更申请审核()
        {
            InitializeComponent();
        }

        private void ECR变更申请审核_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel2, this.Name, cfgfilepath);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            string sql = "select sh.*,sq.申请日期 from  ECR变更申请审核表 sh left join  ECR变更申请单主表 sq on sh.申请单号 = sq.申请单号  where sh.审核 = 0 ";
            
            if (CPublic.Var.LocalUserID != "admin"&&CPublic.Var.LocalUserTeam !="管理员权限")
            {
                sql = sql + "and 审核部门 = '"+CPublic.Var.localUser部门名称+"' and 部门负责人ID = '"+CPublic.Var.LocalUserID+"'";
            }
            dt_审核 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_审核;
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null) return;
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));                    
                }                 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            StockCore.ui_ECR变更申请 ui = new ui_ECR变更申请(dr);
            CPublic.UIcontrol.Showpage(ui, "申请明细");
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("是否确认通过？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    gridView1.CloseEditor();
                    this.BindingContext[dt_审核].EndCurrentEdit();
                    DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    if (dr["审核意见"].ToString() == "")
                    {
                        throw new Exception("审核意见必填");
                    }
                    string sql = string.Format("select * from ECR变更申请审核表 where 申请单号 = '{0}' and 部门负责人ID = '{1}'",dr["申请单号"].ToString(),dr["部门负责人ID"].ToString());
                    DataTable dt_审核表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_审核表.Rows.Count>0)
                    {
                        dt_审核表.Rows[0]["审核"] = true;
                        dt_审核表.Rows[0]["bl"] = true;
                        dt_审核表.Rows[0]["审核意见"] = dr["审核意见"].ToString();
                        dt_审核表.Rows[0]["审核日期"] = CPublic.Var.getDatetime();
                        dt_审核表.Rows[0]["实际操作人"] = CPublic.Var.localUserName;
                        dt_审核表.Rows[0]["实际操作人ID"] = CPublic.Var.LocalUserID;
                    }
                    sql = string.Format("select * from ECR变更申请审核表 where 申请单号 = '{0}' and 部门负责人ID <> '{1}' and 审核 = 0", dr["申请单号"].ToString(), dr["部门负责人ID"].ToString());
                    DataTable dt1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    DataTable dt_变更申请 = new DataTable();
                    DataTable dt_变更申请子 = new DataTable();
                    sql = "select * from 基础数据物料信息表 where 1<>1";
                    DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    DataTable dt_审核_1 = new DataTable();
                    if (dt1.Rows.Count==0)
                    {
                        sql = string.Format("select * from ECR变更申请单主表 where 申请单号 = '{0}'",dr["申请单号"].ToString());
                        dt_变更申请 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        dt_变更申请.Rows[0]["部门审核"] = true;
                        dt_审核_1 = ERPorg.Corg.fun_PA("生效", "ECN变更申请", dr["申请单号"].ToString(), dt_变更申请.Rows[0]["提出单位"].ToString(), dt_变更申请.Rows[0]["申请人ID"].ToString());
                        //sql = string.Format("select * from ECR变更申请审核表 where 申请单号 = '{0}' and 部门负责人ID <> '{1}' and bl = 0", dr["申请单号"].ToString(), dr["部门负责人ID"].ToString());
                        //DataTable dt2 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        //if (dt2.Rows.Count == 0)
                        //{
                        //    sql = string.Format("select * from  ECR变更申请单明细表 where 申请单号 = '{0}'", dr["申请单号"]);
                        //    dt_变更申请子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        //    foreach (DataRow dr_物料 in dt_变更申请子.Rows)
                        //    {
                        //        sql = string.Format("select * from 基础数据物料信息表 where 物料编码 = '{0}'",dr_物料["物料编码"]);
                        //        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                        //        da.Fill(dt_物料);
                        //        DataRow[] dr11 = dt_物料.Select(string.Format("物料编码 = '{0}'",dr_物料["物料编码"]));
                        //        if (dr11.Length>0)
                        //        {
                        //            dr11[0]["在研"] = true;
                        //        }
                        //    }

                        //}                                           
                    }
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("ECR审核");
                    try
                    {
                        sql = "select * from ECR变更申请审核表 where 1<>1";
                        SqlCommand cmm = new SqlCommand(sql, conn, ts);
                        SqlDataAdapter da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(dt_审核表);

                        if (dt_变更申请.Rows.Count>0)
                        {
                            sql = "select * from ECR变更申请单主表 where 1<>1";
                            cmm = new SqlCommand(sql, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_变更申请);

                            sql = "select * from 单据审核申请表 where 1<>1";
                            cmm = new SqlCommand(sql, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_审核_1);
                        }

                        //if (dt_物料.Rows.Count>0)
                        //{
                        //    sql = "select * from 基础数据物料信息表 where 1<>1";
                        //    cmm = new SqlCommand(sql, conn, ts);
                        //    da = new SqlDataAdapter(cmm);
                        //    new SqlCommandBuilder(da);
                        //    da.Update(dt_物料);
                        //}
                                               
                        ts.Commit();
                        MessageBox.Show("审核成功");
                        dt_审核.Rows.Remove(dr);

                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw new Exception(ex.Message);
                    }




                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                    
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("是否确认不通过？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    gridView1.CloseEditor();
                    this.BindingContext[dt_审核].EndCurrentEdit();
                    DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    if (dr["审核意见"].ToString() == "")
                    {
                        throw new Exception("审核意见必填");
                    }
                    string sql = string.Format("select * from ECR变更申请审核表 where 申请单号 = '{0}' and 部门负责人ID = '{1}'", dr["申请单号"].ToString(), dr["部门负责人ID"].ToString());
                    DataTable dt_审核表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_审核表.Rows.Count > 0)
                    {
                        dt_审核表.Rows[0]["审核"] = true;
                        dt_审核表.Rows[0]["bl"] = false;
                        dt_审核表.Rows[0]["审核意见"] = dr["审核意见"].ToString();
                        dt_审核表.Rows[0]["审核日期"] = CPublic.Var.getDatetime();
                        dt_审核表.Rows[0]["实际操作人"] = CPublic.Var.localUserName;
                        dt_审核表.Rows[0]["实际操作人ID"] = CPublic.Var.LocalUserID;
                    }
                    sql = string.Format("select * from ECR变更申请审核表 where 申请单号 = '{0}' and 部门负责人ID <> '{1}' and 审核 = 0", dr["申请单号"].ToString(), dr["部门负责人ID"].ToString());
                    DataTable dt1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    DataTable dt_变更申请 = new DataTable();
                    DataTable dt_变更申请子 = new DataTable();
                    sql = "select * from 基础数据物料信息表 where 1<>1";
                    DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    DataTable dt_审核_1 = new DataTable();
                    if (dt1.Rows.Count == 0)
                    {
                        sql = string.Format("select * from ECR变更申请单主表 where 申请单号 = '{0}'", dr["申请单号"].ToString());
                        dt_变更申请 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        dt_变更申请.Rows[0]["部门审核"] = true;
                        dt_审核_1 = ERPorg.Corg.fun_PA("生效", "ECN变更申请", dr["申请单号"].ToString(), dt_变更申请.Rows[0]["提出单位"].ToString(),dt_变更申请.Rows[0]["申请人ID"].ToString());

                    }
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("ECR审核");
                    try
                    {
                        sql = "select * from ECR变更申请审核表 where 1<>1";
                        SqlCommand cmm = new SqlCommand(sql, conn, ts);
                        SqlDataAdapter da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(dt_审核表);

                        if (dt_变更申请.Rows.Count > 0)
                        {
                            sql = "select * from ECR变更申请单主表 where 1<>1";
                            cmm = new SqlCommand(sql, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_变更申请);

                            sql = "select * from 单据审核申请表 where 1<>1";
                            cmm = new SqlCommand(sql, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_审核_1);
                        }

                         
                        ts.Commit();
                        MessageBox.Show("审核成功");
                        dt_审核.Rows.Remove(dr);

                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw new Exception(ex.Message);
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
