using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPStock
{
    public partial class ui工时统计 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DataTable dt_下拉;
        string strConn_4 = "password={0};persist Security Info=True;User ID={1};Initial Catalog={2};Data Source={3}";
        DataTable dt_考勤;
        public ui工时统计()
        {
            InitializeComponent();

        }

        private void ui工时统计_Load(object sender, EventArgs e)
        {     
          
           

            DateTime time = CPublic.Var.getDatetime();
            time = new DateTime(time.Year, time.Month, time.Day);
            DateTime t2 = new DateTime(time.Year, time.Month, 1);
            dateEdit1.EditValue = t2;
            dateEdit2.EditValue = time;
            fun_load();

        }
        private void fun_load()
        {
            string sql = "select 属性值 as 车间 ,属性字段1 as 车间编号 from 基础数据基础属性表 where  属性类别='课室' and 属性字段1 <>'' ";
            dt_下拉 = new DataTable();
            dt_下拉 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            searchLookUpEdit1.Properties.DataSource = dt_下拉;
            searchLookUpEdit1.Properties.DisplayMember = "车间";
            searchLookUpEdit1.Properties.ValueMember = "车间";

        }
        private void fun_search()
        {
            strConn_4 = string.Format(strConn_4, "a", "sa", "szfuture1", "192.168.10.4");
            string sql_4 = string.Format("select badgenumb,username,sum(cq)出勤 from KQ_RBB where KQDate >'{0}' and KQDate<'{1}' group by username,badgenumb", dateEdit1.EditValue, Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1));
            dt_考勤 = new DataTable();
            dt_考勤 = CZMaster.MasterSQL.Get_DataTable(sql_4, strConn_4);


            string sql = string.Format(@"select a.*,姓名 as 生产人员,班组,课室 as 生产车间  from (
                 select  ROUND(sum(入库数量/基础数据物料信息表.工时*8),2) as 产出工时,COUNT(*)单数,负责人员ID from 生产记录成品入库单明细表
                 left join 生产记录生产检验单主表 on 生产记录成品入库单明细表.生产工单号=生产记录生产检验单主表.生产工单号
                 left join 基础数据物料信息表 on 基础数据物料信息表.物料编码=生产记录成品入库单明细表.物料编码
	             where 生产记录成品入库单明细表.生效日期>'{0}' and 生产记录成品入库单明细表.生效日期<'{1}' and 基础数据物料信息表.工时>0 group by 负责人员ID) a ,人事基础员工表 
	             where a.负责人员ID=人事基础员工表.员工号  ", dateEdit1.EditValue, Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1));

            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                sql = sql + string.Format(" and 课室='{0}'", searchLookUpEdit1.EditValue.ToString().Trim());

            }
            dtM = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dtM);
            }


            dtM.Columns.Add("出勤",typeof(decimal))   ;
            dtM.Columns.Add("产出比", typeof(decimal));

            foreach (DataRow dr in dtM.Rows)
            {
              DataRow []r = dt_考勤.Select(string.Format("badgenumb='{0}'", dr["负责人员ID"]));
              if (r.Length == 0)
              {
                  dr["出勤"] = 0;
                  //dr["产出比"] = Convert.ToDecimal(dr["产出工时"]) / Convert.ToDecimal(dr["出勤"]);
                  
              }
              else
              {
                  dr["出勤"] = r[0]["出勤"];
                  if (Convert.ToDecimal(dr["出勤"]) != 0)
                  {
                      dr["产出比"] = Convert.ToDecimal(dr["产出工时"]) / Convert.ToDecimal(dr["出勤"]);
                  }
              }
              //dr["产出比"] = Convert.ToDecimal(dr["产出工时"]) / Convert.ToDecimal(dr["出勤"]);

            }

            gridControl1.DataSource = dtM;

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                //throw new Exception("出勤数据不知道去哪取,暂未启用");
                //fun_search();
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
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

    }
}
