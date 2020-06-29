using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frm消息弹框 : Form
    {
        string ss = "";
        public frm消息弹框(string s)
        {
            InitializeComponent();
            ss = s;
        }

        private void frm消息弹框_Load(object sender, EventArgs e)
        {
            label1.Text = ss;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
