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

namespace ERPStock
{
    public partial class frm成品退货查询界面 : UserControl
    {

        string strconn = CPublic.Var.strConn;
        DataTable dtM = new DataTable();
        DataTable dt_mx = new DataTable();

        public frm成品退货查询界面()
        {
            InitializeComponent();
        }

        private void frm成品退货查询界面_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime time = CPublic.Var.getDatetime().Date;
                bar_日期_后.EditValue = Convert.ToDateTime(time.AddDays(1).AddSeconds(-1));
                bar_日期_前.EditValue = Convert.ToDateTime(time.AddDays(-14));
                bar_单据状态.EditValue = "已生效";
                fun_载入();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 方法
        private void fun_载入()
        {
            try
            {
                if (dtM != null)
                {
                    dtM.Clear();
                }
                string s_组合 = " select 退货入库主表.*,退货申请主表.备注,客户,客户编号,退货类型 from 退货入库主表,退货申请主表 where 退货申请主表.退货申请单号=退货入库主表.退货申请单号 {0}";
                string s_组合1 = "";

                if (bar_日期_前.EditValue != null && bar_日期_后.EditValue != null && bar_日期_前.EditValue.ToString() != "" && bar_日期_后.EditValue.ToString() != "")
                {
                    s_组合1 += $"  and 退货入库主表.创建日期 >= '{ ((DateTime)bar_日期_前.EditValue).Date }'  and 退货入库主表.创建日期 <= '{ ((DateTime)bar_日期_后.EditValue).Date.AddDays(1) }'";
                }
                if (bar_单据状态.EditValue != null && bar_单据状态.EditValue.ToString() != "")
                {
                    if (bar_单据状态.EditValue.ToString() == "已生效")
                    {
                        s_组合1 += " and 退货入库主表.生效 = 1";
                    }
                    if (bar_单据状态.EditValue.ToString() == "未生效")
                    {
                        s_组合1 += " and 退货入库主表.生效 = 0";
                    }
                    if (bar_单据状态.EditValue.ToString() == "所有")
                    { }
                }
                //if (s_组合1 != "and ")
                //{
                //    s_组合1 = s_组合1.Substring(0, s_组合1.Length - 4);
                   s_组合 = string.Format(s_组合, s_组合1);
                //}
                SqlDataAdapter da = new SqlDataAdapter(s_组合, strconn);
                da.Fill(dtM);
                gc.DataSource = dtM;
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "退货入库主表_刷新操作");
                throw ex;
            }
        }
        #endregion

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPorg.Corg.FlushMemory();
            fun_载入();
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
                gc.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow r = gv.GetDataRow(gv.FocusedRowHandle);
                if (r != null)
                {
                    string sql = string.Format("select * from  退货入库子表 where 退货入库单号='{0}'", r["退货入库单号"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                    {
                        dt_mx= new DataTable();
                        da.Fill(dt_mx);
                        gridControl1.DataSource = dt_mx;
                    }
                }
                //if (e.Clicks == 2 && e.Button == System.Windows.Forms.MouseButtons.Left)
                //{
                //    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                //    frm成品退货界面 fm = new frm成品退货界面(dr);
                //    CPublic.UIcontrol.AddNewPage(fm, "退货入库");
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("确定打印？", "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                DataTable dt_dy = dt_mx.Copy();
                int count = dt_dy.Rows.Count / 14;
                if (dt_dy.Rows.Count % 14 != 0)
                {
                    count++;
                }

                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                this.printDialog1.Document = this.printDocument1;
                DialogResult drt = this.printDialog1.ShowDialog();
                if (drt == DialogResult.OK)
                {
                    string PrinterName = this.printDocument1.PrinterSettings.PrinterName;
                    ItemInspection.print_FMS.fun_print_退货入库_A5(dr["退货申请单号"].ToString(), dr["操作人员"].ToString(), dt_dy, count, true, PrinterName);
                }
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        

            try

            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                DataTable dt_dy = dt_mx.Copy();

                //DataRow drM = (this.BindingContext[gcM.DataSource].Current as DataRowView).Row;
                //DataTable dtm = (DataTable)this.gcP.DataSource;
                Assembly outerAsm = Assembly.LoadFrom(Path.Combine(Application.StartupPath, string.Format(@"{0}", "ERPreport.dll")));  //  ERPproduct.dll
                Type outerForm = outerAsm.GetType("ERPreport.退货打印", false); //打开界面ID 字段 存的值为 ERPproduct.frm报工系统

                object[] drr = new object[2];

                drr[0] = dt_dy;
                drr[1] = dr;
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
    }
}
