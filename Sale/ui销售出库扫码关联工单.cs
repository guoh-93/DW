using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class ui销售出库扫码关联工单 : UserControl
    {
        #region
        string strcon = CPublic.Var.strConn;


        #endregion
         
        public ui销售出库扫码关联工单()
        {
            InitializeComponent();
        }

        private void ui销售出库扫码关联工单_Load(object sender, EventArgs e)
        {
            textBox4.Focus();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_save();
                textBox4.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBox1.Text = "";
                    textBox2.Text="";
                textBox3.Text="";
                textBox4.Text="";

            }




         

        }

        private void textBox4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if (textBox4.Text.Substring(0, 2) == "SA")
                {
                    textBox1.Text=textBox4.Text;
                }
                else if (textBox4.Text.Substring(0, 2) == "MO")
                {
                    textBox2.Text = textBox4.Text;
                }
                textBox4.Text = "";
                textBox4.Focus();

            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void fun_check()
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                throw new Exception("信息不完整");
            }
            string s = string.Format("select  * from 销售出库单号关联工单号表 where 成品出库单号='{0}' and 生产工单号='{1}'", textBox1.Text, textBox2.Text);
            DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (temp.Rows.Count > 0)
            {
                throw new Exception("两张单据已关联过");

            }

        }
        private void fun_save()
        {
            //string sql="e "
          
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                string sql = string.Format("select 客户  from  销售记录成品出库单主表 where 成品出库单号='{0}'", textBox1.Text);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
                if (dt.Rows.Count > 0)
                {
                    textBox3.Text = dt.Rows[0]["客户"].ToString();

                }

            }
        }
    }
}
