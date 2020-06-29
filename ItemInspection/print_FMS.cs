using Microsoft.Office.Interop.Excel;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Printing;
using System.Drawing;

using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using System.Data;

namespace ItemInspection
{
    public class print_FMS
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

        public static void fun_生成校验码(System.Data.DataTable dtP, string fileName)
        {
            //string fileName = @"C:\检验码.xlsx";

            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                Workbook wb;
                if (File.Exists(fileName))
                {
                    wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                Type.Missing, Type.Missing);

                }
                else
                {
                    wb = excelApp.Workbooks.Add(true);
                }
                excelApp.Visible = false;
                excelApp.ScreenUpdating = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];
                ws.Cells.Clear();
                Microsoft.Office.Interop.Excel.Range range;
                int i_first = 1;
                ws.Cells.NumberFormatLocal = "@";
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    range = ws.get_Range("A" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["码"].ToString();
                    if (dtP.Columns.Contains("箱号"))
                    {
                        range = ws.get_Range("B" + (i_first++).ToString(), Type.Missing);
                        range.Value2 = r["箱号"].ToString();
                    }
                    else
                    {
                        i_first++;
                    }
                }
                string str = fileName;
                wb.SaveAs(str);
                //  excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();

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

        public static void fun_print_ECN变更申请(string s_申请单号, System.Data.DataTable dtP, System.Data.DataRow dr, bool blPreview,  string str = "")
        {
            int count = dtP.Rows.Count / 21;
            if (dtP.Rows.Count % 21 != 0)
            {
                count = count + 1;
            }
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\ECN变更申请通知单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = 'ECN变更申请通知单'";
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
                range = ws.get_Range("C3", Type.Missing);
                range.Value2 = s_申请单号;
                range = ws.get_Range("L3", Type.Missing);
                range.Value2 = dr["申请日期"];


                //for (int j = 1; j < count; j++)
                //{
                //    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);

                //}

                //备注
                //range = ws.get_Range("Z" + (i_first + pos).ToString(), Type.Missing);
                //range.Value2 = r["备注"].ToString();

                int pos = 1;
                int i = 1;
                int i_first = 6;
                int i_第几张 = 1;
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    //if ((i_first) >= 29)
                    //{
                    //    break;
                    //}
                    range = ws.get_Range("A" + (i_first).ToString(), Type.Missing);
                    range.Value2 = (i).ToString();
                    //range = ws.get_Range("B" + (i_first).ToString(), Type.Missing);
                    //range.Value2 = r["采购单号"].ToString();
                    range = ws.get_Range("B" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["审核意见"].ToString();

                    range = ws.get_Range("G" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["部门负责人"].ToString();
                    range = ws.get_Range("J" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["审核日期"].ToString();
                   
                    if (pos % 21 == 0 && count != i_第几张)
                    {
                        i_第几张++;

                        ws = (Worksheet)wb.Worksheets.get_Item(pos / 21 + 1);
                        ws.Name = "sheet-" + (pos / 21 + 1).ToString();
                        i_first = 6;



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
                    if (str == "")
                    {
                        wb.PrintOutEx();
                    }
                    else
                    {
                        wb.SaveAs(str);
                    }
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

        public static string fun_生成检验标准(System.Data.DataTable dtP, System.Data.DataRow dr_cs)
        {

            string s_文件编号 = "";
            string fileName = System.Windows.Forms.Application.StartupPath + string.Format(@"\prttmp\{0}.xlsx", dr_cs["小类"].ToString().Trim());
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = string.Format("select * from 基础记录打印模板表 where 模板名 = '{0}'", dr_cs["小类"].ToString());
                new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                if (dtPP.Rows.Count == 0) throw new Exception("没有该小类的模板");
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
                range = ws.get_Range("D4", Type.Missing);
                range.Value2 = CPublic.Var.localUserName;

                //                string sql =string.Format(@"select  top 1 a.产品编号, a.检验日期,产品线,属性字段1 as 产线经理 from 采购记录采购单检验主表 a
                //                left join (select   max(产品编码)父项编码,子项编码 from  基础数据物料BOM表 group by 子项编码 )ax  on ax.子项编码=a.产品编号
                //                left join 基础数据物料信息表  b on ax.父项编码=b.物料编码  
                //                left join  ( select  属性值,属性字段1 from 基础数据基础属性表  where 属性类别='生产线')x on x.属性值=产品线
                //                where   a.产品编号='{0}' and  检验日期>'2017-1-1' order by 检验记录单号  ",dr_cs["物料编码"].ToString());
                //                System.Data.DataTable tem = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);

                //如果没有 用当前时间
                //string time=Convert.ToDateTime(tem.Rows[0]["检验日期"]).ToString("yyyy-MM-dd");
                string time = CPublic.Var.getDatetime().Date.ToString("yyyy-MM-dd");
                range = ws.get_Range("A18", Type.Missing);
                range.Value2 = time;
                range = ws.get_Range("E18", Type.Missing);
                range.Value2 = "A(2)";
                range = ws.get_Range("F18", Type.Missing);
                range.Value2 = "版本升级";
                range = ws.get_Range("M18", Type.Missing);
                range.Value2 = "冯燕" + time;
                range = ws.get_Range("P18", Type.Missing);
                range.Value2 = "谈春萍" + time;
                string s_产线经理 = "";
                if (s_产线经理 == "")
                {
                    //随便找个父项物料的产品线 
                    System.Data.DataTable t = new System.Data.DataTable();
                    t.Columns.Add("父项编号");
                    t.Columns.Add("产品线");
                    t = ERPorg.Corg.fun_运算_成品(t, dr_cs["原ERP物料编号"].ToString(), "");
                    string s = string.Format("select  属性值,属性字段1 from 基础数据基础属性表  where 属性类别='生产线' and 属性值='{0}'"
                        , t.Rows[t.Rows.Count - 1]["产品线"]);
                    using (SqlDataAdapter da = new SqlDataAdapter(s, CPublic.Var.strConn))
                    {
                        t = new System.Data.DataTable();
                        da.Fill(t);
                        if (t.Rows.Count != 0)
                        {
                            s_产线经理 = t.Rows[0]["属性字段1"].ToString();
                        }
                    }
                }
                range = ws.get_Range("S18", Type.Missing);
                range.Value2 = s_产线经理 + time;
                range = ws.get_Range("V18", Type.Missing);
                range.Value2 = "丁冬明" + time;
                //int pos = 1;
                //每个模板 固定的 前几行有可能不一样
                int i = 3; //序号
                int i_first = 8;  //模板的第8行开始 
                for (; i_first < 12; i_first++)  //判断模板第几行空白  从这里开始写
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    if (range.Value2 != null && range.Value2.ToString() != "") //如果不为空 continue 
                    {
                        i = Convert.ToInt32(range.Value2.ToString()) + 1;
                        continue;
                    }

                    break; // 否则记下i_first 即尺寸从这里开始 往下填充
                }

                if (dtP.Rows.Count > 0)
                {
                    range = ws.get_Range("M2", Type.Missing);
                    range.Value2 = dtP.Rows[0]["父项规格"].ToString();
                    range = ws.get_Range("M3", Type.Missing);
                    range.Value2 = dtP.Rows[0]["父项名称"].ToString();
                    foreach (System.Data.DataRow r in dtP.Rows)
                    {
                        range = ws.get_Range("A" + (i_first).ToString(), Type.Missing);
                        range.Value2 = i++.ToString();
                        range = ws.get_Range("C" + (i_first).ToString(), Type.Missing);
                        range.Value2 = r["检验项目"].ToString();
                        range = ws.get_Range("F" + (i_first).ToString(), Type.Missing);
                        range.Value2 = r["检验要求"].ToString();
                        range = ws.get_Range("L" + (i_first).ToString(), Type.Missing);
                        range.Value2 = r["扩大值"].ToString();
                        range = ws.get_Range("N" + (i_first).ToString(), Type.Missing);
                        range.Value2 = r["检验水平"].ToString();
                        range = ws.get_Range("P" + (i_first).ToString(), Type.Missing);
                        range.Value2 = r["AQL"].ToString();
                        range = ws.get_Range("Q" + (i_first).ToString(), Type.Missing);
                        range.Value2 = "游标卡尺、图像测试仪";
                        i_first++;
                    }
                }
                range = ws.get_Range("Q2", Type.Missing);
                range.Value2 = dr_cs["图纸编号"].ToString();
                range = ws.get_Range("Q3", Type.Missing);
                range.Value2 = dr_cs["物料名称"].ToString();
                string s_no = CPublic.CNo.fun_得到最大流水号(dr_cs["小类"].ToString()).ToString("0000");
                range = ws.get_Range("U1", Type.Missing);
                s_文件编号 = range.Value2.ToString() + s_no.ToString();
                range = ws.get_Range("X1", Type.Missing);
                range.Value2 = s_no;
                excelApp.Visible = false;
                string str = System.Windows.Forms.Application.StartupPath + "\\品质检验标准\\" + dr_cs["原ERP物料编号"].ToString() + "_" + dr_cs["小类"].ToString();
                wb.SaveAs(str);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
                // excelApp.Quit();


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
            return s_文件编号;
        }

        public static void fun_print_销售发票明细(System.Data.DataTable dtP, System.Data.DataRow dr_cs, bool blPreview = false, decimal dec_含税金额总 = 0, string str = "")
        {

            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\销售发票明细.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '销售发票明细'";
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

                range = ws.get_Range("C4", Type.Missing);
                range.Value2 = dr_cs["开票票号"].ToString();
                //range = ws.get_Range("O5", Type.Missing);
                //range.Value2 = System.DateTime.Today.ToString("yyyy-MM-dd");
                //供应商名称
                range = ws.get_Range("C5", Type.Missing);
                range.Value2 = dr_cs["客户名称"].ToString();
                range = ws.get_Range("M4", Type.Missing);
                range.Value2 = Convert.ToDateTime(dr_cs["开票日期"]).ToString("yyyy-MM-dd");
                range = ws.get_Range("M5", Type.Missing);
                range.Value2 = dec_含税金额总;



                int pos = 1;
                int i = 1;
                int i_first = 7;

                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    string s = string.Format(@" select  销售记录销售订单主表.销售订单号,客户订单号 from 销售记录销售订单明细表
      left join  销售记录销售订单主表  on 销售记录销售订单主表.销售订单号=销售记录销售订单明细表.销售订单号
         where 销售订单明细号='{0}'", r["销售订单明细号"].ToString());
                    /*
                     * 
                     * 
                     *  union 
         select  c.销售订单号,客户订单号 from L销售记录销售订单明细表L a
      left join  L销售记录销售订单主表L b  on b.销售订单号=a.销售订单号
      left join L销售记录成品出库单明细表L c on c.销售订单明细号=a.销售订单明细号
        where c.销售订单明细号='{0}'"
                     * */
                    System.Data.DataTable dt = new System.Data.DataTable();
                    dt = CZMaster.MasterSQL.Get_DataTable(s, CPublic.Var.strConn);
                    if (dt.Rows.Count > 0)
                    {
                        range = ws.get_Range("C" + (i_first).ToString(), Type.Missing);
                        range.Value2 = dt.Rows[0]["客户订单号"].ToString();
                    }
                    range = ws.get_Range("A" + (i_first).ToString(), Type.Missing);
                    string[] e = r["销售订单明细号"].ToString().Split('-');
                    range.Value2 = e[0];

                    //range = ws.get_Range("B" + (i_first).ToString(), Type.Missing);
                    //range.Value2 = r["采购单号"].ToString();

                    range = ws.get_Range("F" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["出库通知单号"].ToString();
                    range = ws.get_Range("H" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["成品出库单号"].ToString();

                    range = ws.get_Range("I" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["产品名称"].ToString() + " " + r["规格型号"].ToString(); ;

                    range = ws.get_Range("J" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["计量单位"].ToString();
                    range = ws.get_Range("K" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["开票数量"].ToString();
                    range = ws.get_Range("L" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["开票税后单价"].ToString();
                    range = ws.get_Range("M" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["开票税后金额"].ToString();

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

                    if (str == "")
                    {
                        wb.PrintOutEx();
                    }
                    else
                    {
                        wb.SaveAs(str);
                    }
                    //excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                    System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                    excelApp.Quit();

                    System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                    System.GC.Collect();
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
        public static void fun_print_采购开票清单核销(System.Data.DataTable dtP, string str_供应商, bool blPreview = false, decimal dec_不含税金额总 = 0, decimal dec_含税金额总 = 0, string str = "")
        {
            int count = dtP.Rows.Count / 21;
            if (dtP.Rows.Count % 21 != 0)
            {
                count = count + 1;
            }
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\采购开票清单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '采购开票清单'";
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

                range = ws.get_Range("D4", Type.Missing);
                range.Value2 = CPublic.Var.localUserName;
                //range = ws.get_Range("O5", Type.Missing);
                //range.Value2 = System.DateTime.Today.ToString("yyyy-MM-dd");
                //供应商名称
                range = ws.get_Range("D5", Type.Missing);
                range.Value2 = str_供应商;
                for (int j = 1; j < count; j++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);

                }


                int pos = 1;
                int i = 1;
                int i_first = 7;
                int i_第几张 = 1;
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    //if ((i_first) >= 30)
                    //{
                    //    break;
                    //}
                    range = ws.get_Range("A" + (i_first).ToString(), Type.Missing);
                    range.Value2 = (i).ToString();
                    //range = ws.get_Range("B" + (i_first).ToString(), Type.Missing);
                    //range.Value2 = r["采购单号"].ToString();

                    range = ws.get_Range("B" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["送检单号"].ToString();
                    range = ws.get_Range("E" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["入库单号"].ToString();
                    range = ws.get_Range("G" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["物料编码"].ToString();
                    range = ws.get_Range("H" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["物料名称"].ToString();
                    range = ws.get_Range("I" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["图纸编号"].ToString();
                    range = ws.get_Range("J" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["开票数量"].ToString();
                    range = ws.get_Range("K" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["折扣后含税单价"].ToString();
                    range = ws.get_Range("L" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["折扣后含税金额"].ToString();
                    range = ws.get_Range("M" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["折扣后不含税单价"].ToString();
                    range = ws.get_Range("N" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["折扣后不含税金额"].ToString();

                    if (pos % 21 == 0 && count != i_第几张)
                    {
                        i_第几张++;

                        ws = (Worksheet)wb.Worksheets.get_Item(pos / 21 + 1);
                        ws.Name = "sheet-" + (pos / 21 + 1).ToString();
                        i_first = 6;



                    }

                    i_first++;
                    pos++;
                    i++;
                }
                range = ws.get_Range("L28", Type.Missing);
                range.Value2 = dec_含税金额总.ToString();

                range = ws.get_Range("N28", Type.Missing);
                range.Value2 = dec_不含税金额总.ToString();
                range = ws.get_Range("K28", Type.Missing);
                range.Value2 = "总额";
                if (blPreview)
                {
                    excelApp.Visible = true;
                    wb.PrintPreview();
                }
                else
                {
                    excelApp.Visible = false;
                    BringWindowToTop(hwnd);

                    if (str == "")
                    {
                        wb.PrintOutEx();
                    }
                    else
                    {
                        wb.SaveAs(str);
                    }
                    //excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                    System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                    excelApp.Quit();

                    System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                    System.GC.Collect();
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


        public static void fun_print_采购开票清单(System.Data.DataTable dtP, string str_供应商, bool blPreview = false, decimal dec_不含税金额总 = 0, decimal dec_含税金额总 = 0, string str = "")
        {

            int count = dtP.Rows.Count / 21;
            if (dtP.Rows.Count % 21 != 0)
            {
                count = count + 1;
            }
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\采购开票清单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '采购开票清单'";
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
                range = ws.get_Range("D4", Type.Missing);
                range.Value2 = CPublic.Var.localUserName;
                //range = ws.get_Range("O5", Type.Missing);
                //range.Value2 = System.DateTime.Today.ToString("yyyy-MM-dd");
                //供应商名称
                range = ws.get_Range("D5", Type.Missing);
                range.Value2 = str_供应商;
                for (int j = 1; j < count; j++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);

                }

                //备注
                //range = ws.get_Range("Z" + (i_first + pos).ToString(), Type.Missing);
                //range.Value2 = r["备注"].ToString();

                int pos = 1;
                int i = 1;
                int i_first = 7;
                int i_第几张 = 1;
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    //if ((i_first) >= 29)
                    //{
                    //    break;
                    //}
                    range = ws.get_Range("A" + (i_first).ToString(), Type.Missing);
                    range.Value2 = (i).ToString();
                    //range = ws.get_Range("B" + (i_first).ToString(), Type.Missing);
                    //range.Value2 = r["采购单号"].ToString();
                    range = ws.get_Range("B" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["送检单号"].ToString();

                    range = ws.get_Range("E" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["入库单号"].ToString();
                    range = ws.get_Range("G" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["物料编码"].ToString();
                    range = ws.get_Range("H" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["物料名称"].ToString();
                    range = ws.get_Range("I" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["规格型号"].ToString();
                    range = ws.get_Range("J" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["开票数量"].ToString();
                    range = ws.get_Range("K" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["单价"].ToString();
                    range = ws.get_Range("L" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["金额"].ToString();
                    range = ws.get_Range("M" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["未税单价"].ToString();
                    range = ws.get_Range("N" + (i_first).ToString(), Type.Missing);
                    range.Value2 = r["未税金额"].ToString();

                    if (pos % 21 == 0 && count != i_第几张)
                    {
                        i_第几张++;

                        ws = (Worksheet)wb.Worksheets.get_Item(pos / 21 + 1);
                        ws.Name = "sheet-" + (pos / 21 + 1).ToString();
                        i_first = 6;



                    }

                    i_first++;
                    pos++;
                    i++;
                }
                range = ws.get_Range("L28", Type.Missing);
                range.Value2 = dec_含税金额总.ToString();
                //range = ws.get_Range("L40", Type.Missing);
                //range.Value2 = dec_不含税金额总.ToString();
                range = ws.get_Range("N28", Type.Missing);
                range.Value2 = dec_不含税金额总.ToString();
                range = ws.get_Range("K28", Type.Missing);
                range.Value2 = "总额";
                if (blPreview)
                {
                    excelApp.Visible = true;
                    wb.PrintPreview();
                }
                else
                {
                    excelApp.Visible = false;
                    BringWindowToTop(hwnd);
                    if (str == "")
                    {
                        wb.PrintOutEx();
                    }
                    else
                    {
                        wb.SaveAs(str);
                    }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str_采购单"></param>
        /// <param name="str_打印机"></param>
        public static void fun_采购单(string str_采购单, string str_打印机)
        {
            string sql = string.Format(@"select 采购记录采购单主表.*,供应商传真 from 采购记录采购单主表,采购供应商表 
                        where  采购记录采购单主表.供应商ID=采购供应商表.供应商ID and 采购单号='{0}'", str_采购单);
            System.Data.DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
            sql = string.Format(@"select 采购记录采购单明细表.*,原ERP物料编号 from 采购记录采购单明细表,基础数据物料信息表 
            where 采购记录采购单明细表.物料编码=基础数据物料信息表.物料编码  and 采购单号='{0}' order by 原ERP物料编号", str_采购单);
            System.Data.DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);

            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\采购单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '采购单'";
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

                range = ws.get_Range("H4", Type.Missing);
                range.Value2 = dr["采购单号"];
                range = ws.get_Range("C6", Type.Missing);
                range.Value2 = dr["供应商"];
                range = ws.get_Range("H6", Type.Missing);
                range.Value2 = dr["供应商电话"];
                range = ws.get_Range("H7", Type.Missing);
                range.Value2 = dr["供应商传真"];
                range = ws.get_Range("C7", Type.Missing);
                range.Value2 = Convert.ToDateTime(dr["采购计划日期"]).ToString("yyyy-MM-dd");
                range = ws.get_Range("C8", Type.Missing);
                range.Value2 = dr["税率"] + "%";
                range = ws.get_Range("H26", Type.Missing);
                range.Value2 = dr["总金额"];

                int count = dt.Rows.Count / 14;
                if (count % 14 != 0)
                {
                    count = count + 1;
                }
                // */
                //复制 count-1 个 sheet
                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                }
                // int ir = 1;
                int pos = 1;  //记数 循环次数
                int i_first = 12;      // 起始行 
                int i_count = 14; // 每页打多少条
                int i_第几张 = 1;

                foreach (System.Data.DataRow r in dt.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    //range.Value2 = ir++.ToString();  //序号

                    range.Value2 = r["物料编码"].ToString();

                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["物料名称"].ToString();


                    range = ws.get_Range("D" + i_first.ToString(), Type.Missing);

                    range.Value2 = r["规格型号"].ToString();

                    range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["计量单位"].ToString();

                    range = ws.get_Range("F" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["采购数量"].ToString();

                    range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["金额"].ToString();
                    range = ws.get_Range("G" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["单价"].ToString();
                    range = ws.get_Range("J" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["未税单价"].ToString();
                    range = ws.get_Range("I" + i_first.ToString(), Type.Missing);
                    range.Value2 = Convert.ToDateTime(r["到货日期"]).ToString("yyyy-MM-dd");

                    //超过十七条 换下一个sheet
                    if (pos % i_count == 0 && count != i_第几张)
                    {
                        i_第几张++;
                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 11;
                    }
                    i_first++;
                    pos++;
                }



                //if (str_path == "")
                //{
                //range = ws.get_Range("B47", Type.Missing);
                //range.Value2 = Convert.ToDateTime(dr["修改日期"]).ToString("yyyy-MM-dd");
                //range = ws.get_Range("F47", Type.Missing);
                //range.Value2 = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //}
                //else
                //{
                //    range = ws.get_Range("B47", Type.Missing);
                //    range.Value2 = Convert.ToDateTime(dr["修改日期"]).ToString("yyyy-MM-dd");
                //    range = ws.get_Range("F47", Type.Missing);
                //    range.Value2 =CPublic.Var.getDatetime().ToString("yyyy-MM-dd");

                //    wb.SaveAs(str_path);
                //}

                BringWindowToTop(hwnd);



                excelApp.DisplayAlerts = false;
                excelApp.Visible = false;

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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
        public static void fun_成品入库(System.Data.DataTable t, string str_打印机)
        {


            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\成品入库单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '成品入库单'";
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

                range = ws.get_Range("E5", Type.Missing);
                range.Value2 = t.Rows[0]["成品入库单号"].ToString();
                range = ws.get_Range("N5", Type.Missing);
                range.Value2 = Convert.ToDateTime(t.Rows[0]["生效日期"]).ToString("yyyy-MM-dd"); ;
                range = ws.get_Range("E20", Type.Missing);
                range.Value2 = t.Rows[0]["入库人员"].ToString();


                int count = t.Rows.Count / 13;
                if (count % 13 != 0)
                {
                    count = count + 1;
                }
                // */
                //复制 count-1 个 sheet
                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                }
                int ir = 1;
                int pos = 1;  //记数 循环次数
                int i_first = 7;      // 起始行 
                int i_count = 13; // 每页打多少条
                int i_第几张 = 1;

                foreach (System.Data.DataRow r in t.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    range.Value2 = ir++.ToString();  //序号

                    //range.Value2 = r["原ERP物料编号"].ToString();

                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["生产工单号"].ToString();


                    range = ws.get_Range("E" + i_first.ToString(), Type.Missing);

                    range.Value2 = r["生产检验单号"].ToString();

                    range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["物料编码"].ToString();

                    range = ws.get_Range("I" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["物料名称"].ToString();

                    range = ws.get_Range("L" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["入库数量"].ToString();
                    range = ws.get_Range("J" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["送检数量"].ToString();

                    range = ws.get_Range("N" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["车间"].ToString();

                    //超过icount条 换下一个sheet
                    if (pos % i_count == 0 && count != i_第几张)
                    {
                        i_第几张++;

                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 6;   //初始值 减一

                    }
                    i_first++;
                    pos++;
                }



                //excelApp.Visible = false;
                //BringWindowToTop(hwnd);
                wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                excelApp.DisplayAlerts = false;
                // excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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
        //没用到
        public static void fun_print_领料单(System.Data.DataTable dtP, int i, bool blPreview = false)
        {
            string fName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp";
            if (Directory.Exists(fName) == false)
            {
                Directory.CreateDirectory(fName);
            }
            string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + "-" + i.ToString() + ".xlsx";
            //string fileName = "D:\\123" + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";  //zf
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
                string sql = string.Format(@"select 生产记录生产领料单主表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号 from 生产记录生产领料单主表 
                    left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 生产记录生产领料单主表.物料编码 
                    where 生产记录生产领料单主表.领料出库单号 = '{0}'", dtP.Rows[0]["领料出库单号"].ToString());
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception(string.Format("没有找到领料出库单号：{0}", dtP.Rows[0]["领料出库单号"].ToString()));
                }
                range = ws.get_Range("P10", Type.Missing);
                range.Value2 = dt.Rows[0]["生产车间"].ToString();
                range = ws.get_Range("B10", Type.Missing);
                range.Value2 = dt.Rows[0]["领料出库单号"].ToString();
                range = ws.get_Range("P13", Type.Missing);
                range.Value2 = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                range = ws.get_Range("B116", Type.Missing);
                range.Value2 = dt.Rows[0]["领料人员ID"].ToString() + " " + dt.Rows[0]["领料人员"].ToString();
                range = ws.get_Range("B13", Type.Missing);
                range.Value2 = dt.Rows[0]["物料编码"].ToString();
                range = ws.get_Range("B16", Type.Missing);
                range.Value2 = dt.Rows[0]["物料名称"].ToString();
                range = ws.get_Range("P16", Type.Missing);
                range.Value2 = dt.Rows[0]["物料名称"].ToString();

                int pos = 0;
                int i_first = 22;
                int ir = 1;
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    if ((pos + i_first) >= 98)
                    {
                        break;
                    }
                    range = ws.get_Range("A" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = ir.ToString();
                    range = ws.get_Range("J" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["领料数量"].ToString();
                    try
                    {
                        string sqld = string.Format("select 物料名称,原ERP物料编号,计量单位,仓库名称,货架描述,n原ERP规格型号 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                        System.Data.DataTable dtt = new System.Data.DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(sqld, CPublic.Var.strConn);
                        da.Fill(dtt);
                        range = ws.get_Range("D" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = dtt.Rows[0]["物料名称"].ToString() + dtt.Rows[0]["n原ERP规格型号"].ToString();
                        range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = dtt.Rows[0]["原ERP物料编号"].ToString();
                        range = ws.get_Range("I" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = dtt.Rows[0]["计量单位"].ToString();
                        range = ws.get_Range("L" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = dtt.Rows[0]["仓库名称"].ToString();
                        range = ws.get_Range("O" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = dtt.Rows[0]["货架描述"].ToString();
                    }
                    catch { }

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
                    ir++;
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
                    //System.IO.File.Delete(fileName);
                }
            }
        }
        public static void fun_p_送检清单(System.Data.DataTable dt, string str_打印机)
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\送检清单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '送检清单'";
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


                range = ws.get_Range("C4", Type.Missing);
                range.Value2 = CPublic.Var.localUserName;

                range = ws.get_Range("I4", Type.Missing);
                range.Value2 = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                int count = dt.Rows.Count / 13;
                if (count % 13 != 0)
                {
                    count = count + 1;
                }
                // */
                //复制 count-1 个 sheet
                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                }
                int ir = 1;
                int pos = 1;  //记数 循环次数
                int i_first = 6;      // 起始行 
                int i_count = 13; // 每页打多少条
                int i_第几张 = 1;

                foreach (System.Data.DataRow r in dt.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    range.Value2 = ir++.ToString();
                    range = ws.get_Range("B" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["送检单号"].ToString();


                    //range = ws.get_Range("D" + i_first.ToString(), Type.Missing);

                    //range.Value2 = r["采购单号"].ToString();
                    range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["供应商"].ToString();

                    range = ws.get_Range("F" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["物料编码"].ToString();
                    range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["规格型号"].ToString();  //原为图纸编号
                    range = ws.get_Range("G" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["物料名称"].ToString();

                    range = ws.get_Range("I" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["送检数量"].ToString();

                    //超过十七条 换下一个sheet
                    if (pos % i_count == 0 && count != i_第几张)
                    {
                        i_第几张++;

                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 5;

                    }
                    i_first++;
                    pos++;
                }



                //excelApp.Visible = false;
                //BringWindowToTop(hwnd);
                wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                excelApp.DisplayAlerts = false;


                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();

            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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
        public static void fun_p_供应商单价(string str_供应商, string str_打印机, string s_税, System.Data.DataTable dt)
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\供应商单价.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '供应商单价'";
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


                range = ws.get_Range("C3", Type.Missing);
                range.Value2 = str_供应商;
                range = ws.get_Range("E2", Type.Missing);
                range.Value2 = "(含税" + s_税.ToString() + "%)";
                string yy = (CPublic.Var.getDatetime().Year % 100).ToString();
                range = ws.get_Range("F4", Type.Missing);
                range.Value2 = yy + "年价格(元)";
                range = ws.get_Range("A2", Type.Missing);
                range.Value2 = yy + "年供应商价格调整表";

                int count = dt.Rows.Count / 31;
                if (dt.Rows.Count % 31 != 0)
                {
                    count = count + 1;
                }
                // */
                //复制 count-1 个 sheet
                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                }
                int ir = 1;
                int pos = 1;  //记数 循环次数
                int i_first = 5;      // 起始行 
                int i_count = 31; // 每页打多少条
                int i_第几张 = 1;

                foreach (System.Data.DataRow r in dt.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    range.Value2 = ir++.ToString();
                    range = ws.get_Range("B" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["物料编码"].ToString();


                    //range = ws.get_Range("D" + i_first.ToString(), Type.Missing);

                    //range.Value2 = r["采购单号"].ToString();
                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["物料名称"].ToString();

                    range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["计量单位"].ToString();
                    range = ws.get_Range("D" + i_first.ToString(), Type.Missing);
                    //range.Value2 = r["规格型号"].ToString();
                    range.Value2 = r["图纸编号"].ToString();

                    range = ws.get_Range("F" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["不含税单价"].ToString();

                    //超过十七条 换下一个sheet
                    if (pos % i_count == 0 && count != i_第几张)
                    {
                        i_第几张++;

                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 4;

                    }
                    i_first++;
                    pos++;
                }



                excelApp.Visible = false;
                //BringWindowToTop(hwnd);
                wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                excelApp.DisplayAlerts = false;



                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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


            //}
            //catch (Exception ex)
            //{
            //    excelApp = null;
            //    GcCollect();
            //    KillProcess(PID);
            //    CZMaster.MasterLog.WriteLog(ex.Message);
            //    //throw ex;

            //}
            //finally
            //{

            //    if (PID != IntPtr.Zero)
            //    {
            //        excelApp = null;
            //        GcCollect();
            //        KillProcess(PID);
            //        //System.IO.File.Delete(fileName);
            //    }
            //}
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="str_检验单号"></param>
        /// <param name="dec_合格数量"></param>
        /// <param name="flag">  flag=1表示全检 否则抽检 </param>
        public static void fun_P_采购入库通知单(System.Data.DataTable dt)
        {

            string path = System.Windows.Forms.Application.StartupPath + @"\prttmp";
            if (!System.IO.File.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\采购入库通知单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '采购入库通知单'";
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

                range = ws.get_Range("G3", Type.Missing);
                DateTime time = CPublic.Var.getDatetime();
                time = new DateTime(time.Year, time.Month, time.Day);
                range.Value2 = time.ToString("yyyy-MM-dd");

                range = ws.get_Range("B4", Type.Missing);
                range.Value2 = dt.Rows[0]["供应商名称"].ToString();

                range = ws.get_Range("F4", Type.Missing);
                range.Value2 = dt.Rows[0]["采购明细号"].ToString();

                range = ws.get_Range(" B6", Type.Missing);
                range.Value2 = dt.Rows[0]["物料名称"];

                range = ws.get_Range(" D5", Type.Missing);
                range.Value2 = dt.Rows[0]["检验记录单号"];

                range = ws.get_Range("G5", Type.Missing);
                range.Value2 = dt.Rows[0]["送检单号"].ToString();

                string sql = string.Format("select 原ERP物料编号,n原ERP规格型号,计量单位,货架描述 from 基础数据物料信息表 where 物料编码='{0}'", dt.Rows[0]["产品编号"].ToString());
                System.Data.DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
                range = ws.get_Range("B5", Type.Missing);
                range.Value2 = dr["原ERP物料编号"].ToString();
                range = ws.get_Range("B7", Type.Missing);
                range.Value2 = dr["n原ERP规格型号"].ToString();
                range = ws.get_Range("E7", Type.Missing);
                range.Value2 = dr["计量单位"].ToString();
                range = ws.get_Range("D3", Type.Missing);
                range.Value2 = dr["货架描述"].ToString();
                range = ws.get_Range("F7", Type.Missing);
                range.Value2 = dt.Rows[0]["送检数量"].ToString();

                range = ws.get_Range("G7", Type.Missing);

                decimal dec = Convert.ToDecimal(dt.Rows[0]["送检数量"]) - Convert.ToDecimal(dt.Rows[0]["不合格数量"]);

                if (dt.Rows[0]["检验结果"].ToString() == "合格" || dt.Rows[0]["检验结果"].ToString() == "免检")
                {

                    //入库数量 若抽检 入库数=送检数 全检 入库数=送检数-不合格数 
                    if (dt.Rows[0]["数量标记"].Equals(true)) //全检
                    {


                        range = ws.get_Range("G7", Type.Missing);
                        range.Value2 = dec;  // 入库数量=送检数量-不合格数量
                    }
                    else
                    {
                        range = ws.get_Range("G7", Type.Missing);
                        range.Value2 = dt.Rows[0]["送检数量"].ToString();
                    }
                    range = ws.get_Range("F9", Type.Missing);
                    range.Value2 = dt.Rows[0]["检验结果"].ToString();
                }
                else
                {
                    range = ws.get_Range("G7", Type.Missing);
                    range.Value2 = 0;

                    //.....

                    string sql_1 = string.Format(@"select * from 检验上传表单记录表
                                   where 采购入库通知单号='{0}' and 表单类型='不合格品评审单'", dt.Rows[0]["送检单号"].ToString());
                    System.Data.DataTable dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, CPublic.Var.strConn);
                    if (dt_1.Rows.Count > 0)
                    {
                        range = ws.get_Range("F9", Type.Missing);
                        range.Value2 = "评审后合格";

                        if (dt.Rows[0]["数量标记"].Equals(true)) //全检
                        {
                            range = ws.get_Range("G7", Type.Missing);
                            range.Value2 = dec;  // 入库数量=送检数量-不合格数量
                        }
                        else
                        {
                            range = ws.get_Range("G7", Type.Missing);
                            range.Value2 = dt.Rows[0]["送检数量"].ToString();
                        }


                    }
                    else
                    {
                        range = ws.get_Range("F9", Type.Missing);
                        range.Value2 = "不合格";
                    }
                }
                range = ws.get_Range("B9", Type.Missing);
                range.Value2 = dt.Rows[0]["检验员"].ToString();

                range = ws.get_Range("D10", Type.Missing);
                range.Value2 = dt.Rows[0]["送检人"].ToString();
                //                string sql_1= @"select 检验记录单号 from 检验上传表单记录表,[采购记录采购单检验主表] 
                //                                   where 检验上传表单记录表.采购入库通知单号=[采购记录采购单检验主表].送检单号 and 表单类型='不合格品评审单'";


                //暂时打印机去 默认

                wb.PrintOutEx();

                excelApp.DisplayAlerts = false;

                excelApp.Quit();

            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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

        public static void fun_P_来料入库单(System.Data.DataTable dt)
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\来料入库单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '来料入库单'";
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

                range = ws.get_Range("G3", Type.Missing); //入库日期

                range.Value2 = Convert.ToDateTime(dt.Rows[0]["录入日期"]).ToString("yyyy-MM-dd");

                range = ws.get_Range("B4", Type.Missing);
                range.Value2 = dt.Rows[0]["供应商"].ToString();

                range = ws.get_Range("F4", Type.Missing);
                range.Value2 = dt.Rows[0]["入库单号"].ToString();

                range = ws.get_Range(" B6", Type.Missing);
                range.Value2 = dt.Rows[0]["物料名称"];


                string sql = string.Format("select 原ERP物料编号,n原ERP规格型号,计量单位,货架描述 from 基础数据物料信息表 where 物料编码='{0}'", dt.Rows[0]["物料编码"].ToString());
                System.Data.DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
                range = ws.get_Range("B5", Type.Missing);
                range.Value2 = dr["原ERP物料编号"].ToString();
                range = ws.get_Range("B7", Type.Missing);
                range.Value2 = dr["n原ERP规格型号"].ToString();
                range = ws.get_Range("G7", Type.Missing);
                range.Value2 = dr["计量单位"].ToString();
                range = ws.get_Range("D3", Type.Missing);
                range.Value2 = dr["货架描述"].ToString();
                range = ws.get_Range("H7", Type.Missing);
                range.Value2 = dt.Rows[0]["入库量"].ToString();


                sql = string.Format(@"select  采购单明细号,检验结果,检验员,送检人,入库人员 from [采购记录采购单入库明细],[采购记录采购单检验主表]
                where 采购记录采购单入库明细.检验记录单号=采购记录采购单检验主表.检验记录单号 and 入库单号='{0}'", dt.Rows[0]["入库单号"].ToString());
                System.Data.DataTable t = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);

                if (t.Rows[0]["检验结果"].ToString() == "合格")
                {

                    range = ws.get_Range("F9", Type.Missing);
                    range.Value2 = "合格";
                }
                else
                {

                    range = ws.get_Range("F9", Type.Missing);
                    range.Value2 = "评审后合格";

                }
                range = ws.get_Range("F5", Type.Missing);
                range.Value2 = t.Rows[0]["采购单明细号"].ToString();
                range = ws.get_Range("B9", Type.Missing);
                range.Value2 = t.Rows[0]["检验员"].ToString();
                range = ws.get_Range("D9", Type.Missing);
                range.Value2 = t.Rows[0]["入库人员"].ToString();
                range = ws.get_Range("D10", Type.Missing);
                range.Value2 = t.Rows[0]["送检人"].ToString();
                //                string sql_1= @"select 检验记录单号 from 检验上传表单记录表,[采购记录采购单检验主表] 
                //                                   where 检验上传表单记录表.采购入库通知单号=[采购记录采购单检验主表].送检单号 and 表单类型='不合格品评审单'";


                //暂时打印机去 默认

                wb.PrintOutEx();

                excelApp.DisplayAlerts = false;

                excelApp.Quit();

            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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

        public static void fun_p_领料A5(System.Data.DataRow dr_cs, System.Data.DataTable dtP, int count, string str_打印机, bool f_视图, bool blPreview = false)
        {
            //string fName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp";
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\领料单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '领料单'";
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

                range = ws.get_Range("J16", Type.Missing);
                range.Value2 = dr_cs["部门名称"].ToString();
                range = ws.get_Range("C12", Type.Missing);
                range.Value2 = dr_cs["生产工单号"].ToString();
                range = ws.get_Range("I35", Type.Missing);
                range.Value2 = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                range = ws.get_Range("C35", Type.Missing);
                range.Value2 = dr_cs["领料人ID"].ToString() + " " + dr_cs["领料人"].ToString();
                range = ws.get_Range("E35", Type.Missing);
                range.Value2 = "申请人:" + dr_cs["制单人员"].ToString();
                range = ws.get_Range("F35", Type.Missing);
                range.Value2 = "仓管员:" + CPublic.Var.localUserName;
                range = ws.get_Range(" C14", Type.Missing);
                range.Value2 = dr_cs["产品编码"].ToString();
                range = ws.get_Range("J14", Type.Missing);
                range.Value2 = dr_cs["生产数量"].ToString();
                range = ws.get_Range("J12", Type.Missing);
                range.Value2 = dr_cs["产品名称"].ToString();
                range = ws.get_Range("C16", Type.Missing);
                range.Value2 = dr_cs["规格型号"].ToString();
                // */
                //复制 count-1 个 sheet
                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                }
                int ir = 1;
                int pos = 1;  //记数 循环次数
                int i_first = 20;      // 起始行 
                int i_count = 15; // 每页打多少条
                int i_第几张 = 1;
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    range.Value2 = ir++.ToString();
                    range = ws.get_Range("G" + i_first.ToString(), Type.Missing);
                    //克、千克、KG、公斤、米、卷、片             他们要求这些单位的东西显示四位小数，其他的不要小数点
                    if (r["计量单位"].ToString() == "" || r["计量单位"].ToString() == "KG" || r["计量单位"].ToString() == "千克" || r["计量单位"].ToString() == "克"
                        || r["计量单位"].ToString() == "公斤" || r["计量单位"].ToString() == "米" || r["计量单位"].ToString() == "卷" || r["计量单位"].ToString() == "片")
                    {
                        range.Value2 = r["输入领料数量"].ToString();
                    }
                    else
                    {
                        int d = Convert.ToInt32(Math.Round(Convert.ToDecimal(r["输入领料数量"])));
                        range.Value2 = d;
                    }

                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);

                    range.Value2 = r["物料名称"].ToString() + r["规格型号"].ToString();
                    range = ws.get_Range("B" + i_first.ToString(), Type.Missing);
                    // range.Value2 = r["原ERP物料编号"].ToString();
                    range.Value2 = r["物料编码"].ToString();
                    range = ws.get_Range("F" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["计量单位"].ToString();
                    range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["仓库名称"].ToString();
                    range = ws.get_Range("I" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["货架描述"].ToString();
                    range = ws.get_Range("J" + i_first.ToString(), Type.Missing);
                    if (f_视图)
                    {
                        range.Value2 = Convert.ToDecimal(r["库存总数"].ToString());

                    }
                    else
                    {
                        range.Value2 = Convert.ToDecimal(r["库存总数"].ToString()) - Convert.ToDecimal(r["输入领料数量"].ToString());
                    }

                    //range = ws.get_Range("L" + i_first.ToString(), Type.Missing);
                    //range.Value2 = r["主辅料"].ToString();
                    //range = ws.get_Range("L" + i_first.ToString(), Type.Missing);
                    //if (r["铆压"].Equals(true)) { range.Value2 = "是"; }
                    //超过 一页条数 换下一个sheet
                    if (pos % i_count == 0 && count != i_第几张)
                    {
                        i_第几张++;
                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 19;

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
                    //excelApp.Visible = false;
                    //BringWindowToTop(hwnd);
                    wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    excelApp.DisplayAlerts = false;

                    //excelApp.Quit();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();

            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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

        public static void fun_p_领料A5(string str_出库单号, System.Data.DataTable dtP, int count, string str_打印机, bool f_视图, bool blPreview = false)
        {
            //string fName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp";
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\领料单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '领料单'";
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
                // 
                //领用部门  编号 日期 
                string sql = string.Format(@"select slz.*,生产记录生产工单表.生效人 as 申请人,base.BOM确认 from 生产记录生产领料单主表 slz
                    left join 基础数据物料信息表 base on base.物料编码 = slz.物料编码 
                    left join 生产记录生产工单表 on 生产记录生产工单表.生产工单号=slz.生产工单号
                    where slz.领料出库单号 = '{0}'", str_出库单号);
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception(string.Format("没有找到领料出库单号：{0}", dtP.Rows[0]["领料出库单号"].ToString()));
                }
                if (dt.Rows[0]["BOM确认"].Equals(true))
                {
                    range = ws.get_Range("I6", Type.Missing);
                    range.Value2 = "★";
                }
                range = ws.get_Range("J16", Type.Missing);
                range.Value2 = dt.Rows[0]["生产车间"].ToString();

                range = ws.get_Range("C11", Type.Missing);
                range.Value2 = str_出库单号;

                range = ws.get_Range("C12", Type.Missing);
                range.Value2 = dt.Rows[0]["生产工单号"].ToString();

                range = ws.get_Range("I35", Type.Missing);
                range.Value2 = Convert.ToDateTime(dt.Rows[0]["创建日期"]).ToString("yyyy-MM-dd");

                range = ws.get_Range("C35", Type.Missing);
                range.Value2 = dt.Rows[0]["领料人员ID"].ToString() + " " + dt.Rows[0]["领料人员"].ToString();

                range = ws.get_Range("E35", Type.Missing);
                range.Value2 = "申请人:" + dt.Rows[0]["申请人"].ToString();
                range = ws.get_Range("F35", Type.Missing);
                range.Value2 = "仓管员:" + dt.Rows[0]["生效人员"].ToString();
                range = ws.get_Range(" C14", Type.Missing);
                range.Value2 = dt.Rows[0]["物料编码"].ToString();

                range = ws.get_Range("J14", Type.Missing);
                range.Value2 = dt.Rows[0]["生产数量"].ToString();

                range = ws.get_Range("J12", Type.Missing);
                range.Value2 = dt.Rows[0]["物料名称"].ToString();
                range = ws.get_Range("C16", Type.Missing);
                range.Value2 = dt.Rows[0]["规格型号"].ToString();
                // */
                //复制 count-1 个 sheet
                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                }
                int ir = 1;
                int pos = 1;  //记数 循环次数
                int i_first = 20;      // 起始行 
                int i_count = 15; // 每页打多少条
                int i_第几张 = 1;

                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    range.Value2 = ir++.ToString();
                    range = ws.get_Range("G" + i_first.ToString(), Type.Missing);
                    //克、千克、KG、公斤、米、卷、片             他们要求这些单位的东西显示四位小数，其他的不要小数点
                    if (r["计量单位"].ToString() == "" || r["计量单位"].ToString() == "KG" || r["计量单位"].ToString() == "千克" || r["计量单位"].ToString() == "克"
                        || r["计量单位"].ToString() == "公斤" || r["计量单位"].ToString() == "米" || r["计量单位"].ToString() == "卷" || r["计量单位"].ToString() == "片")
                    {
                        range.Value2 = r["输入领料数量"].ToString();
                    }
                    else
                    {
                        int d = Convert.ToInt32(Math.Round(Convert.ToDecimal(r["输入领料数量"])));
                        range.Value2 = d;
                    }


                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);

                    range.Value2 = r["物料名称"].ToString() + r["规格型号"].ToString();
                    range = ws.get_Range("B" + i_first.ToString(), Type.Missing);
                    // range.Value2 = r["原ERP物料编号"].ToString();
                    range.Value2 = r["物料编码"].ToString();


                    range = ws.get_Range("F" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["计量单位"].ToString();
                    range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["仓库名称"].ToString();
                    range = ws.get_Range("I" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["货架描述"].ToString();

                    range = ws.get_Range("J" + i_first.ToString(), Type.Missing);
                    if (f_视图)
                    {
                        range.Value2 = Convert.ToDecimal(r["库存总数"].ToString());

                    }
                    else
                    {
                        range.Value2 = Convert.ToDecimal(r["库存总数"].ToString()) - Convert.ToDecimal(r["输入领料数量"].ToString());
                    }

                    //range = ws.get_Range("L" + i_first.ToString(), Type.Missing);
                    //range.Value2 = r["主辅料"].ToString();
                    //range = ws.get_Range("L" + i_first.ToString(), Type.Missing);
                    //if (r["铆压"].Equals(true)) { range.Value2 = "是"; }
                    //超过 一页条数 换下一个sheet
                    if (pos % i_count == 0 && count != i_第几张)
                    {
                        i_第几张++;
                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 19;

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
                    //excelApp.Visible = false;
                    //BringWindowToTop(hwnd);
                    wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    excelApp.DisplayAlerts = false;

                    //excelApp.Quit();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();

            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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
        /// <summary>
        /// 退货打印
        /// </summary>
        /// <param name="str_出入库申请单号"></param>
        /// <param name="dtP"></param>
        /// <param name="count"></param>  需几张sheet 
        /// <param name="blPreview"></param>
        public static void fun_print_退货入库_A5(string str_退货申请单号, string str_仓管人员, System.Data.DataTable dtP, int count, bool f_视图, string str_打印机, bool blPreview = false)
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\退货入库单.xlsx";


            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '退货入库单'";
                new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                if (dtPP.Rows.Count == 0) return;

                System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
            }
            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
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
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;
                //编号 日期 
                //string sql = string.Format("select * from 销售记录成品出库单主表 where 成品出库单号 = '{0}'", dr_传.Rows[0]["成品出库单号"].ToString());  、

                //                string sql = string.Format(@"select right(出入库申请单号,10)as 编号,申请类型,操作人员,部门 from 其他出入库申请主表,人事基础员工表 where 出入库申请单号='{0}' 
                //                                            and  其他出入库申请主表.操作人员编号=人事基础员工表.员工号 ", str_退货申请单号);
                //                System.Data.DataTable dt = new System.Data.DataTable();
                //new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);

                //range = ws.get_Range("E5", Type.Missing);
                //range.Value2 = dt.Rows[0]["部门"].ToString();

                range = ws.get_Range("A7", Type.Missing);
                range.Value2 = "单号";
                range = ws.get_Range("E7", Type.Missing);
                range.Value2 = dtP.Rows[0]["退货入库单号"].ToString();

                //range = ws.get_Range("E7", Type.Missing);
                //range.Value2 = str_退货申请单号;

                range = ws.get_Range("M7", Type.Missing);
                range.Value2 = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                string ss = string.Format(@"select  a.操作人员 as 申请人,c.领导姓名  as 审核人 from 退货申请主表 a,人事基础员工表 b,人事基础部门表 c
                         where a.操作人员编号=b.员工号 and b.课室编号=c.部门编号 and 退货申请单号='{0}' ", str_退货申请单号);
                using (SqlDataAdapter da = new SqlDataAdapter(ss, CPublic.Var.strConn))
                {
                    System.Data.DataTable temp = new System.Data.DataTable();
                    da.Fill(temp);
                    range = ws.get_Range("E18", Type.Missing);
                    range.Value2 = temp.Rows[0]["申请人"];

                    range = ws.get_Range("H18", Type.Missing);
                    range.Value2 = temp.Rows[0]["审核人"];

                    range = ws.get_Range("M18", Type.Missing);
                    range.Value2 = str_仓管人员;

                }
                //range = ws.get_Range("E21", Type.Missing);
                //range.Value2 = dt.Rows[0]["操作人员"].ToString();


                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                }

                int pos = 1;  //记数 循环次数
                int i_first = 9;      // 起始行 
                int i_count = 9; // 每页打多少条

                int ir = 1;//第几张sheet                                      
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);

                    range.Value2 = pos.ToString();

                    range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
                    //range.Value2 = r["规格型号"].ToString() + r["物料名称"].ToString().Trim();
                    range.Value2 = r["n原ERP规格型号"].ToString() + r["物料名称"].ToString().Trim();

                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["原ERP物料编号"].ToString();

                    range = ws.get_Range("I" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["数量"].ToString();
                    string sql_1 = string.Format(@"select 计量单位,仓库名称,库存总数,货架描述 from 基础数据物料信息表,仓库物料数量表 
                                        where 基础数据物料信息表.物料编码=仓库物料数量表.物料编码 and  原ERP物料编号='{0}'", r["原ERP物料编号"].ToString());
                    System.Data.DataTable dt_1 = new System.Data.DataTable();
                    dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, CPublic.Var.strConn);
                    if (dt_1.Rows.Count > 0)
                    {

                        range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
                        range.Value2 = dt_1.Rows[0]["计量单位"].ToString();

                        //range = ws.get_Range("E4", Type.Missing);
                        //range.Value2 = dt_1.Rows[0]["仓库名称"].ToString();

                        range = ws.get_Range("K" + i_first.ToString(), Type.Missing);
                        range.Value2 = dt_1.Rows[0]["货架描述"].ToString();

                        range = ws.get_Range("M" + i_first.ToString(), Type.Missing);
                        if (f_视图)
                        {
                            range.Value2 = dt_1.Rows[0]["库存总数"].ToString();
                        }
                        else
                        {
                            range.Value2 = Convert.ToDecimal(dt_1.Rows[0]["库存总数"].ToString()) + Convert.ToDecimal(r["数量"].ToString());

                        }
                    }
                    //超过 icount 条 换下一个sheet
                    if (pos % i_count == 0 && ir != count)
                    {
                        ir++;
                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 8;

                    }
                    i_first = i_first + 1;
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
                    //BringWindowToTop(hwnd);
                    wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    excelApp.Quit();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
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

        public static void fun_print_其他出库_A5(string str_仓管员, string str_出入库申请单号, System.Data.DataTable dtP, int count, bool f_视图, string str_打印机, bool blPreview = false)
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\其他出库单.xlsx";


            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '其他出库单'";
                new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                if (dtPP.Rows.Count == 0) return;

                System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
            }
            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
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
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;
                //编号 日期 
                //string sql = string.Format("select * from 销售记录成品出库单主表 where 成品出库单号 = '{0}'", dr_传.Rows[0]["成品出库单号"].ToString());  、
                //right(a.出入库申请单号,10)as 编号
                string sql = string.Format(@"select 其他出库单号 as 编号,申请类型,a.操作人员,部门,a.备注,b.生效日期 from 其他出入库申请主表  a
                        left join 人事基础员工表 on  a.操作人员编号=人事基础员工表.员工号 
                        left join 其他出库主表 b on a.出入库申请单号=b.出入库申请单号 where    a.出入库申请单号='{0}'  ", str_出入库申请单号);
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);

                range = ws.get_Range("E5", Type.Missing);
                range.Value2 = dt.Rows[0]["部门"].ToString();

                range = ws.get_Range("L2", Type.Missing);
                range.Value2 = dt.Rows[0]["申请类型"].ToString();

                range = ws.get_Range("M4", Type.Missing);
                range.Value2 = dt.Rows[0]["编号"].ToString();


                range = ws.get_Range("M5", Type.Missing);
                if (dt.Rows[0]["生效日期"] == DBNull.Value)
                {
                    range.Value2 = CPublic.Var.getDatetime();
                }
                else
                {
                    range.Value2 = Convert.ToDateTime(dt.Rows[0]["生效日期"]).ToString("yyyy-MM-dd");
                }
                range = ws.get_Range("E21", Type.Missing);
                range.Value2 = dt.Rows[0]["操作人员"].ToString();
                range = ws.get_Range("G21", Type.Missing);
                range.Value2 = "申请人:" + dt.Rows[0]["操作人员"].ToString();
                range = ws.get_Range("H21", Type.Missing);
                range.Value2 = "仓管员:" + str_仓管员;
                range = ws.get_Range("E22", Type.Missing);
                range.Value2 = dt.Rows[0]["备注"].ToString();

                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                }

                int pos = 1;  //记数 循环次数
                int i_first = 7;      // 起始行 
                int i_count = 14; // 每页打多少条

                int ir = 1;//第几张sheet                                      
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    //range.Value2 = r["成品出库单号"].ToString();
                    range.Value2 = pos.ToString();

                    range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
                    //range.Value2 = r["规格型号"].ToString() + r["物料名称"].ToString().Trim();
                    range.Value2 = r["规格型号"].ToString() + r["物料名称"].ToString().Trim();

                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["物料编码"].ToString();

                    range = ws.get_Range("I" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["数量"].ToString();
                    string sql_1 = string.Format(@"select 计量单位,kc.仓库名称,库存总数,kc.货架描述 from 基础数据物料信息表 base,仓库物料数量表 kc
                                        where base.物料编码=kc.物料编码 and  kc.物料编码='{0}'", r["物料编码"].ToString());
                    System.Data.DataTable dt_1 = new System.Data.DataTable();
                    dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, CPublic.Var.strConn);
                    if (dt_1.Rows.Count > 0)
                    {

                        range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
                        range.Value2 = dt_1.Rows[0]["计量单位"].ToString();

                        range = ws.get_Range("E4", Type.Missing);
                        range.Value2 = dt_1.Rows[0]["仓库名称"].ToString();

                        range = ws.get_Range("K" + i_first.ToString(), Type.Missing);
                        range.Value2 = dt_1.Rows[0]["货架描述"].ToString();

                        range = ws.get_Range("M" + i_first.ToString(), Type.Missing);
                        if (f_视图)
                        {
                            range.Value2 = dt_1.Rows[0]["库存总数"].ToString();
                        }
                        else
                        {
                            range.Value2 = Convert.ToDecimal(dt_1.Rows[0]["库存总数"].ToString()) - Convert.ToDecimal(r["数量"].ToString());

                        }
                    }
                    //超过 icount 条 换下一个sheet
                    if (pos % i_count == 0 && ir != count)
                    {
                        ir++;
                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 6;

                    }
                    i_first = i_first + 1;
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
                    //BringWindowToTop(hwnd);
                    wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    excelApp.Quit();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
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

        public static void fun_print_借用出库(string str_出入库申请单号, System.Data.DataTable dtP, int count, bool f_视图, string str_打印机, bool blPreview = false)
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\其他出库单.xlsx";


            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '其他出库单'";
                new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                if (dtPP.Rows.Count == 0) return;

                System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
            }
            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
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
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;
                //编号 日期 
                //string sql = string.Format("select * from 销售记录成品出库单主表 where 成品出库单号 = '{0}'", dr_传.Rows[0]["成品出库单号"].ToString());  、

                string sql = string.Format(@" select 借还申请表.*,人事基础员工表.课室 from 借还申请表 
                left join 人事基础员工表 on 人事基础员工表.员工号=借还申请表.工号 where 借还申请表.申请批号 = '{0}'", str_出入库申请单号);
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);

                //range = ws.get_Range("E5", Type.Missing);
                //range.Value2 = dt.Rows[0]["部门"].ToString();

                //range = ws.get_Range("L2", Type.Missing);
                //range.Value2 = dt.Rows[0]["申请类型"].ToString();
                range = ws.get_Range("A4", Type.Missing);
                range.Value2 = "";
                range = ws.get_Range("A5", Type.Missing);
                range.Value2 = "借出仓库:";
                range = ws.get_Range("A21", Type.Missing);
                range.Value2 = "借用人:";
                range = ws.get_Range("M4", Type.Missing);
                range.Value2 = dt.Rows[0]["申请批号"].ToString();

                range = ws.get_Range("M5", Type.Missing);
                range.Value2 = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");

                range = ws.get_Range("E21", Type.Missing);
                range.Value2 = dt.Rows[0]["申请人"].ToString();
                range = ws.get_Range("H21", Type.Missing);
                range.Value2 = dt.Rows[0]["备注"].ToString();

                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                }

                int pos = 1;  //记数 循环次数
                int i_first = 7;      // 起始行 
                int i_count = 14; // 每页打多少条

                int ir = 1;//第几张sheet                                      
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    //range.Value2 = r["成品出库单号"].ToString();
                    range.Value2 = pos.ToString();

                    range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
                    //range.Value2 = r["规格型号"].ToString() + r["物料名称"].ToString().Trim();
                    range.Value2 = r["n原ERP规格型号"].ToString() + r["物料名称"].ToString().Trim();

                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["原ERP物料编号"].ToString();

                    range = ws.get_Range("I" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["申请借用数量"].ToString();
                    string sql_1 = string.Format(@"select 计量单位,仓库名称,库存总数,货架描述 from 基础数据物料信息表,仓库物料数量表 
                                        where 基础数据物料信息表.物料编码=仓库物料数量表.物料编码 and  原ERP物料编号='{0}'", r["原ERP物料编号"].ToString());
                    System.Data.DataTable dt_1 = new System.Data.DataTable();
                    dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, CPublic.Var.strConn);
                    if (dt_1.Rows.Count > 0)
                    {

                        range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
                        range.Value2 = dt_1.Rows[0]["计量单位"].ToString();

                        range = ws.get_Range("E5", Type.Missing);
                        range.Value2 = dt_1.Rows[0]["仓库名称"].ToString();

                        range = ws.get_Range("K" + i_first.ToString(), Type.Missing);
                        range.Value2 = dt_1.Rows[0]["货架描述"].ToString();

                        range = ws.get_Range("M" + i_first.ToString(), Type.Missing);
                        if (f_视图)
                        {
                            range.Value2 = dt_1.Rows[0]["库存总数"].ToString();
                        }
                        else
                        {
                            range.Value2 = Convert.ToDecimal(dt_1.Rows[0]["库存总数"].ToString()) - Convert.ToDecimal(r["申请借用数量"].ToString());

                        }
                    }
                    //超过 icount 条 换下一个sheet
                    if (pos % i_count == 0 && ir != count)
                    {
                        ir++;
                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 6;

                    }
                    i_first = i_first + 1;
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
                    //BringWindowToTop(hwnd);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtP"></param>
        /// <param name="count"></param> 要分几张打印
        /// <param name="blPreview"></param>
        public static void fun_print_送货单_修改(System.Data.DataTable dtP, int count, bool blPreview = false)
        {
            string fName = System.Windows.Forms.Application.StartupPath + @"\prttmp";


            if (Directory.Exists(fName) == false)
            {
                Directory.CreateDirectory(fName);
            }
            //string fileName = "C:\\Program Files\\打印模板\\送货单.xlsx";
            fName = fName + "\\送货单.xlsx";

            if (System.IO.File.Exists(fName).Equals(false))
            {
                FileStream fs = new FileStream(fName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '送货单'";
                new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                if (dtPP.Rows.Count == 0) return;

                System.IO.File.WriteAllBytes(fName, (byte[])dtPP.Rows[0]["数据"]);
            }
            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                Workbook wb = excelApp.Workbooks.Open(fName, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing);
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;
                //编号 日期 
                //string sql = string.Format("select * from 销售记录成品出库单主表 where 成品出库单号 = '{0}'", dr_传.Rows[0]["成品出库单号"].ToString());  、

                string sql = string.Format(@"SELECT a.[成品出库单号],a.[销售订单明细号],a.[送货方式],a.客户,b.销售订单号,c.操作员,
                        a.生效日期,b.客户订单号 from [销售记录成品出库单明细表] a
                         left join  销售记录销售订单主表 b  on  left(a.销售订单明细号,14)=b.销售订单号 
                         left join 销售记录成品出库单主表 c on a.成品出库单号=c.成品出库单号 
                         where c.成品出库单号 = '{0}'", dtP.Rows[0]["成品出库单号"].ToString());
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);

                range = ws.get_Range("D3", Type.Missing);
                range.Value2 = dt.Rows[0]["成品出库单号"].ToString();
                range = ws.get_Range("P7", Type.Missing);
                range.Value2 = dt.Rows[0]["客户订单号"].ToString();
                range = ws.get_Range("T37", Type.Missing);
                range.Value2 = dt.Rows[0]["操作员"].ToString();

                range = ws.get_Range("S3", Type.Missing);
                //range.Value2 = System.DateTime.Today.ToString("yyyy-MM-dd");
                range.Value2 = Convert.ToDateTime(dt.Rows[0]["生效日期"]).Date.ToString("yyyy-MM-dd");

                //收货单位 
                range = ws.get_Range("F7", Type.Missing);
                range.Value2 = dt.Rows[0]["客户"].ToString();

                //string sql1 = string.Format("select * from 销售记录销售出库通知单主表 where 出库通知单号 = '{0}'", dtP.Rows[0]["出库通知单号"].ToString());
                //System.Data.DataTable dt1 = new System.Data.DataTable();
                //new SqlDataAdapter(sql1, CPublic.Var.strConn).Fill(dt1);

                //range = ws.get_Range("U7", Type.Missing);
                //range.Value2 = dt1.Rows[0]["送货方式"].ToString();



                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                }

                int pos = 1;  //记数 循环次数
                int i_first = 13;      // 起始行 
                int i_count = 8; // 每页打多少条

                int ir = 1;  //当前第几张sheet
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);
                    //range.Value2 = r["成品出库单号"].ToString();
                    range.Value2 = dt.Rows[0]["销售订单号"].ToString();

                    // range = ws.get_Range("F" + i_first.ToString(), Type.Missing);
                    //range.Value2 = r["规格型号"].ToString() + r["物料名称"].ToString().Trim();
                    // range.Value2 = r["n原ERP规格型号"].ToString() + r["物料名称"].ToString().Trim();

                    range = ws.get_Range("M" + i_first.ToString(), Type.Missing);
                    //range.Value2 = r["原ERP物料编号"].ToString();
                    range.Value2 = r["物料编码"].ToString();


                    range = ws.get_Range("N" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["出库数量"].ToString();
                    range = ws.get_Range("P" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["计量单位"].ToString();

                    //                    string sqld = string.Format(@"select 销售记录销售订单明细表.*,产品线,规格,原规格型号  from 销售记录销售订单明细表,基础数据物料信息表 
                    //                              where 基础数据物料信息表.物料编码=销售记录销售订单明细表.物料编码 and 销售订单明细号 ='{0}'", r["销售订单明细号"].ToString());

                    string sqld = string.Format(@"select 销售记录销售订单明细表.*  from 销售记录销售订单明细表 
                              where 销售订单明细号 ='{0}'", r["销售订单明细号"].ToString());
                    System.Data.DataTable dtt = new System.Data.DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sqld, CPublic.Var.strConn);
                    da.Fill(dtt);
                    range = ws.get_Range("F" + i_first.ToString(), Type.Missing);
                    range.Value2 = dtt.Rows[0]["规格型号"].ToString() + r["物料名称"].ToString().Trim();

                    //if (dtt.Rows[0]["原规格型号"].ToString().Trim() != "")     //原规格型号中存放的 是 客户型号  要求的 如果有 客户型号 打印客户型号
                    //{
                    //    range.Value2 = dtt.Rows[0]["规格"].ToString() + r["物料名称"].ToString().Trim();
                    //}
                    //else
                    //{
                    //    if (dtt.Rows[0]["产品线"].ToString() == "智能终端电器")
                    //    {
                    //        range.Value2 = dtt.Rows[0]["规格"].ToString() + r["物料名称"].ToString().Trim();
                    //    }
                    //    else
                    //    {
                    //        range.Value2 = r["n原ERP规格型号"].ToString() + r["物料名称"].ToString().Trim();
                    //    }
                    //}
                    range = ws.get_Range("S" + i_first.ToString(), Type.Missing);
                    range.Value2 = dtt.Rows[0]["备注"].ToString();



                    //i++;


                    //超过 icount 条 换下一个sheet
                    if (pos % i_count == 0 && ir != count)
                    {
                        ir++;
                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 10;

                    }
                    i_first = i_first + 3;
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
                    //BringWindowToTop(hwnd);
                    wb.PrintOutEx();
                    excelApp.Quit();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtP"></param> 需打印清单
        /// <param name="i"></param>   需分多少张打印
        /// <param name="str_打印机"></param>
        public static void fun_print_销售出库通知单_A5(System.Data.DataTable dtP, int i, string str_打印机, string str_仓管人员)
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\销售出库通知单_A5.xlsx";

            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '销售出库通知单'";
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
                excelApp.DisplayAlerts = false;
                excelApp.ScreenUpdating = false;

                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;
                //领用部门  编号 日期 
                string sql = string.Format(@"select a.*,isnull(c.领导姓名,'') as 审核人 from 销售记录销售出库通知单主表 a
                    left  join 人事基础员工表 b  on a.操作员ID=b.员工号   
                    left  join  人事基础部门表 c on b.课室编号=c.部门编号   where  出库通知单号 = '{0}'", dtP.Rows[0]["出库通知单号"].ToString());
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);

                range = ws.get_Range("C15", Type.Missing);
                range.Value2 = dt.Rows[0]["出库通知单号"].ToString();
                range = ws.get_Range("S15", Type.Missing);
                range.Value2 = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");

                range = ws.get_Range("C54", Type.Missing);
                range.Value2 = dt.Rows[0]["操作员"];

                range = ws.get_Range("H54", Type.Missing);
                range.Value2 = dt.Rows[0]["审核人"];

                range = ws.get_Range("Q54", Type.Missing);
                range.Value2 = str_仓管人员;
                //收货单位 `
                range = ws.get_Range("C18", Type.Missing);
                range.Value2 = dt.Rows[0]["客户名"].ToString();

                string sss = string.Format("select 地址 from 客户基础信息表 where 客户编号 = '{0}'", dtP.Rows[0]["客户编号"].ToString().Trim());
                System.Data.DataTable ttt = new System.Data.DataTable();
                new SqlDataAdapter(sss, CPublic.Var.strConn).Fill(ttt);
                if (ttt.Rows.Count > 0)
                {
                    range = ws.get_Range("O18", Type.Missing);
                    range.Value2 = ttt.Rows[0]["地址"].ToString();
                }
                range = ws.get_Range("C51", Type.Missing);
                range.Value2 = dt.Rows[0]["备注"].ToString();
                range = ws.get_Range("Q51", Type.Missing);
                range.Value2 = dt.Rows[0]["送货方式"].ToString();





                int pos = 0;
                int ir = 1;
                int i_count = 9; //每页多少条记录
                int i_first = 24;
                for (int j = 1; j < i; j++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);
                }
                string ssql = string.Format("select * from 销售记录销售出库通知单明细表 where 出库通知单号 = '{0}'", dtP.Rows[0]["出库通知单号"].ToString());
                System.Data.DataTable ddt = new System.Data.DataTable();
                SqlDataAdapter dda = new SqlDataAdapter(ssql, CPublic.Var.strConn);
                dda.Fill(ddt);

                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    range = ws.get_Range("B" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["物料名称"].ToString();
                    try
                    {
                        string sqld = string.Format("select 规格型号,货架描述,计量单位 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                        //string sqld = string.Format("select 原ERP物料编号,n原ERP规格型号,货架描述,计量单位 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());

                        System.Data.DataTable dtt = new System.Data.DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(sqld, CPublic.Var.strConn);
                        da.Fill(dtt);
                        range = ws.get_Range("M" + i_first.ToString(), Type.Missing);
                        range.Value2 = dtt.Rows[0]["计量单位"].ToString();
                        range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                        // range.Value2 = dtt.Rows[0]["原ERP物料编号"].ToString().Trim();
                        range.Value2 = r["物料编码"].ToString();

                        range = ws.get_Range("D" + i_first.ToString(), Type.Missing);
                        // range.Value2 = dtt.Rows[0]["n原ERP规格型号"].ToString();
                        range.Value2 = dtt.Rows[0]["规格型号"].ToString();

                        range = ws.get_Range("N" + i_first.ToString(), Type.Missing);
                        range.Value2 = dtt.Rows[0]["货架描述"].ToString();

                        range = ws.get_Range("P" + i_first.ToString(), Type.Missing);
                        range.Value2 = r["销售备注"].ToString();
                    }
                    catch { }
                    range = ws.get_Range("K" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["出库数量"].ToString();
                    range = ws.get_Range("T" + i_first.ToString(), Type.Missing);
                    try
                    {
                        string sqld = string.Format("select 库存总数 from 仓库物料数量表 where 物料编码 = '{0}'", r["物料编码"].ToString());
                        System.Data.DataTable dtt = new System.Data.DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(sqld, CPublic.Var.strConn);
                        da.Fill(dtt);
                        range.Value2 = dtt.Rows[0]["库存总数"].ToString();
                    }
                    catch { }
                    i_first = i_first + 3;

                    pos++;
                    if (pos % i_count == 0 && ir != i)
                    {
                        ir++;
                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 24;

                    }
                }


                excelApp.Visible = false;
                //BringWindowToTop(hwnd);
                wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();

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



        public static void fun_print_生产工单_A5(System.Data.DataRow dr_传, int pt_c, bool blPreview = false, string str_prt = "")
        {


            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\生产工单新.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '生产工单新'";
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
                string sql_3 = string.Format("select * from [需确认包装清单表] where 物料编码='{0}'", dr_传["物料编码"]);

                System.Data.DataTable dt_3 = CZMaster.MasterSQL.Get_DataTable(sql_3, CPublic.Var.strConn);
                if (dt_3.Rows.Count > 0)
                {
                    range = ws.get_Range("O8", Type.Missing);
                    range.Value2 = "★";
                }
                string sql_1 = string.Format("select 货架描述,仓库名称,原规格型号,规格型号 from 基础数据物料信息表 where 物料编码= '{0}'", dr_传["物料编码"]);
                using (SqlDataAdapter da = new SqlDataAdapter(sql_1, CPublic.Var.strConn))
                {
                    System.Data.DataTable dt_1 = new System.Data.DataTable();
                    da.Fill(dt_1);
                    range = ws.get_Range("I17", Type.Missing);
                    range.Value2 = dt_1.Rows[0]["货架描述"].ToString();
                    range = ws.get_Range("C17", Type.Missing);
                    range.Value2 = dt_1.Rows[0]["仓库名称"].ToString();

                    range = ws.get_Range("L28", Type.Missing);
                    range.Value2 = dt_1.Rows[0]["原规格型号"].ToString(); //基础数据物料信息表 中原规格型号 弃用 改为 客户规格
                    range = ws.get_Range("D32", Type.Missing);
                    range.Value2 = dr_传["规格型号"].ToString();
                }
                sql_1 = string.Format(@"select * from 生产记录生产制令表 a  left join 生产记录生产制令子表 b on a.生产制令单号 =b.生产制令单号 
                     
                     where a.生产制令单号= '{0}'", dr_传["生产制令单号"]);

                using (SqlDataAdapter da = new SqlDataAdapter(sql_1, CPublic.Var.strConn))
                {
                    System.Data.DataTable dt_1 = new System.Data.DataTable();
                    da.Fill(dt_1);
                    if (dt_1.Rows.Count > 0)
                    {
                        range = ws.get_Range("Q29", Type.Missing);
                        range.Value2 = dt_1.Rows[0]["制单人员"].ToString();
                        range = ws.get_Range("A15", Type.Missing);
                        range.Value2 = "客户";
                        range = ws.get_Range("C15", Type.Missing);
                        range.Value2 = dt_1.Rows[0]["客户"].ToString();
                    }
                }
                string sql_2 = string.Format(@"select 客户订单号 from 销售记录销售订单主表 where 销售订单号 in (select 销售订单号  from 生产记录生产工单表 
                                         left join 生产记录生产制令子表 on 生产记录生产制令子表.生产制令单号 =生产记录生产工单表.生产制令单号 
                                            where 生产工单号='{0}')", dr_传["生产工单号"].ToString());

                using (SqlDataAdapter da = new SqlDataAdapter(sql_2, CPublic.Var.strConn))
                {
                    System.Data.DataTable dt_2 = new System.Data.DataTable();
                    da.Fill(dt_2);
                    if (dt_2.Rows.Count > 0)
                    {
                        range = ws.get_Range("N17", Type.Missing);
                        range.Value2 = dt_2.Rows[0]["客户订单号"].ToString();
                    }

                }
                range = ws.get_Range("D20", Type.Missing);
                range.Value2 = dr_传["生产工单号"].ToString();

                range = ws.get_Range("B11", Type.Missing);
                range.Value2 = dr_传["加急状态"].ToString();//加急状态 生产工单类型

                range = ws.get_Range("D24", Type.Missing);
                //range.Value2 = dr_传["原ERP物料编号"].ToString();//加急状态 生产工单类型
                range.Value2 = dr_传["物料编码"].ToString();

                range = ws.get_Range("L20", Type.Missing);
                range.Value2 = dr_传["车间名称"].ToString();
                range = ws.get_Range("L24", Type.Missing);
                range.Value2 = dr_传["生效日期"].ToString();
                range = ws.get_Range("D28", Type.Missing);
                //range.Value2 = dr_传.Rows[0]["物料编码"].ToString();
                range.Value2 = dr_传["物料名称"].ToString();
                range = ws.get_Range("Q49", Type.Missing);
                //range.Value2 = dr_传.Rows[0]["物料编码"].ToString();
                range.Value2 = dr_传["生效人"].ToString();

                range = ws.get_Range("D36", Type.Missing);
                range.Value2 = dr_传["规格型号"].ToString();
                range = ws.get_Range("D37", Type.Missing);
                range.Value2 = dr_传["工单负责人ID"].ToString() + dr_传["工单负责人"].ToString();

                range = ws.get_Range("L45", Type.Missing);
                range.Value2 = dr_传["备注1"].ToString();
                range = ws.get_Range("L37", Type.Missing);
                range.Value2 = dr_传["生产数量"].ToString();

                ////物料包装清单
                try
                {
                    //基础数据物料信息表 原规格型号 意思为  客户型号
                    string sql = string.Format(@"select 基础数据包装清单表.*,基础数据物料信息表.原ERP物料编号 from 基础数据包装清单表 
                        left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 基础数据包装清单表.物料编码 
                        where 成品编码 = '{0}' order by  基础数据包装清单表.规格型号 ", dr_传["物料编码"].ToString().Trim());
                    System.Data.DataTable dtdt = new System.Data.DataTable();
                    new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dtdt);
                    int count = dtdt.Rows.Count;
                    if (count > 0)
                    {
                        if (count % 24 != 0)
                        {
                            count = (count / 24) + 1;
                        }
                        else
                        {
                            count = count / 24;
                        }
                        for (int j = 1; j < count; j++)
                        {
                            ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);
                        }
                        int pos = 1;  //记数 循环次数

                        int i_first = 53;      // 起始行 


                        int i_count = 24; // 每页打多少行

                        foreach (System.Data.DataRow r in dtdt.Rows)
                        {
                            string ss = r["类型"].ToString();  //线材还是配件
                            ss = string.Format("({0})", ss);
                            if (pos % 2 == 0)
                            {
                                //  r["原ERP物料编号"].ToString() + " " +
                                range = ws.get_Range("J" + i_first.ToString(), Type.Missing);
                                range.Value2 = r["物料名称"].ToString() + " " + r["规格型号"].ToString() + ss;
                                range = ws.get_Range("O" + i_first.ToString(), Type.Missing);
                                range.Value2 = Convert.ToDecimal(r["数量"]).ToString("n");
                                i_first = i_first + 4;

                            }
                            else
                            {
                                //r["原ERP物料编号"].ToString() + " " + 
                                range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                                range.Value2 = r["物料名称"].ToString() + " " + r["规格型号"].ToString() + ss;
                                range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
                                range.Value2 = Convert.ToDecimal(r["数量"]).ToString("n");
                            }



                            if (pos % i_count == 0)
                            {

                                ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                                ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                                i_first = 53;

                            }
                            pos = pos + 1;


                        }


                    }
                }
                catch (Exception ex)
                {
                    excelApp = null;
                    GcCollect();
                    KillProcess(PID);
                    throw ex;
                }

                if (blPreview)
                {
                    excelApp.Visible = true;
                    wb.PrintPreview();
                }
                else
                {
                    excelApp.Visible = false;

                    //BringWindowToTop(hwnd);


                    excelApp.ScreenUpdating = false;

                    excelApp.DisplayAlerts = false;
                    if (str_prt != "")
                    {
                        wb.PrintOutEx(1, Type.Missing, pt_c, false, str_prt, false, false, Type.Missing, false);
                    }
                    else
                    {
                        wb.PrintOutEx(1, Type.Missing, pt_c, false, Type.Missing, false, false, Type.Missing, false);
                    }
                    excelApp.Quit();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
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


        public static void fun_print_送货单(System.Data.DataTable dtP, int i, bool blPreview = false)
        {
            string fName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp";
            if (Directory.Exists(fName) == false)
            {
                Directory.CreateDirectory(fName);
            }
            string fileName = "C:\\Program Files\\打印模板\\送货单.xlsx";
            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                //{
                //    System.Data.DataTable dtPP = new System.Data.DataTable();
                //    string s = "select * from 基础记录打印模板表 where 模板名 = '送货单'";
                //    new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                //    if (dtPP.Rows.Count == 0) return;
                //    try
                //    {
                //        System.IO.Directory.Delete(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp", true);
                //    }
                //    catch
                //    {
                //    }
                //    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp");
                //    System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
                //}

                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing);
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;
                //编号 日期 


                string sql = string.Format(@"SELECT [成品出库单号],scmx.[销售订单明细号],[客户],szb.销售订单号,sctzb.送货方式
                                        szb.客户订单号 from [销售记录成品出库单明细表] scmx 
                    left join 销售记录销售出库通知单主表 sctzb on sctzb.出库通知单号=scmx.出库通知单号
                    left join 销售记录销售订单明细表 smx on smx.销售订单明细号= szb.销售订单明细号                
                    left join  销售记录销售订单主表 szb  on  smx.销售订单号 =销售记录销售订单主表.销售订单号 where 成品出库单号 = '{0}'", dtP.Rows[0]["成品出库单号"].ToString());
                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);

                range = ws.get_Range("D3", Type.Missing);
                range.Value2 = dt.Rows[0]["成品出库单号"].ToString();
                range = ws.get_Range("P7", Type.Missing);
                range.Value2 = dt.Rows[0]["客户订单号"].ToString();

                range = ws.get_Range("S3", Type.Missing);
                range.Value2 = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                //收货单位 `
                range = ws.get_Range("F7", Type.Missing);
                range.Value2 = dt.Rows[0]["客户"].ToString();

                //string sql1 = string.Format("select * from 销售记录销售出库通知单主表 where 出库通知单号 = '{0}'", dtP.Rows[0]["出库通知单号"].ToString());
                //System.Data.DataTable dt1 = new System.Data.DataTable();
                //new SqlDataAdapter(sql1, CPublic.Var.strConn).Fill(dt1);

                range = ws.get_Range("U7", Type.Missing);
                range.Value2 = dt.Rows[0]["送货方式"].ToString();

                //range = ws.get_Range("E37", Type.Missing);
                //range.Value2 = dt1.Rows[0]["备注"].ToString();

                int pos = 0;
                int i_first = 13;
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    if ((pos + i_first) >= 37)
                    {
                        break;
                    }
                    range = ws.get_Range("C" + (i_first + pos).ToString(), Type.Missing);
                    //range.Value2 = r["成品出库单号"].ToString();
                    range.Value2 = dt.Rows[0]["销售订单号"].ToString();

                    range = ws.get_Range("F" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["规格型号"].ToString() + r["物料名称"].ToString().Trim();
                    //range.Value2 = r["n原ERP规格型号"].ToString() + r["物料名称"].ToString().Trim();

                    range = ws.get_Range("N" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["出库数量"].ToString();
                    range = ws.get_Range("P" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["计量单位"].ToString();

                    string sqld = string.Format("select * from 销售记录销售订单明细表 where 销售订单明细号 = '{0}'", r["销售订单明细号"].ToString());
                    System.Data.DataTable dtt = new System.Data.DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sqld, CPublic.Var.strConn);
                    da.Fill(dtt);
                    range = ws.get_Range("S" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = dtt.Rows[0]["备注"].ToString();

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
                    //System.IO.File.Delete(fileName);
                }
            }
        }

        public static void fun_print_出厂检验报告_原(string flag, System.Data.DataTable dtM, System.Data.DataTable dtP, int i, string str_打印机, decimal dec_发货数量, decimal dec_合格数量, bool blPreview = false)
        {
            //string fName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp";
            //if (Directory.Exists(fName) == false)
            //{
            //    Directory.CreateDirectory(fName);
            //}
            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\出厂检验报告.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '出厂检验报告'";
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
                //{
                //    System.Data.DataTable dtPP = new System.Data.DataTable();
                //    string s = "select * from 基础记录打印模板表 where 模板名 = '出厂检验报告'";
                //    new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                //    if (dtPP.Rows.Count == 0) return;
                //    try
                //    {
                //        System.IO.Directory.Delete(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp", true);
                //    }
                //    catch
                //    {
                //    }
                //    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp");
                //    System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
                //}
                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing);
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;

                //                string sql = string.Format(@"select  生产记录生产检验单主表.*,原ERP物料编号,n原ERP规格型号 from 生产记录生产检验单主表
                //                                          left join   基础数据物料信息表 on 生产记录生产检验单主表.物料编码=基础数据物料信息表.物料编码
                //
                //                                            where 生产记录生产检验单主表.物料编码 = '{0}' order by 生产工单号 desc", dtM.Rows[0]["物料编码"]);

                string sql = string.Format(@"select  生产记录生产检验单主表.*,a.wjbm as 文件编号  from 生产记录生产检验单主表
                            --left join   基础数据物料信息表 on 生产记录生产检验单主表.物料编码=基础数据物料信息表.物料编码
                            left join (select 生产检验单号,wjbm  from [成品检验检验记录明细表] group by 生产检验单号,wjbm)as a on a.生产检验单号=生产记录生产检验单主表.生产检验单号
                            where 生产记录生产检验单主表.物料编码 = '{0}' order by 生产工单号 desc", dtM.Rows[0]["物料编码"]);

                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        //range = ws.get_Range("A8", Type.Missing);
                        //range.Value2 = dt.Rows[0]["原ERP物料编号"].ToString() + "-" + dtM.Rows[0]["物料名称"].ToString();

                        range = ws.get_Range("A8", Type.Missing);
                        range.Value2 = dt.Rows[0]["文件编号"].ToString();
                        //range = ws.get_Range("R8", Type.Missing);
                        //range.Value2 = dt.Rows[0]["生产工单号"].ToString();
                    }
                    catch { }

                    //range = ws.get_Range("D11", Type.Missing);
                    //range.Value2 = dtM.Rows[0]["规格"].ToString();
                    range = ws.get_Range("D11", Type.Missing);
                    range.Value2 = dt.Rows[0]["规格型号"].ToString();
                    range = ws.get_Range("M11", Type.Missing);
                    range.Value2 = dtM.Rows[0]["大类"].ToString();
                    range = ws.get_Range("S11", Type.Missing);
                    range.Value2 = dtM.Rows[0]["小类"].ToString();
                    range = ws.get_Range("D15", Type.Missing);
                    range.Value2 = dtM.Rows[0]["生产者"].ToString();
                    range = ws.get_Range("H15", Type.Missing);
                    range.Value2 = dtM.Rows[0]["班组"].ToString();
                    range = ws.get_Range("M15", Type.Missing);
                    range.Value2 = dtM.Rows[0]["部门"].ToString();
                    if (flag == "销售")
                    {
                        range = ws.get_Range("S15", Type.Missing);
                        range.Value2 = Convert.ToDateTime(dtM.Rows[0]["生产日期"]);
                        range = ws.get_Range("D19", Type.Missing);
                        range.Value2 = dtM.Rows[0]["发货数量"].ToString();
                        range = ws.get_Range("H19", Type.Missing);
                        range.Value2 = dtM.Rows[0]["合格数"].ToString();
                    }
                    else
                    {
                        range = ws.get_Range("S15", Type.Missing);
                        range.Value2 = CPublic.Var.getDatetime().Date.ToString("yyyy-MM-dd");
                        range = ws.get_Range("D19", Type.Missing);
                        range.Value2 = dec_发货数量.ToString();
                        range = ws.get_Range("H19", Type.Missing);
                        range.Value2 = dec_合格数量.ToString();
                    }



                    int pos = 0;
                    int i_first = 27;
                    foreach (System.Data.DataRow r in dtP.Rows)
                    {
                        //if ((pos + i_first) >= 118)
                        if ((pos + i_first) >= 70)
                        {
                            break;
                        }
                        range = ws.get_Range("A" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["序号"].ToString();
                        range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["检验项目"].ToString();
                        range = ws.get_Range("D" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["检验要求"].ToString();
                        range = ws.get_Range("L" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["检测水平"].ToString();
                        range = ws.get_Range("M" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["合格水平"].ToString();
                        range = ws.get_Range("N" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["a"].ToString();
                        range = ws.get_Range("O" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["b"].ToString();
                        range = ws.get_Range("P" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["c"].ToString();
                        range = ws.get_Range("Q" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["d"].ToString();
                        range = ws.get_Range("R" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["e"].ToString();
                        range = ws.get_Range("S" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["f"].ToString();
                        range = ws.get_Range("T" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["g"].ToString();
                        range = ws.get_Range("U" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["h"].ToString();

                        i++;
                        pos = pos + 4;
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
                    excelApp.ScreenUpdating = false;
                    //BringWindowToTop(hwnd);
                    wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    excelApp.DisplayAlerts = false;
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


        public static void fun_print_出厂检验报告(System.Data.DataRow dr_产品序列号, System.Data.DataTable dtM, System.Data.DataTable dtP, int i, string str_打印机, bool blPreview = false)
        {


            //string fName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp";
            //if (Directory.Exists(fName) == false)
            //{
            //    Directory.CreateDirectory(fName);
            //}
            //string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
            string path = System.Windows.Forms.Application.StartupPath + @"\prttmp";
            if (!System.IO.File.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string fileName;
            if (dr_产品序列号 != null)
            {
                fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\出厂检验报告(二厂).xlsx";
            }
            else
            {
                fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\出厂检验报告(一厂).xlsx";
            }

            if (System.IO.File.Exists(fileName).Equals(false)) //判断报告是否存在
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s;
                if (dr_产品序列号 != null)
                {
                    s = "select * from 基础记录打印模板表 where 模板名 = '出厂检验报告(二厂)'";
                }
                else
                {
                    s = "select * from 基础记录打印模板表 where 模板名 = '出厂检验报告(一厂)'";
                }
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
                //{
                //    System.Data.DataTable dtPP = new System.Data.DataTable();
                //    string s = "select * from 基础记录打印模板表 where 模板名 = '出厂检验报告'";
                //    new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                //    if (dtPP.Rows.Count == 0) return;
                //    try
                //    {
                //        System.IO.Directory.Delete(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp", true);
                //    }
                //    catch
                //    {
                //    }
                //    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp");
                //    System.IO.File.WriteAllBytes(fileName, (byte[])dtPP.Rows[0]["数据"]);
                //}

                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing);
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                Microsoft.Office.Interop.Excel.Range range;

                //                string sql = string.Format(@"select  生产记录生产检验单主表.*,原ERP物料编号,n原ERP规格型号 from 生产记录生产检验单主表
                //                                          left join   基础数据物料信息表 on 生产记录生产检验单主表.物料编码=基础数据物料信息表.物料编码
                //
                //                                            where 生产记录生产检验单主表.物料编码 = '{0}' order by 生产工单号 desc", dtM.Rows[0]["物料编码"]);

                string sql = string.Format(@"select  生产记录生产检验单主表.*,原ERP物料编号,规格型号,a.wjbm as 文件编号  from 生产记录生产检验单主表
                            left join   基础数据物料信息表 on 生产记录生产检验单主表.物料编码=基础数据物料信息表.物料编码
                            left join (select 生产检验单号,wjbm  from [成品检验检验记录明细表] group by 生产检验单号,wjbm)as a on a.生产检验单号=生产记录生产检验单主表.生产检验单号
                            where 生产记录生产检验单主表.物料编码 = '{0}' order by 生产工单号 desc", dtM.Rows[0]["物料编码"]);

                System.Data.DataTable dt = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        //range = ws.get_Range("A8", Type.Missing);
                        //range.Value2 = dt.Rows[0]["原ERP物料编号"].ToString() + "-" + dtM.Rows[0]["物料名称"].ToString();

                        range = ws.get_Range("A8", Type.Missing);
                        range.Value2 = dt.Rows[0]["文件编号"].ToString();
                        range = ws.get_Range("R8", Type.Missing);
                        range.Value2 = dt.Rows[0]["生产工单号"].ToString();
                    }
                    catch { }

                    //range = ws.get_Range("D11", Type.Missing);
                    //range.Value2 = dtM.Rows[0]["规格"].ToString();
                    range = ws.get_Range("D11", Type.Missing);
                    range.Value2 = dt.Rows[0]["规格型号"].ToString();
                    if (dr_产品序列号 != null)
                    {
                        range = ws.get_Range("N11", Type.Missing);
                        range.Value2 = dr_产品序列号["产品序列号"].ToString();
                        range = ws.get_Range("D15", Type.Missing);
                        range.Value2 = dtM.Rows[0]["大类"].ToString();
                        range = ws.get_Range("I15", Type.Missing);
                        range.Value2 = dtM.Rows[0]["小类"].ToString();
                        range = ws.get_Range("N15", Type.Missing);
                        range.Value2 = dtM.Rows[0]["生产者"].ToString();
                        range = ws.get_Range("S15", Type.Missing);
                        range.Value2 = dtM.Rows[0]["班组"].ToString();
                        range = ws.get_Range("D19", Type.Missing);
                        range.Value2 = dtM.Rows[0]["部门"].ToString();
                        range = ws.get_Range("I19", Type.Missing);
                        range.Value2 = Convert.ToDateTime(dtM.Rows[0]["生产日期"]);
                        range = ws.get_Range("N19", Type.Missing);
                        range.Value2 = dtM.Rows[0]["发货数量"].ToString();
                        range = ws.get_Range("S19", Type.Missing);
                        range.Value2 = dtM.Rows[0]["合格数"].ToString();

                    }
                    else
                    {

                        range = ws.get_Range("M11", Type.Missing);
                        range.Value2 = dtM.Rows[0]["大类"].ToString();

                        range = ws.get_Range("S11", Type.Missing);
                        range.Value2 = dtM.Rows[0]["小类"].ToString();
                        range = ws.get_Range("D15", Type.Missing);
                        range.Value2 = dtM.Rows[0]["生产者"].ToString();
                        range = ws.get_Range("H15", Type.Missing);
                        range.Value2 = dtM.Rows[0]["班组"].ToString();
                        range = ws.get_Range("M15", Type.Missing);
                        range.Value2 = dtM.Rows[0]["部门"].ToString();
                        range = ws.get_Range("S15", Type.Missing);
                        range.Value2 = Convert.ToDateTime(dtM.Rows[0]["生产日期"]);
                        range = ws.get_Range("D19", Type.Missing);
                        range.Value2 = dtM.Rows[0]["发货数量"].ToString();
                        range = ws.get_Range("H19", Type.Missing);
                        range.Value2 = dtM.Rows[0]["合格数"].ToString();
                    }

                    int count = dtP.Rows.Count;
                    if (count > 0)
                    {
                        if (count % 11 != 0)
                        {
                            count = (count / 11) + 1;
                        }
                        else
                        {
                            count = count / 11;
                        }
                        for (int j = 1; j < count; j++)
                        {
                            ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);
                        }



                        int pos1 = 1;  //记数 循环次数
                        int i_count = 11; // 每页打多少行
                        int pos = 0;
                        int i_first = 27;// 起始行 
                        foreach (System.Data.DataRow r in dtP.Rows)
                        {

                            //if ((pos + i_first) >= 70)
                            //{
                            //    break;
                            //}
                            range = ws.get_Range("A" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["序号"].ToString();
                            range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["检验项目"].ToString();
                            range = ws.get_Range("D" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["检验要求"].ToString();
                            range = ws.get_Range("L" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["检测水平"].ToString();
                            range = ws.get_Range("M" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["合格水平"].ToString();
                            range = ws.get_Range("N" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["a"].ToString();
                            range = ws.get_Range("O" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["b"].ToString();
                            range = ws.get_Range("P" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["c"].ToString();
                            range = ws.get_Range("Q" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["d"].ToString();
                            range = ws.get_Range("R" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["e"].ToString();
                            range = ws.get_Range("S" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["f"].ToString();
                            range = ws.get_Range("T" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["g"].ToString();
                            range = ws.get_Range("U" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["h"].ToString();

                            i++;
                            pos = pos + 4;

                            if (pos1 % i_count == 0)
                            {

                                ws = (Worksheet)wb.Worksheets.get_Item(pos1 / i_count + 1);
                                ws.Name = "sheet-" + (pos1 / i_count + 1).ToString();
                                i_first = 27;
                                pos = 0;
                            }
                            pos1 = pos1 + 1;
                        }

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
                    excelApp.ScreenUpdating = false;
                    //BringWindowToTop(hwnd);
                    wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    excelApp.DisplayAlerts = false;
                    //excelApp.Quit();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
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
        /// <summary>
        /// 不用 
        /// </summary>
        /// <param name="dtP"></param>
        /// <param name="i"></param>
        /// <param name="str"></param>
        /// <param name="blPreview"></param>
        public static void fun_print_TEST(System.Data.DataTable dtP, int i, string str, bool blPreview = false)
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
                    string s = string.Format("select * from 基础记录打印模板表 where 模板名 = '{0}'", str);
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

                foreach (System.Data.DataRow r in dtP.Rows)
                {

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

        public static void fun_print_财务领料单(string str_编号, bool blPreview = false)
        {
            string fName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp";
            if (Directory.Exists(fName) == false)
            {
                Directory.CreateDirectory(fName);
            }
            string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MasterCom\\Future\\prttmp\\" + Guid.NewGuid().ToString() + ".xlsx";
            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            try
            {
                {
                    System.Data.DataTable dtPP = new System.Data.DataTable();
                    string s = "select * from 基础记录打印模板表 where 模板名 = '财务领料单'";
                    new SqlDataAdapter(s, CPublic.Var.strConn).Fill(dtPP);
                    if (dtPP.Rows.Count == 0) return;
                    try
                    {
                        System.IO.Directory.Delete(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\prttmp", true);
                    }
                    catch
                    { }
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

                string sql = string.Format("select * from [临时用领料单表] where 领料编号 = '{0}'", str_编号);
                System.Data.DataTable dtP = new System.Data.DataTable();
                new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dtP);
                try
                {
                    if (dtP.Rows.Count > 0)
                    {
                        range = ws.get_Range("C2", Type.Missing);
                        range.Value2 = str_编号;
                        range = ws.get_Range("C4", Type.Missing);
                        range.Value2 = dtP.Rows[0]["日期"].ToString();
                        range = ws.get_Range("K4", Type.Missing);
                        range.Value2 = dtP.Rows[0]["领用部门"].ToString(); ;

                        int pos = 0;
                        int i_first = 9;
                        int i = 1;
                        foreach (System.Data.DataRow r in dtP.Rows)
                        {
                            if ((pos + i_first) >= 29)
                            {
                                break;
                            }
                            range = ws.get_Range("A" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = i.ToString();
                            range = ws.get_Range("C" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["物料编号"].ToString();
                            range = ws.get_Range("E" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["物料名称"].ToString();
                            range = ws.get_Range("I" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["规格"].ToString();
                            range = ws.get_Range("N" + (i_first + pos).ToString(), Type.Missing);
                            range.Value2 = r["数量"].ToString();

                            pos = pos + 2;
                        }
                    }
                }
                catch { }

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
                    // excelApp.Quit();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
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

        public static void fun_print_采购开票单(System.Data.DataTable dtP, System.Data.DataRow drM, bool blPreview = false)
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\送检清单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '送检清单'";
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



                range = ws.get_Range("D6", Type.Missing);
                range.Value2 = drM["开票通知单号"].ToString();
                range = ws.get_Range("D9", Type.Missing);
                range.Value2 = drM["总金额"].ToString();
                range = ws.get_Range("Y9", Type.Missing);
                range.Value2 = drM["总金额"].ToString();
                range = ws.get_Range("Y6", Type.Missing);
                range.Value2 = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                //供应商名称
                range = ws.get_Range("D13", Type.Missing);
                range.Value2 = drM["供应商名称"].ToString();
                //备注
                //range = ws.get_Range("Z" + (i_first + pos).ToString(), Type.Missing);
                //range.Value2 = r["备注"].ToString();

                int pos = 0;
                int i = 1;
                int i_first = 22;
                foreach (System.Data.DataRow r in dtP.Rows)
                {
                    if ((pos + i_first) >= 70)
                    {
                        break;
                    }
                    range = ws.get_Range("A" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = (i).ToString();
                    range = ws.get_Range("B" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["采购单号"].ToString();
                    range = ws.get_Range("G" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["物料名称"].ToString();
                    range = ws.get_Range("J" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["规格型号"].ToString(); //图纸编号

                    string sql = string.Format("select 计量单位 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"]);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    new SqlDataAdapter(sql, CPublic.Var.strConn).Fill(dt);
                    try
                    {
                        range = ws.get_Range("E" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = r["物料编码"].ToString();
                        range = ws.get_Range("P" + (i_first + pos).ToString(), Type.Missing);
                        range.Value2 = dt.Rows[0]["计量单位"].ToString();
                    }
                    catch { }

                    range = ws.get_Range("R" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["开票数量"].ToString();
                    range = ws.get_Range("T" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["单价"].ToString();
                    range = ws.get_Range("W" + (i_first + pos).ToString(), Type.Missing);
                    range.Value2 = r["金额"].ToString();
                    //range = ws.get_Range("Z" + (i_first + pos).ToString(), Type.Missing);
                    //range.Value2 = r["备注"].ToString();

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
                    //excelApp.Quit();


                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
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


        public static void fun_销售单(string str_销售单, string str_打印机)
        {
            string sql = string.Format(@"select a.销售订单号,b.物料编码,b.物料名称,b.规格型号,c.税率,b.计量单位,
        a.数量,a.税后单价,a.税后金额,a.送达日期,a.备注,c.客户订单号,c.客户名,c.创建日期,c.税后金额 as 总金额,b.产品线
               from  销售记录销售订单明细表 a,基础数据物料信息表 b ,销售记录销售订单主表 c  where 
             a.销售订单号=c.销售订单号 and b.物料编码=a.物料编码 and a.销售订单号='{0}'", str_销售单);

            System.Data.DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);

            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\销售单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '销售单'";
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

                range = ws.get_Range("H4", Type.Missing);
                range.Value2 = dt.Rows[0]["销售订单号"];
                range = ws.get_Range("C6", Type.Missing);
                range.Value2 = dt.Rows[0]["客户名"];
                range = ws.get_Range("H6", Type.Missing);
                range.Value2 = dt.Rows[0]["客户订单号"];
                range = ws.get_Range("H7", Type.Missing);
                range.Value2 = dt.Rows[0]["创建日期"];
                range = ws.get_Range("C7", Type.Missing);
                range.Value2 = dt.Rows[0]["税率"] + "%";

                range = ws.get_Range("G23", Type.Missing);
                range.Value2 = dt.Rows[0]["总金额"];

                int count = dt.Rows.Count / 14;
                if (count % 14 != 0)
                {
                    count = count + 1;
                }
                // */
                //复制 count-1 个 sheet
                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                }
                //int ir = 1;
                int pos = 1;  //记数 循环次数
                int i_first = 9;      // 起始行 
                int i_count = 14; // 每页打多少条
                int i_第几张 = 1;

                foreach (System.Data.DataRow r in dt.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    //range.Value2 = ir++.ToString();  //序号

                    range.Value2 = r["物料编码"].ToString();

                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["物料名称"].ToString();

                    //if (r["产品线"].ToString() == "智能终端电器")
                    //{
                    //    range = ws.get_Range("D" + i_first.ToString(), Type.Missing);

                    //    range.Value2 = r["规格"].ToString();
                    //}
                    //else
                    //{
                    range = ws.get_Range("D" + i_first.ToString(), Type.Missing);

                    range.Value2 = r["规格型号"].ToString();
                    //}

                    range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["计量单位"].ToString();

                    range = ws.get_Range("F" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["数量"].ToString();

                    range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["税后金额"].ToString();
                    range = ws.get_Range("G" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["税后单价"].ToString();

                    range = ws.get_Range("I" + i_first.ToString(), Type.Missing);
                    range.Value2 = Convert.ToDateTime(r["送达日期"]).ToString("yyyy-MM-dd");

                    range = ws.get_Range("J" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["备注"].ToString();
                    //超过十七条 换下一个sheet
                    if (pos % i_count == 0 && count != i_第几张)
                    {
                        i_第几张++;

                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 8;

                    }
                    i_first++;
                    pos++;
                }



                //excelApp.Visible = false;
                //BringWindowToTop(hwnd);
                wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                excelApp.DisplayAlerts = false;

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();

            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pic_1">审核人</param>
        /// <param name="pic_2">申请人</param>
        public static void fun_signPO(string fileName, string pic_1, string pic_2)
        {

            ApplicationClass excelApp = new ApplicationClass();

            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            try
            {

                Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                            Type.Missing, Type.Missing);

                excelApp.Visible = false;
                excelApp.ScreenUpdating = false;
                excelApp.DisplayAlerts = false;

                int count = wb.Worksheets.Count;

                for (int i = 1; i <= count; i++)
                {
                    Worksheet ws = (Worksheet)wb.Worksheets[i];

                    //Microsoft.Office.Interop.Excel.Range range;
                    ws.Shapes.AddPicture(pic_1, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 400, 902, 75, 46);
                    ws.Shapes.AddPicture(pic_2, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 100, 902, 75, 46);

                }

                wb.SaveAs(fileName);

                excelApp.Visible = false;
                BringWindowToTop(hwnd);
                excelApp.DisplayAlerts = false;
                //excelApp.Quit();
                //GcCollect();
                //System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

            finally
            {
                if (PID != IntPtr.Zero)
                {
                    excelApp = null;
                    GcCollect();
                    KillProcess(PID);
                    Kill(excelApp);
                }


            }
        }


        /// <summary>
        ///  
        ///  
        /// </summary>
        /// <param name="str_采购单"></param>
        /// <param name="str_打印机"></param>
        public static void fun_采购审核单(string str_采购单, string path, string pic_1, string pic_2)
        {
            string sql = string.Format(@"select 采购记录采购单主表.*,供应商传真 from 采购记录采购单主表,采购供应商表 
              where  采购记录采购单主表.供应商ID=采购供应商表.供应商ID and 采购单号='{0}'", str_采购单);
            System.Data.DataRow dr = CZMaster.MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
            sql = string.Format(@"select 采购记录采购单明细表.* from 采购记录采购单明细表 
                                    where  采购单号='{0}' order by 物料编码", str_采购单);
            System.Data.DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\采购单.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '采购单'";
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

                range = ws.get_Range("H4", Type.Missing);
                range.Value2 = dr["采购单号"];
                range = ws.get_Range("C6", Type.Missing);
                range.Value2 = dr["供应商"];
                range = ws.get_Range("H6", Type.Missing);
                range.Value2 = dr["供应商电话"];
                range = ws.get_Range("H7", Type.Missing);
                range.Value2 = dr["供应商传真"];
                range = ws.get_Range("C7", Type.Missing);
                range.Value2 = Convert.ToDateTime(dr["采购计划日期"]).ToString("yyyy-MM-dd");
                range = ws.get_Range("C8", Type.Missing);
                range.Value2 = dr["税率"] + "%";
                range = ws.get_Range("H26", Type.Missing);
                range.Value2 = dr["总金额"];

                int count = dt.Rows.Count / 14;
                if (count % 14 != 0)
                {
                    count = count + 1;
                }
                // */
                //复制 count-1 个 sheet
                ws.Shapes.AddPicture(pic_1, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 400, 902, 75, 46);
                ws.Shapes.AddPicture(pic_2, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 100, 902, 75, 46);
                for (int i = 1; i < count; i++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(i)).Copy(Type.Missing, wb.Worksheets[i]);
                    ((Worksheet)wb.Worksheets[i + 1]).Shapes.AddPicture(pic_1, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 400, 902, 75, 46);
                    ((Worksheet)wb.Worksheets[i + 1]).Shapes.AddPicture(pic_2, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 100, 902, 75, 46);
                }
                // int ir = 1;
                int pos = 1;  //记数 循环次数
                int i_first = 12;      // 起始行 
                int i_count = 14; // 每页打多少条
                int i_第几张 = 1;

                foreach (System.Data.DataRow r in dt.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    //range.Value2 = ir++.ToString();  //序号

                    range.Value2 = r["物料编码"].ToString();

                    range = ws.get_Range("C" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["物料名称"].ToString();


                    range = ws.get_Range("D" + i_first.ToString(), Type.Missing);

                    range.Value2 = r["图纸编号"].ToString();

                    range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["数量单位"].ToString();

                    range = ws.get_Range("F" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["采购数量"].ToString();

                    range = ws.get_Range("H" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["金额"].ToString();

                    range = ws.get_Range("G" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["单价"].ToString();

                    range = ws.get_Range("I" + i_first.ToString(), Type.Missing);
                    range.Value2 = Convert.ToDateTime(r["到货日期"]).ToString("yyyy-MM-dd");
                    range = ws.get_Range("J" + i_first.ToString(), Type.Missing);
                    range.Value2 = r["未税单价"].ToString();
                    //超过十七条 换下一个sheet
                    if (pos % i_count == 0 && count != i_第几张)
                    {
                        i_第几张++;

                        ws = (Worksheet)wb.Worksheets.get_Item(pos / i_count + 1);
                        ws.Name = "sheet-" + (pos / i_count + 1).ToString();
                        i_first = 11;

                    }
                    i_first++;
                    pos++;
                }




                range = ws.get_Range("B47", Type.Missing);
                range.Value2 = Convert.ToDateTime(dr["修改日期"]).ToString("yyyy-MM-dd");
                range = ws.get_Range("F47", Type.Missing);
                range.Value2 = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                int xx = wb.Worksheets.Count;
                //for (int i = 1; i <= count; i++)
                //{
                //    Worksheet ws1 = (Worksheet)wb.Worksheets[i];

                //    ws1.Shapes.AddPicture(pic_1, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 400, 902, 75, 46);
                //    ws1.Shapes.AddPicture(pic_2, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 100, 902, 75, 46);

                //}

                wb.SaveAs(path);


                BringWindowToTop(hwnd);



                excelApp.DisplayAlerts = false;
                excelApp.Visible = false;
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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



        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        public static void Kill(ApplicationClass excel)
        {
            IntPtr t = new IntPtr(excel.Hwnd); //得到这个句柄，具体作用是得到 这块内存入口23.24.
            int k = 0; GetWindowThreadProcessId(t, out k); //得到本进程唯一标志k 26. 
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);//得到对进程k的引用27
            p.Kill(); //关闭进程k
        }


        //    public static void fun_修改日志打印(System.Data.DataTable dt_修改日志, string p1, bool p2)
        //    {
        //         int count = dt_修改日志.Rows.Count / 8;
        //        if (dt_修改日志.Rows.Count % 8 != 0)
        //        {
        //            count += 1;
        //        }
        //        string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\固定资产修改日志.xlsx";
        //        ApplicationClass excelApp = new ApplicationClass();

        //        IntPtr hwnd = new IntPtr(excelApp.Hwnd);
        //        IntPtr PID = IntPtr.Zero;
        //        GetWindowThreadProcessId(hwnd, out PID);
        //        try
        //        {
        //            Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing);
        //            excelApp.Visible = false;
        //            excelApp.ScreenUpdating = false;
        //            excelApp.DisplayAlerts = false;
        //            Worksheet ws = (Worksheet)wb.Worksheets[1];   //获取第一个工作表

        //            Microsoft.Office.Interop.Excel.Range range;
        //            range = ws.get_Range("C4", Type.Missing);
        //            range.Value2 = code;   //资产编码
        //            range = ws.get_Range("C5", Type.Missing);
        //            range.Value2 = name;   //资产名称
        //            for (int j = 1; j < count; j++)
        //            {
        //                ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);

        //            }

        //            int pos = 1;
        //            int i = 1;
        //            int i_first = 7;
        //            int i_第几张 = 1;
        //            foreach (System.Data.DataRow dr in dt_修改日志.Rows)
        //            {
        //                range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
        //                range.Value2 = i.ToString();
        //                range = ws.get_Range("B" + i_first.ToString(), Type.Missing);
        //                range.Value2 = dr["修改人"].ToString();
        //                range = ws.get_Range("D" + i_first.ToString(), Type.Missing);
        //                range.Value2 = dr["修改人ID"].ToString();
        //                range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
        //                range.Value2 = dr["修改内容"].ToString();
        //                range = ws.get_Range("J" + i_first.ToString(), Type.Missing);
        //                range.Value2 = Convert.ToDateTime(dr["修改日期"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
        //                if (pos % 8 == 0 && count != i_第几张)
        //                {
        //                    i_第几张++;

        //                    ws = (Worksheet)wb.Worksheets.get_Item(pos / 8 + 1);
        //                    ws.Name = "sheet-" + (pos / 8 + 1).ToString();
        //                    i_first = 6;
        //                }
        //                i_first++;
        //                pos++;
        //                i++;
        //            }
        //            if (blPreview)
        //            {
        //                excelApp.Visible = true;
        //                wb.PrintPreview();
        //            }
        //            else
        //            {
        //                excelApp.Visible = false;
        //                BringWindowToTop(hwnd);

        //                if (str == "")
        //                {
        //                    wb.PrintOutEx();
        //                }
        //                else
        //                {
        //                    wb.SaveAs(str);
        //                }
        //                excelApp.Quit();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            if (PID != IntPtr.Zero)
        //            {
        //                excelApp = null;
        //                GcCollect();
        //                KillProcess(PID);
        //                //System.IO.File.Delete(fileName);
        //            }
        //    }
        //}



        public static void fun_修改日志打印(System.Data.DataTable dt_修改日志, string code, string name, string p1, bool blPreview, string str = "")
        {

            int count = dt_修改日志.Rows.Count / 8;
            if (dt_修改日志.Rows.Count % 8 != 0)
            {
                count += 1;
            }
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\固定资产修改日志.xlsx";
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
                Worksheet ws = (Worksheet)wb.Worksheets[1];   //获取第一个工作表

                Microsoft.Office.Interop.Excel.Range range;
                range = ws.get_Range("C4", Type.Missing);
                range.Value2 = code;   //资产编码
                range = ws.get_Range("C5", Type.Missing);
                range.Value2 = name;   //资产名称
                for (int j = 1; j < count; j++)
                {
                    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);

                }

                int pos = 1;
                int i = 1;
                int i_first = 7;
                int i_第几张 = 1;
                foreach (System.Data.DataRow dr in dt_修改日志.Rows)
                {
                    range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
                    range.Value2 = i.ToString();
                    range = ws.get_Range("B" + i_first.ToString(), Type.Missing);
                    range.Value2 = dr["修改人"].ToString();
                    range = ws.get_Range("D" + i_first.ToString(), Type.Missing);
                    range.Value2 = dr["修改人ID"].ToString();
                    range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
                    range.Value2 = dr["修改内容"].ToString();
                    range = ws.get_Range("J" + i_first.ToString(), Type.Missing);
                    range.Value2 = Convert.ToDateTime(dr["修改日期"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    if (pos % 8 == 0 && count != i_第几张)
                    {
                        i_第几张++;

                        ws = (Worksheet)wb.Worksheets.get_Item(pos / 8 + 1);
                        ws.Name = "sheet-" + (pos / 8 + 1).ToString();
                        i_first = 6;
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

                    if (str == "")
                    {
                        wb.PrintOutEx();
                    }
                    else
                    {
                        wb.SaveAs(str);
                    }
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

        //public static void fun_固定资产打印(System.Data.DataTable dt_固定资产, string p1, bool p2)
        //{
        //    //string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\采购单.xlsx";
        //    string fileName = @"C:\Users\djl\Desktop\固定资产信息表1.xlsx";
        //    ApplicationClass excelApp = new ApplicationClass();

        //    IntPtr hwnd = new IntPtr(excelApp.Hwnd);
        //    IntPtr PID = IntPtr.Zero;
        //    GetWindowThreadProcessId(hwnd, out PID);
        //    try
        //    {

        //        Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                                    Type.Missing, Type.Missing);
        //        excelApp.Visible = false;
        //        excelApp.ScreenUpdating = false;
        //        excelApp.DisplayAlerts = false;
        //        Worksheet ws = (Worksheet)wb.Worksheets[1];

        //        int i_first = 2;
        //        int i_第几张 = 1;

        //        Microsoft.Office.Interop.Excel.Range range;
        //        //string sql_固定资产 = "select * from 固定资产表";
        //        //System.Data.DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql_固定资产, CPublic.Var.strConn);
        //        foreach (System.Data.DataRow dr in dt_固定资产.Rows)
        //        {
        //            range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
        //            range.Value2 = i.ToString("GUID");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        excelApp = null;
        //        GcCollect();
        //        KillProcess(PID);
        //        CZMaster.MasterLog.WriteLog(ex.Message);
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



        public static void fun_P_知识平台打印(System.Data.DataRow drM, string str_打印机, bool blPreview = false)
        {

            string path = System.Windows.Forms.Application.StartupPath + @"\prttmp";
            if (!System.IO.File.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\知识平台模板.xlsx";
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '知识平台模板'";
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
                //for (int j = 1; j < count; j++)
                //{
                //    ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);

                //}
                range = ws.get_Range("B3", Type.Missing);
                range.Value2 = drM["售后单号"].ToString();
                range = ws.get_Range("I3", Type.Missing);
                range.Value2 = drM["录入时间"].ToString();
                range = ws.get_Range("B4", Type.Missing);
                range.Value2 = drM["产品名称"].ToString();
                range = ws.get_Range("E4", Type.Missing);
                range.Value2 = drM["产品编码"].ToString();
                range = ws.get_Range("H4", Type.Missing);
                range.Value2 = drM["服务类型"].ToString();
                range = ws.get_Range("B5", Type.Missing);
                range.Value2 = drM["产品型号"].ToString();
                range = ws.get_Range("H5", Type.Missing);
                range.Value2 = drM["原因分类"].ToString();
                range = ws.get_Range("B6", Type.Missing);
                range.Value2 = drM["信息来员"].ToString();
                range = ws.get_Range("A8", Type.Missing);
                range.Value2 = drM["状况描述"].ToString();
                range = ws.get_Range("A17", Type.Missing);
                range.Value2 = drM["不良反应"].ToString();
                range = ws.get_Range("A26", Type.Missing);
                range.Value2 = drM["改善方法"].ToString();
                range = ws.get_Range("A35", Type.Missing);
                range.Value2 = drM["变更点"].ToString();
                excelApp.DisplayAlerts = false;
                wb.PrintOutEx(Type.Missing, Type.Missing, Type.Missing, Type.Missing, str_打印机, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                excelApp.Quit();

            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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



        public static System.Data.DataTable ExcelToDatatable(string path)
        {
            string[] s = new string[50];

            ApplicationClass excelApp = new ApplicationClass();
            IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            IntPtr PID = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out PID);
            Workbook wb = excelApp.Workbooks.Open(path, Type.Missing, Type.Missing,
                                           Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                           Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                           Type.Missing, Type.Missing);
            System.Data.DataTable dt = new System.Data.DataTable();
            //for (int k = 1; k <= workbook.Worksheets.Count; k++)
            //{
            //worksheet = (Worksheet)workbook.Worksheets[k];
            try
            {


                Worksheet worksheet = (Worksheet)wb.Worksheets[1];
                int rowCount = worksheet.UsedRange.Rows.Count;
                int colCount = worksheet.UsedRange.Columns.Count;
                //s = new string[colCount];
                Microsoft.Office.Interop.Excel.Range range1;
                for (int i = 0; i < colCount; i++)  //按照第一行 添加表结构 
                {
                    Microsoft.Office.Interop.Excel.Range range;
                    range = worksheet.Range[worksheet.Cells[1, i + 1], worksheet.Cells[1, i + 1]];
                    if (range.Value2 == null || range.Value2.ToString() == "")
                        continue;
                    dt.Columns.Add(range.Value2.ToString());
                }
                for (int j = 2; j <= rowCount; j++)//rowCount
                {

                    System.Data.DataRow dr = dt.NewRow();
                    Microsoft.Office.Interop.Excel.Range range;
                    range = worksheet.Range[worksheet.Cells[j, 1], worksheet.Cells[j, 1]];
                    for (int i = 0; i < colCount; i++)
                    {
                        range1 = worksheet.Range[worksheet.Cells[j, i + 1], worksheet.Cells[j, i + 1]];
                        //if (range1.NumberFormatLocal.ToString() == "yyyy-m-d" || range1.NumberFormatLocal.ToString() == "yyyy/m/d")
                        //{
                        //    dr[i] = range1.Cells.Text;
                        //    continue;
                        //}

                        if (range1.Value2 != null)
                        {
                            dr[i] = range1.Text.ToString().Trim();
                        }
                    }
                    dt.Rows.Add(dr);
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
            }
            catch (Exception ex)
            {
                excelApp = null;
                GcCollect();
                KillProcess(PID);
                CZMaster.MasterLog.WriteLog(ex.Message);
                //throw ex;

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
            return dt;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        public static void npoi_export财务盘点(string filepath, System.Data.DataTable dt)
        {
            int row_index = 0;
            NPOI.SS.UserModel.IWorkbook workbook;
            string fileExt = Path.GetExtension(filepath).ToLower();
            if (fileExt == ".xlsx") { workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
            if (workbook == null) { return; }
            NPOI.SS.UserModel.ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);

            NPOI.SS.UserModel.IRow header = sheet.CreateRow(row_index++);

            NPOI.SS.UserModel.ICell cell_head = header.CreateCell(0);
            cell_head.SetCellValue("盘点记录表");
            NPOI.SS.UserModel.ICellStyle style = workbook.CreateCellStyle();
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            //设置单元格的样式：水平对齐居中
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            //新建一个字体样式对象
            NPOI.SS.UserModel.IFont font = workbook.CreateFont();
            //设置字体加粗样式
            font.Boldweight = short.MaxValue;
            font.FontHeightInPoints = 16;
            //使用SetFont方法将字体样式添加到单元格样式中 
            style.SetFont(font);
            //将新的样式赋给单元格
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dt.Columns.Count - 1));

            cell_head.CellStyle = style;


            NPOI.SS.UserModel.ICellStyle style_3 = workbook.CreateCellStyle();
            style_3.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style_3.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style_3.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style_3.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


            //添加列名 
            NPOI.SS.UserModel.IRow row = sheet.CreateRow(row_index++);
            //row.RowStyle = style_3;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                NPOI.SS.UserModel.ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
                cell.CellStyle = style_3;
            }


            //数据  
            for (int i = row_index - 1; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                    cell.CellStyle = style_3;
                }
                // row1.RowStyle = style_3;
            }
            NPOI.SS.UserModel.IRow foot = sheet.CreateRow(dt.Rows.Count + 1);

            NPOI.SS.UserModel.ICell foot_c2 = foot.CreateCell(13);
            foot_c2.SetCellValue("监盘人:");


            NPOI.SS.UserModel.ICellStyle style_1 = workbook.CreateCellStyle();
            //设置单元格的样式：水平对齐居中
            style_1.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            //新建一个字体样式对象
            NPOI.SS.UserModel.IFont font_1 = workbook.CreateFont();
            //设置字体加粗样式
            font_1.Boldweight = short.MaxValue;
            font_1.FontHeightInPoints = 12;
            //使用SetFont方法将字体样式添加到单元格样式中 
            style_1.SetFont(font_1);
            //将新的样式赋给单元格

            foot_c2.CellStyle = style_1;
            //自适应列宽
            for (int columnNum = 0; columnNum <= dt.Columns.Count; columnNum++)
            {
                int columnWidth = sheet.GetColumnWidth(columnNum) / 256;
                for (int rowNum = 1; rowNum <= sheet.LastRowNum; rowNum++)
                {
                    NPOI.SS.UserModel.IRow currentRow;
                    //当前行未被使用过
                    if (sheet.GetRow(rowNum) == null)
                    {
                        currentRow = sheet.CreateRow(rowNum);
                    }
                    else
                    {
                        currentRow = sheet.GetRow(rowNum);
                    }

                    if (currentRow.GetCell(columnNum) != null)
                    {
                        NPOI.SS.UserModel.ICell currentCell = currentRow.GetCell(columnNum);
                        int length = System.Text.Encoding.Default.GetBytes(currentCell.ToString()).Length;
                        if (columnWidth < length)
                        {
                            columnWidth = length;
                        }
                    }
                }
                sheet.SetColumnWidth(columnNum, columnWidth * 256);
            }

            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 19-10-30 扫描盒贴 n张后 打印箱贴
        /// </summary>
        /// <param name="dr_传"> </param>
        /// <param name="pt_c">箱装数：一个箱子实际装了多少个</param>
        /// <param name="blPreview"></param>
        /// <param name="str_prt">打印机名称</param>
        public static void print_箱贴(string path_模板路径,string str_箱装号,int i_箱次,int i_总箱数,string kh,System.Data.DataTable dt_条码列表, bool blPreview = false, string str_prt = "")
        {
           // string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\10-30箱贴模板_A4.xlsx";
            string fileName = path_模板路径;
            if (System.IO.File.Exists(fileName).Equals(false))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                fs.Close();
                System.Data.DataTable dtPP = new System.Data.DataTable();
                string s = "select * from 基础记录打印模板表 where 模板名 = '11-28箱贴模板_A4'";
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


              
                try
                {
                    range = ws.get_Range("D2", Type.Missing);
                    range.Value2 = kh;

                    range = ws.get_Range("D4", Type.Missing);
                   // range.Value2 =  $" {str_箱装号}  第{i_箱次}箱/共{i_总箱数}箱";
                    range.Value2 = $" {str_箱装号}  CNT:{i_箱次}/{i_总箱数}";
                    //range = ws.get_Range("D6", Type.Missing);
                    //range.Value2 = dt_条码列表.Rows.Count;


                    int count = dt_条码列表.Rows.Count;
                    //int i_first = 14;
                    int i_first = 1;

                    if (count > 0)
                    {
                        foreach (System.Data.DataRow dr in dt_条码列表.Rows)
                        {
                            range = ws.get_Range("Y" + (i_first++).ToString(), Type.Missing);
                            range.Value2 = dr["CTNo1"].ToString();

                        }
                    }
                }
                catch (Exception ex)
                {
                    excelApp = null;
                    GcCollect();
                    KillProcess(PID);
                    throw ex;
                }

                if (blPreview)
                {
                    excelApp.Visible = true;
                    wb.PrintPreview();
                }
                else
                {
                    excelApp.Visible = false;

             

                    excelApp.ScreenUpdating = false;

                    excelApp.DisplayAlerts = false;
                    if (str_prt != "")
                    {
                        wb.PrintOutEx(1, Type.Missing, 1, false, str_prt, false, false, Type.Missing, false);
                    }
                    else
                    {
                        wb.PrintOutEx(1, Type.Missing, 1, false, Type.Missing, false, false, Type.Missing, false);
                    }
                    excelApp.Quit();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)ws);

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)wb);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject((object)excelApp);
                System.GC.Collect();
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
