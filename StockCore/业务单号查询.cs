using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace StockCore
{
    public partial class 业务单号查询 : Form
    {
        string strconn = CPublic.Var.strConn;
        string str_单号 = "";
        public 业务单号查询()
        {
            InitializeComponent();
        }
        public 业务单号查询(string s_单号)
        {
            str_单号 = s_单号;
            InitializeComponent();
        }

        private void 业务单号查询_Load(object sender, EventArgs e)
        {
            fun_load();
        }

        private void fun_load()
        {
            string sql = string.Format(@"select 其他出入库申请子表.*,isnull(已处理数量,0)已处理数量  from 其他出入库申请子表
                        left  join (select  出入库申请明细号,sum(数量)as 已处理数量 from  其他出库子表 where 生效=1 group by  出入库申请明细号)a 
                        on a.出入库申请明细号=其他出入库申请子表.出入库申请明细号
                        where 其他出入库申请子表.出入库申请单号 = '{0}'", str_单号);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt);
            gcP.DataSource = dt;

        }
    }
}
