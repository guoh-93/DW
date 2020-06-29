using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace ERPpurchase
{
    public partial class ui采购订单达成率 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        DataTable dtM = new DataTable();

        public ui采购订单达成率()
        {
            InitializeComponent();
        }
        string cfgfilepath = ""; 
        private void ui采购订单达成率_Load(object sender, EventArgs e)
        {
            try
            {    
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(panel2, this.Name, cfgfilepath);
                DateTime t1 = CPublic.Var.getDatetime().Date;
                DateTime t2 = t1.AddMonths(-3);
                dateEdit3.EditValue = t2;
                dateEdit4.EditValue = t1;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        private string fun_check()
        {
            string s = "";

            if (checkBox3.Checked == true)
            {
                if (dateEdit3.EditValue == null || dateEdit4.EditValue == null || dateEdit3.EditValue.ToString() == "" || dateEdit4.EditValue.ToString() == "")
                {
                    throw new Exception("未选择要求交货日期");
                }
                DateTime t1 = Convert.ToDateTime(dateEdit3.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(dateEdit4.EditValue).Date.AddDays(1).AddSeconds(-1);
                // //预计到货日期 <--> 到货日期
                s = string.Format(" and 预计到货日期>'{0}' and 预计到货日期<'{1}'", t1, t2);

            }

            if (checkBox1.Checked == true)
            {
                s = s + string.Format(" and dd.供应商ID='{0}'", searchLookUpEdit1.EditValue.ToString());
            }

            return s;
        }
        private void fun_load(string s_条件)
        {
            //预计到货日期 <--> 到货日期

            string s = string.Format(@"select  x.采购明细号,x.物料编码,x.物料名称,x.供应商 ,x.备注,x.采购数量,x.明细完成,x.明细完成日期,x.预计到货日期
                ,x.生效日期 as 审核日期,dd.采购单类型,case when(预计到货日期+1>明细完成日期) then CONVERT(bit,1) else CONVERT(bit,0) end as 按期到货   from 采购记录采购单明细表 x 
                left join 基础数据物料信息表 on 基础数据物料信息表.物料编码 = x.物料编码 
                left join 采购记录采购单主表 dd on dd.采购单号 = x.采购单号
                where x.作废=0 and dd.作废=0 and x.生效日期>'2019-5-1' {0}", s_条件);
            dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gcP.DataSource = dtM;

            DataView dv = new DataView(dtM);
            dv.RowFilter = "按期到货=1";
            decimal dec_总 = dtM.Rows.Count;
            decimal dec_按期=dv.Count;
            label1.Text = dec_总.ToString();
            decimal dec_lv = 0;
            if (dec_总 > 0) dec_lv = dec_按期 / dec_总 * 100;
            label2.Text = dec_按期.ToString();
            label4.Text = Math.Round(dec_lv, 2) + "%";




        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string s = fun_check();
                fun_load(s);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
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
 
                    gcP.ExportToXlsx(saveFileDialog.FileName);
          
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
