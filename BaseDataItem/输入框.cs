using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class 输入框 : Form
    {
    public string  a;
        public 输入框()
        {
            InitializeComponent();
        }

        private void 输入框_Load(object sender, EventArgs e)
        {

        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            a = textBox1.Text.ToString();
            this.Close();
        }
    }
}
