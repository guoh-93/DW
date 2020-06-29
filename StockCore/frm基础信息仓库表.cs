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
    public partial class frm基础信息仓库表 : UserControl
    {
        #region 成员
        DataTable dtM;
        SqlDataAdapter daM;
        DataRow drM;
        //DataRow r;          //单机选中的行
        string strconn = CPublic.Var.strConn;
        #endregion

        #region 自用类
        public frm基础信息仓库表()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm仓库主表维护_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_载入();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 方法
#pragma warning disable IDE1006 // 命名样式
        public void fun_载入()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 基础数据仓库主表";
            daM = new SqlDataAdapter(sql, strconn);
            dtM = new DataTable();
            daM.Fill(dtM);
            gc.DataSource = dtM;
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_顶层库位()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = "select * from 基础数据仓库库位表";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                foreach (DataRow r in dtM.Rows)
                {
                    r["仓库号"] = Convert.ToInt32(r["仓库号"]).ToString("00");
                    if (r.RowState == DataRowState.Added)
                    {
                        DataRow dr = dt.NewRow();
                        dt.Rows.Add(dr);
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["库位号"] = r["仓库号"].ToString() + "0000";
                        dr["仓库号"] = Convert.ToInt32(r["仓库号"]).ToString("00");
                    }
                    if(r.RowState == DataRowState.Deleted)
                    {
                        DataRow[] ds = dt.Select(string.Format("库位号 = {0}", (r["仓库号"].ToString() + "0000")));
                        try
                        {
                            ds[0].Delete();
                        }
                        catch { }
                    }
                }
                da.Update(dt);
            }
            catch { }
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
                drM = dtM.NewRow();
                dtM.Rows.Add(drM);
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
                string sql = "select * from 基础数据仓库主表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    fun_顶层库位();
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
