using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.IO;


namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class fm补打箱贴标签 : Form
#pragma warning restore IDE1006 // 命名样式
    {

        string str_printer小标签 = new PrintDocument().PrinterSettings.PrinterName;
        string s_资产编码 = "";
        string kh = "";
        DataRow dr;
        /// <summary>
        /// 最小包装
        /// </summary>
        public int zxbz = 0;
        public fm补打箱贴标签(string sxx,string kh)
        {
            InitializeComponent();
            this.Text = "补打箱贴标签";
            s_资产编码 = sxx.Substring(0, sxx.Length -7);
            this.kh = kh;
        }
        /// <summary>
        /// 制六课 工单确认用来维护包装数量
        /// zxbz 现存的最小包装数
        /// </summary>
        public fm补打箱贴标签(DataRow dr )
        {
            InitializeComponent();
            this.dr = dr;
            this.Text = "确认物料最小包装量";

            simpleButton1.Text = "确认";
            label1.Text = string.Format("物料:{0},{1}", dr["原ERP物料编号"].ToString(), dr["原规格型号"].ToString()); //dr["图纸编号"].ToString()
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            if (textBox1.Text.Length != 7)
            {
                throw new Exception("请输入七位");
            }


        }
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dr == null)
                {
                    fun_check();
                    string path_小标贴 = "";
                    Dictionary<string, string> dic_小标贴 = new Dictionary<string, string>();
                    string ss = s_资产编码 + textBox1.Text.ToString().Trim(); //资产编码

                    if (kh == "广东电网有限责任公司茂名供电局")
                    {
                        path_小标贴 = Application.StartupPath + string.Format(@"\Mode\茂名资产码.lab");

                        dic_小标贴.Add("zcm", s_资产编码);
                        dic_小标贴.Add("cc", textBox1.Text.ToString().Trim());
                        if (s_资产编码.Substring(0, 2) == "DD")
                        {
                            dic_小标贴.Add("js", "KD");

                        }
                        else
                        {
                            dic_小标贴.Add("js", "KS");
                        }

                    }
                    else if (kh == "广州供电局有限公司")
                    {
                        path_小标贴 = Application.StartupPath + string.Format(@"\Mode\广州供电小标贴.lab");
                        dic_小标贴.Add("zcbm", ss);
                    }
                    //path_小标贴 = Application.StartupPath + string.Format(@"\Mode\广州供电小标贴.lab");
                    
                   // string ss = "08001XP0000000000" + textBox1.Text.ToString().Trim(); //资产编码
                    //18-1-20  以前临时要加这个功能 
                    //08001XP00000000001092718
                  
                    Lprinter lp = new Lprinter(path_小标贴, dic_小标贴, str_printer小标签, 1);
                    lp.DoWork();
           
                }
                else
                {
                    if (MessageBox.Show("再次核实最小包装数量,是否确认", "确认？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        int number;
                        if (!int.TryParse(textBox1.Text, out number))
                        {
                            throw new Exception("输入不为整数");
                        }
                        else if (number <= 0)
                        {
                            throw new Exception("不可输入小于等于0的整数");
                        }
                        zxbz = number;


                        this.Close();
                    }

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void fm补打箱贴标签_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            if (dr == null)
            {
                string path = Application.StartupPath + string.Format(@"\打印机配置.txt");
                List<string[]> x = ERPorg.Corg.ReadTxt(path);

                str_printer小标签 = x[1][0].ToString();

            }
  

        }
    }
}
