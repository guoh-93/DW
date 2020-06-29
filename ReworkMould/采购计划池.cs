using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Reflection;

namespace ReworkMould
{
    public partial class 采购计划池 : UserControl
    {
        #region 变量
        string strcon = CPublic.Var.strConn;
        /// <summary>
        /// 延续之前计划池的 dtm就不改了
        /// </summary>
        DataTable dtM;
        DataTable dt_主;
        DataTable dt_SaleOrder = new DataTable();
        DataTable dt_SaleCrderCopy;
        DataTable dt_totalcount;

        DataTable IncompletePO;
        DataTable dt_bom = new DataTable();
        DataTable saleDisplay;
        DataTable dt_zjh;

        bool bl_calculate = false;
        string str_log = "";
        string cfgfilepath = "";
        #endregion
        public 采购计划池()
        {
            InitializeComponent();
        }

       

        private void 采购计划池_Load(object sender, EventArgs e)
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
                x.UserLayout(splitContainer1, this.Name, cfgfilepath);
                DateTime t = CPublic.Var.getDatetime();
                fun_load();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            string sql_主计划 = @"select * from V_CalPoolTotal";
            dt_主 = CZMaster.MasterSQL.Get_DataTable(sql_主计划, strcon);

            IncompletePO = CZMaster.MasterSQL.Get_DataTable("select * from 主计划计划生成单 where 关闭 = 0", strcon);

            string s = @"select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.委外 as 子项委外,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
                ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
               left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
               left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码 ";
            dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
 

            DataTable dt_SaleOrder_mx = dt_主;

            dt_SaleOrder = dt_SaleOrder_mx;
            

            dt_SaleCrderCopy = dt_SaleOrder.Copy();
            DataColumn dd = new DataColumn("选择", typeof(bool));
            dd.DefaultValue = false;
            dt_SaleCrderCopy.Columns.Add(dd);
            bl_calculate = false;
            searchLookUpEdit1.Properties.DataSource = dt_SaleOrder;
            searchLookUpEdit1.Properties.DisplayMember = "物料编码";
            searchLookUpEdit1.Properties.ValueMember = "物料编码";
            gridControl1.DataSource = null;
            gridControl2.DataSource = null;
        }

        //private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //        string sql_zjh = "select top 1 主计划单号 from 主计划主表 order by 制单日期 desc";
        //        dt_zjh = CZMaster.MasterSQL.Get_DataTable(sql_zjh, strcon);
        //        string sql = string.Format("select count(*)条数 from 采购计划明细表 where 主计划单号  ='{0}'  ", dt_zjh.Rows[0]["主计划单号"]);
        //        DataTable dt1111 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
        //        if (Convert.ToInt32(dt1111.Rows[0]["条数"]) > 0)
        //        {
        //            throw new Exception("提示：最后一张主计划单已有计算记录，请勿重复计算");
        //        }

        //        if (bl_calculate) throw new Exception("正在计算中..");
                
        //        saleDisplay = new DataTable();
                
        //        Thread th = new Thread(() =>
        //        {
        //            calculate();
        //            fun_save();
        //        });
        //        th.IsBackground = true;
        //        th.Start();
        //        bl_calculate = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        bl_calculate = false;
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void fun_save()
        //{
        //    DateTime t = CPublic.Var.getDatetime();
        //    try
        //    {
        //        string sql = "select* from  采购计划主表 where 1 <> 1";
        //        DataTable dt_采购计划主 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
        //        sql = "select* from  采购计划明细表 where 1 <> 1";
        //        DataTable dt_采购计划明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
        //        string s_单号 = "";
        //        int i = 1;
        //        if (dtM.Rows.Count > 0)
        //        {
        //            s_单号 = string.Format("PP{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PP", t.Year, t.Month, t.Day));
        //            DataRow dr_cgz = dt_采购计划主.NewRow();
        //            dt_采购计划主.Rows.Add(dr_cgz);
        //            dr_cgz["GUID"] = System.Guid.NewGuid();
        //            dr_cgz["采购计划单号"] = s_单号;
        //            dr_cgz["生效人"] = CPublic.Var.localUserName;
        //            dr_cgz["生效人ID"] = CPublic.Var.LocalUserID;
        //            dr_cgz["生效日期"] = t;
                    
        //            foreach (DataRow dr_mx in dtM.Rows)
        //            {
        //                if (Convert.ToBoolean(dr_mx["可购"]) == true || Convert.ToBoolean(dr_mx["委外"]) == true)
        //                {
        //                    DataRow dr_cgmx = dt_采购计划明细.NewRow();
        //                    dt_采购计划明细.Rows.Add(dr_cgmx);
        //                    dr_cgmx["采购计划单号"] = s_单号;
        //                    dr_cgmx["采购计划明细号"] = dr_cgmx["采购计划单号"] + "-" + i.ToString("0000");
        //                    dr_cgmx["主计划单号"] = dt_zjh.Rows[0]["主计划单号"];
        //                    dr_cgmx["POS"] = i++;
        //                    dr_cgmx["物料编码"] = dr_mx["物料编码"];
        //                    dr_cgmx["存货分类"] = dr_mx["存货分类"];
        //                    dr_cgmx["物料名称"] = dr_mx["物料名称"];
        //                    dr_cgmx["规格型号"] = dr_mx["规格型号"];
        //                    dr_cgmx["参考数量"] = Convert.ToDecimal(dr_mx["参考数量"]);
        //                    dr_cgmx["参考数量(含安全库存)"] = Convert.ToDecimal(dr_mx["参考数量(含安全库存)"]);
        //                    dr_cgmx["最早发货日期"] = dr_mx["最早发货日期"];
        //                    dr_cgmx["仓库号"] = dr_mx["仓库号"];
        //                    dr_cgmx["仓库名称"] = dr_mx["仓库名称"];
        //                    dr_cgmx["已转数量"] = 0;
        //                    dr_cgmx["未转数量"] = Convert.ToDecimal(dr_mx["参考数量"]);
        //                    dr_cgmx["采购周期"] = dr_mx["采购周期"];
        //                    dr_cgmx["最小包装"] = Convert.ToDecimal(dr_mx["最小包装"]);
        //                    dr_cgmx["需求数量"] = Convert.ToDecimal(dr_mx["需求数量"]);
        //                    dr_cgmx["订单用量"] = Convert.ToDecimal(dr_mx["订单用量"]);
        //                    if (dr_mx["最早预计开工日期"].ToString() !="")
        //                    {
        //                        dr_cgmx["最早预计开工日期"] = dr_mx["最早预计开工日期"];
        //                    }
        //                }


        //            }

        //        }
        //        else
        //        {
        //            throw new Exception("没有可保存的数据");
        //        }




        //        string sql_采购计划主 = "select * from  采购计划主表 where 1<>1";
        //        string sql_采购计划明细 = "select * from  采购计划明细表 where 1<>1";
        //        SqlConnection conn = new SqlConnection(strcon);
        //        conn.Open();
        //        SqlTransaction ts = conn.BeginTransaction("采购计划保存");
        //        try
        //        {
        //            SqlCommand cmm = new SqlCommand(sql_采购计划主, conn, ts);
        //            SqlDataAdapter da = new SqlDataAdapter(cmm);
        //            new SqlCommandBuilder(da);
        //            da.Update(dt_采购计划主);

        //            cmm = new SqlCommand(sql_采购计划明细, conn, ts);
        //            da = new SqlDataAdapter(cmm);
        //            new SqlCommandBuilder(da);
        //            da.Update(dt_采购计划明细);

        //            ts.Commit();
                    

        //        }
        //        catch (Exception ex)
        //        {
        //            ts.Rollback();
        //            throw ex;
        //        }



        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void calculate()
        //{
        //    try
        //    {
        //        BeginInvoke(new MethodInvoker(() =>
        //        {
        //            label6.Text = "正在计算中,请稍候...";
        //        }));
        //        string sql_主计划 = @"select mx.*,vp.在制量,vp.在途量,vp.库存总数,vp.未领量,vp.受订量 from 主计划子表 mx 
        //                                            left join[V_pooltotal] vp on  mx.物料编码 = vp.物料编码  where 转单未完成数量 > 0  and 撤销 = 0 and 关闭 = 0 ";
        //        dt_主 = CZMaster.MasterSQL.Get_DataTable(sql_主计划, strcon);
        //        ERPorg.Corg.result rs = new ERPorg.Corg.result();
        //        rs = ERPorg.Corg.fun_pool(dt_主, true);
        //        dtM = rs.dtM;
        //        DataColumn dc = new DataColumn("选择", typeof(bool));
        //        dc.DefaultValue = false;
        //        dtM.Columns.Add(dc);
        //        //dtM.Columns.Add("最早发货日期", typeof(DateTime));
        //        dt_bom = rs.Bom;
        //        dt_totalcount = rs.TotalCount;
        //        dt_SaleOrder = rs.salelist_mx;
        //        IncompletePO = rs.Polist_mx;
        //        str_log = rs.str_log;
        //        dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));

        //        foreach (DataRow saleR in dt_SaleOrder.Rows)
        //        {
        //            saleR["应完工日期"] = Convert.ToDateTime(saleR["预计发货日期"]).AddDays(-1);
        //        }

        //        dt_SaleCrderCopy = dt_SaleOrder.Copy();

        //        bl_calculate = false;
        //        BeginInvoke(new MethodInvoker(() =>
        //        {
        //            if (rs.str_log != "")
        //            {
        //                label6.Text = rs.str_log;
        //            }
        //            else
        //            {
        //                label6.Text = "---";
        //            }
        //            DataView dv = new DataView(dtM);
        //            dv.RowFilter = "停用 = 0 and (可购=1 or 委外=1) ";
        //            gc2.DataSource = dv;
        //            DataTable search_source = dt_SaleOrder.Copy();
        //            searchLookUpEdit1.Properties.DataSource = search_source;
        //            searchLookUpEdit1.Properties.DisplayMember = "物料编码";
        //            searchLookUpEdit1.Properties.ValueMember = "物料编码";
        //            gridControl1.DataSource = null;
        //            gridControl2.DataSource = null;
        //        }));
        //    }
        //    catch (Exception ex)
        //    { 
        //        bl_calculate = false;
        //        BeginInvoke(new MethodInvoker(() =>
        //        {
        //            label6.Text = "错误原因:" + ex.Message;
        //            bl_calculate = false;
        //        }));
        //    }
        //}

        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                if(dr == null) return;
                Thread th = new Thread(() =>
                {
                    DataTable dtz = new DataTable();

                    // dtz.Columns.Add("产品编码");

                    string s = string.Format(@"with parent_bom(产品编码,子项编码,仓库号,仓库名称,bom_level ) as
                   (select  产品编码,子项编码,仓库号,仓库名称,1 as level from 基础数据物料BOM表 
                    where 子项编码='{0}'
                      union all 
                   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,b.bom_level+1  from 基础数据物料BOM表 a
                   inner join parent_bom b on a.子项编码=b.产品编码  )
                      select  * from parent_bom ", dr["物料编码"].ToString());
                    dtz = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                    // dtz = ERPorg.Corg.fun_GetFather(dtz, dr["物料编码"].ToString(), 0, true);
                    //加入他自身
                    DataRow rrr = dtz.NewRow();
                    rrr["产品编码"] = dr["物料编码"].ToString();
                    dtz.Rows.Add(rrr);
                    if (dtz.Rows.Count > 0)
                    {
                        s = string.Format("物料编码 in (");
                        foreach (DataRow xx in dtz.Rows)
                        {
                            s = s + "'" + xx["产品编码"].ToString() + "',";
                        }
                        s = s.Substring(0, s.Length - 1) + ")";
                        DataView dv = new DataView(dt_SaleCrderCopy);
                        dv.RowFilter = s;

                        //DataView dv_z = new DataView(IncompletePO);
                        //dv_z.RowFilter = s;

                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl1.DataSource = dv;
                           // gridControl2.DataSource = dv_z;
                        }));
                    }
                    else
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl1.DataSource = dt_SaleCrderCopy.Clone();
                           // gridControl2.DataSource = IncompletePO.Clone();

                        }));

                    }
                   // s = string.Format("物料编码='{0}'", dr["物料编码"].ToString());

                    DataView dv_z = new DataView(IncompletePO);
                    dv_z.RowFilter = s;

                    BeginInvoke(new MethodInvoker(() =>
                    {
                        gridControl2.DataSource = dv_z;
                    }));

                });
                th.Start();
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc2, new Point(e.X, e.Y));
                    gv2.CloseEditor();
                    contextMenuStrip1.Tag = gv2;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv2.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.ActiveControl = null;
                DataTable dt_cs = check();
                string s = dt_cs.Rows[0]["供应商编号"].ToString();
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPpurchase.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPpurchase.frm采购单明细", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                object[] drr = new object[3];
                //drr[0] = drM["关联单号"].ToString();
                drr[0] = dt_cs;
                drr[1] = s;
                drr[2] = true;
                // Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

                CPublic.UIcontrol.Showpage(ui, "采购订单录入");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private DataTable check()
        {
            if (dtM == null) throw new Exception("未有任何记录");

            DataView dv = new DataView(dtM);
            dv.RowFilter = "选择=1";
            if (dv.Count == 0) throw new Exception("未选择任何记录");
            DataTable t = dv.ToTable();
            return t;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
                {
                    string s = "可购='true'";
                    DataTable ListM = new DataTable();
                    ListM = ERPorg.Corg.billofM(ListM, searchLookUpEdit1.EditValue.ToString(), true, dt_bom);
                    if (ListM.Rows.Count > 0)
                    {
                        DataView dv = new DataView(dtM);
                        s = s + " and 物料编码 in (";
                        foreach (DataRow dr in ListM.Rows)
                        {
                            s = s + string.Format("'{0}',", dr["子项编码"]);
                        }
                        s = s.Substring(0, s.Length - 1) + ")";
                        dv.RowFilter = s;
                        gc2.DataSource = dv;
                    }
                    else
                    {
                        MessageBox.Show("无数据");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtM);
            dv.RowFilter = "可购='true'";
            gc2.DataSource = dv;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string s = @" select   mx.物料编码, sum(采购数量) as 已采未审   from    采购记录采购单明细表 mx
                            left join 采购记录采购单主表 zb on zb.采购单号=mx.采购单号
                            where   mx.生效 = 0  AND  mx.作废 = 0 and zb.作废=0 group by 物料编码";
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                foreach (DataRow dr in t.Rows)
                {
                    DataRow[] r = dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (r.Length > 0)
                    {
                        r[0]["已采未审"] = dr["已采未审"];

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataView dv = new DataView(dt_totalcount);
                dv.RowFilter = "需求数量>0";
                DataTable dt = dv.ToTable();
                dt.Columns.Remove("总数");
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {

                    ERPorg.Corg.TableToExcel(dt, saveFileDialog.FileName);
                    MessageBox.Show("导出成功");
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
            }
            
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {

                    ERPorg.Corg.TableToExcel(dtM, saveFileDialog.FileName);

                    MessageBox.Show("导出成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看BOM信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView ff = ((sender as ToolStripDropDownItem).Owner as ContextMenuStrip).Tag as DevExpress.XtraGrid.Views.Grid.GridView;
                DataRow r = ff.GetDataRow(ff.FocusedRowHandle);

                Decimal dec = 1;
                if (contextMenuStrip1.Tag == gridView2)
                {
                    if (r["销售数量"] != DBNull.Value && r["销售数量"].ToString() != "")
                    {
                        dec = Convert.ToDecimal(r["数量"].ToString());
                    }
                    else
                    {
                        dec = 1;
                    }
                }
                else
                {
                    if (r["参考数量"] != DBNull.Value && r["参考数量"].ToString() != "")
                    {
                        dec = Convert.ToDecimal(r["参考数量"].ToString());
                    }
                    else
                    {
                        dec = 1;
                    }
                }
                //ERPproduct.UI物料BOM详细数量 frm = new ERPproduct.UI物料BOM详细数量(r["物料编码"].ToString().Trim(), dec);
                //CPublic.UIcontrol.AddNewPage(frm, "详细数量");
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPproduct.dll")));  
                Type outerForm = outerAsm.GetType("ERPproduct.UI物料BOM详细数量", false); 
                object[] drr = new object[2];
                drr[0] = r["物料编码"].ToString().Trim();
                drr[1] = dec;
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

                CPublic.UIcontrol.Showpage(ui, "查看BOM明细");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看物料详情ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView ff = ((sender as ToolStripDropDownItem).Owner as ContextMenuStrip).Tag as DevExpress.XtraGrid.Views.Grid.GridView;
                DataRow r = ff.GetDataRow(ff.FocusedRowHandle);
                if (r["仓库号"].ToString() == "") throw new Exception("选中记录没有仓库信息");
                Decimal dec = 1;
                if (contextMenuStrip1.Tag == gridView2)
                {
                    if (r["销售数量"] != DBNull.Value && r["销售数量"].ToString() != "")
                    {
                        dec = Convert.ToDecimal(r["数量"].ToString());
                    }
                    else
                    {
                        dec = 1;
                    }
                }
                else
                {
                    if (r["参考数量"] != DBNull.Value && r["参考数量"].ToString() != "")
                    {
                        dec = Convert.ToDecimal(r["参考数量"].ToString());
                    }
                    else
                    {
                        dec = 1;
                    }
                }
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "ERPStock.dll"));//dr["dll全路径"] = "动态载入dll.dll"
                Type outerForm = outerAsm.GetType("ERPStock.frm仓库物料数量明细", false);//动态载入dll.UI动态载入窗体
                object[] dr = new object[2];
                dr[0] = r["物料编码"];
                dr[1] = r["仓库号"]; //仓库号没有
                UserControl ui = Activator.CreateInstance(outerForm, dr) as UserControl;
                CPublic.UIcontrol.Showpage(ui, "物料详情");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 历史采购记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, "ERPStock.dll"));//dr["dll全路径"] = "动态载入dll.dll"
                Type outerForm = outerAsm.GetType("ERPStock.fm空窗体", false);//动态载入dll.UI动态载入窗体
                Form fm = (Form)Activator.CreateInstance(outerForm);

                Type outerui = outerAsm.GetType("ERPStock.UI过往采购单价查询", false);//动态载入dll.UI动态载入窗体
                object[] r = new object[1];
                r[0] = dr["物料编码"].ToString();
                UserControl ui = Activator.CreateInstance(outerui, r) as UserControl;

                fm.Controls.Add(ui);
                ui.Dock = DockStyle.Fill;
                fm.Text = "历史采购价";
                fm.Size = new System.Drawing.Size(1200, 550); ;
                fm.ShowDialog();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        //private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    DateTime t = CPublic.Var.getDatetime();
        //    try
        //    {
        //        string sql = "select* from  采购计划主表 where 1 <> 1";
        //        DataTable dt_采购计划主 = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
        //        sql = "select* from  采购计划明细表 where 1 <> 1";
        //        DataTable dt_采购计划明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);                            
        //        string s_单号 = "";
        //        int i = 1;
        //        if (dtM.Rows.Count > 0)
        //        {
        //            s_单号 = string.Format("PP{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PP", t.Year, t.Month, t.Day));      
        //            DataRow dr_cgz = dt_采购计划主.NewRow();
        //            dt_采购计划主.Rows.Add(dr_cgz);
        //            dr_cgz["GUID"] = System.Guid.NewGuid();
        //            dr_cgz["采购计划单号"] = s_单号;
        //            dr_cgz["生效人"] = CPublic.Var.localUserName;
        //            dr_cgz["生效人ID"] = CPublic.Var.LocalUserID;
        //            dr_cgz["生效日期"] = t;
        //            foreach (DataRow dr_mx in dtM.Rows)
        //            {
        //                if(Convert.ToBoolean(dr_mx["可购"]) == true||Convert.ToBoolean(dr_mx["委外"]) == true)
        //                {
        //                    DataRow dr_cgmx = dt_采购计划明细.NewRow();
        //                    dt_采购计划明细.Rows.Add(dr_cgmx);
        //                    dr_cgmx["采购计划单号"] = s_单号;
        //                    dr_cgmx["采购计划明细号"] = dr_cgmx["采购计划单号"] + "-" + i.ToString("0000");
        //                    dr_cgmx["POS"] = i++;
        //                    dr_cgmx["物料编码"] = dr_mx["物料编码"];
        //                    dr_cgmx["存货分类"] = dr_mx["存货分类"];
        //                    dr_cgmx["物料名称"] = dr_mx["物料名称"];
        //                    dr_cgmx["规格型号"] = dr_mx["规格型号"];
        //                    dr_cgmx["参考数量"] = Convert.ToDecimal(dr_mx["参考数量"]);
        //                    dr_cgmx["参考数量(含安全库存)"] = Convert.ToDecimal(dr_mx["参考数量(含安全库存)"]);
        //                    dr_cgmx["最早发货日期"] = dr_mx["最早发货日期"];
        //                    dr_cgmx["仓库号"] = dr_mx["仓库号"];
        //                    dr_cgmx["仓库名称"] = dr_mx["仓库名称"];
        //                }
                        

        //            }
                    
        //        }
        //        else
        //        {
        //            throw new Exception("没有可保存的数据");
        //        }


        //        string s_删除 = "delete  from  采购计划明细表";
             
        //        CZMaster.MasterSQL.ExecuteSQL(s_删除, strcon);


        //        string sql_采购计划主 = "select * from  采购计划主表 where 1<>1";
        //        string sql_采购计划明细 = "select * from  采购计划明细表 where 1<>1";
        //        SqlConnection conn = new SqlConnection(strcon);
        //        conn.Open();
        //        SqlTransaction ts = conn.BeginTransaction("采购计划保存");
        //        try
        //        {
        //            SqlCommand cmm = new SqlCommand(sql_采购计划主, conn, ts);
        //            SqlDataAdapter da = new SqlDataAdapter(cmm);
        //            new SqlCommandBuilder(da);
        //            da.Update(dt_采购计划主);

        //            cmm = new SqlCommand(sql_采购计划明细, conn, ts);
        //            da = new SqlDataAdapter(cmm);
        //            new SqlCommandBuilder(da);
        //            da.Update(dt_采购计划明细);

        //            ts.Commit();
        //            MessageBox.Show("保存成功");

        //        }
        //        catch (Exception ex)
        //        {
        //            ts.Rollback();
        //            throw ex;
        //        }
                


        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //    }
        //}


        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
             
                string sql = @"   select aa.*,bb.ECN,bb.供应商编号,bb.物料名称,bb.规格型号,bb.存货分类,bb.库存总数,bb.受订量,
                                 bb.未领量,bb.已采未审,bb.在途量,bb.委外在途,bb.采购未送检,bb.已送未检,bb.已检未入,bb.自制,bb.可购,bb.采购员,
                                bb.默认供应商,bb.库存下限,bb.货架描述,bb.采购周期,bb.最小包装,bb.委外,bb.默认仓库号 as 仓库号,bb.仓库名称,bb.供应状态,aa.通知采购数量-isnull(cc.采购数量,0) as 可转数量
                                  from 主计划计划通知单明细 aa
                                 left join V_pooltotal bb on aa.物料编码 = bb.物料编码 
                                 left join (select SUM(采购数量)采购数量,备注9 from 采购记录采购单明细表 where 作废 = 0   group by 备注9 ) cc on cc.备注9=aa.计划通知单明细号
                                where aa.通知采购数量-isnull(cc.采购数量,0)>0 and 生效 = 1 and 关闭 = 0";

                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

               
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dtM.Columns.Add(dc);

                DataColumn dcc = new DataColumn("替代", typeof(bool));
                dcc.DefaultValue = false;
                dtM.Columns.Add(dcc);
                string s = "select  子项编码 from 基础数据物料BOM表  where 组<>'' and 优先级=1 ";
                DataTable t_替代 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                foreach (DataRow r in t_替代.Rows)
                {
                    DataRow[] rr = dtM.Select($"物料编码='{r["子项编码"]}'");
                    if (rr.Length > 0)
                    {
                        rr[0]["替代"] = true;
                    }
                }
                

                gc2.DataSource = dtM;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

  

        private void 保存预计来料日期ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                DateTime tt = CPublic.Var.getDatetime();
                string sql = $@"select * from 主计划计划通知单明细 where 计划通知单明细号 = '{dr["计划通知单明细号"]}'";
                DataTable dt_计划通知单 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                sql = $@"select * from 主计划采购需求单 where 计划单明细号 = '{dr["计划需求明细号"]}'";
                DataTable dt_采购需求 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


                if (dt_计划通知单.Rows[0]["采购备注"].ToString().Trim() !="")
                {
                    throw new Exception(dt_计划通知单.Rows[0]["物料编码"]+"采购备注已填，不可修改");
                }


                修改预计到货日期 frm = new 修改预计到货日期(dr);
                frm.Text = "保存预计来料日期";
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.bl)
                {

                    
                    dt_计划通知单.Rows[0]["采购备注"] = frm.rr["采购备注"];
                    dt_计划通知单.Rows[0]["预计来料日期"] = Convert.ToDateTime(frm.rr["预计来料日期"]);
                    dt_计划通知单.Rows[0]["修改日期"] = tt;


                    if (dt_采购需求.Rows.Count > 0)
                    {
                        dt_采购需求.Rows[0]["采购备注"] = frm.rr["采购备注"];
                        dt_采购需求.Rows[0]["预计来料日期"] = Convert.ToDateTime(frm.rr["预计来料日期"]);
                    }
                   

                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("生效");
                    try
                    {                     
                        string sql3 = "select * from 主计划计划通知单明细 where 1<>1";
                        SqlCommand cmm3 = new SqlCommand(sql3, conn, ts);
                        SqlDataAdapter da3 = new SqlDataAdapter(cmm3);
                        new SqlCommandBuilder(da3);
                        da3.Update(dt_计划通知单);

                        if (dt_采购需求.Rows.Count > 0)
                        {
                            string sql2 = "select * from 主计划采购需求单 where 1<>1";
                            SqlCommand cmm2 = new SqlCommand(sql2, conn, ts);
                            SqlDataAdapter da2 = new SqlDataAdapter(cmm2);
                            new SqlCommandBuilder(da2);
                            da2.Update(dt_采购需求);
                        }
                            



                        ts.Commit();
                        MessageBox.Show("保存成功");
                        sql = $@" select aa.*,bb.ECN,bb.供应商编号,bb.物料名称,bb.规格型号,bb.存货分类,bb.库存总数,bb.受订量,
                                 bb.未领量,bb.已采未审,bb.在途量,bb.委外在途,bb.采购未送检,bb.已送未检,bb.已检未入,bb.自制,bb.可购,bb.采购员,
                                bb.默认供应商,bb.库存下限,bb.货架描述,bb.采购周期,bb.最小包装,bb.委外,bb.默认仓库号 as 仓库号,bb.仓库名称,bb.供应状态
                                  from 主计划计划通知单明细 aa
                                 left join V_pooltotal bb on aa.物料编码 = bb.物料编码 where aa.通知采购数量 - aa.已转采购数量>0
                and 计划通知单明细号 = '{dr["计划通知单明细号"]}'";
                        dt_计划通知单 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                        dr.ItemArray = dt_计划通知单.Rows[0].ItemArray;

                        dr.AcceptChanges();
                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw new Exception(ex.Message);
                    }


                
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gv2.GetRow(e.RowHandle) == null)
                {
                    return;
                }

                if (Convert.ToDecimal(gv2.GetRowCellValue(e.RowHandle, "已采未审")) > 0)
                {
                    e.Appearance.BackColor = Color.Pink;

                }
                if (e.Column.FieldName == "物料编码")
                {
                    DataRow rr = gv2.GetDataRow(e.RowHandle);
                    if (Convert.ToBoolean(rr["替代"]))
                        e.Appearance.BackColor = Color.GreenYellow;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void 暂不采购ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                string sql = $@"select * from 主计划计划通知单明细 where 计划通知单明细号 = '{dr["计划通知单明细号"]}'";
                DataTable dt_计划通知单 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


                sql = $@"select * from 主计划采购需求单 where 计划单明细号 = '{dr["计划需求明细号"]}'";
                DataTable dt_采购需求 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);


                填写暂不采购原因 frm = new 填写暂不采购原因(dr);
                frm.Text = "填写暂不采购原因";
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.bl)
                {
                    dt_计划通知单.Rows[0]["备注1"] = frm.rr["备注1"];
                    dt_采购需求.Rows[0]["备注1"] = frm.rr["备注1"];
                    //dt_计划采购需求.ImportRow(frm.rr); 

                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("生效");
                    try
                    {
                        string sql3 = "select * from 主计划计划通知单明细 where 1<>1";
                        SqlCommand cmm3 = new SqlCommand(sql3, conn, ts);
                        SqlDataAdapter da3 = new SqlDataAdapter(cmm3);
                        new SqlCommandBuilder(da3);
                        da3.Update(dt_计划通知单);

                        string sql2 = "select * from 主计划采购需求单 where 1<>1";
                        SqlCommand cmm2 = new SqlCommand(sql2, conn, ts);
                        SqlDataAdapter da2 = new SqlDataAdapter(cmm2);
                        new SqlCommandBuilder(da2);
                        da2.Update(dt_采购需求);

                        ts.Commit();
                        MessageBox.Show("保存成功");
                        sql = $@" select aa.*,bb.ECN,bb.供应商编号,bb.物料名称,bb.规格型号,bb.存货分类,bb.库存总数,bb.受订量,
                                 bb.未领量,bb.已采未审,bb.在途量,bb.委外在途,bb.采购未送检,bb.已送未检,bb.已检未入,bb.自制,bb.可购,bb.采购员,
                                bb.默认供应商,bb.库存下限,bb.货架描述,bb.采购周期,bb.最小包装,bb.委外,bb.默认仓库号 as 仓库号,bb.仓库名称,bb.供应状态
                                  from 主计划计划通知单明细 aa
                                 left join V_pooltotal bb on aa.物料编码 = bb.物料编码 where aa.通知采购数量 - aa.已转采购数量>0
                and 计划通知单明细号 = '{dr["计划通知单明细号"]}'";
                        dt_计划通知单 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                        dr.ItemArray = dt_计划通知单.Rows[0].ItemArray;

                        dr.AcceptChanges();
                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw new Exception(ex.Message);
                    }
            
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 驳回ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                if (MessageBox.Show(string.Format("是否确定驳回关闭？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DateTime t = CPublic.Var.getDatetime();
                    string sql = $"select * from 主计划计划通知单明细 where 计划通知单明细号 = '{dr["计划通知单明细号"]}'";
                    DataTable dt_1 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);

                    sql = $"select * from 主计划采购需求单 where  计划单明细号  = '{dr["计划需求明细号"]}' ";
                    DataTable dt_2 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt_1.Rows.Count > 0)
                    {
                        sql = $"select * from 采购记录采购单明细表 where 备注9 ='{dr["计划通知单明细号"]}' and 作废 = 0";
                        DataTable dt111 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                        if (dt111.Rows.Count > 0) throw new Exception("该物料有对应的采购记录，不能驳回");
                        dt_1.Rows[0]["关闭"] = true;
                        dt_1.Rows[0]["关闭人ID"] = CPublic.Var.LocalUserID;
                        dt_1.Rows[0]["关闭时间"] = t;
                        if (dt_2.Rows.Count > 0)
                        {
                            dt_2.Rows[0]["生效"] = false;
                            dt_2.Rows[0]["生效时间"] = DBNull.Value;
                            dt_2.Rows[0]["生效人"] = "";
                        }
                        SqlConnection conn = new SqlConnection(strcon);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("关闭");
                        try
                        {




                            string sql3 = "select * from 主计划计划通知单明细 where 1<>1";
                            SqlCommand cmm3 = new SqlCommand(sql3, conn, ts);
                            SqlDataAdapter da3 = new SqlDataAdapter(cmm3);
                            new SqlCommandBuilder(da3);
                            da3.Update(dt_1);


                            string sql2 = "select * from 主计划采购需求单 where 1<>1";
                            SqlCommand cmm2 = new SqlCommand(sql2, conn, ts);
                            SqlDataAdapter da2 = new SqlDataAdapter(cmm2);
                            new SqlCommandBuilder(da2);
                            da2.Update(dt_2);

                            ts.Commit();
                            MessageBox.Show("驳回成功");
                            barLargeButtonItem8_ItemClick(null,null);



                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
