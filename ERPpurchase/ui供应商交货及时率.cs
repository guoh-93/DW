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
    public partial class ui供应商交货及时率 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        DateTime t_today = CPublic.Var.getDatetime().Date;

        public ui供应商交货及时率()
        {
            InitializeComponent();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void ui供应商交货及时率_Load(object sender, EventArgs e)
        {
            string s = "select   供应商ID,供应商名称  from  采购供应商表  where   供应商状态='在用' order by 供应商ID ";
                    DataTable dt =CZMaster.MasterSQL.Get_DataTable(s,strcon);
            searchLookUpEdit1.Properties.DataSource=dt;
            searchLookUpEdit1.Properties.DisplayMember="供应商名称";
            searchLookUpEdit1.Properties.ValueMember="供应商ID"; 
        }
        //月
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            fun_load(t_today.AddMonths(-1));
        }
        //导出
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();

                gridView1.ExportToXlsx(saveFileDialog.FileName);

                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //季度
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            fun_load(t_today.AddMonths(-3));

        }
        //半年
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            fun_load(t_today.AddMonths(-6));

        }
        //年
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            fun_load(t_today.AddYears(-1));

        }
        private void fun_load(DateTime t)
        {
            string s="";
            if (searchLookUpEdit1.EditValue != null && searchLookUpEdit1.EditValue.ToString() != "")
            {
                s = string.Format("a.供应商ID='{0}' and ", searchLookUpEdit1.EditValue.ToString());
            }


            string sql = string.Format(@" select  供应商名称,b.*,原ERP物料编号,物料名称,规格型号,图纸编号,计量单位   from 采购供应商表 a 
  left  join ( select  采购明细号,物料编码,采购数量,到货日期 as 要求到货日期,明细完成日期 as 完成日期,
  case when(明细完成日期< DATEADD(DAY,1,到货日期)) then '是' else '否' end as 是否及时,供应商ID
   from 采购记录采购单明细表  where 生效日期>'{1}')b on  b.供应商ID=a.供应商ID 
   left join 基础数据物料信息表 c on c.物料编码 =b.物料编码
   where {0} a.供应商ID in (select  供应商ID  from 采购记录采购单明细表 where 生效日期 >'{1}'  group by 供应商ID )
   order by 供应商名称  ",s,t);




            dtM = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                da.Fill(dtM);
                gridControl1.DataSource = dtM;
            }
            textBox1.Text=dtM.Rows.Count.ToString();
            DataView dv = new DataView(dtM);
            dv.RowFilter = "是否及时='是'";
            textBox2.Text =dv.ToTable().Rows.Count.ToString();
            textBox4.Text = (dtM.Rows.Count-dv.ToTable().Rows.Count).ToString();
            if (dtM.Rows.Count == 0)
            {
                textBox3.Text ="0";
            }
            else
            {
                textBox3.Text = (Math.Round((decimal)dv.ToTable().Rows.Count/(decimal)dtM.Rows.Count*100,2)).ToString() + "%";
            }

             



        }
 

      
    }
}
