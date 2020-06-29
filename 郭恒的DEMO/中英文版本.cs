using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 郭恒的DEMO
{
    public partial class 中英文版本 : Form
    {
        string strcon = CPublic.Var.strConn;
        

        public 中英文版本()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = "select  top 100 * from 基础数据物料信息表 ";
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s,strcon);
            gridControl2.DataSource = t;
        }
    }
}
