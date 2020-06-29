using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace StockCore
{
    public partial class ECR变更申请查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_申请;
        public ECR变更申请查询()
        {
            InitializeComponent();
        }

        private void ECR变更申请查询_Load(object sender, EventArgs e)
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
                x.UserLayout(this.panel2, this.Name, cfgfilepath);
                DateTime t = CPublic.Var.getDatetime().Date;
                barEditItem2.EditValue = t.AddDays(1);
                barEditItem1.EditValue = t.AddMonths(-3);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            string sql = string.Format("select * from ECR变更申请单主表 where 申请日期>='{0}' and 申请日期<='{1}'",barEditItem1.EditValue.ToString(), barEditItem2.EditValue.ToString());
            dt_申请 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_申请;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load();
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

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (dr == null) return;
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 查询明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            StockCore.ui_ECR变更申请 ui = new ui_ECR变更申请(dr);
            CPublic.UIcontrol.Showpage(ui, "申请明细查询");
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";//txt_gysmc
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if(dr == null)
                {
                    throw new Exception("请选择明细");
                }
                string s_申请单号 = dr["申请单号"].ToString();

                string sql = string.Format("select * from ECR变更申请审核表 where 申请单号 = '{0}'", s_申请单号);
                DataTable dtP = CZMaster.MasterSQL.Get_DataTable(sql, strconn);

                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                
                if (dialogResult == DialogResult.OK)
                {                    
                    ItemInspection.print_FMS.fun_print_ECN变更申请(s_申请单号, dtP, dr,false, saveFileDialog.FileName);
                    MessageBox.Show("ok");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
