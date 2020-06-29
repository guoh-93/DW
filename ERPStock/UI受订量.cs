using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ERPStock
{
    public partial class UI受订量 : UserControl
    {
        #region 变量
       
        string s;
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        #endregion

        #region 加载
        public UI受订量()
        {
            InitializeComponent();
        }
        public UI受订量(string s)
        {
            this.s = s;
            InitializeComponent();
        }

        private void UI受订量_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(panel1, this.Name, cfgfilepath);
          
            DataTable dt = new DataTable();
            string sql1 = string.Format
            ("select * from 销售记录销售订单明细表 where 物料编码='{0}' and 作废=0 and 生效 = 1 and 关闭=0 and 明细完成 = 0 and 生效日期 > '2016-12-01 00:00:00'", s);
            using (SqlDataAdapter da = new SqlDataAdapter(sql1, strconn))
            {
                da.Fill(dt);
            }
            gridControl1.DataSource = dt;
        }
        #endregion 
        // 右键菜单
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                }
            }
            catch
            {

            }
        }
        //跳转
        private void gotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                string cs = dr["销售订单号"].ToString().Trim();
                //string name = string.Format("销售订单明细({0}_{1})", dr["物料编码"].ToString().Trim(), dr["物料名称"].ToString().Trim());
                string name = string.Format("销售订单明细({0})",cs);
                
                ////Sale.frm销售单证详细界面_视图 frm = new Sale.frm销售单证详细界面_视图(cs);
                //CPublic.UIcontrol.AddNewPage(frm, name);
                //frm.Dock = DockStyle.Fill;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

      
      

      

    }
}
