using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERPproduct
{
    public partial class fm箱贴序列码检验码 : Form
    {

        public bool flag = false;
        public string s = "";
        public int i_起始 = 0;
        public fm箱贴序列码检验码()
        {
            InitializeComponent();
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            if (textBox2.Text.Trim() == "")
            {
                throw new Exception("类型代码未填");
            }
      
            if (textBox4.Text.Trim() == "")
            {
                throw new Exception("起始序列号未填");

            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_check();
                flag = true;
                i_起始 = Convert.ToInt32(textBox4.Text.Trim());
                string x = textBox1.Text + textBox2.Text + i_起始.ToString().PadLeft(14, '0');
                //s = x+fun_gccode(x); 郁静华 说 不要校验码
                s = x;
                this.Close();
            }
            catch (Exception)
            {
                
                throw;
            }


          

        }


#pragma warning disable IDE1006 // 命名样式
        public string fun_gccode(string sn)
#pragma warning restore IDE1006 // 命名样式
        {
            //1.获取前21位 
            string s = sn.Substring(0, 21);
            char[] ss = s.ToCharArray();
            //权重值 奇数位为3 偶数位为1  
            //对应位的值乘以权重  并 累加
            int sum = 0;
            int i = 1;
            int weight = 1;
            foreach (char c in ss)
            {
                if (i % 2 == 0) //权重为1 
                {
                    weight = 1;
                }
                else
                {
                    weight = 3;
                }
                i++;
                sum = sum + Convert.ToInt32(c.ToString()) * weight;
            }
            // 10 - (和值模10) 
            int mod = (10 - sum % 10) % 10;

            s = mod.ToString();
            return s;
        }

    }
}
