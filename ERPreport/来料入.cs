using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPreport
{
    public partial class 来料入 : Form
    {
        public 来料入()
        {
            InitializeComponent();
        }
        DataTable dt_main;


        public 来料入(object a )
        {
            InitializeComponent();
            dt_main = (DataTable)a;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            fun();
            this.reportViewer1.RefreshReport();
        }

        string HJ = "";
        private void fun()
        {

            string sql = string.Format("select 原ERP物料编号,n原ERP规格型号,计量单位,货架描述 from 基础数据物料信息表 where 物料编码='{0}'", dt_main.Rows[0]["产品编号"].ToString());
            DataRow dr1 = CZMaster.MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
        HJ = dr1["货架描述"].ToString();
            if (HJ=="")
            {
                HJ = "无";
            }


            DataTable dt_表体2 = new DataTable();


            dt_表体2.Columns.Add("供货单位", typeof(string));
            dt_表体2.Columns.Add("订单号", typeof(string));
            dt_表体2.Columns.Add("物料编码", typeof(string));
            dt_表体2.Columns.Add("检验单号", typeof(string));
            dt_表体2.Columns.Add("送检单号", typeof(string));
            dt_表体2.Columns.Add("计量单位", typeof(string));
            dt_表体2.Columns.Add("物料名称", typeof(string));
            dt_表体2.Columns.Add("单位", typeof(string));
            dt_表体2.Columns.Add("送检数量", typeof(decimal));
            dt_表体2.Columns.Add("合格数量", typeof(decimal));
            dt_表体2.Columns.Add("入库数量", typeof(decimal));
            dt_表体2.Columns.Add("规格型号", typeof(string));
            dt_表体2.Columns.Add("检验人员", typeof(string));
            dt_表体2.Columns.Add("入库人员", typeof(string));
            dt_表体2.Columns.Add("送检部门", typeof(string));
            dt_表体2.Columns.Add("送检人员", typeof(string));
            dt_表体2.Columns.Add("检验结论", typeof(string));



            dt_main.Columns.Add("计量单位");
            dt_main.Columns.Add("检验结论");
            // 规格型号
            try
            {
                foreach (DataRow dr in dt_main.Rows)
                {
                    DataRow drr = dt_表体2.NewRow();
                    dt_表体2.Rows.Add(drr);
                    drr["送检单号"] = dr["送检单号"].ToString();
                  //  drr["送检单明细号"] = dr["送检单明细号"].ToString();
                    drr["物料名称"] = dr["产品名称"].ToString();
                    drr["规格型号"] = dr["规格型号"].ToString();
                    drr["物料名称"] = dr["产品名称"].ToString();
                //   drr["订单号"] = dr["订单号"].ToString();
                    drr["供货单位"] = dr["供应商名称"].ToString();
                    drr["订单号"] = dr["采购单号"].ToString();
                    drr["物料编码"] = dr["产品编号"].ToString();
                    drr["检验单号"] = dr["检验记录单号"].ToString();


                    drr["送检数量"] = dr["送检数量"].ToString();
                    decimal dec = Convert.ToDecimal(dr["送检数量"]) - Convert.ToDecimal(dr["不合格数量"]);

                    if (dr["检验结果"].ToString() == "合格" || dr["检验结果"].ToString() == "免检")
                    {

                        //入库数量 若抽检 入库数=送检数 全检 入库数=送检数-不合格数 
                        if (dr["数量标记"].Equals(true)) //全检
                        {

                            drr["入库数量"] = dec.ToString("0.######");  // 入库数量=送检数量-不合格数量
                        }
                        else
                        {

                            drr["送检数量"] = dr["送检数量"].ToString();
                        }

                        drr["检验结论"] = dr["检验结果"].ToString();
                    }
                    else
                    {
                        // drr["送检数量"] = 0;

                        string sql_1 = string.Format(@"select * from 检验上传表单记录表
                                   where 采购入库通知单号='{0}' and 表单类型='不合格品评审单'", dr["送检单号"].ToString());
                        System.Data.DataTable dt_1 = CZMaster.MasterSQL.Get_DataTable(sql_1, CPublic.Var.strConn);
                        if (dt_1.Rows.Count > 0)
                        {

                            drr["检验结论"] = "评审后合格";

                            if (dr["数量标记"].Equals(true)) //全检
                            {

                                drr["入库数量"] = dec.ToString("0.######");  // 入库数量=送检数量-不合格数量
                            }
                            else
                            {

                                drr["送检数量"] = decimal.Parse(dr["送检数量"].ToString()).ToString("0.######");
                            }


                        }
                        else
                        {

                            drr["检验结论"] = "不合格";
                        }
                    }

                    drr["检验人员"] = dr["检验员"].ToString();
                    drr["送检人员"] = dr["送检人"].ToString();
                    drr["计量单位"] = dr1["计量单位"].ToString();

                    //range.Value2 = dr1["原ERP物料编号"].ToString();

                    //range.Value2 = dr1["n原ERP规格型号"].ToString();



                    // range.Value2 = dr1["货架描述"].ToString();


                }
                string time = "";

                time = dt_main.Rows[0]["检验日期"].ToString();

                DataTable1BindingSource.DataSource = dt_表体2;
                reportViewer1.LocalReport.ReportPath = "Report来料入.rdlc";

                List<ReportParameter> lstParameter = new List<ReportParameter>() {
               //     new ReportParameter("货架描述",HJ),       

                    //  new ReportParameter("供货单位",dt_main.Rows[0]["供应商名称"].ToString()),  
                    //new ReportParameter("图号或规格",dt_main.Rows[0]["规格型号"].ToString()),  
                    //  new ReportParameter("送检数量",dt_main.Rows[0]["送检数量"].ToString()),  
                    //  new ReportParameter("入库通知单号",dt_main.Rows[0]["送检单号"].ToString()),  
                    //  new ReportParameter("结论",dt_main.Rows[0]["检验结果"].ToString()),  
                    //         new ReportParameter("检验员",dt_main.Rows[0]["检验员"].ToString()),  
                new ReportParameter("检验日期",  dt_main.Rows[0]["检验日期"].ToString()),
                     
         
            
            
            
            };


                //PageSettings pages = new System.Drawing.Printing.PageSettings();
                //pages.Landscape = false;//强制设置纵向打印
                //reportViewer1.SetPageSettings(pages);
                //reportViewer1.RefreshReport();

                //  this.reportViewer1
                this.reportViewer1.LocalReport.SetParameters(lstParameter);
                this.reportViewer1.ZoomMode = ZoomMode.Percent;
                this.reportViewer1.ZoomPercent = 100;


                //  this.reportViewer1.RefreshReport();
                this.reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                //缩放模式为百分比,以100%方式显示
                this.reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                this.reportViewer1.ZoomPercent = 100;

                System.Drawing.Printing.PageSettings pg = new System.Drawing.Printing.PageSettings();
                pg.Margins.Top = 0;
                pg.Margins.Bottom = 0;

                pg.Margins.Left = 0;
                pg.Margins.Right = 0;
                pg.Landscape = false;

                //System.Drawing.Printing.PaperSize size = new PaperSize("报表", 857, 500);
                // If you need A5 size then try like below      //size.RawKind = (int)PaperKind.A5;
                // pg.PaperSize = size;

                //PrintDocument pd = new PrintDocument();
                PaperSize p = null;
                foreach (PaperSize ps in pg.PrinterSettings.PaperSizes)
                {
                    if (ps.PaperName.Equals("用友半"))
                        p = ps;
                }
                pg.PaperSize = p;

                //pg.PaperSize = size;

                this.reportViewer1.SetPageSettings(pg);
                this.reportViewer1.RefreshReport();


                reportViewer1.RefreshReport();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }




    }
}
