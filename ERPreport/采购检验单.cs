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
    public partial class 采购检验单 : Form
    {
        public 采购检验单()
        {
            InitializeComponent();
        }
        DataRow dr_主数据;
        DataTable dt_main,dt_tablemain;
        public 采购检验单( object  a, object b ,object c )
        {
            InitializeComponent();
            dt_main = (DataTable)a;
            dr_主数据=(DataRow) b;        
            dt_tablemain=(DataTable) c;
        }
        private void 采购检验单_Load(object sender, EventArgs e)
        {
            fun();
            this.reportViewer1.RefreshReport();
        }



        private void fun()
        {
            dt_main.Columns.Add("供应商名称");
            dt_main.Columns.Add("物料名称");
            dt_main.Columns.Add("规格型号");

            foreach (DataRow dr in dt_main.Rows)
            {

                string str_gys_number = dt_main.Rows[0]["供应商编号"].ToString();
                string str_produce_number = dt_main.Rows[0]["产品编号"].ToString();
                string sql = string.Format("SELECT [供应商名称]  FROM [采购供应商表]where[供应商ID]='{0}'", str_gys_number);
                DataRow dr_sql = CZMaster.MasterSQL.Get_DataRow(sql, CPublic.Var.strConn);
                //sqlstr = "SELECT [gysmc]  FROM [gys]where[gysbh]='{0}'";

                if (dr_sql !=null)
                    {
                        dr["供应商名称"] = dr_sql["供应商名称"].ToString();
                    }

                 string sqlstr = string.Format("SELECT [物料名称],[n原ERP规格型号]  FROM [基础数据物料信息表]where  [物料编码]='{0}'", str_produce_number);
                 DataRow drew=CZMaster.MasterSQL.Get_DataRow(sqlstr,CPublic.Var.strConn);
                    if (drew!=null)
                    {
                        dr["物料名称"] =drew["物料名称"].ToString();
                        dr["规格型号"] = drew["n原ERP规格型号"].ToString();
                    }
               }

            
            //DataTable dt_表体2 = new DataTable();
            //dt_表体2.Columns.Add("料号", typeof(string));
            //dt_表体2.Columns.Add("名称及规格", typeof(string));
            //dt_表体2.Columns.Add("单位", typeof(string));
            //dt_表体2.Columns.Add("数量", typeof(decimal));
            //dt_表体2.Columns.Add("仓库", typeof(string));
            //dt_表体2.Columns.Add("库位", typeof(string));
            //dt_表体2.Columns.Add("当前库存", typeof(decimal));


            try
            {
                //foreach (DataRow dr in dt_表体.Rows)
                //{
                //    DataRow drr = dt_表体2.NewRow();
                //    dt_表体2.Rows.Add(drr);
                //    drr["料号"] = dr["物料编码"].ToString();
                //    drr["名称及规格"] = dr["物料名称"].ToString() + dr["规格型号"].ToString();
                //    drr["单位"] = dr["库存单位"].ToString();
                //    drr["数量"] = dr["输入领料数量"].ToString();
                //    drr["仓库"] = dr["仓库名称"].ToString();
                //    drr["库位"] = "";
                //    drr["当前库存"] = dr["库存总数"].ToString();/
                //}


                DataTable1BindingSource.DataSource = dt_tablemain;
               reportViewer1.LocalReport.ReportPath = "Report检验记录.rdlc";

                List<ReportParameter> lstParameter = new List<ReportParameter>() {
                       new ReportParameter("名称",dt_main.Rows[0]["物料名称"].ToString()),       
                    // new ReportParameter("编号",dt_main.Rows[0]["编号"].ToString()),  

                      new ReportParameter("供货单位",dt_main.Rows[0]["供应商名称"].ToString()),  
                    new ReportParameter("图号或规格",dt_main.Rows[0]["规格型号"].ToString()),  
                      new ReportParameter("送检数量",dt_main.Rows[0]["送检数量"].ToString()),  
                      new ReportParameter("入库通知单号",dt_main.Rows[0]["送检单号"].ToString()),  
                      new ReportParameter("结论",dt_main.Rows[0]["检验结果"].ToString()),  
                             new ReportParameter("检验员",dt_main.Rows[0]["检验员"].ToString()),  
                      new ReportParameter("日期",dt_main.Rows[0]["检验日期"].ToString())
                     
         
            
            
            
            };
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

