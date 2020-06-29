using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class frm关闭工单原因 : Form
    {

        #region
        string strcon = CPublic.Var.strConn;
        DataRow r;
        public bool flag = false;  //指示是否保存
        public string str = "";
        #endregion
  
        
        public frm关闭工单原因()
        {
            InitializeComponent();

        }
        public frm关闭工单原因(DataRow dr)
        {
            InitializeComponent();
            r = dr;
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close(); 
        }
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (textBox5.Text.Trim() == "")
                {
                    throw new Exception("关闭备注不能为空");
                }
                flag = true;
                str = textBox5.Text.Trim();
                this.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            


        }

#pragma warning disable IDE1006 // 命名样式
        private void frm关闭工单原因_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            dataBindHelper1.DataFormDR(r);
        }
    }
}
