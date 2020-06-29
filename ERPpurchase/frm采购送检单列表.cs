using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using CZMaster;
using System.Drawing.Printing;
using DevExpress.XtraPrinting;
using System.IO;

using Microsoft.Reporting.WinForms;

namespace ERPpurchase
{
    public partial class frm采购送检单列表 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        string s_物料编码;
        Boolean s_跳转 = false;
        //strcon = CPublic.Var.strConn;

        public frm采购送检单列表()
        {
            InitializeComponent();
           // strcon = CPublic.Var.strConn;
        }
        public frm采购送检单列表(string s)
        {
            InitializeComponent();
            s_物料编码 = s;
            s_跳转 = true;
        }
        /// <summary>
        /// 采购送检单主表
        /// </summary>
        DataTable dt_送检单主表;
        bool flag = false;//默认0 打印清单时1

        private void frm采购送检单列表_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel1, this.Name, cfgfilepath);
                if(s_跳转 == true)
                {
                    gvv1.FindFilterText = s_物料编码;
                }

                txt_sjdanhao.EditValue = "";
                txt_sjtime1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"));
                txt_sjtime2.EditValue = Convert.ToDateTime(System.DateTime.Today.ToString("yyyy-MM-dd"));
                txt_sjdanstate.EditValue = "已生效";
                fun_searchSongjianDan();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_searchSongjianDan()
        {
            try
            {
                string sql = "";
                if (txt_sjdanhao.EditValue.ToString() != "")
                {
                    sql = sql + string.Format(" 送检单号='{0}' and", txt_sjdanhao.EditValue.ToString());
                }
                else
                {
                    if (txt_sjtime1.EditValue != null && txt_sjtime2.EditValue != null && txt_sjtime1.EditValue.ToString() != "" && txt_sjtime2.EditValue.ToString() != "")
                    {
                        if (Convert.ToDateTime(txt_sjtime1.EditValue) > Convert.ToDateTime(txt_sjtime2.EditValue))
                            throw new Exception("起始日期不能大于终止日期！");
                        sql = sql + string.Format(" csjmx.生效日期>='{0}' and csjmx.生效日期<='{1}' and", txt_sjtime1.EditValue.ToString()
                            , Convert.ToDateTime(txt_sjtime2.EditValue).AddDays(1).AddSeconds(-1));
                    }

                    if (txt_sjdanstate.EditValue.ToString() == "已生效")
                    {
                        sql = sql + " csjmx.生效=1 and csjmx.作废=0 and";
                    }
                    if (txt_sjdanstate.EditValue.ToString() == "未生效")
                    {
                        sql = sql + " csjmx.生效=0 and csjmx.作废=0 and";
                    }
                    if (txt_sjdanstate.EditValue.ToString() == "已作废")
                    {
                        sql = sql + " csjmx.作废=1  and";
                    }

                    if (flag)
                    {
                        DataColumn dc = new DataColumn("选择", typeof(bool));
                        dc.DefaultValue = false;
                        dt_送检单主表.Columns.Add(dc);

                        gridColumn16.Visible = true;
                        gridColumn16.VisibleIndex = 0;
                        sql = sql + " (检验完成=0 or (检验完成=1 and csjmx.备注4='免检')) and";
                        // sql = sql + "  检验完成=0   and";

                    }
                    else
                    {
                        gridColumn16.Visible = false;


                    }
                }


                sql = sql.Substring(0, sql.Length - 3);
                //DataTable view_权限 = ERPorg.Corg.fun_hr("采购", CPublic.Var.LocalUserID);
                //if (view_权限.Rows.Count > 0)
                //{
                //    //sql += " and csjz.生效人员ID in (";
                //    //foreach (DataRow r in view_权限.Rows)
                //    //{
                //    //    sql += "'" + r["工号"].ToString().Trim() + "',";
                //    //}
                //    //sql = sql.Substring(0, sql.Length - 1) + ")";
                //}
                //if (CPublic.Var.LocalUserTeam != "管理员权限" && CPublic.Var.LocalUserID !="910173"    )
                //{
                //    throw new Exception("未配置此界面视图权限,请确认");
                //}
                string s_add ="";
                if(CPublic.Var.LocalUserTeam=="开发部权限" || CPublic.Var.localUser部门名称.Contains("开发"))
                {

                    s_add = " and 采购单类型='开发采购'";
                }
                else if(CPublic.Var.LocalUserTeam !="管理员权限")
                {
                    s_add = " and 采购单类型<>'开发采购'";
                }
               
                sql = " where" + sql+s_add; //+ " order by 优先级 desc,csjz.采购已处理,送检单号 desc";
                //                    sql = string.Format(@"select  jssl.拒收数量,  hjms.货架描述,hjms.仓库名称, csjz.*,base.图纸编号,base.规格型号,cjyz.检验结果,cjyz.关闭 as 仓库关闭,检验完成,ISNULL(已入库数,0)已入库数,ISNULL(x.不合格数量,0)不合格数量,
                //            入库完成,默认检验员,case when(检验结果='不合格'and 采购已处理=0) then 1 else 0 end as 优先级,采购已处理,采购供应商备注 from 采购记录采购送检单主表  csjz
                //                       left join 基础数据物料信息表 base on  base.物料编码 = csjz.物料编码 
                //                     left join 仓库物料数量表 hjms on hjms.物料编码= csjz.物料编码   and hjms.仓库号=base.仓库号
                //                       left join [采购记录采购单检验主表] cjyz  on  cjyz.送检单号 = csjz.送检单号
                //                       left join 采购记录采购送检单明细表 sjmx on  sjmx.送检单号 = csjz.送检单号
                //left join  采购记录采购单明细表 jssl  on  sjmx.采购单明细号 = jssl.采购明细号
                //                       left join  [采购记录采购检验默认人员表] on [采购记录采购检验默认人员表].物料编码=base.物料编码
                //                       left join (select  送检单明细号,sum(已入库数) as 已入库,SUM(不合格数量)as 不合格数量 from [采购记录采购单检验主表] group by 送检单明细号)x
                //                       on x.送检单明细号=sjmx.送检单明细号  {0} ", sql);
                sql = string.Format(@"select isnull(gtx.开票数量,0)开票数量,cmx.仓库名称,csjmx.规格型号,货架描述,检验完成,    ISNULL(y.已入库,0)已入库数,ISNULL(x.不合格数量,0)不合格数量,
    csjmx.*,isnull(仓库关闭量,0)仓库关闭量  from 采购记录采购送检单明细表 csjmx 
    left join (select 送检单明细号,sum(入库量)as 已入库   from 采购记录采购单入库明细  group by 送检单明细号  )y  on  y.送检单明细号=csjmx.送检单明细号  
    left join  ( select  SUM(开票数量)开票数量,送检单明细号   from  采购记录采购开票通知单明细表 where 生效=1  and 送检单明细号 <>'' group by  送检单明细号 )gtx on csjmx.送检单明细号=gtx.送检单明细号 
    left join (select  送检单明细号,SUM(不合格数量)as 不合格数量 from [采购记录采购单检验主表] where 关闭=0  group by 送检单明细号)x on x.送检单明细号=csjmx.送检单明细号  
    left join ( select  送检单明细号,sum(送检数量-不合格数量-已入库数)仓库关闭量   from 采购记录采购单检验主表  
    where   完成=1  and 关闭=0 and 已入库数 <送检数量-不合格数量  group by 送检单明细号)ckclose on ckclose.送检单明细号=csjmx.送检单明细号 
    left join 采购记录采购单明细表 cmx on cmx.采购明细号=csjmx.采购单明细号
    left join 采购记录采购单主表 czb on czb.采购单号=cmx.采购单号
    left join 仓库物料数量表  kc on kc.物料编码=csjmx.物料编码 and kc.仓库号=cmx.仓库号 {0}   ", sql );

                dt_送检单主表 = MasterSQL.Get_DataTable(sql, strcon);
                //  dt_送检单主表 = WSAdapter.webservers_getdata.wsmo.GetData_ERP(sql);
                if (flag)
                {
                    DataColumn dc = new DataColumn("选择", typeof(bool));
                    dc.DefaultValue = false;
                    dt_送检单主表.Columns.Add(dc);

                    gridColumn16.Visible = true;
                }
                else
                {
                    gridColumn16.Visible = false;

                }
                gcc1.DataSource = dt_送检单主表;

            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_searchSongjianDan");
                throw ex;
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frm采购送检单界面 frm = new frm采购送检单界面();
            CPublic.UIcontrol.AddNewPage(frm, "采购送检单");
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gvv1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.Clicks == 2)
                {
                    DataRow r = (this.BindingContext[dt_送检单主表].Current as DataRowView).Row;
                    if (r["生效"].Equals(false))
                    {
                        frm采购送检单界面 frm = new frm采购送检单界面(r["送检单号"].ToString());
                        CPublic.UIcontrol.AddNewPage(frm, "采购送检单");
                    }
                    else
                    {


                    }
                }
                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gcc1, new Point(e.X, e.Y));
                    gvv1.CloseEditor();
                    this.BindingContext[dt_送检单主表].EndCurrentEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region 界面操作
        //清空单号的操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txt_sjdanhao.EditValue = "";
        }
        //查询数据的操作
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                flag = false;
                barLargeButtonItem8.Caption = "打印送检清单";
                fun_searchSongjianDan();
                if (dt_送检单主表.Rows.Count <= 0)
                    throw new Exception("查无数据！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gcc1.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
            DateTime t = CPublic.Var.getDatetime();
            string str_id = CPublic.Var.LocalUserID;
            string str_name = CPublic.Var.localUserName;

            try
            {
                string s = string.Format("select* from 采购记录采购送检单明细表 where 送检单明细号 = '{0}' ", dr["送检单明细号"]);
                DataTable tabel = CZMaster.MasterSQL.Get_DataTable(s,strcon);
                if(Convert.ToDecimal( tabel.Rows[0]["已检验数"])>0)
                {
                    throw new Exception("这条记录已有过检验,不可直接作废");
                }
               
                if (MessageBox.Show(string.Format("确认作废该条送检单？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql1 = string.Format("select * from 采购记录采购送检单主表 where 送检单号 = '{0}'", dr["送检单号"]);
                    DataTable dt1 = new DataTable();
                    SqlDataAdapter da1 = new SqlDataAdapter(sql1, strcon);
                    da1.Fill(dt1);
                    dt1.Rows[0]["作废"] = 1;
                    dt1.Rows[0]["作废日期"] = t;
                    dt1.Rows[0]["作废人员"] = str_name;
                    dt1.Rows[0]["作废人员ID"] = str_id;

                    string sql2 = string.Format("select * from 采购记录采购送检单明细表 where 送检单号 = '{0}'", dr["送检单号"]);
                    DataTable dt2 = new DataTable();
                    SqlDataAdapter da2 = new SqlDataAdapter(sql2, strcon);
                    da2.Fill(dt2);
                    foreach (DataRow r in dt2.Rows)
                    {
                        r["作废"] = 1;
                        r["作废日期"] = t;
                        r["作废人员"] = str_name;
                        r["作废人员ID"] = str_id;
                    }
                    string sql3 = string.Format("select * from 采购记录采购单检验主表 where 送检单号 = '{0}'", dr["送检单号"]);
                    DataTable dt3 = new DataTable();
                    SqlDataAdapter da3 = new SqlDataAdapter(sql3, strcon);
                    da2.Fill(dt3);
                    if (dt3.Rows.Count > 0)
                    {
                        foreach(DataRow r in dt3.Rows)
                        {
                            r["关闭"] = true;
                        }
                    }


                    SqlDataAdapter da;
                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("sjdsx");
                    SqlCommand cmd_zb = new SqlCommand("select * from 采购记录采购送检单主表 where 1<>1", conn, ts);
                    SqlCommand cmd_mx = new SqlCommand("select * from 采购记录采购送检单明细表 where 1<>1", conn, ts);

                    try
                    {   //送检单主表的生效
                        da = new SqlDataAdapter(cmd_zb);
                        new SqlCommandBuilder(da);
                        da.Update(dt1);
                        //送检单明细表的生效
                        da = new SqlDataAdapter(cmd_mx);
                        new SqlCommandBuilder(da);
                        da.Update(dt2);
                        ts.Commit();
                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        throw ex;
                    }
                    MessageBox.Show("作废成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private void 撤销送检ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                //string sql = string.Format("select * from 采购记录采购送检单明细表 where 送检单明细号='{0}'", dr["送检单明细号"].ToString().Trim());
                //DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                //if (dt.Rows.Count > 0)
                //{
                //if (dt.Rows[0]["检验完成"].Equals(false))
                //{
                if(dr["送检单类型"].ToString()=="拒收")
                {
                    throw new Exception("拒收单据暂不支持撤回");
                }
                if (MessageBox.Show(string.Format("确认撤销该条送检单？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //if (dr != null)
                    //{
                    string sql_zb = string.Format("select  * from 采购记录采购送检单主表 where  送检单号='{0}'", dr["送检单号"]);
                    DataTable dt_zb = new DataTable();
                    dt_zb = CZMaster.MasterSQL.Get_DataTable(sql_zb, CPublic.Var.strConn);
                    if (dt_zb.Rows.Count == 0) //这里U8导入的数据   一个到货单 多个明细 主表的送检单号 实则为明细号
                    {
                        sql_zb = string.Format("select  * from 采购记录采购送检单主表 where  送检单号='{0}'", dr["送检单明细号"]);
                        dt_zb = CZMaster.MasterSQL.Get_DataTable(sql_zb, CPublic.Var.strConn);
                    }
                    if (dt_zb.Rows.Count == 0) throw new Exception("此纪录有问题不可撤销");

                    string sql_mx = string.Format("select  * from 采购记录采购送检单明细表 where  送检单明细号='{0}'", dr["送检单明细号"]);
                    DataTable dt_mx = new DataTable();
                    dt_mx = CZMaster.MasterSQL.Get_DataTable(sql_mx, CPublic.Var.strConn);
                    if (dt_mx.Rows.Count == 0)                  
                        throw new Exception("此记录有问题不可撤销");
                    else if(Convert.ToDecimal( dt_mx.Rows[0]["已检验数"])>0)
                    {
                        throw new Exception("该送检单已有检验记录,请先通知检验人员撤回检验记录");
                    }
                    if (dt_zb.Rows.Count > 0 && dt_mx.Rows.Count > 0)
                    {
                        //dt_mx.Rows[0].Delete();
                        //dt_zb.Rows[0].Delete();
                       DateTime t= CPublic.Var.getDatetime();

                        dt_mx.Rows[0]["作废"] = true;
                        dt_mx.Rows[0]["作废人员"] = CPublic.Var.localUserName;
                        dt_mx.Rows[0]["作废人员ID"] = CPublic.Var.LocalUserID;
                        dt_mx.Rows[0]["作废日期"] =t ;

                        dt_zb.Rows[0]["作废"] = true;
                        dt_zb.Rows[0]["作废人员"] = CPublic.Var.localUserName;
                        dt_zb.Rows[0]["作废人员ID"] = CPublic.Var.LocalUserID;
                        dt_zb.Rows[0]["作废日期"] = t;
                    }
                    string sql_采购明细 = string.Format("select * from  采购记录采购单明细表 where  采购明细号='{0}' ", dr["采购单明细号"]);
                    DataTable dt_采购明细 = new DataTable();
                    dt_采购明细 = CZMaster.MasterSQL.Get_DataTable(sql_采购明细, CPublic.Var.strConn);
                    if (dt_采购明细.Rows.Count > 0)
                    {
                        dt_采购明细.Rows[0]["明细完成"] = 0;
                        dt_采购明细.Rows[0]["已送检数"] = Convert.ToDecimal(dt_采购明细.Rows[0]["已送检数"]) - Convert.ToDecimal(dr["送检数量"]);
                    }
                    string sql_检验明细 = string.Format("select * from  采购记录采购单检验主表 where 送检单明细号 = '{0}' ", dr["送检单明细号"]);
                    DataTable dt_检验 = CZMaster.MasterSQL.Get_DataTable(sql_检验明细, strcon);
                    if (dt_检验.Rows.Count > 0)
                    {
                        foreach(DataRow dr_检验 in dt_检验.Rows)
                        {
                            dr_检验["关闭"] = true;
                        }
                    }



                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("kpsw");
                    sql_采购明细 = "select  * from 采购记录采购单明细表 where 1<>1 ";
                    SqlCommand cmd = new SqlCommand(sql_采购明细, conn, ts);

                    try
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_采购明细);


                        sql_zb = "select * from 采购记录采购送检单主表 where 1<>1";
                        cmd = new SqlCommand(sql_zb, conn, ts);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_zb);

                        sql_mx = "select * from 采购记录采购送检单明细表 where 1<>1";
                        cmd = new SqlCommand(sql_mx, conn, ts);
                        da = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(da);
                        da.Update(dt_mx);

                        if (dt_检验.Rows.Count > 0)
                        {
                            string sql_jymx = "select * from 采购记录采购单检验主表 where 1<>1";
                            cmd = new SqlCommand(sql_jymx, conn, ts);
                            da = new SqlDataAdapter(cmd);
                            new SqlCommandBuilder(da);
                            da.Update(dt_检验);
                        }
                        

                        ts.Commit();
                        MessageBox.Show("已撤销");
                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();

                        throw new Exception(ex.Message);
                    }
                   
                  
                 


                    //}
                }
                //}
                //    else
                //    {
                //        throw new Exception("这条记录已检验完成,不可撤销");
                //    }

                //}
                //else
                //{
                //    throw new Exception("未找到该送检单，请确认？");
                //}
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            撤销送检ToolStripMenuItem_Click(null, null);
            fun_searchSongjianDan();
        }

        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barLargeButtonItem8.Caption == "打印送检清单")
            {
                flag = true;
                fun_searchSongjianDan();
                MessageBox.Show("请勾选需打印的清单明细");
                barLargeButtonItem8.Caption = "确认打印";
            }
            else
            {
                gvv1.CloseEditor();
                this.BindingContext[dt_送检单主表].EndCurrentEdit();
                DataView dv = new DataView(dt_送检单主表);
                dv.RowFilter = "选择=1";
                DataTable dt_dy = dv.ToTable();



                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                this.printDialog1.Document = this.printDocument1;

                DialogResult result = this.printDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {

                    string str_打印机 = this.printDocument1.PrinterSettings.PrinterName;


                    ItemInspection.print_FMS.fun_p_送检清单(dt_dy, str_打印机);
                }
                flag = false;
                fun_searchSongjianDan();
                barLargeButtonItem8.Caption = "打印送检清单";
            }
        }

        private void gvv1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                //textBox16.Text = "";

                if (gvv1.GetRow(e.RowHandle) == null)
                {
                    return;
                }
                //int j = gv.RowCount;
                //for (int i = 0; i < j; i++)
                //{
                //if (gvv1.GetRowCellValue(e.RowHandle, "检验结果").ToString() == "不合格" && gvv1.GetRowCellValue(e.RowHandle, "采购已处理").Equals(false))
                //{
                //    e.Appearance.BackColor = Color.Pink;
                //    e.Appearance.BackColor2 = Color.Pink;
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 采购员处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                string sql = string.Format("select * from 采购记录采购送检单主表 where 送检单号='{0}'", dr["送检单号"].ToString().Trim());
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                if (dt.Rows.Count > 0)
                {
                    if (dr["检验结果"].ToString() == "不合格")
                    {
                        if (MessageBox.Show(string.Format("确认处理该条送检单？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {

                            dt.Rows[0]["采购已处理"] = true;
                            DataTable dtx = dt.Clone();
                            dtx.ImportRow(dr);
                            DataTable dt_刷新 = StockCore.StockCorer.fun_四个量(dtx);
                            SqlConnection conn = new SqlConnection(strcon);
                            conn.Open();
                            SqlTransaction cl = conn.BeginTransaction("采购处理");
                            try
                            {
                                string sql_1 = "select * from 采购记录采购送检单主表 where 1<>1";
                                SqlCommand cmm_1 = new SqlCommand(sql_1, conn, cl);
                                string sql_2 = "select * from 仓库物料数量表  where 1<>1";
                                SqlCommand cmm_2 = new SqlCommand(sql_2, conn, cl);

                                SqlDataAdapter da_1 = new SqlDataAdapter(cmm_1);
                                SqlDataAdapter da_2 = new SqlDataAdapter(cmm_2);

                                new SqlCommandBuilder(da_1);
                                new SqlCommandBuilder(da_2);

                                da_1.Update(dt);
                                da_2.Update(dt_刷新);

                                cl.Commit();
                                MessageBox.Show("已处理");
                                dr["采购已处理"] = true;
                                dt_送检单主表.AcceptChanges();

                            }
                            catch (Exception ex)
                            {
                                cl.Rollback();
                                throw new Exception("保存失败,刷新后重试");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("这条记录已检验完成,刷新后重试");
                    }

                }
                else
                {
                    throw new Exception("未找到该送检单，请确认？");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        //退回返修，为送检单 品质检验不合格 采购员需吧这批料退回供应商返修 ，之后再送检
        private void 退回返修ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                if (MessageBox.Show(string.Format("确认将此送检记录退回供应商返修等待再次送检？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                    if (dr["入库完成"].Equals(true))
                    {
                        throw new Exception("该记录已入库");

                    }

                    if (dr["检验结果"].ToString() != "不合格")
                    {
                        throw new Exception("检验结果为不合格的记录才可进行此操作");

                    }
                    DataTable dt_送检单明细 = new DataTable();
                    string s = string.Format("select  *  from 采购记录采购送检单明细表   where  送检单号='{0}'", dr["送检单号"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                    {
                        da.Fill(dt_送检单明细);
                        dt_送检单明细.Rows[0]["作废"] = 1;
                        dt_送检单明细.Rows[0]["作废日期"] = CPublic.Var.getDatetime();
                        dt_送检单明细.Rows[0]["备注1"] = "退回返修";
                    }
                    DataTable dt_送检单主表 = new DataTable();
                    s = string.Format("select  *  from 采购记录采购送检单主表   where  送检单号='{0}'", dr["送检单号"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                    {
                        da.Fill(dt_送检单主表);
                        dt_送检单主表.Rows[0]["作废"] = 1;
                        dt_送检单主表.Rows[0]["作废日期"] = CPublic.Var.getDatetime();
                        dt_送检单主表.Rows[0]["备注1"] = "退回返修";
                    }

                    DataTable dt_采购单 = new DataTable();
                    string ss = string.Format("select  *  from 采购记录采购单明细表   where   采购明细号='{0}'", dr["采购单明细号"]);
                    dt_采购单 = CZMaster.MasterSQL.Get_DataTable(ss, strcon);
                    s = string.Format(@"select  采购单明细号,SUM(送检数量)as 送检数量 from 采购记录采购送检单明细表   
                    where 作废=0   and  采购单明细号='{0}' and 送检单号<>'{1}' group by 采购单明细号 ", dr["采购单明细号"], dr["送检单号"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(s, strcon))
                    {
                        DataTable temp = new DataTable();
                        da.Fill(temp);
                        if (temp.Rows.Count == 0)
                        {
                            dt_采购单.Rows[0]["已送检数"] = 0;
                        }
                        else
                        {
                            dt_采购单.Rows[0]["已送检数"] = Convert.ToDecimal(temp.Rows[0]["送检数量"]);
                        }
                        dt_采购单.Rows[0]["明细完成"] = 0;



                    }
                    //  DataTable dt_fc = StockCore.StockCorer.fun_四个量(dt_送检单主表);



                    SqlDataAdapter daz;
                    SqlConnection conn = new SqlConnection(strcon);
                    conn.Open();
                    SqlTransaction ts = conn.BeginTransaction("sjdfx"); //送检单返修




                    try
                    {   //送检单主表的生效
                        SqlCommand cmd = new SqlCommand("select * from 采购记录采购送检单主表 where 1<>1", conn, ts);
                        daz = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(daz);
                        daz.Update(dt_送检单主表);
                        //送检单明细表的生效
                        cmd = new SqlCommand("select * from 采购记录采购送检单明细表 where 1<>1", conn, ts);

                        daz = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(daz);
                        daz.Update(dt_送检单明细);
                        //采购单明细表

                        cmd = new SqlCommand("select * from 采购记录采购单明细表 where 1<>1", conn, ts);
                        daz = new SqlDataAdapter(cmd);
                        new SqlCommandBuilder(daz);
                        daz.Update(dt_采购单);
                        //仓库物料数量表

                        //cmd = new SqlCommand("select * from 仓库物料数量表  where 1<>1", conn, ts);
                        //daz = new SqlDataAdapter(cmd);
                        //new SqlCommandBuilder(daz);
                        //daz.Update(dt_fc);
                        ts.Commit();



                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();

                        throw ex;
                    }
                    StockCore.StockCorer.fun_物料数量_实际数量(dr["物料编码"].ToString(), dr["仓库号"].ToString(), true);
                    fun_searchSongjianDan();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }



        }

        private void gvv1_ColumnPositionChanged(object sender, EventArgs e)
        {

        }

        private void gvv1_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {

        }

        private void barLargeButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (dt_送检单主表 == null || dt_送检单主表.Rows.Count == 0) throw new Exception("没有任何记录");
                if (barLargeButtonItem8.Caption.ToString() == "打印送检清单_excel" || barLargeButtonItem8.Caption.ToString() == "打印送检清单")
                {
                    flag = true;
                    fun_searchSongjianDan();
                    MessageBox.Show("请勾选需打印的清单明细");
                    barLargeButtonItem8.Caption = "确认打印";
                }
                else
                {
                    gvv1.CloseEditor();
                    this.BindingContext[dt_送检单主表].EndCurrentEdit();
                    DataView dv = new DataView(dt_送检单主表);
                    dv.RowFilter = "选择=1";
                    DataTable dt_dy = dv.ToTable();



                    //PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                    //this.printDialog1.Document = this.printDocument1;

                    //DialogResult result = this.printDialog1.ShowDialog();
                    //if (result == DialogResult.OK)
                    //{

                    //        List<ReportParameter> lstParameter = new List<ReportParameter>()
                    //{

                    //         //  new ReportParameter("含税总额",dec_含税金额总.ToString()),       
                    //         //new ReportParameter("不含税总额",dec_不含税金额总.ToString()),  
                    //         // new ReportParameter("供应商",dt_Main.Rows[0]["供应商名称"].ToString()),

                    //};


                    ERPreport.送检清单 form = new ERPreport.送检清单(dt_dy);
                    form.ShowDialog();
                    // string str_打印机 = this.printDocument1.PrinterSettings.PrinterName;


                    // ItemInspection.print_FMS.fun_p_送检清单(dt_dy, str_打印机);
                    //}
                    flag = false;
                    fun_searchSongjianDan();
                    barLargeButtonItem8.Caption = "打印送检清单";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }




        }

        private void barLargeButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPpurchase.ui拒收操作 ui = new ui拒收操作();
            CPublic.UIcontrol.Showpage(ui, "拒收操作");
        }
        //2019-6-10 仓库关闭功能去除 让采购员关闭
        private void 完成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvv1.GetDataRow(gvv1.FocusedRowHandle);
                string s = string.Format("select  * from 采购记录采购单检验主表 where 送检单明细号='{0}' and 入库完成=0 and 完成=0 and 关闭=0 ", dr["送检单明细号"]);
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                if (t.Rows.Count > 0)
                {
                    foreach (DataRow r in t.Rows)
                    {
                        r["完成"] = 1;
                        r["入库完成"] = 1;
                    }
                    CZMaster.MasterSQL.Save_DataTable(t, "采购记录采购单检验主表", strcon);
                }
                else
                {
                    MessageBox.Show("没有相应的未完成入库的检验单,不需要手动完成");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txt_sjdanstate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
}
