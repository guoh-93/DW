using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace ERPreport
{
    public partial class ui历史物料库存 : UserControl
    {

        string strcon=CPublic.Var.strConn;
        string cfgfilepath = "";
        public ui历史物料库存()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
 
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ui历史物料库存_Load(object sender, EventArgs e)
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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void fun_load()
        {
            string sql_库存 = string.Format(@"select aa.物料编码,aa.物料名称,aa.规格型号,aa.仓库号,aa.仓库名称,aa.货架描述,(aa.库存总数-isnull(xx.出入数量,0))库存总数,aa.出入库时间 from 仓库物料数量表 aa left join 
             (select  物料编码, SUM(实效数量) as 出入数量, 仓库号 from 仓库出入库明细表  where 出入库时间 > '{0}' group by 物料编码, 仓库号) xx on xx.物料编码 = aa.物料编码 and xx.仓库号 = aa.仓库号", barEditItem1.EditValue.ToString());
            //string sql = string.Format("select  物料编码,SUM(实效数量)as 出入数量,仓库号 from 仓库出入库明细表  where 出入库时间 >'{0}'  group by 物料编码", barEditItem1.ToString());
            DataTable dt = new DataTable();
            dt=CZMaster.MasterSQL.Get_DataTable(sql_库存, strcon);
            gridControl1.DataSource = dt;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gridView1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void fun_check()
        {
            if (barEditItem1.ToString() == "" || barEditItem1.EditValue == null)
            {
                throw new Exception("请先选择时间再查询");
            }
        }
    }
}
