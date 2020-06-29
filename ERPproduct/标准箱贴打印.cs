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
    public partial class 标准箱贴打印 : UserControl
    {
        public 标准箱贴打印()
        {
            InitializeComponent();
        }
        DataTable dtM;
        string strcon = CPublic.Var.strConn;
        /// <summary>
        /// 标记 是否正在打印过程中
        /// </summary>
        bool flag = false;
        Dictionary<Dictionary<string, string>, int> dic_打印队列;
        Dictionary<string, int> cache;
        int xx = 1;//用来标记当前是第几箱
        string str_printer箱贴 = "";
        int total = 0;
#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {
                dic_打印队列 = null;
                cache = null;
                textBox3.Text = "";
                textBox2.Text = "";
                //  string s = string.Format("select * from 采购供应商表 where 供应商ID='{0}'",textBox7.Text.ToString());

                //               string s = string.Format(@"select  Customer.cCusAbbName,iQuantity 数量,cInvCode 存货编码,cInvName 存货名称,isnull(cShipAddress,'')发货地址 from  [192.168.20.150].UFDATA_008_2018.dbo.DispatchLists
                //left  join [192.168.20.150].UFDATA_008_2018.dbo.DispatchList on DispatchList.DLID=DispatchLists.DLID 
                // inner join 基础数据物料信息表 base on base.物料编码=cInvCode
                //   left join [192.168.20.150].UFDATA_008_2018.dbo.Customer on Customer.ccuscode=DispatchList.cCusCode  where cDLCode like '%{0}'and cInvName not like '%配件包%'  and cInvName not like '%包装盒%'  ", textBox7.Text);

                string str = textBox7.Text.Substring(0, 2);
                string s = "";
                if (str == "BA")
                {

                    s = string.Format(@"select xm.申请数量  数量,xm.作废,xm.申请批号,xm.申请批号明细,xm.物料编码  存货编码,xm.物料名称 存货名称,xm.规格型号, xz.地址 as 送货地址, xz.相关单位 as 客户名,   xz.*  from   借还申请表附表 xm    
                     left  join  借还申请表  xz  on xm.申请批号=xz.申请批号
                     inner join 基础数据物料信息表 base on base.物料编码=xm.物料编码  where xm.申请批号
                     like '%{0}'and xm.物料名称 not like '%配件包%'  and xm.物料名称 not like '%包装盒%'  and  xm.作废=0    ", textBox7.Text);


                }
                else
                {
                    s = string.Format(@"select xm.出库数量  数量,xm.出库通知单号,xm.出库通知单明细号,xm.物料编码  存货编码,xm.物料名称 存货名称,xm.规格型号,    xz.*  from   销售记录销售出库通知单明细表 xm    
left  join  销售记录销售出库通知单主表  xz  on xm.出库通知单号=xz.出库通知单号
inner join 基础数据物料信息表 base on base.物料编码=xm.物料编码  where xm.出库通知单号 like '%{0}'and xm.物料名称 not like '%配件包%'  and xm.物料名称 not like '%包装盒%'     ", textBox7.Text);


                }




                dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                textBox1.Text = dtM.Rows[0]["客户名"].ToString();
                textBox4.Text = dtM.Rows[0]["送货地址"].ToString();
                dtM.Columns.Add("选择", typeof(bool));
                foreach (DataRow dr in dtM.Rows)
                {
                    dr["选择"] = true;
                }
                gridControl1.DataSource = dtM;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            try
            {
                if (flag == false)
                {

                    fun_check();

                    if (MessageBox.Show(string.Format("确定打印？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {


                        //PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();

                        //this.printDialog1.Document = this.printDocument1;
                        //DialogResult dr = this.printDialog1.ShowDialog();
                        //string PrinterName = this.printDocument1.PrinterSettings.PrinterName;


                        //str_printer箱贴 = PrinterName;

                        string str_kh = textBox1.Text;
                        label7.Text = "正在规划中,请稍候";
                        Thread BG = new Thread(() =>
                        {

                            if (textBox12.Text != "")
                            {
                                fun_规划1();
                            }
                            else
                            {
                                fun_规划();
                            }
                            string path = Application.StartupPath + string.Format(@"\Mode\标准箱贴.lab"); ;
                            Lprinter lp = new Lprinter(path, dic_打印队列, str_printer箱贴);


                            lp.DoWork();
                            flag = false;
                            xx = 1;
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
        private void fun_check()
#pragma warning restore IDE1006 // 命名样式
        {
            if (textBox1.Text.Trim() == "") throw new Exception("收货单位为空");
            if (textBox2.Text.Trim() == "") throw new Exception("联系人");
            if (textBox3.Text.Trim() == "") throw new Exception("联系电话");
            if (textBox4.Text.Trim() == "") throw new Exception("收货地址为空");
            if (textBox5.Text.Trim() == "") throw new Exception("箱装数量为空");
            if (textBox9.Text.Trim() == "") throw new Exception("物料名称为空");
            if (gridView1.DataRowCount == 0) throw new Exception("未取到配件包信息");
        }


#pragma warning disable IDE1006 // 命名样式
        private void fun_规划1()
#pragma warning restore IDE1006 // 命名样式
        {

            gridView1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();


            xx = 1;
            total = 0;
            int tota = 0;
            int i_箱装数 = Convert.ToInt32(textBox5.Text);
            DataView dv = new DataView(dtM);
            dv.Sort = "数量 desc ";

            foreach (DataRow dr in dv.ToTable().Rows)
            {
                if (Convert.ToBoolean(dr["选择"].ToString()) == true)
                {

                    total = total + Convert.ToInt32(dr["数量"]);
                }
            }

            int total1 = total / i_箱装数;
            tota = total / i_箱装数;
            if (textBox12.Text != "")
            {

                tota = tota + int.Parse(textBox12.Text);
                //  total1 = total1 + int.Parse(textBox12.Text);
            }
            cache = new Dictionary<string, int>();
            dic_打印队列 = new Dictionary<Dictionary<string, string>, int>();
            int i_明细 = 0; int i = 0; int i_余数 = 0;
            i_余数 = total % i_箱装数;
            if (i_余数 > 0)
            {
                tota = tota + 1;

            }



            //for (int a = 1;a <= total1; a++)
            //{
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("shdw", textBox1.Text);//收货单位
            dic.Add("fhdw", textBox10.Text);//发货单位
            dic.Add("fhhm", textBox11.Text);//发货号码
            //  dic.Add("pjb", dr["合并简称"].ToString() + "-" + i_箱装数.ToString() + "套");
            // dic.Add("linkman", textBox2.Text);
            dic.Add("shdz", textBox4.Text);//收货地址
            dic.Add("cpmc", textBox9.Text);//产品名称
            dic.Add("sjr", textBox2.Text);//联系人
            dic.Add("dh", textBox3.Text);//电话

            dic.Add("sl", textBox5.Text.ToString());
            string a = textBox5.Text;

            dic.Add("zsl", tota.ToString());
            dic.Add("xh", xx.ToString());  //第几箱

            dic_打印队列.Add(dic, total1);
            xx = xx + total1;






            // i = total / i_箱装数;

            if (i_余数 > 0)
            {

                Dictionary<string, string> dic1 = new Dictionary<string, string>();
                dic1.Add("shdw", textBox1.Text);//收货单位
                dic1.Add("fhdw", textBox10.Text);//发货单位
                dic1.Add("fhhm", textBox11.Text);//发货号码
                dic1.Add("fhdh", textBox7.Text);
                //  dic.Add("pjb", dr["合并简称"].ToString() + "-" + i_箱装数.ToString() + "套");
                // dic.Add("linkman", textBox2.Text);
                dic1.Add("shdz", textBox4.Text);//收货地址
                dic1.Add("cpmc", textBox9.Text);//产品名称
                dic1.Add("sjr", textBox2.Text);//联系人
                dic1.Add("dh", textBox3.Text);//电话
                dic1.Add("sl", i_余数.ToString());
                dic1.Add("zsl", tota.ToString());
                dic1.Add("xh", xx.ToString());  //第几箱

                dic_打印队列.Add(dic1, 1);

                xx = xx + 1;
            }

            if (textBox12.Text != "")
            {

                Dictionary<string, string> dic1 = new Dictionary<string, string>();
                dic1.Add("shdw", textBox1.Text);//收货单位
                dic1.Add("fhdw", textBox10.Text);//发货单位
                dic1.Add("fhhm", textBox11.Text);//发货号码
                dic1.Add("fhdh", textBox7.Text);
                //  dic.Add("pjb", dr["合并简称"].ToString() + "-" + i_箱装数.ToString() + "套");
                // dic.Add("linkman", textBox2.Text);
                dic1.Add("shdz", textBox4.Text);//收货地址
                dic1.Add("cpmc", textBox13.Text);//产品名称
                dic1.Add("sjr", textBox2.Text);//联系人
                dic1.Add("dh", textBox3.Text);//电话
                dic1.Add("sl", "0");
                dic1.Add("zsl", tota.ToString());
                dic1.Add("xh", xx.ToString());  //第几箱

                dic_打印队列.Add(dic1, int.Parse(textBox12.Text));


            }





            //}

            //foreach (DataRow dr in dv.ToTable().Rows)
            //{
            //    i_明细 = Convert.ToInt32(dr["数量"]);
            //    i = i_明细 / i_箱装数;
            //    i_余数 = i_明细 % i_箱装数;
            //    if (i_明细 > 0 && i > 0)
            //    {
            //        Dictionary<string, string> dic = new Dictionary<string, string>();
            //        dic.Add("shdw", textBox1.Text);//收货单位
            //        dic.Add("fhdw", textBox10.Text);//发货单位
            //        dic.Add("fhhm", textBox11.Text);//发货号码
            //        dic.Add("pjb", dr["合并简称"].ToString() + "-" + i_箱装数.ToString() + "套");
            //        dic.Add("linkman", textBox2.Text);
            //        dic.Add("shdz", textBox4.Text);//收货地址
            //        dic.Add("cpmc", textBox9.Text);//产品名称
            //        dic.Add("sjr", textBox2.Text);//联系人
            //        dic.Add("dh", textBox3.Text);//电话
            //        dic.Add("sl", textBox5.Text);
            //        dic.Add("zsl", total.ToString());
            //        dic.Add("xh", xx.ToString());  //第几箱
            //        dic_打印队列.Add(dic, i);
            //        xx = xx + i;
            //    }
            //    if (i_余数 != 0) // 不足一箱扔到 cache里面
            //    {
            //        if (cache.ContainsKey(dr["存货编码"].ToString()))
            //        {
            //            cache[dr["存货编码"].ToString()] = cache[dr["存货编码"].ToString()] + i_余数;
            //        }
            //        else
            //        {
            //            cache.Add(dr["存货编码"].ToString(), i_余数);
            //        }
            //    }
            //}
            //fun_dg(cache);
            BeginInvoke(new MethodInvoker(() =>
            {
                label7.Text = "打印中...";
            }));

        }


#pragma warning disable IDE1006 // 命名样式
        private void fun_规划()
#pragma warning restore IDE1006 // 命名样式
        {

            gridView1.CloseEditor();
            this.BindingContext[dtM].EndCurrentEdit();


            xx = 1;
            total = 0;
            int tota = 0;
            int i_箱装数 = Convert.ToInt32(textBox5.Text);
            DataView dv = new DataView(dtM);
            dv.Sort = "数量 desc ";

            foreach (DataRow dr in dv.ToTable().Rows)
            {
                if (Convert.ToBoolean(dr["选择"].ToString()) == true)
                {
                    total = total + Convert.ToInt32(dr["数量"]);
                }
            }

            int total1 = total / i_箱装数;
            tota = total / i_箱装数;
            //  if (textBox12.Text!="")
            //  {

            //      tota = tota + int.Parse( textBox12.Text);
            //      total1 = total1 + int.Parse(textBox12.Text);
            //}
            cache = new Dictionary<string, int>();
            dic_打印队列 = new Dictionary<Dictionary<string, string>, int>();
            int i_明细 = 0; int i = 0; int i_余数 = 0;
            i_余数 = total % i_箱装数;
            if (i_余数 > 0)
            {
                tota = total1 + 1;

            }



            //for (int a = 1;a <= total1; a++)
            //{
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("shdw", textBox1.Text);//收货单位
            dic.Add("fhdw", textBox10.Text);//发货单位
            dic.Add("fhhm", textBox11.Text);//发货号码
            dic.Add("fhdh", textBox7.Text);
            //  dic.Add("pjb", dr["合并简称"].ToString() + "-" + i_箱装数.ToString() + "套");
            // dic.Add("linkman", textBox2.Text);
            dic.Add("shdz", textBox4.Text);//收货地址
            dic.Add("cpmc", textBox9.Text);//产品名称
            dic.Add("sjr", textBox2.Text);//联系人
            dic.Add("dh", textBox3.Text);//电话

            dic.Add("sl", textBox5.Text.ToString());
            string a = textBox5.Text;

            dic.Add("zsl", tota.ToString());
            dic.Add("xh", xx.ToString());  //第几箱

            dic_打印队列.Add(dic, total1);
            xx = xx + total1;



            // i = total / i_箱装数;

            if (i_余数 > 0)
            {

                Dictionary<string, string> dic1 = new Dictionary<string, string>();
                dic1.Add("shdw", textBox1.Text);//收货单位
                dic1.Add("fhdw", textBox10.Text);//发货单位
                dic1.Add("fhhm", textBox11.Text);//发货号码
                dic1.Add("fhdh", textBox7.Text);
                //  dic.Add("pjb", dr["合并简称"].ToString() + "-" + i_箱装数.ToString() + "套");
                // dic.Add("linkman", textBox2.Text);
                dic1.Add("shdz", textBox4.Text);//收货地址
                dic1.Add("cpmc", textBox9.Text);//产品名称
                dic1.Add("sjr", textBox2.Text);//联系人
                dic1.Add("dh", textBox3.Text);//电话
                dic1.Add("sl", i_余数.ToString());
                dic1.Add("zsl", tota.ToString());
                dic1.Add("xh", xx.ToString());  //第几箱

                dic_打印队列.Add(dic1, 1);


            }





            //}

            //foreach (DataRow dr in dv.ToTable().Rows)
            //{
            //    i_明细 = Convert.ToInt32(dr["数量"]);
            //    i = i_明细 / i_箱装数;
            //    i_余数 = i_明细 % i_箱装数;
            //    if (i_明细 > 0 && i > 0)
            //    {
            //        Dictionary<string, string> dic = new Dictionary<string, string>();
            //        dic.Add("shdw", textBox1.Text);//收货单位
            //        dic.Add("fhdw", textBox10.Text);//发货单位
            //        dic.Add("fhhm", textBox11.Text);//发货号码
            //        dic.Add("pjb", dr["合并简称"].ToString() + "-" + i_箱装数.ToString() + "套");
            //        dic.Add("linkman", textBox2.Text);
            //        dic.Add("shdz", textBox4.Text);//收货地址
            //        dic.Add("cpmc", textBox9.Text);//产品名称
            //        dic.Add("sjr", textBox2.Text);//联系人
            //        dic.Add("dh", textBox3.Text);//电话
            //        dic.Add("sl", textBox5.Text);
            //        dic.Add("zsl", total.ToString());
            //        dic.Add("xh", xx.ToString());  //第几箱
            //        dic_打印队列.Add(dic, i);
            //        xx = xx + i;
            //    }
            //    if (i_余数 != 0) // 不足一箱扔到 cache里面
            //    {
            //        if (cache.ContainsKey(dr["存货编码"].ToString()))
            //        {
            //            cache[dr["存货编码"].ToString()] = cache[dr["存货编码"].ToString()] + i_余数;
            //        }
            //        else
            //        {
            //            cache.Add(dr["存货编码"].ToString(), i_余数);
            //        }
            //    }
            //}
            //fun_dg(cache);
            BeginInvoke(new MethodInvoker(() =>
            {
                label7.Text = "打印中...";
            }));

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_dg(Dictionary<string, int> a)
#pragma warning restore IDE1006 // 命名样式
        {
            int i_箱装数 = Convert.ToInt32(textBox5.Text);
            cache = new Dictionary<string, int>();
            int c = a.Count;
            Dictionary<string, int> b = new Dictionary<string, int>(a);
            bool bl_end = false;
            int i_js = 1;
            //string s_配件包 = "";
            int sum = 0;
            foreach (KeyValuePair<string, int> kv in a)
            {

                sum += kv.Value;
                if (sum < i_箱装数)
                {

                    b.Remove(kv.Key);
                    if (i_js == a.Count) { bl_end = true; }
                }

                else if (sum == i_箱装数)
                {
                    b.Remove(kv.Key);

                    bl_end = true;
                }
                else if (sum > i_箱装数)
                {
                    b[kv.Key] = sum - i_箱装数;
                    //if (s_配件包 != "") { s_配件包 += "/"; }
                    //s_配件包 = s_配件包 + kv.Key + "-" + (kv.Value - (sum - i_箱装数)).ToString() + "套";
                    sum = i_箱装数;
                    bl_end = true;
                }

                if (bl_end)
                {

                    Dictionary<string, string> dic = new Dictionary<string, string>();

                    dic.Add("shdw", textBox1.Text);//收货单位
                    dic.Add("fhdw", textBox10.Text);//发货单位
                    dic.Add("fhhm", textBox11.Text);//发货号码
                    dic.Add("shdz", textBox4.Text);//收货地址
                    dic.Add("cpmc", textBox9.Text);//产品名称
                    dic.Add("sjr", textBox2.Text);//联系人
                    dic.Add("dh", textBox3.Text);//电话
                    dic.Add("sl", sum.ToString());


                    //dic.Add("xzsl", sum.ToString());
                    dic.Add("zsl", total.ToString());
                    //dic.Add("time", dateEdit1.EditValue.ToString());
                    dic.Add("xh", xx++.ToString());  //第几箱
                    dic_打印队列.Add(dic, 1);
                    break;
                }
                i_js++;

            }
            if (b.Count > 0) fun_dg(b);

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (flag)
                {
                    MessageBox.Show("正在打印中....");

                }

                else
                {

                    dtM = new DataTable();
                    var ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        dtM = ERPorg.Corg.ReadExcelToDataTable(ofd.FileName);
                        gridControl1.DataSource = dtM;
                        gridView1.ViewCaption = "EXCEL数据清单";
                    }
                    else return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                str_printer箱贴 = CPublic.Var.li_CFG["printer_chest"].ToString();
                string path = Application.StartupPath + string.Format(@"\Mode\标准箱贴.lab"); ;

                //前提必须dic_打印队列有东西
                if (dic_打印队列 == null || dic_打印队列.Count == 0) fun_规划();
                int x = 0;
                if (!int.TryParse(textBox8.Text, out x)) throw new Exception("输入箱数不正确");
                //if (x < 0 || x > dic_打印队列.Count + 1) throw new Exception(string.Format("请输入正确的箱数,范围为1-{0}", dic_打印队列.Count + 1));
                foreach (KeyValuePair<Dictionary<string, string>, int> kv in dic_打印队列)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>(kv.Key as Dictionary<string, string>);

                    int y = Convert.ToInt32(dic["xh"]);
                    if (y == x || (y < x && y + kv.Value > x))
                    {
                        dic["xh"] = x.ToString();
                        Lprinter lP = new Lprinter(path, dic, str_printer箱贴, 1);
                        lP.DoWork();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void 标准箱贴打印_Load(object sender, EventArgs e)
        {
            str_printer箱贴 = CPublic.Var.li_CFG["printer_chest"].ToString();
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox12_TextChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }






    }
}
