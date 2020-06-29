using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using System.Globalization;
using System.Data.SqlClient;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class ui主机铭牌打印 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        public ui主机铭牌打印()
        {
            InitializeComponent();
        }
        string strcon = CPublic.Var.strConn;
        /// <summary>
        /// 标记 是否正在打印过程中
        /// </summary>
        bool flag = false;
        Dictionary<Dictionary<string, string>, int> dic_打印队列;
        Dictionary<string, int> cache;
       
        string str_printer箱贴 = "";
 
#pragma warning disable IDE1006 // 命名样式
        private void label4_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {
                if (flag == false)
                {

               //     fun_check();

                    if (MessageBox.Show(string.Format("确定打印？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {


                        //PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();

                        //this.printDialog1.Document = this.printDocument1;
                        //DialogResult dr = this.printDialog1.ShowDialog();
                        //string PrinterName = this.printDocument1.PrinterSettings.PrinterName;


                        //str_printer箱贴 = PrinterName;

                     
                        label7.Text = "正在规划中,请稍候";
                        Thread BG = new Thread(() =>
                        {
                            fun_Givemath();
                            string path = Application.StartupPath + string.Format(@"\Mode\加密主机铭牌.lab"); ;
                            Lprinter lp = new Lprinter(path, dic_打印队列, str_printer箱贴);


                            lp.DoWork();
                            flag = false;
                            //xx = 1;
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                label7.Text = "";
                            }));
                        }


                        );
                        BG.IsBackground = true;
                        BG.Start();

                        flag = true;  //指示是否正在打印    

                    }
                }

                else
                {
                    MessageBox.Show("正在打印中,请稍候");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }




#pragma warning disable IDE1006 // 命名样式
        private void fun_Givemath()
#pragma warning restore IDE1006 // 命名样式
        {

            cache = new Dictionary<string, int>();
            dic_打印队列 = new Dictionary<Dictionary<string, string>, int>();
  
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("cpmc", textBox1.Text);
            dic.Add("cpxh", textBox2.Text);
            dic.Add("sydy", textBox3.Text);        
            dic.Add("srdl", textBox4.Text);
            dic.Add("srpl", textBox5.Text);

            dic_打印队列.Add(dic,Convert.ToInt32( textBox6.Text)); 
            BeginInvoke(new MethodInvoker(() =>
            {
                label7.Text = "打印中...";
            }));

        }

    }
}
