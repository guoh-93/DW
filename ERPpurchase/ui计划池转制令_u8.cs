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
namespace ERPpurchase
{
    public partial class ui计划池转制令_u8 : UserControl
    {
        #region 变量
        DataTable t_库存;
        DataTable dt_bom;
        DataTable t_relation;
        DataTable list_PA;
        DataTable salelist;

        DataTable dtM;
        DataTable dtP;
        bool bl_跳转 = false;
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        #endregion
        public ui计划池转制令_u8(DataSet ds)
        {
            InitializeComponent();
            t_库存 = ds.Tables[0];
            dt_bom = ds.Tables[1];
            t_relation = ds.Tables[2];
            list_PA = ds.Tables[3];
            salelist = ds.Tables[4];
        }


        public ui计划池转制令_u8(DataSet ds,bool ss_跳转 )
        {
            InitializeComponent();
            t_库存 = ds.Tables[0];
            dt_bom = ds.Tables[1];
            t_relation = ds.Tables[2];
            list_PA = ds.Tables[3];
            salelist = ds.Tables[4];
            bl_跳转 = ss_跳转;
        }
        private void ui计划池转制令_u8_Load(object sender, EventArgs e)
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
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        //关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show(string.Format("确认是否关闭此界面"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                CPublic.UIcontrol.ClosePage();
            }
        }


        private void fun_load()
        {
            string s = @"select  zl.*,新数据,工时,拼板数量 from  生产记录生产制令表 zl
                        left join 基础数据物料信息表 base on zl.物料编码=base.物料编码  where 1=2";
            dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataColumn dc = new DataColumn("库存总数", typeof(int));
            dc.DefaultValue = 0;
            dtM.Columns.Add(dc);
            DataColumn dc1 = new DataColumn("人力", typeof(int));
            dc1.DefaultValue = 1;
            dtM.Columns.Add(dc1);
            dtM.Columns.Add("总需数量", typeof(decimal));
            dtM.Columns.Add("建议开工日期", typeof(DateTime));
            dtM.Columns.Add("领料类型");


            s = "select  * from 生产记录生产制令子表 where 1=2";
            dtP = CZMaster.MasterSQL.Get_DataTable(s, strcon);

            s = @"select 子项编码,wiptype  from  基础数据物料BOM表 where WIPType in ('虚拟','领料') group by 子项编码,wiptype ";
            DataTable t_wiptype = CZMaster.MasterSQL.Get_DataTable(s, strcon); 

            foreach (DataRow dr in list_PA.Rows)
            {
                DataRow r = dtM.NewRow();
                 DataRow []rtype= t_wiptype.Select(string.Format("子项编码='{0}'",dr["物料编码"]));
                if(rtype.Length==1)
                {
                    r["领料类型"] = rtype[0]["wiptype"]; 
                }
                else
                {
                    r["领料类型"] = "领料";
                }
                r["生产制令类型"] = "计划类型";
                r["加急状态"] = "正常";

                r["物料编码"] = dr["物料编码"];
                r["总需数量"] = r["制令数量"] = dr["参考数量"];
                r["物料名称"] = dr["物料名称"];
                r["规格型号"] = dr["规格型号"];
                DataRow[] dbase = t_库存.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                r["工时"] = dbase[0]["工时"];
                r["生产车间"] = dbase[0]["车间编号"];
                DataTable tt = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
                if (tt.Rows.Count > 0)
                {
                    r["生产车间"] = tt.Rows[0]["生产车间"];
                }
                r["新数据"] = dbase[0]["新数据"];
                r["拼板数量"] = dbase[0]["拼板数量"];

                r["库存总数"] = dbase[0]["库存总数"];
                DataRow[] zlmx = t_relation.Select(string.Format("子项编码='{0}'", dr["物料编码"]));
                DateTime? t = null;

                foreach (DataRow zl in zlmx) //这里需要往dtp里增加记录 关联销售订单的记录
                {
                    //r["备注"] = r["备注"].ToString() + zl["备注"].ToString();
                    //20-5-13前面传过来的时候 备注已经累加好了  只需要取最后一次 同编码都一样
                    r["备注"] =  zl["备注"].ToString();

                    DataRow r_p = dtP.NewRow();
                    r_p["销售订单明细号"] = zl["销售订单明细号"];
                    r_p["销售订单号"] = zl["销售订单号"];
                    r_p["物料编码"] = zl["子项编码"];
                    r_p["销售备注"] = zl["备注"];

                    DataRow[] dbase_zl = t_库存.Select(string.Format("物料编码='{0}'", zl["子项编码"]));
                    r_p["物料名称"] = dbase_zl[0]["物料名称"];
                    r_p["规格型号"] = dbase_zl[0]["规格型号"];
                    if (!t.HasValue || t > Convert.ToDateTime(zl["应完工日期"])) t = Convert.ToDateTime(zl["应完工日期"]);

                    int x = Convert.ToInt32(zl["销售订单明细号"].ToString().Substring(zl["销售订单明细号"].ToString().Length - 2, 2));
                    if (salelist.Columns.Contains("销售订单明细号"))
                    {
                        DataRow[] sale = salelist.Select(string.Format("销售订单明细号='{0}' ", zl["销售订单明细号"]));
                        r_p["数量"] = sale[0]["数量"];
                        r_p["送达日期"] = sale[0]["预计发货日期"];
                        r_p["客户"] = sale[0]["客户名称"];  
                    }
                    else
                    {
                        DataRow[] sale = salelist.Select(string.Format("订单号='{0}' and 行号={1}", zl["销售订单号"], x));
                        r_p["数量"] = sale[0]["销售数量"];
                        r_p["送达日期"] = sale[0]["预计发货日期"];
                        r_p["客户"] = sale[0]["客户名称"];//这边有个U8暂时用的界面 没有统一
                    }
                    // r_p["计量单位"] = sale[0]["计量单位"];
                    dtP.Rows.Add(r_p);
                }
                r["预完工日期"] = t;
                dtM.Rows.Add(r);
            }

            //dtm中每个建议开工日期都要填
            //怎么赋值
            foreach (DataRow dr in salelist.Rows)
            {
                DataRow[] cx = dtM.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                if (cx.Length > 0)
                {
                    ///工时 东屋 指做一个产品一个人工要多少小时 
                    // double dob = Convert.ToDouble(cx[0]["人力"]) * Convert.ToDouble(cx[0]["工时"]);
                    double dob = Convert.ToDouble(cx[0]["工时"])/ Convert.ToDouble(cx[0]["人力"]) ; //那么 多个人 就是 工时/人力
                    int x = 0;
                    if (dob > 0)
                    {
                       // x = (int)Math.Ceiling(Convert.ToDouble(cx[0]["制令数量"]) / dob);
                        x = (int)Math.Ceiling(Convert.ToDouble(cx[0]["制令数量"])* dob);
                        DataView dv = new DataView(t_relation);
                        string xx = string.Format("销售订单明细号='{0}'", dr["销售订单明细号"].ToString());
                        dv.RowFilter = xx;

                        DataTable tc = dv.ToTable();
                        foreach (DataRow tcR in tc.Rows)
                        {
                            DataRow[] rrr = dtM.Select(string.Format("物料编码='{0}'", tcR["子项编码"]));
                            rrr[0]["建议开工日期"] = Convert.ToDateTime(rrr[0]["预完工日期"]).AddDays(-x);
                        }
                    }
                }
            }
            gc.DataSource = dtM;
            dtM.Columns.Add("已转工单数", typeof(decimal));
            dtM.Columns.Add("已转制令数", typeof(decimal));

            foreach (DataRow dr in dtM.Rows)
            {
                string sss = string.Format(@"SELECT     物料编码, SUM(制令数量) AS 已转制令数, SUM(已排单数量) AS 已转工单数
                            FROM dbo.生产记录生产制令表  WHERE 完成 = 0 and  关闭 = 0  and 物料编码 ='{0}'GROUP BY 物料编码", dr["物料编码"]);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(sss, strcon);
                if (t.Rows.Count > 0)
                {
                    dr["已转制令数"] = t.Rows[0]["已转制令数"];
                    dr["已转工单数"] = t.Rows[0]["已转工单数"];
                }
                else
                {
                    dr["已转制令数"] = 0;
                    dr["已转工单数"] = 0;

                }

            }
        }
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr == null) return;
                DataView dv = new DataView(dtP);
                dv.RowFilter = string.Format("物料编码='{0}'", dr["物料编码"]);
                //DataRow []sale= t_relation.Select(string.Format("子项编码='{0}'",dr["物料编码"]));
                //string s = "销售订单号 in (";
                //foreach (DataRow r in sale)
                //{
                //    s = s + "'" + r["销售订单号"] + "',";
                //}
                //s = s.Substring(0, s.Length - 1) + ")";
                //dv.RowFilter = s;
                gc_关联订单.DataSource = dv;

                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                    gv.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                    contextMenuStrip1.Tag = gv;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MessageBox.Show("导出格式未提供,暂未做导出功能");
        }

        /// <summary>
        ///   传入 计算后的天数 和当前选中行,s为修改的列名
        /// </summary>
        /// <param name="x"></param>
        /// <param name="dr"></param>
        /// <param name="s"></param>
        private bool date(int x, DataRow dr, string s)
        {
            DateTime? t = null;
            string ss = "";
            if (s != "建议开工日期") //若为修改人力 根据 x 和预完工日期算 建议开工日期
            {
                t = Convert.ToDateTime(dr["预完工日期"]).AddDays(-x);
                s = "建议开工日期";
                ss = "预完工日期";
            }
            else
            {
                t = Convert.ToDateTime(dr[s]).AddDays(x);
                s = "预完工日期";
                ss = "建议开工日期";
            }
            if (t.HasValue)
            {
                //DataRow[] salsr = salelist.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                DataRow[] rr = salelist.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                if (rr.Length > 0)
                {
                    string SoNo = "";
                    foreach (DataRow sr in rr)
                    {
                        // SoNo = SoNo + "'" + sr["订单号"].ToString() + "-" + Convert.ToInt32(sr["行号"]).ToString("00") + "',";
                        SoNo = SoNo + "'" + sr["销售订单明细号"] + "',";

                    }
                    SoNo = SoNo.Substring(0, SoNo.Length - 1);

                    DataView dv = new DataView(t_relation);
                    dv.RowFilter = string.Format("销售订单明细号 in ({0})", SoNo);
                    DataTable tc = dv.ToTable();
                    foreach (DataRow tcR in tc.Rows)
                    {
                        DataRow[] rrr = dtM.Select(string.Format("物料编码='{0}'", tcR["子项编码"]));
                        if (rrr.Length > 0)
                        {
                            rrr[0][s] = t;
                            rrr[0][ss] = dr[ss];
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Value.ToString() != "")
                {
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    if (e.Column.Caption != "生产备注")
                    {
                        if (e.Column.Caption == "制令数量")
                        {
                            DataTable ListM = new DataTable();
                            ListM = ERPorg.Corg.billofM_带数量(ListM, dr["物料编码"].ToString(), false);
                            foreach (DataRow Mr in dtM.Rows)
                            {
                                DataRow[] Mr_r = ListM.Select(string.Format("子项编码='{0}'", Mr["物料编码"]));
                                if (Mr_r.Length > 0)
                                {
                                    decimal dec = Convert.ToDecimal(dr["制令数量"]) * Convert.ToDecimal(Mr_r[0]["数量"]);
                                    if (dec < Convert.ToDecimal(Mr["总需数量"])) Mr["制令数量"] = dec;
                                    else Mr["制令数量"] = Mr["总需数量"];
                                }
                            }
                        }
                        if (Convert.ToDouble(dr["工时"]) == 0) return;
                        double a = Convert.ToDouble(dr["制令数量"]) / (Convert.ToDouble(dr["工时"]) * Convert.ToDouble(dr["人力"]));
                        int x = (int)Math.Ceiling(a);
                        bool bl = date(x, dr, e.Column.Caption);
                        if (!bl) throw new Exception("检查人力或录入的日期是否有问题");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void gv_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject("");
                e.Handled = true;
                string s = "";
                DevExpress.XtraGrid.Views.Base.GridCell[] c = gv.GetSelectedCells();
                for (int i = 0; i < c.Length; i++)
                {
                    s = s + gv.GetRowCellValue(c[i].RowHandle, c[i].Column) + "\r\n";
                }
                Clipboard.SetDataObject(s);
            }
        }
        private void gv_关联订单_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_关联订单.GetFocusedRowCellValue(gv_关联订单.FocusedColumn));
                e.Handled = true;
            }
        }
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
        private void check()
        {
            this.ActiveControl = null;
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                if (dr["建议开工日期"].ToString() == "" || dr["预完工日期"].ToString() == "")
                    throw new Exception("开工完工日期未确认");
                decimal dec;
                if (!decimal.TryParse(dr["制令数量"].ToString().Trim(), out dec)) throw new Exception("制令数量输入不正确");
                if (dec <= 0) throw new Exception("制令数量不可小于或等于0");
            }
        }
        private void save()
        {
            DateTime t = CPublic.Var.getDatetime();
            string yy = t.Year.ToString().Substring(2, 2);
            string s_ph  = string.Format("{0}{1:00}{2:00}{3:0000}", yy,t.Month, t.Day,
                          CPublic.CNo.fun_得到最大流水号("JHPH", t.Year, t.Month));
            foreach (DataRow dr in dtM.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) continue;
                dr["生产制令单号"] = string.Format("PM{0}{1:00}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                          CPublic.CNo.fun_得到最大流水号("PM", t.Year, t.Month));
                dr["GUID"] = System.Guid.NewGuid();
                dr["未排单数量"] = dr["制令数量"];
                dr["预开工日期"] = dr["建议开工日期"];
                dr["预计完工日期"] = dr["预完工日期"];

                dr["日期"] = t;
                dr["制单人员"] = dr["操作人员"] = dr["生效人员"] = CPublic.Var.localUserName;
                dr["制单人员ID"] = dr["操作人员ID"] = dr["生效人员ID"] = CPublic.Var.LocalUserID;

                string s = string.Format(@"select  仓库号,仓库名称,b_班组编号,b_班组名称  from 基础数据物料信息表 where 物料编码='{0}'", dr["物料编码"]);
                DataTable temp = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                dr["仓库号"] = temp.Rows[0]["仓库号"];
                dr["备注5"] = s_ph;
                dr["班组ID"] = temp.Rows[0]["b_班组编号"];
                dr["班组"] = temp.Rows[0]["b_班组名称"];




                //然后根据 物料编码去  dtP中找 制令子记录 
                DataRow[] r_mx = dtP.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                foreach (DataRow r in r_mx)
                {
                    r["生产制令单号"] = dr["生产制令单号"];
                    r["GUID"] = System.Guid.NewGuid();
                }

            }
            for (int i = dtP.Rows.Count - 1; i >= 0; i--)
            {
                if (dtP.Rows[i]["生产制令单号"].ToString() == "")
                {
                    dtP.Rows[i].Delete();
                }

            }
            DataTable dt_生产计划明细 = new DataTable();
            DataTable dt_主计划明细 = new DataTable();




            if (bl_跳转)
            {

                decimal i = 0;
                foreach (DataRow dr in dtM.Rows)
                {
                    string sql_生产计划 = string.Format("select * from 生产计划明细表 where 物料编码 = '{0}' and 生产计划单号 = (select top 1 生产计划单号 from 生产计划主表 order by 生效日期 desc)", dr["物料编码"]);
                    SqlDataAdapter da = new SqlDataAdapter(sql_生产计划, strcon);
                    da.Fill(dt_生产计划明细);
                    //dt_生产计划明细 = CZMaster.MasterSQL.Get_DataTable(sql_生产计划, strcon);

                    if (dt_生产计划明细.Rows.Count > 0)
                    {
                        DataRow[] dr11 = dt_生产计划明细.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        dr11[0]["已转数量"] = Convert.ToDecimal(dr11[0]["已转数量"]) + Convert.ToDecimal(dr["制令数量"]);
                        dr11[0]["未转数量"] = Convert.ToDecimal(dr11[0]["未转数量"]) - Convert.ToDecimal(dr["制令数量"]);
                        dr11[0]["参考数量"] = Convert.ToDecimal(dr11[0]["未转数量"]);
                        if (Convert.ToDecimal(dr11[0]["未转数量"]) < 0)
                        {
                            dr11[0]["未转数量"] = 0;
                            dr11[0]["参考数量"] = 0;
                        }
                        i = Convert.ToDecimal(dr11[0]["需求数量"]) - Convert.ToDecimal(dr11[0]["未转数量"]);
                    }

                    string sql_主计划 = string.Format("select * from  主计划子表 where 物料编码 = '{0}' order by 预计发货日期", dr["物料编码"]);
                    da = new SqlDataAdapter(sql_主计划, strcon);
                    da.Fill(dt_主计划明细);
                   // dt_主计划明细 = CZMaster.MasterSQL.Get_DataTable(sql_主计划, strcon);
                    if (dt_主计划明细.Rows.Count > 0)
                    {
                        DataRow[] dr22 = dt_主计划明细.Select(string.Format("物料编码 = '{0}'", dr["物料编码"]));
                        foreach (DataRow dr_主 in dr22)
                        {
                            if (Convert.ToDecimal(dr_主["转单未完成数量"]) < i)
                            {
                                dr_主["转单未完成数量"] = 0;
                                dr_主["数量"] = 0;
                                dr_主["转单完成数量"] = dr_主["此次转单数量"];
                                dr_主["完成"] = true;
                                i = i - Convert.ToDecimal(dr_主["此次转单数量"]);

                            }
                            else
                            {
                                dr_主["转单未完成数量"] = Convert.ToDecimal(dr_主["转单未完成数量"]) - i;
                                dr_主["数量"] = Convert.ToDecimal(dr_主["数量"]) - i;
                                dr_主["转单完成数量"] = i;

                            }
                        }
                    }
                }
            }

            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("u8转制令");
            try
            {
                string sql = "select * from 生产记录生产制令表 where 1<>1";
                SqlCommand cmm = new SqlCommand(sql, conn, ts);
                SqlDataAdapter da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(dtM);
                //制令明细表
                sql = "select * from 生产记录生产制令子表 where 1<>1";
                cmm = new SqlCommand(sql, conn, ts);

                da = new SqlDataAdapter(cmm);
                new SqlCommandBuilder(da);
                da.Update(dtP);

                if (bl_跳转)
                {
                    sql = "select * from 生产计划明细表 where 1<>1";
                    cmm = new SqlCommand(sql, conn, ts);
                    da = new SqlDataAdapter(cmm);
                    new SqlCommandBuilder(da);
                    da.Update(dt_生产计划明细);

                    sql = "select * from 主计划子表 where 1<>1";
                    cmm = new SqlCommand(sql, conn, ts);

                    da = new SqlDataAdapter(cmm);
                    new SqlCommandBuilder(da);
                    da.Update(dt_主计划明细);
                }

                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw new Exception(ex.Message);
            }

        }
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                check();

                save();

                MessageBox.Show("ok");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            //if (MessageBox.Show(string.Format("确认删除当前选中行？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            //{
            //    dr.Delete();
            //}
            try
            {
                if (MessageBox.Show(string.Format("确认删除当前选中行？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    int[] dr1 = gv.GetSelectedRows();
                    if (dr1.Length > 0)
                    {
                        for (int i = dr1.Length - 1; i >= 0; i--)
                        {
                            DataRow dr_选中 = gv.GetDataRow(dr1[i]);
                            dr_选中.Delete();

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看料况ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            DataTable t = dtM.Clone();
            t.ImportRow(dr);
            t.Columns["制令数量"].ColumnName = "数量";

            ERPproduct.ui制令料况查询 ui = new ERPproduct.ui制令料况查询(t.Rows[0]);

            CPublic.UIcontrol.Showpage(ui, "料况查询");
        }
    }
}
