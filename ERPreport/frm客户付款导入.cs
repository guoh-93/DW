using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Data.SqlClient;
namespace ERPreport
{
    public partial class frm客户付款导入 : Form
    {


            #region 变量

          DataTable dtM = new DataTable();
          string strconn = CPublic.Var.strConn;
            #endregion

        public frm客户付款导入()
        {
            InitializeComponent();
        }

        private void frm客户付款导入_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;

        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            } 
        }

        /// <summary>
        /// 读取Excel.xlsx
        /// </summary>
        /// <param name="fd">Excel.xlsx文件所在路径</param>
        public static DataTable ExcelXLSX(System.Windows.Forms.OpenFileDialog fd)
        {

            try
            {
                DataTable dt = new DataTable();
                string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + fd.FileName + ";Extended Properties='Excel 12.0; HDR=YES; IMEX=1'"; //此連接可以操作.xls與.xlsx文件
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                DataSet ds = new DataSet();
                //dt1  为excel中 所有sheet名字集合
                DataTable dt1 = new DataTable();

                dt1 = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                String[] excelSheets = new String[dt1.Rows.Count];
                int i = 0;
                // 添加工作表名称到字符串数组    
                foreach (DataRow row in dt1.Rows)
                {
                    string strSheetTableName = row["TABLE_NAME"].ToString();
                    //过滤无效SheetName   
                    if (strSheetTableName.Contains("$") && strSheetTableName.Replace("'", "").EndsWith("$"))
                    {
                        excelSheets[i] = strSheetTableName.Substring(0, strSheetTableName.Length - 1);
                        OleDbDataAdapter odda = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}]", excelSheets[i] + "$"), conn);//("select * from [Sheet1$]", conn);
                        odda.Fill(ds, excelSheets[i] + "$");
                    }
                    else
                    {
                        string str = excelSheets[i];
                    }


                    i++;
                }

                conn.Close();
                dt = ds.Tables[0];

                foreach (DataTable dt2 in ds.Tables)
                {
                    dt.Merge(dt2);
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //导入

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                dtM = new DataTable();
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                  
                    dtM = ExcelXLSX(ofd);
                    dtM.Columns.Add("客户编号", typeof(string ));
                    dtM.Columns.Add("流水号", typeof(string));
                    dtM.Columns.Add("工号", typeof(string));
                    dtM.Columns.Add("操作日期", typeof(DateTime));
                    gridControl1.DataSource = dtM;
                    gridView1.ViewCaption = "EXCEL数据清单";
                }
                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void fun_save()
        {
            string sql_cun = "select * from [客户付款记录表] where 1=2";
            DataTable dt_存 = new DataTable();
            dt_存 = CZMaster.MasterSQL.Get_DataTable(sql_cun, strconn);
            DateTime dtime = Convert.ToDateTime(dateEdit1.EditValue);
            DateTime t = CPublic.Var.getDatetime();
            foreach (DataRow dr in dtM.Rows)
            {
                DataRow r = dt_存.NewRow();
                
                //隔月导
                //dtime = new DateTime(dtime.Year, dtime.Month, 1);
                //dtime = dtime.AddSeconds(-1);
                //dtime = new DateTime(dtime.Year, dtime.Month, 20);
                string str_流水号 = CPublic.CNo.fun_得到最大流水号("FD", dtime.Year, dtime.Month).ToString("000");
                r["流水号"] = dtime.Year.ToString("00") + dtime.Month.ToString("00") + dtime.Day.ToString("00") + "-" + str_流水号;
                r["单号"] = dr["编号"]; //改为存入 财务导入时自带的编号！2017/8/9
                r["客户"] = dr["客户名称"].ToString();
                r["客户编号"] = dr["客户编号"].ToString();
                r["录入日期"] = r["操作日期"] = t;
                r["付款日期"] = dtime;
                //r["金额"] = dr["本月到款"].ToString();
                r["金额"] = dr["总金额"].ToString();
                r["模具金额"] = dr["模具金额"].ToString();
                r["货款金额"] = dr["货款金额"].ToString();
                r["其他金额"] = dr["其他金额"].ToString();
                r["录入人员"] =CPublic.Var.localUserName;
                r["工号"] =CPublic.Var.LocalUserID;

                dt_存.Rows.Add(r);
            }
            SqlDataAdapter da = new SqlDataAdapter(sql_cun, strconn);
            new SqlCommandBuilder(da);
            da.Update(dt_存);
            gridControl1.DataSource = dt_存;
        }
        private void fun_匹配编号()
        {
            DateTime t = CPublic.Var.getDatetime();
            SqlConnection coon = new SqlConnection(strconn);
            SqlCommand cmd = coon.CreateCommand();


            coon.Open();
            SqlDataReader sdr;
        

            int i =1;
            int  icount =dtM.Rows.Count;
            label5.Visible = true;
            label5.Text = "";
            foreach (DataRow dr in dtM.Rows)
            {
                cmd.CommandText = string.Format(@"select 客户编号,客户名称 from 客户基础信息表  where  客户名称='{0}'", dr["客户名称"]);
                sdr = cmd.ExecuteReader();
                sdr.Read();
                if (sdr.HasRows)
                {
                    dr["客户编号"] =sdr["客户编号"].ToString ();
                }
                else
                {
                    sdr.Dispose();
                    cmd.CommandText = string.Format(@"select 客户编号,客户 from 客户付款记录表  where  客户='{0}'", dr["客户名称"]);
                    sdr = cmd.ExecuteReader();
                    sdr.Read();
                    if (sdr.HasRows)
                    {
                        dr["客户编号"] = sdr["客户编号"].ToString();

                    }
                     
                }

                sdr.Dispose();
            
                label5.Text = i.ToString() + "/" + icount.ToString();
                Application.DoEvents();
            }
            gridControl1.DataSource = dtM;
            label5.Visible = false;
            coon.Close();

        }
        private void fun_check()
        {
            DataRow[] dr = dtM.Select(string.Format("客户编号 is null "));
            if (dr.Length > 0)
            {
                DataView dv = new DataView(dtM);
                dv.RowFilter="客户编号 is null";
                gridControl1.DataSource = dv;
                gridView1.ViewCaption = "未匹配到的记录";
                throw new Exception("尚有客户编号为空的客户，请与销售部门核对维护好信息之后再进行导入");
            }
            if (dateEdit1.EditValue == null || dateEdit1.EditValue.ToString() == "")
            {
                throw new Exception("未选择付款日期。");
            }
        }
        //匹配客户编号
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_匹配编号();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //生成单号记录等
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("确认信息无误后导入？"), "警告!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_check();
                    fun_save();
                    MessageBox.Show("导入成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("失败"+ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //gridControl1.ExportToXls(saveFileDialog.FileName, options);  

                gridControl1.ExportToXlsx(saveFileDialog.FileName);



                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }

 

  
    }
}
