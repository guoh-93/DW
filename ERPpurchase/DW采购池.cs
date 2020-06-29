using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class DW采购池 : UserControl
    {
        #region 变量
        string strcon = CPublic.Var.strConn;
        /// <summary>
        /// 延续之前计划池的 dtm就不改了
        /// </summary>
        DataTable dtM;

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

        public DW采购池()
        {
            InitializeComponent();
        }
        //
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
                    label6.Text = "错误原因:" + ex.Message;

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
                    label6.Text = "正在计算中,请稍候...";
                }));
                ERPorg.Corg.result rs = new ERPorg.Corg.result();
                rs = ERPorg.Corg.fun_pool_1(t1, t2, true);
                dtM = rs.dtM;
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dtM.Columns.Add(dc);

                dt_bom = rs.Bom;
                dt_totalcount = rs.TotalCount;
                dt_SaleOrder = rs.salelist_mx;
                IncompletePO = rs.Polist_mx;
                str_log = rs.str_log;

                dt_SaleCrderCopy = dt_SaleOrder.Copy();

                //20-4-14 增加一字段标记是否有替代关系
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
                    dv.RowFilter = "停用=0 and ( 可购=1 or 委外=1)";
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
                    label6.Text = "错误原因:" + ex.Message;
                    bl_calculate = false;
                }));
            }
        }
        /// <summary>
        /// 按单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calculate(string str_danh)
        {
            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label6.Text = "正在计算中,请稍候...";
                }));
                ERPorg.Corg.result rs = new ERPorg.Corg.result();
                rs = ERPorg.Corg.fun_pool(str_danh, true);
                dtM = rs.dtM;
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dtM.Columns.Add(dc);
                //20-4-14 增加一字段标记是否有替代关系
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

                dt_bom = rs.Bom;
                dt_totalcount = rs.TotalCount;
                dt_SaleOrder = rs.salelist_mx;
                IncompletePO = rs.Polist_mx;
                str_log = rs.str_log;
                //dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));
                //foreach (DataRow saleR in dt_SaleOrder.Rows)
                //{
                //    saleR["应完工日期"] = Convert.ToDateTime(saleR["预计发货日期"]).AddDays(-1);
                //}
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
                    dv.RowFilter = "可购=1";
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
                    label6.Text = "错误原因:" + ex.Message;
                    bl_calculate = false;
                }));
            }
        }
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
            if (dr == null) return;
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

        private void 查看bom明细ToolStripMenuItem_Click(object sender, EventArgs e)
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
            ERPproduct.UI物料BOM详细数量 frm = new ERPproduct.UI物料BOM详细数量(r["物料编码"].ToString().Trim(), dec);
            CPublic.UIcontrol.AddNewPage(frm, "详细数量");
        }
        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
            if (dr == null) return;
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                gridView2.CloseEditor();

                contextMenuStrip1.Tag = gridView2;
            }
        }
        private void 查看ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
                {
                    string s = "(可购=1 or 委外=1) ";
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
            dv.RowFilter = "可购=1 or 委外=1";
            gc2.DataSource = dv;
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataView dv = new DataView(dt_totalcount);
            dv.RowFilter = "订单用量>0";
            DataTable dt = dv.ToTable();
            //dt.Columns.Remove("总数");
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

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                gv2.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.ActiveControl = null;
                DataTable dt_cs = check();
                string s = dt_cs.Rows[0]["供应商编号"].ToString();
                frm采购单明细 frm = new frm采购单明细(dt_cs, s);
                CPublic.UIcontrol.Showpage(frm, "采购单明细");

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

        private void DW采购池_Load(object sender, EventArgs e)
        {

            DateTime t1 = CPublic.Var.getDatetime().Date.AddYears(-1);
            DateTime t2 = t1.AddYears(2);

            bar_日期.EditValue = t1;
            barEditItem1.EditValue = t2;
            if (!CPublic.Var.LocalUserTeam.Contains("采购") && CPublic.Var.LocalUserID != "admin")
            {
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                历史采购记录ToolStripMenuItem.Visible = false;
            }
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(splitContainer1, this.Name, cfgfilepath);
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

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_calculate) throw new Exception("正在计算中..");
                if (barEditItem3.EditValue == null || barEditItem3.EditValue.ToString() == "")
                {
                    throw new Exception("订单号未填写");
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
                    label6.Text = "错误原因:" + ex.Message;

                }));
                bl_calculate = false;
                MessageBox.Show("计算出错");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string s = @" select   mx.物料编码, sum(采购数量) as 已采未审   from    采购记录采购单明细表 mx
                            left join 采购记录采购单主表 zb on zb.采购单号=mx.采购单号
                            where   mx.生效 = 0  AND  mx.作废 = 0 and zb.作废=0 group by 物料编码";
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                foreach (DataRow dr in dtM.Rows)
                {
                    dr["已采未审"] = 0;
                }
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

        private void gv2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gv2.GetRow(e.RowHandle) == null)
                {
                    return;
                }

                if (Convert.ToDecimal(gv2.GetRowCellValue(e.RowHandle, "已采未审")) > Convert.ToDecimal(gv2.GetRowCellValue(e.RowHandle, "参考量")))
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
            catch (Exception)
            {

            }
        }

        private void gv2_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {

            if (e.Column.Caption == "订单缺料")
            {
                DataRow drrr = gv2.GetDataRow(e.RowHandle);

                if (Convert.ToDecimal(e.CellValue) < 0)
                {
                    e.Appearance.BackColor = Color.HotPink;
                }
            }

        }
        //20-01-08  只需要到bom中查询 与该物料 同父项同组的一个物料即可
        private void 查看替代料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                string s = string.Format(@"select  top 1 a.子项编码 as 物料编码,c.物料名称,c.规格型号,c.存货分类,d.在途量,采购未送检,已送未检,已检未入,d.库存总数 from [基础数据物料BOM表] a
            inner join (select  top 1 产品编码,子项编码,组 from [基础数据物料BOM表] where  子项编码='{0}' and 组<>'' group by 产品编码,子项编码,组 ) b
            on a.产品编码=b.产品编码 and a.组=b.组 and a.子项编码 <>b.子项编码 
            left join 基础数据物料信息表 c on c.物料编码=a.子项编码
            left join [V_pooltotal] d on d.物料编码=a.子项编码", dr["物料编码"].ToString());

                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (dt.Rows.Count == 0) throw new Exception("无替代料");
                fm查看替代料 fm = new fm查看替代料(dt);
                fm.StartPosition = FormStartPosition.CenterScreen;

                fm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 2020-6-9 增加从这边也可以推送给采购
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 推送采购ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //弹出填写数量 和 要求到料日期 
                fm单推采购 fm = new fm单推采购();
                fm.StartPosition = FormStartPosition.CenterScreen;
                fm.ShowDialog();
                DataRow r = gv2.GetDataRow(gv2.FocusedRowHandle);
                if (fm.bl_save)
                {
                    DataTable t = cg(r, fm.dec_数量, fm.time);
                    CZMaster.MasterSQL.Save_DataTable(t,"主计划采购需求单",strcon);
                    MessageBox.Show("推送成功");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        //20-6-9
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr_focus"></param>
        /// <param name="dec">填写需要推送给采购的数量 </param>
        /// <param name="time">填写的需求来料日期</param>
        private DataTable cg(DataRow dr_focus, decimal dec, DateTime time)
        {
            string sql = "select * from 主计划采购需求单 where 生效 = 0";
            DataTable dt_采购需求_1 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            DateTime t_now = CPublic.Var.getDatetime();
            string s_计划单号 = string.Format("PN{0}{1:00}{2:00}{3:0000}", t_now.Year, t_now.Month, t_now.Day,
                   CPublic.CNo.fun_得到最大流水号("PN", t_now.Year, t_now.Month));
            //主计划采购需求单 表中未生效数据如果有 累加数量
            DataRow[] dr_2 = dt_采购需求_1.Select($"物料编码 = '{dr_focus["物料编码"]}' and 需求来料日期 = '{time}'");
            if (dr_2.Length > 0)
            {
                dr_2[0]["参考数量_h"] = Convert.ToDecimal(dr_2[0]["参考数量_h"]) + Convert.ToDecimal(dr_focus["参考数量"]);
                dr_2[0]["通知采购数量"] = Convert.ToDecimal(dr_2[0]["通知采购数量"]) + dec;
                dr_2[0]["制单时间"] = t_now;
                dr_2[0]["制单人"] = CPublic.Var.localUserName;
            }
            else ///如果没有  新增
            {
                DataRow dr_1 = dt_采购需求_1.NewRow();
                dt_采购需求_1.Rows.Add(dr_1);
                dr_1["GUID"] = System.Guid.NewGuid();
                dr_1["计划单号"] = s_计划单号;
                dr_1["计划单明细号"] = s_计划单号 + "-c-" + "0001";
                dr_1["POS"] = 1;
                dr_1["物料编码"] = dr_focus["物料编码"];
                dr_1["需求来料日期"] = time;
                //if (dr_focus["预计开工日期"] == DBNull.Value || dr_focus["预计开工日期"].ToString() == "")
                //{
                //    dr_1["预计开工日期"] = DBNull.Value;
                //}
                //else
                //{
                //    dr_1["预计开工日期"] = Convert.ToDateTime(dr_focus["预计开工日期"]);
                //}
                dr_1["通知采购数量"] = dec;
                dr_1["参考数量_h"] = Convert.ToDecimal(dr_focus["参考数量"]);
                dr_1["受订量_h"] = Convert.ToDecimal(dr_focus["受订量"]);
                dr_1["库存总数_h"] = Convert.ToDecimal(dr_focus["库存总数"]);
                dr_1["在途量_h"] = Convert.ToDecimal(dr_focus["在途量"]);
                dr_1["委外在途_h"] = Convert.ToDecimal(dr_focus["委外在途"]);
                dr_1["未领量_h"] = Convert.ToDecimal(dr_focus["未领量"]);
                dr_1["未发量_h"] = Convert.ToDecimal(dr_focus["未发量"]);
                dr_1["已采未审_h"] = Convert.ToDecimal(dr_focus["已采未审"]);
                dr_1["采购未送检_h"] = Convert.ToDecimal(dr_focus["采购未送检"]);
                dr_1["已送未检_h"] = Convert.ToDecimal(dr_focus["已送未检"]);
                dr_1["已检未入_h"] = Convert.ToDecimal(dr_focus["已检未入"]);
                dr_1["订单用量_h"] = Convert.ToDecimal(dr_focus["订单用量"]);
                dr_1["制单人"] = CPublic.Var.localUserName;
                dr_1["制单时间"] = t_now;

            }

            return dt_采购需求_1;
        }



    }
}
