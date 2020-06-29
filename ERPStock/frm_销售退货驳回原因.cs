using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPStock
{
    public partial class frm_销售退货驳回原因 : Form
    {
        string strcon = CPublic.Var.strConn;
        DataRow r;
        public bool flag = false;  //指示是否保存


        public int 关闭 = 0;
        public string yijian = "";
        private DataRow dr;

        public frm_销售退货驳回原因()
        {
            InitializeComponent();
        }

        public frm_销售退货驳回原因(DataRow dr)
        {
            InitializeComponent();
            r = dr;
        }

        private void frm_销售退货驳回原因_Load(object sender, EventArgs e)
        {
            try
            {
                string sql = string.Format("select * from 退货申请主表 where 退货申请单号 = '{0}'", r["退货申请单号"]);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt.Rows.Count > 0)
                {
                    textBox1.Text = dt.Rows[0]["退货申请单号"].ToString();
                    textBox2.Text = dt.Rows[0]["客户"].ToString();
                    textBox3.Text = dt.Rows[0]["操作人员"].ToString();
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
                flag = true;
                yijian = textBox4.Text;
                barLargeButtonItem2_ItemClick(null, null);
                关闭 = 1;
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
                throw new Exception("驳回意见必填");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.Close();
                关闭 = 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
    }
}
