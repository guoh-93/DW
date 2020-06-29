using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace StockCore
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm基础信息仓库库位表 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        #region 成员
        DataTable dtM;
        SqlDataAdapter daM;
        DataTable dt;
        string strconn = CPublic.Var.strConn;
        #endregion

        #region 自用类
        public frm基础信息仓库库位表()
        {
            InitializeComponent();
            //fun_载入();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm仓库库位维护_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql2 = "select * from 基础数据仓库主表";
                SqlDataAdapter da = new SqlDataAdapter(sql2, strconn);
                dt = new DataTable();
                da.Fill(dt);
                repositoryItemSearchLookUpEdit1.DataSource = dt;
                repositoryItemSearchLookUpEdit1.ValueMember = "仓库号";
                repositoryItemSearchLookUpEdit1.DisplayMember = "仓库号";
                fun_载入();

                dtM.ColumnChanged += dtM_ColumnChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void dtM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Column.ColumnName == "仓库号")
            {
                string ss = e.Row["仓库号"].ToString();
                DataRow[] ds = dt.Select(string.Format("仓库号 = '{0}'", ss));
                try
                {
                    e.Row["仓库名称"] = ds[0]["仓库名称"].ToString();
                    e.Row["仓库类型"] = ds[0]["仓库类型"].ToString();
                }
                catch { }
            }
        }
        #endregion

        #region 方法
#pragma warning disable IDE1006 // 命名样式
        private void fun_载入()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 基础数据仓库库位表 where 上级库位 <> ''";
            daM = new SqlDataAdapter(sql, strconn);
            dtM = new DataTable();
            daM.Fill(dtM);
            gc.DataSource = dtM;
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_保存时赋值()
#pragma warning restore IDE1006 // 命名样式
        {
            foreach(DataRow r in dtM.Rows)
            {
                r["GUID"] = System.Guid.NewGuid();
                r["上级库位"] = Convert.ToInt32(r["仓库号"]).ToString("00") + "-" + "0000";
                r["仓库号"] = Convert.ToInt32(r["仓库号"]).ToString("00");
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_()
#pragma warning restore IDE1006 // 命名样式
        {
            foreach (DataRow r in dtM.Rows)
            {
                if (r.RowState == DataRowState.Added)
                {
                    r["库位号"] = Convert.ToInt32(r["仓库号"]).ToString("00") + "-" + Convert.ToInt32(r["库位号"]).ToString("0000");
                }
            }
        }
        #endregion

        #region 界面操作
        //新增
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = dtM.NewRow();
                dtM.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //删除
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                dr.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[gc.DataSource].EndCurrentEdit();
                string sql = "select * from 基础数据仓库库位表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    fun_保存时赋值(); fun_();
                    new SqlCommandBuilder(da);
                    da.Update(dtM);
                }
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion
    }
}
