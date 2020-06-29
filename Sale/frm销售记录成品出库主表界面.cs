using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms; 
using System.Runtime.InteropServices;
namespace ERPSale
{
    public partial class frm销售记录成品出库主表界面 : UserControl
    {
        #region 成员
        DataTable dtM;
        DataRow drM;
        string strconn = CPublic.Var.strConn;
        string str_选择条件 = "";
        DataTable dtP;
        #endregion

        #region 自用类
        public frm销售记录成品出库主表界面()
        {
            InitializeComponent();
        }

        private void frm销售记录成品出库主表界面_Load(object sender, EventArgs e)
        {
            try
            {
            
                bar_销售单.EditValue = "";
                bar_日期后.EditValue =System.DateTime.Today;
                bar_日期前.EditValue = System.DateTime.Today.AddDays(-7);
                bar_记录状态.EditValue = "未完成";
                fun_载入成品出库单号();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
     
        private void gvM_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            drM = gvM.GetDataRow(gvM.FocusedRowHandle);
            if (drM == null) return;
            string str_成品出库单号 = drM["成品出库单号"].ToString();
            fun_载入明细(str_成品出库单号);

            //修改
            if (e.Clicks == 2)
            {
                
                //string str_成品出库单号 = drM["成品出库单号"].ToString();
                //新增界面
                //if (drM["生效"].ToString() == "未生效" && "用户" == "用户")
                if (drM["生效"].ToString().ToLower() == "false")
                {
                    frm销售记录成品出库详细界面 fm = new frm销售记录成品出库详细界面(str_成品出库单号, drM, dtM);
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    CPublic.UIcontrol.AddNewPage(fm, "成品出库");
                }
                //视图界面
                else
                {
                    frm销售记录成品出库详细界面_视图 fm = new frm销售记录成品出库详细界面_视图(drM, str_成品出库单号);
                    fm.Dock = System.Windows.Forms.DockStyle.Fill;
                    CPublic.UIcontrol.AddNewPage(fm, "成品出库视图");
                }
            }
        }
        #endregion

        #region 界面操作
        //刷新
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_载入成品出库单号();
        }

        //新增
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //drM = dtM.NewRow();
            //dtM.Rows.Add(drM);
            frm销售记录成品出库详细界面 fm = new frm销售记录成品出库详细界面();
            fm.Dock = System.Windows.Forms.DockStyle.Fill;
            CPublic.UIcontrol.AddNewPage(fm, "新增成品出库单"); 
        }
        #endregion

        #region 方法
        private void fun_载入成品出库单号()
        {
            try
            {
                fun_搜索组合();
                string sql = string.Format(@"select sczb.*,a.销售订单号,客户订单号 from 销售记录成品出库单主表 sczb,(select 成品出库单号,left(销售订单明细号,14)销售订单号 
                                    from  销售记录成品出库单明细表 group by 成品出库单号,left(销售订单明细号,14)) a,销售记录销售订单主表 
                            where sczb.成品出库单号=a.成品出库单号  and 销售记录销售订单主表.销售订单号=a.销售订单号 and {0}", str_选择条件);
                   //出库单主表跟上销售单显示，但是之前有一个出库通知单 分几次出 和 一个出库单出好几个出库通知单 所以 这边查出来会有 相同出库单关联了不同的 销售单的问题
//                string sql_1 = string.Format(@" select 销售记录成品出库单主表.*,v.销售单 from 销售记录成品出库单主表,
//                                            (select a.成品出库单号,a.出库通知单号,left(销售订单明细号,14)销售单  from
//                                        (select 成品出库单号,出库通知单号 from 销售记录成品出库单明细表 group by 成品出库单号,出库通知单号)a,销售记录销售出库通知单明细表
//                                    where a.出库通知单号=销售记录销售出库通知单明细表.出库通知单号 group by  a.成品出库单号,a.出库通知单号,left(销售订单明细号,14))v
//                                    where 销售记录成品出库单主表.成品出库单号=v.成品出库单号
//                                    order by 成品出库单号");
                dtM = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dtM);
                gcM.DataSource = dtM;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库主表界面_fun_载入成品出库单号");
            }
        }
        private void fun_载入明细( string str_成品出库号)
        {
            try
            {
               dtP = new DataTable();
                string sql = "";
                sql = string.Format(@"select 销售记录成品出库单明细表.* from 销售记录成品出库单明细表  where 成品出库单号 = '{0}'", str_成品出库号);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    da.Fill(dtP);
                    gcP.DataSource = dtP;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库明细");
            }
        }
        private void fun_搜索组合()
        {
            try
            {
                //视图权限
                //DataTable dt_销售人员 = ERPorg.Corg.fun_hr("仓库", CPublic.Var.LocalUserID);

                //str_选择条件 = "";
                //if (CPublic.Var.LocalUserTeam != "管理员")
                //{
                //    if (dt_销售人员.Rows.Count != 0)
                //    {
                //        str_选择条件 += " ( ";
                //        foreach (DataRow r_x in dt_销售人员.Rows)
                //        {
                //            str_选择条件 += "操作员ID = '" + r_x["工号"].ToString().Trim() + "' or ";
                //        }
                //        str_选择条件 = str_选择条件.Substring(0, str_选择条件.Length - 3);
                //        str_选择条件 = str_选择条件 + " ) ";
                //        str_选择条件 += " and ";
                //        //str_选择条件 += " 操作员 = '" + CPublic.Var.localUserName + "' and ";
                //    }
                //    else
                //    {
                //        throw new Exception("你没有该视图权限");
                //    }
                //}
                str_选择条件 = "";
                if (bar_销售单.EditValue != null && bar_销售单.EditValue.ToString() != "")
                {
                    str_选择条件 = str_选择条件 + " sczb.成品出库单号 = '" + bar_销售单.EditValue.ToString() + "' and";
                }
                else
                {
                    if (bar_日期前.EditValue != null && bar_日期前.EditValue.ToString() != "" && bar_日期后.EditValue != null && bar_日期后.EditValue.ToString() != "")
                    {
                        str_选择条件 = str_选择条件 + " sczb.日期 >= '" + ((DateTime)bar_日期前.EditValue).ToString("yyyy-MM-dd HH:mm:ss") + "' and sczb.日期 <= '" + (Convert.ToDateTime(bar_日期后.EditValue).AddDays(1).AddSeconds(-1)).ToString("yyyy-MM-dd HH:mm:ss") + "' and";
                    }
                    if (bar_记录状态.EditValue != null && bar_记录状态.EditValue.ToString() != "")
                    {
                        if (bar_记录状态.EditValue.ToString() == "已生效")
                        {
                            str_选择条件 = str_选择条件 + " sczb.生效 = 1 and";
                        }
                        if (bar_记录状态.EditValue.ToString() == "未生效")
                        {
                            str_选择条件 = str_选择条件 + " sczb.生效 = 0 and";
                        }
                        if (bar_记录状态.EditValue.ToString() == "已完成")
                        {
                            str_选择条件 = str_选择条件 + " sczb.完成 = 1 and";
                        }
                        if (bar_记录状态.EditValue.ToString() == "未完成")
                        {
                            str_选择条件 = str_选择条件 + " sczb.完成 = 0 and";
                        }
                        if (bar_记录状态.EditValue.ToString() == "已废弃")
                        {
                            str_选择条件 = str_选择条件 + " sczb.废弃 = 1 and";
                        }
                        if (bar_记录状态.EditValue.ToString() == "未废弃")
                        {
                            str_选择条件 = str_选择条件 + " sczb.废弃 = 0 and";
                        }
                        if (bar_记录状态.EditValue.ToString() == "所有")
                        {

                        }

                    }
                }
                str_选择条件 = str_选择条件.Substring(0, str_选择条件.Length - 4);
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库主表界面_fun_搜索组合");
            }
        }
        #endregion

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gvM_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gvM.GetFocusedRowCellValue(gvM.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gvP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gvP.GetFocusedRowCellValue(gvP.FocusedColumn));
                e.Handled = true;
            }
        }

        private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
        private void fun_打印出厂检验记录(string str_打印机)
        {
            DataTable dt_主 = new DataTable();
            dt_主.Columns.Add("物料编码");
            dt_主.Columns.Add("物料名称");
            dt_主.Columns.Add("规格");
            dt_主.Columns.Add("大类");
            dt_主.Columns.Add("小类");
            dt_主.Columns.Add("发货数量");
            dt_主.Columns.Add("合格数");
            dt_主.Columns.Add("生产者");
            dt_主.Columns.Add("班组");
            dt_主.Columns.Add("部门");
            dt_主.Columns.Add("生产日期");
            foreach (DataRow r in dtP.Rows)
            {
                #region dtM
                string sql = string.Format("select 物料编码,物料名称,规格型号,大类,小类 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"]);
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                DataTable dt = new DataTable();
                dt_主.Clear();
                da.Fill(dt);
                DataRow dr = dt_主.NewRow();
                dt_主.Rows.Add(dr);
                dr["物料编码"] = dt.Rows[0]["物料编码"];
                dr["物料名称"] = dt.Rows[0]["物料名称"];
                //dr["规格"] = dtM.Rows[0]["规格"];
                dr["规格"] = dt.Rows[0]["规格型号"];

                dr["大类"] = dt.Rows[0]["大类"];
                dr["小类"] = dt.Rows[0]["小类"];

                sql = string.Format(@"select 生产记录生产检验单主表.生产检验单号,(生产记录生产检验单主表.负责人员) as 生产者,(人事基础员工表.岗位) as 班组,人事基础员工表.部门,(生产记录生产检验单主表.检验日期) as 生产日期 from 生产记录生产检验单主表 
                    left join 人事基础员工表 on 生产记录生产检验单主表.负责人员ID = 人事基础员工表.员工号 where 生产记录生产检验单主表.物料编码 = '{0}' order by 生产记录生产检验单主表.检验日期 desc", r["物料编码"]);
                da = new SqlDataAdapter(sql, strconn);
                dt = new DataTable();
                da.Fill(dt);
                try
                {
                    dr["生产者"] = dt.Rows[0]["生产者"];
                    dr["班组"] = dt.Rows[0]["班组"];
                    dr["部门"] = dt.Rows[0]["部门"];
                    dr["生产日期"] = dt.Rows[0]["生产日期"];
                }
                catch
                {
                    dr["生产者"] = "无";
                    dr["班组"] = "无";
                    dr["部门"] = "无";
                    dr["生产日期"] = System.DateTime.Now;
                }
                dr["发货数量"] = r["出库数量"];
                dr["合格数"] = r["出库数量"];
                #endregion

                #region dr_传
                try
                {
                    sql = string.Format("select * from 成品检验检验记录明细表 where 生产检验单号 = '{0}'", dt.Rows[0]["生产检验单号"]);
                }
                catch
                {
                    sql = "select * from 成品检验检验记录明细表 where 1<>1";
                }
                da = new SqlDataAdapter(sql, strconn);
                DataTable t = new DataTable();
                da.Fill(t);
                #endregion

                #region 打印
                ItemInspection.print_FMS.fun_print_出厂检验报告_原("销售", dt_主, t, 1, str_打印机, 0, 0);

              //   ItemInspection.print_FMS.fun_print_出厂检验报告_原(dt_主, t, 1, str_打印机);
                #endregion
            }
        }
        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(String Name);
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                //this.printDialog1.Document = this.printDocument1;
                //if (MessageBox.Show("是否打印送货单？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //{

                //    DialogResult dr = this.printDialog1.ShowDialog();
                //    if (dr == DialogResult.OK)
                //    {
                //        //Get the Copy times
                //        int nCopy = this.printDocument1.PrinterSettings.Copies;
                //        //Get the number of Start Page
                //        int sPage = this.printDocument1.PrinterSettings.FromPage;
                //        //Get the number of End Page
                //        int ePage = this.printDocument1.PrinterSettings.ToPage;
                //        string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                //        SetDefaultPrinter(PrinterName);
                //        //* 加
                //        int a = Convert.ToInt32(dtP.Rows.Count) / 8;
                //        int b = Convert.ToInt32(dtP.Rows.Count) % 8;
                //        if (a == 0)
                //        {
                //            a = 1;
                //        }
                //        else if (b != 0)
                //        {
                //            a = a + 1;
                //        }

                //        ItemInspection.print_FMS.fun_print_送货单_修改(dtP, a);
                //        //*加
                //        //ItemInspection.print_FMS.fun_print_送货单(dr_传,1);
                //    }
                //}
                if (MessageBox.Show("是否打印出厂报告？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    printDialog1 = new System.Windows.Forms.PrintDialog();
                    this.printDialog1.Document = this.printDocument1;
                    DialogResult dr = this.printDialog1.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        //Get the Copy times
                        int nCopy = this.printDocument1.PrinterSettings.Copies;
                        //Get the number of Start Page
                        int sPage = this.printDocument1.PrinterSettings.FromPage;
                        //Get the number of End Page
                        int ePage = this.printDocument1.PrinterSettings.ToPage;
                        string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                       // SetDefaultPrinter(PrinterName);

                        fun_打印出厂检验记录(PrinterName);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "出库打印失败");
            }
        }
        private void fun_search_销售()
        {
            string sql = string.Format(@"select * from 销售记录成品出库单主表 where 成品出库单号 in 
            (select  [成品出库单号]  from [销售记录成品出库单明细表] where left(销售订单明细号,14) like '%{0}%')", bar_销售单.EditValue.ToString());
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dtM = new DataTable();

                da.Fill(dtM);
                gcM.DataSource = dtM;
            }
        }
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bar_销售单.EditValue != null && bar_销售单.EditValue.ToString() != "")
                {
                    fun_search_销售();
                }
                else
                {
                    throw new Exception("未输入销售号");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("是否打印送货单？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {


                //编号 日期 
                //string sql = string.Format("select * from 销售记录成品出库单主表 where 成品出库单号 = '{0}'", dr_传.Rows[0]["成品出库单号"].ToString());  、

                string sql = string.Format(@"SELECT a.[成品出库单号],a.[销售订单明细号],a.[送货方式],a.客户,b.销售订单号,c.操作员,
                        a.生效日期,b.客户订单号 from [销售记录成品出库单明细表] a
                         left join  销售记录销售订单主表 b  on  left(a.销售订单明细号,14)=b.销售订单号 
                         left join 销售记录成品出库单主表 c on a.成品出库单号=c.成品出库单号 
                         where c.成品出库单号 = '{0}'", dtP.Rows[0]["成品出库单号"].ToString());
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);


               // ItemInspection.print_FMS.fun_print_送货单_修改(dtP, dt);

                ERPreport.frm送货单 frm = new ERPreport.frm送货单(dt,dtP );
                frm.ShowDialog();



                
            }
        }
        //19-7-24 撤销
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gvM.GetDataRow(gvM.FocusedRowHandle);
            DateTime t = CPublic.Var.getDatetime();

            try
            {
                if (MessageBox.Show(string.Format("是否确认撤销此单据？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string sql = string.Format("select * from  销售记录成品出库单主表 where 成品出库单号 = '{0}'",dr["成品出库单号"].ToString());
                    DataTable dt_成品出库主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    sql = string.Format("select * from 销售记录成品出库单明细表 where 成品出库单号 = '{0}'", dr["成品出库单号"].ToString());
                    DataTable dt_成品出库子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                    DataTable dt_出入库明细 = new DataTable();
                    DataTable dt_物料 = new DataTable();
                    DataTable dt_出库通知主 = new DataTable();
                    DataTable dt_出库通知子 = new DataTable();
                    DataTable dt_退货 = new DataTable();
                    DataTable dt_销售订单主 = new DataTable();
                    DataTable dt_销售订单子 = new DataTable();
                    //Boolean s_开票 = false;
                    DateTime dttt = Convert.ToDateTime(dr["生效日期"]);
                    if (t.Month != dttt.Month)
                    {
                        throw new Exception("该订单不是当月出库，不能撤回");
                    }
                    if (dt_成品出库子.Rows.Count > 0)
                    {
                        foreach(DataRow dr_出库子 in dt_成品出库子.Rows)
                        {
                            if (Convert.ToDecimal(dr_出库子["已开票数"]) > 0)
                            {
                                throw new Exception("该单据已有开票记录，不能撤销");
                            }
                            sql = string.Format("select * from 退货申请子表 where 出库明细号 ='{0}'", dr_出库子["成品出库单明细号"].ToString());
                            dt_退货 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                            if (dt_退货.Rows.Count > 0)
                            {
                                throw new Exception("该单据已有退货申请记录,不能撤销");
                            }
                            sql = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单明细号 = '{0}'",dr_出库子["出库通知单明细号"].ToString());
                            dt_出库通知子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                            if (dt_出库通知子.Rows.Count > 0)
                            {
                                dt_出库通知子.Rows[0]["已出库数量"] = Convert.ToDecimal(dt_出库通知子.Rows[0]["已出库数量"]) - Convert.ToDecimal(dr["已出库数量"]);
                                dt_出库通知子.Rows[0]["未出库数量"] = Convert.ToDecimal(dt_出库通知子.Rows[0]["未出库数量"]) + Convert.ToDecimal(dr["已出库数量"]);
                                dt_出库通知子.Rows[0]["完成"] = false;
                                dt_出库通知子.Rows[0]["完成日期"] = DBNull.Value;
                                sql = string.Format("select * from 销售记录销售出库通知单主表 where 出库通知单号 = '{0}'", dt_出库通知子.Rows[0]["出库通知单号"].ToString());
                                dt_出库通知主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                                dt_出库通知主.Rows[0]["完成"] = false;
                                dt_出库通知主.Rows[0]["完成日期"] = DBNull.Value;
                            }
                            sql = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号 = '{0}'",dr["销售订单明细号"].ToString());
                            dt_销售订单子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
