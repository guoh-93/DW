using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ERPStock
{
    //9/27检查 应该是没有用的 界面
    public partial class frm仓库盘点历史列表 : UserControl
    {
        #region 变量
        string strconn = CPublic.Var.strConn;

        DataTable dt_仓库物料盘点表;
        SqlDataAdapter da;
         string str_盘点批次号;
        #endregion


        public frm仓库盘点历史列表()
        {
            InitializeComponent();
        }

        public void fun_load()
        {
            dt_仓库物料盘点表 = new DataTable();
            string sql = "select * from 仓库物料盘点表";
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_仓库物料盘点表);
            gridControl1.DataSource = dt_仓库物料盘点表;


        }
        private void frm仓库盘点历史列表_Load(object sender, EventArgs e)
        {
            fun_load();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barEditItem1.EditValue = null;
            barEditItem2.EditValue = null;
            dt_仓库物料盘点表 = new DataTable();
            string sql = "select * from 仓库物料盘点表";
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_仓库物料盘点表);
            gridControl1.DataSource = dt_仓库物料盘点表;
        }

        private void RowCClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks==2)
            {
                //DataRow r = (this.BindingContext[gridView1.DataSource].Current as DataRowView).Row;//选中某一行
                DataRow r = gridView1.GetDataRow(gridView1.FocusedRowHandle);//获取焦点所在行
                str_盘点批次号 = r["盘点批次号"].ToString();
                frm仓库盘点明细 frm = new frm仓库盘点明细(str_盘点批次号);
                CPublic.UIcontrol.AddNewPage(frm, "盘点明细");   
            }
            
        }

        private void barEditItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            CPublic.UIcontrol.ClosePage();
        }

        private void EditVChanged(object sender, EventArgs e)
        {
            DateTime time3 = Convert.ToDateTime(barEditItem1.EditValue);
            DateTime time2 = Convert.ToDateTime(barEditItem2.EditValue);

            //Convert.ToDateTime(barEditItem1.EditValue).AddDays(1).AddSeconds(-1);

            //System.DateTime.Today.AddDays(-1);

            //DateTime time3 = time1.AddDays(1).AddSeconds(-1);
            DateTime time4 = time2.AddDays(1).AddSeconds(-1);



            //DateTime.Parse(barEditItem1.EditValue.ToString());



            //barEditItem1.EditValue
            DataView dv = new DataView(dt_仓库物料盘点表);
            //Convert(varchar(10), barEditItem1.EditValue, 120);
            dv.RowFilter = String.Format("盘点生效时间>='{0}'and 盘点生效时间<='{1}'", time3, time4);
            dv.Sort = "盘点生效时间 desc ";
            gridControl1.DataSource = dv;
        }

        private void EditVChanged2(object sender, EventArgs e)
        {
            DateTime time3 = Convert.ToDateTime(barEditItem1.EditValue);
            DateTime time2 = Convert.ToDateTime(barEditItem2.EditValue);

            //Convert.ToDateTime(barEditItem1.EditValue).AddDays(1).AddSeconds(-1);

            //System.DateTime.Today.AddDays(-1);

            //DateTime time3 = time1.AddDays(1).AddSeconds(-1);
            DateTime time4 = time2.AddDays(1).AddSeconds(-1);



            //DateTime.Parse(barEditItem1.EditValue.ToString());



            //barEditItem1.EditValue
            DataView dv = new DataView(dt_仓库物料盘点表);
            //Convert(varchar(10), barEditItem1.EditValue, 120);
            dv.RowFilter = String.Format("盘点生效时间>='{0}'and 盘点生效时间<='{1}'", time3, time4);
            dv.Sort = "盘点生效时间 desc ";
            gridControl1.DataSource = dv;
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barEditItem1.EditValue = null;
            barEditItem2.EditValue = null;
            dt_仓库物料盘点表 = new DataTable();
            string sql = "select * from 仓库物料盘点表";
            da = new SqlDataAdapter(sql, strconn);
            da.Fill(dt_仓库物料盘点表);
            gridControl1.DataSource = dt_仓库物料盘点表;
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

     
    }
}
