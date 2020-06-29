using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace ERPSale
{
    public partial class UI关联制令界面 : UserControl
    {
        #region  变量
        //string str_销售订单号;
        string strconn = CPublic.Var.strConn;
        DataRow dr; //接收传递值
        DataTable dtM;
        #endregion


        #region 加载
        public UI关联制令界面()
        {
            InitializeComponent();
        }

        public UI关联制令界面( DataRow dr)
        {

            this.dr = dr;
            InitializeComponent();
        }
        private void UI关联制令界面_Load(object sender, EventArgs e)
        {
            try
            {
                    
                dataBindHelper1.DataFormDR(dr);
                fun_load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        #endregion


        #region 函数
        private void fun_load()
        {
            string sql = string.Format(@"select 生产记录生产制令子表.*,生产记录生产制令表.制令数量,生产记录生产制令表.生产车间  
                                        from  生产记录生产制令子表,生产记录生产制令表 where 生产记录生产制令子表.生产制令单号 =生产记录生产制令表.生产制令单号 
                                        and 生产记录生产制令子表.销售订单号='{0}'", dr["销售订单号"].ToString());
             using(SqlDataAdapter da =new SqlDataAdapter (sql,strconn))
             {
                 dtM = new DataTable();
                 da.Fill(dtM);
                 gridControl1.DataSource = dtM;
             }
        }


        #endregion

        private void 查看关联工单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            ERPSale.UI关联生产工单界面 frm = new UI关联生产工单界面(dr["生产制令单号"].ToString());
            CPublic.UIcontrol.AddNewPage(frm,"关联的工单列表");
           
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
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

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

    


     
      

     


    }
}
