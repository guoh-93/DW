using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing.Printing;
//using ItemInspection;
using System.Runtime.InteropServices;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm生产领料列表视图 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {

        #region 变量
        string str_领料出库单号;
        string strcon = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        DataTable dtP;
        DataTable dt_仓库;
        string sql_ck;
        #endregion

        #region 加载
        public frm生产领料列表视图()
        {
            InitializeComponent();
        }
        public frm生产领料列表视图(string str_领料出库单号)
        {
            InitializeComponent();
            this.str_领料出库单号 = str_领料出库单号;
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm生产领料列表视图_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            string sql_ck = string.Format("select * from 人员仓库对应表 where 工号='{0}'", CPublic.Var.LocalUserID);
            dt_仓库 = new DataTable();
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_ck, strcon);
            fun_load();
        }
        #endregion

        #region 函数
#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = string.Format("select * from 生产记录生产领料单主表 where 领料出库单号='{0}'", str_领料出库单号);
            using (SqlDataAdapter da = new SqlDataAdapter(sql,strcon))
            {
                da.Fill(dtM);
                if (dtM.Rows.Count > 0)
                {
                    dataBindHelper1.DataFormDR(dtM.Rows[0]);
                }          
            }

            sql_ck = "and sdlmx.仓库号  in(";
            string sql_1 = "";
            if (dt_仓库.Rows.Count == 0)
            {

                sql_1 = string.Format(@"select slmx.*,a.主辅料,库存总数,base.计量单位 as 库存单位 ,sdlmx.仓库名称 from 生产记录生产领料单明细表 slmx
                    left  join 基础数据物料信息表 base  on      slmx.物料编码=base.物料编码
                    left join 生产记录生产工单待领料明细表 sdlmx on sdlmx.待领料单明细号=slmx.待领料单明细号
                    left join 仓库物料数量表 kc   on      slmx.物料编码=kc.物料编码
                    left join  (select 子项编码,主辅料 from  基础数据物料BOM表 where 物料编码='{0}' group by 子项编码,主辅料)a  on  slmx.物料编码= a.子项编码
                where  kc.仓库号=sdlmx.仓库号 and 领料出库单号='{1}' ", textBox4.Text, str_领料出库单号);
            }
            else
            {
                foreach (DataRow dr in dt_仓库.Rows)
                {
                    sql_ck = sql_ck + string.Format("'{0}',", dr["仓库号"]);

                }
                sql_ck = sql_ck.Substring(0, sql_ck.Length - 1) + ")";
                sql_1 = string.Format(@"select slmx.*,a.主辅料,库存总数 ,sdlmx.仓库名称,kc.货架描述 ,计量单位 as 库存单位 from 生产记录生产领料单明细表 slmx
                    --left  join 基础数据物料信息表 base  on   slmx.物料编码=base.物料编码
                    left join 生产记录生产工单待领料明细表 sdlmx on sdlmx.待领料单明细号=slmx.待领料单明细号
                    left join 仓库物料数量表 kc on  slmx.物料编码=kc.物料编码
                    left join  (select 子项编码,主辅料 from  基础数据物料BOM表  where 产品编码='{0}' group by  子项编码,主辅料)a  on      slmx.物料编码= a.子项编码
                    where    kc.仓库号=sdlmx.仓库号  and 领料出库单号='{1}'  {2} order by 规格型号 ", textBox4.Text, str_领料出库单号, sql_ck);
            }


            using (SqlDataAdapter da = new SqlDataAdapter(sql_1, strcon))
            {
                dtP = new DataTable();
                da.Fill(dtP);
                gc.DataSource = dtP;
            }
        }
        #endregion

        #region 界面操作



        //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            
        }
        //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }






        #endregion
      
        //private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    string strDefaultPrinter = new PrintDocument().PrinterSettings.PrinterName;
        //    try
        //    {
        //        if (DialogResult.OK == MessageBox.Show(strDefaultPrinter, "打印机确认？", MessageBoxButtons.OKCancel))
        //        {
        //            if (dtP == null)
        //            {
        //                throw new Exception("数据不能为空！");
        //            }
        //            int count = 0;
        //            if (dtP.Rows.Count % 31 != 0)
        //            {
        //                count = dtP.Rows.Count / 31 + 1;
        //            }
        //            else
        //            {
        //                count = dtP.Rows.Count / 31;
        //            }
        //            DataSet ds = new DataSet();
        //            int ia = 0;
        //            DataTable t = dtP.Clone();
        //            ds.Tables.Add(t);
        //            foreach (DataRow r in dtP.Rows)
        //            {
        //                if (ia > 0 && ia % 31 == 0)
        //                {
        //                    t = dtP.Clone();
        //                    ds.Tables.Add(t);
        //                }
        //                DataRow rr = t.NewRow();
        //                rr.ItemArray = r.ItemArray;
        //                t.Rows.Add(rr);
        //                ia++;
        //            }
        //            foreach (DataTable tt in ds.Tables)
        //            {
        //                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
        //                this.printDialog1.Document = this.printDocument1;
        //                DialogResult dr = this.printDialog1.ShowDialog();
        //                if (dr == DialogResult.OK)
        //                {
        //                    //Get the Copy times
        //                    int nCopy = this.printDocument1.PrinterSettings.Copies;
        //                    //Get the number of Start Page
        //                    int sPage = this.printDocument1.PrinterSettings.FromPage;
        //                    //Get the number of End Page
        //                    int ePage = this.printDocument1.PrinterSettings.ToPage;
        //                    string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
        //                    //SetDefaultPrinter(PrinterName);
        //                   ItemInspection.print_FMS.fun_print_领料单(tt, 1);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}



        //[DllImport("winspool.drv")]
        //public static extern bool SetDefaultPrinter(String Name); //调用win api将指定名称的打印机设置为默认打印机
#pragma warning disable IDE1006 // 命名样式
        private void panel1_Paint(object sender, PaintEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

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
        //    A5 
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            //string strDefaultPrinter = new PrintDocument().PrinterSettings.PrinterName;
            try
            {
                
                    fun_打印();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_打印()
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {

                //int count = 1;
                //if (dtP.Rows.Count % 15 != 0)
                //{
                //    count = (dtP.Rows.Count / 15) + 1;
                //}
                //else
                //{
                //    count = dtP.Rows.Count / 15;
                //}
                //PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                //this.printDialog1.Document = this.printDocument1;
                //DialogResult dr = this.printDialog1.ShowDialog();
                DataTable dt_dy = dtP.Copy();
                dt_dy.Columns["领料数量"].ColumnName = "输入领料数量";

                DataTable dt_表头 = new DataTable();
                dt_表头.Columns.Add("领料出库单号", typeof(string));
                dt_表头.Columns.Add("编号", typeof(string));
                dt_表头.Columns.Add("物料号", typeof(string));
                dt_表头.Columns.Add("规格", typeof(string));
                dt_表头.Columns.Add("物料名称", typeof(string));
                dt_表头.Columns.Add("生产数量", typeof(decimal));
                dt_表头.Columns.Add("领用部门", typeof(string));
                dt_表头.Columns.Add("领用人", typeof(string));
                dt_表头.Columns.Add("申请人", typeof(string));
                dt_表头.Columns.Add("仓管员", typeof(string));
                dt_表头.Columns.Add("日期", typeof(DateTime));

                DataRow dr = dt_表头.NewRow();
                string sql = string.Format("select * from  生产记录生产工单表 where 生产工单号='{0}'", textBox2.Text.ToString());
                DataRow rr = CZMaster.MasterSQL.Get_DataRow(sql,strcon);

                string sql2 = string.Format(" select* from 生产记录生产工单待领料明细表 where  生产工单号='{0}'", textBox2.Text.ToString());
                DataTable drwww = CZMaster.MasterSQL.Get_DataTable(sql2,strcon);
                   
               dr["编号"] = rr["生产工单号"];
                dr["物料号"] = rr["物料编码"];
                dr["规格"] = rr["规格型号"];

                dr["物料名称"] = rr["物料名称"];

                dr["生产数量"] = rr["生产数量"];
                // dr["领用部门"] = "dsa13123";
                dr["领用人"] = rr["领料人ID"].ToString() + "  " + rr["领料人"].ToString();
                dr["申请人"] = rr["制单人员"].ToString();

                //dr["仓管员"] = "dsa13123";
                dr["日期"] = DateTime.Now.ToString();
                dr["领料出库单号"] = drwww.Rows[0]["待领料单号"].ToString();
                dt_表头.Rows.Add(dr);



                //  DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                ERPreport.frm发料打印 frm = new ERPreport.frm发料打印(dt_dy, dt_表头);
                frm.ShowDialog();

                //if (dr == DialogResult.OK)
                //{
                //    string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                //    ItemInspection.print_FMS.fun_p_领料A5(txt_lingliaodan.Text, dt_dy, count, PrinterName, true);


                //}

            }
            catch (Exception ex)
            {

               
                MessageBox.Show(ex.Message);
            }
        }
    }
}
