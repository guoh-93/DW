using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPSale
{
    public partial class UI关联生产工单界面 : UserControl
    {

        #region 变量 
        string strconn = CPublic.Var.strConn;
        string str_生产制令单号;
        DataTable dtM;
        #endregion

        public UI关联生产工单界面()
        {

            InitializeComponent();
        }
        public UI关联生产工单界面( string s)
        {
            this.str_生产制令单号 = s;
            InitializeComponent();
        }

        private void UI关联生产工单界面_Load(object sender, EventArgs e)
        {
            try
            {
               
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void fun_load()
        {
            string sql = string.Format("select * from 生产记录生产制令表 where 生产制令单号='{0}'", str_生产制令单号);
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataRow dr = dt.Rows[0];
                dataBindHelper1.DataFormDR(dr);
            }
            string sql1 = string.Format("select * from 生产记录生产工单表 where  生产制令单号='{0}' ", textBox1.Text);
            using (SqlDataAdapter da =new SqlDataAdapter (sql1,strconn))
            {
                 dtM=new DataTable ();
                 da.Fill(dtM);
                gridControl1.DataSource=dtM;
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
