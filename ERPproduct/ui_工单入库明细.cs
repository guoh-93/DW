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

namespace ERPproduct
{
    public partial class ui_工单入库明细 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dtM;
        public ui_工单入库明细()
        {
            InitializeComponent();
        }

        private void ui_工单入库明细_Load(object sender, EventArgs e)
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
                DateTime t1 = CPublic.Var.getDatetime().Date.AddMonths(-1);
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
            string sql = $@"with t as (select  相关单号,sum(实效数量)入库数量  from 仓库出入库明细表  where 明细类型 ='生产入库' 
                        and 出入库时间 >'{t1.Date}' and 出入库时间 <'{t2.Date.AddDays(1)}' group by 相关单号)
                        select  t.相关单号 as 生产工单号,b.物料编码 ,b.物料名称,b.规格型号,b.班组,t.入库数量,生产工单类型  from t 
                        left join 生产记录生产工单表 b on t.相关单号=b.生产工单号  ";
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, strcon);
            gridControl1.DataSource = dtM;
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

                    gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                    //ERPorg.Corg.TableToExcel(dt_订单明细, saveFileDialog.FileName);

                    DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
