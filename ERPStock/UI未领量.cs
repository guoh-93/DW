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
    public partial class UI未领量 : UserControl
    {
        #region 变量
        string s;
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        #endregion

        #region 加载
        public UI未领量()
        {
            InitializeComponent();
        }
        public UI未领量(string s)
        {
            this.s = s;
            InitializeComponent();
        }
        private void UI未领量_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(panel1, this.Name, cfgfilepath);
            try
            {                                             
                DataTable dt = new DataTable();
                string sql = string.Format(@"select 生产记录生产工单待领料明细表.* from [生产记录生产工单待领料明细表],生产记录生产工单待领料主表 
                        where 物料编码='{0}'  and 生产记录生产工单待领料主表.待领料单号 =[生产记录生产工单待领料明细表].待领料单号 
and 生产记录生产工单待领料主表.关闭=0  and 生产记录生产工单待领料明细表.完成 =0 and 生产记录生产工单待领料明细表.创建日期 > '2016-12-01 00:00:00'", s);
                ;                                                
                using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
                {
                    da.Fill(dt);
                    //dtM.Columns.Remove("GUID");
                }

                gridControl1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }
        #endregion

         //右击
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
         // 跳转至 生产领料等待明细
        private void gotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
          

                ////string s = dr[""].ToString().Trim();
                //fm空窗体 fm = new fm空窗体();
                //fm.Text = "生产领料等待明细";

                //fm.StartPosition = FormStartPosition.CenterScreen;
                ////ERPpurchase.frm采购单明细视图 frm = new ERPpurchase.frm采购单明细视图(s);
                ////fm.Controls.Add(frm);
                ////frm.Dock = DockStyle.Fill;
                //fm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #region 界面操作

        #endregion

    }
}
