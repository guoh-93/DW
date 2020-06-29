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
    public partial class UI查看出库通知明细 : UserControl
    {
        #region 变量
        string str_物料编码;
        string strcon = CPublic.Var.strConn;
        DateTime dtime = CPublic.Var.getDatetime();
        

        #endregion

        public UI查看出库通知明细(string s)
        {
            InitializeComponent();
            this.str_物料编码 = s;
            dtime = new DateTime(dtime.Year, dtime.Month, dtime.Day);
            barEditItem2.EditValue = dtime;
            dtime.AddYears(-1);
            barEditItem1.EditValue = dtime;
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_load(string s)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format(@"select [销售记录销售出库通知单明细表].*,[销售记录销售出库通知单主表].备注 as 主表备注 from 销售记录销售出库通知单明细表 
                                         left join [销售记录销售出库通知单主表] on [销售记录销售出库通知单主表].出库通知单号=销售记录销售出库通知单明细表.出库通知单号
                                         where  销售记录销售出库通知单明细表.生效日期>='{0}' and 销售记录销售出库通知单明细表.生效日期<='{1}' 
                                and 销售记录销售出库通知单明细表.物料编码='{2}'", barEditItem1.EditValue,Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1), str_物料编码);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gridControl1.DataSource = dt;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load(str_物料编码);
        }

        private void UI查看出库通知明细_Load(object sender, EventArgs e)
        {
            fun_load(str_物料编码);

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
    }
}
