using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraPrinting;

namespace ERPreport
{

    public partial class UI生产入库工单领料明细 : UserControl
    {
        #region
        DataTable dtM;
        string strcon = CPublic.Var.strConn;

        #endregion 

        public UI生产入库工单领料明细()
        {
            InitializeComponent();
        }

        private void UI生产入库工单领料明细_Load(object sender, EventArgs e)
        {
            DateTime t = CPublic.Var.getDatetime().AddMonths(-1);
            t = new DateTime(t.Year, t.Month, 1);   //去上月月初 一般财务是要上个月的 数据
            dateEdit1.EditValue = t;
            dateEdit2.EditValue = t.AddMonths(1).AddSeconds(-1);
        }

        private void fun_check()
        {
            if (dateEdit1.EditValue == null && dateEdit2.EditValue.ToString() == "")
            {
                throw new Exception("请选择时间");
            }
        }
        private void fun_search()
        {
            string sql = string.Format(@"select x.生产工单号,b.车间,b.物料编码 as 成品编码, b.规格型号 as 成品型号,b.大类 as 成品大类,b.小类 as 成品小类
,x.待领料总量/生产记录生产工单表.生产数量 as 用量 ,a.物料编码 as 子项编码,a.规格型号 as 子项规格,a.大类 as 子项大类,a.小类 as 子项小类,j.已入库数量,x.待领料总量,x.已领数量,生产记录生产工单表.生产数量
 ,g.已入库数量 as 所选时间入库数,ISNULL(h.所选时间已领数,0)所选时间已领数 from 生产记录生产工单待领料明细表 x
left join 生产记录生产工单表 on  生产记录生产工单表.生产工单号=x.生产工单号
left  join 基础数据物料信息表 a on  a.物料编码=x.物料编码 
left  join 基础数据物料信息表 b on  b.物料编码=生产记录生产工单表.物料编码 
left join  (select 生产工单号,SUM(入库数量)已入库数量 from 生产记录成品入库单明细表 group by 生产工单号)j  on j.生产工单号= x.生产工单号
left join  (select 生产工单号,SUM(入库数量)已入库数量 from 生产记录成品入库单明细表 where 生效日期>'{0}' and 生效日期<'{1}' 
			 group by 生产工单号)g  on g.生产工单号= x.生产工单号
left join  (select 生产工单号,物料编码,SUM(领料数量)所选时间已领数 from 生产记录生产领料单明细表 where 生效日期>'{0}' and 生效日期<'{1}' 
			 group by 生产工单号,物料编码)h on h.生产工单号= x.生产工单号 and h.物料编码=x.物料编码
where x.生产工单号 in (select 生产工单号 from 生产记录成品入库单明细表 where 生效日期>'{0}' and 生效日期<'{1}')",
 Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
            dtM = new DataTable();
            dtM= CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;

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

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
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
                DevExpress.XtraPrinting.XlsxExportOptions options = new XlsxExportOptions(TextExportMode.Text, false, false);

                
                gridView1.ExportToXlsx(saveFileDialog.FileName,options );

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
