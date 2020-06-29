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
    public partial class 形态转换 : Form
    {
        public 形态转换()
        {
            InitializeComponent();
        }
        DataTable dt1;
        DataRow dr1;
        public 形态转换(DataRow  dr  , DataTable dt)
        {
            dr1 = dr;
            dt1 = dt;
            InitializeComponent();
        }
        private void 形态转换_Load(object sender, EventArgs e)
        {
            fun();
            this.reportViewer1.RefreshReport();
        }





        private void fun()
        {




            // 规格型号
            try
            {
                foreach (DataRow dr in dt1.Rows)
                {
                    dr["数量"] = decimal.Parse(dr["数量"].ToString()).ToString("#0.####");           
                        
                        
                        }


                DataTable1BindingSource.DataSource = dt1;//绑数据
                reportViewer1.LocalReport.ReportPath = "Report形态转换.rdlc";//绑报表
                //参数赋值
                List<ReportParameter> lstParameter = new List<ReportParameter>() {
                   new ReportParameter("形态转换单号",dr1["形态转换单号"].ToString()),
                   new ReportParameter("申请人",dr1["申请人"].ToString()),
                   new ReportParameter("部门名称",dr1["部门名称"].ToString()),
                   new ReportParameter("备注",dr1["备注"].ToString()),
                   new ReportParameter("申请日期",      DateTime.Parse( dr1["申请日期"].ToString()   )      .ToString("yyyy-MM-dd")),
          
            };

                this.reportViewer1.LocalReport.SetParameters(lstParameter);
                this.reportViewer1.ZoomMode = ZoomMode.Percent;
                this.reportViewer1.ZoomPercent = 100;
                this.reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                this.reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                this.reportViewer1.ZoomPercent = 100;

                System.Drawing.Printing.PageSettings pg = new System.Drawing.Printing.PageSettings();
                pg.Margins.Top = 15;
                pg.Margins.Bottom = 15;///调整打印边距

                pg.Margins.Left = 10;
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
