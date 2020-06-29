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
    public partial class 退货申请打印啊 : Form
    {
        public 退货申请打印啊()
        {
            InitializeComponent();
        }
        DataTable dt1;
        DataRow dr1;
        public 退货申请打印啊(object a,object b  )
        {

            dr1 = (DataRow)a;

            dt1=(DataTable)b;

            InitializeComponent();
        }



        private void 退货申请打印啊_Load(object sender, EventArgs e)
        {
            fun();
            this.reportViewer1.RefreshReport();
        }




        private void fun()
        {

            //DataTable dt_表体2 = new DataTable();
            //dt_表体2.Columns.Add("料号", typeof(string));
            //dt_表体2.Columns.Add("名称及规格", typeof(string));
            //dt_表体2.Columns.Add("单位", typeof(string));
            //dt_表体2.Columns.Add("数量", typeof(decimal));
            //dt_表体2.Columns.Add("库位", typeof(string));
            //dt_表体2.Columns.Add("当前库存", typeof(decimal));

            //CPublic.Var.localUserName 仓管员

            try
            {
                //    foreach (DataRow dr in dt1.Rows)
                //    {
                //        DataRow drr = dt_表体2.NewRow();
                //        dt_表体2.Rows.Add(drr);
                //        drr["料号"] = dr["物料编码"];
                //        drr["名称及规格"] = dr["物料名称"] + "(" + dr["规格型号"] + ")";
                //        drr["数量"] = decimal.Parse(dr["数量"].ToString()).ToString("0.######");
                //        // drr["库位"] = dr["物料名称"];
                //        drr["当前库存"] = decimal.Parse(dr["库存总数"].ToString()).ToString("0.######");
                //        drr["单位"] = dr["计量单位"];



                //    }


                reportViewer1.LocalReport.ReportPath = "Report退货申请打印啊.rdlc";
                DataTable1BindingSource.DataSource = dt1;


                List<ReportParameter> lstParameter = new List<ReportParameter>() {
                   new ReportParameter("退货申请单号",dr1["退货申请单号"].ToString()),
       
                new ReportParameter("备注",dr1["备注"].ToString()),
                   new ReportParameter("操作人员",dr1["操作人员"].ToString()),
                        new ReportParameter("客户",dr1["客户"].ToString()),   
                   new ReportParameter("申请日期",DateTime.Parse( dr1["申请日期"].ToString()).ToString("yyyy-MM-dd")),




                };
                // this.reportViewer1

                //PageSettings pages = new System.Drawing.Printing.PageSettings();
                //pages.Landscape = false;//强制设置纵向打印
                //reportViewer1.SetPageSettings(pages);
                //reportViewer1.RefreshReport();



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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }
}
