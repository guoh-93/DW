using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace ERPpurchase
{
    public partial class ui请购单列表 : UserControl
    {

        string strcon = CPublic.Var.strConn;
        string str_打印机;
        string cfgfilepath = "";
        DataTable dtM = new DataTable();
        DataTable dtP = new DataTable();



        public ui请购单列表()
        {
            InitializeComponent();
        }

        private void ui请购单列表_Load(object sender, EventArgs e)
        {
            cfgfilepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            cfgfilepath = cfgfilepath + @"\FormLayout";
            if (!Directory.Exists(cfgfilepath))
            {
                Directory.CreateDirectory(cfgfilepath);
            }
            ERPorg.Corg x = new ERPorg.Corg();
            x.UserLayout(xtraTabControl1, this.Name, cfgfilepath);
            DateTime t = CPublic.Var.getDatetime().Date;
            txt_riqi1.EditValue = t.AddMonths(-3);
            txt_riqi2.EditValue = t;
        }


        private void fun_load(DateTime t1, DateTime t2)
        {
            string s = string.Format("select  *  from 请购单主表 where 创建日期>'{0}'  and 创建日期<'{1}'", t1, t2);
            dtM = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gc1.DataSource = dtM;
            s = string.Format(@"select  mx.*,物料名称,规格型号,zb.创建日期,zb.备注,申请人,部门名称 from 请购单明细表 mx
                left join 请购单主表 zb on mx.请购单号=zb.请购单号 
                left join 基础数据物料信息表 base on base.物料编码=mx.物料编码   
                where zb.创建日期>'{0}' and zb.创建日期<'{1}'", t1, t2);
            dtP = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gridControl1.DataSource = dtP;
        }

        private void search(string s_dh)
        {
            string s = string.Format("select  a.*,base.物料名称,base.规格型号  from 请购单明细表 a " +
                "  left join 基础数据物料信息表 base on base.物料编码=a.物料编码    where 请购单号='{0}'", s_dh);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gcP.DataSource = dt;

        }
        private void freshen(DataRow dr )
        {
            string s = string.Format("select  *  from 请购单主表 where 请购单号='{0}'", dr["请购单号"].ToString());
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gcP.DataSource = dt;

            dr.ItemArray = dt.Rows[0].ItemArray;
            dr.AcceptChanges();
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DateTime t1 = Convert.ToDateTime(txt_riqi1.EditValue).Date;
                DateTime t2 = Convert.ToDateTime(txt_riqi2.EditValue).Date.AddDays(1).AddSeconds(-1);

                fun_load(t1, t2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                if (dr == null) return;
                freshen(dr);
                search(dr["请购单号"].ToString());
                if (e.Clicks == 2)
                {
                    DataRow r = gv1.GetDataRow(gv1.FocusedRowHandle) ; //选中一行
                    if (!Convert.ToBoolean(r["审核"]))
                    {
                        ui请购单 ui = new ui请购单(r);
                        CPublic.UIcontrol.AddNewPage(ui, "请购单录入");
                    }
                   
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gv1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                if (dr != null) search(dr["请购单号"].ToString());
            }
            catch
            {

            }
        }

        private void barLargeButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }
        //导出
        private void barLargeButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
}
