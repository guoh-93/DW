using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{

    public partial class ui客户付款管理 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        public ui客户付款管理()
        {
            InitializeComponent();
        }

     
        private void fun_load()
        {
            DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue).Date;

              DateTime t2= Convert.ToDateTime(barEditItem2.EditValue).Date;
            string sql = string.Format(@"select  客户付款记录表.*,客户基础信息表.片区 from 客户付款记录表,客户基础信息表
                          where  客户付款记录表.付款日期>='{0}' and  客户付款记录表.付款日期<='{1}'and  客户付款记录表.客户编号=客户基础信息表.客户编号"
                          , t1,t2.AddDays(1).AddSeconds(-1));
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }

        private void ui客户付款管理_Load(object sender, EventArgs e)
        {
            barEditItem1.EditValue = CPublic.Var.getDatetime().AddMonths(-1);
            barEditItem2.EditValue = CPublic.Var.getDatetime().AddDays(1).AddSeconds(-1);

            fun_load();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                ERPSale.frm增改客户付款 frm = new frm增改客户付款(dr);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                fun_load();

            } 
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPSale.frm增改客户付款 frm = new frm增改客户付款();
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.ShowDialog();
            fun_load();
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定要删除这条记录吗？删除将不可恢复", "警告？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    string sql = string.Format("delete 客户付款记录表 where 单号='{0}'", dr["单号"].ToString());
                    CZMaster.MasterSQL.ExecuteSQL(sql, strcon);
                    dtM.Rows.Remove(dr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            } 
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ERPreport.frm客户付款导入 frm = new ERPreport.frm客户付款导入();
            frm.Text = "付款记录导入";
            frm.ShowDialog();
            fun_load();

        }
  
    }
}
