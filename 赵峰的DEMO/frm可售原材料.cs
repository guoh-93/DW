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
    public partial class frm可售原材料 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM;

        public frm可售原材料()
        {
            InitializeComponent();
        }

        private void frm可售原材料_Load(object sender, EventArgs e)
        {
            string sql = @"select 物料编码,物料类型,大类,小类 from 基础数据物料信息表 
                        where (大类 = '轴类零件' and (小类 = '方轴' or 小类 = '杆')) 
                        or (大类 = '冲制零件' and 小类 = '底板') or 物料编码 = '999125'";
            dtM = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM);
            gc.DataSource = dtM;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DataRow r in dtM.Rows)
            {
                r["物料类型"] = "可售原材料";
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string sql = "select 物料编码,物料类型,大类,小类 from 基础数据物料信息表 where 1<>1";
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            new SqlCommandBuilder(da);
            da.Update(dtM);
            MessageBox.Show("OK");
        }
    }
}
