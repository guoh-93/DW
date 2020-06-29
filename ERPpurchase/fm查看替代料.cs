using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class fm查看替代料 : Form
    {
        //数据库连接字符串
        string strcon = CPublic.Var.strConn;
        DataTable dt_tdl = new DataTable();

        public fm查看替代料()
        {
            InitializeComponent();
        }


        public fm查看替代料(DataTable dt)
        {
            InitializeComponent();
            dt_tdl = dt;
        }

        private void fm查看替代料_Load(object sender, EventArgs e)
        {



            gridControl1.DataSource = dt_tdl;
        }
    }
}
