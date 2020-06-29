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
    public partial class 送检清单 : Form
    {
        public 送检清单()
        {
            InitializeComponent();
        }

        DataTable dt_Main;
        public 送检清单( DataTable a )
        {
            dt_Main = a;
            InitializeComponent();
        }

        private void 送检清单_Load(object sender, EventArgs e)
        {
            fun();
            this.reportViewer1.RefreshReport();
           // this.reportViewer1.LocalReport.ReportEmbeddedResource = "ERPreport.Report送检清单.rdlc";
           //// this.DataSet送检清单.DataSetName = "DataSet送检清单";
           // this.reportViewer1.RefreshReport();
        }


        private void fun()
        {




            DataTable dt_表体2 = new DataTable();
            dt_表体2.Columns.Add("送检单号", typeof(string));
            dt_表体2.Columns.Add("供应商", typeof(string));
            dt_表体2.Columns.Add("物料编码", typeof(string));
            dt_表体2.Columns.Add("物料名称", typeof(string));
            dt_表体2.Columns.Add("送检数量", typeof(decimal));
            dt_表体2.Columns.Add("规格", typeof(string));
            dt_表体2.Columns.Add("库位", typeof(string));
            dt_表体2.Columns.Add("送检人员", typeof(string));
            dt_表体2.Columns.Add("仓库名称", typeof(string));
            try
            {
                foreach (DataRow dr in dt_Main.Rows)
                {
                    DataRow drr = dt_表体2.NewRow();
                    dt_表体2.Rows.Add(drr);
                    drr["送检单号"] = dr["送检单号"].ToString();
                    drr["供应商"] = dr["供应商"].ToString();
                    drr["物料编码"] = dr["物料编码"].ToString();
                    drr["物料名称"] = dr["物料名称"].ToString();
                    drr["送检数量"] = decimal.Parse( dr["送检数量"].ToString()).ToString("0.######");
                    drr["规格"] = dr["规格型号"].ToString();
                    drr["库位"] = dr["货架描述"].ToString();
                    drr["送检人员"] = dr["送检人员"].ToString();
                    drr["仓库名称"] = dr["仓库名称"].ToString();
                }


                DataTable1BindingSource.DataSource = dt_表体2;
                reportViewer1.LocalReport.ReportPath = "Report送检清单.rdlc";

                List<ReportParameter> lstParameter = new List<ReportParameter>() {
                      new ReportParameter("送检人",CPublic.Var.localUserName.ToString()),       
                     new ReportParameter("打印日期",DateTime.Now. ToString("yyyy-MM-dd")),  
  
            
            
            };

                //PageSettings pages = new System.Drawing.Printing.PageSettings();
                //pages.Landscape = false;//强制设置纵向打印
                //reportViewer1.SetPageSettings(pages);
                //reportViewer1.RefreshReport();

                //  this.reportViewer1
                this.reportViewer1.LocalReport.SetParameters(lstParameter);
                this.reportViewer1.ZoomMode = ZoomMode.Percent;
                this.reportViewer1.ZoomPercent = 100;


                this.reportViewer1.RefreshReport();
                this.reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                //缩放模式为百分比,以100%方式显示
                this.reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                this.reportViewer1.ZoomPercent = 100;

                System.Drawing.Printing.PageSettings pg = new System.Drawing.Printing.PageSettings();
                pg.Margins.Top = 10;
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
