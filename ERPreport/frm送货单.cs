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
    public partial class frm送货单 : Form
    {
        public frm送货单()
        {
            InitializeComponent();
        }
        DataTable  dt_hand,dt_main;
        public frm送货单(DataTable a,DataTable b )
        {
            dt_hand = a;
            dt_main = b;
            InitializeComponent();
        }
     
      

        private void frm送货单_Load(object sender, EventArgs e)
        {
            fun();
            this.reportViewer1.RefreshReport();



        }




        private void fun()
        {

            DataTable dt_表体2 = new DataTable();
            dt_表体2.Columns.Add("订单号", typeof(string));
            dt_表体2.Columns.Add("规格名称", typeof(string));
            dt_表体2.Columns.Add("物料编码", typeof(string));
            dt_表体2.Columns.Add("送货数量", typeof(decimal));
            dt_表体2.Columns.Add("单位", typeof(string));
            dt_表体2.Columns.Add("实收数量", typeof(decimal));
            dt_表体2.Columns.Add("备注", typeof(string));
 

            try
            {
                foreach (DataRow dr in dt_main.Rows)
                {
                    DataRow drr = dt_表体2.NewRow();
                    dt_表体2.Rows.Add(drr);
                    drr["订单号"] = dr["销售订单明细号"];

                    drr["物料编码"] = dr["物料编码"].ToString();
                    drr["送货数量"] = decimal.Parse( dr["出库数量"].ToString()).ToString("0.######");

                    drr["单位"] = dr["计量单位"];
               //     drr["实收数量"] = dr["出库数量"];

                




                    string sqld = string.Format(@"select 销售记录销售订单明细表.*  from 销售记录销售订单明细表 
                              where 销售订单明细号 ='{0}'", dr["销售订单明细号"].ToString());
                    System.Data.DataTable dtt = new System.Data.DataTable();
                    dtt = CZMaster.MasterSQL.Get_DataTable(sqld, CPublic.Var.strConn);
                   // da.Fill(dtt);
          
                    drr["规格名称"] = dtt.Rows[0]["规格型号"].ToString() + dr["物料名称"].ToString().Trim();

                       drr["备注"] = dtt.Rows[0]["备注"].ToString();







                }


                reportViewer1.LocalReport.ReportPath = "Report送货单附件.rdlc";
                DataTable1BindingSource.DataSource = dt_表体2;


                    List<ReportParameter> lstParameter = new List<ReportParameter>() 
                  {
               new ReportParameter("出库单号",dt_hand.Rows[0]["成品出库单号"].ToString()),       
               new ReportParameter("收货单位",dt_hand.Rows[0]["客户"].ToString()),  
               new ReportParameter("客户订单号",dt_hand.Rows[0]["客户订单号"].ToString()),       
               new ReportParameter("出库日期",DateTime.Parse( dt_hand.Rows[0]["生效日期"].ToString()).ToString("yyyy-MM-dd")),  
               new ReportParameter("送货方式",dt_hand.Rows[0]["送货方式"].ToString()),       
             //  new ReportParameter("备注",dt_hand.Rows[0]["编号"].ToString()),  
              // new ReportParameter("电话",dt_hand.Rows[0]["领料出库单号"].ToString()),       
             //  new ReportParameter("签收人",dt_hand.Rows[0]["编号"].ToString()),  
               new ReportParameter("经手人",dt_hand.Rows[0]["操作员"].ToString()),       
             //  new ReportParameter("传真地址",dt_hand.Rows[0]["编号"].ToString()),  
                  };



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
