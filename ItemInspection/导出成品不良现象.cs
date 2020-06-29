using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
namespace ItemInspection
{
    public partial class 导出成品不良现象 : Form
    {
        string strcoon = CPublic.Var.strConn;
        DataTable dtM;
        string file;
        public 导出成品不良现象()
        {
            InitializeComponent();
        }
        public 导出成品不良现象(DataTable dt_参, string fileName)
        {
            InitializeComponent();
            dtM = dt_参;
            file = fileName;
        }
        private void 导出成品不良现象_Load(object sender, EventArgs e)
        {

            DataTable1BindingSource.DataSource = dtM;

    
           
            this.reportViewer1.LocalReport.EnableExternalImages = true;

            //转换图片成2进制
            //FileStream fs = new FileStream(file, FileMode.Open);
            //BinaryReader br = new BinaryReader(fs);
            //byte[] imageBuffer = new byte[br.BaseStream.Length];
            //br.Read(imageBuffer, 0, Convert.ToInt32(br.BaseStream.Length));
            //string textString = System.Convert.ToBase64String(imageBuffer);
            //fs.Close();
            //br.Close();
            //转2进制结束

            //设置参数
            //ReportParameter rptParaImage = new ReportParameter("ReportParameter1", textString);
            //this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { rptParaImage });

            file = @"file:///" + file;
            //reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", file));//报表绑定参数
            //reportViewer1.LocalReport.DataSources.Add(rds);
            //reportViewer1.LocalReport.DataSources.Add(rds10);


           // DataTable2BindingSource.DataSource = t;
        
           // System.Diagnostics.Process.Start(file);  

            this.reportViewer1.RefreshReport();
           
            //reportViewer1.EnableExternalImages = true;
        }
    }
}
