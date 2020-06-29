using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm销售记录预计完工日期界面 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        #region
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataView dv;
        #endregion

        #region
        public frm销售记录预计完工日期界面()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm销售记录预计完工日期界面_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
            devGridControlCustom1.strConn = CPublic.Var.strConn;
            bar_日期_后.EditValue = System.DateTime.Today.AddDays(1).AddSeconds(-1);
            bar_日期_前.EditValue = System.DateTime.Today.AddDays(-7);

            fun_载入();
        }

#pragma warning disable IDE1006 // 命名样式
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //打钩显示全部销售订单
            if (checkBox1.Checked == true)
            {
                gc.DataSource = dtM;
            }
            else
            {
                gc.DataSource = dv;
            }
        }
        #endregion

        #region
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入()
#pragma warning restore IDE1006 // 命名样式
        {
            dtM = new DataTable();
            string sql = string.Format("select * from 销售记录销售订单明细表 where 修改日期 >= '{0}' and 修改日期 <= '{1}'", bar_日期_前.EditValue, bar_日期_后.EditValue);
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            dv = new DataView(dtM);
            dv.RowFilter = string.Format("预计完工日期 is null");
            gc.DataSource = dtM;
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_保存()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 销售记录销售订单明细表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dtM);
        }
        #endregion

        #region
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_载入();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                fun_保存();
                MessageBox.Show("保存成功");
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
        #endregion

    }
}
