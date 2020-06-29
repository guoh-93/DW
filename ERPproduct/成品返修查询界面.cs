using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class 成品返修查询界面 : UserControl
    {
        public 成品返修查询界面()
        {
            InitializeComponent();
        }

        private void 成品返修查询界面_Load(object sender, EventArgs e)
        {
            
        }
        #region 数据加载
#pragma warning disable IDE1006 // 命名样式
        private void fun_返工()
#pragma warning restore IDE1006 // 命名样式
        {
            string str = "select * from 成品检验返工原因与产品序列号对应关系表 where 产品序列号 ='" + textBox1.Text.Trim() + "'";
           using(SqlDataAdapter da = new SqlDataAdapter(str,CPublic.Var.strConn))
           {
               DataTable dtM = new DataTable();
               da.Fill(dtM);
               label4.Text = dtM.Rows[0]["返工编号"].ToString();
               label5.Text = dtM.Rows[0]["返工原因"].ToString();
               label6.Text = textBox1.Text.ToString();
               textBox1.Text = null;
           }
        
        }

        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
        //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_返工();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键
            {
                barLargeButtonItem5_ItemClick(null, null);

            }
        }

      

       
    }
}
