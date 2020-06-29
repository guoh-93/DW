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

namespace ERPpurchase
{
    public partial class ui供应商采购入库统计 : UserControl
    {
        string cfgfilepath = "";
        string strconn = CPublic.Var.strConn;
        DataTable dt_数据 = new DataTable();
        public ui供应商采购入库统计()
        {
            InitializeComponent();
        }

        private void ui供应商采购入库统计_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(this.panel1, this.Name, cfgfilepath);
            DateTime t = CPublic.Var.getDatetime();
            barEditItem4.EditValue = Convert.ToDateTime(t.ToString("yyyy-MM-dd"));
            barEditItem3.EditValue = Convert.ToDateTime(t.AddMonths(-1).ToString("yyyy-MM-dd"));
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string sql = string.Format(@"SELECT 
                                               采购记录采购单入库明细.供应商
                                              ,采购记录采购单入库明细.物料编码
                                              ,采购记录采购单入库明细.物料名称
                                              ,采购记录采购单入库明细.规格型号
                                              ,入库单号
                                              ,入库明细号
                                              ,入库量
                                              ,采购记录采购单入库明细.采购单号
                                              ,采购记录采购单入库明细.采购单明细号
                                              ,采购记录采购单明细表.采购数量
                                               ,采购记录采购单明细表.完成数量
                                                ,采购记录采购单明细表.未完成数量
                                                ,采购记录采购单明细表.已送检数
                                              ,采购记录采购单入库明细.检验记录单号
                                              ,采购记录采购单检验主表.送检数量
                                              ,采购记录采购单检验主表.不合格数量
                                              ,采购记录采购单检验主表.待用
                                              
                                              ,采购记录采购单入库明细.生效日期
                                             ,已开票量
                                          FROM 采购记录采购单入库明细,采购记录采购单检验主表,采购记录采购单明细表
                                          where  采购记录采购单入库明细.检验记录单号 = 采购记录采购单检验主表.检验记录单号
                                          and 采购记录采购单入库明细.采购单明细号 = 采购记录采购单明细表.采购明细号 and 采购记录采购单入库明细.生效日期>='{0}' and 采购记录采购单入库明细.生效日期<='{1}' 
                                          order by 供应商 ", barEditItem3.EditValue, barEditItem4.EditValue);
                dt_数据 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                dt_数据.Columns.Add("合格率", typeof(string));
                decimal j =0;
                foreach (DataRow dr in dt_数据.Rows)
                {
                    j = Convert.ToDecimal(dr["不合格数量"]);
                   
                    dr["合格率"] = Math.Round((100-Convert.ToDecimal(j / Convert.ToDecimal(dr["送检数量"])) * 100), 2, MidpointRounding.AwayFromZero) + "%";
                }
                gridControl1.DataSource = dt_数据;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (dt_数据 == null || dt_数据.Columns.Count == 0 || dt_数据.Rows.Count == 0)
                {

                    throw new Exception("没有数据可以导出");
                }

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
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
