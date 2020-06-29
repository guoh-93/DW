using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frm产品金额对照关系 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_产品 = new DataTable();
        DataTable dt_客户 = new DataTable();
        DataTable dt_供应商 = new DataTable();
        #endregion

        #region 自用类
        public frm产品金额对照关系()
        {
            InitializeComponent();
        }

        private void frm产品金额对照关系_Load(object sender, EventArgs e)
        {
            try
            {
                fun_载入();
                fun_下拉框();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm产品金额对照关系_Load");
            }
        }
        #endregion

        #region 方法
        private void fun_载入()
        {
            string sql = "select * from 产品金额对照表";
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gc.DataSource = dtM;

            dtM.ColumnChanged += dtM_ColumnChanged;
        }

        private void dtM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (e.Column.ColumnName == "产品编号")
                {
                    DataRow[] ds = dt_产品.Select(string.Format("物料编码 = '{0}'", dr["产品编号"].ToString()));
                    e.Row["产品名称"] = ds[0]["物料名称"].ToString();
                }
                if (e.Column.ColumnName == "供应商ID")
                {
                    DataRow[] ds = dt_供应商.Select(string.Format("供应商ID = '{0}'", dr["供应商ID"].ToString()));
                    e.Row["供应商名称"] = ds[0]["供应商名称"].ToString();
                }
                if (e.Column.ColumnName == "客户编号")
                {
                    DataRow[] ds = dt_客户.Select(string.Format("客户编号 = '{0}'", dr["客户编号"].ToString()));
                    e.Row["客户名称"] = ds[0]["客户名称"].ToString();
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm产品金额对照关系_dtM_ColumnChanged");
            }
        }

        private void fun_下拉框()
        {
            try
            {
                string sql_产品 = "select 物料编码,物料名称,物料类型 from 基础数据物料信息表";
                string sql_客户 = "select 客户名称,客户编号 from 客户基础信息表";
                string sql_供应商 = "select 供应商ID,供应商名称 from 采购供应商表";
                SqlDataAdapter da_产品 = new SqlDataAdapter(sql_产品, strconn);
                SqlDataAdapter da_客户 = new SqlDataAdapter(sql_客户, strconn);
                SqlDataAdapter da_供应商 = new SqlDataAdapter(sql_供应商, strconn);
                da_产品.Fill(dt_产品);
                da_供应商.Fill(dt_供应商);
                da_客户.Fill(dt_客户);
                dt_产品.Columns.Add("产品编号");
                foreach (DataRow r in dt_产品.Rows)
                {
                    r["产品编号"] = r["物料编码"].ToString();
                }
                repositoryItemSearchLookUpEdit1.DataSource = dt_产品;
                repositoryItemSearchLookUpEdit1.ValueMember = "产品编号";
                repositoryItemSearchLookUpEdit1.DisplayMember = "产品编号";

                repositoryItemSearchLookUpEdit2.DataSource = dt_供应商;
                repositoryItemSearchLookUpEdit2.ValueMember = "供应商ID";
                repositoryItemSearchLookUpEdit2.DisplayMember = "供应商ID";

                repositoryItemSearchLookUpEdit3.DataSource = dt_客户;
                repositoryItemSearchLookUpEdit3.ValueMember = "客户编号";
                repositoryItemSearchLookUpEdit3.DisplayMember = "客户编号";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fun_Check()
        {
            try
            {
                foreach (DataRow r in dtM.Rows)
                {

                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = dtM.NewRow();
            dtM.Rows.Add(dr);
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            dr.Delete();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                string sql = "select * from 产品金额对照表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dtM);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm产品金额对照关系_保存");
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion
    }
}
