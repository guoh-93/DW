using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CPublic;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace 郭恒的Demo
{
    public partial class Form2 : Form
    {
        string strcon = CPublic.Var.strConn;
        int flag = 0;
        DataTable dt = new DataTable();
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "select * from 基础数据物料信息表 where 物料编码 not in (select  物料编码 from 仓库物料表)";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt =new DataTable ();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    dr["是否初始化"] = "否";
                }


                new SqlCommandBuilder(da);
                da.Update(dt);

            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
      
        private void button2_Click(object sender, EventArgs e)
        {
            string sql = @"select * from 基础数据物料信息表
                            where 基础数据物料信息表.是否初始化='否'";
            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dt);
                //foreach (DataRow dr in dt.Rows)
                //{
                //    dr["是否初始化"] = "是";
                //    if (dr["物料类型"].ToString() == "原材料")
                //    {
                //        StockCore.StockCorer.fun_Init初始化仓库物料("01-0001",dr["物料编码"].ToString(), 100);
                //    }
                //    else if (dr["物料类型"].ToString() == "成品")
                //    {
                //        StockCore.StockCorer.fun_Init初始化仓库物料("02-0001", dr["物料编码"].ToString(), 100);

                //    }
                //    else
                //    {
                //        StockCore.StockCorer.fun_Init初始化仓库物料("03-0001",dr["物料编码"].ToString(), 100);

                //    }
                //    new SqlCommandBuilder(da);

                //    da.Update(dt);
                //}
                MessageBox.Show("ok");

            }





           




        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sql="select * from t_product_db ";
            string constr = "server=115.28.57.46;User Id=szfuture;password=szfuture1234;Database=suFuture";
            using (MySqlDataAdapter da = new MySqlDataAdapter(sql, constr))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sql_1 = @"select 人事基础员工表.课室,人事基础部门表.部门编号 from  人事基础员工表 left join 人事基础部门表  
                                on 人事基础部门表.部门名称= 人事基础员工表.课室 where 人事基础员工表.课室<>''  ";


            DataTable dt_1 = new DataTable();
            dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
            string sql = "select * from 人事基础员工表 where 在职状态='在职'";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                new SqlCommandBuilder(da);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    if(dr["课室"].ToString()!="")
                    {
                    DataRow [] drr = dt_1.Select(string.Format("课室='{0}'", dr["课室"].ToString()));
                    dr["课室编号"] = drr[0]["部门编号"];
                    }
 
                }

                new SqlCommandBuilder(da); 
                da.Update(dt);
                MessageBox.Show("ok");

            }
      
           
       
           
        }

  


        private void button5_Click(object sender, EventArgs e)
        {
            
            dt.Columns.Add("税前",typeof(decimal));
            dt.Columns.Add("税后", typeof(decimal));
         
            DataRow dr = dt.NewRow();
            dr["税前"] = 0.2342;
            dr["税后"] = 0.42;
            dt.Rows.Add(dr);  
            gridControl1.DataSource = dt;
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (e.Value.ToString() != "")
            {
                if (e.Column.Caption == "税前")
                {
                    dr["税后"] = Convert.ToDecimal(e.Value) * Convert.ToDecimal(1 - 0.17);

                }
                if (e.Column.Caption == "税后")
                {

                    dr["税前"] = Convert.ToDecimal(e.Value) * Convert.ToDecimal(1 + 0.17);
                }
            }
        }
        //自动审核测试
        private void button8_Click(object sender, EventArgs e)
        {
            string s = $"select  * from 单据审核申请表 where 审核申请号='{textBox1.Text}'";
            DataTable t= CZMaster.MasterSQL.Get_DataTable(s,strcon);
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataRow dr = dt.NewRow();
            dr["税前"] = 0.2342;
            dr["税后"] = 0.42;
            dt.Rows.Add(dr);
            gridControl1.DataSource = dt;
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }



    }
}
