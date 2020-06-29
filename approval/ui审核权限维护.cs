using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace approval
{
    public partial class ui审核权限维护 : UserControl
    {
        DataTable dt_人员;
        string strcon = CPublic.Var.strConn;

        public ui审核权限维护()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void fun_inf()
        {

            string sql = @"  select  员工号,姓名 from  人事基础员工表 where 在职状态='在职' ";
            DataTable dt_计划员 = new DataTable();
            SqlDataAdapter da_计划员 = new SqlDataAdapter(sql, strcon);
            da_计划员.Fill(dt_人员);
            repositoryItemSearchLookUpEdit1.DataSource = dt_人员;
            repositoryItemSearchLookUpEdit1.DisplayMember ="员工号";
            repositoryItemSearchLookUpEdit1.ValueMember = "姓名";


                         
            
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void repositoryItemSearchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            DataRow dr =gv_provider.GetDataRow(gv_provider.FocusedRowHandle);
            DataRow []rr= dt_人员.Select(string.Format("员工号='{0}'", dr["工号"]));
            dr["姓名"] = rr[0]["姓名"];

        }

        private void repositoryItemSearchLookUpEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.' )
            {
                e.Handled = true;
            }

            if (!((e.KeyChar >= 48 && e.KeyChar <= 57) || e.KeyChar == '.' || e.KeyChar == 8))
            {
                e.Handled = true;
            }
        }

        private void gv_provider_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            //行号设置 
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
    }
}
