using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Drawing.Printing;
using CZMaster;

namespace PLCView
{
    public partial class frm自动检测数据 : UserControl
    {

        #region 用户变量
        DataTable dtP, dtM;
        Dictionary<string, string> Dic_str = new Dictionary<string, string>();

        private static string PWD = "a";
        private static string UID = "sa1";
        private static string SQLSERVER = "192.168.10.7";
        private static string DATABASE = "自动检测数据";
        private static string strconn = string.Format("Password={0};Persist Security Info=True;User ID={1};Initial Catalog={2};Data Source={3};Pooling=true;Max Pool Size=40000;Min Pool Size=0", PWD, UID, DATABASE, SQLSERVER);

        #endregion

        #region 类自用

        public frm自动检测数据()
        {
            InitializeComponent();
        }
        private void frm自动检测数据_Load(object sender, EventArgs e)
        {
            try
            {
                txt_SN.EditValue = "";
                fun_fill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion



        #region 数据库操作

        private void fun_fill()
        {
            dtEdit1.EditValue = Convert.ToDateTime(System.DateTime.Today).AddSeconds(-1);
            dtEdit2.EditValue = System.DateTime.Today.AddDays(1).AddSeconds(-1);

            string sql = "select [检测名称] FROM [ABB检测类型主表]";
            dtM = MasterSQL.Get_DataTable(sql, strconn);

            repositoryItemSearchLookUpEdit1.DataSource = dtM;
            repositoryItemSearchLookUpEdit1.ValueMember = "检测名称";
            repositoryItemSearchLookUpEdit1.DisplayMember = "检测名称";

            if (dtM.Rows.Count==0)
            {
                throw new Exception("检测类型数据为空！");
            }
            slueCheck.EditValue = dtM.Rows[0]["检测名称"].ToString();
            gvM.ViewCaption = "自动检测数据统计表";

            Dic_str.Add("检测项目", "");
            Dic_str.Add("日期", "");
            Dic_str.Add("总数量", "");
            Dic_str.Add("不合格数量", "");
            Dic_str.Add("合格数量", "");
            Dic_str.Add("合格率", "");
            Dic_str.Add("F", "");
            Dic_str.Add("SN号", "");
        }

        private void fun_刷新数据()
        {
            //string sql = string.Format("select * from ABB检测结果总表 where (结束检测时间 >= '{0}' and 结束检测时间 <= '{1}') and 检测标准='{2}' and [检测是否通过]<>'放弃' and [检测是否通过] <>'未知' and [检测是否通过]<>'' order by ID",
            //                                            ((DateTime)dtEdit1.EditValue).ToString("yyyy-MM-dd HH:mm:ss"),
            //                                            ((DateTime)dtEdit2.EditValue).ToString("yyyy-MM-dd HH:mm:ss"),
            //                                            slueCheck.EditValue.ToString());
            //string sql = string.Format("select * from ABB检测结果总表 where (结束检测时间 between  '{0}' and '{1}') and 检测标准='{2}' and [检测是否通过]<>'放弃' and [检测是否通过] <>'未知' and [检测是否通过]<>'' order by ID",
            //                                            ((DateTime)dtEdit1.EditValue).ToString("yyyy/MM/dd HH:mm:ss"),
            //                                            ((DateTime)dtEdit2.EditValue).ToString("yyyy/MM/dd HH:mm:ss"),
            //                                            slueCheck.EditValue.ToString());

            //string sql = string.Format("select * from ABB检测结果总表 where 检测标准='{0}' and [检测是否通过]<>'放弃' and [检测是否通过] <>'未知' and [检测是否通过]<>'' order by 出错检测组POS,出错主动作POS ",
            //                                            slueCheck.EditValue.ToString());
            string sql="";
            if (txt_SN.EditValue.ToString() != "")
            {
                 sql = string.Format("select * from ABB检测结果总表 where 产品SN号 like '{0}%' and  检测标准='{1}' and 检测是否通过<>'放弃' and 检测是否通过<>'未知' and 检测是否通过 <>''", txt_SN.EditValue.ToString(), slueCheck.EditValue.ToString());
                 Dic_str["F"] = "A";
            }
            else
            {
                 sql = string.Format("select * from ABB检测结果总表 where  (结束检测时间 >= '{0}' and 结束检测时间 <= '{1}') and 检测标准='{2}' and [检测是否通过]<>'放弃' and [检测是否通过] <>'未知' and [检测是否通过]<>'' order by 出错检测组POS,出错主动作POS ",
                    ((DateTime)dtEdit1.EditValue).ToString("yyyy-MM-dd HH:mm:ss"),
                    ((DateTime)dtEdit2.EditValue).ToString("yyyy-MM-dd HH:mm:ss"),
                    slueCheck.EditValue.ToString());
                 Dic_str["F"] = "B";
            }
           


            DataTable dt_All = MasterSQL.Get_DataTable(sql, strconn);

            DataView dv_All = new DataView(dt_All);

            decimal Num_All = dv_All.Count;

            dv_All.RowFilter = "检测是否通过='NG'";
            decimal Num_No = dv_All.Count;
            dv_All.RowFilter = "";
            dv_All.RowFilter = "检测是否通过='PASS'";
            decimal Num_OK = dv_All.Count;

            decimal Num_Per = 0;
            try
            {
                Num_Per = decimal.Divide(Num_OK, Num_All);
            }
            catch { Num_Per = 0; }

            dv_All.RowFilter = "";
            dv_All.RowFilter = "检测是否通过='NG'";

            DataTable dt_NG = dv_All.ToTable();
            Dictionary<string, DataRow> Dic_Dic_F = new Dictionary<string, DataRow>();

            int num = 0;
            dt_NG.Columns.Add("数量", num.GetType());
            dtP = dt_NG.Clone();

            foreach (DataRow r in dt_NG.Rows)
            {
                string Dic_Key = r["出错检测组POS"].ToString().Trim() + r["出错检测要求"].ToString().Trim() + r["出错主动作POS"].ToString().Trim() + r["出错主动作说明"].ToString().Trim();
                r["数量"] = 1;
                if (Dic_Dic_F.ContainsKey(Dic_Key) == true)
                {
                    Dic_Dic_F[Dic_Key]["数量"] = Convert.ToInt32(Dic_Dic_F[Dic_Key]["数量"]) + 1;
                }
                if (Dic_Dic_F.ContainsKey(Dic_Key) == false)
                {
                    Dic_Dic_F.Add(Dic_Key, r);
                }
            }

            foreach (DataRow r in Dic_Dic_F.Values)
            {
                DataRow rr = dtP.NewRow();
                rr.ItemArray = r.ItemArray;
                dtP.Rows.Add(rr);
            }

            gcM.DataSource = dtP;
            ///标题显示
            {
                gvM.ViewCaption = string.Format("总数：{0},不合格数量：{1},合格数量：{2},百分比：{3}", Num_All.ToString(), Num_No.ToString(), Num_OK.ToString(), (Num_Per*100).ToString("0.00") + "%");
            }
            this.lblNum_All.Text = Num_All.ToString();
            this.lblNum_No.Text = Num_No.ToString();
            this.lblNum_OK.Text = Num_OK.ToString();
            this.lblNum_Per.Text = (Num_Per * 100).ToString("0.00") + "%";

            Dic_str["检测项目"] = slueCheck.EditValue.ToString();
            Dic_str["日期"] = ((DateTime)dtEdit1.EditValue).ToString("yyyy-MM-dd HH:mm:ss") + "--" +((DateTime)dtEdit2.EditValue).ToString("yyyy-MM-dd HH:mm:ss");
            Dic_str["总数量"] = Num_All.ToString();
            Dic_str["不合格数量"] = Num_No.ToString();
            Dic_str["合格数量"] = Num_OK.ToString();
            Dic_str["合格率"] = (Num_Per * 100).ToString("0.00") + "%";
            Dic_str["SN号"] = txt_SN.EditValue.ToString();
        }

        private void fun_刷新数据_复合表头()
        {
            //string sql = string.Format("select * from ABB检测结果总表 where (结束检测时间 >= '{0}' and 结束检测时间 <= '{1}') and 检测标准='{2}' and [检测是否通过]<>'放弃' and [检测是否通过] <>'未知' and [检测是否通过]<>'' order by ID",
            //                                            ((DateTime)dtEdit1.EditValue).ToString("yyyy-MM-dd HH:mm:ss"),
            //                                            ((DateTime)dtEdit2.EditValue).ToString("yyyy-MM-dd HH:mm:ss"),
            //                                            slueCheck.EditValue.ToString());
            //string sql = string.Format("select * from ABB检测结果总表 where (结束检测时间 between  '{0}' and '{1}') and 检测标准='{2}' and [检测是否通过]<>'放弃' and [检测是否通过] <>'未知' and [检测是否通过]<>'' order by ID",
            //                                            ((DateTime)dtEdit1.EditValue).ToString("yyyy/MM/dd HH:mm:ss"),
            //                                            ((DateTime)dtEdit2.EditValue).ToString("yyyy/MM/dd HH:mm:ss"),
            //                                            slueCheck.EditValue.ToString());

            string sql = string.Format("select * from ABB检测结果总表 where 检测标准='{0}' and [检测是否通过]<>'放弃' and [检测是否通过] <>'未知' and [检测是否通过]<>'' order by 出错检测组POS,出错主动作POS ",
                                                        slueCheck.EditValue.ToString());

            DataTable dt_All =MasterSQL.Get_DataTable(sql, strconn);

            DataView dv_All = new DataView(dt_All);

            decimal Num_All = dv_All.Count;

            dv_All.RowFilter = "检测是否通过='NG'";
            decimal Num_No = dv_All.Count;
            dv_All.RowFilter = "";
            dv_All.RowFilter = "检测是否通过='PASS'";
            decimal Num_OK = dv_All.Count;

            decimal Num_Per = 0;
            try
            {
                Num_Per = decimal.Divide(Num_OK, Num_All);
            }
            catch { Num_Per = 0; }

            dv_All.RowFilter = "";
            dv_All.RowFilter = "检测是否通过='NG'";

            DataTable dt_NG = dv_All.ToTable();
            Dictionary<string, DataRow> Dic_Dic_F = new Dictionary<string, DataRow>();

            int num = 0;
            dt_NG.Columns.Add("数量", num.GetType());
            //dtP = dt_NG.Clone();

            foreach (DataRow r in dt_NG.Rows)
            {
                string Dic_Key = r["出错检测组POS"].ToString().Trim() + r["出错检测要求"].ToString().Trim() + r["出错主动作POS"].ToString().Trim() + r["出错主动作说明"].ToString().Trim();
                r["数量"] = 1;
                if (Dic_Dic_F.ContainsKey(Dic_Key) == true)
                {
                    Dic_Dic_F[Dic_Key]["数量"] = Convert.ToInt32(Dic_Dic_F[Dic_Key]["数量"]) + 1;
                }
                if (Dic_Dic_F.ContainsKey(Dic_Key) == false)
                {
                    Dic_Dic_F.Add(Dic_Key, r);
                }
            }

            //foreach (DataRow r in Dic_Dic_F.Values)
            //{
            //    DataRow rr = dtP.NewRow();
            //    rr.ItemArray = r.ItemArray;
            //    dtP.Rows.Add(rr);
            //}

            //gcM.DataSource = dtP;
            ///标题显示
            {
                //gvM.ViewCaption = string.Format("总数：{0},不合格数量：{1},合格数量：{2},百分比：{3}", Num_All.ToString(), Num_No.ToString(), Num_OK.ToString(), (Num_Per*100).ToString("0.00") + "%");
            }
        }

        private void fun_刷新数据_合并单元格()
        {
         

            //string sql = string.Format("select * from ABB检测结果总表 where (结束检测时间 >= '{0}' and 结束检测时间 <= '{1}') and 检测标准='{2}' and [检测是否通过]<>'放弃' and [检测是否通过] <>'未知' and [检测是否通过]<>'' order by ID",
            //                                            ((DateTime)dtEdit1.EditValue).ToString("yyyy-MM-dd HH:mm:ss"),
            //                                            ((DateTime)dtEdit2.EditValue).ToString("yyyy-MM-dd HH:mm:ss"),
            //                                            slueCheck.EditValue.ToString());
            //string sql = string.Format("select * from ABB检测结果总表 where (结束检测时间 between  '{0}' and '{1}') and 检测标准='{2}' and [检测是否通过]<>'放弃' and [检测是否通过] <>'未知' and [检测是否通过]<>'' order by ID",
            //                                            ((DateTime)dtEdit1.EditValue).ToString("yyyy/MM/dd HH:mm:ss"),
            //                                            ((DateTime)dtEdit2.EditValue).ToString("yyyy/MM/dd HH:mm:ss"),
            //                                            slueCheck.EditValue.ToString());

            string sql = string.Format("select * from ABB检测结果总表 where 检测标准='{0}' and [检测是否通过]<>'放弃' and [检测是否通过] <>'未知' and [检测是否通过]<>'' order by 出错检测组POS,出错主动作POS ",
                                                        slueCheck.EditValue.ToString());

            DataTable dt = MasterSQL.Get_DataTable(sql, strconn);

            DataView dv = new DataView(dt);

            decimal Num_All = dv.Count;

            dv.RowFilter = "检测是否通过='NG'";
            decimal Num_No = dv.Count;
            dv.RowFilter = "";
            dv.RowFilter = "检测是否通过='PASS'";
            decimal Num_OK = dv.Count;

            decimal Num_Per = 0;
            try
            {
                Num_Per = decimal.Divide(Num_OK, Num_All);
            }
            catch { Num_Per = 0; }
            ///复合表头
            {

            }
            ///合并单元格
            {

            }

            ///标题显示
            {

            }
            //gcM.DataSource = dt;
        }
        #endregion



        #region 数据处理

        #endregion

        #region 界面相关
        /// <summary>
        /// 确定
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
        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                   // print_AutomaticDetectionData.fun_print_print_AutomaticDetectionData(dtP,Dic_str);
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
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                try
                {
                    FolderBrowserDialog dialog = new FolderBrowserDialog();
                    dialog.Description = "目标位置";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string foldPath = dialog.SelectedPath;

                        string fileName = string.Format("{0}\\自动检测数据统计表_{1}.xlsx", dialog.SelectedPath, System.DateTime.Today.ToLongDateString());

                        System.IO.Directory.CreateDirectory(foldPath);
                        if (dtP == null)
                        {
                            throw new Exception("数据不能为空！");
                        }
                        if (System.IO.File.Exists(fileName) == true)
                        {
                            if (MessageBox.Show("文件已存在是否覆盖", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                              //  print_AutomaticDetectionData.fun_print_print_AutomaticDetectionData_ToExcel(dtP,Dic_str, fileName, true);
                            }
                        }
                        if (System.IO.File.Exists(fileName) == false)
                        {
                          //  print_AutomaticDetectionData.fun_print_print_AutomaticDetectionData_ToExcel(dtP,Dic_str, fileName, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 确定(复合表头)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_刷新数据_复合表头();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 确定(合并单元格)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_刷新数据_合并单元格();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Excel视图导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
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
                        string fileName = string.Format("{0}\\自动检测数据统计表_{1}.xlsx", dialog.SelectedPath, System.DateTime.Today.ToLongDateString());

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Excel视图导出

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

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










    }
}
