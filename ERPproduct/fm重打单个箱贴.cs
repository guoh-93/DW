using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;
using System.Threading;

namespace ERPproduct
{
    public partial class fm重打单个箱贴 : Form
    {
        DataRow r;
        double f_资产编码 = 0;

        string str_x;//资产编码  textBox1.Text.Trim().Length-6

        string str_资产编号止 = "";
        string str_printer箱贴 = new PrintDocument().PrinterSettings.PrinterName;
        bool bl_停止 = false;
        bool flag = false;
        public fm重打单个箱贴()
        {
            InitializeComponent();
        }

        public fm重打单个箱贴(DataRow dr)
        {
            InitializeComponent();
            r = dr;
        }

#pragma warning disable IDE1006 // 命名样式
        private void fm重打单个箱贴_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            string path = Application.StartupPath + string.Format(@"\打印机配置.txt");
            List<string[]> x = ERPorg.Corg.ReadTxt(path);
            str_printer箱贴 = x[0][0].ToString();
         
         
            ////加载 箱贴打印机名
            //StreamReader sr = new StreamReader(Application.StartupPath + string.Format(@"\打印机配置.txt"), Encoding.Default);
            //string s;
            ////string[] line = File.ReadAllLines(sr);
            //while ((s = sr.ReadLine()) != null)
            //{

            //   //配置第一行 为箱贴打印机

            //        str_printer箱贴= s;
            //        break;
            
            //}
            //sr.Close();

            dataBindHelper1.DataFormDR(r);
            textBox12.Text = Convert.ToInt32 (Convert.ToDecimal(textBox12.Text)).ToString();
            str_x = textBox5.Text.Substring(0, textBox5.Text.Trim().Length - 6);

            f_资产编码 = Convert.ToDouble(textBox5.Text.Substring(textBox5.Text.Trim().Length - 6, 6));

             str_资产编号止 = str_x + (f_资产编码 + Convert.ToDouble(textBox12.Text) - 1).ToString("000000"); //保证是 六位 前面不足的 0 补足
            textBox6.Text = str_资产编号止;
            int In_发货数量 =Convert.ToInt32(Convert.ToDecimal(textBox12.Text));
            int i_箱装数量 = Convert.ToInt32(textBox10.Text);
            int i_总箱数 = 0;
            if (In_发货数量 % i_箱装数量 == 0)
            {
                i_总箱数 = In_发货数量 / i_箱装数量;
            }
            else
            {
                //i_余数 = In_发货数量 % i_箱装数量;
                i_总箱数 = In_发货数量 / i_箱装数量 + 1;
            }
            textBox11.Text = i_总箱数.ToString();
        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {
                if (flag == false)
                {
                    fun_print();
                }
                else
                {

                    MessageBox.Show("正在打印中,请稍候");
                }
            }
            catch 
            {
                
                
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox13_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            double order = 0;
            double count=0;
            try
            {
               order=  Convert.ToDouble(textBox13.Text); //第几箱 
               count = Convert.ToDouble(textBox10.Text);//箱装数量
                double x=(f_资产编码+count*(order-1));
                textBox2.Text = str_x+x.ToString("000000");  //本箱起
                textBox1.Text = str_x+(x + count - 1).ToString("000000");         //本箱止

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_print()
#pragma warning restore IDE1006 // 命名样式
        {
        

            string path = Application.StartupPath + string.Format(@"\Mode\广州供电局.lab");

            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (r[5].ToString() == "重庆市电力公司")
            {
                path = Application.StartupPath + string.Format(@"\Mode\重庆电力(日新).lab");
                int i_余数 = Convert.ToInt32(textBox12.Text.Trim()) % Convert.ToInt32(textBox10.Text.Trim());



                string dy = textBox8.Text.ToString().Trim();
                dy = dy.Substring(0, dy.Length - 1);
                dic.Add("dy", dy); //电压 
                string dl = textBox9.Text.ToString().Trim();
                dl = dl.Substring(0, dl.Length - 1);
                dic.Add("dl", dl); //电流

                dic.Add("cpxh", textBox3.Text.ToString().Trim());// 规格型号

                if (textBox4.Text.Trim() == "2")
                {
                    dic.Add("js", "单相"); //极数
                }
                else
                {
                    dic.Add("js", "三相"); //极数

                }

                dic.Add("pch", textBox16.Text.ToString().Trim()); //对应批次号

                string s_jy = ERPorg.Corg.fun_gccode(textBox2.Text.Trim().ToString()); //本箱起
                dic.Add("qsm", textBox2.Text.Trim().ToString() + s_jy);

                s_jy = ERPorg.Corg.fun_gccode(textBox1.Text.Trim().ToString());
                dic.Add("jsm", textBox1.Text.Trim().ToString() + s_jy);


                dic.Add("xc", textBox13.Text.Trim().ToString());


                dic.Add("zxs", textBox11.Text.Trim().ToString());
                dic.Add("ys", i_余数.ToString());
                dic.Add("xzsl", textBox10.Text.Trim().ToString());
            
            
            }
            else
            {


              
                string dy = textBox8.Text.ToString().Trim();
                dy = dy.Substring(0, dy.Length - 1);
                dic.Add("dy", dy); //电压  
                string dl = textBox9.Text.ToString().Trim();
                dl = dl.Substring(0, dl.Length - 1);
                dic.Add("dl", dl); //电流

                dic.Add("ggxh", textBox3.Text.ToString().Trim());// 规格型号

                dic.Add("js", textBox4.Text.ToString().Trim()); //名称参数
                dic.Add("hth", textBox7.Text.ToString().Trim()); //合同号        
                dic.Add("fhh", textBox16.Text.ToString().Trim()); //发货号        

                dic.Add("kh", textBox14.Text.ToString().Trim());// 客户名称
                dic.Add("xmmc", textBox15.Text.ToString().Trim());// 项目名称

                dic.Add("资产编码起", textBox5.Text.ToString().Trim());
                dic.Add("本箱编号起", textBox2.Text.Trim().ToString());
                dic.Add("本箱编号止", textBox1.Text.Trim().ToString());
                dic.Add("资产编码止", textBox6.Text.Trim().ToString());

                //dic.Add("本箱编号止", str_本箱编号止); 

                dic.Add("fhsl", textBox12.Text.Trim().ToString()); //发货数量
                dic.Add("xzsl", textBox10.Text.Trim().ToString()); //发货数量
                dic.Add("xc", textBox13.Text.Trim().ToString());
                dic.Add("总箱数", textBox11.Text.Trim().ToString());
            }
                Lprinter lP_1 = new Lprinter(path, dic, str_printer箱贴, 1);
                lP_1.DoWork();

           
        }
#pragma warning disable IDE1006 // 命名样式
        private void work()
#pragma warning restore IDE1006 // 命名样式
        {
            int i = Convert.ToInt32(textBox13.Text);
            int j = Convert.ToInt32(textBox11.Text);//总箱数

            for (; i <= j; i++)
            {
                if (bl_停止)
                {
                    break;
                }
                fun_print();
                 BeginInvoke(new MethodInvoker(() =>
                {
                     textBox13.Text = (Convert.ToInt32(textBox13.Text) + 1).ToString();
                }));
            }
            bl_停止 = false;
              BeginInvoke(new MethodInvoker(() =>
                {
                    button2.Visible = false;
                }));
              flag = false;  // 指示是否正在打印中  打印结束 状态 恢复
        }
#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (flag==false)  
            {
                Thread BG = new Thread(work);
                BG.IsBackground = true;
                BG.Start();
                button2.Visible = true;
                flag = true;  //指示是否正在打印
            }
            else
            {
                MessageBox.Show("正在打印中,请稍后");
            }
          
        }

#pragma warning disable IDE1006 // 命名样式
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }
        
#pragma warning disable IDE1006 // 命名样式
        private void button2_Click_1(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            bl_停止 = true;
        }

          

    }
}
