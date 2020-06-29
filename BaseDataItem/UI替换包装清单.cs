using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace BaseData
{
    public partial class UI替换包装清单 : UserControl
    {
        DataTable dtM = new DataTable();
        public UI替换包装清单()
        {
            InitializeComponent();
        }

        private void 跳转包装清单替换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            BaseData.frm基础数据包装清单界面 fm = new BaseData.frm基础数据包装清单界面(dr["物料编码"].ToString());
            CPublic.UIcontrol.AddNewPage(fm, "包装清单查询");

        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(gridControl1, new Point(e.X, e.Y));
                gridView1.CloseEditor();
                this.BindingContext[dtM].EndCurrentEdit();

            }
        }

        private void UI替换包装清单_Load(object sender, EventArgs e)
        {
            string sql = @"select 需确认包装清单表.*,基础数据物料信息表.物料名称,n原ERP规格型号,物料编码 from [需确认包装清单表],基础数据物料信息表 
                                where  [需确认包装清单表].物料编码=基础数据物料信息表.物料编码 and 修改完成=0";
            dtM = CZMaster.MasterSQL.Get_DataTable(sql, CPublic.Var.strConn);
            gridControl1.DataSource = dtM;
          
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UI替换包装清单_Load(null, null);
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
    }
}
