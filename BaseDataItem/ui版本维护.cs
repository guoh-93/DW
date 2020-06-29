using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace BaseData
{
    public partial class ui版本维护 : UserControl
    {

        public static DevExpress.XtraTab.XtraTabControl XTC;
        public ui版本维护(DataRow dr)
        {
            InitializeComponent();
            drM_带 = dr;
            textBox1.Text = dr["物料编码"].ToString();
            textBox2.Text = dr["物料名称"].ToString();
            textBox3.Text = dr["规格型号"].ToString();
        }
        /// <summary>
        /// 2019-9-26 加 用来仓库物料信息跳转后 嵌入 x=0
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="x">以后有用可扩展</param>
        /// 
        public ui版本维护(DataRow dr,int x)
        {
            InitializeComponent();
            drM_带 = dr;
            textBox1.Text = dr["物料编码"].ToString();
            textBox2.Text = dr["物料名称"].ToString();
            textBox3.Text = dr["规格型号"].ToString();

            bar2.Visible = false;
            gridView1.OptionsBehavior.Editable = false;
        }

        #region 变量
        DataTable dtM;
        DataRow drM_带;
        DataTable dt_版本;
        DataRow dr;
        string strcon = CPublic.Var.strConn;
        #endregion

        #region 方法
        private void fun_加载()
        {
            string sql = string.Format("select * from 程序版本维护表 where 物料号='{0}'", drM_带["物料编码"]);
            dt_版本 = new DataTable();
            dt_版本 = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
            gridControl1.DataSource = dt_版本;


        }


        #endregion

        private void ui版本维护_Load(object sender, EventArgs e)
        {

            fun_加载();

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                dr = dt_版本.NewRow();
                dr["修改时间"] = CPublic.Var.getDatetime();
                dr["修改人"] = CPublic.Var.localUserName;
                dt_版本.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }//添加

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
      
       
              try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);


                //if (dr.RowState == DataRowState.Unchanged)
                //{
                //    MessageBox.Show("改行已保存，不可删除");
                //    return;
                //}



                if (MessageBox.Show(string.Format("是否确认删除版本:{0}？", dr["版本"].ToString()), "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                
                    dr.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }//删除

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView1.CloseEditor();
            this.BindingContext[dt_版本].EndCurrentEdit();

            try
            {
                int a = 0;
                foreach (DataRow r  in dt_版本.Rows)
                {

                    if (r["版本"].ToString()=="")
                    {
                        throw new Exception("请输入版本");

                    }

                    if (r.RowState == DataRowState.Deleted)
                    {

                        continue;
                    }
                  
                        a++;

                   

                }


                if (dt_版本.DefaultView.ToTable(true, "版本").Rows.Count < a)
                {
                    MessageBox.Show("版本号有重复");
                    return;
                }


                foreach (DataRow dr in dt_版本.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    if (dr.RowState == DataRowState.Modified)
                    {
                        dr["修改时间"] = CPublic.Var.getDatetime();
                    }

                    dr["物料号"] = textBox1.Text.ToString();
                    dr["物料名称"] = textBox2.Text.ToString();
                    dr["规格型号"] = textBox3.Text.ToString();
                    
                }


                SqlDataAdapter da = new SqlDataAdapter();
                da = new SqlDataAdapter("select * from 程序版本维护表 where 1<>1", strcon);
                new SqlCommandBuilder(da);
                da.Update(dt_版本);
                MessageBox.Show("保存成功");

                ui版本维护_Load(null,null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }//保存

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (XTC.TabPages.Count == 1) { }
            if (XTC.SelectedTabPage.Text == "首页") { }
            DevExpress.XtraTab.XtraTabPage xtp = null;
            try
            {
                xtp = XTC.SelectedTabPage;
                XTC.SelectedTabPageIndex = XTC.SelectedTabPageIndex - 1;
            }
            catch { }
            try
            {
                xtp.Controls[0].Dispose();
                XTC.TabPages.Remove(xtp);
                xtp.Dispose();
            }
            catch { }

        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try

            {
                gridView1.CloseEditor();
                this.BindingContext[dt_版本].EndCurrentEdit();


                DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;

                string sql = string.Format("select * from  程序版本维护表  where 文件名='{0}' ", drM["文件名"].ToString());
                DataTable dt_bb = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt_bb.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt_bb.Rows)
                    {
                        dr["停用"] = true;
                        dr["修改人"] = CPublic.Var.localUserName;
                        dr["修改时间"] = CPublic.Var.getDatetime();

                    }
                    SqlDataAdapter da = new SqlDataAdapter();
                    da = new SqlDataAdapter("select * from 程序版本维护表 where 1<>1", strcon);
                    new SqlCommandBuilder(da);
                    da.Update(dt_bb);
                    MessageBox.Show("保存成功");
                    ui版本维护_Load(null, null);
                }
                else

                {

                    throw new Exception("请先保存");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {
                gridView1.CloseEditor();
                this.BindingContext[dt_版本].EndCurrentEdit();


                DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;

                string sql = string.Format("select * from  程序版本维护表  where 文件名='{0}' ", drM["文件名"].ToString());
                DataTable dt_bb = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt_bb.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt_bb.Rows)
                    {
                        dr["停用"] = false;
                        dr["修改时间"] = CPublic.Var.getDatetime();
                        dr["修改人"] = CPublic.Var.localUserName;
                  
                    }


                    SqlDataAdapter da = new SqlDataAdapter();
                    da = new SqlDataAdapter("select * from 程序版本维护表 where 1<>1", strcon);
                    new SqlCommandBuilder(da);
                    da.Update(dt_bb);
                    MessageBox.Show("保存成功");

                    ui版本维护_Load(null, null);



                }
                else

                {

                    throw new Exception("请先保存");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
