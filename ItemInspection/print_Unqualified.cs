using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace ItemInspection
{
    public class print_Unqualified
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool BringWindowToTop(IntPtr hWnd);//将指定的窗口带至窗口列表顶部

        /// <summary>
        /// 打印不合格通知单
        /// </summary>
        /// <param name="product_sn"></param>
        /// <param name="blPreview"></param>
        public static void fun_print_Unqualified(string InspectionRecordNumber, bool blPreview = false)
        {
            string fileName = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                {
                    System.Data.DataTable dtPP = new System.Data.DataTable();
                    string s = "select * from 基础记录打印模板表 where 模板名 = '质检-不合格通知单'";
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

                //检验记录采购件检验明细表

                string sqlstr = "";

                dt = new System.Data.DataTable();
                sqlstr = "SELECT N.供应商编号,N.采购入库通知单号,N.产品编号,N.抽检数量,M.不合格原因,N.检验员,N.检验日期,N.不合格数量 FROM [工作用临时数据库].[dbo].[检验记录采购件检验明细表] AS M,[工作用临时数据库].[dbo].[检验记录采购件检验表]AS N WHERE M.[检验记录单号]=N.[检验记录单号]AND N.[检验记录单号]='{0}'";
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

                range = ws.get_Range("BC6", Type.Missing);
                range.Value2 = dt.Rows[0]["采购入库通知单号"].ToString();

                range = ws.get_Range("AH7", Type.Missing);
                range.Value2 = dt.Rows[0]["抽检数量"].ToString();

                range = ws.get_Range("BC7", Type.Missing);
                range.Value2 = (((Decimal)dt.Rows[0]["抽检数量"] - (Decimal)dt.Rows[0]["不合格数量"]) / (Decimal)dt.Rows[0]["抽检数量"] * 100).ToString() + "%";

                range = ws.get_Range("AO26", Type.Missing);
                range.Value2 = dt.Rows[0]["检验员"].ToString();

                range = ws.get_Range("BB26", Type.Missing);
                range.Value2 = dt.Rows[0]["检验日期"].ToString();

                string str_reason = "";

                foreach (DataRow r in dt.Rows)
                {
                    str_reason += r["不合格原因"].ToString() + System.Environment.NewLine;
                }

                range = ws.get_Range("M10", Type.Missing);
                range.Value2 = str_reason;

                string str_gys = "";
                string str_produce = "";

                dt = new System.Data.DataTable();
                sqlstr = "SELECT [gysmc]  FROM [工作用临时数据库].[dbo].[gys]where[gysbh]='{0}'";
                sqlstr = string.Format(sqlstr, str_gys_number);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_gys = dt.Rows[0]["gysmc"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                dt = new System.Data.DataTable();
                sqlstr = "SELECT[cpmc] FROM [工作用临时数据库].[dbo].[cp]where  [cpbh]='{0}'";
                sqlstr = string.Format(sqlstr, str_produce_number);
                try
                {
                    new SqlDataAdapter(sqlstr, CPublic.Var.strConn).Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        str_produce = dt.Rows[0]["cpmc"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                range = ws.get_Range("M6", Type.Missing);
                range.Value2 = str_gys;
                range = ws.get_Range("M7", Type.Missing);
                range.Value2 = str_produce;

                //int pos = 0;
                //{
                //    if (pos == 1)
                //    {
                //        range = ws.get_Range("AO" + (10 + pos).ToString(), Type.Missing);
                //        range.Value2 = "";
                //    }
                //}

                //if (pos <= 19)
                //{
                //    range = ws.get_Range("A30".ToString(), "A58");
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
    }
}