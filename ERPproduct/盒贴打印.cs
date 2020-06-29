using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Globalization;
namespace ERPproduct
{
    public partial class 盒贴打印 : UserControl
    {
        #region 变量
        int count = -1;
        string strcon = CPublic.Var.strConn;
        string str_打印机 = new PrintDocument().PrinterSettings.PrinterName;
        //string str_打印机 = "Adobe PDF";
        DataTable dt_工单;
        DataTable dt_dy;
        DataTable dt_历史记录;
        string sql_历史记录 = "";
        string str_车间;
        DataTable dt_车间 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
        DataTable dt_未打印;
        int flag = 0;
        #endregion

        #region 加载

        public 盒贴打印()
        {
            
            InitializeComponent();
            barEditItem1.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().AddDays(-4).ToString("yyyy-MM-dd"));
            barEditItem2.EditValue = Convert.ToDateTime(CPublic.Var.getDatetime().ToString("yyyy-MM-dd"));

        }
        private void 包装打印_Load(object sender, EventArgs e)
        {
            try
            {

                fun_load_下拉框();

                fun_未打印工单();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        #endregion

        #region   函数

#pragma warning disable IDE1006 // 命名样式
        private void fun_load_下拉框()
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime dtime_后 = Convert.ToDateTime(barEditItem2.EditValue).AddDays(1);

            //配合现场人员的要求 下拉框里留今天全部的 gridcontrol里 只显示 未打印的
            string pd="";
            if (CPublic.Var.LocalUserID != "admin")
            {

                pd = " and 生产车间='" + dt_车间.Rows[0]["生产车间"].ToString()+"'";
            }
            else
            {
                pd = "";
            }
            string sql_工单 = string.Format(@"select 生产检验单号,生产工单号,规格型号,物料编码,物料名称,生产数量,模板名称 from 生产记录生产检验单主表  
                              where   生效日期 >'{0}' and 生效日期< '{1}' {2} ", barEditItem1.EditValue, dtime_后,pd);
            using (SqlDataAdapter da = new SqlDataAdapter(sql_工单, strcon))
            {
                dt_工单 = new DataTable();
                da.Fill(dt_工单);
                searchLookUpEdit1.Properties.DataSource = dt_工单;
                searchLookUpEdit1.Properties.DisplayMember = "生产工单号";
                searchLookUpEdit1.Properties.ValueMember = "生产工单号";
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_未打印工单()
#pragma warning restore IDE1006 // 命名样式
        {
            string pd = "";
            if (CPublic.Var.LocalUserID != "admin")
            {

                pd = " and 生产车间='" + dt_车间.Rows[0]["生产车间"].ToString() + "'";
            }
            else
            {
                pd = "";
            }
            DateTime dtime_后 = Convert.ToDateTime(barEditItem2.EditValue).AddDays(1);
            string sql_工单 = string.Format(@"select 生产检验单号,生产工单号,jy.规格型号,jy.物料编码,jy.物料名称,生产数量,模板名称 from 生产记录生产检验单主表 jy,基础数据物料信息表 
            
                         where jy.物料编码=基础数据物料信息表.物料编码 and 基础数据物料信息表.是否有盒贴='是' {0} and  生效日期 >= '{1}' and 生效日期<= '{2}' and 已打印=0 ", pd, barEditItem1.EditValue, dtime_后);
            using (SqlDataAdapter da = new SqlDataAdapter(sql_工单, strcon))
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
            if (searchLookUpEdit1.EditValue == DBNull.Value || searchLookUpEdit1.EditValue == null)
            {
                throw new Exception("未选择打印信息");
            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_load_历史记录()
#pragma warning restore IDE1006 // 命名样式
        {
            DateTime time = Convert.ToDateTime(barEditItem2.EditValue);
            time = time.AddDays(1);
            sql_历史记录 = string.Format("select * from 生产_标签打印_历史记录 where  打印日期>='{0}' and 打印日期<'{1}'", barEditItem1.EditValue, time);
            using (SqlDataAdapter da = new SqlDataAdapter(sql_历史记录, strcon))
            {
                dt_历史记录 = new DataTable();
                da.Fill(dt_历史记录);
                gridControl1.DataSource = dt_历史记录;
            }



        }

        //  往历史记录表 里 村一条打印记录    ，并且在 检验单主表中 修改 “是否已打印”
#pragma warning disable IDE1006 // 命名样式
        private void fun_save_打印历史记录()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 生产_标签打印_历史记录 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                dr["GUID"] = System.Guid.NewGuid();
                dr["生产工单号"] = searchLookUpEdit1.EditValue;
                dr["打印人员"] = CPublic.Var.localUserName;
                dr["打印人员ID"] = CPublic.Var.LocalUserID;
                dr["打印日期"] = CPublic.Var.getDatetime() ;
                dr["打印数量"] = count;
                dr["打印模板"] = textBox3.Text;
                if (textBox2.Enabled == false)
                {
                    dr["打印方式"] = "正常打印";
                }
                else
                {
                    dr["打印方式"] = "返修打印";
                }
                new SqlCommandBuilder(da);
                da.Update(dt);
                if (flag == 1)
                {
                    string sql_1 = string.Format("select * from 生产记录生产检验单主表 where 生产工单号='{0}'", searchLookUpEdit1.EditValue);
                    using (SqlDataAdapter da_1 = new SqlDataAdapter(sql_1, strcon))
                    {
                        DataTable dt_1 = new DataTable();
                        da_1.Fill(dt_1);
                        if (dt_1.Rows.Count > 0)
                        {
                            foreach (DataRow drr in dt_1.Rows)
                            {
                                drr["已打印"] = true;
                            }
                            new SqlCommandBuilder(da_1);
                            da_1.Update(dt_1);
                        }


                    }


                }

                flag = 0;
            }
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_打印()
#pragma warning restore IDE1006 // 命名样式
        {
            int c_余数 = 0;
            count = -1;
            DataRow dr = dt_dy.Rows[0];
            int In_生产数量 = 0;
            if (textBox10.Visible == true)    // 手输 生产数量 
            {
               In_生产数量= Convert.ToInt32(textBox10.Text);
            }
            else
            {
                In_生产数量=Convert.ToInt32(dr["生产数量"]);
            }

           

            //打印份数
            if (dt_dy.Rows.Count > 0)
            {
                if (In_生产数量 % Convert.ToInt32(dr["盒装数量"]) == 0)
                {
                    count = In_生产数量 / Convert.ToInt32(dr["盒装数量"]);
                }
                else
                {
                    c_余数 = In_生产数量 % Convert.ToInt32(dr["盒装数量"]);
                    count = In_生产数量 / Convert.ToInt32(dr["盒装数量"]) + 1;
                }
            }
            textBox11.Text = count.ToString("0");
         
            string path = Application.StartupPath + string.Format(@"\Mode\{0}.lab", textBox3.Text.Trim());
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //默认 模板 都拥有 jgddh wlbh eddy khlh jyrq （常熟 两个模板 日期不一样）
            dic.Add("jgddh", dt_dy.Rows[0]["生产工单号"].ToString());
            dic.Add("wlbh", dt_dy.Rows[0]["原ERP物料编号"].ToString());

            dic.Add("eddy", dt_dy.Rows[0]["额定电压"].ToString());

            dic.Add("khlh", dt_dy.Rows[0]["客户料号"].ToString());

            dic.Add("cpxh", dt_dy.Rows[0]["产品型号"].ToString().Trim());
            dic.Add("hzsl", textBox8.Text.ToString());

            dic.Add("jyy", dt_dy.Rows[0]["检验人员ID"].ToString());
            dic.Add("jyrq", Convert.ToDateTime(dt_dy.Rows[0]["检验日期"]).ToString("yyyy-MM-dd"));

            if (textBox3.Text.Trim() == "诺雅克模板")
            {
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());
                if (count == 1)
                {
                    dic["hzsl"] = Convert.ToInt32(dr["生产数量"]).ToString();
                }


                dic.Remove("jgddh");

                dic.Remove("wlbh");


            }
            if (textBox3.Text.Trim() == "伊顿模板")
            {
                dic.Add("jz", dt_dy.Rows[0]["机种"].ToString());
                dic.Remove("wlbh");
     
                dic.Remove("jyy");

                dic["jyrq"] = Convert.ToDateTime(dt_dy.Rows[0]["检验日期"]).ToString("yyMMdd");
                //  jgddh  伊顿要求 是 401509 +   客户料号后六位+一位随机码
                string s= textBox9.Text; //客户料号
         
                dic["jgddh"] = "401509" + s.Substring(s.Length-6,6);


                //dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());
                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }

            }
            if (textBox3.Text.Trim() == "中性模板")
            {
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());
                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }

            }
            if (textBox3.Text.Trim() == "通用模板电流")
            {
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());

                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }
                dic["jgddh"] = dt_dy.Rows[0]["生产工单号"].ToString();
                dic["wlbh"] = dt_dy.Rows[0]["原ERP物料编号"].ToString();
                dic["eddy"] = dt_dy.Rows[0]["额定电压"].ToString();
                dic["khlh"] = dt_dy.Rows[0]["客户料号"].ToString();
                dic.Remove("jyrq");

               

            }
            if (textBox3.Text.Trim() == "辅助英文模板" || textBox3.Text.Trim() == "分励英文模板" || textBox3.Text.Trim() == "欠压英文模板" || textBox3.Text.Trim() == "辅报英文模板" || textBox3.Text.Trim() == "闭合英文模板" || textBox3.Text.Trim() == "报警英文模板")
            {
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());
                //17-10-11
                dic["khlh"] = textBox9.Text;
                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }
                dic.Remove("jgddh");
                dic.Remove("wlbh");
               // dic.Remove("khlh");
            }
            if (textBox3.Text.Trim() == "宁波施耐德")
            {
                dic["cpxh"] = dt_dy.Rows[0]["产品型号"].ToString();
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());
                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }
                dic.Remove("khlh");
                dic.Remove("eddy");

                dic.Add("scph", dt_dy.Rows[0]["生产工单号"].ToString());
            }
            if (textBox3.Text.Trim() == "温州德力西")
            {
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());
                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }
                dic.Remove("jgddh");

                dic.Remove("khlh");
                dic.Remove("eddy");


            }
            if (textBox3.Text.Trim() == "正泰模板")
            {
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());
                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }
                dic.Add("cs", dt_dy.Rows[0]["参数"].ToString());
                dic.Remove("jgddh");

                dic.Remove("wlbh");

            }
            if (textBox3.Text.Trim() == "台安模板" || textBox3.Text.Trim() == "芜湖德力西" || textBox3.Text.Trim() == "芜湖德力西英文")
            {
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());
                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }
                dic.Add("jz", dt_dy.Rows[0]["机种"].ToString());
                dic.Add("ddh", dt_dy.Rows[0]["订单号"].ToString());
                dic.Remove("jgddh");

                dic.Remove("wlbh");
            }

            if (textBox3.Text.Trim() == "常熟外发模板")
            {
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());

                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }
                dic.Remove("khlh");
                dic.Remove("eddy");
                dic.Remove("jgddh");
                dic.Remove("wlbh");
                dic.Remove("jyy");

                string ss = CPublic.Var.getDatetime().Year.ToString().Substring(2, 2);
                string rq = fun_date(Convert.ToDateTime(dt_dy.Rows[0]["检验日期"]).ToString("yyMM"));
                dic["jyrq"] = rq;
                //dic["jyrq"] = Convert.ToDateTime(dt_dy.Rows[0]["检验日期"]).ToString("yyMM");
            }
            if (textBox3.Text.Trim() == "正泰英文版")
            {
                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());

                dic.Remove("wlbh");
                dic.Remove("khlh");
                dic.Remove("jgddh");
            }
            if (textBox3.Text.Trim() == "良信模板")
            {
                GregorianCalendar gc = new GregorianCalendar();
                int week = gc.GetWeekOfYear(CPublic.Var.getDatetime(), CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                string ss = CPublic.Var.getDatetime().Year.ToString().Substring(2, 2);
                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }
                dic["jgddh"] = dt_dy.Rows[0]["生产工单号"].ToString() + "-25-" + week.ToString();
                dic["jyrq"] = ss + week.ToString();
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString().Trim());

                dic.Remove("eddy");
                dic.Remove("jyy");

            }

            if (textBox3.Text.Trim() == "宏美模板")
            {


               
                //int icount = (int)Convert.ToDecimal(textBox11.Text.ToString()) / Convert.ToInt32(textBox8.Text.ToString());// 生产数 除以 盒装数量 取整 
                int icount = In_生产数量 / Convert.ToInt32(textBox8.Text.ToString());
                int i_余数 = In_生产数量 % Convert.ToInt32(textBox8.Text.ToString());
                if (In_生产数量 % Convert.ToInt32(textBox8.Text.ToString()) != 0)
                {
                    icount = icount + 1;
                }
                for (int i = 1; i <= icount; i++)
                {
                    dic = new Dictionary<string, string>();

                    //dic.Add("wlbh", dt_dy.Rows[0]["物料编码"].ToString());
                    //dic.Add("jgddh", "");
                    dic.Add("khlh", dt_dy.Rows[0]["客户料号"].ToString());
                    dic.Add("jyy", dt_dy.Rows[0]["检验人员ID"].ToString());
                    //dic.Add("jz", dt_dy.Rows[0]["机种"].ToString());
                    dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());

                    dic.Add("jz", Convert.ToDateTime(dt_dy.Rows[0]["检验日期"]).ToString("yyMMdd") + dt_dy.Rows[0]["机种"].ToString().Trim() + "10");
                    dic.Add("cpxh", dt_dy.Rows[0]["产品型号"].ToString());
                    //dic["jgddh"]=textBox5.Text.ToString().Trim()+(i+1).ToString("000")+Convert.ToDateTime(dt_dy.Rows[0]["检验日期"]).ToString("yyMMdd");
                    dic["jgddh"] = textBox5.Text.ToString().Trim() + i.ToString("000").Trim()+ dic["jz"];    
                    if ( i_余数!=0 && (i == icount || icount == 1) )
                    {
                        //int a = (int)Convert.ToDecimal(dt_dy.Rows[0]["生产数量"]);


                        dic["hzsl"] = i_余数.ToString();
                    }
                    else
                    {
                        dic["hzsl"] = textBox8.Text.ToString();
                    }

                    Lprinter lp1 = new Lprinter(path, dic, str_打印机, 1);
                    //lp1.Start();
                    lp1.DoWork();
                    if (i == 1)
                    {
                        if (MessageBox.Show("是否继续？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            continue;
                        }
                        else
                        {
                            count = 1;
                            return;
                        }
                    }
                }

                //fun_save_打印历史记录();


            }
            if (textBox3.Text.Trim() == "通用模板")
            {
                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());
                //dic.Add("hzsl", count.ToString());
                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }

            }
            if (textBox3.Text.Trim() == "常熟模板")
            {

                dic.Add("cpmc", dt_dy.Rows[0]["产品名称"].ToString());
                if (count == 1)
                {
                    dic["hzsl"] = In_生产数量.ToString();
                }
                string ss = CPublic.Var.getDatetime().Year.ToString().Substring(2, 2);
                string rq = fun_date(Convert.ToDateTime(dt_dy.Rows[0]["检验日期"]).ToString("yyMM"));
                dic["jyrq"] = rq;
            }
            if (textBox3.Text.Trim() != "宏美模板")
            {
                //Lprinter lp = new Lprinter(path, dic, str_打印机, 1);
                //lp.Start();
                //if (MessageBox.Show("是否继续打印?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                //{
                //    Lprinter lp1 = new Lprinter(path, dic, str_打印机, count - 1);
                //    lp1.Start();
                //    flag = 1;
                //    //fun_save_打印历史记录();
                //}
                //else
                //{
                //    count = 1;
                //}

                Lprinter lp = new Lprinter(path, dic, str_打印机, 1);
                lp.DoWork();
                if (count > 1)
                {
                    if (MessageBox.Show("是否继续打印?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        if (c_余数 == 0)
                        {
                            Lprinter lp1 = new Lprinter(path, dic, str_打印机, count - 1);
                            lp1.Start();
                            flag = 1;
                        }
                        else
                        {
                            Lprinter lp1 = new Lprinter(path, dic, str_打印机, count - 2);
                            lp1.DoWork();
                            dic["hzsl"] = c_余数.ToString();
                            Lprinter lp2 = new Lprinter(path, dic, str_打印机, 1);
                            lp2.DoWork();
                        }
                        //fun_save_打印历史记录();
                    }
                    else
                    {
                        count = 1;
                    }
                }


            }




            fun_save_打印历史记录();


        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_打印箱贴()
#pragma warning restore IDE1006 // 命名样式
        {
            string printer = "";
            try
            {
                printer = CPublic.Var.li_CFG["printer_chest"].ToString();
            }
            catch
            {
                throw new Exception("未配置箱贴打印机");
            }
            int  boxcount = 0;
            int makecount = 0;
            try
            {
               boxcount=Convert.ToInt32(textBox12.Text);
               makecount = Convert.ToInt32(Convert.ToDecimal(textBox11.Text));
               if (boxcount != 0)
               {

                   int count = makecount / boxcount;
                   int ys = makecount % boxcount;
                   if (makecount % boxcount != 0)
                   {
                       count++;
                   }
                   string path = Application.StartupPath + string.Format(@"\Mode\{0}.lab", textBox13.Text.Trim());
                   if (textBox13.Text.Trim() == "通用箱贴")
                   {
                       Dictionary<string, string> dic = new Dictionary<string, string>();
                       //默认 模板 都拥有 jgddh wlbh eddy khlh jyrq （常熟 两个模板 日期不一样）
                       dic.Add("gdh", dt_dy.Rows[0]["生产工单号"].ToString());
                       dic.Add("dyzs", count.ToString());
                       dic.Add("xzsl", boxcount.ToString());
                       dic.Add("ys", ys.ToString());
                       Lprinter lp_box = new Lprinter(path, dic, printer, count);
                       lp_box.DoWork();
                   }
                   //dic.Add("hzsl", textBox8.Text.ToString());
                   if (textBox13.Text.Trim() == "飞腾箱贴")
                   {
                       List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
                       int  i_箱装=0;
                       string qsm = textBox15.Text.Trim(); //起始码
                       int  qs_序列 =Convert.ToInt32(textBox16.Text); //起始序列
                       for (int i = 1; i <= count; i++)
                       {
                           Dictionary<string, string> dic = new Dictionary<string, string>();
                           
                           if (i == count && ys!=0)
                           {
                               i_箱装=ys;
                              
                           }
                           else
                           {
                               i_箱装= boxcount;
                           }
                           //这个是 一箱的序列码最后一张 
                           int i_1=qs_序列+i_箱装-1;
                           string x = qsm.Substring(0, 7) + i_1.ToString().PadLeft(14, '0');
                            x=x+ ERPorg.Corg.fun_gccode(x);

                           dic.Add("xzsl",i_箱装.ToString());
                           dic.Add("cpmc", textBox4.Text.Trim());
                           dic.Add("cpxh", textBox2.Text.Trim());
                           dic.Add("eddy", textBox6.Text.Trim());
                           dic.Add("dl", textBox17.Text.Trim());

                           dic.Add("khmc", textBox14.Text.Trim());
                           dic.Add("qsm", qsm);
                           dic.Add("jsm", x);
                           qs_序列 = i_1;
                           li.Add(dic);
                       }

                       Lprinter lp  = new Lprinter(path, li , printer, 1);
                       lp.DoWork();

                   }
                
                  
               }
               else
               {
                   throw new Exception("箱装数量为0");
               }

            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }


        }
#pragma warning disable IDE1006 // 命名样式
        private string fun_date(string str)
#pragma warning restore IDE1006 // 命名样式
        {
            string sss = "";
            foreach (char c in str)
            {
                if (c == '1')
                {
                    sss = sss + 'A';
                }
                else if (c == '2')
                {
                    sss = sss + 'B';
                }
                else if (c == '3')
                {

                    sss = sss + 'C';
                }
                else if (c == '4')
                {
                    sss = sss + 'D';
                }
                else if (c == '5')
                {
                    sss = sss + 'E';
                }
                else if (c == '6')
                {
                    sss = sss + 'F';
                }
                else if (c == '7')
                {
                    sss = sss + 'G';
                }
                else if (c == '8')
                {
                    sss = sss + 'H';
                }
                else if (c == '9')
                {
                    sss = sss + 'I';
                }
                else
                {
                    sss = sss + 'J';

                }


            }
            return sss;
        }
        //打印
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_check();

                //this.printDialog1.Document = this.printDocument1;
                //DialogResult dr = this.printDialog1.ShowDialog();
                //if (dr == DialogResult.OK)
                //{
                ////Get the Copy times
                //int nCopy = this.printDocument1.PrinterSettings.Copies;
                ////Get the number of Start Page
                //int sPage = this.printDocument1.PrinterSettings.FromPage;
                ////Get the number of End Page
                //int ePage = this.printDocument1.PrinterSettings.ToPage;
                //string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                fun_打印();
                //if (textBox12.Visible) 
                //{
                //    fun_打印箱贴();
                //}


                fun_未打印工单();

                //fun_load_历史记录();
                //    }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }


        #endregion




#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                string sql_dy = string.Format(@"select 生产记录生产检验单主表.*,原ERP物料编号 from  生产记录生产检验单主表  
                                                left join 基础数据物料信息表  on  基础数据物料信息表.物料编码=生产记录生产检验单主表.物料编码
                                                where 生产工单号='{0}'", searchLookUpEdit1.EditValue.ToString().Trim());
                using (SqlDataAdapter da = new SqlDataAdapter(sql_dy, strcon))
                {
                    dt_dy = new DataTable();
                    da.Fill(dt_dy);
                    if (dt_dy.Rows.Count > 0)
                    {
                        if (dt_dy.Rows[0]["已打印"].Equals(true))  //  开放返修打印按钮 可编辑
                        {
                            btt_打印.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            btt_返修打印.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            textBox1.Enabled = true;
                            textBox2.Enabled = true;
                            textBox3.Enabled = true;
                            textBox4.Enabled = true;
                            textBox5.Enabled = true;
                            textBox6.Enabled = true;
                            textBox7.Enabled = true;
                            textBox8.Enabled = true;
                            textBox9.Enabled = true;
                            textBox10.Visible = true;
                            textBox10.Enabled = true;
                            textBox10.Text = "";
                            label11.Visible = true;
                        }
                        else
                        {
                            btt_返修打印.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            btt_打印.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            textBox1.Enabled = false;
                            textBox2.Enabled = false;
                            textBox3.Enabled = false;
                            textBox4.Enabled = false;
                            textBox5.Enabled = false;
                            textBox6.Enabled = false;
                            textBox7.Enabled = false;
                            textBox8.Enabled = false;
                            textBox9.Enabled = false;
                            label11.Visible = true;
                            textBox10.Visible = false;
                  
                            label11.Visible = false;


                        }
                        if (Convert.ToInt32(dt_dy.Rows[0]["箱装数量"]) > 0)
                        {

                            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            textBox12.Visible = true;
                            label13.Visible = true;

                            label14.Visible = true;
                            textBox13.Visible = true;

                            label15.Visible = true;
                            textBox14.Visible = true;

                            label16.Visible = true;
                            textBox15.Visible = true;
                            button1.Visible = true;

                            label17.Visible = true;
                            textBox17.Visible = true;
                        }
                        else
                        {
                            textBox12.Visible = false;
                            label13.Visible = false;

                            label14.Visible = false;
                            textBox13.Visible = false;

                            label15.Visible = false;
                            textBox14.Visible = false;

                            label16.Visible = false;
                            textBox15.Visible = false;
                            button1.Visible = false;
                            barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            label17.Visible = false;
                            textBox17.Visible = false;
                        }
                        dataBindHelper1.DataFormDR(dt_dy.Rows[0]); 

                    }

                }

                if (Convert.ToInt32(dt_dy.Rows[0]["箱装数量"]) > 0)
                {
                    string  s=string.Format(@"select  客户 from  生产记录生产工单表 a 
             left  join  生产记录生产制令子表 b  on  a.生产制令单号 =b.生产制令单号 
             where 生产工单号 ='{0}'", dt_dy.Rows[0]["生产工单号"]);
                    using (SqlDataAdapter da =new SqlDataAdapter (s,strcon))
                    {
                        DataTable tem=new DataTable ();
                        da.Fill(tem);
                        if (tem.Rows.Count > 0 && tem.Rows[0]["客户"] != null && tem.Rows[0]["客户"].ToString() != "")
                        {
                            textBox14.Text = tem.Rows[0]["客户"].ToString();


                        }

                    }
                }
            }
            else
            {

                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox7.Text = "";
                textBox8.Text = "";
                textBox9.Text = "";
            }

        }
        //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_load_下拉框();
                searchLookUpEdit1.EditValue = "";
                if (dt_dy != null)
                {
                    DataRow dr = dt_dy.NewRow();

                    dataBindHelper1.DataFormDR(dr);
                }

                fun_未打印工单();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        //筛选
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load_历史记录();
        }
        //返修打印
#pragma warning disable IDE1006 // 命名样式
        private void btt_返修打印_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            barLargeButtonItem1_ItemClick(null, null);
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (dr.ItemArray.Length > 0)
            {
                searchLookUpEdit1.EditValue = dr["生产工单号"];
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
        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_check();
            fun_打印箱贴();
        }

#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fm箱贴序列码检验码 fm = new fm箱贴序列码检验码();
            fm.StartPosition = FormStartPosition.CenterScreen;
            fm.MaximizeBox = false;
            fm.FormBorderStyle = FormBorderStyle.FixedDialog;
            fm.ShowDialog();
            if (fm.flag)
            {
                textBox15.Text = fm.s;
                textBox16.Text = fm.i_起始.ToString();

            }


        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox15_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void label16_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

      














    }
}
