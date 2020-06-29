using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERPStock
{
    public partial class ui工单关闭退料清单 : UserControl
    {
        public ui工单关闭退料清单()
        {
            InitializeComponent();
        }

        DataTable dt_main ,dt_mx;
        public ui工单关闭退料清单(object a ,object b )
        {
            InitializeComponent();
            dt_main = (DataTable)a;
            dt_mx = (DataTable)b;

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void ui工单关闭退料清单_Load(object sender, EventArgs e)
        {
            gcP.DataSource = dt_mx;
            DataRow drm = dt_main.Rows[0];
            dataBindHelper1.DataFormDR(drm);
         //   dataBindHelper1.
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
