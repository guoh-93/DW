using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CPublic;

namespace ERPStock
{
    public partial class frm仓库初始化 : UserControl
    {
        #region 变量
        string strconn = CPublic.Var.strConn;
        //DataTable dtM; //未初始化列表的dt
        #endregion

        #region 加载
        public frm仓库初始化()
        {
            InitializeComponent();
        }
        private void 仓库初始化_Load(object sender, EventArgs e)
        {
            devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
            devGridControlCustom1.strConn = CPublic.Var.strConn;
            fun_load();
          
        }
        #endregion

        #region 函数
        //加载已初始化列表
        void fun_load()
        {
            string sql = "select * from 仓库物料表";
            using(SqlDataAdapter da =new SqlDataAdapter (sql,strconn))  
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataView dv = new DataView(dt);
                gridControl1.DataSource =dv;

            }
        }

                                                                                 
        #endregion

        
        #region 界面操作
        // 查看未初始化列表
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fm空窗体 fm = new fm空窗体();
            fm.Text = "初始化界面";
            fm.StartPosition = FormStartPosition.CenterScreen;
            frm初始化UI ui = new frm初始化UI();
            ui.Dock = DockStyle.Fill;
            fm.Controls.Add(ui);
            fm.ShowDialog();
            

 
        }

        #endregion

      
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

       
    }
}
