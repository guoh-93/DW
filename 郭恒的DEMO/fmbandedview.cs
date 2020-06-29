using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 郭恒的DEMO
{
    public partial class fmbandedview : Form
    {
        public fmbandedview()
        {
            InitializeComponent();
        }

        private void fmbandedview_Load(object sender, EventArgs e)
        {
            //DevExpress.XtraGrid.Views.BandedGrid.GridBand gb = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            //gb.Caption = "测试";
            //DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn bgc = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            //bgc.FieldName = "测试";
            //gb.Columns.Add(bgc);
            //bandedGridView1.Bands.Add(gb);
            ////bandedGridView1.Columns.Add(bgc);
      
            //gb.AutoFillDown = true;
            //gb.Name = "测试";

            DataTable dt = new DataTable();
            dt.Columns.Add("测试");
            DataRow dr = dt.NewRow();
            dr["测试"] = "是";
            dt.Rows.Add(dr);
            gridControl1.DataSource = dt;


        }
    }
}
