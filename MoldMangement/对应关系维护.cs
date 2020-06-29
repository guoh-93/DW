using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace MoldMangement
{
    public partial class 对应关系维护 : Form
    {
        public 对应关系维护()
        {
            InitializeComponent();
        }

        public 对应关系维护(DataRow drM)
        {
            InitializeComponent();

            drM_带 = drM;
            textBox1.Text = drM["资产编码"].ToString();
            textBox2.Text=drM["资产名称"].ToString();
           // textBox3.Text = drM["录入人员"].ToString();
          //  dateEdit1.Text = drM["录入时间"].ToString();

        }


        #region 变量

        string conn = CPublic.Var.strConn;
        DataTable dtM;
        DataRow drM_带;
        DataTable dt_物料, dt_保存, dt_工装冶具;




        #endregion


        #region 方法
        private DataTable fun_select(string sql, DataTable dt)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
            {
                dt = new DataTable();
                da.Fill(dt);
            }
            return dt;
        }

        private void fun_delete(string sql)
        {
            using (SqlConnection sqlconn = new SqlConnection(conn))
            {
                sqlconn.Open();
                SqlCommand sqlcommand = new SqlCommand(sql, sqlconn);
                sqlcommand.ExecuteNonQuery();
                sqlconn.Close();
            }
        }


        private void fun_cheek()
        {
            //try
            //{
            DataTable dtTableDisinit = (DataTable)this.gridControl1.DataSource;
            DataView dv = new DataView(dtTableDisinit);
            if (dv.Count != dv.ToTable(true, "物料编码").Rows.Count )
            {
            throw new Exception  ("当前数据重复!");
               // return;
            }
  //} catch (Exception ex)
  //          {
  //              MessageBox.Show(ex.Message);
  //          }


        }




        private void fun_load()
        {

            string sql =string.Format( "select * from 工装治具与产品对应基础信息表 where 资产编号='{0}'",drM_带["资产编码"]);
            dt_工装冶具 = new DataTable();
            dt_工装冶具 = fun_select(sql, dt_工装冶具);
            gridControl1.DataSource = dt_工装冶具;
            textBox3.Text = CPublic.Var.localUserName;
            dateEdit1.Text = CPublic.Var.getDatetime().ToString();
            sql = "select  物料编码,物料名称,规格型号    from 基础数据物料信息表";
            dt_物料 = new DataTable();
            dt_物料 = fun_select(sql, dt_物料);
            repositoryItemSearchLookUpEdit1.DataSource = dt_物料;
            repositoryItemSearchLookUpEdit1.ValueMember = "物料编码";
            repositoryItemSearchLookUpEdit1.DisplayMember = "物料编码";

           // dt_物料.ColumnChanged += dt_物料_ColumnChanged;
        }

        //void dt_物料_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        //{
        //  DataRow   drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
        //     if (e.Column.Caption == "物料名称"&& drM["物料名称"].ToString()!="")
        //    {

        //        DataRow drr = dt_物料.Select(string.Format("物料编码='{0}'", drM["物料编码"]))[0];

        //        drM["物料编码"] = drr["物料名称"].ToString();

            
        //    }



           
        //}


      




        #endregion

        private void 对应关系维护_Load(object sender, EventArgs e)
        {
            fun_load();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_cheek();


                string sql = "select * from 工装治具与产品对应基础信息表 where 1<>1";

                foreach (DataRow dr in dt_工装冶具.Rows)
                {
                    //    dr["资产编号"] = textBox1.Text;
                    if (dr.RowState == DataRowState.Deleted)
                    {

                        continue;
                    }

                    
                    //    dr["资产名称"] = textBox2.Text;
                    //    dr["录入人员"] = textBox3.Text;
                    //    dr["录入时间"] = dateEdit1.Text.ToString();
                    //    dr["物料编号"] = searchLookUpEdit1.EditValue.ToString();
                    //    dr["物料名称"] = searchLookUpEdit1.Text.ToString();
                }

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    new SqlCommandBuilder(da);
                    da.Update(dt_工装冶具);
                    MessageBox.Show("保存成功");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

            //fun_刷新();

            //dt_工装冶具 = new DataTable();


            DataRow dr = dt_工装冶具.NewRow();

            dr["资产编号"] = drM_带["资产编码"].ToString();
            dr["资产名称"] = drM_带["资产名称"].ToString();
            dr["录入人员"] = textBox3.Text;
            dr["录入时间"] = dateEdit1.Text;  
            dt_工装冶具.Rows.Add(dr);
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
            if (e.Column.Caption == "物料编码" && drM["物料编码"].ToString() != "")
            {

                DataRow drr = dt_物料.Select(string.Format("物料编码='{0}'", drM["物料编码"]))[0];

                drM["物料名称"] = drr["物料名称"].ToString();


            }




        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                gridView1.CloseEditor();
                this.BindingContext[dt_工装冶具].EndCurrentEdit();
                if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {

                  DataRow  drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                 // string sql = string.Format("delete  from 工装治具与产品对应基础信息表 where 资产编号='{0}'and 物料编码='{1}' ",drM["资产编号"].ToString(),drM["物料编码"].ToString());

                  drM.Delete();

                 

                 // fun_load();
               //   MessageBox.Show("删除成功");
                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }//删除


        }


    
    
    
    
    
    
    
    
    
    }

