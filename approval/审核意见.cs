using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace approval
{
    public partial class 审核意见 : Form
    {
        string strconn = CPublic.Var.strConn;
        string str_单号 = "";

        public bool bl_保存 = false;  //指示是否保存
        public string str_意见 = "";
        public 审核意见()
        {
            InitializeComponent();
        }
        public 审核意见(string str_申请单号)
        {
            InitializeComponent();
            str_单号 = str_申请单号;
        }

        private void 审核意见_Load(object sender, EventArgs e)
        {
            try
            {
                string sql = string.Format("select * from 单据审核申请表 where 审核申请单号 = '{0}'",str_单号);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt.Rows.Count > 0)
                {
                    textBox1.Text = dt.Rows[0]["关联单号"].ToString();
                    textBox2.Text = dt.Rows[0]["相关单位"].ToString();
                    textBox3.Text = dt.Rows[0]["申请人"].ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                bl_保存 = true;
                str_意见 = textBox4.Text;
                barLargeButtonItem2_ItemClick(null, null);
                 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void fun_check()
        {
            if (textBox4.Text == "")
            {
                throw new Exception("审核意见必填");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.Close();
                 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
    }
}
