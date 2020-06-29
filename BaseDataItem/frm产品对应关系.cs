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
    public partial class frm产品对应关系 : UserControl
    {
        #region 成员
        DataTable dtM;               //主表
        SqlDataAdapter da;
        DataTable dt;
        DataTable dt_客户;
        string strconn = CPublic.Var.strConn;
        string str_客户编号 = "";
        #endregion

        #region 自用类
        public frm产品对应关系()
        {
            InitializeComponent();
        }

        public frm产品对应关系(string str)
        {
            InitializeComponent();
            str_客户编号 = str;
        }

        private void frm产品对应关系_Load(object sender, EventArgs e)
        {
            fun_读取数据(str_客户编号);
            gc.DataSource = dtM;
            fun_dtM下拉框();
        }
        #endregion

        #region 方法
        public void fun_读取数据(string str)
        {
            try
            {
                dtM = new DataTable();
                string sql = string.Format("select * from 产品对应关系表 where 客户编号 = '{0}'", str);
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
            }
            catch(Exception ex)
            {
                //数据库没表
            }
            dtM.ColumnChanged += dtM_ColumnChanged;
        }

        void dtM_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "产品编号")
            {
                string ss = e.Row["产品编号"].ToString();
                DataRow[] ds = dt.Select(string.Format("物料编码 = '{0}'",ss));
                e.Row["产品名称"] = ds[0]["物料名称"].ToString();
                e.Row["规格型号"] = ds[0]["规格型号"].ToString();
                e.Row["图纸编号"] = ds[0]["图纸编号"].ToString();
            }
            if (e.Column.ColumnName == "客户编号")
            {
                string ss = e.Row["客户编号"].ToString();
                DataRow[] ds = dt_客户.Select(string.Format("客户编号 = '{0}'", ss));
                e.Row["客户名称"] = ds[0]["客户名称"].ToString();
            }
        }

        public void fun_dtM下拉框()
        {
            dt = new DataTable();
            string sql_1 = string.Format("SELECT 物料编码,物料名称,规格型号,图纸编号 FROM 基础数据物料信息表 where 物料类型 = '{0}'","成品");
            using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strconn))
            {
                try
                {
                    da.Fill(dt);
                    repositoryItemSearchLookUpEdit1.DataSource = dt;
                    repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
                    repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            dt_客户 = new DataTable();
            string sql_2 = string.Format("select 客户名称,客户编号 from 客户基础信息表 where 客户编号 = '{0}'", str_客户编号);
            using (SqlDataAdapter da2 = new SqlDataAdapter(sql_2, strconn))
            {
                try
                {
                    da2.Fill(dt_客户);
                    //repositoryItemSearchLookUpEdit2.DataSource = dt_客户;
                    //repositoryItemSearchLookUpEdit2.ValueMember = "客户编号";
                    //repositoryItemSearchLookUpEdit2.DisplayMember = "客户编号";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region 界面操作
        //新增
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow r = dtM.NewRow();
                dtM.Rows.Add(r);
                r["客户编号"] = str_客户编号;
                r["客户名称"] = dt_客户.Rows[0]["客户名称"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //删除
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认删除该条记录吗？", "询问！！", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
                    r.Delete();
                }
                //删除时  不删除数据库信息  只标记为废除
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //保存
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                string sql = "select * from 产品对应关系表 where 1<>1";
                SqlDataAdapter daa = new SqlDataAdapter(sql,strconn);
                new SqlCommandBuilder(daa);
                foreach (DataRow r in dtM.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    if (r["GUID"].ToString() == "")
                    {
                        r["GUID"] = System.Guid.NewGuid();
                    }
                }
                daa.Update(dtM);
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //CPublic.UIcontrol.ClosePage();
        }
        #endregion
         
    }
}
