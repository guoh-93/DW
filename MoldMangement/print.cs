using Microsoft.Office.Interop.Excel;

using System;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Printing;
using System.Drawing;


namespace MoldMangement
{
    public class print
    {
        #region
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool BringWindowToTop(IntPtr hWnd);

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
        #endregion

        public static void fun_打印开票(System .Data .DataTable dt,string  str_打印机)
        {
            int count = dt.Rows.Count / 15;
            if (dt.Rows.Count / 15 != 0)
            {
                count = count + 1;
            
            }
            //string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\计量器具历史记录卡.xlxs";
            string fileName = @"C:\Users\Administrator\Desktop\历史记录卡.xlsx";
            if (System .IO .File .Exists (fileName ).Equals (false ))
            {
                FileStream fs = new FileStream(fileName ,FileMode.Create ,FileAccess .Write );
                fs.Close();
                System.Data.DataTable dtpp = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '历史记录卡'";
                new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtpp);
                if (dtpp.Rows.Count == 0) 
                    return;
                System.IO.File.WriteAllBytes(fileName, (byte[])dtpp.Rows[0]["数据"]); 
            }
            ApplicationClass excelApp = new ApplicationClass();
            IntPtr hwnd = new IntPtr(excelApp .Hwnd );
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd ,out PID );
            try {

                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,
                                                        Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing);

                excelApp.Visible = false;
                excelApp.ScreenUpdating = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;

                //string sql = string.Format("select 计量器具编号,计量器具名称,计量器具规格,制造单位,测量范围,分度值,准确度,出厂日期,购置日期,领用日期,检定周期 from 计量器具明细卡表 where 计量器具编号 = '{0}'", dt.Rows[0]["计量器具编号"].ToString());
                string sql3 = string.Format(" select * from 计量器具基础信息表 where 计量器具编号='{0}'", dt.Rows[0]["计量器具编号"].ToString());
                //System.Data.DataTable dtt = new System.Data.DataTable();
                //new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dtt);
                range = ws.get_Range("B5", Type.Missing);
                range.Value2 = System.DateTime.Today.ToString("yyyy-MM-dd");
                //range.Value2 = Convert.ToDateTime(r["检定日期"]).ToString("yyyy-MM-dd");
                range = ws.get_Range("B6", Type.Missing);
                range.Value2 = CPublic.Var.localUserName;
                //DateTime tt = CPublic.Var.getDatetime();
                //string str_序号 = string.Format("PO{0}{1:00}{2:00}{3:00000}", tt.Year, tt.Month, tt.Day, CPublic.CNo.fun_得到最大流水号("PO", tt.Year, tt.Month));
                //range = ws.get_Range("D5", Type.Missing);
                //range.Value2 = str_序号;
                //string srr = dt.Rows[0][0].ToString();
                //string ss2 = srr.Split('-')[0].ToString();               
                range = ws.get_Range("L6", Type.Missing);
                //range.Value2 = ss2.ToString();
                range.Value2 = dt.Rows[0]["计量器具编号"].ToString();
                range = ws.get_Range("B7", Type.Missing);
                range.Value2 = dt.Rows[0]["计量器具名称"].ToString();
                range = ws.get_Range("F7", Type.Missing);
                range.Value2 = dt.Rows[0]["计量器具规格"].ToString();
                range = ws.get_Range("J7", Type.Missing);
                range.Value2 = dt.Rows[0]["测量范围"].ToString();
                range = ws.get_Range("N7", Type.Missing);
                range.Value2 = dt.Rows[0]["检定周期"].ToString();
                range = ws.get_Range("B8", Type.Missing);
                range.Value2 = dt.Rows[0]["计量器具编号"].ToString();
                range = ws.get_Range("F8", Type.Missing);
                range.Value2 = dt.Rows[0]["制造单位"].ToString();
                range = ws.get_Range("J8", Type.Missing);
                range.Value2 = dt.Rows[0]["出厂编号"].ToString();
                //range = ws.get_Range("N7", Type.Missing);
                //range.Value2 = dt.Rows[0]["准确度"].ToString();
                range = ws.get_Range("B9", Type.Missing);
                range.Value2 = Convert.ToDateTime(dt.Rows[0]["出厂日期"]).ToString("yyyy-MM-dd");
                range = ws.get_Range("F9", Type.Missing);
                range.Value2 = Convert.ToDateTime(dt.Rows[0]["购置日期"]).ToString("yyyy-MM-dd");
                range = ws.get_Range("J9", Type.Missing);
                range.Value2 = Convert.ToDateTime(dt.Rows[0]["领用日期"]).ToString("yyyy-MM-dd");
                //range = ws.get_Range("N8", Type.Missing);
                //range.Value2 = dt.Rows[0]["检定周期"].ToString();

                for (int j = 1; j < count; j++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);

                }


                int pos = 1;
                int i_第几张 = 1;
                int i_first = 11;//第几行开始
                //int i_第几张 = 1;
                int i_count = 15; //每页多少条记录

                foreach (System.Data.DataRow r in dt .Rows )
                {
                    //range = ws.get_Range("A" + (i_first).ToString(), Type.Missing);
                    //range.Value2 = (i).ToString();//序号
                   
                    range = ws.get_Range("A" + (i_first).ToString(), Type.Missing);
                    range.Value2 = Convert.ToDateTime(r["检定日期"]).ToString("yyyy-MM-dd");
                    range = ws.get_Range("B" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["履历情况"].ToString();
                    range = ws.get_Range("F" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["检定结果"].ToString();
                    range = ws.get_Range("H" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["检定单位"].ToString();
                    range = ws.get_Range("J" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["使用人或地点"].ToString();
                    range = ws.get_Range("N" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["点检备注"].ToString();

                    if (pos % i_count == 0 && count  != i_第几张)
                    {
                        i_第几张++;
                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 10;
                    }
                    i_first++;
                    pos++;
                }                       
                        //BringWindowToTop(hwnd);
                        wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        excelApp.Visible = false;   
                         excelApp.Quit();
                    
               
            } catch (Exception ex)
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
                    //System.IO.File.Delete(fileName);
                }
            }
        }



        public static void fun_print_基础信息(System.Data.DataTable dtt, System.Data.DataRow[] drr_打印, string str_打印机, bool blPreview = false, string str = "")
        {
            int count = drr_打印.Length / 21;
            if (drr_打印.Length % 21 != 0)
            {
                count = count + 1;
            }
            //string fileName = @"C:\Users\Administrator\Desktop\基础信息清单.xlsx";
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\基础信息清单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '基础信息清单'";
                new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                if (dtPP.Rows.Count == 0) return;

                System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
            }

            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {

                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing);

                excelApp.Visible = false;
                excelApp.ScreenUpdating = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;


                for (int j = 1; j < count; j++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);

                }


                int pos = 1;
                int i = 1;
                int i_first = 6;
                int i_第几张 = 1;
                foreach (System.Data.DataRow r in drr_打印)
                {
                   
                  
                    //range = ws.get_Range("A" + (i_first).ToString(), Type.Missing);
                    //range.Value2 = (i).ToString();
                    //range = ws.get_Range("B" + (i_first).ToString(), Type.Missing);
                    //range.Value2 = r["采购单号"].ToString();

                    range = ws.get_Range("A" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["计量器具编号"].ToString();
                    range = ws.get_Range("B" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["计量器具名称"].ToString();
                    range = ws.get_Range("C" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["计量器具规格"].ToString();
                    range = ws.get_Range("D" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["所属大类"].ToString();
                    range = ws.get_Range("E" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["出厂编号"].ToString();
                    range = ws.get_Range("F" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["证书号"].ToString();
                    range = ws.get_Range("G" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["状态"].ToString();
                    //range = ws.get_Range("H" + (i_first).ToString(), Type.Missing);
                    //range.Value2 = r["精度"].ToString();
                    range = ws.get_Range("H" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["管理级别"].ToString();
                    range = ws.get_Range("I" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["所属部门"].ToString();
                    //range = ws.get_Range("K" + (i_first).ToString(), Type.Missing);
                    //range.Value2 = r["制造单位"].ToString();
                    //range = ws.get_Range("L" + (i_first).ToString(), Type.Missing);
                    //range.Value2 = r["使用人"].ToString();
                    //range = ws.get_Range("M" + (i_first).ToString(), Type.Missing);
                    //range.Value2 = r["准用证号"].ToString();
                    range = ws.get_Range("J" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["检定标准"].ToString();
                    range = ws.get_Range("K" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["检定周期"].ToString();
                    range = ws.get_Range("L" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["检定单位"].ToString();
                    range = ws.get_Range("M" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["检定结果"].ToString();
                    range = ws.get_Range("N" + (i_first).ToString(), Type.Missing);
                    if (r["有效期"]!= DBNull.Value)
                    {
                        range.Value2 = Convert.ToDateTime(r["有效期"]).Date.ToString("yyyy-MM-dd");

                    }
                    
                    if (pos % 21 == 0 && count != i_第几张)
                    {
                        i_第几张++;

                        ws = (Worksheet)wb.Worksheets.get_Item(pos / 21 + 1);
                        ws.Name = "sheet-" + (pos / 21 + 1).ToString();
                        i_first = 5;



                    }

                    i_first++;
                    pos++;
                    i++;
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
                    wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
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
                    //System.IO.File.Delete(fileName);
                }
            }
        }


        public static void fun_申请(System.Data.DataTable dt, System.Data.DataRow dr, string str_打印机, bool blPreview = false)
        {
            int count = dt.Rows.Count / 8;
            if (dt.Rows.Count / 8 != 0)
            {
                count = count + 1;

            }

            string fileName = @"C:\Users\Administrator\Desktop\计量器具申请单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                fs.Close();
                System.Data.DataTable dtpp = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '计量器具申请单'";
                new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtpp);
                if (dtpp.Rows.Count == 0)
                    return;
                System.IO.File.WriteAllBytes(fileName, (byte[])dtpp.Rows[0]["数据"]);
            }
            ApplicationClass excelApp = new ApplicationClass();
            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {

                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                excelApp.Visible = false;
                excelApp.ScreenUpdating = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;


                //string sql3 = string.Format(" select * from 计量器具申请主表 where 申请单号='{0}'", dr["申请单号"].ToString());
                //System.Data.DataTable dtt = new System.Data.DataTable();
                //new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dtt);

                range = ws.get_Range("B5", Type.Missing);
                range.Value2 = dr["申请类别"].ToString();
                range = ws.get_Range("E5", Type.Missing);
                range.Value2 = dr["申请原因"].ToString();
                range = ws.get_Range("B17", Type.Missing);
                range.Value2 = Convert.ToDateTime(dr["申请时间"]).ToString("yyyy-MM-dd");
                //range.Value2 = System.DateTime.Today.ToString("yyyy-MM-dd");
                range = ws.get_Range("B15", Type.Missing);
                range.Value2 = dr["申请部门"].ToString();
                range = ws.get_Range("G17", Type.Missing);
                range.Value2 = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");               
                //range = ws.get_Range("G15", Type.Missing);
                //range.Value2 = "";


                for (int j = 1; j < count; j++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);

                }


                int pos = 1;
                int i_第几张 = 1;
                int i_first = 7;//第几行开始
                //int i_第几张 = 1;
                int i_count = 8; //每页多少条记录

                //string ssql = string.Format("select * from 计量器具申请明细表 where 申请单号 = '{0}'", dr["申请单号"]);
                string sssql = string.Format("select * from 计量器具申请明细表 where 申请单号 = '{0}'", dr ["申请单号"].ToString());
                System.Data.DataTable ddt = new System.Data.DataTable();
                SqlDataAdapter dda = new SqlDataAdapter(sssql, CPublic.Var.strConn);
                dda.Fill(ddt);

                foreach (System.Data.DataRow r in ddt.Rows)
                {

                    range = ws.get_Range("B" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["计量器具名称"].ToString();
                    range = ws.get_Range("F" + (i_first).ToString(), Type.Missing);
                    if (r["数量"] != DBNull.Value)
                    {
                        range.Value2 = Convert.ToInt32(r["数量"].ToString());
                    }
                    range = ws.get_Range("D" + (i_first).ToString(), Type.Missing);

                    range.Value2 = r["计量器具规格"].ToString();
                    range = ws.get_Range("G" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["备注"].ToString();

                    if (pos % i_count == 0 && count != i_第几张)
                    {
                        i_第几张++;
                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 6;
                    }
                    i_first++;
                    pos++;
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
                    wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
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
                    //System.IO.File.Delete(fileName);
                }
            }
        }



    }
}

