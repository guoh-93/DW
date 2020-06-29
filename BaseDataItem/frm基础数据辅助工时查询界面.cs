using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BaseData
{
    public partial class frm基础数据辅助工时查询界面 : UserControl
    {
        DataTable dt_基础数据辅助工时表;
        DataTable dt_课室;
        string strconn = CPublic.Var.strConn;
        public frm基础数据辅助工时查询界面()
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

        private void frm基础数据辅助工时查询界面_Load(object sender, EventArgs e)
        {
            try
            {
                string sql = "select 属性值 课室,属性字段1 课室编号 from 基础数据基础属性表 where 属性类别 = '课室' and 属性字段1 <> ''";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                da.Fill(dt);
                searchLookUpEdit2.Properties.DataSource = dt;
                searchLookUpEdit2.Properties.DisplayMember = "课室";
                searchLookUpEdit2.Properties.ValueMember = "课室";
                DataTable dt_人员 = new DataTable();
                string sql3 = "select 基础数据辅助工时表.生产人员 from 基础数据辅助工时表 group by 生产人员";
                fun_GetDataTable(dt_人员, sql3);
                searchLookUpEdit3.Properties.DataSource = dt_人员;
                searchLookUpEdit3.Properties.DisplayMember = "生产人员";
                searchLookUpEdit3.Properties.ValueMember = "生产人员";
                dt_基础数据辅助工时表 = new DataTable();
                string sql2 = "select * from 基础数据辅助工时表 where 1<>1";
                fun_GetDataTable(dt_基础数据辅助工时表, sql2);
                gridControl1.DataSource = dt_基础数据辅助工时表;
                DateTime dtime = CPublic.Var.getDatetime();
                dtime = new DateTime(dtime.Year, dtime.Month, dtime.Day);
                dateEdit1.EditValue = dtime.AddDays(1 - (dtime.Day));
                dateEdit2.EditValue = dtime.AddDays(1).AddSeconds(-1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }         
        }



        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string s_课室 = searchLookUpEdit2.EditValue.ToString();
                DataTable dt_人员 = new DataTable();
                string sql = "select 基础数据辅助工时表.生产人员 from 基础数据辅助工时表 where 生产车间 ='" + s_课室 + "' group by 生产人员";
                fun_GetDataTable(dt_人员, sql);
                searchLookUpEdit3.Properties.DataSource = dt_人员;
                searchLookUpEdit3.Properties.DisplayMember = "生产人员";
                searchLookUpEdit3.Properties.ValueMember = "生产人员";
            }
            catch (Exception ex )
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string sql = " select * from 基础数据辅助工时表 where 工作日期 >= '" + dateEdit1.EditValue.ToString() + "' and 工作日期 <= '" + dateEdit2.EditValue.ToString() + "'";
                if (checkBox1.Checked == true)
                {
                    sql = sql + " and  生产车间 = '" + searchLookUpEdit2.Text + "'";
                }
                if (checkBox3.Checked == true)
                {
                    sql = sql + " and  生产人员 = '" + searchLookUpEdit3.Text + "'";
                }
                dt_基础数据辅助工时表 = new DataTable();
                fun_GetDataTable(dt_基础数据辅助工时表, sql);
                gridControl1.DataSource = dt_基础数据辅助工时表;
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
                if (checkBox1.Checked == true)
                {
                    if (searchLookUpEdit2.EditValue == null || searchLookUpEdit2.EditValue.ToString() == "")
                    {
                        throw new Exception("未选择车间");
                    }

                }
                if (checkBox3.Checked == true)
                {
                    if (searchLookUpEdit3.EditValue == null || searchLookUpEdit3.EditValue.ToString() == "")
                    {
                        throw new Exception("未选择人员");
                    }
                }
                string sql = " select * from 基础数据辅助工时表 where 工作日期 >= '" + dateEdit1.EditValue.ToString() + "' and 工作日期 <= '" + dateEdit2.EditValue.ToString() + "'";
                if (checkBox1.Checked == true)
                {
                    sql = sql + " and  生产车间 = '" + searchLookUpEdit2.Text + "'";
                }
                if (checkBox3.Checked == true)
                {
                    sql = sql + " and  生产人员 = '" + searchLookUpEdit3.Text + "'";
                }
                dt_基础数据辅助工时表 = new DataTable();
                fun_GetDataTable(dt_基础数据辅助工时表, sql);
                gridControl1.DataSource = dt_基础数据辅助工时表;
                decimal s_总计 = 0;
                decimal s_单数 = 0;
                foreach (DataRow dr in dt_基础数据辅助工时表.Rows)
                {
                    s_单数 = Convert.ToDecimal(dr["输入工时"]);
                    s_总计 += s_单数;
                }
                DataRow rr = dt_基础数据辅助工时表.NewRow();
                rr["输入工时"] = s_总计.ToString();
                rr["生产备注"] = "总计";
                dt_基础数据辅助工时表.Rows.Add(rr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
