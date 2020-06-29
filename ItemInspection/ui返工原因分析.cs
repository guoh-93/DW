using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ItemInspection
{
    public partial class ui返工原因分析 : UserControl
    {

        DataTable dt_左;     
        string strcon = CPublic.Var.strConn;
        DataTable dt_右;
        string str_车间 = "";
        public ui返工原因分析()
        {
            InitializeComponent();
        }

        private void ui返工原因分析_Load(object sender, EventArgs e)
        {
            barEditItem1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd"));
            barEditItem2.EditValue = Convert.ToDateTime(System.DateTime.Today.ToString("yyyy-MM-dd"));
            //车间
            //string sql_车间 = "select 部门编号,部门名称 from 人事基础部门表 where 部门编号 >= '0001030101' and 部门编号 <= '0001030107'";
            // DataTable  dt_车间 = new DataTable();
            //SqlDataAdapter da_车间 = new SqlDataAdapter(sql_车间, strcon);
            //da_车间.Fill(dt_车间);
            //DataRow dr = dt_车间.NewRow();
            //dr["部门编号"] = "";
            //dr["部门名称"] = "";
            //dt_车间.Rows.Add(dr);
            //repositoryItemSearchLookUpEdit1.DataSource = dt_车间;
            //repositoryItemSearchLookUpEdit1.DisplayMember = "部门名称";
            //repositoryItemSearchLookUpEdit1.ValueMember = "部门编号";

            string sql = "select 属性字段1 as 部门编号,属性值 as 部门名称 from  基础数据基础属性表  where 属性类别 = '生产车间' order by 部门编号";
            DataTable dt_车间 = new DataTable();
            SqlDataAdapter da_车间 = new SqlDataAdapter(sql, strcon);
            da_车间.Fill(dt_车间);
            DataRow dr = dt_车间.NewRow();
            dr["部门编号"] = "";
            dr["部门名称"] = "";
            dt_车间.Rows.Add(dr);
            repositoryItemSearchLookUpEdit1.DataSource = dt_车间;
            repositoryItemSearchLookUpEdit1.DisplayMember = "部门名称";
            repositoryItemSearchLookUpEdit1.ValueMember = "部门编号";




        }
        private void fun_search()
        {
            str_车间 = "";
            if (barEditItem3.EditValue != null && barEditItem3.EditValue.ToString() != "")
            {
                str_车间 = string.Format(" and 生产车间='{0}'", barEditItem3.EditValue);

            }
            string sql = string.Format(@"select ltrim(rtrim(返工原因))返工原因,sum(数量)数量 from  [成品检验检验记录返工表] where 生产检验单号 
                                    in (select 生产检验单号 from 生产记录生产检验单主表 where  检验日期>='{0}' and 检验日期<='{1}' {2} ) 
                                group by 返工原因 order by 数量", Convert.ToDateTime(barEditItem1.EditValue).ToString("yyyy-MM-dd")
                            ,Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).ToString("yyyy-MM-dd"),str_车间);

            dt_左 = new DataTable();
           dt_左= CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dt_左;

            DataRow dr = dt_左.NewRow();
            int i=0;
            foreach(DataRow r in dt_左.Rows)
            {
                i+=Convert.ToInt32(r["数量"]);
            }
            dr["返工原因"] = "总计:";
            dr["数量"] = i;
            dt_左.Rows.Add(dr);
        }
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                fun_search();
            }
            catch (Exception)
            {
                
                throw;
            }
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
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
             e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string sql = string.Format(@" select jyz.生产工单号,gd.生产工单类型,jyz.生产检验单号,fg.数量 该原因返工数,base.规格型号,负责人员,班组,convert(varchar(10),送检日期,120)生产日期 
   from  生产记录生产检验单主表 jyz,基础数据物料信息表 base ,[成品检验检验记录返工表] fg,生产记录生产工单表 gd
   where   jyz.物料编码= base.物料编码 and jyz.生产检验单号=fg.生产检验单号  and gd.生产工单号=jyz.生产工单号   and   检验日期>='{0}' and 检验日期<='{1}' {3}
                            and   返工原因='{2}'  order by 返工数量 ", Convert.ToDateTime(barEditItem1.EditValue)
                            ,Convert.ToDateTime(barEditItem2.EditValue).AddDays(1), dr["返工原因"],str_车间);
            dt_右 = new DataTable();
            dt_右= CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl2.DataSource = dt_右;
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
                gridControl2.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridControl2_Click(object sender, EventArgs e)
        {

        }
    }
}
