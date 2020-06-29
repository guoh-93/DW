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
    public partial class frm_添加意见 : Form
    {
        string strconn = CPublic.Var.strConn;
        DataRow dr_审核;
        public bool bl_保存 = false;  //指示是否保存
        public string str_意见 = "";
        public frm_添加意见()
        {
            InitializeComponent();
        }

        public frm_添加意见(DataRow dr)
        {
            InitializeComponent();
            dr_审核 = dr;
        }

        private void frm_添加意见_Load(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = dr_审核["申请单号"].ToString();
                textBox2.Text = dr_审核["审核部门"].ToString();
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
                str_意见 = textBox3.Text;
                barLargeButtonItem2_ItemClick(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void fun_check()
        {
            if (textBox3.Text == "")
            {
                throw new Exception("意见必填");
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
