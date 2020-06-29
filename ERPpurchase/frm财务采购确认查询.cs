using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class frm财务采购确认查询 : UserControl
    {
        public frm财务采购确认查询()
        {
            InitializeComponent();
        }
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dt_原数据;




        #endregion
        private void frm财务采购确认查询_Load(object sender, EventArgs e)
        {
            try
            {
                //string sql_原 = string.Format("select 物料编码,存货分类,物料名称,提交人,提交人ID,规格型号,未领量,库存总数,销售数量,需求数量,在途量,委外在途,日期,left (批号,8)as 批号  from 财务采购提交确认表 order by 日期 desc");
                //DataTable dt_原数据 = CZMaster.MasterSQL.Get_DataTable(sql_原, strconn);



                //string sql_原 = string.Format("select 物料编码,存货分类,物料名称,提交人,提交人ID,规格型号,未领量,库存总数,销售数量,需求数量,在途量,委外在途,日期,批号  from 财务采购提交确认表 order by 日期 desc");
               
        string sql_原 =  " select  提交人,提交人ID,日期,批号 from 财务采购提交确认表 group by 提交人,提交人ID,日期,批号";
                
                DataTable dt_原数据 = CZMaster.MasterSQL.Get_DataTable(sql_原, strconn);
                //DataView dv = new DataView(dt_原数据);
                //DataTable distinctTable = dv.ToTable(true, "批号", "日期", "提交人", "提交人ID");

                searchLookUpEdit1.Properties.ValueMember = "批号";
                searchLookUpEdit1.Properties.DisplayMember = "批号";
                searchLookUpEdit1.Properties.DataSource = dt_原数据;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
           // DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;

            string sql_原 = string.Format("select 物料编码,提交人,提交人ID,参考数量,存货分类,物料名称,规格型号,未领量,库存总数,销售数量,需求数量,在途量,委外在途,日期,批号 from 财务采购提交确认表  where 批号 ='{0}' and 参考数量<>0 order by 日期   desc", searchLookUpEdit1.EditValue.ToString());

             dt_原数据 = CZMaster.MasterSQL.Get_DataTable(sql_原, strconn);
            gridControl1.DataSource = dt_原数据;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {





            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {

                ERPorg.Corg.TableToExcel(dt_原数据, saveFileDialog.FileName);


                MessageBox.Show("保存成功");

                //DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                //options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                //gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
              //  DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
    }
}
