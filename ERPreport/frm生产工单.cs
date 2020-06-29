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
    public partial class frm生产工单 : Form
    {
        public frm生产工单()
        {
            InitializeComponent();
        }

        #region
        DataTable dt_main;
        #endregion

        public frm生产工单( DataTable dt)
        {
            InitializeComponent();
            dt_main = new DataTable();
            dt_main = dt.Copy();
        }


        private void fun()
        {

            DataTable dt_表体2 = new DataTable();
            dt_表体2.Columns.Add("制单部门", typeof(string));
            dt_表体2.Columns.Add("物料编码", typeof(string));
            dt_表体2.Columns.Add("制单日期", typeof(DateTime));
            dt_表体2.Columns.Add("物料名称", typeof(string));
            dt_表体2.Columns.Add("客户型号", typeof(string));
            dt_表体2.Columns.Add("规格型号", typeof(string));
            dt_表体2.Columns.Add("生产人员", typeof(string));
            dt_表体2.Columns.Add("生产数量", typeof(decimal));
            dt_表体2.Columns.Add("检验员", typeof(string));
            dt_表体2.Columns.Add("检验日期", typeof(DateTime));
            dt_表体2.Columns.Add("包装人员", typeof(string));
            dt_表体2.Columns.Add("计划备注", typeof(string));
            dt_表体2.Columns.Add("工单编号", typeof(string));
            dt_表体2.Columns.Add("完工日期", typeof(DateTime));
            dt_表体2.Columns.Add("版本备注", typeof(string));
            dt_表体2.Columns.Add("生产工单类型", typeof(string));
            dt_表体2.Columns.Add("班组", typeof(string));
            dt_表体2.Columns.Add("制单人", typeof(string));
            dt_表体2.Columns.Add("客户订单号", typeof(string));




            try
            {
                foreach (DataRow dr in dt_main.Rows)
                {
                    DataRow drr = dt_表体2.NewRow();
                    dt_表体2.Rows.Add(drr);
                    drr["制单部门"] = "生产一厂";
                    drr["物料编码"] = dr["物料编码"];
                    drr["制单日期"] = dr["生效日期"];
                    drr["工单编号"] = dr["生产工单号"];
                    drr["物料名称"] = dr["物料名称"];
                    drr["客户型号"] = "";
                    drr["规格型号"] = dr["规格型号"];
                    drr["生产工单类型"] = dr["生产工单类型"];
                    drr["完工日期"] = dr["预计完工日期"];
                    drr["生产人员"] = dr["生效人"];
                    drr["版本备注"] = dr["版本备注"];
                    drr["生产数量"] = dr["生产数量"];
                    drr["计划备注"] = dr["备注1"];

                    //drr["检验员"] = dr["检验人员"];
                    drr["班组"] = dr["班组"];
                    drr["制单人"] = dr["制单人员"];
                   







                }

                reportViewer1.LocalReport.ReportPath = "Report生产工单备注.rdlc";
                //ReportParameter rp = new ReportParameter("班组", dt_main.Rows[0]["班组"].ToString());
                //ReportParameter rp1 = new ReportParameter("班组", dt_main.Rows[0]["班组"].ToString());

                //List<ReportParameter> parameters = new List<ReportParameter>(reportViewer1.LocalReport.GetParameters().Count);
                //parameters.Add(rp);
                //reportViewer1.LocalReport.SetParameters(parameters);



                DataTable1BindingSource.DataSource = dt_表体2;
                //this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });


                //    List<ReportParameter> lstParameter = new List<ReportParameter>() {
                //   new ReportParameter("领料出库单号",dt_表头.Rows[0]["领料出库单号"].ToString()),       
                // new ReportParameter("编号",dt_表头.Rows[0]["编号"].ToString()),  

                //};
                //  this.reportViewer1
                //  this.reportViewer1.LocalReport.SetParameters(lstParameter);


                //PageSettings pages = new System.Drawing.Printing.PageSettings();
                //pages.Landscape = false;//强制设置纵向打印
                //reportViewer1.SetPageSettings(pages);
                //reportViewer1.RefreshReport();


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
         

        private void frm生产工单_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
            fun();
        }

    
    }
}
