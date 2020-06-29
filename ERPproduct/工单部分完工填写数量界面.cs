using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class 工单部分完工填写数量界面:Form
    {
        public int in_部分完工数;
        public bool bl = false;
        public 工单部分完工填写数量界面()
        {
            InitializeComponent();
            textBox1.Focus();

        }
        //确定
#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                bl = true;
                int a = Convert.ToInt32(textBox1.Text);

                in_部分完工数 = a;

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入数量不正确");
            }
       
            
          
        
            
           
        }

        //取消
#pragma warning disable IDE1006 // 命名样式
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            bl = false;
            in_部分完工数 = 0;
            this.Close();
        }

        private void 工单部分完工填写数量界面_Load(object sender, EventArgs e)
        {

        }
    }
}
