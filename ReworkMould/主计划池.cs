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



namespace ReworkMould
{
    public partial class 主计划池 : UserControl
    {
        string cfgfilepath = "";
        string strconn = CPublic.Var.strConn;
        DataTable dt_mx;
        bool bl_选择 = false;
        public 主计划池()
        {
            InitializeComponent();
        }

        private void 主计划池_Load(object sender, EventArgs e)
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
                x.UserLayout(panel1, this.Name, cfgfilepath);
                DateTime t1 = CPublic.Var.getDatetime();
                DateTime t2 = CPublic.Var.getDatetime().Date.AddMonths(3);
                barEditItem3.EditValue = t1;
                barEditItem4.EditValue = t2;
                fun_load();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

           
        }

        private void fun_load()
        {
            DateTime t1 = Convert.ToDateTime(barEditItem3.EditValue);
            t1 = new DateTime(t1.Year, t1.Month, t1.Day);
            DateTime t2 = Convert.ToDateTime(barEditItem4.EditValue).AddDays(1).AddSeconds(-1);
            t2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
            if (t2 < t1)
            {
                throw new Exception("结束时间需大于开始时间！");
            }
            string sql = string.Format("select * from V_主计划池 where 已转数量<未完成数量 and 预计发货日期 >='{0}' and 预计发货日期 <='{1}' order by 下单日期 desc", t1,t2);
            dt_mx = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
            DataColumn dc = new DataColumn("选择", typeof(bool));
            dc.DefaultValue = false;

            dt_mx.Columns.Add(dc);
            
            gridControl1.DataSource = dt_mx;
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             
            try
            {
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();
                DataTable t = new DataTable(); //用户选择的销售订单
                DataView dv_1 = new DataView(dt_mx.Copy());
                dv_1.RowFilter = "选择=1";
                t = dv_1.ToTable();
                if (t.Rows.Count==0)
                {
                    throw new Exception("未勾选明细,请确认");
                }
                //Form1 fm = new Form1();
                //ui主计划生成单 ui = new ui主计划生成单(t);
                //fm.Controls.Add(ui);
                //fm.Text = "主计划生产单";
                //fm.WindowState = FormWindowState.Maximized;
                //ui.Dock = DockStyle.Fill;
                //fm.ShowDialog();
                //if (ui.bl)
                //{
                //    barLargeButtonItem4_ItemClick(null, null);
                //}
                ui主计划生成单 frm = new ui主计划生成单(t);
                CPublic.UIcontrol.Showpage(frm, "申请明细");



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
             
        }

        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions(DevExpress.XtraPrinting.TextExportMode.Text, false, false);

                    gridControl1.ExportToXlsx(saveFileDialog.FileName, options);

                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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
                if (!bl_选择)
                {
                    foreach (DataRow dr in dt_mx.Rows)
                    {
                        dr["选择"] = true;
                    }
                    bl_选择 = true;
                }
                else
                {
                    foreach (DataRow dr in dt_mx.Rows)
                    {
                        dr["选择"] = false;
                    }
                    bl_选择 = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
