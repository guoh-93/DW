using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frm员工培训导入查询 : UserControl
    {
        #region 自用类
        string strconn = CPublic.Var.strConn;
        //DataTable dt_Excel导入 = null;
        DataTable dt_查询保存 = null;
        //DataView dv = null;
        DataTable dt_新增录入 = null;
        //DataTable dt_下拉框;
        DataTable dt_人员 = null;
        #endregion

        public frm员工培训导入查询()
        {
            InitializeComponent();
        }

        private void frm员工培训导入查询_Load(object sender, EventArgs e)
        {
            //默认新增模式
            //bar_文本.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            bar_日期前.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            bar_日期后.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            //bar_查询.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //bar_导出.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            bar_新增.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            bar_删除.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            bar_新增保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            bar_查询保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            check_奖励.Visible = false;
            check_惩戒.Visible = false; 
            check_编辑.Visible = false;
            DateTime dtime = CPublic.Var.getDatetime();
            dtime = new DateTime(dtime.Year, dtime.Month, 1);
            bar_日期前.EditValue = dtime.AddMonths(-1);
            bar_日期后.EditValue = new DateTime(dtime.Year, dtime.Month, dtime.Day);
            //fun_载入空表();
            //gv.OptionsBehavior.Editable = true;
            fun_员工();
            checkBox1.Visible = false;
        }

        private void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //if (e.Column.Caption == "员工ID")
            //{
            //    DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            //    DataRow[] ds = dt_人员.Select(string.Format("员工ID = '{0}'", dr["员工ID"]));
            //    dr["员工"] = ds[0]["姓名"];
            //}
        }

        #region 方法
        private void fun_载入空表()
        {
            //string sql = "select * from 人事员工培训记录表 where 1<>1";
            //dt_新增录入 = new DataTable();
            //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //da.Fill(dt_新增录入);
            //gc.DataSource = dt_新增录入;
        }

        private void fun_员工()
        {
            string sql = "select 员工号 as 员工ID,姓名 from 人事基础员工表 where 在职状态  = '在职'";
            dt_人员 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_人员);
            repositoryItemSearchLookUpEdit2.DataSource = dt_人员;
            repositoryItemSearchLookUpEdit2.DisplayMember = "员工ID";
            repositoryItemSearchLookUpEdit2.ValueMember = "员工ID";
        }
        #endregion

        #region 编辑
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //DataRow dr = dt_新增录入.NewRow();
            //dt_新增录入.Rows.Add(dr);
            //dr["GUID"] = System.Guid.NewGuid();
            //dr["日期"] = System.DateTime.Now;
            //dr["操作人员"] = CPublic.Var.localUserName;
            //dr["操作人员ID"] = CPublic.Var.LocalUserID;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
            //dr.Delete();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //gc.BindingContext[dt_新增录入].EndCurrentEdit();
            //gv.CloseEditor();
            //if (dt_新增录入 == null) return;
            //string sql = "select * from 人事员工培训记录表 where 1<>1";
            //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //new SqlCommandBuilder(da);
            //da.Update(dt_新增录入);
            //MessageBox.Show("保存成功");
        }
        #endregion

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        #region 查询
        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (bar_员工号_下拉框.EditValue == null)
                {
                    throw new Exception("请先选择员工");
                }
                //查询：1.查单独记录的 2.查培训记录中的
                string str_日期 = bar_日期前.EditValue.ToString();
                string str_日期2 = ((DateTime)bar_日期后.EditValue).AddDays(1).AddSeconds(-1).ToString();
                //string sql = string.Format("select * from 人事员工培训记录表 where 日期 >= '{0}'", str_日期);
                //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
                dt_查询保存 = new DataTable();
                //da.Fill(dt_查询保存);
                {
                    string sll = string.Format(@"select 人事员工培训计划子表.*,人事员工培训计划表.* 
                        from 人事员工培训计划表 left join 人事员工培训计划子表 
                        on 人事员工培训计划子表.培训计划单号 = 人事员工培训计划表.培训计划单号 
                        where /* 人事员工培训计划表.日期 >= '{0}' and 日期 <= '{1}' and */ 员工号 = '{2}'", str_日期, str_日期2, bar_员工号_下拉框.EditValue.ToString());
                    SqlDataAdapter aa = new SqlDataAdapter(sll, strconn);
                    //DataTable dt_查询保存 = new DataTable();
                    aa.Fill(dt_查询保存);
                }
                //dv = new DataView(dt_查询保存);
                //dv.RowFilter = "类型 = '奖励' or 类型 = '惩戒'";
                gc.DataSource = dt_查询保存;
                //check_奖励.Checked = true;
                //check_惩戒.Checked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //gc.BindingContext[dt_查询保存].EndCurrentEdit();
            //gv.CloseEditor();
            //if (dt_查询保存 == null) return;
            //string sql = "select * from 人事员工培训记录表 where 1<>1";
            //SqlDataAdapter da = new SqlDataAdapter(sql, strconn);
            //new SqlCommandBuilder(da);
            //da.Update(dt_查询保存);
            //MessageBox.Show("保存成功");
        }

        private void barLargeButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                gc.ExportToXlsx(saveFileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        private void check_编辑_CheckedChanged(object sender, EventArgs e)
        {
            //if (check_编辑.Checked == true)
            //{
            //    gv.OptionsBehavior.Editable = true;
            //    bar_查询保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            //}
            //else
            //{
            //    gv.OptionsBehavior.Editable = false;
            //    bar_查询保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //}
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {//查询模式
                bar_新增.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_删除.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_新增保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bar_文本.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_日期前.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_日期后.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_查询.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_导出.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bar_查询保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                //check_奖励.Visible = true; 
                check_编辑.Visible = true;
                //check_惩戒.Visible = true; 
                gc.DataSource = null;
                gv.OptionsBehavior.Editable = false;
            }
            else
            {
                //bar_新增.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                //bar_删除.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                //bar_新增保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                //bar_文本.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //bar_日期前.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //bar_日期后.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //bar_查询.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //bar_导出.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //bar_查询保存.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                ////check_奖励.Visible = false;
                //check_编辑.Visible = false;
                ////check_惩戒.Visible = false;
                //gv.OptionsBehavior.Editable = true;
                //fun_载入空表();
            }
        }

        private void check_奖励_CheckedChanged(object sender, EventArgs e)
        {
            //if (dv == null)
            //{
            //    return;
            //}
            //if (check_奖励.Checked == true)
            //{
            //    if (check_惩戒.Checked == true)
            //    {
            //        dv.RowFilter = "类型 = '奖励' or 类型 = '惩戒'";
            //        gc.DataSource = dv;
            //    }
            //    else
            //    {
            //        dv.RowFilter = "类型 = '奖励'";
            //        gc.DataSource = dv;
            //    }
            //}
            //else
            //{
            //    if (check_惩戒.Checked == true)
            //    {
            //        dv.RowFilter = "类型 = '惩戒'";
            //        gc.DataSource = dv;
            //    }
            //    else
            //    {
            //        gc.DataSource = null;
            //    }
            //}
        }

        private void check_惩戒_CheckedChanged(object sender, EventArgs e)
        {
            //if (dv == null)
            //{
            //    return;
            //}
            //if (check_惩戒.Checked == true)
            //{
            //    if (check_奖励.Checked == true)
            //    {
            //        dv.RowFilter = "类型 = '奖励' or 类型 = '惩戒'";
            //        gc.DataSource = dv;
            //    }
            //    else
            //    {
            //        dv.RowFilter = "类型 = '惩戒'";
            //        gc.DataSource = dv;
            //    }
            //}
            //else
            //{
            //    if (check_奖励.Checked == true)
            //    {
            //        dv.RowFilter = "类型 = '奖励'";
            //        gc.DataSource = dv;
            //    }
            //    else
            //    {
            //        gc.DataSource = null;
            //    }
            //}
        }

        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void barLargeButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //查询：1.查单独记录的 2.查培训记录中的
                string str_日期 = bar_日期前.EditValue.ToString();
                string str_日期2 = ((DateTime)bar_日期后.EditValue).AddDays(1).AddSeconds(-1).ToString();
                dt_查询保存 = new DataTable();
                {
                    string sll = string.Format(@"select 人事员工培训计划子表.*,人事员工培训计划表.* 
                        from 人事员工培训计划表 left join 人事员工培训计划子表 
                        on 人事员工培训计划子表.培训计划单号 = 人事员工培训计划表.培训计划单号 
                        where 人事员工培训计划表.日期 >= '{0}' and 日期 <= '{1}'", str_日期, str_日期2);
                    SqlDataAdapter aa = new SqlDataAdapter(sll, strconn);
                    aa.Fill(dt_查询保存);
                }
                gc.DataSource = dt_查询保存;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
