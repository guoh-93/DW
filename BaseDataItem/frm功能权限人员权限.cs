using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BaseData
{
    public partial class frm功能权限人员权限 : UserControl
    {

        #region  变量
        string strcon = CPublic.Var.strConn;
        DataTable dt_代办;
        DataTable dt_权限组;
        DataTable dt_课室;

        #endregion

        #region  加载
        public frm功能权限人员权限()
        {
            InitializeComponent();
        }

        private void frm功能权限人员权限_Load(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Text = "部门";

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region   函数
        private void fun_load()
        {
            string sql = "";
            if (comboBox1.Text == "部门")
            {

                //string sql_课室 = "select * from 人事基础员工表 where  部门名称 like %课%";
                //dt_课室 = new DataTable();
                //dt_课室 = CZMaster.MasterSQL.Get_DataTable(sql_课室, strcon);
                //searchLookUpEdit1.Visible = true;
                //label2.Visible = true;
                //searchLookUpEdit1.Properties.DataSource = dt_课室;
                //searchLookUpEdit1.Properties.ValueMember = "部门编号";
                //searchLookUpEdit1.Properties.DisplayMember = "部门名称";
                //sql = "select 部门编号,部门名称,权限组 from 人事基础部门表  where  部门名称 like '%课%' ";
                //sql = @"select [基础数据基础属性表].属性值,人事基础部门表.部门编号,人事基础部门表.权限组  from 基础数据基础属性表 left join 人事基础部门表 on 人事基础部门表.部门名称 =基础数据基础属性表.属性值 
                //where [基础数据基础属性表].属性类别='课室'";
              
                // sql = @"select  部门编号  ,部门名称 ,权限组  from 人事基础部门表  where LEN(部门编号)=10";

                //19-4-2 revise
                sql = @"select depart.部门编号,部门名称 ,权限组 from 人事基础部门表 depart
                        inner join(select 部门编号, COUNT(员工号) 数量  from 人事基础员工表 group by 部门编号) a on a.部门编号 = depart.部门编号";

            }
            else
            {
                //  searchLookUpEdit1.Visible = false;
                //label2.Visible = false;
                sql = "select 员工号,姓名,权限组 from 人事基础员工表 where 在职状态='在职' ";

            }
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dt_代办 = new DataTable();
                da.Fill(dt_代办);
                gc_代办.DataSource = dt_代办;
            }


            // 
            string sql_权限组 = "select * from 功能权限权限组表 ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_权限组, strcon))
            {
                dt_权限组 = new DataTable();
                da.Fill(dt_权限组);
                dt_权限组.Columns.Add("选择", typeof(bool));
                gc_权限组.DataSource = dt_权限组;

            }

        }
        private DataSet fun_save()
        {
            DataSet ds = new DataSet();
            string str_权限组 = "";
            foreach (DataRow rr in dt_权限组.Rows)
            {
                if (rr["选择"].Equals(true))
                {
                    str_权限组 = rr["权限组"].ToString();
                    break;
                }
            }
            DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);

            if (comboBox1.Text == "部门")
            {
                //给部门表中 权限组 赋值    
                string sql_部门 = string.Format("select * from 人事基础部门表  where  部门编号='{0}'", dr["部门编号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_部门, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            r["权限组"] = str_权限组;
                        }
                    }
                    ds.Tables.Add(dt);
                }
                //相应的这个部门的 所有人 也要相同值
                string sql_人员 = string.Format("select * from 人事基础员工表 where 部门编号='{0}'", dr["部门编号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_人员, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow r in dt.Rows)
                    {
                        r["权限组"] = str_权限组;
                    }
                    ds.Tables.Add(dt);
                }

                
             
            }
            else   // comBox1.text =="人员"
            {
              
                string sql_人员 = string.Format("select * from 人事基础员工表 where 在职状态='在职' and 员工号='{0}' ", dr["员工号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_人员, strcon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow r in dt.Rows)
                    {
                        r["权限组"] = str_权限组;
                    }
                    ds.Tables.Add(dt);
                }
            }
            return ds;
        }

       
        #endregion


        #region 界面操作
        //选择部门或人员
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {

            gv_代办.Columns.Clear();
            fun_load();
          

        }
        //点击代办 权限组 中 对应权限组打钩  gc_权限显示对应权限
        private void gv_代办_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gv_代办.GetDataRow(gv_代办.FocusedRowHandle);

            string sql_权限组 = "select * from 功能权限权限组表 ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_权限组, strcon))
            {
                dt_权限组 = new DataTable();
                da.Fill(dt_权限组);
                dt_权限组.Columns.Add("选择", typeof(bool));
                gc_权限组.DataSource = dt_权限组;

            }

            DataRow[] r = dt_权限组.Select(string.Format("权限组='{0}'", dr["权限组"].ToString()));
            if (r.Length > 0)
            {
                r[0]["选择"] = true;
            }
            //foreach (DataRow r in dt_权限组.Rows)
            //{
            //    if (r["权限组"].ToString() == dr["权限组"].ToString())
            //    {
            //        r["选择"] = true;
            //    }
            //}
            gv_权限组.CloseEditor();
            this.BindingContext[dt_权限组].EndCurrentEdit();

        }
        //权限组 打钩 变化 后面gc_权限 显示对应权限
        private void repositoryItemCheckEdit1_EditValueChanged(object sender, EventArgs e)
        {
            gv_权限组.CloseEditor();
            this.BindingContext[dt_权限组].EndCurrentEdit();
            DataTable dt = new DataTable();
            DataRow dr = gv_权限组.GetDataRow(gv_权限组.FocusedRowHandle);

            foreach (DataRow r in dt_权限组.Rows)
            {
                if (r["权限组"].ToString() != dr["权限组"].ToString())
                {
                    r["选择"] = false;
                }

            }


            if (dr["选择"].Equals(true))
            {
                string sql = string.Format("select * from 功能权限权限组权限表 where 权限组='{0}' and 权限值=1 order by 上级权限", dr["权限组"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {

                    da.Fill(dt);
                   
                    gc_权限.DataSource = dt;
                }
            }
            else
            {
                string sql = "select * from 功能权限权限组权限表 where 1<>1";

                dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                gc_权限.DataSource = dt;

            }
        }
        //保存 
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("确定保存？", "确认", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                gv_权限组.CloseEditor();
                this.BindingContext[dt_权限组].EndCurrentEdit();

                DataSet ds = fun_save();

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("权限");


                string sql_人员 = "select * from 人事基础员工表 where 1<>1";
                string sql_部门 = "select * from 人事基础部门表 where 1<>1";

                try
                {

                    if (comboBox1.Text == "课室")
                    {

                        SqlCommand cmm_1 = new SqlCommand(sql_人员, conn, ts);
                        SqlCommand cmm_2 = new SqlCommand(sql_部门, conn, ts);
                        SqlDataAdapter da_人员 = new SqlDataAdapter(cmm_1);
                        SqlDataAdapter da_部门 = new SqlDataAdapter(cmm_2);
                        new SqlCommandBuilder(da_人员);
                        new SqlCommandBuilder(da_部门);
                        da_部门.Update(ds.Tables[0]);
                        da_人员.Update(ds.Tables[1]);

                    }
                    else
                    {
                        SqlCommand cmm_1 = new SqlCommand(sql_人员, conn, ts);
                        SqlDataAdapter da_人员 = new SqlDataAdapter(cmm_1);
                        new SqlCommandBuilder(da_人员);
                        da_人员.Update(ds.Tables[0]);
                    }
                    ts.Commit();
                    MessageBox.Show("保存成功");

                }                                    
                catch (Exception ex)
                {
                    ts.Rollback();

                    MessageBox.Show("权限设置失败");
                }
                barLargeButtonItem1_ItemClick(null, null);
            }

        }
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv_代办.Columns.Clear();
                fun_load();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //关闭
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }


        #endregion

        private void gv_权限组_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            gv_权限组.CloseEditor();
            this.BindingContext[dt_权限组].EndCurrentEdit();
            DataTable dt = new DataTable();
            DataRow dr = gv_权限组.GetDataRow(gv_权限组.FocusedRowHandle);

            foreach (DataRow r in dt_权限组.Rows)
            {
                if (r["权限组"].ToString() != dr["权限组"].ToString())
                {
                    r["选择"] = false;
                }

            }



                string sql = string.Format("select * from 功能权限权限组权限表 where 权限组='{0}' and 权限值=1  order by 上级权限", dr["权限组"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {

                    da.Fill(dt);

                    gc_权限.DataSource = dt;
                }
         
           
        }

    }
}
