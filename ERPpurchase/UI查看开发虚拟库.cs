using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERPpurchase
{
    public partial class UI查看开发虚拟库 : UserControl
    {

        #region
        string strcon = CPublic.Var.strConn;


        #endregion
        public UI查看开发虚拟库()
        {
            InitializeComponent();
        }

        private void UI查看开发虚拟库_Load(object sender, EventArgs e)
        {
            fun_load();
        }


        private void fun_load()
        {
            string sql = @"select 开发仓库数量表.*,n原ERP规格型号,大类,小类 from 开发仓库数量表,基础数据物料信息表 
                        where 库存总数>0 and  开发仓库数量表.物料编码=基础数据物料信息表.物料编码";

            DataTable dt = new DataTable();
            dt = CZMaster.MasterSQL.Get_DataTable(sql,strcon);
            gridControl1.DataSource = dt;

        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fun_load();
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gridView1.GetFocusedRowCellValue(gridView1.FocusedColumn));
                e.Handled = true;
            }
        }
    }
}
