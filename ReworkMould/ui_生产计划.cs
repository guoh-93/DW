using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.IO;

namespace ReworkMould
{
    public partial class ui_生产计划 : UserControl
    {
        DataTable dtM;      
        string strcon = CPublic.Var.strConn;      
        DataTable dt_SaleOrder = new DataTable();
        string str_log = "";
        string cfgfilepath = "";
        public ui_生产计划()
        {
            InitializeComponent();
        }

        public ui_生产计划(DataTable dt, DataTable dt1,string str)
        {
            InitializeComponent();
            dtM = dt;
            dt_SaleOrder = dt1;
            str_log = str;
        }

        private void ui_生产计划_Load(object sender, EventArgs e)
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
                x.UserLayout(panel2, this.Name, cfgfilepath);
                if (str_log != "")
                {
                    label2.Text = str_log;
                }
                else
                {
                    label2.Text = "---";
                }
                DataView dv = new DataView(dtM);
                dv.RowFilter = "自制='true' and 停用 = 0 and 订单用量 > 0";
                gc2.DataSource = dv;

                DataTable search_source = dt_SaleOrder.Copy();
                foreach (DataRow dr in dtM.Rows)
                {
                    DataRow[] p = search_source.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
                    if (p.Length > 0) continue;
                    DataRow xxxx = search_source.NewRow();
                    xxxx["物料编码"] = dr["物料编码"];
                    xxxx["物料名称"] = dr["物料名称"];
                    xxxx["规格型号"] = dr["规格型号"];
                    xxxx["存货分类"] = dr["存货分类"];
                    search_source.Rows.Add(xxxx);
                }
                searchLookUpEdit1.Properties.DataSource = search_source;
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                searchLookUpEdit1.Properties.ValueMember = "物料编码";

            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }
        }

        //private void calculate()
        //{
        //    try
        //    {
        //        BeginInvoke(new MethodInvoker(() =>
        //        {
        //            label2.Text = "正在计算中,请稍候...";
        //        }));
                
        //        ERPorg.Corg.result rs = new ERPorg.Corg.result();
        //        rs = ERPorg.Corg.fun_pool(dtM, false);
        //        dt_生产计划 = rs.dtM;
        //        //dtM.Columns.Add("最早发货日期", typeof(DateTime));
        //        dt_bom = rs.Bom;
        //        dt_totalcount = rs.TotalCount;
        //        dt_SaleOrder = rs.salelist_mx;

        //        dt_SaleOrder.Columns.Add("应完工日期", typeof(DateTime));
        //        foreach (DataRow saleR in dt_SaleOrder.Rows)
        //        {
        //            saleR["应完工日期"] = Convert.ToDateTime(saleR["预计发货日期"]).AddDays(-1);
        //        }

        //        dt_SaleCrderCopy = dt_SaleOrder.Copy();
        //        DataColumn dc = new DataColumn("选择", typeof(bool));
        //        dc.DefaultValue = false;
        //        dt_SaleCrderCopy.Columns.Add(dc);
        //        bl_calculate = false;
        //        BeginInvoke(new MethodInvoker(() =>
        //        {
        //            if (rs.str_log != "")
        //            {
        //                label2.Text = rs.str_log;
        //            }
        //            else
        //            {
        //                label2.Text = "---";
        //            }
        //            DataView dv = new DataView(dt_生产计划);
        //            dv.RowFilter = "自制='true' and 停用 = 0";
        //            gc2.DataSource = dv;

        //            DataTable search_source = dt_SaleOrder.Copy();
        //            foreach (DataRow dr in dt_生产计划.Rows)
        //            {
        //                DataRow[] p = search_source.Select(string.Format("物料编码='{0}'", dr["物料编码"]));
        //                if (p.Length > 0) continue;
        //                DataRow x = search_source.NewRow();
        //                x["物料编码"] = dr["物料编码"];
        //                x["物料名称"] = dr["物料名称"];
        //                x["规格型号"] = dr["规格型号"];
        //                x["存货分类"] = dr["存货分类"];
        //                search_source.Rows.Add(x);
        //            }
        //            searchLookUpEdit1.Properties.DataSource = search_source;
        //            searchLookUpEdit1.Properties.DisplayMember = "物料编码";
        //            searchLookUpEdit1.Properties.ValueMember = "物料编码";
        //            //gridControl1.DataSource = null;
        //            //gridControl2.DataSource = null;
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        bl_calculate = false;
        //        BeginInvoke(new MethodInvoker(() =>
        //        {
        //            label2.Text = "错误原因:" + ex.Message;

        //            bl_calculate = false;
        //        }));
        //    }
        //}

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
