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
    public partial class 出库通知附件 : Form
    {
        public 出库通知附件()
        {
            InitializeComponent();
        }
        DataTable dt1, dt2, dt3, dt4,dt_目标客户;
        public 出库通知附件(DataTable a, DataTable b, DataTable c, DataTable d,DataTable e)
        {
            dt1 = a;
            dt2 = b;
            dt3 = c;
            dt4 = d;
            dt_目标客户 = e;
            InitializeComponent();
        }
        private void 出库通知附件_Load(object sender, EventArgs e)
        {
            fun();
            this.reportViewer1.RefreshReport();
        }


        private void fun()
        {

            DataTable dt_表体2 = new DataTable();
            dt_表体2.Columns.Add("编码", typeof(string));
            dt_表体2.Columns.Add("名称", typeof(string));
            dt_表体2.Columns.Add("规格", typeof(string));
            dt_表体2.Columns.Add("数量", typeof(decimal));
            dt_表体2.Columns.Add("单位", typeof(string));
            dt_表体2.Columns.Add("库位", typeof(string));
            dt_表体2.Columns.Add("备注", typeof(string));
            dt_表体2.Columns.Add("包装方式", typeof(string));
            dt_表体2.Columns.Add("库存", typeof(decimal));
            dt_表体2.Columns.Add("标识", typeof(string));

            string a = dt2.Rows[0]["出库通知单号"].ToString();
            string str_后三位 = a.Substring(a.Length - 3);
            

            try
            {
                foreach (DataRow dr in dt1.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    DataRow drr = dt_表体2.NewRow();
                    dt_表体2.Rows.Add(drr);
                    drr["标识"] = str_后三位;
                    string sqld = string.Format("select 规格型号,计量单位 from 基础数据物料信息表 where 物料编码 = '{0}' ", dr["物料编码"].ToString());
                    //string sqld = string.Format("select 原ERP物料编号,n原ERP规格型号,货架描述,计量单位 from 基础数据物料信息表 where 物料编码 = '{0}'", r["物料编码"].ToString());

                    DataRow data = CZMaster.MasterSQL.Get_DataRow(sqld, CPublic.Var.strConn);
                    string sqld2 = string.Format("select 库存总数,货架描述 from 仓库物料数量表 where 物料编码 = '{0}' and  仓库号='{1}'", dr["物料编码"].ToString(),dr["仓库号"].ToString());
                    DataRow drrr = CZMaster.MasterSQL.Get_DataRow(sqld2, CPublic.Var.strConn);
                    drr["编码"] = dr["物料编码"].ToString();
                    drr["名称"] = dr["物料名称"].ToString();
                    drr["规格"] = dr["规格型号"].ToString();
                    drr["数量"] = decimal.Parse(dr["出库数量"].ToString()).ToString("0.######");
                    drr["单位"] = dr["计量单位"].ToString();
                    if (dr["包装方式"].ToString() == "其他方式" || dr["包装方式"].ToString() == "")
                    {
                        drr["包装方式"] = dr["包装方式"].ToString();
                    }
                    else  if(dr["包装方式编号"].ToString().Length<14)//20-6-1 之前的包装方式打印
                    {
                        string sql4 = string.Format("select 属性字段1 as 包装描述,属性值 as  包装名称 from 基础数据基础属性表 where 属性类别 =  '包装方式' and 属性值='{0}' ", dr["包装方式"].ToString());
                        DataTable dt_bao = CZMaster.MasterSQL.Get_DataTable(sql4, CPublic.Var.strConn);

                        drr["包装方式"] = dt_bao.Rows[0]["包装描述"].ToString();
                    }
                    //
                    else
                    {
                        string sql4 = $"select  规格型号 from 基础数据物料信息表 where 物料编码='{dr["包装方式编号"].ToString()}'";
                        DataTable dt_bao = CZMaster.MasterSQL.Get_DataTable(sql4, CPublic.Var.strConn);
                        drr["包装方式"] = dt_bao.Rows[0]["规格型号"].ToString();
                    }
                     
                
                    if (drrr!=null)
                    {
                        drr["库位"] = drrr["货架描述"].ToString();
                        drr["库存"] = decimal.Parse(drrr["库存总数"].ToString()).ToString("0.######");

                    }
                    drr["备注"] = dr["销售备注"].ToString();
                }
                DataTable1BindingSource.DataSource = dt_表体2;
                reportViewer1.LocalReport.ReportPath = "Report出库附件.rdlc";

                List<ReportParameter> lstParameter = new List<ReportParameter>() {
               new ReportParameter("收货单位",dt2.Rows[0]["客户名"].ToString()),
               new ReportParameter("地址",dt2.Rows[0]["送货地址"].ToString()),
               
               new ReportParameter("客户订单号",dt_目标客户.Rows[0]["客户订单号"].ToString()),

                new ReportParameter("目标客户",dt_目标客户.Rows[0]["目标客户"].ToString()),
              new ReportParameter("备注",dt2.Rows[0]["备注"].ToString()),
              new ReportParameter("送货方式",dt2.Rows[0]["送货方式"].ToString()),
              new ReportParameter("日期",DateTime.Now.ToString("yyyy-MM-dd")),
              new ReportParameter("申请人",dt2.Rows[0]["操作员"].ToString()),
              new ReportParameter("审核人",dt2.Rows[0]["审核人员"].ToString()),
               new ReportParameter("销售订单号",dt_目标客户.Rows[0]["销售订单号"].ToString()),
                new ReportParameter("编号",dt2.Rows[0]["出库通知单号"].ToString()),
            new ReportParameter("快递单号",dt2.Rows[0]["快递单号"].ToString()),
             new ReportParameter("要求出库日期",Convert.ToDateTime(dt2.Rows[0]["出库日期"]).ToString("yyyy-MM-dd")),
      
            };

                //PageSettings pages = new System.Drawing.Printing.PageSettings();
                //pages.Landscape = true;//强制设置纵向打印
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
                pg.Margins.Top = 45;
                pg.Margins.Bottom = 20;

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

        //public PaperSize(string  a, int b,int c);



    }
}
