using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;

namespace 赵峰的DEMO
{
    public partial class frm链接MySql : Form
    {
        string strcon = "Password=a;Persist Security Info=True;User ID=sa1;Initial Catalog=自动检测数据;Data Source=192.168.10.7;Pooling=true;Max Pool Size=40000;Min Pool Size=0";
        string strconn = "";
        DataTable dt_保存 = null;
        DataTable dt_上传 = null;

        public frm链接MySql()
        {
            InitializeComponent();
        }

        private void frm链接MySql_Load(object sender, EventArgs e)
        {
            try
            {
                strconn = "server=115.28.57.46;User Id=szfuture;password=szfuture1234;Database=suFuture;CharSet=utf8";
                dt_保存 = new DataTable();
                string sql = "SELECT * FROM suFuture.t_product_db where 1<>1";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, strconn);
                da.Fill(dt_保存);

                gc.DataSource = dt_保存;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                fun_处理数据();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_处理数据()//id > '15111'
        {
            timer1.Stop();
            dt_保存.Clear();
          //  string sqll = @"select * from ABB检测结果总表 where 工作台名称 = 'WL-FR6W' and 检测是否通过 = 'PASS' and flag = 0  and 开始检测时间 > '2016-09-19 09:24:57.817' order by 开始检测时间";
            string sqll = @"select * from ABB检测结果总表 where 产品SN号='1706070089W100312'";
           
            dt_上传 = new DataTable();

            SqlDataAdapter daa = new SqlDataAdapter(sqll, strcon);
            daa.Fill(dt_上传);
            if (dt_上传.Rows.Count > 0)
            {
                foreach (DataRow r in dt_上传.Rows)
                {
                    DataRow dr = dt_保存.NewRow();
                    dt_保存.Rows.Add(dr);
                    dr["product_name"] = "电能表外置断路器";
                    dr["product_code"] = r["产品SN号"];//二维码
                    dr["company_name"] = "苏州未来电器";
                    dr["product_spec"] = "FAR6-W100";
                    dr["product_type"] = "智能终端";
                    dr["product_small_type"] = "";
                    dr["check_machine"] = r["机台名称"];
                    dr["check_date"] = r["结束检测时间"];
                    dr["check_people"] = r["操作员"];
                    dr["production_date"] = r["开始检测时间"];
                    dr["version"] = 0;

                    r["flag"] = 1;
                }
                new SqlCommandBuilder(daa);
                daa.Update(dt_上传);

                string sql = "SELECT * FROM suFuture.t_product_db where 1<>1";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, strconn);
                new MySqlCommandBuilder(da);
                da.Update(dt_保存);
                gc.DataSource = dt_保存;
            }
            Thread.Sleep(60000);
            timer1.Start();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            timer1.Start();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            timer1.Stop();
        }
    }
}
