using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;

using System.Windows.Forms;
using System.Data.SqlClient;

namespace MoldMangement
{
    public partial class Form2 : Form
    {
        //static string strConn = "Password = a; Persist Security Info = True; User ID = sa; Initial Catalog = fms; Data Source = XINREN ";
        string strConn = CPublic.Var.strConn;
        DataTable dt = new DataTable();
        //DataRow dr;
        DataRow dr_卡;
        /// <summary>
        /// true为新增状态
        /// </summary>
        bool bl = false;
        /// <summary>
        /// true为修改状态
        /// </summary>
        bool bl_修改 = true;
        public Form2()
        {
            InitializeComponent();
        }

          //带值过来
        public Form2(DataRow dr)
        {
           
            InitializeComponent();
            dr_卡  = dr;
        }

    

        private void fun_load()
        {
            string sql = string.Format(" select * from 计量器具明细卡表 where 计量器具编号='{0}'", dr_卡 ["计量器具编号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter (sql ,strConn ))
            {
                dt = new DataTable();
                da.Fill(dt );
            
            }
            gc1.DataSource = dt;
            bl = false; 
        }



        private void Form2_Load(object sender, EventArgs e)
        {
            fun_load();
            
            textBox1.Text = dr_卡["计量器具编号"].ToString();
            textBox2.Text = dr_卡["计量器具名称"].ToString();
            textBox3.Text = dr_卡["计量器具规格"].ToString();
            textBox4.Text = dr_卡["出厂编号"].ToString();
            textBox5.Text = dr_卡["制造单位"].ToString();
            textBox14.Text = dr_卡["所属大类"].ToString();
            textBox6.Text = dr_卡["证书号"].ToString();
            textBox15.Text = dr_卡["状态"].ToString();
            //textBox7.Text = dr_卡["精度"].ToString();
            textBox8.Text = dr_卡["录入人员"].ToString();
            textBox22.Text = dr_卡["录入时间"].ToString();
            textBox17.Text = dr_卡["管理级别"].ToString();
            textBox18.Text = dr_卡["所属部门"].ToString();
            textBox19.Text = dr_卡["使用人"].ToString();
            //textBox9.Text = dr_卡["准用证号"].ToString(); ;
            textBox10.Text = dr_卡["检定标准"].ToString();
            textBox11.Text = dr_卡["检定周期"].ToString(); 
            textBox12.Text = dr_卡["检定单位"].ToString();
            textBox20.Text = dr_卡["检定结果"].ToString();
            if (dr_卡["有效期"] != DBNull.Value)
            {
                textBox21.Text = Convert.ToDateTime(dr_卡["有效期"]).ToString("yyyy-MM-dd");
            }
                textBox8.Text = dr_卡["录入人员"].ToString();
            if (dr_卡["录入时间"] != DBNull.Value)
            {
                textBox22.Text = Convert.ToDateTime(dr_卡["录入时间"]).ToString("yyyy-MM-dd");
            }
            
                textBox13.Text = dr_卡["测量范围"].ToString();
                 //textBox14.Text = dr_卡["分度值"].ToString();
            if (dr_卡["出厂日期"] != DBNull.Value)
            {
                textBox23.Text = Convert.ToDateTime(dr_卡["出厂日期"]).ToString("yyyy-MM-dd");
            }
            if (dr_卡["购置日期"] != DBNull.Value)
            {
                textBox24.Text = Convert.ToDateTime(dr_卡["购置日期"]).ToString("yyyy-MM-dd");
            }
            if (dr_卡["领用日期"] != DBNull.Value)
            {
                textBox25.Text = Convert.ToDateTime(dr_卡["领用日期"]).ToString("yyyy-MM-dd");
            }
            //textBox15.Text = dr_卡["准确度"].ToString();
            textBox16.Text = dr_卡["备注"].ToString();
            //DataRow[] drr = dt.Select(string.Format("计量器具编号 = '{0}'", dr_卡["计量器具编号"].ToString()));
            //textBox6.Text = drr[0]["测量范围"].ToString();

        }
       
        //保存
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (bl)
            {

                //DataRow dr;
                //dr = dt.NewRow();
                //dt.Rows.Add(dr);
                //dataBindHelper1.DataToDR(dr);

                DataTable dt_单号 = new DataTable();

                string sql_修改 = string.Format("select * from 计量器具基础信息表 where 计量器具编号='{0}'", textBox1.Text);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_修改, strConn))
                {
                    da.Fill(dt_单号);
                }
                string a = dt_单号.Rows[0]["计量器具单号"].ToString();
                //int b = Convert.ToInt32(a);
                if ( a == null || a == "" )
                {
                    fun_保存台账();
                    fun_保存记录();
                }
                else {

                    fun_保存记录();
                }

                //try
                //{
                //    string sql = "select * from 计量器具明细卡表 where 1<>1";
                //    using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                //    {
                //        DateTime t = CPublic.Var.getDatetime();
                //        textBox1.Text = string.Format("JL{0}{1:00}{2:00}{3:00000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("JL", t.Year, t.Month));
                //        new SqlCommandBuilder(da);
                //        da.Update(dt);


                //    }
                //    MessageBox.Show("保存成功");

                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
                //barLargeButtonItem1_ItemClick(null, null);
            }
            else if (bl_修改)
            {
                fun_修改保存();

            }
          
        }

        private void fun_修改保存()
        {

            //DataTable dt_修改 = update();
            //string sql_修改 = "select * from 计量器具明细卡表 where 1<>1";

            //SqlConnection conn = new SqlConnection(strConn);
            //conn.Open();
            //SqlTransaction ts = conn.BeginTransaction("初始化");
            //try
            //{

            //    SqlCommand cm_修改 = new SqlCommand(sql_修改, conn, ts);

            //    SqlDataAdapter da_修改 = new SqlDataAdapter(cm_修改);

            //    new SqlCommandBuilder(da_修改);

            //    try
            //    {
            //        da_修改.Update(dt_修改);

            //    }

            //    catch
            //    {

            //    }

            //    ts.Commit();
            //    MessageBox.Show("修改成功");
            //}
            //catch (Exception ex)
            //{
            //    ts.Rollback();
            //    throw ex;
            //}
            //barLargeButtonItem1_ItemClick(null, null);



        }
        //private DataTable update()
        //{
        //    //DataTable dt_修改 = new DataTable();
           
            //string sql_修改 = string.Format("select * from 计量器具明细卡表 where 计量器具编号='{0}'", textBox1.Text);
            //using (SqlDataAdapter da = new SqlDataAdapter(sql_修改, strConn))
            //{
            //    da.Fill(dt_修改);
            //}
            //dt_修改.Rows[0]["测量范围"] = textBox6.Text;
            //dt_修改.Rows[0]["分度值"] = textBox7.Text;
            //dt_修改.Rows[0]["准确度"] = textBox8.Text;
            //dt_修改.Rows[0]["备注"] = textBox11.Text;
            //dt_修改.Rows[0]["检定人"] = textBox12.Text;
            ////dt_修改.Rows[0]["使用人或地点"] = textBox14.Text;
            ////dt_修改.Rows[0]["履历情况"] = textBox15.Text;
            //dt_修改.Rows[0]["出厂日期"] = dateEdit1.Text;
            //dt_修改.Rows[0]["购置日期"] = dateEdit2.Text;
            //dt_修改.Rows[0]["领用日期"] = dateEdit3.Text;
          

            //return (dt_修改);
        //}

        //删除
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr;
            dr = gv1.GetDataRow(gv1.FocusedRowHandle);           
            if (MessageBox.Show("确认删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                dr.Delete();
                 
                 try
                 {
                     string sql = "select * from 计量器具明细卡表 where 1<>1";
                     using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                     {


                         new SqlCommandBuilder(da);
                         da.Update(dt);

                     }
                     MessageBox.Show("删除成功");
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show(ex.Message);
                 }
                 barLargeButtonItem1_ItemClick(null, null);
            }

        }

        //关闭
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
           
            //textBox6.Clear();
            //textBox7.Clear();
            //textBox8.Clear();
            //textBox9.Clear();
            //textBox10.Clear();
            //textBox11.Clear();
            //textBox12.Clear();            
            ////textBox14.Clear();
            ////textBox15.Clear();
            //dateEdit1.Text = "";
            //dateEdit2.Text = "";
            //dateEdit3.Text = "";
            //dateEdit4.Text = "";
            //comboBox1.Text = "";
        }
        //新增
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ////dr = dt.NewRow();
            ////dt.Rows.Add(dr);
           
            //textBox9.Enabled = true;
            //comboBox1.Enabled = true;
            //dateEdit4.Enabled = true;
            //barLargeButtonItem1_ItemClick(null ,null );
            //bl = true;
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //string str = "";
            //int t = 1;
            string str_打印机;
           
            PrintDialog printDialog1 = new PrintDialog();
            printDialog1.UseEXDialog = true;
            this.printDialog1.Document = this.printDocument1;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                print.fun_打印开票(dt,  printDialog1.PrinterSettings.PrinterName);
            
            }
        }

        //点击事件
        private void gv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr;
            //if (checkBox1 .Checked ==true)
            //{
                //textBox1.Enabled = false;
                //textBox2.Enabled = false;
                //textBox3.Enabled = false;
                //textBox4.Enabled = false;
                //textBox5.Enabled = false;
                //textBox10.Enabled = false;
                //textBox9.Enabled = false;
                //comboBox1.Enabled = false;
                //dateEdit4.Enabled = false;
                dr = gv1.GetDataRow(gv1 .FocusedRowHandle );
                dataBindHelper1.DataFormDR(dr);
            
            //}
        }

        private void fun_保存台账()
        {
            DataTable dt_虚拟 = new DataTable();
           
            string sql = string.Format(" select * from 计量器具基础信息表 where 计量器具编号='{0}'", dr_卡["计量器具编号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
            {
               
                da.Fill(dt_虚拟);
                DataRow dr_虚拟 = dt_虚拟.NewRow();
                DateTime t = CPublic.Var.getDatetime();
                dr_虚拟["计量器具单号"] = string.Format("JL{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("JL", t.Year, t.Month));
                dt_虚拟.Rows[0]["计量器具单号"] = dr_虚拟["计量器具单号"];
                new SqlCommandBuilder(da);
                da.Update(dt_虚拟);
            }                    
        }


        private void fun_保存记录()
        {
           
                if (bl)
                {
                    DataRow dr;
                    dr = dt.NewRow();
                    dt.Rows.Add(dr );
                    //dr["计量器具编号"] = textBox1.Text;
                    //dr["计量器具名称"] = textBox2.Text;
                    //dr["计量器具规格"] = textBox3.Text;                   
                    //dr["出厂编号"] = textBox4.Text;
                    //dr["制造单位"] = textBox5.Text;
                    //dr["测量范围"] = textBox6.Text;                    
                    //dr["分度值"] = textBox7.Text;                    
                    //dr["准确度"] = textBox8.Text;
                    //dr["检定周期"] = textBox9.Text;
                    //dr["出厂日期"] = dateEdit1.Text;
                    //dr["购置日期"] = dateEdit2.Text;
                    //dr["领用日期"] = dateEdit3.Text;

                    //DateTime t = CPublic.Var.getDatetime();
                    //dr["计量器具明细号"] = string.Format("JL{0}{1:00}{2:00}{3:00000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("JL", t.Year, t.Month))+ "-" +1;
                    //dr["计量器具明细号"] = dr_卡["计量器具单号"] + "-" +1;
                    //textBox12.Text = CPublic.Var.localUserName;
                    //dr["检定人"] = textBox12.Text;
                    DataTable dt_查明细号 = new DataTable();
                    string ta = " select * from 计量器具明细卡表";
                    using (SqlDataAdapter da = new SqlDataAdapter(ta, strConn))
                    {

                        da.Fill(dt_查明细号);
                    }
                    DataRow[] drr = dt_查明细号.Select(string.Format("计量器具编号 = '{0}'", dr_卡["计量器具编号"].ToString()));
                    if (drr .Length ==0)
                    {
                       

                       

                    //}
                    //if (dr_卡["计量器具明细号"] == null || dr_卡["计量器具明细号"] == "")
                    //{
                        DataTable dt_已有单号 = new DataTable();
                        string sql3 = string.Format(" select * from 计量器具基础信息表 where 计量器具编号='{0}'", dr_卡["计量器具编号"].ToString());
                        using (SqlDataAdapter da = new SqlDataAdapter(sql3, strConn))
                        {

                            da.Fill(dt_已有单号);
                            textBox10.Text = dt_已有单号.Rows[0]["计量器具单号"].ToString() + "-" + 1;
                            //dataBindHelper1.DataToDR(dr);
                        }
                        
                    }else {

                    string sql2 = string.Format("select max(计量器具明细号) from 计量器具明细卡表 where 计量器具编号 = '{0}'",dr_卡["计量器具编号"]);
                    DataTable dt2 = new DataTable();
                    
                    using (SqlDataAdapter da = new SqlDataAdapter(sql2, strConn))
                    {
                        
                        da.Fill(dt2);


                        //int i = 0;
                       
                        string srr = dt2.Rows[0][0].ToString();
                        string ss = srr.Split('-')[1].ToString();
                        int  a = Convert .ToInt32 ( ss) + 1;
                        //a = dt2.Rows[0][0].ToString().Length ;
                        //string srr2 = dt2.Rows[0][0].ToString();
                        string ss2 = srr.Split('-')[0].ToString();
                        textBox10.Text = ss2  + "-" + a ;

                      
                        
                    }
                    }
                    dr["计量器具明细号"] = textBox10.Text;
                    
                    dataBindHelper1.DataToDR(dr);

                }
                try
                {
                string sql = "select * from 计量器具明细卡表 where 1<>1";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strConn))
                {


                    new SqlCommandBuilder(da);
                    da.Update(dt);

                }
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                barLargeButtonItem1_ItemClick(null, null);
        
        
        }

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }
       
    }
}
