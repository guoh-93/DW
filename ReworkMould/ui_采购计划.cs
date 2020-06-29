using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Reflection;

namespace ReworkMould
{
    public partial class ui_采购计划 : UserControl
    {
        #region 变量
        string strcon = CPublic.Var.strConn;
        /// <summary>
        /// 延续之前计划池的 dtm就不改了
        /// </summary>
        /// 
        DataTable dtM;             
        DataTable dt_SaleOrder;
        string str_log = "";
        string cfgfilepath = "";
        #endregion
        public ui_采购计划()
        {
            InitializeComponent();
        }
     
        public ui_采购计划(DataTable dt, DataTable dt1, string str)
        {
            InitializeComponent();
            dtM = dt;
            dt_SaleOrder = dt1;
            str_log = str;
        }

        private void ui_采购计划_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(panel1, this.Name, cfgfilepath);


                if (str_log != "")
                {
                    label6.Text = str_log;
                }
                else
                {
                    label6.Text = "---";
                }
                DataView dv = new DataView(dtM);
                dv.RowFilter = "停用 = 0 and (可购=1 or 委外=1) ";
                gc2.DataSource = dv;
                DataTable search_source = dt_SaleOrder.Copy();
                searchLookUpEdit1.Properties.DataSource = search_source;
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                searchLookUpEdit1.Properties.ValueMember = "物料编码";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ParentForm.Close();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // if (bl_刷新) throw new Exception("正在查询数据,稍候再试");
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    //gridControl1.ExportToXls(saveFileDialog.FileName, options);  

                    gc2.ExportToXlsx(saveFileDialog.FileName, options);
                    //ERPorg.Corg.TableToExcel(dt_订单明细, saveFileDialog.FileName);

                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
