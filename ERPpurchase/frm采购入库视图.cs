using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace ERPpurchase
{
    public partial class frm采购入库视图 : UserControl
    {
        string strcon = CPublic.Var.strConn;
        string strrkdh = "";
        string wlbm = "";
        string str_采购单 = "";

        string sql ="";
        string sql1="";
        int b = 0; 
      
        /// <summary>
        /// b=1 物料编码  b=2采购单号
        /// </summary>
        /// <param name="wlbm"></param>
        /// <param name="b"></param>
        public frm采购入库视图(string wlbm,int b)
        {
            this.b = b;
            if (b == 1)
            {
                this.wlbm = wlbm;
               
                //sql = string.Format("select * from 采购记录采购单入库主表 where 物料编码='{0}'",wlbm);
                sql1 = string.Format(@"select rkmx.*  from 采购记录采购单入库明细 rkmx    where rkmx.物料编码='{0}'", wlbm);
            }
            else if(b == 2)
            {
                str_采购单 = wlbm;
                sql1 = string.Format(@"select rkmx.*,cmx.未税单价 采购不含税单价, round(cmx.未税单价*入库量,2) 不含税金额  from 采购记录采购单入库明细 rkmx   
                         left join 采购记录采购单明细表  cmx on rkmx.采购单明细号=cmx.采购明细号 where rkmx.采购单号='{0}'", wlbm);

            }
            InitializeComponent();

            panel1.Visible = false;

        }

        public frm采购入库视图(string rkdh)
        {
             strrkdh = rkdh;
             InitializeComponent();
            
             sql = string.Format("select * from 采购记录采购单入库主表 where 入库单号='{0}'", strrkdh);
             sql1 = string.Format(@"select rkmx.*  from 采购记录采购单入库明细 rkmx  where rkmx.入库单号='{0}'", strrkdh);
        }


        DataTable dt_PutIn;

        DataRow drm;

        DataTable dt_PutDetail;

        private void frm采购入库视图_Load(object sender, EventArgs e)
        {
            try
            {
                 
                fun_PutInDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }        
        }

        //入库明细视图
        private void fun_PutInDisplay()
        {
            try
            {
                SqlDataAdapter da;
                if (b == 0)
                {
                    da = new SqlDataAdapter(sql, strcon);
                    dt_PutIn = new DataTable();
                    da.Fill(dt_PutIn);
                    if (dt_PutIn.Rows.Count > 0)
                    {
                        drm = dt_PutIn.Rows[0];
                        dataBindHelper1.DataFormDR(drm);

                        da = new SqlDataAdapter(sql1, strcon);
                        dt_PutDetail = new DataTable();
                        da.Fill(dt_PutDetail);
                        gc_rukuview.DataSource = dt_PutDetail;
                    }
                }
                else
                {
                    da = new SqlDataAdapter(sql1, strcon);
                    dt_PutDetail = new DataTable();
                    da.Fill(dt_PutDetail);
                    gc_rukuview.DataSource = dt_PutDetail;
                }
            }
            catch (Exception ex)
            {
                CZMaster.MasterLog.WriteLog(ex.Message + " fun_PutInDisplay");
                throw new Exception(ex.Message);
            }
        }

        private void 采购单明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow dr = gv_rukuview.GetDataRow(gv_rukuview.FocusedRowHandle);
            string name =string .Format("{0}_{1}的采购明细",dr["物料编码"].ToString().Trim(),dr["物料名称"].ToString().Trim());
            frm采购单明细视图 frm = new frm采购单明细视图(dr["采购单号"].ToString().Trim());
            CPublic.UIcontrol.AddNewPage(frm,name);
        }

        private void gv_rukuview_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(gc_rukuview, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void gv_rukuview_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            //行号设置 
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString("00");
            } 
        }

        private void gv_rukuview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(gv_rukuview.GetFocusedRowCellValue(gv_rukuview.FocusedColumn));
                e.Handled = true;
            }
        }
    }
}
