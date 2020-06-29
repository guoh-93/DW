using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace ERPSale
{
    public partial class ui批量导入形态转换 : UserControl
    {

        DataTable dtM = new DataTable();
        string strconn = CPublic.Var.strConn;

        public ui批量导入形态转换()
        {
            InitializeComponent();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {

                dtM = new DataTable();
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    dtM = ExcelXLSX(ofd);
                   
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

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void ui批量导入形态转换_Load(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否确认转换单已完善？", "确认!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    fun_check();
                    fun_save(false);
                    MessageBox.Show("保存成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_save(bool v)
        {
            throw new NotImplementedException();
        }

        private void fun_check()
        {
            foreach(DataRow dr in dtM.Rows)
            {
                if(dr["数量"].ToString() == "")
                {
                    throw new Exception("数量不能为空");
                }
            }
        }
    }
}
