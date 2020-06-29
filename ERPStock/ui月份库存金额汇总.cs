using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPStock
{
    public partial class ui月份库存金额汇总 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        public ui月份库存金额汇总()
        {
            InitializeComponent();
        }



        private void fun_search()
        {
            string sql_加 = "";
            DataTable dt = new DataTable();
            if (checkBox1.Checked == true)
            {
                sql_加 = string.Format(" and jzb.仓库号='{0}'", searchLookUpEdit1.EditValue.ToString());
            }
            string sql = string.Format(@" select jzb.仓库名称,sum(入库数量)入库数量,abs(sum(出库数量))出库数量,sum(入库金额) as 入库金额,ABS(sum(出库金额)) as 出库金额,sum(上月结转数量)上期结存,
         sum(上月结转金额) as 上期结存金额,SUM(本月结转数量)本期结转,SUM(本月结转金额) as 本期结存金额,SUM(差异数量)差异数量,SUM(差异金额)差异金额
         from 仓库月出入库结转表 jzb,基础数据物料信息表 base where  jzb.物料编码=base.物料编码 and 结算日期>='{0}' and 结算日期<'{1}'  
         {2} group by jzb.仓库名称", Convert.ToDateTime(dateTimePicker1.Text), Convert.ToDateTime(dateTimePicker1.Text).AddMonths(1), sql_加);
            DataTable dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
        }
        private void fun_load()
        {
            string sql_仓库 = "SELECT [属性值] as 仓库名称,属性字段1 as 仓库号 FROM  [基础数据基础属性表] where 属性类别 ='仓库类别'";
            DataTable dt_仓库 = new DataTable();
            SqlDataAdapter da_仓库 = new SqlDataAdapter(sql_仓库, strcon);
            da_仓库.Fill(dt_仓库);
            searchLookUpEdit1.Properties.DataSource = dt_仓库;
            searchLookUpEdit1.Properties.ValueMember = "仓库号";
            searchLookUpEdit1.Properties.DisplayMember = "仓库名称";

            dateTimePicker1.Text = CPublic.Var.getDatetime().AddMonths(-1).ToString();

        }
        private void fun_check()
        {

            if (checkBox1.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未填写订单号");
                }
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
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

                gridControl1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ui月份库存金额汇总_Load(object sender, EventArgs e)
        {
            fun_load();
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
