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
    public partial class 销售开票 : Form
    {
        public 销售开票()
        {
            InitializeComponent();
        }
        DataRow dr1;
        DataTable dt1;
        string bz = "";
        public 销售开票(object a,object b )
        {
            dr1 = (DataRow)a;
            dt1 = (DataTable)b;
            //bz = (string)c;

            InitializeComponent();
        }

        private void 销售开票_Load(object sender, EventArgs e)
        {
            fun();
            this.reportViewer1.RefreshReport();
        }


        private void fun()
        {



            decimal 税前总价=0;
            decimal 税后总价=0;

            // 规格型号
            try
            {
                foreach (DataRow dr in dt1.Rows)
                {
                 
                    dr["开票税前金额"] = decimal.Parse(dr["开票税前金额"].ToString()).ToString("0.######");
                    dr["开票税后金额"] = decimal.Parse(dr["开票税后金额"].ToString()).ToString("0.######");
                    dr["开票税前单价"] = decimal.Parse(dr["开票税前单价"].ToString()).ToString("0.######");
                    dr["本币税后金额"] = decimal.Parse(dr["本币税后金额"].ToString()).ToString("0.######");
                    dr["开票税后单价"] = decimal.Parse(dr["开票税后单价"].ToString()).ToString("0.######");
                    税前总价 += decimal.Parse(dr["开票税前金额"].ToString());
                    税后总价 += decimal.Parse(dr["开票税后金额"].ToString());
                }




                DataTable1BindingSource.DataSource = dt1;
                reportViewer1.LocalReport.ReportPath = "Report销售开票附件.rdlc";

                List<ReportParameter> lstParameter = new List<ReportParameter>() {
                 new ReportParameter("开票票号",dr1["开票票号"].ToString()),       

                     new ReportParameter("销售开票通知单号",dr1["销售开票通知单号"].ToString()),  
                    new ReportParameter("开票员",dr1["开票员"].ToString()),  
                     new ReportParameter("客户名称",dr1["客户名称"].ToString()),  
                     new ReportParameter("日期",DateTime.Now.ToString("yyyy-MM-dd")),
                     new ReportParameter("币种",dr1["币种"].ToString()),
                      new ReportParameter("税率",dr1["税率"].ToString()),
               new ReportParameter("税前总价",税前总价.ToString()),
                     new ReportParameter("税后总价",税后总价.ToString()),



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
