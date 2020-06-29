using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.IO;


namespace ERPproduct
{
    public partial class ui重庆日新箱贴 : UserControl
    {
        #region 变量


        string strcon = CPublic.Var.strConn;

        string str_printer箱贴 = "";
        string str_printer小标签 = "";
        string str_printer箱贴1 = "";

        List<String[]> x;
        bool bl_printer = false;
        DataTable dt_工单;
        DataTable dt_dy;
        DataTable dt_历史记录;
        string sql_历史记录 = "";
        string str_车间;
        DataTable dt_车间 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
        DataTable dt_未打印;

        /// <summary>
        /// 标记 是否正在打印过程中
        /// </summary>
        bool flag = false;
        /// <summary>
        /// 记录当前打印的编码号,中断后可以继续 资产编码 后 6 位
        /// </summary>
        static double f_资产编码 = 0;
        /// <summary>
        /// 资产编码 前半部分 
        /// </summary>
        string str = "";
        int i_箱次 = 0;
        int i_总箱数 = 0;
        int In_发货数量 = 0;
        int i_余数 = 0;
        int i_箱装数量 = 0;
        string str_资产编号止 = "";
        #endregion




        public ui重庆日新箱贴()
        {
            InitializeComponent();
        }
        //设置打印机
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();

            this.printDialog1.Document = this.printDocument1;
            DialogResult dr = this.printDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string str = this.printDocument1.PrinterSettings.PrinterName;
                string file = Application.StartupPath + string.Format(@"\打印机配置.txt");

                if (File.Exists(file) == true)
                {

                    using (StreamWriter SW = File.AppendText(file))
                    {
                        SW.WriteLine(str + '\n');
                        SW.Close();
                    }
                }
                else
                {
                    FileStream myFs = new FileStream(file, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs);
                    mySw.Write(str);
                    mySw.Close();
                    myFs.Close();
                }


            }
        }
        //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_未打印工单();

                simpleButton1.Visible = false;
                i_箱次 = 0;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
        //打印
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                if (flag == false)
                {
                    // flag = true;   //标记为 正在打印中 则可以 按Enter  继续下一次打印  
                    simpleButton1.Visible = true;

                    fun_check();
                    In_发货数量 = Convert.ToInt32(textBox10.Text);
                    i_余数 = 0;
                    i_箱装数量 = 0;
                    i_箱次 = 0;
                    i_箱装数量 = Convert.ToInt32(textBox8.Text);


                    str = textBox1.Text.Substring(0, textBox1.Text.Trim().Length - 7);


                    f_资产编码 = Convert.ToDouble(textBox1.Text.Substring(textBox1.Text.Trim().Length - 7,7));


                    str_资产编号止 = str + (f_资产编码 + Convert.ToDouble(textBox10.Text) - 1).ToString().PadLeft(7, '0'); //保证是 六位 前面不足的 0 补足
                    //打印份数

                    if (In_发货数量 % i_箱装数量 == 0)
                    {
                        i_总箱数 = In_发货数量 / i_箱装数量;
                    }
                    else
                    {
                        i_余数 = In_发货数量 % i_箱装数量;
                        i_总箱数 = In_发货数量 / i_箱装数量 + 1;
                    }


                    Thread BG = new Thread(fun_打印);
                    BG.IsBackground = true;
                    BG.Start();

                    flag = true;  //指示是否正在打印
                }
                else
                {
                    MessageBox.Show("正在打印中,请稍后");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //重打单个小标签
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            fm补打箱贴标签 fm = new fm补打箱贴标签(dr["资产编码起"].ToString(), dr["客户"].ToString().Trim());
            fm.StartPosition = FormStartPosition.CenterScreen;
            fm.ShowDialog();
        }
        //重打单个箱贴
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            fm重打单个箱贴 fm = new fm重打单个箱贴(dr);
            fm.StartPosition = FormStartPosition.CenterScreen;
            fm.ShowDialog();
        }
        //继续打印
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (flag == false)
            {
                Thread BG = new Thread(fun_打印);
                BG.IsBackground = true;
                BG.Start();

                flag = true;  //指示是否正在打印
            }
            else
            {
                MessageBox.Show("正在打印中,请稍后");
            }

        }
        //从第几箱开始
#pragma warning disable IDE1006 // 命名样式
        private void simpleButton2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (flag == false)
                {

                    // flag = true;   //标记为 正在打印中 则可以 按Enter  继续下一次打印  
                    simpleButton1.Visible = true;
                    i_箱装数量 = 0;
                    i_余数 = 0;
                    i_箱装数量 = Convert.ToInt32(textBox8.Text);
                    In_发货数量 = Convert.ToInt32(textBox10.Text);
                    if (In_发货数量 % i_箱装数量 == 0)
                    {
                        i_总箱数 = In_发货数量 / i_箱装数量;
                    }
                    else
                    {
                        i_余数 = In_发货数量 % i_箱装数量;
                        i_总箱数 = In_发货数量 / i_箱装数量 + 1;
                    }
                    if (textBox5.Text == "")
                    {
                        throw new Exception("未输入数值");
                    }
                    i_箱次 = Convert.ToInt32(textBox5.Text);

                    str = textBox1.Text.Substring(0, textBox1.Text.Trim().Length - 7);
                    f_资产编码 = Convert.ToDouble(textBox1.Text.Substring(textBox1.Text.Trim().Length - 7, 7)); //起始 六位
                    str_资产编号止 = str + (f_资产编码 + Convert.ToDouble(textBox10.Text) - 1).ToString().PadLeft(7, '0');

                    textBox7.Text = str_资产编号止;
                    f_资产编码 = (f_资产编码 + i_箱装数量 * (i_箱次 - 1));
                    i_箱次 = i_箱次 - 1;

                    Thread BG = new Thread(fun_打印);
                    BG.IsBackground = true;
                    BG.Start();

                    flag = true;  //指示是否正在打印
                }
                else
                {
                    MessageBox.Show("正在打印中,请稍后");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        #region  函数
#pragma warning disable IDE1006 // 命名样式
        private void fun_未打印工单()
#pragma warning restore IDE1006 // 命名样式
        {

            string sql = string.Format(@"select 销售箱贴信息维护表.销售订单明细号,合同号,发货号,合同名称,项目编号,销售箱贴信息维护表.项目名称,极数,电压,
	 电流,物料号,销售箱贴信息维护表.规格型号,箱装数量,资产编码起,资产编码止,客户,销售记录销售订单明细表.数量 from  销售箱贴信息维护表
     left  join  销售记录销售订单明细表 on 销售记录销售订单明细表.销售订单明细号=销售箱贴信息维护表.销售订单明细号
       left join 销售记录销售订单主表 on 销售记录销售订单明细表.销售订单号=销售记录销售订单主表.销售订单号
          where  销售记录销售订单明细表.关闭=0  and 销售记录销售订单明细表.作废=0 and 销售记录销售订单明细表.明细完成=0
            and 销售记录销售订单主表.作废=0 and  销售记录销售订单主表.关闭=0   and 销售记录销售订单主表.客户编号 ='004525'"); 
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dt_未打印 = new DataTable();
                da.Fill(dt_未打印);
                gridControl1.DataSource = dt_未打印;
            }
        }



#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            if (textBox10.Text == "") //发货数量
            {
                throw new Exception("发货数量不能为空");
            }
 
        }


        ///// <summary>
        ///// 单过程 打印 箱装数量个小标签 和 2个箱贴
        ///// </summary>
#pragma warning disable IDE1006 // 命名样式
        private void fun_打印()
#pragma warning restore IDE1006 // 命名样式
        {

            if (bl_printer == false)
            {
                string path_printer = Application.StartupPath + string.Format(@"\打印机配置.txt");
                x = ERPorg.Corg.ReadTxt(path_printer);
                str_printer箱贴 = x[0][0].ToString();
                str_printer小标签 = x[1][0].ToString();
                str_printer箱贴1 = x[2][0].ToString();
                bl_printer = true;
            }
            i_箱次 = i_箱次 + 1;
            #region 先打箱装数量个小标贴

            int j = 0;
            if (i_箱次 == i_总箱数 && i_余数 != 0)
            {
                j = i_余数;
            }
            else
            {
                j = i_箱装数量;
            }
            string str_本箱编号起 = "";

            str_本箱编号起 = str + f_资产编码.ToString().PadLeft(7,'0');
            string path_小标贴 = Application.StartupPath + string.Format(@"\Mode\重庆日新资产码.lab");
            List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
            //这里需要打两个
            BeginInvoke(new MethodInvoker(() =>
            {
                label7.Text = "正在打印小标签。。。";
            }));
            for (int i = 1; i <= j; i++)
            {
                Dictionary<string, string> dic_小标贴 = new Dictionary<string, string>();
                string ss = str + f_资产编码.ToString().PadLeft(7, '0');//资产编码
                string jy= ERPorg.Corg.fun_gccode(ss);
                dic_小标贴.Add("zcbm", ss+jy);
                f_资产编码 = f_资产编码 + 1;
                li.Add(dic_小标贴);
            }
            Lprinter lp = new Lprinter(path_小标贴, li, str_printer小标签,2);
            lp.DoWork();
 
       



            string str_本箱编号止 = "";

            str_本箱编号止 = str + (f_资产编码 - 1).ToString().PadLeft(7, '0');


            #endregion


            #region 再打 一个箱贴
            // DataRow dr = dt_dy.Rows[0];

            BeginInvoke(new MethodInvoker(() =>
            {
                label7.Text = string.Format("正在打印箱贴(第{0}箱)。。。", i_箱次);

            }));
            string path = Application.StartupPath + string.Format(@"\Mode\重庆电力(日新).lab");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string dy = textBox6.Text.ToString().Trim();
            dy = dy.Substring(0, dy.Length - 1);
            dic.Add("dy", dy); //电压 
            string dl = textBox12.Text.ToString().Trim();
            dl = dl.Substring(0, dl.Length - 1);
            dic.Add("dl", dl); //电流

            dic.Add("cpxh", textBox2.Text.ToString().Trim());// 规格型号
     
            if (textBox4.Text.Trim() == "2")
            {
                dic.Add("js", "单相"); //极数
            }
            else
            {
                dic.Add("js", "三相"); //极数

            }
            
            dic.Add("pch", textBox14.Text.ToString().Trim()); //对应批次号

            string s_jy = ERPorg.Corg.fun_gccode(str_本箱编号起);
            dic.Add("qsm", str_本箱编号起 + s_jy);

            s_jy = ERPorg.Corg.fun_gccode(str_本箱编号止);
            dic.Add("jsm", str_本箱编号止+s_jy); 

       
            dic.Add("xc", i_箱次.ToString());
       

            dic.Add("zxs", i_总箱数.ToString());
            dic.Add("ys", i_余数.ToString());
            dic.Add("xzsl", textBox8.Text.ToString());

     
           


            Lprinter lP_1 = new Lprinter(path, dic, str_printer箱贴, 1);
            lP_1.DoWork();
            flag = false;
            #endregion



            #region 再打一个 外箱条码标签  批次号+箱次
              path = Application.StartupPath + string.Format(@"\Mode\日新外箱条码.lab");
              Dictionary<string, string> dic2 = new Dictionary<string, string>();
              string wxtm = textBox14.Text.ToString().Trim() + i_箱次.ToString("0000");
              dic2.Add("wxtm", wxtm);
              Lprinter lP_2 = new Lprinter(path, dic2, str_printer箱贴1, 1);
              lP_2.DoWork();
            #endregion


              flag = false;
              if (i_箱次==i_总箱数)
              BeginInvoke(new MethodInvoker(() =>
              {
                  simpleButton1.Visible = false;
              }));
        }
        #endregion 

#pragma warning disable IDE1006 // 命名样式
        private void ui重庆日新箱贴_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

    
            string path = Application.StartupPath + string.Format(@"\打印机配置.txt");
            x = ERPorg.Corg.ReadTxt(path);
            str_printer箱贴 = x[0][0].ToString();
            str_printer小标签 = x[1][0].ToString();
            str_printer箱贴1 = x[2][0].ToString();

            bl_printer = true;
            fun_未打印工单();
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            dataBindHelper1.DataFormDR(dr);
            textBox10.Text = Convert.ToInt32(dr["数量"]).ToString("0");
            textBox8.Text = dr["箱装数量"].ToString();
        }






    }
}
