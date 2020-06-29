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
    public partial class ui云浮包装打印 : UserControl
    {
        #region 变量


        string strcon = CPublic.Var.strConn;

        string str_printer箱贴 = "";
        string str_printer盒贴 = "";
        List<String[]> x;
        bool bl_printer = false;

        DataTable dt_车间 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
        DataTable dt_未打印;

        /// <summary>
        /// 标记 是否正在打印过程中
        /// </summary>
        bool flag = false;
        /// <summary>


        int i_箱次 = 0;
        int i_总箱数 = 0;
        int i_总盒数 = 0;
        int In_发货数量 = 0;
        int i_余数 = 0;
        int i_余数_盒贴 = 0;

        int i_箱装数量 = 0;
        int i_盒装数量 = 0;
        #endregion


        public ui云浮包装打印()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui云浮包装打印_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            string path = Application.StartupPath + string.Format(@"\打印机配置.txt");
            x = ERPorg.Corg.ReadTxt(path);
            str_printer箱贴 = x[0][0].ToString();
            str_printer盒贴 = x[1][0].ToString();
            bl_printer = true;

            //fun_config_printer();
            fun_未打印工单();
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_未打印工单()
#pragma warning restore IDE1006 // 命名样式
        {

            string sql = string.Format(@"select 销售箱贴信息维护表.销售订单明细号,合同号,发货号,合同名称,项目编号,销售箱贴信息维护表.项目名称,极数,电压,产品名称,已打印
	,电流,物料号,销售箱贴信息维护表.规格型号,箱装数量,资产编码起,资产编码止,客户,销售记录销售订单明细表.数量 from  销售箱贴信息维护表
 left  join  销售记录销售订单明细表 on 销售记录销售订单明细表.销售订单明细号=销售箱贴信息维护表.销售订单明细号
       left join 销售记录销售订单主表 on 销售记录销售订单明细表.销售订单号=销售记录销售订单主表.销售订单号
          where  销售记录销售订单明细表.关闭=0  and 销售记录销售订单明细表.作废=0 and 销售记录销售订单明细表.明细完成=0
            and 销售记录销售订单主表.作废=0 and  销售记录销售订单主表.关闭=0   and 销售记录销售订单主表.客户编号 ='005107'");  //广州供电局 2018-2-26 个项目可能都不相同 一个客户一个单独模块
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dt_未打印 = new DataTable();
                da.Fill(dt_未打印);
                gridControl1.DataSource = dt_未打印;
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
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            dataBindHelper1.DataFormDR(dr);
            textBox10.Text = Convert.ToInt32(dr["数量"]).ToString("0");
            textBox8.Text = dr["箱装数量"].ToString();
            string s = string.Format("  select * from  [BQ_HZXX] where wlbh='{0}'", dr["物料号"].ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                textBox6.Text = dt.Rows[0]["eddy"].ToString();
                textBox14.Text = dt.Rows[0]["cpmc"].ToString();
                textBox1.Text = dt.Rows[0]["hzsl"].ToString();
                textBox2.Text = dt.Rows[0]["cpxh"].ToString(); 
            }
        }
        //设置打印机 
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            if (textBox10.Text == "") //发货数量
            {
                throw new Exception("发货数量不能为空");
            }
            if (textBox7.Text == "") //工单号 
            {
                throw new Exception("工单号不能为空");
            }
            if (textBox1.Text == "") //工单号 
            {
                throw new Exception("盒装数量不能为空");
            }
            if (textBox8.Text == "") //工单号 
            {
                throw new Exception("箱装数量不能为空");
            }
        }
        ///// <summary>
        ///// 单过程 打印 打印盒贴 和 一个箱贴
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
                str_printer盒贴 = x[1][0].ToString();
                bl_printer = true;
            }
            i_箱次 = i_箱次 + 1;
            #region
            string path_盒贴 = Application.StartupPath + string.Format(@"\Mode\通用模板.lab");
            //BeginInvoke(new MethodInvoker(() =>
            //{
            //    label7.Text = "正在打印盒贴";
            //}));



            int j = 0;
            if (i_箱次 == i_总箱数 && i_余数 != 0)
            {
                j = i_余数;
            }
            else
            {
                j = i_箱装数量;
            }


            if (i_余数_盒贴 == 0)
            {
                Dictionary<string, string> dic_盒贴 = new Dictionary<string, string>();
                dic_盒贴.Add("cpmc", textBox14.Text);
                dic_盒贴.Add("cpxh", textBox2.Text);
                dic_盒贴.Add("eddy", textBox6.Text);
                dic_盒贴.Add("wlbh", textBox12.Text);
                dic_盒贴.Add("jgddh", textBox7.Text);
                dic_盒贴.Add("jyy", "9009");
                dic_盒贴.Add("jyrq", CPublic.Var.getDatetime().ToString("yyyy-MM-dd"));
                dic_盒贴.Add("hzsl", i_盒装数量.ToString()); 
                Lprinter lp = new Lprinter(path_盒贴, dic_盒贴, str_printer盒贴, i_总盒数);
                lp.DoWork();
            }
            else
            {
                if (i_总盒数 == 1)
                {

                    Dictionary<string, string> dic_盒贴 = new Dictionary<string, string>();
                    dic_盒贴.Add("cpmc", textBox14.Text);
                    dic_盒贴.Add("cpxh", textBox2.Text);
                    dic_盒贴.Add("eddy", textBox6.Text);
                    dic_盒贴.Add("wlbh", textBox12.Text);
                    dic_盒贴.Add("jgddh", textBox7.Text);
                    dic_盒贴.Add("jyy", "9009");
                    dic_盒贴.Add("jyrq", CPublic.Var.getDatetime().ToString("yyyy-MM-dd"));
                    dic_盒贴.Add("hzsl", i_余数_盒贴.ToString());
                    Lprinter lp = new Lprinter(path_盒贴, dic_盒贴, str_printer盒贴, 1);
                    lp.DoWork();
                }
                else
                {
                    Dictionary<string, string> dic_盒贴 = new Dictionary<string, string>();
                    dic_盒贴.Add("cpmc", textBox14.Text);
                    dic_盒贴.Add("cpxh", textBox2.Text);
                    dic_盒贴.Add("eddy", textBox6.Text);
                    dic_盒贴.Add("wlbh", textBox12.Text);
                    dic_盒贴.Add("jgddh", textBox7.Text);
                    dic_盒贴.Add("jyy", "9009");
                    dic_盒贴.Add("jyrq", CPublic.Var.getDatetime().ToString("yyyy-MM-dd"));
                    dic_盒贴.Add("hzsl", i_盒装数量.ToString());
                    Lprinter lp = new Lprinter(path_盒贴, dic_盒贴, str_printer盒贴, i_总盒数 - 1);
                    lp.DoWork();
                    dic_盒贴 = new Dictionary<string, string>();
                    dic_盒贴.Add("cpmc", textBox14.Text);
                    dic_盒贴.Add("cpxh", textBox2.Text);
                    dic_盒贴.Add("eddy", textBox6.Text);
                    dic_盒贴.Add("wlbh", textBox12.Text);
                    dic_盒贴.Add("jgddh", textBox7.Text);
                    dic_盒贴.Add("jyy", "9009");
                    dic_盒贴.Add("jyrq", CPublic.Var.getDatetime().ToString("yyyy-MM-dd"));
                    dic_盒贴.Add("hzsl", i_余数_盒贴.ToString());
                    lp = new Lprinter(path_盒贴, dic_盒贴, str_printer盒贴, 1);
                    lp.DoWork();







                }

            }

            #endregion


            #region 再打 一个箱贴
            // DataRow dr = dt_dy.Rows[0];

            //BeginInvoke(new MethodInvoker(() =>
            //{
            //    label7.Text = string.Format("正在打印箱贴(第{0}箱)。。。", i_箱次);

            //}));
            string path = Application.StartupPath + string.Format(@"\Mode\云浮供电局.lab");
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ys", i_余数.ToString().Trim());// 客户名称
            dic.Add("xzs", i_箱装数量.ToString().Trim());// 客户名称
            dic.Add("cpmc", textBox4.Text.Trim());// 客户名称

            dic.Add("kh", textBox9.Text.ToString().Trim());// 客户名称
            dic.Add("xmmc", textBox15.Text.ToString().Trim());// 项目名称
            dic.Add("hth", textBox13.Text.ToString().Trim()); //合同号        
            dic.Add("fhsl", In_发货数量.ToString()); //发货数量
            //dic.Add("xzsl", i_箱装数量.ToString());
            dic.Add("总箱数", i_总箱数.ToString());
            Lprinter lP_1 = new Lprinter(path, dic, str_printer箱贴, i_总箱数);
            lP_1.DoWork();



            flag = false;
            #endregion

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
                    //simpleButton1.Visible = true;

                    fun_check();
                    In_发货数量 = Convert.ToInt32(textBox10.Text);
                    i_余数 = 0;
                    i_余数_盒贴 = 0;
                    i_箱装数量 = 0;
                    i_箱次 = 0;
                    i_箱装数量 = Convert.ToInt32(textBox8.Text);
                    i_盒装数量 = Convert.ToInt32(textBox1.Text);

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
                    if (In_发货数量 % i_盒装数量 == 0)
                    {
                        i_总盒数 = In_发货数量 / i_盒装数量;
                    }
                    else
                    {
                        i_余数_盒贴 = In_发货数量 % i_盒装数量;
                        i_总盒数 = In_发货数量 / i_盒装数量 + 1;

                    }
                   
                    Thread BG = new Thread(fun_打印);
                    BG.IsBackground = true;
                    BG.Start();
                    string s = string.Format("update [销售箱贴信息维护表] set 已打印=1 where 销售订单明细号='{0}'",textBox3.Text.ToString().Trim());
                    CZMaster.MasterSQL.ExecuteSQL(s, strcon);
                    DataRow []r= dt_未打印.Select(string.Format("销售订单明细号='{0}'", textBox3.Text.ToString().Trim()));
                    r[0]["已打印"] = true;
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

        }
    }
}
