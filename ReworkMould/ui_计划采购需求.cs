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


namespace ReworkMould
{
    public partial class ui_计划采购需求 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dt_计划单;
        DataTable dt_计划采购需求;
        bool bl_选择 = false;
        public ui_计划采购需求()
        {
            InitializeComponent();
        }

        private void ui_计划采购需求_Load(object sender, EventArgs e)
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
                DateTime t1 = CPublic.Var.getDatetime().Date.AddMonths(-3);
                DateTime t2 = CPublic.Var.getDatetime();

                barEditItem1.EditValue = t1;
                barEditItem2.EditValue = t2;

                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {
            DateTime t1 = Convert.ToDateTime(barEditItem1.EditValue);
            DateTime t2 = Convert.ToDateTime(barEditItem2.EditValue);
            if (t1 > t2)
            {
                throw new Exception("时间输入有误，请确认");
            }
            string sql = $"select *  from 主计划计划生成单 where 计划生成日期>='{t1}' and 计划生成日期 <='{t2}'";
            dt_计划单 = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl2.DataSource = dt_计划单;
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

       


        private void barLargeButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // if (bl_刷新) throw new Exception("正在查询数据,稍候再试");
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                    //gridControl1.ExportToXls(saveFileDialog.FileName, options);  
                     
                    gridControl2.ExportToXlsx(saveFileDialog.FileName, options);
                        //ERPorg.Corg.TableToExcel(dt_订单明细, saveFileDialog.FileName);
                    
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

       
      
    }
}
