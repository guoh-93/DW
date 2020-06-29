using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ItemInspection
{
    public partial class ui一次合格率_新 : UserControl
    {

        string strcon = CPublic.Var.strConn;
        DataTable dt_左;
        decimal dec;
        public ui一次合格率_新()
        {
            InitializeComponent();
        }

        private void ui一次合格率_新_Load(object sender, EventArgs e)
        {
            barEditItem1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd"));
            barEditItem2.EditValue = Convert.ToDateTime(System.DateTime.Today.ToString("yyyy-MM-dd"));
           // panel1.Visible = false;
           
        }



        private void fun_search()
        {
            string sql = string.Format(@"select a.*,规格型号,基础数据物料信息表.物料名称,大类,小类,(a.合格数量/a.送检数量)一次合格率 from 
                  (select  物料编码,COUNT(生产工单号)单数,sum(合格数量)合格数量,SUM(生产数量)生产数量 ,sum(送检数量)送检数量
                     from 生产记录生产检验单主表    where 生效=1 and 作废=0  and 检验日期>='{0}' and 检验日期<='{1}'              
                  group by 物料编码)a,基础数据物料信息表 
             where 基础数据物料信息表.物料编码=a.物料编码 and left(基础数据物料信息表.物料编码,2) ='10' order by 一次合格率 "
            , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            dt_左 = new DataTable();
            dt_左 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_左;

            string sql_1 = string.Format(@"select a.*,(a.合格数量/a.送检数量)一次合格率 from 
                    (select  大类,COUNT(生产工单号)单数,sum(合格数量)合格数量,SUM(生产数量)生产数量,sum(送检数量)送检数量
                     from (select 生产记录生产检验单主表.*,大类 from 生产记录生产检验单主表,基础数据物料信息表 
				where 生产记录生产检验单主表.物料编码=基础数据物料信息表.物料编码)m    where 生效=1 and 作废=0           
                 and 检验日期>='{0}' and 检验日期<='{1}'  group by 大类)a order by 一次合格率"
                , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

            DataTable dt_1 = new DataTable();
            dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
            //gridControl3.DataSource = dt_1;

            string sql_2 = string.Format(@"  select a.*,(a.合格数量/a.送检数量)一次合格率 from 
                                     (select  大类,小类,COUNT(生产工单号)单数,sum(合格数量)合格数量,SUM(生产数量)生产数量,sum(送检数量)送检数量
                             from (select 生产记录生产检验单主表.*,大类,小类 from 生产记录生产检验单主表,基础数据物料信息表 
				    where 生产记录生产检验单主表.物料编码=基础数据物料信息表.物料编码)m    where 生效=1 and 作废=0           
                 and 检验日期>='{0}' and 检验日期<='{1}'   group by 大类,小类)a order by 一次合格率"
                    , barEditItem1.EditValue,Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

            DataTable dt_2 = new DataTable();
            dt_2 = CZMaster.MasterSQL.Get_DataTable(sql_2, strcon);
            //gridControl2.DataSource = dt_2;

        }

        private void fun_check()
        {
            if (barEditItem1.EditValue != null && barEditItem2.EditValue != null && barEditItem1.EditValue.ToString() != "" && barEditItem2.EditValue.ToString() != "")
            {

            }
            else
            {
                throw new Exception("未选择时间段");
            }
        }
        private void fun_计算平均合格率()
        {
            int i = 1;
            dec = 0;
            foreach (DataRow dr in dt_左.Rows)
            {
                dec = dec + Convert.ToDecimal(dr["一次合格率"]);
                i++;
            }

            if(dec!=0)
            {
                decimal dec_显示;
                dec = dec / i;
                dec_显示 = dec * 100;
                textBox1.Text = dec_显示.ToString("00.00") + '%';
            }

        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
              //  gridControl2.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
             //   gridControl3.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                fun_search();
                fun_计算平均合格率();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                MessageBox.Show("查询失败，重试");
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
                //int j = gv.RowCount;
                //for (int i = 0; i < j; i++)
                //{
                if (Convert.ToDecimal(gridView1.GetRowCellValue(e.RowHandle, "一次合格率"))< dec)
                {
                    e.Appearance.BackColor = Color.Pink;
                   
                }
         
          
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
