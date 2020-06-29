using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace 赵峰的DEMO
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strconn = CPublic.Var.strConn;
            string sql = "select * from  基础数据物料信息表";
            DataTable dt_物料 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataTable dt_属性 = new DataTable();
            dt_属性.Columns.Add("物料编码", typeof(string));
            dt_属性.Columns.Add("字段名", typeof(string));
            dt_属性.Columns.Add("属性值", typeof(string));
            foreach (DataRow dr in dt_物料.Rows)
            {
                foreach (DataColumn dc in dt_物料.Columns)
                {
                    DataRow dr_属性 = dt_属性.NewRow();
                    dt_属性.Rows.Add(dr_属性);
                    dr_属性["物料编码"] = dr["物料编码"];
                    dr_属性["字段名"] = dc.ColumnName;
















































                    
                }
            }
        }
    }
}
