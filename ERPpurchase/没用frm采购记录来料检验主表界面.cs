using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class 没用frm采购记录来料检验主表界面 : UserControl
    {
        #region 成员
        string strconn = CPublic.Var.strConn;
        DataTable dtM;
        DataRow drM;
        #endregion

        #region 自用类
        public 没用frm采购记录来料检验主表界面()
        {
            InitializeComponent();
        }
        private void frm采购记录来料检验主表界面_Load(object sender, EventArgs e)
        {
            devGridControlCustom1.UserName = CPublic.Var.LocalUserID;
            devGridControlCustom1.strConn = CPublic.Var.strConn;
        }
        #endregion

        #region 方法

        #endregion

        #region 界面操作
        //刷新
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
        }
        //新增
        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frm采购记录来料检验详细界面 fm = new frm采购记录来料检验详细界面();
            CPublic.UIcontrol.AddNewPage(fm, "来料检验单");
        }
        //关闭
        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        #endregion
    }
}
