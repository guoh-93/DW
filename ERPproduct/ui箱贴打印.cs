using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Data.SqlClient;
using System.Threading;
using System.IO;

namespace ERPproduct
{
    public partial class ui箱贴打印 : UserControl
    {


        #region 变量


        string strcon = CPublic.Var.strConn;

        string str_printer箱贴 = "";
        string str_printer小标签 = "";
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

        public ui箱贴打印()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_load_下拉框()
#pragma warning restore IDE1006 // 命名样式
        {
       


        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            if (textBox10.Text == "") //发货数量
            {
                throw new Exception("发货数量不能为空");
            }
            //    else //发货数不为空 判断转换为数字有没有问题
            //    {
            //        try
            //        {
            //            decimal a = Convert.ToDecimal(textBox10.Text);
            //        }
            //        catch
            //        {
            //            throw new Exception("检查发货数量是否正确");
            //        }
            //    }
        }

        ///// <summary>
        ///// 单过程 打印 箱装数量个小标签 和 一个箱贴
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

            str_本箱编号起 = str + f_资产编码.ToString("000000");
            string path_小标贴 = "";
            if (textBox9.Text.Trim() == "广东电网有限责任公司茂名供电局")
            {
                path_小标贴 = Application.StartupPath + string.Format(@"\Mode\茂名资产码.lab");

                Dictionary<string, string> dic_小标贴 = new Dictionary<string, string>();
                dic_小标贴.Add("zcm", str);
                dic_小标贴.Add("cc", f_资产编码.ToString("000000"));
                
                if(textBox4.Text.Trim()=="2")
                {
                dic_小标贴.Add("js", "KD");

                }
                else 
                {
                dic_小标贴.Add("js","KS");
                }
                Lprinter lpx = new Lprinter(path_小标贴, dic_小标贴, str_printer小标签, j);
                lpx.DoWork();
                f_资产编码 = f_资产编码 + j;

            }
            else if (textBox9.Text.Trim() == "广州供电局有限公司")
            {
                path_小标贴 = Application.StartupPath + string.Format(@"\Mode\广州供电小标贴.lab");
            

                List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();

                for (int i = 1; i <= j; i++)
                {
                    Dictionary<string, string> dic_小标贴 = new Dictionary<string, string>();
                    string ss = str + f_资产编码.ToString("000000"); //资产编码
                    dic_小标贴.Add("zcbm", ss);
                    f_资产编码 = f_资产编码 + 1;
                    li.Add(dic_小标贴);

                }
                Lprinter lp = new Lprinter(path_小标贴, li, str_printer小标签, 1);
                lp.DoWork();
            }
            BeginInvoke(new MethodInvoker(() =>
              {
                  label7.Text = "正在打印小标签。。。改";
              }));

          

            string str_本箱编号止 = "";

            str_本箱编号止 = str + (f_资产编码 - 1).ToString("000000");


            #endregion


            #region 再打 一个箱贴
            // DataRow dr = dt_dy.Rows[0];

            BeginInvoke(new MethodInvoker(() =>
            {
                label7.Text = string.Format("正在打印箱贴(第{0}箱)。。。", i_箱次);

            }));
            string path = Application.StartupPath + string.Format(@"\Mode\广州供电局.lab");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string dy = textBox6.Text.ToString().Trim();
            dy = dy.Substring(0, dy.Length - 1);
            dic.Add("dy", dy); //电压 
            string dl = textBox12.Text.ToString().Trim();
            dl = dl.Substring(0, dl.Length - 1);
            dic.Add("dl", dl); //电流

            dic.Add("ggxh", textBox2.Text.ToString().Trim());// 规格型号
            dic.Add("kh", textBox9.Text.ToString().Trim());// 客户名称
            dic.Add("xmmc", textBox15.Text.ToString().Trim());// 项目名称

            dic.Add("js", textBox4.Text.ToString().Trim()); //极数
            dic.Add("hth", textBox13.Text.ToString().Trim()); //合同号        
            dic.Add("fhh", textBox14.Text.ToString().Trim()); //发货号

            dic.Add("资产编码起", textBox1.Text.ToString().Trim());
            dic.Add("本箱编号起", str_本箱编号起);

            dic.Add("资产编码止", textBox7.Text.Trim().ToString());

            //dic.Add("本箱编号止", str_本箱编号止);
            //dic.Add("fhsl", j.ToString()); //发货数量

            dic.Add("fhsl", In_发货数量.ToString()); //发货数量
            dic.Add("xc", i_箱次.ToString());
            //dic.Add("箱次", "1");

            dic.Add("总箱数", i_总箱数.ToString());


            if (i_箱次 == i_总箱数 && i_余数 != 0) //打到最后一箱 且有余数 则 xzsl 赋余数  flag 赋为false
            {
              
                dic.Add("xzsl", i_余数.ToString());
                flag = false;
                BeginInvoke(new MethodInvoker(() =>
                 {
                     simpleButton1.Visible = false;
                 }));
                dic.Add("本箱编号止", str_资产编号止);

            }
            else 
            {
                dic.Add("xzsl", textBox8.Text.ToString());
                dic.Add("本箱编号止", str_本箱编号止);

            }
            //18-4-25 增 尽量不动上面的
             DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (i_箱次 == i_总箱数)
            {
                string s = string.Format("select  * from 销售箱贴信息维护表 where 销售订单明细号='{0}' ",dr["销售订单明细号"]);
                //dr["已打印"] = 2; //2表示已打完
                using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                {
                    DataTable t=new DataTable ();
                    da.Fill(t);
                    t.Rows[0]["已打印"] = 2;
                    new SqlCommandBuilder(da);
                    da.Update(t);
                }
            }
            else if(Convert.ToInt32(dr["已打印"])==0)
            {
                string s = string.Format("select  * from 销售箱贴信息维护表 where 销售订单明细号='{0}' ", dr["销售订单明细号"]);
                //dr["已打印"] =1; //1表示已打过 未打完
                using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                {
                    DataTable t = new DataTable();
                    da.Fill(t);
                    t.Rows[0]["已打印"] = 1;
                    new SqlCommandBuilder(da);
                    da.Update(t);
                }
            }
            method(gridControl1, gd => fun_refresh());
            Lprinter lP_1 = new Lprinter(path, dic, str_printer箱贴, 1);
            lP_1.DoWork();
            flag = false;
            #endregion

        }
#pragma warning disable IDE1006 // 命名样式
        private void method<T>(T c, Action<T> action) where T : DevExpress.XtraGrid.GridControl
#pragma warning restore IDE1006 // 命名样式
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => action(c)));
            }
            else
                action(c);
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_refresh()
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string s = string.Format("select  * from 销售箱贴信息维护表 where 销售订单明细号='{0}' ", dr["销售订单明细号"]);
            using (SqlDataAdapter da = new SqlDataAdapter(s,strcon))
            {
                DataTable dt=new DataTable ();
                da.Fill(dt);
                dr["已打印"] = dt.Rows[0]["已打印"];
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_未打印工单()
#pragma warning restore IDE1006 // 命名样式
        {

            string sql = string.Format(@"select 销售箱贴信息维护表.销售订单明细号,合同号,发货号,合同名称,项目编号,销售箱贴信息维护表.项目名称,极数,电压,已打印,
	电流,物料号,销售箱贴信息维护表.规格型号,箱装数量,资产编码起,资产编码止,客户,销售记录销售订单明细表.数量 from  销售箱贴信息维护表
 left  join  销售记录销售订单明细表 on 销售记录销售订单明细表.销售订单明细号=销售箱贴信息维护表.销售订单明细号
       left join 销售记录销售订单主表 on 销售记录销售订单明细表.销售订单号=销售记录销售订单主表.销售订单号
          where  销售记录销售订单明细表.关闭=0  and 销售记录销售订单明细表.作废=0 and 销售记录销售订单明细表.明细完成=0
            and 销售记录销售订单主表.作废=0 and  销售记录销售订单主表.关闭=0   and 销售记录销售订单主表.客户编号 in ('004765','005112' )");  //广州供电局 2018-2-26 个项目可能都不相同 一个客户一个单独模块
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon)) 
            {
                dt_未打印 = new DataTable();
                da.Fill(dt_未打印);
                gridControl1.DataSource = dt_未打印;
            }
        }
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
            

                str = textBox1.Text.Substring(0, textBox1.Text.Trim().Length-6);

             
                f_资产编码 = Convert.ToDouble(textBox1.Text.Substring(textBox1.Text.Trim().Length - 6, 6));


                str_资产编号止 = str + (f_资产编码 + Convert.ToDouble(textBox10.Text) - 1).ToString("000000"); //保证是 六位 前面不足的 0 补足
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

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (flag)
            {
                MessageBox.Show("正在打印,请不要关闭");
            }
            else
            {
                CPublic.UIcontrol.ClosePage();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui箱贴打印_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.KeyValue == 13 && flag == true) //enter 
            {
                fun_打印();
            }
        }

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

#pragma warning disable IDE1006 // 命名样式
        private void ui箱贴打印_KeyUp(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

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

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.KeyValue == 13 && flag == true) //enter 
            {
                fun_打印();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui箱贴打印_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //textBox13.Text = "0024HC1711190";  //合同号

            //textBox2.Text = "";  //产品型号

            //textBox1.Text = "08001XP00000000000750999";//编码起始号


            //textBox6.Text = "220V";//电压
            //textBox12.Text = "10A";//电流
            //textBox8.Text = "60";

            DateTime t = CPublic.Var.getDatetime();
            t = new DateTime(t.Year, t.Month, t.Day);
            barEditItem2.EditValue = t;
            barEditItem1.EditValue = t.AddMonths(-2);
            string path = Application.StartupPath + string.Format(@"\打印机配置.txt");
            x =ERPorg.Corg.ReadTxt(path);
            str_printer箱贴 = x[0][0].ToString();
            str_printer小标签 = x[1][0].ToString();
            bl_printer = true;

            //fun_config_printer();
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

#pragma warning disable IDE1006 // 命名样式
        private void label7_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            fm补打箱贴标签 fm = new fm补打箱贴标签(dr["资产编码起"].ToString(),dr["客户"].ToString().Trim());
            fm.StartPosition = FormStartPosition.CenterScreen;
            fm.ShowDialog();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            fm重打单个箱贴 fm = new fm重打单个箱贴(dr);
            fm.StartPosition = FormStartPosition.CenterScreen;
            fm.ShowDialog();
        }

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

                str = textBox1.Text.Substring(0, textBox1.Text.Trim().Length-6);
                f_资产编码 = Convert.ToDouble(textBox1.Text.Substring(textBox1.Text.Trim().Length - 6, 6)); //起始 六位
                str_资产编号止 = str + (f_资产编码 + Convert.ToDouble(textBox10.Text) - 1).ToString("000000");

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
       

#pragma warning disable IDE1006 // 命名样式
        private void fun_config_printer()
#pragma warning restore IDE1006 // 命名样式
        {
            StreamReader sr = new StreamReader(Application.StartupPath + string.Format(@"\打印机配置.txt"), Encoding.Default);
            string s;
            int i = 0;
            while ((s = sr.ReadLine()) != null)
            {
                if (i == 0)
                {
                    str_printer箱贴 = s;

                }
                else if (i == 1)
                {
                    str_printer小标签 = s;
                }
                else
                {
                    break;
                }
                i++;
            }
            sr.Close();
        }
#pragma warning disable IDE1006 // 命名样式
        private void textBox5_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void label1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox15_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
              

                if (gridView1.GetRow(e.RowHandle) == null)
                {
                    return;
                }

                if (gridView1.GetRowCellValue(e.RowHandle, "已打印").ToString() == "1")
                {
                    e.Appearance.BackColor = Color.Pink;
            
                }
                else if(gridView1.GetRowCellValue(e.RowHandle, "已打印").ToString() == "2")
                {
                    e.Appearance.BackColor = Color.Yellow;
 
                }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



    }
}
