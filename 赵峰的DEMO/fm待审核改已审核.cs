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
    public partial class fm待审核改已审核 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM;

        public fm待审核改已审核()
        {
            InitializeComponent();
        }

        private void fun_()
        {
            string sql = "select * from 基础数据物料信息表 where 停用 = 0 and 物料类型 = '成品' and 审核 = '待审核'";
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            DataView dv = new DataView(dtM);
            dv.RowFilter = "审核 = '待审核'";
            gc.DataSource = dtM;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (DataRow dr in dtM.Rows)
            {
                dr["审核"] = "已审核";
                dr["审核人ID"] = "admin";
                dr["审核人"] = "admin" + "批量";
                dr["审核日期"] = System.DateTime.Now;
            }
            string sql = "select * from 基础数据物料信息表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dtM);
        }

        private void fm待审核改已审核_Load(object sender, EventArgs e)
        {
            fun_();
        }
    }
}
