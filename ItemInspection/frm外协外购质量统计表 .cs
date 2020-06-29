using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;//试验
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CZMaster;
using System.Data.SqlClient;
namespace ItemInspection
{
    public partial class frm外协外购质量统计表 : UserControl
    {
        #region 用户变量

        private Dictionary<string, int> Dic_C = new Dictionary<string, int>();
        private Dictionary<string, string> Dic_Month = new Dictionary<string, string>();
        private Dictionary<string, decimal> Dic_P = new Dictionary<string, decimal>();
        private DataTable dtCP = null;
        private DataTable dtGYS = null;

        private DataTable dtP;
       
        #endregion 用户变量

        #region 类自用

        public frm外协外购质量统计表()
        {
            InitializeComponent();
        }

        private void frm外协外购质量统计表_Load(object sender, EventArgs e)
        {              
             DateTime dtime = CPublic.Var.getDatetime();
            dtime = new DateTime(dtime.Year, dtime.Month, dtime.Day);

            dtEdit1.EditValue =  dtime.AddDays(-1);
           // dtEdit1.EditValue = Convert.ToDateTime(System.DateTime.Today.Year.ToString() + "/01/01").AddSeconds(-1);
            dtEdit2.EditValue = dtime;
            string sql = "select  姓名 as 检验员   from 人事基础员工表 where 部门='品质部'  and 在职状态 ='在职'  ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                DataTable dt =new DataTable ();
                da.Fill(dt);
                repositoryItemSearchLookUpEdit1.DataSource = dt;
                repositoryItemSearchLookUpEdit1.DisplayMember = "检验员";
                repositoryItemSearchLookUpEdit1.ValueMember ="检验员";

            }


        }

        #endregion 类自用
          


        #region 数据处理

        /// <summary>
        /// 批次
        /// </summary>
        /// <param name="Compang"></param>
        /// <param name="sql"></param>
        /// <param name="dv"></param>
        /// <returns></returns>
        private int fun_Count(string Compang, string sql, DataView dv)
        {
            int All = 0;
         
            dv.RowFilter = string.Format("{0} and 供应商名称 ='{1}' and 修改检验日期 is null", sql, Compang);
         
            if (barEditItem1.EditValue.ToString() == false.ToString())
            {
                All = dv.Count;
            }
            if (barEditItem1.EditValue.ToString() == true.ToString())
            {
                All = dv.Count;
                //  2/17 备注 改数量为批次
                //DataTable dt_All = dv.ToTable();
                //foreach (DataRow r in dt_All.Rows)
                //{
                //    All += Convert.ToInt32(r["送检数量"]);
                //}
            }
       
            return All;
        }

        /// <summary>
        /// 合格率
        /// </summary>
        /// <param name="Compang"></param>
        /// <param name="sql"></param>
        /// <param name="dv"></param>
        /// <returns></returns>
        private decimal fun_Percentage(string Compang, string sql, DataView dv)
        {
            int All = 0;
            decimal No = 0;

            if (barEditItem1.EditValue.ToString() == false.ToString())
            {
                dv.RowFilter = string.Format("{0} and 供应商名称 ='{1}' and 修改检验日期 is null", sql, Compang);//gysmc
                All = dv.Count;

                dv.RowFilter = "";

                sql = string.Format("{0} and 供应商名称 ='{1}' and (检验结果='合格'or 检验结果='免检')  and 修改检验日期 is null ", sql, Compang);//gysmc
                dv.RowFilter = sql;
                No = dv.Count;
            }
            if (barEditItem1.EditValue.ToString() == true.ToString())
            {
                dv.RowFilter = string.Format("{0} and 供应商名称 ='{1}'and 修改检验日期 is null", sql, Compang);//gysmc
                DataTable dt_All = dv.ToTable();
                foreach (DataRow r in dt_All.Rows)
                {
                    All += Convert.ToInt32(r["送检数量"]);
                }

                dv.RowFilter = "";

                sql = string.Format("{0} and 供应商名称 ='{1}' and (检验结果='合格'or 检验结果='免检')  and 修改检验日期 is null ", sql, Compang);//gysmc
                dv.RowFilter = sql;
                DataTable dt_No = dv.ToTable();
                foreach (DataRow r in dt_All.Rows)
                {
                    No += Convert.ToInt32(r["送检数量"]) - Convert.ToInt32(r["不合格数量"]);
                }
            }
            decimal Per = 0;
            try
            {
                Per = decimal.Divide(No, All);
            }
            catch { Per = 0; }

            return (Per.ToString("0.0000") == "0.0000") ? 00 : Per;
        }

        private void fun_刷新数据()
        {
            //DateTime time1 = System.DateTime.Now;
            string str_检验员 = "";
            if (barEditItem2.EditValue != null && barEditItem2.EditValue.ToString() != "")
            {

                str_检验员 = "and 检验员='"+ barEditItem2.EditValue.ToString()+"'";

            }
            string sql = string.Format(@"select * from 采购记录采购单检验主表 where  关闭=0 and  (检验日期 >= '{0}' 
                                    and 检验日期 <= '{1}') and 数量标记='{2}' {3} order by ID",
                                                        ((DateTime)dtEdit1.EditValue).ToString("yyyy-MM-dd HH:mm:ss"),
                                                        ((DateTime)dtEdit2.EditValue).AddSeconds(-1).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"),
                                                        (barEditItem1.EditValue.ToString() == true.ToString()) ? 1 : 0,str_检验员 );
       
            DataTable dt = MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);

            if (dtCP == null)
            {
                dtCP = MasterSQL.Get_DataTable("select 物料编码,规格型号,物料名称 from 基础数据物料信息表", CPublic.Var.strConn);
                //dtCP = MasterSQL.Get_DataTable("select cpbh,ggxh,cpmc from cp", CPublic.Var.geConn("WL"));
            }
            if (dtGYS == null)
            {
                dtGYS = MasterSQL.Get_DataTable("select 供应商ID,供应商名称 from 采购供应商表", CPublic.Var.strConn);
                //dtGYS = MasterSQL.Get_DataTable("select gysbh,gysmc from gys", CPublic.Var.geConn("WL"));
            }

            CPublic.CConstrFun.fun_数据关联扩展(dt, dtCP, new string[] { "产品编号|物料编码" }, new string[] { "规格型号", "物料名称" });
            //CPublic.CConstrFun.fun_数据关联扩展(dt, dtCP, new string[] { "产品编号|cpbh" }, new string[] { "规格型号", "物料名称" });//原版 zf
            System.Diagnostics.Debug.Write("1");
            CPublic.CConstrFun.fun_数据关联扩展(dt, dtGYS, new string[] { "供应商编号|供应商ID" }, new string[] { "供应商名称" });
            //CPublic.CConstrFun.fun_数据关联扩展(dt, dtGYS, new string[] { "供应商编号|gysbh" }, new string[] { "供应商名称" });//原版 zf
            System.Diagnostics.Debug.Write("2");

            //gcM.DataSource = dt;
            //gvM.ViewCaption = string.Format(gvM.Tag.ToString());

            //DateTime time2 = System.DateTime.Now;

            DataView dv = new DataView(dt);
            //dv.Sort = "gysmc DESC ";
            System.Diagnostics.Debug.Write("3");
            DataTable dtM = dv.ToTable();

            List<string> li_str = new List<string>();
            foreach (DataRow r in dtM.Rows)
            {
                if (li_str.Contains(r["供应商名称"].ToString()) == false)
                {
                    li_str.Add(r["供应商名称"].ToString());
                }
                //if (li_str.Contains(r["gysmc"].ToString()) == false)
                //{
                //    li_str.Add(r["gysmc"].ToString());
                //}
            }
            System.Diagnostics.Debug.Write("4");
            dtP = new DataTable();
            dtP.Columns.Add("POS");
            dtP.Columns.Add("单位");
            for (int i = 1; i < 13; i++)
            {
                dtP.Columns.Add(i.ToString() + "月");
                if (i != 12)
                {
                    if (Dic_Month.ContainsKey(i.ToString()) == false)
                    {
                        Dic_Month.Add(i.ToString(), string.Format("检验日期>'" + Convert.ToDateTime(Convert.ToDateTime(dtEdit1.EditValue).Year.ToString() + "/" + i.ToString() + "/01").AddSeconds(-1).ToString()) + "' AND 检验日期<'" + Convert.ToDateTime(System.DateTime.Today.Year.ToString() + "/" + (i + 1).ToString() + "/01").AddSeconds(-1).ToString() + "'");
                    }
                }

                if (i == 12)
                {
                    if (Dic_Month.ContainsKey(i.ToString()) == false)
                    {
                        Dic_Month.Add(i.ToString(), string.Format("检验日期>'" + Convert.ToDateTime(Convert.ToDateTime(dtEdit1.EditValue).Year.ToString() + "/" + i.ToString() + "/01").AddSeconds(-1).ToString()) + "' AND 检验日期<'" + Convert.ToDateTime(System.DateTime.Today.Year.ToString() + "/01/01").AddYears(1).AddSeconds(-1).ToString() + "'");
                    }
                }

                if (Dic_C.ContainsKey(i.ToString() + "月") == false)
                {
                    Dic_C.Add(i.ToString() + "月", 0);
                }
                if (Dic_C.ContainsKey(i.ToString() + "月") == true)
                {
                    Dic_C[i.ToString() + "月"] = 0;
                }
                if (Dic_P.ContainsKey(i.ToString() + "月") == false)
                {
                    Dic_P.Add(i.ToString() + "月", 0);
                }
                if (Dic_P.ContainsKey(i.ToString() + "月") == true)
                {
                    Dic_P[i.ToString() + "月"] = 0;
                }
            }
            string LastColumn = Convert.ToDateTime(dtEdit1.EditValue).Year.ToString() + "年合计";
            dtP.Columns.Add(LastColumn);
            if (Dic_C.ContainsKey(LastColumn) == false)
            {
                Dic_C.Add(LastColumn, 0);
            }
            if (Dic_C.ContainsKey(LastColumn) == true)
            {
                Dic_C[LastColumn] = 0;
            }
            if (Dic_P.ContainsKey(LastColumn) == false)
            {
                Dic_P.Add(LastColumn, 0);
            }
            if (Dic_P.ContainsKey(LastColumn) == true)
            {
                Dic_P[LastColumn] = 0;
            }

            int POS = 1;
            foreach (string str_Com in li_str)
            {
                DataRow r1 = dtP.NewRow();
                r1["POS"] = POS;
                r1["单位"] = str_Com;
                DataRow r2 = dtP.NewRow();
                //r2["POS"] = POS+".1";
                r2["单位"] = str_Com;

                decimal Per_count = 0;//百分比之和
                //int M_Count = 0;//有效批次
                int Count = 0;//批次之和

                for (int i = 1; i < 13; i++)
                {
                    int c_num = fun_Count(str_Com, Dic_Month[i.ToString()], dv);

                    dv.RowFilter = "";
                    if (c_num != 0)
                    {
                        r2[i.ToString() + "月"] = c_num;
                        Count += c_num;
                        //M_Count++;

                        decimal num = fun_Percentage(str_Com, Dic_Month[i.ToString()], dv);
                        dv.RowFilter = "";

                        r1[i.ToString() + "月"] = (num == 1) ? "100%" : (num * 100).ToString("0.00") + "%";
                        //Per_count += num;
                        Per_count += num * c_num;

                        if (Dic_C.ContainsKey(i.ToString() + "月") == true)
                        {
                            Dic_C[i.ToString() + "月"] += c_num;
                        }
                        if (Dic_P.ContainsKey(i.ToString() + "月") == true)
                        {
                            Dic_P[i.ToString() + "月"] += decimal.Multiply(Convert.ToDecimal(num), Convert.ToDecimal(c_num));
                        }
                    }
                }

                decimal Per_AVE = 0;
                try
                {
                    //Per_AVE = decimal.Divide(Per_count, M_Count);
                    Per_AVE = decimal.Divide(Per_count, Count);
                }
                catch { Per_AVE = 0; }
                r1[LastColumn] = (Per_AVE == 1) ? "100%" : (Per_AVE * 100).ToString("0.00") + "%";
                r2[LastColumn] = Count;
                if (Dic_C.ContainsKey(LastColumn) == true)
                {
                    Dic_C[LastColumn] += Count;
                }
                if (Dic_P.ContainsKey(LastColumn) == true)
                {
                    Dic_P[LastColumn] += Per_AVE * Count;
                }
                dtP.Rows.Add(r1);
                dtP.Rows.Add(r2);

                POS++;
            }

            //DateTime time3 = System.DateTime.Now;

            DataView dvP = new DataView(dtP);
            DataRow r3 = dtP.NewRow();
            DataRow r4 = dtP.NewRow();
            r4["单位"] = (barEditItem1.EditValue.ToString() == true.ToString()) ? "总数量" : "总批次";
            r3["单位"] = "平均合格率";
            foreach (DataColumn column in dtP.Columns)
            {
                if (column.ColumnName == "POS" || column.ColumnName == "单位")
                {
                    continue;
                }
                r4[column.ColumnName] = Dic_C[column.ColumnName];
                decimal M_num = 0;
                try
                {
                    M_num = decimal.Divide((Dic_P[column.ColumnName] * 100), Dic_C[column.ColumnName]);
                }
                catch { M_num = 0; }
                r3[column.ColumnName] = (M_num == 100) ? "100%" : (M_num).ToString("0.00") + "%";
            }

            dtP.Rows.Add(r3);
            dtP.Rows.Add(r4);

            gcM.DataSource = dtP;

            //DateTime time4 = System.DateTime.Now;
            //gvM.ViewCaption = (time2 - time1) + "|" + (time3 - time2) + "|" + (time4 - time3) + "|" + (time4 - time1);
            gvM.ViewCaption = string.Format("{0}年外协、外购质量统计表(白色行为合格率、蓝色行为批次)", System.DateTime.Today.Year.ToString());
        }

        #endregion 数据处理

        #region 界面相关

        /// <summary>
        /// 刷新报表数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_刷新数据();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string strDefaultPrinter = new PrintDocument().PrinterSettings.PrinterName;
            try
            {
                if (DialogResult.OK == MessageBox.Show(strDefaultPrinter, "打印机确认？", MessageBoxButtons.OKCancel))
                {
                    if (dtP == null)
                    {
                        throw new Exception("数据不能为空！");
                    }
                    print_QualityReportForTheWholeYear.fun_print_QualityReportForTheWholeYear(dtP);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Excel导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
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
                //FolderBrowserDialog dialog = new FolderBrowserDialog();
                //dialog.Description = "目标位置";
                //if (dialog.ShowDialog() == DialogResult.OK)
                //{
                //    string foldPath = dialog.SelectedPath;

                //    string fileName = string.Format("{0}\\{1}年合计报表_{2}.xlsx", dialog.SelectedPath, System.DateTime.Today.Year.ToString(), System.DateTime.Today.ToLongDateString());

                //    System.IO.Directory.CreateDirectory(foldPath);
                //    if (dtP == null)
                //    {
                //        throw new Exception("数据不能为空！");
                //    }
                //    if (System.IO.File.Exists(fileName) == true)
                //    {
                //        if (MessageBox.Show("文件已存在是否覆盖", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                //        {
                //            print_QualityReportForTheWholeYear.fun_print_QualityReportForTheWholeYear_ToExcel(dtP, fileName, true);
                //        }
                //    }
                //    if (System.IO.File.Exists(fileName) == false)
                //    {
                //        print_QualityReportForTheWholeYear.fun_print_QualityReportForTheWholeYear_ToExcel(dtP, fileName, true);
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 打印(视图)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Microsoft.Office.Interop.Excel.ApplicationClass excelApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                string foldPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\Excelprttmp";
                DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();
                string fileName = string.Format("{0}\\{1}年合计报表_{2}.xlsx", foldPath, System.DateTime.Today.Year.ToString(), System.DateTime.Today.ToLongDateString());
                try
                {
                    System.IO.Directory.Delete(foldPath, true);
                }
                catch
                {
                }
                System.IO.Directory.CreateDirectory(foldPath);
                gcM.ExportToXlsx(fileName);

                excelApp.Visible = false;
                Microsoft.Office.Interop.Excel.Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                                           Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                           Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                           Type.Missing, Type.Missing);
                Microsoft.Office.Interop.Excel.Worksheet mysheet = wb.Sheets[1] as Microsoft.Office.Interop.Excel.Worksheet;//第一个sheet页
                mysheet.Cells.EntireColumn.AutoFit();
                mysheet.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;
                wb.PrintOutEx();
                excelApp.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (PID != IntPtr.Zero)
                {
                    excelApp = null;
                    GcCollect();
                    KillProcess(PID);
                }
            }
        }

        /// <summary>
        /// Excel导出(视图)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Microsoft.Office.Interop.Excel.ApplicationClass excelApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "目标位置";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();
                    string fileName = string.Format("{0}\\{1}年合计报表_{2}.xlsx", dialog.SelectedPath, System.DateTime.Today.Year.ToString(), System.DateTime.Today.ToLongDateString());

                    if (System.IO.File.Exists(fileName) == true)
                    {
                        if (MessageBox.Show("文件已存在是否覆盖", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            gcM.ExportToXlsx(fileName);
                        }
                    }
                    if (System.IO.File.Exists(fileName) == false)
                    {
                        gcM.ExportToXlsx(fileName);
                    }

                    excelApp.Visible = false;
                    Microsoft.Office.Interop.Excel.Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                                  Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                  Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                  Type.Missing, Type.Missing);
                    Microsoft.Office.Interop.Excel.Worksheet mysheet = wb.Sheets[1] as Microsoft.Office.Interop.Excel.Worksheet;//第一个sheet页
                    mysheet.Cells.EntireColumn.AutoFit();
                    wb.Save();
                    excelApp.Quit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (PID != IntPtr.Zero)
                {
                    excelApp = null;
                    GcCollect();
                    KillProcess(PID);
                }
            }
        }

        #endregion 界面相关

        #region 试验

        /// <summary>
        ///  回收垃圾
        /// </summary>
        public static void GcCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);
        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="H"></param>
        private static void KillProcess(IntPtr H)
        {
            System.Diagnostics.Process myproc = new System.Diagnostics.Process();

            try
            {
                foreach (System.Diagnostics.Process thisproc in System.Diagnostics.Process.GetProcessesByName("excel"))
                {
                    if (thisproc.Id == (int)H)
                    {
                        if (!thisproc.CloseMainWindow())
                        {
                            thisproc.Kill();
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        #endregion 试验

        /// <summary>
        /// 背景颜色变化(单元格)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvM_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName != "POS" && e.Column.FieldName != "单位")
            {
                int hand = e.RowHandle;
                if (hand < 0)
                    return;
                DataRow dr = this.gvM.GetDataRow(hand);
                if (dr["单位"].ToString() == "总批次" || dr["单位"].ToString() == "平均合格率" || dr["单位"].ToString() == "总数量")
                    return;

                if (e.CellValue.ToString().IndexOf("%") > 0)
                {
                    decimal M_num = 0;
                    try
                    {
                        M_num = decimal.Divide((Dic_P[e.Column.FieldName] * 100), Dic_C[e.Column.FieldName]);
                    }
                    catch { M_num = 0; }
                    if (Convert.ToDecimal(e.CellValue.ToString().Replace("%", "")) < M_num) // Dic_P[e.Column.FieldName])
                    {
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.BackColor2 = Color.Red;
                    }
                }
            }
        }

        /// <summary>
        /// 双击展开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvM_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
                int hand = e.RowHandle;
                if (hand < 0)
                    return;
                DataRow dr = this.gvM.GetDataRow(hand);
                if (dr["单位"].ToString() == "总批次" || dr["单位"].ToString() == "平均合格率" || dr["单位"].ToString() == "总数量")
                    return;
                if (e.Column.FieldName == dtP.Columns[dtP.Columns.Count - 1].ColumnName)
                {
                    //int hand = e.RowHandle;
                    //if (hand < 0)
                    //    return;
                    //DataRow dr = this.gvM.GetDataRow(hand);

                    frm采购件检验记录列表 fm = new frm采购件检验记录列表(); ;

                    fm.time1 = Convert.ToDateTime(System.DateTime.Today.Year.ToString() + "/01/01").AddSeconds(-1);
                    fm.time2 = System.DateTime.Today.AddDays(1).AddSeconds(-1);

                    fm.dw = dr["单位"].ToString();
                    CPublic.UIcontrol.AddNewPage(fm, string.Format("来料检验汇总[{0}]", dr["单位"].ToString() + dtP.Columns[dtP.Columns.Count - 1].ColumnName + e.Column.FieldName + "数据"));
                }

                if (e.Column.FieldName != "POS" && e.Column.FieldName != "单位" && e.Column.FieldName != dtP.Columns[dtP.Columns.Count - 1].ColumnName)
                {
                    //int hand = e.RowHandle;
                    //if (hand < 0)
                    //    return;
                    //DataRow dr = this.gvM.GetDataRow(hand);

                    frm采购件检验记录列表 fm = new frm采购件检验记录列表();
                    string str = Dic_Month[e.Column.FieldName.Replace("月", "")];
                    str = str.Replace("检验日期>'", "");

                    fm.time1 = Convert.ToDateTime(str.Replace("检验日期>'", "").Substring(0, str.IndexOf("'")));
                    fm.time2 = Convert.ToDateTime(str.Remove(0, str.IndexOf("AND 检验日期<'") + 10).Replace("'", ""));

                    fm.dw = dr["单位"].ToString();
                    CPublic.UIcontrol.AddNewPage(fm, string.Format("来料检验汇总[{0}]", dr["单位"].ToString() + dtP.Columns[dtP.Columns.Count - 1].ColumnName + e.Column.FieldName + "数据"));
                }
            }
        }
    }
}