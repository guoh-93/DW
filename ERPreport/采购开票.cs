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
    public partial class 采购开票 : Form
    {
        public 采购开票()
        {
            InitializeComponent();
        }
        DataTable dt_Main;
        decimal dec_不含税金额总, dec_含税金额总;
        public 采购开票( DataTable  a,decimal b,decimal c)
        {
            dt_Main = a;
            dec_不含税金额总 = b;
            dec_含税金额总 =  c;
            InitializeComponent();
      
        
        }

        private void 采购开票_Load(object sender, EventArgs e)
        {

            fun();
            this.reportViewer1.RefreshReport();

        }

        private void fun()
        {




            DataTable dt_表体2 = new DataTable();
            dt_表体2.Columns.Add("送检单号", typeof(string));
            dt_表体2.Columns.Add("入库单号", typeof(string));
            dt_表体2.Columns.Add("物料编码", typeof(string));
            dt_表体2.Columns.Add("物料名称", typeof(string));
            dt_表体2.Columns.Add("数量", typeof(decimal));
            dt_表体2.Columns.Add("规格", typeof(string));
            dt_表体2.Columns.Add("含税单价", typeof(decimal));
            dt_表体2.Columns.Add("不含税单价", typeof(decimal));
            dt_表体2.Columns.Add("含税金额", typeof(decimal));
            dt_表体2.Columns.Add("不含税金额", typeof(decimal));
            dt_表体2.Columns.Add("备注", typeof(string));
            try
            {
                foreach (DataRow dr in dt_Main.Rows)
                {
                    DataRow drr = dt_表体2.NewRow();
                    dt_表体2.Rows.Add(drr);
                    drr["送检单号"] = dr["送检单号"].ToString();
                    drr["入库单号"] = dr["入库单号"].ToString();
                    drr["物料编码"] = dr["物料编码"].ToString();
                    drr["物料名称"] = dr["物料名称"].ToString();
                    drr["数量"] = dr["开票数量"].ToString();
                    drr["规格"] = dr["规格型号"].ToString();
                    drr["含税单价"] = decimal.Parse(dr["单价"].ToString()).ToString("0.####");
                    drr["不含税单价"] = decimal.Parse(dr["未税金额"].ToString()).ToString("0.####");
                    drr["含税金额"] = decimal.Parse(dr["金额"].ToString()).ToString("0.####");
                    drr["不含税金额"] = decimal.Parse(dr["未税单价"].ToString()).ToString("0.####");
                    //drr["备注"] = decimal.Parse(dr["未税单价"].ToString()).ToString("0.00");
                }


                DataTable1BindingSource.DataSource = dt_表体2;
                reportViewer1.LocalReport.ReportPath = "Report采购开票通知单.rdlc";

                List<ReportParameter> lstParameter = new List<ReportParameter>() {
                       new ReportParameter("含税总额",dec_含税金额总.ToString("0.####")),       
                     new ReportParameter("不含税总额",dec_不含税金额总.ToString("0.####")),  
                      new ReportParameter("供应商",dt_Main.Rows[0]["供应商名称"].ToString()),  
                     // new ReportParameter("采购员",dr_math["采购计划日期"].ToString())  
                  
                     
                     
                     //new ReportParameter("传真",dr_math["供应商传真"].ToString()),  
                    //  new ReportParameter("税率",dr_math["税率"].ToString()),  
                    ////  new ReportParameter("付款方式",dr_math["送检单号"].ToString()),  
                    //  new ReportParameter("制单",dr_math["生成人员"].ToString()),  
                    //         new ReportParameter("核准",dr_math["审核人员"].ToString()),  
                    //  new ReportParameter("制单日期",dr_math["录入日期"].ToString()),
                     
                    //    //  new ReportParameter("供应商确认",dr_math["检验员"].ToString()),  
                    //  new ReportParameter("核准日期",dr_math["审核日期"].ToString())
            
            
            
            };
                //  this.reportViewer1
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


                reportViewer1.RefreshReport();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    
    
    }
}
