using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BaseData
{
    public partial class frmPDM接口 : UserControl
    {

        #region 变量

        string strcon = CPublic.Var.strConn;
        DataTable dtM;
        #endregion



        #region 加载
        public frmPDM接口()
        {
            InitializeComponent();
        }

        private void frmPDM接口_Load(object sender, EventArgs e)
        {

            fun_load();
        }

        #endregion




        #region 函数

        private void fun_load()
        {
            string sql = "select * from TH_PDM_BOM ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strcon))
            {
                dtM = new DataTable();
                da.Fill(dtM);
                gridControl1.DataSource = dtM;
            }
        }

        #endregion

       
        
        
        
        #region 界面操作
          //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }
         //关闭
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        //保存 
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
        #endregion

        

       




    }
}
