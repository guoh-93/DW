using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class frm来料入库关闭完成原因 : Form
    {

        #region
        string strcon = CPublic.Var.strConn;
        DataRow r;
        public bool flag = false;  //指示是否保存
        public string str = "";
        #endregion
  
        public frm来料入库关闭完成原因()
        {
            InitializeComponent();
        }
        public frm来料入库关闭完成原因(DataRow dr)
        {
            InitializeComponent();
            r = dr;
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (comboBox1.Text.Trim() == "")
                {
                    throw new Exception("原因不能为空");
                }
                flag = true;
                str = comboBox1.Text.Trim();
                this.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close(); 
        }

        private void frm来料入库关闭完成原因_Load(object sender, EventArgs e)
        {
            dataBindHelper1.DataFormDR(r);
        }
    }
}
