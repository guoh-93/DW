using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PLCView
{
    public partial class frm6WMain : UserControl
    {
        #region 窗体变量

        int iFlexHeight = 63;


        #endregion

        public frm6WMain()
        {
            InitializeComponent();
        }

        private void frm6WMain_Load(object sender, EventArgs e)
        {
            fun_重排();
            
            frm6W fm1 = new frm6W();
            fm1.MachineName = "HC_FR6W1";
            panel_W1.Controls.Add(fm1);
            fm1.Dock = DockStyle.Fill;


            frm6W fm2 = new frm6W();
            fm2.MachineName = "HC_FR6W2";
            panel_W2.Controls.Add(fm2);
            fm2.Dock = DockStyle.Fill;

            frm6W fm3 = new frm6W();
            fm3.MachineName = "HC_FR6W3";
            panel_W3.Controls.Add(fm3);
            fm3.Dock = DockStyle.Fill;

            frm6W fm4 = new frm6W();
            fm4.MachineName = "HC_FR6W4";
            panel_W4.Controls.Add(fm4);
            fm4.Dock = DockStyle.Fill;
        }

        private void frm6WMain_SizeChanged(object sender, EventArgs e)
        {
            fun_重排();
        }


        #region 变化尺寸
        private void fun_重排()
        {
            try
            {
                panel_W1.Left = 0;
                panel_W1.Top = iFlexHeight;
                panel_W1.Height = (this.Height - iFlexHeight) / 2;
                panel_W1.Width = (this.Width - 4) / 2;
            }
            catch { }
            try
            {
                panel_W2.Left = (this.Width - 4) / 2;
                panel_W2.Top = iFlexHeight;
                panel_W2.Height = (this.Height - iFlexHeight) / 2;
                panel_W2.Width = (this.Width - 4) / 2;
            }
            catch { }
            try
            {
                panel_W3.Left = 0;
                panel_W3.Top = iFlexHeight + (this.Height - iFlexHeight) / 2; ;
                panel_W3.Height = (this.Height - iFlexHeight) / 2;
                panel_W3.Width = (this.Width - 4) / 2;
            }
            catch { }
            try
            {
                panel_W4.Left = (this.Width - 4) / 2;
                panel_W4.Top = iFlexHeight + (this.Height - iFlexHeight) / 2; ;
                panel_W4.Height = (this.Height - iFlexHeight) / 2;
                panel_W4.Width = (this.Width - 4) / 2;
            }
            catch { }

        }

        #endregion

        private void tm_SN_Tick(object sender, EventArgs e)
        {
            text_SN1.EditValue = PLCC.W6_SNSCAN.SN1;
            text_SN2.EditValue = PLCC.W6_SNSCAN.SN2;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PLCC.W6_SNSCAN.ClearSN1();
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }


    }
}
