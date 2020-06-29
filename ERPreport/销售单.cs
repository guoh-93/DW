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
    public partial class 销售单 : Form
    {
        public 销售单()
        {
            InitializeComponent();
        }
        DataTable dt_main;
        public 销售单(DataTable a)
        {
            dt_main = a;
            InitializeComponent();
        }

        private void 销售单_Load(object sender, EventArgs e)
        {
            fun();

            this.reportViewer1.RefreshReport();
            //this.reportViewer2.RefreshReport();
        }


        private void fun()
        {




            DataTable dt_表体2 = new DataTable();
            dt_表体2.Columns.Add("物料编码", typeof(string));
            dt_表体2.Columns.Add("物料名称", typeof(string));

            dt_表体2.Columns.Add("规格", typeof(string));
            dt_表体2.Columns.Add("单位", typeof(string));
            dt_表体2.Columns.Add("数量", typeof(decimal));
            dt_表体2.Columns.Add("单价", typeof(decimal));
            dt_表体2.Columns.Add("金额", typeof(decimal));
            dt_表体2.Columns.Add("送货日期", typeof(string));
            dt_表体2.Columns.Add("备注", typeof(string));

            try
            {
                foreach (DataRow dr in dt_main.Rows)
                {
                    DataRow drr = dt_表体2.NewRow();
                    dt_表体2.Rows.Add(drr);
                    drr["物料编码"] = dr["物料编码"].ToString();
                    drr["物料名称"] = dr["物料名称"].ToString();
                    drr["规格"] = dr["规格型号"].ToString();
                    drr["单位"] = dr["计量单位"].ToString();
                    drr["数量"] = decimal.Parse(dr["数量"].ToString()).ToString("0.######");
                    drr["单价"] = decimal.Parse(dr["税后单价"].ToString()).ToString("0.####");
                    drr["金额"] = decimal.Parse(dr["税后金额"].ToString()).ToString("0.####");
                    drr["送货日期"] = DateTime.Parse(dr["送达日期"].ToString()).ToString("yyyy-MM-dd");
                    drr["备注"] = dr["备注"].ToString();
                }


                DataTable销售单BindingSource.DataSource = dt_表体2;
                reportViewer1.LocalReport.ReportPath = "Report销售单.rdlc";

                List<ReportParameter> lstParameter = new List<ReportParameter>() {
                       new ReportParameter("客户名称",dt_main.Rows[0]["客户名"].ToString()),       
                     new ReportParameter("订单号",dt_main.Rows[0]["销售订单号"].ToString()),  
                      new ReportParameter("客户订单",dt_main.Rows[0]["客户订单号"].ToString()),  
                      new ReportParameter("税率",dt_main.Rows[0]["税率"].ToString() + "%"),  
                    new ReportParameter("销售日期", DateTime.Parse(dt_main.Rows[0]["创建日期"].ToString()).ToString("yyyy-MM-dd")),  
                     new ReportParameter("合计",decimal.Parse( dt_main.Rows[0]["总金额"].ToString()).ToString("0.####")),  
                    //  new ReportParameter("付款方式",dr_math["送检单号"].ToString()),  
                    //  new ReportParameter("制单",dr_math["生成人员"].ToString()),  
                    //         new ReportParameter("核准",dr_math["审核人员"].ToString()),  
                    //  new ReportParameter("制单日期",dr_math["录入日期"].ToString()),
                     
                    //      new ReportParameter("供应商确认",dr_math["检验员"].ToString()),  
                    //  new ReportParameter("核准日期",dr_math["审核日期"].ToString())
            
            
            
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


                //this.reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



    }
}
