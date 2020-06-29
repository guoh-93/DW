using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTab;

namespace ERPproduct
{
#pragma warning disable IDE1006 // 命名样式
    public partial class frm快速检验入库单加载 : UserControl
#pragma warning restore IDE1006 // 命名样式
    {
        public frm快速检验入库单加载()
        {
            InitializeComponent();
        }
        string strcoon1 = CPublic.Var.geConn("DW");
#pragma warning disable IDE1006 // 命名样式
        private void frm快速检验入户单加载_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {

            DateTime t = CPublic.Var.getDatetime();
            barEditItem1.EditValue = t.AddDays(-30);
            barEditItem2.EditValue = t.AddDays(1).AddSeconds(-1);




        }
        //查询
#pragma warning disable IDE1006 // 命名样式
        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            fun_load();
        }

#pragma warning disable IDE1006 // 命名样式
        private void fun_load()
#pragma warning restore IDE1006 // 命名样式
        {
            string str = string.Format(@"select a.cBusType,a.cCode as 入库单号,a.dDate,b.cInvCode,b.iQuantity as 数量,b.cFree1,b.cFree2,b.cmocode,c.cInvName as 物料名称 from rdrecords10 b 
             left join rdrecord10 a on a.ID = b.ID
             left join Inventory c  on b.cInvCode = c.cInvCode
             where a.dDate>='{0}' and a.dDate <='{1}' order by dDate desc", barEditItem1.EditValue, barEditItem2.EditValue);
        using(SqlDataAdapter da = new SqlDataAdapter(str,strcoon1))
        {
            DataTable dtM = new DataTable();
            da.Fill(dtM);
            gc.DataSource = dtM;
        
        
        }
        
        }

        private void 跳转检验ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gv.GetDataRow(gv.FocusedRowHandle);
                frm快速检验 frm = new frm快速检验(dr["cmocode"].ToString(), Convert.ToDecimal(dr["数量"]), dr["入库单号"].ToString(), dr["cInvCode"].ToString());

                CPublic.UIcontrol.Showpage(frm, "显示生产工单");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //XtraTabPage xtp = XTC.TabPages.Add(Caption);
            //xtp.ShowCloseButton = DefaultBoolean.Default;

            //xtp.Controls.Add(fm);
            //fm.Dock = DockStyle.Fill;
            //XTC.SelectedTabPage = xtp;
        }

#pragma warning disable IDE1006 // 命名样式
        private void gv_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
        //颜色
#pragma warning disable IDE1006 // 命名样式
        private void gv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
#pragma warning restore IDE1006 // 命名样式
        {




        }






    }
}
