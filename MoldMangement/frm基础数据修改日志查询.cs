using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MoldMangement
{
    public partial class frm基础数据修改日志查询 : UserControl
    {
        public frm基础数据修改日志查询()
        {
            InitializeComponent();
        }


        private void frm基础数据修改日志查询_Load(object sender, EventArgs e)
        {
            this.gv.IndicatorWidth = 40;
            DateTime t = CPublic.Var.getDatetime().AddMonths(-1);
            t = new DateTime(t.Year, t.Month, 1);   //去上月月初 一般财务是要上个月的 数据
            dateEdit1.EditValue = t;
            dateEdit2.EditValue = t.AddMonths(1).AddSeconds(-1);
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();
                gv.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string sql = string.Format(@"select 基础数据物料信息修改日志表.*,基础数据物料信息表.原ERP物料编号,物料名称,n原ERP规格型号,物料类型 from 基础数据物料信息修改日志表
            LEFT join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 基础数据物料信息修改日志表.物料编码 
            where 基础数据物料信息修改日志表.日期 > '{0}' and 基础数据物料信息修改日志表.日期 < '{1}'", dateEdit1.Text, dateEdit2.Text);

            if (checkBox1.Checked == true)
            {
                sql = sql + string.Format(" and 基础数据物料信息修改日志表.姓名 = '{0}'", textBox1.Text);
            }
            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format(" and 基础数据物料信息修改日志表.员工号 = '{0}'", textBox2.Text);
            }
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
            gc.DataSource = dt;
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            } 
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }


    }
}
