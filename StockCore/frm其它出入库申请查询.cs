using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace StockCore
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm其它出入库申请查询 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        string cfgfilepath = "";
        public frm其它出入库申请查询()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm其它出入库申请查询_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
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
                x.UserLayout(this.splitContainer1,this.Name, cfgfilepath);
                DateTime t = CPublic.Var.getDatetime().Date;

                bar_日期后.EditValue =t.AddDays(1).AddSeconds(-1);
                bar_日期前.EditValue =t.AddMonths(-2);
                bar_单据状态.EditValue = "未完成";
                fun_载入();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_载入()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (dtM != null)
                {
                    dtM.Clear();
                }
                string s_组合 = "select * from 其他出入库申请主表 {0}";
                string s_组合1 = "where ";

                if (bar_日期前.EditValue != null && bar_日期后.EditValue != null && bar_日期前.EditValue.ToString() != "" && bar_日期后.EditValue.ToString() != "")
                {
                    s_组合1 += " 申请日期 >= '" + ((DateTime)bar_日期前.EditValue).ToString("yyyy-MM-dd HH:mm:ss") + "'" + " and 申请日期 <= '" + ((DateTime)bar_日期后.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "' and ";
                }
                if (bar_单据状态.EditValue != null && bar_单据状态.EditValue.ToString() != "")
                {
                    if (bar_单据状态.EditValue.ToString() == "完成")
                    {
                        s_组合1 += "完成 = 'True' and ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "未完成")
                    {
                        s_组合1 += "完成 <>1 and ";
                    }
                    if (bar_单据状态.EditValue.ToString() == "所有")
                    { }
                }

                s_组合1 += "单据类型 <>'材料出库' and ";
                if (s_组合1 != "where ")
                {
                    s_组合1 = s_组合1.Substring(0, s_组合1.Length - 4);
                    s_组合 = string.Format(s_组合, s_组合1);
                }
                SqlDataAdapter da = new SqlDataAdapter(s_组合, strconn);
                da.Fill(dtM);
                gcM.DataSource = dtM;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "退货申请主表_刷新操作");
                throw ex;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            ERPorg.Corg.FlushMemory();
            fun_载入();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                gcM.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gvM_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (e.Clicks == 1 && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    if (dr == null) return;
                    string sql = "";
                    if (dr["申请类型"].ToString() == "其他出库")
                    {
                        sql = string.Format(@"select 其他出入库申请子表.*,isnull(已处理数量,0)已处理数量  from 其他出入库申请子表
                        left  join (select  出入库申请明细号,sum(数量)as 已处理数量 from  其他出库子表 where 生效=1 group by  出入库申请明细号)a 
                        on a.出入库申请明细号=其他出入库申请子表.出入库申请明细号
                        where 其他出入库申请子表.出入库申请单号 = '{0}'", dr["出入库申请单号"]);
                    }
                    else
                    {
                        sql = string.Format(@"select 其他出入库申请子表.*,isnull(已处理数量,0)已处理数量 from 其他出入库申请子表
                        left  join (select  出入库申请明细号,sum(数量)as 已处理数量 from  其他入库子表 where 生效=1 group by  出入库申请明细号)a 
                        on a.出入库申请明细号=其他出入库申请子表.出入库申请明细号
                        where 其他出入库申请子表.出入库申请单号 = '{0}'", dr["出入库申请单号"]);
                    }
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    gcP.DataSource = dt;
                }

                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gcM, new Point(e.X, e.Y));
                    gvM.CloseEditor();
                    this.BindingContext[dtM].EndCurrentEdit();
                    DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                    if(dr["申请类型"].ToString() == "其他出库")
                    {
                        归还ToolStripMenuItem.Visible = true;
                    }
                    else
                    {
                        归还ToolStripMenuItem.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                if (dr == null)
                    throw new Exception("请先选择需要作废的记录");
                if(dr["完成"].ToString().ToLower() == "true")
                    throw new Exception("该记录已完成，不需要作废");
                dr["作废"] = true;
                dr["作废日期"] = System.DateTime.Now;
                dr["作废人员编号"] = CPublic.Var.LocalUserID;

                string sql = string.Format("select * from 其他出入库申请子表 where 出入库申请单号 = '{0}'", dr["出入库申请单号"]);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                foreach (DataRow r in dt.Rows)
                {
                    dr["作废"] = true;
                    dr["作废日期"] = System.DateTime.Now;
                    dr["作废人员编号"] = CPublic.Var.LocalUserID;
                }
                sql = "select * from 其他出入库申请子表 where 1<> 1";
                da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt);

                sql = "select * from 其他出入库申请主表 where 1<> 1";
                da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dtM);
                MessageBox.Show("已作废:" + dr["出入库申请单号"].ToString());
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
                string sql =  string.Format("select * from 其他出入库申请主表 where 出入库申请单号 = '{0}' ", dr["出入库申请单号"].ToString());
                DataTable dt11 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                if (dt11.Rows[0]["作废"].Equals(true))
                {
                    throw new Exception("该记录已作废");

                }
                if (dt11.Rows[0]["完成"].Equals(true))
                {
                    MessageBox.Show("该记录已完成不能修改");
                }
                else
                {
                    StockCore.frm其它出入库申请 frm = new frm其它出入库申请(dr);
                    CPublic.UIcontrol.Showpage(frm, "申请明细");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }

#pragma warning disable IDE1006 // 命名样式
        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try

            {
           DataRow       drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                DataTable dtm=(DataTable)this.gcP.DataSource;
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.Form其他出入库申请", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                object[] drr = new object[3];

                drr[0] = drM; 
                drr[1] = dtm;
                drr[2] = "其他出入库申请";
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

        private void 归还ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                string s = dr["出入库申请单号"].ToString();
                string s1 = dr["原因分类"].ToString();
                string sql_归还 = string.Format(@"select aa.*,isnull(aaa.申请归还总数,0)申请归还总数,isnull(aaa.实还数量,0)实还数量,isnull(已处理数量,0)已处理数量 from 其他出入库申请子表 aa
                                                left join
                                                (
                                                select  x.物料编码,sum(x.申请归还总数)申请归还总数,x.仓库号,x.仓库名称,x.货架描述,x.业务单号,sum(y.实还数量)实还数量 from (
                                                select a.出入库申请单号,a.出入库申请明细号,a.物料编码,sum(a.数量)申请归还总数,a.仓库号,a.仓库名称,a.货架描述,业务单号  from 其他出入库申请子表  a
                                                left join 其他出入库申请主表 b  on   a.出入库申请单号 =b.出入库申请单号 
                                                where 业务单号 ='{0}'  group by a.出入库申请单号,a.出入库申请明细号,a.物料编码,a.仓库号,a.仓库名称,a.货架描述,业务单号)x
                                                left join (
                                                select a.出入库申请单号,a.出入库申请明细号,SUM(rk.数量)实还数量,a.物料编码,a.仓库号,a.仓库名称,a.货架描述,业务单号  from 其他入库子表 rk
                                                left join 其他出入库申请子表 a on rk.出入库申请明细号=a.出入库申请明细号
                                                left join 其他出入库申请主表 b on  a.出入库申请单号 =b.出入库申请单号 
                                                where 业务单号 ='{0}' group by a.出入库申请单号,a.出入库申请明细号,a.物料编码,a.仓库号,a.仓库名称,a.货架描述,业务单号 )y
                                                on x.出入库申请明细号=y.出入库申请明细号
                                                group by x.物料编码,x.仓库号,x.仓库名称,x.货架描述,x.业务单号) aaa

                                                on aa.物料编码 = aaa.物料编码 and aa.仓库号 = aaa.仓库号
                                                left  join (select  出入库申请明细号,sum(数量)as 已处理数量 from  其他出库子表 where 生效=1 group by  出入库申请明细号)abc 
                                                on abc.出入库申请明细号=aa.出入库申请明细号  
                                                 where 出入库申请单号 = '{0}'", dr["出入库申请单号"].ToString());
                DataTable dt_明细 = CZMaster.MasterSQL.Get_DataTable(sql_归还, strconn);
                string sql = @"select a.*,库存总数,b.仓库名称 from 其他出入库申请子表 a ,仓库物料数量表 b ,基础数据物料信息表 c
                        where   a.物料编码=b.物料编码 and 
                            a.物料编码=c.物料编码 and 1<>1";
                DataTable dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                DataTable dt_仓 = new DataTable();
                foreach (DataRow dr_出入库子 in dt_明细.Rows)
                {
                    //19-6-17申请子表增加 已完成数量  以处理数量替换成已完成数量
                    if (Convert.ToDecimal(dr_出入库子["已完成数量"]) != 0 && Convert.ToDecimal(dr_出入库子["已完成数量"]) > Convert.ToDecimal(dr_出入库子["申请归还总数"]))
                    {
                        string sql111 = "select * from 仓库物料数量表 where 物料编码 = '" + dr_出入库子["物料编码"] + "' and 仓库号 = '" + dr_出入库子["仓库号"] + "'";
                        dt_仓 = CZMaster.MasterSQL.Get_DataTable(sql111, strconn);
                        DataRow dr11 = dtP.NewRow();
                        dtP.Rows.Add(dr11);
                        dr11["物料编码"] = dr_出入库子["物料编码"];
                        dr11["物料名称"] = dr_出入库子["物料名称"];
                        dr11["规格型号"] = dr_出入库子["规格型号"];
                        dr11["货架描述"] = dr_出入库子["货架描述"];
                        if (dt_仓.Rows.Count == 0)
                        {
                            dr["库存总数"] = 0;

                        }
                        else
                        {
                            dr11["库存总数"] = dt_仓.Rows[0]["库存总数"];
                        }
                        dr11["仓库号"] = dr_出入库子["仓库号"];
                        dr11["仓库名称"] = dr_出入库子["仓库名称"];
                        dr11["数量"] = Convert.ToDecimal(dr_出入库子["已完成数量"]) - Convert.ToDecimal(dr_出入库子["申请归还总数"]);
                    }
                }
                if (dr["作废"].Equals(true))
                {
                    throw new Exception("该记录已作废");

                }
                if (dtP.Rows.Count<= 0)
                {
                    throw new Exception("该单据无可做归还操作的物料！");
                }

                
                
                else
                {
                    StockCore.frm其它出入库申请 frm = new frm其它出入库申请(s,s1, dt_明细);
                    CPublic.UIcontrol.Showpage(frm, "申请明细");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gvM_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
                string sql = "";
                if (dr["申请类型"].ToString() == "其他出库")
                {
                    sql = string.Format(@"select 其他出入库申请子表.*,isnull(已处理数量,0)已处理数量  from 其他出入库申请子表
                        left  join (select  出入库申请明细号,sum(数量)as 已处理数量 from  其他出库子表 where 生效=1 group by  出入库申请明细号)a 
                        on a.出入库申请明细号=其他出入库申请子表.出入库申请明细号
                        where 其他出入库申请子表.出入库申请单号 = '{0}'", dr["出入库申请单号"]);
                }
                else
                {
                    sql = string.Format(@"select 其他出入库申请子表.*,isnull(已处理数量,0)已处理数量 from 其他出入库申请子表
                        left  join (select  出入库申请明细号,sum(数量)as 已处理数量 from  其他入库子表 where 生效=1 group by  出入库申请明细号)a 
                        on a.出入库申请明细号=其他出入库申请子表.出入库申请明细号
                        where 其他出入库申请子表.出入库申请单号 = '{0}'", dr["出入库申请单号"]);
                }
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                gcP.DataSource = dt;
            }
            catch  
            {

            
            }
          
        }
     
    }
}
