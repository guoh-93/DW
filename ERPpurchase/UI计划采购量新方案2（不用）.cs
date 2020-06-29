using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CZMaster;
using ERPproduct;
using System.IO;
namespace ERPpurchase
{
    public partial class UI计划采购量新方案2 : UserControl
    {
        #region 成员
        DataTable dtP = null;
        DataTable dt_制令子表;
        DataTable dt_制令主表_附表;
        DataTable dt_displaymx;
        string str_制令单号 = "";
        DataTable dt_权限;
        string strcon = "";
        //string str_person = "";                 //记录是采购员是谁 or  制造部老大或高管 要看个人的
        string cfgfilepath = "";
 
        DataView dv_制令子;
        #endregion

        #region 自用类
        public UI计划采购量新方案2()
        {
            InitializeComponent();
            strcon = CPublic.Var.strConn;
        }

        private void UI计划采购量新方案2_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            if (File.Exists(cfgfilepath + string.Format(@"\{0}.xml", this.Name)))
            {

                gv2.RestoreLayoutFromXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
            }

            DateTime t = CPublic.Var.getDatetime().AddMonths(-3).Date;

            bar_日期.EditValue = t;
            try
            {

                //权限的dt表
                dt_权限 = ERPorg.Corg.fun_hr("生产", CPublic.Var.LocalUserID);
                //string sql = string.Format("select * from 人事基础员工表 where 员工号='{0}'", CPublic.Var.LocalUserID);
                //DataTable dt = new DataTable();
                //dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                //if ((dt.Rows[0]["部门编号"].ToString() == "00010301" && dt.Rows[0]["职务"].ToString() == "部长") || dt.Rows[0]["权限组"].ToString() == "公司高管权限")
                //{
                //    str_person = "admin";  //ID
                //    barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                //    barEditItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                //}

                string sql = string.Format(@"select 生产记录生产制令子表.*,[销售记录销售订单明细表].备注,反馈备注 from 生产记录生产制令子表,销售记录销售订单明细表,基础数据物料信息表
    
                                        where 生产记录生产制令子表.销售订单明细号 =销售记录销售订单明细表.销售订单明细号  and 生产记录生产制令子表.物料编码=基础数据物料信息表.物料编码

                                         and 1=2");
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    dt_displaymx = new DataTable();
                    da.Fill(dt_displaymx);
                    dv_制令子 = new DataView(dt_displaymx);
                }

                //这里筛选条件有可能要改为  物料的 物料类型 为 库存商品 的
                string s = @"select 物料编码,物料名称,规格型号  from 基础数据物料信息表 base 
                inner join (select  产品编码 from 基础数据物料BOM表  group by 产品编码 ) a   on base.物料编码=a.产品编码 ";
                DataTable dt_产品 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                searchLookUpEdit1.Properties.DataSource = dt_产品;
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                searchLookUpEdit1.Properties.ValueMember = "物料编码";



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 转生产制令
        //选择需要转制令的计划
        private void fun_selectJH()
        {
            try
            {
                dt_制令主表_附表 = dtP.Clone();
                foreach (DataRow r in dtP.Rows)
                {
                    if (r["选择"].Equals(true))
                    {
                        dt_制令主表_附表.Rows.Add(r.ItemArray);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + "　fun_selectJH");
                throw new Exception(ex.Message);
            }
        }

        //转制令检测
        private void fun_checkToZL()
        {
            try
            {
                if (dt_制令主表_附表.Rows.Count <= 0)
                    throw new Exception("请选择需要转生产制令的生产计划！");
                foreach (DataRow r in dt_制令主表_附表.Rows)
                {
                    ////1、检查勾选转制令的计划单号下面有没有明细，如果没有明细，需要先新增。
                    //if (r["物料类型"].ToString() == "成品")
                    //{
                    //    DataRow[] dr = dt_productdetail.Select(string.Format("生产计划单号='{0}'", r["生产计划单号"].ToString()));
                    //    if (dr.Length <= 0)
                    //        throw new Exception(string.Format("勾选的生产计划单号\"{0}\"无明细，请选中该行新增明细！", r["生产计划单号"].ToString()));
                    //}
                    //2、检查数量，输入的是生产数量是用户输入的，并检查输入的是否是数字。
                    if (r["输入生产数量"].ToString() == "")
                        throw new Exception("选择转生产制令的计划单，输入生产数量不能为空，请填写！");

                    Decimal dd = 0;
                    if (!decimal.TryParse(r["输入生产数量"].ToString(), out dd)) throw new Exception("输入生产数量应该为数字，请检查！");
                    decimal dec = 0;
                    decimal.TryParse(r["计算量包含安全库存"].ToString(), out dec);
                    if (dd > dec) throw new Exception("转制令数量不可超过包含安全库存的参考量");

                    // decimal.TryParse(r["计算量包含安全库存"].ToString(), out dd);

                    //DataRow []xx=dt_displaymx.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
                    //if (xx.Length == 0) throw new Exception("产品编码:" + r["物料编码"].ToString() + "尚未关联销售订单,请确认");
                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + this.Name + " fun_checkToZL");
                throw new Exception(ex.Message);
            }
        }

        //转生产制令
        private void fun_ToSHCZL()
        {
            try
            {
                DataTable dt_zlmain = new DataTable();
                DataTable dt_zlmx = new DataTable();
                SqlDataAdapter da;
                string sql = "select * from 生产记录生产制令表 where 1<>1";
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_zlmain);
                sql = "select * from 生产记录生产制令子表 where 1<>1";
                da = new SqlDataAdapter(sql, strcon);
                da.Fill(dt_zlmx);

                foreach (DataRow r in dt_制令主表_附表.Rows)
                {
                    //转生产制令主表
                    DataRow r_zlmian = dt_zlmain.NewRow();
                    r_zlmian["GUID"] = System.Guid.NewGuid();
                    DateTime time = CPublic.Var.getDatetime();
                    //标准类型的单号
                    str_制令单号 = string.Format("PM{0}{1:00}{2:00}{3:0000}", time.Year, time.Month, time.Day, CPublic.CNo.fun_得到最大流水号("PM", time.Year, time.Month));
                    r_zlmian["生产制令单号"] = str_制令单号;
                    r_zlmian["生产计划单号"] = "";
                    r_zlmian["生产制令类型"] = "计划类型";
                    r_zlmian["物料编码"] = r["物料编码"];
                    r_zlmian["物料名称"] = r["物料名称"];
                    r_zlmian["图纸编号"] = r["图纸编号"];
                    r_zlmian["规格型号"] = r["规格型号"];
                    string x = string.Format("select  仓库号,仓库名称 from 基础数据物料信息表  where 物料编码='{0}'", r["物料编码"]);
                    DataTable temp = CZMaster.MasterSQL.Get_DataTable(x, strcon);
                    // 这里如果成品半成品对应两个仓库没法选择 默认仓库
                    r_zlmian["仓库号"] = temp.Rows[0]["仓库号"];
                    r_zlmian["仓库名称"] = temp.Rows[0]["仓库名称"];


                    r_zlmian["特殊备注"] = r["特殊备注"];
                    r_zlmian["生产车间"] = r["车间编号"];
                    r_zlmian["制令数量"] = r["输入生产数量"];
                    r_zlmian["未排单数量"] = r["输入生产数量"];
                    r_zlmian["计划生产量"] = r["计算量"];
                    r_zlmian["日期"] = time;
                    r_zlmian["操作人员"] = CPublic.Var.localUserName;
                    r_zlmian["操作人员ID"] = CPublic.Var.LocalUserID;
                    r_zlmian["加急状态"] = r["加急状态"];
                    dt_zlmain.Rows.Add(r_zlmian);
                    //转生产制令子表  这里要改！！！ 因为可能是多条 每条都关联了销售明细 
                    //  if (dt_displaymx == null) continue;
                    DataRow[] r_detail = dt_displaymx.Select(string.Format("物料编码='{0}'", r["物料编码"].ToString()));
                    if (r_detail.Length > 0)
                    {
                        foreach (DataRow r1 in r_detail)
                        {
                            DataRow r_zlmx = dt_zlmx.NewRow();
                            r_zlmx["GUID"] = System.Guid.NewGuid();
                            r_zlmx["生产制令单号"] = str_制令单号;

                            r_zlmx["销售订单明细号"] = r1["销售订单明细号"];
                            r_zlmx["销售订单号"] = r1["销售订单号"];
                            r_zlmx["物料编码"] = r1["物料编码"];
                            r_zlmx["销售备注"] = r1["备注"];
                            r_zlmx["物料名称"] = r1["物料名称"];
                            r_zlmx["规格型号"] = r1["规格型号"];
                            //r_zlmx["n原ERP规格型号"] = r1["n原ERP规格型号"];
                            r_zlmx["特殊备注"] = r1["特殊备注"];
                            r_zlmx["图纸编号"] = r1["图纸编号"];
                            r_zlmx["客户"] = r1["客户"];
                            r_zlmx["送达日期"] = r1["送达日期"];
                            r_zlmx["计量单位"] = r1["计量单位"];
                            r_zlmx["数量"] = r1["数量"];
                            dt_zlmx.Rows.Add(r_zlmx);
                        }
                    }


                }
                SqlConnection conn = new SqlConnection(strcon);
                conn.Open();
                SqlTransaction ts = conn.BeginTransaction("ZLD");
                SqlCommand cmd_zld = new SqlCommand("select * from 生产记录生产制令表 where 1<>1", conn, ts);
                SqlCommand cmd_zlmx = new SqlCommand("select * from 生产记录生产制令子表 where 1<>1", conn, ts);
                try
                {
                    //制令主表
                    da = new SqlDataAdapter(cmd_zld);
                    new SqlCommandBuilder(da);
                    da.Update(dt_zlmain);
                    //制令子表
                    da = new SqlDataAdapter(cmd_zlmx);
                    new SqlCommandBuilder(da);
                    da.Update(dt_zlmx);
                    ts.Commit();
                }
                catch
                {
                    ts.Rollback();
                }
            }
            catch (Exception ex)
            {
                MasterLog.WriteLog(ex.Message + " fun_ToSHCZL");
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 界面操作
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //dtP = StockCore.StockCorer.fun_计划_需生产(Convert.ToDateTime(bar_日期.EditValue));

                dtP = StockCore.StockCorer.fun_计划_需生产2(Convert.ToDateTime(bar_日期.EditValue));

                dtP.Columns.Add("选择", typeof(Boolean));
                dtP.Columns.Add("输入生产数量", typeof(Decimal));
                dtP.Columns.Add("加急状态", typeof(string));
                DataView dv = new DataView(dtP);
                dv.RowFilter = "计算量包含安全库存 > 0  ";
                gc2.DataSource = dv;
                foreach (DataRow dr in dtP.Rows)
                {
                    dr["加急状态"] = "正常";
                }
                gc_detailproduct.DataSource = new DataTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                gv2.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();

                DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);

                if (dr == null)
                {
                    throw new Exception("无数据");
                }
                string sql = string.Format("select 物料状态,更改预计完成时间 from 基础数据物料信息表 where 物料编码 = '{0}'", dr["物料编码"].ToString());

                DataTable t = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strcon);
                da.Fill(t);
                if (t.Rows[0]["物料状态"].ToString() == "更改")
                {
                    DateTime time = (DateTime)t.Rows[0]["更改预计完成时间"];
                    if (MessageBox.Show(string.Format("当前物料为更改状态，预计完成时间：{0}，是否继续？", time.ToString("yyyy-MM-dd")), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        try
                        {
                            if (dtP == null || dtP.Rows.Count <= 0)
                                throw new Exception("没有生产计划不能进行转生产制令操作！");
                            fun_selectJH();
                            fun_checkToZL();  //转制令检测
                            fun_ToSHCZL();  //转制令
                            dt_displaymx = dt_displaymx.Clone();
                            if (MessageBox.Show("转生产制令单成功，已保存！是否跳转生产制令界面？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                ERPproduct.frm生产制令表 fm = new ERPproduct.frm生产制令表(dr, str_制令单号);
                                CPublic.UIcontrol.AddNewPage(fm, "制令生效");
                            }
                            barLargeButtonItem1_ItemClick(null, null);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (dtP == null || dtP.Rows.Count <= 0)
                            throw new Exception("没有生产计划不能进行转生产制令操作！");
                        fun_selectJH();
                        fun_checkToZL();  //转制令检测
                        fun_ToSHCZL();  //转制令
                        dt_displaymx = dt_displaymx.Clone();
                        if (MessageBox.Show("转生产制令单成功，已保存！是否跳转生产制令界面？", "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            ERPproduct.frm生产制令表 fm = new ERPproduct.frm生产制令表(dr, str_制令单号);
                            CPublic.UIcontrol.Showpage(fm, "生产制令");
                        }
                        barLargeButtonItem1_ItemClick(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                if (xtraTabControl1.SelectedTabPage.Name == "xtraTabPage1")
                {
                    //gc.ExportToXls(saveFileDialog.FileName);
                }
                else
                {
                    gc2.ExportToXlsx(saveFileDialog.FileName);
                }
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        #region  关联销售单
        //明细回传值处理
        private void fun_detailDeal(DataTable dt, string danhao)
        {
            try
            {
                //给dt加上生产计划单号，关联的销售单号对应相应的生产计划单号
                dt.Columns.Add("生产计划单号");
                if (dt_制令子表 == null)  //明细的dt
                {
                    dt_制令子表 = dt.Clone();
                }
                if (dt_displaymx == null)
                {
                    dt_displaymx = dt.Clone();
                }
                //向总的明细dt中加入选择项
                dt_displaymx.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    r["生产计划单号"] = danhao;
                    dt_制令子表.Rows.Add(r.ItemArray);
                    dt_displaymx.Rows.Add(r.ItemArray);
                }
                gc_detailproduct.DataSource = dt_displaymx;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_detailDeal");
                throw new Exception(ex.Message);
            }
        }

        //新增明细的操作
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow r_focus = gv2.GetDataRow(gv2.FocusedRowHandle);
                if (!r_focus["选择"].Equals(true))
                {
                    throw new Exception("未选择物料不可关联订单");

                }

                //                if (dtP.Rows.Count <= 0)
                //                    throw new Exception("无生产计划，不可新增明细！");
                //   DataRow r = gv2.GetDataRow(gv2.FocusedRowHandle);
                //                if (dt_displaymx == null || dt_displaymx.Columns.Count == 0)
                //                {
                //                    string sql = string.Format(@"select 生产记录生产制令子表.*,[销售记录销售订单明细表].备注,反馈备注,原ERP物料编号 from 生产记录生产制令子表,销售记录销售订单明细表,基础数据物料信息表
                //    
                //                                        where 生产记录生产制令子表.销售订单明细号 =销售记录销售订单明细表.销售订单明细号  and 生产记录生产制令子表.物料编码=基础数据物料信息表.物料编码
                //
                //                                         and 1=2");
                //                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                //                    {
                //                        dt_displaymx = new DataTable();
                //                        da.Fill(dt_displaymx);

                //                    }
                //                }

                //选择关联的销售单，只能选择跟生产计划相一致的物料编码
                fm关联销售明细选择 fm = new fm关联销售明细选择(dt_displaymx, r_focus["物料编码"].ToString(), "");
                fm.ShowDialog();
                if (fm.dt != null)
                {
                    dt_displaymx = fm.dt;
                    dv_制令子 = new DataView(dt_displaymx);
                    dv_制令子.RowFilter = string.Format("物料编码='{0}'", r_focus["物料编码"].ToString());
                    gc_detailproduct.DataSource = dv_制令子;
                    // fun_detailDeal(dt_displaymx, r["物料编码"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //删除明细的操作
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {   //删除显示在界面上的明细
                if (dt_displaymx == null || dt_displaymx.Rows.Count <= 0)
                    throw new Exception("无明细可以删除,请先新增明细！");
                DataRow r = gv_关联订单.GetDataRow(gv_关联订单.FocusedRowHandle);
                if (MessageBox.Show(string.Format("你确定要删除明细号为\"{0}\"的明细吗？", r["销售订单明细号"].ToString()), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //删除总的明细dt中的该明细
                    DataRow[] dr = dt_displaymx.Select(string.Format("销售订单明细号='{0}'", r["销售订单明细号"].ToString()));
                    if (dr.Length > 0)
                    {
                        dr[0].Delete();
                    }
                    r.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void gv2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                if (e.FocusedRowHandle >= 0)
                {
                    DataRow r = gv2.GetDataRow(gv2.FocusedRowHandle);
                    dv_制令子.RowFilter = null;
                    dv_制令子.RowFilter = string.Format("物料编码={0}", r["物料编码"].ToString());
                    ////显示不同的明细
                    //if (dt_制令子表 != null && dt_制令子表.Rows.Count > 0)
                    //{
                    //    dt_displaymx = dt_制令子表.Clone();
                    //    dt_displaymx.Clear();
                    //    DataRow[] dr = dt_制令子表.Select(string.Format("生产计划单号 = '{0}'", r["物料编码"].ToString()));
                    //    foreach (DataRow r5 in dr)
                    //    {
                    //        dt_displaymx.Rows.Add(r5.ItemArray);
                    //    }
                    //    gc_detailproduct.DataSource = dt_displaymx;
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void 查看物料BOM信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow r = gv2.GetDataRow(gv2.FocusedRowHandle);
            Decimal dec;
            if (r["输入生产数量"] != DBNull.Value && r["输入生产数量"].ToString() != "")
            {
                dec = Convert.ToDecimal(r["输入生产数量"].ToString());
            }
            else
            {
                dec = 1;
            }
            ERPproduct.UI物料BOM详细数量 frm = new UI物料BOM详细数量(r["物料编码"].ToString().Trim(), dec);
            CPublic.UIcontrol.AddNewPage(frm, "详细数量");
        }

        private void 查看过往制令ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow r = gv2.GetDataRow(gv2.FocusedRowHandle);
            UI查看制令列表 UI = new UI查看制令列表(r["物料编码"].ToString().Trim());
            CPublic.UIcontrol.AddNewPage(UI, "过往制令");
        }

        private void 过往通知出库记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow r = gv2.GetDataRow(gv2.FocusedRowHandle);
            UI查看出库通知明细 ui = new UI查看出库通知明细(r["物料编码"].ToString().Trim());
            CPublic.UIcontrol.AddNewPage(ui, "过往通知出库记录");
        }

        private void gv2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gc2, new Point(e.X, e.Y));
                gv2.CloseEditor();
                this.BindingContext[dtP].EndCurrentEdit();
            }
        }

        private void gv_关联订单_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_关联订单.GetFocusedRowCellValue(gv_关联订单.FocusedColumn));
                e.Handled = true;
            }
        }

        private void 修改基础信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
            fm计划池修改基础信息 fm = new fm计划池修改基础信息(dr["物料编码"].ToString());

            fm.StartPosition = FormStartPosition.CenterScreen;
            fm.ShowDialog();

            if (fm.bl)
            {
                dr["计算量包含安全库存"] = fm.dec + Convert.ToDecimal(dr["计算量"].ToString());
                dr["库存下限"] = fm.dec;

                dr.AcceptChanges();
            }
        }

        private void gv2_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv2.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        private void gv2_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {

                if (cfgfilepath != "")
                {
                    gv2.SaveLayoutToXml(cfgfilepath + string.Format(@"\{0}.xml", this.Name));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void gv2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "选择" && Convert.ToBoolean(e.Value) == false)
            {
                DataRow r_focus = gv2.GetDataRow(gv2.FocusedRowHandle);
                DataView dv_temp = new DataView(dt_displaymx);
                dv_temp.RowFilter = string.Format("物料编码<>'{0}'", r_focus["物料编码"].ToString());
                dt_displaymx = dv_temp.ToTable();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() == "")
                {
                    DataTable ListM = new DataTable();
                     string s = "select  产品编码,子项编码  from 基础数据物料BOM表 ";
                     DataTable dt_bom = CZMaster.MasterSQL.Get_DataTable(s, strcon);
                     ListM = ERPorg.Corg.billofM(ListM, searchLookUpEdit1.EditValue.ToString(),true,dt_bom);

                    DataView dv = new DataView(dtP);
                      s = "计算量包含安全库存 > 0 and 物料编码 in (";

                    foreach (DataRow dr in ListM.Rows)
                    {
                        s = s + string.Format("'{0}',", dr["子项编码"]);
                    }
                    s = s.Substring(0, s.Length - 1) + ")";
                    dv.RowFilter = s;
                    gc2.DataSource = dv;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void gv2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv2.GetFocusedRowCellValue(gv2.FocusedColumn));
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtP);
            dv.RowFilter = "计算量包含安全库存 > 0  ";
            gc2.DataSource = dv;
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }





    }
}
