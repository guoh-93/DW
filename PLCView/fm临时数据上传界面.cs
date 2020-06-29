using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

using System.Windows.Forms;

namespace PLCView
{
    public partial class fm临时数据上传界面 : UserControl
    {
        string strconn = CPublic.Var.geConn("PLC");
        DataTable dtP;

        public fm临时数据上传界面()
        {
            InitializeComponent();
        }

        private void fm临时数据上传界面_Load(object sender, EventArgs e)
        {
            fun_获取ABB_W();
        }

        private void fun_xxx()
        {
            DataTable dt_c7;
          string sql="select * from ABB检测结果总表 where flag=0 ";
          using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
          {
              dt_c7 = new DataTable();
              da.Fill(dt_c7);

          }
          string constr = "server=115.28.57.46;User Id=szfuture;password=szfuture1234;Database=suFuture;Charset=utf8";
         
          string sql_mysql = "select * from  t_product_db where 1<>1";
          using (MySqlDataAdapter da = new MySqlDataAdapter(sql_mysql, constr))
          {
              DataTable dt_1 = new DataTable();
              da.Fill(dt_1);
              foreach (DataRow dr in dt_c7.Rows)
              {
                  DataRow dr_1 = dt_1.NewRow();
                  dr_1["product_code"] = dr["产品SN号"].ToString();
                  dr_1["company_name"] = "SuFuture";

                  if (dr["产品产线"].ToString() == "智能断路器")
                  {
                      //                {
                      dr_1["product_name"] = "自复式过欠压保护器";

                      dr_1["product_spec"] = "ABB";

                      dr_1["product_type"] = "ABB产品类型";
                      dr_1["product_small_type"] = "ABB产品小类";


                  }
                  else
                  {
                      dr_1["product_name"] = "电能表外置断路器";
                      dr_1["product_spec"] = "FAR6-W";

                      dr_1["product_type"] = "W产品类型";
                      dr_1["product_small_type"] = "W产品小类";

                  }

                  dr_1["check_machine"] = dr["机台名称"].ToString();

                  dr_1["check_date"] = dr["结束检测时间"].ToString();
                  dr_1["production_date"] = dr["结束检测时间"].ToString();
                  dr_1["check_people"] = dr["操作员"].ToString();
                  dr_1["version"] = 0;

                  dt_1.Rows.Add(dr_1);

                  dr["flag"] = 1;

              }
              new MySqlCommandBuilder(da);
              da.Update(dt_1);

          }
            string sql_2="select * from ABB检测结果总表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql_2, strconn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt_c7);
            }
        }

        private void fun_Mysql(DataTable dt)
        {
            //打开数据库连接
            string sql = "";
            string constr = "server=115.28.57.46;User Id=szfuture;password=szfuture1234;Database=suFuture;Charset=utf8";
//            MySqlConnection mycon = new MySqlConnection(constr);
//            mycon.Open();

//            for (int i = 0; i < dt.Rows.Count; i++)
//            {
//                if (dt.Rows[i]["产品产线"].ToString() == "智能断路器")
//                {
//                    sql = string.Format(@"insert into t_product_db(product_code,company_name,product_name,product_spec,product_type,product_small_type,check_machine,
//                    check_date,check_people,production_date,version) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
//                    dt.Rows[i]["产品SN号"].ToString(), "苏州未来电器", "自复式过欠压保护器","ABB", "ABB产品类型","ABB产品小类",
//                    dt.Rows[i]["机台名称"].ToString(), dt.Rows[i]["结束检测时间"].ToString(), dt.Rows[i]["操作员"].ToString(), dt.Rows[i]["结束检测时间"].ToString(), 0);
//                }
//                else
//                {
//                    sql = string.Format(@"insert into t_product_db(product_code,company_name,product_name,product_spec,product_type,product_small_type,check_machine,
//                    check_date,check_people,production_date,version) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
//                    dt.Rows[i]["产品SN号"].ToString(), "苏州未来电器", "电能表外置断路器器", "FAR6-W", "W产品类型", "W产品小类", dt.Rows[i]["机台名称"].ToString(), 
//                    dt.Rows[i]["结束检测时间"].ToString() ,dt.Rows[i]["操作员"].ToString(),
//                    dt.Rows[i]["结束检测时间"].ToString(),0);
//                }
//                MySqlCommand mycmd = new MySqlCommand(sql, mycon);
//                mycmd.ExecuteNonQuery();
              
            //}

            ////关闭连接
            //Console.ReadLine();
            //mycon.Close();
            string sql_1 = "select * from  t_product_db where 1<>1";
            using (MySqlDataAdapter da = new MySqlDataAdapter(sql_1, constr))
            {
                DataTable dt_1 = new DataTable();
                da.Fill(dt_1);
                foreach (DataRow dr in dt.Rows)
                {
                   DataRow dr_1= dt_1.NewRow();
                   dr_1["product_code"] = dr["产品SN号"].ToString();
                   dr_1["company_name"] = "苏州未来电器";

                   if (dr["产品产线"].ToString() == "智能断路器")
                   {
                       //                {
                       dr_1["product_name"] = "自复式过欠压保护器";

                       dr_1["product_spec"] = "ABB";

                       dr_1["product_type"] = "ABB产品类型";
                       dr_1["product_small_type"] = "ABB产品小类";


                   }
                   else
                   {
                       dr_1["product_name"] = "电能表外置断路器";
                       dr_1["product_spec"] = "FAR6-W";

                       dr_1["product_type"] = "W产品类型";
                       dr_1["product_small_type"] = "W产品小类";

                   }

                   dr_1["check_machine"] = dr["机台名称"].ToString();

                   dr_1["check_date"] = dr["结束检测时间"].ToString();
                   dr_1["production_date"] = dr["结束检测时间"].ToString();
                   dr_1["check_people"] = dr["操作员"].ToString();
                   dr_1["version"] = 0;
  
                   dt_1.Rows.Add(dr_1);
                }
                new MySqlCommandBuilder(da);
                da.Update(dt_1);

            }

        }

        private void fun_获取ABB_W()
        {
            string sql = "select * from ABB检测结果总表 where 开始检测时间 > '2016-06-01 00:00:00'";
            dtP = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtP);
            gridControl1.DataSource = dtP;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_获取ABB_W();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_Mysql(dtP);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            fun_xxx();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                timer1.Start();
            }
            catch (Exception ex)
            {

                CZMaster.MasterLog.WriteLog(ex.Message, "");
            }
        }

     
    }
}
