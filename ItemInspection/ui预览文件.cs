using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ItemInspection
{
    public partial class ui预览文件 : UserControl
    {


        string filename = "";
        public ui预览文件()
        {
            InitializeComponent();
        }

        public ui预览文件( string path)
        {
            this.filename = path;
            InitializeComponent();
        
        }

        private void ui预览文件_Load(object sender, EventArgs e)
        {
            axAcroPDF1.setPageMode("thumbs");
            axAcroPDF1.setPageMode("None");
            axAcroPDF1.setShowToolbar(false);
           
            axAcroPDF1.LoadFile(filename);
       
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
  
        protected override void WndProc(ref Message m)
        {
            if ((int)m.WParam == 516)
            {
                //按下鼠标右键
                m.Msg = 528;
               
                m.WParam = new IntPtr(513);
           
            }
            base.WndProc(ref m);
        }

 

    
       
    }
}
