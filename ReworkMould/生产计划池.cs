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
    public partial class 生产计划池 : UserControl
    {
        string strcon = CPublic.Var.strConn;

        DataTable dtM;
        DataTable dt_SaleOrder = new DataTable();
        DataTable dt_SaleCrderCopy;
        DataTable dt_totalcount;             
        DataTable saleDisplay;
        DataTable dt_bom;
        DataTable dt_zjh;
        bool s_查询 = false;



        DataTable IncompleteWorkOrder = new DataTable();
        DataTable dt_主;
        bool bl_calculate = false;
        string cfgfilepath = "";
        public 生产计划池()
        {
            InitializeComponent();
        }

        private void 生产计划池_Load(object sender, EventArgs e)
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
                //string sql_主计划 = @"select mx.*,vp.在制量,vp.在途量,vp.库存总数,vp.未领量,vp.受订量 from 主计划子表 mx 
                //                     left join[V_pooltotal] vp on  mx.物料编码 = vp.物料编码  where 转单未完成数量 > 0 and 撤销 = 0 and 关闭 = 0 ";

                //dt_主 = CZMaster.MasterSQL.Get_DataTable(sql_主计划, strcon);
                string s = @"select  产品编码,产品名称,fx.存货分类 as 父项分类,fx.规格型号 as 父项规格,子项编码,子项名称,数量,zx.委外 as 子项委外,zx.自制 as 子项自制,zx.可购 as 子项可购,zx.存货分类 as 子项分类
                ,zx.规格型号 as 子项规格       from 基础数据物料BOM表 bom 
               left join 基础数据物料信息表 zx on bom.子项编码=zx.物料编码 
               left join 基础数据物料信息表 fx on bom.产品编码=fx.物料编码 ";
                dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DataColumn[] pk_bom = new DataColumn[2];
                pk_bom[0] = dt_bom.Columns["产品编码"];
                pk_bom[1] = dt_bom.Columns["子项编码"];
                dt_bom.PrimaryKey = pk_bom;
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

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string sql_zjh = "select top 1 主计划单号 from 主计划主表 order by 制单日期 desc";
                dt_zjh = CZMaster.MasterSQL.Get_DataTable(sql_zjh, strcon);
                string sql = string.Format("select count(*)条数 from 生产计划明细表 where 主计划单号  ='{0}'  ", dt_zjh.Rows[0]["主计划单号"]);
                DataTable dt1111 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if(Convert.ToInt32(dt1111.Rows[0]["条数"]) > 0)
                {
                    throw new Exception("提示：最后一张主计划单已有计算记录，请勿重复计算");
                }



                if (bl_calculate) throw new Exception("正在计算中..");
                
                Thread th = new Thread(() =>
                {
                    calculate();
                    fun_save();
                });
                th.IsBackground = true;

                th.Start();
                bl_calculate = true;
                s_查询 = false;
            }
            catch (Exception ex)
            {
                //BeginInvoke(new MethodInvoker(() =>
                //{
                //    label2.Text = "错误原因:" + ex.Message;

                //}));
                bl_calculate = false;
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_save()
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                try
                {
                    //string sql_zjh = "select top 1 主计划单号 from 主计划主表 order by 制单日期 desc";
                    //DataTable dt_zjh = CZMaster.MasterSQL.Get_DataTable(sql_zjh, strcon);
                    string sql = "select* from  生产计划主表 where 1 <> 1";
                    DataTable dt_生产计划主 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    sql = "select* from  生产计划明细表 where 1 <> 1";
                    DataTable dt_生产计划明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    string s_单号 = "";
                    int i = 1;
                    if (dtM.Rows.Count > 0)
                    {
                        s_单号 = string.Format("PS{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PS", t.Year, t.Month, t.Day));
                        DataRow dr_cgz = dt_生产计划主.NewRow();
                        dt_生产计划主.Rows.Add(dr_cgz);
                        dr_cgz["GUID"] = System.Guid.NewGuid();
                        dr_cgz["生产计划单号"] = s_单号;
                        dr_cgz["生效人"] = CPublic.Var.localUserName;
                        dr_cgz["生效人ID"] = CPublic.Var.LocalUserID;
                        dr_cgz["生效日期"] = t;
                        foreach (DataRow dr_mx in dtM.Rows)
                        {
                            if (Convert.ToBoolean(dr_mx["自制"]) == true)
                            {
                                DataRow dr_scmx = dt_生产计划明细.NewRow();
                                dt_生产计划明细.Rows.Add(dr_scmx);
                                dr_scmx["生产计划单号"] = s_单号;
                                dr_scmx["生产计划明细号"] = dr_scmx["生产计划单号"] + "-" + i.ToString("0000");
                                dr_scmx["POS"] = i++;
                                dr_scmx["物料编码"] = dr_mx["物料编码"];
                                dr_scmx["存货分类"] = dr_mx["存货分类"];
                                dr_scmx["物料名称"] = dr_mx["物料名称"];
                                dr_scmx["规格型号"] = dr_mx["规格型号"];
                                dr_scmx["最早发货日期"] = dr_mx["最早发货日期"];
                                dr_scmx["需求数量"] = Convert.ToDecimal(dr_mx["需求数量"]);
                                dr_scmx["参考数量"] = Convert.ToDecimal(dr_mx["参考数量"]);
                                dr_scmx["仓库号"] = dr_mx["仓库号"];
                                dr_scmx["仓库名称"] = dr_mx["仓库名称"];
                                dr_scmx["已转数量"] = 0;
                                dr_scmx["未转数量"] = Convert.ToDecimal(dr_mx["参考数量"]);
                                dr_scmx["主计划单号"] = dt_zjh.Rows[0]["主计划单号"].ToString();
                                if (dr_mx["订单用量"] != DBNull.Value)
                                {
                                    dr_scmx["订单用量"] = Convert.ToDecimal(dr_mx["订单用量"]);
                                }
                                if (dr_mx["最早预计开工日期"].ToString() != "")
                                {
                                    dr_scmx["最早预计开工日期"] = dr_mx["最早预计开工日期"];
                                }
                            }

                        }

                    }
                    else
                    {
                        throw new Exception("没有可保存的数据");
                    }


                    //string s_删除 = "delete  from  生产计划明细表";

                    //CZMaster.MasterSQL.ExecuteSQL(s_删除, strcon);


                    string sql_生产计划主 = "select * from  生产计划主表 where 1<>1";
                    string sql_生产计划明细 = "select * from  生产计划明细表 where 1<>1";
                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("生产计划保存");
                    try
                    {
                        SqlCommand cmm = new SqlCommand(sql_生产计划主, conn, ts);
                        SqlDataAdapter da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(dt_生产计划主);

                        cmm = new SqlCommand(sql_生产计划明细, conn, ts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(dt_生产计划明细);

                        ts.Commit();
                        

                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw ex;
                    }



                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void calculate()
        {
            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    label2.Text = "正在计算中,请稍候...";
                }));
                string sql_主计划 = @"select mx.*,vp.在制量,vp.在途量,vp.库存总数,vp.未领量,vp.受订量 from 主计划子表 mx 
                                     left join[V_pooltotal] vp on  mx.物料编码 = vp.物料编码  where 转单未完成数量 > 0 and 撤销 = 0 and 关闭 = 0 ";
                dt_主 = CZMaster.MasterSQL.Get_DataTable(sql_主计划, strcon);
                ERPorg.Corg.result rs = new ERPorg.Corg.result();
                rs = ERPorg.Corg.fun_pool(dt_主, false);
                dtM = rs.dtM;
                //dtM.Columns.Add("最早发货日期", typeof(DateTime));
                dt_bom = rs.Bom;
                dt_totalcount = rs.TotalCount;
                dt_SaleOrder = rs.salelist_mx;
                
                dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));
                foreach (DataRow saleR in dt_SaleOrder.Rows)
                {
                    saleR["应完工日期"] = Convert.ToDateTime(saleR["预计发货日期"]).AddDays(-1);
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
                    dv.RowFilter = "自制='true' and 停用 = 0";
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

        private void 查看BOM明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DevExpress.XtraGrid.Views.Grid.GridView ff = ((sender as ToolStripDropDownItem).Owner as ContextMenuStrip).Tag as DevExpress.XtraGrid.Views.Grid.GridView;
            DataRow r = ff.GetDataRow(ff.FocusedRowHandle);

            Decimal dec = 1;
            if (contextMenuStrip1.Tag == gridView2)
            {
                if (r["转单未完成数量"] != DBNull.Value && r["转单未完成数量"].ToString() != "")
                {
                    dec = Convert.ToDecimal(r["转单未完成数量"].ToString());
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
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPproduct.dll")));
            Type outerForm = outerAsm.GetType("ERPproduct.UI物料BOM详细数量", false);
            object[] drr = new object[2];
            drr[0] = r["物料编码"].ToString().Trim();
            drr[1] = dec;
            UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

            CPublic.UIcontrol.Showpage(ui, "查看BOM明细");
        }

        private void 查看料况ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
            DataTable t = dtM.Clone();
            t.ImportRow(dr);
            t.Columns["参考数量"].ColumnName = "数量";

            //ERPproduct.ui制令料况查询 ui = new ERPproduct.ui制令料况查询(t.Rows[0]);

            //CPublic.UIcontrol.Showpage(ui, "料况查询");
            Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPproduct.dll")));
            Type outerForm = outerAsm.GetType("ERPproduct.ui制令料况查询", false);
            object[] drr = new object[1];
            drr[0] = t.Rows[0];
             
            UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

            CPublic.UIcontrol.Showpage(ui, "料况查询");
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                check();

                if (bl_calculate) throw new Exception("正在计算中,不可进行此操作");

                if(s_查询 == true)
                {
                    string sssa = @"select * from [V_pooltotal]  ";
                    dt_totalcount = CZMaster.MasterSQL.Get_DataTable(sssa, strcon);
                }
              

                //dt 取生产制令表 结构  
                DataTable dt = CZMaster.MasterSQL.Get_DataTable("select  * from 生产记录生产制令表  where 1=2", strcon); //此dt传入 转制令界面

                DataTable t = new DataTable(); //用户选择的销售订单
                DataView dv_1 = new DataView(saleDisplay);
                dv_1.RowFilter = "选择=1";
                t = dv_1.ToTable();
                t.Columns["关联订单号"].ColumnName = "销售订单号";
                t.Columns["关联订单明细号"].ColumnName = "销售订单明细号";
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
                    DataRow[] sr = dt_SaleCrderCopy.Select(string.Format("主计划明细号='{0}'", dr["主计划明细号"].ToString()));
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
                        tr["备注"] = dr["备注"];

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
                bool sss = true;

                ds.Tables.Add(dt_totalcount.Copy());//基础信息及库存 
                ds.Tables.Add(dt_bom.Copy());
                ds.Tables.Add(t_relation.Copy());//tt中物料与销售订单的对应关系
                ds.Tables.Add(tt.Copy());//根据所有需要生成制令的物料清单
                ds.Tables.Add(t.Copy());//用户选中的 销售订单
                
                 

                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPpurchase.dll")));
                Type outerForm = outerAsm.GetType("ERPpurchase.ui计划池转制令_u8", false);
                object[] drr = new object[2];
                drr[0] = ds;
                drr[1] = sss;
                 
                UserControl ui = Activator.CreateInstance(outerForm, drr) as UserControl;

                CPublic.UIcontrol.Showpage(ui, "转制令确认");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void check()
        {
            DataView v = new DataView(saleDisplay);
            v.RowFilter = "选择=1";
            DataTable t = v.ToTable();
            if (v.ToTable().Rows.Count == 0) throw new Exception("未选择关联任何销售明细");
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_calculate) throw new Exception("正在计算中,不可进行此操作");
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DataView v = new DataView(dt_SaleOrder);
                    v.Sort = "生效日期 asc";
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

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bl_calculate) throw new Exception("正在计算中,不可进行此操作");
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //  dtM.Columns.Remove("已关联");
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

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView2.GetDataRow(gridView2.FocusedRowHandle);
            if (dr == null) return;
            查看料况ToolStripMenuItem.Visible = false;
            
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                gridView2.CloseEditor();
                contextMenuStrip1.Tag = gridView2;

            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DateTime t = CPublic.Var.getDatetime();
                try
                {
                    string sql = "select* from  生产计划主表 where 1 <> 1";
                    DataTable dt_生产计划主 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    sql = "select* from  生产计划明细表 where 1 <> 1";
                    DataTable dt_生产计划明细 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    string s_单号 = "";
                    int i = 1;
                    if (dtM.Rows.Count > 0)
                    {                        
                        s_单号 = string.Format("PS{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day, CPublic.CNo.fun_得到最大流水号("PS", t.Year, t.Month, t.Day));
                        DataRow dr_cgz = dt_生产计划主.NewRow();
                        dt_生产计划主.Rows.Add(dr_cgz);
                        dr_cgz["GUID"] = System.Guid.NewGuid();
                        dr_cgz["生产计划单号"] = s_单号;
                        dr_cgz["生效人"] = CPublic.Var.localUserName;
                        dr_cgz["生效人ID"] = CPublic.Var.LocalUserID;
                        dr_cgz["生效日期"] = t;
                        foreach (DataRow dr_mx in dtM.Rows)
                        {
                            if (Convert.ToBoolean(dr_mx["自制"]) == true)
                            {
                                DataRow dr_scmx = dt_生产计划明细.NewRow();
                                dt_生产计划明细.Rows.Add(dr_scmx);
                                dr_scmx["生产计划单号"] = s_单号;
                                dr_scmx["生产计划明细号"] = dr_scmx["生产计划单号"] + "-" + i.ToString("0000");
                                dr_scmx["POS"] = i++;
                                dr_scmx["物料编码"] = dr_mx["物料编码"];
                                dr_scmx["存货分类"] = dr_mx["存货分类"];
                                dr_scmx["物料名称"] = dr_mx["物料名称"];
                                dr_scmx["规格型号"] = dr_mx["规格型号"];
                                dr_scmx["最早发货日期"] = dr_mx["最早发货日期"];
                                dr_scmx["需求数量"] = Convert.ToDecimal(dr_mx["需求数量"]);
                                dr_scmx["参考数量"] = Convert.ToDecimal(dr_mx["参考数量"]);                               
                                dr_scmx["仓库号"] = dr_mx["仓库号"];
                                dr_scmx["仓库名称"] = dr_mx["仓库名称"];
                                dr_scmx["完成数量"] = 0;
                                dr_scmx["未完成数量"] = Convert.ToDecimal(dr_mx["参考数量"]);
                                dr_scmx["订单用量"] = Convert.ToDecimal(dr_mx["订单用量"]);
                            }

                        }
                        
                    }
                    else
                    {
                        throw new Exception("没有可保存的数据");
                    }


                    //string s_删除 = "delete  from  生产计划明细表";

                    //CZMaster.MasterSQL.ExecuteSQL(s_删除, strcon);


                    string sql_生产计划主 = "select * from  生产计划主表 where 1<>1";
                    string sql_生产计划明细 = "select * from  生产计划明细表 where 1<>1";
                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("生产计划保存");
                    try
                    {
                        SqlCommand cmm = new SqlCommand(sql_生产计划主, conn, ts);
                        SqlDataAdapter da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(dt_生产计划主);

                        cmm = new SqlCommand(sql_生产计划明细, conn, ts);
                        da = new SqlDataAdapter(cmm);
                        new SqlCommandBuilder(da);
                        da.Update(dt_生产计划明细);

                        ts.Commit();
                        MessageBox.Show("保存成功");

                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw ex;
                    }



                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
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
                DateTime t = CPublic.Var.getDatetime();
                string sql_主计划 = @"select mx.*,vp.在制量,vp.在途量,vp.库存总数,vp.未领量,vp.受订量 from 主计划子表 mx 
                                                    left join[V_pooltotal] vp on  mx.物料编码 = vp.物料编码  where 转单未完成数量 > 0  and 撤销 = 0 and 关闭 = 0  ";
                dt_主 = CZMaster.MasterSQL.Get_DataTable(sql_主计划, strcon);
                DataColumn[] pk_dt_主 = new DataColumn[1];
                pk_dt_主[0] = dt_主.Columns["主计划明细号"];
                dt_主.PrimaryKey = pk_dt_主;
                DateTime tt = CPublic.Var.getDatetime();
                string sql = @"select mx.物料编码,vp.存货分类,mx.物料名称,mx.规格型号,mx.最早发货日期,mx.需求数量, mx.订单用量,mx.参考数量,mx.仓库号,mx.仓库名称,mx.最早预计开工日期,vp.库存总数,
                              vp.未领量,vp.在途量,vp.在制量,vp.受订量,vp.自制,vp.已转制令数,vp.工时,(mx.参考数量*vp.工时)总耗时,vp.已转工单数,vp.拼板数量 
                              from  生产计划明细表 mx
                              left join  [V_pooltotal] vp on  mx.物料编码 = vp.物料编码 
                              where 生产计划单号  = (select top 1 生产计划单号 from 生产计划主表 order by 生效日期 desc)";



                dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                DataColumn[] pk_dtM = new DataColumn[1];
                pk_dtM[0] = dtM.Columns["物料编码"];
                dtM.PrimaryKey = pk_dtM;
                DateTime ttt = CPublic.Var.getDatetime();
                
                DateTime tttt = CPublic.Var.getDatetime();
                // s = @"select * from [V_pooltotal]  ";
                // dt_totalcount = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                DataTable dt_SaleOrder_mx = dt_主;
                
                dt_SaleOrder = dt_SaleOrder_mx;
                dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));
                foreach (DataRow saleR in dt_SaleOrder.Rows)
                {
                    saleR["应完工日期"] = Convert.ToDateTime(saleR["预计发货日期"]).AddDays(-1);
                }

                dt_SaleCrderCopy = dt_SaleOrder.Copy();
                DataColumn dd = new DataColumn("选择", typeof(bool));
                dd.DefaultValue = false;
                dt_SaleCrderCopy.Columns.Add(dd);
                bl_calculate = false;

                searchLookUpEdit1.Properties.DataSource = dt_主;
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                searchLookUpEdit1.Properties.ValueMember = "物料编码";
                gridControl1.DataSource = null;
                gridControl2.DataSource = null;
                //BeginInvoke(new MethodInvoker(() =>
                //{



                //    DataTable search_source = dt_SaleOrder.Copy();
                //    //foreach (DataRow dr in dtM.Rows)
                //    //{
                //    //    DataRow[] p = search_source.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                //    //    if (p.Length > 0) continue;
                //    //    DataRow x = search_source.NewRow();
                //    //    x["物料编码"] = dr["物料编码"];
                //    //    x["物料名称"] = dr["物料名称"];
                //    //    x["规格型号"] = dr["规格型号"];
                //    //    x["存货分类"] = dr["存货分类"];
                //    //    search_source.Rows.Add(x);
                //    //}
                //    searchLookUpEdit1.Properties.DataSource = search_source;
                //    searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                //    searchLookUpEdit1.Properties.ValueMember = "物料编码";
                //    gridControl1.DataSource = null;
                //    gridControl2.DataSource = null;
                //}));

                gc2.DataSource = dtM;
                t = CPublic.Var.getDatetime();
                s_查询 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
