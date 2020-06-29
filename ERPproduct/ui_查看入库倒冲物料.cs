using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace ERPproduct
{
    public partial class ui_查看入库倒冲物料 : UserControl
    {
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        DataRow dr;
        DataTable dt_入库倒冲;
        public ui_查看入库倒冲物料()
        {
            InitializeComponent();
        }
        public ui_查看入库倒冲物料(DataRow r,DataTable dt)
        {
            InitializeComponent();
            dr = r;
            dt_入库倒冲 = dt;
        }

        private void ui_查看入库倒冲物料_Load(object sender, EventArgs e)
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
                x.UserLayout(panel2, this.Name, cfgfilepath);
                textBox1.Text = dr["物料编码"].ToString();
                textBox2.Text = dr["物料名称"].ToString();
                textBox3.Text = dr["规格型号"].ToString();
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fun_load()
        {           
            dt_入库倒冲.Columns.Add("此单需求量", typeof(decimal));
            if (dt_入库倒冲.Rows.Count>0)
            {                
                foreach (DataRow dr_rkdc in dt_入库倒冲.Rows)
                {
                    dr_rkdc["此单需求量"] = Convert.ToDecimal(dr_rkdc["数量"]) * Convert.ToDecimal(dr["生产数量"]);
                }
            }
            gridControl1.DataSource = dt_入库倒冲;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
