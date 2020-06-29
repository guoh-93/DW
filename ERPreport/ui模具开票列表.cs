using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPreport
{
    public partial class ui模具开票列表 : UserControl
    {

        string strcon = CPublic.Var.strConn;
        public ui模具开票列表()
        {
            InitializeComponent();
        }

        private void ui模具开票列表_Load(object sender, EventArgs e)
        {
            try
            {

                barEditItem1.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().AddDays(-7).ToString("yyyy-MM-dd"));
                barEditItem2.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                fun_loadM();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 发票核销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr =gvv1.GetDataRow (gvv1.FocusedRowHandle);
            if (dr["作废"].Equals(false))
            {
                ERPreport.ui模具发票核销界面 ui = new ui模具发票核销界面(dr["模具开票通知号"].ToString());
                CPublic.UIcontrol.Showpage(ui, "核销界面");
            }
        }

        private void fun_loadM()
        {

            DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue);
            t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            DateTime t2 = Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1);
            t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
            string sql = string.Format(@"select * from [模具开票通知单主表] where 生效日期>'{0}' and 生效日期<'{1}'",t1,t2);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gcc1.DataSource = dt;
        }

        private void fun_loaddetail(string s)
        {
            string sql = string.Format(@"select * from [模具开票通知明细表] where  模具开票通知号='{0}'",s);
            DataTable dtP = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gcM.DataSource = dtP;
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_loadM();
        }

        private void gvv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
            fun_loaddetail(dr["模具开票通知号"].ToString());

            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gcc1, new Point(e.X, e.Y));
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

        private void gvv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

    
    }
}
