using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frm物料修改日志 : UserControl
    {
        string str_物料编码 = "";
        string str_原规格 = "";
        string str_物料名称 = "";
        string strconn = CPublic.Var.strConn;

        public frm物料修改日志(string str物料编码, string str物料名称, string str原规格)
        {
            InitializeComponent();
            str_物料编码 = str物料编码;
            str_物料名称 = str物料名称;
            str_原规格 = str原规格;
        }

        private void frm物料修改日志_Load(object sender, EventArgs e)
        {
            label1.Text = string.Format("当前物料为:{0}-{1}-规格型号:{2}", str_物料编码, str_物料名称, str_原规格);
            fun_载入();
        }

        private void fun_载入()
        {
            string sql = string.Format("select * from 基础数据物料信息修改日志表 where 物料编码 = '{0}'", str_物料编码);
            DataTable dtttt = new DataTable();
            SqlDataAdapter daaaa = new SqlDataAdapter(sql, strconn);
            daaaa.Fill(dtttt);
            gc.DataSource = dtttt;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
