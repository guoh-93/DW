using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ReworkMould
{
    public partial class ui_驳回记录查询 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataTable dtm;
        public ui_驳回记录查询()
        {
            InitializeComponent();
        }

        public ui_驳回记录查询(DataTable dt)
        {
            InitializeComponent();
            dtm = dt;
        }


        private void ui_驳回记录查询_Load(object sender, EventArgs e)
        {
            try
            {
                cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                cfgfilepath = cfgfilepath + @"\FormLayout";
                if (!Directory.Exists(cfgfilepath))
                {
                    Directory.CreateDirectory(cfgfilepath);
                }
                ERPorg.Corg x = new ERPorg.Corg();
                x.UserLayout(panel1, this.Name, cfgfilepath);
                gridControl2.DataSource = dtm;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ParentForm.Close();
        }
    }
}
