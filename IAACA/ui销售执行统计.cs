﻿using System;
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
    public partial class ui销售执行统计 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";

        public ui销售执行统计()
        {
            InitializeComponent();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                //DataTable tt = dtM.Copy();
                //tt.Columns.Remove("作废");
                gridControl1.ExportToXlsx(saveFileDialog.FileName);
                //ERPorg.Corg.TableToExcel(tt, saveFileDialog.FileName);
                MessageBox.Show("导出成功");
            }
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void ui销售执行统计_Load(object sender, EventArgs e)
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
            dateEdit2.EditValue = t;
            dateEdit1.EditValue = Convert.ToDateTime(t.AddMonths(-1).ToString("yyyy-MM-dd"));
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = string.Format(" exec xszxtj '{0}' ,'{1}'", dateEdit1.EditValue, Convert.ToDateTime(dateEdit2.EditValue).AddDays(1));
                DataTable dt_显示 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gridControl1.DataSource = dt_显示;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
