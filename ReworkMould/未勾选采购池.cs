using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Threading;
using System.Reflection;

namespace ReworkMould
{
    public partial class 未勾选采购池 : UserControl
    {
        #region 变量
        string strcon = CPublic.Var.strConn;
        /// <summary>
        /// 延续之前计划池的 dtm就不改了
        /// </summary>
        DataTable dtM;
        DataTable dt_主;
        DataTable dt_未转;
        DataTable dt_SaleOrder = new DataTable();
        DataTable dt_SaleCrderCopy;
        DataTable dt_totalcount;

        DataTable IncompletePO;
        DataTable dt_bom = new DataTable();
        DataTable saleDisplay;


        bool bl_calculate = false;
        string str_log = "";
        string cfgfilepath = "";
        #endregion



        public 未勾选采购池()
        {
            InitializeComponent();
        }

        

        private void 未勾选采购池_Load(object sender, EventArgs e)
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
                string sql_主计划 = @"select mx.*,vp.在制量,vp.在途量,vp.库存总数,vp.未领量,vp.受订量,vp.停用 from 主计划子表 mx 
                                                    left join[V_pooltotal] vp on  mx.物料编码 = vp.物料编码  where 转单未完成数量 > 0 ";
                dt_主 = CZMaster.MasterSQL.Get_DataTable(sql_主计划, strcon);

                string sql_未转主计划 = @"select va.客户,va.销售订单明细号,va.销售订单号,va.目标客户,va.客户名称,va.生效日期,va.下单日期,va.物料编码,
                                      va.物料名称,va.规格型号,va.未完成数量,va.销售数量,va.库存总数,va.未领量,va.在制量,va.备注,va.在途量,
                                      va.预计发货日期,va.存货分类,va.表头备注,va.已转数量,(va.未完成数量-va.已转数量) as 数量,vp.受订量,vp.停用 from  V_主计划池 va 
                                      left join[V_pooltotal] vp on  va.物料编码 = vp.物料编码
                                      where (va.未完成数量-va.已转数量)>0 ";
                dt_未转 = CZMaster.MasterSQL.Get_DataTable(sql_未转主计划, strcon);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
        }

        private void calculate()
        {
            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label6.Text = "正在计算中,请稍候...";
                }));
                if(dt_主.Rows.Count > 0)
                {
                    ERPorg.Corg.result rs = new ERPorg.Corg.result();
                    rs = ERPorg.Corg.fun_pool(dt_主, true);
                    dt_totalcount = rs.TotalCount;
                    rs = ERPorg.Corg.fun_pool(dt_未转, true, dt_totalcount);
                    dtM = rs.dtM;
                    DataColumn dc = new DataColumn("选择", typeof(bool));
                    dc.DefaultValue = false;
                    dtM.Columns.Add(dc);
                    //dtM.Columns.Add("最早发货日期", typeof(DateTime));
                    dt_bom = rs.Bom;
                    dt_totalcount = rs.TotalCount;
                    dt_SaleOrder = rs.salelist_mx;
                    IncompletePO = rs.Polist_mx;
                    str_log = rs.str_log;
                    dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));

                    foreach (DataRow saleR in dt_SaleOrder.Rows)
                    {
                        saleR["应完工日期"] = Convert.ToDateTime(saleR["预计发货日期"]).AddDays(-1);
                    }

                    dt_SaleCrderCopy = dt_SaleOrder.Copy();

                    bl_calculate = false;
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        if (rs.str_log != "")
                        {
                            label6.Text = rs.str_log;
                        }
                        else
                        {
                            label6.Text = "---";
                        }
                        DataView dv = new DataView(dtM);
                        dv.RowFilter = "可购=1 or 委外=1";
                        gc2.DataSource = dv;
                        DataTable search_source = dt_SaleOrder.Copy();
                        searchLookUpEdit1.Properties.DataSource = search_source;
                        searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                        searchLookUpEdit1.Properties.ValueMember = "物料编码";
                        gridControl1.DataSource = null;
                        gridControl2.DataSource = null;
                    }));
                }
                else
                {
                    ERPorg.Corg.result rs = new ERPorg.Corg.result();
                    rs = ERPorg.Corg.fun_pool(dt_未转, true);
                    dt_totalcount = rs.TotalCount;
                    //rs = ERPorg.Corg.fun_pool(dt_未转, true, dt_totalcount);
                    dtM = rs.dtM;
                    DataColumn dc = new DataColumn("选择", typeof(bool));
                    dc.DefaultValue = false;
                    dtM.Columns.Add(dc);
                    //dtM.Columns.Add("最早发货日期", typeof(DateTime));
                    dt_bom = rs.Bom;
                    dt_totalcount = rs.TotalCount;
                    dt_SaleOrder = rs.salelist_mx;
                    IncompletePO = rs.Polist_mx;
                    str_log = rs.str_log;
                    dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));

                    foreach (DataRow saleR in dt_SaleOrder.Rows)
                    {
                        saleR["应完工日期"] = Convert.ToDateTime(saleR["预计发货日期"]).AddDays(-1);
                    }

                    dt_SaleCrderCopy = dt_SaleOrder.Copy();

                    bl_calculate = false;
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        if (rs.str_log != "")
                        {
                            label6.Text = rs.str_log;
                        }
                        else
                        {
                            label6.Text = "---";
                        }
                        DataView dv = new DataView(dtM);
                        dv.RowFilter = "可购=1 or 委外=1";
                        gc2.DataSource = dv;
                        DataTable search_source = dt_SaleOrder.Copy();
                        searchLookUpEdit1.Properties.DataSource = search_source;
                        searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                        searchLookUpEdit1.Properties.ValueMember = "物料编码";
                        gridControl1.DataSource = null;
                        gridControl2.DataSource = null;
                    }));
                }
                
            }
            catch (Exception ex)
            {
                bl_calculate = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    label6.Text = "错误原因:" + ex.Message;
                    bl_calculate = false;
                }));
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                object[] drr = new object[2];
                //drr[0] = drM["关联单号"].ToString();
                drr[0] = dt_cs;
                drr[1] = s;
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

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
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

        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
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
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl1.DataSource = dv;
                        }));
                    }
                    else
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControl1.DataSource = dt_SaleCrderCopy.Clone();
                        }));

                    }
                    s = string.Format("物料编码='{0}'", dr["物料编码"].ToString());

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

        private void 查看BOM明细ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_calculate) throw new Exception("正在计算中..");

                saleDisplay = new DataTable();

                Thread th = new Thread(() =>
                {
                    calculate();

                });
                th.IsBackground = true;
                th.Start();
                bl_calculate = true;
            }
            catch (Exception ex)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label6.Text = "错误原因:" + ex.Message;

                }));
                bl_calculate = false;
                MessageBox.Show("计算出错");
            }
        }
    }
}
