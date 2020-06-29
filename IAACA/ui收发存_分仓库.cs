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
    public partial class ui收发存_分仓库 : UserControl
    {
        string strcon = CPublic.Var.strConn;

        string cfgfilepath = "";

        public ui收发存_分仓库()
        {
            InitializeComponent();
        }

        private void ui收发存_分仓库_Load(object sender, EventArgs e)
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

                DateTime t_now = CPublic.Var.getDatetime();
                int xx = 2019;// 系统开始使用时间
                for (int i = xx; i <= t_now.Year; i++)
                {
                    comboBox1.Items.Add(i.ToString());
                    comboBox3.Items.Add(i.ToString());
                }
                for (int i = 1; i <= 12; i++)
                {
                    comboBox2.Items.Add(i.ToString());
                    comboBox4.Items.Add(i.ToString());
                }

                DateTime t1 = t_now.AddMonths(-1);
                comboBox1.Text = t1.Year.ToString();
                comboBox2.Text = t1.Month.ToString();
                comboBox3.Text = t1.Year.ToString();
                comboBox4.Text = t1.Month.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           



        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime t1 = new DateTime(Convert.ToInt32( comboBox1.Text), Convert.ToInt32(comboBox2.Text),1);
                DateTime t2 = new DateTime(Convert.ToInt32(comboBox3.Text), Convert.ToInt32(comboBox4.Text), 1);
                t2 = t2.AddMonths(1).AddSeconds(-1);

                string s = $"exec [sfc_fck] '{t1}','{t2}'";
                DataTable t = CZMaster.MasterSQL.Get_DataTable(s,strcon);

                gridControl1.DataSource = t;
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

                gridControl1.ExportToXlsx(saveFileDialog.FileName);

                MessageBox.Show("导出成功");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
