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
    public partial class mps_采购需求查询 : UserControl
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

        public mps_采购需求查询()
        {
            InitializeComponent();
        }

       

        private void mps_采购需求查询_Load(object sender, EventArgs e)
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
                    if (r["参考数量_h"] != DBNull.Value && r["参考数量_h"].ToString() != "")
                    {
                        dec = Convert.ToDecimal(r["参考数量_h"].ToString());
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
                    if (r["参考数量_h"] != DBNull.Value && r["参考数量_h"].ToString() != "")
                    {
                        dec = Convert.ToDecimal(r["参考数量_h"].ToString());
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

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DateTime dtime = CPublic.Var.getDatetime().Date;
                DateTime dtime1 = dtime.AddMonths(-3);  //三个月前
                DateTime dtime3 = dtime.AddMonths(-6);  //半年前
                string t0 = dtime1.ToString("yyyy-MM-dd");
                string t1 = dtime.AddDays(1).ToString("yyyy-MM-dd");
                string t3 = dtime3.ToString("yyyy-MM-dd");



                string sql = $@"   select aa.*,bb.ECN,bb.供应商编号,bb.物料名称,bb.规格型号,bb.存货分类,bb.库存总数,bb.受订量,bb.计划在途,
                                 bb.未领量,bb.已采未审,bb.在途量,bb.委外在途,bb.采购未送检,bb.已送未检,bb.已检未入,bb.自制,bb.可购,bb.采购员,
                                bb.默认供应商,bb.库存下限,bb.货架描述,bb.采购周期,bb.最小包装,bb.委外,bb.默认仓库号 as 仓库号,bb.仓库名称,bb.供应状态,
                                a.季度用量,b.半年用量 
                                  from 主计划采购需求单 aa
                                 left join V_pooltotal bb on aa.物料编码 = bb.物料编码
                                  left join  (select 物料编码,-sum(实效数量)as 季度用量  from 仓库出入库明细表 where  出库入库='出库'  and  出入库时间>'{t0}' and 
                            出入库时间<'{t1}'   group by 物料编码)a on  a.物料编码=aa.物料编码  
                            left join  (select 物料编码,-sum(实效数量)as 半年用量  from 仓库出入库明细表 where  出库入库='出库'  and  出入库时间>'{t3}' and 
                            出入库时间<'{t1}'  group by 物料编码)b on  b.物料编码=bb.物料编码                      
                            where aa.关闭 = 0 ";

                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                DataColumn dc = new DataColumn("选择", typeof(bool));
                dc.DefaultValue = false;
                dtM.Columns.Add(dc);

                DataColumn dcc = new DataColumn("替代", typeof(bool));
                dcc.DefaultValue = false;
                dtM.Columns.Add(dcc);

                //dtM.Columns.Add("通知采购数量",typeof(decimal));
                //foreach(DataRow dr in dtM.Rows)
                //{
                //    dr["通知采购数量"] = Convert.ToDecimal(dr["参考数量_h"]) - Convert.ToDecimal(dr["已通知采购数量"]);
                //}

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

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {                 
                gv2.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.ActiveControl = null;
                DateTime t = CPublic.Var.getDatetime();
                DataTable dt_cs = check();

                string sql_采购需求 = $"select * from 主计划采购需求单";
                DataTable dt_采购需求 = CZMaster.MasterSQL.Get_DataTable(sql_采购需求, strcon);

                string sql_计划通知单 = "select * from 主计划计划通知单 where 1<>1";
                DataTable dt_通知主 = CZMaster.MasterSQL.Get_DataTable(sql_计划通知单, strcon);
                string sql_计划通知单明细 = "select * from 主计划计划通知单明细 where 1<>1";
                DataTable dt_通知明细 = CZMaster.MasterSQL.Get_DataTable(sql_计划通知单明细, strcon);

                string s_计划通知单号 = string.Format("PA{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                                      CPublic.CNo.fun_得到最大流水号("PA", t.Year, t.Month));

                DataRow dr_计划通知主 = dt_通知主.NewRow();
                dr_计划通知主["计划通知单号"] = s_计划通知单号;
                dr_计划通知主["GUID"] = System.Guid.NewGuid();
                dr_计划通知主["生效人"] = CPublic.Var.localUserName;
                dr_计划通知主["生效人ID"] = CPublic.Var.LocalUserID;
                dr_计划通知主["生效时间"] = t;
                dt_通知主.Rows.Add(dr_计划通知主);
                int i = 1;

                foreach (DataRow dr in dt_cs.Rows)
                {
                    DataRow[] dr1 = dt_采购需求.Select(string.Format("计划单明细号 = '{0}'", dr["计划单明细号"]));
                    if (Convert.ToDecimal(dr["通知采购数量"])<Convert.ToDecimal(dr1[0]["参考数量_h"]))
                    {
                        throw new Exception(dr["物料编码"]+ "通知采购数量小于原始数量，请拆分生效");
                    }
                    //2020-6-24 这边有可能报错  
                    if (dr1.Length == 0) throw new Exception("数据已变更,需要刷新后再试");


                    dr1[0]["生效"] = true;
                    dr1[0]["生效时间"] = t;
                    dr1[0]["生效人"] = CPublic.Var.localUserName;
                    dr1[0]["需求来料日期"] = Convert.ToDateTime(dr["需求来料日期"]);
                    dr1[0]["通知采购数量"] = Convert.ToDecimal(dr["通知采购数量"]);
                    dr1[0]["计划备注"] = dr["计划备注"];

                    DataRow dr_计划通知明细 = dt_通知明细.NewRow();
                    dt_通知明细.Rows.Add(dr_计划通知明细);
                    dr_计划通知明细["计划通知单号"] = s_计划通知单号;
                    dr_计划通知明细["计划通知单明细号"] = s_计划通知单号 + "-" + i.ToString("0000");
                    dr_计划通知明细["POS"] = i++;
                    dr_计划通知明细["计划需求明细号"] = dr["计划单明细号"];
                    dr_计划通知明细["物料编码"] = dr["物料编码"];
                    dr_计划通知明细["计划备注"] = dr["计划备注"];
                    dr_计划通知明细["需求来料日期"] = Convert.ToDateTime(dr["需求来料日期"]);
                    if (dr["预计开工日期"] == null || dr["预计开工日期"].ToString() == "")
                    {
                        dr_计划通知明细["预计开工日期"] = DBNull.Value;
                    }
                    else
                    {
                        dr_计划通知明细["预计开工日期"] = Convert.ToDateTime(dr["预计开工日期"]);
                    }
                    dr_计划通知明细["参考数量"] = Convert.ToDecimal(dr["参考数量_h"]);
                    dr_计划通知明细["通知采购数量"] = Convert.ToDecimal(dr["通知采购数量"]);
                    dr_计划通知明细["生效"] = true;
                    dr_计划通知明细["生效时间"] = t;
                }
                //CZMaster.MasterSQL.Save_DataTable(dt_tzmx, "主计划计划通知单明细", strcon);

                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("生效");
                try
                {


                    string sql = "select * from 主计划计划通知单 where 1<>1";
                    SqlCommand cmm = new SqlCommand(sql, conn, ts);
                    SqlDataAdapter da = new SqlDataAdapter(cmm);
                    new SqlCommandBuilder(da);
                    da.Update(dt_通知主);                            

                    string sql3 = "select * from 主计划计划通知单明细 where 1<>1";
                    SqlCommand cmm3 = new SqlCommand(sql3, conn, ts);
                    SqlDataAdapter da3 = new SqlDataAdapter(cmm3);
                    new SqlCommandBuilder(da3);
                    da3.Update(dt_通知明细);
                   

                    string sql2 = "select * from 主计划采购需求单 where 1<>1";
                    SqlCommand cmm2 = new SqlCommand(sql2, conn, ts);
                    SqlDataAdapter da2 = new SqlDataAdapter(cmm2);
                    new SqlCommandBuilder(da2);
                    da2.Update(dt_采购需求);



                    ts.Commit();
                    MessageBox.Show("生效成功");
                    barLargeButtonItem8_ItemClick(null, null);
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

        private DataTable check()
        {
            if (dtM == null) throw new Exception("未有任何记录");

            DataView dv = new DataView(dtM);
            dv.RowFilter = "选择=1";
            if (dv.Count == 0) throw new Exception("未选择任何记录");
            DataTable t = dv.ToTable();
            foreach (DataRow dr in t.Rows)
            {
                if (Convert.ToBoolean(dr["生效"])) throw new Exception(dr["物料编码"]+"该条数据已生效，请确认");

               

            }
            return t;
        }

         

        private void gv2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {

                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                if (dr == null) return;
                if (Convert.ToBoolean(dr["生效"]))
                {

                    //gridColumn31.OptionsColumn.ReadOnly = true;
                    //gridColumn31.OptionsColumn.AllowEdit = false;
                    拆分ToolStripMenuItem.Visible = false;
                    gridColumn42.OptionsColumn.ReadOnly = true;
                    gridColumn42.OptionsColumn.AllowEdit = false;
                    gridColumn76.OptionsColumn.ReadOnly = true;
                    gridColumn76.OptionsColumn.AllowEdit = false;
                }
                else
                {
                    //gridColumn31.OptionsColumn.ReadOnly = false;
                    //gridColumn31.OptionsColumn.AllowEdit = true;
                    gridColumn42.OptionsColumn.ReadOnly = false;
                    gridColumn42.OptionsColumn.AllowEdit = true;
                    gridColumn76.OptionsColumn.ReadOnly = false;
                    gridColumn76.OptionsColumn.AllowEdit = true;
                    拆分ToolStripMenuItem.Visible = true;
                }
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

                //if (Convert.ToDecimal(gv2.GetRowCellValue(e.RowHandle, "已采未审")) > Convert.ToDecimal(gv2.GetRowCellValue(e.RowHandle, "参考量")))
                //{
                //    e.Appearance.BackColor = Color.Pink;

                //}
                if (e.Column.FieldName == "物料编码")
                {
                    DataRow rr = gv2.GetDataRow(e.RowHandle);
                    if (Convert.ToBoolean(rr["替代"]))
                        e.Appearance.BackColor = Color.GreenYellow;

                    string sql = $@"select 物料编码,关闭  FROM 主计划计划通知单明细   where
                                  关闭 = 1 and 物料编码 = '{rr["物料编码"]}'";
                    DataTable dtt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    if (dtt.Rows.Count > 0)
                    {
                        e.Appearance.BackColor = Color.LightBlue;
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                if (dr == null) return;
                if (Convert.ToBoolean(dr["生效"]))
                {
                    
                    //gridColumn31.OptionsColumn.ReadOnly = true;
                    //gridColumn31.OptionsColumn.AllowEdit = false;
                    拆分ToolStripMenuItem.Visible = false;
                    gridColumn42.OptionsColumn.ReadOnly = true;
                    gridColumn42.OptionsColumn.AllowEdit = false;
                    gridColumn76.OptionsColumn.ReadOnly = true;
                    gridColumn76.OptionsColumn.AllowEdit = false;

                }
                else
                {
                    //gridColumn31.OptionsColumn.ReadOnly = false;
                    //gridColumn31.OptionsColumn.AllowEdit = true;
                    gridColumn42.OptionsColumn.ReadOnly = false;
                    gridColumn42.OptionsColumn.AllowEdit = true;
                    gridColumn76.OptionsColumn.ReadOnly = false;
                    gridColumn76.OptionsColumn.AllowEdit = true;
                    拆分ToolStripMenuItem.Visible = true;
                }

                Thread th = new Thread(() =>
                {
                    try
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
                    }
                    catch  
                    {

                         
                    }
                   

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

        private void 拆分ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                gv2.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.ActiveControl = null;
                DataRow r = gv2.GetDataRow(gv2.FocusedRowHandle);
                DataTable dt111 = CZMaster.MasterSQL.Get_DataTable($"select POS from 主计划采购需求单 where 计划单号 = '{r["计划单号"]}'", strcon);

                DataRow[] xx = dt111.Select("POS=max(POS)");
                int max_pos = Convert.ToInt32(xx[0]["POS"]);

                fm拆分采购需求单 frm = new fm拆分采购需求单(r, max_pos);
                frm.Text = "分批生效制令";
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.bl)
                {

                    DataRow[] tr = dtM.Select($"需求来料日期='{frm.rr["需求来料日期"].ToString()}' and 物料编码='{frm.rr["物料编码"].ToString()}'");
                    if (tr.Length > 0)
                    {
                        tr[0]["通知采购数量"] = Convert.ToDecimal(tr[0]["通知采购数量"]) + Convert.ToDecimal(frm.rr["参考数量_h"]);
                        tr[0]["参考数量_h"] = Convert.ToDecimal(tr[0]["参考数量_h"]) + Convert.ToDecimal(frm.rr["参考数量_h"]);

                    }
                    else
                    {
                        DataRow r_add = dtM.NewRow();
                        r_add.ItemArray = frm.rr.ItemArray;
                        dtM.Rows.Add(r_add);
                    }


                    //dt_计划采购需求.ImportRow(frm.rr);
                    r["通知采购数量"] = Convert.ToDecimal(r["通知采购数量"]) - Convert.ToDecimal(frm.rr["参考数量_h"]);
                    r["参考数量_h"] = Convert.ToDecimal(r["参考数量_h"]) - Convert.ToDecimal(frm.rr["参考数量_h"]);
                    CZMaster.MasterSQL.Save_DataTable(dtM, "主计划采购需求单", strcon);
                    gc2.DataSource = dtM;
                    MessageBox.Show("拆分成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gv2.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();
                this.ActiveControl = null;

                DataView dv = new DataView(dtM);
                dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
                DataTable dt = dv.ToTable();

                string sql = "select * from 主计划采购需求单 where 生效 = 0";
                DataTable dt_1 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow[] dr_1 = dt_1.Select($"计划单明细号 = '{dr["计划单明细号"]}'");
                    if (dr_1.Length > 0)
                    {
                        dr_1[0]["通知采购数量"] = Convert.ToDecimal(dr["通知采购数量"]);
                        dr_1[0]["需求来料日期"] = Convert.ToDateTime(dr["需求来料日期"]);
                        dr_1[0]["计划备注"] = dr["计划备注"];
                    }
                }
                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("保存");
                try
                {
                    string sql2 = "select * from 主计划采购需求单 where 1<>1";
                    SqlCommand cmm2 = new SqlCommand(sql2, conn, ts);
                    SqlDataAdapter da2 = new SqlDataAdapter(cmm2);
                    new SqlCommandBuilder(da2);
                    da2.Update(dt_1);
                    ts.Commit();
                    MessageBox.Show("保存成功");
                    barLargeButtonItem8_ItemClick(null, null);
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

        private void gv2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    DevExpress.XtraGrid.Views.Base.GridCell[] gcell = gv2.GetSelectedCells();
                    IDataObject iData = Clipboard.GetDataObject();
                    if (iData.GetDataPresent(DataFormats.Text))
                    {
                        string s = (String)iData.GetData(DataFormats.Text);
                        string[] xx = s.Split('\n');
                        xx = xx.Where(r => !string.IsNullOrEmpty(r)).ToArray();
                        gridColumn42.OptionsColumn.AllowEdit = false;
                        //dt_计划;
                        //选中列   

                        if (gv2.FocusedColumn.FieldName == "需求来料日期")
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
                                if(!Convert.ToBoolean(gv2.GetRowCellValue(gcell[x].RowHandle, "生效")))
                                {
                                    gv2.SetRowCellValue(gcell[x].RowHandle, gcell[x].Column, x_x);
                                }
                                
                            }
                        }
                        if (gv2.FocusedColumn.FieldName == "计划备注")
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
                                if (!Convert.ToBoolean(gv2.GetRowCellValue(gcell[x].RowHandle, "生效")))
                                {
                                    gv2.SetRowCellValue(gcell[x].RowHandle, gcell[x].Column, x_x);
                                }
                                    
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

        private void 查询驳回原因ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);

                string sql = $@"select a.*,b.物料名称,b.规格型号    FROM 主计划计划通知单明细 a
                                left join V_pooltotal b ON a.物料编码 = b.物料编码 where a.关闭 = 1 and a.物料编码 = '{dr["物料编码"]}' ";
                DataTable dttt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dttt.Rows.Count == 0) throw new Exception("没有驳回记录");
                Form2 fm = new Form2();
                ui_驳回记录查询 ui = new ui_驳回记录查询(dttt);
                fm.Controls.Add(ui);
                fm.Text = "驳回记录";
                fm.WindowState = FormWindowState.Maximized;
                ui.Dock = DockStyle.Fill;
                fm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
