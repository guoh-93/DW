using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPSale
{
    public partial class ui帐期提醒界面 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM;

        public ui帐期提醒界面()
        {
            InitializeComponent();
        }

        private void fun_load()
        {
            string sql = string.Format(@"select 原ERP物料编号,n原ERP规格型号,a.*,片区,业务员  from (
                 select c.销售订单明细号,a.开票票号,a.成品出库单明细号,a.产品编码,a.产品名称,开票数量,b.开票日期,c.客户,c.客户编号,a.开票税前金额 as 不含税金额
                 ,a.开票税后金额  as 含税金额,
                 d.帐期,SUBSTRING(c.销售订单明细号,0,CHARINDEX('-',c.销售订单明细号,0)) as 销售订单号
                 from  销售记录销售开票明细表 a,销售记录销售开票主表 b,销售记录成品出库单明细表 c,销售记录销售订单主表 d
                 where a.开票票号=b.开票票号 and a.生效=1 and b.开票日期>'{0}' and b.开票日期<'{1}' and a.成品出库单明细号=c.成品出库单明细号 
                 and SUBSTRING(c.销售订单明细号,0,CHARINDEX('-',c.销售订单明细号,0))=d.销售订单号) a
                 left  join  客户基础信息表 on a.客户编号=客户基础信息表.客户编号
                 left join 基础数据物料信息表 on 基础数据物料信息表.物料编码=a.产品编码 ",
                 barEditItem1.EditValue,Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
            dtM = new DataTable();
            dtM = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
            dtM.Columns.Add("剩余天数",typeof(int));
            dtM.Columns.Add("应到款日期", typeof(DateTime));

            foreach (DataRow dr in dtM.Rows)
            {
                dr["应到款日期"] = Convert.ToDateTime(dr["开票日期"]).AddDays(Convert.ToInt32(dr["帐期"]));
                dr["剩余天数"] = (Convert.ToDateTime(dr["应到款日期"]) - CPublic.Var.getDatetime()).Days ;
            }
            DataView dv = new DataView(dtM);
            dv.Sort = "剩余天数";
            gridControl1.DataSource = dv ;
      

        }

        private void ui帐期提醒界面_Load(object sender, EventArgs e)
        {
            DateTime dtime = CPublic.Var.getDatetime();
            //当前月的第一天 和 最后一天
            dtime = new DateTime(dtime.Year, dtime.Month,1);
            DateTime dtime1 = dtime.AddMonths(1).AddDays(-1);
            barEditItem1.EditValue = dtime;
            barEditItem2.EditValue = dtime1;


        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
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

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
         

                if (gridView1.GetRow(e.RowHandle) == null)
                {
                    return;
                }
            
                if ( Convert.ToInt32(gridView1.GetRowCellValue(e.RowHandle, "剩余天数"))<0)
                {
                    e.Appearance.BackColor = Color.Red;
                    e.Appearance.BackColor2 = Color.Red;
                }
                else if (Convert.ToInt32(gridView1.GetRowCellValue(e.RowHandle, "剩余天数").ToString()) <10)
                {
                    e.Appearance.BackColor = Color.Pink;
                    e.Appearance.BackColor2 = Color.Pink;
                }
             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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


    }
}
