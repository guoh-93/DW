using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace ERPproduct
{
    public partial class ui工行箱贴 : UserControl
    {

        DataTable dtM;
        string strcon = CPublic.Var.strConn;
        /// <summary>
        /// 标记 是否正在打印过程中
        /// </summary>
        bool flag = false;
        Dictionary<Dictionary<string, string>, int> dic_打印队列;
        Dictionary<string, int> cache;
        int xx = 1;//用来标记当前是第几箱
        string str_printer箱贴 ="";
        int total = 0;
        public ui工行箱贴()
        {
            InitializeComponent();   
        }

#pragma warning disable IDE1006 // 命名样式
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            { e.Handled = true; }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            { e.Info.DisplayText = (e.RowHandle + 1).ToString(); }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                        // dtM = ERPorg.Corg.ExcelXLSX(ofd);
                        //string path = System.IO.Path.GetFullPath(ofd.FileName);
                        //dtM = ItemInspection.print_FMS.ExcelToDatatable(path);
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
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                str_printer箱贴 = CPublic.Var.li_CFG["printer_chest"].ToString();
                if (flag == false)
                {
                    gridView1.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                    DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    fun_check();

                    if (MessageBox.Show(string.Format("确认打印?"), "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {

                        string str_kh = textBox1.Text;
                        label7.Text = "正在规划中,请稍候";
                        Thread BG = new Thread(() => 
                        { 
                            fun_规划();

                            string path = Application.StartupPath + string.Format(@"\Mode\工行箱贴.lab"); ;
                            //foreach (KeyValuePair<Dictionary<string, string>, int> kv in dic_打印队列)
                            //{
                            //    if (kv.Value == 0) continue;
                            //    Dictionary<string, string> dix = kv.Key as Dictionary<string, string>;
                            //    Lprinter lP = new Lprinter(path, dix, str_printer箱贴, kv.Value);
                            //    lP.DoWork();
                            //}

                            Lprinter lp = new Lprinter(path, dic_打印队列, str_printer箱贴);
                            lp.DoWork();

                            flag = false;
                            xx = 1;
                            //ERPorg.Corg cg = new ERPorg.Corg();
                            //cg.kill_lppa();
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                label7.Text = "";
                            }));
                        
                        
                        }
                        
                        
                        );
                        BG.IsBackground = true;
                        BG.Start();
                        //fun_打印(str_kh, str_ddh);
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
            if (textBox6.Text.Trim() == "") throw new Exception("物料名称为空");
            if (gridView1.DataRowCount == 0) throw new Exception("未取到配件包信息");
        }
#pragma warning disable IDE1006 // 命名样式
        private void fun_规划()
#pragma warning restore IDE1006 // 命名样式
        {
            xx = 1;
            total=0;
            int i_箱装数 = Convert.ToInt32(textBox5.Text);
            DataView dv = new DataView(dtM);
            dv.Sort = "数量 desc ";

            foreach (DataRow dr in dv.ToTable().Rows)
            {
                total = total + Convert.ToInt32(dr["数量"]);
            }

            total = (int) Math.Ceiling((double)total / (double)i_箱装数);
             
            cache = new Dictionary<string, int>();
            dic_打印队列 = new Dictionary<Dictionary<string, string>, int>();
            int i_明细 = 0; int i = 0; int i_余数 = 0;
            foreach (DataRow dr in dv.ToTable().Rows)
            {
                i_明细 = Convert.ToInt32(dr["数量"]);
                i = i_明细 / i_箱装数;
                i_余数 = i_明细 % i_箱装数;
                if (i_明细 > 0 && i>0)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("khmc", textBox1.Text);
                    dic.Add("ggxh", textBox6.Text);
                    dic.Add("address", textBox4.Text);
                    dic.Add("pjb", dr["合并简称"].ToString() + "-" + i_箱装数.ToString()+"套");
                    dic.Add("linkman", textBox2.Text);
                    dic.Add("fhdh", textBox7.Text);
                    dic.Add("tel", textBox3.Text);
                    dic.Add("xzsl", textBox5.Text);
                    dic.Add("time", dateEdit1.EditValue.ToString());
                    dic.Add("total", total.ToString());
                    dic.Add("xh", xx.ToString());  //第几箱
                    dic_打印队列.Add(dic, i);
                    xx = xx + i;
                }
                if (i_余数 != 0) // 不足一箱扔到 cache里面
                {
                    if (cache.ContainsKey(dr["合并简称"].ToString()))
                    {
                        cache[dr["合并简称"].ToString()] = cache[dr["合并简称"].ToString()] + i_余数;
                    }
                    else
                    {
                        cache.Add(dr["合并简称"].ToString(), i_余数);
                    }
                }
            }
            fun_dg(cache);
            BeginInvoke(new MethodInvoker(() =>
            {
                label7.Text = "打印中...";
            }));
            //string path = Application.StartupPath + string.Format(@"\Mode\工行箱贴.lab"); ;
            ////foreach (KeyValuePair<Dictionary<string, string>, int> kv in dic_打印队列)
            ////{
            ////    if (kv.Value == 0) continue;
            ////    Dictionary<string, string> dix = kv.Key as Dictionary<string, string>;
            ////    Lprinter lP = new Lprinter(path, dix, str_printer箱贴, kv.Value);
            ////    lP.DoWork();
            ////}

            //Lprinter lp = new Lprinter(path, dic_打印队列, str_printer箱贴);
            //lp.DoWork();

            //flag = false;
            //xx = 1;
            //BeginInvoke(new MethodInvoker(() =>
            //{
            //    label7.Text = "";
            //}));
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
            string s_配件包 = "";
            int sum = 0;
            foreach (KeyValuePair<string, int> kv in a)
            {
                
                sum += kv.Value;
                if (sum < i_箱装数)
                {

                    b.Remove(kv.Key);
                    if (s_配件包 != "") { s_配件包 += "/"; }
                    s_配件包 = s_配件包 + kv.Key + "-" + kv.Value.ToString() + "套";
                    if (i_js == a.Count) { bl_end = true; }
                }

                else if (sum == i_箱装数)
                {
                    b.Remove(kv.Key);
                    if (s_配件包 != "") { s_配件包 += "/"; }
                    s_配件包 = s_配件包 + kv.Key + "-" + kv.Value.ToString() + "套";
                    bl_end = true;
                }
                else if (sum > i_箱装数)
                {
                    b[kv.Key] = sum - i_箱装数;
                    if (s_配件包 != "") { s_配件包 += "/"; }
                    s_配件包 = s_配件包 + kv.Key + "-" + (kv.Value-(sum - i_箱装数)).ToString() + "套";
                    sum = i_箱装数;
                    bl_end = true;
                }

                if (bl_end)
                {

                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("khmc", textBox1.Text);
                    dic.Add("ggxh", textBox6.Text);
                    dic.Add("address", textBox4.Text);
                    dic.Add("pjb", s_配件包);
                    dic.Add("linkman", textBox2.Text);
                    dic.Add("fhdh", textBox7.Text);
                    dic.Add("tel", textBox3.Text);
                    dic.Add("xzsl", sum.ToString());
                    dic.Add("total", total.ToString());
                    dic.Add("time", dateEdit1.EditValue.ToString());
                    dic.Add("xh", xx++.ToString());  //第几箱
                    dic_打印队列.Add(dic, 1);
                    break;
                }
                i_js++;
                    
            }
            if (b.Count > 0) fun_dg(b);

        }

#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                textBox3.Text = "";
                textBox2.Text = "";


                //               string s = string.Format(@"select  Customer.cCusAbbName,iQuantity 数量,cInvCode 存货编码,cInvName 存货名称,isnull(cShipAddress,'')发货地址,合并简称 from  [192.168.20.150].UFDATA_008_2018.dbo.DispatchLists
                //left  join [192.168.20.150].UFDATA_008_2018.dbo.DispatchList on DispatchList.DLID=DispatchLists.DLID 
                //inner   join   配件包简称对应关系 on 存货编码 =cInvCode
                // inner join 基础数据物料信息表 base on base.物料编码=cInvCode
                //   left join [192.168.20.150].UFDATA_008_2018.dbo.Customer on Customer.ccuscode=DispatchList.cCusCode  where cDLCode like '%{0}'", textBox7.Text);


                //               string s = string.Format(@"select  Customer.cCusAbbName,iQuantity 数量,cInvCode 存货编码,cInvName 存货名称,isnull(cShipAddress,'')发货地址 from  [192.168.20.150].UFDATA_008_2018.dbo.DispatchLists
                //left  join [192.168.20.150].UFDATA_008_2018.dbo.DispatchList on DispatchList.DLID=DispatchLists.DLID 
                // inner join 基础数据物料信息表 base on base.物料编码=cInvCode
                //   left join [192.168.20.150].UFDATA_008_2018.dbo.Customer on Customer.ccuscode=DispatchList.cCusCode  where cDLCode like '%{0}'and cInvName not like '%配件包%'  and cInvName not like '%包装盒%'  ", textBox7.Text);




                string s = string.Format(@"select hb.合并简称, xm.出库数量  数量,xm.出库通知单号,xm.出库通知单明细号,xm.物料编码  存货编码,xm.物料名称 存货名称,xm.规格型号,    xz.*  from   销售记录销售出库通知单明细表 xm    
                left  join  销售记录销售出库通知单主表  xz  on xm.出库通知单号=xz.出库通知单号
                inner join 基础数据物料信息表 base on base.物料编码=xm.物料编码 
                 inner   join   配件包简称对应关系 hb on hb.存货编码 =xm.物料编码
                 where xm.出库通知单号 like '%{0}'   ", textBox7.Text);



              //  dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
         
                dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (dtM.Rows.Count > 0)
                {
                    textBox1.Text = dtM.Rows[0]["客户名"].ToString();
                    textBox4.Text = dtM.Rows[0]["送货地址"].ToString();
                    gridControl1.DataSource = dtM;
                }
                else
                {

                    MessageBox.Show("未找到数据");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        
           
        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 如果打印队列有不会重新计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                str_printer箱贴 = CPublic.Var.li_CFG["printer_chest"].ToString();
                string path = Application.StartupPath + string.Format(@"\Mode\工行箱贴.lab"); ;

                //前提必须dic_打印队列有东西
                if (dic_打印队列==null ||dic_打印队列.Count==0) fun_规划();
                int x = 0;
                if (!int.TryParse(textBox8.Text, out x)) throw new Exception("输入箱数不正确");
                //if (x < 0 || x > dic_打印队列.Count + 1) throw new Exception(string.Format("请输入正确的箱数,范围为1-{0}", dic_打印队列.Count + 1));
                foreach (KeyValuePair<Dictionary<string, string>, int> kv in dic_打印队列)
                {
                    Dictionary<string, string> dic =new Dictionary<string,string> (kv.Key as Dictionary<string, string>);

                    int y = Convert.ToInt32(dic["xh"]);
                    if (y==x||(y<x&&y+kv.Value>x))
                    {
                        dic["xh"] = x.ToString();
                        Lprinter lP = new Lprinter(path, dic, str_printer箱贴, 1);
                        lP.DoWork(); 
                        break;
                    }
                }
            }
            catch (Exception ex )
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui工行箱贴_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            dateEdit1.EditValue = CPublic.Var.getDatetime().Date;
        }
   
    }
}
