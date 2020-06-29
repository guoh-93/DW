using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPreport
{
    public partial class 其他出库查询 : Form
    {
        public 其他出库查询()
        {
            InitializeComponent();
        }

        private void 其他出库查询_Load(object sender, EventArgs e)
        {
     
 
            this.reportViewer1.RefreshReport();
            System.Drawing.Printing.PageSettings pg = new System.Drawing.Printing.PageSettings();

            pg.Margins.Bottom = 20;
            pg.Margins.Bottom = 50;

        
        }
    }
}
