using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 赵峰的DEMO
{
    public partial class Form1 : Form
    {
        string strconn = CPublic.Var.strConn;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //string sql = "select 物料编码,库存上限,库存下限 from 基础数据物料信息表";
            //dt = new DataTable();
            //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //da.Fill(dt);
            //gcM.DataSource = dt;

            //string sql2 = "select cpbh,未来最低安全库存,未来最高库存 from 原料安全库存";
            //dt2 = new DataTable();
            //SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            //da2.Fill(dt2);
            //gcP.DataSource = dt2;
        }

        DataTable dt; DataTable dt2;

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DataRow r in dt.Rows)
            {
                DataRow[] ds = dt2.Select(string.Format("cpbh = '{0}'", r["物料编码"].ToString()));
                if (ds.Length > 0)
                {
                    r["库存上限"] = ds[0]["未来最高库存"];
                    r["库存下限"] = ds[0]["未来最低安全库存"];
                }
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string sql = "select 物料编码,库存上限,库存下限 from 基础数据物料信息表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt);
            MessageBox.Show("2");
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            web_未来_测试.MESWSMain web = new web_未来_测试.MESWSMain();
            string str = web.fun_测试();
            MessageBox.Show(str);
        }
    }
}
