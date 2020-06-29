using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace ERPpurchase
{
    public partial class DW生产计划池 : UserControl
    {
        string strcon = CPublic.Var.strConn;

        DataTable dtM;
        DataTable dt_SaleOrder = new DataTable();
        DataTable dt_SaleCrderCopy;
        DataTable dt_totalcount;
        DataTable IncompleteWorkOrder = new DataTable();
        //DataTable IncompleteWorkOrdercopy;
        DataTable saleDisplay;
        DataTable dt_bom;
        bool bl_calculate = false;
        string cfgfilepath = "";
        public DW生产计划池()
        {
            InitializeComponent();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {


                if (bl_calculate) throw new Exception("正在计算中..");
                if (bar_日期.EditValue == null || bar_日期.EditValue.ToString() == "" || barEditItem1.EditValue == null || barEditItem1.EditValue.ToString() == "")
                    throw new Exception("时间为必选项");
                saleDisplay = new DataTable();
                DateTime t1 = Convert.ToDateTime(bar_日期.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(barEditItem1.EditValue).Date.AddDays(1);

                Thread th = new Thread(() =>
                {
                    calculate(t1, t2);
                });
                th.IsBackground = true;

                th.Start();
                bl_calculate = true;
            }
            catch (Exception ex)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label2.Text = "错误原因:" + ex.Message;

                }));
                bl_calculate = false;
                MessageBox.Show("计算出错");
            }
        }
        private void calculate(DateTime t1, DateTime t2)
        {
            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label2.Text = "正在计算中,请稍候...";
                }));
                ERPorg.Corg.result rs = new ERPorg.Corg.result();
                rs = ERPorg.Corg.fun_pool(t1, t2, false);
                dtM = rs.dtM;
                //dtM.Columns.Add("最早发货日期", typeof(DateTime));
                dt_bom = rs.Bom;
                dt_totalcount = rs.TotalCount;
                dt_SaleOrder = rs.salelist_mx;
                //dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));
                //foreach (DataRow saleR in dt_SaleOrder.Rows)
                //{
                //    saleR["应完工日期"] = Convert.ToDateTime(saleR["预计发货日期"]).AddDays(-1);
                //}
                //foreach (DataRow dr in dtM.Rows)
                //{

                //    DataRow[] rr = dt_SaleOrder.Select(string.Format("物料编码='{0}'", dr["物料编码"]), "预计发货日期 asc ");
                //    if (rr.Length > 0)
                //    {
                //        dr["最早发货日期"] = rr[0]["预计发货日期"];
                //        DataTable t = new DataTable();
                //        t = ERPorg.Corg.billofM(t, dr["物料编码"].ToString(), false, dt_bom);
                //        foreach (DataRow rrr in t.Rows)
                //        {
                //            DataRow[] r = dtM.Select(string.Format("物料编码='{0}'", rrr["子项编码"]));
                //            // r.length 只会小于等于一条 
                //            if (r.Length == 0) continue;
                //            else
                //                r[0]["最早发货日期"] = rr[0]["预计发货日期"];

                //        }

                //    }
                //}
                dt_SaleCrderCopy = dt_SaleOrder.Copy();
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dt_SaleCrderCopy.Columns.Add(dc);
                bl_calculate = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    if (rs.str_log != "")
                    {
                        label2.Text = rs.str_log;
                    }
                    else
                    {
                        label2.Text = "---";
                    }
                    DataView dv = new DataView(dtM);
                    dv.RowFilter = "自制='true' ";
                    gc2.DataSource = dv;

                    DataTable search_source = dt_SaleOrder.Copy();
                    foreach (DataRow dr in dtM.Rows)
                    {
                        DataRow[] p = search_source.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (p.Length > 0) continue;
                        DataRow x = search_source.NewRow();
                        x["物料编码"] = dr["物料编码"];
                        x["物料名称"] = dr["物料名称"];
                        x["规格型号"] = dr["规格型号"];
                        x["存货分类"] = dr["存货分类"];
                        search_source.Rows.Add(x);
                    }
                    searchLookUpEdit1.Properties.DataSource = search_source;
                    searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                    searchLookUpEdit1.Properties.ValueMember = "物料编码";
                    gridControl1.DataSource = null;
                    gridControl2.DataSource = null;
                }));
            }
            catch (Exception ex)
            {
                bl_calculate = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    label2.Text = "错误原因:" + ex.Message;

                    bl_calculate = false;
                }));
            }
        }

        /// <summary>
        /// 按单
        /// </summary>
        private void calculate(string str_单号)
        {
            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label2.Text = "正在计算中,请稍候...";
                }));
                ERPorg.Corg.result rs = new ERPorg.Corg.result();
                rs = ERPorg.Corg.fun_pool(str_单号, false);
                dtM = rs.dtM;
                //dtM.Columns.Add("最早发货日期", typeof(DateTime));
                dt_bom = rs.Bom;
                dt_totalcount = rs.TotalCount;
                dt_SaleOrder = rs.salelist_mx;
                //dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));
                //foreach (DataRow saleR in dt_SaleOrder.Rows)
                //{
                //    saleR["应完工日期"] = Convert.ToDateTime(saleR["预计发货日期"]).AddDays(-1);
                //}
                dt_SaleCrderCopy = dt_SaleOrder.Copy();
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dt_SaleCrderCopy.Columns.Add(dc);
                bl_calculate = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    if (rs.str_log != "")
                    {
                        label2.Text = rs.str_log;
                    }
                    else
                    {
                        label2.Text = "---";
                    }
                    DataView dv = new DataView(dtM);
                    dv.RowFilter = "自制='true'";
                    gc2.DataSource = dv;

                    DataTable search_source = dt_SaleOrder.Copy();
                    searchLookUpEdit1.Properties.DataSource = search_source;
                    searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                    searchLookUpEdit1.Properties.ValueMember = "物料编码";
                    gridControl1.DataSource = null;
                    gridControl2.DataSource = null;
                }));
            }
            catch (Exception ex)
            {
                bl_calculate = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    label2.Text = "错误原因:" + ex.Message;

                    bl_calculate = false;
                }));
            }
        }

        private void check()
        {
            DataView v = new DataView(saleDisplay);
            v.RowFilter = "选择=1";
            DataTable t = v.ToTable();
            if (v.ToTable().Rows.Count == 0) throw new Exception("未选择关联任何销售明细");

            //20-1-19 增加限制 判断关联的明细内容有没有变
            foreach(DataRow dr in t.Rows)
            {
                string s = string.Format("select  * from  [V_CalPoolTotal] where 销售订单明细号='{0}'",dr["销售订单明细号"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s,strcon);
                if (temp.Rows.Count == 0) throw new Exception("选中的单据已变更,请确认后重新计算计划池");
                else
                {
                    if(dr["物料编码"].ToString()!=temp.Rows[0]["物料编码"].ToString()  ||Convert.ToDecimal(dr["未完成数量"]) != Convert.ToDecimal(temp.Rows[0]["未完成数量"]))
                    {
                        throw new Exception("选中的单据已变更,请确认后重新计算计划池");
                    }
                }
            }
        }
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //try
            //{
            //    if (bl_calculate) throw new Exception("正在计算中,不可进行此操作");
            //    this.ActiveControl = null;
            //    DataTable t = gridControl1.DataSource as DataTable; //用户选择的销售订单
            //    //dt 取生产制令表 结构  

            //    DataTable dt = CZMaster.MasterSQL.Get_DataTable("select  * from 生产记录生产制令表  where 1=2", strcon);
            //    //step1.遍历 t

            //    DataView dv = new DataView(dtM);
            //    string s = "自制=1 and 物料编码 in (";


            //    DataTable t_relation = new DataTable();
            //    foreach (DataRow dr in t.Rows)
            //    {
            //        //取 dr["物料编码"]及其所有子项得 计算结果 
            //        DataTable dt_x = new DataTable();
            //        dt_x = ERPorg.Corg.billofM(dt_x,dr["物料编码"].ToString(), true,dt_bom);
            //        DataColumn dc = new DataColumn("销售订单号",typeof(string));
            //        dc.DefaultValue = dr["销售订单号"].ToString();
            //        dt_x.Columns.Add(dc);

            //        DataColumn dc1 = new DataColumn("销售订单明细号", typeof(string));
            //        dc1.DefaultValue = dr["销售订单明细号"].ToString();
            //        dt_x.Columns.Add(dc1);
            //        if (t_relation.Columns.Count == 0) t_relation = dt_x.Copy();
            //        else t_relation.Merge(dt_x);

            //        foreach (DataRow cdr in dt_x.Rows) //这边重复也没事
            //        {
            //            s = s + string.Format("'{0}',", cdr["子项编码"]);
            //        }
            //    }
            //    s = s.Substring(0, s.Length - 1) + ")";
            //    dv.RowFilter = s;
            //    DataTable tt= dv.ToTable();



            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //
            try
            {
                this.ActiveControl = null;
                check();

                if (bl_calculate) throw new Exception("正在计算中,不可进行此操作");


                //dt 取生产制令表 结构  
                DataTable dt = CZMaster.MasterSQL.Get_DataTable("select  * from 生产记录生产制令表  where 1=2", strcon); //此dt传入 转制令界面

                DataTable t = new DataTable(); //用户选择的销售订单
                DataView dv_1 = new DataView(saleDisplay);
                dv_1.RowFilter = "选择=1";
                t = dv_1.ToTable();

                //  DataTable tttt = gridControl1;
                //step1.遍历 t
                DataView dv = new DataView(dtM);
                string s = " 自制=1 and 参考数量>0 and 物料编码 in (";

                DataTable t_relation = new DataTable();
                foreach (DataRow dr in t.Rows)
                {
                    //取 dr["物料编码"]及其所有子项得 计算结果 
                    DataTable dt_x = new DataTable();
                    dt_x = ERPorg.Corg.billofM(dt_x, dr["物料编码"].ToString(), true, dt_bom);
                    DataColumn dc = new DataColumn("销售订单号", typeof(string));
                    dc.DefaultValue = dr["销售订单号"].ToString();
                    dt_x.Columns.Add(dc);

                    DataColumn dc1 = new DataColumn("销售订单明细号", typeof(string));
                    dc1.DefaultValue = dr["销售订单明细号"].ToString();
                    dt_x.Columns.Add(dc1);

                    DataColumn dc2 = new DataColumn("应完工日期", typeof(DateTime));
                    DataRow[] sr = dt_SaleCrderCopy.Select(string.Format("销售订单明细号='{0}'", dr["销售订单明细号"].ToString()));
                    dc2.DefaultValue = sr[0]["应完工日期"];
                    dt_x.Columns.Add(dc2);

                    if (t_relation.Columns.Count == 0) t_relation = dt_x.Copy();
                    else t_relation.Merge(dt_x); //取到 

                    foreach (DataRow cdr in dt_x.Rows) //这边重复也没事
                    {
                        s = s + string.Format("'{0}',", cdr["子项编码"]);
                    }
                }
                t_relation.Columns.Add("备注");
                foreach (DataRow dr in t.Rows)
                {
                    DataRow[] r = t_relation.Select(string.Format("子项编码='{0}'", dr["物料编码"]));
                    foreach (DataRow tr in r)
                    {
                        tr["备注"] = tr["备注"].ToString() +" "+ dr["备注"].ToString();
                    }

                }
                s = s.Substring(0, s.Length - 1) + ")";

                dv.RowFilter = s;//这里筛选所有需要带过去的 产品、半成品,此为需要生成生产制令的清单，t_relation 为销售订单明细  及 物料的对应关系 需要根据这个生产 制令子表的记录
                DataTable tt = dv.ToTable();

                if (tt.Rows.Count == 0)
                    throw new Exception("所选物料没有任何子项需要生产,料均不缺");
                else
                {
                    s = "子项编码 in (";
                    foreach (DataRow vr in tt.Rows)
                    {
                        s = s + string.Format("'{0}',", vr["物料编码"]);
                    }
                    s = s.Substring(0, s.Length - 1) + ")";

                    DataView v_relation = new DataView(t_relation);
                    v_relation.RowFilter = s;
                    t_relation = v_relation.ToTable();
                }
                DataSet ds = new DataSet();
                ds.Tables.Add(dt_totalcount.Copy());//基础信息及库存
                ds.Tables.Add(dt_bom.Copy());
                ds.Tables.Add(t_relation.Copy());//tt中物料与销售订单的对应关系
                ds.Tables.Add(tt.Copy());//根据所有需要生成制令的物料清单
                ds.Tables.Add(t.Copy());//用户选中的 销售订单
                ui计划池转制令_u8 ui = new ui计划池转制令_u8(ds);
                CPublic.UIcontrol.Showpage(ui, "转制令确认");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //try
            //{
            //    if (bl_calculate) throw new Exception("正在计算中,不可进行此操作");
            //    SaveFileDialog saveFileDialog = new SaveFileDialog();
            //    saveFileDialog.Title = "导出Excel";
            //    saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            //    DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            //    if (dialogResult == DialogResult.OK)
            //    {
            //        //  dtM.Columns.Remove("已关联");
            //        ERPorg.Corg.TableToExcel(dtM, saveFileDialog.FileName);
            //        MessageBox.Show("导出成功");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

            Control c = GetFocusedControl();
            if (c != null && c.GetType().Equals(gridControl1.GetType()))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                    DevExpress.XtraGrid.GridControl gc = (c) as DevExpress.XtraGrid.GridControl;

                    gc.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {

                MessageBox.Show("若要导出请先选中要导出的表格(鼠标点一下表格)");
            }
        }
        [DllImport("user32.dll")]
        public static extern int GetFocus();
        ///获取 当前拥有焦点的控件
        private Control GetFocusedControl()
        {
            Control c = null;
            // string focusedControl = null;
            IntPtr handle = (IntPtr)GetFocus();

            if (handle == null)
                this.FindForm().KeyPreview = true;
            else
            {
                c = Control.FromHandle(handle);//这就是
                //focusedControl =
                //c.Parent.TopLevelControl.Name.ToString();
            }

            return c;

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_calculate) throw new Exception("正在计算中,不可进行此操作");
                CPublic.UIcontrol.ClosePage();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看bom明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView ff = ((sender as ToolStripDropDownItem).Owner as ContextMenuStrip).Tag as DevExpress.XtraGrid.Views.Grid.GridView;
            DataRow r = ff.GetDataRow(ff.FocusedRowHandle);

            Decimal dec = 1;
            if (contextMenuStrip1.Tag == gridView2)
            {
                if (r["未完成数量"] != DBNull.Value && r["未完成数量"].ToString() != "")
                {
                    dec = Convert.ToDecimal(r["未完成数量"].ToString());
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
            ERPproduct.UI物料BOM详细数量 frm = new ERPproduct.UI物料BOM详细数量(r["物料编码"].ToString().Trim(), dec);
            CPublic.UIcontrol.AddNewPage(frm, "详细数量");
        }

        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                if (dr == null) return;
                查看料况ToolStripMenuItem.Visible = true;
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
                        //19-8-20  
                        s = string.Format("物料编码 in (");
                        foreach (DataRow xx in dtz.Rows)
                        {
                            s = s + "'" + xx["产品编码"].ToString() + "',";
                        }
                        s = s.Substring(0, s.Length - 1) + ")";
                        DataView dv = new DataView(dt_SaleCrderCopy);
                        dv.RowFilter = s;
                        saleDisplay = dv.ToTable();
                        BeginInvoke(new MethodInvoker(() =>
                               {
                                   gridControl1.DataSource = saleDisplay;
                               }));
                    }
                    else
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl1.DataSource = dt_SaleCrderCopy.Clone();
                        }));

                    }
                    DataTable dt_x = new DataTable();
                    dt_x = ERPorg.Corg.billofM(dt_x, dr["物料编码"].ToString(), true, dt_bom);
                    s = "";
                    if (dt_x.Rows.Count > 0)
                    {
                        s = "where x.物料编码 in (";
                        foreach (DataRow xx in dt_x.Rows)
                        {
                            s = s + "'" + xx["子项编码"].ToString() + "',";
                        }
                        s = s.Substring(0, s.Length - 1) + ")" + "order by x.物料编码";

                        s = string.Format(@"select  x.*,kc.仓库名称,库存总数,未领量,base.物料编码,base.物料名称,base.规格型号,base.存货分类 from (
                                 select  gd.生产工单号,物料编码,生产数量,sum(生产数量)-isnull(SUM(a.已入库数量),0) as 数量,仓库号  from 生产记录生产工单表  gd 
                                 left join (select  生产工单号,SUM(入库数量) as 已入库数量 from  生产记录成品入库单明细表  where 作废=0 group by 生产工单号 )a 
		                        on a.生产工单号=gd.生产工单号 
                               where gd.生效=1 and 完成=0 and gd.关闭=0 and 作废=0 group by 物料编码,生产数量,gd.生产工单号,仓库号)x
                                left join 基础数据物料信息表 base on base.物料编码=x.物料编码 
                                left join 仓库物料数量表  kc on kc.物料编码=base.物料编码 and x.仓库号=kc.仓库号 {0}  ", s);

                        DataTable t = new DataTable();
                        t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                        BeginInvoke(new MethodInvoker(() =>
                             {
                                 gridControl2.DataSource = t;
                             }));

                    }
                });
                th.Start();


                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc2, new Point(e.X, e.Y));
                    gv2.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                    contextMenuStrip1.Tag = gv2;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
                {
                    string s = "自制='true'";
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

        private void button2_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtM);
            dv.RowFilter = "自制='true'";
            gc2.DataSource = dv;
        }

        private void gridView3_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            //if (e.Button == MouseButtons.Right)
            //{
            //    contextMenuStrip1.Show(gridControl2, new Point(e.X, e.Y));
            //    gridView3.CloseEditor();
            //    contextMenuStrip1.Tag = gridView3;

            //}
        }

        private void gv2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

        }

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            查看料况ToolStripMenuItem.Visible = false;
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                gridView2.CloseEditor();
                contextMenuStrip1.Tag = gridView2;

            }
        }
        private void DW生产计划池_Load(object sender, EventArgs e)
        {
            DateTime t1 = CPublic.Var.getDatetime().Date.AddMonths(-18);
            DateTime t2 = t1.AddYears(2);

            bar_日期.EditValue = t1;
            barEditItem1.EditValue = t2;


            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(splitContainer1, this.Name, cfgfilepath);
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (barEditItem3.EditValue == null || barEditItem3.EditValue.ToString() == "")
                {
                    throw new Exception("订单号未填写");
                }

                if (bl_calculate) throw new Exception("正在计算中..");

                saleDisplay = new DataTable();

                Thread th = new Thread(() =>
                {
                    calculate(barEditItem3.EditValue.ToString());
                });
                th.IsBackground = true;

                th.Start();
                bl_calculate = true;
            }
            catch (Exception ex)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label2.Text = "错误原因:" + ex.Message;

                }));
                bl_calculate = false;
                MessageBox.Show("计算出错");
            }
        }

        private void 查看料况ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
            DataTable t = dtM.Clone();
            t.ImportRow(dr);
            t.Columns["参考数量"].ColumnName = "数量";

            ERPproduct.ui制令料况查询 ui = new ERPproduct.ui制令料况查询(t.Rows[0]);

            CPublic.UIcontrol.Showpage(ui, "料况查询");

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_calculate) throw new Exception("正在计算中,不可进行此操作");

                //DevExpress.XtraGrid.GridControl gg = new DevExpress.XtraGrid.GridControl();
                //DevExpress.XtraGrid.Views.Grid.GridView view = new DevExpress.XtraGrid.Views.Grid.GridView();
                //gg.Name = "gg";
                //gg.MainView = view;
               
                //view.GridControl = gg;
                //view.Name = "view";
                //gg.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { view });
                //gg.DataSource = dt_SaleOrder;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DataView v = new DataView(dt_SaleOrder);
                    v.Sort = "生效日期,销售订单明细号 asc";

                    //view.ExportToXlsx(saveFileDialog.FileName);
                    DataTable t = v.ToTable();
                    ERPorg.Corg.TableToExcel(t, saveFileDialog.FileName);

                    MessageBox.Show("导出成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gridView2.GetRow(e.RowHandle) == null)
                {
                    return;
                }

                if (e.Column.FieldName == "生效日期" && Convert.ToInt32(gridView2.GetRowCellValue(e.RowHandle, "已关联")) == 1)
                {

                    e.Appearance.BackColor = Color.Yellow;

                }

            }
            catch
            { }

        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(gridView2.GetFocusedRowCellValue(gridView2.FocusedColumn).ToString());

            }
            catch  
            {

            }
        }
    }
}
