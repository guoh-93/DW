using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static ERPorg.Corg;

namespace ReworkMould
{
    public partial class ui_生产池_1 : UserControl
    {
        #region 变量      
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_SaleOrder = new DataTable();
        DataTable dt_SaleOrder_1 = new DataTable();
        DataTable dt_SaleCrderCopy;
        DataTable dt_totalcount;
        DataTable IncompleteWorkOrder = new DataTable();
        DataTable saleDisplay;
        DataTable dt_bom;
        DataTable dt_计划;
        DataTable dt_计划_copy;
        DataTable dt_采购计划;
        DataTable dt_采购需求;
        bool bl_calculate = false;
        bool bl_保存 = false;
        bool bl_需求单 = false;
        bool bl_选择 = false;
        string cfgfilepath = "";
        string str_log;
        bool bl_跳转 = false;
        bool bl_bc = false;
        DataTable dt_xsmx;
        #endregion



        public ui_生产池_1()
        {
            InitializeComponent();
        }

        public ui_生产池_1(DataTable dt_生产计划, DataTable dt_采购, DataTable dt_Sale, DataTable bom, DataTable dt_total, string str, DataTable dt_m)
        {
            InitializeComponent();
            dtM = dt_生产计划;
            dt_采购计划 = dt_采购;
            dt_SaleOrder = dt_Sale;
            dt_bom = bom;
            dt_totalcount = dt_total;
            str_log = str;
            dt_xsmx = dt_m;
            bl_跳转 = true;
            barEditItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barEditItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barLargeButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private void ui_生产池_1_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime t1 = CPublic.Var.getDatetime().Date.AddMonths(-18);
                DateTime t2 = t1.AddYears(2);

                barEditItem1.EditValue = t1;
                barEditItem2.EditValue = t2;


                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(panel3, this.Name, cfgfilepath);
                string sql = "select * from 主计划计划生成单 where 1<>1";
                dt_计划 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                dt_计划.Columns.Add("需求数量", typeof(decimal));
                dt_计划.Columns.Add("建议计划数量", typeof(decimal));
                if (bl_跳转) 
                {
                     
                    fun_load();
                    barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barLargeButtonItem7.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                }
                else
                {
                    barLargeButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    barLargeButtonItem7.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {

            string s = @"select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.委外 as 子项委外,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
                ,zx.规格型号 as 子项规格,wiptype       from 基础数据物料BOM表 bom 
               left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
               left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码  where 优先级=1 "; //20-4-13 替代料屏蔽
            dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn[] pk_bom = new DataColumn[2];
            pk_bom[0] = dt_bom.Columns["产品编码"];
            pk_bom[1] = dt_bom.Columns["子项编码"];
            dt_bom.PrimaryKey = pk_bom;
            if (!dtM.Columns.Contains("选择"))
            {
                DataColumn dcc = new DataColumn("选择", typeof(bool));
                dcc.DefaultValue = false;
                dtM.Columns.Add(dcc);
            }

            dt_SaleOrder_1 = dt_SaleOrder.Clone();
            DataColumn dc_1 = new DataColumn("参考数量", typeof(decimal));
            dc_1.DefaultValue = 0;
            dt_SaleOrder_1.Columns.Add(dc_1);
            foreach (DataRow dr1 in dt_SaleOrder.Rows)
            {
                string sss = dr1["物料编码"].ToString().Substring(0, 2);
                if (sss == "01")
                {
                    DataRow dr_xm = dt_SaleOrder_1.NewRow();
                    dt_SaleOrder_1.Rows.Add(dr_xm);
                    dr_xm["客户"] = dr1["客户"].ToString();
                    dr_xm["销售订单明细号"] = dr1["销售订单明细号"].ToString();
                    dr_xm["销售订单号"] = dr1["销售订单号"].ToString();
                    dr_xm["目标客户"] = dr1["目标客户"].ToString();
                    dr_xm["客户名称"] = dr1["客户名称"].ToString();
                    dr_xm["生效日期"] = Convert.ToDateTime(dr1["生效日期"].ToString());
                    dr_xm["下单日期"] = Convert.ToDateTime(dr1["下单日期"].ToString());
                    dr_xm["物料编码"] = dr1["物料编码"].ToString();
                    dr_xm["物料名称"] = dr1["物料名称"].ToString();
                    dr_xm["规格型号"] = dr1["规格型号"].ToString();

                    if (dr1["数量"] == DBNull.Value || dr1["数量"].ToString() == "")
                    {
                        dr_xm["数量"] = 0;
                    }
                    else
                    {
                        dr_xm["数量"] = Convert.ToDecimal(dr1["数量"].ToString());
                    }
                    if (dr1["未完成数量"] == DBNull.Value || dr1["未完成数量"].ToString() == "")
                    {
                        dr_xm["未完成数量"] = 0;
                    }
                    else
                    {
                        dr_xm["未完成数量"] = Convert.ToDecimal(dr1["未完成数量"].ToString());
                    }
                    if (dr1["销售数量"] == DBNull.Value || dr1["销售数量"].ToString() == "")
                    {
                        dr_xm["销售数量"] = 0;
                    }
                    else
                    {
                        dr_xm["销售数量"] = Convert.ToDecimal(dr1["销售数量"].ToString());
                    }
                    //dr_xm["数量"] = Convert.ToDecimal(dr1["数量"].ToString());
                    //dr_xm["未完成数量"] = Convert.ToDecimal(dr1["未完成数量"].ToString());
                    //dr_xm["销售数量"] = Convert.ToDecimal(dr1["销售数量"].ToString());
                    if (dr1["库存总数"] == DBNull.Value || dr1["库存总数"].ToString() == "")
                    {
                        dr_xm["库存总数"] = 0;
                    }
                    else
                    {
                        dr_xm["库存总数"] = Convert.ToDecimal(dr1["库存总数"].ToString());
                    }
                    if (dr1["未领量"] == DBNull.Value || dr1["未领量"].ToString() == "")
                    {
                        dr_xm["未领量"] = 0;
                    }
                    else
                    {
                        dr_xm["未领量"] = Convert.ToDecimal(dr1["未领量"].ToString());
                    }
                    if (dr1["在制量"] == DBNull.Value || dr1["在制量"].ToString() == "")
                    {
                        dr_xm["在制量"] = 0;
                    }
                    else
                    {
                        dr_xm["在制量"] = Convert.ToDecimal(dr1["在制量"].ToString());
                    }
                    if (dr1["在途量"] == DBNull.Value || dr1["在途量"].ToString() == "")
                    {
                        dr_xm["在途量"] = 0;
                    }
                    else
                    {
                        dr_xm["在途量"] = Convert.ToDecimal(dr1["在途量"].ToString());
                    }

                    //dr_xm["未领量"] = Convert.ToDecimal(dr1["未领量"].ToString());
                    //dr_xm["在制量"] = Convert.ToDecimal(dr1["在制量"].ToString());
                    dr_xm["备注"] = dr1["备注"].ToString();
                    //  dr_xm["在途量"] = Convert.ToDecimal(dr1["在途量"].ToString());
                    dr_xm["预计发货日期"] = Convert.ToDateTime(dr1["预计发货日期"].ToString());
                    dr_xm["存货分类"] = dr1["存货分类"].ToString();
                    dr_xm["应完工日期"] = Convert.ToDateTime(dr1["应完工日期"].ToString());
                    dr_xm["表头备注"] = dr1["表头备注"].ToString();
                    //dr_xm["已关联"] = dr1["已关联"];
                    DataRow[] dr = dt_采购计划.Select(string.Format("物料编码 = '{0}'", dr1["物料编码"].ToString()));
                    if (dr.Length > 0)
                    {
                        dr_xm["参考数量"] = Convert.ToDecimal(dr[0]["参考数量"]);
                    }
                }
            }
            dt_SaleCrderCopy = dt_SaleOrder.Copy();

            bl_calculate = false;
            BeginInvoke(new MethodInvoker(() =>
            {

                DataView dv = new DataView(dtM);
                dv.RowFilter = "自制='true' ";

                gc2.DataSource = dv;


                DataTable search_source = dt_SaleOrder.Copy();
                foreach (DataRow dr in dtM.Rows)
                {
                    DataRow[] p = search_source.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (p.Length > 0)
                    {
                        continue;
                    }

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

                searchLookUpEdit3.Properties.DataSource = search_source;
                searchLookUpEdit3.Properties.DisplayMember = "物料编码";
                searchLookUpEdit3.Properties.ValueMember = "物料编码";

                MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                DataTable dt_bom_1 = RBQ.SelectGroupByInto("", dt_bom, "子项编码,子项名称,子项规格", "", "子项编码");
                searchLookUpEdit2.Properties.DataSource = dt_bom_1;
                searchLookUpEdit2.Properties.DisplayMember = "子项编码";
                searchLookUpEdit2.Properties.ValueMember = "子项编码";

                gridControl1.DataSource = null;
                gridControl2.DataSource = null;
            }));
        }

        //按时间段计算
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_calculate)
                {
                    throw new Exception("正在计算中..");
                }

                if (barEditItem1.EditValue == null || barEditItem1.EditValue.ToString() == "" || barEditItem2.EditValue == null || barEditItem2.EditValue.ToString() == "")
                {
                    throw new Exception("时间为必选项");
                }

                saleDisplay = new DataTable();
                DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1);

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
                rs = ERPorg.Corg.fun_pool(t1, t2, true);
                dt_采购计划 = rs.dtM;
                DataColumn dcc = new DataColumn("选择", typeof(bool));
                dcc.DefaultValue = false;
                dtM.Columns.Add(dcc);
                dt_bom = rs.Bom;
                dt_totalcount = rs.TotalCount;
                dt_SaleOrder = rs.salelist_mx;
                dt_SaleOrder_1 = dt_SaleOrder.Clone();
                DataColumn dc_1 = new DataColumn("参考数量", typeof(decimal));
                dc_1.DefaultValue = 0;
                dt_SaleOrder_1.Columns.Add(dc_1);
                foreach (DataRow dr1 in dt_SaleOrder.Rows)
                {
                    string sss = dr1["物料编码"].ToString().Substring(0, 2);
                    if (sss == "01")
                    {
                        DataRow dr_xm = dt_SaleOrder_1.NewRow();
                        dt_SaleOrder_1.Rows.Add(dr_xm);
                        dr_xm["客户"] = dr1["客户"].ToString();
                        dr_xm["销售订单明细号"] = dr1["销售订单明细号"].ToString();
                        dr_xm["销售订单号"] = dr1["销售订单号"].ToString();
                        dr_xm["目标客户"] = dr1["目标客户"].ToString();
                        dr_xm["客户名称"] = dr1["客户名称"].ToString();
                        dr_xm["生效日期"] = Convert.ToDateTime(dr1["生效日期"].ToString());
                        dr_xm["下单日期"] = Convert.ToDateTime(dr1["下单日期"].ToString());
                        dr_xm["物料编码"] = dr1["物料编码"].ToString();
                        dr_xm["物料名称"] = dr1["物料名称"].ToString();
                        dr_xm["规格型号"] = dr1["规格型号"].ToString();
                        dr_xm["数量"] = Convert.ToDecimal(dr1["数量"].ToString());
                        dr_xm["未完成数量"] = Convert.ToDecimal(dr1["未完成数量"].ToString());
                        dr_xm["销售数量"] = Convert.ToDecimal(dr1["销售数量"].ToString());
                        dr_xm["库存总数"] = Convert.ToDecimal(dr1["库存总数"].ToString());
                        dr_xm["未领量"] = Convert.ToDecimal(dr1["未领量"].ToString());
                        dr_xm["在制量"] = Convert.ToDecimal(dr1["在制量"].ToString());
                        dr_xm["备注"] = dr1["备注"].ToString();
                        dr_xm["在途量"] = Convert.ToDecimal(dr1["在途量"].ToString());
                        dr_xm["预计发货日期"] = Convert.ToDateTime(dr1["预计发货日期"].ToString());
                        dr_xm["存货分类"] = dr1["存货分类"].ToString();
                        dr_xm["应完工日期"] = Convert.ToDateTime(dr1["应完工日期"].ToString());
                        dr_xm["表头备注"] = dr1["表头备注"].ToString();
                        dr_xm["已关联"] = dr1["已关联"];
                        DataRow[] dr = dt_采购计划.Select(string.Format("物料编码 = '{0}'", dr1["物料编码"].ToString()));
                        if (dr.Length > 0)
                        {
                            dr_xm["参考数量"] = Convert.ToDecimal(dr[0]["参考数量"]);
                        }
                    }
                }
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
                    dv.RowFilter = "自制='true'  ";
                    gc2.DataSource = dv;

                    DataTable search_source = dt_SaleOrder.Copy();
                    foreach (DataRow dr in dtM.Rows)
                    {
                        DataRow[] p = search_source.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                        if (p.Length > 0)
                        {
                            continue;
                        }

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

                    searchLookUpEdit3.Properties.DataSource = search_source;
                    searchLookUpEdit3.Properties.DisplayMember = "物料编码";
                    searchLookUpEdit3.Properties.ValueMember = "物料编码";

                    MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                    DataTable dt_bom_1 = RBQ.SelectGroupByInto("", dt_bom, "子项编码,子项名称,子项规格", "", "子项编码");
                    searchLookUpEdit2.Properties.DataSource = dt_bom_1;
                    searchLookUpEdit2.Properties.DisplayMember = "子项编码";
                    searchLookUpEdit2.Properties.ValueMember = "子项编码";

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


        #region //按订单号计算
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (barEditItem3.EditValue == null || barEditItem3.EditValue.ToString() == "")
                {
                    throw new Exception("订单号未填写");
                }

                if (bl_calculate)
                {
                    throw new Exception("正在计算中..");
                }

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
                dt_计划.Columns.Add("需求数量", typeof(decimal));
                DataColumn dcc = new DataColumn("选择", typeof(bool));
                dcc.DefaultValue = false;
                dtM.Columns.Add(dcc);
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
        #endregion
        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                if (dr == null)
                {
                    return;
                }
                // 查看料况ToolStripMenuItem.Visible = true;
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

                });
                th.Start();


                //if (e.Button == MouseButtons.Right)
                //{
                //    contextMenuStrip1.Show(gc2, new Point(e.X, e.Y));
                //    gv2.CloseEditor();
                //    this.BindingContext[dtM].EndCurrentEdit();
                //    contextMenuStrip1.Tag = gv2;
                //}

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

        //引用
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (bl_calculate)
                {
                    throw new Exception("正在计算中,不可进行此操作");
                }

                if (dtM == null)
                {
                    throw new Exception("未计算请确认");
                }
                DataTable t = new DataTable();
                DataView dv_1 = new DataView(dtM);
                dv_1.RowFilter = "选择=1 and 自制 = 1";
                t = dv_1.ToTable();

                if (t.Rows.Count > 0)
                {
                    if (!dt_计划.Columns.Contains("领料类型"))
                    {
                        dt_计划.Columns.Add("领料类型");
                    }
                    if (!dt_计划.Columns.Contains("自制"))
                    {
                        dt_计划.Columns.Add("自制", typeof(bool));
                        dt_计划.Columns.Add("可购", typeof(bool));
                        dt_计划.Columns.Add("委外", typeof(bool));

                    }


                    string x = "select 子项编码 from 基础数据物料BOM表 where WIPType = '虚拟' group by 子项编码";
                    DataTable t_x = CZMaster.MasterSQL.Get_DataTable(x, strcon);
                    foreach (DataRow dr in t.Rows)
                    {

                        // if (Convert.ToDecimal(dr["参考数量"]) == 0) continue;
                        string wipt = "";
                        DataRow[] r_x = t_x.Select($"子项编码='{dr["物料编码"].ToString()}'");
                        if (r_x.Length > 0)
                        {
                            wipt = "虚拟";
                        }
                        else
                        {
                            wipt = "领料";
                            DataRow dr1 = dt_计划.NewRow();
                            dt_计划.Rows.Add(dr1);
                            dr1["物料编码"] = dr["物料编码"];
                            dr1["物料名称"] = dr["物料名称"];
                            dr1["规格型号"] = dr["规格型号"];
                            dr1["存货分类"] = dr["存货分类"];
                            dr1["领料类型"] = wipt;
                            if (dr["未领量"] == null || dr["未领量"].ToString() == "")
                            {
                                dr1["未领量"] = 0;
                            }
                            else
                            {
                                dr1["未领量"] = Convert.ToDecimal(dr["未领量"]);
                            }
                            if (dr["在途量"] == null || dr["在途量"].ToString() == "")
                            {
                                dr1["在途量"] = 0;
                            }
                            else
                            {
                                dr1["在途量"] = Convert.ToDecimal(dr["在途量"]);
                            }
                            if (dr["在制量"] == null || dr["在制量"].ToString() == "")
                            {
                                dr1["在制量"] = 0;
                            }
                            else
                            {
                                dr1["在制量"] = Convert.ToDecimal(dr["在制量"]);
                            }
                            dr1["工时"] = Convert.ToDecimal(dr["工时"]);
                            dr1["库存总数"] = Convert.ToDecimal(dr["库存总数"]);
                            dr1["参考数量"] = Convert.ToDecimal(dr["参考数量"]);
                            dr1["计划数量"] = Convert.ToDecimal(dr["参考数量"]);

                            dr1["受订量"] = Convert.ToDecimal(dr["受订量"]);
                            dr1["需求数量"] = Convert.ToDecimal(dr["需求数量"]);
                            dr1["已转制令数"] = Convert.ToDecimal(dr["已转制令数"]);
                            dr1["已转工单数"] = Convert.ToDecimal(dr["已转工单数"]);
                            if (dr["拼板数量"] == null || dr["拼板数量"].ToString() == "")
                            {
                                dr1["拼板数量"] = 0;
                                dr1["建议计划数量"] = Convert.ToDecimal(dr1["计划数量"]);
                            }
                            else
                            {
                                dr1["拼板数量"] = Convert.ToDecimal(dr["拼板数量"]);
                                if (Convert.ToDecimal(dr["拼板数量"]) == 0)
                                {
                                    dr1["建议计划数量"] = Convert.ToDecimal(dr1["计划数量"]);
                                }
                                else
                                {
                                    dr1["建议计划数量"] = Math.Ceiling(Convert.ToDecimal(dr1["计划数量"]) / Convert.ToDecimal(dr1["拼板数量"])) * Convert.ToDecimal(dr1["拼板数量"]);
                                }

                            }


                            if (dr["订单用量"] == null || dr["订单用量"].ToString() == "")
                            {
                                dr1["订单用量"] = 0;
                            }
                            else
                            {
                                dr1["订单用量"] = Convert.ToDecimal(dr["订单用量"]);
                            }
                            dr1["总耗时"] = Convert.ToDecimal(dr["总耗时"]);
                            // dr1["停用"] = dr["停用"];
                            DataRow[] prt = dt_totalcount.Select($"物料编码='{dr["物料编码"].ToString()}'");

                            dr1["自制"] = prt[0]["自制"];
                            dr1["可购"] = prt[0]["可购"];
                            dr1["委外"] = prt[0]["委外"];

                            //dr1["默认仓库号"] = dr["仓库号"];
                            //dr1["默认仓库名称"] = dr["仓库名称"];
                            dr1["班组编号"] = dr["班组编号"];
                            dr1["班组名称"] = dr["班组名称"];
                            if (dr["最早发货日期"] == null || dr["最早发货日期"].ToString() == "")
                            {
                                dr1["最早发货日期"] = DBNull.Value;
                            }
                            else
                            {
                                dr1["最早发货日期"] = Convert.ToDateTime(dr["最早发货日期"]);
                            }
                        }
                        //dr1["最早发货日期"] = Convert.ToDateTime(dr["最早发货日期"]);
                    }
                    DataView dv = new DataView(dt_计划);
                    dv.Sort = "最早发货日期 ";
                    gridControl2.DataSource = dv;
                }
                else
                {
                    throw new Exception("未勾选明细，请确认");
                }
                gridView1.FocusedRowHandle = gridView1.RowCount - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //删除
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {

                int[] dr1 = gridView1.GetSelectedRows();
                if (dr1.Length > 0)
                {
                    for (int i = dr1.Length - 1; i >= 0; i--)
                    {
                        DataRow dr_选中 = gridView1.GetDataRow(dr1[i]);
                        dr_选中.Delete();
                    }

                    DataRow drs = gridView1.GetDataRow(Convert.ToInt32(dr1[0]));
                    if (drs != null)
                    {
                        gridView1.SelectRow(dr1[0]);
                    }
                    else if (gridView1.GetDataRow(Convert.ToInt32(dr1[0]) - 1) != null)
                    {
                        gridView1.SelectRow(Convert.ToInt32(dr1[0]) - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //关闭界面
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_calculate)
                {
                    throw new Exception("正在计算中,不可进行此操作");
                }

                CPublic.UIcontrol.ClosePage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //生成计划单
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.ActiveControl = null;
                fun_check();
                fun_save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void fun_check()
        {
            if (bl_保存) throw new Exception("该数据已保存,不可重复操作");
            if (bl_calculate)
            {
                throw new Exception("正在计算中,不可进行此操作");
            }

            if (bl_需求单)
            {
                throw new Exception("正在计算需求单，不可进行此操作");
            }

            if (dtM == null)
            {
                throw new Exception("未计算请确认");
            }
            if (dt_计划.Rows.Count == 0)
            {
                throw new Exception("没有计划明细，请确认");
            }
            if (dt_采购需求 == null)
            {
                throw new Exception("未计算需求单，请确认");
            }
            foreach (DataRow dr in dt_计划.Rows)
            {
                if (dr["计划数量"] == DBNull.Value || Convert.ToDecimal(dr["计划数量"]) <= 0)
                {
                    continue;
                }

                DataRow[] dr_copy = dt_计划_copy.Select(string.Format("物料编码 = '{0}' and 计划数量 ={1} and 开工日期 = '{2}'", dr["物料编码"].ToString(), Convert.ToDecimal(dr["计划数量"].ToString()), Convert.ToDateTime(dr["开工日期"].ToString())));
                if (dr_copy.Length == 0)
                {
                    throw new Exception("计划单前后数据不一致，请重新计算需求单");
                }
            }
        }
        /// <summary>
        /// 主计划计划生成单表 全删掉 全覆盖
        ///  主计划采购需求单未生效的 全删 全覆盖
        /// </summary>
        private void fun_save()
        {
            //主计划计划生成单表 全删掉覆盖
            string sss = "select * from 主计划计划生成单";
            DataTable dt_计划新 = CZMaster.MasterSQL.Get_DataTable(sss, strcon);
            DataTable dt111 = dt_计划新.Copy();
            foreach (DataRow dr in dt111.Rows)
            {
                dr.SetAdded();
            }

            for (int ii = dt_计划新.Rows.Count - 1; ii >= 0; ii--)
            {
                dt_计划新.Rows[ii].Delete();
            }
            int i = 1;
            DateTime t = CPublic.Var.getDatetime();
            string s_计划单号 = string.Format("PN{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                                  CPublic.CNo.fun_得到最大流水号("PN", t.Year, t.Month));
            DataView dv = new DataView(dt_计划);
            dv.RowFilter = "计划数量>0";
            DataTable dt_1 = dv.ToTable();
            foreach (DataRow dr in dt_1.Rows)
            {
                //dr["GUID"] = System.Guid.NewGuid();
                //dr["计划单号"] = s_计划单号;
                //dr["计划单明细号"] = s_计划单号 + "-" + i.ToString("00");
                //dr["POS"] = i++;
                //dr["计划生成人"] = CPublic.Var.localUserName;
                //dr["计划生成日期"] = t;
                DataRow dr1 = dt_计划新.NewRow();
                dt_计划新.Rows.Add(dr1);
                dr1["GUID"] = System.Guid.NewGuid();
                dr1["计划单号"] = s_计划单号;
                dr1["计划单明细号"] = s_计划单号 + "-" + i.ToString("0000");
                dr1["POS"] = i++;
                dr1["计划生成人"] = CPublic.Var.localUserName;
                dr1["计划生成日期"] = t;
                dr1["物料编码"] = dr["物料编码"];
                dr1["物料名称"] = dr["物料名称"];
                dr1["规格型号"] = dr["规格型号"];
                dr1["存货分类"] = dr["存货分类"];
                dr1["未领量"] = Convert.ToDecimal(dr["未领量"]);
                dr1["在途量"] = Convert.ToDecimal(dr["在途量"]);
                dr1["在制量"] = Convert.ToDecimal(dr["在制量"]);
                dr1["工时"] = Convert.ToDecimal(dr["工时"]);
                dr1["库存总数"] = Convert.ToDecimal(dr["库存总数"]);
                dr1["参考数量"] = Convert.ToDecimal(dr["参考数量"]);
                dr1["计划数量"] = Convert.ToDecimal(dr["计划数量"]);
                dr1["受订量"] = Convert.ToDecimal(dr["受订量"]);
                // dr1["需求数量"] = Convert.ToDecimal(dr["需求数量"]);
                dr1["已转制令数"] = Convert.ToDecimal(dr["已转制令数"]);
                dr1["已转工单数"] = Convert.ToDecimal(dr["已转工单数"]);
                dr1["拼板数量"] = Convert.ToDecimal(dr["拼板数量"]);
                dr1["订单用量"] = Convert.ToDecimal(dr["订单用量"]);
                dr1["总耗时"] = Convert.ToDecimal(dr["总耗时"]);
                dr1["班组编号"] = dr["班组编号"];
                dr1["班组名称"] = dr["班组名称"];
                dr1["开工日期"] = Convert.ToDateTime(dr["开工日期"]);
                if (dr["最早发货日期"] == null || dr["最早发货日期"].ToString() == "")
                {
                    dr1["最早发货日期"] = DBNull.Value;
                }
                else
                {
                    dr1["最早发货日期"] = Convert.ToDateTime(dr["最早发货日期"]);
                }
            }
            string sql_采购需求 = "select * from 主计划采购需求单 ";
            DataTable dt_采购需求_1 = CZMaster.MasterSQL.Get_DataTable(sql_采购需求, strcon);
            DataTable dt_采购需求历史 = dt_采购需求_1.Copy();
            foreach (DataRow dr in dt_采购需求历史.Rows)
            {
                dr.SetAdded();
            }

            for (int ii = dt_采购需求_1.Rows.Count - 1; ii >= 0; ii--)
            {
                if (Convert.ToBoolean(dt_采购需求_1.Rows[ii]["生效"])) continue;
                dt_采购需求_1.Rows[ii].Delete();
            }
            int j = 1;
            if (dt_采购需求.Rows.Count > 0)
            {
                dv = new DataView(dt_采购需求);
                dv.RowFilter = "参考数量>0";
                DataTable dt_2 = dv.ToTable();
                foreach (DataRow dr in dt_2.Rows)
                {
                    DataRow dr_1 = dt_采购需求_1.NewRow();
                    dt_采购需求_1.Rows.Add(dr_1);
                    dr_1["GUID"] = System.Guid.NewGuid();
                    dr_1["计划单号"] = s_计划单号;
                    dr_1["计划单明细号"] = s_计划单号 + "-c-" + j.ToString("0000");
                    dr_1["POS"] = j++;
                    dr_1["物料编码"] = dr["物料编码"];
                    dr_1["需求来料日期"] = Convert.ToDateTime(dr["需求来料日期"]);
                    if (dr["预计开工日期"] == DBNull.Value || dr["预计开工日期"].ToString() == "")
                    {
                        dr_1["预计开工日期"] = DBNull.Value;
                    }
                    else
                    {
                        dr_1["预计开工日期"] = Convert.ToDateTime(dr["预计开工日期"]);
                    }
                    dr_1["参考数量_h"] = dr_1["通知采购数量"] = Convert.ToDecimal(dr["参考数量"]);
                    dr_1["受订量_h"] = Convert.ToDecimal(dr["受订量"]);
                    dr_1["库存总数_h"] = Convert.ToDecimal(dr["库存总数"]);
                    dr_1["在途量_h"] = Convert.ToDecimal(dr["在途量"]);
                    dr_1["委外在途_h"] = Convert.ToDecimal(dr["委外在途"]);
                    dr_1["未领量_h"] = Convert.ToDecimal(dr["未领量"]);
                    dr_1["未发量_h"] = Convert.ToDecimal(dr["未发量"]);
                    dr_1["已采未审_h"] = Convert.ToDecimal(dr["已采未审"]);
                    dr_1["采购未送检_h"] = Convert.ToDecimal(dr["采购未送检"]);
                    dr_1["已送未检_h"] = Convert.ToDecimal(dr["已送未检"]);
                    dr_1["已检未入_h"] = Convert.ToDecimal(dr["已检未入"]);
                    dr_1["订单用量_h"] = Convert.ToDecimal(dr["订单用量"]);
                    dr_1["制单人"] = CPublic.Var.localUserName;
                    dr_1["制单时间"] = t;
                }
            }
            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("计划单生成");
            try
            {
                string sql = "select * from 主计划计划生成单 where 1<>1";
                SqlCommand cmm = new SqlCommand(sql, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(dt_计划新);
                if (dt_采购需求.Rows.Count > 0)
                {
                    string sql1 = "select * from 主计划采购需求单 where 1<>1";
                    SqlCommand cmm1 = new SqlCommand(sql1, conn, ts);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmm1);
                    new SqlCommandBuilder(da1);
                    da1.Update(dt_采购需求_1);

                    string sql3 = "select * from 主计划采购需求单_历史 where 1<>1";
                    SqlCommand cmm3 = new SqlCommand(sql3, conn, ts);
                    SqlDataAdapter da3 = new SqlDataAdapter(cmm3);
                    new SqlCommandBuilder(da3);
                    da3.Update(dt_采购需求历史);

                }
                string sql2 = "select * from 主计划计划生成单_历史 where 1<>1";
                SqlCommand cmm2 = new SqlCommand(sql2, conn, ts);
                SqlDataAdapter da2 = new SqlDataAdapter(cmm2);
                new SqlCommandBuilder(da2);
                da2.Update(dt111);

                ts.Commit();
                MessageBox.Show("计划单生成成功");
                bl_保存 = true;
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                bl_保存 = false;
                if (bl_calculate)
                {
                    throw new Exception("正在计算中,不可进行此操作");
                }

                if (bl_需求单)
                {
                    throw new Exception("正在计算需求单，不可进行此操作");
                }

                if (dtM == null)
                {
                    throw new Exception("未计算请确认");
                }
                if (dt_计划.Rows.Count == 0)
                {
                    throw new Exception("没有计划明细，请确认");
                }
                DataView dv = new DataView(dt_计划);
                dv.RowFilter = "计划数量>0";
                DataTable dt_1 = dv.ToTable();
                foreach (DataRow dr in dt_1.Rows)
                {
                    if (dr["计划数量"] == DBNull.Value || Convert.ToDecimal(dr["计划数量"]) <= 0)
                    {
                        throw new Exception("计划数量有误，请确认");
                    }
                    if (dr["开工日期"].ToString() == "")
                    {
                        throw new Exception("开工日期未填，请确认");
                    }
                }
                dt_计划_copy = dt_1.Copy();
                DataTable dt_SaleOrder_1_copy = dt_SaleOrder_1.Copy();
                DataTable dt_bom_copy = dt_bom.Copy();

                string s_total = " select * from V_pooltotal ";
                DataTable total_原 = CZMaster.MasterSQL.Get_DataTable(s_total, strcon);
                DataTable dt_totalcount_copy = dt_totalcount.Copy();
                //用原来的总数覆盖 
                foreach (DataRow rr in dt_totalcount_copy.Rows)
                {
                    DataRow[] tr = total_原.Select($"物料编码='{rr["物料编码"].ToString()}'");
                    rr["总数"] = tr[0]["总数"];
                    rr["需求数量"] = 0;
                    rr["订单用量"] = 0;//2020-6-5

                }

                //DataTable dt_totalcount_copy = dt_totalcount.Copy();
                DataTable dt_采购计划_copy = dt_采购计划.Copy();
                Thread th = new Thread(() =>
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        label2.Text = "正在计算中,请稍候...";
                    }));
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt_计划_copy);
                    ds.Tables.Add(dt_SaleOrder_1_copy);
                    ds.Tables.Add(dt_bom_copy);
                    ds.Tables.Add(dt_totalcount_copy);
                    ds.Tables.Add(dt_采购计划_copy);
                    ERPorg.Corg cg = new ERPorg.Corg();
                    result rs = cg.main_Plan_calu(ds);
                    dt_采购需求 = rs.dtM;
                    foreach (DataRow dr in dt_采购需求.Rows)
                    {
                        dr["参考数量"] = Math.Ceiling(Convert.ToDecimal(dr["参考数量"]));
                    }

                    #region 不行 不能用
                    //2020-6-5 即是自制  又是外购的 需要加入 dt_采购需求
                    // dt_计划 里面录入的 自制 & 外购的 计划数量是安排生产的数量 ,要买的数量 是 dtm(生产总共缺的数量)
                    //foreach (DataRow dr in dt_计划.Rows)
                    //{
                    //    if (dr["物料编码"].ToString() == "05020103010036")
                    //    {
                    //        string s = dr["参考数量"].ToString();
                    //        string x = dr["计划数量"].ToString();
 
                    //    }

                    //    if (Convert.ToBoolean(dr["自制"]) && Convert.ToBoolean(dr["可购"]))
                    //    {
                    //        //DataRow[] r = dtM.Select($"物料编码='{dr["物料编码"].ToString()}' and 预计开工日期 = '{Convert.ToDateTime(dr["开工日期"].ToString()).Date}'");
                    //        //if (r.Length > 0)
                    //        //{
                    //        DataRow[] r_temp = dt_采购需求.Select($"物料编码='{dr["物料编码"].ToString()}' and 预计开工日期='{Convert.ToDateTime(dr["开工日期"].ToString()).Date}'");
                    //      //  用算出来缺的数- 界面录入的
                        
                    //        decimal dec = Convert.ToDecimal(dr["参考数量"]) - Convert.ToDecimal(dr["计划数量"]);
                    //            if (r_temp.Length == 0 && dec > 0)
                    //            {
                    //                DataRow[] r_total = dt_totalcount.Select($"物料编码 = '{dr["物料编码"].ToString()}'");
                    //                DataRow r_need = dt_采购需求.NewRow();
                    //                r_need["未领量"] = r_total[0]["未领量"];
                    //                r_need["在途量"] = r_total[0]["在途量"];
                    //                r_need["委外在途"] = Convert.ToDecimal(r_total[0]["委外在途"]);
                    //                r_need["预计开工日期"] = dr["开工日期"].ToString();
                    //                r_need["需求来料日期"] = Convert.ToDateTime(r_need["预计开工日期"]).AddDays(-3).Date;
                    //                r_need["物料编码"] = dr["物料编码"].ToString();
                    //                r_need["仓库号"] = r_total[0]["默认仓库号"].ToString();
                    //                r_need["仓库名称"] = r_total[0]["仓库名称"].ToString();
                    //                r_need["未发量"] = r_total[0]["未发量"].ToString();
                    //                r_need["供应商编号"] = r_total[0]["供应商编号"].ToString();
                    //                r_need["默认供应商"] = r_total[0]["默认供应商"].ToString();
                    //                r_need["采购员"] = r_total[0]["采购员"].ToString();
                    //                r_need["物料名称"] = r_total[0]["物料名称"].ToString();
                    //                r_need["规格型号"] = r_total[0]["规格型号"].ToString();
                    //                r_need["存货分类"] = r_total[0]["存货分类"].ToString();
                    //                r_need["库存总数"] = r_total[0]["库存总数"];
                    //                r_need["受订量"] = r_total[0]["受订量"].ToString();
                    //                r_need["自制"] = r_total[0]["自制"].ToString();
                    //                r_need["委外"] = r_total[0]["委外"].ToString();
                    //                r_need["ECN"] = r_total[0]["ECN"].ToString();
                    //                r_need["可购"] = r_total[0]["可购"].ToString();
                    //                r_need["已采未审"] = r_total[0]["已采未审"].ToString();
                    //                r_need["采购未送检"] = r_total[0]["采购未送检"].ToString();
                    //                r_need["已送未检"] = r_total[0]["已送未检"].ToString();
                    //                r_need["已检未入"] = r_total[0]["已检未入"].ToString();
                    //                r_need["库存下限"] = r_total[0]["库存下限"].ToString();
                    //                r_need["采购周期"] = r_total[0]["采购周期"].ToString();
                    //                r_need["最小包装"] = r_total[0]["最小包装"].ToString();
                    //                //20-1-8
                    //                r_need["供应状态"] = r_total[0]["供应状态"].ToString();
                    //                //20-1-14
                    //                r_need["停用"] = r_total[0]["停用"].ToString();
                    //                r_need["订单用量"] = r_total[0]["订单用量"].ToString();
                    //                r_need["参考数量"] = dec;
                    //            //}
                    //            dt_采购需求.Rows.Add(r_need);
                    //            }
                           
                    //    }
                    //}

                    #endregion



                    bl_需求单 = false;
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        if (rs.str_log != "")
                        {
                            label2.Text = rs.str_log;
                        }
                        else
                        {
                            label2.Text = "计算完成";
                        }
                    }));
                });
                th.IsBackground = true;
                th.Start();
                bl_需求单 = true;

            }
            catch (Exception ex)
            {
                bl_需求单 = false;
                BeginInvoke(new MethodInvoker(() =>
                {
                    label2.Text = "错误原因:" + ex.Message;
                    bl_calculate = false;
                }));
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (bl_需求单)
                {
                    throw new Exception("正在计算需求单");
                }

                if (dt_采购需求 == null)
                {
                    throw new Exception("未计算需求单,请确认");
                }

                if (dt_计划.Rows.Count == 0)
                {
                    throw new Exception("没有计划明细，请确认");
                }
                Form2 fm = new Form2();
                ui_采购需求 ui = new ui_采购需求(dt_采购需求);
                fm.Controls.Add(ui);
                fm.Text = "采购需求";
                fm.WindowState = FormWindowState.Maximized;
                ui.Dock = DockStyle.Fill;
                fm.ShowDialog();
                bl_bc = ui.bl_保存;
                dt_采购需求 = ui.dtM;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtM == null)
                {
                    throw new Exception("未计算请确认");
                }

                if (!bl_选择)
                {
                    foreach (DataRow dr in dtM.Rows)
                    {
                        if (!Convert.ToBoolean(dr["自制"]))
                        {
                            continue;
                        }

                        dr["选择"] = true;
                    }
                    bl_选择 = true;
                }
                else
                {
                    foreach (DataRow dr in dtM.Rows)
                    {
                        if (!Convert.ToBoolean(dr["自制"]))
                        {
                            continue;
                        }

                        dr["选择"] = false;
                    }
                    bl_选择 = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            {
                this.FindForm().KeyPreview = true;
            }
            else
            {
                c = Control.FromHandle(handle);//这就是
                //focusedControl =
                //c.Parent.TopLevelControl.Name.ToString();
            }

            return c;

        }
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

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

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {

                if (searchLookUpEdit2.EditValue != null && searchLookUpEdit2.EditValue.ToString() != "")
                {
                    string sql = string.Format(@"with temp_bom(产品编码, 子项编码, 仓库号, 仓库名称, wiptype, 子项类型, 数量, bom类型, bom_level ) as
         (select 产品编码, 子项编码, 仓库号, 仓库名称, WIPType, 子项类型, 数量, bom类型,1 as level from 基础数据物料BOM表
           where 子项编码 = '{0}'
           union all
   select a.产品编码,a.子项编码,a.仓库号,a.仓库名称,a.WIPType,a.子项类型,a.数量,a.bom类型,b.bom_level + 1  from 基础数据物料BOM表 a
     inner join temp_bom b on a.子项编码 = b.产品编码   ) 
          select 子项编码, 子项名称, 产品编码,  产品名称,wiptype,子项类型,数量,bom类型, 仓库号, 仓库名称
  , bom_level, 子项规格,停用  from (
  select 产品编码 as 子项编码,fx.物料名称 as 子项名称,子项编码 as 产品编码,base.物料名称 as 产品名称,wiptype,子项类型,数量,bom类型,a.仓库号,a.仓库名称
  , bom_level,fx.规格型号 as 子项规格,fx.停用 from temp_bom a
  left  join 基础数据物料信息表 base on base.物料编码 = a.子项编码
     left  join 基础数据物料信息表 fx  on fx.物料编码 = a.产品编码
   )dd  
     group by 子项编码, 子项名称, 产品编码,  产品名称,wiptype,子项类型,数量,bom类型, 仓库号, 仓库名称, bom_level, 子项规格,停用", searchLookUpEdit2.EditValue);
                    DataTable dt_fx = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt_fx.Rows.Count > 0)
                    {
                        DataView dv = new DataView(dt_计划);
                        string s = "物料编码 in (";
                        foreach (DataRow dr in dt_fx.Rows)
                        {
                            s = s + string.Format("'{0}',", dr["子项编码"]);
                        }
                        s = s.Substring(0, s.Length - 1) + ")";
                        dv.RowFilter = s;
                        gridControl2.DataSource = dv;
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

        private void button9_Click(object sender, EventArgs e)
        {
            gridControl2.DataSource = dt_计划;
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    DevExpress.XtraGrid.Views.Base.GridCell[] gcell = gridView1.GetSelectedCells();
                    IDataObject iData = Clipboard.GetDataObject();
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        string s = (String)iData.GetData(DataFormats.Text);
                        string[] xx = s.Split('\n');
                        xx = xx.Where(r => !string.IsNullOrEmpty(r)).ToArray();
                        gridColumn32.OptionsColumn.AllowEdit = false;
                        //dt_计划;
                        //选中列   
                        if (gridView1.FocusedColumn.FieldName == "开工日期")
                        {
                            for (int x = 0; x < gcell.Length; x++)
                            {
                                string x_x = "";
                                if (x >= xx.Length)
                                {
                                    x_x = xx[xx.Length - 1];
                                }
                                else
                                {
                                    x_x = xx[x];

                                }
                                gridView1.SetRowCellValue(gcell[x].RowHandle, gcell[x].Column, x_x);
                            }
                        }
                        if (gridView1.FocusedColumn.FieldName == "计划数量")
                        {
                            for (int x = 0; x < gcell.Length; x++)
                            {
                                decimal x_x = 0;
                                if (x >= xx.Length)
                                {
                                    x_x = Convert.ToDecimal(xx[xx.Length - 1]);
                                }
                                else
                                {
                                    x_x = Convert.ToDecimal(xx[x]);
                                }
                                gridView1.SetRowCellValue(gcell[x].RowHandle, gcell[x].Column, x_x);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (dr == null) return;
            if (e.Column.FieldName == "开工日期")
            {
                e.Column.OptionsColumn.AllowEdit = true;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit3.EditValue != null && searchLookUpEdit3.EditValue.ToString() != "")
                {
                    string s = "";
                    DataTable ListM = new DataTable();
                    ListM = ERPorg.Corg.billofM(ListM, searchLookUpEdit3.EditValue.ToString(), true, dt_bom);
                    if (ListM.Rows.Count > 0)
                    {
                        DataView dv = new DataView(dt_计划);
                        s = "  物料编码 in (";
                        foreach (DataRow dr in ListM.Rows)
                        {
                            s = s + string.Format("'{0}',", dr["子项编码"]);
                        }
                        s = s.Substring(0, s.Length - 1) + ")";
                        dv.RowFilter = s;
                        gridControl2.DataSource = dv;
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

        //添加主计划按钮  这个只有跳转过来 才会显示
        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridView1.CloseEditor();
                this.ActiveControl = null;
                fun_check();
                int i = 1;
                DateTime t = CPublic.Var.getDatetime();
                string s_计划单号 = string.Format("PN{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                                      CPublic.CNo.fun_得到最大流水号("PN", t.Year, t.Month));
                DataView dv = new DataView(dt_计划); //gridcontrol2 绑定的 dt

                dv.RowFilter = "计划数量>0";
                DataTable dt_1 = dv.ToTable();
                foreach (DataRow dr in dt_1.Rows)
                {
                    dr["GUID"] = System.Guid.NewGuid();
                    dr["计划单号"] = s_计划单号;
                    dr["计划单明细号"] = s_计划单号 + "-" + i.ToString("00");
                    dr["POS"] = i++;
                    dr["计划生成人"] = CPublic.Var.localUserName;
                    dr["计划生成日期"] = t;
                }
                string sql = "";
                 
                DataTable dt_采购需求_1 = new DataTable(); //主计划采购需求单 表中未生效数据
                if (bl_bc)
                {
                    sql = "select * from 主计划采购需求单 where 生效 = 0";
                    dt_采购需求_1 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dt_采购需求.Rows.Count > 0)
                    {
                        int j = 1;
                        dv = new DataView(dt_采购需求);//dt_采购需求这个是计算需求算出来的 rs.dtM
                        dv.RowFilter = "参考数量>0";
                        ///dt_2是 dt_采购需求中参考数量>0 的数据
                        DataTable dt_2 = dv.ToTable();
                        foreach (DataRow dr in dt_2.Rows)
                        {
                            //主计划采购需求单 表中未生效数据如果有       累加数量
                            DataRow[] dr_2 = dt_采购需求_1.Select($"物料编码 = '{dr["物料编码"]}' and 需求来料日期 = '{Convert.ToDateTime(dr["需求来料日期"]).Date}'");
                            if (dr_2.Length > 0)
                            {
                                dr_2[0]["参考数量_h"] = Convert.ToDecimal(dr_2[0]["参考数量_h"]) + Convert.ToDecimal(dr["参考数量"]);
                                dr_2[0]["通知采购数量"] = Convert.ToDecimal(dr_2[0]["通知采购数量"]) + Convert.ToDecimal(dr["参考数量"]);
                                dr_2[0]["制单时间"] = t;
                                dr_2[0]["制单人"] = CPublic.Var.localUserName;
                            }
                            else ///如果没有  新增
                            {
                                DataRow dr_1 = dt_采购需求_1.NewRow();
                                dt_采购需求_1.Rows.Add(dr_1);
                                dr_1["GUID"] = System.Guid.NewGuid();
                                dr_1["计划单号"] = s_计划单号;
                                dr_1["计划单明细号"] = s_计划单号 + "-c-" + j.ToString("0000");
                                dr_1["POS"] = j++;
                                dr_1["物料编码"] = dr["物料编码"];
                                dr_1["需求来料日期"] = Convert.ToDateTime(dr["需求来料日期"]).Date;
                                if (dr["预计开工日期"] == DBNull.Value || dr["预计开工日期"].ToString() == "")
                                {
                                    dr_1["预计开工日期"] = DBNull.Value;
                                }
                                else
                                {
                                    dr_1["预计开工日期"] = Convert.ToDateTime(dr["预计开工日期"]);
                                }
                                dr_1["参考数量_h"] = dr_1["通知采购数量"] = Convert.ToDecimal(dr["参考数量"]);
                                dr_1["受订量_h"] = Convert.ToDecimal(dr["受订量"]);
                                dr_1["库存总数_h"] = Convert.ToDecimal(dr["库存总数"]);
                                dr_1["在途量_h"] = Convert.ToDecimal(dr["在途量"]);
                                dr_1["委外在途_h"] = Convert.ToDecimal(dr["委外在途"]);
                                dr_1["未领量_h"] = Convert.ToDecimal(dr["未领量"]);
                                dr_1["未发量_h"] = Convert.ToDecimal(dr["未发量"]);
                                dr_1["已采未审_h"] = Convert.ToDecimal(dr["已采未审"]);
                                dr_1["采购未送检_h"] = Convert.ToDecimal(dr["采购未送检"]);
                                dr_1["已送未检_h"] = Convert.ToDecimal(dr["已送未检"]);
                                dr_1["已检未入_h"] = Convert.ToDecimal(dr["已检未入"]);
                                dr_1["订单用量_h"] = Convert.ToDecimal(dr["订单用量"]);
                                dr_1["制单人"] = CPublic.Var.localUserName;
                                dr_1["制单时间"] = t;
                            }
                        }
                    }
                }
                

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("计划单生成");
                try
                {
                    sql = "select * from 主计划计划生成单_制令 where 1<>1";
                    SqlCommand cmm = new SqlCommand(sql, conn, ts);
                    SqlDataAdapter da = new SqlDataAdapter(cmm);
                    new SqlCommandBuilder(da);
                    da.Update(dt_1);
                    if (bl_bc)
                    {
                        if (dt_采购需求.Rows.Count > 0)
                        {
                            string sql1 = "select * from 主计划采购需求单 where 1<>1";
                            SqlCommand cmm1 = new SqlCommand(sql1, conn, ts);
                            SqlDataAdapter da1 = new SqlDataAdapter(cmm1);
                            new SqlCommandBuilder(da1);
                            da1.Update(dt_采购需求_1);
                        }
                    }                   
                    ts.Commit();
                    MessageBox.Show("保存成功");
                    bl_保存 = true;
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
