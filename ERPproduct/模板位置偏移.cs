using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace ERPproduct
{
    public partial class 模板位置偏移 : Form
    {
        public 模板位置偏移()
        {
            InitializeComponent();
        }
       //保存
#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                int b = int.Parse(textBox2.Text);
                int a = int.Parse(textBox1.Text);
            }
            catch
            {
                MessageBox.Show("请输入数字");
                return;
            }

            fun_保存文件();
            this.Close();
        }
        //取消
#pragma warning disable IDE1006 // 命名样式
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            this.Close();

        }

        private void 模板位置偏移_Load(object sender, EventArgs e)
        {
            fun_读取文件();
        }


#pragma warning disable IDE1006 // 命名样式
        public void fun_保存文件()
#pragma warning restore IDE1006 // 命名样式
        {
            FileStream aFile = new FileStream(Application.StartupPath + @"\Mode\log.txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(aFile);
            sw.WriteLine(textBox1.Text);
            sw.WriteLine(textBox2.Text);
            sw.Close();
        }
#pragma warning disable IDE1006 // 命名样式
        public void fun_读取文件()
#pragma warning restore IDE1006 // 命名样式
        {
            string strLine;
            FileStream aFile = new FileStream(Application.StartupPath + @"\Mode\log.txt", FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(aFile);
            strLine = sr.ReadLine();
            textBox1.Text = strLine;
            strLine = sr.ReadLine();
            textBox2.Text = strLine;
            sr.Close();
        }
    }
}
