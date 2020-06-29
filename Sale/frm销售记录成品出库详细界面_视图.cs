using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Printing;

namespace ERPSale
{
    public partial class frm销售记录成品出库详细界面_视图 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        /// <summary>
        /// 明细表
        /// </summary>
        DataTable dtP = new DataTable();
        /// <summary>
        /// 新增订单：drM = dtM.NewRow()；修改订单：drM = gv.GetDataRow(gv.FocusedRowHandle);
        /// </summary>
        DataRow drM = null;
        /// <summary>
        /// 销售订单主表
        /// </summary>
        DataTable dtM = null;
        /// <summary>
        /// 新增明细：dr = dtM.NewRow()；
        /// </summary>
        DataRow dr = null;
        string str_成品出库号 = "";
        #endregion

        public frm销售记录成品出库详细界面_视图(DataRow dr, string s_成品出库单号)
        {
            InitializeComponent();
            drM = dr;
            str_成品出库号 = s_成品出库单号;
        }

        public frm销售记录成品出库详细界面_视图(string s_成品出库单号)
        {
            InitializeComponent();
            str_成品出库号 = s_成品出库单号;
            fun_载入主表();
        }

        private void frm销售记录成品出库详细界面_视图_Load(object sender, EventArgs e)
        {
            dataBindHelper1.DataFormDR(drM);
            fun_载入明细();
        }

        private void fun_载入主表()
        {
            try
            {
                string sql = string.Format("select * from 销售记录成品出库单主表 where 销售订单号 = '{0}'", str_成品出库号);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                drM = dt.Rows[0];
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库详细界面_视图_fun_载入主表");
            }
        }

        private void fun_载入明细()
        {
            try
            {
                string sql = "";
                sql = string.Format(@"select scmx.* from 销售记录成品出库单明细表 scmx
               
                where 成品出库单号 = '{0}'", str_成品出库号);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    da.Fill(dtP);
                    gcP.DataSource = dtP;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "frm销售记录成品出库详细界面_视图_fun_载入");
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
                string sql = string.Format("select 物料编码,物料名称,规格,n原ERP规格型号,大类,小类 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"]);
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

                 //ItemInspection.print_FMS.fun_print_出厂检验报告(dt_主, t, 1,str_打印机);
                #endregion
            }
        }
        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(String Name); //调用win api将指定名称的打印机设置为默认打印机
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                this.printDialog1.Document = this.printDocument1;
                if (MessageBox.Show("是否打印送货单？", "询问？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

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

                        SetDefaultPrinter(PrinterName);
                        this.printDocument1.DefaultPageSettings.PaperSize = new PaperSize("Custum", 210, 139);
                       
                        //* 加
                        int a = Convert.ToInt32(dtP.Rows.Count) / 8;
                        int b=  Convert.ToInt32(dtP.Rows.Count)% 8;
                        if (a == 0)
                        {
                            a = 1;
                        }
                        else if (b != 0)
                        {
                            a = a + 1;
                        }

                        ItemInspection.print_FMS.fun_print_送货单_修改(dtP, a);
                         //*加
                        //ItemInspection.print_FMS.fun_print_送货单(dr_传,1);
                    }
                }
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
                        //SetDefaultPrinter(PrinterName);


                        fun_打印出厂检验记录(PrinterName);
                    }
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "出库打印失败");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }



        private void 查看物料明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow dr = gvP.GetDataRow(gvP.FocusedRowHandle);
            ERPStock.frm仓库物料数量明细 frm = new ERPStock.frm仓库物料数量明细(dr["物料编码"].ToString(),dr["仓库号"].ToString());
            CPublic.UIcontrol.AddNewPage(frm, "物料明细");
        }

        private void gvP_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gcP, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

                ERPreport.frm送货单 frm = new ERPreport.frm送货单(dt, dtP);
                frm.ShowDialog();




            }
        }
    }
}
