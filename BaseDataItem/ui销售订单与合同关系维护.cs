using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace BaseData
{
    public partial class ui销售订单与合同关系维护 : UserControl
    {
        public ui销售订单与合同关系维护()
        {
            InitializeComponent();
        }

        public ui销售订单与合同关系维护(DataRow drrr)
        {
            InitializeComponent();
            dr_jump = drrr;

        }

        #region 成员
        DataTable dtM, dt_客户;
        DataRow dr_jump;
        DataRow dr_fasu;
        DataTable dt_销售;//所有销售订单
        //主表
        string strconn = CPublic.Var.strConn;

 
        #endregion

        private void ui销售订单与合同关系维护_Load(object sender, EventArgs e)
        {

             dt_销售 = new DataTable();
             string sql = "select dDate 日期,cSOCode 销售订单号,cCusCode 客户编码 from [192.168.20.150].UFDATA_008_2018.dbo.SO_SOMain  where  iStatus=1 and cCloser is null ";
            dt_销售 = CZMaster.MasterSQL.Get_DataTable(sql,strconn);

            if (dr_jump != null)
            {
                label1.Text = "当前合同名称：" + dr_jump["合同名称"].ToString() + "," + "当前合同号：" + dr_jump["合同号"].ToString();

            }

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {        
            DataTable dt2 = (DataTable)gridControl1.DataSource;
            gridView1.CloseEditor();
            this.BindingContext[dt2].EndCurrentEdit();
            string sql2 = string.Format("select * from 销售订单与合同对应关系表 where 合同号='{0}' ",dr_jump["合同号"].ToString());
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql2, strconn);

            string sql = string.Format("select * from 销售订单与合同对应关系表 where 1<>1 ");
            DataTable dt_辅数据 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);


            foreach (DataRow dr in dt2.Rows)
            {
                if (Convert.ToBoolean(dr["选择"].ToString()) == true)
                {
                    DataRow dr_dt = dt_辅数据.NewRow();
                    dt_辅数据.Rows.Add(dr_dt);
                    dr_dt["销售单号"] = dr["销售订单号"].ToString();
                    dr_dt["合同号"] = dr_jump["合同号"].ToString();

                }

            }///////dt_辅数据 所有数据

            DataTable dt_same = new DataTable();
            dt_same = CZMaster.MasterSQL.Get_DataTable(sql, strconn);////dt_same 共同数据
   
               foreach(DataRow drt in dt.Rows )
               {
                  
               foreach(DataRow dr_fu  in dt_辅数据.Rows   )
                {
              
                 if (drt["销售单号"].ToString() == dr_fu["销售单号"].ToString() && drt["合同号"].ToString() == dr_fu["合同号"].ToString())
                 {
                     DataRow dr_same = dt_same.NewRow();
                     dr_same = drt;
                     dt_same.ImportRow(dr_same);
                   
                 }                        
               }      
            }

               bool a = false;
               /////////////  a 状态代表删除行

               if (dt_same.Rows.Count > 0)
               {

                   for (int i = dt_辅数据.Rows.Count-1; i >=0; i--)
                   {
                       DataRow dr_fasu = null;
                  
                           dr_fasu = dt_辅数据.Rows[i];
                  
                       string 销售单号 = dr_fasu["销售单号"].ToString();
                       string 合同号 = dr_fasu["合同号"].ToString();

                       for (int j = 0; j < dt_same.Rows.Count; j++)
                       {
                           if (dr_fasu.RowState == DataRowState.Deleted)
                           {

                               continue;
                           }


                           DataRow dr_same = dt_same.Rows[j];
                           if (销售单号 == dr_same["销售单号"].ToString() && 合同号 == dr_same["合同号"].ToString())
                           {
                               dr_fasu.Delete();
                              // a = true;
                           }
                       }

                   }

               }
                

               //bool a = false;
               /////////////  a 状态代表删除行

               //if (dt_same.Rows.Count > 0)
               //{
               //    int k = 0;
               //    k = dt_辅数据.Rows.Count;
               //    for (int i = 0; i < k; i++)
               //    {
               //        DataRow dr_fasu = null;
               //        if (a)
               //        {
               //            dr_fasu = dt_辅数据.Rows[i - 1];
               //            a = false;
               //        }
               //        else
               //        {
               //            dr_fasu = dt_辅数据.Rows[i];
               //            a = false;
               //        }

               //        string 销售单号 = dr_fasu["销售单号"].ToString();
               //        string 合同号 = dr_fasu["合同号"].ToString();

               //        for (int j = 0; j < dt_same.Rows.Count; j++)
               //        {
               //            DataRow dr_same = dt_same.Rows[j];
               //            if (销售单号 == dr_same["销售单号"].ToString() && 合同号 == dr_same["合同号"].ToString())
               //            {
               //                dr_fasu.Delete();
               //                a = true;
               //            }
               //        }

               //    }

               //}
                           
                SqlDataAdapter daa = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(daa);
                daa.Update(dt_辅数据);
                dt_辅数据.AcceptChanges();
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }//baocun 


        public void fun_客户编号()
        {
            dt_客户 = new DataTable();
            string sql = string.Format("SELECT *  FROM 客户基础信息表 ");
            //string strconn2 = "Persist Security Info=True;User ID=sa;Password=a;Initial Catalog=asasasas;Data Source=.";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                try
                {
                    da.Fill(dt_客户);
                    searchLookUpEdit1.Properties.DataSource = dt_客户;
                    searchLookUpEdit1.Properties.ValueMember = "客户编号";
                    searchLookUpEdit1.Properties.DisplayMember = "客户名称";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

   
        private void simpleButton1_Click(object sender, EventArgs e)
        {

            try
            {  
                DataTable dt = new DataTable();
                string sql = "select dDate 日期,cSOCode 销售订单号,cCusCode 客户编码  from [192.168.20.150].UFDATA_008_2018.dbo.SO_SOMain  where  iStatus=1 and cCloser is null ";
              //  dt_销售 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
   
                if (checkBox2.Checked == true)
                {
                    sql = sql + string.Format(" and dDate > '{0}'  and dDate<'{1}' ", date_前.Text.ToString(), date_后.Text.ToString());
                }

                if (checkBox1.Checked == true)
                {
                    sql = sql + string.Format(" and cCusCode = '{0}'", searchLookUpEdit1.EditValue.ToString());
                }
          
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    da.Fill(dt);
                    DataColumn dc = new DataColumn("选择", typeof(bool));
       dc.DefaultValue = false;
        dt.Columns.Add(dc);
                    //dt.Columns.Add("选择",typeof(Boolean));
                }
                string sqwl = string.Format("select * from  销售订单与合同对应关系表 where 合同号='{0}' ", dr_jump["合同号"]);
                DataTable dt_已有数据 = CZMaster.MasterSQL.Get_DataTable(sqwl, strconn);
                if(dt_已有数据.Rows.Count>0 ){
                    foreach (DataRow dr in dt_已有数据.Rows)
                    {
                        foreach(DataRow drr  in  dt.Rows ){
                            if (dr["销售单号"].ToString() == drr["销售订单号"].ToString())
                            {
                                drr["选择"] = true;
                            }                           
                        }                              
                    }
                }
                gridControl1.DataSource = dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {

                    DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                    string sql = string.Format("select * from 销售订单与合同对应关系表 where 合同号='{0}' and 销售单号='{1}' ", dr_jump["合同号"].ToString(), drM["销售订单号"].ToString());
                    DataTable dt_remove = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_remove.Rows.Count > 0)
                    {
                        DataRow dr = dt_remove.Rows[0];
                        dr.Delete();
                        SqlDataAdapter daa = new SqlDataAdapter(sql, strconn);
                        new SqlCommandBuilder(daa);
                        daa.Update(dt_remove);
                        dt_remove.AcceptChanges();
                        MessageBox.Show("删除成功！");
                        simpleButton1_Click(null, null);

                    }
                    else
                    {
                        throw new Exception("当前行为保存，不可删除");
                    }
                }
           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            simpleButton1_Click(null,null);
        }

    }
}
