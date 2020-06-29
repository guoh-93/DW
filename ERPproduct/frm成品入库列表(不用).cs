using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPproduct
{
    public partial class frm成品入库列表 : UserControl
    {

        #region 变量
        DataTable dtM = new DataTable();
        DataView dvM;
        string strconn = CPublic.Var.strConn;
        #endregion

        #region 加载
        public frm成品入库列表()
        {
            InitializeComponent();
            barEditItem4.EditValue = "已生效";
        }

#pragma warning disable IDE1006 // 命名样式
        private void frm成品入库_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
            devGridControlCustom1.strConn = CPublic.Var.strConn;
            fun_load();
        }

        #endregion

#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            string sql = "select * from 生产记录生产工单表 where  生效=true  ";
            using (SqlDataAdapter da = new SqlDataAdapter(sql, strconn))
            {
                 da.Fill(dtM);
                
                
            }

        }
         
       

         //刷新
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            dtM = new DataTable();
            fun_load();
        }
         //新增
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

        }
        //关闭
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            CPublic.UIcontrol.ClosePage();
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Clicks == 2)
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                ERPproduct.frm成品入库单列表视图 frm = new frm成品入库单列表视图(dr["成品入库单号"].ToString());
                CPublic.UIcontrol.AddNewPage(frm, "成品入库明细");
            }
        }

      

        
       
    }
}
