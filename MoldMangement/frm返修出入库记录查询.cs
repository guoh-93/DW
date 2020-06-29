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
    public partial class frm返修出入库记录查询 : UserControl
    {
        string strConn = CPublic.Var.strConn;
        public frm返修出入库记录查询()
        {
            InitializeComponent();
        }

        private void fun_SetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }

        private void fun_GetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
                da.Fill(dt);
            }
        }

        private void fun_载入()
        {
            string sql = "";
            DataTable dt_主表 = new DataTable();
            //if (barEditItem3.EditValue.ToString() == "全部")
            //{
            sql = string.Format("select * from 返修出入库申请主表 where 生效日期 > '{0}' and 生效日期 < '{1}'", barEditItem1.EditValue, barEditItem2.EditValue);
            //}
            if (barEditItem3.EditValue.ToString() == "已生效")
            {
                sql = sql + string.Format(" and 生效 = {0}", 1);
            }
            if (barEditItem3.EditValue.ToString() == "未生效")
            {
                sql = sql + string.Format(" and 生效 = {0}", 0);
            }
            fun_GetDataTable(dt_主表, sql);
            gc1.DataSource = dt_主表;
        }
        private void frm返修出入库记录查询_Load(object sender, EventArgs e)
        {
            this.gv1.IndicatorWidth = 40;
            this.gv2.IndicatorWidth = 40;
            barEditItem1.EditValue = System.DateTime.Today.AddDays(-14);
            barEditItem2.EditValue = System.DateTime.Today.AddDays(1).AddSeconds(-1);
            barEditItem3.EditValue = "已生效";
            fun_载入();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_载入();
        }

        private void gv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            //if (e.Button == MouseButtons.Left && e.Clicks == 1)
            //{
                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                DataTable dt_子表 = new DataTable();
                string sql = string.Format("select * from 返修出入库申请子表 where 出入库申请单号 = '{0}'", dr["出入库申请单号"].ToString());
                fun_GetDataTable(dt_子表, sql);
                gc2.DataSource = dt_子表;
            //}
        }

        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }




    }
}
