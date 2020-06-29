using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace ReworkMould
{
    public partial class ui_采购需求 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        public DataTable dtM;
        public bool bl_保存 = false;
        public ui_采购需求()
        {
            InitializeComponent();
        }

        public ui_采购需求(DataTable dt)
        {
            InitializeComponent();
            dtM = dt;
        }

        private void ui_采购需求_Load(object sender, EventArgs e)
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
                gridControl1.DataSource = dtM;
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
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(DevExpress.XtraPrinting.TextExportMode.Text, false, false);

                    gridControl1.ExportToXlsx(saveFileDialog.FileName, options);

                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("是否确定推料？"), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    bl_保存 = true;
                    this.ParentForm.Close();
                }
                   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                 
            }
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                bl_保存 = false;
                this.ParentForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                 
            }
        }

      

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                int[] dr1 = gridView1.GetSelectedRows();
                if (dr1.Length > 0)
                {
                    for (int i = dr1.Length - 1; i >= 0; i--)
                    {
                        DataRow dr_选中 = gridView1.GetDataRow(dr1[i]);
                        dr_选中.Delete();
                    }

                    DataRow drs = gridView1.GetDataRow(Convert.ToInt32(dr1[0]));
                    if (drs != null)
                    {
                        gridView1.SelectRow(dr1[0]);
                    }
                    else if (gridView1.GetDataRow(Convert.ToInt32(dr1[0]) - 1) != null)
                    {
                        gridView1.SelectRow(Convert.ToInt32(dr1[0]) - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
