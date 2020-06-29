using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;

using System.Windows.Forms;
using System.Data.SqlClient;

using System.Runtime.InteropServices;
using MoldMangement;
using LabelManager2;


namespace MoldMangement
{
    public partial class modifiedLog : Form
    {
        string strConn = CPublic.Var.strConn;
        string code = "";  //资产编码
        string name = "";   //资产名称
        System.Data.DataTable dt_修改日志;
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

        public modifiedLog()        
        {
            InitializeComponent();
            //strConn = string.Format(strConn, "a", "sa", "FMS", "HMK");
        }

        public modifiedLog(string assetCode,string assetName)
        {
            InitializeComponent();
            //strConn = string.Format(strConn, "a", "sa", "FMS", "HMK");
            code = assetCode;
            name = assetName;
            label2.Text = code + "-" + name;
        }

        

        private void modifiedLog_Load(object sender, EventArgs e)
        {
            dt_修改日志 = new System.Data.DataTable();
            string sql_修改日志 = string.Format("select * from 固定资产信息修改日志表 where 资产编码 = '{0}' order by 修改日期",code);
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter(sql_修改日志, strConn))
                {
                    da.Fill(dt_修改日志);
                }
                gc.DataSource = dt_修改日志;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //关闭
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        //显示行号
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        //点击打印按钮
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (printDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    print_ModifiedRecord(dt_修改日志, code, name);
            //}
            try
            {
                gv.CloseEditor();
                this.BindingContext[dt_修改日志].EndCurrentEdit();
                string str = "";
                string str_打印机;
                PrintDialog printDialog1 = new System.Windows.Forms.PrintDialog();
                this.printDialog1.Document = this.printDocument1;
                DialogResult drt = this.printDialog1.ShowDialog();
                if (drt == DialogResult.OK)
                {
                    ItemInspection.print_FMS.fun_修改日志打印(dt_修改日志,code,name, printDialog1.PrinterSettings.PrinterName, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //打印修改日志
        private void print_ModifiedRecord(System.Data.DataTable dtP, string code, string name, bool blPreview = false, string str = "") 
        {
            //int count = dtP.Rows.Count / 8;
            //if (dtP.Rows.Count % 8 != 0)
            //{
            //    count += 1;
            //}
            //string fileName = System.Windows.Forms.Application.StartupPath + @"\prttmp\固定资产修改日志.xlsx";
            //ApplicationClass excelApp = new ApplicationClass();

            //IntPtr hwnd = new IntPtr(excelApp.Hwnd);
            //IntPtr PID = IntPtr.Zero;
            //GetWindowThreadProcessId(hwnd, out PID);
            //try
            //{
            //    Workbook wb = excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
            //                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            //                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            //                            Type.Missing, Type.Missing);
            //    excelApp.Visible = false;
            //    excelApp.ScreenUpdating = false;
            //    excelApp.DisplayAlerts = false;
            //    Worksheet ws = (Worksheet)wb.Worksheets[1];   //获取第一个工作表

            //    Microsoft.Office.Interop.Excel.Range range;
            //    range = ws.get_Range("C4", Type.Missing);
            //    range.Value2 = code;   //资产编码
            //    range = ws.get_Range("C5", Type.Missing);
            //    range.Value2 = name;   //资产名称
            //    for (int j = 1; j < count; j++)
            //    {
            //        ((Microsoft.Office.Interop.Excel._Worksheet)wb.Worksheets.get_Item(j)).Copy(Type.Missing, wb.Worksheets[j]);

            //    }

            //    int pos = 1;
            //    int i = 1;
            //    int i_first = 7;
            //    int i_第几张 = 1;
            //    foreach (System.Data.DataRow dr in dtP.Rows)
            //    {
            //        range = ws.get_Range("A" + i_first.ToString(), Type.Missing);
            //        range.Value2 = i.ToString();
            //        range = ws.get_Range("B" + i_first.ToString(), Type.Missing);
            //        range.Value2 = dr["修改人"].ToString();
            //        range = ws.get_Range("D" + i_first.ToString(), Type.Missing);
            //        range.Value2 = dr["修改人ID"].ToString();
            //        range = ws.get_Range("E" + i_first.ToString(), Type.Missing);
            //        range.Value2 = dr["修改内容"].ToString();
            //        range = ws.get_Range("J" + i_first.ToString(), Type.Missing);
            //        range.Value2 = Convert.ToDateTime(dr["修改日期"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            //        if (pos % 8 == 0 && count != i_第几张)
            //        {
            //            i_第几张++;

            //            ws = (Worksheet)wb.Worksheets.get_Item(pos / 8 + 1);
            //            ws.Name = "sheet-" + (pos / 8 + 1).ToString();
            //            i_first = 6;
            //        }
            //        i_first++;
            //        pos++;
            //        i++;
            //    }
            //    if (blPreview)
            //    {
            //        excelApp.Visible = true;
            //        wb.PrintPreview();
            //    }
            //    else
            //    {
            //        excelApp.Visible = false;
            //        BringWindowToTop(hwnd);

            //        if (str == "")
            //        {
            //            wb.PrintOutEx();
            //        }
            //        else
            //        {
            //            wb.SaveAs(str);
            //        }
            //        excelApp.Quit();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
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
    }
}
