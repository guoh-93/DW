using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPproduct
{
    public partial class ui产能统计 : UserControl
    {
        #region  变量

        string strcon = CPublic.Var.strConn;
        DataTable dtM;


        #endregion





        public ui产能统计()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // 命名样式
        private void ui产能统计_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                fun_load();

                 DateTime t=CPublic.Var.getDatetime();
                 dateEdit4.EditValue = t.Date;

                 t=  t.AddMonths(-1);
                 t=new DateTime (t.Year,t.Month,1);
                dateEdit2.EditValue = t;


            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message);
                throw;
            }

        }


#pragma warning disable IDE1006 // 命名样式
        private void simpleButton1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            try
            {
                if (barEditItem1.EditValue != null && barEditItem1.EditValue.ToString() != "")   //选择车间
                {

                    //if (checkBox1.CheckState != CheckState.Checked && checkBox2.CheckState != CheckState.Checked)
                    //{

                    //    throw new Exception("请选择搜索条件"); //两个时间段至少勾选一个

                    //}
                    //else if (checkBox1.CheckState == CheckState.Checked)
                    //{
                    //    if ((dateEdit1.EditValue == null || dateEdit1.EditValue.ToString() == "") || (dateEdit3.EditValue == null || dateEdit3.EditValue.ToString() == ""))
                    //    {
                    //        throw new Exception("请选择搜索条件");

                    //    }
                    //}
                    //else if (checkBox2.CheckState == CheckState.Checked)
                    //{
                        if ((dateEdit2.EditValue == null|| dateEdit2.EditValue.ToString() == "") || (dateEdit4.EditValue == null || dateEdit4.EditValue.ToString() == ""))
                        {
                            throw new Exception("请选择入库时间");


                        }
                    //}
                    fun_search();
                }
                else
                {
                    throw new Exception("未选择车间");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }


#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select  属性字段1 部门编号,属性值 部门名称 from 基础数据基础属性表 where 属性类别='生产车间'";
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            repositoryItemSearchLookUpEdit1.DataSource = dt;
            repositoryItemSearchLookUpEdit1.ValueMember = "部门编号";
            repositoryItemSearchLookUpEdit1.DisplayMember = "部门名称";

            string sql_1 = string.Format("select * from  人事基础员工表 where 员工号='{0}'", CPublic.Var.LocalUserID);

            DataTable dt_1 = new DataTable();
            dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, strcon);
            if (dt_1.Rows.Count > 0)
            {
                barEditItem1.EditValue = dt_1.Rows[0]["课室"];
            }

        }
        //开始加工时间 checkBox1   已经检验的
#pragma warning disable IDE1006 // 命名样式
        private void fun_search()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "";
            
            
            string sql1 = string.Format(@"and a.生效日期>='{0}'and a.生效日期 <='{1}'", dateEdit2.EditValue, Convert.ToDateTime(dateEdit4.EditValue).AddDays(1).AddSeconds(-1));

            
            sql = string.Format(@" select x.*,物料名称,规格型号,合格数量*n核算单价 as 合格产值, 工时 as 定额,(合格数量/总送检数) as 合格率,总入库数*n核算单价 as 入库产值,总送检数*n核算单价 as 送检产值
 ,总生产数 *n核算单价 as 生产产值,n核算单价 from (
  select  物料编码,SUM(生产数量) as 总生产数,SUM(入库数量)总入库数 ,SUM(送检数量) as 总送检数 ,sum(不合格数量) as 不合格数量,SUM(合格数量) as 合格数量,COUNT(生产工单号) as 制造批次 
 from ( 
   select  a.*,b.不合格数量,b.合格数量,b.生产数量,b.送检数量 from (
 select a.物料编码,a.生产工单号,sum(a.入库数量)入库数量 from 生产记录成品入库单明细表 a
 left  join 生产记录生产工单表 ax on ax.生产工单号=a.生产工单号
  where 1=1 {0} and 生产车间='{1}'
 group  by a.物料编码,a.生产工单号)a
 left  join (select   生产工单号,生产数量,sum(送检数量)送检数量,sum(合格数量)合格数量,sum(不合格数量)不合格数量 from  生产记录生产检验单主表 where 生产车间='{1}'
group  by 生产工单号,生产数量) b  on a.生产工单号=b.生产工单号 )a  group by 物料编码)x
 left  join 基础数据物料信息表 base  on base.物料编码=x.物料编码  ", sql1, barEditItem1.EditValue);
             using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
             {
                dtM = new DataTable();
                da.Fill(dtM);
                gridControl1.DataSource = dtM;
             }
        }

#pragma warning disable IDE1006 // 命名样式
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            Decimal dec_总制造产值= 0;
            Decimal dec_总合格产值= 0;
            int count_界面行数 = this.gridView1.DataRowCount;
            if (count_界面行数 > 0)
            {
                for (int i = 0; i < count_界面行数; i++)
                {
                    DataRow dr = gridView1.GetDataRow(i);
                    dec_总制造产值 = Convert.ToDecimal(dr["制造产值"]) + dec_总制造产值;
                    dec_总合格产值 = Convert.ToDecimal(dr["合格产值"]) + dec_总合格产值;
                }
            }
            textBox1.Text = dec_总制造产值.ToString();
            textBox2.Text = dec_总合格产值.ToString();   
        }
          //导出/
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
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

#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

    }
}
