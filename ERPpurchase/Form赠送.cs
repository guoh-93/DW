using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class Form赠送 : Form
    {
        public Form赠送()
        {
            InitializeComponent();
        }
        #region
        string strcon = CPublic.Var.strConn;
        DataRow r;
        public bool flag = false;  //指示是否保存
    
        public string xiala = "";

        #endregion
        public Form赠送( DataRow dr )
        {
            InitializeComponent();
            r = dr;
            this.StartPosition = FormStartPosition.CenterScreen;
        }



        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            xiala = textBox1.Text.ToString();
            flag = true;
            this.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
