using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ItemInspection
{
    public partial class UI一次合格率 : UserControl
    {

        #region 
        string strcon = CPublic.Var.strConn;


        #endregion 
        public UI一次合格率()
        {
            InitializeComponent();
        }

        private void UI一次合格率_Load(object sender, EventArgs e)
        {
            barEditItem1.EditValue = Convert.ToDateTime(System.DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd"));
            barEditItem2.EditValue = Convert.ToDateTime(System.DateTime.Today.ToString("yyyy-MM-dd"));
           // panel3.Visible = false;

        }

        private void fun_search()
        {
            string sql = "";
            string sql_1 = "";
            string sql_2 = "";
            if (CPublic.Var.LocalUserID == "admin")
            {
                sql = string.Format(@"select a.*,(a.合格数量/a.送检数量)一次合格率,班组,课室 from 
                  (select 负责人员ID,负责人员,COUNT(生产工单号)单数,sum(合格数量)合格数量,SUM(生产数量)生产数量,sum(送检数量)送检数量
             from 生产记录生产检验单主表    where 生效=1 and 作废=0  and 检验日期>='{0}' and 检验日期<='{1}'             
           group by 负责人员,负责人员ID)a,人事基础员工表  "
               , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));


                sql_1 = string.Format(@"select a.*,部门名称,(a.合格数量/a.送检数量)合格率 from 人事基础部门表,
                             (select 生产车间,COUNT(生产工单号)单数,sum(合格数量)合格数量,SUM(生产数量)生产数量,sum(送检数量)送检数量
                              from 生产记录生产检验单主表    where 生效=1 and 作废=0 and 检验日期>='{0}' and 检验日期<='{1}'
                                group by 生产车间 ) a   "
             , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

                sql_2 = string.Format(@"select a.*,(a.合格数量/a.送检数量)一次合格率 from
                        (select  班组,课室,课室编号,COUNT(生产工单号)单数,sum(合格数量)合格数量,SUM(生产数量)生产数量,sum(送检数量)送检数量
                    from 生产记录生产检验单主表,人事基础员工表   where 生效=1 and 作废=0  
                     and  检验日期>='{0}' and 检验日期<='{1}'
                    group by  班组,课室,课室编号 )a order by 课室编号 
 "
              , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            }
            else
            {
                sql = string.Format(@"select a.*,(a.合格数量/a.送检数量)一次合格率,班组,课室 from 
                  (select 负责人员ID,负责人员,COUNT(生产工单号)单数,sum(合格数量)合格数量,SUM(生产数量)生产数量,sum(送检数量)送检数量
             from 生产记录生产检验单主表    where 生效=1 and 作废=0  and 检验日期>='{0}' and 检验日期<='{1}'             
           group by 负责人员,负责人员ID)a,人事基础员工表 where a.负责人员ID=人事基础员工表.员工号 and 课室<>''"
               , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

                sql_1 = string.Format(@"select a.*,部门名称,(a.合格数量/a.送检数量)合格率 from 人事基础部门表,
                             (select 生产车间,COUNT(生产工单号)单数,sum(合格数量)合格数量,SUM(生产数量)生产数量,sum(送检数量)送检数量
                              from 生产记录生产检验单主表    where 生效=1 and 作废=0 and 检验日期>='{0}' and 检验日期<='{1}'
                                group by 生产车间 ) a  where 生产车间 =部门编号 "
             , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

                sql_2 = string.Format(@"select a.*,(a.合格数量/a.送检数量)一次合格率 from
                        (select  班组,课室,课室编号,COUNT(生产工单号)单数,sum(合格数量)合格数量,SUM(生产数量)生产数量,sum(送检数量)送检数量
                    from 生产记录生产检验单主表,人事基础员工表   where 生效=1 and 作废=0 and 生产记录生产检验单主表.负责人员ID=人事基础员工表.员工号 
                    and 课室<>'' and  检验日期>='{0}' and 检验日期<='{1}'
                    group by  班组,课室,课室编号 )a order by 课室编号 "
              , barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));
            }
           
           DataTable dt = new DataTable();
           dt=CZMaster.MasterSQL.Get_DataTable(sql, strcon);
           gridControl1.DataSource = dt;

          
          
           DataTable dt_1 = new DataTable();
           dt_1=CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
           //gridControl3.DataSource = dt_1;

           

                DataTable dt_2 = new DataTable();
           dt_2=CZMaster.MasterSQL.Get_DataTable(sql_2, strcon);
           gridControl2.DataSource = dt_2;
        }
        private void fun_check()
        {
            if (barEditItem1.EditValue != null && barEditItem2.EditValue != null && barEditItem1.EditValue.ToString() != "" && barEditItem2.EditValue.ToString()!="")
            {

            }
            else
            {
                throw new Exception("未选择时间段");
            }
        }
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();
                fun_search();
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
             
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
                 gridControl2.ExportToXlsx(saveFileDialog.FileName, options);
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
                //gridControl3.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (e.Clicks == 2)
            {
                DataRow dr =gridView1.GetDataRow(gridView1.FocusedRowHandle);
                DateTime t = Convert.ToDateTime(barEditItem1.EditValue);
                DateTime t1 = Convert.ToDateTime(barEditItem2.EditValue).Date.AddDays(1).AddSeconds(-1);

                frm成品检验列表 frm = new frm成品检验列表(t, t1, dr["负责人员ID"].ToString());
                frm.ShowDialog();
                frm.Text = dr["负责人员"].ToString() + "时间段内所做工单的检验单";
                frm.StartPosition = FormStartPosition.CenterScreen;

            }
               
        }


    }
}
