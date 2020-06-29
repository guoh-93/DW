using Microsoft.Office.Interop.Excel;
using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;


namespace ItemInspection
{
    public class print_QualityReportForTheWholeYear
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        public static void fun_print_QualityReportForTheWholeYear(System.Data.DataTable dtP, bool blPreview = false)
        {
            string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx"; ;
            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                {
                    System.Data.DataTable dtPP = new System.Data.DataTable();
                    string s = "select * from 基础记录打印模板表 where 模板名 = '质检-全年质量报表'";
                    new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                    if (dtPP.Rows.Count == 0) return;
                    try
                    {
                        System.IO.Directory.Delete(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp", true);
                    }
                    catch
                    {
                    }
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp");
                    System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
                }

                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing);
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;

                range = ws.get_Range("B3", Type.Missing);
                range.Value2 = string.Format("{0}年外协、外购质量统计表", dtP.Columns[dtP.Columns.Count - 1].ColumnName.Substring(0, 4));
                range = ws.get_Range("BU4", Type.Missing);
                string LastColumn = dtP.Columns[dtP.Columns.Count - 1].ColumnName;
                range.Value2 = LastColumn;

                int pos = 0;
                int i_first = 6;
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    if (pos == dtP.Rows.Count - 2)
                    {
                        if (pos <= 305)
                        {
                            range = ws.get_Range("B" + (i_first + pos).ToString(), "B305");
                            range.EntireRow.Delete(Type.Missing);
                        }
                    }
                    range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["POS"].ToString();

                    range = ws.get_Range("E" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["单位"].ToString();

                    range = ws.get_Range("Y" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["1月"].ToString();
                    range = ws.get_Range("AC" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["2月"].ToString();
                    range = ws.get_Range("AG" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["3月"].ToString();
                    range = ws.get_Range("AK" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["4月"].ToString();
                    range = ws.get_Range("AO" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["5月"].ToString();
                    range = ws.get_Range("AS" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["6月"].ToString();
                    range = ws.get_Range("AW" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["7月"].ToString();
                    range = ws.get_Range("BA" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["8月"].ToString();
                    range = ws.get_Range("BE" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["9月"].ToString();
                    range = ws.get_Range("BI" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["10月"].ToString();
                    range = ws.get_Range("BM" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["11月"].ToString();
                    range = ws.get_Range("BQ" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["12月"].ToString();

                    range = ws.get_Range("BU" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r[LastColumn].ToString();

                    pos++;

                    //if (pos == dr_传.Rows.Count - 2)
                    //{
                    //    break;
                    //}
                }

                //if (pos < 305)
                //{
                //    range = ws.get_Range("B" + (i_first + pos).ToString(), "B305");
                //    range.EntireRow.Delete(Type.Missing);
                //}

                if (blPreview)
                {
                    excelApp.Visible = true;
                    wb.PrintPreview();
                }
                else
                {
                    excelApp.Visible = false;
                    BringWindowToTop(hwnd);
                    wb.PrintOutEx();
                    excelApp.Quit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        public static void fun_print_QualityReportForTheWholeYear_ToExcel(System.Data.DataTable dtP, string fileName, bool blPreview = false)
        {
            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                {
                    System.Data.DataTable dtPP = new System.Data.DataTable();
                    string s = "select * from 基础记录打印模板表 where 模板名 = '质检-全年质量报表'";
                    new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                    if (dtPP.Rows.Count == 0) return;

                    System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
                }

                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing);
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;

                range = ws.get_Range("B3", Type.Missing);
                range.Value2 = string.Format("{0}年外协、外购质量统计表", dtP.Columns[dtP.Columns.Count - 1].ColumnName.Substring(0, 4));
                range = ws.get_Range("BU4", Type.Missing);
                string LastColumn = dtP.Columns[dtP.Columns.Count - 1].ColumnName;
                range.Value2 = LastColumn;

                int pos = 0;
                int i_first = 6;
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    if (pos == dtP.Rows.Count - 2)
                    {
                        if (pos <= 305)
                        {
                            range = ws.get_Range("B" + (i_first + pos).ToString(), "B305");
                            range.EntireRow.Delete(Type.Missing);
                        }
                    }
                    range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["POS"].ToString();

                    range = ws.get_Range("E" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["单位"].ToString();

                    range = ws.get_Range("Y" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["1月"].ToString();
                    range = ws.get_Range("AC" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["2月"].ToString();
                    range = ws.get_Range("AG" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["3月"].ToString();
                    range = ws.get_Range("AK" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["4月"].ToString();
                    range = ws.get_Range("AO" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["5月"].ToString();
                    range = ws.get_Range("AS" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["6月"].ToString();
                    range = ws.get_Range("AW" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["7月"].ToString();
                    range = ws.get_Range("BA" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["8月"].ToString();
                    range = ws.get_Range("BE" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["9月"].ToString();
                    range = ws.get_Range("BI" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["10月"].ToString();
                    range = ws.get_Range("BM" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["11月"].ToString();
                    range = ws.get_Range("BQ" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["12月"].ToString();

                    range = ws.get_Range("BU" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r[LastColumn].ToString();

                    pos++;
                }

                //if (pos <= 305)
                //{
                //    range = ws.get_Range("B" + (i_first + pos).ToString(), "B305");
                //    range.EntireRow.Delete(Type.Missing);
                //}
                if (blPreview)
                {
                    wb.Save();
                }
                else
                {
                    excelApp.Visible = false;
                    BringWindowToTop(hwnd);
                    wb.PrintOutEx();
                    excelApp.Quit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
        ///  回收垃圾
        /// </summary>
        public static void GcCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

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
        //没用到
        public static void fun_print_领料单(System.Data.DataTable dtP, int i, bool blPreview = false)
        {
            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx"; 
            string fileName = "D:\\123" + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";  //zf
            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                {
                    System.Data.DataTable dtPP = new System.Data.DataTable();
                    string s = "select * from 基础记录打印模板表 where 模板名 = '领料单'";
                    new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                    if (dtPP.Rows.Count == 0) return;
                    try
                    {
                        System.IO.Directory.Delete(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp", true);
                    }
                    catch
                    {
                    }
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp");
                    System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
                }

                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing);
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;
                //领用部门  编号 日期 
                string sql = string.Format("select * from 生产记录生产领料单主表 where 领料出库单号 = '{0}'", dtP.Rows[0]["领料出库单号"].ToString());
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);

                range = ws.get_Range("B6", Type.Missing);
                range.Value2 = dt.Rows[0]["生产车间"].ToString();
                range = ws.get_Range("P4", Type.Missing);
                range.Value2 = dt.Rows[0]["领料出库单号"].ToString();

                range = ws.get_Range("F5", Type.Missing);
                range.Value2 = dt.Rows[0]["领料出库单号"].ToString();

                range = ws.get_Range("P6", Type.Missing);
                range.Value2 = System.DateTime.Today.ToString("yyyy-MM-dd");
                range = ws.get_Range("B114", Type.Missing);
                range.Value2 = dt.Rows[0]["领料人员ID"].ToString() + " " + dt.Rows[0]["领料人员"].ToString();

                int pos = 0;
                int i_first = 11;
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    //if (((pos/3)) == dr_传.Rows.Count)
                    //{
                    //    if (pos <= 113)
                    //    {
                    //        range = ws.get_Range("A" + (i_first + pos).ToString(), "A113");
                    //        range.EntireRow.Delete(Type.Missing);
                    //    }
                    //}

                    if ((pos + i_first) >= 98)
                    {
                        break;
                    }
                    range = ws.get_Range("A" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = i.ToString();
                    range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["物料编码"].ToString();
                    range = ws.get_Range("D" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["物料名称"].ToString() + r["规格型号"].ToString();
                    //range = ws.get_Range("I" + (i_first + pos).ToString(), Type.Missing);
                    //range.Value2 = r["单位"].ToString();  //缺单位
                    range = ws.get_Range("J" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["领料数量"].ToString();
                    range = ws.get_Range("L" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["领料仓库"].ToString();
                    range = ws.get_Range("O" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["领料库位"].ToString();
                    range = ws.get_Range("Q" + (i_first + pos).ToString(), Type.Missing);
                    try
                    {
                        string sqld = string.Format("select 库存总数 from 仓库物料数量表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                        System.Data.DataTable dtt = new System.Data.DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(sqld, CPublic.Var.strConn);
                        da.Fill(dtt);
                        range.Value2 = dtt.Rows[0]["库存总数"].ToString();
                    }
                    catch { }
                    i++;
                    pos = pos + 3;
                }

                if (blPreview)
                {
                    excelApp.Visible = true;
                    wb.PrintPreview();
                }
                else
                {
                    excelApp.Visible = false;
                    BringWindowToTop(hwnd);
                    wb.PrintOutEx();
                    excelApp.Quit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (PID != IntPtr.Zero)
                {
                    excelApp = null;
                    GcCollect();
                    KillProcess(PID);
                    System.IO.File.Delete(fileName);
                }
            }
        }
        /// <summary>
        /// 重构
        /// 3.21 郭恒
        /// </summary>
        /// <param name="str_模板名称"></param>
        /// <param name="blPreview"></param>


        public static void fun_print_Test(string str_模板名称, System.Data.DataTable dtP, bool blPreview = false)
        {
            string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
            //string fileName = "D:\\123" + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";  //zf
            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                {
                    System.Data.DataTable dtPP = new System.Data.DataTable();
                    string s = string.Format("select * from 基础记录打印模板表 where 模板名 = '{0}'", str_模板名称);
                    new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                    if (dtPP.Rows.Count == 0) return;
                    try
                    {
                        System.IO.Directory.Delete(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp", true);
                    }
                    catch
                    {
                    }
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp");
                    System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
                }

                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing);
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;

                string strcon = CPublic.Var.strConn;
                int Max_x = 0;
                int Max_y = 0;
                string sql = string.Format("select * from  模板起始位置表 where 模板名称='{0}'", str_模板名称);
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    System.Data.DataTable dt_起始位置 = new System.Data.DataTable();
                    da.Fill(dt_起始位置);
                    if (dt_起始位置.Rows.Count > 0)
                    {
                        Max_x = int.Parse(dt_起始位置.Rows[0]["X"].ToString());
                        Max_y = int.Parse(dt_起始位置.Rows[0]["Y"].ToString());
                    }
                }
                string sql_M = string.Format("select * from 模块维护表 where 模块名称='{0}'", str_模板名称);
                System.Data.DataTable dt_1 = new System.Data.DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                {
                    da.Fill(dt_1);
                }

                for (int i = 1; i <= Max_y; i++)
                    for (int j = 65; j <= Max_x; j++)    //   max_x 一定要 <   90
                    {                                               
                        range = ws.get_Range((char)j + i.ToString(), Type.Missing);
                        if (range.Value2 == "%1%")
                        {
                            System.Data.DataRow[] dr = dt_1.Select(string.Format("模块名='%1%'"));
                            if (dr.Length > 0)
                            {
                                string str_字段 = dr[0]["字段"].ToString();
                                range.Value2 = dtP.Rows[0][str_字段].ToString();

                            }
                        }
                        if (range.Value2 == "%2%")
                        {

                        }

                    }
                if (blPreview)
                {
                    excelApp.Visible = true;
                    wb.PrintPreview();
                }
                else
                {
                    excelApp.Visible = false;
                    BringWindowToTop(hwnd);
                    wb.PrintOutEx();
                    excelApp.Quit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (PID != IntPtr.Zero)
                {
                    excelApp = null;
                    GcCollect();
                    KillProcess(PID);
                    System.IO.File.Delete(fileName);
                }
            }
        }
    }
}