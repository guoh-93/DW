using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
namespace ERPproduct
{
    public partial class ui外贸箱贴打印 : UserControl
    {

        #region
        DataTable dtM;
        DataTable dt_客户;
        string strcon = CPublic.Var.strConn;


        string str_printer箱贴 = "";

        List<String[]> x;
        bool bl_printer = false;

        Dictionary<string, int> dic_cache = new Dictionary<string, int>();

        Dictionary<Dictionary<string, string>, int> dic_打印队列;
        /// <summary>
        /// 标记 是否正在打印过程中
        /// </summary>
        bool flag = false;

        bool flag_save = false;

        string str = "";
        /// <summary>
        /// 当前箱次
        /// </summary>
        int i_箱次 = 0;
        int i_总箱数 = 0;
        int In_发货数量 = 0;
        // int i_余数 = 0;
        int i_箱装数量 = 0;
        //19-3-15
        DataTable display;

        #endregion


        public ui外贸箱贴打印()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                dic_打印队列 = new Dictionary<Dictionary<string, string>, int>();
                dic_cache = new Dictionary<string, int>();

                dtM = new DataTable();
                textBox4.Text = "";
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // dtM = ERPorg.Corg.ExcelXLSX(ofd);

                    //string path = System.IO.Path.GetFullPath(ofd.FileName);
                    //dtM = ItemInspection.print_FMS.ExcelToDatatable(path);
                    dtM = ERPorg.Corg.ReadExcelToDataTable(ofd.FileName);
                    gridControl1.DataSource = dtM;
                    dtM.Columns.Add("天差");
                    int i = dtM.Rows.Count - 1;
                    for (; i >= 0; i--)
                    {
                        if (dtM.Rows[i]["产品型号"].ToString().Trim() == "" && dtM.Rows[i]["产品总数量"].ToString().Trim() == "")
                        { dtM.Rows.RemoveAt(i); continue; } //去空行
                        DateTime time1 = DateTime.Now.Date;
                        DateTime time2 = Convert.ToDateTime(dtM.Rows[i]["日期"].ToString().Trim()).Date;
                        TimeSpan ts = time2 - time1;
                        dtM.Rows[i]["天差"] = ts.Days.ToString();

                    }


                    gridView1.ViewCaption = "EXCEL数据清单";
                    gridControl2.DataSource = null;
                    flag_save = false;
                }
                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui外贸箱贴打印_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //string s = "select  客户编号,客户名称 from 客户基础信息表 ";
            //dt_客户 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            //searchLookUpEdit1.Properties.DataSource = dt_客户;
            //searchLookUpEdit1.Properties.DisplayMember = "客户编号";
            //searchLookUpEdit1.Properties.ValueMember = "客户编号";
            checkBox1.Checked = true;
        }

#pragma warning disable IDE1006 // 命名样式
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            //{
            //    DataRow[] r = dt_客户.Select(string.Format("客户编号='{0}'", searchLookUpEdit1.EditValue));

            //    textBox1.Text = r[0]["客户名称"].ToString();
            //}
            //else
            //{
            //    textBox1.Text = "";


            //}
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                if (display == null || display.Columns.Count == 0 || display.Rows.Count == 0)
                {

                    throw new Exception("");
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DataTable tt = display.Copy();
                    tt.Columns.Remove("打印日期");
                    tt.Columns.Remove("ID");

                    ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }

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
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                // DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (MessageBox.Show(string.Format("确认打印?"), "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (textBox4.Text.Trim() == "")
                    {
                        //点打印时生成批次号，如果有判断数据库是否有改批次号的
                        DateTime t = CPublic.Var.getDatetime().Date;
                        string ss = t.Year.ToString().Substring(2, 2);
                        ss = string.Format("WMDY{0}{1:D2}{2:0000}", ss, t.Month, CPublic.CNo.fun_得到最大流水号("WMDY", t.Year, t.Month));
                        textBox4.Text = ss;
                        flag_save = true;
                    }
                    else
                    {
                        string sql = string.Format("select  * from 箱贴打印报表 where 批号='{0}'", textBox4.Text.Trim());
                        display = new DataTable();
                        display = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                        if (display.Rows.Count > 0)
                        {
                            gridControl2.DataSource = display;
                            return;
                        }
                        else
                        {
                            throw new Exception("您输入的批号未找到相关记录,请确认后重新输入或者清空让系统自动生成并记录");
                        }
                    }
                    int x = dtM.Rows.Count;
                    for (int i = x - 1; i >= 0; i--)
                    {
                        if (dtM.Rows[i]["客户名称"].ToString().Trim() == "")
                        {
                            dtM.Rows.Remove(dtM.Rows[i]);
                        }
                    }

                    string CoName = comboBoxEdit1.Text;
                    Thread BG = new Thread(() =>
                    {
                        fun_打印(CoName);
                        string path = Application.StartupPath + string.Format(@"\Mode\外贸箱贴.lab");
                        Lprinter lp = new Lprinter(path, dic_打印队列, str_printer箱贴);
                        lp.DoWork();

                        //ERPorg.Corg cg = new ERPorg.Corg();
                        //cg.kill_lppa();


                    });
                    BG.IsBackground = true;
                    BG.Start();
                    // fun_打印(CoName);
                    flag = true;  //指示是否正在打印    
                    //}
                    //else
                    //{
                    //    MessageBox.Show("正在打印中,请稍后");
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        ///// <summary>
        ///// 打印销售订单号 对应的所有明细,一次性打印出来, Math.Ceiling(明细数量/箱装数量),
        //如果合箱发 是否是箱装数量一致的前提下.不是所有产品都能合箱发的
        //每一箱需要一个箱贴，当一箱里有n个产品就需要n个箱贴
        //s_客户为 客户名称,companyName为本公司名称,为securam 或者 Easthouse
        ///// </summary>
#pragma warning disable IDE1006 // 命名样式
        private void fun_打印(string compangName)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (bl_printer == false)
                {
                    string path_printer = Application.StartupPath + string.Format(@"\打印机配置.txt");
                    x = ERPorg.Corg.ReadTxt(path_printer);
                    str_printer箱贴 = x[0][0].ToString();
                    //str_printer小标签 = x[1][0].ToString();
                    bl_printer = true;
                }

                dic_打印队列 = new Dictionary<Dictionary<string, string>, int>();
                dic_cache = new Dictionary<string, int>();




                //算总箱数,
                DataTable tableTemp = dtM.Copy();
                tableTemp.Columns.Add("箱数", typeof(int));
                int totalCtn = 0;
                List<string> li = new List<string>();
                foreach (DataRow r in tableTemp.Rows)
                {
                    int int_箱装数 = Convert.ToInt32(r["箱装数量"].ToString());
                    int int_明细数量 = (int)Convert.ToDecimal(r["产品总数量"]);
                    int i_box_count = (int)Math.Ceiling((decimal)int_明细数量 / (decimal)int_箱装数);
                    r["箱数"] = i_box_count;
                    if (r["分组"].ToString().Trim() != "")
                    {
                        if (li.Contains(r["分组"].ToString().Trim()))
                        {
                            totalCtn += i_box_count - 1;
                        }
                        else
                        {
                            totalCtn += i_box_count;
                            li.Add(r["分组"].ToString().Trim());
                        }
                    }
                    else
                    {
                        totalCtn += i_box_count;
                    }
                }
                DataTable dtM_cp = dtM.Copy();
                if (!dtM_cp.Columns.Contains("CTN#"))
                {
                    dtM_cp.Columns.Add("CTN#");
                }
                i_箱次 = 1;
                string ss = "select  * from 箱贴打印报表 where 1=2";
                DataTable t_save = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                DateTime t = CPublic.Var.getDatetime();
                foreach (DataRow r in dtM_cp.Rows)
                {
                    int temp = i_箱次;
                    int int_箱装数 = Convert.ToInt32(r["箱装数量"].ToString());
                    int int_明细数量 = (int)Convert.ToDecimal(r["产品总数量"].ToString());
                    int i_箱数 = (int)Math.Ceiling((decimal)int_明细数量 / (decimal)int_箱装数); //就是打印的count 参数,不管合不合箱每个产品每放在一个箱子里就需要一个箱贴
                    int i_余数 = int_明细数量 % int_箱装数;
                    int p_count = i_箱数; // 当有组时 p_count=i_箱贴-1,另外一个需要单独打印 xc不同
                    if (r["分组"].ToString().Trim() == "")  //没有组不需要合箱
                    {
                        if (i_箱数 == 1)
                        {
                            r["CTN#"] = i_箱次.ToString();
                        }
                        else
                        {
                            r["CTN#"] = i_箱次.ToString() + "-" + (i_箱次 + i_箱数 - 1).ToString();
                        }



                        if (i_余数 == 0)
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add("color", r["颜色"].ToString().Trim());
                            dic.Add("dw", r["计量单位"].ToString().Trim());
                            string sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                            dic.Add("jcbh", sss);
                            dic.Add("wlmc", r["产品名称"].ToString().Trim());
                            dic.Add("khddh", r["客户订单号"].ToString().Trim());
                            dic.Add("ggxh", r["产品型号"].ToString().Trim());
                            sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                            dic.Add("khgg", sss);
                            dic.Add("securam", compangName);
                            dic.Add("mdg", r["目的国"].ToString().Trim());
                            dic.Add("khmc", r["客户名称"].ToString());
                            dic.Add("time1", r["天差"].ToString().Trim());
                            dic.Add("sl", int_箱装数.ToString().Trim());
                            dic.Add("xc", i_箱次.ToString().Trim());
                            if (checkBox1.Checked)
                            {
                                dic.Add("zxc", "/" + totalCtn.ToString().Trim());
                            }
                            else
                            {
                                dic.Add("zxc", "");

                            }

                            dic_打印队列.Add(dic, p_count);
                            //Lprinter lP = new Lprinter(path, dic, str_printer箱贴, p_count);
                            //lP.DoWork();
                            i_箱次 = i_箱次 + p_count;
                        }
                        else
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add("color", r["颜色"].ToString().Trim());
                            dic.Add("dw", r["计量单位"].ToString().Trim());
                            string sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                            dic.Add("jcbh", sss);
                            dic.Add("wlmc", r["产品名称"].ToString().Trim());
                            dic.Add("khddh", r["客户订单号"].ToString().Trim());
                            dic.Add("ggxh", r["产品型号"].ToString().Trim());
                            sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                            dic.Add("khgg", r["客户规格"].ToString().Trim());
                            dic.Add("securam", compangName);
                            dic.Add("khmc", r["客户名称"].ToString());
                            dic.Add("time1", r["天差"].ToString().Trim());
                            dic.Add("mdg", r["目的国"].ToString().Trim());

                            dic.Add("sl", int_箱装数.ToString().Trim());
                            dic.Add("xc", i_箱次.ToString().Trim());
                            if (checkBox1.Checked)
                            {
                                dic.Add("zxc", "/" + totalCtn.ToString().Trim());
                            }
                            else
                            {
                                dic.Add("zxc", "");

                            }
                            dic_打印队列.Add(dic, p_count - 1);
                            //Lprinter lP = new Lprinter(path, dic, str_printer箱贴, p_count - 1);
                            //lP.DoWork();
                            i_箱次 = i_箱次 + p_count - 1;
                            Dictionary<string, string> dic_1 = new Dictionary<string, string>();
                            dic_1.Add("color", r["颜色"].ToString().Trim());
                            dic_1.Add("dw", r["计量单位"].ToString().Trim());
                            sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                            dic_1.Add("jcbh", sss);
                            dic_1.Add("wlmc", r["产品名称"].ToString().Trim());
                            dic_1.Add("khddh", r["客户订单号"].ToString().Trim());
                            dic_1.Add("ggxh", r["产品型号"].ToString().Trim());
                            sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                            dic_1.Add("khgg", r["客户规格"].ToString().Trim());
                            dic_1.Add("securam", compangName);
                            dic_1.Add("khmc", r["客户名称"].ToString());
                            dic_1.Add("mdg", r["目的国"].ToString().Trim());

                            dic_1.Add("time1", r["天差"].ToString().Trim());
                            dic_1.Add("sl", i_余数.ToString().Trim());
                            dic_1.Add("xc", i_箱次.ToString().Trim());
                            if (checkBox1.Checked)
                            {
                                dic_1.Add("zxc", "/" + totalCtn.ToString().Trim());
                            }
                            else
                            {
                                dic_1.Add("zxc", "");

                            }
                            dic_打印队列.Add(dic_1, 1);

                            i_箱次++;
                        }
                    }
                    else
                    {
                        //取组
                        if (!dic_cache.ContainsKey(r["分组"].ToString().Trim())) // 第一次遍历到该组记录
                        {
                            dic_cache.Add(r["分组"].ToString(), i_箱次);
                            temp++; //temp为i_箱次副本,控制i_箱次为正确数字
                            if (i_箱数 == 1)
                            {
                                r["CTN#"] = i_箱次.ToString();
                            }
                            else if (i_箱数 == 2)
                            {
                                r["CTN#"] = i_箱次.ToString() + "," + (i_箱次 + 1).ToString();
                            }
                            else
                            {
                                r["CTN#"] = i_箱次.ToString() + "-" + (i_箱次 + i_箱数 - 1).ToString();
                            }
                        }
                        else  //需要合箱的其他记录
                        {
                            if (i_箱数 == 1) r["CTN#"] = dic_cache[r["分组"].ToString()].ToString();
                            else if (i_箱数 == 2)
                                r["CTN#"] = dic_cache[r["分组"].ToString()].ToString() + "," + i_箱次.ToString().ToString();
                            else
                                r["CTN#"] = dic_cache[r["分组"].ToString()].ToString() + "," + i_箱次.ToString() + "-" + (i_箱次 + i_箱数 - 2).ToString();
                        }
                        //单独打印合箱那张箱贴
                        Dictionary<string, string> dic_other = new Dictionary<string, string>();
                        dic_other.Add("color", r["颜色"].ToString().Trim());
                        dic_other.Add("dw", r["计量单位"].ToString().Trim());
                        string sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                        dic_other.Add("jcbh", sss);
                        dic_other.Add("wlmc", r["产品名称"].ToString().Trim());
                        dic_other.Add("khddh", r["客户订单号"].ToString().Trim());
                        dic_other.Add("ggxh", r["产品型号"].ToString().Trim());
                        sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                        dic_other.Add("khgg", sss);
                        dic_other.Add("securam", compangName);
                        dic_other.Add("khmc", r["客户名称"].ToString());
                        dic_other.Add("time1", r["天差"].ToString().Trim());
                        dic_other.Add("mdg", r["目的国"].ToString().Trim());

                        dic_other.Add("sl", i_余数.ToString().Trim());
                        dic_other.Add("xc", dic_cache[r["分组"].ToString()].ToString().Trim());
                        if (checkBox1.Checked)
                        {
                            dic_other.Add("zxc", "/" + totalCtn.ToString().Trim());
                        }
                        else
                        {
                            dic_other.Add("zxc", "");

                        }
                        dic_打印队列.Add(dic_other, 1);
                        //Lprinter lP_other = new Lprinter(path, dic_other, str_printer箱贴, 1);
                        //lP_other.DoWork();
                        p_count = i_箱数 - 1;
                        i_箱次 = temp;
                        //正常打印其他
                        if (p_count > 0)
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add("color", r["颜色"].ToString().Trim());
                            dic.Add("dw", r["计量单位"].ToString().Trim());
                            sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                            dic.Add("jcbh", sss);
                            dic.Add("wlmc", r["产品名称"].ToString().Trim());
                            dic.Add("khddh", r["客户订单号"].ToString().Trim());
                            dic.Add("ggxh", r["产品型号"].ToString().Trim());
                            sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                            dic.Add("khgg", sss);
                            dic.Add("securam", compangName);
                            dic.Add("khmc", r["客户名称"].ToString());
                            dic.Add("mdg", r["目的国"].ToString().Trim());

                            dic.Add("time1", r["天差"].ToString().Trim());
                            dic.Add("sl", int_箱装数.ToString().Trim());
                            dic.Add("xc", i_箱次.ToString().Trim());
                            if (checkBox1.Checked)
                            {
                                dic.Add("zxc", "/" + totalCtn.ToString().Trim());
                            }
                            else
                            {
                                dic.Add("zxc", "");

                            }
                            dic_打印队列.Add(dic, p_count);
                        }
                        //Lprinter lP = new Lprinter(path, dic, str_printer箱贴, p_count);
                        //lP.DoWork();
                        i_箱次 = temp + p_count;
                    }
                    if (flag_save)
                    {
                        DataRow r_save = t_save.NewRow();
                        r_save["产品型号"] = r["产品型号"];
                        r_save["批号"] = textBox4.Text;
                        r_save["日期"] = r["日期"];
                        r_save["打印日期"] = t;

                        r_save["产品名称"] = r["产品名称"];
                        r_save["进仓编号"] = r["进仓编号"];
                        r_save["客户规格"] = r["客户规格"];
                        r_save["总数量"] = (int)Convert.ToDecimal(r["产品总数量"]);
                        r_save["CTN#"] = r["CTN#"];
                        r_save["客户订单号"] = r["客户订单号"];
                        t_save.Rows.Add(r_save);

                    }
                }
                display = new DataTable();
                display = t_save.Copy();
                BeginInvoke(new MethodInvoker(() =>
                {
                    gridControl2.DataSource = display;


                }));

                CZMaster.MasterSQL.Save_DataTable(t_save, "箱贴打印报表", strcon);
            }
            catch (Exception)
            {

                GC.Collect();
            }


        }

        //单独打印 计算用
#pragma warning disable IDE1006 // 命名样式
        private void fun_单独(string compangName)
#pragma warning restore IDE1006 // 命名样式
        {

            if (bl_printer == false)
            {
                string path_printer = Application.StartupPath + string.Format(@"\打印机配置.txt");
                x = ERPorg.Corg.ReadTxt(path_printer);
                str_printer箱贴 = x[0][0].ToString();
                //str_printer小标签 = x[1][0].ToString();
                bl_printer = true;
            }

            dic_打印队列 = new Dictionary<Dictionary<string, string>, int>();
            dic_cache = new Dictionary<string, int>();

            //算总箱数,
            DataTable tableTemp = dtM.Copy();
            tableTemp.Columns.Add("箱数", typeof(int));
            int totalCtn = 0;
            List<string> li = new List<string>();
            foreach (DataRow r in tableTemp.Rows)
            {
                int int_箱装数 = Convert.ToInt32(r["箱装数量"].ToString());
                int int_明细数量 = (int)Convert.ToDecimal(r["产品总数量"]);
                int i_box_count = (int)Math.Ceiling((decimal)int_明细数量 / (decimal)int_箱装数);
                r["箱数"] = i_box_count;
                if (r["分组"].ToString().Trim() != "")
                {
                    if (li.Contains(r["分组"].ToString().Trim()))
                    {
                        totalCtn += i_box_count - 1;
                    }
                    else
                    {
                        totalCtn += i_box_count;
                        li.Add(r["分组"].ToString().Trim());
                    }
                }
                else
                {
                    totalCtn += i_box_count;
                }
            }
            DataTable dtM_cp = dtM.Copy();
            if (!dtM_cp.Columns.Contains("CTN#"))
            {
                dtM_cp.Columns.Add("CTN#");
            }
            i_箱次 = 1;

            foreach (DataRow r in dtM_cp.Rows)
            {
                int temp = i_箱次;
                int int_箱装数 = Convert.ToInt32(r["箱装数量"].ToString());
                int int_明细数量 = (int)Convert.ToDecimal(r["产品总数量"].ToString());
                int i_箱数 = (int)Math.Ceiling((decimal)int_明细数量 / (decimal)int_箱装数); //就是打印的count 参数,不管合不合箱每个产品每放在一个箱子里就需要一个箱贴
                int i_余数 = int_明细数量 % int_箱装数;
                int p_count = i_箱数; // 当有组时 p_count=i_箱贴-1,另外一个需要单独打印 xc不同
                if (r["分组"].ToString().Trim() == "")  //没有组不需要合箱
                {
                    if (i_箱数 == 1)
                    {
                        r["CTN#"] = i_箱次.ToString();
                    }
                    else
                    {
                        r["CTN#"] = i_箱次.ToString() + "-" + (i_箱次 + i_箱数 - 1).ToString();
                    }



                    if (i_余数 == 0)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("color", r["颜色"].ToString().Trim());
                        dic.Add("dw", r["计量单位"].ToString().Trim());
                        string sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                        dic.Add("jcbh", sss);
                        dic.Add("wlmc", r["产品名称"].ToString().Trim());
                        dic.Add("khddh", r["客户订单号"].ToString().Trim());
                        dic.Add("ggxh", r["产品型号"].ToString().Trim());
                        sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                        dic.Add("khgg", sss);
                        dic.Add("securam", compangName);
                        dic.Add("mdg", r["目的国"].ToString().Trim());

                        dic.Add("khmc", r["客户名称"].ToString());
                        dic.Add("time1", r["天差"].ToString().Trim());
                        dic.Add("sl", int_箱装数.ToString().Trim());
                        dic.Add("xc", i_箱次.ToString().Trim());

                        if (checkBox1.Checked)
                        {
                            dic.Add("zxc", "/" + totalCtn.ToString().Trim());
                        }
                        else
                        {
                            dic.Add("zxc", "");

                        }
                        //
                        dic_打印队列.Add(dic, p_count);
                        //Lprinter lP = new Lprinter(path, dic, str_printer箱贴, p_count);
                        //lP.DoWork();
                        i_箱次 = i_箱次 + p_count;
                    }
                    else
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("color", r["颜色"].ToString().Trim());
                        dic.Add("dw", r["计量单位"].ToString().Trim());
                        string sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                        dic.Add("jcbh", sss);
                        dic.Add("wlmc", r["产品名称"].ToString().Trim());
                        dic.Add("khddh", r["客户订单号"].ToString().Trim());
                        dic.Add("ggxh", r["产品型号"].ToString().Trim());
                        sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                        dic.Add("khgg", r["客户规格"].ToString().Trim());
                        dic.Add("securam", compangName);
                        dic.Add("khmc", r["客户名称"].ToString());
                        dic.Add("time1", r["天差"].ToString().Trim());
                        dic.Add("mdg", r["目的国"].ToString().Trim());

                        dic.Add("sl", int_箱装数.ToString().Trim());
                        dic.Add("xc", i_箱次.ToString().Trim());
                        if (checkBox1.Checked)
                        {
                            dic.Add("zxc", "/" + totalCtn.ToString().Trim());
                        }
                        else
                        {
                            dic.Add("zxc", "");

                        }
                        dic_打印队列.Add(dic, p_count - 1);
                        //Lprinter lP = new Lprinter(path, dic, str_printer箱贴, p_count - 1);
                        //lP.DoWork();
                        i_箱次 = i_箱次 + p_count - 1;
                        Dictionary<string, string> dic_1 = new Dictionary<string, string>();
                        dic_1.Add("color", r["颜色"].ToString().Trim());
                        dic_1.Add("dw", r["计量单位"].ToString().Trim());
                        sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                        dic_1.Add("jcbh", r["进仓编号"].ToString().Trim());
                        dic_1.Add("wlmc", r["产品名称"].ToString().Trim());
                        dic_1.Add("khddh", r["客户订单号"].ToString().Trim());
                        dic_1.Add("ggxh", r["产品型号"].ToString().Trim());
                        sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                        dic_1.Add("khgg", r["客户规格"].ToString().Trim());
                        dic_1.Add("securam", compangName);
                        dic_1.Add("khmc", r["客户名称"].ToString());
                        dic_1.Add("mdg", r["目的国"].ToString().Trim());

                        dic_1.Add("time1", r["天差"].ToString().Trim());
                        dic_1.Add("sl", i_余数.ToString().Trim());
                        dic_1.Add("xc", i_箱次.ToString().Trim());
                        if (checkBox1.Checked)
                        {
                            dic_1.Add("zxc", "/" + totalCtn.ToString().Trim());
                        }
                        else
                        {
                            dic_1.Add("zxc", "");

                        }
                        dic_打印队列.Add(dic_1, 1);
                        //Lprinter lP_1 = new Lprinter(path, dic_1, str_printer箱贴, 1);
                        //lP_1.DoWork();
                        i_箱次++;
                    }
                }
                else
                {
                    //取组
                    if (!dic_cache.ContainsKey(r["分组"].ToString().Trim())) // 第一次遍历到该组记录
                    {
                        dic_cache.Add(r["分组"].ToString(), i_箱次);
                        temp++; //temp为i_箱次副本,控制i_箱次为正确数字
                        if (i_箱数 == 1)
                        {
                            r["CTN#"] = i_箱次.ToString();
                        }
                        else if (i_箱数 == 2)
                        {
                            r["CTN#"] = i_箱次.ToString() + "," + (i_箱次 + 1).ToString();
                        }
                        else
                        {
                            r["CTN#"] = i_箱次.ToString() + "-" + (i_箱次 + i_箱数 - 1).ToString();
                        }
                    }
                    else  //需要合箱的其他记录
                    {
                        if (i_箱数 == 1) r["CTN#"] = dic_cache[r["分组"].ToString()].ToString();
                        else if (i_箱数 == 2)
                            r["CTN#"] = dic_cache[r["分组"].ToString()].ToString() + "," + i_箱次.ToString().ToString();
                        else
                            r["CTN#"] = dic_cache[r["分组"].ToString()].ToString() + "," + i_箱次.ToString() + "-" + (i_箱次 + i_箱数 - 2).ToString();
                    }
                    //单独打印合箱那张箱贴
                    Dictionary<string, string> dic_other = new Dictionary<string, string>();
                    dic_other.Add("color", r["颜色"].ToString().Trim());
                    dic_other.Add("dw", r["计量单位"].ToString().Trim());
                    string sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                    dic_other.Add("jcbh", sss);
                    dic_other.Add("wlmc", r["产品名称"].ToString().Trim());
                    dic_other.Add("khddh", r["客户订单号"].ToString().Trim());
                    dic_other.Add("ggxh", r["产品型号"].ToString().Trim());
                    sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                    dic_other.Add("khgg", sss);
                    dic_other.Add("securam", compangName);
                    dic_other.Add("khmc", r["客户名称"].ToString());
                    dic_other.Add("time1", r["天差"].ToString().Trim());
                    dic_other.Add("mdg", r["目的国"].ToString().Trim());

                    dic_other.Add("sl", i_余数.ToString().Trim());
                    dic_other.Add("xc", dic_cache[r["分组"].ToString()].ToString().Trim());
                    if (checkBox1.Checked)
                    {
                        dic_other.Add("zxc", "/" + totalCtn.ToString().Trim());
                    }
                    else
                    {
                        dic_other.Add("zxc", "");

                    }
                    dic_打印队列.Add(dic_other, 1);
                    //Lprinter lP_other = new Lprinter(path, dic_other, str_printer箱贴, 1);
                    //lP_other.DoWork();
                    p_count = i_箱数 - 1;
                    i_箱次 = temp;
                    //正常打印其他
                    if (p_count > 0)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("color", r["颜色"].ToString().Trim());
                        dic.Add("dw", r["计量单位"].ToString().Trim());
                        sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                        dic.Add("jcbh", sss);
                        dic.Add("wlmc", r["产品名称"].ToString().Trim());
                        dic.Add("khddh", r["客户订单号"].ToString().Trim());
                        dic.Add("ggxh", r["产品型号"].ToString().Trim());
                        sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                        dic.Add("khgg", sss);
                        dic.Add("securam", compangName);
                        dic.Add("khmc", r["客户名称"].ToString());
                        dic.Add("mdg", r["目的国"].ToString().Trim());

                        dic.Add("time1", r["天差"].ToString().Trim());
                        dic.Add("sl", int_箱装数.ToString().Trim());
                        dic.Add("xc", i_箱次.ToString().Trim());
                        if (checkBox1.Checked)
                        {
                            dic.Add("zxc", "/" + totalCtn.ToString().Trim());
                        }
                        else
                        {
                            dic.Add("zxc", "");

                        }
                        dic_打印队列.Add(dic, p_count);
                    }
                    //Lprinter lP = new Lprinter(path, dic, str_printer箱贴, p_count);
                    //lP.DoWork();
                    i_箱次 = temp + p_count;
                }


            }


        }

        private void fun_预览(string compangName)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (bl_printer == false)
                {
                    string path_printer = Application.StartupPath + string.Format(@"\打印机配置.txt");
                    x = ERPorg.Corg.ReadTxt(path_printer);
                    str_printer箱贴 = x[0][0].ToString();
                    //str_printer小标签 = x[1][0].ToString();
                    bl_printer = true;
                }

                dic_打印队列 = new Dictionary<Dictionary<string, string>, int>();
                dic_cache = new Dictionary<string, int>();


                //算总箱数,
                DataTable tableTemp = dtM.Copy();
                tableTemp.Columns.Add("箱数", typeof(int));
                int totalCtn = 0;
                List<string> li = new List<string>();
                foreach (DataRow r in tableTemp.Rows)
                {
                    int int_箱装数 = Convert.ToInt32(r["箱装数量"].ToString());
                    int int_明细数量 = (int)Convert.ToDecimal(r["产品总数量"]);
                    int i_box_count = (int)Math.Ceiling((decimal)int_明细数量 / (decimal)int_箱装数);
                    r["箱数"] = i_box_count;
                    if (r["分组"].ToString().Trim() != "")
                    {
                        if (li.Contains(r["分组"].ToString().Trim()))
                        {
                            totalCtn += i_box_count - 1;
                        }
                        else
                        {
                            totalCtn += i_box_count;
                            li.Add(r["分组"].ToString().Trim());
                        }
                    }
                    else
                    {
                        totalCtn += i_box_count;
                    }
                }
                DataTable dtM_cp = dtM.Copy();
                if (!dtM_cp.Columns.Contains("CTN#"))
                {
                    dtM_cp.Columns.Add("CTN#");
                }
                i_箱次 = 1;
                string ss = "select  * from 箱贴打印报表 where 1=2";
                DataTable t_save = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                DateTime t = CPublic.Var.getDatetime();
                foreach (DataRow r in dtM_cp.Rows)
                {
                    int temp = i_箱次;
                    int int_箱装数 = Convert.ToInt32(r["箱装数量"].ToString());
                    int int_明细数量 = (int)Convert.ToDecimal(r["产品总数量"].ToString());
                    int i_箱数 = (int)Math.Ceiling((decimal)int_明细数量 / (decimal)int_箱装数); //就是打印的count 参数,不管合不合箱每个产品每放在一个箱子里就需要一个箱贴
                    int i_余数 = int_明细数量 % int_箱装数;
                    int p_count = i_箱数; // 当有组时 p_count=i_箱贴-1,另外一个需要单独打印 xc不同
                    if (r["分组"].ToString().Trim() == "")  //没有组不需要合箱
                    {
                        if (i_箱数 == 1)
                        {
                            r["CTN#"] = i_箱次.ToString();
                        }
                        else
                        {
                            r["CTN#"] = i_箱次.ToString() + "-" + (i_箱次 + i_箱数 - 1).ToString();
                        }



                        if (i_余数 == 0)
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add("color", r["颜色"].ToString().Trim());
                            dic.Add("dw", r["计量单位"].ToString().Trim());
                            string sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                            dic.Add("jcbh", sss);
                            dic.Add("wlmc", r["产品名称"].ToString().Trim());
                            dic.Add("khddh", r["客户订单号"].ToString().Trim());
                            dic.Add("ggxh", r["产品型号"].ToString().Trim());
                            sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                            dic.Add("khgg", sss);
                            dic.Add("securam", compangName);
                            dic.Add("mdg", r["目的国"].ToString().Trim());
                            dic.Add("khmc", r["客户名称"].ToString());
                            dic.Add("time1", r["天差"].ToString().Trim());
                            dic.Add("sl", int_箱装数.ToString().Trim());
                            dic.Add("xc", i_箱次.ToString().Trim());
                            if (checkBox1.Checked)
                            {
                                dic.Add("zxc", "/" + totalCtn.ToString().Trim());
                            }
                            else
                            {
                                dic.Add("zxc", "");

                            }

                            dic_打印队列.Add(dic, p_count);
                            //Lprinter lP = new Lprinter(path, dic, str_printer箱贴, p_count);
                            //lP.DoWork();
                            i_箱次 = i_箱次 + p_count;
                        }
                        else
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add("color", r["颜色"].ToString().Trim());
                            dic.Add("dw", r["计量单位"].ToString().Trim());
                            string sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                            dic.Add("jcbh", sss);
                            dic.Add("wlmc", r["产品名称"].ToString().Trim());
                            dic.Add("khddh", r["客户订单号"].ToString().Trim());
                            dic.Add("ggxh", r["产品型号"].ToString().Trim());
                            sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                            dic.Add("khgg", r["客户规格"].ToString().Trim());
                            dic.Add("securam", compangName);
                            dic.Add("khmc", r["客户名称"].ToString());
                            dic.Add("time1", r["天差"].ToString().Trim());
                            dic.Add("mdg", r["目的国"].ToString().Trim());

                            dic.Add("sl", int_箱装数.ToString().Trim());
                            dic.Add("xc", i_箱次.ToString().Trim());
                            if (checkBox1.Checked)
                            {
                                dic.Add("zxc", "/" + totalCtn.ToString().Trim());
                            }
                            else
                            {
                                dic.Add("zxc", "");

                            }
                            dic_打印队列.Add(dic, p_count - 1);
                            //Lprinter lP = new Lprinter(path, dic, str_printer箱贴, p_count - 1);
                            //lP.DoWork();
                            i_箱次 = i_箱次 + p_count - 1;
                            Dictionary<string, string> dic_1 = new Dictionary<string, string>();
                            dic_1.Add("color", r["颜色"].ToString().Trim());
                            dic_1.Add("dw", r["计量单位"].ToString().Trim());
                            sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                            dic_1.Add("jcbh", sss);
                            dic_1.Add("wlmc", r["产品名称"].ToString().Trim());
                            dic_1.Add("khddh", r["客户订单号"].ToString().Trim());
                            dic_1.Add("ggxh", r["产品型号"].ToString().Trim());
                            sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                            dic_1.Add("khgg", r["客户规格"].ToString().Trim());
                            dic_1.Add("securam", compangName);
                            dic_1.Add("khmc", r["客户名称"].ToString());
                            dic_1.Add("mdg", r["目的国"].ToString().Trim());

                            dic_1.Add("time1", r["天差"].ToString().Trim());
                            dic_1.Add("sl", i_余数.ToString().Trim());
                            dic_1.Add("xc", i_箱次.ToString().Trim());
                            if (checkBox1.Checked)
                            {
                                dic_1.Add("zxc", "/" + totalCtn.ToString().Trim());
                            }
                            else
                            {
                                dic_1.Add("zxc", "");

                            }
                            dic_打印队列.Add(dic_1, 1);

                            i_箱次++;
                        }
                    }
                    else
                    {
                        //取组
                        if (!dic_cache.ContainsKey(r["分组"].ToString().Trim())) // 第一次遍历到该组记录
                        {
                            dic_cache.Add(r["分组"].ToString(), i_箱次);
                            temp++; //temp为i_箱次副本,控制i_箱次为正确数字
                            if (i_箱数 == 1)
                            {
                                r["CTN#"] = i_箱次.ToString();
                            }
                            else if (i_箱数 == 2)
                            {
                                r["CTN#"] = i_箱次.ToString() + "," + (i_箱次 + 1).ToString();
                            }
                            else
                            {
                                r["CTN#"] = i_箱次.ToString() + "-" + (i_箱次 + i_箱数 - 1).ToString();
                            }
                        }
                        else  //需要合箱的其他记录
                        {
                            if (i_箱数 == 1) r["CTN#"] = dic_cache[r["分组"].ToString()].ToString();
                            else if (i_箱数 == 2)
                                r["CTN#"] = dic_cache[r["分组"].ToString()].ToString() + "," + i_箱次.ToString().ToString();
                            else
                                r["CTN#"] = dic_cache[r["分组"].ToString()].ToString() + "," + i_箱次.ToString() + "-" + (i_箱次 + i_箱数 - 2).ToString();
                        }
                        //单独打印合箱那张箱贴
                        Dictionary<string, string> dic_other = new Dictionary<string, string>();
                        dic_other.Add("color", r["颜色"].ToString().Trim());
                        dic_other.Add("dw", r["计量单位"].ToString().Trim());
                        string sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                        dic_other.Add("jcbh", sss);
                        dic_other.Add("wlmc", r["产品名称"].ToString().Trim());
                        dic_other.Add("khddh", r["客户订单号"].ToString().Trim());
                        dic_other.Add("ggxh", r["产品型号"].ToString().Trim());
                        sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                        dic_other.Add("khgg", sss);
                        dic_other.Add("securam", compangName);
                        dic_other.Add("khmc", r["客户名称"].ToString());
                        dic_other.Add("time1", r["天差"].ToString().Trim());
                        dic_other.Add("mdg", r["目的国"].ToString().Trim());

                        dic_other.Add("sl", i_余数.ToString().Trim());
                        dic_other.Add("xc", dic_cache[r["分组"].ToString()].ToString().Trim());
                        if (checkBox1.Checked)
                        {
                            dic_other.Add("zxc", "/" + totalCtn.ToString().Trim());
                        }
                        else
                        {
                            dic_other.Add("zxc", "");

                        }
                        dic_打印队列.Add(dic_other, 1);
                        //Lprinter lP_other = new Lprinter(path, dic_other, str_printer箱贴, 1);
                        //lP_other.DoWork();
                        p_count = i_箱数 - 1;
                        i_箱次 = temp;
                        //正常打印其他
                        if (p_count > 0)
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add("color", r["颜色"].ToString().Trim());
                            dic.Add("dw", r["计量单位"].ToString().Trim());
                            sss = r["进仓编号"].ToString().Trim() == "" ? "N/A" : r["进仓编号"].ToString().Trim();
                            dic.Add("jcbh", sss);
                            dic.Add("wlmc", r["产品名称"].ToString().Trim());
                            dic.Add("khddh", r["客户订单号"].ToString().Trim());
                            dic.Add("ggxh", r["产品型号"].ToString().Trim());
                            sss = r["客户规格"].ToString().Trim() == "" ? "N/A" : r["客户规格"].ToString().Trim();
                            dic.Add("khgg", sss);
                            dic.Add("securam", compangName);
                            dic.Add("khmc", r["客户名称"].ToString());
                            dic.Add("mdg", r["目的国"].ToString().Trim());

                            dic.Add("time1", r["天差"].ToString().Trim());
                            dic.Add("sl", int_箱装数.ToString().Trim());
                            dic.Add("xc", i_箱次.ToString().Trim());
                            if (checkBox1.Checked)
                            {
                                dic.Add("zxc", "/" + totalCtn.ToString().Trim());
                            }
                            else
                            {
                                dic.Add("zxc", "");

                            }
                            dic_打印队列.Add(dic, p_count);
                        }
                        //Lprinter lP = new Lprinter(path, dic, str_printer箱贴, p_count);
                        //lP.DoWork();
                        i_箱次 = temp + p_count;
                    }
                   
                        DataRow r_save = t_save.NewRow();
                        r_save["产品型号"] = r["产品型号"];
                        r_save["批号"] = textBox4.Text;
                        r_save["日期"] = r["日期"];
                        r_save["打印日期"] = t;

                        r_save["产品名称"] = r["产品名称"];
                        r_save["进仓编号"] = r["进仓编号"];
                        r_save["客户规格"] = r["客户规格"];
                        r_save["总数量"] = (int)Convert.ToDecimal(r["产品总数量"]);
                        r_save["CTN#"] = r["CTN#"];
                        r_save["客户订单号"] = r["客户订单号"];
                        t_save.Rows.Add(r_save);

                    
                }
                display = new DataTable();
                display = t_save.Copy();
                BeginInvoke(new MethodInvoker(() =>
                {
                    gridControl2.DataSource = display;


                }));

                //CZMaster.MasterSQL.Save_DataTable(t_save, "箱贴打印报表", strcon);
            }
            catch (Exception)
            {

                GC.Collect();
            }


        }


        //private void fun_p()
        //{
        //    string path = Application.StartupPath + string.Format(@"\Mode\外贸箱贴.lab");
        //    foreach (KeyValuePair<Dictionary<string, string>, int> kv in dic_打印队列)
        //    {
        //        Dictionary<string, string> dix = kv.Key as Dictionary<string, string>;
        //        Lprinter lP = new Lprinter(path, dix, str_printer箱贴, kv.Value);
        //        lP.DoWork();
        //    }
        //    GC.Collect();
        //    flag = false;
        //}

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string path = Application.StartupPath + string.Format(@"\Mode\外贸箱贴.lab"); ;

                //前提必须dic_打印队列有东西
                if (dic_打印队列 == null || dic_打印队列.Count == 0)
                {
                    string CoName = comboBoxEdit1.Text;
                    fun_单独(CoName);
                }
                int x = 0;
                if (!int.TryParse(textBox8.Text, out x)) throw new Exception("输入箱数不正确");
                //if (x < 0 || x > dic_打印队列.Count + 1) throw new Exception(string.Format("请输入正确的箱数,范围为1-{0}", dic_打印队列.Count + 1));
                foreach (KeyValuePair<Dictionary<string, string>, int> kv in dic_打印队列)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>(kv.Key as Dictionary<string, string>);

                    int y = Convert.ToInt32(dic["xc"]);
                    if (y == x || (y < x && y + kv.Value > x))
                    {
                        dic["xc"] = x.ToString();
                        Lprinter lP = new Lprinter(path, dic, str_printer箱贴, 1);
                        lP.DoWork();
                        //ERPorg.Corg cg = new ERPorg.Corg();
                        //cg.kill_lppa();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void simpleButton6_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            SaveFileDialog save = new SaveFileDialog();
            if (save.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(save.FileName, FileMode.Create, FileAccess.Write);
                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = string.Format("select * from 基础记录打印模板表 where 模板名 = '外贸导入空模板'");
                dtPP = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (dtPP.Rows.Count == 0) throw new Exception("没有该模板");
                System.IO.File.WriteAllBytes(save.FileName + ".xlsx", (byte[])dtPP.Rows[0]["数据"]);
                MessageBox.Show(" 下载成功");

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {


            try
            {
                button1.Enabled = false;
                //这里面临时用的 第几箱到第几箱的集合
                Dictionary<Dictionary<string, string>, int> dic_print = new Dictionary<Dictionary<string, string>, int>();
                int begin = 0;
                if (!int.TryParse(textBox2.Text, out begin)) throw new Exception("输入起始箱数不正确");
                int end = 0;
                if (!int.TryParse(textBox3.Text, out end)) end = 100000; //随便设置一个比较大的数字
                string path = Application.StartupPath + string.Format(@"\Mode\外贸箱贴.lab"); ;

                //前提必须dic_打印队列有东西
                if (dic_打印队列 == null || dic_打印队列.Count == 0)
                {
                    string CoName = comboBoxEdit1.Text;
                    fun_单独(CoName); //重新计算
                }
                int x = 0;

                //if (x < 0 || x > dic_打印队列.Count + 1) throw new Exception(string.Format("请输入正确的箱数,范围为1-{0}", dic_打印队列.Count + 1));
                foreach (KeyValuePair<Dictionary<string, string>, int> kv in dic_打印队列)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>(kv.Key as Dictionary<string, string>);

                    int y = Convert.ToInt32(dic["xc"]); //比如 第5箱开始   打印4箱  即 5 6 7 8 
                    int y_end = y + kv.Value - 1;
                    if (y <= begin && y_end >= begin) //箱次序号小于起始箱号
                    {
                        dic["xc"] = begin.ToString();
                        int dd = 0;
                        if (y_end <= end)
                        {
                            dd = y_end - begin + 1;
                        }
                        else                    //5 6 7 8 9  打印6-8箱  则 kv.value 5    变成4=end(8-5+1)
                        {
                            dd = end - begin + 1;

                        }
                        dic_print.Add(dic, dd);

                    }
                    else if (y_end > begin)    //前提 begin<y 再细分  
                    {
                        if (y_end > end) //  
                        {

                            dic_print.Add(dic, end - y + 1);

                        }
                        else if (y_end <= end) //   56789  客户要打印 4-10 的情况
                        {
                            dic_print.Add(dic, kv.Value);

                        }


                    }
                    //  剩余两种情况都不要打
                }

                Lprinter lp = new Lprinter(path, dic_print, str_printer箱贴);
                lp.DoWork();

                button1.Enabled = true;
                //ERPorg.Corg cg = new ERPorg.Corg();
                //cg.kill_lppa();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            dtM = new DataTable();
            gridControl1.DataSource = dtM;
            gridControl2.DataSource = null;
            flag_save = false;
            textBox4.Text = "";
            dic_打印队列 = new Dictionary<Dictionary<string, string>, int>();
            dic_cache = new Dictionary<string, int>();
        }

#pragma warning disable IDE1006 // 命名样式
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            dic_打印队列 = new Dictionary<Dictionary<string, string>, int>();
            dic_cache = new Dictionary<string, int>();
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Trim().Length >= 12)
            {
                string sql = string.Format("select  * from 箱贴打印报表 where 批号='{0}'", textBox4.Text.Trim());
                display = new DataTable();
                display = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                BeginInvoke(new MethodInvoker(() =>
                {
                    gridControl2.DataSource = display;

                }));

            }
        }
        //
        private void button3_Click(object sender, EventArgs e)
        {
            string CoName = comboBoxEdit1.Text;
            fun_预览(CoName);
        }
    }
}
