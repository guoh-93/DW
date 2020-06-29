using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ERPproduct
{
    public partial class 部分完工记录 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataRow dr_完工工单;
        public 部分完工记录()
        {
            InitializeComponent();
        }

        public 部分完工记录(DataRow dr)
        {
            InitializeComponent();
            dr_完工工单 = dr;
        }

        private void 部分完工记录_Load(object sender, EventArgs e)
        {
            try
            {
                string sql = string.Format("select * from 生产工单完工记录表 where 生产工单号 = '{0}'",dr_完工工单["生产工单号"]);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gridControl1.DataSource = dt;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
               
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
    }
}
