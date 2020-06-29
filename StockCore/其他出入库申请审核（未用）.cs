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
    public partial class 其他出入库申请审核 : UserControl
    {
        #region 变量
        DataTable dt_left;
        DataTable dt_right;
        string strcon = CPublic.Var.strConn;
        DataTable dt_权限;
 
        #endregion

        public 其他出入库申请审核()
        {
            InitializeComponent();
        }

        private void 其他出入库申请审核_Load(object sender, EventArgs e)
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

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_load();
                fun_detail("");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_save();
                MessageBox.Show("已审核");
                barLargeButtonItem1_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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
        private void gv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            if (dr == null) return;
            fun_detail(dr["出入库申请单号"].ToString());
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            string sx = "";
            if (CPublic.Var.localUser组织关系.Trim() != "")
            {
              DataTable dt= ERPorg.Corg.fun_hr(CPublic.Var.LocalUserTeam, CPublic.Var.LocalUserID);
              if (dt.Rows.Count > 0)
              {
                  sx = "and 待审核人ID in (";
                  foreach (DataRow r in dt.Rows)
                  {
                      sx = sx + string.Format("'{0}',", r["工号"]);
                  }
                  sx = sx.Substring(0, sx.Length - 1) + ")";
              }
            }
           
            string s = string.Format("select  * from 其他出入库申请主表 where 待审核=1 and 作废=0 and 审核=0 {0} ",sx);
            dt_left = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gc1.DataSource = dt_left;
            DataView dv = new DataView(dt_left);
            dv.RowFilter = string.Format("待审核人ID='{0}'", CPublic.Var.LocalUserID);
            gc1.DataSource = dv;
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_detail( string str_单号)
#pragma warning restore IDE1006 // 命名样式
        {
            string s = string.Format("select  * from 其他出入库申请子表 where 出入库申请单号='{0}'", str_单号);
            dt_right = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gcP.DataSource = dt_right;
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_save()
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime time= CPublic.Var.getDatetime();
            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            dr["审核"]=true;
            dr["审核人员"]=CPublic.Var.localUserName;
            dr["审核日期"] = time;
            dr["生效"] = true;
            dr["生效人员编号"] = CPublic.Var.LocalUserID;
            dr["生效日期"] = time;
            foreach (DataRow xr in dt_right.Rows)
            {
                xr["生效"] = true;
                dr["生效人员编号"] = CPublic.Var.LocalUserID;
                dr["生效日期"] = time;
            }
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("qs"); //事务的名称
            SqlCommand cmd1 = new SqlCommand("select * from 其他出入库申请主表 where 1<>1", conn, ts);
            SqlCommand cmd = new SqlCommand("select * from 其他出入库申请子表 where 1<>1", conn, ts);
            try
            {
                SqlDataAdapter da;
                da = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da);
                da.Update(dt_left);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_right);
                ts.Commit();
            }
            catch
            {
                ts.Rollback();
            }


        }

    }
}
