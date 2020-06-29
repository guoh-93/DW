using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CZMaster
{
    public partial class fmGridControlCustom : Form
    {
        public string strconn = "";

        public fmGridControlCustom()
        {
            InitializeComponent();
            //strcon = CPublic.Var.strConn;
        }

        string gvKey = "";

        DataTable dt_gcKeyMain;

        DataTable dt_authority;

        DataTable dt_gvKeyDisplay;

        //主表数据的获取
        private void fun_getBase()
        {
            SqlDataAdapter da;
            string sql = "";
            sql = "select gckey,gcDesc from Sys_GridControl";
            da = new SqlDataAdapter(sql, strconn);
            dt_gcKeyMain = new DataTable();
            da.Fill(dt_gcKeyMain);
            txt_gcKey.Properties.DataSource = dt_gcKeyMain;
            txt_gcKey.Properties.ValueMember = "gckey";
            txt_gcKey.Properties.DisplayMember = "gckey";
        }

        //描述的更改
        private void txt_gcKey_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dt_gcKeyMain != null)
                {
                    DataRow[] dr = dt_gcKeyMain.Select(string.Format("gcKey='{0}'", txt_gcKey.EditValue.ToString()));
                    if (dr.Length > 0)
                    {
                        txt_gcDesc.Text = dr[0]["gcDesc"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void fmGridControlCustom_Load(object sender, EventArgs e)
        {
            try
            {
                fun_getBase();
                txt_gcKey.EditValue = "";
                txt_authority.EditValue = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //查询dt_authority的信息
        private void fun_searchdtP()
        {
            SqlDataAdapter da;
            string sql = "";
            sql = string.Format("select distinct(authority) from Sys_GridControlField where gcKey='{0}'", txt_gcKey.EditValue.ToString());
            dt_authority = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_authority);

            if (dt_authority.Rows.Count > 1)
            {
                txt_authority.Properties.DataSource = dt_authority;
                txt_authority.Properties.ValueMember = "authority";
                txt_authority.Properties.DisplayMember = "authority";
            }

            if (dt_authority.Rows.Count == 1)
            {
                txt_authority.Properties.DataSource = dt_authority;
                txt_authority.Properties.ValueMember = "authority";
                txt_authority.Properties.DisplayMember = "authority";
                txt_authority.EditValue = dt_authority.Rows[0]["authority"].ToString();
                fun_refreshData();
            }
        }


        private void fun_refreshData()
        {
            SqlDataAdapter da;
            string sql = "";
            sql = string.Format("select * from Sys_GridControlField where gcKey='{0}' and authority='{1}'", txt_gcKey.EditValue.ToString(),txt_authority.EditValue.ToString());
            dt_gvKeyDisplay = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_gvKeyDisplay);
            gridControl1.DataSource = dt_gvKeyDisplay;
        }


        //查询
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dt_gvKeyDisplay != null)
                {
                    dt_gvKeyDisplay.Clear();
                }     
                fun_searchdtP();
                gvKey = txt_gcKey.EditValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //刷新
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                fun_refreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region  保存操作

        private void fun_保存()
        {

            SqlDataAdapter da;
            //string sql = "";
            string sql = string.Format("select * from Sys_GridControl where gcKey='{0}'", txt_gcKey.EditValue.ToString());
            DataTable dt_GridControl = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_GridControl);
            if (dt_GridControl.Rows.Count > 0)
            {
                dt_GridControl.Rows[0]["gcDesc"] = txt_gcDesc.Text;
            }
            sql = "select * from Sys_GridControl where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_GridControl);

            //保存子表
            sql = "select * from Sys_GridControlField where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_gvKeyDisplay);
        }

        //保存
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gvKey == "")
                    throw new Exception("先查询，再保存！");
                gridView1.CloseEditor();
                this.BindingContext[dt_gvKeyDisplay].EndCurrentEdit();
                fun_保存();
                button1_Click(null, null);
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }     
        }

        #endregion


        #region 删除操作：把主表子表数据全部删除

        private void fun_删除()
        {
            SqlDataAdapter da;
            string sql = string.Format("select * from Sys_GridControlLayout where gcKey='{0}'",gvKey);
            DataTable dt_layout = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_layout);
            foreach (DataRow r in dt_layout.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                r.Delete();
            }
            sql = "select * from Sys_GridControlLayout where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_layout);

            sql = string.Format("select * from Sys_GridControlField where gcKey='{0}'", gvKey);
            DataTable dt_Field = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_Field);
            foreach (DataRow r in dt_Field.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                r.Delete();
            }
            sql = "select * from Sys_GridControlField where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_Field);


            sql = string.Format("select * from Sys_GridControl where gcKey='{0}'", gvKey);
            DataTable dt = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);
            foreach (DataRow r in dt.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                r.Delete();
            }
            sql = "select * from Sys_GridControlField where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt);

        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gvKey == "")
                    throw new Exception("先查询，再删除！");
                fun_删除();
                button1_Click(null, null);
                MessageBox.Show("数据删除成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }       
        }

        #endregion

        #region  清空格式

        private void fun_清空格式()
        {
            SqlDataAdapter da;
            string sql = string.Format("select * from Sys_GridControlLayout where gcKey='{0}'", gvKey);
            DataTable dt_layout = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_layout);
            foreach (DataRow r in dt_layout.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                r.Delete();
            }
            sql = "select * from Sys_GridControlLayout where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_layout);
        }


        private void barLargeButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gvKey == "")
                    throw new Exception("请先查询，再清空格式！");
                fun_清空格式();
                button1_Click(null, null);
                MessageBox.Show("格式清空成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region 清空字段


        private void fun_清空字段()
        {
            SqlDataAdapter da;
            string sql = "";
            sql = string.Format("select * from Sys_GridControlField where gcKey='{0}'", gvKey);
            DataTable dt_Field = new DataTable();
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_Field);
            foreach (DataRow r in dt_Field.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                r.Delete();
            }
            sql = "select * from Sys_GridControlField where 1<>1";
            da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_Field);
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gvKey == "")
                    throw new Exception("请先查询，再清空字段！");
                fun_清空字段();
                button1_Click(null, null);
                MessageBox.Show("字段清空成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion
















    }
}
