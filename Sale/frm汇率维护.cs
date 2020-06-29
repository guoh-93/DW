using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CPublic;
using CZMaster;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.SqlClient;
using DevExpress.XtraPrinting;


namespace ERPSale
{
    public partial class frm汇率维护 : UserControl
    {
        public frm汇率维护()
        {
            InitializeComponent();
        }

        DataTable dt_汇率表;
        DataTable dtM;
        DataRow  drM;
        string strConn = CPublic.Var.strConn;
        

        private void frm汇率维护_Load(object sender, EventArgs e)
        {

            fun_show();
       


        }






        private void fun_show()
        {


            this.repositoryItemDateEdit3.NullText = DateTime.Today.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM");
            barEditItem1.EditValue = DateTime.Today.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM");
            string time = (barEditItem1.EditValue).ToString();
            DateTime time_shijia = Convert.ToDateTime(time);
            string time1 = time_shijia.Year.ToString();
            string time2 = time_shijia.Month.ToString();
            string sql = string.Format("select * from 汇率维护表 where 年='{0}'and 月='{1}'", time1, time2);
          //  string sql = "select * from 汇率维护表 where 维护日期="+time;
            dt_汇率表 = new DataTable();

            using (SqlDataAdapter da=new SqlDataAdapter(sql,strConn))
            {

                da.Fill(dt_汇率表);
             
            
            }


            gridControl1.DataSource = dt_汇率表;

            string sql1 = "select * from 基础数据基础属性表 where 属性类别='币种' order by 属性类别,POS";
            dtM = new DataTable();
            dtM = MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
            foreach (DataRow r in dtM.Rows)
            {
                if (r["属性类别"].ToString().Equals("币种"))   
                {
                    repositoryItemComboBox1.Properties.Items.Add(r["属性值"].ToString());
                }
            }


            int i = 0;
          if (dt_汇率表.Rows.Count == 0)
            {

                foreach(DataRow dr in dtM.Rows   ){
                    if (dr["属性类别"].ToString().Equals("币种"))
                    {
                       drM= dt_汇率表.NewRow();

                        drM["币种"]=dr["属性值"];
                        string date = DateTime.Now.Date.ToString();
                        DateTime NowTime = Convert.ToDateTime(date);

                        date = NowTime.ToString("yyyy/MM");

                        drM["维护日期"] = date;
                        drM["生效"] = "false";
                        drM["年"] = time1;
                        drM["月"] = time2;
                        drM["POS"] = i++;
                        dt_汇率表.Rows.Add(drM);

                    }                            
                }
            }             
        
        }//加载


        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {




            drM = dt_汇率表.NewRow();
            string date = DateTime.Now.Date.ToString();
            DateTime NowTime = Convert.ToDateTime(date);

            date = NowTime.ToString("yyyy/MM");
            drM["生效"] ="false";
            drM["维护日期"] = date;

            dt_汇率表.Rows.Add(drM);



        }//新增

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
          DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                if ( Convert.ToBoolean(drM["生效"].ToString()) == true)
               {
                    MessageBox.Show("该行已生效不可删除");

               }

                if(Convert.ToBoolean(drM["生效"].ToString()) == false)
                {
                 drM.Delete();
                }
              
       }//删除


       
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

          

            try
            {



                drM = gridView1.GetDataRow(gridView1.FocusedRowHandle);
              
               fun_保存();
           
            }
            catch(Exception  ex)
            {
                MessageBox.Show( ex.Message);

            }

         

        }//保存

        
        
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }//关闭

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
            foreach (DataRow dr in dt_汇率表.Rows)
            {
                dr["生效"] = true;

            }


            drM["生效"] = true;
            fun_保存();
          

        }//生效

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    
                    DevExpress.XtraPrinting.XlsxExportOptions options = new XlsxExportOptions(TextExportMode.Text, false, false);
                    gridControl1.ExportToXlsx(saveFileDialog.FileName,options);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            
        }//导出

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
         
            fun_刷新();
            fun_半加载();

        }//刷新




        #region 方法

        private void fun_半加载()
        {

             string time = (barEditItem1.EditValue).ToString();
            DateTime time_shijia = Convert.ToDateTime(time);
            string time1 = time_shijia.Year.ToString();
            string time2 = time_shijia.Month.ToString();
            string sql = string.Format("select * from 汇率维护表 where 年='{0}'and 月='{1}'", time1, time2);
          //  string sql = "select * from 汇率维护表 where 维护日期="+time;
            dt_汇率表 = new DataTable();

            using (SqlDataAdapter da=new SqlDataAdapter(sql,strConn))
            {

                da.Fill(dt_汇率表);
             
            }


            gridControl1.DataSource = dt_汇率表;

            string sql1 = "select * from 基础数据基础属性表 order by 属性类别,POS";
            dtM = new DataTable();
            dtM = MasterSQL.Get_DataTable(sql1, CPublic.Var.strConn);
       

           // int i = 0;
                
            if (dt_汇率表.Rows.Count == 0)
            {

                foreach (DataRow dr in dtM.Rows)
                {
                    if (dr["属性类别"].ToString().Equals("币种"))
                    {
                        drM = dt_汇率表.NewRow();

                        drM["币种"] = dr["属性值"];
                        string date = DateTime.Now.Date.ToString();
                        DateTime NowTime = Convert.ToDateTime(date);

                        date = NowTime.ToString("yyyy/MM");

                        drM["维护日期"] = date;
                        drM["年"] = time1;
                        drM["月"] = time2;
                       // drM["POS"] = i++;
                        dt_汇率表.Rows.Add(drM);
                        
                    }
                }
                gridControl1.DataSource = dt_汇率表;
            }

        }

     
        private void fun_保存()
        {
            gridView1.CloseEditor();
            this.BindingContext[dt_汇率表].EndCurrentEdit();
            int i = 0;
            foreach(DataRow dr in dt_汇率表.Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                {
                     continue;
                }
                //fun_check();
                if (dr["币种"].ToString() == "")
                {
                    throw new Exception("币种不为空，请重新检查");
                    
                }
              try
                {
                    double i1 = Convert.ToDouble(dr["汇率"]);

                }
                catch
                {

                    throw new Exception("汇率只能输入数字，请检查！");
                   
                }
                if (Convert.ToDouble(dr["汇率"]) < 0)
                {
                    throw new Exception("汇率不可为负数");
                    
                 }

               DateTime dy = DateTime.Parse(barEditItem1.EditValue.ToString());

                dr["年"] = dy.Year.ToString();
                dr["月"] = dy.Month.ToString();
                dr["POS"] = i++;
             }
            
          
            SqlConnection strconn = new SqlConnection(strConn);
            strconn.Open();
            SqlTransaction ts1 = strconn.BeginTransaction("pur"); //事务的名称
            SqlCommand cmd1 = new SqlCommand("select * from 汇率维护表 where 1<>1", strconn, ts1);
           
            try
            {
                SqlDataAdapter da;
                da = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da);
                da.Update(dt_汇率表);
         
                MessageBox.Show("保存成功");

                ts1.Commit();
            }
            catch
            {
                ts1.Rollback();
            }



        }




        private void fun_刷新()
        {

            string time = (barEditItem1.EditValue).ToString();

          //string a=  time.Substring(0,6);

          DateTime time_shijia = Convert.ToDateTime(time);
          string time1 = time_shijia.Year.ToString();
          string time2 = time_shijia.Month.ToString();



          string sql = string.Format("select * from 汇率维护表 where 年='{0}'and 月='{1}'", time1, time2);

            using(SqlDataAdapter da=new SqlDataAdapter(sql,strConn)){

                dt_汇率表 = new DataTable();
                da.Fill(dt_汇率表);


            }
            gridControl1.DataSource = dt_汇率表;


        }



        private void fun_check()
        {
            //int i = 0;
            foreach (DataRow dr in dt_汇率表.Rows)
            {
                //dr["POS"] = i++;
                

               


            }



           
          //  string sql = string.Format("select * from 汇率维护表 where 币种='{0}'and 年='{1}'and 月='{2}'", drM["币种"].ToString(), drM["年"].ToString(), drM["月"].ToString());
           
            //DataTable dt_临时 = new DataTable();
            //using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            //{

                
            //    da.Fill(dt_临时);

            //}

            //if (dt_临时.Rows.Count > 0)
            //{

            //    throw new Exception("当月已存在改币种汇率，请重新检查");
            //    return; 
            //}
           



            


        }





        #endregion

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {


            try
            {
                drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                if (drM["生效"].ToString() == "True")
                {

                    e.RepositoryItem.ReadOnly = true;



                }
                else
                {
                    e.RepositoryItem.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message
                    );


            } 






        }

    }
}
