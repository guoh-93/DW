using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPpurchase
{
    public partial class UI采购未开票查询 : UserControl
    {
        DataTable dt_主界面;
        DataTable dt_供应商;
        public UI采购未开票查询()
        {
            InitializeComponent();
        }

        private void fun_GetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dt);
            }
        }

        private void fun_SetDataTable(DataTable dt, string sql)
        {
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }
        }

        private void UI采购未开票查询_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime dtime = CPublic.Var.getDatetime();
                dtime = new DateTime(dtime.Year, dtime.Month, dtime.Day);
                dateEdit3.EditValue = dtime.AddDays(1 - (dtime.Day));
                dateEdit4.EditValue = dtime.AddDays(1).AddSeconds(-1);
                gridView1.IndicatorWidth = 40;
                dt_供应商 = new DataTable();
                simpleButton1_Click(null, null);
                string sql2 = "select * from 采购供应商表 where 供应商状态='在用'";
                fun_GetDataTable(dt_供应商, sql2);
                txt_gysbh.Properties.DataSource = dt_供应商;
                txt_gysbh.Properties.DisplayMember = "供应商名称";
                txt_gysbh.Properties.ValueMember = "供应商名称";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ERPorg.Corg.FlushMemory();
                if (checkBox1.Checked == true)
                {
                    if (txt_gysbh.Text == null || txt_gysbh.Text.ToString() == "")
                    {
                        throw new Exception("未填写供应商");
                    }

                }
                if (checkBox6.Checked == true)
                {
                    if (dateEdit3.EditValue == null || dateEdit4.EditValue == null || dateEdit3.EditValue.ToString() == "" || dateEdit4.EditValue.ToString() == "")
                    {
                        throw new Exception("未选择日期");
                    }

                }
              DateTime dtime= Convert.ToDateTime(dateEdit4.Text).AddDays(1).AddSeconds(-1);

                dt_主界面 = new DataTable();
                string sql = @"  select a.*,入库量*单价 as 入库含税金额,入库量*未税单价 as 入库不含税金额,计量单位,isnull(czb.采购单类型,'')采购单类型 from 采购记录采购单入库明细 a
    left join  基础数据物料信息表 b on b.物料编码 =a.物料编码 
    left join 采购记录采购单主表  czb on czb.采购单号 =a.采购单号 
     where a.生效=1 and abs(a.入库量)>abs(a.已开票量) and  a.作废=0";
                if (checkBox1.Checked == true)
                {
                    sql = sql + "and a.供应商 = '" + txt_gysbh.Text + "'";
                }

                if (checkBox6.Checked == true)
                {
                    sql = sql + "and a.生效日期 <= '" + dtime + "' and a.生效日期 >= '" + dateEdit3.Text + "'";
                }

                ////弃用的
                //string sql_补 = string.Format(@"select L采购记录采购单入库明细L.*,计量单位  from L采购记录采购单入库明细L
                //                           left join  基础数据物料信息表 on 基础数据物料信息表.物料编码 =L采购记录采购单入库明细L.物料编码 
                //                            where L采购记录采购单入库明细L.生效=1 and abs(L采购记录采购单入库明细L.入库量)>abs(L采购记录采购单入库明细L.已开票量) and  作废=0 ");
                //if (checkBox1.Checked == true)
                //{
                //    sql_补 = sql_补 + "and 供应商 = '" + txt_gysbh.Text + "'";
                //}

                //if (checkBox6.Checked == true)
                //{
                //    sql_补 = sql_补 + "and 生效日期 <= '" + dtime + "' and 生效日期 >= '" + dateEdit3.Text + "'";
                //}
                fun_GetDataTable(dt_主界面, sql);
                //fun_GetDataTable(dt_主界面, sql_补);
                gridControl1.DataSource = dt_主界面;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
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
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gridControl1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
