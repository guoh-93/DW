using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace StockCore
{
    public partial class UI返修申请查询 : UserControl
    {

        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        public UI返修申请查询()
        {
            InitializeComponent();
        }

        private void UI返修申请查询_Load(object sender, EventArgs e)
        {
            try
            {
                barEditItem2.EditValue = System.DateTime.Today.AddDays(1).AddSeconds(-1);
                barEditItem1.EditValue = System.DateTime.Today.AddDays(-14);
                
                fun_载入();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dtM != null)
                {
                    dtM.Clear();
                }
                string s_组合 = "select * from 返修出入库申请主表 {0}";
                string s_组合1 = "where ";

                if (barEditItem1.EditValue != null && barEditItem2.EditValue != null && barEditItem1.EditValue.ToString() != "" && barEditItem2.EditValue.ToString() != "")
                {
                    s_组合1 += " 申请日期 >= '" + ((DateTime)barEditItem1.EditValue).ToString("yyyy-MM-dd HH:mm:ss") + "'" + " and 申请日期 <= '" + ((DateTime)barEditItem2.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "' and ";
                }
             
                if (s_组合1 != "where ")
                {
                    s_组合1 = s_组合1.Substring(0, s_组合1.Length - 4);
                    s_组合 = string.Format(s_组合, s_组合1);
                }
                SqlDataAdapter da = new SqlDataAdapter(s_组合, strconn);
                da.Fill(dtM);
                gcM.DataSource = dtM;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("返修作废");
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                if (dr == null)
                    throw new Exception("请先选择需要作废的记录");
                if (dr["完成"].ToString().ToLower() == "true")
                    throw new Exception("该记录已完成，不需要作废");
                dr["作废"] = true;
                dr["作废日期"] = CPublic.Var.getDatetime ();
                dr["作废人员编号"] = CPublic.Var.LocalUserID;

                string sql = string.Format("select * from 返修出入库申请子表 where 出入库申请单号 = '{0}'", dr["出入库申请单号"]);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                foreach (DataRow r in dt.Rows)
                {
                    dr["作废"] = true;
                    dr["作废日期"] = CPublic.Var.getDatetime();
                    dr["作废人员编号"] = CPublic.Var.LocalUserID;
                }
             
                sql = "select * from 返修出入库申请子表 where 1<> 1";
                SqlCommand cmd = new SqlCommand(sql, conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt);

                sql = "select * from 返修出入库申请主表 where 1<> 1";
                cmd = new SqlCommand(sql, conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dtM);
                ts.Commit();

                MessageBox.Show("已作废:" + dr["出入库申请单号"].ToString());
            }
            catch (Exception ex)
            {
                ts.Rollback();
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            ERPorg.Corg.FlushMemory();
            fun_载入();
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_GetDataTable(DataTable dt, string sql)
#pragma warning restore IDE1006 // 命名样式
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                da.Fill(dt);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void gvM_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
            if (dr == null) return;
            DataTable dt_子表 = new DataTable();
            string sql = string.Format("select * from 返修出入库申请子表 where 出入库申请单号 = '{0}'", dr["出入库申请单号"].ToString());
            fun_GetDataTable(dt_子表, sql);
            gcP.DataSource = dt_子表;
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
