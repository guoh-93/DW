using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ERPproduct
{

    public partial class ui锁具打印 : UserControl
    {

        string con_BQ = CPublic.Var.geConn("BQ");
        string strcon = CPublic.Var.strConn;
        DataTable dt_历史;
        /// <summary>
        /// 打印的盒贴总数
        /// </summary>
        int int_盒贴数 = 0;

        bool Print_bl = false;
        DataTable dt_发货产品 = new DataTable();

        public ui锁具打印()
        {
            InitializeComponent();
        }

        //sn号 1 
        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && textBox2.Text.Trim() != "")
                {

                    string x = textBox2.Text;
                    fun_识别SN(x);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //sn号 2
        private void textBox4_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter && textBox4.Text.Trim() != "")
                {
                    string x = textBox4.Text;
                    fun_识别SN(x);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();
        private void fun_识别SN(string SN)
        {
            string x = string.Format(@"select  * from [Print_发货SN号对应表] where SerialNum='{0}'
             or SerialNum2 = '{0}' or LockID1 = '{0}' ", SN);
            DataTable tb = CZMaster.MasterSQL.Get_DataTable(x, strcon);
            if (tb.Rows.Count > 0)
            {
                Control focusedControl = null;
                // To get hold of the focused control:
                IntPtr focusedHandle = GetFocus();
                if (focusedHandle != IntPtr.Zero)
                    //focusedControl = Control.FromHandle(focusedHandle);
                    focusedControl = Control.FromChildHandle(focusedHandle);
                focusedControl.Text = "";
                throw new Exception("该产品信息已打印过盒贴");

            }
            if (SN.Substring(0, 2).ToLower() == "sk") //扫的是发货通知单 
            {
                string s = string.Format("select  * from 销售记录销售出库通知单明细表 where 出库通知单号 ='{0}'", SN);
                dt_发货产品 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                textBox1.Text = SN;
                if (textBox2.Text.Trim() == SN) textBox2.Text = "";
                if (textBox4.Text.Trim() == SN) textBox4.Text = "";

                s = $"select * from Print_发货SN号对应表 where 出库通知单号='{SN}'";
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DataRow[] tr = t.Select($"打印时间>'{CPublic.Var.getDatetime().Date}'");
                label12.Text = "今日:" + tr.Length.ToString() + "/总:" + t.Rows.Count;
                textBox2.Focus();
            }
            else
            {
                string s = string.Format("select  *  from  [ShareLockInfo] where CTNo='{0}'", SN);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, con_BQ);
                if (dt.Rows.Count > 0) //有lockID的那个产品  扫到的SN号应该 置于 textBox2   textBox5 要放其规格型号
                {
                    textBox3.Text = dt.Rows[0]["lockID"].ToString();
                    //20-3-18 w266要用另一个模板 
                    s = $@"select a.规格型号,bqxx.名称简称 from 生产记录生产工单表 a left join 基础物料标签维护信息表 bqxx on bqxx.物料编号=a.物料编码 
                    where 生产工单号='{dt.Rows[0]["TaskNo"].ToString()}'";
                    DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (t.Rows[0]["规格型号"].ToString().Contains("W266"))
                    {
                        textBox5.Text = t.Rows[0]["规格型号"].ToString();
                        textBox2.Text = SN;
                        textBox6.Text = t.Rows[0]["名称简称"].ToString();
                        comboBox1.Items.Add("labelPrint_盒贴_W266");
                        comboBox1.Text = "labelPrint_盒贴_W266";
                        comboBoxEdit1.Enabled = false;
                    }
                    else
                    {


                        s = string.Format(@" with t as (select  物料编码  from 借还申请表附表 where 申请批号 ='{1}' 
                                                        union select  物料编码  from 销售记录销售出库通知单明细表 where 出库通知单号 = '{1}' ) 
               
                                    select  bom.产品编码,子项编码 ,base.规格型号,bqxx.名称简称  from 生产记录生产工单表 gd 
                                    left join 基础数据物料BOM表 bom on gd.物料编码 = bom.子项编码
                                    inner join t   on t.物料编码 = 产品编码
                                    left join 基础数据物料信息表 base on base.物料编码 = 产品编码
                                    left join 基础物料标签维护信息表 bqxx on bqxx.物料编号=子项编码
                                    where 生产工单号 = '{0}' ", dt.Rows[0]["TaskNo"].ToString(), textBox1.Text.Trim());
                        DataTable dt_xxx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        if (dt_xxx.Rows.Count == 0)
                        {

                            Control focusedControl = null;
                            IntPtr focusedHandle = GetFocus();
                            if (focusedHandle != IntPtr.Zero)
                                focusedControl = Control.FromChildHandle(focusedHandle);
                            focusedControl.Text = "";
                            throw new Exception("根据规则此发货单和该SN号对应的产品匹配不到规格型号,请确认");
                        }
                        textBox5.Text = dt_xxx.Rows[0]["规格型号"].ToString();
                        textBox6.Text = dt_xxx.Rows[0]["名称简称"].ToString();
                        comboBox1.Items.Add("labelPrint_盒贴_E");
                        comboBox1.Items.Add("labelPrint_盒贴_S");

                        if (comboBoxEdit1.EditValue.ToString() == "SECURAM")
                            comboBox1.Text = "labelPrint_盒贴_S";
                        else
                            comboBox1.Text = "labelPrint_盒贴_E";
                        comboBoxEdit1.Enabled = true;
                    }
                    textBox2.Text = SN;
                    if (textBox4.Text != "" && textBox4.Text.Trim() == SN)
                    { textBox4.Text = ""; }
                    if (textBox1.Text.Trim() == SN) textBox1.Text = "";
                    //正式启用 再开
                    if (dt.Rows[0]["CheckFlag"].ToString() != "1")
                    {

                        throw new Exception("该产品未检验合格");
                    }


                    textBox4.Focus();
                }
                else //此处说明这个SN号是没有对应锁号的  那么另外一个就必须有 而且这个内容要写到SN2 的testbox 里面去
                {
                    s = string.Format(@"select  a.*, 名称简称 from [Print_ShareLockInfo] a
                        left join 生产记录生产工单表  gd on gd.生产工单号 = a.MakeOrder
                        left join 基础物料标签维护信息表 bqxx on bqxx.物料编号 = gd.物料编码  where CTNo = '{0}'", SN);
                    DataTable dt_2 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    if (dt_2.Rows.Count == 0) throw new Exception("该SN号未找到对应记录");
                    textBox4.Text = SN;
                    textBox7.Text = dt_2.Rows[0]["名称简称"].ToString().Trim();
                    if (textBox2.Text != "" && textBox2.Text.Trim() == SN)
                    {
                        textBox3.Text = "";
                        textBox2.Text = "";
                        textBox5.Text = "";
                        textBox6.Text = "";
                        //textBox7.Text = "";
                        textBox2.Focus();
                    }
                    else
                    {
                        textBox2.Focus();
                    }
                    if (textBox1.Text.Trim() == SN)
                    {
                        textBox1.Text = "";
                        textBox2.Focus();
                    }

                }
                ////这边前五位的 后位,即第2到4位对应的是产品 需要 取对应表中的  产品简称 
                //string x = textBox2.Text.Trim().Substring(1, 3);
                //s = string.Format("select  * from  [Label_Print产品简码对应表] where ParentCode='{0}'", x);
                //dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                //if (dt.Rows.Count > 0)
                //{
                //    textBox6.Text = dt.Rows[0]["labelIDName"].ToString();
                //}
            }
            if (textBox1.Text.Trim() != "" && textBox2.Text.Trim() != "" && textBox4.Text.Trim() != "")
            {
                simpleButton1_Click(null, null);

                //kill_lppa();

            }
        }
        //private void kill_lppa()
        //{
        //    var process = Process.GetProcesses().Where(pr => pr.ProcessName.Contains("lppa.exe"));
        //    foreach (var pk in process)
        //    {
        //        try
        //        {
        //            pk.Kill();
        //        }
        //        catch
        //        {
        //           continue;
        //        }
        //    }
        //}
        private void fun_print()
        {
            string Printer_盒贴 = "";
            try
            {
                Printer_盒贴 = CPublic.Var.li_CFG["printer_chest"].ToString();
            }
            catch (Exception)
            {
                throw new Exception("标签打印机未配置,printer_chest未找到");
            }
            string s_发货单号 = textBox1.Text;
            string CTNo1 = textBox2.Text.Trim();
            string CTNo2 = textBox4.Text;
            string lockID1 = textBox3.Text;
            string str_产品简码1 = textBox6.Text;
            string str_产品简码2 = textBox7.Text;
            //string lockID2 = textBox5.Text;
            //string str_产品简码2 = textBox7.Text;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("CTNo1", CTNo1);
            dic.Add("CTNo2", CTNo2);
            dic.Add("labelIDName1", str_产品简码1);
            dic.Add("labelIDName2", str_产品简码2);
            dic.Add("CoName", comboBoxEdit1.EditValue.ToString());
            dic.Add("lockID1", lockID1);
            dic.Add("ggxh", textBox5.Text.Trim());
            //dic.Add("ggxh",  );
            //dic.Add("LockID", PrintRow["LockID"].ToString().Trim());
            //dic.Add("FCCID", PrintRow["FCCID"].ToString().Trim());
            string path = "";
            path = Application.StartupPath + $@"\Mode\{comboBox1.Text}.lab";
            //if (comboBoxEdit1.EditValue.ToString() == "SECURAM")
            //    path = Application.StartupPath + string.Format(@"\Mode\labelPrint_盒贴_S.lab");
            //else
            //    path = Application.StartupPath + string.Format(@"\Mode\labelPrint_盒贴_E.lab");
            DataTable dt_历史clone = dt_历史.Clone();
            DataRow dr = dt_历史clone.NewRow();
            dr["出库通知单号"] = s_发货单号;
            dr["SerialNum"] = CTNo1;
            dr["LockID1"] = lockID1;
            dr["产品简称1"] = str_产品简码1;
            dr["产品简称2"] = str_产品简码2;
            dr["SerialNum2"] = CTNo2;
            dr["公司名称"] = comboBoxEdit1.EditValue.ToString();
            dr["打印时间"] = CPublic.Var.getDatetime();
            dr["规格型号"] = textBox5.Text.Trim();
            dr["模板"] = comboBox1.Text;
            dt_历史clone.Rows.Add(dr);
            CZMaster.MasterSQL.Save_DataTable(dt_历史clone, "Print_发货SN号对应表", strcon);
            dt_历史.ImportRow(dr);
            dt_历史.AcceptChanges();
            Lprinter lp = new Lprinter(path, dic, Printer_盒贴, 1);
            lp.DoWork();

            int_盒贴数++;
            //if (int_盒贴数 == Convert.ToInt32(textBox8.Text)) //达到箱装数量或者为最后一箱 需要打印一张 装箱清单  
            //{int_盒贴数 = 0;}
            string s = $"select LockID1,打印时间 from Print_发货SN号对应表 where 出库通知单号='{s_发货单号}'";
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataRow[] tr = t.Select($"打印时间>'{CPublic.Var.getDatetime().Date}'");
            label12.Text = "今日:" + tr.Length.ToString() + "/总:" + t.Rows.Count;
            Print_bl = false;
            gridControl1.DataSource = dt_历史;
            textBox2.Text = "";
            textBox3.Text = "";
            textBox6.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox7.Text = "";
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_print();
                //kill_lppa();
                GC.Collect();
                ERPorg.Corg.FlushMemory();
                Print_bl = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_check()
        {
            if (textBox1.Text == "") throw new Exception("发货单号为空");
            if (textBox2.Text == "") throw new Exception("SN1为空");
            if (textBox4.Text == "") throw new Exception("SN2为空");
            //if(!int.TryParse(textBox8.Text, out ii))
            //{
            //    throw new Exception("箱装数量输入不正确");
            //}
        }
        private void fun_load()
        {
            DateTime t = CPublic.Var.getDatetime().Date;
            string s = string.Format("select  *  from Print_发货SN号对应表 where  打印时间>'{0}'", t.AddDays(-1));
            dt_历史 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gridControl1.DataSource = dt_历史;
            textBox1.Focus();
        }
        private void ui锁具打印_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime t2 = CPublic.Var.getDatetime().Date;
                DateTime t1 = t2.AddMonths(-1);
                dateEdit1.EditValue = t1;
                dateEdit2.EditValue = t2;
                fun_load();
                comboBoxEdit1.EditValue = "SECURAM";//Nanjing Easthouse Electrical Co., Ltd.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {

        }
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && textBox1.Text.Trim() != "")
                {
                    string x = textBox1.Text;
                    fun_识别SN(x);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 补打ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string Printer_盒贴 = "";
                try
                {
                    Printer_盒贴 = CPublic.Var.li_CFG["printer_chest"].ToString();
                }
                catch (Exception)
                {
                    throw new Exception("标签打印机未配置,printer_chest未找到");
                }
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);

                string s = string.Format("select  *  from  [Print_ShareLockInfo] where CTNo='{0}'", dr["SerialNum"]);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                //19-12-18 修改
                string s_规格型号 = dr["规格型号"].ToString();
                //if (dt.Rows.Count > 0) //有lockID的那个产品  扫到的SN号应该 置于 textBox2   textBox5 要放其规格型号
                //{
                //    // textBox3.Text = dt.Rows[0]["lockID"].ToString();
                //    s = string.Format(@"select  bom.产品编码,base.规格型号  from 生产记录生产工单表 gd 
                //                        left join 基础数据物料BOM表 bom on gd.物料编码 = bom.子项编码
                //                        inner join 销售记录销售出库通知单明细表 ckmx on ckmx.物料编码 = 产品编码
                //                        left join 基础数据物料信息表 base on base.物料编码 = 产品编码
                //                        where 生产工单号 = '{0}' and 出库通知单号 = '{1}'", dt.Rows[0]["MakeOrder"].ToString(), dr["出库通知单号"]);
                //    DataTable dt_xxx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                //    if (dt_xxx.Rows.Count == 0) throw new Exception("根据规则此发货单和改SN号对应的产品匹配不到规格型号,请确认");
                //    s_规格型号 = dt_xxx.Rows[0]["规格型号"].ToString();
                //}
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("CTNo1", dr["SerialNum"].ToString());
                dic.Add("CTNo2", dr["SerialNum2"].ToString());
                dic.Add("labelIDName1", dr["产品简称1"].ToString());
                dic.Add("labelIDName2", dr["产品简称2"].ToString());
                dic.Add("CoName", dr["公司名称"].ToString());
                dic.Add("lockID1", dr["lockID1"].ToString());
                dic.Add("ggxh", s_规格型号);
                string path = "";
                if (dr["模板"].ToString() == "")
                    path = Application.StartupPath + string.Format(@"\Mode\labelPrint_盒贴_S.lab");
                else
                    path = Application.StartupPath + $@"\Mode\{dr["模板"].ToString()}.lab";
                //if (dr["公司名称"].ToString().Trim() == "SECURAM")
                //    path = Application.StartupPath + $@"\Mode\ +{comboBox1.Text}.lab";  // string.Format(@"\Mode\labelPrint_盒贴_S.lab");
                //else
                //    path = Application.StartupPath + string.Format(@"\Mode\labelPrint_盒贴_E.lab");
                //// string path = Application.StartupPath + string.Format(@"\Mode\labelPrint_盒贴.lab");
                Lprinter lp = new Lprinter(path, dic, Printer_盒贴, 1);
                lp.DoWork();
                // kill_lppa();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void gridControl1_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e != null && e.Button == MouseButtons.Right)
        //    {
        //        contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));

        //    }
        //}

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));

            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
            string s = string.Format("select  *  from Print_发货SN号对应表 where  打印时间>'{0}' and 打印时间<'{1}' ", t1, t2);
            dt_历史 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gridControl1.DataSource = dt_历史;
            textBox1.Focus();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //导出excel
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void comboBoxEdit1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                if (comboBoxEdit1.EditValue.ToString() == "SECURAM")
                    comboBox1.Text = "labelPrint_盒贴_S";
                else
                    comboBox1.Text = "labelPrint_盒贴_E";

            }
        }

        private void 删除记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
