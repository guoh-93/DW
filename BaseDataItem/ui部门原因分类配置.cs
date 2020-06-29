using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace BaseData
{
    public partial class ui部门原因分类配置 : UserControl
    {

        #region  变量
        string strcon = CPublic.Var.strConn;
        DataTable dt_代办;
        DataTable dt_权限组;
        DataTable dt_课室;
        #endregion

        public ui部门原因分类配置()
        {
            InitializeComponent();
        }

        private void fun_load()
        {
            string sql = "";

            sql = "  select  部门编号,部门 from 人事基础员工表 where 在职状态='在职' and 部门编号 <>'' group by  部门编号,部门 ";


            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dt_代办 = new DataTable();
                da.Fill(dt_代办);
                gc_代办.DataSource = dt_代办;
            }

            string sql_权限组 = "  select  属性值 as 原因分类 from 基础数据基础属性表  where 属性类别='原因分类' ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_权限组, strcon))
            {
                dt_权限组 = new DataTable();
                da.Fill(dt_权限组);
                dt_权限组.Columns.Add("选择", typeof(bool));
                gc_权限组.DataSource = dt_权限组;

            }

        }

        private void fun_save(string s_部门编号)
        {
            string sql_权限组 = string.Format("select * from  部门原因分类配置表 where 部门编号='{0}'", s_部门编号);
            DataTable dt_save = CZMaster.MasterSQL.Get_DataTable(sql_权限组, strcon);
            foreach (DataRow r in dt_权限组.Rows)
            {
                DataRow[] ir = dt_save.Select(string.Format("原因分类='{0}'", r["原因分类"]));

                if (Convert.ToBoolean(r["选择"]))
                {
                    if (ir.Length == 0)
                    {
                        DataRow add = dt_save.NewRow();
                        add["部门编号"] = s_部门编号;
                        add["原因分类"] = r["原因分类"];
                        dt_save.Rows.Add(add);
                    }
                }
                else
                {
                    if (ir.Length > 0)
                    {
                        ir[0].Delete();
                    }
                }
            }
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("权限");



            try
            {

                SqlCommand cmm = new SqlCommand(sql_权限组, conn, ts);

                SqlDataAdapter da = new SqlDataAdapter(cmm);

                new SqlCommandBuilder(da);

                da.Update(dt_save);

                ts.Commit();
                MessageBox.Show("保存成功");

            }
            catch (Exception ex)
            {
                ts.Rollback();

                MessageBox.Show("设置失败");
            }

        }

        private void gv_代办_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);

            string sql_权限组 = string.Format("select * from  部门原因分类配置表 where 部门编号='{0}'", dr["部门编号"]);
            using (SqlDataAdapter da = new SqlDataAdapter(sql_权限组, strcon))
            {

                DataTable temp = new DataTable();
                da.Fill(temp);
                foreach (DataRow rr in dt_权限组.Rows)
                {
                    DataRow[] tr = temp.Select(string.Format("原因分类='{0}'", rr["原因分类"]));
                    if (tr.Length > 0)
                    {
                        rr["选择"] = true;

                    }
                    else
                    {
                        rr["选择"] = false;

                    }
                }
            }


        }

        private void ui部门原因分类配置_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
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

        private void gv_代办_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {

            
            DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);

            string sql_权限组 = string.Format("select * from  部门原因分类配置表 where 部门编号='{0}'", dr["部门编号"]);
            using (SqlDataAdapter da = new SqlDataAdapter(sql_权限组, strcon))
            {

                DataTable temp = new DataTable();
                da.Fill(temp);
                foreach (DataRow rr in dt_权限组.Rows)
                {
                    DataRow[] tr = temp.Select(string.Format("原因分类='{0}'", rr["原因分类"]));
                    if (tr.Length > 0)
                    {
                        rr["选择"] = true;

                    }
                    else
                    {
                        rr["选择"] = false;

                    }
                }
            }
            }
            catch 
            {
 
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);
                fun_save(dr["部门编号"].ToString());
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
