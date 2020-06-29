using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class fm过往明细 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        string str_物料编码 = "";

        public fm过往明细()
        {
            InitializeComponent();
        }

        public fm过往明细(string str物料编码)
        {
            InitializeComponent();
            str_物料编码 = str物料编码;
        }

        private void fm过往明细_Load(object sender, EventArgs e)
        {
            fun_载入();
        }

        private void fun_载入()
        {
            string sql = string.Format("select * from 销售记录销售订单明细表 where 物料编码 = '{0}' and 生效日期 > '{1}' and 生效日期 < '{2}'"
                , str_物料编码, System.DateTime.Today.AddDays(1).AddSeconds(-1).AddYears(-1), (System.DateTime.Today.AddDays(1).AddSeconds(-1)));
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gc.DataSource = dtM;
        }
    }
}
