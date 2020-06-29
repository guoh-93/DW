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
    public partial class ui收发存汇总 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        public ui收发存汇总()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {

                DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).Date.AddDays(1).AddSeconds(-1);
                string s = string.Format("exec sfchzb '{0}',{1},{2},'{3}',{4},{5}",t1,t1.Year,t1.Month,t2,t2.Year,t2.Month);
                DataTable dt = CZMaster.MasterSQL.Get_DataTable(s,strcon);
                gridControl1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ui收发存汇总_Load(object sender, EventArgs e)
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

                DateTime t = CPublic.Var.getDatetime().Date;
                t = new DateTime(t.Year, t.Month, 1);
                DateTime t1 = new DateTime(t.Year, t.Month, 1).AddSeconds(-1);
                DateTime t2 = t.AddMonths(-1);
                dateEdit2.EditValue = t1;
                dateEdit1.EditValue = t2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
          
                gridControl1.ExportToXlsx(saveFileDialog.FileName);
               
                MessageBox.Show("导出成功");
            }
        }
    }
}
