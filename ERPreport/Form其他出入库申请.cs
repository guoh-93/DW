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
    public partial class Form其他出入库申请 : Form
    {
        public Form其他出入库申请()
        {
            InitializeComponent();
        }
        DataTable dt1;
        DataRow dr1;
        string sss;

        public Form其他出入库申请(object a, object b,object c)
        {
            InitializeComponent();
     
            dr1 = (DataRow)a;
            dt1 = (DataTable)b;
            sss = (string)c;
        }

        private void Form其他出入库申请_Load(object sender, EventArgs e)
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
                //        //DataRow drr = dt_表体2.NewRow();
                //        ////dt_表体2.Rows.Add(drr);
                //        //drr["料号"] = dr["物料编码"];
                //        //drr["名称及规格"] = dr["物料名称"] + "(" + dr["规格型号"] + ")";
                //        //drr["数量"] = decimal.Parse(dr["数量"].ToString()).ToString("0.00");
                //        //// drr["库位"] = dr["物料名称"];
                //        //drr["当前库存"] = dr["库存总数"];
                //        //drr["单位"] = dr["计量单位"];



                //    }

                reportViewer1.LocalReport.ReportPath = "Report其他出入库申请.rdlc";

                if (dt1.Columns.Contains("库存总数")!=true)

                {
                    dt1.Columns.Add("库存总数", typeof(decimal));
                }

         
                foreach (DataRow dr  in dt1.Rows  )
                {
                    string sql = string.Format("select 库存总数 from 仓库物料数量表 where 物料编码='{0}' and 仓库号='{1}' ",dr["物料编码"].ToString(), dr["仓库号"].ToString());
                    DataRow dr_kucun = CZMaster.MasterSQL.Get_DataRow(sql,CPublic.Var.strConn);
                    if (dr_kucun!=null) {
                        dr["库存总数"] = decimal.Parse(dr_kucun["库存总数"].ToString()).ToString("0.######");

                        dr["已处理数量"] = decimal.Parse(dr["已处理数量"].ToString()).ToString("0.######");


                    }
                    else
                    {
                        dr["库存总数"] = 0;
                        dr["已处理数量"] = 0;
                    }



                }


                DataTable1BindingSource.DataSource = dt1;


                List<ReportParameter> lstParameter = new List<ReportParameter>() {
                   new ReportParameter("出入库申请单号",dr1["出入库申请单号"].ToString()),
                   new ReportParameter("备注",dr1["备注"].ToString()),
                   new ReportParameter("申请人",dr1["操作人员"].ToString()),
                   new ReportParameter("申请类型",dr1["申请类型"].ToString()),
                   new ReportParameter("模板名",sss),
                   new ReportParameter("申请日期",DateTime.Now.ToString("yyyy-MM-dd")),
                   new ReportParameter("原因分类",dr1["原因分类"].ToString()),
               


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
                pg.Margins.Top =35;
                pg.Margins.Bottom =35;

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
