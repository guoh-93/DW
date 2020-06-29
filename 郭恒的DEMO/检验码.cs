using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace 郭恒的DEMO
{
    public partial class 检验码 : Form
    {
        public 检验码()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;                                     
            this.FormBorderStyle = FormBorderStyle.FixedDialog;    
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {


            try
            {
                fun_check();

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {

                    DataTable dtM = new DataTable();
                    dtM.Columns.Add("码");
                    int count = Convert.ToInt32(textBox3.Text);
                    int beginN = Convert.ToInt32(textBox4.Text);
                    
                    for (int i = 1; i <= count; i++)
                    {
                        string s = textBox1.Text + textBox2.Text + Convert.ToInt32(beginN++).ToString().PadLeft(14, '0');
                        string j = fun_gccode(s);
                        s = s + j;
                        DataRow dr = dtM.NewRow();
                        dr["码"] = s.ToString();
                        dtM.Rows.Add(dr);
                    }
                    button1.Text = "请稍等";
                    button1.Enabled = false;

                    ItemInspection.print_FMS.fun_生成校验码(dtM, saveFileDialog.FileName);
                    button1.Text = "生成excel";
                    button1.Enabled = true;
                    MessageBox.Show(this,"ok");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_check()
        {
            if (textBox2.Text.Trim() == "")
            {
                throw new Exception("类型代码未填");
            }
            if (textBox3.Text.Trim() == "")
            {
                throw new Exception("生产数量未填");

            }
            if (textBox4.Text.Trim() == "")
            {
                throw new Exception("起始序列号未填");

            }
        }
        private string fun_gccode(string sn)
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

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void 检验码_Load(object sender, EventArgs e)
        {
            
        }


        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }


    }
}
