using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;

namespace ItemInspection
{
    public class print_Check
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool BringWindowToTop(IntPtr hWnd);//将指定的窗口带至窗口列表顶部

        //public static Boolean workFlag = false;
        //public static Thread thread1;
        //static string InspectionRecordNumber;

        //public static void fun_print_control_thread(string IRN)
        //{
        //    InspectionRecordNumber = IRN;
        //    thread1 = new Thread(new ThreadStart(fun_print_control_bool));
        //    thread1.Start();
        //}

        //private static void fun_print_control_bool()
        //{
        //    workFlag = true;
        //    fun_print_Check(InspectionRecordNumber);
        //    workFlag = false;
        //}

        /// <summary>
        /// 打印检验记录单
        /// </summary>
        /// <param name="product_sn"></param>
        /// <param name="blPreview"></param>
        public static void fun_print_Check_First(string InspectionRecordNumber, bool blPreview = false)
        {
            //string fileName = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
            string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp" + Guid.NewGuid().ToString() + ".xlsx"; ;
            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                {
                    System.Data.DataTable dtPP = new System.Data.DataTable();
                    string s = "select * from 基础记录打印模板表 where 模板名 = '质检-来料检验记录'";
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
                System.Data.DataTable dt = new System.Data.DataTable();

                //检验记录采购件检验表
                string sqlstr = "";

                dt = new System.Data.DataTable();
                sqlstr = "SELECT [检验记录单号],[产品编号] ,[供应商编号],[送检单号],[检验日期],[送检数量],[抽检数量],[检验员],[检验结果],[不合格数量] FROM 采购记录采购单检验主表 WHERE [检验记录单号]='{0}'";
                //sqlstr = "SELECT [检验记录单号],[产品编号] ,[供应商编号],[采购入库通知单号],[检验日期],[送检数量],[抽检数量],[检验员],[检验结果],[不合格数量] FROM 检验记录采购件检验表 WHERE [检验记录单号]='{0}'";
                sqlstr = string.Format(sqlstr, InspectionRecordNumber);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("没有找到这个检验记录单号");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                string str_gys_number = dt.Rows[0]["供应商编号"].ToString();
                string str_produce_number = dt.Rows[0]["产品编号"].ToString();

                range = ws.get_Range("AV5", Type.Missing);
                range.Value2 = dt.Rows[0]["检验记录单号"].ToString();

                range = ws.get_Range("AE7", Type.Missing);
                range.Value2 = dt.Rows[0]["送检单号"].ToString();

                range = ws.get_Range("AR7", Type.Missing);
                range.Value2 = dt.Rows[0]["检验日期"].ToString();

                range = ws.get_Range("H8", Type.Missing);
                range.Value2 = dt.Rows[0]["送检数量"].ToString();

                range = ws.get_Range("AE8", Type.Missing);
                range.Value2 = dt.Rows[0]["抽检数量"].ToString();

                range = ws.get_Range("AR8", Type.Missing);
                range.Value2 = dt.Rows[0]["不合格数量"].ToString();
                // range = ws.get_Range("E11", Type.Missing);
                //range = range.MergeArea;
                // double num1 = (double)range.Width;
                // range = ws.get_Range("L11", Type.Missing);
                // range = range.MergeArea;
                // double num2 = (double)range.Width;

                range = ws.get_Range("E31", Type.Missing);
                range.Value2 = dt.Rows[0]["检验结果"].ToString();

                range = ws.get_Range("T33", Type.Missing);
                range.Value2 = dt.Rows[0]["检验员"].ToString();

                range = ws.get_Range("AP33", Type.Missing);
                range.Value2 = Convert.ToDateTime(dt.Rows[0]["检验日期"]).Year.ToString();

                range = ws.get_Range("AY33", Type.Missing);
                range.Value2 = Convert.ToDateTime(dt.Rows[0]["检验日期"]).Month.ToString();

                range = ws.get_Range("BC33", Type.Missing);
                range.Value2 = Convert.ToDateTime(dt.Rows[0]["检验日期"]).Day.ToString();

                string str_gys = "";
                string str_produce = "";
                string str_type = "";

                dt = new System.Data.DataTable();
                sqlstr = "select 供应商名称 from 采购供应商表 where 供应商ID='{0}'";
               // sqlstr = "SELECT [gysmc]  FROM [工作用临时数据库].[dbo].[gys]where[gysbh]='{0}'";
                sqlstr = string.Format(sqlstr, str_gys_number);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    //new SqlDataAdapter(sqlstr, CPublic.Var.geConn("WL")).Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_gys = dt.Rows[0]["供应商名称"].ToString();
                        //str_gys = dt.Rows[0]["gysmc"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                dt = new System.Data.DataTable();
                sqlstr = "select 物料名称,规格型号 from 基础数据物料信息表 where 物料编码='{0}'";
                //sqlstr = "SELECT [cpmc],[ggxh]  FROM [工作用临时数据库].[dbo].[cp]where  [cpbh]='{0}'";
                sqlstr = string.Format(sqlstr, str_produce_number);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    //new SqlDataAdapter(sqlstr, CPublic.Var.geConn("WL")).Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_produce = dt.Rows[0]["物料名称"].ToString();
                        str_type = dt.Rows[0]["规格型号"].ToString();
                        //str_produce = dt.Rows[0]["cpmc"].ToString();
                        //str_type = dt.Rows[0]["ggxh"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                range = ws.get_Range("H6", Type.Missing);
                range.Value2 = str_produce;

                range = ws.get_Range("AK6", Type.Missing);
                range.Value2 = str_gys;

                range = ws.get_Range("H7", Type.Missing);
                range.Value2 = str_type;

                //检验记录采购件检验明细表

                System.Data.DataTable dt_circulation = new System.Data.DataTable();

                try
                {
                    sqlstr = "SELECT[POS],[检验项目],[检验要求],[检验下限],[检验上限],[允许下限],[允许上限],[合格] ,[不合格原因]FROM 采购记录采购单检验明细表 where [检验记录单号]='{0}'order by [POS]";
                    //sqlstr = "SELECT[POS],[检验项目],[检验要求],[检验下限],[检验上限],[允许下限],[允许上限],[合格] ,[不合格原因]FROM 检验记录采购件检验明细表 where [检验记录单号]='{0}'order by [POS]";
                    sqlstr = string.Format(sqlstr, InspectionRecordNumber);
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt_circulation);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                int pos = 0;
                int i_first = 11;
                foreach (DataRow r in dt_circulation.Rows)
                {
                    range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["POS"].ToString();

                    range = ws.get_Range("E" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验项目"].ToString();

                    range = ws.get_Range("L" + (i_first + pos).ToString(), Type.Missing);
                    range = range.MergeArea;
                    System.Windows.Forms.Form fm = new System.Windows.Forms.Form();
                    System.Drawing.Graphics g = fm.CreateGraphics();

                    System.Drawing.Font fn = new System.Drawing.Font(range.Font.Name.ToString(), float.Parse(range.Font.Size.ToString()));
                    int iL = ((int)(g.MeasureString(r["检验要求"].ToString(), fn, Convert.ToInt32(range.Width)).Height / fn.Height) == 0) ? 1 : (int)(g.MeasureString(r["检验要求"].ToString(), fn, Convert.ToInt32(range.Width)).Height / fn.Height);

                    range.RowHeight = (double)range.Height * iL;
                    range.Value2 = r["检验要求"].ToString();

                    range = ws.get_Range("AE" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验下限"].ToString();

                    range = ws.get_Range("AH" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验上限"].ToString();

                    range = ws.get_Range("AL" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["允许下限"].ToString();

                    range = ws.get_Range("AP" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["允许上限"].ToString();

                    range = ws.get_Range("AT" + (i_first + pos).ToString(), Type.Missing);
                    if ((bool)r["合格"] == true)
                    {
                        range.Value2 = "合格";
                    }
                    if ((bool)r["合格"] == false)
                    {
                        range.Value2 = "不合格";
                    }

                    range = ws.get_Range("AY" + (i_first + pos).ToString(), Type.Missing);
                    range = range.MergeArea;
                    System.Windows.Forms.Form fm_N = new System.Windows.Forms.Form();
                    System.Drawing.Graphics g_N = fm_N.CreateGraphics();

                    System.Drawing.Font fn_N = new System.Drawing.Font(range.Font.Name.ToString(), float.Parse(range.Font.Size.ToString()));
                    int iL_N = ((int)(g_N.MeasureString(r["不合格原因"].ToString(), fn_N, Convert.ToInt32(range.Width)).Height / fn.Height) == 0) ? 1 : (int)(g_N.MeasureString(r["不合格原因"].ToString(), fn_N, Convert.ToInt32(range.Width)).Height / fn.Height);

                    range.RowHeight = (double)range.Height * iL_N;
                    range.Value2 = r["不合格原因"].ToString();

                    pos++;
                }

                if (pos <= 19)
                {
                    range = ws.get_Range("B" + (i_first + pos).ToString(), "B30");
                    range.EntireRow.Delete(Type.Missing);
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
                }
            }
        }

        /// <summary>
        /// 输出检验记录单到Excel
        /// </summary>
        /// <param name="InspectionRecordNumber"></param>
        /// <param name="blPreview"></param>
        public static void fun_print_Check_ToExcel_First(string InspectionRecordNumber, string foldPath, bool blPreview = false)
        {
            string fileName = foldPath + "\\" + InspectionRecordNumber + ".xlsx";

            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                {
                    System.Data.DataTable dtPP = new System.Data.DataTable();
                    string s = "select * from 基础记录打印模板表 where 模板名 = '质检-来料检验记录'";
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
                System.Data.DataTable dt = new System.Data.DataTable();

                //检验记录采购件检验表
                string sqlstr = "";

                dt = new System.Data.DataTable();
                sqlstr = "SELECT [检验记录单号],[产品编号] ,[供应商编号],[送检单号],[检验日期],[送检数量],[抽检数量],[不合格数量]FROM [ERPDB].[dbo].[采购记录采购单检验明细表]WHERE [检验记录单号]='{0}'";
                //sqlstr = "SELECT [检验记录单号],[产品编号] ,[供应商编号],[采购入库通知单号],[检验日期],[送检数量],[抽检数量],[不合格数量]FROM [工作用临时数据库].[dbo].[检验记录采购件检验表]WHERE [检验记录单号]='{0}'";
                sqlstr = string.Format(sqlstr, InspectionRecordNumber);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("没有找到这个检验记录单号");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                string str_gys_number = dt.Rows[0]["供应商编号"].ToString();
                string str_produce_number = dt.Rows[0]["产品编号"].ToString();

                range = ws.get_Range("AV5", Type.Missing);
                range.Value2 = dt.Rows[0]["检验记录单号"].ToString();

                range = ws.get_Range("AE7", Type.Missing);
                range.Value2 = dt.Rows[0]["送检单号"].ToString();

                range = ws.get_Range("AR7", Type.Missing);
                range.Value2 = dt.Rows[0]["检验日期"].ToString();

                range = ws.get_Range("H8", Type.Missing);
                range.Value2 = dt.Rows[0]["送检数量"].ToString();

                range = ws.get_Range("AE8", Type.Missing);
                range.Value2 = dt.Rows[0]["抽检数量"].ToString();

                range = ws.get_Range("AR8", Type.Missing);
                range.Value2 = dt.Rows[0]["不合格数量"].ToString();
                // range = ws.get_Range("E11", Type.Missing);
                //range = range.MergeArea;
                // double num1 = (double)range.Width;
                // range = ws.get_Range("L11", Type.Missing);
                // range = range.MergeArea;
                // double num2 = (double)range.Width;

                string str_gys = "";
                string str_produce = "";
                string str_type = "";

                dt = new System.Data.DataTable();
                sqlstr = "SELECT [供应商名称]  FROM [ERPDB].[dbo].[采购供应商表]where[供应商ID]='{0}'";
                //sqlstr = "SELECT [gysmc]  FROM [工作用临时数据库].[dbo].[gys]where[gysbh]='{0}'";
                sqlstr = string.Format(sqlstr, str_gys_number);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    //new SqlDataAdapter(sqlstr, CPublic.Var.geConn("WL")).Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_gys = dt.Rows[0]["供应商名称"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                dt = new System.Data.DataTable();
                sqlstr = "SELECT [物料名称],[规格型号]  FROM [ERPDB].[dbo].[基础数据物料信息表]where  [物料编码]='{0}'";
                //sqlstr = "SELECT [cpmc],[ggxh]  FROM [工作用临时数据库].[dbo].[cp]where  [cpbh]='{0}'";
                sqlstr = string.Format(sqlstr, str_produce_number);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    //new SqlDataAdapter(sqlstr, CPublic.Var.geConn("WL")).Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_produce = dt.Rows[0]["物料名称"].ToString();
                        str_type = dt.Rows[0]["规格型号"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                range = ws.get_Range("H6", Type.Missing);
                range.Value2 = str_produce;

                range = ws.get_Range("AK6", Type.Missing);
                range.Value2 = str_gys;

                range = ws.get_Range("H7", Type.Missing);
                range.Value2 = str_type;

                //检验记录采购件检验明细表

                System.Data.DataTable dt_circulation = new System.Data.DataTable();

                try
                {
                    sqlstr = "SELECT[POS],[检验项目],[检验要求],[检验下限],[检验上限],[允许下限],[允许上限],[合格] ,[备注]FROM [工作用临时数据库].[dbo].[采购记录采购单检验明细表]where [检验记录单号]='{0}'order by [POS]";
                    sqlstr = string.Format(sqlstr, InspectionRecordNumber);
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt_circulation);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                int pos = 0;
                int i_first = 11;
                foreach (DataRow r in dt_circulation.Rows)
                {
                    range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["POS"].ToString();

                    range = ws.get_Range("E" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验项目"].ToString();

                    range = ws.get_Range("L" + (i_first + pos).ToString(), Type.Missing);
                    range = range.MergeArea;
                    System.Windows.Forms.Form fm = new System.Windows.Forms.Form();
                    System.Drawing.Graphics g = fm.CreateGraphics();

                    System.Drawing.Font fn = new System.Drawing.Font(range.Font.Name.ToString(), float.Parse(range.Font.Size.ToString()));
                    int iL = (int)(g.MeasureString(r["检验要求"].ToString(), fn, Convert.ToInt32(range.Width)).Height / fn.Height);

                    range.RowHeight = (double)range.Height * iL;
                    range.Value2 = r["检验要求"].ToString();

                    range = ws.get_Range("AE" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验下限"].ToString();

                    range = ws.get_Range("AH" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验上限"].ToString();

                    range = ws.get_Range("AL" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["允许下限"].ToString();

                    range = ws.get_Range("AP" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["允许上限"].ToString();

                    range = ws.get_Range("AT" + (i_first + pos).ToString(), Type.Missing);
                    if ((bool)r["合格"] == true)
                    {
                        range.Value2 = "合格";
                    }
                    if ((bool)r["合格"] == false)
                    {
                        range.Value2 = "不合格";
                    }

                    range = ws.get_Range("AY" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["备注"].ToString();

                    pos++;
                }

                if (pos <= 19)
                {
                    range = ws.get_Range("B" + (i_first + pos).ToString(), "B30");
                    range.EntireRow.Delete(Type.Missing);
                }

                if (blPreview)
                {
                    //excelApp.Visible = true;
                    wb.Save();
                    //wb.PrintPreview();
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

        /// <summary>
        /// 打印检验记录单
        /// </summary>
        /// <param name="product_sn"></param>
        /// <param name="blPreview"></param>
        public static void fun_print_Check(string InspectionRecordNumber, bool blPreview = false)
        {
            string path = System.Windows.Forms.Application.StartupPath + @"\prttmp";
            if (!System.IO.File.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx"; 
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\质检-来料检验记录.xlsx";
            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                {
                    //System.Data.DataTable dtPP = new System.Data.DataTable();
                    //string s = "select * from 基础记录打印模板表 where 模板名 = '质检-来料检验记录'";
                    //new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                    //if (dtPP.Rows.Count == 0) return;

                    //try
                    //{
                    //    System.IO.Directory.Delete(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\prttmp", true);
                    //}
                    //catch
                    //{
                    //}
                    //System.IO.Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\prttmp");
                    //System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);

                    if (System.IO.File.Exists(fileName).Equals(false))
                    {
                        FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                        fs.Close();
                        System.Data.DataTable dtPP = new System.Data.DataTable();
                        string s = "select * from 基础记录打印模板表 where 模板名 = '质检-来料检验记录'";
                        new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                        if (dtPP.Rows.Count == 0) return;

                        System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
                    }
                }

                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing);
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;
                System.Data.DataTable dt = new System.Data.DataTable();

                //检验记录采购件检验表
                string sqlstr = "";

                dt = new System.Data.DataTable();
                sqlstr = "SELECT [检验记录单号],[产品编号] ,[供应商编号],[送检单号],[检验日期],[送检数量],[抽检数量],[检验员],[检验结果],[不合格数量] FROM 采购记录采购单检验主表 WHERE [检验记录单号]='{0}'";
                sqlstr = string.Format(sqlstr, InspectionRecordNumber);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("没有找到这个检验记录单号");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                string str_gys_number = dt.Rows[0]["供应商编号"].ToString();
                string str_produce_number = dt.Rows[0]["产品编号"].ToString();

                range = ws.get_Range("BY5", Type.Missing);
                range.Value2 = dt.Rows[0]["检验记录单号"].ToString();

                range = ws.get_Range("AN7", Type.Missing);
                range.Value2 = dt.Rows[0]["送检数量"].ToString();

                range = ws.get_Range("BQ7", Type.Missing);
                range.Value2 = dt.Rows[0]["送检单号"].ToString();

                range = ws.get_Range("E36", Type.Missing);
                range.Value2 = dt.Rows[0]["检验结果"].ToString();

                range = ws.get_Range("AJ36", Type.Missing);
                range.Value2 = dt.Rows[0]["检验员"].ToString();

                range = ws.get_Range("BL36", Type.Missing);
                range.Value2 = dt.Rows[0]["检验日期"].ToString();

                string str_gys = "";
                string str_produce = "";
                string str_type = "";

                dt = new System.Data.DataTable();
                sqlstr = "SELECT [供应商名称]  FROM [采购供应商表]where[供应商ID]='{0}'";
                //sqlstr = "SELECT [gysmc]  FROM [gys]where[gysbh]='{0}'";
                sqlstr = string.Format(sqlstr, str_gys_number);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_gys = dt.Rows[0]["供应商名称"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                dt = new System.Data.DataTable();
                sqlstr = "SELECT [物料名称],[n原ERP规格型号]  FROM [基础数据物料信息表]where  [物料编码]='{0}'";
                //sqlstr = "SELECT [cpmc],[ggxh]  FROM [cp]where  [cpbh]='{0}'";
                sqlstr = string.Format(sqlstr, str_produce_number);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_produce = dt.Rows[0]["物料名称"].ToString();
                        str_type = dt.Rows[0]["n原ERP规格型号"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                range = ws.get_Range("I6", Type.Missing);
                range.Value2 = str_produce;

                range = ws.get_Range("BC6", Type.Missing);
                range.Value2 = str_gys;

                range = ws.get_Range("I7", Type.Missing);
                range.Value2 = str_type;

                //检验记录采购件检验明细表

                System.Data.DataTable dt_circulation = new System.Data.DataTable();

                try
                {
                    sqlstr = "SELECT[POS],[检验项目],[检验要求],[抽检数],[扩大值],[检验下限],[检验上限],[允许下限],[允许上限],[合格] ,[不合格原因],[不合格数量] FROM 采购记录采购单检验明细表 where [检验记录单号]='{0}'order by [POS]";
                    sqlstr = string.Format(sqlstr, InspectionRecordNumber);
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt_circulation);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                int pos = 0;
                int i_first = 10;
                foreach (DataRow r in dt_circulation.Rows)
                {
                    range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["POS"].ToString();

                    range = ws.get_Range("E" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验项目"].ToString();

                    range = ws.get_Range("L" + (i_first + pos).ToString(), Type.Missing);
                    range = range.MergeArea;
                    System.Windows.Forms.Form fm = new System.Windows.Forms.Form();
                    System.Drawing.Graphics g = fm.CreateGraphics();

                    System.Drawing.Font fn = new System.Drawing.Font(range.Font.Name.ToString(), float.Parse(range.Font.Size.ToString()));
                    int iL = ((int)(g.MeasureString(r["检验要求"].ToString(), fn, Convert.ToInt32(range.Width)).Height / fn.Height) == 0) ? 1 : (int)(g.MeasureString(r["检验要求"].ToString(), fn, Convert.ToInt32(range.Width)).Height / fn.Height);

                    range.RowHeight = (double)range.Height * iL;
                    range.Value2 = r["检验要求"].ToString();

                    range = ws.get_Range("AE" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["抽检数"].ToString();

                    range = ws.get_Range("AJ" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["扩大值"].ToString();

                    range = ws.get_Range("AN" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["允许上限"].ToString();

                    range = ws.get_Range("AR" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["允许下限"].ToString();

                    range = ws.get_Range("AV" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验上限"].ToString();

                    range = ws.get_Range("AZ" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验下限"].ToString();

                    range = ws.get_Range("BD" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = ((bool)r["合格"] == true) ? "合格" : "不合格";

                    range = ws.get_Range("BI" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["不合格数量"].ToString();

                    range = ws.get_Range("BN" + (i_first + pos).ToString(), Type.Missing);
                    range = range.MergeArea;
                    System.Windows.Forms.Form fm_N = new System.Windows.Forms.Form();
                    System.Drawing.Graphics g_N = fm_N.CreateGraphics();

                    System.Drawing.Font fn_N = new System.Drawing.Font(range.Font.Name.ToString(), float.Parse(range.Font.Size.ToString()));
                    int iL_N = ((int)(g_N.MeasureString(r["不合格原因"].ToString(), fn_N, Convert.ToInt32(range.Width)).Height / fn.Height) == 0) ? 1 : (int)(g_N.MeasureString(r["不合格原因"].ToString(), fn_N, Convert.ToInt32(range.Width)).Height / fn.Height);

                    range.RowHeight = (double)range.Height * iL_N;
                    range.Value2 = r["不合格原因"].ToString();

                    pos++;
                }

                if (pos <= 25)
                {
                    range = ws.get_Range("B" + (i_first + pos).ToString(), "B35");
                    range.EntireRow.Delete(Type.Missing);
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
                }
            }
        }

        /// <summary>
        /// 输出检验记录单到Excel
        /// </summary>
        /// <param name="InspectionRecordNumber"></param>
        /// <param name="blPreview"></param>
        public static void fun_print_Check_ToExcel(string InspectionRecordNumber, string fileName, bool blPreview = false)
        {
            //string fileName = foldPath + "\\" + InspectionRecordNumber + ".xlsx";

            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                {
                    System.Data.DataTable dtPP = new System.Data.DataTable();
                    string s = "select * from 基础记录打印模板表 where 模板名 = '质检-来料检验记录'";
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
                System.Data.DataTable dt = new System.Data.DataTable();

                //检验记录采购件检验表
                string sqlstr = "";

                dt = new System.Data.DataTable();
                sqlstr = "SELECT [检验记录单号],[产品编号] ,[供应商编号],[送检单号],[检验日期],[送检数量],[抽检数量],[检验员],[检验结果],[不合格数量] FROM 采购记录采购单检验主表 WHERE [检验记录单号]='{0}'";
                sqlstr = string.Format(sqlstr, InspectionRecordNumber);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("没有找到这个检验记录单号");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                string str_gys_number = dt.Rows[0]["供应商编号"].ToString();
                string str_produce_number = dt.Rows[0]["产品编号"].ToString();

                range = ws.get_Range("BY5", Type.Missing);
                range.Value2 = dt.Rows[0]["检验记录单号"].ToString();

                range = ws.get_Range("AN7", Type.Missing);
                range.Value2 = dt.Rows[0]["送检数量"].ToString();

                range = ws.get_Range("BQ7", Type.Missing);
                range.Value2 = dt.Rows[0]["送检单号"].ToString();

                range = ws.get_Range("E36", Type.Missing);
                range.Value2 = dt.Rows[0]["检验结果"].ToString();

                range = ws.get_Range("AJ36", Type.Missing);
                range.Value2 = dt.Rows[0]["检验员"].ToString();

                range = ws.get_Range("BL36", Type.Missing);
                range.Value2 = dt.Rows[0]["检验日期"].ToString();

                string str_gys = "";
                string str_produce = "";
                string str_type = "";

                dt = new System.Data.DataTable();
                sqlstr = "SELECT [供应商名称]  FROM [采购供应商表]where[供应商ID]='{0}'";
                //sqlstr = "SELECT [gysmc]  FROM [gys]where[gysbh]='{0}'";
                sqlstr = string.Format(sqlstr, str_gys_number);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_gys = dt.Rows[0]["供应商名称"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                dt = new System.Data.DataTable();
                sqlstr = "SELECT [物料名称],[规格型号]  FROM [基础数据物料信息表]where  [物料编码]='{0}'";
                sqlstr = string.Format(sqlstr, str_produce_number);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_produce = dt.Rows[0]["物料名称"].ToString();
                        str_type = dt.Rows[0]["规格型号"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                range = ws.get_Range("I6", Type.Missing);
                range.Value2 = str_produce;

                range = ws.get_Range("BC6", Type.Missing);
                range.Value2 = str_gys;

                range = ws.get_Range("I7", Type.Missing);
                range.Value2 = str_type;

                //检验记录采购件检验明细表

                System.Data.DataTable dt_circulation = new System.Data.DataTable();

                try
                {
                    sqlstr = "SELECT[POS],[检验项目],[检验要求],[抽检数],[扩大值],[检验下限],[检验上限],[允许下限],[允许上限],[合格] ,[不合格原因],[不合格数量] FROM 采购记录采购单检验明细表 where [检验记录单号]='{0}'order by [POS]";
                    sqlstr = string.Format(sqlstr, InspectionRecordNumber);
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt_circulation);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                int pos = 0;
                int i_first = 10;
                foreach (DataRow r in dt_circulation.Rows)
                {
                    range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["POS"].ToString();

                    range = ws.get_Range("E" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验项目"].ToString();

                    range = ws.get_Range("L" + (i_first + pos).ToString(), Type.Missing);
                    range = range.MergeArea;
                    System.Windows.Forms.Form fm = new System.Windows.Forms.Form();
                    System.Drawing.Graphics g = fm.CreateGraphics();

                    System.Drawing.Font fn = new System.Drawing.Font(range.Font.Name.ToString(), float.Parse(range.Font.Size.ToString()));
                    int iL = ((int)(g.MeasureString(r["检验要求"].ToString(), fn, Convert.ToInt32(range.Width)).Height / fn.Height) == 0) ? 1 : (int)(g.MeasureString(r["检验要求"].ToString(), fn, Convert.ToInt32(range.Width)).Height / fn.Height);

                    range.RowHeight = (double)range.Height * iL;
                    range.Value2 = r["检验要求"].ToString();

                    range = ws.get_Range("AE" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["抽检数"].ToString();

                    range = ws.get_Range("AJ" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["扩大值"].ToString();

                    range = ws.get_Range("AN" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["允许上限"].ToString();

                    range = ws.get_Range("AR" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["允许下限"].ToString();

                    range = ws.get_Range("AV" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验上限"].ToString();

                    range = ws.get_Range("AZ" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["检验下限"].ToString();

                    range = ws.get_Range("BD" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = ((bool)r["合格"] == true) ? "合格" : "不合格";

                    range = ws.get_Range("BI" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["不合格数量"].ToString();

                    range = ws.get_Range("BN" + (i_first + pos).ToString(), Type.Missing);
                    range = range.MergeArea;
                    System.Windows.Forms.Form fm_N = new System.Windows.Forms.Form();
                    System.Drawing.Graphics g_N = fm_N.CreateGraphics();

                    System.Drawing.Font fn_N = new System.Drawing.Font(range.Font.Name.ToString(), float.Parse(range.Font.Size.ToString()));
                    int iL_N = ((int)(g_N.MeasureString(r["不合格原因"].ToString(), fn_N, Convert.ToInt32(range.Width)).Height / fn.Height) == 0) ? 1 : (int)(g_N.MeasureString(r["不合格原因"].ToString(), fn_N, Convert.ToInt32(range.Width)).Height / fn.Height);

                    range.RowHeight = (double)range.Height * iL_N;
                    range.Value2 = r["不合格原因"].ToString();

                    pos++;
                }

                if (pos <= 25)
                {
                    range = ws.get_Range("B" + (i_first + pos).ToString(), "B35");
                    range.EntireRow.Delete(Type.Missing);
                }

                if (blPreview)
                {
                    //excelApp.Visible = true;
                    wb.Save();
                    //wb.PrintPreview();
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
    }
}