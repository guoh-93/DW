using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ERPSale
{
    public partial class ui销售历史单价查询 : UserControl
    {
        public ui销售历史单价查询()
        {
            InitializeComponent();
        }

        string strconn = CPublic.Var.strConn;
        DataTable dt_物料,dt_客户;

        private void simpleButton1_Click(object sender, EventArgs e)
        {

            try
            {
                //if (searchLookUpEdit1.EditValue.ToString() == "")
                //{

                //    throw new Exception("请选择物料");
                //}
                //
                DataTable dt = new DataTable();
                //string sql = string.Format("select * from  销售记录销售订单明细表 where 物料编码='{0}'  ", searchLookUpEdit1.EditValue.ToString());

//                string sql = string.Format(@"select a.成品出库单明细号,b.*,c.物料编码,c.物料名称,c.客户编号,c.客户,c.规格型号
//                         ,c.税后单价,convert(decimal,c.税率)税率 from 销售记录销售订单明细表 c
//                      left join 销售记录成品出库单明细表 a on a.销售订单明细号=c.销售订单明细号                  
//                      left join 销售记录销售开票明细表 b on  a.成品出库单明细号=b.成品出库单明细号
//                      where b.成品出库单明细号 in ( select 成品出库单明细号 from 销售记录销售开票明细表 group by 成品出库单明细号,产品编码,开票税后单价 ) and b.生效='1'");

                string sql = string.Format(@"select a.成品出库单明细号,b.*,c.物料编码,c.物料名称,c.客户编号,c.客户,c.规格型号,c.生效日期,c.税后单价,convert(decimal,c.税率)税率 from 销售记录销售订单明细表 c
       left join 销售记录成品出库单明细表 a on a.销售订单明细号=c.销售订单明细号  
         left join  (select 成品出库单明细号,开票税后单价  from 销售记录销售开票明细表 where 生效=1 group by 成品出库单明细号,开票税后单价 )b on a.成品出库单明细号=b.成品出库单明细号
      where 1=1  ");
//                string strrrr = @" select  rdrecords32.iordercode as 销售订单号,rdrecords32.iorderseq as 行号,a.cInvCode 物料编码,a.cInvName 物料名称,a.cInvStd 规格型号, rdrecords32.iQuantity as 销售数量,dDate  最近出库日期 FROM   rdrecords32 
// left join rdrecord32 on rdrecord32.ID=rdrecords32.ID 
// left join inventory a on a.cInvCode = rdrecords32.cInvCode
// where iordercode='DWXD2018010030' and iorderseq=2";


                if (checkBox2.Checked == true)
                {
                    sql = sql + string.Format(" and c.物料编码 = '{0}'", searchLookUpEdit1.EditValue.ToString());
                }

                if (checkBox1.Checked == true)
                {
                    sql = sql + string.Format(" and c.客户编号 = '{0}'", searchLookUpEdit2.EditValue.ToString());
                }
                if (checkBox4.Checked == true)
                {
                    sql = sql + string.Format(" and  c.生效日期>'{0}' and c.生效日期<'{1}'  ", dateEdit1.Text.ToString(), dateEdit2.Text.ToString());
                }
                using (SqlDataAdapter da = new SqlDataAdapter(sql, CPublic.Var.strConn))
                {
                    da.Fill(dt);
                }  
                gridControl1.DataSource = dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


           

        }

        private void 销售历史单价查询_Load(object sender, EventArgs e)
        {

            //this.gv.IndicatorWidth = 40;
            DateTime t = CPublic.Var.getDatetime().AddMonths(-1);
            t = new DateTime(t.Year, t.Month, 1);   //去上月月初 一般财务是要上个月的 数据
            dateEdit1.EditValue = t;
            dateEdit2.EditValue = t.AddMonths(1).AddSeconds(-1);
            dt_物料 = new DataTable();
            string sql = string.Format(@"select 物料编码,图纸编号,规格型号,物料名称  from 基础数据物料信息表 
                                where 物料编码 in(select 物料编码 from 销售记录销售订单明细表 group by 物料编码)");
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dt_物料 = new DataTable();
                da.Fill(dt_物料);
                searchLookUpEdit1.Properties.DataSource = dt_物料;
                searchLookUpEdit1.Properties.DisplayMember = "物料编码";
                searchLookUpEdit1.Properties.ValueMember = "物料编码";

            }



            dt_客户 = new DataTable();
            sql = string.Format(@"select 客户编号,客户名称,客户类型,客户简称,省,市,县,税率  from 客户基础信息表 
                                where 客户编号 in(select 客户编号 from 销售记录销售订单明细表 group by 客户编号)");
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                dt_客户 = new DataTable();
                da.Fill(dt_客户);
                searchLookUpEdit2.Properties.DataSource = dt_客户;
                searchLookUpEdit2.Properties.DisplayMember = "客户名称";
                searchLookUpEdit2.Properties.ValueMember = "客户编号";

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

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }//查询
    }
}
