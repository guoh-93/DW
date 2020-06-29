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
    public partial class frm返修出入库查询 : UserControl
    {
        DataTable dtM = new DataTable();
        string strcon = CPublic.Var.strConn;
        public frm返修出入库查询()
        {
            InitializeComponent();
        }

        private void frm返修出入库查询_Load(object sender, EventArgs e)
        {
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

                gridView1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void fun_search()
        {
            dtM = new DataTable();
                    string sql = string.Format(@"select a.出入库申请单号 as 相关单号,返修出库单号 as 单号,b.申请类型,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.物料名称,
 基础数据物料信息表.图纸编号,大类,小类,产品线, n核算单价, 数量,a.生效日期 as 出库日期,b.备注,b.操作人员 as 申请人,c.姓名 as 操作人
 from 返修出库子表 a,基础数据物料信息表,返修出入库申请主表 b,人事基础员工表 c where a.物料编码=基础数据物料信息表.物料编码
  and a.生效日期 >'{0}' and   a.生效日期 <'{1}' and  c.员工号=a.生效人员编号
 and a.出入库申请单号=b.出入库申请单号", Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                    if (checkBox1.Checked == true)
                    {
                        sql = sql + string.Format(" and b.出入库申请单号 like '%{0}%'", textBox1.Text);
                    }
                    if (checkBox2.Checked == true)
                    {
                        sql = sql + string.Format(" and b.申请类型 = '{0}'", comboBox1.Text);
                    }

                    dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
                    sql = string.Format(@"select a.出入库申请单号 as 相关单号,返修入库单号 as 单号,b.申请类型,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.n原ERP规格型号,基础数据物料信息表.物料名称,
 基础数据物料信息表.图纸编号,大类,小类,产品线, n核算单价, 数量,a.生效日期 as 出库日期,b.备注,b.操作人员 as 申请人,c.姓名 as 操作人
 from 返修入库子表 a,基础数据物料信息表,返修出入库申请主表 b,人事基础员工表 c   where a.物料编码=基础数据物料信息表.物料编码
  and a.生效日期 >'{0}' and   a.生效日期 <'{1}' and c.员工号=a.生效人员编号
  and a.出入库申请单号=b.出入库申请单号", Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd"));
                    if (checkBox1.Checked == true)
                    {
                        sql = sql + string.Format(" and b.出入库申请单号 like '%{0}%'", textBox1.Text);
                    }
                    if (checkBox2.Checked == true)
                    {
                        sql = sql + string.Format(" and b.申请类型 = '{0}'", comboBox1.Text);
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
                    {
                        da.Fill(dtM);
                    }

            gridControl1.DataSource = dtM;

        }


        private void fun_check()
        {
            if (dateEdit1.EditValue == null && dateEdit2.EditValue.ToString() == "")
            {
                throw new Exception("请选择时间");
            }
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

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            } 
        }
    }
}
