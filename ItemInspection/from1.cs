using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ItemInspection
{
    public partial class from1 : Form
    {

        string strconn = CPublic.Var.strConn;
        public string str_文件编号="";
        public bool flag = false;  //指示是否保存

        public from1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            str_文件编号 = textBox1.Text.Trim();
            flag = true;
            this.Close();
        }
    }
}
