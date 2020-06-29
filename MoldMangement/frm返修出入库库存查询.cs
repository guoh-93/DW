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
    public partial class frm返修出入库库存查询 : UserControl
    {
        DataTable dtM = new DataTable();
        string strcon = CPublic.Var.strConn;
        string strconn = CPublic.Var.strConn;
        DataTable dt;
        DataTable dt_仓库;
        public frm返修出入库库存查询()
        {
            InitializeComponent();
        }

        private void frm返修出入库库存查询_Load(object sender, EventArgs e)
        {
            //DateTime t = CPublic.Var.getDatetime().AddMonths(-1);
            //t = new DateTime(t.Year, t.Month, 1);   //去上月月初 一般财务是要上个月的 数据
            //dateEdit1.EditValue = t;
            //dateEdit2.EditValue = t.AddMonths(1).AddSeconds(-1);
            string sql_仓库 = "SELECT [属性值] as 仓库名称,属性字段1 as 仓库号 FROM [FMS].[dbo].[基础数据基础属性表] where 属性类别 ='仓库类别'";
            dt_仓库 = new DataTable();
            dt_仓库 = CZMaster.MasterSQL.Get_DataTable(sql_仓库, strconn);
            searchLookUpEdit2.Properties.DataSource = dt_仓库;
            searchLookUpEdit2.Properties.ValueMember = "仓库号";
            searchLookUpEdit2.Properties.DisplayMember = "仓库名称";
            string sql4 = "select 物料编码,物料名称,规格,原ERP物料编号,n原ERP规格型号 from 基础数据物料信息表 where 停用=0";
            DataTable dt_物料 = new DataTable();
            SqlDataAdapter da_物料 = new SqlDataAdapter(sql4, strconn);
            da_物料.Fill(dt_物料);
            searchLookUpEdit1.Properties.DataSource = dt_物料;
            searchLookUpEdit1.Properties.ValueMember = "物料编码";
            searchLookUpEdit1.Properties.DisplayMember = "原ERP物料编号";
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
            string sql = string.Format(@"select 基础数据物料信息表.*,返修仓库物料数量表.库存总数,有效总数,在途量,在制量,受订量,未领量,MRP计划采购量,MRP计划生产量,MRP库存锁定量,物品单价,成本 from 返修仓库物料数量表
  left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = 返修仓库物料数量表.物料编码 where 1=1");

            if (checkBox2.Checked == true)
            {
                sql = sql + string.Format("and 基础数据物料信息表.物料编码 = '{0}'", searchLookUpEdit1.EditValue.ToString());
            }
            if (checkBox1.Checked == true)
            {
                sql = sql + string.Format("and 基础数据物料信息表.仓库号='{0}'", searchLookUpEdit2.EditValue.ToString());
            }
            if (checkBox3.Checked == true)
            {
                sql = sql + string.Format("and 基础数据物料信息表.货架描述 like'%{0}%'", textBox1.Text.ToString());
            }
            using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
            {
                da.Fill(dtM);
            }
            gridControl1.DataSource = dtM;
        }


        private void fun_check()   
        {
            //if (dateEdit1.EditValue == null && dateEdit2.EditValue.ToString() == "")
            //{
            //    throw new Exception("请选择时间");
            //}
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

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            } 
        }
    }
}
