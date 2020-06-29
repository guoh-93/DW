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
    public partial class Form2 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dt1;
        DataTable dt2;
        SqlDataAdapter da1;
        SqlDataAdapter da2;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            fun_载入仓库物料表();
            fun_载入仓库物料数量表();
        }

        private void fun_载入仓库物料表()
        {
            string sql = "select * from 仓库物料表";
            dt1 = new DataTable();
            da1 = new SqlDataAdapter(sql, strconn);
            da1.Fill(dt1);
            gridControl1.DataSource = dt1;
        }

        private void fun_载入仓库物料数量表()
        {
            string sql = "select * from 仓库物料数量表";
            dt2 = new DataTable();
            da2 = new SqlDataAdapter(sql, strconn);
            da2.Fill(dt2);
            gridControl2.DataSource = dt2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fun_载入仓库物料表();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fun_载入仓库物料数量表();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataTable dt_返回值 = StockCore.StockCorer.fun_物料_单_计算("00011", "", strconn, true);
        }
    }
}
