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
namespace ItemInspection
{
    public partial class ui来料检验报表 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        public ui来料检验报表()
        {
            InitializeComponent(); 
        }

        private void ui来料检验报表_Load(object sender, EventArgs e)
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
            barEditItem1.EditValue = t.AddMonths(-1);
            barEditItem2.EditValue = t;
            
        }

        
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //加载检验明细表

                DataTable dt_主 = new DataTable();
                string sql = @"select a.*,b.送检单号 from 采购记录采购单检验主表 b
                               left join 采购记录采购单检验明细表 a on a.检验记录单号 = b.检验记录单号 where b.关闭 = 0";
                dt_主 = CZMaster.MasterSQL.Get_DataTable(sql, strconn);
                //检验主表 并搭建报表结构
                string stre = string.Format(@"select  a.*,case when b.检验水平 IS null then  '抽检' else   b.检验水平 end as 检验水平,存货分类,大类,小类,物料等级,确认到货日期  from (
                  select 送检单号,供应商编号,供应商名称,产品编号,产品名称,规格型号,sum(送检数量)送检数量,SUM(抽检数量)抽检数量,SUM(不合格数量)不合格数量,SUM(送检数量-不合格数量)合格数量,待用 as 代用,二次检验 as 二次加工 from 采购记录采购单检验主表
                  where 检验日期 >'{0}'  and 检验日期 <='{1}' and 关闭 = 0  group by 送检单号,供应商编号,供应商名称,产品编号,产品名称,规格型号,待用,二次检验)a
                 left join ( select b.送检单号 ,检验水平 from 采购记录采购单检验明细表 a
                             left join 采购记录采购单检验主表 b on a.检验记录单号=b.检验记录单号
                             where 检验水平 ='全检' and a.检验日期>'{0}' and  a.检验日期<'{1}' group by b.送检单号,检验水平)b 
                 on a.送检单号=b.送检单号 
                 left join 基础数据物料信息表 base on base.物料编码=a.产品编号 
                 left join 采购记录采购送检单主表 sjz on sjz.送检单号=a.送检单号", 
                 barEditItem1.EditValue, Convert.ToDateTime(barEditItem2.EditValue).AddDays(1).AddSeconds(-1));

                DataTable dtM = CZMaster.MasterSQL.Get_DataTable(stre, strconn);
                dtM.Columns.Add("功能不良数", typeof(decimal));
                dtM.Columns.Add("功能不良率", typeof(string));
                dtM.Columns.Add("外观不良数", typeof(decimal));
                dtM.Columns.Add("外观不良率", typeof(string));
                dtM.Columns.Add("检验合格率", typeof(string));
                dtM.Columns.Add("不合格原因", typeof(string));
                dtM.Columns.Add("备注", typeof(string));
                decimal j = 0;
                decimal k = 0;

                foreach (DataRow dr in dtM.Rows)     
                {
                    if(Convert.ToBoolean(dr["代用"])== true|| Convert.ToBoolean(dr["二次加工"]) == true)
                    {
                        dr["不合格数量"] = Convert.ToDecimal(dr["送检数量"]);
                        dr["合格数量"] = 0;
                    }
                    if(dr["检验水平"].ToString() == "全检")
                    {
                        dr["抽检数量"] = Convert.ToDecimal(dr["送检数量"]);
                    }
                    DataRow[] r = dt_主.Select(string.Format("送检单号='{0}'", dr["送检单号"]));
                    

                    foreach (DataRow rt in r)
                    {
                        if (rt["检验项目"].ToString() == "外观" && Convert.ToDecimal(rt["不合格数量"]) != 0)
                        { 
                            j += Convert.ToDecimal(rt["不合格数量"]);
                            dr["外观不良数"] = j;
                            dr["外观不良率"] = Math.Round(Convert.ToDecimal(j / Convert.ToDecimal(dr["送检数量"]) * 100), 2, MidpointRounding.AwayFromZero) + "%";
                        }
                        if (rt["检验项目"].ToString() == "性能" && Convert.ToDecimal(rt["不合格数量"]) != 0)
                        {
                            k += Convert.ToDecimal(rt["不合格数量"]);
                            dr["功能不良数"] = k;
                            dr["功能不良率"] = Math.Round(Convert.ToDecimal(k / Convert.ToDecimal(dr["送检数量"]) * 100), 2, MidpointRounding.AwayFromZero) + "%";
                        }

                        if (rt["不合格原因"].ToString() != "尺寸不准" && rt["不合格原因"].ToString() != "")
                        {
                            if (dr["不合格原因"].ToString() == "")
                            {
                                dr["不合格原因"] = rt["不合格原因"].ToString();
                            }
                            else
                            {
                                dr["不合格原因"] = dr["不合格原因"].ToString() + "," + rt["不合格原因"].ToString();
                            }

                        }

                        if (rt["备注"].ToString() != "")
                        {
                            if (dr["备注"].ToString() == "")
                            {
                                dr["备注"] = rt["备注"].ToString();
                            }
                            else
                            {
                                dr["备注"] = dr["备注"].ToString() + "," + rt["备注"].ToString();
                            }

                        }

                    }
                    j = 0;
                    k = 0;


                    //检验合格率
                    //if (dr["检验水平"].ToString() == "全检")
                    //{
                    //    dr["检验合格率"] = Math.Round(Convert.ToDecimal(dr["不合格数量"]) / Convert.ToDecimal(dr["送检数量"]) * 100, 2, MidpointRounding.AwayFromZero) + "%";
                    //}
                    //else
                    //{
                    //    dr["检验合格率"] = Math.Round(Convert.ToDecimal(dr["不合格数量"]) / Convert.ToDecimal(dr["抽检数量"]) * 100, 2, MidpointRounding.AwayFromZero) + "%";
                    //}

                    if (Convert.ToDecimal(dr["不合格数量"]) == 0)
                    {
                        dr["检验合格率"] = "100%";
                    }
                    else
                    {
                        //if (Convert.ToDecimal(dr["抽检数量"]) == 0)
                        //{
                        //    dr["检验合格率"] = Math.Round(100 - Convert.ToDecimal(dr["不合格数量"]) / Convert.ToDecimal(dr["送检数量"]) * 100, 2, MidpointRounding.AwayFromZero) + "%";
                        //}
                        //if (Convert.ToDecimal(dr["抽检数量"]) < Convert.ToDecimal(dr["不合格数量"]))
                        //{
                            dr["检验合格率"] = Math.Round(100 - Convert.ToDecimal(dr["不合格数量"]) / Convert.ToDecimal(dr["送检数量"]) * 100, 2, MidpointRounding.AwayFromZero) + "%";
                        //}
                        //else
                        //{
                        //    dr["检验合格率"] = Math.Round(100 - Convert.ToDecimal(dr["不合格数量"]) / Convert.ToDecimal(dr["抽检数量"]) * 100, 2, MidpointRounding.AwayFromZero) + "%";
                        //}
                    }
                }

                //MasterMESWS.DataSetHelper RBQ = new MasterMESWS.DataSetHelper();
                //DataTable dt_SaleOrder_1 = RBQ.SelectGroupByInto("", dtM, "送检单号,sum(送检数量)送检数量,sum(抽检数量)抽检数量,sum(不合格数量)不合格数量", "", "送检单号");



                gridControl1.DataSource = dtM;
            
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
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "导出Excel";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions();
                options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
                gridControl1.ExportToXlsx(saveFileDialog.FileName, options);
                DevExpress.XtraEditors.XtraMessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
