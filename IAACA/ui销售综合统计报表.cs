using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace IAACA
{
    public partial class ui销售综合统计报表 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        public ui销售综合统计报表()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue).Date;
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
            string s = string.Format(@"select  * from [V_销售综合统计报表] where 日期>'{0}' and 日期<'{1}' order by 日期,销售部门,客户名",t1,t2);
            DataTable dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gridControl1.DataSource = dtM;
        }
        private void fun_check()
        {


        }

        private void ui销售综合统计报表_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";

            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(panel2, this.Name, cfgfilepath);

            DateTime time = CPublic.Var.getDatetime().Date;
            DateTime time1 = time.AddMonths(-3);
            time1 = new DateTime(time1.Year, time1.Month, 1);
            
            dateEdit1.EditValue = time1;
            dateEdit2.EditValue = time;



        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "导出Excel",
                    Filter = "Excel文件(*.xlsx)|*.xlsx"
                };
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message, "");
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
