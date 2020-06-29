using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class fm采购开票驳回 : Form
    {
        public bool bl_enter = false;
        public string s = "";

        public fm采购开票驳回(string s_dh,string str_gys)
        {

            InitializeComponent();

            textBox1.Text = str_gys;
            textBox2.Text = s_dh;

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                if (richTextBox1.Text.Trim() == "") throw new Exception("驳回意见不可为空");
                s = richTextBox1.Text;
                bl_enter = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
    }
}
