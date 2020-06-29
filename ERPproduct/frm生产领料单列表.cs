using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPproduct
{
    public partial class frm生产领料单列表 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string sql_人 = "";
        DataTable dt_视图权限 = new DataTable();
        DataTable dt_仓库;
        string sql_ck;

        public frm生产领料单列表()
        {
            InitializeComponent();
            
        }

        /// <summary>
        ///生产记录生产领料单列表 
        /// </summary>
        DataTable dtM;
       
#pragma warning disable IDE1006 // 命名样式
        private void frm生产领料单列表_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
                dt_仓库 = new DataTable();
                dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strconn);
 
                txt_GetMaterialsDan.EditValue = "";
                txt_time1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"));  //起始时间，往前推7天
                txt_time2.EditValue = Convert.ToDateTime(System.DateTime.Today.AddDays(1).ToString("yyyy-MM-dd")); //结束时间：系统时间的的23:59:59
                txt_DanjuState.EditValue = "所有";
                //fun_load();
                dt_视图权限 = ERPorg.Corg.fun_hr("仓库",CPublic.Var.LocalUserID);
                  
                fun_SearchData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void fun_load()
        //{
        //    string sql = "select * from 生产记录生产领料单主表 ";
        //    using (SqlDataAdapter da =new  SqlDataAdapter (sql,strconn))
        //    {
        //         dtM = new DataTable();
        //         da.Fill(dtM);
        //         gcM.DataSource = dtM;
        //    }
           
        //}

        //查找领料单的数据
#pragma warning disable IDE1006 // 命名样式
        private void fun_SearchData()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                string sql = "";
                //如果领料单号不为空的话，就以领料单号来查询该领料单
                if (txt_GetMaterialsDan.EditValue.ToString() != "")
                {
                    sql = string.Format(" 领料出库单号='{0}' and", txt_GetMaterialsDan.EditValue.ToString());
                }
                else   //如果领料单号为空的话，那就以时间跟单据状态来进行查询
                {
                    //如果两个时间都不为空的话才已时间进行查询
                    if (txt_time1.EditValue != null && txt_time2.EditValue != null && txt_time1.EditValue.ToString() != "" && txt_time2.EditValue.ToString() != "")
                    {
                        if (Convert.ToDateTime(txt_time1.EditValue) > Convert.ToDateTime(txt_time2.EditValue))
                            throw new Exception("第一个是起始时间，不能大于终止时间，请重新选择时间");
                        sql = sql + string.Format(" slz.修改日期>='{0}' and slz.修改日期<='{1}' and", txt_time1.EditValue, Convert.ToDateTime(txt_time2.EditValue).AddDays(1).ToString("yyyy-MM-dd "));
                    }
                    //单据状态
                    //if (txt_DanjuState.EditValue.ToString() == "未生效")  //生效
                    //{
                    //    sql = sql + string.Format(" 生效=0 and");
                    //}
                    //if (txt_DanjuState.EditValue.ToString() == "已生效")
                    //{
                    //    sql = sql + string.Format(" 生效=1 and");
                    //}
                    //if (txt_DanjuState.EditValue.ToString() == "已作废") //作废
                    //{
                    //    sql = sql + string.Format(" 作废=1 and");
                    //}
                    //if (txt_DanjuState.EditValue.ToString() == "未作废")
                    //{
                    //    sql = sql + string.Format(" 作废=0 and");
                    //}
                    if (txt_DanjuState.EditValue.ToString() == "未完成")  //完成
                    {
                        sql = sql + string.Format("slz.完成=0 and");
                    }
                    if (txt_DanjuState.EditValue.ToString() == "完成")
                    {
                        sql = sql + string.Format(" slz.完成=1 and");
                    }
                }
                sql = " where " + sql.Substring(0, sql.Length - 3);
                //if (dt_视图权限.Rows.Count == 0)
                //{

                    sql = string.Format(@"select slz.*,sdlz.领料类型 as 类型,base.规格型号 as 规格型号1 from 生产记录生产领料单主表 slz
                                         left join  生产记录生产工单待领料主表 sdlz on sdlz.待领料单号=slz.待领料单号
                                        left join 基础数据物料信息表 base on  base.物料编码=slz.物料编码   {0}", sql);
                //}
                //else 
                //{
                //     sql_人="and 生效人员ID in (";
                //    foreach (DataRow dr in dt_视图权限.Rows)
                //    {
                //      sql_人=sql_人+string.Format("'{0}',",dr["工号"]) ;
                //    }
                //    sql_人 = sql_人.Substring(0, sql_人.Length - 1) + ")";

                //    sql = string.Format(@"select slz.*,sdlz.领料类型 as 类型,base.规格型号 as 规格型号1 from 生产记录生产领料单主表 slz
                //                         left join  生产记录生产工单待领料主表 sdlz on sdlz.待领料单号=slz.待领料单号
                //                        left join 基础数据物料信息表 base on  base.物料编码=slz.物料编码   {0} {1}", sql, sql_人);
                //}
                SqlDataAdapter da;
                dtM = new DataTable();
                da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                gcM.DataSource = dtM;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + this.Name + " fun_SearchData");
                throw ex;
            }
        }


        #region  界面操作
        //查询操作
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_SearchData();
                if (dtM.Rows.Count <= 0)
                    throw new Exception("查无数据！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //领料单新增
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                frm生产领料单界面 frm = new frm生产领料单界面();
                CPublic.UIcontrol.AddNewPage(frm, "生产领料单界面");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //清空单号
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                txt_GetMaterialsDan.EditValue = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion


        #region  根据领料单列表，查询领料单的明细;1、已生效的领料单，查询的是领料单的视图，2、未生效的领料单，查询的是领料单的明细界面

        //查询明细
        //private void 查询明细ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (dtM == null || dtM.Rows.Count <= 0)
        //            throw new Exception("没有领料单，无法查询明细！");
        //        DataRow r = (this.BindingContext[dtM].Current as DataRowView).Row;
        //        if (r["完成"].Equals(true))  //已经生效的领料单，查询的是视图
        //        {

        //        }
        //        else
        //        {
        //            frm生产领料单界面 frm = new frm生产领料单界面(r["领料出库单号"].ToString());
        //            CPublic.UIcontrol.AddNewPage(frm, "生产领料单界面");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void gv_linliaolist_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                DataRow r = gv_linliaolist.GetDataRow(gv_linliaolist.FocusedRowHandle);
                fun_load(r["领料出库单号"].ToString());


                if (e.Clicks == 2)
                {
                    frm生产领料列表视图 frm = new frm生产领料列表视图(r["领料出库单号"].ToString());
                 
                    CPublic.UIcontrol.AddNewPage(frm, "生产领料单界面");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查询明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dtM == null || dtM.Rows.Count <= 0)
                throw new Exception("没有领料单，无法查询明细！");
            DataRow r = gv_linliaolist.GetDataRow(gv_linliaolist.FocusedRowHandle);
            if (r["完成"].Equals(true))  //已经生效的领料单，查询的是视图
            {

            }
            else
            {
                frm生产领料列表视图 frm = new frm生产领料列表视图(r["领料出库单号"].ToString());
                CPublic.UIcontrol.AddNewPage(frm, "生产领料单界面");
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
                gcM.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_linliaolist_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }


#pragma warning disable IDE1006 // 命名样式
        private void fun_load(string str_领料出库单号)
#pragma warning restore IDE1006 // 命名样式
        {


            sql_ck = "and  dlmx.仓库号  in(";
            string sql_1 = "";
            if (dt_仓库.Rows.Count == 0)
            {

                sql_1 = string.Format(@"select llmx.*,base.规格型号 as 规格型号1,库存总数,dlmx.仓库名称,kc.货架描述
                                        from 生产记录生产领料单明细表 llmx,基础数据物料信息表 base,仓库物料数量表 kc,生产记录生产工单待领料明细表 dlmx
                where llmx.物料编码=base.物料编码 and base.物料编码=kc.物料编码  and  dlmx.待领料单明细号=llmx.待领料单明细号 and kc.仓库号=dlmx.仓库号 and  领料出库单号='{0}' ", str_领料出库单号);
            }
            else
            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }
                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
                sql_1 = string.Format(@"select llmx.*,base.仓库名称,库存总数,base.规格型号 as 规格型号1
                                    from 生产记录生产领料单明细表 llmx,基础数据物料信息表 base,仓库物料数量表 kc,生产记录生产工单待领料明细表 dlmx
                where llmx.物料编码=base.物料编码 and base.物料编码=kc.物料编码 and  dlmx.待领料单明细号=llmx.待领料单明细号 and kc.仓库号=dlmx.仓库号
                and  领料出库单号='{0}' {1} order by base.规格型号 ", str_领料出库单号, sql_ck);
            }


            using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strconn))
            {
                DataTable  dt  = new DataTable();
                da.Fill(dt);
                gc.DataSource = dt;
            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_linliaolist_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_linliaolist.GetFocusedRowCellValue(gv_linliaolist.FocusedColumn));
                e.Handled = true;
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_KeyDown(object sender, KeyEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv.GetFocusedRowCellValue(gv.FocusedColumn));
                e.Handled = true;
            }
        }















    }
}
