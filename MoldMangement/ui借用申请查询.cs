using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;

namespace MoldMangement
{
    public partial class ui借用申请查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        DataRow dr_借还;
        DataTable dt_借还申请表附表 = new DataTable();
        DataTable dt_借还申请表 = new DataTable();
        DataTable dt_归还申请主 = new DataTable();
        DataTable dt_归还申请子 = new DataTable();

        string cfgfilepath = "";
        public ui借用申请查询()
        {
            InitializeComponent();
        }

        private void ui借用申请查询_Load(object sender, EventArgs e)
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
                bar_日期后.EditValue = t.Date.AddDays(1).AddSeconds(-1);
                bar_日期前.EditValue = t.Date.AddDays(-15);
                //bar_单据状态.EditValue = "未归还";
                fun_载入();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                DateTime t1 = Convert.ToDateTime(bar_日期前.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(bar_日期后.EditValue).Date.AddDays(1).AddSeconds(-1);
                string s_条件 = string.Format(" and 申请日期>'{0}' and 申请日期<'{1}'", t1, t2);
                string sql = "select * from 借还申请表 where 1=1 " + s_条件;
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                
                sql = string.Format(@"select jhz.申请批号,jhz.借用类型,jhz.原因分类,jhz.申请人,jhz.申请日期,jhz.备注 as 表头备注,jhz.目标客户,jhz.锁定,
                                     jhz.相关单位,jhz.借用人员,jhz.申请人部门,jhmx.申请批号明细,jhmx.物料编码,jhmx.物料名称,jhmx.规格型号,jhmx.仓库名称,jhmx.申请数量,
                                     jhmx.归还数量,jhmx.预计出库日期,jhmx.已借出数量,jhmx.备注 as 表体备注,jhmx.领取完成,jhmx.作废,isnull(zwx.数量,0)借用转销售数量
                                    ,isnull(zs.数量,0)as 借用转客户试用数量,isnull(zhy.数量,0)as 借用转耗用数量,jhmx.销售预订单明细号   from 借还申请表 jhz  
                                     left join 借还申请表附表 jhmx on jhz.申请批号 = jhmx.申请批号
                                     left join (select  a.备注5,SUM(数量)数量 from 销售记录销售订单明细表 a
                                               left join 销售记录销售订单主表 b on a.销售订单号 =b.销售订单号 
                                               where 销售备注   like '借出转外销%'  group by  a.备注5) zwx
                                               on zwx.备注5=jhmx.申请批号明细           
                                  left join (select  a.备注5,SUM(数量)数量 from 销售记录销售订单明细表 a
                                               left join 销售记录销售订单主表 b on a.销售订单号 =b.销售订单号 
                                               where 销售备注    like '借出转赠送%' or 销售备注  like '借出转客户试用%'  group by  a.备注5) zs
                                               on zs.备注5=jhmx.申请批号明细   
                                     
                                     left join (select 物料编码,备注,SUM(数量)数量 from 其他出入库申请子表 where 出入库申请单号 in (
                                                 select 出入库申请单号 from 其他出入库申请主表 where 备注 like '借用转耗用%' ) group by 物料编码,备注 ) zhy on zhy.备注  = jhmx.申请批号明细

                                     where    jhz.申请日期>'{0}' and jhz.申请日期<'{1}'", t1, t2);
                DataTable dt_明细 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gcM.DataSource = dtM;
                gc_明细.DataSource = dt_明细;
            }
            catch (Exception ex)
            {
                //CZMaster.MasterLog.WriteLog(ex.Message, "退货申请主表_刷新操作");
                throw ex;
            }
        }
        private void fun_detail(string str_单号)
        {
            string sql = string.Format(@"select a.*,b.库存总数,b.仓库名称,b.货架描述 as 库位 from 借还申请表附表 a 
                  left join 仓库物料数量表 b on a.物料编码 = b.物料编码  and a.仓库号=b.仓库号
                where  申请批号 = '{0}'", str_单号);
            DataTable dtP = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtP);
            dtP.Columns.Add("借用转外销数量",typeof(decimal));          
            dtP.Columns.Add("借用转客户试用数量", typeof(decimal));
            dtP.Columns.Add("借用转耗用数量", typeof(decimal));
            DataTable dt_jczwx = new DataTable();
            DataTable dt_jczzs = new DataTable();
            DataTable dt_jczhy = new DataTable();
            string sss = "";
            foreach (DataRow dr in dtP.Rows)
            {
                string sql_zxs = string.Format(@"select  sum(mx.数量)数量,mx.备注5 from 销售记录销售订单明细表  mx
                                                       left join 销售记录销售订单主表 z on mx.销售订单号 = z.销售订单号
                                                       where mx.销售订单号 in(select  销售订单号 from 销售记录销售订单主表  where 销售备注   like '借出转外销%')
                                                       and mx.备注5= '{0}'  
                                                       group by mx.备注5 ", dr["申请批号明细"]);
                da = new SqlDataAdapter(sql_zxs, strconn);
                da.Fill(dt_jczwx);

                string sql_zzs = string.Format(@"select sum(mx.数量)数量,mx.备注5 from 销售记录销售订单明细表  mx
                                                       left join 销售记录销售订单主表 z on mx.销售订单号 = z.销售订单号
                                                       where mx.销售订单号 in(select  销售订单号 from 销售记录销售订单主表  where 销售备注    like '借出转赠送%' or 销售备注  like '借出转客户试用%')
                                                       and mx.备注5= '{0}' 
                                                       group by mx.备注5", dr["申请批号明细"]);
                da = new SqlDataAdapter(sql_zzs, strconn);
                da.Fill(dt_jczzs);

                string sql_zhy = string.Format(@"select * from 其他出入库申请子表 where 出入库申请单号 in (
                                                 select 出入库申请单号 from 其他出入库申请主表 where 备注 like '借用转耗用%' ) and 备注 = '{0}'", dr["申请批号明细"]);
                da = new SqlDataAdapter(sql_zhy, strconn);
                da.Fill(dt_jczhy);

                DataRow[] dr_zxs = dt_jczwx.Select(string.Format("备注5 = '{0}'  ", dr["申请批号明细"]));
                DataRow[] dr_zzs = dt_jczzs.Select(string.Format("备注5 = '{0}'  ", dr["申请批号明细"]));
                DataRow[] dr_zhy = dt_jczhy.Select(string.Format("备注 = '{0}'  and 物料编码 = '{1}' ", dr["申请批号明细"], dr["物料编码"]));

                if (dr_zxs.Length>0)
                {
                    dr["借用转外销数量"] = Convert.ToDecimal(dr_zxs[0]["数量"]);
                    
                    if (dr["借用转耗用数量"].ToString() == "")
                    {
                        dr["借用转耗用数量"] = 0;
                    }
                    if (dr["借用转客户试用数量"].ToString() == "")
                    {
                        dr["借用转客户试用数量"] = 0;
                    }
                }
                if (dr_zzs.Length> 0)
                {

                    dr["借用转客户试用数量"] = Convert.ToDecimal(dr_zzs[0]["数量"]);
                    if (dr["借用转耗用数量"].ToString()== "")
                    {
                        dr["借用转耗用数量"] = 0;
                    }
                    if (dr["借用转外销数量"].ToString() == "")
                    {
                        dr["借用转外销数量"] = 0;
                    }

                }
                if (dr_zhy.Length>0)
                {

                    dr["借用转耗用数量"] = Convert.ToDecimal(dr_zhy[0]["数量"]);
                    
                    if (dr["借用转客户试用数量"].ToString() == "")
                    {
                        dr["借用转客户试用数量"] = 0;
                    }
                    if (dr["借用转外销数量"].ToString() == "")
                    {
                        dr["借用转外销数量"] = 0;
                    }
                }   
                else if(dr_zxs.Length == 0&& dr_zzs.Length == 0&& dr_zhy.Length == 0)
                {
                    dr["借用转耗用数量"] =0;
                    dr["借用转客户试用数量"] = 0;
                    dr["借用转外销数量"] = 0;
                }
                    

                  
            }
            gcP.DataSource = dtP;

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // if (bl_刷新) throw new Exception("正在查询数据,稍候再试");
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
                    if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
                    {
                        gcM.ExportToXlsx(saveFileDialog.FileName);
                        // ERPorg.Corg.TableToExcel(dtM,saveFileDialog.FileName);
                    }
                    else
                    {
                        gc_明细.ExportToXlsx(saveFileDialog.FileName);
                        //ERPorg.Corg.TableToExcel(dt_订单明细, saveFileDialog.FileName);
                    }
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                string sql = string.Format("select * from 借还申请表 where 申请批号 = '{0}'", dr["申请批号"]);
                DataTable ttt = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (ttt.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(ttt.Rows[0]["作废"]) == true)
                    {
                        throw new Exception("该记录已作废");
                    }
                    if (Convert.ToBoolean(ttt.Rows[0]["提交审核"]) == true)
                    {
                        throw new Exception("该记录已提交审核，请撤销提交后修改");
                    }
                    if (Convert.ToBoolean(ttt.Rows[0]["审核"]) == true)
                    {
                        throw new Exception("该记录已审核不能修改");
                    }
                    else
                    {
                        frm借还申请 frm = new frm借还申请(dr);
                        CPublic.UIcontrol.Showpage(frm, "借还申请");
                    }
                }

                //if (dr["作废"].Equals(true))
                //{
                //    throw new Exception("该记录已作废");

                //}
                //if (dr["审核"].Equals(true))
                //{
                //    MessageBox.Show("该记录已审核不能修改");
                //}

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gvM_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow r = gvM.GetDataRow(gvM.FocusedRowHandle);
                fun_detail(r["申请批号"].ToString());
                if (CPublic.Var.LocalUserID == r["工号"].ToString())
                {
                    barLargeButtonItem6.Enabled = true;
                }
                else
                {
                    barLargeButtonItem6.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

        }

        private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_载入();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// bl=true 为 作废 需判断是否提交审核
        /// </summary>
        /// <param name="str_单号"></param>
        /// <param name="bl"></param>
        private void fun_check(string str_单号, bool bl)
        {
            try

            {
                string s = string.Format("select  * from 借还申请表 where 申请批号='{0}'", str_单号);
                DataTable dt_check = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                if (dt_check.Rows.Count > 1) throw new Exception("单号：" + str_单号 + "出现异常");
                if (dt_check.Rows[0]["审核"].Equals(true)) throw new Exception("该单据已审核完成，不能进行此操作！");
                if (dt_check.Rows[0]["作废"].Equals(true)) throw new Exception("该单据已作废");
                if (bl)
                {
                    if (dt_check.Rows[0]["提交审核"].Equals(true)) throw new Exception("该单据已提交审核");
                }
                else
                {

                    if (dt_check.Rows[0]["提交审核"].Equals(false)) throw new Exception("该单据尚未提交审核,不可撤销提交");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }



        }
        private void barLargeButtonItem4_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认作废吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    if (dr == null)
                        throw new Exception("请先选择需要作废的记录");

                    fun_check(dr["申请批号"].ToString(), true);

                    dr["作废"] = true;
                    dr["作废日期"] = CPublic.Var.getDatetime();
                    dr["作废人员ID"] = CPublic.Var.LocalUserID;
                    dr["作废人员"] = CPublic.Var.LocalUserID;
                    string sql = string.Format("select * from 借还申请表附表 where 申请批号='{0}'", dr["申请批号"]);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    foreach (DataRow r in dt.Rows)
                    {
                        r["作废"] = true;
                    }
                    sql = string.Format("select * from  单据审核申请表 where 关联单号 = '{0}'", dr["申请批号"]);
                    DataTable dt_审核 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    if (dt_审核.Rows.Count > 0)
                    {
                        dt_审核.Rows[0]["作废"] = 1;
                    }
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("借用作废");
                    try
                    {
                        sql = "select * from 借还申请表附表 where 1<> 1";
                        SqlCommand cmd = new SqlCommand(sql, conn, ts);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt);
                        sql = "select * from 借还申请表 where 1<> 1";
                        cmd = new SqlCommand(sql, conn, ts);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dtM);
                        sql = "select * from 单据审核申请表 where 1<> 1";
                        cmd = new SqlCommand(sql, conn, ts);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_审核);
                        ts.Commit();
                    }

                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw new Exception(ex.Message);
                    }
                    MessageBox.Show("已作废:" + dr["申请批号"].ToString());
                    fun_载入();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void 撤销提交ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                fun_check(dr["申请批号"].ToString(), false);
                string s = string.Format("select * from 单据审核申请表 where 关联单号='{0}'", dr["申请批号"].ToString());
                DataTable dt_审核申请 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                if (dt_审核申请.Rows.Count > 0)
                {
                    dt_审核申请.Rows[0].Delete();
                }

                s = string.Format("select * from 借还申请表附表 where 申请批号 = '{0}'",dr["申请批号"].ToString());

                DataTable dtP = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                string sql = "select * from  销售预订单明细表";
                DataTable dt_ymx = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                sql = "select * from 销售预订单主表";
                DataTable dt_yz = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                foreach (DataRow dr1 in dtP.Rows)
                {
                    if (dr1["销售预订单明细号"].ToString() != "")
                    {
                        DataRow[] dr_ymx = dt_ymx.Select(string.Format("销售预订单明细号 = '{0}'", dr1["销售预订单明细号"]));
                        dr_ymx[0]["转换订单数量"] = Convert.ToDecimal(dr_ymx[0]["转换订单数量"]) - Convert.ToDecimal(dr1["申请数量"]);
                        dr_ymx[0]["未转数量"] = Convert.ToDecimal(dr_ymx[0]["未转数量"]) + Convert.ToDecimal(dr1["申请数量"]);
                        dr_ymx[0]["完成"] = false;
                        DataRow[] dr_yz = dt_yz.Select(string.Format("销售预订单号 = '{0}'", dr1["销售预订单号"]));
                        dr_yz[0]["完成"] = false;
                    }
                }
                //s = string.Format("select  * from 单据审核日志表 where 审核申请单号='{0}'", dr["申请批号"].ToString());
                //DataTable dt_history = CZMaster.MasterSQL.Get_DataTable(s, strconn);

                //int count = dt_history.Rows.Count;
                //for (int i = count - 1; i >= 0; i--)
                //{
                //    dt_history.Rows[i].Delete();
                //}
                dr["提交审核"] = false;

                SqlConnection conn = new SqlConnection(strconn);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("借用撤销提交");
                try
                {
                    s = "select * from 单据审核申请表 where 1<> 1";
                    SqlCommand cmd = new SqlCommand(s, conn, ts);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_审核申请);
                    //s = "select * from 单据审核日志表 where 1<> 1";
                    //cmd = new SqlCommand(s, conn, ts);
                    //da = new SqlDataAdapter(cmd);
                    //new SqlCommandBuilder(da);
                    //da.Update(dt_history);
                    s = "select * from 借还申请表 where 1<> 1";
                    cmd = new SqlCommand(s, conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dtM);

                    s = "select * from 销售预订单主表 where 1<> 1";
                    cmd = new SqlCommand(s, conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_yz);

                    s = "select * from 销售预订单明细表 where 1<> 1";
                    cmd = new SqlCommand(s, conn, ts);
                    da = new SqlDataAdapter(cmd);
                    new SqlCommandBuilder(da);
                    da.Update(dt_ymx);

                    ts.Commit();
                    MessageBox.Show("撤销成功");
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    throw new Exception(ex.Message);
                }
                fun_载入();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //右击
        private void gcM_MouseClick(object sender, MouseEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gcM, new Point(e.X, e.Y));
                 
            }
        }

        private void 归还申请ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try

            {
                DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                DataTable dt_mx = (DataTable)this.gcP.DataSource;
                if (bool.Parse(drM["作废"].ToString()) == true)
                {
                    throw new Exception("当前单据已作废");

                }
                DataView dv = new DataView(dt_mx);
                dv.RowFilter = "已借出数量>归还数量+正在申请数";
                if (dv.Count == 0) throw new Exception("当前单据无可归还明细");
                ui归还申请流程 fm = new ui归还申请流程(drM, dt_mx);
                //fm.Dock = System.Windows.Forms.DockStyle.Fill;
                CPublic.UIcontrol.AddNewPage(fm, "归还申请");
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
                DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                string sql = string.Format("select * from 借还申请表 where 申请批号 = '{0}' ",drM["申请批号"]);
                DataTable dt11 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                if (dt11.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(dt11.Rows[0]["锁定"]))
                    {
                        throw new Exception("该单据已锁定，不可打印");
                    }
                }
                if (bool.Parse(drM["审核"].ToString()) == false)
                {
                    throw new Exception("当前单据未审核不可打印");

                }
                sql = string.Format(@"select a.*,b.库存总数,b.仓库名称,b.货架描述 as 库位 from 借还申请表附表 a 
                  left join 仓库物料数量表 b on a.物料编码 = b.物料编码  and a.仓库号=b.仓库号
                where  申请批号 = '{0}'",drM["申请批号"]);
                DataTable dtm = CZMaster.MasterSQL.Get_DataTable(sql,strconn);
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.借用申请", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统
                                                                            // CPublic.UIcontrol.Showpage(ui, t.Rows[0]["打开界面名称"].ToString());
                object[] drr = new object[2];

                drr[0] = drM;
                drr[1] = dtm;
                //   drr[2] = dr["出入库申请单号"].ToString();
                Form ui = Activator.CreateInstance(outerForm, drr) as Form;
                //  UserControl ui = Activator.CreateInstance(outerForm) as UserControl;
                ui.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /*
        //private DataSet fun_归还(string ss, DataRow dr_借还)
        //{
        //    DataSet ds = new DataSet();
        //    DateTime t = CPublic.Var.getDatetime();
        //    string sql = string.Format(@"select b.*,a.图纸编号,a.计量单位,仓库物料数量表.库存总数 from 借还申请表附表 b
        //         left join 基础数据物料信息表 a on a.物料编码 = b.物料编码
        //         left join 仓库物料数量表 on 仓库物料数量表.物料编码 = b.物料编码 and 仓库物料数量表.仓库号 = b.仓库号
        //         where 申请批号 = '{0}' and 归还完成 = 0 ", dr_借还["申请批号"]);
        //    dt_借还申请表附表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

        //    sql = string.Format("select * from 借还申请表 where 申请批号 = '{0}' ", dr_借还["申请批号"]);
        //    dt_借还申请表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

        //    sql = string.Format("select * from 归还申请主表 where 申请批号 = '{0}' ", dr_借还["申请批号"]);
        //    dt_归还申请主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

        //    sql = string.Format("select * from 归还申请子表 where 申请批号 = '{0}' ", dr_借还["申请批号"]);
        //    dt_归还申请子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

        //    DataTable dt_归还表;
        //    //   DataTable dt_归还关联表;
        //    DataTable dt_仓库出入库明细;
        //    string sql_归还 = "select * from 借还申请表归还记录 where 1<>1";
        //    dt_归还表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);
        //    //sql_归还 = "select * from 借还申请批量归还关联 where 1<>1";
        //    //dt_归还关联表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);

        //    sql_归还 = "select * from 仓库出入库明细表 where 1<>1";
        //    dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);

        //    string s_归还单号 = string.Format("RA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"), t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RA", t.Year, t.Month).ToString("0000"));
        //    //string s_归还单号111 = string.Format("RA{0}",CPublic.CNo.fun_得到最大流水号("RA").ToString("0000"));
        //    int i = 1;
        //    int j = 1;
        //    string s_归还申请单号 = string.Format("GH{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day,
        //            CPublic.CNo.fun_得到最大流水号("GH", t.Year, t.Month));
        //    if (dt_借还申请表.Rows.Count > 0)
        //    {
        //        if(dt_归还申请主.Rows.Count == 0)
        //        {
        //            DataRow dr_申请主 = dt_归还申请主.NewRow();
        //            dt_归还申请主.Rows.Add(dr_申请主);
        //            dr_申请主["归还批号"] = s_归还申请单号;
        //            dr_申请主["申请批号"] = dr_借还["申请批号"];
        //            dr_申请主["归还操作人"] = CPublic.Var.localUserName;
        //            dr_申请主["备注"] = dr_借还["备注"];
        //            dr_申请主["归还申请日期"] = t;
        //            dr_申请主["归还完成"] = true;
        //            dr_申请主["原因分类"] = dr_借还["原因分类"];
        //            dr_申请主["借用类型"] = dr_借还["借用类型"];
        //            dr_申请主["归还日期"] = t;

        //        }
        //        else
        //        {
        //            dt_归还申请主.Rows[0]["归还申请日期"] = t;
        //            dt_归还申请主.Rows[0]["归还完成"] = true;
        //        }
        //        DataTable ttttt = new DataTable();
        //        foreach (DataRow dr in dt_借还申请表附表.Rows)
        //        {
        //            //dt_借还申请表附表 只显示未归还记录
        //         //   DataRow[] ds_申请子 = dt_归还申请子.Select("申请批号明细 = '{0}'",dr["申请批号明细"].ToString());
        //            string sqlll =  string.Format("select * from  归还申请子表 where 申请批号明细 = '{0}'", dr["申请批号明细"].ToString());
        //            ttttt = CZMaster.MasterSQL.Get_DataTable(sqlll, strconn);
        //            if (ttttt.Rows.Count == 0)
        //            {
        //                DataRow dr_申请子 = dt_归还申请子.NewRow();
        //                dt_归还申请子.Rows.Add(dr_申请子);

        //                dr_申请子["POS"] = j;
        //                dr_申请子["归还批号"] = s_归还申请单号;
        //                dr_申请子["归还明细号"] = s_归还申请单号 + "-" + j++.ToString("00");
        //                dr_申请子["申请批号"] = dr["申请批号"];
        //                dr_申请子["申请批号明细"] = dr["申请批号明细"];
        //                dr_申请子["物料编码"] = dr["物料编码"];
        //                dr_申请子["物料名称"] = dr["物料名称"];
        //                dr_申请子["规格型号"] = dr["规格型号"];
        //                dr_申请子["仓库名称"] = dr["仓库名称"];
        //                dr_申请子["仓库号"] = dr["仓库号"];
        //                dr_申请子["货架描述"] = dr["货架描述"];
        //                dr_申请子["归还日期"] =t;
        //                dr_申请子["需归还数量"] = Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["归还数量"]); ;
        //                dr_申请子["借用数量"] = dr["申请数量"];
        //                dr_申请子["已归还数量"] = dr["申请数量"];
        //                dr_申请子["归还完成"] = true;
        //                dr_申请子["录入归还数量"] = dr["申请数量"];
        //                dr_申请子["申请已归还数量"] = dr["申请数量"];
        //            }
        //            else
        //            {
        //                DataRow[] ds_申请子 = dt_归还申请子.Select("申请批号明细 = '{0}'", dr["申请批号明细"].ToString());
        //                ds_申请子[0]["申请批号"] = dr["申请批号"];
        //                ds_申请子[0]["申请批号明细"] = dr["申请批号明细"];
        //                ds_申请子[0]["物料编码"] = dr["物料编码"];
        //                ds_申请子[0]["物料名称"] = dr["物料名称"];
        //                ds_申请子[0]["规格型号"] = dr["规格型号"];
        //                ds_申请子[0]["仓库名称"] = dr["仓库名称"];
        //                ds_申请子[0]["仓库号"] = dr["仓库号"];
        //                ds_申请子[0]["货架描述"] = dr["货架描述"];
        //                ds_申请子[0]["归还日期"] = t;
        //                ds_申请子[0]["需归还数量"] = Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["归还数量"]); ;
        //                ds_申请子[0]["借用数量"] = dr["申请数量"];
        //                ds_申请子[0]["已归还数量"] = dr["申请数量"];
        //                ds_申请子[0]["归还完成"] = true;
        //                ds_申请子[0]["录入归还数量"] = dr["申请数量"];
        //                ds_申请子[0]["申请已归还数量"] = dr["申请数量"];
        //            }
        //            DataRow dr_归还 = dt_归还表.NewRow();
        //            dt_归还表.Rows.Add(dr_归还);
        //            dr_归还["guid"] = System.Guid.NewGuid();
        //            dr_归还["申请批号"] = s_归还单号;
        //            dr_归还["申请批号明细"] = s_归还单号 + "-" + i++.ToString("00");
        //            dr_归还["借用申请明细号"] = dr["申请批号明细"];
        //            dr_归还["计量单位"] = dr["计量单位"];
        //            dr_归还["计量单位编码"] = dr["计量单位编码"];

        //            dr_归还["物料编码"] = dr["物料编码"];
        //            dr_归还["物料名称"] = dr["物料名称"];
        //            dr_归还["规格型号"] = dr["规格型号"];
        //            dr_归还["仓库号"] = dr["仓库号"];
        //            dr_归还["仓库名称"] = dr["仓库名称"];
        //            dr_归还["备注"] = ss + "自动生成记录";
        //            decimal dec = Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["归还数量"]);
        //            dr_归还["归还数量"] = dec;
        //            dr_归还["归还日期"] = t;
        //            dr_归还["货架描述"] = dr["货架描述"];
        //            dr_归还["归还操作人"] = CPublic.Var.localUserName;

        //            dr["归还日期"] = t;
        //            dr["归还完成"] = 1;
        //            dr["借还状态"] = "已归还";

        //            dr["归还数量"] = dr["申请数量"];


        //            DataRow dr_仓库出入库明细 = dt_仓库出入库明细.NewRow();
        //            dt_仓库出入库明细.Rows.Add(dr_仓库出入库明细);
        //            dr_仓库出入库明细["GUID"] = System.Guid.NewGuid();
        //            dr_仓库出入库明细["明细类型"] = "归还入库";
        //            dr_仓库出入库明细["单号"] = s_归还单号;
        //            dr_仓库出入库明细["物料编码"] = dr["物料编码"];
        //            dr_仓库出入库明细["物料名称"] = dr["物料名称"];
        //            dr_仓库出入库明细["明细号"] = dr_归还["申请批号明细"];
        //            dr_仓库出入库明细["出库入库"] = "入库";
        //            dr_仓库出入库明细["实效数量"] = dec;
        //            dr_仓库出入库明细["实效时间"] = t;
        //            dr_仓库出入库明细["出入库时间"] = t;
        //            dr_仓库出入库明细["相关单号"] = dr_归还["借用申请明细号"];
        //            dr_仓库出入库明细["相关单位"] = dr_借还["相关单位"];
        //            dr_仓库出入库明细["仓库号"] = dr["仓库号"];
        //            dr_仓库出入库明细["仓库名称"] = dr["仓库名称"];
        //            dr_仓库出入库明细["单位"] = dr["计量单位"];
        //            //DataRow dr_归还关联 = dt_归还关联表.NewRow();
        //            //dt_归还关联表.Rows.Add(dr_归还关联);
        //            //dr_归还关联["关联批号"] = dr_借还["申请批号"];
        //            //dr_归还关联["归还批号"] = s_归还单号;
        //            //ds.Tables.Add(dt_归还关联表);
                    
        //        }
        //        dt_借还申请表.Rows[0]["归还"] = true;
        //        dt_借还申请表.Rows[0]["归还日期"] = t;
        //        dt_借还申请表.Rows[0]["手动归还原因"] = ss;
        //    }
        //    dr_借还["归还日期"] = t;
        //    dr_借还["归还"] = true;
        //    //dr_借还["借还状态"] = "已归还";
        //    dr_借还["手动归还原因"] = ss;

        //    ds.Tables.Add(dt_归还表);
        //    ds.Tables.Add(dt_仓库出入库明细);

        //    return ds;
        //}
        */
        /// <summary>
        /// 2019-8-27  修改
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="dr_借还"></param>
        /// <param name="ds_销售">    </param>
        /// <returns></returns>
        private DataSet fun_归还(string ss, DataRow dr_借还, DataSet ds_销售)
        {
            DataSet ds = new DataSet();

            //销售主表
            //通知单主表
            //出库单主表
            //dt_销售附表;
            //dt_出库通知单明细表;
            //dt_成品出库单明细表;
            //dt_仓库出入库明细;



            DateTime t = CPublic.Var.getDatetime();
            //DateTime t = Convert.ToDateTime("2020-3-31 09:46:19.353");

            string sql = string.Format(@"select  *  from 借还申请表附表  where 申请批号 = '{0}'", dr_借还["申请批号"]);
            dt_借还申请表附表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);



            sql = string.Format("select * from 借还申请表 where 申请批号 = '{0}' ", dr_借还["申请批号"]);
            dt_借还申请表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);


            sql = string.Format("select * from 归还申请主表 where 申请批号 = '{0}' ", dr_借还["申请批号"]);
            dt_归还申请主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            sql = string.Format("select * from 归还申请子表 where 申请批号 = '{0}' ", dr_借还["申请批号"]);
            dt_归还申请子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            DataTable dt_归还表;
            //   DataTable dt_归还关联表;
            DataTable dt_仓库出入库明细 = ds_销售.Tables[6].Copy();
            string sql_归还 = "select * from 借还申请表归还记录 where 1<>1";
            dt_归还表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);






            string s_归还单号 = string.Format("RA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"), t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RA", t.Year, t.Month).ToString("0000"));
            int i = 1;
            int j = 1;
            string s_归还申请单号 = string.Format("GH{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                    CPublic.CNo.fun_得到最大流水号("GH", t.Year, t.Month));
            DataRow dr_申请主 = dt_归还申请主.NewRow();
            dt_归还申请主.Rows.Add(dr_申请主);
            dr_申请主["归还批号"] = s_归还申请单号;
            dr_申请主["申请批号"] = dr_借还["申请批号"];
            dr_申请主["归还操作人"] = CPublic.Var.localUserName;
            dr_申请主["备注"] = dr_借还["备注"];
            dr_申请主["归还申请日期"] = t;
            dr_申请主["归还完成"] = true;
            dr_申请主["原因分类"] = dr_借还["原因分类"];
            dr_申请主["借用类型"] = dr_借还["借用类型"];
            dr_申请主["归还日期"] = t;
            dr_申请主["归还申请日期"] = t;
            dr_申请主["归还完成"] = true;
            dr_申请主["归还方式"] = ss;

            foreach (DataRow dr in ds_销售.Tables[3].Rows)
            {
                #region 判断 借用单和明细的完成状态
                if (dr.RowState == DataRowState.Deleted) continue;
                DataRow[] x = dt_借还申请表附表.Select(string.Format("申请批号明细='{0}'", dr["备注5"]));
                //这里x.lenth >0  恒成立 
                x[0]["归还数量"] = Convert.ToDecimal(x[0]["归还数量"]) + Convert.ToDecimal(dr["数量"]);
                if (Convert.ToDecimal(x[0]["申请数量"]) == Convert.ToDecimal(x[0]["归还数量"]))
                {
                    x[0]["归还完成"] = true;
                    x[0]["归还日期"] = t;
                    x[0]["借还状态"] = "已归还";

                }
                #endregion

                #region  归还申请子表添加记录 
                DataRow dr_申请子 = dt_归还申请子.NewRow();
                dt_归还申请子.Rows.Add(dr_申请子);

                dr_申请子["POS"] = j;
                dr_申请子["归还批号"] = s_归还申请单号;
                dr_申请子["归还明细号"] = s_归还申请单号 + "-" + j++.ToString("00");
                dr_申请子["申请批号"] = dr["备注5"].ToString().Split('-')[0];
                dr_申请子["申请批号明细"] = dr["备注5"];
                dr_申请子["物料编码"] = dr["物料编码"];
                dr_申请子["物料名称"] = dr["物料名称"];
                dr_申请子["规格型号"] = dr["规格型号"];
                dr_申请子["仓库名称"] = dr["仓库名称"];
                dr_申请子["仓库号"] = dr["仓库号"];
                // dr_申请子["货架描述"] = dr["货架描述"];
                dr_申请子["归还日期"] = t;
                dr_申请子["需归还数量"] = dr["数量"];

                dr_申请子["借用数量"] = x[0]["申请数量"];
                //dr_申请子["已归还数量"] = dr["申请数量"];   // 这里经验证 孙杰这个数量  是对应的借用的累计归还数量 无用
                dr_申请子["归还完成"] = true;
                // dr_申请子["录入归还数量"] = dr["申请数量"]; // 这里是界面录入的数量 正常归还申请界面录入的数量 无用
                dr_申请子["申请已归还数量"] = dr["数量"];
                #endregion

                DataRow dr_归还 = dt_归还表.NewRow();
                dt_归还表.Rows.Add(dr_归还);
                dr_归还["guid"] = System.Guid.NewGuid();
                dr_归还["申请批号"] = s_归还单号;
                dr_归还["申请批号明细"] = s_归还单号 + "-" + i++.ToString("00");
                dr_归还["借用申请明细号"] = dr["备注5"];
                dr_归还["计量单位"] = dr["计量单位"];
                dr_归还["计量单位编码"] = dr["计量单位编码"];

                dr_归还["物料编码"] = dr["物料编码"];
                dr_归还["物料名称"] = dr["物料名称"];
                dr_归还["规格型号"] = dr["规格型号"];
                dr_归还["仓库号"] = dr["仓库号"];
                dr_归还["仓库名称"] = dr["仓库名称"];
                dr_归还["备注"] = ss + "自动生成记录";
                // decimal dec = Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["归还数量"]);
                dr_归还["归还数量"] = dr["数量"];
                dr_归还["归还日期"] = t;
                //  dr_归还["货架描述"] = dr["货架描述"];
                dr_归还["归还操作人"] = CPublic.Var.localUserName;

                DataRow dr_仓库出入库明细 = dt_仓库出入库明细.NewRow();
                dt_仓库出入库明细.Rows.Add(dr_仓库出入库明细);
                dr_仓库出入库明细["GUID"] = System.Guid.NewGuid();
                dr_仓库出入库明细["明细类型"] = "归还入库";
                dr_仓库出入库明细["单号"] = s_归还单号;
                dr_仓库出入库明细["物料编码"] = dr["物料编码"];
                dr_仓库出入库明细["物料名称"] = dr["物料名称"];
                dr_仓库出入库明细["明细号"] = dr_归还["申请批号明细"];
                dr_仓库出入库明细["出库入库"] = "入库";
                dr_仓库出入库明细["实效数量"] = dr["数量"];
                dr_仓库出入库明细["实效时间"] = t;
                dr_仓库出入库明细["出入库时间"] = t;
                dr_仓库出入库明细["相关单号"] = dr_归还["借用申请明细号"];
                dr_仓库出入库明细["相关单位"] = dr_借还["相关单位"];
                dr_仓库出入库明细["仓库号"] = dr["仓库号"];
                dr_仓库出入库明细["仓库名称"] = dr["仓库名称"];
                dr_仓库出入库明细["单位"] = dr["计量单位"];

            }
            DataView dv = new DataView(dt_借还申请表附表);
            dv.RowFilter = "归还完成=0";
            if (dv.Count == 0)
            {
                dt_借还申请表.Rows[0]["归还"] = 1;
                dt_借还申请表.Rows[0]["归还日期"] = t;

            }




            ds.Tables.Add(dt_归还表);
            ds.Tables.Add(dt_仓库出入库明细);

            return ds;
        }
        private DataSet fun_赠送(DataTable dt_归还记录, DataTable dt_仓库出入库明细)
        {
            DataSet ds = new DataSet();
            DateTime t = CPublic.Var.getDatetime();

            DataTable dt_销售订单主表;
            DataTable dt_销售订单明细表;
            DataTable dt_出库通知单主表;
            DataTable dt_出库通知单明细表;
            DataTable dt_成品出库单主表;
            DataTable dt_成品出库单明细表;
            DataTable dt_客户;
            string s = "select * from 销售记录销售订单主表 where 1<>1";
            dt_销售订单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录销售订单明细表 where 1<>1";
            dt_销售订单明细表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录销售出库通知单主表 where 1<>1";
            dt_出库通知单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录销售出库通知单明细表 where 1<>1";
            dt_出库通知单明细表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = string.Format("select * from 客户基础信息表 where 客户名称 = '{0}'", dr_借还["相关单位"]);
            dt_客户 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录成品出库单主表 where 1<>1";
            dt_成品出库单主表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            s = "select * from 销售记录成品出库单明细表 where 1<>1";
            dt_成品出库单明细表 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
            //s = "select * from 仓库出入库明细表 where 1<>1";
            //dt_仓库出入库明细 = CZMaster.MasterSQL.Get_DataTable(s, strconn);


            string s_销售单号 = string.Format("SO{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
          t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SO", t.Year, t.Month).ToString("0000"));
            string s_出库通知单号 = string.Format("SK{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
          t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SK", t.Year, t.Month).ToString("0000"));
            string s_成品出库单号 = string.Format("SA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"),
          t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("SA", t.Year, t.Month).ToString("0000"));

            DataRow dr_销售订单主 = dt_销售订单主表.NewRow();
            dt_销售订单主表.Rows.Add(dr_销售订单主);
            dr_销售订单主["GUID"] = System.Guid.NewGuid();
            dr_销售订单主["销售订单号"] = s_销售单号;
            dr_销售订单主["录入人员"] = CPublic.Var.localUserName;
            dr_销售订单主["录入人员ID"] = CPublic.Var.LocalUserID;
            dr_销售订单主["待审核"] = true;
            dr_销售订单主["审核"] = true;
            dr_销售订单主["备注1"] = dt_仓库出入库明细.Rows[0]["相关单号"].ToString().Split('-')[0]; //记录借用申请单号


            if (dt_客户.Rows.Count > 0)
            {
                dr_销售订单主["客户编号"] = dt_客户.Rows[0]["客户编号"];
                dr_销售订单主["客户名"] = dr_借还["相关单位"];
                dr_销售订单主["税率"] = dt_客户.Rows[0]["税率"];
                dr_销售订单主["业务员"] = dt_客户.Rows[0]["业务员"];
                //dr_销售订单主["客户名"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_销售订单主["日期"] = t;
            dr_销售订单主["销售备注"] = "借出转赠送";

            dr_销售订单主["税前金额"] = 0;
            dr_销售订单主["税后金额"] = 0;
            dr_销售订单主["生效"] = true;
            dr_销售订单主["生效日期"] = t;
            dr_销售订单主["生效人员"] = CPublic.Var.localUserName;
            dr_销售订单主["生效人员ID"] = CPublic.Var.LocalUserID;

            dr_销售订单主["创建日期"] = t;
            dr_销售订单主["修改日期"] = t;
            dr_销售订单主["完成"] = true;
            dr_销售订单主["完成日期"] = t;
            ds.Tables.Add(dt_销售订单主表);

            DataRow dr_出库通知单主 = dt_出库通知单主表.NewRow();
            dt_出库通知单主表.Rows.Add(dr_出库通知单主);
            dr_出库通知单主["GUID"] = System.Guid.NewGuid();
            dr_出库通知单主["出库通知单号"] = s_出库通知单号;
            if (dt_客户.Rows.Count > 0)
            {
                dr_出库通知单主["客户编号"] = dt_客户.Rows[0]["客户编号"];
                dr_出库通知单主["客户名"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_出库通知单主["出库日期"] = t;
            dr_出库通知单主["创建日期"] = t;
            dr_出库通知单主["修改日期"] = t;
            dr_出库通知单主["操作员ID"] = CPublic.Var.LocalUserID;
            dr_出库通知单主["操作员"] = CPublic.Var.localUserName;
            dr_出库通知单主["生效"] = true;
            dr_出库通知单主["生效日期"] = t;
            ds.Tables.Add(dt_出库通知单主表);

            DataRow dr_成品出库主 = dt_成品出库单主表.NewRow();
            dt_成品出库单主表.Rows.Add(dr_成品出库主);
            dr_成品出库主["GUID"] = System.Guid.NewGuid();
            dr_成品出库主["成品出库单号"] = s_成品出库单号;
            dr_成品出库主["操作员ID"] = CPublic.Var.LocalUserID;
            dr_成品出库主["操作员"] = CPublic.Var.localUserName;
            if (dt_客户.Rows.Count > 0)
            {
                dr_成品出库主["客户"] = dt_客户.Rows[0]["客户名称"];
            }
            dr_成品出库主["日期"] = t;
            dr_成品出库主["创建日期"] = t;
            dr_成品出库主["修改日期"] = t;
            dr_成品出库主["生效"] = true;
            dr_成品出库主["生效日期"] = t;
            ds.Tables.Add(dt_成品出库单主表);

            int i = 1;
            foreach (DataRow dr in dt_归还记录.Rows)
            {
                DataRow dr_saleDetail = dt_销售订单明细表.NewRow();
                dt_销售订单明细表.Rows.Add(dr_saleDetail);
                dr_saleDetail["GUID"] = System.Guid.NewGuid();
                dr_saleDetail["销售订单号"] = s_销售单号;
                dr_saleDetail["POS"] = i;
                dr_saleDetail["销售订单明细号"] = s_销售单号 + "-" + i.ToString("00");
                dr_saleDetail["物料编码"] = dr["物料编码"];
                dr_saleDetail["数量"] = dr["归还数量"];
                dr_saleDetail["完成数量"] = dr["归还数量"];
                dr_saleDetail["未完成数量"] = 0;
                dr_saleDetail["已通知数量"] = dr["归还数量"];
                dr_saleDetail["未通知数量"] = 0;
                dr_saleDetail["物料名称"] = dr["物料名称"];
                //dr_销售订单子["n原ERP规格型号"] = dr["n原ERP规格型号"];
                dr_saleDetail["规格型号"] = dr["规格型号"];
                // dr_销售订单子["图纸编号"] = dr["图纸编号"];
                dr_saleDetail["仓库号"] = dr["仓库号"];
                dr_saleDetail["仓库名称"] = dr["仓库名称"];
                dr_saleDetail["计量单位"] = dr["计量单位"];
                // dr_saleDetail["销售备注"] = "借出转赠送";
                dr_saleDetail["税前金额"] = 0;
                dr_saleDetail["税后金额"] = 0;
                dr_saleDetail["税前单价"] = 0;
                dr_saleDetail["税后单价"] = 0;
                dr_saleDetail["送达日期"] = t;
                if (dt_客户.Rows.Count > 0)
                {
                    dr_saleDetail["客户编号"] = dt_客户.Rows[0]["客户编号"];
                    dr_saleDetail["客户"] = dt_客户.Rows[0]["客户名称"];
                }
                dr_saleDetail["生效"] = true;
                dr_saleDetail["生效日期"] = t;
                dr_saleDetail["明细完成"] = true;
                dr_saleDetail["明细完成日期"] = t;
                dr_saleDetail["总完成"] = true;
                dr_saleDetail["总完成日期"] = t;
                dr_saleDetail["已计算"] = true;
                dr_saleDetail["录入人员ID"] = CPublic.Var.LocalUserID;
                dr_saleDetail["含税销售价"] = 0;

                DataRow dr_stockOutNotice = dt_出库通知单明细表.NewRow();
                dt_出库通知单明细表.Rows.Add(dr_stockOutNotice);
                dr_stockOutNotice["GUID"] = System.Guid.NewGuid();
                dr_stockOutNotice["出库通知单号"] = s_出库通知单号;
                dr_stockOutNotice["POS"] = i;
                dr_stockOutNotice["出库通知单明细号"] = s_出库通知单号 + "-" + i.ToString("00");
                dr_stockOutNotice["销售订单明细号"] = dr_saleDetail["销售订单明细号"];
                dr_stockOutNotice["物料编码"] = dr["物料编码"];
                dr_stockOutNotice["物料名称"] = dr["物料名称"];
                dr_stockOutNotice["出库数量"] = dr["归还数量"];
                dr_stockOutNotice["规格型号"] = dr["规格型号"];
                //dr_stockOutNotice["图纸编号"] = dr["图纸编号"];
                dr_stockOutNotice["操作员ID"] = CPublic.Var.LocalUserID;
                dr_stockOutNotice["操作员"] = CPublic.Var.localUserName;
                dr_stockOutNotice["生效"] = true;
                dr_stockOutNotice["生效日期"] = t;
                dr_stockOutNotice["完成"] = true;
                dr_stockOutNotice["完成日期"] = t;
                dr_stockOutNotice["计量单位"] = dr["计量单位"];
                dr_stockOutNotice["销售备注"] = "借出转赠送";


                if (dt_客户.Rows.Count > 0)
                {
                    dr_stockOutNotice["客户"] = dt_客户.Rows[0]["客户名称"];
                    dr_stockOutNotice["客户编号"] = dt_客户.Rows[0]["客户编号"];
                }
                dr_stockOutNotice["已出库数量"] = dr["归还数量"];
                dr_stockOutNotice["未出库数量"] = 0;
                //dr_出库通知单明细["n原ERP规格型号"] = dr["n原ERP规格型号"];

                DataRow dr_stockOutDetaail = dt_成品出库单明细表.NewRow();
                dt_成品出库单明细表.Rows.Add(dr_stockOutDetaail);
                dr_stockOutDetaail["GUID"] = System.Guid.NewGuid();
                dr_stockOutDetaail["成品出库单号"] = s_成品出库单号;
                dr_stockOutDetaail["POS"] = i;
                dr_stockOutDetaail["成品出库单明细号"] = s_成品出库单号 + "-" + i++.ToString("00");
                dr_stockOutDetaail["销售订单号"] = s_销售单号;
                dr_stockOutDetaail["销售订单明细号"] = dr_saleDetail["销售订单明细号"];
                dr_stockOutDetaail["出库通知单号"] = s_出库通知单号;
                dr_stockOutDetaail["出库通知单明细号"] = dr_stockOutNotice["出库通知单明细号"];
                dr_stockOutDetaail["物料编码"] = dr["物料编码"];
                dr_stockOutDetaail["物料名称"] = dr["物料名称"];
                dr_stockOutDetaail["出库数量"] = dr["归还数量"];
                dr_stockOutDetaail["已出库数量"] = dr["归还数量"];
                dr_stockOutDetaail["未开票数量"] = dr["归还数量"];
                dr_stockOutDetaail["规格型号"] = dr["规格型号"];
                dr_stockOutNotice["计量单位"] = dr["计量单位"];
                dr_stockOutNotice["销售备注"] = "借出转赠送";
                //dr_stockOutDetaail["图纸编号"] = dr["图纸编号"];
                if (dt_客户.Rows.Count > 0)
                {
                    dr_stockOutDetaail["客户"] = dt_客户.Rows[0]["客户名称"];
                    dr_stockOutDetaail["客户编号"] = dt_客户.Rows[0]["客户编号"];
                }
                dr_stockOutDetaail["仓库号"] = dr["仓库号"];
                dr_stockOutDetaail["仓库名称"] = dr["仓库名称"];
                dr_stockOutDetaail["生效"] = true;
                dr_stockOutDetaail["生效日期"] = t;
                //dr_成品出库明细["n原ERP规格型号"] = dr["n原ERP规格型号"];

                DataRow dr_stockcrmx = dt_仓库出入库明细.NewRow();
                dt_仓库出入库明细.Rows.Add(dr_stockcrmx);
                dr_stockcrmx["GUID"] = System.Guid.NewGuid();
                dr_stockcrmx["明细类型"] = "销售出库";
                dr_stockcrmx["单号"] = s_成品出库单号;
                dr_stockcrmx["物料编码"] = dr["物料编码"];
                dr_stockcrmx["物料名称"] = dr["物料名称"];
                dr_stockcrmx["明细号"] = dr_stockOutDetaail["成品出库单明细号"];
                dr_stockcrmx["出库入库"] = "出库";
                dr_stockcrmx["实效数量"] = "-" + dr["归还数量"];
                dr_stockcrmx["实效时间"] = t;
                dr_stockcrmx["出入库时间"] = t;
                dr_stockcrmx["相关单号"] = dr_saleDetail["销售订单明细号"];
                dr_stockcrmx["仓库号"] = dr["仓库号"];
                dr_stockcrmx["仓库名称"] = dr["仓库名称"];
                dr_stockcrmx["相关单位"] = dr_借还["相关单位"];
                dr_stockcrmx["单位"] = dr["计量单位"];


            }
            ds.Tables.Add(dt_销售订单明细表);
            ds.Tables.Add(dt_出库通知单明细表);
            ds.Tables.Add(dt_成品出库单明细表);
            //ds.Tables.Add(dt_仓库出入库明细);               

            return ds;
        }

        private void 查询信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try

            {
                DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;

                frm借还申请 fm = new frm借还申请(drM, true);
                //fm.Dock = System.Windows.Forms.DockStyle.Fill;
                CPublic.UIcontrol.AddNewPage(fm, "借还信息");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 归还转外销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try

            {
                DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                DataTable dt_mx = (DataTable)this.gcP.DataSource;
                if (bool.Parse(drM["作废"].ToString()) == true)
                {
                    throw new Exception("当前单据已作废");

                }
                if (Convert.ToBoolean(drM["归还"]) == true)
                {
                    throw new Exception("当前单据已归还，无法转外销");
                }
                DataView dv = new DataView(dt_mx);
                dv.RowFilter = "已借出数量>归还数量+正在申请数";
                if (dv.Count == 0) throw new Exception("当前单据无可归还明细");




                dr_借还 = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                //返回ds.tables[0]归还记录明细，ds.tables[1]仓库出入库明细,ds.tables[2]归还关联
                //保存ds_借还,dt_借用申请表，dt_借用申请表附表
                //DataSet ds_借还 = fun_归还("借用转销售", dr_借还);

                DataSet ds_外销 = new DataSet();
                //MoldMangement.fm_归还转外销 fm = new fm_归还转外销(dr_借还, ds_借还.Tables[1], ds_外销, ds_借还.Tables[0]);
                //2019-8-27 修改 
                string s = string.Format("select *,已借出数量-归还数量-正在申请数 as 最大归还数  from 借还申请表附表   where 申请批号 ='{0}'", dr_借还["申请批号"]);
                DataTable dt_归还清单 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                MoldMangement.fm_归还转外销 fm = new fm_归还转外销(dr_借还, dt_归还清单);
                //fm.Dock = System.Windows.Forms.DockStyle.Fill;
                fm.ShowDialog();
                if (fm.flag)
                {
                    // fm.ds_外销
                    DataSet ds_借还 = fun_归还("借用转销售", dr_借还, fm.ds_外销);
                    SqlConnection conn = new SqlConnection(strconn);
                    conn.Open();
                    SqlTransaction thrk = conn.BeginTransaction("归还转销售");
                    try
                    {
                        string sql1 = "select * from 借还申请表 where 1<>1";
                        SqlCommand cmd1 = new SqlCommand(sql1, conn, thrk);
                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_借还申请表);

                        sql1 = "select * from 借还申请表附表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_借还申请表附表);

                        sql1 = "select * from 归还申请主表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_归还申请主);

                        sql1 = "select * from 归还申请子表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(dt_归还申请子);

                        sql1 = "select * from 借还申请表归还记录 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(ds_借还.Tables[0]);

                        //sql1 = "select * from 借还申请批量归还关联 where 1<>1";
                        //cmd1 = new SqlCommand(sql1, conn, thrk);
                        //da1 = new SqlDataAdapter(cmd1);
                        //new SqlCommandBuilder(da1);
                        //da1.Update(ds_借还.Tables[2]);

                        sql1 = "select * from 仓库出入库明细表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(ds_借还.Tables[1]);

                        sql1 = "select * from 销售记录销售订单主表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[0]);

                        sql1 = "select * from 销售记录销售出库通知单主表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[1]);

                        sql1 = "select * from 销售记录成品出库单主表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[2]);

                        sql1 = "select * from 销售记录销售订单明细表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[3]);
                        sql1 = "select * from 销售记录销售出库通知单明细表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[4]);

                        sql1 = "select * from 销售记录成品出库单明细表 where 1<>1";
                        cmd1 = new SqlCommand(sql1, conn, thrk);
                        da1 = new SqlDataAdapter(cmd1);
                        new SqlCommandBuilder(da1);
                        da1.Update(fm.ds_外销.Tables[5]);



                        thrk.Commit();
                        MessageBox.Show("归还转外销成功");
                    }
                    catch (Exception ex)
                    {
                        thrk.Rollback();
                        throw ex;
                    }
                }





                //ui赠送 fm = new ui赠送(drM);
                ////fm.Dock = System.Windows.Forms.DockStyle.Fill;
                //CPublic.UIcontrol.AddNewPage(fm, "归还申请");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 归还转外销ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void 借用转赠送ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try

            {
                DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                DataTable dt_mx = (DataTable)this.gcP.DataSource;
                if (bool.Parse(drM["作废"].ToString()) == true)
                {
                    throw new Exception("当前单据已作废");

                }
                DataView dv = new DataView(dt_mx);
                dv.RowFilter = "已借出数量>归还数量+正在申请数";
                if (dv.Count == 0) throw new Exception("当前单据无可归还明细");

                //if (dt_借xi.Columns.Contains("请输入赠送数量")==false)
                //{
                //    dt_借xi.Columns.Add("请输入赠送数量", typeof(decimal));

                //}
                //foreach (DataRow dr  in dt_mx.Rows)
                //{

                //}




                ui赠送 fm = new ui赠送(drM);
                //fm.Dock = System.Windows.Forms.DockStyle.Fill;
                CPublic.UIcontrol.AddNewPage(fm, "归还申请");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            //try
            //{
            //    dr_借还 = gvM.GetDataRow(gvM.FocusedRowHandle);
            //    //返回ds.tables[0]归还记录明细，ds.tables[1]归还关联,ds.tables[2]
            //    DataSet ds_借还 = fun_归还("借用转赠送", dr_借还);
            //    //保存ds_借还,dt_借用申请表，dt_借用申请表附表
            //    //返回ds.tables[0]dt_销售订单主表dt_，ds.tables[1]出库通知单主表,ds.tables[2]dt_成品出库单主表,ds.tables[3]dt_销售订单明细表，
            //    //ds.tables[4]dt_出库通知单明细表,ds.tables[5]dt_成品出库单明细表,ds.tables[6]dt_仓库出入库明细
            //    DataSet ds_zs = fun_赠送(ds_借还.Tables[0], ds_借还.Tables[1]);
            //    //保存ds_zs
            //    SqlConnection conn = new SqlConnection(strconn);
            //    conn.Open();
            //    SqlTransaction thrk = conn.BeginTransaction("归还转赠送");
            //    try
            //    {
            //        string sql1 = "select * from 借还申请表 where 1<>1";
            //        SqlCommand cmd1 = new SqlCommand(sql1, conn, thrk);
            //        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(dt_主);

            //        sql1 = "select * from 借还申请表附表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(dt_借xi);

            //        sql1 = "select * from 借还申请表归还记录 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_借还.Tables[0]);

            //        //sql1 = "select * from 借还申请批量归还关联 where 1<>1";
            //        //cmd1 = new SqlCommand(sql1, conn, thrk);
            //        //da1 = new SqlDataAdapter(cmd1);
            //        //new SqlCommandBuilder(da1);
            //        //da1.Update(ds_借还.Tables[2]);

            //        sql1 = "select * from 销售记录销售订单主表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[0]);

            //        sql1 = "select * from 销售记录销售出库通知单主表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[1]);

            //        sql1 = "select * from 销售记录成品出库单主表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[2]);

            //        sql1 = "select * from 销售记录销售订单明细表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[3]);
            //        sql1 = "select * from 销售记录销售出库通知单明细表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[4]);

            //        sql1 = "select * from 销售记录成品出库单明细表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_zs.Tables[5]);

            //        sql1 = "select * from 仓库出入库明细表 where 1<>1";
            //        cmd1 = new SqlCommand(sql1, conn, thrk);
            //        da1 = new SqlDataAdapter(cmd1);
            //        new SqlCommandBuilder(da1);
            //        da1.Update(ds_借还.Tables[1]);

            //        thrk.Commit();
            //        MessageBox.Show("归还转赠送成功");

            //        barLargeButtonItem3_ItemClick(null, null);
            //    }
            //    catch (Exception ex)
            //    {
            //        thrk.Rollback();
            //        throw ex;
            //    }


            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}






        }

        private void 归还转耗用ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                DataTable dt_mx = (DataTable)this.gcP.DataSource;
                if (bool.Parse(drM["作废"].ToString()) == true)
                {
                    throw new Exception("当前单据已作废");

                }
                if (Convert.ToBoolean(drM["归还"]) == true)
                {
                    throw new Exception("当前单据已归还，无法转耗用");
                }
                DataView dv = new DataView(dt_mx);
                dv.RowFilter = "已借出数量>归还数量+正在申请数";
                if (dv.Count == 0) throw new Exception("当前单据无可归还明细");




                dr_借还 = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;


                string s = string.Format("select *,已借出数量-归还数量-正在申请数 as 最大归还数  from 借还申请表附表   where 申请批号 ='{0}'", dr_借还["申请批号"]);
                DataTable dt_归还清单 = CZMaster.MasterSQL.Get_DataTable(s, strconn);
                foreach (DataRow dr_归还清单 in dt_归还清单.Rows)
                {
                    if (Convert.ToDecimal(dr_归还清单["最大归还数"]) <= 0)
                    {

                    }
                }

                Form1 fm = new Form1();
                ui归还转耗用 ui = new ui归还转耗用(dr_借还, dt_归还清单);
                fm.Controls.Add(ui);
                fm.Text = "归还转耗用";
                fm.WindowState = FormWindowState.Maximized;
                ui.Dock = DockStyle.Fill;
                fm.ShowDialog();
                //if (ui.bl_转耗用)
                //{
                //    DataSet ds_借还 = fun_归还("借用转耗用", dr_借还, ui.ds_耗用.Tables[4], ui.ds_耗用.Tables[1]);
                //    SqlConnection conn = new SqlConnection(strconn);
                //    conn.Open();
                //    SqlTransaction thrk = conn.BeginTransaction("借用转耗用");
                //    try
                //    {
                //        string sql1 = "select * from 借还申请表 where 1<>1";
                //        SqlCommand cmd1 = new SqlCommand(sql1, conn, thrk);
                //        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                //        new SqlCommandBuilder(da1);
                //        da1.Update(dt_借还申请表);

                //        sql1 = "select * from 借还申请表附表 where 1<>1";
                //        cmd1 = new SqlCommand(sql1, conn, thrk);
                //        da1 = new SqlDataAdapter(cmd1);
                //        new SqlCommandBuilder(da1);
                //        da1.Update(dt_借还申请表附表);

                //        sql1 = "select * from 归还申请主表 where 1<>1";
                //        cmd1 = new SqlCommand(sql1, conn, thrk);
                //        da1 = new SqlDataAdapter(cmd1);
                //        new SqlCommandBuilder(da1);
                //        da1.Update(dt_归还申请主);

                //        sql1 = "select * from 归还申请子表 where 1<>1";
                //        cmd1 = new SqlCommand(sql1, conn, thrk);
                //        da1 = new SqlDataAdapter(cmd1);
                //        new SqlCommandBuilder(da1);
                //        da1.Update(dt_归还申请子);

                //        sql1 = "select * from 借还申请表归还记录 where 1<>1";
                //        cmd1 = new SqlCommand(sql1, conn, thrk);
                //        da1 = new SqlDataAdapter(cmd1);
                //        new SqlCommandBuilder(da1);
                //        da1.Update(ds_借还.Tables[0]);

                //        //sql1 = "select * from 借还申请批量归还关联 where 1<>1";
                //        //cmd1 = new SqlCommand(sql1, conn, thrk);
                //        //da1 = new SqlDataAdapter(cmd1);
                //        //new SqlCommandBuilder(da1);
                //        //da1.Update(ds_借还.Tables[2]);

                //        sql1 = "select * from 仓库出入库明细表 where 1<>1";
                //        cmd1 = new SqlCommand(sql1, conn, thrk);
                //        da1 = new SqlDataAdapter(cmd1);
                //        new SqlCommandBuilder(da1);
                //        da1.Update(ds_借还.Tables[1]);

                //        sql1 = "select * from 其他出入库申请主表 where 1<>1";
                //        cmd1 = new SqlCommand(sql1, conn, thrk);
                //        da1 = new SqlDataAdapter(cmd1);
                //        new SqlCommandBuilder(da1);
                //        da1.Update(ui.ds_耗用.Tables[0]);

                //        sql1 = "select * from 其他出入库申请子表 where 1<>1";
                //        cmd1 = new SqlCommand(sql1, conn, thrk);
                //        da1 = new SqlDataAdapter(cmd1);
                //        new SqlCommandBuilder(da1);
                //        da1.Update(ui.ds_耗用.Tables[1]);

                //        sql1 = "select * from 其他出库主表 where 1<>1";
                //        cmd1 = new SqlCommand(sql1, conn, thrk);
                //        da1 = new SqlDataAdapter(cmd1);
                //        new SqlCommandBuilder(da1);
                //        da1.Update(ui.ds_耗用.Tables[2]);

                //        sql1 = "select * from 其他出库子表 where 1<>1";
                //        cmd1 = new SqlCommand(sql1, conn, thrk);
                //        da1 = new SqlDataAdapter(cmd1);
                //        new SqlCommandBuilder(da1);
                //        da1.Update(ui.ds_耗用.Tables[3]);






                //        thrk.Commit();
                //        MessageBox.Show("归还转耗用成功");
                //    }
                //    catch (Exception ex)
                //    {
                //        thrk.Rollback();
                //        throw ex;
                //    }
                //    barLargeButtonItem1_ItemClick(null, null);
                //}


                //MoldMangement.ui归还转耗用 fm = new ui归还转耗用(dr_借还, dt_归还清单);
                ////fm.Dock = System.Windows.Forms.DockStyle.Fill;
                //CPublic.UIcontrol.AddNewPage(fm, "归还申请");
                //if (fm.bl_转耗用)
                //{

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private DataSet fun_归还(string ss, DataRow dr_借还, DataTable dt_出入库明细, DataTable dt_其他出库子)
        {
            DataSet ds = new DataSet();

            DateTime t = CPublic.Var.getDatetime();
            string sql = string.Format(@"select  *  from 借还申请表附表  where 申请批号 = '{0}'", dr_借还["申请批号"]);
            dt_借还申请表附表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);



            sql = string.Format("select * from 借还申请表 where 申请批号 = '{0}' ", dr_借还["申请批号"]);
            dt_借还申请表 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);


            sql = string.Format("select * from 归还申请主表 where 申请批号 = '{0}' ", dr_借还["申请批号"]);
            dt_归还申请主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            sql = string.Format("select * from 归还申请子表 where 申请批号 = '{0}' ", dr_借还["申请批号"]);
            dt_归还申请子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

            DataTable dt_归还表;
            //   DataTable dt_归还关联表;
            DataTable dt_仓库出入库明细 = dt_出入库明细.Copy();
            string sql_归还 = "select * from 借还申请表归还记录 where 1<>1";
            dt_归还表 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);






            string s_归还单号 = string.Format("RA{0}{1}{2}{3}", t.Year.ToString(), t.Month.ToString("00"), t.Day.ToString("00"), CPublic.CNo.fun_得到最大流水号("RA", t.Year, t.Month).ToString("0000"));
            int i = 1;
            int j = 1;
            string s_归还申请单号 = string.Format("GH{0}{1:D2}{2:00}{3:0000}", t.Year, t.Month, t.Day,
                    CPublic.CNo.fun_得到最大流水号("GH", t.Year, t.Month));
            DataRow dr_申请主 = dt_归还申请主.NewRow();
            dt_归还申请主.Rows.Add(dr_申请主);
            dr_申请主["归还批号"] = s_归还申请单号;
            dr_申请主["申请批号"] = dr_借还["申请批号"];
            dr_申请主["归还操作人"] = CPublic.Var.localUserName;
            dr_申请主["备注"] = dr_借还["备注"];
            dr_申请主["归还申请日期"] = t;
            dr_申请主["归还完成"] = true;
            dr_申请主["原因分类"] = dr_借还["原因分类"];
            dr_申请主["借用类型"] = dr_借还["借用类型"];
            dr_申请主["归还日期"] = t;
            dr_申请主["归还申请日期"] = t;
            dr_申请主["归还完成"] = true;
            dr_申请主["归还方式"] = ss;
            foreach (DataRow dr in dt_其他出库子.Rows)
            {
                #region 判断 借用单和明细的完成状态
                if (dr.RowState == DataRowState.Deleted) continue;
                DataRow[] x = dt_借还申请表附表.Select(string.Format("申请批号明细='{0}'", dr["备注"]));
                //这里x.lenth >0  恒成立 
                x[0]["归还数量"] = Convert.ToDecimal(x[0]["归还数量"]) + Convert.ToDecimal(dr["数量"]);
                if (Convert.ToDecimal(x[0]["申请数量"]) == Convert.ToDecimal(x[0]["归还数量"]))
                {
                    x[0]["归还完成"] = true;
                    x[0]["归还日期"] = t;
                    x[0]["借还状态"] = "已归还";

                }
                #endregion

                #region  归还申请子表添加记录 
                DataRow dr_申请子 = dt_归还申请子.NewRow();
                dt_归还申请子.Rows.Add(dr_申请子);

                dr_申请子["POS"] = j;
                dr_申请子["归还批号"] = s_归还申请单号;
                dr_申请子["归还明细号"] = s_归还申请单号 + "-" + j++.ToString("00");
                dr_申请子["申请批号"] = dr["备注"].ToString().Split('-')[0];
                dr_申请子["申请批号明细"] = dr["备注"];
                dr_申请子["物料编码"] = dr["物料编码"];
                dr_申请子["物料名称"] = dr["物料名称"];
                dr_申请子["规格型号"] = dr["规格型号"];
                dr_申请子["仓库名称"] = dr["仓库名称"];
                dr_申请子["仓库号"] = dr["仓库号"];
                // dr_申请子["货架描述"] = dr["货架描述"];
                dr_申请子["归还日期"] = t;
                dr_申请子["需归还数量"] = dr["数量"];

                dr_申请子["借用数量"] = x[0]["申请数量"];
                //dr_申请子["已归还数量"] = dr["申请数量"];   // 这里经验证 孙杰这个数量  是对应的借用的累计归还数量 无用
                dr_申请子["归还完成"] = true;
                // dr_申请子["录入归还数量"] = dr["申请数量"]; // 这里是界面录入的数量 正常归还申请界面录入的数量 无用
                dr_申请子["申请已归还数量"] = dr["数量"];
                #endregion

                DataRow dr_归还 = dt_归还表.NewRow();
                dt_归还表.Rows.Add(dr_归还);
                dr_归还["guid"] = System.Guid.NewGuid();
                dr_归还["申请批号"] = s_归还单号;
                dr_归还["申请批号明细"] = s_归还单号 + "-" + i++.ToString("00");
                dr_归还["借用申请明细号"] = dr["备注"];


                dr_归还["物料编码"] = dr["物料编码"];
                dr_归还["物料名称"] = dr["物料名称"];
                dr_归还["规格型号"] = dr["规格型号"];
                dr_归还["仓库号"] = dr["仓库号"];
                dr_归还["仓库名称"] = dr["仓库名称"];
                dr_归还["备注"] = ss + "自动生成记录";
                // decimal dec = Convert.ToDecimal(dr["申请数量"]) - Convert.ToDecimal(dr["归还数量"]);
                dr_归还["归还数量"] = dr["数量"];
                dr_归还["归还日期"] = t;
                //  dr_归还["货架描述"] = dr["货架描述"];
                dr_归还["归还操作人"] = CPublic.Var.localUserName;

                DataRow dr_仓库出入库明细 = dt_仓库出入库明细.NewRow();
                dt_仓库出入库明细.Rows.Add(dr_仓库出入库明细);
                dr_仓库出入库明细["GUID"] = System.Guid.NewGuid();
                dr_仓库出入库明细["明细类型"] = "归还入库";
                dr_仓库出入库明细["单号"] = s_归还单号;
                dr_仓库出入库明细["物料编码"] = dr["物料编码"];
                dr_仓库出入库明细["物料名称"] = dr["物料名称"];
                dr_仓库出入库明细["明细号"] = dr_归还["申请批号明细"];
                dr_仓库出入库明细["出库入库"] = "入库";
                dr_仓库出入库明细["实效数量"] = dr["数量"];
                dr_仓库出入库明细["实效时间"] = t;
                dr_仓库出入库明细["出入库时间"] = t;
                dr_仓库出入库明细["相关单号"] = dr_归还["借用申请明细号"];
                dr_仓库出入库明细["相关单位"] = dr_借还["相关单位"];
                dr_仓库出入库明细["仓库号"] = dr["仓库号"];
                dr_仓库出入库明细["仓库名称"] = dr["仓库名称"];


            }
            DataView dv = new DataView(dt_借还申请表附表);
            dv.RowFilter = "归还完成=0";
            if (dv.Count == 0)
            {
                dt_借还申请表.Rows[0]["归还"] = 1;
                dt_借还申请表.Rows[0]["归还日期"] = t;

            }




            ds.Tables.Add(dt_归还表);
            ds.Tables.Add(dt_仓库出入库明细);

            return ds;
        }

        private void gvM_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow r = gvM.GetDataRow(gvM.FocusedRowHandle);
                fun_detail(r["申请批号"].ToString());
                if (CPublic.Var.LocalUserID == r["工号"].ToString())
                {
                    barLargeButtonItem6.Enabled = true;
                }
                else
                {
                    barLargeButtonItem6.Enabled = false;
                }
            }
            catch (Exception)
            {

            }
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                string sql = string.Format("select * from 借还申请表附表 where 申请批号 = '{0}'", dr["申请批号"]);
                DataTable dt_附 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                string sql_jy = string.Format("select * from 借还申请表 where 申请批号 = '{0}'", dr["申请批号"]);
                DataTable dt_jy = CZMaster.MasterSQL.Get_DataTable(sql_jy, strconn);

                if (Convert.ToBoolean(dt_jy.Rows[0]["提交审核"]) == false)
                {
                    throw new Exception("该单据未提交审核，无需弃审");
                }
                if (Convert.ToBoolean(dt_jy.Rows[0]["提交审核"]) == true && Convert.ToBoolean(dt_jy.Rows[0]["审核"]) == false)
                {
                    throw new Exception("该单据未审核，请做撤销提交操作");
                }
                if (Convert.ToBoolean(dt_jy.Rows[0]["作废"]) == true)
                {
                    throw new Exception("该单据已作废，不能弃审");
                }
                foreach (DataRow dr_fu in dt_附.Rows)
                {
                    if (Convert.ToDecimal(dr_fu["已借出数量"]) > 0)
                    {
                        throw new Exception("该单据已有借出记录，不可弃审");
                    }
                }
                if (MessageBox.Show("确认弃审吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string sql_主 = string.Format("select * from 借还申请表 where 申请批号 = '{0}'", dr["申请批号"]);
                    DataTable dt_主 = CZMaster.MasterSQL.Get_DataTable(sql_主, strconn);
                    if (dt_主.Rows.Count > 0)
                    {
                        DataTable dt_审核 = new DataTable();
                        dt_审核 = ERPorg.Corg.fun_PA("弃审", "借用申请单弃审申请", dt_主.Rows[0]["申请批号"].ToString(), dt_主.Rows[0]["相关单位"].ToString());
                        dt_主.Rows[0]["锁定"] = true;

                        SqlConnection conn = new SqlConnection(strconn);
                        conn.Open();
                        SqlTransaction ts = conn.BeginTransaction("pur"); //事务的名称
                        SqlCommand cmd = new SqlCommand("select * from 借还申请表 where 1<>1", conn, ts);
                        SqlCommand cmd1 = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);

                        try
                        {

                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(dt_主);
                            da = new SqlDataAdapter(cmd1);
                            new SqlCommandBuilder(da);
                            da.Update(dt_审核);
                            ts.Commit();
                            MessageBox.Show("弃审申请成功");
                        }
                        catch
                        {
                            ts.Rollback();
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvM_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (gvM.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                DataRow r_focus = gvM.GetDataRow(e.RowHandle);
                DateTime t = CPublic.Var.getDatetime().Date;
                if (!Convert.ToBoolean(r_focus["作废"]) &&  !Convert.ToBoolean(r_focus["归还"]))
                {
                    if (r_focus["预计归还日期"] == null || r_focus["预计归还日期"].ToString() == "")
                    {
                        e.Appearance.BackColor = Color.Yellow;
 
                    }
                    else if (Convert.ToDateTime( r_focus["预计归还日期"])<t)
                    {
                        e.Appearance.BackColor = Color.Pink;
                      
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        string strcon_FS = CPublic.Var.geConn("FS");
        private void 查看订单文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                string sql = string.Format("select * from 借还申请表 where 申请批号 = '{0}'",dr["申请批号"]);
                DataRow dr_主 = CZMaster.MasterSQL.Get_DataRow(sql, strconn);
                if (dr_主["文件GUID"] == null || dr_主["文件GUID"].ToString() == "")
                {
                    throw new Exception("没有文件可以查看，请先上传文件");
                }
                //string type = dr["后缀"].ToString();

                string foldPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\查看文件\\";
                //string fileName = foldPath + CPublic.Var.getDatetime().ToString("yyyy-MM-dd") + "T" + CPublic.Var.getDatetime().ToString("HH_mm_ss") + "Z" + "_" + Guid.NewGuid().ToString() + "." + type;
                string fileName = foldPath + dr_主["文件"].ToString();

                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch (Exception)
                {
                }
                CFileTransmission.CFileClient.strCONN = strcon_FS;
                CFileTransmission.CFileClient.Receiver(dr_主["文件GUID"].ToString(), fileName);
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
