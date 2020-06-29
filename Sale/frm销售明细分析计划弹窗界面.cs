using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;


namespace ERPSale
{
    public partial class frm销售明细分析计划弹窗界面 : UserControl
    {
        #region 成员
        /// <summary>
        /// 传过来的dt采购，由计算生成
        /// </summary>
        DataTable dt1_采购 = null;
        /// <summary>
        /// 传过来的dt生产，由计算生成
        /// </summary>
        DataTable dt2_生产 = null;
        /// <summary>
        /// 传过来的dt_物料数量，生效MRP3种数量
        /// </summary>
        DataTable dt3 = null;
        /// <summary>
        /// 传过来的dM，用于区分明细是否以计算 
        /// </summary>
        DataTable dt4 = null;
        /// <summary>
        /// 由 采购记录采购计划表 读取
        /// </summary>
        DataTable dtM_采购;
        /// <summary>
        /// 由 生产记录生产计划表 读取
        /// </summary>
        DataTable dtM_生产;
        string strconn = CPublic.Var.strConn;

        public Boolean bl_保存确认 = false;
        #endregion

        #region 自用类
        public frm销售明细分析计划弹窗界面()
        {
            InitializeComponent();

        }

        public frm销售明细分析计划弹窗界面(DataTable dt, DataTable dtt, DataTable dttt, DataTable dtttt)
        {
            InitializeComponent();
          
            dt1_采购 = dt;
            dt2_生产 = dtt;
            dt3 = dttt;
            dt4 = dtttt;
            try
            {
                dt1_采购.Columns.Add("库存有效数量");
                dt1_采购.Columns.Add("序号");
                dt1_采购.Columns.Add("单位");
                dt1_采购.Columns.Add("图纸编号1");
                dt2_生产.Columns.Add("库存有效数量");
                dt2_生产.Columns.Add("序号");
                dt2_生产.Columns.Add("单位");
            }
            catch { }
            int i = 0;
            foreach (DataRow dr in dt1_采购.Rows)
            {
                dr["序号"] = i++;
                //string sql = string.Format("select 库存总数 from 仓库物料数量表 where 物料编码 = '{0}'", dr["物料编码"].ToString());
                string sql = string.Format(@" select 仓库物料数量表.库存总数,基础数据物料信息表.计量单位,基础数据物料信息表.图纸编号 from 仓库物料数量表,基础数据物料信息表 
                where 仓库物料数量表.物料编码 = 基础数据物料信息表.物料编码 and 基础数据物料信息表.物料编码 = '{0}'", dr["物料编码"].ToString());
                DataTable dt_1 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_1);
                try
                {
                    dr["库存有效数量"] = Convert.ToDecimal(dt_1.Rows[0]["库存总数"]);
                    dr["单位"] = dt_1.Rows[0]["计量单位"];
                    dr["图纸编号1"] = dt_1.Rows[0]["图纸编号"];
                }
                catch {
                    CZMaster.MasterLog.WriteLog(string.Format("不存在物料{0}", dr["物料编码"].ToString()), "MRP分析界面");
                    continue;
                }
            }

            foreach (DataColumn dc in dt1_采购.Columns)
            {
                if (dc.ColumnName == "总需数量" || dc.ColumnName == "物料数量" || dc.ColumnName == "库存有效数量")
                {
                    foreach (DataRow dr in dt1_采购.Rows)
                    {
                        if (dr[dc.ColumnName] != DBNull.Value)
                        {
                            dr[dc.ColumnName] = Convert.ToDecimal(dr[dc.ColumnName]).ToString("0.0000");
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            gc1.DataSource = dt1_采购;
            int ii = 0;
            foreach (DataRow r in dt2_生产.Rows)
            {
                r["序号"] = ii++;
                if (r["物料类型"].ToString() == "半成品")
                {
                    try
                    {
                        r["订单数量"] = Convert.ToDecimal(r["订单数量"]) * Convert.ToDecimal(r["欠缺数量"]);
                    }
                    catch
                    {
                        r["订单数量"] = 0;
                    }
                    if (r["欠缺数量"] == DBNull.Value)
                    {
                        r["欠缺数量"] = 0;
                    }
                }
                //string sql = string.Format("select 库存总数 from 仓库物料数量表 where 物料编码 = '{0}'", r_x["物料编码"].ToString());
                string sql = string.Format(@" select 仓库物料数量表.库存总数,基础数据物料信息表.计量单位 from 仓库物料数量表,基础数据物料信息表 
                where 仓库物料数量表.物料编码 = 基础数据物料信息表.物料编码 and 基础数据物料信息表.物料编码 = '{0}'", r["物料编码"].ToString());
                DataTable dt_2 = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_2);
                try
                {
                    r["库存有效数量"] = Convert.ToDecimal(dt_2.Rows[0]["库存总数"]);
                    r["单位"] = dt_2.Rows[0]["计量单位"];
                }
                catch
                {
                    CZMaster.MasterLog.WriteLog(string.Format("不存在物料{0}", r["物料编码"].ToString()), "MRP分析界面");
                    continue;
                }
            }
            gc2.DataSource = dt2_生产;
        }

        private void frm采购_生产计划弹窗界面_Load(object sender, EventArgs e)
        {

            xtraTabControl1.SelectedTabPage = xtraTabPage2;
            fun_载入();
        }
        #endregion

        #region 方法
        private void fun_载入()
        {
            string sql = "select * from 采购记录采购计划表";
            dtM_采购 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dtM_采购);

            string sql2 = "select * from 生产记录生产计划表";
            dtM_生产 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, strconn);
            da2.Fill(dtM_生产);
        }

        private void fun_记录缺料情况()
        {
            try
            {
                string str_销售订单号 = "";
                foreach (DataRow rr in dt4.Rows)
                {
                    str_销售订单号 = rr["销售订单号"].ToString() + "|";
                }
                str_销售订单号 = str_销售订单号.Substring(0, str_销售订单号.Length - 1);

                DataTable dt_缺料 = new DataTable();
                string sql = "select * from 销售订单分析缺料记录表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_缺料);
                foreach (DataRow r in dt1_采购.Rows)
                {
                    if (Convert.ToDecimal(r["物料数量"]) > 0)
                    {
                        DataRow dr = dt_缺料.NewRow();
                        dt_缺料.Rows.Add(dr);
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["销售订单号"] = str_销售订单号;
                        dr["物料编码"] = r["物料编码"];
                        dr["物料名称"] = r["物料名称"];
                        dr["数量"] = r["物料数量"];
                        dr["日期"] = System.DateTime.Now;
                    }
                }

                new SqlCommandBuilder(da);
                da.Update(dt_缺料);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm采购_生产计划弹窗界面_fun_记录缺料情况");
            }
        }

        private void fun_保存_采购计划()
        {
            try
            {
                foreach (DataRow r in dt1_采购.Rows)
                {
                    DataRow[] ds = dtM_采购.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString().Trim()));
                    if (ds.Length > 0)
                    {
                        ds[0]["数量"] = Convert.ToDecimal(ds[0]["数量"]) + Convert.ToDecimal(r["物料数量"]);
                        if (Convert.ToDecimal(ds[0]["未完成采购数量"]) < 0) ds[0]["未完成采购数量"] = 0;
                        ds[0]["未完成采购数量"] = Convert.ToDecimal(ds[0]["未完成采购数量"]) + Convert.ToDecimal(r["物料数量"]);
                        ds[0]["总需数量"] = Convert.ToDecimal(ds[0]["总需数量"]) + Convert.ToDecimal(r["总需数量"]);
                        ds[0]["日期"] = System.DateTime.Now;
                    }
                    else
                    {
                        DataRow dr = dtM_采购.NewRow();
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["采购计划明细号"] = "MRP_PS_" + r["物料编码"].ToString();
                        dr["采购计划类型"] = "MRP类型";
                        dr["物料编码"] = r["物料编码"].ToString().Trim();
                        dr["物料名称"] = r["物料名称"].ToString();
                        dr["规格型号"] = r["规格型号"].ToString();
                        dr["图纸编号"] = r["图纸编号"].ToString();
                        dr["数量"] = Convert.ToDecimal(r["物料数量"]);
                        dr["已生成采购数量"] = (Decimal)0;
                        dr["未完成采购数量"] = Convert.ToDecimal(r["物料数量"]);
                        dr["日期"] = System.DateTime.Now;
                        dr["操作人员"] = CPublic.Var.localUserName;
                        dr["操作人员ID"] = CPublic.Var.LocalUserID;
                        dr["年"] = DateTime.Now.Year;
                        dr["月"] = DateTime.Now.Month;
                        //dr["库存有效数量"] = (Decimal)0;
                        dr["总需数量"] = Convert.ToDecimal(r["总需数量"]);
                        //dr["节点标记"] = r_x["节点标记"].ToString();
                        dr["是否生成"] = "否";
                        dtM_采购.Rows.Add(dr);
                    }
                }        
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm采购_生产计划弹窗界面_fun_保存_采购计划");
                throw ex;
            }
        }

        private void fun_保存_生产计划()
        {
            try
            {
                foreach (DataRow r in dt2_生产.Rows)
                {
                    DataRow[] ds = dtM_生产.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString().Trim()));
                    if (ds.Length > 0)
                    {
                        int count = 0;
                        foreach (DataRow rrr in ds)
                        {
                            if (rrr["操作人员ID"].ToString() == CPublic.Var.LocalUserID)
                            {
                                ds[0]["计划数量"] = Convert.ToDecimal(ds[0]["计划数量"]) + Convert.ToDecimal(r["欠缺数量"]);
                                if (Convert.ToDecimal(ds[0]["未生成数量"]) < 0) ds[0]["未生成数量"] = 0;
                                ds[0]["未生成数量"] = Convert.ToDecimal(ds[0]["未生成数量"]) + Convert.ToDecimal(r["欠缺数量"]);
                                ds[0]["规格型号"] = r["规格型号"].ToString();
                                ds[0]["原规格型号"] = r["原规格型号"].ToString();
                                ds[0]["日期"] = System.DateTime.Now;
                            }
                            else
                            {
                                count++;
                            }
                        }
                        if (count == ds.Length)
                        {
                            string sql11 = string.Format("select 产品线 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                            DataTable dt11 = new DataTable();
                            SqlDataAdapter da11 = new SqlDataAdapter(sql11, strconn);
                            da11.Fill(dt11);
                            DataRow dr = dtM_生产.NewRow();
                            dr["GUID"] = System.Guid.NewGuid();
                            dr["生产计划单号"] = "MRP_PP_" + r["物料编码"].ToString() + "_" + CPublic.Var.LocalUserID;
                            dr["生产计划类型"] = "MRP类型";
                            dr["物料编码"] = r["物料编码"].ToString();
                            dr["物料名称"] = r["物料名称"].ToString();
                            dr["规格型号"] = r["规格型号"].ToString();
                            dr["原规格型号"] = r["原规格型号"].ToString();
                            dr["图纸编号"] = r["图纸编号"].ToString();
                            dr["特殊备注"] = r["特殊备注"].ToString();
                            try
                            {
                                dr["生产线"] = dt11.Rows[0]["产品线"].ToString();
                            }
                            catch { }
                            dr["物料类型"] = r["物料类型"].ToString();
                            if (r["欠缺数量"] == DBNull.Value)
                            {
                                r["欠缺数量"] = 0;
                            }
                            dr["计划数量"] = Convert.ToDecimal(r["欠缺数量"]);
                            dr["已生成数量"] = (Decimal)0;
                            dr["未生成数量"] = Convert.ToDecimal(r["欠缺数量"]);
                            dr["日期"] = System.DateTime.Now;
                            dr["操作人员"] = CPublic.Var.localUserName;
                            dr["操作人员ID"] = CPublic.Var.LocalUserID;
                            dtM_生产.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        string sql11 = string.Format("select 产品线 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                        DataTable dt11 = new DataTable();
                        SqlDataAdapter da11 = new SqlDataAdapter(sql11, strconn);
                        da11.Fill(dt11);
                        DataRow dr = dtM_生产.NewRow();
                        dr["GUID"] = System.Guid.NewGuid();
                        dr["生产计划单号"] = "MRP_PP_" + r["物料编码"].ToString() + "_" + CPublic.Var.LocalUserID;
                        dr["生产计划类型"] = "MRP类型";
                        dr["物料编码"] = r["物料编码"].ToString();
                        dr["物料名称"] = r["物料名称"].ToString();
                        dr["规格型号"] = r["规格型号"].ToString();
                        dr["原规格型号"] = r["原规格型号"].ToString();
                        dr["图纸编号"] = r["图纸编号"].ToString();
                        dr["特殊备注"] = r["特殊备注"].ToString();
                        try
                        {
                            dr["生产线"] = dt11.Rows[0]["产品线"].ToString();
                        }
                        catch { }
                        dr["物料类型"] = r["物料类型"].ToString();
                        if (r["欠缺数量"] == DBNull.Value)
                        {
                            r["欠缺数量"] = 0;
                        }
                        dr["计划数量"] = Convert.ToDecimal(r["欠缺数量"]);
                        dr["已生成数量"] = (Decimal)0;
                        dr["未生成数量"] = Convert.ToDecimal(r["欠缺数量"]);
                        dr["日期"] = System.DateTime.Now;
                        dr["操作人员"] = CPublic.Var.localUserName;
                        dr["操作人员ID"] = CPublic.Var.LocalUserID;
                        dtM_生产.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm采购_生产计划弹窗界面_fun_保存_生产计划");
                throw ex;
            }
        }

        /// <summary>
        /// 保存采购计划、生产计划，叠加MRP计划数量（2种）
        /// </summary>
        private void fun_保存()
        {
            SqlConnection conn = new SqlConnection(strconn);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("生效");

            fun_保存_采购计划(); 
            fun_记录缺料情况();
            fun_保存_生产计划();
            string sql_采购 = "select * from 采购记录采购计划表 where 1<>1";
            SqlCommand cmd_采购 = new SqlCommand(sql_采购, conn, ts);
            SqlDataAdapter da_采购 = new SqlDataAdapter(cmd_采购);
            new SqlCommandBuilder(da_采购);

            string sql_生产 = "select * from 生产记录生产计划表 where 1 <> 1";
            SqlCommand cmd_生产 = new SqlCommand(sql_生产, conn, ts);
            SqlDataAdapter da_生产 = new SqlDataAdapter(cmd_生产);
            new SqlCommandBuilder(da_生产);

            DataTable dt_销售订单明细 = new DataTable();
            foreach (DataRow r in dt4.Rows)
            {
                string sql = string.Format("select * from 销售记录销售订单明细表 where GUID = '{0}'", r["GUID"].ToString());
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt_销售订单明细);
                DataRow[] ds = dt_销售订单明细.Select(string.Format("GUID = '{0}'", r["GUID"].ToString()));
                ds[0]["已计算"] = true;
            }
            string sql_已计算 = "select * from 销售记录销售订单明细表 where 1<>1";
            SqlCommand cmd_已计算 = new SqlCommand(sql_已计算, conn, ts);
            SqlDataAdapter da_已计算 = new SqlDataAdapter(cmd_已计算);
            new SqlCommandBuilder(da_已计算);

            string sql_MRP = "select * from 仓库物料数量表 where 1<>1";
            SqlCommand cmd_MRP = new SqlCommand(sql_MRP, conn, ts);
            SqlDataAdapter da_MRP = new SqlDataAdapter(cmd_MRP);
            new SqlCommandBuilder(da_MRP);
            foreach (DataRow r in dt1_采购.Rows)
            {
                try
                {
                    DataRow[] ds = dt3.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                    ds[0]["MRP计划采购量"] = Convert.ToDecimal(ds[0]["MRP计划采购量"]) + Convert.ToDecimal(r["物料数量"]);

                    string sql = string.Format(@"select 子项编码 ,子项名称,sum((基础数据物料BOM表.数量*a.成品数量)) as 总量 from  基础数据物料BOM表,
                        (SELECT 物料编码,SUM([销售记录销售订单明细表].数量)as 成品数量 FROM [FMS].[dbo].[销售记录销售订单明细表]  where 明细完成=0 and 作废=0   
                        group by [销售记录销售订单明细表].物料编码)as a  where 产品编码= a.物料编码 and 子项编码 = '{0}' group by 子项编码 ,子项名称 ", r["物料编码"].ToString());
                    DataTable ttt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(ttt);
                    //if (Convert.ToDecimal(ds[0]["MRP库存锁定量"]) > (Convert.ToDecimal(ttt.Rows[0]["总量"]) + Convert.ToDecimal(ds[0]["受订量"])) && Convert.ToDecimal(ttt.Rows[0]["总量"]) > 0)
                    //    dtM.Rows[0]["MRP库存锁定量"] = Convert.ToDecimal(ttt.Rows[0]["总量"]) + Convert.ToDecimal(dtM.Rows[0]["受订量"]);

                    Decimal de = Convert.ToDecimal(ttt.Rows[0]["总量"]) + Convert.ToDecimal(ds[0]["受订量"]) + Convert.ToDecimal(ds[0]["未领量"]) -
                        Convert.ToDecimal(ds[0]["在制量"]) - Convert.ToDecimal(ds[0]["在途量"]) - Convert.ToDecimal(ds[0]["库存总数"]);
                    if (Convert.ToDecimal(ds[0]["MRP计划采购量"]) > de)
                    {
                        ds[0]["MRP计划采购量"] = de;
                    }
                }
                catch { }
            }
            foreach (DataRow r in dt2_生产.Rows)
            {
                try
                {
                    DataRow[] ds = dt3.Select(string.Format("物料编码 = '{0}'", r["物料编码"].ToString()));
                    ds[0]["MRP计划生产量"] = Convert.ToDecimal(ds[0]["MRP计划生产量"]) + Convert.ToDecimal(r["欠缺数量"]);
                    Decimal de = Convert.ToDecimal(ds[0]["受订量"]) + Convert.ToDecimal(ds[0]["未领量"]) -
                        Convert.ToDecimal(ds[0]["在制量"]) - Convert.ToDecimal(ds[0]["在途量"]) - Convert.ToDecimal(ds[0]["库存总数"]);
                    if (Convert.ToDecimal(ds[0]["MRP计划生产量"]) > de)
                    {
                        ds[0]["MRP计划生产量"] = de;
                    }
                }
                catch { }
            }

            try
            {
                da_采购.Update(dtM_采购);
                da_生产.Update(dtM_生产);
                da_MRP.Update(dt3);
                da_已计算.Update(dt_销售订单明细);
                ts.Commit();
            }
            catch (Exception ex)
            {
                ts.Rollback();
                throw ex;
            }
            //fun_记录缺料情况();
        }
        #endregion

        #region 界面操作
        //删除
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                {
                    DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                    dr.Delete();
                }
                if (xtraTabControl1.SelectedTabPage == xtraTabPage2)
                {
                    DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
                    dr.Delete();
                }
            }
            catch (Exception ex)
            {

            }
        }

        //保存
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string str = "";
                fun_保存();
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                str = "保存成功";
                bl_保存确认 = true;
                MessageBox.Show(str);

                //frm销售明细分析界面 frm = new frm销售明细分析界面();
                //CPublic.UIcontrol.ShowNewPage(frm, "计划分析");
            }
            catch (Exception ex)
            {
                barLargeButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                CZMaster.MasterLog.WriteLog(ex.Message);
                MessageBox.Show("保存失败,请刷新重试");

            }
        }

        //关闭
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
           
        }
        #endregion

        #region GH added
        private void 查看物料详细数量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);

            //ERPStock.frm仓库物料数量明细 frm = new ERPStock.frm仓库物料数量明细(dr["物料编码"].ToString());
            //CPublic.UIcontrol.AddNewPage(frm, "物料详细数量");
        }

        private void gv1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc1, new Point(e.X, e.Y));
                }
            }
            catch
            {

            }
        }

        private void gv2_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip2.Show(gc2, new Point(e.X, e.Y));
                }
            }
            catch
            {

            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DataRow dr = gv2.GetDataRow(gv2.FocusedRowHandle);
            //ERPStock.frm仓库物料数量明细 frm = new ERPStock.frm仓库物料数量明细(dr["物料编码"].ToString());
            //CPublic.UIcontrol.AddNewPage(frm, "物料详细数量");
        }
        #endregion

        private void gv1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv1.GetFocusedRowCellValue(gv1.FocusedColumn));
                e.Handled = true;
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
    }
}
