using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data .SqlClient;
using System.IO;
namespace ERPStock
{
    public partial class UI在制量 : UserControl
    {
        #region 变量
        String s;
        string strconn = CPublic.Var.strConn;
        string cfgfilepath = "";
        #endregion

        #region 加载
        public UI在制量()
        {
            InitializeComponent();
        }
        public UI在制量(string s)
        {
            this.s = s;
            InitializeComponent();
        }
        #endregion

        private void UI在制量_Load(object sender, EventArgs e)
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
             string sql = string.Format(@"select 生产记录生产工单表.生产工单号,生产制令单号,j.物料编码 ,生产记录生产工单表.物料名称,生产数量,车间名称,已检验数量,isnull(已入库数量,0)已入库数量  from 生产记录生产工单表
  left  join  (select 生产工单号,sum(入库数量) as 已入库数量 from  生产记录成品入库单明细表  group by 生产工单号)x  on    x.生产工单号= 生产记录生产工单表.生产工单号
  left  join  基础数据物料信息表 j on   j.物料编码= 生产记录生产工单表.物料编码  where  生产记录生产工单表.生效 = 1 
  and 生产记录生产工单表.完成 = 0 and 生产记录生产工单表.关闭 = 0 and 生效日期>'2016-12-1' and j.物料编码='{0}' ", s);
             using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
             {
                da.Fill(dt);
                
                 //dtM.Columns.Remove("GUID");
             }
                
             gridControl1.DataSource = dt;
    
        }

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
        //跳转至 frm生产制令表
        private void gotoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                ERPproduct.frm生产制令表 frm = new ERPproduct.frm生产制令表(dr["生产制令单号"].ToString().Trim());
                CPublic.UIcontrol.AddNewPage(frm, string.Format("生产制令({0})", dr["生产制令单号"].ToString()));
                frm.Dock = DockStyle.Fill;
               
             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
