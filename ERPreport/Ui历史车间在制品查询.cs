using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPreport
{
    public partial class Ui历史车间在制品查询 : UserControl
    {

        string strcon = CPublic.Var.strConn;

        public Ui历史车间在制品查询()
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

        private void Ui历史车间在制品查询_Load(object sender, EventArgs e)
        {

        }

        private void fun_check()
        {
            if (dateEdit1.EditValue == null || dateEdit1.EditValue.ToString() == "")
            {
                throw new Exception("未选择时间");
            }
        }
        private void fun_search()
        {
            DateTime t = (Convert.ToDateTime(dateEdit1.EditValue));
            t = new DateTime(t.Year, t.Month, t.Day);
            string sql = string.Format("select * from [财务即时库存记录] where 时间>='{0}' and  时间<'{1}' ", t, t.AddDays(1));
            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt;
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
               fun_check();
               fun_search();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
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

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
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
