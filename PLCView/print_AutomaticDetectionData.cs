using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Text;

namespace PLCView
{
    public class print_AutomaticDetectionData
    {
        //private static string PWD = "a";
        //private static string UID = "sa1";
        //private static string SQLSERVER = "192.168.10.2";
        //private static string DATABASE = "工作用临时数据库";
        //private static string strconn = string.Format("Password={0};Persist Security Info=True;User ID={1};Initial Catalog={2};Data Source={3};Pooling=true;Max Pool Size=40000;Min Pool Size=0", PWD, UID, DATABASE, SQLSERVER);


        //[DllImport("User32.dll", CharSet = CharSet.Auto)]
        //internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern bool BringWindowToTop(IntPtr hWnd);

        //public static void fun_print_print_AutomaticDetectionData(System.Data.DataTable dtP,Dictionary<string,string> Dic_str, bool blPreview = false)
        //{
        //    string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx"; ;
        //    ApplicationClass excelApp = new ApplicationClass();

        //    IntPtr hwnd = new IntPtr(excelApp.Hwnd);
        //    IntPtr PID = IntPtr.Zero;
        //    GetWindowThreadProcessId(hwnd, out PID);
        //    try
        //    {
        //        {
        //            System.Data.DataTable dtPP = new System.Data.DataTable();
        //            string s = "";

        //            if (Dic_str["F"].ToString() == "A")
        //            {
        //                s = "select * from 基础记录打印模板表 where 模板名 = '质检-自动检测数据统计表SN号'";
        //            }
        //            else
        //            {
        //                s = "select * from 基础记录打印模板表 where 模板名 = '质检-自动检测数据统计表'";
        //            }
        //            new SqlDataAdapter(s, strconn).Fill(dtPP);
        //            if (dtPP.Rows.Count == 0) return;
        //            try
        //            {
        //                System.IO.Directory.Delete(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp", true);
        //            }
        //            catch
        //            {
        //            }
        //            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp");
        //            System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
        //        }

        //        Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing);
        //        excelApp.Visible = false;
        //        excelApp.DisplayAlerts = false;
        //        Worksheet ws = (Worksheet)wb.Worksheets[1];

        //        Microsoft.Office.Interop.Excel.Range range;
        //        range = ws.get_Range("J4", Type.Missing);
        //        range.Value2 = Dic_str["检测项目"].ToString();
        //        if (Dic_str["F"].ToString() == "A")
        //        {
        //            range = ws.get_Range("AE4", Type.Missing);
        //            range.Value2 = Dic_str["SN号"].ToString();
        //        }
        //        else
        //        {
        //            range = ws.get_Range("AE4", Type.Missing);
        //            range.Value2 = Dic_str["日期"].ToString();
        //        }
        //        range = ws.get_Range("J6", Type.Missing);
        //        range.Value2 = Dic_str["总数量"].ToString();
        //        range = ws.get_Range("AE6", Type.Missing);
        //        range.Value2 = Dic_str["不合格数量"].ToString();
        //        range = ws.get_Range("AZ6", Type.Missing);
        //        range.Value2 = Dic_str["合格数量"].ToString();
        //        range = ws.get_Range("BU6", Type.Missing);
        //        range.Value2 = Dic_str["合格率"].ToString();

        //        int pos = 0;
        //        int i_first = 10;
        //        foreach (System.Data.DataRow r in dtP.Rows)
        //        {
        //            if (pos == dtP.Rows.Count - 1)
        //            {
        //                if (pos <= 331)
        //                {
        //                    range = ws.get_Range("B" + (i_first + pos).ToString(), "B331");
        //                    range.EntireRow.Delete(Type.Missing);
        //                }
        //            }
        //            range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
        //            range.Value2 = r["出错检测组POS"].ToString();
        //            range = ws.get_Range("F" + (i_first + pos).ToString(), Type.Missing);
        //            range.Value2 = r["出错主动作POS"].ToString();
        //            range = ws.get_Range("J" + (i_first + pos).ToString(), Type.Missing);
        //            range.Value2 = r["出错检测要求"].ToString();
        //            range = ws.get_Range("BR" + (i_first + pos).ToString(), Type.Missing);
        //            range.Value2 = r["出错主动作说明"].ToString();
        //            range = ws.get_Range("CE" + (i_first + pos).ToString(), Type.Missing);
        //            range.Value2 = r["数量"].ToString();

        //            pos++;

        //        }

        //        if (blPreview)
        //        {
        //            excelApp.Visible = true;
        //            wb.PrintPreview();
        //        }
        //        else
        //        {
        //            excelApp.Visible = false;
        //            BringWindowToTop(hwnd);
        //            wb.PrintOutEx();
        //            excelApp.Quit();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (PID != IntPtr.Zero)
        //        {
        //            excelApp = null;
        //            GcCollect();
        //            KillProcess(PID);
        //        }
        //    }
        //}

        //public static void fun_print_print_AutomaticDetectionData_ToExcel(System.Data.DataTable dtP,Dictionary<string,string> Dic_str, string fileName, bool blPreview = false)
        //{
        //    ApplicationClass excelApp = new ApplicationClass();

        //    IntPtr hwnd = new IntPtr(excelApp.Hwnd);
        //    IntPtr PID = IntPtr.Zero;
        //    GetWindowThreadProcessId(hwnd, out PID);
        //    try
        //    {
        //        {
        //            System.Data.DataTable dtPP = new System.Data.DataTable();
        //            string s = "";

        //            if (Dic_str["F"].ToString() == "A")
        //            {
        //                s = "select * from 基础记录打印模板表 where 模板名 = '质检-自动检测数据统计表SN号'";
        //            }
        //            else
        //            {
        //                s = "select * from 基础记录打印模板表 where 模板名 = '质检-自动检测数据统计表'";
        //            }

    
        //            new SqlDataAdapter(s, strconn).Fill(dtPP);
        //            if (dtPP.Rows.Count == 0) return;

        //            System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
        //        }

        //        Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing);
        //        excelApp.Visible = false;
        //        excelApp.DisplayAlerts = false;
        //        Worksheet ws = (Worksheet)wb.Worksheets[1];

        //        Microsoft.Office.Interop.Excel.Range range;

        //        range = ws.get_Range("J4", Type.Missing);
        //        range.Value2 = Dic_str["检测项目"].ToString();

        //        if (Dic_str["F"].ToString() == "A")
        //        {
        //            range = ws.get_Range("AE4", Type.Missing);
        //            range.Value2 = Dic_str["SN号"].ToString();
        //        }
        //        else
        //        {
        //            range = ws.get_Range("AE4", Type.Missing);
        //            range.Value2 = Dic_str["日期"].ToString();
        //        }

        //        range = ws.get_Range("J6", Type.Missing);
        //        range.Value2 = Dic_str["总数量"].ToString();
        //        range = ws.get_Range("AE6", Type.Missing);
        //        range.Value2 = Dic_str["不合格数量"].ToString();
        //        range = ws.get_Range("AZ6", Type.Missing);
        //        range.Value2 = Dic_str["合格数量"].ToString();
        //        range = ws.get_Range("BU6", Type.Missing);
        //        range.Value2 = Dic_str["合格率"].ToString();

        //        int pos = 0;
        //        int i_first = 10;
        //        foreach (System.Data.DataRow r in dtP.Rows)
        //        {
        //            if (pos == dtP.Rows.Count-1)
        //            {
        //                if (pos <= 331)
        //                {
        //                    range = ws.get_Range("B" + (i_first + pos).ToString(), "B331");
        //                    range.EntireRow.Delete(Type.Missing);
        //                }
        //            }
        //            range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
        //            range.Value2 = r["出错检测组POS"].ToString();
        //            range = ws.get_Range("F" + (i_first + pos).ToString(), Type.Missing);
        //            range.Value2 = r["出错主动作POS"].ToString();
        //            range = ws.get_Range("J" + (i_first + pos).ToString(), Type.Missing);
        //            range.Value2 = r["出错检测要求"].ToString();
        //            range = ws.get_Range("BR" + (i_first + pos).ToString(), Type.Missing);
        //            range.Value2 = r["出错主动作说明"].ToString();
        //            range = ws.get_Range("CE" + (i_first + pos).ToString(), Type.Missing);
        //            range.Value2 = r["数量"].ToString();

        //            pos++;

        //        }
                
        //        if (blPreview)
        //        {
        //            wb.Save();
        //        }
        //        else
        //        {
        //            excelApp.Visible = false;
        //            BringWindowToTop(hwnd);
        //            wb.PrintOutEx();
        //            excelApp.Quit();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (PID != IntPtr.Zero)
        //        {
        //            excelApp = null;
        //            GcCollect();
        //            KillProcess(PID);
        //        }
        //    }
        //}

        ///// <summary>
        /////  回收垃圾
        ///// </summary>
        //public static void GcCollect()
        //{
        //    GC.Collect();
        //    GC.WaitForPendingFinalizers();
        //    GC.Collect();
        //    GC.WaitForPendingFinalizers();
        //}

        ///// <summary>
        ///// 杀死进程
        ///// </summary>
        ///// <param name="H"></param>
        //private static void KillProcess(IntPtr H)
        //{
        //    System.Diagnostics.Process myproc = new System.Diagnostics.Process();

        //    try
        //    {
        //        foreach (System.Diagnostics.Process thisproc in System.Diagnostics.Process.GetProcessesByName("excel"))
        //        {
        //            if (thisproc.Id == (int)H)
        //            {
        //                if (!thisproc.CloseMainWindow())
        //                {
        //                    thisproc.Kill();
        //                    System.Threading.Thread.Sleep(1000);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Console.WriteLine(ex.Message);
        //    }
        //}
    }
}
