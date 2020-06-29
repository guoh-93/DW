﻿using Microsoft.Reporting.WinForms;
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
    public partial class 返库单 : Form
    {
        public 返库单()
        {
            InitializeComponent();
        }

        DataRow dr1;
        DataTable dt1;
        public 返库单(object a, object b)
        {

            dr1 = (DataRow)a;
            dt1 = (DataTable)b;
            InitializeComponent();
        }

        private void 返库单_Load(object sender, EventArgs e)
        {
            fun();
            this.reportViewer1.RefreshReport();
        }
        private void fun()
        {
            // 规格型号
            try
            {
                //if (dt1.Columns.Contains("货架描述")==false  )
                //{
                //    dt1.Columns.Add("货架描述",typeof(string));
                //}
   


                foreach (DataRow dr in dt1.Rows)
                {
                  //  dr["申请数量"] = decimal.Parse(dr["申请数量"].ToString()).ToString("0.######");
                    //  dr["库存总数"] = decimal.Parse(dr["库存总数"].ToString()).ToString("0.######");

                }




                DataTable1BindingSource.DataSource = dt1;
                reportViewer1.LocalReport.ReportPath = "Report返库单.rdlc";

                List<ReportParameter> lstParameter = new List<ReportParameter>() {
                 new ReportParameter("退料单号",dr1["退料单号"].ToString()),

                     new ReportParameter("工单号",dr1["工单号"].ToString()),
                    new ReportParameter("产品编码",dr1["产品编码"].ToString()),

                     new ReportParameter("日期",DateTime.Parse( dr1["日期"].ToString()).ToString("yyyy-MM-dd")),

                          new ReportParameter("产品名称",dr1["产品名称"].ToString()),

                        //  new ReportParameter("车间  ",dr1["车间  "].ToString()),
                     new ReportParameter("操作人",dr1["操作人"].ToString()),



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
