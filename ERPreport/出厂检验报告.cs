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
    public partial class 出厂检验报告 : Form
    {
        public 出厂检验报告()
        {
            InitializeComponent();
        }
        DataRow dr;
        DataTable dt_hand,   dt_main;
        /**
         * datatable b  (表头数据)
         * datatable c  (表数据 )
         * 
         * 
         * **/

        public 出厂检验报告(DataRow  a, DataTable b, DataTable c )
        {
            dr = a;
            dt_hand = b;
            dt_main = c;
            InitializeComponent();
        }
        private void 出厂检验报告_Load(object sender, EventArgs e)
        {
            fun();
            this.reportViewer1.RefreshReport();
        }



        private void fun()
        {

            string sql = string.Format(@"select  生产记录生产检验单主表.*,基础数据物料信息表.原ERP物料编号,基础数据物料信息表.规格型号,a.wjbm as 文件编号  from 生产记录生产检验单主表
                            left join   基础数据物料信息表 on 生产记录生产检验单主表.物料编码=基础数据物料信息表.物料编码
                            left join (select 生产检验单号,wjbm  from [成品检验检验记录明细表] group by 生产检验单号,wjbm)as a on a.生产检验单号=生产记录生产检验单主表.生产检验单号
                            where 生产记录生产检验单主表.物料编码 = '{0}' order by 生产工单号 desc", dt_hand .Rows[0]["物料编码"]);

                System.Data.DataTable dt = new System.Data.DataTable();
                dt = CZMaster.MasterSQL.Get_DataTable(sql,CPublic.Var.strConn);
               

                        //range = ws.get_Range("A8", Type.Missing);
                        //range.Value2 = dt.Rows[0]["文件编号"].ToString();
                        //range = ws.get_Range("R8", Type.Missing);
                        //range.Value2 = dt.Rows[0]["生产工单号"].ToString();


            DataTable dt_表体2 = new DataTable();
            dt_表体2.Columns.Add("检验项目", typeof(string));
            dt_表体2.Columns.Add("检验要求", typeof(string));
            dt_表体2.Columns.Add("检验水平", typeof(string));
            dt_表体2.Columns.Add("合格水平", typeof(string));
            dt_表体2.Columns.Add("A", typeof(string));
            dt_表体2.Columns.Add("B", typeof(string));
            dt_表体2.Columns.Add("C", typeof(string));
            dt_表体2.Columns.Add("D", typeof(string));
            dt_表体2.Columns.Add("E", typeof(string));
            dt_表体2.Columns.Add("F", typeof(string));
            dt_表体2.Columns.Add("G", typeof(string));
            dt_表体2.Columns.Add("H", typeof(string));
       

            try
            {
                foreach (DataRow dr in dt_main.Rows)
                {
                    DataRow drr = dt_表体2.NewRow();
                    dt_表体2.Rows.Add(drr);
                    drr["检验项目"] = dr["检验项目"].ToString();
                    drr["检验要求"] = dr["检验要求"].ToString();
                    drr["检验水平"] = dr["检测水平"].ToString();
                    drr["合格水平"] = dr["合格水平"].ToString();
                    drr["A"] = dr["a"].ToString();
                    drr["B"] = dr["b"].ToString();
                    drr["C"] = dr["c"].ToString();
                    drr["D"] = dr["d"].ToString();
                    drr["E"] = dr["e"].ToString();
                    drr["F"] = dr["f"].ToString();
                    drr["G"] = dr["g"].ToString();
                    drr["H"] = dr["h"].ToString();
                }


                DataTable1BindingSource.DataSource = dt_表体2;
                reportViewer1.LocalReport.ReportPath = "Report出厂检验报告.rdlc";

                List<ReportParameter> lstParameter = new List<ReportParameter>() {
                       new ReportParameter("生产规格型号",dt.Rows[0]["规格型号"].ToString()),
                      
                           new ReportParameter("编号",dt.Rows[0]["生产工单号"].ToString()),
                     
                          
                     new ReportParameter("大类",dt_hand.Rows[0]["大类"].ToString()),  
                      new ReportParameter("小类",dt_hand.Rows[0]["小类"].ToString()),  
                      new ReportParameter("生产者",dt_hand.Rows[0]["生产者"].ToString() + "%"),  
                    new ReportParameter("班组",dt_hand.Rows[0]["班组"].ToString()),  
                     new ReportParameter("部门",dt_hand.Rows[0]["部门"].ToString()), 
 
                        new ReportParameter("检验日期",dt_hand.Rows[0]["生产日期"].ToString()),  
                      new ReportParameter("发货数量",dt_hand.Rows[0]["发货数量"].ToString() + "%"),  
                    new ReportParameter("合格数",dt_hand.Rows[0]["合格数"].ToString()) 
                   
                
             

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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


    
    
    
    }
}
