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
    public partial class ui模具合同列表 : UserControl
    {

        DataTable dtM;
        DataTable dtP;
        string strcon = CPublic.Var.strConn;
        public ui模具合同列表()
        {
            InitializeComponent();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }

        private void ui模具合同列表_Load(object sender, EventArgs e)
        {
            try
            {
                barEditItem2.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                barEditItem1.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().AddMonths(-1).ToString("yyyy-MM-dd"));
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
         
        }

        private void fun_load()
        {
            string sql = string.Format("select * from 模具合同台账主表 where 生效时间>'{0}' and 生效时间<'{1}'", barEditItem1.EditValue, barEditItem2.EditValue);
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gc.DataSource = dtM;

            string s = "select * from 模具合同台账明细表 where 1<>1";
            dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(s,strcon);
            gridControl1.DataSource = dtP;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            string s = string.Format("select * from 模具合同台账明细表 where 模具订单号='{0}'",dr["模具订单号"]);
            dtP = new DataTable();
            dtP = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gridControl1.DataSource = dtP;

            if (e.Clicks == 2)
            {
                ERPreport.ui模具合同台账 ui = new ui模具合同台账(dr);
                CPublic.UIcontrol.Showpage(ui,"模具合同台账");
            }
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
    }
}
