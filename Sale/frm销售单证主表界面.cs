using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Web;
using System.IO;
using System.Runtime.InteropServices;
namespace ERPSale
{
    public partial class frm销售单证主表界面 : UserControl
    {
        #region 成员
        //yyyy-MM-dd HH:mm:ss 时间格式
        string UserID = CPublic.Var.LocalUserID;
        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        DataRow drM;
        DataTable dt_订单明细;
        DataTable dt_销售人员;
        bool bl_刷新 = false;

        DataTable t_片区 = ERPorg.Corg.fun_业务员片区(CPublic.Var.localUserName); //19-4-4 东屋暂时用不到
        string strConn_FS = CPublic.Var.geConn("FS");
        string cfgfilepath = "";
        #endregion

        #region 自用类
        public frm销售单证主表界面()
        {
            InitializeComponent();
        }

        private void frm销售单证主表界面_Load(object sender, EventArgs e)
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
                x.UserLayout(this.xtraTabControl1, this.Name, cfgfilepath);



                DateTime t = CPublic.Var.getDatetime();

                bar_日期_后.EditValue = Convert.ToDateTime(t.ToString("yyyy-MM-dd"));
                bar_日期_前.EditValue = Convert.ToDateTime(t.AddMonths(-1).ToString("yyyy-MM-dd"));
                bar_单据状态.EditValue = "已生效";
                bl_刷新 = true;
                bar_销售订单号.EditValue = "";

                fun_载入();
                Thread th = new Thread(() =>
                {
                    fun_载入明细();
                });
                th.Start();
                if (CPublic.Var.LocalUserTeam != "营销助理权限" && CPublic.Var.LocalUserTeam != "营销助理主管权限")
                {
                    gridColumn2.Visible = false;
                    gridColumn57.Visible = false;
                    gridColumn24.Visible = false;
                    gridColumn25.Visible = false;
                    gridColumn22.Visible = false;

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        private void refresh_single(string s_单号)
        {
            string sql = string.Format(@"select sz.*,khfl.类别名称 from 销售记录销售订单主表 sz
                                left  join 客户基础信息表 k on k.客户编号=sz.客户编号 
                                left join 客户分类表 khfl  on khfl.客户分类编码=k.客户分类编码 where  销售订单号='{0}'", s_单号);
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataRow[] r_1 = dtM.Select(string.Format("销售订单号='{0}'", s_单号));
            r_1[0].ItemArray = dt.Rows[0].ItemArray;
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            //修改
            try
            {
                drM = gv.GetDataRow(gv.FocusedRowHandle);
                if (drM == null) return;
                refresh_single(drM["销售订单号"].ToString());

                string sql_mx = string.Format(@"select smx.*,base.原ERP物料编号,/*isnull(库存总数,0) 库存总数,*/新数据  from 销售记录销售订单明细表 smx
                                 left join 基础数据物料信息表 base on  base.物料编码=smx.物料编码
                                 left join 仓库物料数量表 kc on base.物料编码=  kc.物料编码 and  kc.仓库号=smx.仓库号
                                 where   销售订单号='{0}'", drM["销售订单号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dt.Columns.Add("库存总数", typeof(decimal));
                    DataTable dt_库存1 = new DataTable();
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sql_库存 = string.Format(@"select 物料编码,sum(库存总数)库存总数 from 仓库物料数量表 
                                                      where 物料编码 = '{0}' and 仓库号 in(select 属性字段1 from 基础数据基础属性表 where 属性类别 = '仓库类别' and 布尔字段3 = 1) 
                                                     group by 物料编码", dr["物料编码"].ToString());
                        dt_库存1 = CZMaster.MasterSQL.Get_DataTable(sql_库存, strconn);
                        if (dt_库存1.Rows.Count > 0)
                        {
                            dr["库存总数"] = dt_库存1.Rows[0]["库存总数"];
                        }
                        else
                        {
                            dr["库存总数"] = 0;
                        }

                    }
                    gridControl1.DataSource = dt;

                }


                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            if (e.Clicks == 2 && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                drM = gv.GetDataRow(gv.FocusedRowHandle);
                string str_销售订单号 = drM["销售订单号"].ToString();
                //新增界面
                //if (drM["生效"].ToString() == "未生效" && "用户" == "用户")
                if (drM["生效"].ToString().ToLower() == "false")
                {
                    bool pd = false;
                    frm销售单证详细界面 fm = new frm销售单证详细界面(str_销售订单号, drM, dtM);
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    CPublic.UIcontrol.AddNewPage(fm, "销售订单");
                }
                //视图界面
                else
                {
                    frm销售单证详细界面_视图 fm = new frm销售单证详细界面_视图(drM, str_销售订单号);
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    CPublic.UIcontrol.AddNewPage(fm, "销售订单");
                }
            }
        }
        #endregion

        #region 方法
        private void fun_载入明细()
        {
            try
            {
                //DateTime t2 = System.DateTime.Now;

                SqlDataAdapter da;

                string sql = string.Format(@"select  * from v_销售明细查询 where 创建日期> '{0}' and  创建日期 <'{1}'", bar_日期_前.EditValue, Convert.ToDateTime(bar_日期_后.EditValue).AddDays(1).AddSeconds(-1));
                if (bar_单据状态.EditValue != null && bar_单据状态.EditValue.ToString() != "")
                {
                    if (bar_单据状态.EditValue.ToString() == "已生效")
                    {
                        sql += " and   生效 = 1";
                    }
                    if (bar_单据状态.EditValue.ToString() == "未生效")
                    {
                        sql += " and  生效 = 0";
                    }
                    if (bar_单据状态.EditValue.ToString() == "已完成")
                    {
                        sql += " and 明细完成 = 1 ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "未完成")
                    {
                        sql += " and 明细完成 = 0";
                    }
                    if (bar_单据状态.EditValue.ToString() == "已关闭")
                    {
                        sql += "  and   关闭 = 1";
                    }
                    if (bar_单据状态.EditValue.ToString() == "所有")
                    { }
                }


                //11/12 改 明细完成=0 为 作废=0  114 行


                //19-4-4 
                if (dt_销售人员.Rows.Count > 0)
                {

                    //sql += " and (   ";
                    //foreach (DataRow r_x in dt_销售人员.Rows)
                    //{
                    //    sql += "录入人员ID = '" + r_x["工号"].ToString().Trim() + "' or ";
                    //}
                    //sql = sql.Substring(0, sql.Length - 3);
                    //sql = sql + " ) ";
                }
                else if (CPublic.Var.LocalUserTeam != "管理员权限" && CPublic.Var.localUser部门编号 != "00010202")
                {
                    throw new Exception("你没有对应的视图权限,请找信息部核实");
                }
                string sql1;
                if (CPublic.Var.localUserName != "admin" && CPublic.Var.LocalUserTeam != "管理员权限" && CPublic.Var.localUser部门编号 != "00010202")
                {
                    string aaa = CPublic.Var.localUser部门编号;
                    if (aaa == "00010402" || aaa == "00010403")
                    {
                        sql1 = "and (v_销售明细查询.部门编号 = '00010402' or v_销售明细查询.部门编号 = '00010403')";
                        sql = sql + sql1;
                    }
                    else
                    {
                        sql1 = "and v_销售明细查询.部门编号 = '" + aaa + "'";
                        sql = sql + sql1;
                    }
                }
                //if (t_片区.Rows.Count > 0)
                //{
                //    string sx = " and 片区 in (";
                //    foreach (DataRow r in t_片区.Rows)
                //    {
                //        sx = sx + string.Format("'{0}',", r["片区"]);
                //    }
                //    sx = sx.Substring(0, sx.Length - 1) + ")";
                //    sql = sql + sx;
                //}

                dt_订单明细 = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_订单明细);


                // dt_订单明细 = WSAdapter.webservers_getdata.wsfun.fun_销售明细(sql);
                // dt_订单明细 = WSAdapter.webservers_getdata.wsmo.fun_销售明细(sql);
                BeginInvoke(new MethodInvoker(() =>
                {
                    gc_明细.DataSource = dt_订单明细;

                    bl_刷新 = false;
                }));

            }
            catch (Exception ex)
            {
                bl_刷新 = false;
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_载入明细");

            }
        }

        private void fun_载入()
        {
            try
            {
                if (dtM != null)
                {
                    dtM.Clear();
                }
                bar_销售订单号.EditValue = bar_销售订单号.EditValue.ToString().Trim();
                //视图权限
                dt_销售人员 = ERPorg.Corg.fun_hr("销售", CPublic.Var.LocalUserID);

                string s_组合 = @"select sz.*,khfl.类别名称 from 销售记录销售订单主表 sz
                                left  join 客户基础信息表 k on k.客户编号=sz.客户编号 
                                left join 客户分类表 khfl  on khfl.客户分类编码=k.客户分类编码 where 1=1 {0}";
                string s_组合1 = "";

                if (bar_销售订单号.EditValue.ToString() != "")
                {
                    s_组合1 += "and  销售订单号 = '" + bar_销售订单号.EditValue.ToString() + "'" + " and ";
                }
                else
                {
                    if (bar_日期_前.EditValue != null && bar_日期_后.EditValue != null && bar_日期_前.EditValue.ToString() != "" && bar_日期_后.EditValue.ToString() != "")
                    {
                        s_组合1 += " and 创建日期 >= '" + ((DateTime)bar_日期_前.EditValue).ToString("yyyy-MM-dd HH:mm:ss") + "'" + " and 创建日期 <= '" + ((DateTime)bar_日期_后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    }
                    if (bar_单据状态.EditValue != null && bar_单据状态.EditValue.ToString() != "")
                    {
                        if (bar_单据状态.EditValue.ToString() == "已生效")
                        {
                            s_组合1 += "and  生效 = 1 and ";
                        }
                        if (bar_单据状态.EditValue.ToString() == "未生效")
                        {
                            s_组合1 += "and  生效 = 0 and ";
                        }
                        if (bar_单据状态.EditValue.ToString() == "已完成")
                        {
                            s_组合1 += "and   完成 = 1 and ";
                        }
                        if (bar_单据状态.EditValue.ToString() == "未完成")
                        {
                            s_组合1 += "and  完成 = 0 and ";
                        }
                        if (bar_单据状态.EditValue.ToString() == "已关闭")
                        {
                            s_组合1 += "and  关闭 = 1 and ";
                        }
                        if (bar_单据状态.EditValue.ToString() == "所有")
                        {

                        }
                        //if (dt_销售人员.Rows.Count > 0)
                        //{
                        //    s_组合1 += " ( ";
                        //    foreach (DataRow r_x in dt_销售人员.Rows)
                        //    {
                        //        s_组合1 += "录入人员ID = '" + r_x["工号"].ToString().Trim() + "' or ";
                        //    }
                        //    s_组合1 = s_组合1.Substring(0, s_组合1.Length - 3);
                        //    s_组合1 = s_组合1 + " ) ";
                        //}

                        //else if (CPublic.Var.LocalUserTeam != "管理员权限")
                        //{
                        //    throw new Exception("你没有对应的视图权限,请找信息部核实");
                        //}
                        else
                        {
                            s_组合1 = s_组合1.Substring(0, s_组合1.Length - 4);
                        }
                    }
                    else
                    {
                        s_组合1 = s_组合1.Substring(0, s_组合1.Length - 4);
                    }
                }
                if (s_组合1 != "")
                {
                    //s_组合1 = s_组合1.Substring(0, s_组合1.Length - 4);
                    s_组合 = string.Format(s_组合, s_组合1);
                }
                //if (t_片区.Rows.Count > 0)
                //{
                //    string sx = " and 片区 in (";
                //    foreach (DataRow r in t_片区.Rows)
                //    {
                //        sx = sx + string.Format("'{0}',", r["片区"]);
                //    }
                //    sx = sx.Substring(0, sx.Length - 1) + ")";
                //    s_组合 = s_组合 + sx;
                ////}
                ///
                string sql1;
                if (CPublic.Var.localUserName != "admin" && CPublic.Var.LocalUserTeam != "管理员权限" && CPublic.Var.localUser部门编号 != "00010202")
                {
                    string aaa = CPublic.Var.localUser部门编号;
                    if (aaa == "00010402" || aaa == "00010403")
                    {
                        sql1 = "and (sz.部门编号 = '00010402' or sz.部门编号 = '00010403')";
                        s_组合 = s_组合 + sql1;
                    }
                    else
                    {
                        sql1 = "and sz.部门编号 = '" + aaa + "'";
                        s_组合 = s_组合 + sql1;
                    }
                }


                SqlDataAdapter da = new SqlDataAdapter(s_组合, strconn);
                da.Fill(dtM);
                //DataView dv = new DataView(dtM);
                //dv.RowFilter = string.Format("生效 = 0");
                gc.DataSource = dtM;

            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "销售单证主表界面_刷新操作");
                throw ex;
            }
        }

        private void fun_隐藏()
        {
            gv.Columns["生效日期"].Visible = false;
            gv.Columns["作废日期"].Visible = false;
            gv.Columns["完成日期"].Visible = false;
            gv.Columns["录入人员"].Visible = false;
            gv.Columns["生效人员"].Visible = false;
            gv.Columns["作废人员"].Visible = false;
            gv.Columns["完成人员"].Visible = false;
            gv.Columns["生效日期"].Visible = false;
            gv.Columns["生效日期"].Visible = false;
        }
        #endregion

        #region 界面操作
        //新增
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //drM = dtM.NewRow();
            //dtM.Rows.Add(drM);
            frm销售单证详细界面 fm = new frm销售单证详细界面();
            fm.Dock = System.Windows.Forms.DockStyle.Fill;
            CPublic.UIcontrol.AddNewPage(fm, "新增销售订单");
            fun_载入();
        }

        //刷新
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                if (bl_刷新) throw new Exception("正在查询中...");
                bl_刷新 = true;
                fun_载入();
                var cancelTokenSource = new CancellationTokenSource();
            
                Thread th = new Thread(() =>
                {
                    fun_载入明细();
                });
                th.Start();

                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    while (!cancelTokenSource.IsCancellationRequested)
                    { Console.WriteLine(DateTime.Now); Thread.Sleep(2000); }
                }
            , cancelTokenSource.Token);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //清空销售订单号
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bar_销售订单号.EditValue = "";
        }
        //关闭
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion


        private void 查看销售明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drM = gv.GetDataRow(gv.FocusedRowHandle);
            string str_销售订单号 = drM["销售订单号"].ToString();
            //新增界面
            //if (drM["生效"].ToString() == "未生效" && "用户" == "用户")
            if (drM["生效"].ToString().ToLower() == "false")
            {
                frm销售单证详细界面 fm = new frm销售单证详细界面(str_销售订单号, drM, dtM);
                fm.Dock = System.Windows.Forms.DockStyle.Fill;
                CPublic.UIcontrol.AddNewPage(fm, "销售订单");
            }
            //视图界面
            else
            {
                frm销售单证详细界面_视图 fm = new frm销售单证详细界面_视图(drM, str_销售订单号);
                fm.Dock = System.Windows.Forms.DockStyle.Fill;
                CPublic.UIcontrol.AddNewPage(fm, "销售订单");
            }
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            drM = gv_明细.GetDataRow(gv_明细.FocusedRowHandle);
            ERPSale.UI关联制令界面 frm = new ERPSale.UI关联制令界面(drM);
            CPublic.UIcontrol.AddNewPage(frm, "有关制令明细");
        }

        private void gv_明细_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip2.Show(gc_明细, new Point(e.X, e.Y));
                }
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
                if (bl_刷新) throw new Exception("正在查询数据,稍候再试");

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

                //SaveFileDialog saveFileDialog = new SaveFileDialog();
                //saveFileDialog.Title = "导出Excel";
                //saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                //DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                //if (dialogResult == DialogResult.OK)
                //{

                //    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //    //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
                //    if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
                //    {
                //        gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                //        // ERPorg.Corg.TableToExcel(dtM,saveFileDialog.FileName);
                //    }
                //    else
                //    {
                //        gc_明细.ExportToXlsx(saveFileDialog.FileName, options);
                //        //ERPorg.Corg.TableToExcel(dt_订单明细, saveFileDialog.FileName);
                //    }
                //    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                txt_显示金额_税后.Text = "";
                txt_显示金额_税前.Text = "";
                Decimal dec_税前 = 0;
                Decimal dec_税后 = 0;
                int count_界面行数 = this.gv_明细.DataRowCount;
                if (count_界面行数 > 0)
                {
                    for (int i = 0; i < count_界面行数; i++)
                    {
                        DataRow dr = gv_明细.GetDataRow(i);
                        dec_税后 = Convert.ToDecimal(dr["税后金额"]) + dec_税后;
                        dec_税前 = Convert.ToDecimal(dr["税前金额"]) + dec_税前;
                    }
                }
                txt_显示金额_税后.Text = dec_税后.ToString();
                txt_显示金额_税前.Text = dec_税前.ToString();
            }
            catch (Exception ex)
            {

            }
        }

        private void 过往明细查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv_明细.GetDataRow(gv_明细.FocusedRowHandle);
            ERPSale.fm过往明细 fm = new ERPSale.fm过往明细(dr["物料编码"].ToString());
            fm.ShowDialog();
        }

        private void 查看生产制令ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drM = gv.GetDataRow(gv.FocusedRowHandle);
            ERPSale.UI关联制令界面 frm = new ERPSale.UI关联制令界面(drM);
            CPublic.UIcontrol.AddNewPage(frm, "有关制令明细");
        }



        private void gv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gv.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                if (Convert.ToBoolean(gv.GetRowCellValue(e.RowHandle, "作废")))
                {
                    e.Appearance.BackColor = Color.LightGray;
                }
                else if (Convert.ToBoolean(gv.GetRowCellValue(e.RowHandle, "完成")))
                {
                    e.Appearance.BackColor = Color.LightGreen;

                }
                else if (!Convert.ToBoolean(gv.GetRowCellValue(e.RowHandle, "完成")))
                {
                    e.Appearance.BackColor = Color.Pink;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);

            string sql = string.Format(@"select a.销售订单号,b.物料编码,b.物料名称,b.规格型号,c.税率,b.计量单位,
        a.数量,a.税后单价,a.税后金额,a.送达日期,a.备注,c.客户订单号,c.客户名,c.创建日期,c.税后金额 as 总金额,b.产品线
               from  销售记录销售订单明细表 a,基础数据物料信息表 b ,销售记录销售订单主表 c  where 
             a.销售订单号=c.销售订单号 and b.物料编码=a.物料编码 and a.销售订单号='{0}'", dr["销售订单号"].ToString());

            System.Data.DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);

            ERPreport.销售单 form = new ERPreport.销售单(dt);
            form.ShowDialog();

        }
        public void Dowork()
        {
            //DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            //   ItemInspection.print_FMS.fun_销售单(dr["销售订单号"].ToString(), str_打印机);
        }

        private void 查看文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr["文件GUID"] == null || dr["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以查看，请先上传文件");
                }
                //string type = dr["后缀"].ToString();

                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //string fileName = foldPath + CPublic.Var.getDatetime().ToString("yyyy-MM-dd") + "T" + CPublic.Var.getDatetime().ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + dr["文件"].ToString();

                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {
                }
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                CFileTransmission.CFileClient.Receiver(dr["文件GUID"].ToString(), fileName);
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }



        private void 查看工单完成情况ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv_明细.GetDataRow(gv_明细.FocusedRowHandle);
            string s = string.Format("select  * from [生产记录生产制令子表] where 销售订单明细号='{0}'", dr["销售订单明细号"].ToString());
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            if (t.Rows.Count > 0)
            {
                ERPproduct.frm查看制令相关工单的状态 fm = new ERPproduct.frm查看制令相关工单的状态(t.Rows[0]["生产制令单号"].ToString());
                CPublic.UIcontrol.AddNewPage(fm, "工单状态查询");
            }
            else
            {
                MessageBox.Show("该销售明细尚未关联生产制令,无数据");
            }

        }

        private void 查看物料库存明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                ERPStock.ui可用库存查询 ui = new ERPStock.ui可用库存查询(dr["物料编码"].ToString());
                CPublic.UIcontrol.AddNewPage(ui, "物料库存查询");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip3.Show(gridControl1, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查看物料明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv_明细.GetDataRow(gv_明细.FocusedRowHandle);
                ERPStock.ui可用库存查询 ui = new ERPStock.ui可用库存查询(dr["物料编码"].ToString());
                CPublic.UIcontrol.AddNewPage(ui, "物料库存查询");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                drM = gv.GetDataRow(gv.FocusedRowHandle);
                string sql_mx = string.Format(@"select smx.*,base.原ERP物料编号,/*isnull(库存总数,0) 库存总数,*/新数据  from 销售记录销售订单明细表 smx
                                 left join 基础数据物料信息表 base on  base.物料编码=smx.物料编码
                                 left join 仓库物料数量表 kc on base.物料编码=  kc.物料编码 and  kc.仓库号=smx.仓库号
                                 where   销售订单号='{0}'", drM["销售订单号"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_mx, strconn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dt.Columns.Add("库存总数", typeof(decimal));
                    DataTable dt_库存1 = new DataTable();
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sql_库存 = string.Format(@"select 物料编码,sum(库存总数)库存总数 from 仓库物料数量表 
                               where 物料编码 = '{0}' and 仓库号 in(select 属性字段1 from 基础数据基础属性表 where 属性类别 = '仓库类别' and 布尔字段3 = 1) 
                               group by 物料编码", dr["物料编码"].ToString());
                        dt_库存1 = CZMaster.MasterSQL.Get_DataTable(sql_库存, strconn);
                        if (dt_库存1.Rows.Count > 0)
                        {
                            dr["库存总数"] = dt_库存1.Rows[0]["库存总数"];
                        }
                        else
                        {
                            dr["库存总数"] = 0;
                        }
                    }
                    gridControl1.DataSource = dt;
                }
            }
            catch
            {

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
    }
}
