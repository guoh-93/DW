using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPStock
{
    public partial class frm初始化UI : UserControl
    {
        #region 变量
        string strconn = CPublic.Var.strConn;
        DataTable dt = new DataTable();   //视图
        DataTable dt1 = new DataTable();        //    仓库主表
        DataTable dt2 = new DataTable();         //    数量
        DataTable dt3 = new DataTable();
        DataView dv;
        int check;

        #endregion

        #region 加载
        public frm初始化UI()
        {
            InitializeComponent();
        }
        private void frm初始化UI_Load(object sender, EventArgs e)
        {
            fun_load();
        }
        #endregion

        #region 函数
        void fun_load()
        {
            try
            {
                string sql = "select 物料编码,物料名称,是否初始化 from 基础数据物料信息表 where 是否初始化='否' ";
                dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {

                    da.Fill(dt);
                    dt.Columns.Add("请选择", typeof(bool));
                    dt.Columns.Add("库存总数");
                    dt.Columns.Add("仓库名称");
                    dt.Columns.Add("仓库号");
                    //dtM.Columns.Add("库位号");
                    dv = new DataView(dt);
                    gridControl1.DataSource = dv;

                }
                string sql1 = "select 属性值,属性字段1 FROM [FMS].[dbo].[基础数据基础属性表] where 属性类别='仓库类别' ";
                using (SqlDataAdapter da = new SqlDataAdapter(sql1, strconn))
                {
                    da.Fill(dt1);
                }

                //string sql2 = "select 物料编码,库存总数 from  仓库物料数量表";
                //using (SqlDataAdapter da = new SqlDataAdapter(sql2, strconn))
                //{
                //    da.Fill(dt2);
                //    foreach (DataRow dr in dtM.Rows)         // 视图
                //        foreach (DataRow drr in dt2.Rows)     //数量表
                //        {
                //            if (dr["物料编码"] == drr["物料编码"])
                //            {
                //                dr["库存总数"] = drr["库存总数"];
                //            }
                //        }
                //}

                //string sql3 = "select 库位号 from 基础数据仓库库位表";
                //using (SqlDataAdapter da = new SqlDataAdapter(sql3, strconn))
                //{
                //    da.Fill(dt3);

                //}
                //" 基础数据仓库库位表 "
                repositoryItemSearchLookUpEdit1.DataSource = dt1;
                repositoryItemSearchLookUpEdit1.DisplayMember = "属性值";
                repositoryItemSearchLookUpEdit1.ValueMember = "属性值";

                //repositoryItemSearchLookUpEdit2.DataSource = dt3;
                //repositoryItemSearchLookUpEdit2.DisplayMember = "库位号";
                //repositoryItemSearchLookUpEdit2.ValueMember = "库位号";
                //dtM.ColumnChanged += dt_ColumnChanged;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            gridView1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();
            try
            {

                if (e.Column.Caption == "仓库名称")
                {
                    if (e.ProposedValue.ToString() != "")
                    {
                        DataRow[] dr = dt1.Select(string.Format("属性值='{0}'", e.ProposedValue));
                        e.Row["仓库号"] = dr[0]["属性字段1"];



                        DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);

                        //if (r["仓库名称"].ToString() == "")
                        //{
                        //    dt3.Clear();
                        //}
                        //else
                        //{
                        //    dt3.Clear();
                        //    string sql3 = string.Format("select 库位号 from 基础数据仓库库位表 where 仓库名称='{0}'", r["仓库名称"]);
                        //    using (SqlDataAdapter da = new SqlDataAdapter(sql3, strconn))
                        //    {
                        //        da.Fill(dt3);
                        //    }
                        //}
                    }
                    else
                    {
                        e.Row["仓库号"] = "";
                        //e.Row["库位号"] = "";
                        //dt3.Clear();
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        private void repositoryItemSearchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            gridView1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();
            DataRow rr = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            try
            {


                if (rr["仓库名称"] != DBNull.Value && rr["仓库名称"] != null)
                {
                    DataRow[] dr = dt1.Select(string.Format("属性值='{0}'", rr["仓库名称"]));
                    rr["仓库号"] = dr[0]["属性字段1"];



                    DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);

                    //if (r["仓库名称"].ToString() == "")
                    //{
                    //    dt3.Clear();
                    //}
                    //else
                    //{
                    //    dt3.Clear();
                    //    string sql3 = string.Format("select 库位号 from 基础数据仓库库位表 where 仓库名称='{0}'", r["仓库名称"]);
                    //    using (SqlDataAdapter da = new SqlDataAdapter(sql3, strconn))
                    //    {
                    //        da.Fill(dt3);
                    //    }
                    //}
                }
                else
                {
                    rr["仓库号"] = "";
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        void fun_check()
        {
            gridView1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();

            //foreach (DataRow dr in dtM.Rows)
            //{
            //    if (dr["请选择"].Equals(true))
            //    {

            //        if (dr["仓库名称"].ToString() == "")
            //        {

            //            throw new Exception("仓库名称不能为空");

            //        }
            //        else if (dr["库位号"].ToString() == "")
            //        {

            //            throw new Exception("库位号不能为空");

            //        }

            //    }

            //}

        }
        #endregion


        #region 界面操作
        //初始化按钮
        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt].EndCurrentEdit();
                fun_check();

                string sql = "select 物料编码,是否初始化 from 基础数据物料信息表 ";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    //DataTable dt_1 = new DataTable();
                    //da.Fill(dt_1);
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["请选择"].Equals(true) && dr["是否初始化"].ToString().Trim() == "否")
                        {
                            dr["是否初始化"] = "是";

                            //string a = dr["库位号"].ToString();
                            string b = dr["物料编码"].ToString();
                            decimal c = 0;
                            try
                            {
                                c = Convert.ToDecimal(dr["库存总数"]);
                            }
                            catch
                            {
                                c = 0;
                            }
                            gridView1.CloseEditor();
                            this.BindingContext[dt].EndCurrentEdit();
                            //StockCore.StockCorer.fun_Init初始化仓库物料("", b, c);
                            //new SqlCommandBuilder(da);
                            //da.Update(dtM);

                        }
                    }

                    new SqlCommandBuilder(da);
                    da.Update(dt);
                    MessageBox.Show("初始化成功");
                    fun_load();
                    gridView1.FocusedRowHandle = gridView1.FocusedRowHandle + 1;
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        // 关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            ////System.Windows.Forms.Form fm = null;
            ////while (this.ParentForm != null)
            ////{
            ////    fm.Close();
            ////}

            gridView1.CloseEditor();
            this.BindingContext[dt].EndCurrentEdit();//CPublic.UIcontrol.ClosePage();
            this.ParentForm.Close();
        }

        private void repositoryItemSearchLookUpEdit2_Popup(object sender, EventArgs e)
        {

            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            if (r["仓库名称"].ToString() == "")
            {
                dt3.Clear();
            }
            else
            {
                dt3.Clear();
                string sql3 = string.Format("select 库位号 from 基础数据仓库库位表 where 仓库名称='{0}'", r["仓库名称"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql3, strconn))
                {
                    da.Fill(dt3);
                }
            }
        }
        # endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataRow r in dt.Rows)
            {
                r["请选择"] = true;
                //r["库位号"] = "";
                r["库存总数"] = 0;
            }
        }


        //批量初始化

    }
}
