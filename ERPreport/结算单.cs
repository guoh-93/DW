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
    public partial class 结算单 : Form
    {
        public 结算单()
        {
            InitializeComponent();
        }
        DataRow dr1;
        DataTable dt1;
        string str_单号 = "";
        public 结算单(object a, object b)
        {

            dr1 = (DataRow)a;
            dt1 = (DataTable)b;
            InitializeComponent();
        }

        private void 结算单_Load(object sender, EventArgs e)
        {
            fun();
            this.reportViewer1.RefreshReport();
        }




        private void fun()
        {
            // 规格型号
            try
            {
                str_单号="";
                string sql = string.Format("select  * from 采购记录采购开票通知发票核销表  where 开票通知单号='{0}'",dr1["开票通知单号"]);
                DataTable dt_ph = CZMaster.MasterSQL.Get_DataTable(sql,CPublic.Var.strConn);

                foreach (DataRow drr  in dt_ph.Rows)
                {
                    str_单号 = str_单号 +"," +drr["发票号"];



                }

                foreach (DataRow dr in dt1.Rows)
                {


                    dr["折扣后不含税单价"] = decimal.Parse(dr["折扣后不含税单价"].ToString()).ToString("0.######");
                    dr["折扣后含税单价"] = decimal.Parse(dr["折扣后含税单价"].ToString()).ToString("0.######");

                    dr["折扣后不含税金额"] = decimal.Parse(dr["折扣后不含税金额"].ToString()).ToString("0.######");
                    dr["折扣后含税金额"] = decimal.Parse(dr["折扣后含税金额"].ToString()).ToString("0.######");
                    dr["开票数量"] = decimal.Parse(dr["开票数量"].ToString()).ToString("0.######");
                }




                DataTable1BindingSource.DataSource = dt1;
                reportViewer1.LocalReport.ReportPath = "Report结算单.rdlc";

                List<ReportParameter> lstParameter = new List<ReportParameter>() {
                 new ReportParameter("开票通知单号",dr1["开票通知单号"].ToString()),

                     new ReportParameter("供应商名称",dr1["供应商名称"].ToString()),
                    new ReportParameter("开票人",dr1["开票人"].ToString()),

                     new ReportParameter("录入日期",DateTime.Parse( dr1["录入日期"].ToString()).ToString("yyyy-MM-dd")),


                         new ReportParameter("票号",str_单号),
                    new ReportParameter("总金额",dr1["总金额"].ToString()),


                         new ReportParameter("未税金额",dr1["未税金额"].ToString()),
                    new ReportParameter("折扣",dr1["折扣"].ToString()),


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
