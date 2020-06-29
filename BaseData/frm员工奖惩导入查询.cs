using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Threading;
using CZMaster;

namespace BaseData
{
    public partial class frm员工奖惩导入查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        DataTable dt_Excel导入 = null;
        DataTable dt_查询保存 = null;
        DataView dv = null;
        DataTable dt_新增录入 = null;
        DataTable dt_下拉框;
        DataTable dt_人员 = null;
        string strConn_FS = CPublic.Var.geConn("FS");

        public frm员工奖惩导入查询()
        {
            InitializeComponent();
        }

        private void frm员工培训奖惩导入查询_Load(object sender, EventArgs e)
        {
            try
            {
                //默认新增模式
                bar_导入.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_导入保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_文本.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                time_前.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_查询.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_导出.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_查询保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                check_奖励.Visible = false;
                check_惩戒.Visible = false; check_编辑.Visible = false;
                DateTime dtime = CPublic.Var.getDatetime();
                dtime = new DateTime(dtime.Year, dtime.Month, 1);
                time_前.EditValue = dtime;
                time_后.EditValue = new DateTime(dtime.Year, dtime.Month, dtime.Day);
                fun_载入空表();
                gv.OptionsBehavior.Editable = true;
                fun_载入级别类别();
                fun_员工();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.Caption == "类型")
                {
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    string str = dr["类型"].ToString();
                    rescb_奖惩大类.Items.Clear();
                    string sql = string.Format("select 属性字段2 from 基础数据基础属性表 where 属性类别 = '员工-{0}-内容' group by 属性字段2", str);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            rescb_奖惩大类.Items.Add(r["属性字段2"].ToString());
                        }
                    }
                }
                if (e.Column.Caption == "奖惩大类")
                {
                    rescb_内容.Items.Clear();
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    string str = dr["奖惩大类"].ToString();
                    string str2 = dr["类型"].ToString();
                    DataRow[] ds = dt_下拉框.Select(string.Format("属性字段2 = '{0}' and 属性类别 = '员工-{1}-内容'", str, str2));
                    if (ds.Length > 0)
                    {
                        foreach (DataRow r in ds)
                        {
                            rescb_内容.Items.Add(r["属性值"].ToString());
                        }
                    }
                }
                if (e.Column.Caption == "内容")
                {                  
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    string str = dr["内容"].ToString();
                    string str2 = dr["类型"].ToString();
                    DataRow[] ds = dt_下拉框.Select(string.Format("属性值 = '{0}' and 属性类别 = '员工-{1}-内容'", str, str2));
                    if (ds.Length > 0)
                    {
                        dr["级别"] = ds[0]["属性字段1"];
                    }
                    ds = dt_下拉框.Select(string.Format("属性值 = '{0}' and 属性类别 = '员工-{1}-级别'", dr["级别"], str2));
                    dr["类别"] = ds[0]["属性字段1"];
                    dr["备注"] = ds[0]["属性字段2"];
                }
                if (e.Column.Caption == "员工号")
                {
                    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                    DataRow[] ds = dt_人员.Select(string.Format("员工号 = '{0}'", dr["员工号"]));
                    dr["姓名"] = ds[0]["姓名"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 方法
        private void fun_载入空表()
        {
            string sql = "select * from 人事员工奖惩记录表 where 1<>1";
            dt_新增录入 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_新增录入);
            gc.DataSource = dt_新增录入;
        }

        private void fun_载入级别类别()
        {
            string sql = string.Format("select * from 基础数据基础属性表 where 属性类别 = '员工-奖励-级别' or 属性类别 = '员工-惩戒-级别' or 属性类别 = '员工-惩戒-内容' or 属性类别 = '员工-奖励-内容'");
            dt_下拉框 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_下拉框);
        }

        private void fun_员工()
        {
            string sql = "select 员工号,姓名 from 人事基础员工表 where 在职状态  = '在职'";
            dt_人员 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_人员);
            repositoryItemSearchLookUpEdit1.DataSource = dt_人员;
            repositoryItemSearchLookUpEdit1.DisplayMember = "员工号";
            repositoryItemSearchLookUpEdit1.ValueMember = "员工号";
        }

        private void fun_上传照片(string pathName)
        {
            try
            {
                FileInfo info = new FileInfo(pathName);
                long maxinfo = info.Length;
                if (maxinfo > 1024 * 1024 * 8 )
                {
                    throw new Exception("上传的照片不能超过1M，请重新选择上传！");
                }
                CFileTransmission.CFileClient.strCONN = strConn_FS;
                string strguid = CFileTransmission.CFileClient.sendFile(pathName);
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                dr["文件"] = strguid;
                //byte[] bs = System.IO.File.ReadAllBytes(pathName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Excel数据导入
        private List<String[]> ReadCsv(string filePathName)
        {
            List<String[]> ls = new List<String[]>();
            StreamReader fileReader = new StreamReader(filePathName, System.Text.Encoding.Default);
            string strLine = "";
            while (strLine != null)
            {
                strLine = fileReader.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                    ls.Add(strLine.Split(',')); //换成你实际的分隔符
                }
            }
            fileReader.Close();
            return ls;
        }

        /// <summary>
        ///  回收垃圾
        /// </summary>
        public void GcCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// 杀死进程
        /// </summary>
        private void KillProcess(IntPtr H)
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

        private void fun_处理文件()
        {
            Microsoft.Office.Interop.Excel.Application ExclApp = new Microsoft.Office.Interop.Excel.Application();// 初始化
            IntPtr PID = IntPtr.Zero;
            OpenFileDialog openpic = new OpenFileDialog();
            if (openpic.ShowDialog() == DialogResult.OK)
            {
                Object Nothing = Type.Missing;//由于COM组件很多值需要用Missing.Value代替             
                Microsoft.Office.Interop.Excel.Workbook ExclDoc = ExclApp.Workbooks.Open(openpic.FileName, Nothing, Nothing, Nothing,
                    Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing);//打开Excl工作薄   
                try
                {
                    Object format = Microsoft.Office.Interop.Excel.XlFileFormat.xlCSV;
                    ExclApp.DisplayAlerts = false;

                    int i = openpic.FileName.LastIndexOf(".");
                    string str = openpic.FileName.Substring(i + 1);
                    if (str == "xlsx")
                    {
                        ExclDoc.SaveAs(openpic.FileName.Replace("xlsx", "csv"), format, Nothing, Nothing, Nothing, Nothing,
                            Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Nothing, Nothing, Nothing, Nothing, Nothing);
                    }
                    else
                    {
                        ExclDoc.SaveAs(openpic.FileName.Replace("xls", "csv"), format, Nothing, Nothing, Nothing, Nothing,
                            Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Nothing, Nothing, Nothing, Nothing, Nothing);
                    }
                }
                catch (Exception ex) { }
                ExclApp.Quit();
                if (PID != IntPtr.Zero)
                {
                    ExclApp = null;
                    GcCollect();
                    KillProcess(PID);
                }
                
                string str_源 = openpic.FileName;
                string str_目的 = "";
                int ia = openpic.FileName.LastIndexOf(".");
                string stra = openpic.FileName.Substring(ia + 1);
                if (stra == "xlsx")
                {
                    str_源 = openpic.FileName.Replace("xlsx", "csv");
                    str_目的 = openpic.FileName.Replace("xlsx", "txt");
                }
                else
                {
                    str_源 = openpic.FileName.Replace("xls", "csv");
                    str_目的 = openpic.FileName.Replace("xls", "txt");
                }

                Thread.Sleep(1000);

                System.IO.File.Move(@str_源, @str_目的);
                
                List<String[]> ls = ReadCsv(str_目的);
                dt_Excel导入 = new DataTable();
                foreach (string str in ls[0])
                {
                    dt_Excel导入.Columns.Add(str);
                }
                for (int i = 1; i < ls.Count; i++)
                {
                    DataRow dr = dt_Excel导入.NewRow();
                    dt_Excel导入.Rows.Add(dr);
                    for (int j = 0; j < dt_Excel导入.Columns.Count; j++)
                    {
                        dr[dt_Excel导入.Columns[j].Caption] = ls[i][j];
                    }
                }
                gc.DataSource = dt_Excel导入;

                if (File.Exists(@str_目的))
                {
                    //如果存在则删除
                    File.Delete(@str_目的);
                }
            }
        }
        #endregion

        #region 界面操作
        #region 导入
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_处理文件();
                MessageBox.Show("导入成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                dt_Excel导入.Columns.Add("GUID");
                foreach (DataRow dr in dt_Excel导入.Rows)
                {
                    dr["GUID"] = System.Guid.NewGuid();
                }
                string sql = "select * from 人事员工奖惩记录表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_Excel导入);
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 查询
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string str_前 = time_前.EditValue.ToString();
                string str_后 = time_后.EditValue.ToString();
                string sql = string.Format("select * from 人事员工奖惩记录表 where 日期 >= '{0}' and 日期 <= '{1}'", str_前, str_后);
                SqlDataAdapter da = new SqlDataAdapter(sql,strconn);
                dt_查询保存 = new DataTable();
                da.Fill(dt_查询保存);
                dv = new DataView(dt_查询保存);
                dv.RowFilter = "类型 = '奖励' or 类型 = '惩戒'";
                gc.DataSource = dv;
                check_奖励.Checked = true;
                check_惩戒.Checked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bar_查询保存_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gc.BindingContext[dt_查询保存].EndCurrentEdit();
                gv.CloseEditor();
                if (dt_查询保存 == null) return;
                string sql = "select * from 人事员工奖惩记录表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_查询保存);
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 关闭
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion

        #region 选择模式
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {//查询模式
                //bar_导入.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //bar_导入保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_新增.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_删除.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_新增保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_文本.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                time_前.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                time_后.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_查询.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_导出.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_查询保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                check_奖励.Visible = true; check_编辑.Visible = true;
                check_惩戒.Visible = true; gc.DataSource = null;
                gv.OptionsBehavior.Editable = false;
            }
            else
            {//导入模式
                //bar_导入.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                //bar_导入保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_新增.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_删除.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_新增保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_文本.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                time_前.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                time_后.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_查询.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_导出.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_查询保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                check_奖励.Visible = false; check_编辑.Visible = false;
                check_惩戒.Visible = false;
                gv.OptionsBehavior.Editable = true;
                fun_载入空表();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (dv == null)
            {
                return;
            }
            if (check_奖励.Checked == true)
            {
                if (check_惩戒.Checked == true)
                {
                    dv.RowFilter = "类型 = '奖励' or 类型 = '惩戒'";
                    gc.DataSource = dv;
                }
                else
                {
                    dv.RowFilter = "类型 = '奖励'";
                    gc.DataSource = dv;
                }
            }
            else
            {
                if (check_惩戒.Checked == true)
                {
                    dv.RowFilter = "类型 = '惩戒'";
                    gc.DataSource = dv;
                }
                else
                {
                    gc.DataSource = null;
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (dv == null)
            {
                return;
            }
            if (check_惩戒.Checked == true)
            {
                if (check_奖励.Checked == true)
                {
                    dv.RowFilter = "类型 = '奖励' or 类型 = '惩戒'";
                    gc.DataSource = dv;
                }
                else
                {
                    dv.RowFilter = "类型 = '惩戒'";
                    gc.DataSource = dv;
                }
            }
            else
            {
                if (check_奖励.Checked == true)
                {
                    dv.RowFilter = "类型 = '奖励'";
                    gc.DataSource = dv;
                }
                else
                {
                    gc.DataSource = null;
                }
            }
        }

        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (check_编辑.Checked == true)
            {
                gv.OptionsBehavior.Editable = true;
                bar_查询保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                gv.OptionsBehavior.Editable = false;
                bar_查询保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }
        #endregion

        #region 新增
        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = dt_新增录入.NewRow();
            dt_新增录入.Rows.Add(dr);
            dr["GUID"] = System.Guid.NewGuid();
            dr["日期"] = System.DateTime.Now;
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            dr.Delete();
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gc.BindingContext[dt_新增录入].EndCurrentEdit();
                gv.CloseEditor();
                if (dt_新增录入 == null) return;
                string sql = "select * from 人事员工奖惩记录表 where 1<>1";
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                new SqlCommandBuilder(da);
                da.Update(dt_新增录入);
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                    gc.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick_2(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                OpenFileDialog openpic = new OpenFileDialog();
                if (openpic.ShowDialog() == DialogResult.OK)
                {
                    fun_上传照片(openpic.FileName);
                    MessageBox.Show("文件上传成功！");
                    bar_查询保存_ItemClick(null, null);
                }          
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                if (dr["文件"] == null || dr["文件"].ToString() == "")
                {
                    throw new Exception("没有文件可以下载，请先上传文件");
                }
                SaveFileDialog save = new SaveFileDialog();
                save.FileName = dr["文件"].ToString();
                if (save.ShowDialog() == DialogResult.OK)
                {
                    CFileTransmission.CFileClient.strCONN = strConn_FS;
                    CFileTransmission.CFileClient.Receiver(dr["文件"].ToString(), save.FileName);
                    MessageBox.Show("文件下载成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
    }
}
