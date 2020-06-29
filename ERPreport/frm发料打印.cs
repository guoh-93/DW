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
    public partial class frm发料打印 : Form
    {
        public frm发料打印()
        {
            InitializeComponent();
        }
        /// <summary>
        /// dt1 表体，dt2表头
        /// </summary>
        /// <param name="dt1"> 表体</param>
        /// <param name="dt2"> 表头</param>
        public frm发料打印(DataTable dt1, DataTable dt2, DataRow c)
        {
            InitializeComponent();
            dt_表头 = dt2.Copy();
            dr = c;
            dt_表体 = dt1.Copy();
        }
        public frm发料打印(DataTable dt1, DataTable dt2)
        {
            InitializeComponent();
            dt_表头 = dt2.Copy();
     
            dt_表体 = dt1.Copy();
        }

        DataTable dt_表头, dt_表体;
        DataRow dr;
        private void fun()
        {

            DataTable dt_表体2 = new DataTable();
            dt_表体2.Columns.Add("料号", typeof(string));
            dt_表体2.Columns.Add("名称及规格", typeof(string));
            dt_表体2.Columns.Add("单位", typeof(string));
            dt_表体2.Columns.Add("数量", typeof(decimal));
            dt_表体2.Columns.Add("仓库", typeof(string));
            dt_表体2.Columns.Add("库位", typeof(string));
            dt_表体2.Columns.Add("当前库存", typeof(decimal));
            dt_表体2.Columns.Add("标识", typeof(string));
            string a = dt_表头.Rows[0]["编号"].ToString();
            string str_后三位 = a.Substring(a.Length - 3);

            foreach (DataRow dr in dt_表体.Rows)
                {
                    DataRow drr = dt_表体2.NewRow();
                    dt_表体2.Rows.Add(drr);
                drr["标识"] = str_后三位;

                drr["料号"] = dr["物料编码"].ToString();
                    drr["名称及规格"] = dr["物料名称"].ToString() + dr["规格型号"].ToString();
                    drr["单位"] = dr["库存单位"].ToString();
                    drr["数量"] = decimal.Parse( dr["输入领料数量"].ToString()).ToString("0.######");
                    drr["仓库"] = dr["仓库名称"].ToString();
                    drr["库位"] = dr["货架描述"].ToString();
                    drr["当前库存"] = decimal.Parse(dr["库存总数"].ToString()).ToString("0.######");

                }

            /////加数据测试    
            //DataTable dt_copy = dt_表体2.Copy();

            //for (int i = 0; i < 10; i++)
            //{
            //    foreach (DataRow drr in dt_copy.Rows)
            //    {
            //        DataRow dr = dt_表体2.NewRow();
            //        dr = drr;
            //        dt_表体2.ImportRow(dr);


            //    }

            //}

              DataTable1BindingSource.DataSource = dt_表体2;

            this.reportViewer1.ProcessingMode = ProcessingMode.Local;
           reportViewer1.LocalReport.ReportPath = "Report发料.rdlc";
           // reportViewer1.LocalReport.ReportEmbeddedResource = "Report发料.rdlc";
            //   reportViewer1.Reset();

            // Microsoft.Reporting.WinForms.ReportDataSource("DataSet1_DataTable1", this.DataSet1.DataTable1));
            //     ReportDataSource rdsItem = new ReportDataSource("DataSet发料", dt_表体2);

            //this.reportViewer1 .LocalReport.DataSources.Add(rdsItem);

            List<ReportParameter> lstParameter = new List<ReportParameter>() {
              new ReportParameter("领料出库单号",dt_表头.Rows[0]["领料出库单号"].ToString()),
             new ReportParameter("编号",dt_表头.Rows[0]["编号"].ToString()),

              new ReportParameter("物料号",dt_表头.Rows[0]["物料号"].ToString()),
            new ReportParameter("规格",dt_表头.Rows[0]["规格"].ToString()),
              new ReportParameter("物料名称",dt_表头.Rows[0]["物料名称"].ToString()),
              new ReportParameter("生产数量",decimal.Parse( dt_表头.Rows[0]["生产数量"].ToString()).ToString("0.######")),
              new ReportParameter("领用部门",dt_表头.Rows[0]["领用部门"].ToString()),
             new ReportParameter("仓管员",CPublic.Var.localUserName.ToString()),

              new ReportParameter("领用人",dt_表头.Rows[0]["领用人"].ToString()),

            new ReportParameter("申请人",dt_表头.Rows[0]["申请人"].ToString()),

              //new ReportParameter("领用部门",dt_表头.Rows[0]["领用部门"].ToString()),//

      new ReportParameter("日期",DateTime.Parse( dt_表头.Rows[0]["日期"].ToString()).ToString("yyyy-MM-dd")),
       
            


            };
            //PageSettings pages = new System.Drawing.Printing.PageSettings();
            //pages.Landscape = false;//强制设置纵向打印
            //reportViewer1.SetPageSettings(pages);
            //reportViewer1.RefreshReport();

            //this.reportViewer1
            this.reportViewer1.LocalReport.SetParameters(lstParameter);
        
                this.reportViewer1.ZoomMode = ZoomMode.Percent;
                this.reportViewer1.ZoomPercent = 100;


                this.reportViewer1.RefreshReport();
                this.reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                //缩放模式为百分比,以100%方式显示
                this.reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                this.reportViewer1.ZoomPercent = 100;



            System.Drawing.Printing.PageSettings pg = new System.Drawing.Printing.PageSettings();
            pg.Margins.Top =30;
            pg.Margins.Bottom = 30;

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

        private void frm发料打印_Load(object sender, EventArgs e)
        {


            fun();



        }


        private void TSMI_W_Click(object sender, EventArgs e)
        {
           
        }


    }





}
