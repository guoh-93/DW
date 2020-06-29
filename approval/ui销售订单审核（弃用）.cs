using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
namespace approval
{
    public partial class ui销售订单审核 : UserControl
    {

        #region 变量
        DataTable dt_ll;
        DataTable dt_r;
        string strcon = CPublic.Var.strConn;
        DataTable dt_权限;
        // string strConn_FS = CPublic.Var.geConn("FS");
        #endregion

        public ui销售订单审核()
        {
            InitializeComponent();
        }

        private void barLargeButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_check();

                DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
                if (MessageBox.Show(string.Format("确认审销售单{0}", dr["关联单号"]), "询问？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string fileadress = "";
                    fun_审核(dr["关联单号"].ToString(), fileadress);

                    Thread ths;
                    ths = new Thread(fun_刷受订);
                    ths.IsBackground = true;
                    ths.Start(dr["关联单号"].ToString());

                    MessageBox.Show("审核成功");
                    fun_load();

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void fun_刷受订(object str_销售单)
        {
            string str = str_销售单 as string;
            string s = string.Format(@"select  物料编码,仓库号 from  销售记录销售订单明细表  where 关闭=0 and   作废=0 and 销售订单号='{0}' group by 物料编码,仓库号", str_销售单);
            DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataTable dt = StockCore.StockCorer.fun_四个量(dt_mx);
            string ss = "select  * from 仓库物料数量表 where 1<>1";
            using (SqlDataAdapter da = new SqlDataAdapter(ss, strcon))
            {
                new SqlCommandBuilder(da);
                da.Update(dt);
            }

        }
        private void fun_load()
        {
            DataTable dt = ERPorg.Corg.fun_hr("销售", CPublic.Var.LocalUserID);
            string sx = "";
            if (dt.Rows.Count > 0)
            {
                sx = "and 待审核人ID in (";
                foreach (DataRow r in dt.Rows)
                {
                    sx = sx + string.Format("'{0}',", r["工号"]);
                }
                sx = sx.Substring(0, sx.Length - 1) + ")";
            }
            string s = string.Format(@" select  a.*,客户名 as 客户,c.税率 from [单据审核申请表] a 
            left  join 销售记录销售订单主表 c    on c.销售订单号=a.关联单号 
             where    a.作废=0 and a.审核=0 and c.生效=0 and a.单据类型='销售' and 待审核=1 {0} ", sx);
            dt_ll = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            DataView dv = new DataView(dt_ll);



            dv.RowFilter = string.Format("待审核人ID='{0}'", CPublic.Var.LocalUserID);
            gc1.DataSource = dv;
            s = " select  * from 销售记录销售订单明细表  where 1=2";
            dt_r = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gcP.DataSource = dt_r;
            checkBox1.Checked = false;
        }

        private void gv1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            fun_loadmx(dr["关联单号"].ToString());
        }

        private void fun_loadmx(string ss)
        {
            string s = string.Format(@"select  a.*,b.原ERP物料编号 from  销售记录销售订单明细表  a
            left join 基础数据物料信息表 b on a.物料编码=b.物料编码
            where  作废=0 and 销售订单号='{0}'", ss);
            dt_r = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            gcP.DataSource = dt_r;
        }

        private void barLargeButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CPublic.UIcontrol.ClosePage();
        }

        private void fun_check()
        {
            // 可能退回审核
            DataRow dr = gv1.GetDataRow(gv1.FocusedRowHandle);
            string s = string.Format("select * from  销售记录销售订单主表 where 待审核=1 and 作废=0 and 销售订单号='{0}'", dr["关联单号"]);
            DataTable t = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            if (t.Rows.Count == 0) //状态有变更
            {
                throw new Exception("该单据状态已更改，刷新后重试");

            }
        }
        private void fun_审核(string str_销售单, string str_文件地址)
        {
            DateTime time = CPublic.Var.getDatetime();
            string s = string.Format("select  * from  销售记录销售订单主表 where  作废=0 and 销售订单号='{0}'", str_销售单);
            DataTable dt = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            s = string.Format(@"select  * from  销售记录销售订单明细表  where  作废=0 and 销售订单号='{0}'", str_销售单);
            DataTable dt_mx = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            dt.Rows[0]["生效"] = 1;
            dt.Rows[0]["生效人员"] = dt.Rows[0]["录入人员"];
            dt.Rows[0]["生效人员ID"] = dt.Rows[0]["录入人员ID"];
            dt.Rows[0]["生效日期"] = time;
            dt.Rows[0]["审核"] = 1;
            dt.Rows[0]["审核人员"] = CPublic.Var.localUserName;
            dt.Rows[0]["审核人员ID"] = CPublic.Var.LocalUserID;
            dt.Rows[0]["审核日期"] = time;
            foreach (DataRow dr in dt_mx.Rows)
            {

                dr["生效"] = 1;
                dr["生效日期"] = time;
                dr["含税销售价"] = dr["税后单价"];

            }
            s = string.Format("select  * from  [单据审核申请表] where  作废=0 and 关联单号='{0}'", str_销售单);

            DataTable dt_审核 = CZMaster.MasterSQL.Get_DataTable(s, strcon);
            dt_审核.Rows[0]["审核"] = 1;
            dt_审核.Rows[0]["最终审核人"] = CPublic.Var.localUserName;
            dt_审核.Rows[0]["最终审核人ID"] = CPublic.Var.LocalUserID;
            dt_审核.Rows[0]["审核时间"] = time;
            dt_审核.Rows[0]["文件地址"] = str_文件地址;




            SqlConnection conn = new SqlConnection(strcon);
            conn.Open();
            SqlTransaction ts = conn.BeginTransaction("ASA"); //事务的名称
            SqlCommand cmd1 = new SqlCommand("select * from 销售记录销售订单主表 where 1<>1", conn, ts);
            SqlCommand cmd = new SqlCommand("select * from 销售记录销售订单明细表 where 1<>1", conn, ts);
            try
            {
                SqlDataAdapter da;
                da = new SqlDataAdapter(cmd1);
                new SqlCommandBuilder(da);
                da.Update(dt);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_mx);
                cmd = new SqlCommand("select * from 单据审核申请表 where 1<>1", conn, ts);
                da = new SqlDataAdapter(cmd);
                new SqlCommandBuilder(da);
                da.Update(dt_审核);

                ts.Commit();
            }
            catch
            {
                ts.Rollback();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                DataView dv = new DataView(dt_ll);
                dv.RowFilter = string.Format("待审核人ID='{0}'", CPublic.Var.LocalUserID);
                gc1.DataSource = dv;

            }
            else
            {
                gc1.DataSource = dt_ll;
            }
        }

        private void ui销售订单审核_Load(object sender, EventArgs e)
        {
            try
            {
                fun_load();
            }
            catch (Exception ex )
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gv1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }

        private void gvP_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {

                e.Info.DisplayText = (e.RowHandle + 1).ToString();

            }
        }
    }
}
