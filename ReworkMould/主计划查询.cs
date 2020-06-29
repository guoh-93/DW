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

namespace ReworkMould
{
    public partial class 主计划查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_主;
        DataTable dt_子;
        public 主计划查询()
        {
            InitializeComponent();
        }

        private void 主计划查询_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(splitContainer1, this.Name, cfgfilepath);
            try
            {
                fun_load();
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
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            string sql = "select * from 主计划主表";
            dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_主;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string sql = string.Format("select * from 主计划子表 where 主计划单号 = '{0}'", dr["主计划单号"]);
                dt_子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gridControl3.DataSource = dt_子;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否确认关闭此条明细？", "提示!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow dr = gridView3.GetDataRow(gridView3.FocusedRowHandle);
                    string sql = string.Format("select * from 主计划子表 where 主计划明细号 = '{0}'",dr["主计划明细号"]);
                    DataTable dt_明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_明细.Rows.Count > 0)
                    {
                        if(Convert.ToBoolean(dt_明细.Rows[0]["完成"]) == true)
                        {
                            throw new Exception("该单据已完成，不可关闭");
                        }
                        dt_明细.Rows[0]["关闭"] = true;
                        sql = "select * from 主计划子表 where 1<>1";
                        SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                        new SqlCommandBuilder(da);
                        da.Update(dt_明细);
                        MessageBox.Show("关闭成功");
                        DataRow[] dr_子 = dt_子.Select(string.Format("主计划明细号 = '{0}'", dr["主计划明细号"]));
                        dr_子[0]["关闭"] = true;
                        dr_子[0].AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否确认撤销此条明细？", "提示!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow dr = gridView3.GetDataRow(gridView3.FocusedRowHandle);
                    string sql = string.Format("select * from 主计划子表 where 主计划明细号 = '{0}'", dr["主计划明细号"]);
                    DataTable dt_明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                    if (dt_明细.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt_明细.Rows[0]["完成"]) == true)
                        {
                            throw new Exception("该单据已完成，不可撤销");
                        }
                        dt_明细.Rows[0]["撤销"] = true;
                        string sqll_xm = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号 = '{0}'", dr["关联订单明细号"]);
                        DataTable dt_xm = CZMaster.MasterSQL.Get_DataTable(sqll_xm,strconn);


                        string sql_ym = string.Format("select * from 销售预订单明细表 where 销售预订单明细号 = '{0}'", dr["关联订单明细号"]);
                        DataTable dt_ym = CZMaster.MasterSQL.Get_DataTable(sql_ym, strconn);



                        string sql_jm = string.Format("select * from 借还申请表附表 where 申请批号明细 = '{0}'", dr["关联订单明细号"]);
                        DataTable dt_jm = CZMaster.MasterSQL.Get_DataTable(sql_jm, strconn);

                         


                        if (dt_xm.Rows.Count > 0)
                        {
                            dt_xm.Rows[0]["已转数量"] = Convert.ToDecimal(dt_xm.Rows[0]["已转数量"]) - Convert.ToDecimal(dr["转单未完成数量"]);
                        }

                        if (dt_ym.Rows.Count > 0)
                        {
                            dt_ym.Rows[0]["已转数量"] = Convert.ToDecimal(dt_ym.Rows[0]["已转数量"]) - Convert.ToDecimal(dr["转单未完成数量"]);
                        }

                        if (dt_jm.Rows.Count > 0)
                        {
                            dt_jm.Rows[0]["已转数量"] = Convert.ToDecimal(dt_jm.Rows[0]["已转数量"]) - Convert.ToDecimal(dr["转单未完成数量"]);
                        }

                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("撤销单据");
                        try
                        {
                            
                            sql = "select * from 主计划子表 where 1<>1";
                            SqlCommand cmm = new SqlCommand(sql, conn, ts);
                            SqlDataAdapter da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_明细);

                            sql = "select * from 销售记录销售订单明细表 where 1<>1";
                            cmm = new SqlCommand(sql, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_xm);

                            sql = "select * from 销售预订单明细表 where 1<>1";
                            cmm = new SqlCommand(sql, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_ym);

                            sql = "select * from 借还申请表附表 where 1<>1";
                            cmm = new SqlCommand(sql, conn, ts);
                            da = new SqlDataAdapter(cmm);
                            new SqlCommandBuilder(da);
                            da.Update(dt_jm);
                            ts.Commit();
                            MessageBox.Show("撤销成功");

                            DataRow[] dr_子 = dt_子.Select(string.Format("主计划明细号 = '{0}'", dr["主计划明细号"]));
                            dr_子[0]["撤销"] = true;
                            dr_子[0].AcceptChanges();
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gridView3_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl3, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }
        }
    }
}
