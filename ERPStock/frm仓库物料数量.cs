using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using DevExpress.XtraPrinting;
using System.IO;
namespace ERPStock
{
    public partial class frm仓库物料数量 : UserControl
    {
        #region  变量
        string strconn = CPublic.Var.strConn;
        DataTable dt;
        DataTable dt_仓库;

        string PrinterName = "";
        string cfgfilepath = "";
        #endregion
        bool s_删除 = false;

        #region 加载
        public frm仓库物料数量()
        {
            InitializeComponent();
        }
        private void frm仓库物料数量_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(this, this.Name, cfgfilepath);
                //timer1.Start();
                fun_load();
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion
        #region 函数
        private void fun_check()
        {

            if (checkBox1.Checked == true)
            {
                if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                {
                    throw new Exception("未选择仓库");
                }

            }

            if (checkBox2.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择物料");
                }


            }


            if (checkBox3.Checked == true)
            {
                if (textBox1.Text == "")
                {
                    throw new Exception("未选择货架");
                }

            }
        }

        private void fun_search()
        {
            // int index = gridView1.FocusedRowHandle;

            DateTime dtime = CPublic.Var.getDatetime();
            dtime = new DateTime(dtime.Year, dtime.Month, dtime.Day);


            dtime = Convert.ToDateTime(dtime.ToString("yyyy-MM-dd"));
            // dtime = dtime.AddDays(-dtime.Day + 1);   //  本月初   谢刚华要求 当天直接往前推三个月 推一个月 和 半年

            DateTime dtime1 = dtime.AddMonths(-3);  //三个月前
            DateTime dtime2 = dtime.AddMonths(-1);  //一个月前
            DateTime dtime3 = dtime.AddMonths(-6);  //半年前

            string t0 = dtime1.ToString("yyyy-MM-dd");
            string t1 = dtime.ToString("yyyy-MM-dd");
            string t2 = dtime2.ToString("yyyy-MM-dd");
            string t3 = dtime3.ToString("yyyy-MM-dd");

            //20-3-31
            string s = "and 明细类型 not in('借用出库','拆单申请出库','形态转换出库')";

            string sql = string.Format($@"select kc.*,base.物料名称 as 名称,产品线,base.规格,base.停用,base.n原ERP规格型号,
                 base.原ERP物料编号,库存下限,base.图纸编号 as 图纸编号1,kc.仓库号,kc.仓库名称
                    ,base.物料类型,计量单位,base.大类,base.小类,kc.货架描述,供应商名称
                     ,采购供应商备注,a.季度用量,b.月度用量,半年用量,base.物料等级,base.工时,base.BOM确认,环保 ,isnull(检验标准,0) 检验标准
                            from  仓库物料数量表 kc left join 基础数据物料信息表 base on  kc.物料编码=base.物料编码
                            left join 采购供应商表  on 采购供应商表.供应商ID=base.供应商编号
                            left join  (select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库' {s}  and  出入库时间>'{t0}' and 
                            出入库时间<'{t1}'   group by 物料编码)a on  kc.物料编码=a.物料编码  
                            left join  (select 物料编码,-sum(实效数量)as 月度用量  from 仓库出入库明细表 where  出库入库='出库' {s} and  出入库时间>'{t2}' and 
                            出入库时间<'{t1}'  group by 物料编码)b on  kc.物料编码=b.物料编码  
                            left join  (select 物料编码,-sum(实效数量)as 半年用量  from 仓库出入库明细表 where  出库入库='出库' {s} and  出入库时间>'{t3}' and 
                            出入库时间<'{t1}'  group by 物料编码)c on  kc.物料编码=c.物料编码 
                            left join  (select  物料编码,单价 as 结转单价 from 仓库月出入库结转表  where 年={dtime.Year} and 月={dtime.Month - 1})jz on jz.物料编码= kc.物料编码 
                            left join  (select  产品编码,检验标准 from (
                                            select  产品编码,CONVERT(bit,1) 检验标准 from [基础数据物料检验要求表] group by 产品编码
                                            union   select   cpbh 产品编码,CONVERT(bit,1) 检验标准 from [ZZ_JYXM] group by cpbh) a  group by 产品编码,检验标准) bz 
                            on bz.产品编码 =base.物料编码   where 1=1 ");

            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format("and base.物料编码='{0}'", searchLookUpEdit1.EditValue.ToString());
            }

            if (checkBox1.Checked == true)
            {
                sql = sql + string.Format("and kc.仓库号='{0}'", searchLookUpEdit2.EditValue.ToString());
            }
            if (checkBox3.Checked == true)
            {
                sql = sql + string.Format("and kc.货架描述 like'%{0}%'", textBox1.Text.ToString());
            }
            if (checkBox6.Checked == false)
            {
                sql = sql + "and 停用=0";
            }
            if (checkBox7.Checked == false)
            {
                sql = sql + "and 库存总数<>0";
            }




            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dt = new DataTable();

               
                da.Fill(dt);
            }
           
        
                gridControl1.DataSource = dt;

        

            //    if(index>0)  gridView1.FocusedRowHandle = index;
        }
        private void fun_search_2(string itemID,string stockID)
        {
            // int index = gridView1.FocusedRowHandle;

            DateTime dtime = CPublic.Var.getDatetime();
            dtime = new DateTime(dtime.Year, dtime.Month, dtime.Day);
            dtime = Convert.ToDateTime(dtime.ToString("yyyy-MM-dd"));
            // dtime = dtime.AddDays(-dtime.Day + 1);   //  本月初   谢刚华要求 当天直接往前推三个月 推一个月 和 半年

            DateTime dtime1 = dtime.AddMonths(-3);  //三个月前
            DateTime dtime2 = dtime.AddMonths(-1);  //一个月前
            DateTime dtime3 = dtime.AddMonths(-6);  //半年前

            string t0 = dtime1.ToString("yyyy-MM-dd");
            string t1 = dtime.ToString("yyyy-MM-dd");
            string t2 = dtime2.ToString("yyyy-MM-dd");
            string t3 = dtime3.ToString("yyyy-MM-dd");

            //20-3-31
            string s = "and 明细类型 not in('借用出库','拆单申请出库','形态转换出库')";


            string sql = string.Format($@"select kc.*,base.物料名称 as 名称,产品线,base.规格,base.停用,base.n原ERP规格型号,
                 base.原ERP物料编号,库存下限,base.图纸编号 as 图纸编号1,kc.仓库号,kc.仓库名称
                    ,base.物料类型,计量单位,base.大类,base.小类,kc.货架描述,供应商名称
                     ,采购供应商备注,a.季度用量,b.月度用量,半年用量,base.物料等级,base.工时,base.BOM确认,环保,isnull(检验标准,0) 检验标准
                            from  仓库物料数量表 kc left join 基础数据物料信息表 base on  kc.物料编码=base.物料编码
                            left join 采购供应商表  on 采购供应商表.供应商ID=base.供应商编号
                            left join  (select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库' {s} and  出入库时间>'{t0}' and 
                            出入库时间<'{t1}'  group by 物料编码)a on  kc.物料编码=a.物料编码  
                            left join  (select 物料编码,-sum(实效数量)as 月度用量  from 仓库出入库明细表 where  出库入库='出库' {s} and  出入库时间>'{t2}' and 
                            出入库时间<'{t1}'  group by 物料编码)b on  kc.物料编码=b.物料编码  
                            left join  (select 物料编码,-sum(实效数量)as 半年用量  from 仓库出入库明细表 where  出库入库='出库' {s} and  出入库时间>'{t3}' and 
                            出入库时间<'{t1}'  group by 物料编码)c on  kc.物料编码=c.物料编码 
                            left join  (select  物料编码,单价 as 结转单价 from 仓库月出入库结转表  where 年={dtime.Year} and 月={dtime.Month - 1})jz on jz.物料编码= kc.物料编码 
                            left join  (select  产品编码,检验标准 from (
                                            select  产品编码,CONVERT(bit,1) 检验标准 from [基础数据物料检验要求表] group by 产品编码
                                            union   select   cpbh 产品编码,CONVERT(bit,1) 检验标准 from [ZZ_JYXM] group by cpbh) a  group by 产品编码,检验标准) bz 
                            on bz.产品编码 =base.物料编码 
                            where 1=1 and base.物料编码='{itemID}' and kc.仓库号='{stockID}' ");

          

            DataTable temp = new DataTable();

            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {

                da.Fill(temp);


                //只有一条
                BeginInvoke(new MethodInvoker(() =>
                {
                    foreach (DataRow dr in temp.Rows)
                    {
                        DataRow[] tr = dt.Select(string.Format("物料编码='{0}' and 仓库号='{1}'", dr["物料编码"], dr["仓库号"]));
                        if (tr.Length > 0)
                            tr[0].ItemArray = dr.ItemArray;
                    }
                }));
               
 
            }
            //    if(index>0)  gridView1.FocusedRowHandle = index;
        }


        private void fun_load()
        {
            //            string sql = @"select 仓库物料数量表.* ,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.仓库名称,
            //              标准单价,n销售单价,基础数据物料信息表.物料类型,基础数据物料信息表.大类,基础数据物料信息表.小类,基础数据物料信息表.货架描述 
            //                            from  仓库物料数量表 left join 基础数据物料信息表 on  仓库物料数量表.物料编码=基础数据物料信息表.物料编码";

            //            //string sql = "select *,基础数据物料信息表.n原ERP规格型号 as 规格型号  from 仓库物料数量表  left join 基础数据物料信息表  on 仓库物料数量表.物料编码 = 基础数据物料信息表.物料编码  ";
            //            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            //            {
            //                dtM = new DataTable();

            //                da.Fill(dtM);
            //                gridControl1.DataSource = dtM;
            //            }
            string sql_仓库 = "SELECT [属性值] as 仓库名称,属性字段1 as 仓库号 FROM [基础数据基础属性表] where 属性类别 ='仓库类别'";
            dt_仓库 = new DataTable();
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_仓库, strconn);
            searchLookUpEdit2.Properties.DataSource = dt_仓库;
            searchLookUpEdit2.Properties.ValueMember = "仓库号";
            searchLookUpEdit2.Properties.DisplayMember = "仓库名称";
            string sql4 = "select 物料编码,物料名称,规格型号 from 基础数据物料信息表 where 停用=0";
            DataTable dt_物料 = new DataTable();
            SqlDataAdapter da_物料 = new SqlDataAdapter(sql4, strconn);
            da_物料.Fill(dt_物料);
            searchLookUpEdit1.Properties.DataSource = dt_物料;
            searchLookUpEdit1.Properties.ValueMember = "物料编码";
            searchLookUpEdit1.Properties.DisplayMember = "物料编码";

            string sql = string.Format(@"select * from 人事基础员工表 where 课室='物管课'  and right(班组,1)='库' 
                        and 在职状态 ='在职'  and 员工号='{0}'", CPublic.Var.LocalUserID);
            DataTable t_c = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            if (t_c.Rows.Count > 0 || CPublic.Var.LocalUserID == "admin")
            {
                label1.Visible = true;
                checkBox4.Visible = true;
                checkBox5.Visible = true;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                label1.Visible = false;

                checkBox4.Visible = false;
                checkBox5.Visible = false;
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            }

        }
        #endregion


        #region 界面操作
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (r != null)
            {
                try
                {
                   
                        Thread th = new Thread(() =>
                        {
                            fun_search_2(r["物料编码"].ToString(), r["仓库号"].ToString());

                        });
                        th.IsBackground = true;
                        th.Start();

                    
                }
                catch (Exception ex)
                {

                }


                if (e.Clicks == 2)
                {


                    frm仓库物料数量明细 ui = new frm仓库物料数量明细(r["物料编码"].ToString().Trim(), r["仓库号"].ToString());
                    string name = string.Format("物料明细({0}_{1})", r["物料编码"].ToString().Trim(), r["物料名称"].ToString().Trim());
                    CPublic.UIcontrol.AddNewPage(ui, name);

                    ui.Dock = DockStyle.Fill;

                    //test
                    //ERPStock.UI物料BOM视图 frm = new UI物料BOM视图(r["物料编码"].ToString());
                    //CPublic.UIcontrol.AddNewPage(frm, "");

                }

                if (e != null && e.Button == MouseButtons.Right)
                {
                   //timer1.Stop();
                    //string sql = string.Format("select * from 仓库物料数量表 where 物料编码 = '{0}' and 仓库号 = '{1}'", r["物料编码"], r["仓库号"]);
                    //DataTable dt1 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    //sql = string.Format("select count(*)条数 from 仓库出入库明细表 where 物料编码  ='{0}' and 仓库号 = '{1}'", r["物料编码"], r["仓库号"]);
                    //DataTable dt2 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    //if (Convert.ToDecimal(dt1.Rows[0]["库存总数"]) == 0 && Convert.ToInt32(dt2.Rows[0]["条数"]) == 0)
                    //{
                    //    删除ToolStripMenuItem.Visible = true;
                    //}
                    //else
                    //{
                    //    删除ToolStripMenuItem.Visible = false;
                    //}

                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                    gridView1.CloseEditor();
                    this.BindingContext[dt].EndCurrentEdit();

                }
            }
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        #endregion
        //刷新
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception)
            {

                throw;
            }
        }
        //导出
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new XlsxExportOptions(TextExportMode.Text, false, false);
                gridControl1.ExportToXlsx(saveFileDialog.FileName, options);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }



        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                fun_check();
                fun_search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void fun_打印()
        {

            Thread thDo;
            thDo = new Thread(Dowork);
            thDo.IsBackground = true;
            thDo.Start();

        }

        public void Dowork()
        {
            DataTable dtx = dt.Clone();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                dtx.ImportRow(gridView1.GetDataRow(i));
            }

            DataView dv = new DataView(dtx);
            dv.Sort = "货架描述";
            DataTable dt_dy = dv.ToTable();
            List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
            foreach (DataRow drr in dt_dy.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                dic.Add("wldj", drr["物料等级"].ToString());
                dic.Add("hjh", drr["货架描述"].ToString());
                dic.Add("wlmc", drr["名称"].ToString());
                dic.Add("wlbh", drr["物料编码"].ToString().Trim());
                dic.Add("ggxh", drr["规格型号"].ToString().Trim());
                dic.Add("ckmc", drr["仓库名称"].ToString().Trim());
                dic.Add("ckh", drr["仓库号"].ToString().Trim());

                li.Add(dic);
                //string path = "";
                //if (checkBox4.Checked == true)
                //{

                //      path = Application.StartupPath + string.Format(@"\Mode\货架打印.lab");
                //}
                //else
                //{
                //      path = Application.StartupPath + string.Format(@"\Mode\货架打印小.lab");
                //}
                //LabelPrint.LPrinter lp = new LabelPrint.LPrinter(path, dic, PrinterName, 1);
                //lp.DoWork();
            }
            string path = "";
            if (checkBox4.Checked == true)
            {

                path = Application.StartupPath + string.Format(@"\Mode\货架打印.lab");
            }
            else
            {
                path = Application.StartupPath + string.Format(@"\Mode\货架打印小.lab");
            }
            LabelPrint.LPrinter lp = new LabelPrint.LPrinter(path, li, PrinterName, 1);
            lp.DoWork();
        }

        private void fun_check_dy()
        {
            //if (checkBox3.Checked!=true && checkBox2.Checked!= true)
            //{
            //    throw new Exception("未选择物料或者货架号不可打印");
            //}
            if (gridView1.DataRowCount == 0)
            {
                throw new Exception("没有选择物料打印");
            }
            if (gridView1.DataRowCount > 100)
            {
                throw new Exception("当前打印标贴数大于100");
            }
            if (checkBox4.Checked != true && checkBox5.Checked != true)
            {

                throw new Exception("请选择要打印的模板大小再进行打印");
            }

        }
        private void barLargeButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check_dy();

                if (MessageBox.Show(string.Format("确定打印？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                    this.printDialog1.Document = this.printDocument1;
                    DialogResult dr = this.printDialog1.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                        fun_打印();
                        //LabelPrint.LPrinter lp=new LabelPrint.LPrinter(path, dic, str_打印机, 1)
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void 修改货架信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr_当前行 = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            ERPStock.仓库物料数量货架修改 fm = new 仓库物料数量货架修改(dr_当前行);

            fm.ShowDialog();

            if (fm.fl)
            {
                dr_当前行["货架描述"] = fm.hjms;
            }

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            try
            {
                // fun_check_dy();

                if (MessageBox.Show(string.Format("确定打印？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                    this.printDialog1.Document = this.printDocument1;
                    DialogResult dr = this.printDialog1.ShowDialog();

                    PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                    Dowork21();
                    //LabelPrint.LPrinter lp=new LabelPrint.LPrinter(path, dic, str_打印机, 1)
                    // }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }



        public void Dowork21()
        {
            DataTable dtx = dt.Clone();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                dtx.ImportRow(gridView1.GetDataRow(i));
            }

            DataView dv = new DataView(dtx);
            dv.Sort = "货架描述";
            DataTable dt_dy = dv.ToTable();
            List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
            foreach (DataRow drr in dt_dy.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();


                //string sql = string.Format("select 规格型号,物料名称, 物料等级 from 基础数据物料信息表 where  物料编码='{0}'", drr["物料编码"]);
                //DataRow drj = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                dic.Add("wldj", drr["物料等级"].ToString());
                // dic.Add("itemid", drr["ItemId"].ToString());
                dic.Add("hjh", drr["货架描述"].ToString());
                dic.Add("wlmc", drr["物料名称"].ToString());
                dic.Add("wlbh", drr["物料编码"].ToString().Trim());
                dic.Add("ggxh", drr["规格型号"].ToString().Trim());
                dic.Add("ckmc", drr["仓库名称"].ToString().Trim());
                dic.Add("ckh", drr["仓库号"].ToString().Trim());
                //dic.Add("free1", drr["备注1"].ToString().Trim());
                //dic.Add("free2", drr["备注2"].ToString().Trim());
                li.Add(dic);
                //string path = "";
                //if (checkBox4.Checked == true)
                //{

                //      path = Application.StartupPath + string.Format(@"\Mode\货架打印.lab");
                //}
                //else
                //{
                //      path = Application.StartupPath + string.Format(@"\Mode\货架打印小.lab");
                //}
                //LabelPrint.LPrinter lp = new LabelPrint.LPrinter(path, dic, PrinterName, 1);
                //lp.DoWork();
            }
            string path = Application.StartupPath + string.Format(@"\Mode\temp货架版本1.lab");
            LabelPrint.LPrinter lp = new LabelPrint.LPrinter(path, li, PrinterName, 1);
            lp.DoWork();
        }
        public void Dowork版本2()
        {
            DataTable dtx = dt.Clone();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                dtx.ImportRow(gridView1.GetDataRow(i));
            }

            DataView dv = new DataView(dtx);
            dv.Sort = "货架描述";
            DataTable dt_dy = dv.ToTable();
            List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
            foreach (DataRow drr in dt_dy.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();


                //string sql = string.Format("select 规格型号,物料名称, 物料等级 from 基础数据物料信息表 where  物料编码='{0}'", drr["物料编码"]);
                //DataRow drj = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                dic.Add("wldj", drr["物料等级"].ToString());

                dic.Add("wlmc", drr["物料名称"].ToString());
                dic.Add("wlbh", drr["物料编码"].ToString().Trim());
                dic.Add("ggxh", drr["规格型号"].ToString().Trim());
                dic.Add("ckmc", drr["仓库名称"].ToString().Trim());
                dic.Add("ckh", drr["仓库号"].ToString().Trim());

                li.Add(dic);

            }

            List<Dictionary<string, string>> lI2 = new List<Dictionary<string, string>>();

            foreach (DataRow drr in dt_dy.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();


                //string sql = string.Format("select 规格型号,物料名称, 物料等级 from 基础数据物料信息表 where  物料编码='{0}'", drr["物料编码"]);
                //DataRow drj = CZMaster.MasterSQL.Get_DataRow(sql, strcon);
                //  dic.Add("wldj", drr["物料等级"].ToString());
                // dic.Add("itemid", drr["ItemId"].ToString());
                dic.Add("hjh", drr["货架描述"].ToString());
                // dic.Add("wlmc", drr["物料名称"].ToString());
                dic.Add("wlbh", drr["物料编码"].ToString().Trim());
                // dic.Add("ggxh", drr["规格型号"].ToString().Trim());
                dic.Add("ckmc", drr["仓库名称"].ToString().Trim());
                dic.Add("ckh", drr["仓库号"].ToString().Trim());
                //dic.Add("free1", drr["备注1"].ToString().Trim());
                //dic.Add("free2", drr["备注2"].ToString().Trim());
                lI2.Add(dic);

            }


            string path = Application.StartupPath + string.Format(@"\Mode\temp货架.lab");
            string path2 = Application.StartupPath + string.Format(@"\Mode\temp货架描述.lab");
            LabelPrint.LPrinter lp = new LabelPrint.LPrinter(path, li, PrinterName, 1);
            LabelPrint.LPrinter lp2 = new LabelPrint.LPrinter(path2, lI2, PrinterName, 1);

            lp.DoWork();
            lp2.DoWork();
        }
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                // fun_check_dy();

                if (MessageBox.Show(string.Format("确定打印？"), "询问", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                    this.printDialog1.Document = this.printDocument1;
                    DialogResult dr = this.printDialog1.ShowDialog();

                    PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                    Dowork版本2();
                    //LabelPrint.LPrinter lp=new LabelPrint.LPrinter(path, dic, str_打印机, 1)
                    // }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

      

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            try
            {
                s_删除 = true;
                if (MessageBox.Show("是否确认删除此条记录？", "提示!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = string.Format("delete  仓库物料数量表 where 物料编码 = '{0}' and 仓库号 = '{1}'", r["物料编码"], r["仓库号"]);
                    CZMaster.MasterSQL.ExecuteSQL(sql, strconn);
                    r.Delete();

                }
                s_删除 = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

  
    }
}
