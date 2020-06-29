using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
namespace BaseData
{
    public partial class BOM修改查询 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_产品;
        DataTable dt_修改主;
        DataTable dt_修改子;
        public BOM修改查询()
        {
            InitializeComponent();
        }

        private void BOM修改查询_Load(object sender, EventArgs e)
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
                dateEdit1.EditValue = CPublic.Var.getDatetime().AddDays(-15).ToString("yyyy-MM-dd");
                dateEdit2.EditValue = CPublic.Var.getDatetime().ToString("yyyy-MM-dd");
                fun_下拉框();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void fun_下拉框()
        {
            string sql = @" select  物料编码 as 产品编码,物料名称 as 产品名称,规格型号 from 基础数据物料信息表 
                            where  物料编码 in  (select  产品编码 from 基础数据物料BOM表 group  by 产品编码)";
            dt_产品 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            searchLookUpEdit1.Properties.PopupFormMinSize = new Size(800, 400);
            searchLookUpEdit1.Properties.DataSource = dt_产品;
            searchLookUpEdit1.Properties.DisplayMember = "产品编码";
            searchLookUpEdit1.Properties.ValueMember = "产品编码";
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                fun_check();
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            DateTime t1 = Convert.ToDateTime(dateEdit1.EditValue);
            t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            DateTime t2 = Convert.ToDateTime(dateEdit2.EditValue).AddDays(1).AddSeconds(-1);
            t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
            string sql = string.Format(@"select * from 基础数据BOM修改主表 where 作废 = 0 and 修改日期>='{0}' and 修改日期<='{1}'", t1, t2);
            string sql_补 = "";
            if(checkBox1.Checked == true)
            {
                sql_补 = string.Format(" and 产品编码 = '{0}'",searchLookUpEdit1.EditValue.ToString());
                sql += sql_补;
            }
            if(checkBox2.Checked == true)
            {
                sql_补 = string.Format(" and BOM修改单号 = '{0}'", textBox2.Text);
                sql += sql_补;
            }
            dt_修改主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            gridControl1.DataSource = dt_修改主;
        }

        private void fun_check()
        {
            if (checkBox1.Checked == true)
            {
                if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "")
                {
                    throw new Exception("未选择产品编码");
                }
            }
            if (checkBox2.Checked == true)
            {
                if (textBox2.Text == null || textBox2.Text.ToString() == "")
                {
                    throw new Exception("未填写BOM修改单号");
                }
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {


                if (e != null && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                    gridView1.CloseEditor();
                   // this.BindingContext[dtM].EndCurrentEdit();

                }
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string sql = string.Format("select * from 基础数据BOM修改明细表 where BOM修改单号 = '{0}'", dr["BOM修改单号"].ToString());
                dt_修改子 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                gridControl2.DataSource = dt_修改子;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    gridControl1.ExportToXlsx(saveFileDialog.FileName);
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try

            {
                DataRow drM = (this.BindingContext[gridControl1.DataSource].Current as DataRowView).Row;
                if (bool.Parse( drM["审核"].ToString())==false )
                {

                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
